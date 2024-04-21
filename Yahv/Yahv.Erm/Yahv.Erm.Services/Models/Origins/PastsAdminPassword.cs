using System;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 密码修改历史表
    /// </summary>
    public class PastsAdminPassword : IUnique
    {
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 添加事件
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new PvbErmReponsitory())
            {
                reponsitory.Insert<Layers.Data.Sqls.PvbErm.Pasts_AdminPassword>(new PastsAdminPassword()
                {
                    ID = PKeySigner.Pick(PKeyType.PastsAdminPassword),
                    CreateDate = DateTime.Now,
                    AdminID = this.AdminID,
                    Password = this.Password.MD5("x").PasswordOld(),
                });
            }
        }
        #endregion
    }
}