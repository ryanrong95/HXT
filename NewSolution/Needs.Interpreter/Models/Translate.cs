using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Interpreter.Extends;

namespace Needs.Interpreter.Models
{
    public interface ITranslate : IUnique, IPersistence, IFulSuccess
    {
        int Type { get; set; }
        string Name { get; set; }
        string Language { get; set; }
        string Value { get; set; }
    }

    public class Translate : ITranslate
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string Value { get; set; }

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        public void Enter()
        {
            using (var repository = new Layer.Data.Sqls.BvTesterReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Abandon()
        {
            using (var repository = new Layer.Data.Sqls.BvTesterReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvTester.TopObjects>(item => item.ID == ID);
            }

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
