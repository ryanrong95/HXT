using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Finance.Services.Models.Origins
{
    public class MapsPurpose : IUnique
    {
        public string ID { get; set; }

        #region 数据库属性

        /// <summary>
        /// AccountID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// AccountPurposeID
        /// </summary>
        public string AccountPurposeID { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MapsPurpose>()
                    .Any(item => item.AccountID == this.AccountID && item.AccountPurposeID == this.AccountPurposeID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.MapsPurpose()
                    {
                        AccountID = this.AccountID,
                        AccountPurposeID = this.AccountPurposeID,
                    });
                }
            }
        }

        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.MapsPurpose>(
                    item => item.AccountID == this.AccountID && item.AccountPurposeID == this.AccountPurposeID);
            }
        }

        #endregion

    }
}
