using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 自动交接接口
    /// </summary>
    public class HandOverController : ApiController
    {
        /// <summary>
        /// 将请假人员的客户的跟单，设置为交接人员
        /// </summary>
        /// <param name="para">参数</param>      
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage HandOver(HandOverPostPara para)
        {
            var adminView = new Needs.Ccs.Services.Views.Origins.PvbErmOrigin();
            var adminLeave = adminView.Where(item => item.OriginID == para.AdminLeave).FirstOrDefault();
            var adminWork = adminView.Where(item => item.OriginID == para.AdminWork).FirstOrDefault();

            if (adminLeave == null || adminWork == null)
            {
                var errorjson = new JMessage()
                {
                    code = 100,
                    success = false,
                    data = "没有匹配到芯达通员工"
                };
                return ApiResultModel.OutputResult(errorjson);
            }

            var handOverView = new Needs.Ccs.Services.Views.HandOverView().Where(t => t.AdminLeave == para.AdminLeave && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();
            if (handOverView.Count() != 0)
            {
                var errorjson = new JMessage()
                {
                    code = 300,
                    success = false,
                    data = "该员工已请假,请勿重复提交!"
                };
                return ApiResultModel.OutputResult(errorjson);
            }

            try
            {
                List<string> clientsIDs = new List<string>();
                //修改跟单员，记录ClientLog
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().
                                Where(item => item.AdminID == adminLeave.OriginID &&
                                item.Status == (int)Needs.Ccs.Services.Enums.Status.Normal &&
                                item.Type == (int)Needs.Ccs.Services.Enums.ClientAdminType.Merchandiser
                                );
                    clientsIDs = clients.Select(t => t.ClientID).Distinct().ToList();

                    if (clientsIDs.Count() == 0)
                    {
                        var errorjson = new JMessage()
                        {
                            code = 300,
                            success = false,
                            data = "该员工无负责客户!"
                        };
                        return ApiResultModel.OutputResult(errorjson);
                    }

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientAdmins>(new
                    {
                        AdminID = adminWork.OriginID 
                    },
                    item => item.AdminID == adminLeave.OriginID &&
                    item.Status == (int)Needs.Ccs.Services.Enums.Status.Normal &&
                    item.Type == (int)Needs.Ccs.Services.Enums.ClientAdminType.Merchandiser);

                    foreach (var clientID in clientsIDs)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientLogs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            AdminID = Icgoo.DefaultCreator,
                            ClientID = clientID,
                            CreateDate = DateTime.Now,
                            Summary = "跟单员【" + adminLeave.RealName + "】请假,跟单员变更为【" + adminWork.RealName + "】"
                        });
                    }

                    reponsitory.Submit();
                }

                //记录HandOver
                foreach (var clientID in clientsIDs)
                {
                    HandOver handOver = new HandOver();
                    handOver.ID = ChainsGuid.NewGuidUp();
                    handOver.ClientID = clientID;
                    handOver.AdminLeave = adminLeave.OriginID;
                    handOver.AdminWork = adminWork.OriginID;
                    handOver.ApplyID = para.ApplyID;
                    handOver.Enter();
                }


                //返回归类信息
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "交接成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {
                var errorjson = new JMessage()
                {
                    code = 300,
                    success = false,
                    data = ex.ToString()
                };
                return ApiResultModel.OutputResult(errorjson);
            }

        }

        /// <summary>
        /// 将请假人员的客户的跟单，设置回为请假人员
        /// </summary>
        /// <param name="para">参数</param>     
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage HandOverReturn(HandOverPostPara para)
        {
            var adminView = new Needs.Ccs.Services.Views.Origins.PvbErmOrigin();
            var adminLeave = adminView.Where(item => item.OriginID == para.AdminLeave).FirstOrDefault();
            var adminWork = adminView.Where(item => item.OriginID == para.AdminWork).FirstOrDefault();

            if (adminLeave == null || adminWork == null)
            {
                var errorjson = new JMessage()
                {
                    code = 100,
                    success = false,
                    data = "没有匹配到芯达通员工"
                };
                return ApiResultModel.OutputResult(errorjson);
            }
            
            var handOverView = new Needs.Ccs.Services.Views.HandOverView().Where(t => t.ApplyID == para.ApplyID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();

            if (handOverView.Count() == 0)
            {
                var errorjson = new JMessage()
                {
                    code = 300,
                    success = false,
                    data = "没有查询到请假记录"
                };
                return ApiResultModel.OutputResult(errorjson);
            }

            Dictionary<string, string> clientAdminWorkMap = new Dictionary<string, string>();
            foreach (var item in handOverView)
            {
                if (!clientAdminWorkMap.ContainsKey(item.ClientID))
                {
                    clientAdminWorkMap.Add(item.ClientID, item.AdminWork);
                }                
            }

            try
            {
                var handOverIDs = handOverView.Select(t => t.ID).ToList();
                var clientIDs = handOverView.Select(t => t.ClientID).Distinct().ToList();
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.HandOverRecord>(new
                    {
                        Status = (int)Needs.Ccs.Services.Enums.Status.Delete,
                        UpdateDate = DateTime.Now
                    },
                    item => handOverIDs.Contains(item.ID));

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientAdmins>(new
                    {
                        AdminID = adminLeave.OriginID
                    },
                    item => clientIDs.Contains(item.ClientID) &&
                    item.Status == (int)Needs.Ccs.Services.Enums.Status.Normal &&
                    item.Type == (int)Needs.Ccs.Services.Enums.ClientAdminType.Merchandiser);

                    foreach (var clientID in clientIDs)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientLogs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            AdminID = Icgoo.DefaultCreator,
                            ClientID = clientID,
                            CreateDate = DateTime.Now,
                            Summary = "跟单员【" + adminLeave.RealName + "】回来上班,跟单员变更为【" + adminLeave.RealName + "】"
                        });
                    }

                    reponsitory.Submit();
                }

                //删除多重请假的HandOverRecord 记录
                var handOverRecord = new Needs.Ccs.Services.Views.HandOverView();
                string takeOverAdminID = "";
                foreach (var client in clientIDs)
                {
                    takeOverAdminID = clientAdminWorkMap[client];
                    var clientRecord = handOverRecord.Where(t => t.ClientID == client && t.Status == Needs.Ccs.Services.Enums.Status.Normal).OrderBy(t => t.CreateDate);
                    foreach(var t in clientRecord)
                    {
                        if(t.AdminLeave == takeOverAdminID)
                        {
                            var handOver = handOverRecord[t.ID];
                            handOver.UpdateDate = DateTime.Now;
                            handOver.Delete();
                            takeOverAdminID = t.AdminWork;
                        }
                        else
                        {
                            break;
                        }
                    }

                }

                //返回归类信息
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {
                var errorjson = new JMessage()
                {
                    code = 300,
                    success = false,
                    data = ex.ToString()
                };
                return ApiResultModel.OutputResult(errorjson);
            }
        }

    }
}
