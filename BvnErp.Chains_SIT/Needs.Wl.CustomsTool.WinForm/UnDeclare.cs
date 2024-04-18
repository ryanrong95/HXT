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
    public partial class UnDeclare : UserControl
    {
        private int pageSize = 25; //每页显示行数
        private int pageCount1 = 0; //页数
        private int curPage1 = 1; //当前页
        private int pageCount2 = 0; //页数
        private int curPage2 = 1; //当前页

        public UnDeclare()
        {
            InitializeComponent();
            InitDeclareGrid();
            InitManifestGrid();
        }

        /// <summary>
        /// 报关单初始化
        /// </summary>
        public void InitDeclareGrid()
        {
            int page = curPage1;
            int rows = pageSize;
            var ContrNO = this.textBox1.Text;
            var DecHead = Tool.Current.Customs.DecHeads.AsQueryable();

            //过滤：只有已制单的才显示
            DecHead = DecHead.Where(t => t.CusDecStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Make));

            if (!string.IsNullOrEmpty(ContrNO))
            {
                ContrNO = ContrNO.Trim();
                DecHead = DecHead.Where(t => t.ContrNo.Contains(ContrNO));
            }

            var total = DecHead.Count();
            DecHead = DecHead.OrderByDescending(item => item.CreateTime).Skip(rows * (page - 1)).Take(rows); //分页
            this.laltotal1.Text = total.ToString(); //总条数
            var pages = (total / pageSize) == 0 ? 1 : (total / pageSize);
            this.pagecount1.Text = pages.ToString(); //页数
            pageCount1 = pages;

            dataGridView1.DataSource = DecHead.Select(item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                ContrNo = item.ContrNo,
                BillNo = item.BillNo,
                EntryId = item.EntryId,
                PreEntryId = item.PreEntryId,
                AgentName = item.AgentName,
                IsInspection = item.IsInspection == true ? "是" : "否",
                CreateDate = item.CreateTime.ToShortDateString(),
                item.Inputer.RealName,
                StatusName = item.StatusName,
            }).ToArray();

            dataGridView1.AutoResizeColumns();
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].HeaderText = "订单号";
            dataGridView1.Columns[3].HeaderText = "合同号";
            dataGridView1.Columns[4].HeaderText = "运单号";
            dataGridView1.Columns[5].HeaderText = "海关单号";
            dataGridView1.Columns[6].HeaderText = "预录入号";
            dataGridView1.Columns[7].HeaderText = "代理企业";
            dataGridView1.Columns[8].HeaderText = "是否商检";
            dataGridView1.Columns[9].HeaderText = "制单员";
            dataGridView1.Columns[10].HeaderText = "录单时间";
            dataGridView1.Columns[11].HeaderText = "状态";
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            InitDeclareGrid();
        }

        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var decList = new List<string>();
            int i;
            for (i = 0; i < this.dataGridView1.RowCount; i++)
            {
                if (this.dataGridView1.Rows[i].Cells[0].Value != null && bool.Parse(this.dataGridView1.Rows[i].Cells[0].Value.ToString()))
                {
                    decList.Add(this.dataGridView1.Rows[i].Cells[1].Value.ToString());
                }
            }

            if (decList.Count < 1)
            {
                MessageBox.Show("请先勾选需要申报的报关单！", "提示", MessageBoxButtons.OK);
            }
            else
            {
                DialogResult dr = MessageBox.Show("确定申报？", "提示", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    BtnDeclare.Enabled = false;
                    try
                    {
                        //申报
                        foreach (string s in decList)
                        {
                            var head = Tool.Current.Customs.DecHeads.First(t => t.ID == s);
                            if (head == null)
                            {
                                MessageBox.Show("报关单状态异常，请刷新列表重试", "提示", MessageBoxButtons.OK);
                                break;
                            }
                            else
                            {
                                head.Declare();
                            }
                        }
                        InitDeclareGrid();
                        MessageBox.Show("导入成功，等待海关回执！", "提示", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                    }
                    BtnDeclare.Enabled = true;
                }
            }
        }

        bool CheckAll = false;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == -1)//如果单击列表头，全选．
            {
                dataGridView1.CurrentCell = null;
                int i;
                for (i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = !CheckAll;//如果为true则为选中,false未选中
                }
                CheckAll = !CheckAll;
            }
        }

        /// <summary>
        /// 舱单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            InitManifestGrid();
        }

        /// <summary>
        /// 舱单表格数据绑定
        /// </summary>
        public void InitManifestGrid()
        {
            var BillNo = this.tbBillNo.Text;
            var VoyageNo = this.tbVoyageNo.Text;
            int page = curPage1;
            int rows = pageSize;
            var ManifestBill = Tool.Current.Customs.Manifests.AsQueryable();

            //过滤：只有已制单的才显示
            ManifestBill = ManifestBill.Where(t => t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make)
                                                  || t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Deleting));
            if (!string.IsNullOrEmpty(BillNo))
            {
                BillNo = BillNo.Trim();
                ManifestBill = ManifestBill.Where(t => t.ID.Contains(BillNo));
            }
            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                ManifestBill = ManifestBill.Where(t => t.Manifest.ID.Contains(VoyageNo));
            }

            var total = ManifestBill.Count();
            //分页
            ManifestBill = ManifestBill.OrderByDescending(item => item.CreateDate).Skip(rows * (page - 1)).Take(rows);

            //分页数据绑定
            this.laltotal2.Text = total.ToString(); //总条数
            var pages = (total / pageSize) == 0 ? 1 : (total / pageSize);
            this.pagecount2.Text = pages.ToString(); //页数
            pageCount2 = pages;

            dataGridView2.DataSource = ManifestBill.Select(item => new
            {
                ID = item.ID,
                VoyageNo = item.Manifest.ID,
                BillNo = item.ID,
                PackNo = item.PackNum.ToString(),
                AgentName = item.ConsigneeName,
                CreateDate = item.CreateDate.ToShortDateString(),
                StatusName = item.StatusName,
            }).ToArray();

            dataGridView2.AutoResizeColumns();
            dataGridView2.Columns[1].Visible = false;
            dataGridView2.Columns[2].HeaderText = "批次号";
            dataGridView2.Columns[3].HeaderText = "运单号";
            dataGridView2.Columns[4].HeaderText = "件数";
            dataGridView2.Columns[5].HeaderText = "境内收货人";
            dataGridView2.Columns[6].HeaderText = "录单时间";
            dataGridView2.Columns[7].HeaderText = "状态";
        }

        /// <summary>
        /// 舱单申报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            var decList = new List<string>();
            int i;
            for (i = 0; i < this.dataGridView2.RowCount; i++)
            {
                if (this.dataGridView2.Rows[i].Cells[0].Value != null && bool.Parse(this.dataGridView2.Rows[i].Cells[0].Value.ToString()))
                {
                    decList.Add(this.dataGridView2.Rows[i].Cells[3].Value.ToString());
                }
            }

            if (decList.Count < 1)
            {
                MessageBox.Show("请先勾选需要申报的舱单！", "提示", MessageBoxButtons.OK);
            }
            else
            {
                DialogResult dr = MessageBox.Show("确定申报？", "提示", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    button2.Enabled = false;
                    try
                    {
                        //申报
                        foreach (string s in decList)
                        {
                            var head = Tool.Current.Customs.Manifests.First(t => t.ID == s);
                            if (head == null)
                            {
                                MessageBox.Show("运单号【"+s+"】状态异常，请刷新列表重试", "提示", MessageBoxButtons.OK);
                                break;
                            }
                            else
                            {
                                head.Apply();
                            }
                        }
                        InitManifestGrid();
                        MessageBox.Show("导入成功，等待海关回执！", "提示", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                    }
                    button2.Enabled = true;
                }
            }
        }

        bool CheckAll2 = false;
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == -1)//如果单击列表头，全选．
            {
                dataGridView2.CurrentCell = null;
                int i;
                for (i = 0; i < this.dataGridView2.RowCount; i++)
                {
                    this.dataGridView2.Rows[i].Cells[0].Value = !CheckAll2;//如果为true则为选中,false未选中
                }
                CheckAll2 = !CheckAll2;
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

        private void dataGridView1_CellMouseEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = BaseStyleSetting.DefaultCellStyle_BackColor;
            }
        }

        private void dataGridView1_CellMouseLeave_1(object sender, DataGridViewCellEventArgs e)
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
        private void toolStripButton1_Click_1(object sender, EventArgs e)
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
        private void toolStripButton2_Click_1(object sender, EventArgs e)
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
                this.index1.Text = curPage1.ToString();
                InitDeclareGrid();
            }
        }

        /// <summary>
        /// 上一页(舱单)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click_1(object sender, EventArgs e)
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
        private void toolStripButton4_Click_1(object sender, EventArgs e)
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

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == -1)//如果单击列表头，全选．
            {
                dataGridView1.CurrentCell = null;
                int i;
                for (i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = !CheckAll;//如果为true则为选中,false未选中
                }
                CheckAll = !CheckAll;
            }
        }
    }
}