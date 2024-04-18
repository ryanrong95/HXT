using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class PD_PreClassifyProductExtends
    {
        /// <summary>
        /// 更新预处理一操作人
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void UpdateFirstOperator(this PD_PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifyFirstOperator = classifyProduct.Admin.ID,
            }, item => item.ID == classifyProduct.ID);
        }

        /// <summary>
        /// 更新预处理二操作人
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void UpdateSecondOperator(this PD_PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifySecondOperator = classifyProduct.Admin.ID,
            }, item => item.ID == classifyProduct.ID);
        }

        /// <summary>
        /// 更新归类状态
        /// </summary>
        /// <param name="classifyProduct">归类产品</param>
        /// <param name="status">归类状态</param>
        /// <param name="reponsitory"></param>
        public static void UpdateClassifyStatus(this PD_PreClassifyProduct classifyProduct, Enums.ClassifyStatus status, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifyStatus = (int)status,
            }, item => item.ID == classifyProduct.ID);
        }

        /// <summary>
        /// 生成推送通知
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="reponsitory"></param>
        public static void ToApiNotice(this PD_PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
            {
                ID = ChainsGuid.NewGuidUp(),
                PushType = (int)Enums.PushType.ClassifyResult,
                ClientID = classifyProduct.PreProduct.ClientID,
                ItemID = classifyProduct.ID,
                PushStatus = (int)Enums.PushStatus.Unpush,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            });
        }

        /// <summary>
        /// 生成管控审批
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="type"></param>
        /// <param name="reponsitory"></param>
        public static void ToPreProductControl(this PD_PreClassifyProduct classifyProduct, Enums.ItemCategoryType type, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            string id = string.Concat(classifyProduct.PreProduct.ID, type).MD5();
            if (reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductControls>().Any(item => item.ID == id))
                return;

            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PreProductControls
            {
                ID = id,
                PreProductID = classifyProduct.PreProduct.ID,
                Type = (int)type,
                Status = (int)Enums.PreProductControlStatus.Waiting,
                CreateDate = DateTime.Now,
                ApproveDate = DateTime.Now,
            });
        }

        /// <summary>
        /// 获取特殊类型
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <returns></returns>
        public static string GetSpecialType(this PD_PreClassifyProduct classifyProduct)
        {
            if (classifyProduct.Type == null || classifyProduct.Type == Enums.ItemCategoryType.Normal)
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

                    if ((classifyProduct.Type & type) > 0)
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
