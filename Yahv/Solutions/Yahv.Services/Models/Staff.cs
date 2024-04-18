namespace Yahv.Services.Models
{
    /// <summary>
    /// Staff
    /// </summary>
    public class Staff : Linq.IUnique
    {
        public Staff()
        {

        }

        #region 属性

        public string ID { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string DyjCompanyCode { get; set; }
        public string DyjDepartmentCode { get; set; }
        public string DyjCode { get; set; }
        public string AdminID { get; set; }

        public string WorkCity { get; set; }
        #endregion
    }
}
