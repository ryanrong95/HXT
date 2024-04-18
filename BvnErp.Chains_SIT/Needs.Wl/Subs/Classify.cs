using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Classify Classify
        {
            get
            {
                return new Classify(this);
            }
        }
    }

    public class Classify
    {
        IGenericAdmin Admin;

        public Classify(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 归类产品
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.ClassifyProductsAll ClassifyProductsAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.ClassifyProductsAll();
            }
        }

        /// <summary>
        /// 预归类产品
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.PreClassifyProductsAll PreClassifyProductsAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.PreClassifyProductsAll();
            }
        }

        /// <summary>
        /// 归类产品
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.PD_ClassifyProductsAll PD_ClassifyProductsAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.PD_ClassifyProductsAll();
            }
        }

        /// <summary>
        /// 预归类产品
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsAll PD_PreClassifyProductsAll
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsAll();
            }
        }

        /// <summary>
        /// 用于产品预归类/咨询归类预处理一列表
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsStep1 PD_PreClassifyProductsStep1
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsStep1();
            }
        }

        /// <summary>
        /// 用于产品预归类/咨询归类预处理二列表
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsStep2 PD_PreClassifyProductsStep2
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsStep2();
            }
        }

        /// <summary>
        /// 用于产品预归类/咨询归类已完成列表
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsDone PD_PreClassifyProductsDone
        {
            get
            {
                return new Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsDone();
            }
        }

        /// <summary>
        /// 自动归类记录
        /// </summary>
        public Needs.Ccs.Services.Views.ProductCategoriesDefaultsView ProductCategoriesDefaults
        {
            get
            {
                return new Needs.Ccs.Services.Views.ProductCategoriesDefaultsView();
            }
        }
    }
}
