using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public class PackingHistory
    {
        public List<string> OrderIDs { get; set; }
        public string ConnectionString { get; set; }
      
        public PackingHistory()
        {
            this.OrderIDs = new List<string>();
            this.ConnectionString = "Data Source=101.200.55.149,6522;Initial Catalog=foricScCustoms;Persist Security Info=True;User ID=u149_v8site;Password=WH1g0LfN42o29HyS7fsqvuTMMh7wgOa6X798cshVI14ae1ylnta";
        }
        public void GetOrderIDs(DateTime dtStart,DateTime dtEnd)
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var Orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.CreateDate > dtStart && t.CreateDate < dtEnd).OrderBy(t=>t.CreateDate).ToList();
                foreach(var order in Orders)
                {
                    this.OrderIDs.Add(order.ID);
                }
            }
        }

        public void GetOrderIDs()
        {
            
        }

        public void GenerateHistory()
        {
            GenerateEntryHistory();
            GeneratePackingHistory();
        }

        public void GenerateEntryHistory()
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                foreach (var orderID in this.OrderIDs)
                {
                    try
                    {
                        var decHead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.OrderID == orderID).FirstOrDefault();
                        if (decHead == null)
                        {
                            continue;
                        }


                        Console.WriteLine(orderID + "开始Entry");
                        //var OrderInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == orderID).FirstOrDefault();
                        var entryNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Where(t => t.OrderID == orderID).FirstOrDefault();
                        if (entryNotice != null)
                        {
                            continue;
                        }

                        var OrderItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == orderID);
                        DateTime dtCreate = OrderItems.FirstOrDefault().CreateDate;
                        #region 新增EntryNotice
                        string entryNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                        SqlCommand sqlCmd = new SqlCommand("EntryNotice", conn);
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                        sqlCmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                        sqlCmd.Parameters.Add(new SqlParameter("@WarehouseType", SqlDbType.Int));
                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", SqlDbType.VarChar, 50));
                        sqlCmd.Parameters.Add(new SqlParameter("@SortingRequire", SqlDbType.Int));
                        sqlCmd.Parameters.Add(new SqlParameter("@EntryNoticeStatus", SqlDbType.Int));
                        sqlCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                        sqlCmd.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                        sqlCmd.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                        sqlCmd.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 300));

                        sqlCmd.Parameters["@ID"].Value = entryNoticeID;
                        sqlCmd.Parameters["@OrderID"].Value = orderID;
                        sqlCmd.Parameters["@WarehouseType"].Value = (int)WarehouseType.HongKong;
                        sqlCmd.Parameters["@ClientCode"].Value = orderID.Substring(0, 5);
                        sqlCmd.Parameters["@SortingRequire"].Value = SortingRequire.UnPacking;
                        sqlCmd.Parameters["@EntryNoticeStatus"].Value = (int)EntryNoticeStatus.Sealed;
                        sqlCmd.Parameters["@Status"].Value = (int)Status.Normal;
                        sqlCmd.Parameters["@CreateDate"].Value = dtCreate;
                        sqlCmd.Parameters["@UpdateDate"].Value = dtCreate;
                        sqlCmd.Parameters["@Summary"].Value = "";

                        sqlCmd.ExecuteNonQuery();
                        #endregion

                        #region 新增EntryNoticeItem
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID");
                        dt.Columns.Add("EntryNoticeID");
                        dt.Columns.Add("OrderItemID");
                        dt.Columns.Add("DecListID");
                        dt.Columns.Add("IsSpotCheck");
                        dt.Columns.Add("EntryNoticeStatus");
                        dt.Columns.Add("Status");
                        dt.Columns.Add("CreateDate");
                        dt.Columns.Add("UpdateDate");

                        foreach (var item in OrderItems)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                            dr[1] = entryNoticeID;
                            dr[2] = item.ID;
                            dr[3] = null;
                            dr[4] = item.IsSampllingCheck;
                            dr[5] = (int)EntryNoticeStatus.Sealed;
                            dr[6] = (int)Status.Normal;
                            dr[7] = dtCreate;
                            dr[8] = dtCreate;

                            dt.Rows.Add(dr);
                        }

                        SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                        bulkCopy.DestinationTableName = "EntryNoticeItems";
                        bulkCopy.BatchSize = dt.Rows.Count;
                        bulkCopy.WriteToServer(dt);
                        #endregion


                        Console.WriteLine(orderID + "结束Entry");
                    }
                    catch (Exception ex)
                    {
                        ex.CcsLog("Entry Error " + orderID);
                        Console.WriteLine(orderID + "出错Entry" + ex.ToString());
                        continue;
                    }
                }
            }
        }

        public void GeneratePackingHistory()
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var order in this.OrderIDs)
                {
                    try
                    {
                        var decHead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.OrderID == order).FirstOrDefault();
                        if (decHead == null)
                        {
                            continue;
                        }


                        Console.WriteLine(order + "开始Packing");
                        var PackingHistroy = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>().Where(t => t.TinyOrderID == order).ToList();
                        var EntryNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Where(t => t.OrderID == order).FirstOrDefault();
                        var EntryNoticeItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>().Where(t => t.EntryNoticeID == EntryNotice.ID).ToList();
                       
                        var Boxes = PackingHistroy.Select(t => t.BoxCode).Distinct().ToList();
                        foreach (var box in Boxes)
                        {
                            var Products = PackingHistroy.Where(item => item.BoxCode == box).ToList();

                            IcgooHKSortingContext hkSorting = new IcgooHKSortingContext();
                            hkSorting.ToShelve("history", box);

                            PackingModel packing = new PackingModel();
                            packing.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Packing);
                            packing.AdminID = Products.FirstOrDefault().Packer;
                            packing.OrderID = order;
                            packing.BoxIndex = box;
                            packing.Weight = Products.Select(item => item.Weight.Value).Sum();
                            packing.WrapType = Icgoo.WrapType;
                            packing.PackingDate = Products.FirstOrDefault().BoxingDate;
                            packing.Quantity = Products.Select(item => item.Quantity).Sum();
                            packing.PackingStatus = PackingStatus.Exited;
                            hkSorting.SetPacking(packing);

                            List<InsideSortingModel> list = new List<InsideSortingModel>();

                            Products.ForEach(p =>
                            {
                                InsideSortingModel model = new InsideSortingModel();
                                model.EntryNoticeItemID = EntryNoticeItems.Where(t => t.OrderItemID == p.OrderItemID).FirstOrDefault().ID;
                                model.OrderItemID = p.OrderItemID;
                                model.Quantity = p.Quantity;
                                model.NetWeight = p.NetWeight.Value;
                                model.GrossWeight = p.Weight.Value;
                                list.Add(model);
                            });

                            hkSorting.InsideItems = list;
                            hkSorting.ConnectionString = this.ConnectionString;
                            //hkSorting.PackSpeed(packing.PackingDate);
                        }
                        Console.WriteLine(order + "结束Packing");
                    }
                    catch(Exception ex)
                    {
                        ex.CcsLog("Packing Error "+ order);
                        Console.WriteLine(order + "出错Packing" + ex.ToString());
                        continue;
                    }
                }
            }                
        }

    }
}
