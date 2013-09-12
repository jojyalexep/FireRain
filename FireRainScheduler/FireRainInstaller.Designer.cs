using System.ServiceProcess;
namespace FireRainScheduler
{
    partial class FireRainInstaller
    {
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
            this.FireRainServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.FireRainProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // FireRainServiceInstaller
            // 
            this.FireRainServiceInstaller.Description = "FireRain Service Scheduler";
            this.FireRainServiceInstaller.DisplayName = "FireRain Service";
            this.FireRainServiceInstaller.ServiceName = "FRService";
            // 
            // FireRainProcessInstaller
            // 
            this.FireRainProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FireRainProcessInstaller.Password = null;
            this.FireRainProcessInstaller.Username = null;
            // 
            // FireRainInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FireRainProcessInstaller,
            this.FireRainServiceInstaller});

        }

        #endregion

        private ServiceInstaller FireRainServiceInstaller;
        private ServiceProcessInstaller FireRainProcessInstaller;
    }
}