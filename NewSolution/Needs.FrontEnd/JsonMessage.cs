using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.FrontEnd
{
    public class JsonMessage
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public object Target { get; set; }
    }

    public class JsonMessage<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        public string Description
        {
            get
            {
                return this.Status.ToString();
            }
        }

        T status;

        public T Status
        {
            get
            {
                return this.status;
            }
            set
            {

                this.status = value;
                this.Message = ((Enum)Enum.Parse(typeof(T), this.Description)).GetDescription();
            }
        }
        public string Message { get; set; }

        public JsonMessage()
        {
        }
    }
}
