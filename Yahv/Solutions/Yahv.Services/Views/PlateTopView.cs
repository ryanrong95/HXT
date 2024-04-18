using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 库房视图
    /// </summary>
    public class PlateTopView<TReponsitory> : UniqueView<Plate, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PlateTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PlateTopView(TReponsitory reponsitory) : base(reponsitory)
        {
            //this.Where(item=>item.ConsinStatusDes)
        }
        protected override IQueryable<Plate> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<PlateTopView>()
                   select new Plate
                   {
                       ID = entity.ID,
                       Name = entity.Title,
                       Code = entity.Code,
                       PostZip = entity.Postzip,

                       //Region = (Region)entity.District,
                       Address = entity.Address,
                       //DyjCode = entity.,
                       //Grade = (WarehouseGrade)entity.Grade,
                       //Corporation = entity.Corporation,
                       //Uscc = entity.Uscc,
                       //RegAddress = entity.RegAddress,
                       //WareHouseStatus = (ApprovalStatus)entity.Status,
                       //ConsinStatus = (ApprovalStatus)entity.ConsinStatus,
                       EnterpriseID = entity.EnterpriseID,
                       //Name = entity.Title,
                   };
        }


    }
}
