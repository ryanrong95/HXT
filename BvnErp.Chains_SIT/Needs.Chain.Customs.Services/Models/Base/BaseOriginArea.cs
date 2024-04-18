using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 港口代码
    /// </summary>
    public class BaseOriginArea : IUnique
    {
        private string id;

        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Code, this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string Code { get; set; }

        public string Name { get; set; }

       

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;


        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseOriginArea>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.BaseOriginArea
                    {
                        ID = this.ID,
                        Code = this.Code,                       
                        Name = this.Name
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.BaseOriginArea
                    {
                        Code = this.Code,                   
                        Name = this.Name
                    }, item => item.ID == this.ID);
                }

                this.OnEnter();
            }
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
