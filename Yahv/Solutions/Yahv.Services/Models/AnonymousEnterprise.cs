using Yahv.Utils.Converters.Contents;

namespace Yahv.Services
{
    /// <summary>
    /// 匿名企业
    /// </summary>
    public class AnonymousEnterprise
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { get; set; }

        AnonymousEnterprise()
        {
            this.Name = "匿名";
            this.ID = this.Name.MD5();
        }

        static object obj = new object();
        static AnonymousEnterprise current;

        static public AnonymousEnterprise Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new AnonymousEnterprise();
                        }
                    }
                }

                return current;
            }
        }
    }
}