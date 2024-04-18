using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yahv.PsWms.DappForm.Controls
{
    public partial class UcStatusBar : UserControl, Services.ISimHelper
    {
        public UcStatusBar()
        {
            ////线程异常
            //throw new Exception("4");

            InitializeComponent();
        }

        /// <summary>
        /// 打印状态
        /// </summary>
        public string PrintStatus
        {
            get
            {
                return this.txtPrintStatus.Text;
            }
            set
            {
                this.txtPrintStatus.Text = value;
            }
        }

        /// <summary>
        /// 上载与下载状态
        /// </summary>
        public string TransferStatus
        {
            get
            {
                return this.txtTransferStatus.Text;
            }
            set
            {
                this.txtTransferStatus.Text = value;
            }
        }

        private void UcStatusBar_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }
    }
}
