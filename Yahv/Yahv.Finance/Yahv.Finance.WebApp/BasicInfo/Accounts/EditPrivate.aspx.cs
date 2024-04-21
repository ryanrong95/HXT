using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Accounts
{
    public partial class EditPrivate : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var AccountID = Request.QueryString["AccountID"];
                this.Model.AccountID = AccountID;
                var account = Yahv.Erp.Current.Finance.Accounts.Where(t => t.ID == AccountID).FirstOrDefault();
                this.Model.Data = account;
                this.Model.OpeningTime = (account != null && account.OpeningTime != null) ? account.OpeningTime?.ToString("yyyy-MM-dd") : "";

                //该账户的银行账户类型
                this.Model.theAccountMapsAccountType = Yahv.Erp.Current.Finance.MapsAccountType.Where(t => t.AccountID == AccountID)
                    .Select(t => new { AccountTypeID = t.AccountTypeID, }).ToArray();

                //该账户的账户用途
                this.Model.theAccountMapsPurpose = Yahv.Erp.Current.Finance.MapsPurpose.Where(t => t.AccountID == AccountID)
                    .Select(t => new { AccountPurposeID = t.AccountPurposeID, }).ToArray();

                //账户类型
                if (account?.Enterprise?.Type != null)
                {
                    this.Model.theEpAccountType = Enum.GetValues(typeof(EnterpriseAccountType)).Cast<EnterpriseAccountType>()
                    .Where(item => account.Enterprise.Type.HasFlag(item)).Select(item => new { ID = item }).ToArray();

                }

                //关联公司名称
                Enterprise enterprise = null;
                if (account != null)
                {
                    enterprise = Yahv.Erp.Current.Finance.Enterprises
                        .Where(t => t.Status == GeneralStatus.Normal && t.ID == account.EnterpriseID).FirstOrDefault();
                }
                this.Model.EnterpriseName = (enterprise != null) ? enterprise.Name : "";

                //银行名称
                this.Model.Banks = Yahv.Erp.Current.Finance.Banks
                    .Where(t => t.Status == Underly.GeneralStatus.Normal)
                    .OrderBy(t => t.Name)
                    .Select(item => new
                    {
                        item.ID,
                        Name = item.Name,
                        EnglishName = item.EnglishName,
                        SwiftCode = item.SwiftCode,
                    })
                    .ToArray();

                //银行账户类型
                this.Model.AccountTypes = Yahv.Erp.Current.Finance.AccountTypes
                    .Where(t => t.Status == Underly.GeneralStatus.Normal)
                    .OrderBy(t => t.Name)
                    .Select(item => new { value = item.ID, text = item.Name })
                    .ToArray();

                //管理类型
                this.Model.ManageType = ExtendsEnum.ToDictionary<ManageType>().Select(item => new { value = item.Key, text = item.Value });

                //币种
                this.Model.Currency = ExtendsEnum.ToDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value });

                //国家及地区
                this.Model.Districts = ExtendsEnum.ToDictionary<Origin>().Select(item => new { value = item.Value, text = item.Value });

                //有无U盾
                Dictionary<string, string> dicIsHaveU = new Dictionary<string, string>();
                dicIsHaveU.Add("1", "有");
                dicIsHaveU.Add("2", "无");
                this.Model.IsHaveU = dicIsHaveU.Select(item => new { value = item.Key, text = item.Value });

                //帐户管理人
                this.Model.Admins = Yahv.Erp.Current.Finance.Admins
                    .GetApplyAdmins()
                    .OrderBy(t => t.RealName)
                    .Select(item => new
                    {
                        value = item.ID,
                        text = item.RealName,
                        selected = item.ID == Erp.Current.ID,
                    })
                    .ToArray();

                //所在金库
                this.Model.GoldStores = Yahv.Erp.Current.Finance.GoldStores
                    .Where(t => t.Status == GeneralStatus.Normal)
                    .OrderBy(t => t.Name)
                    .Select(item => new { value = item.ID, text = item.Name })
                    .ToArray();

                //账户用途
                this.Model.AccountPurposes = Yahv.Erp.Current.Finance.AccountPurposes
                    .Where(t => t.Status == Underly.GeneralStatus.Normal)
                    .OrderBy(t => t.Name)
                    .Select(item => new { value = item.ID, text = item.Name })
                    .ToArray();

                //账户类型
                this.Model.EnterpriseAccountTypes = ExtendsEnum.ToDictionary<Yahv.Finance.Services.Enums.EnterpriseAccountType>()
                    .Select(item => new { value = item.Key, text = item.Value });

                //来源
                this.Model.AccountSources = ExtendsEnum.ToDictionary<Yahv.Finance.Services.Enums.AccountSource>()
                    .Select(item => new { value = item.Key, text = item.Value });
            }
        }

        #region 提交保存
        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };
            string AccountID = Request.Form["ID"];
            Account account = null;
            //填充 关联公司
            Enterprise enterprise = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                try
                {
                    string Name = Request.Form["Name"];
                    string BankName = Request.Form["Bank"];
                    string AccountType = Request.Form["AccountType"];
                    string Code = Request.Form["Code"];
                    string Enterprise = Request.Form["Enterprise"];

                    string ManageType = Request.Form["ManageType"];
                    string SwiftCode = Request.Form["SwiftCode"];
                    string Currency = Request.Form["Currency"];
                    string OpeningBank = Request.Form["OpeningBank"];
                    string BankAddress = Request.Form["BankAddress"];

                    string District = Request.Form["District"];
                    string OpeningTime = Request.Form["OpeningTime"];
                    string IsHaveU = Request.Form["IsHaveU"];
                    string BankNo = Request.Form["BankNo"];
                    string Owner = Request.Form["Owner"];

                    string GoldStore = Request.Form["GoldStore"];
                    string AccountPurpose = Request.Form["AccountPurpose"];
                    var epAccountType = Request.Form["EnterpriseAccountType"];
                    string shortName = Request.Form["ShortName"];
                    string summary = Request.Form["Summary"];
                    var AccountSource = Request.Form["Source"];

                    //填充 Account
                    var accounts = Erp.Current.Finance.Accounts;
                    bool isVirtual = bool.Parse(Request.Form["IsVirtual"].Trim(','));

                    if (string.IsNullOrWhiteSpace(AccountID))
                    {
                        var source = (AccountSource)(int.Parse(AccountSource));
                        if (source == Services.Enums.AccountSource.Standard && accounts.Any(item => item.Code == Code))
                        {
                            json.success = false;
                            json.data = "银行账号不能重复!";
                            return json;
                        }

                        account = new Account()
                        {
                            ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Account),
                            NatureType = NatureType.Private,
                            Name = Name,
                            BankName = BankName,
                            Code = Code,
                            ManageType = (Underly.ManageType)(int.Parse(ManageType)),
                            SwiftCode = SwiftCode,
                            Currency = (Underly.Currency)(int.Parse(Currency)),
                            OpeningBank = OpeningBank,
                            BankAddress = BankAddress,
                            District = District,
                            OpeningTime =
                                string.IsNullOrEmpty(OpeningTime) ? null : (DateTime?)(DateTime.Parse(OpeningTime)),
                            IsHaveU = (IsHaveU == "1"),
                            BankNo = BankNo,
                            OwnerID = Owner,
                            GoldStoreID = string.IsNullOrEmpty(GoldStore) ? null : GoldStore,
                            ShortName = shortName,
                            Summary = summary,
                            CreatorID = Erp.Current.ID,
                            ModifierID = Erp.Current.ID,
                            Source = (AccountSource)(int.Parse(AccountSource)),
                            IsVirtual = isVirtual,
                        };
                    }
                    else
                    {
                        //if (accounts.Any(item => item.Code == Code && item.ID != AccountID))
                        //{
                        //    json.success = false;
                        //    json.data = "银行账号不能重复!";
                        //    return json;
                        //}

                        account = accounts.Where(t => t.ID == AccountID).FirstOrDefault();
                        account.NatureType = NatureType.Private;
                        account.Name = Name;
                        account.BankName = BankName;
                        account.ManageType = (Underly.ManageType)(int.Parse(ManageType));
                        account.SwiftCode = SwiftCode;
                        account.OpeningBank = OpeningBank;
                        account.BankAddress = BankAddress;
                        account.District = District;
                        account.OpeningTime = string.IsNullOrEmpty(OpeningTime)
                            ? null
                            : (DateTime?)(DateTime.Parse(OpeningTime));
                        account.IsHaveU = (IsHaveU == "1");
                        account.BankNo = BankNo;
                        account.OwnerID = Owner;
                        account.GoldStoreID = string.IsNullOrEmpty(GoldStore) ? null : GoldStore;
                        account.ShortName = shortName;
                        account.Summary = summary;
                        account.ModifierID = Erp.Current.ID;
                        account.ModifyDate = DateTime.Now;
                        account.Source = (AccountSource)(int.Parse(AccountSource));
                        account.IsVirtual = isVirtual;
                        //account.Code = string.IsNullOrEmpty(Code) ? null : Code;
                        //account.Currency = (Underly.Currency)(int.Parse(Currency));
                    }



                    if (string.IsNullOrEmpty(Enterprise))
                    {
                        account.EnterpriseID = null;
                    }
                    else
                    {
                        enterprise = Erp.Current.Finance.Enterprises
                            .Where(t => t.Name == Enterprise && t.Status == GeneralStatus.Normal).FirstOrDefault();
                        if (enterprise != null)
                        {
                            account.EnterpriseID = enterprise.ID;
                            if (!string.IsNullOrEmpty(epAccountType))
                            {
                                enterprise.Type = GetEpAccountType(epAccountType);
                            }
                            enterprise.Enter();
                        }
                        else
                        {
                            //要新增一个 Enterprise
                            enterprise = new Enterprise()
                            {
                                ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Enterprise),
                                Name = Enterprise,
                                District = District,
                                CreatorID = Erp.Current.ID,
                                ModifierID = Erp.Current.ID,
                            };

                            if (!string.IsNullOrEmpty(epAccountType))
                            {
                                enterprise.Type = GetEpAccountType(epAccountType);
                            }
                            account.EnterpriseID = enterprise.ID;
                            //保存
                            enterprise.Enter();
                        }
                    }

                    //填充 AccountType
                    string[] accountTypeIDs = AccountType?.Split(',');
                    List<MapsAccountType> mapsAccountTypes = new List<MapsAccountType>();
                    if (accountTypeIDs?.Length > 0)
                    {
                        foreach (var accountTypeID in accountTypeIDs)
                        {
                            mapsAccountTypes.Add(new MapsAccountType()
                            {
                                AccountID = account.ID,
                                AccountTypeID = accountTypeID,
                            });
                        }
                    }
                    //填充 AccountPurpose
                    string[] accountPurposeIDs = AccountPurpose?.Split(',');
                    List<MapsPurpose> mapsPurposes = new List<MapsPurpose>();
                    if (accountPurposeIDs?.Length > 0)
                    {
                        foreach (var accountPurposeID in accountPurposeIDs)
                        {
                            mapsPurposes.Add(new MapsPurpose()
                            {
                                AccountID = account.ID,
                                AccountPurposeID = accountPurposeID,
                            });
                        }
                    }

                    account.Enter();
                    Erp.Current.Finance.MapsAccountType.BatchEnter(account.ID, mapsAccountTypes.ToArray());
                    Erp.Current.Finance.MapsPurpose.BatchEnter(account.ID, mapsPurposes.ToArray());
                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账户管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(ID) ? "新增" : "修改", new { account, enterprise }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账户管理, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(ID) ? "新增" : "修改") + " 异常!", new { account, enterprise, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        #endregion

        #region 私有函数
        /// <summary>
        /// 根据字符串返回 组合枚举
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        EnterpriseAccountType GetEpAccountType(string values)
        {
            EnterpriseAccountType epType = default(EnterpriseAccountType);
            foreach (var type in values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                epType |= (EnterpriseAccountType)int.Parse(type);
            }

            return epType;
        }
        #endregion
    }
}