using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System.Timers;
using Nancy.Hosting.Self;
using SG.GeneralLib;

namespace CommService
{
    static class Program
    {
        static void Main()
        {
            var appSettings = ConfigurationManager.AppSettings;

            if (appSettings.Count > 0 &&
                appSettings["DebugLogging"] != null &&
                appSettings["DebugLoggingFile"] != null &&
                appSettings["WebServerOn"] != null &&
                appSettings["WebServerPort"] != null &&
                appSettings["WebPageRefresh"] != null &&
                appSettings["FtpServerOn"] != null &&
                appSettings["FtpServerPort"] != null &&
                appSettings["FtpServerBuffer"] != null &&
                appSettings["FtpLoggingFile"] != null &&
                appSettings["FtpServerVerbose"] != null &&
                appSettings["FtpServerAnyAddr"] != null &&
                appSettings["FtpServerUserName"] != null &&
                appSettings["FtpServerPassword"] != null &&
                appSettings["TcpipServerOn"] != null &&
                appSettings["TcpipServerProto"] != null &&
                appSettings["TcpipServerPort"] != null &&
                appSettings["SharedFolderOn"] != null &&
                appSettings["SharedFolder"] != null &&
                appSettings["ProcessSubDirs"] != null &&
                appSettings["TimerFunctionOn"] != null &&
                appSettings["TimerFuncInterval"] != null)
            {
                try
                {
                    // For windows service paths must be absolute not relative, hence Path.Combine()
                    Global.svc.cfg.With(c =>
                    {
                        c.debugLogging   = appSettings["DebugLogging"].ToString().ToBool();
                        c.debugLogFile   = Path.Combine(Utility.pAppStartPath, appSettings["DebugLoggingFile"].ToString());
                        c.webServerOn    = appSettings["WebServerOn"].ToString().ToBool();
                        c.webServerPort  = appSettings["WebServerPort"].ToString().ToInt();
                        c.webPageRefresh = appSettings["WebPageRefresh"].ToString().ToInt();
                        c.ftpServerOn    = appSettings["FtpServerOn"].ToString().ToBool();
                        c.ftpServerPort  = appSettings["FtpServerPort"].ToString().ToInt();
                        c.ftpServerBuf   = appSettings["FtpServerBuffer"].ToString().ToInt();
                        c.ftpLoggingFile = Path.Combine(Utility.pAppStartPath, appSettings["FtpLoggingFile"].ToString());
                        c.ftpSrvVerbose  = appSettings["FtpServerVerbose"].ToString().ToBool();
                        c.ftpSrvAnyAddr  = appSettings["FtpServerAnyAddr"].ToString().ToBool();
                        c.ftpSrvUserName = appSettings["FtpServerUserName"].ToString();
                        c.ftpSrvPassword = appSettings["FtpServerPassword"].ToString();
                        c.tcpipServerOn  = appSettings["TcpipServerOn"].ToString().ToBool();
                        c.tcpipSvrProto  = appSettings["TcpipServerProto"].ToString().ToLower();
                        c.tcpipSvrPort   = appSettings["TcpipServerPort"].ToString().ToInt();
                        c.sharedFolderOn = appSettings["SharedFolderOn"].ToString().ToBool();
                        c.sharedFolder   = Path.Combine(Utility.pAppStartPath, appSettings["SharedFolder"].ToString());
                        c.processSubDirs = appSettings["ProcessSubDirs"].ToString().ToBool();
                        c.timerFuncOn    = appSettings["TimerFunctionOn"].ToString().ToBool();
                        c.timerInterval  = appSettings["TimerFuncInterval"].ToString().ToInt();
                    });

                    // Create the webserver if configured, used to monitor stuff at the moment
                    if (Global.svc.cfg.webServerOn)
                    {
                        HostConfiguration hostConfig = new HostConfiguration();
                        hostConfig.UrlReservations.CreateAutomatically = true;

                        Global.webServer = new NancyHost(
                            new Uri(string.Format("http://localhost:{0}", Global.svc.cfg.webServerPort)),
                            new BootStrapper(), hostConfig);
                    }

                    // Create the embedded FTP server, basic config with session logging
                    // NOTE: If trying to access FTP server from Linux use passive mode: ftp -p <address> <port>
                    if (Global.svc.cfg.ftpServerOn)
                    {
                        Global.ftpServer = new Server();
                        Global.ftpServer.With(ftp => Global.svc.cfg.With(cfg =>
                        {
                            {
                                // Ensure path for the FTP log file exists before writing to the log file
                                string path = Path.GetDirectoryName(cfg.ftpLoggingFile);
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);

                                ftp.LocalPort = cfg.ftpServerPort;
                                ftp.BufferSize = cfg.ftpServerBuf;
                                Global.ftpSessFile = new FileStream(
                                    cfg.ftpLoggingFile, FileMode.Append, FileAccess.Write);
                                Global.ftpSession = new StreamWriter(Global.ftpSessFile);
                                ftp.LogHandler = new LogHandler(Global.ftpSession, cfg.ftpSrvVerbose);
                                ftp.AuthHandler = new AuthHandler(cfg.ftpSrvAnyAddr,
                                    cfg.ftpSrvUserName.DecryptAES(Global.SecurityKey),
                                    cfg.ftpSrvPassword.DecryptAES(Global.SecurityKey));
                            }
                        }));
                    }

                    // Create the embedded TCP/IP server, basic config
                    if (Global.svc.cfg.tcpipServerOn)
                    {
                        Global.tcpipThread = new Thread(
                            new ThreadStart(CommServiceTCPIP.TCPIPThreadFunction));
                    }

                    // Create the shared folder processing objects, basic config
                    if (Global.svc.cfg.sharedFolderOn)
                    {
                        Global.shareFolder = new FolderWatch(
                            Global.svc.cfg.sharedFolder, Global.svc.cfg.processSubDirs);
                    }

                    // Create the timer function, a polling task, basic config
                    if (Global.svc.cfg.timerFuncOn)
                    {
                        Global.timerFunc = new System.Timers.Timer();
                        Global.timerFunc.With(tim =>
                        {
                            tim.Enabled = false;
                            tim.Interval = Global.svc.cfg.timerInterval;
                            tim.Elapsed += new ElapsedEventHandler(CommServiceTimer.TimerFunction);
                        });
                    }

                    // Register windows service, run in debug if in Visual Studio, otherwise service
                    ServiceBase[] servicesToRun;
                    servicesToRun = new ServiceBase[]
                    {
                        new CommService()
                    };

                    if (Environment.UserInteractive)
                    {
                        Utility.RunInteractive(servicesToRun);
                    }
                    else
                    {
                        ServiceBase.Run(servicesToRun);
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("[System]: Exception: {0}", ex.Message);

                    Global.WriteEventLog(error);
                    Global.WriteLogFile(error);
                }
            }
            else
            {
                string error = "[System]: CommService service start failed, please check config.";

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }
        }
    }
}
