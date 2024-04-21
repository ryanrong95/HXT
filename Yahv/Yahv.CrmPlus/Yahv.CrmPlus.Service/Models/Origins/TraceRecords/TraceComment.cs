using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class TraceComment : IUnique
    {
        public string ID { get; set; }
        public string TraceRecordID { get; set; }

        public TraceRecord TraceRecord { get; set; }
        /// <summary>
        /// 是否指定阅读人
        /// </summary>
        public bool IsPointed { get; set; }
        /// <summary>
        /// 点评
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 阅读人ID
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// 阅读人
        /// </summary>
        public Admin Admin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
      
        public DateTime ModifyDate { get; set; }
        public TraceComment()
        {
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;

        }
        public event SuccessHanlder EnterSuccess;
        public void Enter()
        {

            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    //添加
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.TraceComments>().Any(item => item.ID == this.ID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Comment);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.TraceComments()
                        {
                            ID = this.ID,
                            TraceRecordID=this.TraceRecordID,
                            IsPointed=this.IsPointed,
                            Comments=this.Comments,
                            AdminID=this.AdminID,
                            CreateDate=this.CreateDate,
                            ModifyDate=this.ModifyDate

                        });
                    }
                    //修改
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.TraceComments>(new
                        {
                            ID = this.ID,
                            TraceRecordID = this.TraceRecordID,
                            IsPointed = this.IsPointed,
                            Comments = this.Comments,
                            AdminID = this.AdminID,
                            ModifyDate = this.ModifyDate

                        }, item => item.ID == this.ID);
                    }
                   
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));


        }


        /// <summary>
        /// 删除阅读人
        /// </summary>
        //public void Abandon()
        //{
        //    using (var reponsitory=new PvdCrmReponsitory())
        //    {
        //        reponsitory.Delete<Layers.Data.Sqls.PvdCrm.TraceComments>(item => item.TraceRecordID==this.TraceRecordID && item.AdminID==this.AdminID);
        //    }

        //}
    }
}
