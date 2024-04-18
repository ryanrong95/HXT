using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    ///// <summary>
    ///// 供应商收货地址视图
    ///// </summary>
    //public class MySupplierAddressView : WsConsignors<PvbCrmReponsitory>
    //{
    //    private string EnterpriseID;
    //    private string[] SupplierIDs;

    //    private MySupplierAddressView()
    //    {

    //    }

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="enterpriseid">客户ID</param>
    //    /// <param name="supplierid">供应商ID</param>
    //    internal MySupplierAddressView(string enterpriseid, string supplierid)
    //    {
    //        this.EnterpriseID = enterpriseid;
    //        this.SupplierIDs = new string[] { supplierid };
    //    }

    //    public MySupplierAddressView(string enterpriseid, string[] supplierids)
    //    {
    //        this.EnterpriseID = enterpriseid;
    //        this.SupplierIDs = supplierids;
    //    }

    //    /// <summary>
    //    /// 查询结果集
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override IQueryable<Consignor> GetIQueryable()
    //    {
    //        return base.GetIQueryable().Where(item => item.ClientID == this.EnterpriseID && this.SupplierIDs.Contains(item.EnterpriseID));
    //    }

    //    /// <summary>
    //    /// 数据持久化
    //    /// </summary>
    //    /// <param name="consignor"></param>
    //    public void Enter(Consignor consignor)
    //    {
    //        var id = consignor.ID ?? string.Join("", consignor.EnterpriseID, consignor.DyjCode, consignor.Address, consignor.Postzip).MD5();

    //        if(Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Consignors>().Any(item=>item.ID == id))
    //        {
    //            Reponsitory.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
    //            {
    //                Address = consignor.Address,
    //                Postzip = consignor.Postzip,
    //                Name = consignor.Name,
    //                Tel = consignor.Tel,
    //                Mobile = consignor.Mobile,
    //                Email = consignor.Email,
    //                UpdateDate = DateTime.Now,
    //                IsDefault = consignor.IsDefault
    //            }, item => item.ID == id);
    //        }
    //        else
    //        {
    //            Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Consignors
    //            {
    //                ID = id,
    //                EnterpriseID = consignor.EnterpriseID,
    //                Title = consignor.Title,
    //                DyjCode = consignor.DyjCode ?? string.Empty,
    //                Address = consignor.Address,
    //                Postzip = consignor.Postzip,
    //                Name = consignor.Name,
    //                Tel = consignor.Tel,
    //                Mobile = consignor.Mobile,
    //                Email = consignor.Email,
    //                Status = (int)ApprovalStatus.Normal,
    //                CreateDate = DateTime.Now,
    //                UpdateDate = DateTime.Now,
    //                AdminID = consignor.AdminID,
    //                IsDefault = consignor.IsDefault,
    //            });
    //        }

    //        var mapsid = "WsConsignor_" + string.Join("", this.EnterpriseID, id).MD5();
    //        if (!Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
    //        {
    //            Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
    //            {
    //                ID = mapsid,
    //                Bussiness = (int)Business.WarehouseServicing,
    //                Type = (int)MapsType.Consignor,
    //                EnterpriseID = this.EnterpriseID,
    //                SubID = id,
    //                CreateDate = DateTime.Now,
    //                CtreatorID = consignor.AdminID,
    //                IsDefault = false,
    //            });
    //        }
    //        //设为默认提货地址
    //        if(consignor.IsDefault)
    //        {
    //            SetDefault(id);
    //        }
    //    }


    //    /// <summary>
    //    /// 删除
    //    /// </summary>
    //    /// <param name="ID"></param>
    //    public bool Delete(string ID)
    //    {
    //        var consignor = this[ID];
    //        if (consignor == null)
    //        {
    //            return false;
    //        }
    //        var mapsid = "WsConsignor_" + string.Join("", this.EnterpriseID, ID).MD5();
    //        //删除收益人与供应商的关系
    //        this.Reponsitory.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == mapsid);
    //        return true;
    //    }

    //    /// <summary>
    //    /// 设置默认
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool SetDefault(string ID)
    //    {
    //        var mapsid = "WsConsignor_" + string.Join("", this.EnterpriseID, ID).MD5();
    //        Reponsitory.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
    //        {
    //            IsDefault = false
    //        }, item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)MapsType.Consignor && item.Bussiness == (int)Business.WarehouseServicing);

    //        Reponsitory.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
    //        {
    //            IsDefault = true,
    //        }, item => item.ID == mapsid);
    //        return true;
    //    }
    //}
}
