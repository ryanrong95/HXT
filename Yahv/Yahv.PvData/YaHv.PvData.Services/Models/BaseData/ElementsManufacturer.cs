using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Handlers;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 申报要素品牌
    /// </summary>
    public class ElementsManufacturer
    {
        #region 属性
        /// <summary>
        /// 唯一码，Md5(Manufacturer)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 归类品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 申报要素中对应的中文或外文品牌
        /// </summary>
        public string MfrMapping { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public string CnEn
        {
            get
            {
                return $"{MfrMapping}/{Manufacturer}牌";
            }
        }
        #endregion

        #region 操作人

        internal Admin Admin;

        public void SetAdmin(Admin admin)
        {
            this.Admin = admin;
        }

        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ElementsMfrChangedHandler MfrChanged;
        #endregion

        public ElementsManufacturer()
        {
            this.MfrChanged += OnMfrChanged;
        }

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                this.ID = this.Manufacturer.ToLower().MD5();
                var em = new Views.Alls.ElementsManufacturersAll(repository).SingleOrDefault(item => item.ID == this.ID);
                if (em == null)
                {
                    repository.Insert(new Layers.Data.Sqls.PvData.ElementsManufacturers()
                    {
                        ID = this.ID,
                        Manufacturer = this.Manufacturer,
                        MfrMapping = this.MfrMapping,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    });

                    if (this.MfrChanged != null)
                        this.MfrChanged(this, new ElementsMfrChangedEventArgs(this, null, this.MfrMapping));
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvData.ElementsManufacturers>(new
                    {
                        MfrMapping = this.MfrMapping,
                        ModifyDate = DateTime.Now
                    }, a => a.ID == this.ID);

                    if (this.MfrChanged != null)
                        this.MfrChanged(this, new ElementsMfrChangedEventArgs(this, em.MfrMapping, this.MfrMapping));
                }
            }
        }

        #endregion

        void OnMfrChanged(object sender, ElementsMfrChangedEventArgs e)
        {
            var em = e.EM;
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                //新增品牌
                if (string.IsNullOrEmpty(e.From))
                {
                    em.Log(e.From, e.To, $"报关员【{em.Admin.RealName}】新增品牌【{em.Manufacturer}】，中文或外文名称【{em.MfrMapping}】", repository);
                }
                //修改品牌
                else
                {
                    em.Log(e.From, e.To, $"报关员【{em.Admin.RealName}】将品牌【{em.Manufacturer}】的中文或外文名称由【{e.From}】修改为【{e.To}】", repository);
                }
            }

            if (!string.IsNullOrEmpty(e.From))
            {
                //修正归类历史记录
                var task1 = Task.Run(() =>
                {
                    SqlView.UpdateClassifiedHistories(em.Manufacturer, e.From, e.To);
                    using (var repository = LinqFactory<PvDataReponsitory>.Create())
                    {
                        em.Log(e.From, e.To, $"报关员【Npc-Robot】修正了品牌【{em.Manufacturer}】的归类历史记录", repository);
                    }
                });

                //修正产品预归类数据
                var task2 = Task.Run(() =>
                {
                    SqlView.UpdatePreProductCategories(em.Manufacturer, e.From, e.To);
                    using (var repository = LinqFactory<PvDataReponsitory>.Create())
                    {
                        em.Log(e.From, e.To, $"报关员【Npc-Robot】修正了品牌【{em.Manufacturer}】的产品预归类数据", repository);
                    }
                });
            }
        }
    }
}
