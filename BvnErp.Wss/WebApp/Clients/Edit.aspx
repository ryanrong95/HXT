<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Clients.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server"></uc:EasyUI>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="用户详情">
        <%
            var model = this.Model as NtErp.Wss.Services.Generic.Models.ClientTop;
        %>
        <table class="liebiao">
            <tr>
                <td style="width: 150px;">ID：</td>
                <td><%=model.ID %></td>
            </tr>
            <tr>
                <td>用户名：</td>
                <td><%=model.UserName %></td>
            </tr>
            <tr>
                <td>邮箱：</td>
                <td><%=model.Email %></td>
            </tr>
            <tr>
                <td>手机：</td>
                <td><%=model.Mobile %></td>
            </tr>
          <%--  <tr>
                <td>创建时间：</td>
                <td><%=model.CreateDate %></td>
            </tr>
            <tr>
                <td>最后一次更新时间：</td>
                <td><%=model.UpdateDate %></td>
            </tr>--%>
            <tr>
                <td colspan="2">
                    <button type="button" onclick="$.myWindow.close()">关闭</button></td>
            </tr>
        </table>
    </div>
</body>
</html>
