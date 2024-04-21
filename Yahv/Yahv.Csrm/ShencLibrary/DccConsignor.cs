using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace ShencLibrary
{
    /// <summary>
    /// 客户的收件地址
    /// </summary>
    public class SyncConsignor
    {
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        //public string DyjCode { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        ///  国家/地区：Underly.Origin的Code
        /// </summary>
        public string Place { set; get; }
    }

    public class DccConsignor
    {
        public DccConsignor()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="entity"></param>
        /// <param name="nconsignorid">提货地址ID：修改时要赋值</param>
        public string Enter(string clientid, string supplierid, SyncConsignor entity, string nconsignorid = null)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                var supplier = client.nSuppliers[supplierid];

                nConsignor data;
                (data = new nConsignor
                {
                    ID = nconsignorid,
                    Title = "",
                    nSupplierID = supplierid,
                    EnterpriseID = clientid,
                    RealID = supplier.RealID,
                    Address = entity.Address,
                    Postzip = entity.Postzip,
                    Contact = entity.Name,
                    Tel = entity.Tel,
                    Mobile = entity.Mobile,
                    Email = entity.Email,
                    IsDefault = entity.IsDefault,
                    Creator = "",
                    Place = (supplier.RealEnterprise != null && !string.IsNullOrEmpty(supplier.RealEnterprise.Place))
                                ? supplier.RealEnterprise.Place : Origin.HKG.GetOrigin().Code, //entity.Place
                }).Enter();
                this.Synchro(data); //向芯达通同步
                return data.ID;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="entity"></param>
        public void Abandon(string clientid, string nsupplierid, string nconsnorid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var supplier = client.nSuppliers[nsupplierid];
                var entity = supplier.nConsignors[nconsnorid];
                entity.Abandon();
                this.AbandonSynchro(client.Enterprise.Name, supplier.Enterprise.Name, entity.Address);//向芯达通同步
            }
        }
        public void SetDefault(string clientid, string nsupplierid, string nconsnorid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var supplier = client.nSuppliers[nsupplierid];
                var entity = supplier.nConsignors[nconsnorid];
                entity.IsDefault = true;
                entity.Enter();
                Synchro(entity);//向芯达通同步
            }
        }
        public void Synchro(nConsignor data)
        {
            string url = Commons.UnifyApiUrl + "/suppliers/address";
            Enterprise client = new EnterprisesRoll()[data.EnterpriseID];
            Enterprise supplier = new EnterprisesRoll()[data.RealID];
            Commons.HttpPostRaw(url, new
            {
                Enterprise = client,
                SupplierName = supplier.Name,
                Address = data.Address.Replace("中国 ", ""),
                Name = data.Contact,
                Tel = data.Tel,
                Mobile = data.Mobile,
                Place = data.Place,
                IsDefault = data.IsDefault
            }.Json());
        }
        public void AbandonSynchro(string clientname, string suppliername, string address)
        {
            string url = Commons.UnifyApiUrl + "/suppliers/address";
            Commons.CommonHttpRequest(url + "?name=" + clientname + "&supplierName=" + suppliername + "&address=" + address, "DELETE");
        }
    }

}
