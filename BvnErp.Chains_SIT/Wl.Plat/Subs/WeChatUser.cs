namespace Needs.Wl.User.Plat.Models
{
    public partial class WeChatUser
    {
        public new WebSites WebSite
        {
            get
            {
                return new WebSites(this);
            }
        }
    }
}
