using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Wms.Services.Enums;
using Yahv.Linq;
using Yahv.Web.Mvc;

namespace Wms.Services.chonggous.Views
{
    public class CgLogOperatorsView : QueryView<CgLogs_Operator, PvWmsRepository>
    {
        #region 构造函数

        public CgLogOperatorsView()
        {
        }

        protected CgLogOperatorsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgLogOperatorsView(PvWmsRepository reponsitory, IQueryable<CgLogs_Operator> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        protected override IQueryable<CgLogs_Operator> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Operator>()
                       select new CgLogs_Operator
                       {
                           ID = entity.ID,
                           Conduct = entity.Conduct,
                           Content = entity.Content,
                           CreateDate = entity.CreateDate,
                           CreatorID = entity.CreatorID,
                           MainID = entity.MainID,
                           Type = (LogOperatorType)Enum.Parse(typeof(LogOperatorType), entity.Type),
                       };

            return view;
        }

        public object ToMyPage(int pageIndex, int pageSize)
        {
            var iquery = this.IQueryable.Cast<CgLogs_Operator>();
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = iquery.ToArray(),
            };

        }

        /// <summary>
        /// 根据MainID来搜索对应的日志
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public CgLogOperatorsView SearchByMainID(string waybillID)
        {
            var iquery = this.IQueryable.Cast<CgLogs_Operator>();            
            var logs = iquery.Where(item => item.MainID == waybillID);

            var view = new CgLogOperatorsView(this.Reponsitory, logs);
            return view;
        }

        /// <summary>
        /// 根据Key关键字来搜索日志
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CgLogOperatorsView SearyByKey(string key)
        {
            var iquery = this.IQueryable.Cast<CgLogs_Operator>();
            var logs = iquery.Where(item => item.Content.Contains(key));

            var view = new CgLogOperatorsView(this.Reponsitory, logs);
            return view;
        }

        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="jpost"></param>
        public void Enter(JPost jpost)
        {
            List<CgLogs_Operator> logList = new List<CgLogs_Operator>();
            var logs = jpost["Logs"];
            foreach (var log in logs)
            {
                var mainID = log["MainID"].Value<string>();
                var content = log["Content"].Value<string>();
                var createDate = log["CreateDate"].Value<DateTime>();
                var creatorID = log["CreatorID"].Value<string>();
                var conduct = log["Conduct"].Value<string>();
                var type = log["Type"].Value<string>();

                logList.Add(new CgLogs_Operator
                {
                    ID = PKeySigner.Pick(PkeyType.Logs_Operator),
                    Conduct = conduct,
                    Content = content,
                    CreatorID = creatorID,
                    CreateDate = createDate,
                    MainID = mainID,
                    Type = (LogOperatorType)Enum.Parse(typeof(LogOperatorType), type),
                });
            }

            if (logList.Count() > 0)
            {
                foreach (var log in logList)
                {
                    log.Enter(this.Reponsitory);
                }
            }
             
        }
    }
}
