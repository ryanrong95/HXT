using System;
using System.Linq;

namespace Needs.Web.Faces
{
    public class ErpGlobals : Globals
    {
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
