using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.PvWsOrder.Services.Extends;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 代仓储客户供应商
    /// </summary>
    //public class MySuppliersView : UniqueView<ClientModels.Supplier,PvbCrmReponsitory>
    //{
    //    private string enterpriseid;

    //    /// <summary>
    //    /// 无参构造函数
    //    /// </summary>
    //    private MySuppliersView()
    //    {

    //    }

    //    /// <summary>
    //    /// 带参构造函数
    //    /// </summary>
    //    /// <param name="EnterpriseID">客户ID</param>
    //    public MySuppliersView(string EnterpriseID)
    //    {
    //        this.enterpriseid = EnterpriseID;
    //    }

    //    /// <summary>
    //    /// 查询结果集
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override IQueryable<ClientModels.Supplier> GetIQueryable()
    //    {
    //        var WsSupplier = new WsSuppliersTopView<PvbCrmReponsitory>(Reponsitory);
    //        return from entity in WsSupplier
    //               where entity.ClientID == this.enterpriseid
    //               select new ClientModels.Supplier
    //               {
    //                   ID = entity.ID,
    //                   ClientID=entity.ClientID,
    //                   Name = entity.Name,
    //                   ChineseName = entity.ChineseName,
    //                   EnglishName = entity.EnglishName,
    //                   Corporation = entity.Corporation,
    //                   RegAddress = entity.RegAddress,
    //                   District = entity.District,
    //                   Uscc = entity.Uscc,
    //                   Grade = entity.Grade,
    //                   Status = entity.Status,
    //                   CreateDate = entity.CreateDate,
    //                   Summary = entity.Summary,
    //               };
    //    }

    //    /// <summary>
    //    /// 持久化
    //    /// </summary>
    //    /// <param name="supplier"></param>
    //    public void Enter(ClientModels.Supplier supplier)
    //    {
    //        var id = supplier.ID ?? supplier.Name.MD5();

    //        //企业信息插入
    //        var enterprise = new Layers.Data.Sqls.PvbCrm.Enterprises
    //        {
    //            ID = id,
    //            Name = supplier.Name,
    //            AdminCode = string.Empty,
    //            Corporation = supplier.Corporation,
    //            RegAddress = supplier.RegAddress,
    //            Uscc = supplier.Uscc,
    //            Status = (int)ApprovalStatus.Normal,
    //        };
    //        enterprise.EnterpriseEnter(this.Reponsitory);

    //        var mapid = string.Join("", supplier.ClientID, id).MD5();
    //        //代仓储供应商数据插入
    //        if (this.Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>().Any(item=>item.ID == id))
    //        {
    //            if(Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item=>item.ID == mapid))
    //            {
    //                throw new Exception("代仓储供应商已经存在！");
    //            }
    //        }

    //        var wssupplier = new Layers.Data.Sqls.PvbCrm.WsSuppliers()
    //        {
    //            ID = id,
    //            ChineseName = supplier.ChineseName,
    //            EnglishName = supplier.EnglishName,
    //            Grade = (int)supplier.Grade,
    //            CreateDate = DateTime.Now,
    //            UpdateDate = DateTime.Now,
    //            Summary = supplier.Summary,
    //            Status = (int)ApprovalStatus.Normal,
    //            AdminID = supplier.AdminID,
    //        };
    //        wssupplier.SupplierEnter(this.Reponsitory);

    //        var maps = new Layers.Data.Sqls.PvbCrm.MapsBEnter
    //        {
    //            ID = mapid,
    //            Bussiness = (int)Business.WarehouseServicing,
    //            Type = (int)MapsType.Supplier,
    //            EnterpriseID = supplier.ClientID,
    //            SubID = id,
    //            CreateDate = DateTime.Now,
    //            CtreatorID = supplier.AdminID,
    //            IsDefault = false,
    //        };
    //        Reponsitory.Insert(maps);
    //    }

    //    /// <summary>
    //    /// 删除
    //    /// </summary>
    //    /// <param name="ID">供应商ID</param>
    //    /// <returns></returns>
    //    public bool Delete(string ID)
    //    {
    //        var supplier = this[ID];
    //        if (supplier == null)
    //        {
    //            return false;
    //        }
    //        var mapid = string.Join("", this.enterpriseid, ID).MD5();
    //        //供应商与客户关系删除
    //        Reponsitory.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == mapid);
    //        return true;
    //    }
    //}
}
