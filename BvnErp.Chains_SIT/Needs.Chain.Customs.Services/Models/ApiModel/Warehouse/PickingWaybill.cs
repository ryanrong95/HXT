using System;
using System.Collections.Generic;
using System.Linq;
using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;

namespace Needs.Ccs.Services.Models
{
    public class PickingWaybill : Waybills
    {
        public DateTime? AppointTime { get; set; }

        public bool IsAuto { get; set; }
        public string InitExType { get; set; }
        public string InitPayType { get; set; }
        public PickingNotice[] Notices { get; set; }

        public Express Express { get; set; }

        public CgNoticeType NoticeType { get; set; }

        /// <summary>
        /// 运费支付人
        /// </summary>
        public WaybillPayer FreightPayer { get; set; }

        /// <summary>
        /// 运单冗余执行状态
        /// </summary>
        /// <remarks>
        /// 冗余字段
        /// </remarks>
        public CgPickingExcuteStatus? ExcuteStatus { get; set; }

        public string ExcuteStatusDescription
        {
            get
            {
                return this.ExcuteStatus.GetDescription();
            }
        }

        public string TotalGoodsValue { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public decimal TotalPieces { get; set; }


        private CenterFileDescription[] files;
        public CenterFileDescription[] Files
        {
            get
            {
                files = new CenterFileDescription[] { };
                if (files == null && this.DataFiles != null)
                {
                    files = this.DataFiles.Where(item => item.NoticeID == null).ToArray();
                }

                return files;
            }
            set { files = value; }
        }

        string orderID;

        public string OrderID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    return this.orderID;
                }

                if (this.Notices != null)
                {
                    return string.Join(",", this.Notices.Select(item => item.Output.OrderID).Distinct());
                }
                return this.orderID;
            }
            set
            {
                this.orderID = value;
            }
        }



        public string WaybillTypeDescription
        {
            get
            {
                return this.WaybillType.GetDescription();
            }
        }

        public CgNoticeSource Source { get; set; }

        public string SourceDescription
        {
            get
            {
                return this.Source.GetDescription();
            }
        }

        public string PlaceID
        {
            get
            {
                if (string.IsNullOrEmpty(this.Place))
                { return ""; }
                var index = (int)Enum.GetValues(typeof(Origin)).Cast<Origin>().Where(item => item.GetOrigin().Code == this.Place).FirstOrDefault();
                return index.ToString();
            }
        }

        public string PlaceDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.Place))
                { return ""; }
                var origins = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item => item.GetOrigin());
                origins = origins.Where(item => item.Code == this.Place);
                if (origins.Count() == 0)
                {
                    try
                    {
                        return ((Origin)int.Parse(this.Place)).GetOrigin().ChineseName;
                    }
                    catch
                    {
                        return "";
                    }
                }

                var name = origins.FirstOrDefault().ChineseName;
                return name;
            }
        }

        public WayCondition Conditions
        {
            get
            {
                if (Condition != null)
                {
                    return this.Condition.JsonTo<WayCondition>();
                }
                else
                {

                    return null;
                }
            }
        }

    }

    public class Express
    {
        public string ExType { get; set; }

        public string ExPayType { get; set; }

        public string ThirdPartyCardNo { get; set; }
    }
}
