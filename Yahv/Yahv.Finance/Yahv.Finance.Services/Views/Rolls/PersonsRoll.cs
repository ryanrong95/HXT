using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Linq;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 人员管理视图
    /// </summary>
    public class PersonsRoll : QueryView<Person, PvFinanceReponsitory>
    {
        public PersonsRoll()
        {
        }

        public PersonsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Person> GetIQueryable()
        {
            var persons = new PersonsOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);

            return from entity in persons
                   join admin in admins on entity.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Person()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       Status = entity.Status,
                       CreatorID = entity.CreatorID,
                       RealName = entity.RealName,
                       ModifierID = entity.ModifierID,
                       ModifyDate = entity.ModifyDate,
                       CreatorName = admin.RealName,
                       Gender = entity.Gender,
                       IDCard = entity.IDCard,
                       Mobile = entity.Mobile,
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Person this[string personId]
        {
            get { return this.Single(item => item.ID == personId); }
        }

        /// <summary>
        /// 批量启用
        /// </summary>
        /// <param name="ids"></param>
        public void Enable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Persons>(new
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
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Persons>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => ids.Contains(item.ID));
            }
        }
    }
}
