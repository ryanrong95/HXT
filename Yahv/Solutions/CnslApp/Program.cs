using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using Yahv.Payments;
using Yahv.Payments.Tools;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Utils.Http;
using Yahv.Services.Views;
using System.Data.Linq;
using System.Reflection;
using CnslApp.Test.Models;
using Yahv.Linq.Extends;
using System.Reflection.Emit;
using System.Linq.Expressions;
using Yahv.Services.Models;

namespace CnslApp
{
    class Program
    {
        static Type BuildType(Type type)
        {




            AssemblyName aName = new AssemblyName("DynamicAssemblyExample");
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name);

            TypeBuilder typeBuilder = mb.DefineType("MyDynamicType", TypeAttributes.Public);

            // Add a private field of type int (Int32).
            //FieldBuilder fbNumber = typeBuilder.DefineField("m_number", typeof(int), FieldAttributes.Private);


            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

            ILGenerator ctor0IL = ctorBuilder.GetILGenerator();
            // For a constructor, argument zero is a reference to the new
            // instance. Push it on the stack before pushing the default
            // value on the stack, then call constructor ctor1.
            ctor0IL.Emit(OpCodes.Ldarg_0);
            //ctor0IL.Emit(OpCodes.Ldc_I4_S, 42);
            //ctor0IL.Emit(OpCodes.Call, ctor1);
            ctor0IL.Emit(OpCodes.Ret);

            foreach (var item in type.GetProperties())
            {
                string name = item.Name;
                var bType = item.PropertyType;


                FieldBuilder fieldBuilder = typeBuilder.DefineField("p_" + name, bType, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, bType, null);

                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                // Define the "get" accessor method for Number. The method returns
                // an integer and has no arguments. (Note that null could be 
                // used instead of Types.EmptyTypes)
                MethodBuilder methodBuilderGetAccessor = typeBuilder.DefineMethod("get_" + name, getSetAttr, bType, Type.EmptyTypes);

                ILGenerator numberGetIL = methodBuilderGetAccessor.GetILGenerator();
                // For an instance property, argument zero is the instance. Load the 
                // instance, then load the private field and return, leaving the
                // field value on the stack.
                numberGetIL.Emit(OpCodes.Ldarg_0);
                numberGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
                numberGetIL.Emit(OpCodes.Ret);


                MethodBuilder methodBuilderSetAccessor = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { bType });

                ILGenerator numberSetIL = methodBuilderSetAccessor.GetILGenerator();
                // Load the instance and then the numeric argument, then store the
                // argument in the field.
                numberSetIL.Emit(OpCodes.Ldarg_0);
                numberSetIL.Emit(OpCodes.Ldarg_1);
                numberSetIL.Emit(OpCodes.Stfld, fieldBuilder);
                numberSetIL.Emit(OpCodes.Ret);


                propertyBuilder.SetGetMethod(methodBuilderGetAccessor);
                propertyBuilder.SetSetMethod(methodBuilderSetAccessor);
            }
            return typeBuilder.CreateType();

        }

        static object dfe()
        {
            var uer2 = new
            {
                ID = "002",
                Name = "Jim"
            };
            return uer2;
        }

        class MyQueryProvider : IQueryProvider
        {
            public IQueryable CreateQuery(Expression expression)
            {
                throw new NotImplementedException();
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                //根据表达式，做好lucene查询条件的翻译




                throw new NotImplementedException();
            }

            public object Execute(Expression expression)
            {
                throw new NotImplementedException();
            }

            public TResult Execute<TResult>(Expression expression)
            {
                throw new NotImplementedException();
            }
        }


        static void Main(string[] args)
        {
            //费用测试
            PaysTest.Test();
            Console.WriteLine("Complete");
            Console.Read();
        }

        //static protected void PaySuccessEvent(PayConfirmedEventArgs sender, ConfirmedEventArgs e)
        //{

        //}

        /// <summary>
        /// LinqFactory 连接工厂测试
        /// </summary>
        static void LinqCreate()
        {
            //如下代码示例不要删除
            //new Test.Models.Menu().Enter();
        }

        #region 注释掉的
        ///// <summary>
        ///// LinqFactory 连接工厂测试-跨库 1s 
        ///// </summary>
        //static void LinqCreate2()
        //{
        //    Console.WriteLine("LinqCreate2 start : " + DateTime.Now.ToString());
        //    using (var r = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        new Test.Models.Menu().Enter();

        //        new Yahv.RFQ.Services.Models.Client
        //        {
        //            Enterprise = new Yahv.RFQ.Services.Models.Enterprise
        //            {
        //                Name = "测试Enterprise1",
        //                Business = Yahv.Underly.EnterpriseType.Client,
        //                MainID = "",
        //                Tel = "tel",
        //                Address = "address"
        //            },
        //            Grade = Yahv.Underly.ClientGrade.Eighth,
        //            Type = Yahv.Underly.ClientType.Terminals,
        //            Vip = false
        //        }.Enter();
        //    }
        //    Console.WriteLine("LinqCreate2 end : " + DateTime.Now.ToString());
        //}

        ///// <summary>
        ///// LinqFactory 连接工厂测试-跨库 1000次 2 m
        ///// </summary>
        //static void LinqCreate3()
        //{
        //    Console.WriteLine("LinqCreate3 start : " + DateTime.Now.ToString());
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        using (var r = LinqFactory<PvbCrmReponsitory>.Create())
        //        {
        //            new Test.Models.Menu().Enter();

        //            new Yahv.RFQ.Services.Models.Client
        //            {
        //                Enterprise = new Yahv.RFQ.Services.Models.Enterprise
        //                {
        //                    Name = "测试Enterprise" + i,
        //                    Business = Yahv.Underly.EnterpriseType.Client,
        //                    MainID = "",
        //                    Tel = "tel",
        //                    Address = "address"
        //                },
        //                Grade = Yahv.Underly.ClientGrade.Eighth,
        //                Type = Yahv.Underly.ClientType.Terminals,
        //                Vip = false
        //            }.Enter();
        //        }
        //    }
        //    Console.WriteLine("LinqCreate3 end : " + DateTime.Now.ToString());
        //}

        ///// <summary>
        ///// LinqFactory 连接工厂测试-跨库 多线程
        ///// </summary>
        //static void LinqCreate4()
        //{
        //    Console.WriteLine("LinqCreate4 start : " + DateTime.Now.ToString());
        //    Thread thread1 = new Thread(new ThreadStart(LinqCreate2));
        //    thread1.Start();
        //    Thread thread2 = new Thread(new ThreadStart(LinqCreate2));
        //    thread2.Start();
        //    Console.WriteLine("LinqCreate4 end : " + DateTime.Now.ToString());
        //}
        ///// <summary>
        ///// LinqFactory 连接工厂测试-跨库 1000次 多线程
        ///// </summary>
        //static void LinqCreate5()
        //{
        //    Console.WriteLine("LinqCreate5 start : " + DateTime.Now.ToString());
        //    Thread thread1 = new Thread(new ThreadStart(LinqCreate3));
        //    thread1.Start();
        //    Thread thread2 = new Thread(new ThreadStart(LinqCreate3));
        //    thread2.Start();
        //    Console.WriteLine("LinqCreate5 end : " + DateTime.Now.ToString());
        //}

        ///// <summary>
        ///// LinqFactory 连接工厂测试-跨库 多线程 异步
        ///// </summary>
        //static void LinqCreate6()
        //{
        //    Console.WriteLine("LinqCreate6 start : " + DateTime.Now.ToString());

        //    // 定义一个无参数、int类型返回值的委托
        //    Func<int> func = () =>
        //    {
        //        LinqCreate2();
        //        return DateTime.Now.Day;
        //    };
        //    // 输出委托同步调用的返回值
        //    Console.WriteLine($"func.Invoke()={func.Invoke()}");
        //    // 委托的异步调用
        //    IAsyncResult asyncResult = func.BeginInvoke(p =>
        //    {
        //        Console.WriteLine(p.AsyncState);
        //    }, "异步调用返回值");
        //    // 输出委托异步调用的返回值
        //    Console.WriteLine($"func.EndInvoke(asyncResult)={func.EndInvoke(asyncResult)}" + " " + DateTime.Now.ToString());
        //    //Thread thread1 = new Thread(new ThreadStart(LinqCreate3));
        //    //thread1.Start();
        //    //Thread thread2 = new Thread(new ThreadStart(LinqCreate3));
        //    //thread2.Start();
        //    //Console.WriteLine("LinqCreate6 end : " + DateTime.Now);
        //}
        #endregion
    }
}
