using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class EnterprisesRoll : QueryView<Enterprise, PvFinanceReponsitory>
    {
        public EnterprisesRoll()
        {
        }


        public EnterprisesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Enterprise> GetIQueryable()
        {
            var enterprises = new EnterprisesOrigin(this.Reponsitory);
            var creators = new AdminsTopView(this.Reponsitory).Where(item => enterprises.Select(e => e.CreatorID).Contains(item.ID));

            return from enterprise in enterprises
                   join creator in creators on enterprise.CreatorID equals creator.ID into _creator
                   from creator in _creator.DefaultIfEmpty()
                   select new Enterprise
                   {
                       ID = enterprise.ID,
                       Name = enterprise.Name,
                       Type = enterprise.Type,
                       District = enterprise.District,
                       CreatorID = enterprise.CreatorID,
                       ModifierID = enterprise.ModifierID,
                       CreateDate = enterprise.CreateDate,
                       ModifyDate = enterprise.ModifyDate,
                       Status = enterprise.Status,
                       Summary = enterprise.Summary,
                       CreatorName = creator != null ? creator.RealName : "",
                   };

        }

        public Enterprise this[string epID]
        {
            get { return this.Single(item => item.ID == epID); }
        }

        /// <summary>
        /// 批量启用
        /// </summary>
        /// <param name="ids"></param>
        public void Enable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Enterprises>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Normal,
                }, item => ids.Contains(item.ID));
            }
        }

        /// <summary>
        /// 批量禁用
        /// </summary>
        /// <param name="ids"></param>
        public void Disable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Enterprises>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => ids.Contains(item.ID));
            }
        }

        /// <summary>
        /// 批量添加企业
        /// </summary>
        /// <param name="flows"></param>
        public void AddRange(IEnumerable<Enterprise> eps)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                var data = eps.Select(item => new Layers.Data.Sqls.PvFinance.Enterprises()
                {
                    ID = item.ID ?? PKeySigner.Pick(PKeyType.Enterprise),
                    Name = item.Name,
                    Type = (int)item.Type,
                    District = item.District,
                    CreatorID = item.CreatorID,
                    ModifierID = item.CreatorID,
                    CreateDate = item.CreateDate,
                    ModifyDate = item.ModifyDate,
                    Status = (int)item.Status,
                    Summary = item.Summary,
                });

                reponsitory.Insert(data);
            }
        }

    }

    #region _bak
    //public class EnterprisesRoll : QueryView<Enterprise, PvFinanceReponsitory>
    //{
    //    public EnterprisesRoll()
    //    {
    //    }

    //    protected EnterprisesRoll(PvFinanceReponsitory reponsitory, IQueryable<Enterprise> iQueryable) : base(reponsitory, iQueryable)
    //    {
    //    }

    //    protected override IQueryable<Enterprise> GetIQueryable()
    //    {
    //        return new EnterprisesOrigin(this.Reponsitory);
    //    }

    //    /// <summary>
    //    /// 分页方法
    //    /// </summary>
    //    /// <returns></returns>
    //    public object ToMyPage(int? pageIndex = null, int? pageSize = null)
    //    {
    //        IQueryable<Enterprise> iquery = this.IQueryable.Cast<Enterprise>().OrderByDescending(item => item.CreateDate);
    //        int total = iquery.Count();

    //        if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
    //        {
    //            iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
    //        }

    //        //获取数据
    //        var ienum_myEnterprise = iquery.ToArray();

    //        //CreatorIDs
    //        var creatorIDs = ienum_myEnterprise.Select(item => item.CreatorID);

    //        #region 创建人姓名

    //        var creatorAdminsTopView = new AdminsTopView(this.Reponsitory);

    //        var linq_creator = from creator in creatorAdminsTopView
    //                           where creatorIDs.Contains(creator.ID)
    //                           select new
    //                           {
    //                               CreatorID = creator.ID,
    //                               CreatorName = creator.RealName,
    //                           };

    //        var ienums_creator = linq_creator.ToArray();

    //        #endregion

    //        var ienums_linq = from enterprise in ienum_myEnterprise
    //                          join creator in ienums_creator on enterprise.CreatorID equals creator.CreatorID into ienums_creator2
    //                          from creator in ienums_creator2.DefaultIfEmpty()
    //                          select new Enterprise
    //                          {
    //                              ID = enterprise.ID,
    //                              Name = enterprise.Name,
    //                              Type = enterprise.Type,
    //                              District = enterprise.District,
    //                              CreatorID = enterprise.CreatorID,
    //                              ModifierID = enterprise.ModifierID,
    //                              CreateDate = enterprise.CreateDate,
    //                              ModifyDate = enterprise.ModifyDate,
    //                              Status = enterprise.Status,
    //                              Summary = enterprise.Summary,

    //                              CreatorName = creator != null ? creator.CreatorName : "",
    //                          };

    //        var results = ienums_linq.ToArray();

    //        Func<Enterprise, object> convert = item => new
    //        {
    //            EnterpriseID = item.ID,
    //            Name = item.Name,
    //            DistrictName = ((Origin)Enum.Parse(typeof(Origin), item.District)).GetDescription(),
    //            TypeName = item.Type.GetDescription(),
    //            CreatorName = item.CreatorName,
    //            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
    //            StatusName = item.Status.GetDescription(),
    //        };

    //        if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
    //        {
    //            Func<dynamic, object> convertAgain = item => new
    //            {

    //            };

    //            return results.Select(convert).Select(convertAgain).ToArray();
    //        }

    //        return new
    //        {
    //            total = total,
    //            Size = pageSize ?? 20,
    //            Index = pageIndex ?? 1,
    //            rows = results.Select(convert).ToArray(),
    //        };
    //    }

    //    /// <summary>
    //    /// 根据往来单位名称查询
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <returns></returns>
    //    public EnterprisesRoll SearchByName(string name)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.Name.Contains(name)
    //                   select query;

    //        var view = new EnterprisesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 根据地区查询
    //    /// </summary>
    //    /// <param name="district"></param>
    //    /// <returns></returns>
    //    public EnterprisesRoll SearchByDistrict(string district)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.District == district
    //                   select query;

    //        var view = new EnterprisesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 根据状态查询
    //    /// </summary>
    //    /// <param name="status"></param>
    //    /// <returns></returns>
    //    public EnterprisesRoll SearchByStatus(GeneralStatus status)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.Status == status
    //                   select query;

    //        var view = new EnterprisesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 批量启用
    //    /// </summary>
    //    /// <param name="ids"></param>
    //    public void Enable(string[] ids)
    //    {
    //        using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
    //        {
    //            reponsitory.Update<Layers.Data.Sqls.PvFinance.Enterprises>(new
    //            {
    //                ModifyDate = DateTime.Now,
    //                Status = (int)GeneralStatus.Normal,
    //            }, item => ids.Contains(item.ID));
    //        }
    //    }

    //    /// <summary>
    //    /// 批量禁用
    //    /// </summary>
    //    /// <param name="ids"></param>
    //    public void Disable(string[] ids)
    //    {
    //        using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
    //        {
    //            reponsitory.Update<Layers.Data.Sqls.PvFinance.Enterprises>(new
    //            {
    //                ModifyDate = DateTime.Now,
    //                Status = (int)GeneralStatus.Closed,
    //            }, item => ids.Contains(item.ID));
    //        }
    //    }

    //}
    #endregion
}
