using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Entry
{
    public partial class EditCaseNumber : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }


        protected void LoadData()
        {
            string PackingID = Request.Form["PackingID"];


        }


        /// <summary>
        /// 修改箱号
        /// </summary>
        protected void ChangeCaseNumber()
        {
            try
            {
                string PackingID = Request.Form["PackingID"];
                string NewCaseNumber = Request.Form["NewCaseNumber"];
                DateTime NewPackingDate = Convert.ToDateTime(Request.Form["NewPackingDate"]);

                var packingView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Packing;
                var packings = packingView.Where(item => item.PackingDate == NewPackingDate && item.BoxIndex.StartsWith("HXT"));
                var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking().Where(x => x.Packing.ID == PackingID);
                if (packingBill.Count() > 1 && NewCaseNumber.IndexOf("-") != -1)
                {
                    
                    Response.Write((new { success = false, message = "输入箱号为连续箱号时，只能勾选一个装箱产品" }).Json());
                    return;
                }
                //验证箱号是否用过
                int[] arr1 = this.GetCaseNumbers(NewCaseNumber);
                int[] arr2 = this.GetCaseNumbers(packings);
                var diffArr = arr1.Where(c => arr2.Contains(c)).ToArray();
                if (diffArr.Count() > 0)
                {
                    string caseNumber = "HXT" + diffArr.First().ToString().PadLeft(3, '0');
                    Response.Write((new { success = false, message = "箱号" + caseNumber + "已使用过：请选择其它箱号。" }).Json());
                    return;
                }
                //更新箱号 ，同时修改sorting的箱号
                var data = packingView[PackingID];
                if (data.PackingStatus == Needs.Ccs.Services.Enums.PackingStatus.Sealed)
                {
                    Response.Write((new { success = false, message = "已封箱，不能修改箱号" }).Json());
                    return;
                }
                data.BoxIndex = NewCaseNumber;
                data.PackingDate = NewPackingDate;
                data.ChangeBoxIndex();
                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改失败：" + ex.Message }).Json());
            }
        }

        private int[] GetCaseNumbers(string CaseNumber)
        {
            List<int> list = new List<int>();
            if (CaseNumber.Contains("-"))
            {
                string[] str = CaseNumber.Split('-');
                int box1 = int.Parse(str[0].Remove(0, 3));
                int box2 = int.Parse(str[1].Remove(0, 3));
                for (int i = box1; i < box2 + 1; i++)
                {
                    list.Add(i);
                }
            }
            else
            {
                list.Add(int.Parse(CaseNumber.Remove(0, 3)));
            }
            return list.ToArray();
        }

        private int[] GetCaseNumbers(IQueryable<Needs.Ccs.Services.Models.Packing> packings)
        {
            List<int> list = new List<int>();
            foreach (var packing in packings)
            {
                int[] array = this.GetCaseNumbers(packing.BoxIndex);
                list.AddRange(array);
            }
            return list.ToArray();
        }
    }
}