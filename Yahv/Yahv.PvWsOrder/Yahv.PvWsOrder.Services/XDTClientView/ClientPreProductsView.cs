using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels.Client;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 客户的预归类产品
    /// 不包含归类信息
    /// </summary>
    public class ClientPreProductsView : UniqueView<PreProduct, ScCustomReponsitory>
    {
        IUser User;

        public ClientPreProductsView(IUser user)
        {
            this.User = user;
        }
        protected override IQueryable<PreProduct> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PreProducts>()
                   join clients in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>() on product.ClientID equals clients.ID
                   where product.ClientID == this.User.XDTClientID && product.Status == (int)GeneralStatus.Normal
                   select new PreProduct
                   {
                       ID = product.ID,
                       ClientID = product.ClientID,
                       ProductUnionCode = product.ProductUnionCode,
                       Model = product.Model,
                       Manufacturer = product.Manufacturer,
                       BatchNo = product.BatchNo,
                       Price = product.Price,
                       Currency = product.Currency,
                       Supplier = product.Supplier,
                       Status = (int)product.Status,
                       CreateDate = product.CreateDate,
                       UpdateDate = product.UpdateDate
                   };
        }

        /// <summary>
        /// 根据物料号查询
        /// </summary>
        /// <param name="unionCode"></param>
        /// <returns></returns>
        public PreProduct FindByUnionCode(string unionCode)
        {
            return this.GetIQueryable().Where(s => s.ProductUnionCode == unionCode).FirstOrDefault();
        }

        /// <summary>
        /// Excel批量导入
        /// </summary>
        /// <param name="preProducts"></param>
        public void ExcelEnter(params PreProduct[] preProducts)
        {
            #region  预归类产品操作   
            //更新的产品
            var updatePrepros = (from entity in preProducts
                                 join pros in this.IQueryable on entity.ProductUnionCode equals pros.ProductUnionCode
                                 select new PreProduct
                                 {
                                     ID = pros.ID,
                                     ClientID = entity.ClientID,
                                     ProductName= entity.ProductName,
                                     ProductUnionCode = entity.ProductUnionCode,
                                     Model = entity.Model,
                                     Manufacturer = entity.Manufacturer,
                                     Price = entity.Price,
                                     Qty = entity.Qty,
                                     Currency = entity.Currency,
                                     DueDate = entity.DueDate,
                                     CompanyType = entity.CompanyType,
                                 }).ToArray();
            //重新定义一个list存放产品集合
            List<PreProduct> newPre = new List<PreProduct>();
            //新增的产品
            var insertPrepros = preProducts.Where(item => !updatePrepros.Select(a => a.ProductUnionCode).Contains(item.ProductUnionCode)).ToArray();
            //批量新增
            insertPrepros.BatchInsert(this.Reponsitory);
            newPre = insertPrepros.ToList();
            foreach (var item in updatePrepros)
            {
                item.Update(this.Reponsitory);
                newPre.Add(item);
            }
            #endregion

            #region  归类信息操作
            var linq = (from entity in newPre
                        join category in Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PreProductCategories>() on entity.ID equals category.PreProductID
                        where category.Status == (int)GeneralStatus.Normal
                        select new PreProduct
                        {
                            ID = entity.ID,
                            ClientID = entity.ClientID,
                            ProductUnionCode = entity.ProductUnionCode,
                            Model = entity.Model,
                            Manufacturer = entity.Manufacturer,
                            Price = entity.Price,
                            Qty = entity.Qty,
                            Currency = entity.Currency,
                            DueDate = entity.DueDate,
                            CompanyType = entity.CompanyType,
                            ClassifyStatus = (ClassifyStatus?)category.ClassifyStatus,
                        }).ToArray();
            //修改集合
            var updateCate = linq.Where(c => c.ClassifyStatus == ClassifyStatus.Unclassified).ToArray();
            //新增集合
            var insertCate = newPre.Where(item=>!linq.Select(c=>c.ID).Contains(item.ID));
            //批量新增
            insertCate.ToArray().BatchInsertCategories(this.Reponsitory);
            //重新赋值
            newPre = insertCate.ToList();
            //修改
            foreach (var item in updateCate)
            {
                item.UpdateCategories(this.Reponsitory);
                newPre.Add(item);
            }
            #endregion

            #region 自动归类
            Task task = new Task(() =>
            {
                this.AutoClassfiy(newPre.Select(item => item.PreProductCategoryID).ToList());
            });
            task.Start();
            #endregion
        }

        /// <summary>
        /// 保存预归类产品
        /// </summary>
        /// <param name="preProduct"></param>
        public void Enter(PreProduct preProduct)
        {
            #region 保存预归类产品
            var pre = this.Where(item => item.ProductUnionCode == preProduct.ProductUnionCode).FirstOrDefault();
            //保存预归类产品
            if (pre == null)
            {
                preProduct.Insert(Reponsitory);
            }
            else
            {
                preProduct.ID = pre.ID;
                preProduct.Update(Reponsitory);
            }
            #endregion

            #region 保存归类信息
            var category = Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PreProductCategories>().Where(item => item.PreProductID == preProduct.ID).FirstOrDefault();
            if (category == null)
            {
                preProduct.InsertCategories(this.Reponsitory);
            }
            else if (category.ClassifyStatus == (int)ClassifyStatus.Unclassified) //未归类修改
            {
                preProduct.PreProductCategoryID = category.ID;
                preProduct.UpdateCategories(this.Reponsitory);
            }
            //自动归类
            if (!string.IsNullOrWhiteSpace(preProduct.PreProductCategoryID))
            {
                Task task = new Task(() =>
                {
                    this.AutoClassfiy(new List<string>() { preProduct.PreProductCategoryID });
                });
                task.Start();
            }
            #endregion
        }

        /// <summary>
        /// 自动归类
        /// </summary>
        /// <param name="list">categoryID数组</param>
        private void AutoClassfiy(List<string> categoryIDs)
        {
            //调用归类接口
            var apisetting = new WlAdminApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.AutoPreClassify;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                MainIDs = categoryIDs,
            });
        }
    }
}
