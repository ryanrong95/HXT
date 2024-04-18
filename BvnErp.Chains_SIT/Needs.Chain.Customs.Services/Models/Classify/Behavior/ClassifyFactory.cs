using Needs.Ccs.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 归类操作简单工厂
    /// </summary>
    public class ClassifyFactory
    {
        public static Classify Create(Enums.ClassifyStep step, IClassifyProduct product)
        {
            switch (step)
            {
                case Enums.ClassifyStep.Step1:
                    return new ClassifyStep1(product);

                case Enums.ClassifyStep.Step2:
                    return new ClassifyStep2(product);

                case Enums.ClassifyStep.DoneEdit:
                    return new ClassifyDoneEdit(product);

                case Enums.ClassifyStep.ReClassify:
                    return new ReClassify(product);

                case Enums.ClassifyStep.PreStep1:
                    return new PreClassifyStep1(product);

                case Enums.ClassifyStep.PreStep2:
                    return new PreClassifyStep2(product);

                case Enums.ClassifyStep.PreDoneEdit:
                    return new PreClassifyDoneEdit(product);

                default:
                    return null;
            }
        }
    }
}
