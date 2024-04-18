using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PvData.WebApi.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.PvData.Services;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Interfaces;
using YaHv.PvData.Services.Models;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApi.Controllers
{
    public class PreClassifyController : ClientController
    {
        #region 提交归类

        /// <summary>
        /// 提交归类信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitClassified(ClassifiedResult result)
        {
            try
            {
                var step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), result.Step);
                var classifyProduct = GetPreProduct(result);
                var classify = ClassifyFactory.Create(step, classifyProduct);
                classify.DoClassify();

                //返回自动归类信息
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        ProductID = classifyProduct.ProductID,
                        HSCodeID = classifyProduct.CpnID,
                        FVARate = ConstConfig.FVARate,
                    }
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<dynamic>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 一键归类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult QuickClassify(string results)
        {
            try
            {
                results = results.FixSpecialChars();
                ClassifiedResult[] crArr = results.JsonTo<ClassifiedResult[]>();
                IClassifyProduct classifyProduct;
                Classify classify;

                System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();
                watcher.Start();

                foreach (var cr in crArr)
                {
                    var step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), cr.Step);
                    classifyProduct = GetPreProduct(cr);
                    classify = ClassifyFactory.Create(step, classifyProduct);
                    classify.DoClassify();
                }

                watcher.Stop();
                TimeSpan span = watcher.Elapsed;

                var json = new JMessage() { code = 200, success = true, data = "归类成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private PreProduct GetPreProduct(ClassifiedResult result)
        {
            var step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), result.Step);
            var role = (DeclarantRole)Enum.Parse(typeof(DeclarantRole), result.Role);

            return new PreProduct()
            {
                ID = result.MainID,
                ClientName = result.ClientName,
                ClientCode = result.ClientCode,
                CallBackUrl = result.CallBackUrl,
                PartNumber = result.PartNumber,
                Manufacturer = result.Manufacturer,

                Currency = result.Currency,
                UnitPrice = result.UnitPrice,
                Quantity = result.Quantity,

                HSCode = result.HSCode,
                TariffName = result.TariffName,
                TaxCode = result.TaxCode,
                TaxName = result.TaxName,
                LegalUnit1 = result.LegalUnit1,
                LegalUnit2 = result.LegalUnit2,
                VATRate = result.VATRate,
                ImportPreferentialTaxRate = result.ImportPreferentialTaxRate,
                OriginATRate = result.OriginRate,
                ExciseTaxRate = result.ExciseTaxRate,
                CIQCode = result.CIQCode,
                Elements = result.Elements,
                Summary = result.Summary,

                Ccc = result.Ccc,
                Embargo = result.Embargo,
                HkControl = result.HkControl,
                Coo = result.Coo,
                CIQ = result.CIQ,
                CIQprice = result.CIQprice,
                IsHighPrice = result.IsHighPrice,
                IsDisinfected = result.IsDisinfected,
                IsSysCcc = result.IsSysCcc,
                IsSysEmbargo = result.IsSysEmbargo,
                IsCustomsInspection = result.IsCustomsInspection,

                CreatorID = result.CreatorID,
                CreatorName = result.CreatorName,
                Step = step,
                Role = role
            };
        }

        #endregion

        #region 锁定、解锁

        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="mainId">预归类产品ID</param>
        /// <param name="creatorId">操作人ID</param>
        /// <param name="creatorName">操作人真实姓名</param>
        /// <param name="step">归类阶段</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Lock(string mainId, string creatorId, string creatorName, string step)
        {
            try
            {
                var lock_Classify = new YaHv.PvData.Services.Views.Alls.Locks_ClassifyAll().FirstOrDefault(cl => cl.MainID == mainId);
                if (lock_Classify != null && lock_Classify.LockerID != creatorId)
                {
                    throw new Exception("当前产品归类已被【" + lock_Classify.LockerName + "】锁定，锁定时间【" + lock_Classify.LockDate + "】");
                }
                else
                {
                    if (lock_Classify == null)
                    {
                        var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                        var classifyProduct = new PreProduct()
                        {
                            ID = mainId,
                            CreatorID = creatorId,
                            CreatorName = creatorName,
                            Step = stepEnum
                        };

                        var classify = ClassifyFactory.Create(stepEnum, classifyProduct);
                        classify.Lock();
                    }

                    var json = new JMessage()
                    {
                        code = 200,
                        success = true,
                        data = "锁定成功"
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="mainId">预归类产品ID</param>
        /// <param name="creatorId">操作人ID</param>
        /// <param name="creatorName">操作人真实姓名</param>
        /// <param name="step">归类阶段</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnLock(string mainId, string creatorId, string creatorName, string step)
        {
            try
            {
                var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                var classifyProduct = new PreProduct()
                {
                    ID = mainId,
                    CreatorID = creatorId,
                    CreatorName = creatorName,
                    Step = stepEnum
                };

                var classify = ClassifyFactory.Create(stepEnum, classifyProduct);
                classify.UnLock();

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "解锁成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 批量锁定
        /// </summary>
        /// <param name="tobeLockedItems">预归类产品</param>
        /// <param name="creatorId">操作人ID</param>
        /// <param name="creatorName">操作人真实姓名</param>
        /// <param name="step">归类阶段</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchLock(string tobeLockedItems, string creatorId, string creatorName, string step)
        {
            try
            {
                var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                List<PreProduct> preproducts = new List<PreProduct>();

                var tobeLockedProductArr = tobeLockedItems.FixSpecialChars().JsonTo<TobeLockedProduct[]>();
                var preProductIdArr = tobeLockedProductArr.Select(t => t.MainID);
                var locks_Classify = new YaHv.PvData.Services.Views.Alls.Locks_ClassifyAll().Where(cl => preProductIdArr.Contains(cl.MainID)).ToArray();
                bool isNeedRemindLocked = false;
                StringBuilder sbLockedRemind = new StringBuilder();

                foreach (var product in tobeLockedProductArr)
                {
                    var lock_Classify = locks_Classify.FirstOrDefault(cl => cl.MainID == product.MainID);
                    if (lock_Classify != null && lock_Classify.LockerID != creatorId)
                    {
                        isNeedRemindLocked = true;
                        //锁定人不是当前操作人,提示已经被他人锁定
                        sbLockedRemind.Append("【" + product.PartNumber + "】、");
                    }
                    else
                    {
                        if (lock_Classify == null)
                        {
                            preproducts.Add(new PreProduct()
                            {
                                ID = product.MainID,
                                CreatorID = creatorId,
                                CreatorName = creatorName,
                                Step = stepEnum
                            });
                        }
                    }
                }
                preproducts.BatchLock();

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = isNeedRemindLocked ? "以下型号已被他人锁定：\r\n" + sbLockedRemind.ToString().Trim(new char[] { '、' }) : "批量锁定成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 退回

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="creatorId"></param>
        /// <param name="creatorName"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public ActionResult Return(string mainId, string creatorId, string creatorName, string step,string summary)
        {
            try
            {
                var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                var classifyProduct = new PreProduct()
                {
                    ID = mainId,
                    CreatorID = creatorId,
                    CreatorName = creatorName,
                    Step = stepEnum,
                    Summary = summary
                };

                var classify = ClassifyFactory.Create(stepEnum, classifyProduct);
                classify.Return();

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "退回成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 获取归类结果

        /// <summary>
        /// 获取子系统的归类结果
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClassified(string mainId)
        {
            try
            {
                //根据型号、品牌/制造商获取产品最新的归类信息
                var cr = new YaHv.PvData.Services.Views.Alls.SubSystemClassifiedResultsAll().FirstOrDefault(c => c.MainID == mainId);
                if (cr == null)
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        cr.ID,
                        cr.MainID,
                        cr.ItemID,
                        cr.Step,
                        cr.CpnID,
                        cr.ClientName,
                        cr.ClientCode,
                        cr.OrderedDate,
                        cr.PIs,
                        cr.CallBackUrl,
                        cr.Unit,
                        cr.CreateDate,
                        cr.ModifyDate,
                        cr.Summary
                    }
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<dynamic>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}