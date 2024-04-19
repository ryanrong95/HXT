using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NtErp.Wss.Sales.Services.Models.Orders
{
    class Tutopo
    {
        public const string toFather = "father";
        public const string fire = "Fire";

        static public T2 From<T1, T2>(T1 from, T2 to) where T1 : class where T2 : class, new()
        {
            Type toType = to.GetType();
            var field = toType.GetField(toFather, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new NotImplementedException($"The type:{toType.FullName} does not implement the field：{toFather}!");
            }

            field.SetValue(to, from);
            return to as T2;
        }

        static public void Fire<T1>(T1 from, EventArgs e) where T1 : class
        {
            Type fromType = from.GetType();
            var method = fromType.GetMethod(fire, BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new NotImplementedException($"The type:{fromType.FullName} does not implement the field：{fire}!");
            }
            method.Invoke(from, new object[] { e });
        }
    }
}
