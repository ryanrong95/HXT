using Needs.Ccs.Services;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models
{
    /// <summary>
    /// 舱单
    /// </summary>
    public sealed class ManifestConsignment : Needs.Ccs.Services.Models.ManifestConsignment
    {
        /// <summary>
        /// 舱单申报（报文准备就绪）
        /// </summary>
        public event ManifestApplyHanlder ManifestApplied;

        public ManifestConsignment()
        {
            //报关申报（报文准备就绪）
            this.ManifestApplied += ManifestConsignment_ManifestApply;
        }

        /// <summary>
        /// 单据状态名字
        /// </summary>
        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Ccs.Services.Enums.CusMftStatus>(this.CusMftStatus);
            }
        }

        /// <summary>
        /// 舱单申报
        /// </summary>
        public new void Apply()
        {
            var FunCode = this.CusMftStatus == MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleting) ? "_03" : string.Empty;
            string fileName = this.Manifest.ID + "_" + this.ID + FunCode + ".zip.temp";
            var fileServerUrl = Tool.Current.Company.FileServerUrl;  //服务器地址
            var rmftMainFolder = Tool.Current.Folder.RmftMainFolder; //舱单主目录

            #region 下载文件到本地
            //创建文件夹
            FileDirectory file = new FileDirectory();
            System.Net.WebClient wbClient = new System.Net.WebClient();
            List<string> files = new List<string>();
            var clientPath = string.Empty;
            clientPath = rmftMainFolder + @"\" + ConstConfig.Message + @"\" + this.Manifest.ID + "_" + this.ID + FunCode + ".xml";
            files.Add(clientPath);
            var filepath = fileServerUrl + @"\" + this.MarkingUrl;
            wbClient.DownloadFile(filepath, clientPath);
            #endregion

            #region 压缩并删除文件
            //压缩单个文件
            ZipSingleFile zip = new ZipSingleFile(fileName);
            zip.File = files[0];
            zip.ContainedFileName = this.Manifest.ID + "_" + this.ID + FunCode + ".xml";
            zip.ZipedPath = rmftMainFolder + @"\" + ConstConfig.OutBox + @"\";
            zip.ZipFileManifest();
            //删除已被压缩的源文件
            files.ForEach(t =>
            {
                File.Delete(t);
            });
            //.temp文件重命名
            File.Move(zip.ZipedPath + fileName, zip.ZipedPath + this.Manifest.ID + "_" + this.ID + FunCode + ".zip");
            #endregion

            #region 更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //删除舱单，直接修改为已删除，不再等待海关回执
                if (this.CusMftStatus == MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleting))
                {
                    this.CusMftStatus = MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleted);
                }
                else
                {
                    this.CusMftStatus = MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Declare);
                }
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.CusMftStatus }, item => item.ID == this.ID);
            }
            this.OnApplied(new ManifestApplyEventArgs(this));
            #endregion
        }

        public override void OnApplied(ManifestApplyEventArgs args)
        {
            this.ManifestApplied?.Invoke(this, args);
        }

        private void ManifestConsignment_ManifestApply(object sender, ManifestApplyEventArgs e)
        {
            if (e.ManifestConsignment.CusMftStatus == MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleted))
            {
                e.ManifestConsignment.Trace("舱单已删除");
            }
            else
            {
                e.ManifestConsignment.Trace("导出报文.zip至文件夹，等待发送或自动发送至海关");
            }
            
        }
    }
}
