using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class NoticeController : BaseController
    {
        #region 页面

        /// <summary>
        /// 枚举
        /// </summary>
        /// <returns></returns>
        public ActionResult Enum() { return View(); }

        #endregion

        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEnums()
        {
            List<GetEnumsReturnModel.EnumInfo> enumInfos = new List<GetEnumsReturnModel.EnumInfo>();

            Assembly assembly = Assembly.Load("Yahv.PsWms.SzMvc.Services");
            Type[] typeArr = assembly.GetTypes();

            foreach (Type item in typeArr)
            {
                if (item.FullName.StartsWith("Yahv.PsWms.SzMvc.Services.Enums"))
                {
                    enumInfos.Add(new GetEnumsReturnModel.EnumInfo
                    {
                        EnumName = item.Name,
                    });
                }
            }

            enumInfos = enumInfos.OrderBy(t => t.EnumName).ToList();

            return Json(new { type = "success", msg = "", data = new GetEnumsReturnModel { EnumInfos = enumInfos.ToArray(), } }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="EnunName"></param>
        /// <returns></returns>
        public JsonResult GetEnumValue(string EnunName)
        {
            List<GetEnumValueRetuenModel.EnumValue> enumValues = new List<GetEnumValueRetuenModel.EnumValue>();

            Assembly assembly = Assembly.Load("Yahv.PsWms.SzMvc.Services");
            Type[] typeArr = assembly.GetTypes();

            var targetType = typeArr.Where(t => t.FullName == ("Yahv.PsWms.SzMvc.Services.Enums." + EnunName)).FirstOrDefault();
            FieldInfo[] fields = targetType.GetFields();

            foreach (var item in fields)
            {
                if (item.Name == "value__")
                {
                    continue;
                }

                string description = string.Empty;

                try
                {
                    description = ((Enum)System.Enum.Parse(targetType, item.Name)).GetDescription();
                }
                catch (Exception)
                {

                }

                enumValues.Add(new GetEnumValueRetuenModel.EnumValue
                {
                    Name = item.Name,
                    Value = Convert.ToString((int)item.GetValue(null)),
                    Description = description,
                });
            }

            return Json(new { type = "success", msg = "", data = new GetEnumValueRetuenModel { EnumValues = enumValues.ToArray(), } }, JsonRequestBehavior.AllowGet);
        }

    }
}