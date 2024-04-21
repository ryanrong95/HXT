using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Extends
{
    static public class PayBillItemExtends
    {
        static public void Enter(this IEnumerable<PayBillItem> list)
        {
            if (list.Any())
            {
                using (var repository = LinqFactory<PvbErmReponsitory>.Create())
                {
                    StringBuilder sql = new StringBuilder();

                    //添加工资单
                    var payBills_Insert = list.Where(item => item.IsInsert).Select(t => new Layers.Data.Sqls.PvbErm.PayBills()
                    {
                        Status = (int)t.Status,
                        ID = t.ID,
                        StaffID = t.StaffID,
                        CreaetDate = t.CreaetDate,
                        DateIndex = t.DateIndex,
                        EnterpriseID = t.EnterpriseID,
                        PostionID = t.PostionID,
                    }).ToArray();
                    if (payBills_Insert.Length > 0)
                    {
                        //repository.Insert<Layers.Data.Sqls.PvbErm.PayBills>(payBills_Insert);
                        foreach (var payBill in payBills_Insert)
                        {
                            sql.Append("INSERT INTO dbo.PayBills(ID,StaffID,DateIndex,EnterpriseID,CreaetDate,Status,PostionID) VALUES('").Append(payBill.ID).Append("','").Append(payBill.StaffID).Append("','").Append(payBill.DateIndex).Append("','").Append(payBill.EnterpriseID).Append("','").Append(payBill.CreaetDate).Append("',").Append((int)payBill.Status).Append(",'").Append(payBill.PostionID).Append("');");
                        }
                    }


                    //修改工资单
                    var payBills_Update = list.Where(item => !item.IsInsert && item.IsUpdate).ToList();

                    foreach (var payBillItem in payBills_Update)
                    {
                        //repository.Update<Layers.Data.Sqls.PvbErm.PayBills>(new
                        //{
                        //    EnterpriseID = payBillItem.EnterpriseID,
                        //    UpdateDate = DateTime.Now,
                        //}, a => a.ID == payBillItem.ID);

                        sql.Append("UPDATE dbo.PayBills	set EnterpriseID='").Append(payBillItem.EnterpriseID).Append("',UpdateDate=GETDATE(),PostionID='").Append(payBillItem.PostionID).Append("' where id='").Append(payBillItem.ID).Append("';");
                    }


                    //添加或修改工资项
                    Layers.Data.Sqls.PvbErm.PayItems[] payItems_Insert;
                    List<PayItem> payItems_Update;

                    foreach (var payBillItem in list)
                    {
                        if (payBillItem.PayItems != null && payBillItem.PayItems.Any())
                        {
                            //添加
                            payItems_Insert = payBillItem.PayItems.Where(item => item.IsInsert).Select(t => new Layers.Data.Sqls.PvbErm.PayItems()
                            {
                                DateIndex = t.DateIndex,
                                Name = t.Name,
                                PayID = t.PayID,
                                Value = t.Value,
                                AdminID = t.AdminID,
                                ID = string.Join("", t.PayID, t.Name).MD5(),
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                WageItemFormula = t.WageItemFormula,
                                Status = (int)t.Status
                            }).ToArray();
                            if (payItems_Insert.Length > 0)
                            {
                                //repository.Insert<Layers.Data.Sqls.PvbErm.PayItems>(payItems_Insert);
                                foreach (var payItem in payItems_Insert)
                                {
                                    sql.Append(
                                        "INSERT INTO dbo.PayItems(ID,PayID,Name,Value,DateIndex,WageItemFormula,CreateDate,UpdateDate,AdminID,Status)")
                                        .Append(" Values('").Append(payItem.ID).Append("','").Append(payItem.PayID).Append("','").Append(payItem.Name).Append("','")
                                        .Append(payItem.Value).Append("','").Append(payItem.DateIndex).Append("','").Append(payItem.WageItemFormula).Append("','").Append(payItem.CreateDate).Append("','")
                                        .Append(payItem.UpdateDate).Append("','").Append(payItem.AdminID).Append("',").Append((int)payItem.Status).Append(");");

                                }
                            }

                            //修改
                            payItems_Update = payBillItem.PayItems.Where(item => !item.IsInsert).ToList();
                            foreach (var payItem in payItems_Update)
                            {
                                //repository.Update<Layers.Data.Sqls.PvbErm.PayItems>(new
                                //{
                                //    Value = payItem.Value,
                                //    UpdateDate = DateTime.Now,
                                //    //ActualFormula = payItem.ActualFormula,
                                //    WageItemFormula = payItem.WageItemFormula,
                                //    Description = payItem.Description,
                                //}, a => a.PayID == payItem.PayID && a.Name == payItem.Name);

                                sql.Append("UPDATE dbo.PayItems SET Value=").Append(payItem.Value)
                                    .Append(",UpdateDate=getdate(),WageItemFormula='").Append(payItem.WageItemFormula)
                                    .Append("',Description='").Append(payItem.Description).Append("',Status=").Append((int)payItem.Status)
                                    .Append(" where ID='").Append(string.Join("", payItem.PayID, payItem.Name).MD5()).Append("';");
                                //.Append("' where PayID='").Append(payItem.PayID).Append("' and Name='").Append(payItem.Name).Append("';");
                            }
                        }
                    }

                    if (sql.Length > 0)
                    {
                        var array = sql.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        int count = 2000;      //执行个数
                        int i = 0;

                        while (i * count < array.Length)
                        {
                            repository.Command(string.Join(";", array.Skip(i * count).Take(count)));
                            i++;
                        }
                    }
                }
            }
        }
    }
}
