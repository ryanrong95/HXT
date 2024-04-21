using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Yahv.Erm.WebApp.Tests
{
    public partial class RenderTester : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var k = Services.UploadWages.Current;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //把最终要输出的html压缩后再输出
            StringWriter html = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(html);
            base.Render(tw);

            string outhtml = html.ToString();
            outhtml = Regex.Replace(outhtml, "\\s+", " ");
            outhtml = Regex.Replace(outhtml, ">\\s+<", "><");
            outhtml = outhtml.Trim();
            writer.Write(outhtml);
        }

    }
}