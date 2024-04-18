using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Extends
{
    /// <summary>
    /// Crm接口拓展方法
    /// </summary>
    public static class CrmNoticeExtends
    {
        /// <summary>
        /// Crm客户信息对接接口
        /// </summary>
        /// <param name="Json">客户信息</param>
        /// <returns></returns>
        public static JMessage ClientInfo(string Json)
        {
            //调用芯达通接口，传递订单数据
            var apisetting = new PvCrmApiSetting();
            var apiurl = apisetting.ApiUrl + apisetting.ClientInfo;
            var result = NoticeExtends.HttpPostRaw(apiurl, Json);
            var message = result.JsonTo<JMessage>();
            return message;
        }

        /// <summary>
        /// Crm对接注册后企业名称修改
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static JMessage ClientNameModify(string oldName, string newName)
        {
            //调用芯达通接口，传递企业名称
            var apisetting = new PvCrmApiSetting();
            var apiurl = apisetting.ApiUrl + apisetting.ClientNameModify;
            var result = NoticeExtends.HttpPostRaw(apiurl, string.Format("OldName={0}&NewName={1}", oldName, newName), "application/x-www-form-urlencoded");
            var message = result.JsonTo<JMessage>();
            return message;
        }

        /// <summary>
        /// 根据关键字模糊查询
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static JMessage GetEnterpriseByKeyword(string keyword, string userID)
        {
            var apisetting = new PvCrmApiSetting();
            var apiurl = apisetting.ApiUrl + apisetting.GetEnterpriseByKeyword;
            var result = NoticeExtends.HttpPostRaw(apiurl, string.Format("keyword={0}&siteuser={1}", keyword, userID), "application/x-www-form-urlencoded");
            var message = result.JsonTo<JMessage>();
            return message;
        }


        public static JMessage GetEnterpriseByName(string key, string userID)
        {
            var apisetting = new PvCrmApiSetting();
            var apiurl = apisetting.ApiUrl + apisetting.GetEnterpriseByName;
            var result = NoticeExtends.HttpPostRaw(apiurl, string.Format("key={0}&siteuserID={1}", key, userID), "application/x-www-form-urlencoded");
            var message = result.JsonTo<JMessage>();
            return message;
        }

    }
}
