using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class UsersExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Users ToLinq(this User entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Users
            {
                ID = entity.ID,
                AdminID = entity.AdminID,
                RealName = entity.RealName,
                ClientID = entity.ClientID,
                Email = entity.Email,
                Mobile = entity.Mobile,
                Name = entity.Name,
                Password = entity.Password,
                IsMain = entity.IsMain,
                Status = entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}