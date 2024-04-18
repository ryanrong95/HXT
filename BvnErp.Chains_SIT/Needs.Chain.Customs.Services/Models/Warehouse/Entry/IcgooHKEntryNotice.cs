using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 香港库房的入库通知
    /// </summary>
    public class IcgooHKEntryNotice : HKEntryNotice
    {
        public string ConnectionString { get; set; }

        public event SealedHanlder SpeedSealed;

        public IcgooHKEntryNotice()
        {
            this.SpeedSealed += EntryNotice_SpeedSealed;
        }

        public void SealSpeed()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新入库通知状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(new { EntryNoticeStatus = EntryNoticeStatus.Sealed }, item => item.ID == this.ID);
            }
            this.OnSpeedSealed();
        }

        public virtual void OnSpeedSealed()
        {
            if (this != null && this.SpeedSealed != null)
            {
                this.SpeedSealed(this, new SealedEventArgs(this.Order.ID));
            }
        }

        private void EntryNotice_SpeedSealed(object sender, SealedEventArgs e)
        {
            string AdminID = System.Configuration.ConfigurationManager.AppSettings["AdminID"];

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var sortings = new SortingsView(reponsitory).Where(item => item.OrderID == e.OrderID && item.DecStatus == SortingDecStatus.No);
                GenerateCreator generateCreator = new GenerateCreator();
                string creatorID = generateCreator.getCreator();

                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();

                    string DeclarationNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.DeclareNotice);
                    //生成报关通知
                    DataTable dtNotice = new DataTable();
                    dtNotice.Columns.Add("ID");
                    dtNotice.Columns.Add("OrderID");
                    dtNotice.Columns.Add("AdminID");
                    dtNotice.Columns.Add("Status");
                    dtNotice.Columns.Add("CreateDate");
                    dtNotice.Columns.Add("UpdateDate");
                    dtNotice.Columns.Add("Summary");
                    dtNotice.Columns.Add("CreateDeclareAdminID");

                    DataRow drNotict = dtNotice.NewRow();
                    drNotict[0] = DeclarationNoticeID;
                    drNotict[1] = e.OrderID;
                    drNotict[2] = AdminID;
                    drNotict[3] = (int)Enums.DeclareNoticeStatus.UnDec;
                    drNotict[4] = DateTime.Now;
                    drNotict[5] = DateTime.Now;
                    drNotict[6] = "";
                    drNotict[7] = creatorID;

                    dtNotice.Rows.Add(drNotict);

                    SqlBulkCopy bulkNotice = new SqlBulkCopy(conn);
                    bulkNotice.DestinationTableName = "DeclarationNotices";
                    bulkNotice.BatchSize = dtNotice.Rows.Count;
                    bulkNotice.WriteToServer(dtNotice);

                    //生成报关通知项
                    DataTable dtNoticeItem = new DataTable();
                    dtNoticeItem.Columns.Add("ID");
                    dtNoticeItem.Columns.Add("DeclarationNoticeID");
                    dtNoticeItem.Columns.Add("SortingID");
                    dtNoticeItem.Columns.Add("Status");
                    dtNoticeItem.Columns.Add("CreateDate");
                    dtNoticeItem.Columns.Add("UpdateDate");
                    dtNoticeItem.Columns.Add("Summary");

                    foreach (var item in sortings.ToList())
                    {
                        DataRow drItem = dtNoticeItem.NewRow();
                        drItem[0] = ChainsGuid.NewGuidUp();
                        drItem[1] = DeclarationNoticeID;
                        drItem[2] = item.ID;
                        drItem[3] = (int)Enums.DeclareNoticeItemStatus.UnMake;
                        drItem[4] = DateTime.Now;
                        drItem[5] = DateTime.Now;
                        drItem[6] = "";

                        dtNoticeItem.Rows.Add(drItem);
                    }

                    if (dtNoticeItem.Rows.Count>0)
                    {
                        SqlBulkCopy bulkNoticeItem = new SqlBulkCopy(conn);
                        bulkNoticeItem.DestinationTableName = "DeclarationNoticeItems";
                        bulkNoticeItem.BatchSize = dtNoticeItem.Rows.Count;
                        bulkNoticeItem.WriteToServer(dtNoticeItem);
                    }
                }


                //更新装箱结果状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { PackingStatus = PackingStatus.Sealed }, t => t.OrderID == e.OrderID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { DecStatus = SortingDecStatus.Yes }, t => t.OrderID == e.OrderID);

                this.Order.Log(this.Operator, "通过接口下单，等待报关");
                //this.Log(this.Operator, "香港" + VendorContext.Current.CompanyName + "库房[" + this.Operator.RealName + "]完成了封箱。");
            }
        }

    }
}