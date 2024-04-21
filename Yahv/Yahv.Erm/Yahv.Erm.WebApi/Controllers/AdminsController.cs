using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Erm.WebApi.Models;
using Yahv.Models;
using Yahv.Plats.Services;
using Yahv.Plats.Services.Views.Origins;
using Yahv.Services.Json;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Erm.WebApi.Controllers
{
    /// <summary>
    /// 管理员
    /// </summary>
    /// <remarks>
    /// 目前代仓储专用
    /// </remarks>
    public class AdminsController : ClientController
    {
        public ActionResult Index()
        {
            return Content(1 + "1");
        }

        #region HttpGet
        [HttpGet]
        public ActionResult Health()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取被指派采购员集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BeOffers(string callback)
        {
            var admins = Yahv.Erp.Choice[FixedRole.PM, FixedRole.PMa, FixedRole.Purchaser];

            var result = new JList<object>
            {
                success = true,
                code = 200,
                data = admins.Select(item => new
                {
                    item.ID,
                    item.RealName,
                    item.UserName
                })
            };

            return this.Jsonp(result, callback);
        }

        /// <summary>
        /// 根据token返回用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Token(string token)
        {
            AdminJson admin = new AdminJson();
            try
            {
                admin = Erp.Token(token);

                return this.Json(new
                {
                    admin.ID,
                    admin.Role,
                    admin.RealName,
                    admin.UserName,
                    admin.Status
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(new
                {
                    admin.ID,
                    admin.Role,
                    admin.RealName,
                    admin.UserName,
                    admin.Status
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="business">业务名称</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Roles(string business = null)
        {
            var view = new MyRingRole(Erp.Current);

            var json = new JSingle<dynamic>() { code = 200, success = true, data = view[business] };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 库房权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WareHouseRoles()
        {
            var json = new JSingle<dynamic>()
            {
                code = 200,
                success = true,
                data = Erp.Current.Erm.MyWareHouses.Where(item => item.ID != item.ParentID).OrderBy(item => item.Code).Select(item => new
                {
                    value = item.ID,
                    //item.AdminID,
                    code = item.Code,
                    label = item.Name,
                    //item.ParentName,
                    //item.ParentID,
                }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 库房权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WareHouseTreeRoles()
        {
            var list = Erp.Current.Erm.MyWareHouses;

            var json = new JSingle<dynamic>()
            {
                code = 200,
                success = true,
                data = from p in list
                       group p by new { p.ParentID, p.ParentName, p.ParentCode } into p
                       select new
                       {
                           value = p.Key.ParentID,
                           label = p.Key.ParentName,
                           code = p.Key.ParentCode,
                           children = from c in list
                                      where c.ParentID == p.Key.ParentID && c.ID != p.Key.ParentID
                                      select new
                                      {
                                          value = c.ID,
                                          label = c.Name,
                                          code = c.Code,
                                      }

                       }
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据Role获取管理员列表
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="isCompose">是否包含兼容角色，默认包含</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetByRoleID(string roleID, bool isCompose = true)
        {
            var list = new Yahv.Erm.Services.Views.AdminsAll().GetByRoleID(roleID, isCompose);

            var json = new JList<object>
            {
                success = true,
                code = 200,
                data = list.Select(item => new
                {
                    item.ID,
                    item.RoleID,
                    item.RoleName,
                    item.RealName,
                    item.UserName
                })
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region HttpPost
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginViewModel loginUser)
        {
            var user = new LoginUser()
            {
                UserName = loginUser.UserName,
                Password = loginUser.Password,
            };

            user.LoginFailed += User_LoginFailed;

            if (true) // 找到一种手段判断当前是什么业务？如果是代仓储业务只允许：超级管理、芯达通（深圳、香港）库房人员登录
            {
                user.LoginSuccess += User_LoginSuccess_RoleChcek;
            }

            user.LoginSuccess += User_LoginSuccess;



            user.Login();

            return eJson();
        }

        private void User_LoginSuccess_RoleChcek(object sender, Usually.SuccessEventArgs e)
        {
            //
        }

        private void User_LoginFailed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            JsonResult(new JMessage()
            {
                code = 500,
                success = false,
                data = null
            });
        }
        private void User_LoginSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            var admin = sender as Yahv.Plats.Services.Models.AdminRoll;
            JsonResult(new
            {
                code = 200,
                success = true,
                data = new
                {
                    admin.ID,
                    admin.Role,
                    admin.RealName,
                    admin.UserName,
                    admin.Status,
                }
            });
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyPassword(string password)
        {
            var erpAdmin = Erp.Current;
            var admin = new Admin()
            {
                ID = erpAdmin.ID,
                RoleID = erpAdmin.Role.ID,
                Password = password
            };

            admin.EnterSuccess += UserPassword_Success;
            admin.Enter();

            return eJson();
        }

        private void UserPassword_Success(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            eJson(new JSingle<dynamic>() { data = sender, code = 200, success = true });
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="code">大赢家Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginFromDyj(string code)
        {
            var json = new JMessage()
            {
                success = true,
                code = 200,
            };

            try
            {
                var data = ZEncyptDES.DESDecrypt(code);

                if (!new LoginUser().IsExist(data))
                {
                    json.success = false;
                    json.code = 500;
                    json.data = $"[{data}]未找到该员工信息!";
                }
                else
                {
                    json.data = $"{ConfigurationManager.AppSettings["ErpHome"]}?code={code.Base64Encode()}";
                }

                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.Host.ToString(),
                    nameof(Yahv.Systematic.Erm), "大赢家登录检查", $"大赢家ID [{data}]", $"{code}");
            }
            catch (Exception ex)
            {
                json.success = false;
                json.code = 500;
                json.data = ex.Message;

                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.Host.ToString(),
                    nameof(Yahv.Systematic.Erm), "大赢家登录检查", $"编码 [{code}]", $"{ex.Message}");

                return Json(json);
            }

            return Json(json);
        }

#if DEBUG
        [HttpGet]
        public ActionResult DESEncrypt(string code)
        {
            return Json(ZEncyptDES.DESEncrypt(code), JsonRequestBehavior.AllowGet);
        }
#endif
        #endregion
    }
}