using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;

namespace YaHv.VcCsrm.Service
{

    public class AdminBase
    {

    }

    /// <summary>
    /// 销售
    /// </summary>
    public class Sales : AdminBase
    {

    }

    /// <summary>
    /// 跟单
    /// </summary>
    public class Tracker : AdminBase
    {

    }



    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class ShipUniqueAttribute : Attribute
    {

    }

    public class ShipEnterBase
    {
        public string Unique { get; set; }

        string md5;
        public string MD5
        {
            get { return this.md5 ?? this.ValueForMd5(); }
            set { this.md5 = value; }
        }

        /// <summary>
        /// 检查MD5字段是否正确
        /// </summary>
        /// <param name="isValue">强行复制</param>
        /// <returns>争取</returns>
        public bool Md5Check(bool isValue = false)
        {
            if (isValue)
            {
                this.md5 = this.ValueForMd5();
                return true;
            }
            return this.md5 == this.ValueForMd5();
        }

        public ShipEnterBase()
        {

        }

        /// <summary>
        /// 获取MD5的值
        /// </summary>
        /// <returns>MD5的值</returns>
        string ValueForMd5()
        {
            var type = this.GetType();

            StringBuilder builder = new StringBuilder();
            foreach (var item in type.GetProperties().Where(item => item.GetCustomAttribute<ShipUniqueAttribute>() != null))
            {
                builder.Append(item.GetValue(this));
            }

            string mtype = this.TypeForMd5()?.Trim();
            if (!string.IsNullOrWhiteSpace(mtype))
            {
                builder.Append(type);
            }

            return builder.ToString().MD5();
        }

        /// <summary>
        /// 私有唯一类型定制
        /// </summary>
        /// <returns>类型名称</returns>
        virtual protected string TypeForMd5()
        {
            return null;
        }
    }


    public class WsClient : ShipEnterBase
    {
        [ShipUnique]
        public string Name { get; set; }

        /// <summary>
        /// 关系ID
        /// </summary>
        public string ShipID { get; set; }

        /// <summary>
        /// 这就是精神
        /// </summary>
        /// <returns></returns>
        protected override string TypeForMd5()
        {
            return this.ShipID;
        }

        public object[] 合作公司 { get; set;  }
    }

    class 合作公司
    {
        public Sales[] Sales { get; set; }

        public Sales[] Tracker { get; set; }

    }
}
