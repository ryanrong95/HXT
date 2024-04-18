using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 币种
    /// </summary>
    public class Currency : IUnique
    {
        private string id;

        public string ID
        {
            get
            {
                return this.id ?? this.Code;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCurrencies>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.BaseCurrencies
                    {
                        ID = this.ID,
                        Code = this.Code,
                        EnglishName = this.EnglishName,
                        Name = this.Name
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.BaseCurrencies
                    {
                        Code = this.Code,
                        EnglishName = this.EnglishName,
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
