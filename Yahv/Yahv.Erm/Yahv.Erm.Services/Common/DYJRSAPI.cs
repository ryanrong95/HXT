using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Common
{
    public sealed class DYJRSAPI
    {
        private static readonly DYJRSAPI instance = new DYJRSAPI();

        private DYJRSService.RSServerClient service;

        static DYJRSAPI()
        {
        }

        private DYJRSAPI()
        {
            service = new DYJRSService.RSServerClient();
        }

        public static DYJRSAPI Instance
        {
            get
            {
                return instance;
            }
        }

        public string GetDYJCorp()
        {
            return service.GetDYJCorp();
        }

        public string GetDYJDept()
        {
            return service.GetDYJDept();
        }

        public string GetWorkDate(string code, DateTime date)
        {
            var result = service.GetWorkDate(code.Trim(','), date.Date, date.Date.AddDays(1));
            return result;
        }

        public string GetUserInfoList(string corpid, string departid, string uiid)
        {
            return service.GetUserInfoList(corpid, departid, uiid);
        }

        public string GetEmployeeByID(string uiid)
        {
            return service.GetEmployeeByID(uiid);
        }

        public List<QingJiaData> GetQingJiaList(string dyjId, DateTime start, DateTime end)
        {
            var result = service.GetQingJiaList(dyjId, start, end).JsonTo<DYJQingjiaRSMessage>();
            List<QingJiaData> list = new List<QingJiaData>();
            if (result.errCod == "0")
            {
                list = result.data;
            }
            return list;
        }

        /// <summary>
        /// 保存新员工
        /// </summary>
        /// <param name="staff">新员工</param>
        /// <param name="creater">创建人</param>
        public string SetUserInfo(Staff staff, Staff creater)
        {
            if (staff == null || creater == null)
            {
                throw new Exception("当前员工或审批人为空");
            }
            var name = staff.Name;
            var corpID = "6900";//创新很远分公司
            var deptID = "6903";//国内业务部
            var idCard = staff.Personal.IDCard;
            var sex = staff.Gender.GetDescription();
            var email = staff.Personal.Email;
            var phone = staff.Personal.Mobile;
            var mobilePhone = staff.Personal.Mobile;
            var address = staff.Personal.PassAddress;
            var addressnow = staff.Personal.HomeAddress;
            var createUid = creater.DyjCode;
            var workDate = staff.Labour.EntryDate.ToString("yyyy-MM-dd HH:mm:ss");
            var hunFou = staff.Personal.IsMarry == true ? "已婚" : "未婚";
            var zhengzhimianmao = staff.Personal.PoliticalOutlook;
            var minzu = staff.Personal.Volk;
            var jinjilianxiren = staff.Personal.EmergencyContact;
            var jinjiphone = staff.Personal.EmergencyMobile;

            var bank = new Views.Origins.BankCardsOrigin().FirstOrDefault(item => item.ID == staff.ID);
            if (string.IsNullOrEmpty(bank?.Bank) || string.IsNullOrEmpty(bank?.Account))
            {
                throw new Exception("当前员工的银行或银行账号为空");
            }
            var BankType = bank.Bank;
            var BankNumber = bank.Account;
            //var result = service.SetUserInfo("测试人", "6900", "6903", "男", "130181199110133011", "1085412279@qq.com", "13241128350", "13241128350", "1085412279", "河北石家庄", "1+1大厦", "", "", "101", "2020-5-13 15:38:10", "已婚", "群众", "民族", "北京昌平", "郝磊2", "13241125855", "0", "中国工商银行", "888888888", "base64", "芯达通", "我是测试数据！请不要审批");
            var result = service.SetUserInfo(name, corpID, deptID, sex, idCard, email, phone, mobilePhone, "", address, "", "", "", createUid, workDate, hunFou, zhengzhimianmao, minzu, "", "", "", "0", BankType, BankNumber, "base64", "芯达通", "我是芯达通测试数据！请不要审批").JsonTo<DYJRSMessage>();

            return result.Json();
        }
    }

    public class DYJRSMessage
    {
        public string errCod { get; set; }

        public string errMsg { get; set; }

    }
    public class DYJQingjiaRSMessage
    {
        public string errCod { get; set; }

        public string errMsg { get; set; }

        public List<QingJiaData> data { get; set; }
    }

    public class QingJiaData
    {
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string Stype { get; set; }

        public string STATE { get; set; }

        public string sdate { get; set; }

        public string bantian { get; set; }
    }
}
