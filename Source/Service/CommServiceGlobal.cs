using System;
using System.IO;
using System.Threading;
using Nancy.Hosting.Self;
using SG.GeneralLib;

namespace CommService
{
    // Global config object, the config is decoded from the application config file
    public class Config
    {
        public bool   debugLogging   { get; set; }
        public string debugLogFile   { get; set; }
        public bool   webServerOn    { get; set; }
        public int    webServerPort  { get; set; }
        public int    webPageRefresh { get; set; }
        public bool   ftpServerOn    { get; set; }
        public int    ftpServerPort  { get; set; }
        public int    ftpServerBuf   { get; set; }
        public string ftpLoggingFile { get; set; }
        public bool   ftpSrvVerbose  { get; set; }
        public bool   ftpSrvAnyAddr  { get; set; }
        public string ftpSrvUserName { get; set; }
        public string ftpSrvPassword { get; set; }
        public bool   tcpipServerOn  { get; set; }
        public string tcpipSvrProto  { get; set; }
        public int    tcpipSvrPort   { get; set; }
        public bool   sharedFolderOn { get; set; }
        public string sharedFolder   { get; set; }
        public bool   processSubDirs { get; set; }
        public bool   timerFuncOn    { get; set; }
        public int    timerInterval  { get; set; }
    }

    // Global object that stores general windows service information
    public class Values
    {
        public string moduleName { get; set; }
        public string rootFolder { get; set; }
        public string wsVersion  { get; set; }
        public string configJson { get; set; }
        public string lastError  { get; set; }
    }

    public class Service
    {
        public Config cfg = null;
        public Values val = null;
    }

    static class Global
    {
        // Decryption key for the secure information stored in the service config
        public const string SecurityKey = "SlaterGordon";

        // Global data object, config and values, config decoded from application config file
        public static Service svc = new Service();

        // Internal service objects and variables, webserver, worker threads and timer
        public static NancyHost    webServer   = null;
        public static FileStream   ftpSessFile = null;
        public static StreamWriter ftpSession  = null;
        public static Server       ftpServer   = null;
        public static bool         tcpipActive = false;
        public static Thread       tcpipThread = null;
        public static FolderWatch  shareFolder = null;
        public static System.Timers.Timer timerFunc = null;

        static Global()
        {
            svc.cfg = new Config();
            svc.val = new Values();

            // General config settings for the windows service
            svc.cfg.With(c =>
            {
                c.debugLogging   = true;
                c.debugLogFile   = Utility.pLogFile; // Default, same path as the service
                c.webServerOn    = false;
                c.webServerPort  = 0;
                c.webPageRefresh = 0;
                c.ftpServerOn    = false;
                c.ftpServerPort  = 0;
                c.ftpServerBuf   = 0;
                c.ftpLoggingFile = string.Empty;
                c.ftpSrvVerbose  = false;
                c.ftpSrvAnyAddr  = false;
                c.ftpSrvUserName = string.Empty;
                c.ftpSrvPassword = string.Empty;
                c.tcpipServerOn  = false;
                c.tcpipSvrProto  = string.Empty;
                c.tcpipSvrPort   = 0;
                c.sharedFolderOn = false;
                c.processSubDirs = false;
                c.sharedFolder   = string.Empty;
                c.timerFuncOn    = false;
                c.timerInterval  = 0;
            });

            // Website displayed values for windows service
            svc.val.With(v =>
            {
                v.moduleName = Utility.pAssembly;
                v.rootFolder = Utility.pAppStartPath;
                v.wsVersion  = Utility.pAppName + " - &copy; SGS " + DateTime.Now.Year.ToString();
                v.configJson = string.Empty;
                v.lastError  = "None.";
            });
        }

        public static void WriteEventLog(string msg)
        {
            // NOTE: Only used to write exception errors to the windows application event log
            Utility.WinEventLogError(msg);
        }

        public static void WriteLogFile(string msg)
        {
            if (svc.cfg.debugLogging)
            {
                Utility.WriteToLog(svc.cfg.debugLogFile, msg);
            }
        }
    }
}
