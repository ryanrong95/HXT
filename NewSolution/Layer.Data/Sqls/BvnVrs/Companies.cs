using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls.BvnVrs
{
    public partial class Companies
    {
        List<string> changeds = new List<string>();


        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += Companies_PropertyChanged;
                    }
                }
            }
        }

        private void Companies_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }
    }

    public partial class Invoices
    {
        List<string> changeds = new List<string>();


        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += Invoices_PropertyChanged;
                    }
                }
            }
        }

        private void Invoices_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }
    }

    public partial class Contacts
    {
        List<string> changeds = new List<string>();


        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += Contacts_PropertyChanged;
                    }
                }
            }
        }

        private void Contacts_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }
    }

    public partial class Beneficiaries
    {
        List<string> changeds = new List<string>();


        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += Beneficiaries_PropertyChanged;
                    }
                }
            }
        }

        private void Beneficiaries_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }
    }

    public partial class MapsAdminCompany
    {
        List<string> changeds = new List<string>();


        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += MapsAdminCompany_PropertyChanged;
                    }
                }
            }
        }

        private void MapsAdminCompany_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }
    }

    public partial class MapsAdminContact
    {
        List<string> changeds = new List<string>();


        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += MapsAdminContact_PropertyChanged;
                    }
                }
            }
        }

        private void MapsAdminContact_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }
    }
}
