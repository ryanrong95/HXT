using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class MapsAccountType : IUnique
    {
        public string ID { get; set; }

        #region 数据库属性

        /// <summary>
        /// AccountID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// AccountTypeID
        /// </summary>
        public string AccountTypeID { get; set; }

        #endregion

        #region 拓展属性
        /// <summary>
        /// 类型名称
        /// </summary>
        public string AccountTypeName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MapsAccountType>()
                    .Any(item => item.AccountID == this.AccountID && item.AccountTypeID == this.AccountTypeID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.MapsAccountType()
                    {
                        AccountID = this.AccountID,
                        AccountTypeID = this.AccountTypeID,
                    });
                }
            }
        }

        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.MapsAccountType>(
                    item => item.AccountID == this.AccountID && item.AccountTypeID == this.AccountTypeID);
            }
        }

        #endregion

    }
}
