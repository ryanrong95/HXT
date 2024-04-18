using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class TakersView : QueryView<Taker, PsWmsRepository>
    {
        #region 构造函数

        public TakersView()
        {
        }

        public TakersView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Taker> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Takers>()
                       select new Taker
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           Type = (TakerType)entity.Type,
                           IsTrustee = entity.IsTrustee,
                           Licence = entity.Licence,
                           Phone = entity.Phone,
                           Status = (Underly.GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };

            return view;
        }

        /// <summary>
        /// 保存新建的拿货人
        /// </summary>
        /// <param name="jobject"></param>
        public void Enter(JObject jobject)
        {
            var name = jobject["Name"].Value<string>().Trim();
            var type = jobject["Type"].Value<int>();
            var licence = jobject["Licence"].Value<string>();
            var isTrustee = jobject["isTrustee"].Value<bool>();
            var phone = jobject["Phone"].Value<string>();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(licence) || string.IsNullOrEmpty(phone))
            {
                throw new Exception("参数 Name, Licence, Phone 不能为空");
            }

            var view = this.IQueryable;
            if (view.Any(item => item.Name == name))
            {
                throw new Exception($"司机{name}已经存在,不能填加");
            }

            this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Takers
            {
                ID = PKeySigner.Pick(PKeyType.Taker),
                IsTrustee = isTrustee,
                Type = type,
                Name = name,
                Licence = licence,
                Phone = phone,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = (int)Underly.GeneralStatus.Normal,
            });
        }

        /// <summary>
        /// 修改Taker信息
        /// </summary>
        /// <param name="jobject"></param>
        public void Modify(JObject jobject)
        {
            var name = jobject["Name"].Value<string>().Trim();
            var licence = jobject["Licence"].Value<string>();            
            var phone = jobject["Phone"].Value<string>();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(licence) || string.IsNullOrEmpty(phone))
            {
                throw new Exception("参数 Name, Licence, Phone 不能为空");
            }

            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Takers>(new
            {
                Licence = licence,
                Phone = phone,
            }, item => item.Name == name);
        }
    }
}
