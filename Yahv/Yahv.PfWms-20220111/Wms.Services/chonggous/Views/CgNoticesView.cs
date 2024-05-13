using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Views
{
    public class CgNoticesView : UniqueView<Models.CgNotice, PvWmsRepository>
    {
        public CgNoticesView()
        {

        }

        protected override IQueryable<CgNotice> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                   select new Models.CgNotice
                   {
                       ID = entity.ID,
                       Type = (CgNoticeType)entity.Type,
                       WareHouseID = entity.WareHouseID,
                       WaybillID = entity.WaybillID,
                       InputID = entity.InputID,
                       OutputID = entity.OutputID,
                       ProductID = entity.ProductID,
                       Supplier = entity.Supplier,
                       Quantity = entity.Quantity,
                       Conditions = entity.Conditions.JsonTo<NoticeCondition>(),
                       CreateDate = entity.CreateDate,
                       ShelveID = entity.ShelveID,
                       Status = (NoticesStatus)entity.Status,
                       Source = (NoticeSource)entity.Source,
                       Target = (NoticesTarget)entity.Target,
                       BoxCode = entity.BoxCode,
                       DateCode = entity.DateCode,
                       Weight = entity.Weight,
                       Volume = entity.Volume,
                       BoxingSpecs = entity.BoxingSpecs,
                       NetWeight = entity.NetWeight,
                       StorageID = entity.StorageID,
                   };
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mainID"></param>
        /// <param name="operation"></param>
        /// <param name="summary"></param>
        /// <param name="creator"></param>
        public void AddLog(LogType type, string mainID, string operation, string summary, string creator)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Operating
                {
                    ID = Guid.NewGuid().ToString(),
                    Type = (int)type,
                    MainID = mainID,
                    Operation = operation,
                    Summary = summary,
                    CreateDate = DateTime.Now,
                    Creator = creator,
                });
            }
        }

    }
}
