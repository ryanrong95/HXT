using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace Yahv.PsWms.DappForm.Services.Controls
{
    public partial class WordShow : Form
    {
        public static string docUrl;

        private static List<int> wordProcess = new List<int>();//word进程集合
        public WordShow()
        {
            InitializeComponent();
        }

        private void WordShow_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            //GetWordApp(wordProcess);//获取客户端现有的word进程

            OpenWord(docUrl);//richtextbox控件加载temp.rtf
        }

        /// <summary>
        /// 打开Word文档
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenWord(string fileName)
        {

            Word.Application wordApp = new Word.Application();
            Word.Document document = null;

            object missing = System.Reflection.Missing.Value;
            object File = fileName;

            object readOnly = true;

            object isVisible = true;

            try
            {
                document = wordApp.Documents.Open(ref File, ref missing, ref readOnly,
                 ref missing, ref missing, ref missing, ref missing, ref missing,
                 ref missing, ref missing, ref missing, ref isVisible, ref missing,
                 ref missing, ref missing, ref missing);

                document.ActiveWindow.Selection.WholeStory();//全选word文档中的数据

                document.ActiveWindow.Selection.Copy();//复制数据到剪切板

                richTextBox1.Paste();//richTextBox粘贴数据
                richTextBox1.ReadOnly = true;

                //richTextBox1.Text = doc.Content.Text;//显示无格式数据

            }

            finally
            {

                document.Close(ref missing, ref missing, ref missing);
                wordApp.Quit(ref missing, ref missing, ref missing);
                Marshal.ReleaseComObject(wordApp);//释放
            }
        }

        static WordShow current;
        static public WordShow Current
        {
            get
            {
                if (current == null)
                {
                    current = new WordShow();
                }

                return current;
            }
        }

        private void WordShow_FormClosed(object sender, FormClosedEventArgs e)
        {
            current = null;
        }
    }
}
