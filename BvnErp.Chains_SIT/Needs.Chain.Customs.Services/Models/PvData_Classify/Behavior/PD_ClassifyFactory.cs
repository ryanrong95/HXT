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
    public class PD_ClassifyFactory
    {
        public static PD_Classify Create(Enums.ClassifyStep step, PD_IClassifyProduct product)
        {
            switch (step)
            {
                case Enums.ClassifyStep.Step1:
                    return new PD_ClassifyStep1(product);

                case Enums.ClassifyStep.Step2:
                    return new PD_ClassifyStep2(product);

                case Enums.ClassifyStep.DoneEdit:
                    return new PD_ClassifyDoneEdit(product);

                case Enums.ClassifyStep.ReClassify:
                    return new PD_ReClassify(product);

                case Enums.ClassifyStep.PreStep1:
                    return new PD_PreClassifyStep1(product);

                case Enums.ClassifyStep.PreStep2:
                    return new PD_PreClassifyStep2(product);

                case Enums.ClassifyStep.PreDoneEdit:
                    return new PD_PreClassifyDoneEdit(product);

                default:
                    return null;
            }
        }
    }
}
