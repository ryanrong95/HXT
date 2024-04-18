<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApp.tests.WebForm1" %>

<%@ Import Namespace="NtErp.Crm.Services.Enums" %>
<%@ Import Namespace="Needs.Utils.Descriptions" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%
                var value = JobType.PME | JobType.Sales;
                var values = new[] { JobType.PME, JobType.Sales, value };

                foreach (var item in values)
                {
                    Response.Write((int)item);
                    Response.Write("<br />");
                }

                var k = value.GetDescription();

                Response.Write(k);

                //Response.Write(value.GetDescription());

            %>
        </div>
    </form>
</body>
</html>
