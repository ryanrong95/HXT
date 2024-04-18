using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;
using Spire.Pdf.Annotations;
using Spire.Pdf.Annotations.Appearance;
using Needs.Wl.Models;
using Needs.Utils;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单文件
    /// </summary>
    public class DecHeadFile : IUnique, IPersist
    {
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DecHeadID, this.FileType).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string DecHeadID { get; set; }

        string adminID;
        public string AdminID
        {
            get
            {
                return this.adminID ?? this.Admin?.ID;
            }
            set
            {
                this.adminID = value;
            }
        }

        public Admin Admin { get; set; }

        public string Name { get; set; }

        public FileType FileType { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        //新添加
        public DecHead DecHead { get; set; }

        public DecHeadFile()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            //0625 不加盖章
            //this.EnterSuccess += DecHeadFile_Successed;

            //读取缴款书填发日期 20220510 ryan
            this.EnterSuccess += DecHeadFile_EnterSuccess;
        }

        public DecHeadFile(DecHead DecHead) : this()
        {

        }
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual public void OnEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>().Where(item => item.DecHeadID == this.DecHeadID && item.FileType == (int)this.FileType);
                if (count.Count() == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecHeadFiles
                    {
                        ID = this.ID,
                        DecHeadID = this.DecHeadID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecHeadFiles
                    {
                        ID = this.ID,
                        DecHeadID = this.DecHeadID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    }, t => t.ID == this.ID);
                }
            }
        }

        private void DecHeadFile_Successed(object sender, SuccessEventArgs e)
        {
            var decheadfile = (DecHeadFile)e.Object;
            if (decheadfile.FileType == FileType.DecHeadFile)
            {
                var AgentName = PurchaserContext.Current.CompanyName;
                #region 加盖图章

                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(new FileDirectory().FilePath + @"\" + decheadfile.Url);
                PdfPageBase page = doc.Pages[0];

                #region 公司章
                PdfRubberStampAnnotation loStampDec = new PdfRubberStampAnnotation(new RectangleF(new PointF(470, -50), new SizeF(216, 300)));
                PdfAppearance loApprearanceDec = new PdfAppearance(loStampDec);
                var image =PurchaserContext.Current.DeclareStamp;


                PdfImage imageDec = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, image));
                PdfTemplate templateDec = new PdfTemplate(210, 297);
                templateDec.Graphics.DrawImage(imageDec, 0, 0);
                loApprearanceDec.Normal = templateDec;
                loStampDec.Appearance = loApprearanceDec;
                page.AnnotationsWidget.Add(loStampDec);
                #endregion

                #region 人名章
                PdfRubberStampAnnotation loStampUser = new PdfRubberStampAnnotation(new RectangleF(new PointF(505, -465), new SizeF(216, 300)));
                PdfAppearance loApprearanceUser = new PdfAppearance(loStampUser);
                var penson = PurchaserContext.Current.DeclareNameStampUrl;
                PdfImage imageUser = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, penson));
                PdfTemplate templateUser = new PdfTemplate(210, 297);
                templateUser.Graphics.DrawImage(imageUser, 0, 0);
                loApprearanceUser.Normal = templateUser;
                loStampUser.Appearance = loApprearanceUser;
                page.AnnotationsWidget.Add(loStampUser);
                #endregion

                doc.SaveToFile(new FileDirectory().FilePath + @"\" + decheadfile.Url);
                #endregion
            }

        }

        //读取缴款书 填发日期
        private void DecHeadFile_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var decheadfile = (DecHeadFile)e.Object;
            if (decheadfile.FileType == FileType.DecHeadVatFile)
            {
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(new FileDirectory().FilePath + @"\" + decheadfile.Url);

                //实例化一个StringBuilder 对象
                StringBuilder content = new StringBuilder();

                //提取PDF所有页面的文本
                foreach (PdfPageBase page in doc.Pages)
                {
                    content.Append(page.ExtractText());
                }

                var txt = content.ToString();

                var date中文 = txt.Substring(txt.IndexOf("填发日期：") + 5, 12).Replace("缴", "");

                var date中文标准 = System.Text.RegularExpressions.Regex.Match(date中文, @"(\s*)(\d+)(\s*)年(\s*)(\d+)(\s*)月(\s*)(\d+)(\s*)日(\s*)").Value;

                var date = DateTime.Parse(date中文标准.Replace(" ", ""));

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var vatflow = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().FirstOrDefault(item => item.DecTaxID == this.DecHeadID && item.TaxType == (int)DecTaxType.AddedValueTax);
                    if (vatflow != null)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(new { FillinDate = date }, item => item.ID == vatflow.ID);
                    }
                }
            }
        }
    }
}