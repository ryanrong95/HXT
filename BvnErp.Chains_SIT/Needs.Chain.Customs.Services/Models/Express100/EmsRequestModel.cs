using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Ccs.Services.Models
{
    [XmlRoot(ElementName = "OrderNormal")]
    public class EmsRequestModel
    {

        public EmsRequestModel()
        {
           
            this.CreatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.LogisticsProvider = "B";//快递
            this.EcommerceNo = System.Configuration.ConfigurationManager.AppSettings["EMSecCompanyId"];//渠道来源标识  FORIC
            this.EcommerceUserId = ChainsGuid.NewGuidUp();//电商客户标识
            this.SenderType = "1";//协议客户
            this.SenderNo = System.Configuration.ConfigurationManager.AppSettings["EMSMonthlyCard"];
            this.InnerChannel = string.Empty;
            //this.LogisticsOrderNo = string.Empty;//订单编号 必填
            this.BatchNo = string.Empty;
            this.WaybillNo = string.Empty;
            this.OneBillFlag = string.Empty;
            this.SubmailNo = string.Empty;
            this.OneBillFeeType = string.Empty;
            this.ContentsAttribute = "1";//内件性质。发票默认 1 文件
            this.BaseProductNo = "1";//基础产品代码。邮寄发票默认 1 标准快递
            this.BizProductNo = string.Empty;
            this.Weight = string.Empty;
            this.Volume = string.Empty;
            this.Length = string.Empty;
            this.Width = string.Empty;
            this.Height = string.Empty;
            this.PostageTotal = string.Empty;
            this.PickupNotes = string.Empty;
            this.InsuranceFlag = string.Empty;
            this.InsuranceAmount = string.Empty;
            this.DeliverType = string.Empty;
            this.DeliverPreDate = string.Empty;
            this.PickupType = string.Empty;
            this.PickupPreBeginTime = string.Empty;
            this.PickupPreEndTime = string.Empty;
            //付款方式
            //1:寄件人 2:收件人 3:第三方 4:收件人集中付费 5:免费 6:寄/收件人 7:预付卡
            //邮寄发票默认 1
            this.PaymentMode = "1";
            this.CodFlag = string.Empty;
            this.CodAmount = string.Empty;
            this.ReceiptFlag = string.Empty;
            this.ReceiptWaybillNo = string.Empty;
            this.ElectronicPreferentialNo = string.Empty;
            this.ElectronicPreferentialAmount = string.Empty;
            this.ValuableFlag = string.Empty;
            this.SenderSafetyCode = string.Empty;
            this.ReceiverSafetyCode = string.Empty;
            this.Note = string.Empty;
            this.ProjectId = string.Empty;
        }


        /// <summary>
        /// <created_time>2020-01-15 17:28:04</created_time>
        /// </summary>
        [XmlElement(ElementName = "created_time")]
        public string CreatedTime { get; set; }

        //<logistics_provider>B</logistics_provider>
        [XmlElement(ElementName = "logistics_provider")]
        public string LogisticsProvider { get; set; }

        //<ecommerce_no>CQFSK</ecommerce_no>
        [XmlElement(ElementName = "ecommerce_no")]
        public string EcommerceNo { get; set; }

        //<ecommerce_user_id>2</ecommerce_user_id>
        [XmlElement(ElementName = "ecommerce_user_id")]
        public string EcommerceUserId { get; set; }

        //<sender_type>1</sender_type>
        [XmlElement(ElementName = "sender_type")]
        public string SenderType { get; set; }

        //<sender_no>90000004235330</sender_no>
        [XmlElement(ElementName = "sender_no")]
        public string SenderNo { get; set; }

        //<inner_channel>0</inner_channel>
        [XmlElement(ElementName = "inner_channel")]
        public string InnerChannel { get; set; }

        //<logistics_order_no>816047776752203569</logistics_order_no>
        [XmlElement(ElementName = "logistics_order_no")]
        public string LogisticsOrderNo { get; set; }

        //<batch_no></batch_no>
        [XmlElement(ElementName = "batch_no")]
        public string BatchNo { get; set; }

        //<waybill_no></waybill_no>
        [XmlElement(ElementName = "waybill_no")]
        public string WaybillNo { get; set; }

        //<one_bill_flag></one_bill_flag>
        [XmlElement(ElementName = "one_bill_flag")]
        public string OneBillFlag { get; set; }

        //<submail_no></submail_no>
        [XmlElement(ElementName = "submail_no")]
        public string SubmailNo { get; set; }

        //<one_bill_fee_type></one_bill_fee_type>
        [XmlElement(ElementName = "one_bill_fee_type")]
        public string OneBillFeeType { get; set; }

        //<contents_attribute>3</contents_attribute>
        [XmlElement(ElementName = "contents_attribute")]
        public string ContentsAttribute { get; set; }

        //<base_product_no>1</base_product_no>
        [XmlElement(ElementName = "base_product_no")]
        public string BaseProductNo { get; set; }

        //<biz_product_no></biz_product_no>
        [XmlElement(ElementName = "biz_product_no")]
        public string BizProductNo { get; set; }

        //<weight>10</weight>
        [XmlElement(ElementName = "weight")]
        public string Weight { get; set; }

        //<volume></volume>
        [XmlElement(ElementName = "volume")]
        public string Volume { get; set; }

        //<length></length>
        [XmlElement(ElementName = "length")]
        public string Length { get; set; }

        //<width></width>
        [XmlElement(ElementName = "width")]
        public string Width { get; set; }

        //<height></height>
        [XmlElement(ElementName = "height")]
        public string Height { get; set; }

        //<postage_total></postage_total>
        [XmlElement(ElementName = "postage_total")]
        public string PostageTotal { get; set; }

        //<pickup_notes></pickup_notes>
        [XmlElement(ElementName = "pickup_notes")]
        public string PickupNotes { get; set; }

        //<insurance_flag>1</insurance_flag>
        [XmlElement(ElementName = "insurance_flag")]
        public string InsuranceFlag { get; set; }

        //<insurance_amount></insurance_amount>
        [XmlElement(ElementName = "insurance_amount")]
        public string InsuranceAmount { get; set; }

        //<deliver_type></deliver_type>
        [XmlElement(ElementName = "deliver_type")]
        public string DeliverType { get; set; }

        //<deliver_pre_date></deliver_pre_date>
        [XmlElement(ElementName = "deliver_pre_date")]
        public string DeliverPreDate { get; set; }

        //<pickup_type></pickup_type>
        [XmlElement(ElementName = "pickup_type")]
        public string PickupType { get; set; }

        //<pickup_pre_begin_time></pickup_pre_begin_time>
        [XmlElement(ElementName = "pickup_pre_begin_time")]
        public string PickupPreBeginTime { get; set; }

        //<pickup_pre_end_time></pickup_pre_end_time>
        [XmlElement(ElementName = "pickup_pre_end_time")]
        public string PickupPreEndTime { get; set; }

        //<payment_mode>1</payment_mode>
        [XmlElement(ElementName = "payment_mode")]
        public string PaymentMode { get; set; }

        //<cod_flag></cod_flag>
        [XmlElement(ElementName = "cod_flag")]
        public string CodFlag { get; set; }

        //<cod_amount></cod_amount>
        [XmlElement(ElementName = "cod_amount")]
        public string CodAmount { get; set; }

        //<receipt_flag>1</receipt_flag>
        [XmlElement(ElementName = "receipt_flag")]
        public string ReceiptFlag { get; set; }

        //<receipt_waybill_no></receipt_waybill_no>
        [XmlElement(ElementName = "receipt_waybill_no")]
        public string ReceiptWaybillNo { get; set; }

        //<electronic_preferential_no></electronic_preferential_no>
        [XmlElement(ElementName = "electronic_preferential_no")]
        public string ElectronicPreferentialNo { get; set; }

        //<electronic_preferential_amount></electronic_preferential_amount>
        [XmlElement(ElementName = "electronic_preferential_amount")]
        public string ElectronicPreferentialAmount { get; set; }

        //<valuable_flag>0</valuable_flag>
        [XmlElement(ElementName = "valuable_flag")]
        public string ValuableFlag { get; set; }

        //<sender_safety_code>0</sender_safety_code>
        [XmlElement(ElementName = "sender_safety_code")]
        public string SenderSafetyCode { get; set; }

        //<receiver_safety_code></receiver_safety_code>
        [XmlElement(ElementName = "receiver_safety_code")]
        public string ReceiverSafetyCode { get; set; }

        //<note></note>
        [XmlElement(ElementName = "note")]
        public string Note { get; set; }

        //<project_id></project_id>
        [XmlElement(ElementName = "project_id")]
        public string ProjectId { get; set; }





        /// <summary>
        /// 寄件人
        /// </summary>
        [XmlElement(ElementName = "sender")]
        public EmsSender Sender { get; set; }

        /// <summary>
        /// 拣货人，留空
        /// </summary>
        [XmlElement(ElementName = "pickup")]
        public EmsSender Pickup { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        [XmlElement(ElementName = "receiver")]
        public EmsSender Receiver { get; set; }

        /// <summary>
        /// 货物信息
        /// </summary>
        [XmlElement(ElementName = "cargos")]
        public Cargos Cargos { get; set; }

    }


    public class EmsSender
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "post_code")]
        public string PostCode { get; set; }

        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }

        [XmlElement(ElementName = "mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "prov")]
        public string Prov { get; set; }

        [XmlElement(ElementName = "city")]
        public string City { get; set; }

        [XmlElement(ElementName = "county")]
        public string County { get; set; }

        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
    }

    public class Cargos
    {
        [XmlElement(ElementName = "Cargo")]
        public List<Cargo> Cargo { get; set; }
    }

    public class Cargo
    {

        [XmlElement(ElementName = "cargo_name")]
        public string CargoName { get; set; }

        [XmlElement(ElementName = "cargo_category")]
        public string CargoCategory { get; set; }

        [XmlElement(ElementName = "cargo_quantity")]
        public string CargoQuantity { get; set; }

        [XmlElement(ElementName = "cargo_value")]
        public string CargoValue { get; set; }

        [XmlElement(ElementName = "cargo_weight")]
        public string CargoWeight { get; set; }
    }
}
