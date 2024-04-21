using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Extends
{
    public static class FilesDescriptionExtend
    {
        static public void Enter(this List<Models.Origins.FilesDescription> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = new PvdCrmReponsitory())
                {
                    foreach (var it in list)
                    {
                        repository.Insert(new Layers.Data.Sqls.PvdCrm.FilesDescription
                        {
                            ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.File),
                            EnterpriseID = it.EnterpriseID,
                            SubID = it.SubID,
                            CustomName = it.CustomName,
                            Url = it.Url,
                            Summary = it.Summary,
                            Type = (int)it.Type,
                            CreateDate = it.CreateDate,
                            CreatorID = it.CreatorID,
                            Status = (int)it.Status,
                        });

                    }

                }

            }
        }
        static public void Enter(this IEnumerable<Models.Origins.FilesDescription> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
                {
                    foreach (var it in list)
                    {
                        repository.Insert(new Layers.Data.Sqls.PvdCrm.FilesDescription
                        {
                            ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.File),
                            EnterpriseID = it.EnterpriseID,
                            SubID = it.SubID,
                            CustomName = it.CustomName,
                            Url = it.Url,
                            Summary = it.Summary,
                            Type = (int)it.Type,
                            CreateDate = it.CreateDate,
                            CreatorID = it.CreatorID,
                            Status = (int)it.Status,
                        });
                    }

                }

            }
        }
        static public void Abandon(this IEnumerable<Models.Origins.FilesDescription> list)
        {
            if (list.Count() > 0)
            {
                using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
                {
                    var ids = list.Select(item => item.ID).ToArray();
                    repository.Update<Layers.Data.Sqls.PvdCrm.FilesDescription>(new
                    {
                        Status = (int)Underly.DataStatus.Closed
                    }, item => ids.Contains(item.ID));
                }

            }
        }
    }
}
