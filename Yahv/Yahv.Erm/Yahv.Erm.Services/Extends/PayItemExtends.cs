using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Extends
{
    static public class PayItemExtends
    {
        /// <summary>
        /// 工资项
        /// </summary>
        static public void Enter(this IEnumerable<PayItem> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = LinqFactory<PvbErmReponsitory>.Create())
                {
                    StringBuilder sql = new StringBuilder();

                    foreach (var payItem in list)
                    {
                        //更新
                        if (!payItem.IsInsert)
                        {
                            //repository.Update<PayItems>(new
                            //{
                            //    Value = payItem.Value,
                            //    UpdateDate = DateTime.Now,
                            //    ActualFormula = payItem.ActualFormula,
                            //    WageItemFormula = payItem.WageItemFormula,
                            //    Description = payItem.Description,
                            //    UpdateAdminID = payItem.UpdateAdminID,
                            //}, a => a.PayID == payItem.PayID && a.Name == payItem.Name);

                            sql.Append("UPDATE dbo.PayItems SET Value=").Append(payItem.Value)
                                    .Append(",UpdateDate=getdate(),WageItemFormula='").Append(payItem.WageItemFormula)
                                    .Append("',Description='").Append(payItem.Description).Append(",Status=").Append(payItem.Status)
                                    .Append("' where ID='").Append(string.Join("", payItem.PayID, payItem.Name).MD5()).Append("';");
                            //.Append("' where PayID='").Append(payItem.PayID).Append("' and Name='").Append(payItem.Name).Append("';");
                        }
                        else
                        {
                            //repository.Insert<Layers.Data.Sqls.PvbErm.PayItems>(new PayItems()
                            //{
                            //    ID = string.Join("", payItem.PayID, payItem.Name).MD5(),
                            //    CreateDate = DateTime.Now,
                            //    UpdateDate = DateTime.Now,
                            //    DateIndex = payItem.DateIndex,
                            //    Name = payItem.Name,
                            //    Value = payItem.Value,
                            //    AdminID = payItem.AdminID,
                            //    PayID = payItem.PayID,
                            //    //ActualFormula = payItem.ActualFormula,
                            //    WageItemFormula = payItem.WageItemFormula,
                            //    //Description = payItem.Description,
                            //    UpdateAdminID = payItem.AdminID,
                            //    Status = (int)payItem.Status,
                            //});

                            sql.Append(
                                       "INSERT INTO dbo.PayItems(ID,PayID,Name,Value,DateIndex,WageItemFormula,CreateDate,UpdateDate,AdminID,Status)")
                                       .Append(" Values('").Append(string.Join("", payItem.PayID, payItem.Name).MD5()).Append("','").Append(payItem.PayID).Append("','").Append(payItem.Name).Append("','")
                                       .Append(payItem.Value).Append("','").Append(payItem.DateIndex).Append("','").Append(payItem.WageItemFormula).Append("','").Append(DateTime.Now).Append("','")
                                       .Append(DateTime.Now).Append("','").Append(payItem.AdminID).Append("',").Append((int)payItem.Status).Append(");");
                        }
                    }

                    if (sql.Length > 0)
                    {
                        var array = sql.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        int count = 30000;      //执行个数
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
