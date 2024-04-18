using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ProductControlAllsView : UniqueView<Models.ProductControl, ScCustomsReponsitory>
    {
        public ProductControlAllsView()
        {
        }

        internal ProductControlAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductControl> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductControls>()
                   select new Models.ProductControl
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Manufacturer = entity.Manufacturer,
                       Model = entity.Model,
                       Type = (Enums.ProductControlType)entity.Type,
                       CreateDate = entity.CreateDate
                   };
        }
    }

    /// <summary>
    /// 产品管控
    /// </summary>
    public sealed class ProductControlsView : ProductControlAllsView
    {
        string Model = string.Empty;
        string Name = string.Empty;

        bool IsModelLike = false;

        /// <summary>
        /// 产品管控
        /// </summary>
        /// <param name="model">型号</param>
        public ProductControlsView(string model)
        {
            this.Model = model;
        }

        public ProductControlsView(string model, string name)
        {
            this.Model = model;
            this.Name = name;
        }

        public ProductControlsView(string model, bool isModelLike)
        {
            this.Model = model;
            this.IsModelLike = isModelLike;
        }

        protected override IQueryable<ProductControl> GetIQueryable()
        {
            List<LambdaExpression> expressions = new List<LambdaExpression>();

            //是否需要 this.Name 判断条件
            if (string.IsNullOrEmpty(this.Name) == false)
            {
                expressions.Add((Expression<Func<ProductControl, bool>>)(item => item.Name == this.Name));
            }

            //this.Model 判断条件是否使用 Like
            if (this.IsModelLike)
            {
                expressions.Add((Expression<Func<ProductControl, bool>>)(item => item.Model.Contains(this.Model)));
            }
            else
            {
                expressions.Add((Expression<Func<ProductControl, bool>>)(item => item.Model == this.Model));
            }

            //给 view 添加条件
            var view = base.GetIQueryable();
            foreach (var expression in expressions)
            {
                view = view.Where(expression as Expression<Func<ProductControl, bool>>);
            }

            return view;
        }
    }
}
