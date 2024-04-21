using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace ShencLibrary
{
    public class SyncConsignee
    {
        /// <summary>
        /// 收件单位
        /// </summary>
        public string Title { get; set; }
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
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 国家/地区：Underly.Origin的Code
        /// </summary>
        public string Place { set; get; }
    }
    public class DccConsignee
    {
        public DccConsignee()
        {

        }
        public string Enter(string clientid, SyncConsignee entity, string id = "")
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                if (client.Enterprise.Name.StartsWith("reg-", System.StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                WsConsignee data;
                (data = new WsConsignee
                {
                    ID = id,
                    Title = entity.Title,
                    EnterpriseID = client.ID,
                    Enterprise = client.Enterprise,
                    DyjCode = entity.DyjCode,
                    District = District.CN,
                    Address = entity.Address,
                    Postzip = entity.Postzip,
                    Name = entity.Name,
                    Tel = entity.Tel,
                    Mobile = entity.Mobile,
                    Email = entity.Email,
                    CreatorID = "",
                    Place = entity.Place,
                    IsDefault = entity.IsDefault
                }).Enter();
                this.Synchro(data);//向芯达通同步

                return data.ID;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entity"></param>
        public void Abandon(string clientid, string cosigneid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var entity = client.Consignees[cosigneid];
                entity.Abandon();
                this.AbandonSynchro(client.Enterprise.Name, entity.Address, entity.Mobile);//向芯达通同步
            }
        }

        public void SetDefault(string clientid, string cosigneid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var entity = client.Consignees[cosigneid];
                entity.IsDefault = true;
                entity.Enter();
                Synchro(entity);//向芯达通同步
            }
        }
        public void Synchro(WsConsignee data)
        {
            string url = Commons.UnifyApiUrl + "/clients/consignee";
            Commons.HttpPostRaw(url, new
            {
                Enterprise = data.Enterprise,
                Receiver = data.Title,
                Name = data.Name,
                Mobile = data.Tel,
                Address = data.Address,
                IsDefault = data.IsDefault,
                Email = data.Email,
                Summary = "",
                Place = data.Place
            }.Json());

        }
        public void AbandonSynchro(string clientname, string address, string mobile)
        {
            string url = Commons.UnifyApiUrl + "/clients/consignee";
            Commons.CommonHttpRequest(url + "?name=" + clientname + "&receiver=" + clientname + "&address=" + address + "&mobile=" + mobile, "DELETE");
        }
    }
}
