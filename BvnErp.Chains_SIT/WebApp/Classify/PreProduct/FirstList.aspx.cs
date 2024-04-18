using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Classify.PreProduct
{
    /// <summary>
    /// 预处理一查询界面
    /// </summary>
    public partial class FirstList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int page, rows;
            int.TryParse(Request.QueryString["PageNumber"], out page);
            int.TryParse(Request.QueryString["PageSize"], out rows);

            var currentSc = new CurrentSc()
            {
                InitUrl = Request.QueryString["InitUrl"] ?? string.Empty,
                PageNumber = page,
                PageSize = rows,
                IsShowLocked = Convert.ToBoolean(Request.QueryString["IsShowLocked"]),
                Model = Convert.ToString(Request.QueryString["Model"]) ?? string.Empty,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ProductName = Convert.ToString(Request.QueryString["ProductName"]) ?? string.Empty,
                HSCode = Convert.ToString(Request.QueryString["HSCode"]) ?? string.Empty,
                LastClassifyTimeBegin = Convert.ToString(Request.QueryString["LastClassifyTimeBegin"]) ?? string.Empty,
                LastClassifyTimeEnd = Convert.ToString(Request.QueryString["LastClassifyTimeEnd"]) ?? string.Empty,
            };

            this.Model.CurrentSc = currentSc.Json();
        }

        /// <summary>
        /// 初始化预归类产品数据
        /// </summary>
        protected void data()
        {
            string Model = Request.QueryString["Model"];
            string strIsShowLocked = Request.QueryString["IsShowLocked"];
            bool isShowLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PreClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.Unclassified;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Expression<Func<PreClassifyProduct, bool>> lambda1 = item => item.Model == Model;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(strIsShowLocked))
            {
                bool.TryParse(strIsShowLocked, out isShowLocked);
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
            lamdasOrderByAscDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.CreateDate));
            lamdasOrderByAscDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.PreProduct.DueDate ?? DateTime.MaxValue));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll.GetPageList(
                page, 
                rows, 
                expression,
                lamdasOrderByAscDateTime.ToArray(),
                null,
                Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                isShowLocked,
                lamdas.ToArray());

            Response.Write(new
            {
                rows = products.Select(
                       item => new
                       {
                           item.ID,
                           ClientCode = item.PreProduct.Client.ClientCode,
                           ClientName = item.PreProduct.Client.Company.Name,
                           item.ProductName,
                           item.Model,
                           item.Manufacturer,
                           UnitPrice = item.PreProduct.Price,
                           Currency = item.PreProduct.Currency,
                           ClassifyStatus = item.ClassifyStatus.GetDescription(),
                           PreProductID = item.PreProductID,
                           DueDate = item.PreProduct.DueDate?.ToString("yyyy-MM-dd") ?? "--",
                           CreateDate = item.CreateDate.ToString().Replace("T", " ") ?? "--",

                           //产品归类锁定
                           LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                           Locker = item.Locker?.RealName ?? "--",
                           LockTime = item.LockDate?.ToString().Replace("T", " ") ?? "--",
                           IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                           IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID
                       }
                    ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 批量锁定
        /// </summary>
        protected void BatchLock()
        {
            try
            {
                string[] ids = Request.Form["IDs"].Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');

                List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                lamdasOrderByAscDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.CreateDate));
                lamdasOrderByAscDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.PreProduct.DueDate?? DateTime.MaxValue));

                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll.GetTop(50, i => ids.Contains(i.ID), lamdasOrderByAscDateTime.ToArray(), null);

                bool isNeedRemindLocked = false;
                StringBuilder sbLockedRemind = new StringBuilder();

                foreach (var item in items)
                {
                    if (item.IsLocked)//已经被锁定
                    {
                        if (item.Locker != null && item.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                        {
                            isNeedRemindLocked = true;
                            //锁定人不是当前登录者,提示已经被他人锁定
                            sbLockedRemind.Append("【" + item.Model + "】、");
                        }
                    }
                    else//未被锁定,进行锁定
                    {
                        item.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.PreStep1, item);
                        classify.Lock();
                    }
                }

                if (isNeedRemindLocked)
                {
                    Response.Write((new { success = true, message = "以下型号已被他人锁定：\r\n" + sbLockedRemind.ToString().Trim(new char[] { '、' }), }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "批量锁定成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "批量锁定失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 归类锁定
        /// </summary>
        protected void Lock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll[id];
                if (classifyProduct.IsLocked && classifyProduct.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                {
                    Response.Write((new { success = false, message = "当前产品归类已被锁定，锁定人【" + classifyProduct.Locker.RealName + "】，锁定时间【" + classifyProduct.LockDate + "】" }).Json());
                }
                else
                {
                    if (!classifyProduct.IsLocked)
                    {
                        classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.PreStep1, classifyProduct);
                        classify.Lock();
                    }
                    Response.Write((new { success = true, message = "" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 解除归类锁定
        /// </summary>
        protected void UnLock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll[id];
                classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var classify = ClassifyFactory.Create(ClassifyStep.PreStep1, classifyProduct);
                classify.UnLock();

                Response.Write((new { success = true, message = "已解除产品归类锁定" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }
    }
}