using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Needs.Wl.CustomsTool.WinForm
{
    public partial class FormLoading : Form
    {
        public FormLoading()
        {
            InitializeComponent();
            var name = Tool.Current.Company.Name;
            this.Text = name + "-报关辅助工具";
        }
        private void Init()
        {
            progressBar1.Minimum = 0;//设置ProgressBar组件最小值为0
            progressBar1.Maximum = 10;//Maximum最大值为10
            progressBar1.MarqueeAnimationSpeed = 50;//设定进度快在进度栏中移动的时间段
            CheckFolders();//文件夹配置检查
            autorun();  //开机自启检查
            LoadingSuccess();//配置成功
        }

        /// <summary>
        /// 配置成功
        /// </summary>
        private void LoadingSuccess()
        {
            this.Hide();//隐藏本窗体
            FormMain mainForm = new FormMain();
            mainForm.Show();//显示窗体
        }

        /// <summary>
        /// 检查创建文件夹
        /// </summary>
        private void CheckFolders()
        {
            var arrBegin = new string[2];
            arrBegin[0] = Tool.Current.Folder.DecMainFolder;  //报关单
            arrBegin[1] = Tool.Current.Folder.RmftMainFolder;  //舱单
            var arrLast = new string[9];  //子文件夹名称
            arrLast[0] = ConstConfig.InBox;
            arrLast[1] = ConstConfig.OutBox;
            arrLast[2] = ConstConfig.FailBox;
            arrLast[3] = ConstConfig.SentBox;
            arrLast[4] = ConstConfig.Message;
            arrLast[5] = ConstConfig.WaitReceipt;
            arrLast[6] = ConstConfig.WaitFail;
            arrLast[7] = ConstConfig.InBox_BK;
            arrLast[8] = ConstConfig.FailBox_BK;
            for (int i=0;i< arrBegin.Length;i++)
            {
                var exPath = arrBegin[i];
                for(int j=0;j< arrLast.Length; j++)
                {
                    var path = Path.Combine(exPath, arrLast[j]);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);//创建一个路径的文件夹
                    }
                }
            }
            var pathEdoc = Path.Combine(arrBegin[0], ConstConfig.Edoc);
            if (!Directory.Exists(pathEdoc))
            {
                Directory.CreateDirectory(pathEdoc);//创建一个路径的文件夹
            }
        }

        /// <summary>
        /// 开机自启
        /// </summary>
        private void autorun()
        {
            //获取程序路径
            string execPath = Application.ExecutablePath;
            bool isexc = false;
            try
            {
                RegistryKey RKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                //设置自启的程序叫获取目录下的程序名字
                string[] ar = RKey.GetValueNames();
                if (ar.Contains("Needs.Wl.CustomsTool.WinForm"))
                {
                    isexc = true;
                }
                if (!isexc)
                {
                    //设置自启的程序叫test
                    RKey.SetValue("Needs.Wl.CustomsTool.WinForm", execPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void FormLoading_Shown(object sender, EventArgs e)
        {
            Init();
        }
    }
}
