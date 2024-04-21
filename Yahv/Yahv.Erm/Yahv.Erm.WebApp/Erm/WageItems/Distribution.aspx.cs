using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.WageItems
{
    public partial class Distribution : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = Alls.Current.WageItems[Request.QueryString["id"]];
            }
        }

        /// <summary>
        /// 加载树形菜单
        /// </summary>
        /// <returns></returns>
        protected string tree()
        {
            var type = Request.QueryString["selectType"];

            if ((DistributeType)int.Parse(type) == DistributeType.Area)
            {
                var district = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity);
                var staffIds_selected = Alls.Current.WageItems.GetStaffs(Request.QueryString["id"]);
                var district_selected = Alls.Current.Staffs.Where(item => staffIds_selected.Contains(item.ID)).Select(item => item.WorkCity).ToArray();

                return new[]
                {
                    new
                    {
                        id = "1",
                        text = "所属区域",
                        attributes = new {type = DistributeType.Area},
                        children = (from dis in district
                            where dis.FatherID != null
                            select new
                            {
                                id = dis.ID,
                                text = dis.Name,
                                attributes = new {type = DistributeType.Area},
                                @checked=district_selected.Contains(dis.ID),
                            }).ToArray(),
                    }
                }.Json();
            }
            else if ((DistributeType)int.Parse(type) == DistributeType.Postion)
            {
                var postion = Alls.Current.Postions;
                var staffIds_selected = Alls.Current.WageItems.GetStaffs(Request.QueryString["id"]);
                var postion_selected = Alls.Current.Staffs.Where(item => staffIds_selected.Contains(item.ID)).Select(item => item.PostionID).ToArray();

                return new[]
                {
                     new {
                        id = "2",
                        text = "考核岗位",
                        attributes = new { type = DistributeType.Postion },
                        children = (from p in postion
                                    select new
                                    {
                                        id = p.ID,
                                        text = p.Name,
                                        attributes = new { type = DistributeType.Postion },
                                        @checked=postion_selected.Contains(p.ID),
                                    }).ToArray()
                    }
                }.Json();
            }

            return string.Empty;
        }

        protected object data()
        {
            var type = Request.QueryString["treeType"];
            var id = Request.QueryString["treeId"];
            var name = Request.QueryString["sName"];

            var staffs = Alls.Current.Staffs.Where(item => true);
            Expression<Func<Staff, bool>> predicate = null;
            var staffs_selected = Alls.Current.WageItems.GetStaffs(Request.QueryString["id"]);      //已勾选员工

            if ((DistributeType)int.Parse(type) == DistributeType.Area)
            {
                if (id.ToString() != ((int)DistributeType.Area).ToString())
                {
                    predicate = predicate.And(item => item.WorkCity == id);
                }
                //else
                //{
                //    predicate = predicate.And(item => staffs_selected.Contains(item.ID));
                //}
            }
            else if ((DistributeType)int.Parse(type) == DistributeType.Postion)
            {
                if (id.ToString() != ((int)DistributeType.Postion).ToString())
                {
                    predicate = predicate.And(item => item.PostionID == id);
                }
                //else
                //{
                //    predicate = predicate.And(item => staffs_selected.Contains(item.ID));
                //}
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name) || item.DyjCode.Contains(name) || item.Code.Contains(name));
            }

            if (predicate != null)
            {
                staffs = staffs.Where(predicate);
            }



            return staffs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.Code,
                item.DyjCode,
                Gender = item.Gender.GetDescription(),
                WorkCity = item.CityName,
                PostionName = item.PostionName,
                Status = item.Status.GetDescription(),
                item.EnterpriseID,
                LeaveDate = item.LeaveDate?.ToString("yyyy-MM-dd"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                Selected = staffs_selected.Contains(item.ID),
            });
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        /// <returns></returns>
        protected object bindStaffs()
        {
            var result = new { code = "200", message = "操作成功!" };

            var id = Request["itemId"];
            var staffIds = Request["staffIds"];

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(staffIds))
            {
                result = new { code = "400", message = "工资项或员工不能为空!" };
                return result;
            }

            var array = staffIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length <= 0)
            {
                result = new { code = "400", message = "请您选择员工!" };
                return result;
            }
            Alls.Current.WageItems.BindStaffs(id, array);
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资项管理",
                    $"工资项绑定员工", array.Json());
            return result;
        }

        /// <summary>
        /// 去除员工
        /// </summary>
        /// <returns></returns>
        protected object unbindStaffs()
        {
            var result = new { code = "200", message = "操作成功!" };

            var id = Request["itemId"];
            var staffIds = Request["staffIds"];

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(staffIds))
            {
                result = new { code = "400", message = "工资项或员工不能为空!" };
                return result;
            }

            var array = staffIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length <= 0)
            {
                result = new { code = "400", message = "请您选择员工!" };
                return result;
            }

            Alls.Current.WageItems.UnbindStaffs(id, array);
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资项管理",
                   $"工资项取消绑定员工", array.Json());
            return result;
        }
    }

    /// <summary>
    /// 分配类型
    /// </summary>
    public enum DistributeType
    {
        Area = 1,
        Postion = 2,
    }
}