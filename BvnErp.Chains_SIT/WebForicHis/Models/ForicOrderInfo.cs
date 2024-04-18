using Needs.Utils.Serializers;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WebForicHis.Models
{
    public class ForicOrderInfo
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string DYJOrder;

        public string ConnectionString { get; set; }
        public ForicOrderInfo(string DYJOrder)
        {

            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;
            this.DYJOrder = DYJOrder;
        }


        public string GetMessage()
        {
            Logger.Info(DYJOrder + " Start");
            string result = "";
            var url = "http://office.51db.com:81/ApiCenter/OutService/ApplyCustomsInvoices.ashx";
            StringBuilder postData = new StringBuilder();
            postData.Append("CustomsID");
            postData.Append("=");
            postData.Append(DYJOrder);

            var sss = "{'requestitem': '报关单详情'," +
                                  "'data':" +
                                  "{" +
                                  "'CustomsID':'" + this.DYJOrder + "'}," +
                                  "'key':'78fc22d55739b169d06de663133bc467'}";

            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(sss);
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();

                Logger.Info(this.DYJOrder + " read success");
            }
            catch (Exception ex)
            {
                Logger.Info(this.DYJOrder + " read Error");
                result = ex.Message;
            }
            return result;

        }

        public string SaveForicHisData()
        {
            var message = GetMessage();

            try
            {
                Logger.Info(this.DYJOrder + " deal start");
                //获取返回结果
                var ResponseResult = message.JsonTo<ForicHisViewModel>();

                var PIList = new List<string>();
                DataTable dtProductSupplierMap = new DataTable();
                dtProductSupplierMap.Columns.Add("ID");
                dtProductSupplierMap.Columns.Add("SupplierID");
                var list = new List<string>();

                var supplierMap = new Needs.Ccs.Services.Views.PreProduct.ProductSupplierMapView();

                foreach (var pi in ResponseResult.data)
                {
                    if (pi.PI文件路径 != null)
                    {
                        PIList.Add(pi.PI文件路径);
                    }

                    var SupplierName = pi.付款公司;
                    switch (SupplierName.ToLower())
                    {
                        case "hk lianchuang electronics co.,limited":
                            SupplierName = "HK Lianchuang Electronics Co.,Limited";
                            break;
                        case "ic360 electronics limited":
                            SupplierName = "IC360 Electronics Limited";
                            break;
                        case "ic360 group limited":
                            SupplierName = "IC360 Group Limited";
                            break;
                        case "hongkong hongtu international logistics co., ltd.":
                        case "hong kong changyun international logistics co.,limited":
                            SupplierName = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO.,LIMITED";
                            break;
                        case "b one b electronic co.,limited":
                            SupplierName = "B ONE B ELECTRONIC CO.,LIMITED";
                            break;
                        case "hk huanyu electronics technology co.,limited":
                            SupplierName = "HK HUANYU ELECTRONICS TECHNOLOGY CO.,LIMITED";
                            break;
                        case "icgoo group limited":
                            SupplierName = "ICGOO GROUP LIMITED";
                            break;
                        case "hong kong wanlutong international logistics co limited":
                            SupplierName = "HONG KONG WANLUTONG INTERNATIONAL LOGISTICS CO LIMITED";
                            break;
                        case "kb electronics development limited":
                            SupplierName = "KB ELECTRONICS DEVELOPMENT LIMITED";
                            break;
                        default:
                            SupplierName = "Anda International Trade Group Limited";
                            break;
                    }
                    //杭州比一比电子科技有限公司
                    //上海比亿电子技术有限公司
                    var client1 = new Needs.Ccs.Services.Views.ClientsView().Where(item => item.Company.Name == "杭州比一比电子科技有限公司").FirstOrDefault();
                    var SupplierView1 = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();
                    //var SupplierView1 = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID && item.Name == pi.付款公司)?.FirstOrDefault();
                    var supplierID = "";

                    var supplier = SupplierView1.Find(t => t.Name == SupplierName);
                    if (supplier == null)
                    {
                        var SupplierView2 = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();
                        if (SupplierView2.Find(t => t.Name == SupplierName) == null)
                        {
                            var saveSupplier = new Needs.Ccs.Services.Models.ClientSupplier();
                            saveSupplier.Name = SupplierName;
                            saveSupplier.ClientID = client1.ID;
                            saveSupplier.ChineseName = SupplierName;
                            saveSupplier.Enter();
                            supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID && item.Name == SupplierName)?.FirstOrDefault();
                            supplierID = supplier.ID;
                        }
                    }
                    else
                    {

                        supplierID = supplier.ID;
                    }

                    foreach (var item in pi.明细列表)
                    {
                        if (!supplierMap.Any(t => t.ID == item.ID) && !list.Contains(item.ID))
                        {
                            DataRow dr = dtProductSupplierMap.NewRow();
                            dr[0] = item.ID;
                            dr[1] = supplierID;
                            dtProductSupplierMap.Rows.Add(dr);
                            list.Add(item.ID);
                        }
                    }

                }

                #region 型号/供应商匹配关系

                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    if (dtProductSupplierMap.Rows.Count > 0)
                    {
                        SqlBulkCopy bulkPremiums = new SqlBulkCopy(conn);
                        bulkPremiums.DestinationTableName = "ProductSupplierMap";
                        bulkPremiums.BatchSize = dtProductSupplierMap.Rows.Count;
                        bulkPremiums.WriteToServer(dtProductSupplierMap);
                    }
                    conn.Close();
                }

                Logger.Info(this.DYJOrder + " 对应关系处理结束");

                #endregion


                #region PI附件处理

                GetPI(PIList);

                Logger.Info(this.DYJOrder + " PI附件处理结束");
                #endregion

                Logger.Info(this.DYJOrder + " deal Success");
            }
            catch (Exception ex)
            {
                Logger.Info(this.DYJOrder + " deal Error" + ex.Message);
                //result = ex.Message;
            }


            return message;

        }


        public string SaveForicAllData()
        {
            var message = GetMessage();

            try
            {
                Logger.Info(this.DYJOrder + " deal start");
                //获取返回结果
                var ResponseResult = message.JsonTo<ForicHisViewModel>();

                var PIList = new List<string>();
               
                var list = new List<string>();

                var supplierMap = new Needs.Ccs.Services.Views.PreProduct.ProductSupplierMapView();

                foreach (var pi in ResponseResult.data)
                {
                    if (pi.PI文件路径 != null)
                    {
                        PIList.Add(pi.PI文件路径);
                    }
                }

                #region PI附件处理

                GetPI(PIList);

                Logger.Info(this.DYJOrder + " PI附件处理结束");
                #endregion

                Logger.Info(this.DYJOrder + " deal Success");
            }
            catch (Exception ex)
            {
                Logger.Info(this.DYJOrder + " deal Error" + ex.Message);
                //result = ex.Message;
            }


            return message;

        }

        public void GetPI(List<string> PIList)
        {
            string data = DateTime.Now.ToString("yyyyMM");
            string day = DateTime.Now.ToString("yyyyMMdd").Substring(6, 2);
            var fileName = string.Empty;//this.IcgooOrder + "_pi.pdf";//TempFiles/
            string dataPath = string.Concat("Order/", data, "/", day, "/");
            var path = Path.Combine("D:/foric/admin/Files/", dataPath);


            var OrderIDs = new Needs.Ccs.Services.Views.IcgooMapView().Where(t => t.IcgooOrder == this.DYJOrder).Select(t => t.OrderID).ToList();
            if (OrderIDs.Count() > 0)
            {
                foreach (var piPath in PIList)
                {
                    fileName = piPath.Replace("TempFiles/", "");
                    //http://office.51db.com:81/TempFiles/194415263071_83_114_PI.pdf
                    var isDownload = GetPDF(string.Format("http://office.51db.com:81/{0}", piPath), path, fileName);

                    //获取成功，写入数据
                    if (isDownload)
                    {
                        string[] ids = OrderIDs[0].Split('-');

                        var mainOrderID = "";
                        if (OrderIDs[0].IndexOf("-") > 0)
                        {
                            mainOrderID = ids[0];

                            var orderfilebool = new Needs.Ccs.Services.Views.MainOrderFilesView().Any(t => t.MainOrderID == mainOrderID
                           && t.FileType == Needs.Ccs.Services.Enums.FileType.OriginalInvoice && t.Name == fileName && t.Status == Needs.Ccs.Services.Enums.Status.Normal);

                            if (!orderfilebool)
                            {
                                var orderfile = new Needs.Ccs.Services.Models.MainOrderFile()
                                {
                                    MainOrderID = mainOrderID,
                                    Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin"),
                                    Name = fileName,
                                    FileType = Needs.Ccs.Services.Enums.FileType.OriginalInvoice,
                                    FileFormat = "application/pdf",
                                    Url = dataPath + fileName,
                                    FileStatus = Needs.Ccs.Services.Enums.OrderFileStatus.Audited
                                };
                                orderfile.Enter();

                            }
                        }
                        else
                        {
                            foreach (var id in OrderIDs)
                            {
                                var orderfilebool = new Needs.Ccs.Services.Views.MainOrderFilesView().Any(t => t.MainOrderID == id
                                && t.FileType == Needs.Ccs.Services.Enums.FileType.OriginalInvoice && t.Name == fileName && t.Status == Needs.Ccs.Services.Enums.Status.Normal);

                                if (!orderfilebool)
                                {
                                    var orderfile = new Needs.Ccs.Services.Models.MainOrderFile()
                                    {
                                        MainOrderID = id,
                                        Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin"),
                                        Name = fileName,
                                        FileType = Needs.Ccs.Services.Enums.FileType.OriginalInvoice,
                                        FileFormat = "application/pdf",
                                        Url = dataPath + fileName,
                                        FileStatus = Needs.Ccs.Services.Enums.OrderFileStatus.Audited
                                    };
                                    orderfile.Enter();
                                }
                            }
                        }
                    }
                }
            }

        }
        public bool GetPDF(string url, string SavePath, string FileName)
        {
            var result = false;
            try
            {
                // 创建一个HTTP请求
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "GET";

                //设置超时时间
                request.Timeout = 9000;

                //获取请求
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        //直到request.GetResponse()程序才开始向目标网页发送Post请求
                        System.IO.Stream responseStream = response.GetResponseStream();

                        //不存在就创建目录
                        if (!System.IO.Directory.Exists(SavePath))
                        {
                            System.IO.Directory.CreateDirectory(SavePath);
                        }

                        //创建本地文件写入流
                        System.IO.Stream stream = new System.IO.FileStream(SavePath + FileName, System.IO.FileMode.Create);

                        byte[] bArr = new byte[1024];
                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            stream.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        }
                        stream.Close();
                        responseStream.Close();

                        result = true;
                    }
                }
                request.Abort();
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }





        public string SaveProductUnionCode()
        {

            var message = GetMessage();

            Logger.Info(" ===========" + this.DYJOrder + "======= Begin ===== ");
            Logger.Info(message);
            Logger.Info(" ===========" + this.DYJOrder + "======= End ===== ");

            //return "";
            try
            {
                Logger.Info(this.DYJOrder + " deal start");
                //获取返回结果
                var ResponseResult = message.JsonTo<ForicHisViewModel>();
                DataTable dtProductSupplierMap = new DataTable();
                dtProductSupplierMap.Columns.Add("ID");
                dtProductSupplierMap.Columns.Add("SupplierID");
                var list = new List<string>();

                var supplierMap = new Needs.Ccs.Services.Views.PreProduct.ProductSupplierMapView();

                var orderids = new Needs.Ccs.Services.Views.IcgooMapView().Where(t => t.IcgooOrder == DYJOrder).Select(t => t.OrderID).Distinct();


                foreach (var pi in ResponseResult.data)
                {
                    var SupplierName = pi.付款公司;
                    switch (SupplierName.ToLower())
                    {
                        case "hk lianchuang electronics co.,limited":
                            SupplierName = "HK Lianchuang Electronics Co.,Limited";
                            break;
                        case "ic360 electronics limited":
                            SupplierName = "IC360 Electronics Limited";
                            break;
                        case "ic360 group limited":
                            SupplierName = "IC360 Group Limited";
                            break;
                        case "hongkong hongtu international logistics co., ltd.":
                        case "hong kong changyun international logistics co.,limited":
                            SupplierName = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO.,LIMITED";
                            break;
                        case "b one b electronic co.,limited":
                            SupplierName = "B ONE B ELECTRONIC CO.,LIMITED";
                            break;
                        case "hk huanyu electronics technology co.,limited":
                            SupplierName = "HK HUANYU ELECTRONICS TECHNOLOGY CO.,LIMITED";
                            break;
                        case "icgoo group limited":
                            SupplierName = "ICGOO GROUP LIMITED";
                            break;
                        case "hong kong wanlutong international logistics co limited":
                            SupplierName = "HONG KONG WANLUTONG INTERNATIONAL LOGISTICS CO LIMITED";
                            break;
                        case "kb electronics development limited":
                            SupplierName = "KB ELECTRONICS DEVELOPMENT LIMITED";
                            break;
                        default:
                            SupplierName = "Anda International Trade Group Limited";
                            break;
                    }
                    //杭州比一比电子科技有限公司
                    //上海比亿电子技术有限公司
                    var client1 = new Needs.Ccs.Services.Views.ClientsView().Where(item => item.Company.Name == "杭州比一比电子科技有限公司").FirstOrDefault();
                    var SupplierView1 = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();
                    //var SupplierView1 = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID && item.Name == pi.付款公司)?.FirstOrDefault();
                    var supplierID = "";

                    var supplier = SupplierView1.Find(t => t.Name == SupplierName);
                    if (supplier == null)
                    {
                        var SupplierView2 = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();
                        if (SupplierView2.Find(t => t.Name == SupplierName) == null)
                        {
                            var saveSupplier = new Needs.Ccs.Services.Models.ClientSupplier();
                            saveSupplier.Name = SupplierName;
                            saveSupplier.ClientID = client1.ID;
                            saveSupplier.ChineseName = SupplierName;
                            saveSupplier.Enter();
                            supplier = new Needs.Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID && item.Name == SupplierName)?.FirstOrDefault();
                        }
                    }
                    else
                    {

                        supplierID = supplier.ID;
                    }

                    //var s = pi.明细列表.Where(t => t.PartNo == "STD5NM50T4").ToList();

                    foreach (var item in pi.明细列表)
                    {
                        var orderitem = new Needs.Ccs.Services.Views.OrderItemsView().Where(t => t.ProductUniqueCode == null
                                && t.Model == item.PartNo.Replace("3C", "CCC") && t.Manufacturer == item.MFC).FirstOrDefault();
                        if (orderitem != null)
                        {
                            orderitem.ProductUniqueCode = item.ID;
                            orderitem.Enter();
                            Logger.Info(item.ID);
                        }

                        if (!supplierMap.Any(t => t.ID == item.ID) && !list.Contains(item.ID))
                        {
                            DataRow dr = dtProductSupplierMap.NewRow();
                            dr[0] = item.ID;
                            dr[1] = supplierID;
                            dtProductSupplierMap.Rows.Add(dr);
                            list.Add(item.ID);
                        }
                    }

                    #region 型号/供应商匹配关系

                    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                    {
                        conn.Open();
                        if (dtProductSupplierMap.Rows.Count > 0)
                        {
                            SqlBulkCopy bulkPremiums = new SqlBulkCopy(conn);
                            bulkPremiums.DestinationTableName = "ProductSupplierMap";
                            bulkPremiums.BatchSize = dtProductSupplierMap.Rows.Count;
                            bulkPremiums.WriteToServer(dtProductSupplierMap);
                        }
                        conn.Close();
                    }

                    Logger.Info(this.DYJOrder + " 对应关系处理结束");

                    #endregion

                }


            }
            catch (Exception ex)
            {
                Logger.Info(this.DYJOrder + " deal Error" + ex.Message);
            }
            return "";
        }
    }
}