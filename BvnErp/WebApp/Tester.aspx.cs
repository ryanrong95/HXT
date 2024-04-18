using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Web.Forms;
using Needs.Overall;


namespace WebApp
{
    public partial class Tester : Needs.Web.Forms.ClientPage
    {

        class MyClass
        {
            public MyClass()
            {
                //using (var view = Needs.Erp.ErpPlot.Current.Plots.MyStaffs)
                //{
                //    view.Bind("");
                //    //view.Bind("");
                //}

                //Needs.Erp.ErpPlot.Current.Plots.MyStaffs.Bind("");
                //Needs.Erp.ErpPlot.Current.Limits.Admins["asd"].Plots.MyStaffs.UnBind("");

                //Devlopers.Create(Devloper.Chenhan);
                //Devlopers.Create(Devloper.XiaoYa);
            }
        }

        public Tester()
        {
         
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object Tester1(int i)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public T Tester1<T>(object o)
        {
            return default(T);
        }

    }
}