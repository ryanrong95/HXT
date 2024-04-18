using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Needs.Wl.CustomsTool.WinForm
{
    public partial class FormExceptionDetail : Form
    {
        private DataGridViewRow dgvr;

        public FormExceptionDetail(DataGridViewRow dgvr)
        {
            InitializeComponent();
            this.dgvr = dgvr;
        }

        private void FormExceptionDetail_Load(object sender, EventArgs e)
        {
            label报关单号.Text = "报关单号：" + dgvr.Cells[0].Value.ToString();
            label生成时间.Text = "生成时间：" + dgvr.Cells[1].Value.ToString();
            txt异常内容.Text = dgvr.Cells[2].Value.ToString();
        }
    }
}
