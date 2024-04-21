using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Payments.Models
{
    /// <summary>
    /// 客户供应商
    /// </summary>
    /// <remarks>
    /// 其实应该收取的是客户自己的承运商
    /// 如果是快递公司送货，也要收取费用应该是获取实际的快递公司记录费用
    /// 由于开发时间有限，暂时固定一个供应商（承运商）
    /// </remarks>
    public class WsFixedCarrier
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public WsFixedCarrier()
        {
            this.Name = "客户供应商";
            string id = this.ID = this.Name.MD5();

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                        .Any(item => item.ID == id))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises()
                    {
                        ID = id,
                        Name = Name,
                        Status = (int)ApprovalStatus.Normal,
                        AdminCode = "",
                    });
                }

                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>()
                    .Any(item => item.ID == id))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.WsSuppliers()
                    {
                        ID = id,
                        ChineseName = Name,
                        Grade = 9,
                        AdminID = Underly.Npc.Robot.Obtain(),
                        Status = (int)ApprovalStatus.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
            }

        }



        static object locker = new object();
        static WsFixedCarrier current;
        static public WsFixedCarrier Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new WsFixedCarrier();
                        }
                    }
                }

                return current;
            }
        }
    }
}
