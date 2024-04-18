using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Enums
{
    public enum PkeyType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("M", PKeySigner.Mode.Normal, 6)]
        Menus,

        /// <summary>
        /// 通知
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("NT", PKeySigner.Mode.Date, 6)]
        Notices,

        /// <summary>
        /// 租赁通知 
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey(nameof(LsNotice), PKeySigner.Mode.Date, 4)]
        LsNotice,

        /// <summary>
        /// 进项单
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("IptWH", PKeySigner.Mode.Date, 6)]
        Inputs,

        /// <summary>
        /// 销项单
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("OptWH", PKeySigner.Mode.Date, 6)]
        Outputs,

        /// <summary>
        /// 分拣单
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("", PKeySigner.Mode.Date, 6)]
        Sortings,

        /// <summary>
        /// 拣货单
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("", PKeySigner.Mode.Date, 6)]
        Pickings,

        /// <summary>
        /// 库存
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("STOR", PKeySigner.Mode.Date, 6)]
        Storages,

        /// <summary>
        /// 库存form
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("STORF", PKeySigner.Mode.Date, 6)]
        StoragesForm,

        /// <summary>
        /// 箱子
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("", PKeySigner.Mode.Normal, 3)]
        Boxes,

        /// <summary>
        /// 箱子
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("", PKeySigner.Mode.Normal, 3)]
        NewBoxes,

        /// <summary>
        /// 申报日志
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("LD", PKeySigner.Mode.Date, 4)]
        LogsDeclare,

        /// <summary>
        /// 申报项目日志
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("LDI", PKeySigner.Mode.Date, 6)]
        LogsDeclareItem,

        /// <summary>
        /// 中心运单
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Waybill", PKeySigner.Mode.Date, 4)]
        Waybills,

        /// <summary>
        /// 收款人
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("Payee", PKeySigner.Mode.Normal, 4)]
        Payee,
    }
}
