using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Views;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Erm.WebApi.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Erm.WebApi.Controllers
{
    /// <summary>
    /// 员工
    /// </summary>
    public class StaffController : ClientController
    {
        #region 获取员工信息
        /// <summary>
        /// 添加员工信息
        /// </summary>
        /// <param name="code">大赢家编码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetOrInsertStaff(int code, string callback = "")
        {
            JMessage json = new JMessage() { success = false };
            string resultJson = string.Empty;

            try
            {
                if (code <= 0)
                {
                    json.IsFailed("大赢家编码不能为空!");
                    return this.Jsonp(json, callback);
                }

                using (var adminsView = new AdminsAll())
                {
                    var admin = adminsView.FirstOrDefault(item => item.StaffDyjCode == code.ToString());
                    if (admin != null && !string.IsNullOrEmpty(admin.ID))
                    {
                        json.IsSuccess(admin.ID);
                        return this.Jsonp(json, callback);
                    }

                    var api = DyjApi.Contact.GetApiAddress();
                    resultJson = Yahv.Utils.Http.ApiHelper.Current.JPost(api, new DyjInputDto()
                    {
                        Token = DyjApi.Token.GetHostName(),
                        Requestitem = "Contact",
                        Data = new
                        {
                            ID = code
                        }
                    });

                    var result = resultJson.JsonTo<Yahv.Erm.WebApi.Models.DyjResultModel<StaffContactDto>>();

                    if (result.Data.Length > 0)
                    {
                        json.IsSuccess(result.Data[0].Enter());
                    }
                    else
                    {
                        json.IsFailed("大赢家数据返回为空!");
                        return this.Jsonp(json, callback);
                    }
                }

                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.ToString(),
                    nameof(Yahv.Systematic.CrmPlus), "获取员工信息", $"大赢家ID [{code}]", $"{resultJson}");
                return this.Jsonp(json, callback);
            }
            catch (System.Exception ex)
            {
                json.success = false;
                json.code = 500;
                json.data = ex.Message;

                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.ToString(),
                    nameof(Yahv.Systematic.CrmPlus), "获取员工信息", $"{new { code, resultJson }}", $"{ex.Message}");

                return this.Jsonp(json, callback);
            }
        }
        #endregion

        #region 同步大赢家员工
        /// <summary>
        /// 同步大赢家员工
        /// </summary>
        /// <param name="count">同步员工个数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SyncStaffsFromDyj(int count, int? dyjId = null) 
        {
            JMessage json = new JMessage() { success = true, code = 200 };
            string resultJson = string.Empty;

            try
            {
                var api = DyjApi.Contact.GetApiAddress();

                object param = new
                {
                    TopNum = count
                };

                if (dyjId > 0)
                {
                    param = new
                    {
                        ID = dyjId
                    };
                }

                resultJson = Yahv.Utils.Http.ApiHelper.Current.JPost(api, new DyjInputDto()
                {
                    Token = DyjApi.Token.GetHostName(),
                    Requestitem = "Contact",
                    Data = param
                });

                //获取大赢家接口数据
                var result = resultJson.JsonTo<Yahv.Erm.WebApi.Models.DyjResultModel<StaffContactDto>>();
                var data = new List<StaffContactDto>();     //比对后，需要同步的员工

                if (result.Data.Length > 0)
                {
                    //大赢家数据ids
                    var dyjIds = result.Data.Select(item => item.ID.ToString()).ToArray();
                    //检查没有同步的员工
                    using (var staffView = new StaffsRoll())
                    {
                        //需要同步的大赢家Ids
                        var ids = staffView.GetDyjIdsNotExist(dyjIds);
                        data = result.Data.Where(item => ids.Contains(item.ID.ToString())).ToList();
                        new StaffContactDto().Batch(data);      //批量同步员工
                        staffView.UpdateDyjCodeToXdt(result.Data);      //更新芯达通公司员工的大赢家ID
                    }
                }

                json.data = data.Json();
                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.ToString(),
                    nameof(Yahv.Systematic.CrmPlus), "同步员工信息成功", $"{data.Json()}", $"{resultJson}");
            }
            catch (Exception ex)
            {
                json.success = false;
                json.code = 500;
                json.data = ex.ToString();

                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.ToString(),
                    nameof(Yahv.Systematic.CrmPlus), "同步员工信息异常", $"{count}", $"{new { dyj = resultJson, error = ex.ToString() }}");

                return this.Json(json);
            }

            return this.Json(json);
        }
        #endregion
    }
}