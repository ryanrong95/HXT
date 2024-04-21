using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.Web.UI.WebControls;
using Layers.Data;
using Newtonsoft.Json;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Utils;
using PKeyType = Yahv.Erm.Services.PKeyType;

namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class StaffEdit : ErpParticlePage
    {
        string tempID;
        private void SetTempID(string id)
        {
            tempID = id;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                this.Model.AllData = Alls.Current.Staffs[Request.QueryString["ID"]];
            }
        }

        protected object roles()
        {
            return Alls.Current.Roles.Select(item => new
            {
                id = item.ID,
                name = item.Name
            });
        }

        protected void WageData()
        {
            string staffPostion = Request.Form["PostionID"];
            string staffid = Request.Form["ID"];
            if (string.IsNullOrEmpty(staffPostion))
            {
                Response.Write((new { success = false, data = "职位ID为空！" }).Json());
                return;
            }
            var PositionItems = Alls.Current.WageItems.Where(item => Alls.Current.Postions.PositionWages(staffPostion).Contains(item.ID));
            if (PositionItems.Count() == 0)
            {
                Response.Write((new { success = false, data = "职位对应工资项为空！" }).Json());
                return;
            }
            bool Ischange = false;
            if (!string.IsNullOrEmpty(staffid))
            {
                if (Alls.Current.Staffs[staffid].PostionID != staffPostion)
                {
                    Ischange = true;
                }
            }
            Response.Write((new { success = true, PositionItems = PositionItems, Ischange = Ischange }).Json());
        }

        protected void LoadComboBoxData()
        {
            this.Model.PostionData = Alls.Current.Postions.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.CityData = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity).Select(item => new { Value = item.ID, Text = item.Name });

            string[] enums = new string[]
            {
               ((int)StaffStatus.Period).ToString(),
               ((int)StaffStatus.Normal).ToString(),
               ((int)StaffStatus.Departure).ToString(),
               ((int)StaffStatus.Cancel).ToString(),
            };
            this.Model.Status = ExtendsEnum.ToDictionary<Services.StaffStatus>().Where(item => enums.Contains(item.Key))
                .Select(item => new { Value = item.Key, Text = item.Value });
        }
        protected void Save()
        {
            try
            {
                var id = Request.Form["ID"];
                var staff = Alls.Current.Staffs[id] ?? new Staff() as Staff;
                var newIDCard = Request.Form["IDCard"].Trim();
                var username = Request.Form["UserName"].Trim();
                if (string.IsNullOrEmpty(id))
                {
                    if (Alls.Current.Personals.Where(item => item.IDCard == newIDCard).Count() != 0)
                    {
                        Response.Write((new { success = false, message = "身份证ID重复，保存失败" }).Json());
                        return;
                    }
                    if (Alls.Current.Admins.Where(item => item.UserName == username).Count() != 0)
                    {
                        Response.Write((new { success = false, message = "用户名重复，保存失败" }).Json());
                        return;
                    }

                    staff.RoleID = FixedRole.NewStaff.GetFixedID();
                    staff.UserName = Request.Form["UserName"].Trim();
                    staff.Password = "123456";
                    //判断账号是否存在
                    if (IsExistsAccount(staff.UserName))
                    {
                        staff.UserName = GetAccountByName(Request.Form["Name"].Trim());
                    }
                }
                else if (Alls.Current.Personals.Where(item => item.ID != id).Where(item => item.IDCard == newIDCard).Count() >= 1)
                {
                    Response.Write((new { success = false, message = "身份证ID重复，保存失败" }).Json());
                    return;
                }
                else if (staff.PostionID != Request.Form["PostionID"])
                {
                    var tempTime = DateTime.Now;
                    var oldItems = Alls.Current.WageItems.Where(item => Alls.Current.Postions.PositionWages(Alls.Current.Staffs[id].PostionID).Contains(item.ID));
                    foreach (var item in oldItems)
                    {
                        string itemid = item.ID;
                        decimal? oldvalue = Alls.Current.Postions.ClearWageValue(itemid, id);
                        if (oldvalue != null)
                        {
                            var pastWageitem = new PastsWageItem();
                            pastWageitem.StaffID = id;
                            pastWageitem.WageItemID = itemid;
                            pastWageitem.AdminID = Erp.Current.ID;
                            pastWageitem.DefaultValue = oldvalue;
                            pastWageitem.CreateDate = tempTime;
                            pastWageitem.Enter();
                        }
                    }
                    Alls.Current.MyWageItems.Delete(id);
                }
                if (!string.IsNullOrEmpty(id))
                {
                    staff.UserName = "";
                }
                staff.DyjCode = Request.Form["DyjCode"].Trim();
                staff.Name = Request.Form["Name"].Trim();
                staff.DyjCompanyCode = Request.Form["DyjCompanyCode"].Trim();
                staff.DyjDepartmentCode = Request.Form["DyjDepartmentCode"].Trim();
                staff.Gender = (Gender)Enum.Parse(typeof(Gender), Request.Form["Gender"]);
                staff.IDCard = Request.Form["IDCard"].Trim();
                staff.PostionID = Request.Form["PostionID"].Trim();
                staff.WorkCity = Request.Form["WorkCity"].Trim();
                staff.AdminID = Erp.Current.ID;
                staff.Status = (StaffStatus)Enum.Parse(typeof(StaffStatus), Request.Form["Status"]);
                staff.Mobile = Request.Form["Mobile"].Trim();
                staff.Email = Request.Form["Email"].Trim();
                staff.EnterError += Staff_EnterError;
                staff.EnterSuccess += Staff_EnterSuccess;
                staff.Enter();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Staff_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var staff = (Staff)e.Object;
            string id = staff.ID;
            #region personal
            var personal = Alls.Current.Personals[id] ?? new Personal() as Personal;
            if (personal.ID == null)
            {
                personal.ID = id;
            }
            personal.IDCard = staff.IDCard;
            personal.Email = staff.Email;
            personal.Mobile = staff.Mobile;
            personal.Enter();
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                string.IsNullOrWhiteSpace(id) ? $"新增员工明细" : $"修改员工明细", (new { staff = staff, personal = personal }).Json());
            #endregion           
            #region admin
            if (staff.UserName != "")
            {
                var admin = new Admin() as Admin;
                admin.RoleID = staff.RoleID;
                admin.StaffID = id;
                admin.RealName = staff.Name;
                admin.UserName = staff.UserName;
                admin.Password = staff.Password;
                admin.Enter();
            }
            #endregion
            Response.Write((new { success = true, message = "保存成功", ID = id }).Json());
        }

        /// <summary>
        /// 保存失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Staff_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(id))
            {
                return string.Empty.Json();
            }
            var staffPostion = Alls.Current.Staffs[id].PostionID;
            var mapwageitems = Alls.Current.MyWageItems
                .Where(item => Alls.Current.Postions.PositionWages(staffPostion).Contains(item.WageItemID))
                .Where(item => item.ID == id).Select(item => new StaffWageItem
                {
                    ID = item.ID,
                    WageItemID = item.WageItemID,
                    DefaultValue = item.DefaultValue
                });
            List<string> dynColumns;
            if (!mapwageitems.Any())
            {
                return string.Empty.Json();
            }

            var dataList = mapwageitems.ToList();
            return ExportWages.Current.DynamicLinq(dataList, GetFixedColumns(), "WageItemID", "DefaultValue", out dynColumns); ;
        }

        /// <summary>
        /// 获取固定列
        /// </summary>
        /// <returns></returns>
        private List<string> GetFixedColumns()
        {
            return new List<string>()
                    {
                         "ID",
                    };
        }

        protected void SaveDatagrid1()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
            string staffid = Request.Form["ID"];
            var staffPostion = Alls.Current.Staffs[staffid].PostionID;
            var PositionItems = Alls.Current.WageItems.Where(item => Alls.Current.Postions.PositionWages(staffPostion).Contains(item.ID));
            bool ischange = false;
            foreach (var item in PositionItems)
            {
                string itemid = item.ID;
                decimal value = model[itemid];
                ischange = Alls.Current.Postions.WageCheckchange(itemid, staffid, value);
                if (ischange)
                {
                    break;
                }
            }
            if (ischange)
            {
                var tempTime = DateTime.Now;
                foreach (var item in PositionItems)
                {
                    string itemid = item.ID;
                    decimal? oldvalue = Alls.Current.Postions.WageOldvalue(itemid, staffid);
                    var pastWageitem = new PastsWageItem();
                    pastWageitem.StaffID = staffid;
                    pastWageitem.WageItemID = itemid;
                    pastWageitem.AdminID = Erp.Current.ID;
                    pastWageitem.DefaultValue = oldvalue;
                    pastWageitem.CreateDate = tempTime;
                    pastWageitem.Enter();
                }
            }

            foreach (var item in PositionItems)
            {
                string itemid = item.ID;
                decimal value = model[itemid];
                Alls.Current.Postions.SetWageValue(itemid, staffid, value);
            }
        }

        protected void SaveDatagrid()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
            string staffid = Request.Form["ID"];
            DataTable dt;

            //json转为datatable
            try
            {
                dt = ExportWages.Current.JsonToDataTable(("[" + Model + "]").Json().Replace("\"", "").Replace("'", "\""));
            }
            catch (Exception)
            {
                dt = JsonConvert.DeserializeObject<DataTable>("[" + Model + "]");
            }


            //判断员工工资默认值是否有值(新增)
            if (new MyWageItemsRoll().Count(item => item.ID == staffid) <= 0)
            {
                foreach (var column in dt.Columns)
                {
                    if (column.ToString() != "ID")
                        Alls.Current.Postions.SetWageValue(column.ToString(), staffid, decimal.Parse(dt.Rows[0][column.ToString()].ToString()));
                }
            }
            //修改
            else
            {
                //判断是否有更新
                var myWageItems = new MyWageItemsRoll(staffid).ToList();
                bool ischange = false;

                foreach (var column in dt.Columns)
                {
                    if (column.ToString() != "ID")
                    {
                        var wageItem = myWageItems.FirstOrDefault(item => item.WageItemID == column.ToString());
                        if (wageItem != null && wageItem.DefaultValue != decimal.Parse(dt.Rows[0][column.ToString()].ToString()))
                        {
                            ischange = true;
                            break;
                        }
                    }
                }

                if (ischange)
                {
                    //添加历史记录表
                    var PositionItems = Alls.Current.WageItems.Where(item => Alls.Current.Postions.PositionWages(Alls.Current.Staffs[staffid].PostionID).Contains(item.ID));
                    var tempTime = DateTime.Now;
                    foreach (var item in PositionItems)
                    {
                        string itemid = item.ID;
                        decimal? oldvalue = Alls.Current.Postions.WageOldvalue(itemid, staffid);
                        var pastWageitem = new PastsWageItem();
                        pastWageitem.StaffID = staffid;
                        pastWageitem.WageItemID = itemid;
                        pastWageitem.AdminID = Erp.Current.ID;
                        pastWageitem.DefaultValue = oldvalue;
                        pastWageitem.CreateDate = tempTime;
                        pastWageitem.Enter();
                    }

                    //清空工资项
                    Alls.Current.MyWageItems.Delete(staffid);

                    //循环添加值
                    foreach (var column in dt.Columns)
                    {
                        if (column.ToString() != "ID")
                            Alls.Current.Postions.SetWageValue(column.ToString(), staffid, decimal.Parse(dt.Rows[0][column.ToString()].ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// 根据姓名生成账号
        /// </summary>
        /// <returns></returns>
        protected object createAccount()
        {
            string account = string.Empty;

            try
            {
                string name = Request["name"];
                if (!string.IsNullOrWhiteSpace(name))
                {
                    account = GetAccountByName(name);
                }
            }
            catch (Exception)
            {
            }

            return account;
        }

        /// <summary>
        /// 根据姓名获取账号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetAccountByName(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(name)) return result;

            result = PinyinHelper.GetPinyin(name);
            int num = 1;
            while (Alls.Current.Admins.Where(item => item.UserName == result).Count() >= 1)
            {
                if (num == 1)
                {
                    result = result + num.ToString();
                }
                else
                {
                    result = result.Replace((num - 1).ToString(), num.ToString());
                }
                num++;
            }

            return result;
        }

        /// <summary>
        /// 判断账号是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool IsExistsAccount(string account)
        {
            return Alls.Current.Admins.Any(item => item.UserName == account);
        }
    }
}