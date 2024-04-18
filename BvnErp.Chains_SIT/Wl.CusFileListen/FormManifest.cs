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
    public partial class FormManifest : Form
    {
        public FormManifest()
        {
            InitializeComponent();
            InitGrid();
        }

        public void InitGrid()
        {
            var ContractNo = this.tbContractNo.Text;
            var BillNo = this.tbBillNo.Text;
            var VoyageNo = this.tbVoyageNo.Text;
            var ManifestBill = new Needs.Ccs.Services.Views.ManifestConsignmentListView().AsQueryable();

            //过滤：只有已制单的才显示
            ManifestBill = ManifestBill.Where(t => t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make)
                                                  ||t.CusMftStatus== Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Deleting));

            if (!string.IsNullOrEmpty(ContractNo))
            {
                ContractNo = ContractNo.Trim();
                ManifestBill = ManifestBill.Where(t => t.ContrNO.Contains(ContractNo));
            }
            if (!string.IsNullOrEmpty(BillNo))
            {
                BillNo = BillNo.Trim();
                ManifestBill = ManifestBill.Where(t => t.BillNo.Contains(BillNo));
            }
            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                ManifestBill = ManifestBill.Where(t => t.VoyageNo.Contains(VoyageNo));
            }

            Func<Needs.Ccs.Services.Models.ManifestConsignmentList, Models.ManifestListViewModel> convert = head => new Models.ManifestListViewModel
            {
                ID = head.ID,
                VoyageNo = head.VoyageNo,
                BillNo = head.BillNo,
                Port = head.Port,
                PackNo = head.PackNo.ToString(),
                AgentName = head.ConsigneeName,
                CreateDate = head.CreateTime.ToShortDateString(),
                StatusName = head.StatusName,
            };

            dataGridView1.DataSource = ManifestBill.Select(convert).ToArray();

            dataGridView1.AutoResizeColumns();
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].HeaderText = "批次号";
            dataGridView1.Columns[3].HeaderText = "运单号";
            dataGridView1.Columns[4].HeaderText = "口岸";
            dataGridView1.Columns[5].HeaderText = "件数";
            dataGridView1.Columns[6].HeaderText = "境内收货人";
            dataGridView1.Columns[7].HeaderText = "录单时间";
            dataGridView1.Columns[8].HeaderText = "状态";
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
                    decList.Add(this.dataGridView1.Rows[i].Cells[3].Value.ToString());
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
                    BtnDeclare.Enabled = false;
                    try
                    {
                        //申报
                        foreach (string s in decList)
                        {
                            var head = new Needs.Ccs.Services.Views.ManifestConsignmentsView().First(t => t.ID == s);
                            head.Apply();
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

        private void timerManifest_Tick(object sender, EventArgs e)
        {
            //var ManifestBill = new Needs.Ccs.Services.Views.ManifestConsignmentListView().AsQueryable();
            ////过滤：只有已制单的才显示
            //ManifestBill = ManifestBill.Where(t => t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make));

            //foreach (var item in ManifestBill)
            //{
            //    var head = new Needs.Ccs.Services.Views.ManifestConsignmentsView().Where(t => t.ID == item.ID).FirstOrDefault();
            //    head.Apply();
            //}

            var ManifestBill = new Needs.Ccs.Services.Views.ManifestConsignmentsView().Where(t => t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make));
            foreach (var item in ManifestBill)
            {
                item.Apply();
            }

            InitGrid();
        }
    }
}
