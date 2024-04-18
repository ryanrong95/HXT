using Yahv.Linq;

namespace Yahv.Plats.Services.Models.Origins
{
    public class StaffWxs : IUnique
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 用户统一标识
        /// </summary>
        public string UnionID { get; set; }

        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        public string OpenID { get; set; }

        public string Nickname { get; set; }
        public int? Sex { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string HeadImgurl { get; set; }
    }
}