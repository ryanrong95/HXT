using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class PcFile : IUnique
    {
        public string ID { get; set; }
        public string MainID { get; set; }
        public Enums.PsOrderFileType Type { get; set; }
        public string CustomName { get; set; }
        public string Url { get; set; }
        public DateTime CreateDate { get; set; }
        public string AdminID { get; set; }
        public string SiteuserID { get; set; }

        //管理端fileUrl
        public string HttpUrl
        {
            get
            {
                string fileUrlPrefix = System.Configuration.ConfigurationManager.AppSettings["FileUrlPrefix"];
                var url = System.IO.Path.Combine(fileUrlPrefix, this.Url);
                return url;
            }
        }


        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.PcFile);

                    repository.Insert(new Layers.Data.Sqls.PsOrder.PcFiles()
                    {
                        ID = this.ID,
                        MainID = this.MainID,
                        Type = (int)this.Type,
                        CustomName = this.CustomName,
                        Url = this.Url,
                        CreateDate = this.CreateDate,
                        AdminID = this.AdminID,
                        SiteuserID = this.SiteuserID,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.Products>(new
                    {
                        this.MainID,
                        Type = (int)this.Type,
                        this.CustomName,
                        this.Url,
                        this.AdminID,
                        this.SiteuserID,
                    }, t => t.ID == this.ID);
                }
            }
        }

    }
}
