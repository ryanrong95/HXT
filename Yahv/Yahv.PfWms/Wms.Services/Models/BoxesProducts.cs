using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Views;
using Yahv.Linq.Extends;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Wms.Services.Models
{

    public class TinyOrder
    {
        public string TinyOrderID { get; set; }
        BoxesProducts[] BoxProducts { get; set; }
    }

    public class DataBoxesProducts : Boxes
    {



        public string ShelveID { get; set; }


        public int BoxingSpecs { get; set; }

        public PickingNotice[] Notices { get; set; }

        public decimal TotalWeight { get; set; }

        public string EnterCode { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsCIQ { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool IsEmbargo { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        public bool IsHighPrice { get; set; }

        public decimal TotalParts { get; set; }


        public static implicit operator BoxesProducts(DataBoxesProducts entity)
        {
            return new BoxesProducts
            {
                AdminID = entity.AdminID,
                ID = entity.ID,
                BoxingSpecs = (BoxingSpecs)entity.BoxingSpecs,
                Code = entity.Code,
                CodePrefix = entity.CodePrefix,
                CreateDate = entity.CreateDate,
                Notices = entity.Notices,
                ShelveID = entity.ShelveID,
                DateStr = entity.DateStr,
                Summary = entity.Summary,
                WarehouseID = entity.WarehouseID,    
                TotalWeight=entity.TotalWeight,
                Status = entity.Status,
                TotalParts=entity.TotalParts,
                IsCCC=entity.IsCCC,
                IsCIQ=entity.IsCIQ,
                IsEmbargo=entity.IsEmbargo,
                IsHighPrice=entity.IsHighPrice,
                EnterCode=entity.EnterCode                
                
            };
        }




    }

    public class BoxesProducts : Boxes
    {
        public string ShelveID { get; set; }

        public BoxingSpecs BoxingSpecs { get; set; }

        public PickingNotice[] Notices { get; set; }

        public decimal TotalWeight { get; set; }
        public string EnterCode { get; set; }
        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsCIQ { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool IsEmbargo { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        public bool IsHighPrice { get; set; }

        public decimal TotalParts { get; set; }

        public string BoxingSpecsDescription
        {
            get
            {
                return this.BoxingSpecs.GetDescription();
            }
        }
    }
}
