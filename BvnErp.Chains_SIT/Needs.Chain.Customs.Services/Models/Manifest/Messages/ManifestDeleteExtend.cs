using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.ManifestMessageDelete
{
    public partial class Manifest
    {
        public Manifest()
        {

        }

        public Manifest(Models.ManifestConsignment consignment)
        {
            this.Head = new Head
            {
                //建议生成唯一的编码信息
                MessageID = consignment.ID + "DEL",
                FunctionCode = new HeadFunctionCodeType
                {
                    Value = CN010.Item3
                },
                MessageType = new HeadMessageTypeCodeType
                {
                    Value = CC002.MT1401
                },
                //海关备案编号（4位关区号+组织机构代码）_客户端ID（如0100123456789_ DXPESW0000000001），其中DXPESW为固定值，0000000001为客户端ID。
                SenderID = consignment.Manifest.CustomMaster + consignment.Manifest.UnitCode.Substring(8, 9) + "_DXPESW" + consignment.Manifest.ClientID,
                ReceiverID = "EPORT",                            
                SendTime = new HeadSendDateTimeType
                {
                    Value = DateTime.Now.ToString("yyyyMMddhhmmssfff")
                },
                Version = new HeadVersionIDType
                {
                    Value = CC015.Item10
                }
            };
            this.Declaration = new ManifestDeclaration
            {
                DeclarationOfficeID = new DeclarationDeclarationOfficeIDType
                {
                    Value = consignment.Manifest.CustomsCode
                },
                ID = consignment.Manifest.ID,
                BorderTransportMeans = new BorderTransportMeans
                {
                    TypeCode = new BorderTransportMeansTypeCodeType
                    {
                        Value = (CN012)consignment.Manifest.TrafMode,
                    },
                },
                Consignment  = new Consignment[]
                {
                    new Consignment
                    {
                        TransportContractDocument = new TransportContractDocument
                        {
                            ID = consignment.ID,
                            Amendment = new TransportContractDocumentAmendment
                            {
                                ChangeInformation = new ChangeInformation{
                                    Reason = "信息错误，申请删除",
                                    Contact = PurchaserContext.Current.UseOrgPersonCode,
                                    Phone = PurchaserContext.Current.UseOrgPersonTel
                                },
                                ChangeReasonCode = new AmendmentChangeReasonCodeType[]
                                {
                                    new AmendmentChangeReasonCodeType
                                    {
                                        Value = CC001.Item999,
                                    }
                                }
                            },
                        }
                    }                    
                },
                RepresentativePerson = new RepresentativePerson {
                    Name = PurchaserContext.Current.CompanyName
                }
                
            };
        }

        public string ToXml()
        {
            return this.Xml(Encoding.GetEncoding("utf-8"), true, false, false);
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SaveAs(string fileName)
        {
            //创建文件目录
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.DecMessageDirectory);
            fileDic.CreateDataDirectory();

            var xmldoc = new System.Xml.XmlDocument();
            xmldoc.LoadXml(this.ToXml());
            xmldoc.Save(fileDic.FilePath);

            return fileDic.VirtualPath;
        }
    }
}
