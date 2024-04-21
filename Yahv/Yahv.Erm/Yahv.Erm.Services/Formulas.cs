using System;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic;

namespace Yahv.Erm.Services
{
    public class Formulas
    {
        public class Result
        {
            CompilerResults result;

            internal Result(CompilerResults result)
            {
                this.result = result;
            }

            public object Calc(params object[] parameters)
            {
                Assembly objAssembly = this.result.CompiledAssembly;
                object objInstance = objAssembly.CreateInstance("DE55AB813039493C969201768D1C1BB4.Calc");
                MethodInfo objMI = objInstance.GetType().GetMethod("Excute");
                return objMI.Invoke(objInstance, new object[] { parameters });
            }
        }

        public Dictionary<string, CompilerResults> data;

        public Formulas()
        {
            data = new Dictionary<string, CompilerResults>();
        }

        /// <summary>
        /// 创建者
        /// </summary>
        static public void Create(string name, string formula)
        {
            // 1.CSharpCodePrivoder
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.ICodeComplier
            ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();

            // 3.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            objCompilerParameters.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "Bin\\Yahv.Erm.Services.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            // 4.CompilerResults
            CompilerResults result = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, GenCode(formula));

            Current.data[name] = result;

            if (result.Errors.HasErrors)
            {
                Console.WriteLine("编译错误：");

                StringBuilder txt = new StringBuilder();
                foreach (CompilerError err in result.Errors)
                {
                    txt.AppendLine(err.ErrorText);
                }

                throw new Exception(txt.ToString());
            }

            //通过反射，调用HelloWorld的实例
            //Assembly objAssembly = result.CompiledAssembly;
            //object objHelloWorld1 = objAssembly.CreateInstance("DynamicCodeGenerate.Calc");
            //MethodInfo objMI = objHelloWorld1.GetType().GetMethod("Excute");
            //Console.WriteLine(objMI.Invoke(objHelloWorld1, null));
        }

        static string GenCode(string formula)
        {
            formula = formula.Replace("&gt;", ">").Replace("&lt;", "<");
            string code = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Bin\\FormulaTemple.cs", Encoding.UTF8);

            //获取全部的工资项目，工资项目名称只能是：标准C#命名
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var list = parameters.OfType<PayWageItem>();");

            //读取公式里边的工资项
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[\u4e00-\u9fa5_a-zA-Z0-9]+");
            var varsTemp = string.Empty;        //去除重复项
            foreach (var item in reg.Matches(formula))
            {
                if (!varsTemp.Contains($",{item},"))
                {
                    sb.Append(" decimal ").Append(item.ToString()).
                        Append(" = list.FirstOrDefault(item => item.Name == \"").Append(item.ToString()).Append("\") ==null ? 0 : list.FirstOrDefault(item => item.Name == \"").Append(item.ToString()).Append("\").Value; ")
                        .AppendLine();
                }

                varsTemp += $",{item},";
            }

            //var temp = code.Replace("[formula]", formula.Replace("零", "0")).Replace("//...define", sb.ToString());

            return code.Replace("[formula]", formula.Replace("零", "0")).Replace("//...define", sb.ToString());
        }

        public Result this[string index]
        {
            get { return new Result(this.data[index]); }
        }

        static object locker = new object();
        static Formulas current;
        /// <summary>
        /// 单例
        /// </summary>
        static public Formulas Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Formulas();
                        }
                    }
                }

                return current;
            }
        }
    }
}