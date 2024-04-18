using Needs.Ccs.Services.Models;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class NoticeController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mianID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpResponseMessage SendNotice(string mianID, Needs.Ccs.Services.Enums.SendNoticeType type )
        {
            try
            {
                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = mianID;
                noticeLog.NoticeType = type;
                noticeLog.Readed = true;
                noticeLog.SendNotice();

                //返回归类信息
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch(Exception ex)
            {
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }
            
        }

    }
}