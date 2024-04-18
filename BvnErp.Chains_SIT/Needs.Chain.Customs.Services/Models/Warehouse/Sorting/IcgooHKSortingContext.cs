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
    /// 库房分拣
    /// </summary>
    public class IcgooHKSortingContext: HKSortingContext
    {
        /// <summary>
        /// 内单或者Icgoo分拣项
        /// </summary>
        public IEnumerable<InsideSortingModel> InsideItems;

        public string ConnectionString { get; set; }

        public IcgooHKSortingContext()
        {

        }

     
   
      
        public void PackSpeed()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    this.Packing.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Packing);
                    #region 新增装箱结果Packing
                    SqlCommand sqlCmd = new SqlCommand("PackingsPro", conn);
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    sqlCmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                    sqlCmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                    sqlCmd.Parameters.Add(new SqlParameter("@BoxIndex", SqlDbType.VarChar,50));
                    sqlCmd.Parameters.Add(new SqlParameter("@PackingDate", SqlDbType.DateTime));
                    sqlCmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
                    sqlCmd.Parameters.Add(new SqlParameter("@WrapType", SqlDbType.VarChar,50));
                    sqlCmd.Parameters.Add(new SqlParameter("@PackingStatus", SqlDbType.Int));
                    sqlCmd.Parameters.Add(new SqlParameter("@AdminID", SqlDbType.VarChar,50));
                    sqlCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                    sqlCmd.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                    sqlCmd.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                    sqlCmd.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 300));

                    sqlCmd.Parameters["@ID"].Value = this.Packing.ID;
                    sqlCmd.Parameters["@OrderID"].Value = this.Packing.OrderID;
                    sqlCmd.Parameters["@BoxIndex"].Value = this.Packing.BoxIndex;
                    sqlCmd.Parameters["@PackingDate"].Value = this.Packing.PackingDate;
                    sqlCmd.Parameters["@Weight"].Value = this.Packing.Weight;
                    sqlCmd.Parameters["@WrapType"].Value = this.Packing.WrapType;
                    sqlCmd.Parameters["@PackingStatus"].Value = (int)this.Packing.PackingStatus;
                    sqlCmd.Parameters["@AdminID"].Value = this.Packing.AdminID;
                    sqlCmd.Parameters["@Status"].Value = (int)Enums.Status.Normal;
                    sqlCmd.Parameters["@CreateDate"].Value = DateTime.Now;
                    sqlCmd.Parameters["@UpdateDate"].Value = DateTime.Now;
                    sqlCmd.Parameters["@Summary"].Value = "";

                    sqlCmd.ExecuteNonQuery();
                    #endregion

                    #region Sorting
                    DataTable dtSorting = new DataTable();
                    dtSorting.Columns.Add("ID");
                    dtSorting.Columns.Add("OrderID");
                    dtSorting.Columns.Add("OrderItemID");
                    dtSorting.Columns.Add("EntryNoticeItemID");
                    dtSorting.Columns.Add("WarehouseType");
                    dtSorting.Columns.Add("WrapType");                   
                    dtSorting.Columns.Add("Quantity");
                    dtSorting.Columns.Add("BoxIndex");
                    dtSorting.Columns.Add("NetWeight");
                    dtSorting.Columns.Add("GrossWeight");
                    dtSorting.Columns.Add("DecStatus");
                    dtSorting.Columns.Add("AdminID");
                    dtSorting.Columns.Add("Status");
                    dtSorting.Columns.Add("SZPackingDate");
                    dtSorting.Columns.Add("CreateDate");
                    dtSorting.Columns.Add("UpdateDate");
                    dtSorting.Columns.Add("Summary");
                    #endregion

                    #region StoreStorages
                    DataTable dtStorage = new DataTable();
                    dtStorage.Columns.Add("ID");
                    dtStorage.Columns.Add("OrderItemID");
                    dtStorage.Columns.Add("SortingID");                 
                    dtStorage.Columns.Add("Purpose");
                    dtStorage.Columns.Add("Quantity");
                    dtStorage.Columns.Add("StockCode");
                    dtStorage.Columns.Add("BoxIndex");
                    dtStorage.Columns.Add("CreateDate");
                    dtStorage.Columns.Add("UpdateDate");
                    dtStorage.Columns.Add("Status");
                    dtStorage.Columns.Add("Summary");
                    #endregion

                    #region PackingItems
                    DataTable dtPackingItems = new DataTable();
                    dtPackingItems.Columns.Add("ID");
                    dtPackingItems.Columns.Add("PackingID");
                    dtPackingItems.Columns.Add("SortingID");
                    dtPackingItems.Columns.Add("Status");
                    dtPackingItems.Columns.Add("CreateDate");
                    #endregion

                    foreach (var model in InsideItems)
                    {
                        #region Sorting
                        DataRow drSorting = dtSorting.NewRow();
                        drSorting[0] = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting);
                        drSorting[1] = this.Packing.OrderID;
                        drSorting[2] = model.OrderItemID;
                        drSorting[3] = model.EntryNoticeItemID;
                        drSorting[4] = (int)WarehouseType.HongKong;
                        drSorting[5] = this.Packing.WrapType;                      
                        drSorting[6] = model.Quantity;
                        drSorting[7] = this.Packing.BoxIndex;
                        drSorting[8] = model.NetWeight;
                        drSorting[9] = model.GrossWeight;
                        drSorting[10] = (int)SortingDecStatus.No;
                        drSorting[11] = this.Packing.AdminID;
                        drSorting[12] = (int)Status.Normal;
                        drSorting[13] = null;
                        drSorting[14] = DateTime.Now;
                        drSorting[15] = DateTime.Now;
                        drSorting[16] = "";

                        dtSorting.Rows.Add(drSorting);
                        #endregion

                        #region StoreStorages
                        DataRow drStorage = dtStorage.NewRow();
                        drStorage[0] = Needs.Overall.PKeySigner.Pick(PKeyType.StoreStorage);
                        drStorage[1] = model.OrderItemID;
                        drStorage[2] = drSorting[0];                       
                        drStorage[3] = (int)StockPurpose.Declared;
                        drStorage[4] = model.Quantity;
                        drStorage[5] = this.Stock;
                        drStorage[6] = this.BoxIndex;
                        drStorage[7] = DateTime.Now;
                        drStorage[8] = DateTime.Now;
                        drStorage[9] = (int)Status.Normal;                       
                        drStorage[10] = "";

                        dtStorage.Rows.Add(drStorage);
                        #endregion

                        #region PackingItems
                        DataRow drItems = dtPackingItems.NewRow();
                        drItems[0] = Needs.Overall.PKeySigner.Pick(PKeyType.PackingItem);
                        drItems[1] = this.Packing.ID;
                        drItems[2] = drSorting[0];
                        drItems[3] = (int)Status.Normal;
                        drItems[4] = DateTime.Now;

                        dtPackingItems.Rows.Add(drItems);
                        #endregion
                    }

                    if (dtSorting.Rows.Count >0)
                    {
                        SqlBulkCopy bulkSorting = new SqlBulkCopy(conn);
                        bulkSorting.DestinationTableName = "Sortings";
                        bulkSorting.BatchSize = dtSorting.Rows.Count;
                        bulkSorting.WriteToServer(dtSorting);
                    }

                    if (dtStorage.Rows.Count>0)
                    {
                        SqlBulkCopy bulkStorage = new SqlBulkCopy(conn);
                        bulkStorage.DestinationTableName = "StoreStorages";
                        bulkStorage.BatchSize = dtStorage.Rows.Count;
                        bulkStorage.WriteToServer(dtStorage);
                    }

                    if (dtPackingItems.Rows.Count>0)
                    {
                        SqlBulkCopy bulkItems = new SqlBulkCopy(conn);
                        bulkItems.DestinationTableName = "PackingItems";
                        bulkItems.BatchSize = dtPackingItems.Rows.Count;
                        bulkItems.WriteToServer(dtPackingItems);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }

}