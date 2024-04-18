namespace Needs.Wl.PlanningService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ServiceEznet = new System.ServiceProcess.ServiceProcessInstaller();
            this.EznetService = new System.ServiceProcess.ServiceInstaller();
            // 
            // ServiceEznet
            // 
            this.ServiceEznet.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ServiceEznet.Password = null;
            this.ServiceEznet.Username = null;
            // 
            // EznetService
            // 
            this.EznetService.Description = "执行主动获取Icgoo、快报电子、大赢家预归类产品及代理报关订单数据";
            this.EznetService.ServiceName = "ServiceEznet";
            this.EznetService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.EznetService.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.创新恒远接口计划任务服务_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ServiceEznet,
            this.EznetService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ServiceEznet;
        private System.ServiceProcess.ServiceInstaller EznetService;
    }
}