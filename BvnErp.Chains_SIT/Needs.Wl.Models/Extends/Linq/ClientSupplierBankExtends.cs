namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientSupplierBankExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientSupplierBanks ToLinq(this ClientSupplierBank entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientSupplierBanks
            {
                ID = entity.ID,
                ClientSupplierID = entity.ClientSupplierID,
                BankAccount = entity.BankAccount,
                BankName = entity.BankName,
                BankAddress = entity.BankAddress,
                SwiftCode = entity.SwiftCode,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}