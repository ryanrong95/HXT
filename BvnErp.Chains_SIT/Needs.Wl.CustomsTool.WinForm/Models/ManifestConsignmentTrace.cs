using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models
{
    public class ManifestConsignmentTrace : IUnique, IPersist
    {
        public string ID { get; set; }
        public string ManifestConsignmentID { get; set; }
        public string StatementCode { get; set; }
        public string StatementName
        {
            get
            {
                return MultiEnumUtils.ToText<CusMftStatus>(this.StatementCode);
            }
        }
        public string Message { get; set; }
        public DateTime NoticeDate { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string BackupUrl { get; set; }

        public ManifestConsignmentTrace()
        {
            //TODO：设置默认值
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignmentTraces>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ManifestConsignmentTrace);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ManifestConsignmentTraces
                    {
                        ID = this.ID,
                        ManifestConsignmentID = this.ManifestConsignmentID,
                        StatementCode = this.StatementCode,
                        Message = this.Message,
                        NoticeDate = this.NoticeDate,
                        FileName = this.FileName,
                        BackupUrl = this.BackupUrl
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ManifestConsignmentTraces
                    {
                        ID = this.ID,
                        ManifestConsignmentID = this.ManifestConsignmentID,
                        StatementCode = this.StatementCode,
                        Message = this.Message,
                        NoticeDate = this.NoticeDate,
                        FileName = this.FileName,
                        BackupUrl = this.BackupUrl
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
