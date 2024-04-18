using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.HistoryImport
{
    public partial class frmRePost : Form
    {
        public frmRePost()
        {
            InitializeComponent();
            formLoad();
        }

        private void formLoad()
        {
            this.dtPicker.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            string Model = this.txtModel.Text;
            string ProductID = this.txtProductID.Text;

            int rePostCount = 0;

            if (!string.IsNullOrEmpty(Model))
            {
                var items = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.Model == Model);               
                foreach (var item in items)
                {
                    if (item.ClassifyStatus == ClassifyStatus.Done)
                    {
                        //var postLog = new Needs.Ccs.Services.Views.PreProductPostLogView().Where(t => t.id == item.ID);
                        //if (postLog.Count() == 0)
                        //{
                        IcgooPost icgooPost = new IcgooPost(item.PreProduct.CompanyType);
                        icgooPost.id = item.ID;
                        icgooPost.sale_orderline_id = item.PreProduct.sale_orderline_id;
                        icgooPost.partno = item.Model;
                        icgooPost.supplier = item.PreProduct.supplier;
                        icgooPost.mfr = item.Manufacturer;
                        icgooPost.brand = item.Manufacturer;
                        icgooPost.origin = "";
                        icgooPost.customs_rate = item.TariffRate;
                        icgooPost.add_rate = item.AddedValueRate;
                        icgooPost.product_name = item.ProductName;
                        icgooPost.category = "";                       
                        icgooPost.hs_code = item.HSCode;
                        icgooPost.tax_code = item.TaxCode;
                       
                        //Type 转换
                        if ((item.Type & ItemCategoryType.Normal) >= 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Normal;
                        }

                        if ((item.Type & ItemCategoryType.Inspection) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Inspection;
                        }

                        if ((item.Type & ItemCategoryType.CCC) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.CCC;
                        }

                        if ((item.Type & ItemCategoryType.Forbid) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Embargo;
                        }

                        icgooPost.PostData();
                        rePostCount++;
                        //}
                    }
                }
            }

            if (!string.IsNullOrEmpty(ProductID))
            {
                var items = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.PreProduct.sale_orderline_id == ProductID);
                foreach (var item in items)
                {
                    if (item.ClassifyStatus == ClassifyStatus.Done)
                    {
                        //var postLog = new Needs.Ccs.Services.Views.PreProductPostLogView().Where(t => t.id == item.ID);
                        //if (postLog.Count() == 0)
                        //{
                        IcgooPost icgooPost = new IcgooPost(item.PreProduct.CompanyType);
                        icgooPost.id = item.ID;
                        icgooPost.sale_orderline_id = item.PreProduct.sale_orderline_id;
                        icgooPost.partno = item.Model;
                        icgooPost.supplier = item.PreProduct.supplier;
                        icgooPost.mfr = item.Manufacturer;
                        icgooPost.brand = item.Manufacturer;
                        icgooPost.origin = "";
                        icgooPost.customs_rate = item.TariffRate;
                        icgooPost.add_rate = item.AddedValueRate;
                        icgooPost.product_name = item.ProductName;
                        icgooPost.category = "";                       
                        icgooPost.hs_code = item.HSCode;
                        icgooPost.tax_code = item.TaxCode;

                        
                        //Type 转换
                        if ((item.Type & ItemCategoryType.Normal) >= 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Normal;
                        }

                        if ((item.Type & ItemCategoryType.OriginProof) >= 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Normal;
                        }

                        if ((item.Type & ItemCategoryType.Inspection) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Inspection;
                        }

                        if ((item.Type & ItemCategoryType.CCC) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.CCC;
                        }

                        if ((item.Type & ItemCategoryType.HighValue) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.HighValue;
                        }

                        if ((item.Type & ItemCategoryType.HKForbid) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.HKLimit;
                        }

                        if ((item.Type & ItemCategoryType.Forbid) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Embargo;
                        }

                        icgooPost.PostData();
                        rePostCount++;
                        //}
                    }
                }
            }

                    
        

            if (rePostCount < 1)
            {
                MessageBox.Show("未提交任何记录!");
            }
            else
            {
                MessageBox.Show("已提交 "+rePostCount+" 条记录");
            }
        }

        private void btnAllPost_Click(object sender, EventArgs e)
        {
            DateTime dtStart = Convert.ToDateTime(this.dtPicker.Text+" 00:00:00");
            DateTime dtEnd = Convert.ToDateTime(this.dtPicker.Text + "23:59:59");

            var classifiedIds = new Needs.Ccs.Services.Views.PendingClassifyView().
                                    Where(item => item.ClassifyStatus == Needs.Ccs.Services.Enums.ClassifyStatus.Done&&
                                          item.UpdateDate>=dtStart&&item.UpdateDate<=dtEnd).
                                    Where(t => t.PreProduct.CompanyType == CompanyTypeEnums.FastBuy || t.PreProduct.CompanyType == CompanyTypeEnums.Icgoo).Select(item=>item.ID).ToList();

            var PostIds = new Needs.Ccs.Services.Views.PreProductPostLogView().Where(item => classifiedIds.Contains(item.id)).Select(item => item.id).ToList();

            List<string> PendingRePostIds = classifiedIds.Except(PostIds).ToList();

            int rePostCount = 0;
            foreach (var id in PendingRePostIds)
            {
                var item = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.ID == id).FirstOrDefault();
                if (item != null)
                {
                    IcgooPost icgooPost = new IcgooPost(item.PreProduct.CompanyType);
                    icgooPost.id = item.ID;
                    icgooPost.sale_orderline_id = item.PreProduct.sale_orderline_id;
                    icgooPost.partno = item.Model;
                    icgooPost.supplier = item.PreProduct.supplier;
                    icgooPost.mfr = item.Manufacturer;
                    icgooPost.brand = item.Manufacturer;
                    icgooPost.origin = "";
                    icgooPost.customs_rate = item.TariffRate;
                    icgooPost.add_rate = item.AddedValueRate;
                    icgooPost.product_name = item.ProductName;
                    icgooPost.category = "";
                    icgooPost.type = (int)item.Type;
                    icgooPost.hs_code = item.HSCode;
                    icgooPost.tax_code = item.TaxCode;

                    icgooPost.PostData();
                    rePostCount++;
                }
            }

            if (rePostCount < 1)
            {
                MessageBox.Show("未提交任何记录!");
            }
            else
            {
                MessageBox.Show("已提交 " + rePostCount + " 条记录");
            }
        }

        private void btnInsidePost_Click(object sender, EventArgs e)
        {
            DateTime dtStart = Convert.ToDateTime(this.dtPicker.Text + " 00:00:00");
            DateTime dtEnd = Convert.ToDateTime(this.dtPicker.Text + "23:59:59");

            var classifiedIds = new Needs.Ccs.Services.Views.PendingClassifyView().
                                    Where(item => item.ClassifyStatus == Needs.Ccs.Services.Enums.ClassifyStatus.Done && 
                                          item.PreProduct.CompanyType == CompanyTypeEnums.Inside &&
                                          item.UpdateDate >= dtStart && item.UpdateDate <= dtEnd).Select(item => item.PreProduct.ID).ToList();

            var PostIds = new Needs.Ccs.Services.Views.PreProductPostLogView().Where(item => classifiedIds.Contains(item.id)&&item.status==true).Select(item => item.id).ToList();

            List<string> PendingRePostIds = classifiedIds.Except(PostIds).ToList();

            foreach (var id in PendingRePostIds)
            {
                var item = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.ID == id).FirstOrDefault();
                if (item != null)
                {
                    item.InsidePostUrl = System.Configuration.ConfigurationManager.AppSettings["InsidePostUrl"];
                    item.InsideElementsPostUrl = System.Configuration.ConfigurationManager.AppSettings["InsideElementsPostUrl"];
                    item.InsideKey = System.Configuration.ConfigurationManager.AppSettings["InsidePostKey"];
                    item.CompanyType = CompanyTypeEnums.Inside;
                    item.OnSecondClassified(new IcgooClassifyEventArgs(item));
                }
            }
        }

        private void btnInsideSinglePost_Click(object sender, EventArgs e)
        {
            string Model = this.txtModel.Text;
            string ProductID = this.txtProductID.Text;

            if (!string.IsNullOrEmpty(Model))
            {
                var item = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.Model == Model).SingleOrDefault();
                item.InsidePostUrl = System.Configuration.ConfigurationManager.AppSettings["InsidePostUrl"];
                item.InsideElementsPostUrl = System.Configuration.ConfigurationManager.AppSettings["InsideElementsPostUrl"];
                item.InsideKey = System.Configuration.ConfigurationManager.AppSettings["InsidePostKey"];
                item.CompanyType = CompanyTypeEnums.Inside;
                item.OnSecondClassified(new IcgooClassifyEventArgs(item));
            }

            if (!string.IsNullOrEmpty(ProductID))
            {
                var item = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.PreProduct.sale_orderline_id== ProductID).SingleOrDefault();
                item.InsidePostUrl = System.Configuration.ConfigurationManager.AppSettings["InsidePostUrl"];
                item.InsideElementsPostUrl = System.Configuration.ConfigurationManager.AppSettings["InsideElementsPostUrl"];
                item.InsideKey = System.Configuration.ConfigurationManager.AppSettings["InsidePostKey"];
                item.CompanyType = CompanyTypeEnums.Inside;
                item.OnSecondClassified(new IcgooClassifyEventArgs(item));
            }

           
        }

        private List<string> IDList()
        {
            List<string> IDS = new List<string>();
         
            return IDS;
        }

        private void PushByID(List<string> ids)
        {
            int rePostCount = 0;
            foreach (var id in ids)
            {
                var items = new Needs.Ccs.Services.Views.PendingClassifyView().Where(t => t.PreProduct.sale_orderline_id == id);
                foreach (var item in items)
                {
                    if (item.ClassifyStatus == ClassifyStatus.Done)
                    {
                        //var postLog = new Needs.Ccs.Services.Views.PreProductPostLogView().Where(t => t.id == item.ID);
                        //if (postLog.Count() == 0)
                        //{
                        IcgooPost icgooPost = new IcgooPost(item.PreProduct.CompanyType);
                        icgooPost.id = item.ID;
                        icgooPost.sale_orderline_id = item.PreProduct.sale_orderline_id;
                        icgooPost.partno = item.Model;
                        icgooPost.supplier = item.PreProduct.supplier;
                        icgooPost.mfr = item.Manufacturer;
                        icgooPost.brand = item.Manufacturer;
                        icgooPost.origin = "";
                        icgooPost.customs_rate = item.TariffRate;
                        icgooPost.add_rate = item.AddedValueRate;
                        icgooPost.product_name = item.ProductName;
                        icgooPost.category = "";
                        icgooPost.hs_code = item.HSCode;
                        icgooPost.tax_code = item.TaxCode;

                        //Type 转换
                        if ((item.Type & ItemCategoryType.Normal) >= 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Normal;
                        }

                        if ((item.Type & ItemCategoryType.Inspection) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Inspection;
                        }

                        if ((item.Type & ItemCategoryType.CCC) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.CCC;
                        }

                        if ((item.Type & ItemCategoryType.Forbid) > 0)
                        {
                            icgooPost.classifyType = (int)IcgooClassifyTypeEnums.Embargo;
                        }

                        icgooPost.PostData();
                        rePostCount++;
                        //}
                    }
                }
            }

            if (rePostCount < 1)
            {
                MessageBox.Show("未提交任何记录!");
            }
            else
            {
                MessageBox.Show("已提交 " + rePostCount + " 条记录");
            }
        }

        private void btnIcgooIDS_Click(object sender, EventArgs e)
        {
            PushByID(IDList());
        }

      
    }
}
