namespace CommService
{
    partial class ProjectInstaller
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();

            this.serviceInstaller.DelayedAutoStart = true;
            this.serviceInstaller.Description = "CommService communication service with TCPIP/FTP/File/Web/REST interfaces.";
            this.serviceInstaller.DisplayName = "CommService Communication Service";
            this.serviceInstaller.ServiceName = "CommService";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.Add(this.serviceInstaller);
            this.Installers.Add(this.serviceProcessInstaller);
        }

        #endregion
    }
}
