using Layer.Data.Sqls;
using Layer.Linq;
using Needs.Linq;
using NtErp.Wss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web;
using System.Configuration;
using System.Transactions;
using System.IO;

namespace NtErp.Wss.Services.Models
{
    public partial class Boms :  Needs.Linq.IUnique
    {
        public string ID { set; get; }
        public string ClientID { get; set; }
        public string Uri { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }

        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder AbandonSuccess;

        public Boms()
        {

            this.CreateDate = DateTime.Now;
        }
        public void Enter()
        {
            using (var repository = new BomsReponsitory())
            {

                if (string.IsNullOrEmpty(this.ID))
                {
                    repository.Insert(new Layer.Data.Sqls.Boms.Boms
                    {
                        ID = Needs.Overall.PKeySigner.Pick(Needs.Overall.PKeyType.NewBom),
                        ClientID = this.ClientID,
                        CreateDate = this.CreateDate,
                        Contact = this.Contact,
                        Email = this.Email,
                        Uri = this.Uri
                    });
                }
                else
                {
                    this.EnterError(this, new Needs.Linq.ErrorEventArgs());
                }
            }


            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Abandon()
        {
            using (var repository = new BomsReponsitory())
            {
                var entiy = new BomsView().Where(item => item.ClientID == this.ClientID && item.ID == this.ID).FirstOrDefault();
                if (entiy == null)
                {
                    throw new Exception("no exist!");
                }

                using (var trans=new TransactionScope())
                {
                    repository.Delete<Layer.Data.Sqls.Boms.Boms>
                       (item => item.ID == this.ID);

                    //删除上传文件
                    
                    var file = string.Concat(Needs.Utils.NetFile.GetFolderPath(Needs.Utils.NetFileType.NewBom),"\\", System.IO.Path.GetFileName(entiy.Uri));
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }


                    trans.Complete();
                }
               
                

                if (AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
            }

        }

     
    }
}
