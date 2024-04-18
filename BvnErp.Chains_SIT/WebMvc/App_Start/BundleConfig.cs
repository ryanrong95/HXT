using System.Web;
using System.Web.Optimization;

namespace WebMvc
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterLayout(bundles);
            RegisterMemberCenter(bundles);
            RegisterAccount(bundles);

        }

        /// <summary>
        /// 布局绑定
        /// </summary>
        /// <param name="bundles"></param>
        private static void RegisterLayout(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Shared/_Layout").Include(
                "~/Scripts/Shared/_Layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // plugins | jquery-validate
            bundles.Add(new ScriptBundle("~/Scripts/jquery-validate/js").Include(
                                         "~/Scripts/jquery.validate*"));
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));           
            // plugins | jigsaw
            bundles.Add(new ScriptBundle("~/Scripts/jigsaw/js").Include(
                "~/Scripts/jigsaw.js"));

            bundles.Add(new StyleBundle("~/Content/jigsaw/css").Include(
                "~/Content/jigsaw.css"));

            // plugins | Vue.js
            bundles.Add(new ScriptBundle("~/Scripts/vue/js").Include(
                "~/Scripts/vue.min.js"));

            // plugins | axios.js
            bundles.Add(new ScriptBundle("~/Scripts/axios/js").Include(
                "~/Scripts/axios.min.js"));
            // plugins | element-ui
            bundles.Add(new ScriptBundle("~/Scripts/element-ui/js").Include(
                "~/Scripts/ElementUI/index.js"));
            bundles.Add(new StyleBundle("~/Content/element-ui/css").Include(
                "~/Content/ElementUI/index.css"));
            // plugins | polyfill
            bundles.Add(new ScriptBundle("~/Scripts/polyfill/js").Include(
                "~/Scripts/polyfill.min.js"));
        }

        /// <summary>
        /// 会员中心
        /// </summary>
        /// <param name="bundles"></param>
        private static void RegisterMemberCenter(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/MemberCenter/MemberInformation").Include(
                                         "~/Scripts/MemberCenter/MemberInformation.js"
                ));
        }

        /// <summary>
        /// 账号信息
        /// </summary>
        /// <param name="bundles"></param>
        private static void RegisterAccount(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Account/MyAccountInfo").Include(
                                         "~/Scripts/Account/MyAccountInfo.js"
                ));
        }
    }
}
