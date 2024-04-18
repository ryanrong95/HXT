using Layer.Data.Sqls;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Newtonsoft.Json;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class ApiAdminController : BaseController
    {
        /// <summary>
        /// 绑定微信号
        /// </summary>
        /// <param name="WXID">微信号</param>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Bind(string WXID,string UserName,string Password)
        {
            //校验微信ID不能为空
            if(string.IsNullOrWhiteSpace(WXID))
            {
                return Result(new { IsSuccess = false, Message = "微信ID不能为空!" });
            }

            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var password = Password.PasswordOld();
                var admin = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>().Where(item => item.UserName == UserName && item.Password == password).SingleOrDefault();
                if (admin == null)
                {
                    return Result(new { IsSuccess = false, Message = "用户名或者密码错误！" });
                }
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminsProject>().Count(item => item.AdminID == admin.ID);
                if (count == 0)
                {

                    return Result(new { IsSuccess = false, Message = "请在Crm中设置该人员！" });
                }
                reponsitory.Update<Layer.Data.Sqls.BvCrm.AdminsProject>(new
                {
                    WXID = WXID,
                    Token = Guid.NewGuid().ToString(),
                }, item => item.AdminID == admin.ID);
                return Result(new { IsSuccess = true, Message = "绑定成功" });
            }
        }

        /// <summary>
        /// 获取管理员
        /// </summary>
        /// <param name="WXID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(string WXID)
        {
            //校验微信ID不能为空
            if (string.IsNullOrWhiteSpace(WXID))
            {
                return Result( new { result = false } );
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var admin = new AdminTopView().Where(item => item.WXID == WXID).SingleOrDefault();

                if(admin == null)
                {
                    return Result(new { result = false });
                }
                Response.Cookies["Token"].Value = admin.Token;

                //设置Token
                return Result(new
                {
                    admin.ID,
                    admin.UserName,
                    admin.RealName,
                    JobType = admin.JobType.GetDescription(),
                    admin.CreateDate,
                    admin.IsAgree,
                });
            }
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnBind()
        {
            if (!IsAgree)
            {
                return Result(new { IsSuccess = false, Message = "没有授权！" });
            }
            using (BvCrmReponsitory responsitory = new BvCrmReponsitory())
            {
                responsitory.Update<Layer.Data.Sqls.BvCrm.AdminsProject>(
                    new {
                        WXID = string.Empty,
                        IsAgree = false,
                        Token = string.Empty,
                    }, item => item.AdminID == this.Admin.ID);
                return Result(new { IsSuccess = true, Message = "解绑成功" });
            }
        }
    }
}