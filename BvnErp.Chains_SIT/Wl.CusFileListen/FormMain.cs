using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.CusFileListen
{
    public partial class FormMain : Form
    {
        FormWatcher formWatcher = new FormWatcher();
        public FormMain()
        {
            InitializeComponent();


            //TODO:
            //StartPosition = FormStartPosition.CenterScreen;
            //Width = 1000;
            //Height = 600;
            //// 背景
            //formWatcher.MdiParent = this;
            //formWatcher.Show();
            //formWatcher.Size = new Size(Convert.ToInt32(Width*0.2), Height-50);

            FormManifest frmMft = new FormManifest();
            frmMft.MdiParent = this;
            frmMft.Show();

            FormDeclare frmDec = new FormDeclare();
            frmDec.MdiParent = this;
            frmDec.Show();


            //监听程序保持开启
            DeclareResponse declareResponse = new DeclareResponse();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            formWatcher.Height = ClientSize.Height-50;
            formWatcher.Width = Convert.ToInt32(ClientSize.Width * 0.2);
        }


        private void 报关单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = HaveOpened(this, "报关单");
            if (index == -1)
            {
                FormDeclare frmDec = new FormDeclare();
                frmDec.MdiParent = this;
                frmDec.Show();
            }
            else
            {
                this.MdiChildren[index].Show();
            }
        }

        private void 舱单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = HaveOpened(this, "舱单");
            if (index == -1)
            {
                FormManifest frmMft = new FormManifest();
                frmMft.MdiParent = this;
                frmMft.Show();
            }
            else
            {
                this.MdiChildren[index].Show();
            }
        }

        /// <summary>
        /// 查看MDI子窗体是否已经被打开
        /// </summary>
        public static int HaveOpened(Form frmMdiFather, string strMdiChild)
        {
            int bReturn = -1;
            for (int i = 0; i < frmMdiFather.MdiChildren.Length; i++)
            {
                if (frmMdiFather.MdiChildren[i].Text == strMdiChild)
                {
                    frmMdiFather.MdiChildren[i].BringToFront();
                    bReturn = i;
                    break;
                }
            }
            return bReturn;
        }
    }
}
