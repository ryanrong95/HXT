using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.VcCsrm.Service.Extends;

namespace YaHv.VcCsrm.Service.Models
{
    /// <summary>
    /// 代仓储合同
    /// </summary>
    public class WsContract : Yahv.Linq.IUnique
    {
        public WsContract()
        {
            this.Status = GeneralStatus.Normal;
            this.CreateDate = DateTime.Now;
            this.TrusteeID = "HK LIANCHUANG ELECTRONICS., LIMITED";//暂时默认
            this.Currency = Currency.HKD;//暂时默认
        }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 受托方即内部公司，乙方
        /// </summary>

        public string TrusteeID { set; get; }
        /// <summary>s
        /// 委托方即客户
        /// </summary>
        public string WsClientID { set; get; }
        /// <summary>
        /// 合同协议开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 合同协议结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 货柜数量
        /// </summary>
        public int ContainerNum { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }

        /// <summary>
        /// 仓储费
        /// </summary>
        public decimal Charges { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        //public Admin Creator { internal set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
      
        /// <summary>
        /// 服务协议
        /// </summary>

        Yahv.Services.Models.CenterFileDescription agreement;
        public Yahv.Services.Models.CenterFileDescription Agreement
        {
            get
            {
                using (var view = new Yahv.Services.Views.CenterFilesTopView())
                {
                    return null;
                    //view.FirstOrDefault(item =>item.ShipID==this.ShipID&& item.Status == ApprovalStatus.Normal);
                }
            }
            set
            {
                this.agreement = value;
            }
        }

        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        virtual public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (string.IsNullOrEmpty(this.ID))
                {
                    this.ID = PKeySigner.Pick(Services.PKeyType.WsContact);
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    var old = new Views.Rolls.WsContractsRoll(this.WsClientID).SingleOrDefault(item => item.ID == this.ID);
                    if (old.Equals(this))
                    {
                        if (old.Summary != this.Summary)
                        {
                            repository.Update<Layers.Data.Sqls.PvcCrm.WsContracts>(new
                            {
                                Summary = this.Summary
                            }, item => item.ID == this.ID);
                        }
                    }
                    else
                    {
                        this.ID = PKeySigner.Pick(Services.PKeyType.WsContact);
                        repository.Update<Layers.Data.Sqls.PvcCrm.WsContracts>(new
                        {
                            Status = GeneralStatus.Deleted
                        }, item => item.WsClientID == this.WsClientID && item.Trustee == this.TrusteeID);
                        repository.Insert(this.ToLinq());
                    }
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void Abandon()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public override bool Equals(object obj)
        {
            var model = obj as WsContract;
            if (model == null)
            {
                return false;
            }
            else
            {
                return model.GetHashCode() == this.GetHashCode();
            }
        }
        public override int GetHashCode()
        {
            return string.Concat(
                TrusteeID,
                WsClientID,
                StartDate,
                EndDate,
                ContainerNum,
                Currency,
               System.Math.Round(Charges, 4)
                ).GetHashCode();
        }
        //#region 模板导出
        ///// <summary>
        ///// 保存文件
        ///// </summary>
        ///// <param name="filePath">文件路径</param>
        //public void SaveAs(string filePath, bool toHtml = false)
        //{
        //    var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates\\委托代收、代发、代管协议书（香港库房最终定稿）.doc");
        //    Aspose.Words.Document doc = new Aspose.Words.Document(tempPath);//新建一个空白的文档
        //    Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

        //    Dictionary<string, string> dic = ContractDic();

        //    foreach (var key in dic.Keys)   //循环键值对
        //    {
        //        builder.MoveToBookmark(key);  //将光标移入书签的位置
        //        builder.Write(dic[key]);   //填充值
        //    }
        //    if (toHtml)
        //    {
        //        doc.Save(filePath, Aspose.Words.SaveFormat.Html);
        //    }
        //    else
        //    {
        //        doc.Save(filePath);
        //    }
        //}


        //public Dictionary<string, string> ContractDic()
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();   //创建键值对   第一个string 为书签名称 第二个string为要填充的数据
        //    #region 组装内容
        //    dic.Add("StartDate", this.StartDate.ToString("yyyy年MM月dd日"));
        //    dic.Add("EndDate", this.EndDate.ToString("yyyy年MM月dd日"));
        //    dic.Add("WsClientName", this.WsClient.Name);
        //    dic.Add("Charges", this.Charges.ToString());
        //    dic.Add("ContainerNum", this.ContainerNum.ToString());

        //    #region 计算年份
        //    int year = this.EndDate.Year - this.StartDate.Year;
        //    var y = "";
        //    string[] parm = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖", "拾" };
        //    if (year < 10 || year == 10)
        //    {
        //        y = parm[year];
        //    }
        //    else if (year < 100)
        //    {
        //        y = year / 10 == 1 ? "" : parm[year / 10] + '拾' + parm[year % 10];
        //    }

        //    dic.Add("Years", y);
        //    #endregion


        //    #endregion
        //    return dic;
        //}
        //#endregion
        #endregion
    }
}
