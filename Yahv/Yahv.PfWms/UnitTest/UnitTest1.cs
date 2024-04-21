using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Services.Models;
using Yahv.Web.Mvc;
using Layers.Data.Sqls;
using Wms.Services;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        #region 测试
        //[TestMethod]
        //public void TestMethod1()
        //{

        //    //组织对象
        //    var list = new List<WebApp.Services.Notice>();
        //    list.Add(new WebApp.Services.Notice { WaybillID = "123" });
        //    list.Add(new WebApp.Services.Notice { WaybillID = "456" });
        //    var notice = new WebApp.Services.NoticeEnter { Notices = list.ToArray() };
        //    string js = "{\"Notices\":[{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.29\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"NICHICON\",\"PartNumber\":\"UVR1V102MHD1TO\",\"Manufacturer\":\"NICHICON\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260103\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000107\",\"ProductID\":\"A42D240A57B2F631772B55D893F09BE4\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"1\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000107\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"TE\",\"PartNumber\":\"150545-2\",\"Manufacturer\":\"TE\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260104\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000108\",\"ProductID\":\"78560DDC4E81F2EA154006230E50C8B5\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000108\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"Schneider Electric/Legacy Relays\",\"PartNumber\":\"W172DIP-5\",\"Manufacturer\":\"Schneider Electric/Legacy Relays\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260105\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000109\",\"ProductID\":\"16DD445A58A4FAE90E8D67136087108A\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"9\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000109\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"SILERGY\",\"PartNumber\":\"SY6982CQDC\",\"Manufacturer\":\"SILERGY\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260106\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000110\",\"ProductID\":\"842DED5DA11E925C630FAE750477B04F\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"1\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000110\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"TI\",\"PartNumber\":\"CD74HC390M\",\"Manufacturer\":\"TI\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260107\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000111\",\"ProductID\":\"10BC3C4EB9A982CD9767DF0446FC742E\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000111\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"TRI-STAR\",\"PartNumber\":\"M39029/28-211\",\"Manufacturer\":\"TRI-STAR\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260108\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000112\",\"ProductID\":\"83FA3FFB4E083488123A60E1E54974C0\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000112\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"MINI\",\"PartNumber\":\"ADP-2-1\",\"Manufacturer\":\"MINI\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260109\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000113\",\"ProductID\":\"5F5FC3C42CFE8450C1C0FEFD9ECD9492\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000113\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"DIODES\",\"PartNumber\":\"AH1809-W-7\",\"Manufacturer\":\"DIODES\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260110\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000114\",\"ProductID\":\"C0E63BD7219C74934106A62186F154A4\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000114\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"HIROSE\",\"PartNumber\":\"FH52-28S-0.5SH\",\"Manufacturer\":\"HIROSE\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260111\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000115\",\"ProductID\":\"8B75D634DE5099F2D1AF02C70D1C7697\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000115\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"},{\"Type\":10,\"WareHouseID\":\"\",\"WaybillID\":\"Waybill201909260027\",\"DateCode\":\"\",\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-09-26T16:25:39.353\",\"Source\":300,\"Target\":400,\"Weight\":0.0200000,\"Volume\":0.0000000,\"Product\":{\"Catalog\":\"CORNELL\",\"PartNumber\":\"108TTA100M\",\"Manufacturer\":\"CORNELL\",\"Packing\":null,\"PackageCase\":null,\"UnitGrossWeightTL\":null,\"UnitGrossWeightBL\":null,\"UnitGrossVolume\":null},\"Files\":[],\"Input\":{\"Code\":\"Ipt201909260112\",\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000116\",\"ProductID\":\"9BD4ECC5B673A6A323D26184543F337C\",\"EnterpriseID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"TrackerID\":\"\",\"SalerID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"UnitPrice\":100.0000000,\"DateCode\":\"\",\"Origin\":\"44\"},\"Output\":{\"OrderID\":\"Order201909260014\",\"ItemID\":\"OrderItem20190926000116\",\"OwnerID\":\"013EEC9B29F8749B2E4E87F707C952E2\",\"SalerID\":\"\",\"CustomerServiceID\":\"\",\"PurchaserID\":\"\",\"Currency\":2,\"Price\":100.0000000},\"Supplier\":\"运大创新\"}]}";
        //    byte[] postData = Encoding.UTF8.GetBytes(notice.Json());

        //    //上传数据
        //    System.Net.WebClient wc = new System.Net.WebClient();
        //    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        //    var resdata = wc.UploadData("http://hv.warehouse.b1b.com/ApiWms/NoticeEnter/index", "Post", postData);
        //    string srcString = Encoding.UTF8.GetString(resdata);//解码  返回值：{"obj":0,"msg":"成功"}或{"obj":1,"msg":"失败"}
        //}
        #endregion

        [TestMethod]
        public void waybills()
        {
            var aa = string.Concat("11", null).MD5();

            //var wv = new Wms.Services.Views.WaybillsView().ToList();
        }

        [TestMethod]
        void UploadFile()
        {
            using (var webclient = new WebClient())
            {
                webclient.UploadFile("http://uuws.b1b.com/upload/warehouse", "Post", @"C:\Users\Administrator\Pictures\366x582a0a0.jpg");
            }

        }

        [TestMethod]
        public void LsNoticeEnter()
        {
            try
            {
                var lsnotices = new LsNoticeSubmit
                {
                    List = new LsNotice[]
                {
                    new LsNotice
                    {
                        SpecID = "AB01",
                        Quantity = 3,
                        StartDate = DateTime.Parse("2019-11-19"),
                        EndDate = DateTime.Parse("2020-11-19"),
                        Summary = "",
                        OrderID = "Order201911190001",
                        ClientID = "Admin0040",
                        PayeeID = "dfsgvfrs"
                    },
                     new LsNotice
                    {
                        SpecID = "AB02",
                        Quantity = 2,
                        StartDate = DateTime.Parse("2019-11-19"),
                        EndDate = DateTime.Parse("2020-11-19"),
                        Summary = "AB02规格的库位两个",
                        OrderID = "Order201911190001",
                        ClientID = "Admin0040",
                        PayeeID = "dfsgvfrs"
                    },
                      new LsNotice
                    {
                        SpecID = "AB03",
                        Quantity = 3,
                        StartDate = DateTime.Parse("2019-11-19"),
                        EndDate = DateTime.Parse("2020-11-19"),
                        Summary = "",
                        OrderID = "Order201911190001",
                        ClientID = "Admin0040",
                        PayeeID = "dfsgvfrs"
                    },
               }
                };


                //var result = Yahv.Utils.Http.ApiHelper.Current.PostData("http://hv.warehouse.b1b.com/wmsapi/LsNotice/Submit", lsnotices);

            }
            catch (Exception ex)
            {

                throw;
            }






        }

        [TestMethod]
        public void SortingNoticeTest()
        {
            
        }


        [TestMethod]
        public void BoxesPickingNoticesTest()
        {
            var data = new Wms.Services.Views.CustomPickingNoticeView().ToArray();

           
        }



        /// <summary>
        /// 包装种类代码测试
        /// </summary>
        [TestMethod]
        public void PackageTypesTest()
        {
            var data = PackageTypes.Current;
            var data1 = PackageTypes.Bulk;
        }
    }


}
