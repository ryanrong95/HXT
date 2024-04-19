using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models
{
    public interface IPerson
    {
        string Email { get; set; }
        string Mobile { get; set; }
        string Tel { get; set; }
        string PostZipCode { get; set; }
    }

    public class Person : Document, IPerson
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get
            {
                return this[nameof(this.Email)];
            }
            set
            {
                this[nameof(this.Email)] = value;
            }
        }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get
            {
                return this[nameof(this.Mobile)];
            }
            set
            {
                this[nameof(this.Mobile)] = value;
            }
        }

        /// <summary>
        /// 座机号
        /// </summary>
        public string Tel
        {
            get
            {
                return this[nameof(this.Mobile)];
            }
            set
            {
                this[nameof(this.Mobile)] = value;
            }
        }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZipCode
        {
            get
            {
                return this[nameof(this.PostZipCode)];
            }
            set
            {
                this[nameof(this.PostZipCode)] = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Person ? this.GetHashCode() == obj.GetHashCode() : false;
        }

        public override int GetHashCode()
        {
            var type = this.GetType();

            var properties = type.GetProperties();


            StringBuilder builder = new StringBuilder();

            foreach (var item in properties)
            {
                builder.Append(item.Name).Append('\a').Append(item.GetValue(this)).Append('\a');
            }

            return builder.ToString().GetHashCode();
        }
    }
}
