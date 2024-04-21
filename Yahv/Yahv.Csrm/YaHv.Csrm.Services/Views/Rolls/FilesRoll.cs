using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class FilesRoll : Origins.FilesOrigin
    {
        Enterprise enterprise;
        string companyid;
        FileType filetype;
        public FilesRoll(Enterprise Enterprise)
        {
            this.enterprise = Enterprise;
            this.filetype = FileType.BusinessLicense;
        }
        public FilesRoll(Enterprise Enterprise, FileType filetype, string companyid)
        {
            this.enterprise = Enterprise;
            this.companyid = companyid;
            this.filetype = filetype;
        }
        protected override IQueryable<FileDescription> GetIQueryable()
        {
            if (filetype == FileType.BusinessLicense)
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterprise.ID && item.Type == FileType.BusinessLicense);
            }

            else if (filetype == FileType.ServiceAgreement)
            {
                return from entity in base.GetIQueryable()
                       join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>()
                        on entity.ID equals maps.SubID
                       where maps.EnterpriseID == this.companyid && maps.Type == (int)MapsType.ServiceAgreement && entity.Enterprise.ID == this.enterprise.ID
                       select entity;
            }
            else if (filetype == FileType.WsAgreement)
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterprise.ID && item.Type == FileType.WsAgreement && item.CompanyID == this.companyid);
            }
            else
            {
                return null;
            }
        }
    }
}
