using System;
using System.ServiceProcess;
using SG.GeneralLib;

namespace CommService
{
    public partial class CommService : ServiceBase
    {
        public CommService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (Global.svc.cfg.webServerOn)
                {
                    Global.webServer.Start();
                    Global.WriteLogFile(
                        string.Format("[System]: Webserver started, URL = [http://localhost:{0}]",
                        Global.svc.cfg.webServerPort));
                }

                if (Global.svc.cfg.ftpServerOn)
                {
                    Global.ftpServer.Start();
                    Global.WriteLogFile(
                        string.Format("[System]: FTP server started, IP = [localhost:{0}]",
                        Global.svc.cfg.ftpServerPort));
                }

                if (Global.svc.cfg.tcpipServerOn)
                {
                    Global.tcpipActive = true;
                    Global.tcpipThread.IsBackground = true;
                    Global.tcpipThread.Start();
                    Global.WriteLogFile(
                        string.Format("[System]: TCP/IP server started, IP = [{0}:localhost:{1}]",
                        Global.svc.cfg.tcpipSvrProto, Global.svc.cfg.tcpipSvrPort));
                }

                if (Global.svc.cfg.sharedFolderOn)
                {
                    Global.shareFolder.Start();
                    Global.WriteLogFile(
                        string.Format("[System]: Shared folder started, folder = [{0}]",
                        Global.svc.cfg.sharedFolder));
                }

                if (Global.svc.cfg.timerFuncOn)
                {
                    Global.timerFunc.Enabled = true;
                    Global.WriteLogFile(
                        string.Format("[System]: Timer function started, interval = [{0}ms]",
                        Global.svc.cfg.timerInterval));
                }

                Global.WriteLogFile("[System]: CommService service started.");
            }
            catch (Exception ex)
            {
                string error = String.Format("[System]: OnStart exception: {0}", ex.Message);

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            try
            {
                if (Global.svc.cfg.webServerOn)
                {
                    Global.webServer.Stop();
                    Global.WriteLogFile("[System]: Webserver stopped.");
                }

                if (Global.svc.cfg.ftpServerOn)
                {
                    Global.ftpServer.Stop();
                    Global.WriteLogFile("[System]: FTP server thread stopped.");
                }

                if (Global.svc.cfg.tcpipServerOn)
                {
                    Global.tcpipActive = false;
                    Global.WriteLogFile("[System]: TCP/IP server thread stopped.");
                }

                if (Global.svc.cfg.sharedFolderOn)
                {
                    Global.shareFolder.Stop();
                    Global.WriteLogFile("[System]: Shared folder monitoring stopped.");
                }

                if (Global.svc.cfg.timerFuncOn)
                {
                    Global.timerFunc.Enabled = false;
                    Global.WriteLogFile("[System]: Timer function stopped.");
                }

                Global.WriteLogFile("[System]: CommService service stopped.");
            }
            catch (Exception ex)
            {
                string error = String.Format("[System]: OnStop exception: {0}", ex.Message);

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }

            base.OnStop();
        }
    }
}
