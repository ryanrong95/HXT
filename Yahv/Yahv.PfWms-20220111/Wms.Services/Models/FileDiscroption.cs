//using Layers.Data;
//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Enums;
//using Wms.Services.Extends;
//using Yahv.Linq;
//using Yahv.Linq.Persistence;
//using Yahv.Services.Enums;
//using Yahv.Underly;
//using Yahv.Usually;

//namespace Wms.Services.Models
//{
//    /// <summary>
//    /// 文件类
//    /// </summary>
//    public class FileDescription : IUnique, IPersisting
//    {
//        #region 事件
//        public event SuccessHanlder EnterSuccess;
//        public event ErrorHanlder EnterError;
//        public event SuccessHanlder AbandonSuccess;
//        public event ErrorHanlder AbandonError;
//        #endregion

//        #region 属性
//        /// <summary>
//        /// 唯一码 四位年+2位月+2日+6位流水
//        /// </summary>
//        public string ID { get; set; }
//        /// <summary>
//        /// 运单ID
//        /// </summary>
//        public string WaybillID { get; set; }

//        public string NoticeID { get; set; }

//        /// <summary>
//        /// 库存ID
//        /// </summary>
//        public string StorageID { get; set; }
        
//        /// <summary>
//        /// 客户自定义名称
//        /// </summary>
//        public string CustomName { get; set; }
//        /// <summary>
//        /// 文件类型
//        /// </summary>
//        public FileType Type { get; set; }
//        /// <summary>
//        /// 文件地址(自动变更名称)
//        /// </summary>
//        public string Url { get; set; }

//        /// <summary>
//        /// 绝对url
//        /// </summary>
//        public string AbsoluteUrl
//        {
//            get
//            {
//                if (Url!=null && (Url.StartsWith("https://") || Url.StartsWith("http://")))
//                {
//                    return Url;
//                }
//                return string.Concat(FromType.Scheme.GetDescription(), "://", FromType.WebApi.GetDescription(), this.Url);
//            }
//        }
//        /// <summary>
//        /// 上传时间
//        /// </summary>
//        public DateTime CreateDate { get; internal set; }
//        /// <summary>
//        /// 状态：200、正常；400、删除；500、停用
//        /// </summary>
//        public FileStatus Status { get; set; }
//        /// <summary>
//        /// 客户ID
//        /// </summary>
//        public string ClientID { get; set; }
//        /// <summary>
//        /// 添加人
//        /// </summary>
//        public string AdminID { get; set; }
//        /// <summary>
//        /// 进项ID
//        /// </summary>
//        public string InputID { get; set; }
        
//        #endregion

//        #region 扩展属性
//        /// <summary>
//        /// Type的枚举扩展
//        /// </summary>
//        public string TypeDes
//        {
//            get
//            {
//                return this.Type.GetDescription();
//            }
//        }
//        /// <summary>
//        /// Status的枚举扩展
//        /// </summary>
//        public string StatusDes
//        {
//            get
//            {
//                return this.Status.GetDescription();
//            }
//        }

//        /// <summary>
//        /// 本地文件路径
//        /// </summary>
//        public string LocalFile { get; set; }
//        /// <summary>
//        /// 文件base64位编码
//        /// </summary>
//        public string FileBase64Code { get; set; }
//        #endregion

//        #region 废弃
//        public void Abandon()
//        {
//            try
//            {
//                using (var repository = new PvWmsRepository())
//                {
//                    this.CreateDate = DateTime.Now;
//                    this.Status = FileStatus.Deleted;
//                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
//                }
//                AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
//            }
//            catch (Exception ex)
//            {
//                AbandonError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
//            }

//        }
//        #endregion

//        #region 持久化
//        public void Enter()
//        {
//            try
//            {
//                using (var repository = new PvWmsRepository())
//                {
//                    //ID为空是新增
//                    if (string.IsNullOrWhiteSpace(this.ID))
//                    {
//                        this.ID = PKeySigner.Pick(PkeyType.FileInfos);
//                        this.Status = FileStatus.Normal;
//                        this.CreateDate = DateTime.Now;
//                        repository.Insert(this.ToLinq());
//                    }
//                    else
//                    {
//                        repository.Update(this.ToLinq(), item => item.ID == this.ID);
//                    }
//                }
//                EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
//            }
//            catch (Exception ex)
//            {
//                EnterError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));

//            }
//        }
//        #endregion
//    }
//}
