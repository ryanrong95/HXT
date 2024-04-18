using Newtonsoft.Json;
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
    public class ClassifyController : ClientController
    {
        private Encoding defaultEncoding = Utils.Http.ApiHelper.Current.DefaultEncoding;

        // GET: Classify
        public ActionResult Index()
        {
            return View();
        }

        #region 自动归类

        /// <summary>
        /// 获取自动归类信息
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌/制造商</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="isVerifyPriceFluctuation">是否需要验证价格波动</param>
        /// <param name="highPriceLimit">价格高于历史平均价格时的浮动百分比</param>
        /// <param name="lowPriceLimit">价格低于历史平均价格时的浮动百分比</param>
        /// <param name="origin">产地</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAutoClassified(string partNumber, string manufacturer, decimal unitPrice,
                                              bool isVerifyPriceFluctuation = true, decimal highPriceLimit = 0.8m, decimal lowPriceLimit = 0.3m, 
                                              string origin = null)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                manufacturer = manufacturer.FixSpecialChars();

                //根据型号、品牌/制造商获取产品最近一年的归类信息
                var cpn = new YaHv.PvData.Services.Views.Alls.ClassifiedPartNumbersAll()[partNumber, manufacturer, -12];
                bool hasClassified = true;
                if (cpn == null)
                    hasClassified = false;
                else
                {
                    var tariff = new YaHv.PvData.Services.Views.Alls.TariffsAll()[cpn.HSCode];
                    if (tariff == null)
                        hasClassified = false;
                }

                if (!hasClassified)
                {
                    //没有归类历史数据, 则只返回系统管控信息, 用于(非代报关或转报关)订单的自动归类管控
                    var sysEmbargo = new YaHv.PvData.Services.Views.Alls.ProductControlsAll()[partNumber, ControlType.Embargo];
                    bool isSysEmbargo = sysEmbargo == null ? false : true;

                    return Json(new JSingle<dynamic>()
                    {
                        code = 100,
                        success = true,
                        data = new
                        {
                            OriginRate = 0m,
                            FVARate = ConstConfig.FVARate,
                            Ccc = false,
                            Embargo = false,
                            HkControl = false,
                            Coo = false,
                            CIQ = false,
                            CIQprice = 0m,
                            IsHighPrice = false,
                            IsDisinfected = false,
                            IsSysEmbargo = isSysEmbargo,
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                //返回自动归类信息
                var autoClassified = cpn.FillAutoClassified(unitPrice, isVerifyPriceFluctuation, highPriceLimit, lowPriceLimit, origin);
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        autoClassified.AutoHSCodeID,

                        autoClassified.OriginRate,
                        autoClassified.FVARate,
                        autoClassified.Ccc,
                        autoClassified.Embargo,
                        autoClassified.HkControl,
                        autoClassified.Coo,
                        autoClassified.CIQ,
                        autoClassified.CIQprice,
                        autoClassified.IsHighPrice,
                        autoClassified.IsDisinfected,
                        autoClassified.IsSysCcc,
                        autoClassified.IsSysEmbargo,
                        autoClassified.IsSpecialType,
                        autoClassified.IsPriceFluctuation,

                        //后续对接芯达通代报关系统时需要
                        autoClassified.HSCode,
                        autoClassified.TariffName,
                        autoClassified.LegalUnit1,
                        autoClassified.LegalUnit2,
                        autoClassified.VATRate,
                        autoClassified.ImportPreferentialTaxRate,
                        autoClassified.ExciseTaxRate,
                        autoClassified.Elements,
                        autoClassified.CIQCode,
                        autoClassified.TaxCode,
                        autoClassified.TaxName,
                    }
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<dynamic>() { code = 300, success = false, data = $"异常信息：{ex.Message} 堆栈信息：{ex.StackTrace}" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 提交归类

        /// <summary>
        /// 校验归类信息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ValidateClassified(ClassifiedResult result)
        {
            try
            {
                //根据型号、品牌/制造商获取产品最新的归类信息
                var cpn = new YaHv.PvData.Services.Views.Alls.ClassifiedPartNumbersAll()[result.PartNumber, result.Manufacturer];
                if (cpn == null)
                {
                    return Json(new JMessage() { code = 100, success = true, data = "验证通过" }, JsonRequestBehavior.AllowGet);
                }

                StringBuilder msg = new StringBuilder();
                bool isPassed = true;
                var historyClassified = cpn.FillHistoryClassified();

                #region 验证归类结果

                Validate(ref msg, ref isPassed, "品牌", result.Manufacturer, historyClassified.Manufacturer);
                Validate(ref msg, ref isPassed, "海关编码", result.HSCode, historyClassified.HSCode);
                Validate(ref msg, ref isPassed, "报关品名", result.TariffName, historyClassified.TariffName);
                Validate(ref msg, ref isPassed, "税务编码", result.TaxCode, historyClassified.TaxCode);
                Validate(ref msg, ref isPassed, "税务名称", result.TaxName, historyClassified.TaxName);
                Validate(ref msg, ref isPassed, "优惠税率", result.ImportPreferentialTaxRate, historyClassified.ImportPreferentialTaxRate);
                Validate(ref msg, ref isPassed, "增值税率", result.VATRate, historyClassified.VATRate);
                Validate(ref msg, ref isPassed, "消费税率", result.ExciseTaxRate, historyClassified.ExciseTaxRate);
                Validate(ref msg, ref isPassed, "法定第一单位", result.LegalUnit1, historyClassified.LegalUnit1);
                Validate(ref msg, ref isPassed, "法定第二单位", result.LegalUnit2, historyClassified.LegalUnit2);
                Validate(ref msg, ref isPassed, "检验检疫编码", result.CIQCode, historyClassified.CIQCode);
                Validate(ref msg, ref isPassed, "申报要素", result.Elements, historyClassified.Elements);
                Validate(ref msg, ref isPassed, "CCC认证", result.Ccc ? "是" : "否", historyClassified.Ccc ? "是" : "否");
                Validate(ref msg, ref isPassed, "原产地证明", result.Coo ? "是" : "否", historyClassified.Coo ? "是" : "否");
                Validate(ref msg, ref isPassed, "禁运", result.Embargo ? "是" : "否", historyClassified.Embargo ? "是" : "否");
                Validate(ref msg, ref isPassed, "香港管制", result.HkControl ? "是" : "否", historyClassified.HkControl ? "是" : "否");
                Validate(ref msg, ref isPassed, "是否商检", result.CIQ ? "是" : "否", historyClassified.CIQ ? "是" : "否");
                if (result.CIQ)
                {
                    Validate(ref msg, ref isPassed, "商检费", result.CIQprice, historyClassified.CIQprice);
                }

                #endregion

                if (isPassed)
                {
                    return Json(new JMessage() { code = 100, success = true, data = "验证通过" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string warning = "该型号的以下归类结果与历史纪录不一致，请仔细核对！<br/>" +
                        "点击“<label style=\"color:green\">确定</label>”完成归类，点击“<label style=\"color:red\">取消</label>”继续修改<br/><br/>";
                    return Json(new JMessage() { code = 200, success = true, data = warning + msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void Validate(ref StringBuilder msg, ref bool isPassed, string title, object current, object history)
        {
            if (current != null && !current.Equals(history))
            {
                isPassed = false;

                if (history != null && history.GetType() == typeof(decimal))
                    history = ((decimal)history).ToString("#0.0000");
                msg.Append(title + "：当前归类<label style=\"color:green\">【" + current + "】</label>，" +
                           " 历史纪录<label style=\"color:red\">【" + history + "】</label><br/><br/>");
            }
        }

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
                var classifyProduct = GetOrderedProduct(result);
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
                    classifyProduct = GetOrderedProduct(cr);
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

        private OrderedProduct GetOrderedProduct(ClassifiedResult result)
        {
            var step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), result.Step);
            var role = (DeclarantRole)Enum.Parse(typeof(DeclarantRole), result.Role);

            return new OrderedProduct()
            {
                ID = result.ItemID,
                MainID = result.MainID,
                ClientName = result.ClientName,
                ClientCode = result.ClientCode,
                OrderedDate = result.OrderedDate,
                PIs = result.PIs.FixSpecialChars(),
                CallBackUrl = result.CallBackUrl,
                PartNumber = result.PartNumber,
                Manufacturer = result.Manufacturer,

                Origin = result.Origin,
                Currency = result.Currency,
                UnitPrice = result.UnitPrice,
                Quantity = result.Quantity,

                HSCode = result.HSCode,
                CustomName = result.CustomName,
                TariffName = result.TariffName,
                TaxCode = result.TaxCode,
                TaxName = result.TaxName,
                Unit = result.Unit,
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
        /// <param name="itemId">orderItemId</param>
        /// <param name="creatorId">操作人ID</param>
        /// <param name="creatorName">操作人真实姓名</param>
        /// <param name="step">归类阶段</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Lock(string itemId, string creatorId, string creatorName, string step)
        {
            try
            {
                var lock_Classify = new YaHv.PvData.Services.Views.Alls.Locks_ClassifyAll().FirstOrDefault(cl => cl.MainID == itemId);
                if (lock_Classify != null && lock_Classify.LockerID != creatorId)
                {
                    throw new Exception("当前产品归类已被【" + lock_Classify.LockerName + "】锁定，锁定时间【" + lock_Classify.LockDate + "】");
                }
                else
                {
                    if (lock_Classify == null)
                    {
                        var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                        var classifyProduct = new OrderedProduct()
                        {
                            ID = itemId,
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
        /// <param name="itemId">orderItemId</param>
        /// <param name="creatorId">操作人ID</param>
        /// <param name="creatorName">操作人真实姓名</param>
        /// <param name="step">归类阶段</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnLock(string itemId, string creatorId, string creatorName, string step)
        {
            try
            {
                var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                var classifyProduct = new OrderedProduct()
                {
                    ID = itemId,
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
        /// <param name="tobeLockedItems">orderItems</param>
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
                List<OrderedProduct> items = new List<OrderedProduct>();

                var tobeLockedItemArr = tobeLockedItems.FixSpecialChars().JsonTo<TobeLockedProduct[]>();
                var itemIdArr = tobeLockedItemArr.Select(t => t.ItemID);
                var locks_Classify = new YaHv.PvData.Services.Views.Alls.Locks_ClassifyAll().Where(cl => itemIdArr.Contains(cl.MainID)).ToArray();
                bool isNeedRemindLocked = false;
                StringBuilder sbLockedRemind = new StringBuilder();

                foreach (var item in tobeLockedItemArr)
                {
                    var lock_Classify = locks_Classify.FirstOrDefault(cl => cl.MainID == item.ItemID);
                    if (lock_Classify != null && lock_Classify.LockerID != creatorId)
                    {
                        isNeedRemindLocked = true;
                        //锁定人不是当前操作人,提示已经被他人锁定
                        sbLockedRemind.Append("【" + item.PartNumber + "】、");
                    }
                    else
                    {
                        if (lock_Classify == null)
                        {
                            items.Add(new OrderedProduct()
                            {
                                ID = item.ItemID,
                                CreatorID = creatorId,
                                CreatorName = creatorName,
                                Step = stepEnum
                            });
                        }
                    }
                }
                items.BatchLock();

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

        #region 获取归类结果

        /// <summary>
        /// 获取子系统的归类结果
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClassified(string itemId)
        {
            try
            {
                //根据型号、品牌/制造商获取产品最新的归类信息
                var cr = new YaHv.PvData.Services.Views.Alls.SubSystemClassifiedResultsAll().FirstOrDefault(c => c.ItemID == itemId);
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
                        cr.CustomName,
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

        #region 基础数据

        /// <summary>
        /// 根据型号获取系统管控类型(3C、禁运)
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSysControls(string partNumber)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                var sysCcc = new YaHv.PvData.Services.Views.Alls.ProductControlsAll()[partNumber, ControlType.Ccc];
                var sysEmbargo = new YaHv.PvData.Services.Views.Alls.ProductControlsAll()[partNumber, ControlType.Embargo];

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        IsSysCcc = sysCcc == null ? false : true,
                        IsSysEmbargo = sysEmbargo == null ? false : true
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
        /// 根据型号获取系统管控类型(3C、禁运)
        /// </summary>
        /// <param name="partNumbers"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMultiSysControls(List<string> partNumbers)
        {
            try
            {
                var controls = partNumbers.GetMultiSysControls();
                var json = new JSingle<string>()
                {
                    code = 200,
                    success = true,
                    data = controls.Json()
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<string>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 判断该产地是否需要消毒/检疫
        /// </summary>
        /// <param name="origin">原产地</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetOriginDisinfection(string origin)
        {
            try
            {
                var curDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                var disinfected = new YaHv.PvData.Services.Views.Alls.OriginsDisinfectionAll().FirstOrDefault(od => od.Origin == origin &&
                                                                od.StartDate <= curDate && (od.EndDate == null || od.EndDate >= curDate));

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        IsDisinfected = disinfected == null ? false : true
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
        /// 获取原产地附加税率
        /// </summary>
        /// <param name="hsCode"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetOriginATRate(string hsCode, string origin)
        {
            try
            {
                var originRate = new YaHv.PvData.Services.Views.Alls.OriginsATRateAll()[hsCode, origin];

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        originRate = originRate == null ? 0 : originRate.Rate / 100
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
        /// 删除系统管控类型(3C、禁运)
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="controlType">管控类型</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSysControl(string partNumber, int type)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                var sysControl = new ProductControl(partNumber, (ControlType)type);
                sysControl.Delete();

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = "删除成功"
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
        /// 更新Ccc管控历史记录
        /// </summary>
        /// <param name="partNumber">品牌</param>
        /// <param name="manufacturer">型号</param>
        /// <param name="isCcc">是否Ccc</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateCccControl(string partNumber, string manufacturer, bool isCcc)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                var other = new Other()
                {
                    PartNumber = partNumber,
                    Manufacturer = manufacturer,
                    Ccc = isCcc
                };
                other.UpdateCccControl();

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = "更新成功"
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
        /// 更新禁运管控历史记录
        /// </summary>
        /// <param name="partNumber">品牌</param>
        /// <param name="manufacturer">型号</param>
        /// <param name="isEmbargo">是否禁运</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateEmbargoControl(string partNumber, string manufacturer, bool isEmbargo)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                var other = new Other()
                {
                    PartNumber = partNumber,
                    Manufacturer = manufacturer,
                    Embargo = isEmbargo
                };
                other.UpdateEmbargoControl();

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = "更新成功"
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

        #region 归类历史记录查询

        /// <summary>
        /// 根据产品型号查找归类历史纪录
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClassifiedPartNumberLogs(string partNumber)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                //20191113 与黄柏云确认不再校验日期
                //var date = DateTime.Now.AddMonths(-2);
                var linq = new YaHv.PvData.Services.Views.Alls.Logs_ClassifiedPartNumberView(partNumber)
                           .OrderByDescending(item => item.CreateDate).Take(5);
                var list = linq.ToList().Select(item => new
                {
                    item.ID,
                    item.PartNumber,
                    item.HSCode,
                    item.Name,
                    item.Elements,
                    ImportPreferentialTaxRate = item.ImportPreferentialTaxRate,
                    OriginATRate = item.OriginATRate,
                    VATRate = item.VATRate,
                    ExciseTaxRate = item.ExciseTaxRate,
                    item.Currency,
                    item.UnitPrice,
                    CIQprice = item.CIQ ? (decimal?)item.CIQprice : null,
                    item.Quantity,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                    CIQ = item.CIQ ? "是" : "否",
                });

                if (list == null || !list.Any())
                {
                    return Json(new JMessage() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                var json = new
                {
                    code = 200,
                    success = true,
                    data = list,
                    avgUnitPrice = list.Average(t => t.UnitPrice)
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
        /// 根据报关品名查找税务归类纪录
        /// </summary>
        /// <param name="name">报关品名</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClassifiedTaxLogs(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                var view = new YaHv.PvData.Services.Views.Alls.Logs_ClassifiedTaxView(name);
                var linq = from item in view
                           group item by new
                           {
                               item.Name,
                               item.TaxCode,
                               item.TaxName,
                           } into groups
                           select new
                           {
                               groups.Key.Name,
                               groups.Key.TaxCode,
                               groups.Key.TaxName,
                               CreateDate = groups.Max(item => item.CreateDate)
                           };

                var arry = linq.ToArray();
                var rslt = arry.Select(item => new
                {
                    item.Name,
                    item.TaxCode,
                    item.TaxName,
                    OrderIndex = item.Name == name ? 1 : 2,
                    item.CreateDate
                }).OrderBy(item => item.OrderIndex)
                .ThenByDescending(item => item.CreateDate);

                watch.Stop();
                TimeSpan span = watch.Elapsed;

                //20191113 与黄柏云确认不再校验日期
                //var date = DateTime.Now.AddMonths(-2);
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = rslt.Take(5).Select((item, index) => new
                    {
                        ID = index,
                        item.Name,
                        item.TaxCode,
                        item.TaxName,
                    })
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
        /// 根据型号、品牌查找单价历史记录
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUnitPriceLogs(string partNumber, string manufacturer = null)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                if(!string.IsNullOrEmpty(manufacturer))
                    manufacturer = manufacturer.FixSpecialChars();

                //2020-12-04 荣检与魏晓毅确认显示近三年的产品报价历史记录
                //var productQuotes = new YaHv.PvData.Services.Views.Alls.ProductQuotesAll()[partNumber, manufacturer, -36]
                var productQuotes = new YaHv.PvData.Services.Views.Alls.ProductQuotesAll()[partNumber, -36]
                    .OrderByDescending(pq => pq.CreateDate).ToList()
                    .Select(pq => new
                    {
                        pq.ID,
                        pq.UnitPrice,
                        pq.Currency,
                        pq.Quantity,
                        CreateDate = pq.CreateDate.ToString("yyyy-MM-dd HH:mm")
                    });
                if (productQuotes.Count() == 0)
                {
                    //productQuotes = new YaHv.PvData.Services.Views.Alls.Logs_ClassifiedPartNumberAll()[partNumber, manufacturer, -36]
                    productQuotes = new YaHv.PvData.Services.Views.Alls.Logs_ClassifiedPartNumberAll()[partNumber, -36]
                        .OrderByDescending(cpl => cpl.CreateDate).ToList()
                        .Select(cpl => new
                        {
                            cpl.ID,
                            cpl.UnitPrice,
                            cpl.Currency,
                            Quantity = cpl.Quantity.GetValueOrDefault(),
                            CreateDate = cpl.CreateDate.ToString("yyyy-MM-dd HH:mm")
                        });
                    if (productQuotes.Count() == 0)
                    {
                        return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                    }
                }

                var json = new
                {
                    code = 200,
                    success = true,
                    data = productQuotes,
                    avgUnitPrice = productQuotes.Average(t => t.UnitPrice)
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

        #region 海关编码、申报要素查询

        /// <summary>
        /// 根据海关编码查询对应的海关税则
        /// </summary>
        /// <param name="hsCode">海关编码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTariff(string hsCode)
        {
            try
            {
                var tariff = new YaHv.PvData.Services.Views.Alls.TariffsAll()[hsCode];
                if (tariff == null)
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        tariff.ID,
                        tariff.HSCode,
                        tariff.Name,
                        tariff.LegalUnit1,
                        tariff.LegalUnit2,
                        tariff.VATRate,
                        ImportPreferentialTaxRate = tariff.ImportTaxRate,
                        tariff.ExciseTaxRate,
                        tariff.DeclareElements,
                        tariff.CIQCode
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
        /// 海关编码查询
        /// </summary>
        /// <param name="hsCode">用户在文本框中输入的海关编码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMatchedHSCodes(string hsCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hsCode))
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }
                //海关验估编码
                var controls = new YaHv.PvData.Services.Views.Alls.CustomsControlsAll().Where(item => item.Type == CustomsControlType.HSCode)
                    .Select(item => item.HSCode).ToArray();

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new YaHv.PvData.Services.Views.Alls.TariffsAll().Where(item => item.HSCode.StartsWith(hsCode)).Take(10)
                            .ToList().Select(tariff => new
                            {
                                ID = tariff.ID,
                                HSCode = tariff.HSCode,
                                Name = tariff.Name,
                                IsCustomsInspection = controls.Contains(tariff.HSCode)
                            })
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
        /// 获取申报要素格式(用于归类编辑窗口)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetElementsFormat(string hsCode)
        {
            try
            {
                var tariff = new YaHv.PvData.Services.Views.Alls.TariffsAll().FirstOrDefault(item => item.HSCode == hsCode.Trim());
                if (tariff == null)
                {
                    return Json(new JMessage() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                return Json(new JMessage() { code = 200, success = true, data = tariff.DeclareElements }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取申报要素品牌
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetElementsManufaturer(string manufacturer)
        {
            try
            {
                var mfr = new YaHv.PvData.Services.Views.Alls.ElementsManufacturersAll()[manufacturer];
                if (mfr == null)
                {
                    return Json(new JSingle<object>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                return Json(new JSingle<object>() { code = 200, success = true, data = mfr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<object>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取申报要素(用于申报要素查询窗口)
        /// </summary>
        /// <param name="hsCode">海关编码</param>
        /// <param name="origin">原产地</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetElements(string hsCode, string origin)
        {
            try
            {
                var tariff = new YaHv.PvData.Services.Views.Alls.TariffsAll().FirstOrDefault(item => item.HSCode == hsCode.Trim());
                if (tariff == null)
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                //查询申报要素默认值
                var elementsDefaults = tariff.ElementsDefaults.Select(item => new { item.ElementName, item.DefaultValue });
                //查询原产地税率
                var curDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                var originTax = tariff.OriginsATRate.FirstOrDefault(item => item.Origin == origin &&
                                                                            item.StartDate <= curDate && (item.EndDate == null || item.EndDate >= curDate));

                //监管条件 海关监管条件中含有A的 或者 商检监管条件中，带有L或者带有M的商品编码
                var flag = false;
                if (tariff.CIQC != null)
                {
                    //是否商检标志
                    string[] ciqCodes = { "L", "M" };

                    foreach (var ins in ciqCodes)
                    {
                        if (tariff.CIQC.IndexOf(ins) != -1)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (tariff.SupervisionRequirements != null && tariff.SupervisionRequirements.IndexOf("A") != -1)
                {
                    flag = true;
                }

                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        DeclareElements = tariff.DeclareElements,
                        TariffName = tariff.Name,
                        TariffID = tariff.ID,
                        ImportPreferentialTaxRate = tariff.ImportTaxRate / 100,
                        OriginATRate = originTax == null ? 0 : (originTax.Rate) / 100,
                        VATRate = tariff.VATRate / 100,
                        ExciseTaxRate = tariff.ExciseTaxRate.GetValueOrDefault() / 100,
                        LegalUnit1 = tariff.LegalUnit1,
                        LegalUnit2 = tariff.LegalUnit2,
                        CIQCode = tariff.CIQCode,
                        //监管条件
                        SupervisionRequirements = tariff.SupervisionRequirements,
                        //是否需要商检，
                        CIQFlag = flag,
                        //申报要素默认值 
                        ElementsDefaults = elementsDefaults
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

        #region 归类日志

        /// <summary>
        /// 获取归类操作日志
        /// </summary>
        /// <param name="itemId">OrderItemID或预归类产品ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClassifyOperatingLogs(string itemId)
        {
            try
            {
                //获取itemId对应的归类操作日志
                var logs = new YaHv.PvData.Services.Views.Alls.Logs_ClassifyOperatingAll().Where(log => log.MainID == itemId);
                if (logs == null || !logs.Any())
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                //返回归类操作日志
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = logs.ToList().Select(log => new
                    {
                        log.ID,
                        CreateDate = log.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                        log.Summary
                    })
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
        /// 获取某型号的归类变更日志
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetClassifyModifiedLogs(string partNumber)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();

                var logs = new YaHv.PvData.Services.Views.Alls.Logs_ClassifyModifiedAll().Where(log => log.PartNumber == partNumber);
                if (logs == null || !logs.Any())
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                //返回归类变更日志
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = logs.ToList().Select(log => new
                    {
                        log.ID,
                        CreateDate = log.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                        log.Summary
                    })
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
        /// 获取归类历史数据的变更日志
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetClassifiedModifiedPasts(string partNumber, string manufacturer)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                manufacturer = manufacturer.FixSpecialChars();

                var logs = new YaHv.PvData.Services.Views.Alls.Pasts_ClassifiedModifiedAll().Where(log => log.PartNumber == partNumber && log.Manufacturer == manufacturer);
                if (logs == null || !logs.Any())
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                //返回归类变更日志
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = logs.ToList().Select(log => new
                    {
                        log.ID,
                        CreateDate = log.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                        log.Summary
                    })
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

        #region 归类历史数据

        /// <summary>
        /// 提交归类历史数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitClassifiedHistory(ClassifiedResult result)
        {
            try
            {
                var classifiedHistory = new ClassifiedHistory()
                {
                    PartNumber = result.PartNumber,
                    Manufacturer = result.Manufacturer,
                    HSCode = result.HSCode,
                    TariffName = result.TariffName,
                    LegalUnit1 = result.LegalUnit1,
                    LegalUnit2 = result.LegalUnit2,
                    VATRate = result.VATRate,
                    ImportPreferentialTaxRate = result.ImportPreferentialTaxRate,
                    ExciseTaxRate = result.ExciseTaxRate,
                    Elements = result.Elements,
                    CIQCode = result.CIQCode,
                    TaxCode = result.TaxCode,
                    TaxName = result.TaxName,

                    Ccc = result.Ccc,
                    Embargo = result.Embargo,
                    HkControl = result.HkControl,
                    Coo = result.Coo,
                    CIQ = result.CIQ,
                    CIQprice = result.CIQprice,

                    CreatorID = result.CreatorID,
                    CreatorName = result.CreatorName
                };
                classifiedHistory.Enter();

                //返回自动归类信息
                var json = new JMessage() { code = 200, success = true, data = "提交成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取归类历史数据的备注信息
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetRemark(string partNumber, string manufacturer)
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();
                manufacturer = manufacturer.FixSpecialChars();

                var other = new YaHv.PvData.Services.Views.Alls.OthersAll()[partNumber, manufacturer];
                if (other == null || string.IsNullOrEmpty(other.Summary))
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = "无备注信息" }, JsonRequestBehavior.AllowGet);
                }

                //返回备注信息
                var json = new JMessage() { code = 200, success = true, data = other.Summary };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}