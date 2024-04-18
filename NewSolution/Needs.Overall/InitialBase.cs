using Needs.Overall.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall
{
    abstract public class InitialBase
    {
        protected InitialBase()
        {
            this.Versions();
        }

        void Versions()
        {
            Assembly assembly = Assembly.GetAssembly(this.GetType());
            using (VersionsView view = new VersionsView())
            {
                new Models.Version
                {
                    ID = this.ProjcetName,
                    Name = this.ProjcetName,
                    Code = assembly.GetName().Version.ToString(),
                    LastGenerationDate = File.GetLastWriteTime(assembly.Location),
                }.Enter();
            }
        }

        /// <summary>
        /// 项目命名
        /// </summary>
        abstract protected string ProjcetName { get; }
    }
}
