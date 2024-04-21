using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Rolls
{

    public class FilesDescriptionRoll : Origins.FilesDescriptionOrigin
    {
        String EnterpriseID;
        CrmFileType crmFileType;

        public FilesDescriptionRoll()
        {

        }
        public FilesDescriptionRoll(string enterpriseid)
        {
            this.EnterpriseID = enterpriseid;
        }

        public FilesDescriptionRoll(string enterpriseid,CrmFileType type)
        {
            this.EnterpriseID = enterpriseid;
            this.crmFileType = type;
        }
        protected override IQueryable<FilesDescription> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.EnterpriseID))
            {
                return new Origins.FilesDescriptionOrigin(this.Reponsitory).Where(item => item.EnterpriseID == this.EnterpriseID);
            }
            return new Origins.FilesDescriptionOrigin(this.Reponsitory);
        }
        public IQueryable<FilesDescription> this[string EnterpriseID, CrmFileType type]
        {
            get
            {
                return
                    this.Where(item => item.EnterpriseID == EnterpriseID && item.Type == type && item.Status == DataStatus.Normal);
            }
        }
        public IQueryable<FilesDescription> this[string EnterpriseID, params CrmFileType[] types]
        {
            get
            {
                return
                    this.Where(item => item.EnterpriseID == EnterpriseID && types.Contains(item.Type) && item.Status == DataStatus.Normal);
            }
        }
        public IQueryable<FilesDescription> this[string EnterpriseID, string subid, CrmFileType type]
        {
            get
            {
                return
                    this.Where(item => item.EnterpriseID == EnterpriseID && item.SubID == subid && item.Type == type && item.Status == DataStatus.Normal);
            }
        }



    }
}
