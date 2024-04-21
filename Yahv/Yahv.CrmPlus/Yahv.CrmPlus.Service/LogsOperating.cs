using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service
{
    static public class LogsOperating
    {
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="admin">当前登录人</param>
        /// <param name="mainid">主对象ID </param>
        /// <param name="context">操作内容</param>
        /// <param name="subid">辅ID</param>
        /// <example>
        /// 调用示例：
        ///  Yahv.CrmPlus.Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"修改标准型号:{ entity.ID}");
        /// </example>
        static public void LogOperating(this IErpAdmin admin, string mainid, string context, string subid = null)
        {
            try
            {
                using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.LogsOperating
                    {
                        ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Log),
                        OperatorID = admin.ID,
                        MainID = mainid,
                        Context = context,
                        SubID = subid,
                        CreateDate = DateTime.Now
                    });
                }
            }
            catch(Exception ex)
            {

            }

        }

    }

   
}
