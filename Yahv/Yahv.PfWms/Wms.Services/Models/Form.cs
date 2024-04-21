using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Services.Enums;
using Yahv.Usually;

namespace Wms.Services.Models
{
    public class Form : Yahv.Services.Models.Form, IUnique, IPersisting
    {
        #region 事件
        public event SuccessHanlder FormSuccess;
        public event ErrorHanlder FormError;

        #endregion 


        #region 方法

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            throw new NotImplementedException("不支持");
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    //ID为空是新增
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = PKeySigner.Pick(PkeyType.Inputs);
                        repository.Insert(this.ToLinq());
                    }

                }

                FormSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch (Exception ex)
            {
                FormError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
            }
        }

        #endregion
    }
}
