using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SupplierGradeByEngNameView
    {
        private string _EngName = string.Empty;

        public SupplierGradeByEngNameView(string engName)
        {
            this._EngName = engName;
        }

        public SupplierGradeByEngNameViewModel GetGrade()
        {
            SupplierGradeByEngNameViewModel result = null;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var clientSuppliers = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();

                result = (from clientSupplier in clientSuppliers
                          where clientSupplier.Name == this._EngName
                             && clientSupplier.Status == (int)Status.Normal
                          orderby clientSupplier.UpdateDate descending
                          select new SupplierGradeByEngNameViewModel
                          {
                              SupplierEngName = clientSupplier.Name,
                              SupplierGrade = clientSupplier.Grade != null ? (SupplierGrade?)clientSupplier.Grade : null,
                          }).FirstOrDefault();
            }

            return result;
        }
    }

    public class SupplierGradeByEngNameViewModel
    {
        public string SupplierEngName { get; set; }

        public SupplierGrade? SupplierGrade { get; set; }
    }
}
