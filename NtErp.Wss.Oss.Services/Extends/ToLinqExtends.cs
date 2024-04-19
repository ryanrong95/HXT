using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Extends
{
    static public class ToLinqExtends
    {
        static public Layer.Data.Sqls.CvOss.UserOutputs ToLinq(this Models.UserOutput entity)
        {
            return new Layer.Data.Sqls.CvOss.UserOutputs
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Type = (int)entity.Type,
                From = (int)entity.From,
                OrderID = entity.OrderID,
                UserInputID = entity.UserInputID,
                Currency = (int)entity.Currency,
                Amount = entity.Amount,
                DateIndex = entity.DateIndex,
                CreateDate = entity.CreateDate
            };
        }
        static public Layer.Data.Sqls.CvOss.UserInputs ToLinq(this Models.UserInput entity)
        {
            return new Layer.Data.Sqls.CvOss.UserInputs
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Type = (int)entity.Type,
                From = (int)entity.From,
                Code = entity.Code,
                Currency = (int)entity.Currency,
                Amount = entity.Amount,
                CreateDate = entity.CreateDate
            };
        }

        static public Layer.Data.Sqls.CvOss.Orders ToLinq(this Models.Order entity)
        {
            return new Layer.Data.Sqls.CvOss.Orders
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                ClientID = entity.Client.ID,
                BeneficiaryID = entity.Beneficiary.ID,
                InvoiceID = entity.Invoice.ID,
                ConsigneeID = entity.Consignee.ID,
                DelivererID = entity.Deliverer.ID,
                Status = (int)entity.Status,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Paid = entity.Paid,
                Total = entity.Total,

            };
        }

        static public Layer.Data.Sqls.CvOss.OrderItems ToLinq(this Models.OrderItem entity)
        {
            return new Layer.Data.Sqls.CvOss.OrderItems
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                ServiceID = entity.ServiceID,
                From = (int)entity.From,
                Origin = entity.Origin,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                ProductID = entity.Product.ID,
                SupplierID = entity.Supplier.ID,
                Weight = entity.Weight,
                Status = (int)entity.Status,
                CustomerCode = entity.CustomerCode,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Leadtime = entity.Leadtime,
                Note = entity.Note
            };
        }

        static public Layer.Data.Sqls.CvOss.Invoices ToLinq(this Models.Invoice entity)
        {
            return new Layer.Data.Sqls.CvOss.Invoices
            {
                ID = entity.ID,
                Required = entity.Required,
                Type = (int)entity.Type,
                CompanyID = entity.CompanyID,
                ContactID = entity.ContactID,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
            };
        }
        static public Layer.Data.Sqls.CvOss.TransportTerms ToLinq(this Models.TransportTerm entity)
        {
            return new Layer.Data.Sqls.CvOss.TransportTerms
            {
                ID = entity.ID,
                TransportMode = (int)entity.TransportMode,
                FreightMode = (int)entity.FreightMode,
                PriceClause = (int)entity.PriceClause,
                Carrier = entity.Carrier,
                Address = entity.Address
            };
        }

        static public Layer.Data.Sqls.CvOss.Waybills ToLinq(this Models.Waybill entity)
        {
            return new Layer.Data.Sqls.CvOss.Waybills
            {
                ID = entity.ID,
                Carrier = entity.Carrier,
                Weight = entity.Weight
            };
        }

        static public Layer.Data.Sqls.CvOss.WayItems ToLinq(this Models.WayItem entity)
        {
            return new Layer.Data.Sqls.CvOss.WayItems
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                WaybillID = entity.WaybillID,
                Weight = entity.Weight,
                Count = entity.Count,
                Source = (int)entity.Source,
            };
        }

        static public Layer.Data.Sqls.CvOss.Premiums ToLinq(this Models.Premium entity)
        {
            return new Layer.Data.Sqls.CvOss.Premiums
            {
                ID = entity.ID,
                Name = entity.Name,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                Count = entity.Count,
                Price = entity.Price,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate
            };
        }

        static public Layer.Data.Sqls.CvOss.StandardProducts ToLinq(this Models.StandardProduct entity)
        {
            return new Layer.Data.Sqls.CvOss.StandardProducts
            {
                ID = entity.ID,
                SignCode = entity.SignCode,
                Name = entity.Name,
                ManufactruerID = entity.Manufacturer.ID,
                PackageCase = entity.PackageCase,
                Packaging = entity.Packaging,
                Batch = entity.Batch,
                DateCode = entity.DateCode,
                Description = entity.Description,
            };
        }
        static public Layer.Data.Sqls.CvOss.Beneficiaries ToLinq(this Models.Beneficiary entity)
        {
            return new Layer.Data.Sqls.CvOss.Beneficiaries
            {
                ID = entity.ID,
                Bank = entity.Bank,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Address = entity.Address,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                ContactID = entity.Contact?.ID,
                CompanyID = entity.Company.ID
            };
        }


        static public Layer.Data.Sqls.CvOss.Parties ToLinq(this Models.Party entity)
        {
            return new Layer.Data.Sqls.CvOss.Parties
            {
                ID = entity.ID,
                CompanyID = entity.CompanyID,
                ContactID = entity.ContactID,
                Address = entity.Address,
                Postzip = entity.Postzip,
            };
        }

        static public Layer.Data.Sqls.CvOss.Companies ToLinq(this Models.Company entity)
        {
            return new Layer.Data.Sqls.CvOss.Companies
            {
                ID = entity.ID,
                Name = entity.Name,
                Address = entity.Address,
                Code = string.IsNullOrWhiteSpace(entity.Code) ? "" : entity.Code,
                Type = (int)entity.Type,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }

        static public Layer.Data.Sqls.CvOss.Contacts ToLinq(this Models.Contact entity)
        {
            return new Layer.Data.Sqls.CvOss.Contacts
            {
                ID = entity.ID,
                Name = entity.Name,
                CompanyID = entity.Company.ID,
                Email = entity.Email,
                Mobile = entity.Mobile,
                Tel = entity.Tel
            };
        }

    }
}
