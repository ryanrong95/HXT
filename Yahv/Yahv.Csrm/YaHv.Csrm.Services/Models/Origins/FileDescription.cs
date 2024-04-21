using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class FileDescription : Yahv.Linq.IUnique
    {
        public readonly string UpLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\UpLoad\";

        public FileDescription()
        {
            this.CreateDate = DateTime.Now;
            this.Status = ApprovalStatus.Normal;
        }
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return string.Join("", this.EnterpriseID, this.Url, this.Type, this.CompanyID).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 文件类型：营业执照、委托合同
        /// </summary>
        public FileType Type { set; get; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { set; get; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 录入人
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 企业信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 内部公司
        /// </summary>
        public string CompanyID { set; get; }
        public string MapsID
        {
            get
            {
                return string.Join("", "Agreement_", this.ID);
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化
        public void Enter(object dic = null)
        {
            SaveCenterFiles();
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }

        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.FilesDescription>().Any(item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)FileType.BusinessLicense && item.Status == (int)ApprovalStatus.Normal))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.FilesDescription>(new
                    {
                        Status = (int)ApprovalStatus.Deleted
                    }, item => item.ID == this.ID);
                }

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }


        /// <summary>
        //        /// 创建文件目录
        //        /// </summary>
        public void CreateDirectory()
        {
            //上传目录
            FileInfo uploadDirectory = new FileInfo(this.UpLoadRoot);
            if (!uploadDirectory.Directory.Exists)
            {
                uploadDirectory.Directory.Create();
            }
            //下载目录
            FileInfo downDirectory = new FileInfo(this.UpLoadRoot);
            if (!downDirectory.Directory.Exists)
            {
                downDirectory.Directory.Create();
            }
        }
        /// <summary>
        /// Copy文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetFileFromNetUrl(string url)
        {
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.Method = "GET";
                //获得用户名密码的Base64编码  添加Authorization到HTTP头 不需要的账号密码的可以注释下面两行代码
                string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "File", "pdejhff1")));
                req.Headers.Add("Authorization", "Basic " + code);
                byte[] fileBytes;
                using (WebResponse webRes = req.GetResponse())
                {
                    int length = (int)webRes.ContentLength;
                    HttpWebResponse response = webRes as HttpWebResponse;
                    Stream stream = response.GetResponseStream();

                    //读取到内存
                    MemoryStream stmMemory = new MemoryStream();
                    byte[] buffer = new byte[length];
                    int i;
                    //将字节逐个放入到Byte中
                    while ((i = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stmMemory.Write(buffer, 0, i);
                    }
                    fileBytes = stmMemory.ToArray();//文件流Byte，需要文件流可直接return，不需要下面的保存代码
                    stmMemory.Close();

                    MemoryStream m = new MemoryStream(fileBytes);
                    string file = string.Format(UpLoadRoot + System.IO.Path.GetFileName(this.Url));//可根据文件类型自定义后缀
                    FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
                    m.WriteTo(fs);
                    m.Close();
                    fs.Close();
                    return file;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 上传至中心库
        /// </summary>
        void SaveCenterFiles()
        {
            CreateDirectory();
            string path = GetFileFromNetUrl(this.Url);
            var result = Yahv.Services.Views.CenterFilesTopView.Upload(path, this.Type,
                       new
                       {
                           CustomName = this.Name,
                           AdminID = this.CreatorID,
                           ClientID = this.EnterpriseID,
                       });
            var uploadResult = result.FirstOrDefault();
            System.IO.File.Delete(path);
            using (var reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //目前上传到中心的有营业执照，服务协议，代仓储协议和企业Logo，都是唯一的
                reponsitory.Update(new Layers.Data.Sqls.PvCenter.FilesDescription
                {
                    Status = (int)FileDescriptionStatus.Delete,
                }, item => item.ClientID == this.EnterpriseID && item.ID != uploadResult.FileID && item.Type == (int)this.Type);
                //switch (this.Type)
                //{
                //    //营业执照：一个客户只有一个营业执照
                //    case FileType.BusinessLicense:
                //        reponsitory.Update(new Layers.Data.Sqls.PvCenter.FilesDescription
                //        {
                //            Status = (int)FileDescriptionStatus.Delete,
                //        }, item => item.ClientID == this.EnterpriseID && item.ID != uploadResult.FileID && item.Type == (int)FileType.BusinessLicense);
                //        break;
                //    //服务协议上传
                //    case FileType.ServiceAgreement:
                //        reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>( new Layers.Data.Sqls.PvCenter.FilesDescription
                //        {
                //            Status = (int)FileDescriptionStatus.Delete,
                //        }, item => item.ClientID == this.EnterpriseID && item.ID != uploadResult.FileID && item.Type == (int)FileType.ServiceAgreement);

                //        break;
                //    //代仓储协议上传
                //    case FileType.WsAgreement:
                //        reponsitory.Update(new Layers.Data.Sqls.PvCenter.FilesDescription
                //        {
                //            Status = (int)FileDescriptionStatus.Delete,
                //        }, item => item.ClientID == this.EnterpriseID && item.ID != uploadResult.FileID && item.Type == (int)FileType.WsAgreement);

                //        break;
                //    //企业Logo
                //    case FileType.EnterpriseLogo:
                //        reponsitory.Update(new Layers.Data.Sqls.PvCenter.FilesDescription
                //        {
                //            Status = (int)FileDescriptionStatus.Delete,
                //        }, item => item.ClientID == this.EnterpriseID && item.ID != uploadResult.FileID && item.Type == (int)FileType.EnterpriseLogo);
                //        break;
                //}
            }
        }
    }
    #endregion
}

