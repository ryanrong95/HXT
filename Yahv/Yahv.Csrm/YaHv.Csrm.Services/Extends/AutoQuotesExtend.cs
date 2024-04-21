using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Extends
{
    public static class AutoQuotesExtend
    {
        /// <summary>
        /// 品牌批量录入
        /// </summary>
        /// <param name="list"></param>
        static public void Enter(this List<Models.Origins.AutoQuote> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
                {
                    foreach (var it in list)
                    {
                        if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.AutoQuotes>().Any(item => item.ID == it.ID))
                        {
                            repository.Insert<Layers.Data.Sqls.PvbCrm.AutoQuotes>(new Layers.Data.Sqls.PvbCrm.AutoQuotes
                            {
                                ID = it.ID,
                                Name = it.Name,
                                SupplierID = it.SupplierID,
                                Supplier = it.Supplier,
                                Manufacturer = it.Manufacturer,
                                DateCode = it.DateCode,
                                PackageCase = it.PackageCase,
                                Packaging = it.Packaging,
                                Prices = it.Prices,
                                UnitPrice = it.UnitPrice,
                                Quantity = it.Quantity,
                                ReporterID = it.ReporterID,
                                Deadline = it.Deadline,
                                CreateDate = DateTime.Now
                            });
                        }
                    }

                }

            }
        }

        static public void Delete(this IEnumerable<Models.Origins.AutoQuote> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Delete<Layers.Data.Sqls.PvbCrm.AutoQuotes>(item => arry.Contains(item.ID));
            }
        }
    }
}
