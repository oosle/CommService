using System;
using System.IO;
using System.Runtime.InteropServices;
using SG.GeneralLib;

namespace CommService
{
    public class FolderWatch
    {
        private FileWatcher folderWatch = new FileWatcher();
        private string folderToWatch = string.Empty;
        private bool processSubDirs = false;

        #region Process a file here

        private bool ProcessFile(string file)
        {
            bool result = false;

            try
            {
                // Anything you want here!
                result = true;
            }
            catch
            {
                result = false;
            }

            return (result);
        }

        #endregion

        #region File watching interface event, uses OnChanged to check file downloaded, FTP/SSH latency

        // Check file has completed being downloaded before processing, FTP/SSH latency issue
        private const int ERROR_SHARING_VIOLATION = 32;
        private const int ERROR_LOCK_VIOLATION = 33;
        private bool IsFileLocked(string file)
        {
            // Check that problem is not in destination file
            if (File.Exists(file))
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex)
                {
                    int errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);
                    if ((ex is IOException) &&
                        (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return (true);
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return (false);
        }

        private void FolderWatch_Changed(object sender, FileSystemEventArgs e)
        {
            string file = (e).FullPath;

            if (IsFileLocked(file)) { return; }

            Global.WriteLogFile(
                string.Format("[File]: File=[{0}]", file));

            if (ProcessFile(file))
                Global.WriteLogFile("[File]: File OK.");
            else
                Global.WriteLogFile("[File]: File FAIL.");
        }

        #endregion

        #region This is the file watching interface control methods

        public bool folderStarted { get; set; }

        public void Dispose()
        {
            if (folderWatch != null) folderWatch.Dispose();
        }

        public FolderWatch(string folder, bool subdir)
        {
            this.folderWatch = new FileWatcher();
            this.folderToWatch = folder;
            this.processSubDirs = subdir;
        }

        public void Start()
        {
            try
            {
                if (folderStarted) { return; }

                // Make sure the shared folder exists ready for processing
                if (!this.folderToWatch.IsNullOrEmpty() && !Directory.Exists(this.folderToWatch))
                {
                    Directory.CreateDirectory(this.folderToWatch);
                }

                folderWatch.OrderByOldestFirst = true;
                folderWatch.Path = this.folderToWatch;
                folderWatch.IncludeSubdirectories = this.processSubDirs;
                folderWatch.NotifyFilter = NotifyFilters.LastWrite;
                folderWatch.Changed += FolderWatch_Changed;
                folderWatch.EnableRaisingEvents = true;

                folderStarted = true;
            }
            catch (Exception ex)
            {
                string error = string.Format("[File]: Folder=[{0}]; Exception: {1}",
                    this.folderToWatch, ex.Message);

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }
        }

        public void Stop()
        {
            try
            {
                if (!folderStarted) { return; }

                folderWatch.EnableRaisingEvents = false;
                folderWatch.OrderByOldestFirst = true;
                folderWatch.Path = this.folderToWatch;
                folderWatch.IncludeSubdirectories = false;
                folderWatch.NotifyFilter = NotifyFilters.LastWrite;
                folderWatch.Changed -= FolderWatch_Changed;

                folderStarted = false;
            }
            catch (Exception ex)
            {
                string error = string.Format("[File]: Folder=[{0}]; Exception: {1}",
                    this.folderToWatch, ex.Message);

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }
        }

        #endregion
    }
}
