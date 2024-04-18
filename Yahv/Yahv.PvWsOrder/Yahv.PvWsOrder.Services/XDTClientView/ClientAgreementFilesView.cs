using System;
using System.Linq;
using System.Security.Cryptography;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 我的协议书
    /// </summary>
    public class ClientAgreementFilesView : IUnique
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public FileType FileType { get; set; }

        public string Url { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }


        public void Enter()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    Status = 400,
                }, item => item.ClientID == this.ClientID && item.Type == (int)this.FileType);

                CenterFileDescription[] files = new CenterFileDescription[] {
                    new CenterFileDescription
                    {
                        CustomName = this.Name,
                        Url = this.Url,
                        AdminID = this.UserID,
                        ClientID = this.ClientID,
                        Type = (int) this.FileType,
                        Status=FileDescriptionStatus.Normal
                    }
                };
                //上传文件
                new Yahv.PvWsOrder.Services.ClientViews.CenterFilesView().Upload(files);
            }
        }
    }
}
