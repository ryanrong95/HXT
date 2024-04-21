using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Utils.Http;

namespace Wms.Services.chonggous.Views
{
    public class CgNewTempStocksView : QueryView<object, PvWmsRepository>
    {
        #region 构造函数
        public CgNewTempStocksView()
        {

        }

        protected CgNewTempStocksView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgNewTempStocksView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        #endregion

        protected override IQueryable<object> GetIQueryable()
        {   
            var twaybills = from twaybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TWaybills>()
                            orderby twaybill.CreateDate descending
                                 select new MyTWaybill
                                 {
                                     ID = twaybill.ID,
                                     Code = twaybill.WaybillCode,
                                     EnterCode = twaybill.EnterCode,
                                     Supplier = twaybill.Supplier,
                                     CarrierID = twaybill.CarrierID,
                                     WareHouseID = twaybill.WareHouseID,
                                     Summary = twaybill.Summary,
                                     CreateDate = twaybill.CreateDate,
                                     ModifyDate = twaybill.ModifyDate,
                                     ShelveID = twaybill.ShelveID,
                                     AdminID = twaybill.AdminID,
                                     Status = (TempStockStatus)twaybill.Status,
                                     ForOrderID = twaybill.ForOrderID,
                                     CompleteDate = twaybill.CompleteDate,
                                 };
            return twaybills;

        }

        /// <summary>
        /// 补全数据
        /// </summary>
        /// <returns></returns>
        public object[] ToMyArray()
        {
            return this.ToMyPage() as object[];
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<MyTWaybill>();
            int total = iquery.Count();

            if(pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_myTWaybill = iquery.ToArray();
            var waybillIds = ienum_myTWaybill.Select(item => item.ID).Distinct();

            #region 文件处理
            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                            where waybillIds.Contains(file.WaybillID)
                            select new CenterFileDescription
                            {
                                ID = file.ID,
                                WaybillID = file.WaybillID,
                                CustomName = file.CustomName,
                                Type = file.Type,
                                Url = CenterFile.Web + file.Url,
                                AdminID = file.AdminID,
                                Status = file.Status,
                            };

            var files = filesView.ToArray();
            #endregion

            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>();
            var carriersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();

            var linq = from waybill in ienum_myTWaybill
                       join _storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TStorages>() on waybill.ID equals _storage.WaybillID into storages
                       from storage in storages.DefaultIfEmpty()
                       
                       join _client in clientView on waybill.EnterCode equals _client.EnterCode into clients
                       from client in clients.DefaultIfEmpty()
                       join _carrier in carriersTopView on waybill.CarrierID equals _carrier.ID into carriers
                       from carrier in carriers.DefaultIfEmpty()
                       select new
                       {
                           Waybill = new
                           {
                               ID = waybill.ID,
                               Code = waybill.Code,
                               EnterCode = waybill.EnterCode,
                               ClientName = client?.Name,
                               Supplier = waybill.Supplier,
                               Status = waybill.Status,
                               StatusDes = waybill.Status.GetDescription(),
                               CarrierID = waybill.CarrierID,
                               CarrierName = carrier?.Name,
                               ForOrderID = waybill.ForOrderID,
                               ShelveID = waybill.ShelveID,
                               CreateDate = waybill.CreateDate,
                               ModifyDate = waybill.ModifyDate,
                               CompleteDate = waybill.CompleteDate,
                               TempDays = waybill.CompleteDate.HasValue ? (waybill.CompleteDate.Value.Date - waybill.CreateDate.Date).Days + 1 : (DateTime.Now.Date - waybill.CreateDate.Date).Days + 1,
                               Summary = waybill.Summary,
                               Files = files.Where(file => file.WaybillID == waybill.ID),
                           },
                           Storage = storage
                       };

            var linq_ienum = linq.ToArray();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return new
                {
                    Total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    Data = linq_ienum,
                };
            }

            if (!pageIndex.HasValue && !pageSize.HasValue)
            {
                return linq_ienum.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = linq_ienum,
            };
            
        }

        /// <summary>
        /// 暂存入库保存
        /// </summary>
        /// <param name="jobject"></param>
        public void Enter(JObject jobject)
        {
            var waybill = jobject["Waybill"];
            var storage = jobject["Storage"];
            var adminID = jobject["AdminID"].Value<string>();    
            var waybillView = Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TWaybills>();

            var waybillID = waybill["ID"]?.Value<string>();
            var code = waybill["Code"]?.Value<string>();
            var warehouseID = waybill["WareHouseID"].Value<string>();
            var shelveID = waybill["ShelveID"].Value<string>();
            var supplier = waybill["Supplier"].Value<string>();
            var enterCode = waybill["EnterCode"].Value<string>();
            var summary = waybill["Summary"].Value<string>();
            var carrierID = waybill["CarrierID"].Value<string>();

            string realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;


            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                // 新增保存
                if (!waybillView.Any(item => item.ID == waybillID))
                {
                    #region 新增保存Waybill
                    if (string.IsNullOrEmpty(waybillID))
                    {
                        waybillID = PKeySigner.Pick(PkeyType.TWaybills);
                    }

                    this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.TWaybills
                    {
                        ID = waybillID,
                        WaybillCode = code,
                        WareHouseID = warehouseID,
                        ShelveID = shelveID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        EnterCode = enterCode,
                        Status = (int)TempStockStatus.Waiting,
                        AdminID = adminID,                                                
                        Summary = summary,
                        Supplier = supplier,                        
                        CarrierID = carrierID,
                    });

                    var files = waybill["Files"];

                    if (files.Count() > 0)
                    {
                        List<string> fileids = new List<string>();
                        foreach (var file in files)
                        {
                            fileids.Add(file["ID"].Value<string>());
                        }

                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            WaybillID = waybillID,
                        }, item => fileids.Contains(item.ID));
                    }

                    CgLogs_Operator logs = new CgLogs_Operator();
                    logs.CreatorID = adminID;
                    logs.MainID = waybillID;
                    logs.Type = Enums.LogOperatorType.Insert;
                    logs.Conduct = "新增暂存";
                    logs.CreateDate = DateTime.Now;
                    logs.Content = $"{realName} 在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {Enums.LogOperatorType.Insert.GetDescription()} 新增暂存，TWaybillID:{waybillID} EnterCode:{enterCode}";
                    logs.Enter(this.Reponsitory);
                    #endregion

                    #region 新增库存
                    this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.TStorages
                    {
                        ID = PKeySigner.Pick(PkeyType.TStorages),
                        WaybillID = waybillID,
                        ModifyDate = DateTime.Now,
                        Quantity = 1,                        
                        CreateDate = DateTime.Now,                        
                    });
                                         
                    #endregion                    
                }
                else
                {
                    #region 修改Waybill  
                    // 修改
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.TWaybills>(new
                    {
                        ID = waybillID,
                        WaybillCode = code,
                        ModifyDate = DateTime.Now,
                        EnterCode = enterCode,                                                    
                        Summary = summary,
                        Supplier = supplier,                        
                        CarrierID = carrierID,
                        ShelveID = shelveID,
                    }, item => item.ID == waybillID);

                    var files = waybill["Files"];

                    if (files.Count() > 0)
                    {
                        List<string> fileids = new List<string>();
                        foreach (var file in files)
                        {
                            fileids.Add(file["ID"].Value<string>());
                        }

                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            WaybillID = waybillID,                            
                        }, item => fileids.Contains(item.ID));
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 获取库房
        /// </summary>
        /// <returns></returns>
        public object GetWarehouseInfos()
        {
            var results = new List<object>();
            results.Add(new
            {
                ID = ConfigurationManager.AppSettings["TempWarehouseIDForHK"],
                Name = ConfigurationManager.AppSettings["TempWarehouseNameForHK"]
            });

            return results.ToArray();
        }

        /// <summary>
        /// 返回WaybillID
        /// </summary>
        /// <returns></returns>
        public string GetWaybillID()
        {
            return PKeySigner.Pick(PkeyType.TWaybills);
        }

        #region 搜索方法
        string wareHouseID;
        /// <summary>
        /// 根据库房ID搜索
        /// </summary>
        /// <param name="wareHouseID"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyTWaybill>();

            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgNewTempStocksView(this.Reponsitory, waybillView);
            }

            var linq_waybillIDs = from waybill in waybillView 
                                  where waybill.WareHouseID.Contains(wareHouseID)
                                  select waybill.ID;
            var linq_ids = linq_waybillIDs.Distinct();

            var linq = from waybill in waybillView
                       join id in linq_ids on waybill.ID equals id
                       orderby waybill.CreateDate descending
                       select waybill;
            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };
            return view;
        }

        /// <summary>
        /// 根据运单号或订单号搜索已有暂存运单
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByWaybillCode(string code)
        {
            var waybillView = IQueryable.Cast<MyTWaybill>();

            var linq = from waybill in waybillView
                       where waybill.Code == code || waybill.ForOrderID == code
                       orderby waybill.CreateDate descending
                       select waybill;
            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单ID搜索已有暂存运单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByWaybillID(string id)
        {
            var waybillView = IQueryable.Cast<MyTWaybill>();

            var linq = from waybill in waybillView
                       where waybill.ID == id
                       select waybill;
            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据承运商搜索已有暂存运单
        /// </summary>
        /// <param name="carrierID"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByCarrier(string carrierID)
        {
            var waybillView = this.IQueryable.Cast<MyTWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CarrierID == carrierID
                       orderby waybill.CreateDate descending
                       select waybill;

            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据库位ID来搜索已有运单
        /// </summary>
        /// <param name="shelveID"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByShelveID(string shelveID)
        {
            var waybillView = this.IQueryable.Cast<MyTWaybill>();

            var linq_waybillIDs = from waybill in waybillView
                                  where waybill.ShelveID == shelveID
                                  select waybill.ID;
            var linq_ids = linq_waybillIDs.Distinct().Take(500);

            var linq = from waybill in waybillView
                       join id in linq_ids on waybill.ID equals id
                       orderby waybill.CreateDate descending
                       select waybill;
            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };
            return view;
        }

        /// <summary>
        /// 根据运单录入时间搜索已有暂存运单
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByCreateTime(DateTime start, DateTime end)
        {
            var waybillView = this.IQueryable.Cast<MyTWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CreateDate >= start && waybill.CreateDate < end.AddDays(1)
                       orderby waybill.CreateDate descending
                       select waybill;
            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据是否待处理，已处理状态来搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public CgNewTempStocksView SearchByStatus(TempStockStatus status)
        {
            var waybillView = this.IQueryable.Cast<MyTWaybill>();

            var linq = from waybill in waybillView
                       where waybill.Status == status
                       orderby waybill.CreateDate descending
                       select waybill;

            var view = new CgNewTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        #endregion

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public bool CompressImage(string sFile, string dFile, int flag = 90, int size = 150, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                //firstFileInfo.Delete();
                return true;
            }
            Image iSource = Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int dHeight = iSource.Height / 2;
            int dWidth = iSource.Width / 2;
            int sW = 0, sH = 0;
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                    FileInfo fi = new FileInfo(dFile);
                    if (fi.Length > 1024 * size)
                    {
                        flag = flag - 10;
                        CompressImage(sFile, dFile, flag, size, false);
                    }
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }


        }

        public string UploadFile(string sFile, string adminID, string WaybillID)
        {
            //string address = $"{UploadUrl}?WaybillID={WaybillID}&AdminID={adminID}&Type=8000";
            string uploadUrl = CenterFile.Web + "api/Upload";
            string address = $"{uploadUrl}?WaybillID={WaybillID}&AdminID={adminID}&Type=8000";
            using (WebClient client = new WebClient())
            {
                var responseStr = client.UploadFile(new Uri(address), "POST", sFile);
                var result = System.Text.Encoding.UTF8.GetString(responseStr);
                return result;
            }
        }        

        /// <summary>
        /// 同时删除多个文件
        /// </summary>
        /// <param name="files"></param>
        public void DeleteFiles(string[] ids)
        {
            if (ids.Count() > 0)
            {                
                string uploadUrl = CenterFile.Web + "api/Upload";
                foreach (var id in ids)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        string url = $"{uploadUrl}/DeleteFile?FileID={id}";
                        ApiHelper.Current.JPost(url);
                    }                   
                }                
            }
        }

        /// <summary>
        /// 更新暂存的状态及OrderID
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="OrderID"></param>
        public void UpdateTWaybills(string waybillID, string OrderID, string adminID = "")
        {
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.TWaybills>(new
            {
                ForOrderID = OrderID,
                Status = (int)TempStockStatus.Completed,
                CompleteDate = DateTime.Now,
            }, item => item.ID == waybillID);

            string realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().SingleOrDefault(item => item.ID == adminID)?.RealName;
            
            CgLogs_Operator logs = new CgLogs_Operator();
            logs.CreatorID = adminID;
            logs.MainID = waybillID;
            logs.Type = Enums.LogOperatorType.Update;
            logs.Conduct = "认领暂存";
            logs.CreateDate = DateTime.Now;
            logs.Content = $"{realName} 在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {Enums.LogOperatorType.Update.GetDescription()} 认领暂存，TWaybillID:{waybillID} OrderID:{OrderID}";
            logs.Enter(this.Reponsitory);
        }

        #region Helper Class

        private class MyTWaybill
        {      
            public string ID { get; set; }
            public string Code { get; set; } // 运单号
            public string EnterCode { get; set; }
            public string ClientName { get; set; }
            public string WareHouseID { get; set; }
            public string Supplier { get; set; }
            public string CarrierID { get; set; }
            public string CarrierName { get; set; }            
            public string ShelveID { get; set; }
            public string AdminID { get; set; }
            public string ForOrderID { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime ModifyDate { get; set; }
            public DateTime? CompleteDate { get; set; }
            public string Summary { get; set; }
            public TempStockStatus Status { get; set; }
        }

        public class FileDescrition
        {
            public string SessionID { get; set; }
            public string FileID { get; set; }
            public string Url { get; set; }
            public string FileName { get; set; }
        }

        public enum TempStockStatus
        {
            /// <summary>
            /// 待处理
            /// </summary>
            [Description("待处理")]
            Waiting = 1,

            /// <summary>
            /// 已处理
            /// </summary>
            [Description("已处理")]
            Completed = 2,
        }
        #endregion

    }
    
}
