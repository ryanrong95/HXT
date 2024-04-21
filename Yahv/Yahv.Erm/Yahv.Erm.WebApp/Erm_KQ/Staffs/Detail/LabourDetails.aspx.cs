using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail
{
    public partial class LabourDetails : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.CompaniesData = Alls.Current.Companies.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.BankTypeData = ExtendsEnum.ToDictionary<Services.Bank>().Select(item => new { Value = item.Value, Text = item.Value });
        }

        protected void LoadData()
        {
            string StaffID = Request.QueryString["ID"];
            var labour = Alls.Current.Labours[StaffID];
            var bank = Alls.Current.BankCards[StaffID];
            this.Model.LabourData = null;
            this.Model.BankData = null;
            if (labour != null)
            {
                this.Model.LabourData = new
                {
                    EntryDate = labour?.EntryDate,
                    LeaveDate = labour?.LeaveDate,
                    ContractPeriod = labour?.ContractPeriod,
                    ProbationMonths = labour?.ProbationEndDate?.ToString("yyyy-MM-dd"),
                    EntryCompany = labour?.EnterpriseID,
                    SocialSecurityAccount = labour?.SocialSecurityAccount,
                };
            }
            if (bank != null)
            {
                this.Model.BankData = new
                {
                    Bank = bank?.Bank,
                    BankAddress = bank?.BankAddress,
                    BankAccount = bank?.Account,
                };
            }
            //工资项明细
            this.Model.WageItemData = null;
            Staff staff = Alls.Current.Staffs.Single(item => item.ID == StaffID);
            if (!string.IsNullOrEmpty(staff.PostionID))
            {
                var PositionItems = Alls.Current.WageItems.Where(item => item.Type == WageItemType.Normal)
                .Where(item => Alls.Current.Postions.PositionWages(staff.PostionID).Contains(item.ID));
                this.Model.WageItemData = PositionItems.Select(item => new
                {
                    ID = item.ID,
                    Name = item.Name,
                });
            }
        }

        protected object data()
        {
            string StaffID = Request.QueryString["ID"];

            Staff staff = Alls.Current.Staffs.Single(item => item.ID == StaffID);
            var wage = Alls.Current.MyWageItems;
            if (wage.Count() == 0 && string.IsNullOrEmpty(staff.PostionID))
            {
                //初始化工资项
                wage.InitWageItems(StaffID, staff.PostionID);
            }

            var wages = wage.Where(item => item.ID == StaffID).Select(item => new StaffWageItem
            {
                ID = item.ID,
                WageItemID = item.WageItemID,
                DefaultValue = item.DefaultValue
            });
            List<string> dynColumns;
            if (!wages.Any())
            {
                return string.Empty.Json();
            }
            var dataList = wages.ToList();
            var linq = ExportWages.Current.DynamicLinq(dataList, GetFixedColumns(), "WageItemID", "DefaultValue", out dynColumns);
            return linq;
        }

        /// <summary>
        /// 获取固定列
        /// </summary>
        /// <returns></returns>
        private List<string> GetFixedColumns()
        {
            return new List<string>() { "ID" };
        }
    }
}