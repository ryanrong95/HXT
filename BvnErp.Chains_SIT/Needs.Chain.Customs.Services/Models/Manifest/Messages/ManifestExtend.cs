using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.ManifestMessage
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
                MessageID = consignment.ID,
                //海关备案编号（4位关区号+组织机构代码）_客户端ID（如0100123456789_ DXPESW0000000001），其中DXPESW为固定值，0000000001为客户端ID。
                SenderID = consignment.Manifest.CustomMaster + consignment.Manifest.UnitCode.Substring(8, 9) + "_DXPESW" + consignment.Manifest.ClientID,
                ReceiverID = "EPORT",
                FunctionCode = new HeadFunctionCodeType
                {
                    Value = CN010.Item9
                },
                MessageType = new HeadMessageTypeCodeType
                {
                    Value = CC002.MT1401
                },
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
                //AdditionalInformation = new AdditionalInformation
                //{
                //    Content = manifest.AdditionalInformation
                //},
                //Agent = new Agent
                //{
                //    ID = manifest.TransAgentCode
                //},
                BorderTransportMeans = new BorderTransportMeans
                {
                    TypeCode = new BorderTransportMeansTypeCodeType
                    {
                        Value = (CN012)consignment.Manifest.TrafMode
                    }
                },
                //Carrier = new Carrier
                //{
                //    ID = manifest.CarrierCode
                //},
                LoadingLocation = new LoadingLocation
                {
                    LoadingDateTime = new LoadingLocationLoadingDateTimeType
                    {
                        Value = consignment.Manifest.LoadingDate?.ToString("yyyyMMddhhmmss")
                    }
                },
                RepresentativePerson = new RepresentativePerson
                {
                    Name = consignment.Manifest.MsgRepName
                },
                UnloadingLocation = new UnloadingLocation
                {
                    ID = consignment.Manifest.LoadingLocationCode,
                    ArrivalDateTime = new UnloadingLocationArrivalDateTimeType
                    {
                        Value = consignment.Manifest.ArrivalDate.Value.ToString("yyyyMMdd")
                    }
                },
                Consignment = new Consignment[] { new Consignment {
                    //TODO:体积是否还需要？
                    //GrossVolumeMeasure = new ConsignmentGrossVolumeMeasureType
                    //{
                    //    Value = c.Cube.HasValue ? c.Cube.Value : new decimal()
                    //},
                    TotalPackageQuantity = new ConsignmentTotalPackageQuantityType
                    {
                        unitCode = (CN005)Convert.ToInt32(Enum.Parse(typeof(CN005), consignment.PackType)),
                        Value = consignment.PackNum
                    },
                    ValueAmount = new ConsignmentValueAmountType
                    {
                        currencyID = (CN025)Convert.ToInt32(Enum.Parse(typeof(CN025), consignment.Currency)),
                        Value = consignment.GoodsValue
                    },
                    Consignee = new Consignee
                    {
                        Name = consignment.ConsigneeName
                    },
                    ConsignmentItem = consignment.Items.Select(t => new ConsignmentItem
                    {
                        SequenceNumeric = new ConsignmentItemSequenceNumericType
                        {
                            Value = t.GoodsSeqNo,
                        },
                        Commodity = new Commodity
                        {
                            CargoDescription = t.GoodsBriefDesc
                        },
                        GoodsMeasure = new GoodsMeasure
                        {
                            GrossMassMeasure = new GoodsMeasureGrossMassMeasureType
                            {
                                Value = t.GoodsGrossWt.ToRound(3)
                            }
                        },
                        Packaging = new Packaging
                        {
                            QuantityQuantity = new PackagingQuantityQuantityType
                            {
                                unitCode = (CN005)Convert.ToInt32(Enum.Parse(typeof(CN005), consignment.PackType)),
                                Value = t.GoodsPackNum
                            }
                        }
                    }).ToArray(),
                    Consignor = new Consignor
                    {
                        Name = consignment.ConsignorName
                    },
                    Freight = new Freight
                    {
                        PaymentMethodCode = new FreightPaymentMethodCodeType
                        {
                            Value = (CN004)consignment.PaymentType
                        }
                    },
                    GovernmentAgencyGoodsItem = new GovernmentAgencyGoodsItem
                    {
                        GoodsMeasure = new GoodsMeasure
                        {
                            GrossMassMeasure = new GoodsMeasureGrossMassMeasureType
                            {
                                Value = consignment.GrossWt.ToRound(3)
                            }
                        }
                    },
                    GovernmentProcedure = new GovernmentProcedure
                    {
                        CurrentCode = new GovernmentProcedureCurrentCodeType
                        {
                            Value = (CC007)Convert.ToInt32(Enum.Parse(typeof(CC007), consignment.GovProcedureCode)),
                        }
                    },
                    //TransitDestination = new TransitDestination
                    //{
                    //    ID = new TransitDestinationIdentificationIDType
                    //    {
                    //        Value = c.TransitDestination
                    //    }
                    //},
                    TransportContractDocument = new TransportContractDocument
                    {
                        ID = consignment.ID,
                        //TODO:运输条款是否还需要？暂时不需要，数据库保留
                        //ConditionCode = new TransportContractDocumentConditionCodeType
                        //{
                        //    Value = (CN014)c.ConditionCode
                        //},
                        //Consolidator = new Consolidator
                        //{
                        //    ID = c.Consolidator
                        //}
                    },
                    TransportEquipment = consignment.Containers.Select(t => new TransportEquipment
                    {
                        ID = t.ContainerNo
                    }).ToArray()
                } },
                //Consignment = manifest.Consignments.Select(c => new Consignment
                //{
                //    //TODO:体积是否还需要？
                //    //GrossVolumeMeasure = new ConsignmentGrossVolumeMeasureType
                //    //{
                //    //    Value = c.Cube.HasValue ? c.Cube.Value : new decimal()
                //    //},
                //    TotalPackageQuantity = new ConsignmentTotalPackageQuantityType
                //    {
                //        unitCode = (CN005)Convert.ToInt32(Enum.Parse(typeof(CN005), c.PackType)),
                //        Value = c.PackNum
                //    },
                //    ValueAmount = new ConsignmentValueAmountType
                //    {
                //        currencyID = (CN025)Convert.ToInt32(Enum.Parse(typeof(CN025), c.Currency)),
                //        Value = c.GoodsValue
                //    },
                //    Consignee = new Consignee
                //    {
                //        Name = c.ConsigneeName
                //    },
                //    ConsignmentItem = c.Items.Select(t => new ConsignmentItem
                //    {
                //        SequenceNumeric = new ConsignmentItemSequenceNumericType
                //        {
                //            Value = t.GoodsSeqNo,
                //        },
                //        Commodity = new Commodity
                //        {
                //            CargoDescription = t.GoodsBriefDesc
                //        },
                //        GoodsMeasure = new GoodsMeasure
                //        {
                //            GrossMassMeasure = new GoodsMeasureGrossMassMeasureType
                //            {
                //                Value = t.GoodsGrossWt.ToRound(3)
                //            }
                //        },
                //        Packaging = new Packaging
                //        {
                //            QuantityQuantity = new PackagingQuantityQuantityType
                //            {
                //                unitCode = (CN005)Convert.ToInt32(Enum.Parse(typeof(CN005), c.PackType)),
                //                Value = t.GoodsPackNum
                //            }
                //        }
                //    }).ToArray(),
                //    Consignor = new Consignor
                //    {
                //        Name = c.ConsignorName
                //    },
                //    Freight = new Freight
                //    {
                //        PaymentMethodCode = new FreightPaymentMethodCodeType
                //        {
                //            Value = (CN004)c.PaymentType
                //        }
                //    },
                //    GovernmentAgencyGoodsItem = new GovernmentAgencyGoodsItem
                //    {
                //        GoodsMeasure = new GoodsMeasure
                //        {
                //            GrossMassMeasure = new GoodsMeasureGrossMassMeasureType
                //            {
                //                Value = c.GrossWt.ToRound(3)
                //            }
                //        }
                //    },
                //    GovernmentProcedure = new GovernmentProcedure
                //    {
                //        CurrentCode = new GovernmentProcedureCurrentCodeType
                //        {
                //            Value = (CC007)Convert.ToInt32(Enum.Parse(typeof(CC007), c.GovProcedureCode)),
                //        }
                //    },
                //    //TransitDestination = new TransitDestination
                //    //{
                //    //    ID = new TransitDestinationIdentificationIDType
                //    //    {
                //    //        Value = c.TransitDestination
                //    //    }
                //    //},
                //    TransportContractDocument = new TransportContractDocument
                //    {
                //        ID = c.ID,
                //        //TODO:运输条款是否还需要？暂时不需要，数据库保留
                //        //ConditionCode = new TransportContractDocumentConditionCodeType
                //        //{
                //        //    Value = (CN014)c.ConditionCode
                //        //},
                //        //Consolidator = new Consolidator
                //        //{
                //        //    ID = c.Consolidator
                //        //}
                //    },
                //    //TransportEquipment = c.Containers.Select(t => new TransportEquipment
                //    //{
                //    //    ID = t.ContainerNo
                //    //}).ToArray()
                //}).ToArray()
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
