namespace Needs.Wl.User.Plat
{
    public static partial class UserPlatExtends
    {
        public static Needs.Wl.Models.User ToUser(this Models.IPlatUser user)
        {
            return new Wl.Models.User()
            {
                ID = user.ID,
                ClientID = user.ClientID,
                Name = user.UserName,
                RealName = user.RealName,
                Email = user.Email,
                IsMain = user.IsMain,
                Mobile = user.Mobile,
                Password = user.Password
            };
        }
    }
}
