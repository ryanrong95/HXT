using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{
    public class Driver : Yahv.Linq.IUnique
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.EnterpriseID, this.Name, this.IDCard, this.Mobile).MD5();

            }
            set
            {
                this.id = value;
            }
        }
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { set; get; }
        /// <summary>
        /// 大陆手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 香港手机号
        /// </summary>
        //public string HKMobile { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 其他手机号：香港或其他地区
        /// </summary>

        public string Mobile2 { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>

        public string CustomsCode { set; get; }
        /// <summary>
        /// 司机卡号
        /// </summary>

        public string CardCode { set; get; }
        /// <summary>
        /// 口岸电子编号
        /// </summary>

        public string PortCode { set; get; }
        /// <summary>
        /// 寮步密码
        /// </summary>

        public string LBPassword { set; get; }
        #endregion
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.Drivers>().Any(item => item.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.Drivers
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        Name = this.Name,
                        IDCard = this.IDCard,
                        Mobile = this.Mobile,
                        Status = (int)this.Status,
                        Creator = "",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,

                        Mobile2 = this.Mobile2,
                        CustomsCode = this.CustomsCode,
                        CardCode = this.CardCode,
                        PortCode = this.PortCode,
                        LBPassword = this.LBPassword
                    });
                }

            }
        }
    }
}
