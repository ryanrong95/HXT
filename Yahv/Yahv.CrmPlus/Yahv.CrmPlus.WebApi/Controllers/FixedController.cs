using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;

namespace Yahv.CrmPlus.WebApi.Controllers
{

    public class FixedController : Controller
    {
        Type[] types;

        public FixedController()
        {
            this.types = new[] {
                typeof(Underly.CrmPlus.ClientType), typeof(Currency), typeof(ClientGrade), typeof(VIPLevel), typeof(ConductGrade),
                typeof(AuditStatus), typeof(OrderType),
                typeof(InvoiceType), typeof(Origin), typeof(Yahv.Underly.CrmPlus.SupplierType),
                typeof(EnterpriseNature), typeof(BusinessRelationType), typeof(SupplierGrade),
                typeof(SettlementType),typeof(ClearType),
                typeof(nBrandType),
                typeof(CommissionMethod),typeof(CommissionType),
                typeof(BookAccountMethord),typeof(AddressType),
                typeof(SampleType),typeof(ReportStatus),typeof(ProductStatus),typeof(FollowWay),typeof(DataStatus),typeof(FixedArea),
                typeof(FixedSource),
                typeof(FreightPayer),typeof(QuoteMethod)
            };
        }

        // GET: Enum
        public ActionResult Index()
        {
            ClientType clientType;
            if (Enum.TryParse(Request.Form["asdfasdf"], out clientType))
            {
                //拼接条件
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Enums(string name)
        {
            var type = types.Single(item => item.Name == name.Trim());
            return Json(Enum.GetValues(type).Cast<Enum>().Select(item =>
            {
                return new
                {
                    ID = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type)),
                    Name = item.GetDescription()
                };
            }).OrderBy(item => item.ID), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Dic(string name)
        {
            var dics= new EnumsDictionariesRoll().Where(x => x.Enum == name).Select(x => new { ID = x.ID, Name = x.Description });
            return Json(dics.OrderBy(x=>x.ID), JsonRequestBehavior.AllowGet);
        }
    }
}