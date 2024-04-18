using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Needs.Wl.CustomsTool.WinForm.Services;
using Needs.Wl.CustomsTool.WinForm.Models;
using System.IO;
using Needs.Utils.Descriptions;
using Needs.Utils;
using Needs.Ccs.Services.Models;
using System.Net;
using Needs.Utils.Converters;
using System.Threading;

namespace Needs.Wl.CustomsTool.WinForm
{
    public partial class UploadControl : UserControl
    {
        List<FileModels> filelist = new List<FileModels>();  //文件表

        public UploadControl()
        {
            InitializeComponent();
            BindGridData();
        }

        /// <summary>
        /// 初始化文件列表
        /// </summary>
        /// <param name="listView"></param>
        private void InitListView(ListView listView)
        {
            listView.SmallImageList = new ImageList();
            listView.LargeImageList = new ImageList();

            listView.View = View.Details;
            listView.AllowDrop = true;
        }

        /// <summary>
        /// 绑定订单列表
        /// </summary>
        private void BindGridData()
        {
            var unUploadDecHeads = Tool.Current.Customs.UnUploadDecHeadsList.ToList().Where(item => item.IsSuccess && item.IsDecHeadFile == "否").OrderByDescending(item => item.DDate);
            dataGridView1.DataSource = unUploadDecHeads.Select(item => new
            {
                ID = item.ID,
                ContrNo = item.ContrNo,
                OrderID = item.OrderID,
                EntryID = item.EntryId,
                Currency = item.Currency,
                SwapAmount = item.DecAmount,
                Status = item.StatusName,
            }).ToArray();

            dataGridView1.AutoResizeColumns();
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "合同号";
            dataGridView1.Columns[2].HeaderText = "订单编号";
            dataGridView1.Columns[3].HeaderText = "海关单号";
            dataGridView1.Columns[4].HeaderText = "币种";
            dataGridView1.Columns[5].HeaderText = "报关金额";
            dataGridView1.Columns[6].HeaderText = "报关状态";
        }

        /// <summary>
        /// 绑定文件列表
        /// </summary>
        private void ListFolder()
        {
            ListFolder(labelCurFolder.Text);
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ListFolder(folderBrowserDialog.SelectedPath);
            }
        }

        /// <summary>
        /// 绑定文件列表
        /// </summary>
        /// <param name="directory">the directory of the folder</param>
        private void ListFolder(string directory)
        {
            String[] files = System.IO.Directory.GetFiles(directory);


            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string name = Path.GetFileNameWithoutExtension(file);
                FileModels model = new FileModels();
                model.Name = name;
                model.FileFormat = extension;
                model.URL = file;
                model.ID = Guid.NewGuid().ToString();
                if (extension.ToLower() != ".pdf")
                {
                    model.Status = Status.Fail;
                }
                else
                {
                    model.Status = Status.UnDo;
                }
                filelist.Add(model);
                Action<FileModels> action = new Action<FileModels>(uploadFiles);
                action.Invoke(model);
            }
        }

        /// <summary>
        /// 列表数据绑定
        /// </summary>
        private void bindListView()
        {
            listViewFolder.Items.Clear();
            foreach (var file in filelist)
            {
                ListViewItem itemName = new ListViewItem(file.Name);

                //Show file icon
                IconImageProvider iconImageProvider = new IconImageProvider(listViewFolder.SmallImageList, listViewFolder.LargeImageList);
                itemName.ImageIndex = iconImageProvider.GetIconImageIndex(file.URL);

                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = file.Status.GetDescription();
                itemName.SubItems.Add(subItem);

                //Show file time
                subItem = new ListViewItem.ListViewSubItem();
                DateTime fileTime = System.IO.File.GetLastWriteTime(file.URL);

                subItem.Text = (string)fileTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                itemName.SubItems.Add(subItem);
                listViewFolder.Items.Add(itemName);
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        private void uploadFiles(FileModels model)
        {
            try
            {
                var path = model.URL;
                var uploadPath = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
                var fileName = Path.GetFileName(path);
                //要上传的文件
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                //二进制对象
                BinaryReader r = new BinaryReader(fs);
                //时间戳
                string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
                //请求的头部信息
                StringBuilder sb = new StringBuilder();
                sb.Append("--");
                sb.Append(strBoundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"");
                sb.Append("file");
                sb.Append("\"; filename=\"");
                sb.Append(fileName);
                sb.Append("\";");
                sb.Append("\r\n");
                sb.Append("Content-Type: ");
                sb.Append("application/pdf");
                sb.Append("\r\n");
                sb.Append("\r\n");
                string strPostHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
                // 根据uri创建HttpWebRequest对象   
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(uploadPath));
                httpReq.Method = "POST";
                //对发送的数据不使用缓存   
                httpReq.AllowWriteStreamBuffering = false;
                //设置获得响应的超时时间（300秒）   
                httpReq.Timeout = 300000;
                httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
                long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
                long fileLength = fs.Length;
                httpReq.ContentLength = length;
                try
                {
                    //每次上传4k  
                    int bufferLength = 4096;
                    byte[] buffer = new byte[bufferLength]; //已上传的字节数   
                    long offset = 0;         //开始上传时间   
                    DateTime startTime = DateTime.Now;
                    int size = r.Read(buffer, 0, bufferLength);
                    Stream postStream = httpReq.GetRequestStream();         //发送请求头部消息   
                    postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                    while (size > 0)
                    {
                        postStream.Write(buffer, 0, size);
                        offset += size;
                        TimeSpan span = DateTime.Now - startTime;
                        double second = span.TotalSeconds;
                        Application.DoEvents();
                        size = r.Read(buffer, 0, bufferLength);
                    }
                    //添加尾部的时间戳   
                    postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    postStream.Close();
                    //获取服务器端的响应   
                    WebResponse webRespon = httpReq.GetResponse();
                    Stream s = webRespon.GetResponseStream();
                    //读取服务器端返回的消息  
                    StreamReader sr = new StreamReader(s);
                    String sReturnString = sr.ReadLine();
                    var file = filelist.Where(item => item.ID == model.ID).FirstOrDefault();
                    if (sReturnString == "Success")
                    {
                        file.Status = Status.Success;
                    }
                    else
                    {
                        file.Status = Status.Fail;
                    }
                    bindListView();
                    BindGridData();
                    s.Close();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    // var aa = ex;
                }
                finally
                {
                    fs.Close();
                    r.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void UploadControl_Load(object sender, EventArgs e)
        {
            InitListView(listViewFolder);
        }

        private void listViewFolder_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
                //Copy file from external application
                foreach (string srcfile in files)
                {
                    var ext = Path.GetExtension(srcfile);
                    if (ext.ToLower() == ".pdf")
                    {
                        FileModels model = new FileModels()
                        {
                            ID = Guid.NewGuid().ToString(),
                            FileFormat = ext,
                            Name = Path.GetFileName(srcfile),
                            Status = Status.UnDo,
                            URL = srcfile,
                        };
                        filelist.Add(model);
                        bindListView();
                        Action<FileModels> action = new Action<FileModels>(uploadFiles);
                        action.Invoke(model);
                    }
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listViewFolder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = BaseStyleSetting.DefaultCellStyle_BackColor;
            }
        }

        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }
    }
}
