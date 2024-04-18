namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientSupplierAddressExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses ToLinq(this ClientSupplierAddress entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses
            {
                ID = entity.ID,
                ClientSupplierID = entity.ClientSupplierID,
                ContactID = entity.Contact.ID,
                Address = entity.Address,
                ZipCode = entity.ZipCode,
                IsDefault = entity.IsDefault,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}