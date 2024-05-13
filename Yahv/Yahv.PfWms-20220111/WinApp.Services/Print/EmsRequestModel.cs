using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WinApp.Services.Print
{
    [XmlRoot(ElementName = "OrderNormal")]
    public class EmsRequestModel
    {

        public EmsRequestModel()
        {

            //this.CreatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //this.LogisticsProvider = "B";//快递
            //this.EcommerceNo = "DKH";//渠道来源标识
            //this.EcommerceUserId = "2";//电商客户标识
            //this.SenderType = 1;//协议客户
            //this.SenderNo = string.Empty;
            //this.InnerChannel = string.Empty;
            ////this.LogisticsOrderNo = string.Empty;//订单编号 必填
            //this.BatchNo = string.Empty;
            //this.WaybillNo = string.Empty;
            //this.OneBillFlag = string.Empty;
            //this.SubmailNo = string.Empty;
            //this.OneBillFeeType = string.Empty;
            //this.ContentsAttribute = "1";//内件性质。发票默认 1 文件
            //this.BaseProductNo = "1";//基础产品代码。邮寄发票默认 1 标准快递
            //this.BizProductNo = string.Empty;
            //this.Weight = string.Empty;
            //this.Volume = string.Empty;
            //this.Length = string.Empty;
            //this.Width = string.Empty;
            //this.Height = string.Empty;
            //this.PostageTotal = string.Empty;
            //this.PickupNotes = string.Empty;
            //this.InsuranceFlag = string.Empty;
            //this.InsuranceAmount = string.Empty;
            //this.DeliverType = string.Empty;
            //this.DeliverPreDate = string.Empty;
            //this.PickupType = string.Empty;
            //this.PickupPreBeginTime = string.Empty;
            //this.PickupPreEndTime = string.Empty;
            ////付款方式
            ////1:寄件人 2:收件人 3:第三方 4:收件人集中付费 5:免费 6:寄/收件人 7:预付卡
            ////邮寄发票默认 1
            //this.PaymentMode = "1";
            //this.CodFlag = string.Empty;
            //this.CodAmount = string.Empty;
            //this.ReceiptFlag = string.Empty;
            //this.ReceiptWaybillNo = string.Empty;
            //this.ElectronicPreferentialNo = string.Empty;
            //this.ElectronicPreferentialAmount = string.Empty;
            //this.ValuableFlag = string.Empty;
            //this.SenderSafetyCode = string.Empty;
            //this.ReceiverSafetyCode = string.Empty;
            //this.Note = string.Empty;
            //this.ProjectId = string.Empty;
        }


        /// <summary>
        ///  订单接入时间 <created_time>2020-01-15 17:28:04</created_time>
        /// </summary>
        /// <remarks>必填</remarks>
        [XmlElement(ElementName = "created_time")]
        public string CreatedTime { get; set; }

        /// <summary>
        /// 物流承运方
        /// </summary>
        /// <remarks>必填 A：邮务 B：速递</remarks>
        //<logistics_provider>B</logistics_provider>
        [XmlElement(ElementName = "logistics_provider")]
        public string LogisticsProvider { get; set; }

        /// <summary>
        /// 渠道来源标识
        /// </summary>
        /// <remarks>必填 大多渠道以拼音首字母为准，例如：仓库配送（CKPS），个别渠道以定义的标准为主，例如：大客户（DKH）。</remarks>
        //<ecommerce_no>CQFSK</ecommerce_no>
        [XmlElement(ElementName = "ecommerce_no")]
        public string EcommerceNo { get; set; }

        /// <summary>
        /// 电商客户标识
        /// </summary>
        /// <remarks>必填 可以填一个<50位随机数</remarks>
        //<ecommerce_user_id>2</ecommerce_user_id>
        [XmlElement(ElementName = "ecommerce_user_id")]
        public string EcommerceUserId { get; set; }

        /// <summary>
        /// 客户类型
        /// </summary>
        /// <remarks>非必填 0 散户 1协议客户（默认为1）</remarks>
        //<sender_type>1</sender_type>
        [XmlElement(ElementName = "sender_type")]
        public int SenderType { get; set; }

        /// <summary>
        /// 协议客户代码
        /// </summary>
        /// <remarks>非必填 如果有协议客户号，请填写</remarks>
        //<sender_no>90000004235330</sender_no>
        [XmlElement(ElementName = "sender_no")]
        public string SenderNo { get; set; }

        /// <summary>
        /// 内部订单来源标识
        /// </summary>
        /// <remarks>非必填0：直接对接 1：邮务国内小包订单系统 2：邮务国际小包订单系统 3：速递国内订单系统 4：速递国际订单系统（shipping） 5：在线发货平台------默认为’0’</remarks>
        //<inner_channel>0</inner_channel>
        [XmlElement(ElementName = "inner_channel")]
        public string InnerChannel { get; set; }

        /// <summary>
        /// 物流订单号
        /// </summary>
        /// <remarks>必填</remarks>
        //<logistics_order_no>816047776752203569</logistics_order_no>
        [XmlElement(ElementName = "logistics_order_no")]
        public string LogisticsOrderNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        /// <remarks>非必填</remarks>
        //<batch_no></batch_no>
        [XmlElement(ElementName = "batch_no")]
        public string BatchNo { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        /// <remarks>非必填</remarks>
        //<waybill_no></waybill_no>
        [XmlElement(ElementName = "waybill_no")]
        public string WaybillNo { get; set; }

        /// <summary>
        /// 一票多件标识
        /// </summary>
        /// <remarks>非必填 一票多件标志:0正常 1一票多件</remarks>
        //<one_bill_flag></one_bill_flag>
        [XmlElement(ElementName = "one_bill_flag")]
        public string OneBillFlag { get; set; }

        /// <summary>
        /// 子单数量（reserved28）
        /// </summary>
        /// <remarks>非必填 最大‘一主九子’，即submail_no<=9</remarks>
        //<submail_no></submail_no>
        [XmlElement(ElementName = "submail_no")]
        public string SubmailNo { get; set; }

        /// <summary>
        /// 一票多件计费方式
        /// </summary>
        /// <remarks>非必填 一票多件计费方式:1主单统一计费 2分单免首重计费 3平均重量计费 4主分单单独计费</remarks>
        //<one_bill_fee_type></one_bill_fee_type>
        [XmlElement(ElementName = "one_bill_fee_type")]
        public string OneBillFeeType { get; set; }

        /// <summary>
        /// 内件性质
        /// </summary>
        /// <remarks>非必填 1：文件  2：信函  3、物品 4、包裹</remarks>
        //<contents_attribute>3</contents_attribute>
        [XmlElement(ElementName = "contents_attribute")]
        public string ContentsAttribute { get; set; }

        /// <summary>
        /// 基础产品代码
        /// </summary>
        /// <remarks>必填  1：标准快递  2：快递包裹 3：代收/到付（标准快递）</remarks>
        //<base_product_no>1</base_product_no>
        [XmlElement(ElementName = "base_product_no")]
        public string BaseProductNo { get; set; }

        /// <summary>
        /// 业务产品分类（可售卖产品代码）
        /// </summary>
        /// <remarks>非必填 </remarks>
        //<biz_product_no></biz_product_no>
        [XmlElement(ElementName = "biz_product_no")]
        public string BizProductNo { get; set; }

        /// <summary>
        /// 订单重量
        /// </summary>
        /// <remarks>非必填 单位：克</remarks>
        //<weight>10</weight>
        [XmlElement(ElementName = "weight")]
        public string Weight { get; set; }

        /// <summary>
        /// 订单体积
        /// </summary>
        /// <remarks>非必填 单位：立方厘米</remarks>
        //<volume></volume>
        [XmlElement(ElementName = "volume")]
        public string Volume { get; set; }

        /// <summary>
        /// 长
        /// </summary>
        /// <remarks>非必填 单位：厘米</remarks>
        //<length></length>
        [XmlElement(ElementName = "length")]
        public string Length { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        /// <remarks>非必填 单位：厘米</remarks>
        //<width></width>
        [XmlElement(ElementName = "width")]
        public string Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        /// <remarks>非必填 单位：厘米</remarks>
        //<height></height>
        [XmlElement(ElementName = "height")]
        public string Height { get; set; }

        /// <summary>
        /// 邮费
        /// </summary>
        /// <remarks>非必填 单位：元</remarks>
        //<postage_total></postage_total>
        [XmlElement(ElementName = "postage_total")]
        public string PostageTotal { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <remarks>非必填 显示在面单备注</remarks>
        //<pickup_notes></pickup_notes>
        [XmlElement(ElementName = "pickup_notes")]
        public string PickupNotes { get; set; }

        /// <summary>
        /// 保险保价标志
        /// </summary>
        /// <remarks>非必填 1:基本2:保价</remarks>
        //<insurance_flag>1</insurance_flag>
        [XmlElement(ElementName = "insurance_flag")]
        public string InsuranceFlag { get; set; }

        /// <summary>
        /// 保价金额
        /// </summary>
        /// <remarks>非必填 单位：元</remarks>
        //<insurance_amount></insurance_amount>
        [XmlElement(ElementName = "insurance_amount")]
        public string InsuranceAmount { get; set; }

        /// <summary>
        /// 投递方式
        /// </summary>
        /// <remarks>非必填 1:客户自提2:上门投递3:智能包裹柜4:网点代投</remarks>
        //<deliver_type></deliver_type>
        [XmlElement(ElementName = "deliver_type")]
        public string DeliverType { get; set; }

        /// <summary>
        /// 投递预约时间
        /// </summary>
        /// <remarks>非必填 yyyy-mm-dd hh:mm:ss</remarks>
        //<deliver_pre_date></deliver_pre_date>
        [XmlElement(ElementName = "deliver_pre_date")]
        public string DeliverPreDate { get; set; }

        /// <summary>
        /// 揽收方式
        /// </summary>
        /// <remarks>非必填 揽收方式：0 客户送货上门，1 机构上门揽收</remarks>
        //<pickup_type></pickup_type>
        [XmlElement(ElementName = "pickup_type")]
        public string PickupType { get; set; }

        /// <summary>
        /// 揽收预约起始时间
        /// </summary>
        /// <remarks>非必填 yyyy-mm-dd hh:mm:ss</remarks>
        //<pickup_pre_begin_time></pickup_pre_begin_time>
        [XmlElement(ElementName = "pickup_pre_begin_time")]
        public string PickupPreBeginTime { get; set; }

        /// <summary>
        /// 揽收预约截至时间
        /// </summary>
        /// <remarks>非必填 yyyy-mm-dd hh:mm:ss</remarks>
        //<pickup_pre_end_time></pickup_pre_end_time>
        [XmlElement(ElementName = "pickup_pre_end_time")]
        public string PickupPreEndTime { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        /// <remarks>非必填 1:寄件人 2:收件人 3:第三方 4:收件人集中付费 5:免费 6:寄/收件人 7:预付卡</remarks>
        //<payment_mode>1</payment_mode>
        [XmlElement(ElementName = "payment_mode")]
        public string PaymentMode { get; set; }

        /// <summary>
        /// 代收款标志
        /// </summary>
        /// <remarks>非必填 1:代收货款2:代缴费9:无</remarks>
        //<cod_flag></cod_flag>
        [XmlElement(ElementName = "cod_flag")]
        public string CodFlag { get; set; }

        /// <summary>
        /// 代收款金额
        /// </summary>
        /// <remarks>非必填 单位：元</remarks>
        //<cod_amount></cod_amount>
        [XmlElement(ElementName = "cod_amount")]
        public string CodAmount { get; set; }

        /// <summary>
        /// 回单标识
        /// </summary>
        /// <remarks>非必填 1:基本2:回执 3:短信 5:电子返单6:格式返单7:自备返单8:反向返单</remarks>
        //<receipt_flag>1</receipt_flag>
        [XmlElement(ElementName = "receipt_flag")]
        public string ReceiptFlag { get; set; }

        /// <summary>
        /// 回单运单号
        /// </summary>
        /// <remarks>非必填 </remarks>
        //<receipt_waybill_no></receipt_waybill_no>
        [XmlElement(ElementName = "receipt_waybill_no")]
        public string ReceiptWaybillNo { get; set; }

        /// <summary>
        /// 电子优惠券号
        /// </summary>
        /// <remarks>非必填</remarks>
        //<electronic_preferential_no></electronic_preferential_no>
        [XmlElement(ElementName = "electronic_preferential_no")]
        public string ElectronicPreferentialNo { get; set; }

        /// <summary>
        /// 电子优惠券金额
        /// </summary>
        /// <remarks>非必填 A：邮务 B：速递</remarks>
        //<electronic_preferential_amount></electronic_preferential_amount>
        [XmlElement(ElementName = "electronic_preferential_amount")]
        public string ElectronicPreferentialAmount { get; set; }

        /// <summary>
        /// 贵品标识
        /// </summary>
        /// <remarks>非必填 贵品标识:0 无 1有</remarks>
        //<valuable_flag>0</valuable_flag>
        [XmlElement(ElementName = "valuable_flag")]
        public string ValuableFlag { get; set; }

        /// <summary>
        /// 寄件人安全码
        /// </summary>
        /// <remarks>非必填</remarks>
        //<sender_safety_code>0</sender_safety_code>
        [XmlElement(ElementName = "sender_safety_code")]
        public string SenderSafetyCode { get; set; }

        /// <summary>
        /// 收件人安全码
        /// </summary>
        /// <remarks>非必填</remarks>
        //<receiver_safety_code></receiver_safety_code>
        [XmlElement(ElementName = "receiver_safety_code")]
        public string ReceiverSafetyCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <remarks>非必填 公安交管邮件必填</remarks>
        //<note></note>
        [XmlElement(ElementName = "note")]
        public string Note { get; set; }

        /// <summary>
        /// 项目标识
        /// </summary>
        /// <remarks>非必填 山西公安户籍（SXGAHJ），公安网上车管（GAWSCG），苹果（APPLE）</remarks>
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
        /// <summary>
        /// 用户姓名
        /// </summary>
        /// <remarks>必填</remarks>
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 用户邮编
        /// </summary>
        /// <remarks>非必填 </remarks>
        [XmlElement(ElementName = "post_code")]
        public string PostCode { get; set; }

        /// <summary>
        /// 用户电话，包括区号、电话号码及分机号，中间用“-”分隔；
        /// </summary>
        /// <remarks>非必填 A：邮务 B：速递</remarks>
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 用户移动电话
        /// </summary>
        /// <remarks>必填</remarks>
        [XmlElement(ElementName = "mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 用户所在省，使用国标全称
        /// </summary>
        /// <remarks>必填</remarks>
        [XmlElement(ElementName = "prov")]
        public string Prov { get; set; }

        /// <summary>
        /// 用户所在市，使用国标全称
        /// </summary>
        /// <remarks>必填</remarks>
        [XmlElement(ElementName = "city")]
        public string City { get; set; }

        /// <summary>
        /// 用户所在县（区），使用国标全称
        /// </summary>
        /// <remarks>必填 </remarks>
        [XmlElement(ElementName = "county")]
        public string County { get; set; }

        /// <summary>
        /// 用户详细地址
        /// </summary>
        /// <remarks>必填</remarks>
        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
    }

    public class Cargos
    {

        /// <summary>
        /// 商品信息
        /// </summary>
        /// <remarks>非必填 如果是一票多件：cargo这个节点必传</remarks>
        [XmlElement(ElementName = "Cargo")]
        public List<Cargo> Cargo { get; set; }
    }

    public class Cargo
    {

        /// <summary>
        /// 商品名称
        /// </summary>
        /// <remarks>必填 内件名称</remarks>
        [XmlElement(ElementName = "cargo_name")]
        public string CargoName { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        /// <remarks>非必填</remarks>
        [XmlElement(ElementName = "cargo_category")]
        public string CargoCategory { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        /// <remarks>非必填</remarks>
        [XmlElement(ElementName = "cargo_quantity")]
        public string CargoQuantity { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>
        /// <remarks>非必填 （单位：元）</remarks>
        [XmlElement(ElementName = "cargo_value")]
        public string CargoValue { get; set; }

        /// <summary>
        /// 商品重量
        /// </summary>
        /// <remarks>非必填 （单位：克）</remarks>
        [XmlElement(ElementName = "cargo_weight")]
        public string CargoWeight { get; set; }
    }
}
