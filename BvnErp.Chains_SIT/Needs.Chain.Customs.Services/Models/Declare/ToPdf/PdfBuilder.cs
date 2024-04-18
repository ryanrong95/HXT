extern alias globalB;

using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using Needs.Utils.Flow.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PdfBuilder
    {
        private List<EventBuilder<PdfBuilder>> Steps = new List<EventBuilder<PdfBuilder>>();

        private Document Document { get; set; }

        private string FileName { get; set; } = string.Empty;

        public List<string> DottedFunNames { get; set; } = new List<string>();

        public Dictionary<string, object> Params = new Dictionary<string, object>();

        public PdfBuilder(string fileName, Dictionary<string, object> parameters)
        {
            this.FileName = fileName;
            this.Params = parameters;

            Rectangle rec = new Rectangle(PageSize.A4);
            //创建一个文档实例。 去除边距
            this.Document = new Document(rec);
        }

        public void SetDocumentMargins(float marginLeft, float marginRight, float marginTop, float marginBottom)
        {
            this.Document.SetMargins(marginLeft, marginRight, marginTop, marginBottom);
        }

        public PdfBuilder AddStep(Utils.Flow.Event.EventHandler<PdfBuilder> handler)
        {
            var eventHandler = new EventBuilder<PdfBuilder>();
            eventHandler.Append(handler);
            this.Steps.Add(eventHandler);
            return this;
        }

        public void ToPdf()
        {
            try
            {
                //创建一个writer实例
                var pdfWriter = PdfWriter.GetInstance(this.Document, new FileStream(this.FileName, FileMode.OpenOrCreate));

                //打开当前文档
                this.Document.Open();

                foreach (var step in this.Steps)
                {
                    var handlerInfo = step.HandlerInfos[0];

                    if (this.DottedFunNames.Contains(handlerInfo.MethodName))
                    {
                        PdfContentByte cb = pdfWriter.DirectContent;
                        cb.SetLineWidth(1f);
                        cb.SetLineDash(1f, 2f, 0f);
                    }
                    else
                    {
                        PdfContentByte cb = pdfWriter.DirectContent;
                        cb.SetLineWidth(1f);
                        cb.SetLineDash(0f, 0f, 0f);
                    }

                    object element = step.Execute(this);
                    this.Document.Add((IElement)element);
                }

            }
            catch (DocumentException docEx)
            {
                throw (docEx);
            }
            catch (IOException ex)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                //关闭document
                this.Document.Close();
            }
        }
    }

    public enum LineStyle
    {
        Solid = 1,

        Dotted = 2,
    }
}
