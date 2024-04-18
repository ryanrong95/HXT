using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CnslApp
{
    public class OurTypes
    {
        AssemblyBuilder assemblyBuilder;
        ModuleBuilder moduleBuilder;
        TypeBuilder typeBuilder;

        OurTypes()
        {

            AssemblyName aName = new AssemblyName("_" + Guid.NewGuid().ToString("N"));
            AssemblyBuilder assemblyBuilder = this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = this.moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);
            TypeBuilder typeBuildert = this.typeBuilder = moduleBuilder.DefineType("MyDynamicType", TypeAttributes.Public);
        }
    }
}
