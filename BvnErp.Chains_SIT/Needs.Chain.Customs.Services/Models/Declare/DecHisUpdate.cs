using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DecHisUpdate
    {
        public string DecListID { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public string HSCode { get; set; }
        public string Name { get; set; }
        public string LegalUnit1 { get; set; }
        public string LegalUnit2 { get; set; }
        public string Elements { get; set; }

        public DecHisUpdate(DecList list)
        {
            this.DecListID = list.ID;
            this.PartNumber = list.GoodsModel;
            this.Manufacturer = list.GoodsBrand;
            this.HSCode = list.CodeTS;
            this.Name = list.GName;
            this.LegalUnit1 = list.FirstUnit;
            this.LegalUnit2 = list.SecondUnit;
            this.Elements = list.GModel;
        }

        public void UpdataHistory()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmmd = new SqlCommand("ClassifiedInfosSyncPro", conn);
                cmmd.CommandType = CommandType.StoredProcedure;

                cmmd.Parameters.AddWithValue("@DecListID", this.DecListID); //报关单表体ID
                cmmd.Parameters.AddWithValue("@PartNumber",this.PartNumber); //型号
                cmmd.Parameters.AddWithValue("@Manufacturer", this.Manufacturer); //品牌
                cmmd.Parameters.AddWithValue("@HSCode", this.HSCode); //海关编码
                cmmd.Parameters.AddWithValue("@Name", this.Name); //报关品名
                cmmd.Parameters.AddWithValue("@LegalUnit1", this.LegalUnit1); //法一单位
                cmmd.Parameters.AddWithValue("@LegalUnit2", this.LegalUnit2); //法二单位
                cmmd.Parameters.AddWithValue("@Elements", this.Elements); //申报要素

                cmmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
