using Layer.Data.Sqls;
using Newtonsoft.Json;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 登录Token
        /// </summary>
        public string Token
        {
            get
            {
                return this.Admin?.Token;
            }
        }

        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsAgree
        {
            get
            {
                if(this.Admin == null)
                {
                    return false;
                }
                else
                {
                    return this.Admin.IsAgree;
                }
            }
        }

        /// <summary>
        /// 管理员信息
        /// </summary>
        public AdminTop Admin
        {
            get
            {
                return GetAdmin();
            }
        }

        /// <summary>
        /// 数据转为Json格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ActionResult Result(object obj)
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(obj)
            };
        }

        /// <summary>
        /// 根据token获取当前管理员数据
        /// </summary>
        /// <returns></returns>
        private AdminTop GetAdmin()
        {
            var token = Request.Cookies["Token"].Value;
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var admin = new AdminTopView().SingleOrDefault(item => item.Token == token);
                return admin;
            }
        }
    }
}