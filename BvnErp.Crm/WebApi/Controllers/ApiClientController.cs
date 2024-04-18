using Layer.Data.Sqls;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class ApiClientController : BaseController
    {
        //获取当前管理员的用户
        [HttpGet]
        public ActionResult Get()
        {
            if(!IsAgree)
            {
                return Result(new { result = false });
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var clients = new MyClientBaseView(this.Admin, reponsitory).Where(item => item.Status == NtErp.Crm.Services.Enums.ActionStatus.Complete
                    || item.Status == NtErp.Crm.Services.Enums.ActionStatus.Auditing);
                var data = clients.Select(item => new
                {
                    item.ID,
                    item.Name,
                }).ToArray();
                return Result(data);
            }
        }

        /// <summary>
        /// 根据大赢家ID获取对应的客户
        /// </summary>
        /// <param name="dyjid">大赢家ID</param>
        /// <param name="pageindex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClients(string dyjid,int pageindex,string ClientName)
        {
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                var admin = new AdminTopView().Where(item => item.DyjID == dyjid).SingleOrDefault();
                var pageinfo = new { counts = 0, pagesize = 50, pages = 0, currentpage = 0 };
                if (admin == null)
                {
                    return Result(new { Status = 400, Message = "没有用户绑定此大赢家ID！", dyjid, pageinfo, list = new { } });
                }
                var clients = new MyClientBaseView(admin, reponsitory).Where(item => item.Status == NtErp.Crm.Services.Enums.ActionStatus.Complete);
                //客户名称过滤
                if(!string.IsNullOrWhiteSpace(ClientName))
                {
                    clients = clients.Where(item => item.Name.Contains(ClientName));
                }
                if(clients.Count() > 0)
                {
                    //校验页码是否超出范围
                    if(pageindex > clients.Count() / 50 + 1)
                    {
                        pageindex = 1;
                    }
                    pageinfo = new { counts = clients.Count(), pagesize = 50, pages = clients.Count() / 50 + 1, currentpage = pageindex };
                }
                var data = clients.Select(item => new
                {
                    item.ID,
                    item.Name,
                }).Skip((pageindex - 1) * 50).Take(50).ToArray();
                return Result(new { Status = 200, Message = "成功！", dyjid, pageinfo, list = data });
            }
        }
    }
}