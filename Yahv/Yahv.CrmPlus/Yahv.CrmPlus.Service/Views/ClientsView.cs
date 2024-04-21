using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Data.Models;
using Yahv.CrmPlus.Service.Models;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Models
{
    public class MyClient : Linq.IEntity
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDraft { get; set; }
        public string Place { get; set; }
        public string District { get; set; }
        public string Summary { get; set; }
        public string DyjCode { get; set; }
        public ClientGrade ClientGrade { get; set; }
        public Underly.CrmPlus.ClientType ClientType { get; set; }
        public VIPLevel Vip { get; set; }
        public string Source { get; set; }
        public bool IsMajor { get; set; }
        public bool IsSpecial { get; set; }
        public string Industry { get; set; }
        public AuditStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public decimal? ProfitRate { get; set; }
        public bool IsSupplier { get; set; }
        public Admin Owner { get; internal set; }
        public EnterpriseRegister Register { get; set; }
        public MyConduct[] Conducts { get; set; }

    }

    public class MyConduct : Linq.IEntity
    {
        public string ID { get; set; }
        public ConductType Type { get; set; }
        public ConductGrade Grade { get; set; }
        public bool IsPublic { get; set; }
    }

}

namespace Yahv.CrmPlus.Service.Data.Models
{
    public class MyClientData : Linq.IDataEntity, IUnique
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDraft { get; set; }
        public string Place { get; set; }
        public string District { get; set; }
        public string Summary { get; set; }
        public string DyjCode { get; set; }
        public ClientGrade ClientGrade { get; set; }
        public Underly.CrmPlus.ClientType ClientType { get; set; }
        public VIPLevel Vip { get; set; }
        public string Source { get; set; }
        public bool IsMajor { get; set; }
        public bool IsSpecial { get; set; }
        public string Industry { get; set; }
        public AuditStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public decimal? ProfitRate { get; set; }
        public bool IsSupplier { get; set; }
        public string OwnerID { get; set; }

        public EnterpriseRegister Register { get; set; }

    }


}

namespace Yahv.CrmPlus.Service.Views
{
    public interface ISeach<Tview, TDataEntity>
    {
        Tview Searh(Expression<Func<TDataEntity, bool>> predicate);
    }

    public interface IClientsView<T>
    {
        void Enter(T t);

        void Remove(Expression<Func<T, bool>> predicate);
    }



    public class ClientsView : vDepthView<MyClientData, MyClient, PvdCrmReponsitory>
        , ISeach<ClientsView, MyClientData>
    {
        public ClientsView()
        {
        }

        public ClientsView(IQueryable<MyClientData> iQueryable) : base(iQueryable)
        {
        }

        protected ClientsView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected ClientsView(PvdCrmReponsitory reponsitory, IQueryable<MyClientData> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        public ClientsView Searh(Expression<Func<MyClientData, bool>> predicate)
        {
            var query = this.IQueryable.Where(predicate);
            return new ClientsView(this.Reponsitory, query);
        }

        protected override IQueryable<MyClientData> GetIQueryable()
        {
            var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            //以企业为主表查询，在OnMyPage 中进行数据补充 client chains
            return from client in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
                   join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on client.ID equals enterprise.ID
                   join register in registerView on client.ID equals register.ID
                   select new MyClientData
                   {
                       ID = client.ID,
                       ClientGrade = (ClientGrade)client.Grade,
                       ClientType = (Underly.CrmPlus.ClientType)client.Type,
                       Vip = (VIPLevel)client.Vip,
                       Source = client.Source,
                       IsMajor = client.IsMajor,
                       IsSpecial = client.IsSpecial,
                       Industry = client.Industry,
                       CreateDate = client.CreateDate,
                       ModifyDate = client.ModifyDate,
                       IsSupplier = client.IsSupplier,
                       ProfitRate = client.ProfitRate,
                       Name = enterprise.Name,
                       IsDraft = enterprise.IsDraft,
                       Place = enterprise.Place,
                       District = enterprise.District,
                       Summary = enterprise.Summary,
                       Register = register,
                       OwnerID = client.Owner,
                       Status = (AuditStatus)client.Status,
                   };
        }

        protected override IEnumerable<MyClient> OnMyPage(IQueryable<MyClientData> iquery)
        {
            var data = iquery.ToArray();
            var ownersId = data.Select(item => item.OwnerID).Distinct();
            var admins = new AdminsAllRoll(this.Reponsitory).Where(item => ownersId.Contains(item.ID)).ToArray();

            var clientsId = data.Select(item => item.ID).Distinct();

            var linq_conducts = from conduct in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Conducts>()
                                where clientsId.Contains(conduct.EnterpriseID)
                                select new
                                {
                                    ID = conduct.ID,
                                    EnterpriseID = conduct.EnterpriseID,
                                    Type = conduct.Type,
                                    Grade = conduct.Grade,
                                    IsPublic = conduct.IsPublic,
                                };

            var conducts = linq_conducts.ToArray();

            var ienums_clients = from client in data
                                 join admin in admins on client.OwnerID equals admin.ID
                                 join conduct in conducts on client.ID equals conduct.EnterpriseID into _conducts
                                 select new MyClient
                                 {
                                     ID = client.ID,
                                     ClientGrade = client.ClientGrade,
                                     ClientType = client.ClientType,
                                     Vip = client.Vip,
                                     Source = client.Source,
                                     IsMajor = client.IsMajor,
                                     IsSpecial = client.IsSpecial,
                                     Industry = client.Industry,
                                     CreateDate = client.CreateDate,
                                     ModifyDate = client.ModifyDate,
                                     IsSupplier = client.IsSupplier,
                                     ProfitRate = client.ProfitRate,
                                     Name = client.Name,
                                     IsDraft = client.IsDraft,
                                     Place = client.Place,
                                     District = client.District,
                                     Summary = client.Summary,
                                     Register = client.Register,
                                     Owner = admin,
                                     Status = client.Status,
                                     Conducts = _conducts.Select(conduct => new MyConduct
                                     {
                                         ID = conduct.ID,
                                         Type = (ConductType)conduct.Type,
                                         Grade = (ConductGrade)conduct.Grade,
                                         IsPublic = conduct.IsPublic,
                                     }).ToArray()
                                 };

            return ienums_clients;

        }
    }


    class MyClass
    {

        public MyClass()
        {
            using (var view = new ClientsView())
            {
                view.Searh(item => item.OwnerID == "?");

            }
        }
    }

}
