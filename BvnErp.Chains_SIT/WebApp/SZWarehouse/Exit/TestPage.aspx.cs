using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Exit
{
    public partial class TestPage : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Upload()
        {
            try
            {
                string ID = Request.Form["ID"];

                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler(ID);
                testHandler.Add();

                Response.Write((new
                {
                    success = true,
                    message = "Add ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void BarchInsert()
        {
            try
            {
                string IDs = Request.Form["IDs"];
                string[] IDsArray = IDs.Split(',');

                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler(IDsArray);
                testHandler.BarchInsert();

                Response.Write((new
                {
                    success = true,
                    message = "BarchInsert ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"];

                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler(ID);
                testHandler.Delete();

                Response.Write((new
                {
                    success = true,
                    message = "Delete ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void Update()
        {
            try
            {
                string ID = Request.Form["ID"];
                string AdminID = Request.Form["AdminID"];
                string Name = Request.Form["Name"];

                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler(ID, AdminID, Name);
                testHandler.Update();

                Response.Write((new
                {
                    success = true,
                    message = "Update ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void UpdateChangeds()
        {
            try
            {
                string ID = Request.Form["ID"];
                string AdminID = Request.Form["AdminID"];
                string Name = Request.Form["Name"];

                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler(ID, AdminID, Name);
                testHandler.UpdateChangeds();

                Response.Write((new
                {
                    success = true,
                    message = "UpdateChangeds ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }


        protected void UpdateProblem()
        {
            try
            {
                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler();
                testHandler.UpdateProblem();

                Response.Write((new
                {
                    success = true,
                    message = "UpdateProblem ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void UpdateObjects()
        {
            try
            {
                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler();
                testHandler.UpdateObjects();

                Response.Write((new
                {
                    success = true,
                    message = "UpdateProblem ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void DeleteBoolean()
        {
            try
            {
                Needs.Ccs.Services.Models.TestHandler testHandler = new Needs.Ccs.Services.Models.TestHandler();
                testHandler.DeleteBoolean();

                Response.Write((new
                {
                    success = true,
                    message = "DeleteBoolean ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void TestExceptionLog()
        {
            try
            {
                //Needs.Ccs.Services.Models.PaymentToYahv paymentToYahv = new Needs.Ccs.Services.Models.PaymentToYahv();
                //paymentToYahv.Execute();

                Response.Write((new
                {
                    success = true,
                    message = "TestExceptionLog ok",
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }


    }
}