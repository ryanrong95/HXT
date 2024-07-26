<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>华芯通首页</title>
</head>
<body>
    <%
        string id = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
        var adminWl = Needs.Wl.Admin.Plat.AdminPlat.Admins[id];
        if (adminWl == null)
        {
            var admin = new Needs.Ccs.Services.Models.Admin();
            admin.ID = id;
            admin.Summary = string.Empty;
            admin.Enter();
        }
    %>
</body>
</html>

