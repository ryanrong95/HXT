using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Models.Origins
{
    /// <summary>
    /// 海关检验检疫编码
    /// </summary>
    public class CustomsCiqCode : IUnique, IPersist, IPersistence
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 检验检疫名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 检验检疫类目
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 检疫代码
        /// </summary>
        public string InspectionCode { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public CustomsCiqCode()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
    }
}
