using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Needs.Wl.CustomsTool.WinForm.Views;
using Needs.Wl.CustomsTool.WinForm.Services;

namespace Needs.Wl.CustomsTool.WinForm
{
    public partial class ExceptionInfo : UserControl
    {
        private int pageSize = 25; //每页显示行数
        private int pageCount1 = 0; //页数
        private int curPage1 = 1; //当前页
        private int pageCount2 = 0; //页数
        private int curPage2 = 1; //当前页

        ExceptionRemindView exRemindViewDeclare;
        ExceptionRemindView exRemindViewManifest;

        public ExceptionInfo()
        {
            string macAddress = MacService.GetMacAddress();
            exRemindViewDeclare = new ExceptionRemindView(macAddress, Ccs.Services.Enums.BalanceQueueBusinessType.DecHead);
            exRemindViewDeclare.LoadExceptionDBInfo();
            exRemindViewManifest = new ExceptionRemindView(macAddress, Ccs.Services.Enums.BalanceQueueBusinessType.Manifest);
            exRemindViewManifest.LoadExceptionDBInfo();

            InitializeComponent();
            //InitDeclareGrid();
            //InitManifestGrid();
        }

        /// <summary>
        /// 报关单
        /// </summary>
        public void InitDeclareGrid()
        {
            int page = curPage1;
            int rows = pageSize;

            int total = 0;
            var decHeadRemind = exRemindViewDeclare.GetInfos(out total, page, rows);
            this.laltotal1.Text = total.ToString(); //总条数
            var pages = (total / pageSize) == 0 ? 1 : (total / pageSize);
            this.pagecount3.Text = pages.ToString(); //页数
            pageCount1 = pages;

            var result = decHeadRemind.ToList().Select(item => new {
                BusinessID = item.BusinessID.TrimStart(new char[] { 'X', 'D', 'T' }).TrimStart(new char[] { 'H', 'Y' }),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Brief,
            }).ToArray();

            this.dataGridView1.DataSource = result; //绑定数据源
            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.dataGridView1.Columns[0].HeaderText = "报关单号";
            this.dataGridView1.Columns[1].HeaderText = "生成时间";
            this.dataGridView1.Columns[2].HeaderText = "内容";

            this.dataGridView1.Columns[0].FillWeight = 28;
            this.dataGridView1.Columns[1].FillWeight = 25;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            FormExceptionDetail formExceptionDetail = new FormExceptionDetail(dataGridView1.CurrentRow);
            formExceptionDetail.ShowDialog();
        }

        /// <summary>
        /// 舱单
        /// </summary>
        public void InitManifestGrid()
        {
            int page = curPage2;
            int rows = pageSize;

            int total = 0;
            var manifestRemind = exRemindViewManifest.GetInfos(out total, page, rows);
            this.laltotal2.Text = total.ToString(); //总条数
            var pages = (total / pageSize) == 0 ? 1 : (total / pageSize);
            this.pagecount4.Text = pages.ToString(); //页数
            pageCount2 = pages;

            var result = manifestRemind.ToList().Select(item => new {
                BusinessID = item.BusinessID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Brief,
            }).ToArray();

            this.dataGridView2.DataSource = result; //绑定数据源
            this.dataGridView2.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.dataGridView2.Columns[0].HeaderText = "运单号";
            this.dataGridView2.Columns[1].HeaderText = "生成时间";
            this.dataGridView2.Columns[2].HeaderText = "内容";

            this.dataGridView2.Columns[0].FillWeight = 20;
            this.dataGridView2.Columns[1].FillWeight = 25;
        }

        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            FormExceptionDetail formExceptionDetail = new FormExceptionDetail(dataGridView2.CurrentRow);
            formExceptionDetail.ShowDialog();
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = BaseStyleSetting.DefaultCellStyle_BackColor;
            }
        }

        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = BaseStyleSetting.DefaultCellStyle_BackColor;
            }
        }

        private void dataGridView2_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 上一页(报关单)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            curPage1--;
            if (curPage1 <= 0)
            {
                curPage1++;
                MessageBox.Show("已经是第一页，请点击“下一页”查看！");
                return;
            }
            else
            {
                this.index1.Text = curPage1.ToString();
                InitDeclareGrid();
            }
        }

        /// <summary>
        /// 下一页(报关单)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            curPage1++;
            if (curPage1 > pageCount1)
            {
                curPage1--;
                MessageBox.Show("已经是最后一页，请点击“上一页”查看！");
                return;
            }
            else
            {
                this.index1.Text = curPage1.ToString();
                InitDeclareGrid();
            }
        }

        /// <summary>
        /// 上一页(舱单)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            curPage2--;
            if (curPage2 <= 0)
            {
                curPage2++;
                MessageBox.Show("已经是第一页，请点击“下一页”查看！");
                return;
            }
            else
            {
                this.index2.Text = curPage2.ToString();
                InitManifestGrid();
            }
        }

        /// <summary>
        /// 下一页(舱单)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            curPage2++;
            if (curPage2 > pageCount2)
            {
                curPage2--;
                MessageBox.Show("已经是最后一页，请点击“上一页”查看！");
                return;
            }
            else
            {
                this.index2.Text = curPage2.ToString();
                InitManifestGrid();
            }
        }
    }
}
