using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 我的供应商视图
    /// </summary>
    public class MySuppliers : UniqueView<ClientModels.Supplier, PvbCrmReponsitory>
    {
        string ClientID;

        public MySuppliers(string clientid)
        {
            this.ClientID = clientid;
        }

        protected override IQueryable<ClientModels.Supplier> GetIQueryable()
        {
            var supplierview = new wsnSuppliersTopView<PvbCrmReponsitory>(this.Reponsitory).Where(item => item.OwnID == this.ClientID && item.Status != GeneralStatus.Deleted && item.Status != GeneralStatus.Closed);

            return supplierview.Select(item => new ClientModels.Supplier
            {
                ID = item.ID,
                OwnID = item.OwnID,//所属企业即客户ID 
                OwnName = item.OwnName,//客户名称
                RealEnterpriseID = item.RealEnterpriseID,//供应商ID
                RealEnterpriseName = item.RealEnterpriseName,//供应商名称
                nGrade = item.nGrade,
                EnterCode = item.EnterCode,
                ClientGrade = item.ClientGrade,
                Place = item.Place,
                Status = item.Status,
                Corporation = item.Corporation,//供应商企业信息
                Uscc = item.Uscc,//供应商企业信息
                RegAddress = item.RegAddress,//供应商企业信息
                ChineseName = item.ChineseName,
                EnglishName = item.EnglishName,
                CHNabbreviation = item.CHNabbreviation//中文简称
            });
        }
    }
}
