namespace Yahv.Web.Faces.Erp
{
    /// <summary>
    /// Yahv.Web.Faces.Erp 的 Globals
    /// </summary>
    public class Globals : Faces.Globals
    {
        /// <summary>
        /// 重写 在添加所有事件处理程序模块之后执行自定义初始化代码
        /// </summary>
        public override void Init()
        {
            //var services = AppDomain.CurrentDomain.GetAssemblies().Where(item => item.FullName.Contains(".Services")).ToArray();

            //foreach (var assembly in services)
            //{
            //    var items = assembly.GetTypes().Where(item => item.FullName.EndsWith(".Services.Initializer"));
            //    foreach (var type in items)
            //    {
            //        try
            //        {
            //            Activator.CreateInstance(type, false);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //    }
            //}
        }
    }
}
