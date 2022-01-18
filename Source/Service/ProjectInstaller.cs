using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace CommService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public string GetContextParameter(string key)
        {
            string sValue = "";
            try
            {
                sValue = this.Context.Parameters[key].ToString();
            }
            catch
            {
                sValue = "";
            }

            return (sValue);
        }

        // Override 'OnBeforeInstall', force installation script to set credentials, otherwise default
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            string user = GetContextParameter("user").Trim();
            string pass = GetContextParameter("password").Trim();

            if (user != "" && pass != "" && user != "." && pass != ".")
            {
                this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.User;
                this.serviceProcessInstaller.Username = user;
                this.serviceProcessInstaller.Password = pass;
            }
            else
            {
                this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
                this.serviceProcessInstaller.Username = null;
                this.serviceProcessInstaller.Password = null;
            }
        }
    }
}
