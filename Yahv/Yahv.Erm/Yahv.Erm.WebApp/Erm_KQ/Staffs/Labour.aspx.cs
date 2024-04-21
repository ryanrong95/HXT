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

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class Labour : ErpParticlePage
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

        protected void Submit()
        {
            try
            {
                #region 界面数据
                string StaffID = Request.Form["StaffID"];
                string EntryDate = Request.Form["EntryDate"];
                string ContractPeriod = Request.Form["ContractPeriod"];
                string ProbationMonths = Request.Form["ProbationMonths"];
                string EntryCompany = Request.Form["EntryCompany"];
                string EntryCompanyName = Request.Form["EntryCompanyName"];
                string SocialSecurityAccount = Request.Form["SocialSecurityAccount"];
                string Bank = Request.Form["Bank"];
                string BankAddress = Request.Form["BankAddress"];
                string BankAccount = Request.Form["BankAccount"];
                #endregion
                var labour = Alls.Current.Labours[StaffID] ?? new Services.Models.Origins.Labour();
                labour.ID = StaffID;
                labour.EntryDate = Convert.ToDateTime(EntryDate);
                labour.SigningTime = labour.EntryDate;
                labour.ContractPeriod = Convert.ToDateTime(ContractPeriod);
                //labour.EnterpriseID = EntryCompany;
                //labour.EntryCompany = EntryCompanyName;

                var ProbationEndDate = Convert.ToDateTime(ProbationMonths);
                var days = (double)ProbationEndDate.Subtract(Convert.ToDateTime(EntryDate)).Days;

                labour.ProbationMonths = (days / 30).ToString();
                labour.SocialSecurityAccount = SocialSecurityAccount;

                labour.Enter();
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"劳资信息编辑", labour.Json());
                if (!string.IsNullOrEmpty(BankAccount))
                {
                    var bank = Alls.Current.BankCards[StaffID] ?? new BankCard();
                    bank.ID = StaffID;
                    bank.Bank = Bank;
                    bank.BankAddress = BankAddress;
                    bank.Account = BankAccount;

                    bank.Enter();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"银行信息编辑", bank.Json());
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                string message = "空间名：" + ex.Source + "；" + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf("\\") + 1, ex.StackTrace.Length - ex.StackTrace.LastIndexOf("\\") - 1) + '\n' +
                    "错误提示：" + ex.Message;
                Response.Write((new { success = false, message = "保存失败：" + message }).Json());
            }
        }

        protected void SaveWages()
        {
            try
            {
                dynamic Wages = Request.Form["Wages"].JsonTo<dynamic>();
                string StaffID = Wages["ID"].ToString();
                var wageView = Alls.Current.MyWageItems;
                var wages = wageView.Where(item => item.ID == StaffID);
                foreach (var item in wages)
                {
                    string value = Wages[item.WageItemID].ToString();//value值
                    if (!string.IsNullOrEmpty(value))
                    {
                        wageView.UpdateWageItem(item.ID, item.WageItemID, decimal.Parse(value));
                    }
                }
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
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