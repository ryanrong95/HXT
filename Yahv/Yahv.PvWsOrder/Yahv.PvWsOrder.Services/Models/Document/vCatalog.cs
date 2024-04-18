using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 分类
    /// </summary>
    public class vCatalog : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public string FatherID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.vCatalogs>().Any(item => item.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Yahv.Underly.PKeyType.vCatalogs);

                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.vCatalogs()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        FatherID = this.FatherID,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.vCatalogs>(new
                    {
                        Name = this.Name,
                        FatherID = this.FatherID,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}