using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DecChargeStandard : IUnique
    {
        #region 属性

        /// <summary>
        ///
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? EnumValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? SerialNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.ChargeStandardType? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsMenuLeaf { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Remark1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark2 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Summary { get; set; }

        #endregion

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var Reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (!Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecChargeStandards>().Any(item => item.ID == this.ID))
                {
                    Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecChargeStandards
                    {
                        ID = this.ID,
                        FatherID = this.FatherID,
                        EnumValue = this.EnumValue,
                        SerialNo = this.SerialNo,
                        Type = this.Type != null ? (int?)this.Type : null,
                        IsMenuLeaf = this.IsMenuLeaf,
                        Name = this.Name,
                        Unit1 = this.Unit1,
                        Unit2 = this.Unit2,
                        Price = this.Price,
                        Remark1 = this.Remark1,
                        Remark2 = this.Remark2,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    Reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecChargeStandards>(new
                    {
                        FatherID = this.FatherID,
                        EnumValue = this.EnumValue,
                        SerialNo = this.SerialNo,
                        Type = this.Type != null ? (int?)this.Type : null,
                        IsMenuLeaf = this.IsMenuLeaf,
                        Name = this.Name,
                        Unit1 = this.Unit1,
                        Unit2 = this.Unit2,
                        Price = this.Price,
                        Remark1 = this.Remark1,
                        Remark2 = this.Remark2,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}
