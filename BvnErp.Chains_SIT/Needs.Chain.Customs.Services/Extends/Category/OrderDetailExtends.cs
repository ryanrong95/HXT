using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public static class OrderDetailExtends
    {
        /// <summary>
        /// 获取特殊类型
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <returns></returns>
        public static string GetSpecialType(this Views.OrderDetailOrderItemsModel classifyProduct)
        {
            if (classifyProduct.CategoryType == Enums.ItemCategoryType.Normal)
            {
                return "--";
            }
            else
            {
                StringBuilder specialType = new StringBuilder();
                foreach (Enums.ItemCategoryType type in Enum.GetValues(typeof(Enums.ItemCategoryType)))
                {
                    //20191113 与魏晓毅确认，归类预处理二列表界面特殊类型只显示归类界面上有的类型
                    if (type == Enums.ItemCategoryType.Quarantine)
                        continue;

                    if ((classifyProduct.CategoryType & type) > 0)
                    {
                        specialType.Append(type.GetDescription() + "|");
                    }
                }
                if (specialType.Length > 0)
                    return specialType.ToString().TrimEnd('|');
                else
                    return "--";
            }
        }
    }
}
