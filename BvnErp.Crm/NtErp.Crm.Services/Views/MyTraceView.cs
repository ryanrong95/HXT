using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Views
{
    public class MyTraceView : UniqueView<Trace, BvCrmReponsitory>
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        MyTraceView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyTraceView(IGenericAdmin admin)
        {
            this.Admin = admin; 
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal MyTraceView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 获取跟踪记录数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Trace> GetIQueryable()
        {
            AdminsTopView topview = new AdminsTopView(this.Reponsitory);
            //MyClientsView clientview = new MyClientsView(this.Admin,this.Reponsitory);
            return from trace in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm._bak_Traces>()
                    join admin in topview on trace.AdminID equals admin.ID
                    //join client in clientview on trace.ClientID equals client.ID
                    select new Trace
                    {
                        ID = trace.ID,
                        Type = (ActionMethord)trace.Type,
                        Date = trace.Date,
                        NextDate=trace.NextDate,
                        Context = trace.Context,
                        Admin = admin,
                        Status = (Status)trace.Status,
                        CreateDate = trace.CreateDate,
                        UpdateDate = trace.UpdateDate,
                        //Client = client,
                    };
        }
    }
}
