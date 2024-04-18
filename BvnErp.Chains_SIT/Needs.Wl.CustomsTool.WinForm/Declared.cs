using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Needs.Wl.CustomsTool.WinForm.Services;

namespace Needs.Wl.CustomsTool.WinForm
{
    public partial class Declared : UserControl
    {
        private int pageSize = 25; //每页显示行数
        private int pageCount1 = 0; //页数
        private int curPage1 = 1; //当前页
        private int pageCount2 = 0; //页数
        private int curPage2 = 1; //当前页

        public Declared()
        {
            InitializeComponent();
            InitDeclareGrid();
            InitManifestGrid();
        }

        /// <summary>
        /// 报关单
        /// </summary>
        private void InitDeclareGrid()
        {
            int page = curPage1;
            int rows = pageSize;
            var DecHead = Tool.Current.Customs.DecHeads.AsQueryable(); 
           

            //过滤：非草稿，已制单，取消
            DecHead = DecHead.Where(t => t.CusDecStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Draft) &&
                t.CusDecStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Make)&&
                t.CusDecStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Cancel)
                );
            var total = DecHead.Count();
            DecHead = DecHead.OrderByDescending(item => item.CreateTime).Skip(rows * (page - 1)).Take(rows); //分页
            this.laltotal1.Text = total.ToString(); //总条数
            var pages = (total / pageSize) == 0 ? 1 : (total / pageSize);
            this.pagecount1.Text = pages.ToString(); //页数
            pageCount1 = pages;

            var result= DecHead.ToList().Select(item => new {
                item.ID,
                item.OrderID,
                item.ContrNo,
                item.VoyNo,
                item.BillNo,
                item.StatusName,
                CreateDate = item.CreateTime.ToShortDateString()
            }).ToArray();

            dataGridView1.DataSource = result; //绑定数据源

            dataGridView1.AutoResizeColumns();
            dataGridView1.Columns[0].HeaderText = "报关单号";
            dataGridView1.Columns[1].HeaderText = "订单号";
            dataGridView1.Columns[2].HeaderText = "合同号";
            dataGridView1.Columns[3].HeaderText = "运输批次号";
            dataGridView1.Columns[4].HeaderText = "提运单号";
            dataGridView1.Columns[5].HeaderText = "状态";
            dataGridView1.Columns[6].HeaderText = "录单时间";
        }

        /// <summary>
        /// 舱单
        /// </summary>
        private void InitManifestGrid()
        {
            int page = curPage1;
            int rows = pageSize;
            var ManifestBill = Tool.Current.Customs.Manifests.AsQueryable();
            //过滤：只有已制单的才显示
            ManifestBill = ManifestBill.Where(t => t.CusMftStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make)
                                                  && t.CusMftStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Draft)

                                                  && t.CusMftStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Cancel));
            var total = ManifestBill.Count();
            //分页
            ManifestBill = ManifestBill.OrderByDescending(item => item.CreateDate).Skip(rows * (page - 1)).Take(rows);

            //分页数据绑定
            this.laltotal2.Text = total.ToString(); //总条数
            var pages = (total / pageSize) == 0 ? 1 : (total / pageSize);
            this.pagecount2.Text = pages.ToString(); //页数
            pageCount2= pages;

            dataGridView2.DataSource = ManifestBill.Select(item=>new {
                VoyageNo = item.Manifest.ID,
                BillNo = item.ID,
                PackNo = item.PackNum,
                AgentName = item.ConsigneeName,
                CreateDate = item.CreateDate.ToShortDateString(),
                StatusName = item.StatusName,
            }).ToArray();

            dataGridView2.AutoResizeColumns();
            dataGridView2.Columns[0].HeaderText = "批次号";
            dataGridView2.Columns[1].HeaderText = "运单号";
            dataGridView2.Columns[2].HeaderText = "件数";
            dataGridView2.Columns[3].HeaderText = "境内收货人";
            dataGridView2.Columns[4].HeaderText = "录单时间";
            dataGridView2.Columns[5].HeaderText = "状态";
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = BaseStyleSetting.DefaultCellStyle_BackColor;
            }
        }

        private void dataGridView2_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }
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
