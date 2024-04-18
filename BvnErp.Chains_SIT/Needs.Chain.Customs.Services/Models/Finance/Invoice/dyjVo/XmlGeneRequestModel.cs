using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class XmlGeneRequestModel
    {
        public string 发票标识 { get; set; }
        public string 开票公司 { get; set; }
        public string 寄件人 { get; set; }
        public string 寄件电话 { get; set; }
        public string 寄件省份 { get; set; }
        public string 寄件城市 { get; set; }
        public string 寄件区县 { get; set; }
        public string 寄件地址 { get; set; }
        public string 客户公司 { get; set; }
        public string 收件人 { get; set; }
        public string 收件电话 { get; set; }
        public string 收件省份 { get; set; }
        public string 收件城市 { get; set; }
        public string 收件区县 { get; set; }
        public string 收件地址 { get; set; }
        public string 快递类型 { get; set; }
        public string 快递时效类型 { get; set; }
        public string 月结账号 { get; set; }
        public string 申请备注 { get; set; }
        public List<XML_Entity> XMLs { get; set; }

        public XmlGeneRequestModel(string InvoiceNoticeID, int ExpressCompany, int ExpressType)
        {
            this.发票标识 = InvoiceNoticeID;
            var notice = new InvoiceNoticeViewRJ(InvoiceNoticeID).GetInvoice();
            var client = new Needs.Ccs.Services.Views.ClientsView().Where(t => t.ID == notice.Client.ID).FirstOrDefault();
            this.寄件人 = client.ServiceManager.RealName;
            this.寄件电话 = client.ServiceManager.Mobile;

            this.开票公司 = "深圳市芯达通供应链管理有限公司";
            this.寄件省份 = "广东省";
            this.寄件城市 = "深圳市";
            this.寄件区县 = "龙岗区";
            this.寄件地址 = "深圳市龙岗区吉华路393号英达丰科技园1号楼";
            this.客户公司 = notice.Client.Company.Name;
            this.收件人 = notice.MailName;
            this.收件电话 = notice.MailMobile;

            var address = new XmlAddressHelper().HandleAddress(notice.MailAddress);
            this.收件省份 = address["Province"];
            this.收件城市 = address["City"];
            this.收件区县 = address["Area"];
            this.收件地址 = address["DetailsAddress"];

            string ExpressCode = "";
            if (ExpressCompany == 0)
            {
                ExpressCode = "SF";
                Enums.SFType sFType = (Enums.SFType)ExpressType;
                this.快递类型 = "KDN_SF";
                this.快递时效类型 = sFType.GetDescription();
            }
            else
            {
                ExpressCode = "EMS";
                Enums.EMSType eMSType = (Enums.EMSType)ExpressType;
                this.快递类型 = "EMS";
                this.快递时效类型 = eMSType.GetDescription();
            }

            var ExpressCompanies = new ExpressCompanyView().Where(t => t.Code == ExpressCode).FirstOrDefault();
            this.月结账号 = ExpressCompanies.MonthCode;

            this.XMLs = new List<XML_Entity>();

            var xmls = new InvoiceNoticeXmlView().Where(t => t.InvoiceNoticeID == InvoiceNoticeID).ToList();
         
            foreach (var xml in xmls)
            {
                var xmlItems = new InvoiceNoticeXmlItemView().Where(t => t.InvoiceNoticeXmlID == xml.ID).ToList();
                var xmlItemids = xmlItems.Select(t => t.ID).ToList();
                var StorageList = new InvoiceXmlMapView().Where(t => xmlItemids.Contains(t.InvoiceXmlID)).ToList();
                Kp kp = new Kp();

                kp.Version = InvoiceXmlConfig.Version;
                kp.Fpxx = new Fpxx();
                kp.Fpxx.Fpsj = new List<Fp>();

                Fp fp = new Fp();
                fp.Djh = xml.Djh;
                fp.Gfmc = xml.Gfmc;
                fp.Gfsh = xml.Gfsh;
                fp.Gfyhzh = xml.Gfyhzh;
                fp.Gfdzdh = xml.Gfdzdh;
                fp.Bz = xml.Bz;
                fp.Fhr = xml.Fhr;
                fp.Skr = xml.Skr;
                fp.Spbmbbh = xml.Spbmbbh;
                fp.Hsbz = xml.Hsbz;

                fp.Spxx = new List<Sph>();
                foreach (var xmlItem in xmlItems)
                {
                    Sph sph = new Sph();
                    sph.Xh = xmlItem.Xh.ToString();
                    sph.Spmc = xmlItem.Spmc;
                    sph.Ggxh = xmlItem.Ggxh;
                    sph.Jldw = xmlItem.Jldw;
                    //鲁亚慧 20230516 开服务费发票时，数量和单价不填 ryan
                    if (xmlItem.Slv == 0.13M)
                    {
                        sph.Sl = xmlItem.Sl;
                        sph.Dj = xmlItem.Dj;
                    }
                    
                    sph.Spbm = xmlItem.Spbm;
                    sph.Qyspbm = "";
                    sph.Syyhzcbz = "";
                    sph.Lslbz = "";
                    sph.Yhzcsm = "";

                    sph.Je = xmlItem.Je;
                    //sph.Dj = xmlItem.Dj;
                    sph.Slv = xmlItem.Slv;
                    sph.Se = xmlItem.Se;
                    sph.Kce = "";

                    //对应库存ID
                    //sph.KC_ID = notice.InvoiceType == Enums.InvoiceType.Full ? StorageList.FirstOrDefault(t => t.InvoiceNoticeItemID == xmlItem.InvoiceNoticeItemID)?.DeclistItemID : "";
                    sph.KCXX = notice.InvoiceType == Enums.InvoiceType.Full ? new KCXX
                    {
                        KC = StorageList.Where(t => t.InvoiceXmlID == xmlItem.ID).Select(t =>
                        new KC
                        {
                            KC_ID = t.DecListID,
                            SL = t.OutQty 
                        }).ToList()
                    } : null;

                    fp.Spxx.Add(sph);
                }

                kp.Fpxx.Fpsj.Add(fp);
                kp.Fpxx.Zsl = kp.Fpxx.Fpsj.Count;
                string xmlResult = kp.Xml(System.Text.Encoding.GetEncoding("GBK"));

                XML_Entity xML_Entity = new XML_Entity();
                xML_Entity.XML标识 = xml.ID;
                xML_Entity.XML = xmlResult;

                this.XMLs.Add(xML_Entity);
            }
        }
    }
}
