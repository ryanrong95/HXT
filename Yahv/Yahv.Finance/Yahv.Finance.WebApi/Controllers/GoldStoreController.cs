using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Finance.WebApi.Filter;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 金库
    /// </summary>
    public class GoldStoreController : ClientController
    {
        /// <summary>
        /// 金库
        /// </summary>
        /// <param name="sender">调用方</param>
        /// <param name="option">操作</param>
        /// <param name="model">Json实体</param>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult Enter(InputParam<GoldStoreInputDto> param)
        {
            var result = new JMessage() { success = true, code = 200, data = "操作成功!" };
            GoldStoreInputDto input = new GoldStoreInputDto();

            try
            {
                using (var view = new GoldStoresRoll())
                {
                    input = param.Model;
                    var goldStore = view.FirstOrDefault(item => item.Name == input.Name);

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        if (goldStore != null && !string.IsNullOrWhiteSpace(goldStore.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"新增失败! [{input.Name}]已存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            new GoldStore()
                            {
                                CreatorID = input.CreatorID,
                                Name = input.Name,
                                IsSpecial = false,
                                ModifierID = input.CreatorID,
                                OwnerID = input.OwnerID,
                                Summary = input.Summary,
                            }.Enter();
                        }
                    }
                    else if (param.Option.ToLower() == OptionConsts.update)
                    {
                        goldStore = view.FirstOrDefault(item => item.Name == input.OriginName);

                        if (goldStore == null || string.IsNullOrWhiteSpace(goldStore.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"修改失败，[{input.OriginName}]不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            goldStore.Name = input.Name;
                            goldStore.ModifierID = input.CreatorID;
                            goldStore.Summary = input.Summary;
                            goldStore.OwnerID = input.OwnerID;
                            goldStore.Enter();
                        }
                    }
                    else if (param.Option.ToLower() == OptionConsts.delete)
                    {
                        if (goldStore == null || string.IsNullOrWhiteSpace(goldStore.ID))
                        {
                            return Json(new JMessage()
                            {
                                data = $"删除失败，[{input.Name}]不存在!",
                                success = false,
                                code = 500,
                            });
                        }
                        else
                        {
                            goldStore.Disable();
                        }
                    }
                    else
                    {
                        result = new JMessage()
                        {
                            data = $"操作失败,不支持该操作!",
                            success = false,
                            code = 500,
                        };
                    }
                }

                //Services.Oplogs.Oplog(input.CreatorID, LogModular.金库管理_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                return Json(result);
            }
            catch (Exception ex)
            {
                result = new JMessage()
                {
                    data = $"操作异常!{ex.Message}",
                    success = false,
                    code = 500,
                };
                //Services.Oplogs.Oplog(input.CreatorID, LogModular.金库管理_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                return Json(result);
            }
        }
    }
}