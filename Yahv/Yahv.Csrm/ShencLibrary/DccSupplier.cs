using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace ShencLibrary
{
    public class SyncSupplier
    {
        /// <summary>
        /// 中文简称
        /// </summary>
        public string CHNabbreviation { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string Chinesename { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string Corporation { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade Grade { get; set; }

        public string Summary { get; set; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { get; set; }
        /// <summary>
        /// 企业注册地址
        /// </summary>

        public string RegAddress { get; set; }

        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Place { get; set; }
    }
    public class DccSupplier
    {
        public DccSupplier()
        {

        }

        public string Enter(string clientid, SyncSupplier entity)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                var supplier = client.nSuppliers.SingleOrDefault(item => item.RealEnterprise.Name == entity.EnglishName);

                if (supplier == null)
                {
                    var gradeInfo = new SupplierGradeByEngNameView(entity.EnglishName).GetGrade();
                    if (gradeInfo != null)
                    {
                        entity.Grade = (SupplierGrade)gradeInfo.SupplierGrade;
                    }
                }

                nSupplier data;
                data = new nSupplier
                {
                    ID = supplier?.ID,
                    EnterpriseID = clientid,

                    RealEnterprise = new Enterprise
                    {
                        Name = entity.EnglishName,
                        AdminCode = string.IsNullOrWhiteSpace(supplier?.Enterprise?.AdminCode) ? "" : supplier?.Enterprise?.AdminCode,
                        Corporation = entity.Corporation,
                        RegAddress = entity.RegAddress,
                        Uscc = entity.Uscc,
                        Place = entity.Place
                    },
                    CHNabbreviation = entity.CHNabbreviation,
                    ChineseName = entity.Chinesename,
                    EnglishName = entity.EnglishName,
                    Grade = entity.Grade,
                    Creator = "",
                    Summary = entity.Summary
                };

                data.Enter();

                this.Synchro(clientid, entity);//向芯达通同步

                return data.ID;
            }
        }
        public void Abandon(string clientid, string supplierid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var entity = client.nSuppliers[supplierid];
                entity.Abandon();
                this.AbandonSynchro(client.Enterprise.Name, entity.Enterprise.Name);//向芯达通同步
            }
        }
        private void Synchro(string clientid, SyncSupplier entity)
        {
            string url = Commons.UnifyApiUrl + "/clients/suppliers";
            Enterprise client = new EnterprisesRoll()[clientid];
            Commons.HttpPostRaw(url, new
            {
                Enterprise = client,
                EnglishName = entity.EnglishName,
                ChineseName = entity.Chinesename,
                Summary = entity.Summary,
                Place = entity.Place,
                Grade = (int)entity.Grade
            }.Json());
        }
        private void AbandonSynchro(string clientname, string suppliername)
        {
            string url = Commons.UnifyApiUrl + "/clients/suppliers";
            Commons.CommonHttpRequest(url + "?name=" + clientname + "&supplierName=" + suppliername, "DELETE");
        }

    }

    public class SupplierGradeByEngNameView
    {
        private string _EngName = string.Empty;

        public SupplierGradeByEngNameView(string engName)
        {
            this._EngName = engName;
        }

        public GradeInfoModel GetGrade()
        {
            GradeInfoModel result = null;

            using (Layers.Data.Sqls.PvbCrmReponsitory reponsitory = new Layers.Data.Sqls.PvbCrmReponsitory())
            {
                var nSuppliers = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nSuppliers>();

                result = (from nSupplier in nSuppliers
                          where nSupplier.EnglishName == this._EngName
                             && nSupplier.Status == (int)GeneralStatus.Normal
                          orderby nSupplier.UpdateDate descending
                          select new GradeInfoModel
                          {
                              SupplierEngName = nSupplier.EnglishName,
                              SupplierGrade = (SupplierGrade)nSupplier.Grade,
                          }).FirstOrDefault();
            }

            return result;
        }

        public class GradeInfoModel
        {
            public string SupplierEngName { get; set; }

            public SupplierGrade SupplierGrade { get; set; }
        }
    }

}
