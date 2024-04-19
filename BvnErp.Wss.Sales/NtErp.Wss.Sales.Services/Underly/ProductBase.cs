using NtErp.Wss.Sales.Services.Underly.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 实际product运营规范
    /// </summary>
    abstract public class ProductBase : StandardProduct, IMain
    {
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override string ID
        {
            get
            {
                return this.GenID(this.Name
                    , this.B1bSign
                    , this.Manufacturer
                    , this.PackageCase
                    , this.Packaging
                    , this.Origin
                    , this.Supplier
                    , this.Batch
                    , this.DistributorSign);
            }
            set
            {
                base.ID = value;
            }
        }

        /// <summary>
        /// 分销商标识
        /// </summary>
        [OldNaming("Sign")]
        [StandardNaming("Distributor Part Number")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string DistributorSign
        {
            get { return this[nameof(this.DistributorSign)]; }
            set
            {
                this[nameof(this.DistributorSign)] = value;
            }
        }

        /// <summary>
        /// 封装
        /// </summary>
        /// <example>
        /// Tray
        /// reel
        /// Tape 
        /// tube
        /// </example>
        [OldNaming("Postting")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string PackageCase
        {
            get { return this["Package / Case"]; }
            set
            {
                this["Package / Case"] = value;
            }
        }

        /// <summary>
        /// 包装
        /// </summary>
        [OldNaming("Packing")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Packaging
        {
            get { return this[nameof(this.Packaging)]; }
            set
            {
                this[nameof(this.Packaging)] = value;
            }
        }

        /// <summary>
        /// 库存数量
        /// </summary>
        [StandardNaming("Quantity Available")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public int Count
        {
            get
            {
                int outs;
                if (int.TryParse(this[nameof(this.Count)], out outs))
                {
                    return outs;
                }
                return 0;
            }
            set
            {
                this[nameof(this.Count)] = value.ToString();
            }
        }

        /// <summary>
        /// 批次
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Batch
        {
            get { return this[nameof(this.Batch)]; }
            set
            {
                this[nameof(this.Batch)] = value;
            }
        }

        /// <summary>
        /// 原产地
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Origin
        {
            get { return this[nameof(this.Origin)]; }
            set
            {
                this[nameof(this.Origin)] = value;
            }
        }

        /// <summary>
        /// 最小起订量
        /// </summary>
        [OldNaming("MOQ")]
        [StandardNaming("MOQ")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public int Moq
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this[nameof(this.Moq)]))
                {
                    return 1;
                }
                return int.Parse(this[nameof(this.Moq)]);
            }
            set
            {
                this[nameof(this.Moq)] = value.ToString();
            }
        }

        /// <summary>
        /// 阶梯跳跃量
        /// </summary>
        [OldNaming("Jump")]
        [StandardNaming("Jump")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public int Jump
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this[nameof(this.Jump)]))
                {
                    return 1;
                }
                return int.Parse(this[nameof(this.Jump)]);
            }
            set
            {
                this[nameof(this.Jump)] = value.ToString();
            }
        }
        /// <summary>
        /// 最小金额限制
        /// </summary>
        [StandardNaming("Minimum amount limit")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public decimal Mal
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this[nameof(this.Mal)]))
                {
                    return 0M;
                }
                return decimal.Parse(this[nameof(this.Mal)]);
            }
            set
            {
                this[nameof(this.Mal)] = value.ToString();
            }
        }

        /// <summary>
        /// 库存地
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public District District
        {
            get
            {
                District outs;
                if (Enum.TryParse(this[nameof(this.District)], out outs))
                {
                    return outs;
                }

                return District.Unknown;
            }
            set
            {
                this[nameof(this.District)] = Enum.GetName(typeof(District), value);
            }
        }

        /// <summary>
        /// 交货期
        /// </summary>
        [OldNaming("Delivery")]
        [StandardNaming("Lead-time")]
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Leadtime
        {
            get { return this[nameof(this.Leadtime)]; }
            set
            {
                this[nameof(this.Leadtime)] = value;
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Description
        {
            get { return this[nameof(this.Description)]; }
            set
            {
                this[nameof(this.Description)] = value;
            }
        }

        /// <summary>
        /// 摘要
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Summary
        {
            get { return this[nameof(this.Summary)]; }
            set
            {
                this[nameof(this.Summary)] = value;
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SelfStatus Status { get; set; }

        public string Supplier { get; set; }
        /// <summary>
        /// 期货
        /// </summary>
        public bool Futures { get; set; }

        public ProductBase()
        {
            //如果要是使用环境变量的化就需要 用开发
            //var district = InRuntime<Builder>.Current.District;

            this.Catalogs = new Catalogs();
            this.Status = SelfStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;

            this.Embargos = new Embargos();
        }
    }
}
