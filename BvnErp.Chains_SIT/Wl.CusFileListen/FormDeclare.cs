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
    public partial class FormDeclare : Form
    {
        public FormDeclare()
        {
            InitializeComponent();
            InitGrid();
        }

        public void InitGrid()
        {
            var ContrNO = this.tbContractNo.Text;
            var DecHead = new Needs.Ccs.Services.Views.DecHeadsListView().AsQueryable();

            //过滤：只有已制单的才显示
            DecHead = DecHead.Where(t => t.Status == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Make));

            if (!string.IsNullOrEmpty(ContrNO))
            {
                ContrNO = ContrNO.Trim();
                DecHead = DecHead.Where(t => t.ContrNo.Contains(ContrNO));
            }

            Func<Needs.Ccs.Services.Models.DecHeadList, Models.DeclareListViewModel> convert = head => new Models.DeclareListViewModel
            {
                ID = head.ID,
                ContrNo = head.ContrNo,
                OrderID = head.OrderID,
                BillNo = head.BillNo,
                EntryId = head.EntryId,
                PreEntryId = head.PreEntryId,
                AgentName = head.AgentName,
                IsInspection = head.IsInspection == true ? "是" : "否",
                CreateDate = head.CreateTime.ToShortDateString(),
                InputerID = head.InputerID,
                StatusName = head.StatusName,
            };

            dataGridView1.DataSource = DecHead.Select(convert).ToArray();

            dataGridView1.AutoResizeColumns();
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].HeaderText = "订单号";
            dataGridView1.Columns[3].HeaderText = "合同号";
            dataGridView1.Columns[4].HeaderText = "运单号";
            dataGridView1.Columns[5].HeaderText = "海关单号";
            dataGridView1.Columns[6].HeaderText = "预录入号";
            dataGridView1.Columns[7].HeaderText = "代理企业";
            dataGridView1.Columns[8].HeaderText = "是否商检";
            dataGridView1.Columns[9].HeaderText = "录单人";
            dataGridView1.Columns[10].HeaderText = "录单时间";
            dataGridView1.Columns[11].HeaderText = "状态";
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            InitGrid();
        }


        private void BtnDeclare_Click(object sender, EventArgs e)
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
                            var head = new Needs.Ccs.Services.Views.DecHeadsView().First(t => t.ID == s);
                            head.ClientDeclare();
                        }
                        InitGrid();
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

        private void FormDeclare_SizeChanged(object sender, EventArgs e)
        {
            //AutoSizeColumn(dataGridView1);
        }

        bool CheckAll = false;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 0)//如果单击列表头，全选．
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

        private void timerDeclare_Tick(object sender, EventArgs e)
        {
            var DecHead = new Needs.Ccs.Services.Views.DecHeadsListView().AsQueryable();
            DecHead = DecHead.Where(t => t.Status == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Make));

            foreach(var item in DecHead)
            {
                var head = new Needs.Ccs.Services.Views.DecHeadsView().First(t => t.ID == item.ID);
                head.ClientDeclare();
            }

            InitGrid();
        }
    }

}
