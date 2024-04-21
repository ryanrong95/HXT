using System;

namespace Yahv.Erm.WebApi.Models
{
    /// <summary>
    /// 大赢家 接口参数信息
    /// </summary>
    public class DyjInputDto
    {
        /// <summary>
        /// 接口方法名称
        /// </summary>
        public string Requestitem { get; set; }

        /// <summary>
        /// 权限验证码 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public object Data { get; set; }
    }

    //public class Data
    //{
    //    /// <summary>
    //    /// 用户ID
    //    /// </summary>
    //    public int ID { get; set; }

    //    /// <summary>
    //    /// 姓名 配合Islike支持模糊
    //    /// </summary>
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// 昵称 配合Islike支持模糊
    //    /// </summary>
    //    public string Nickname { get; set; }

    //    /// <summary>
    //    /// 是否模糊查询
    //    /// </summary>
    //    public bool Islike { get; set; }

    //    /// <summary>
    //    /// 前多少条 最大返回20条
    //    /// </summary>
    //    public int TopNum { get; set; }
    //}
}