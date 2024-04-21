using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Extends
{
    static public class ManufacturerExtend
    {
        /// <summary>
        /// 品牌批量录入
        /// </summary>
        /// <param name="list"></param>
        static public void Enter(this List<Models.Origins.Manufacturer> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
                {
                    foreach (var it in list)
                    {
                        if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Manufacturers>().Any(item => item.ID == it.ID))
                        {
                            repository.Update<Layers.Data.Sqls.PvbCrm.Manufacturers>(new
                            {
                                Agent = it.Agent
                            }, item => item.ID == it.ID);
                        }
                        else
                        {
                            repository.Insert<Layers.Data.Sqls.PvbCrm.Manufacturers>(new Layers.Data.Sqls.PvbCrm.Manufacturers
                            {
                                ID = it.ID,
                                Name = it.Name,
                                Agent = it.Agent
                            });
                        }
                    }

                }

            }
        }
        static public void Enter(this List<Models.Origins.PartNumber> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
                {
                    foreach (var it in list)
                    {
                        if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.PartNumbers>().Any(item => item.ID == it.ID))
                        {
                            repository.Insert<Layers.Data.Sqls.PvbCrm.PartNumbers>(new Layers.Data.Sqls.PvbCrm.PartNumbers
                            {
                                ID = it.ID,
                                Name = it.Name,
                                Manufacturer = it.Manufacturer
                            });
                        }
                    }

                }

            }
        }

        static public IEnumerable<ViewManufacturer> ToManufacturerView(this string obj)
        {
            return JsonSerializerExtend.JsonTo<Newtonsoft.Json.Linq.JArray>(obj).ToObject<List<ViewManufacturer>>();
        }
        static public IEnumerable<ViewPartNumber> ToPartNumberView(this string obj)
        {
            return JsonSerializerExtend.JsonTo<Newtonsoft.Json.Linq.JArray>(obj).ToObject<List<ViewPartNumber>>();
        }
    }
}
