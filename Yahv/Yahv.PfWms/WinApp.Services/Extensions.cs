using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services
{
    internal class Extensions : IEnumerable<string>
    {
        Dictionary<string, FilePrintType> dic;

        internal Extensions()
        {
            this.dic = new Dictionary<string, FilePrintType>();
            var words = ".doc|.docx".Split('|');
            //var excels = ".xls|.xlsx".Split('|');
            this.Init(FilePrintType.Word, words);
            //this.Init(FilePrintType.Excel, excels);
            var images = ".BMP、.JPG、.JPEG、.PNG".ToLower().Split('、');
            this.Init(FilePrintType.Image, images);

            var pdf = ".pdf".ToLower().Split('|');
            this.Init(FilePrintType.Pdf, pdf);



        }

        void Init(FilePrintType type, IEnumerable<string> collent)
        {
            foreach (var item in collent)
            {
                this.dic.Add(item, type);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.dic.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public FilePrintType this[string index]
        {
            get { return this.dic[index.ToLower()]; }
        }
        static Extensions current;
        public static Extensions Current
        {

            get
            {
                if (current == null)
                {
                    current = new Extensions();
                }

                return current;
            }
        }


        public string GetOpenFileDialogFilter()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("可用文件:");
            builder.Append(nameof(FilePrintType.Word));
            builder.Append(',');
            builder.Append(nameof(FilePrintType.Pdf));
            builder.Append(',');
            builder.Append(nameof(FilePrintType.Image));
            builder.Append('|');    

            builder.Append(string.Join(";", this.Select(item => "*" + item)));

            return  builder.ToString();

        }
    }
}
