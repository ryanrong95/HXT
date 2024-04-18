using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 归类操作简单工厂
    /// </summary>
    public class ClassifyFactory
    {
        public static Classify Create(ClassifyStep step, Interfaces.IClassifyProduct product)
        {
            switch (step)
            {
                case ClassifyStep.Step1:
                    return new ClassifyStep1(product);

                case ClassifyStep.Step2:
                    return new ClassifyStep2(product);

                case ClassifyStep.DoneEdit:
                    return new ClassifyDoneEdit(product);

                case ClassifyStep.ReClassify:
                    return new ReClassify(product);

                case ClassifyStep.PreStep1:
                    return new PreClassifyStep1(product);

                case ClassifyStep.PreStep2:
                    return new PreClassifyStep2(product);

                case ClassifyStep.PreDoneEdit:
                    return new PreClassifyDoneEdit(product);

                default:
                    return null;
            }
        }
    }
}
