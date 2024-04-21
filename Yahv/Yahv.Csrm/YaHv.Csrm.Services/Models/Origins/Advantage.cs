using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class Advantage : Yahv.Linq.IUnique
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.Enterprise.ID;

            }
            set
            {
                this.id = value;
            }
        }
        public string Manufacturers { set; get; }
        public string PartNumbers { set; get; }
        public Enterprise Enterprise { set; get; }

        //IEnumerable<ViewManufacturer> maf;
        //public IEnumerable<ViewManufacturer> manufacturers
        //{
        //    get
        //    {
        //        if (this.manufacturers != null && this.maf == null)
        //        {
        //            this.maf = JsonSerializerExtend.JsonTo<Newtonsoft.Json.Linq.JArray>(Manufacturers).ToObject<List<ViewManufacturer>>();
        //        };
        //        return this.maf;
        //    }

        //}
        //IEnumerable<PartNumber> pn;
        //public IEnumerable<PartNumber> partNumbers
        //{
        //    get
        //    {
        //        List<PartNumber> list = new List<PartNumber>();
        //        if (this.partNumbers != null && this.pn == null)
        //        {
        //            this.pn = JsonSerializerExtend.JsonTo<Newtonsoft.Json.Linq.JArray>(Manufacturers).ToObject<List<PartNumber>>();
        //        };
        //        return list.ToArray();
        //    }
        //}
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Advantages>().Any(item => item.EnterpriseID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Advantages>(new
                    {
                        Manufacturers = this.Manufacturers,
                        PartNumbers = this.Manufacturers
                    }, item => item.EnterpriseID == this.ID);
                }
                else
                {
                    repository.Insert<Layers.Data.Sqls.PvbCrm.Advantages>(new Layers.Data.Sqls.PvbCrm.Advantages
                    {
                        EnterpriseID = this.ID,
                        Manufacturers = this.Manufacturers,
                        PartNumbers = this.PartNumbers
                    });
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion


    }

}
