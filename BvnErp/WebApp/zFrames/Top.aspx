<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Top.aspx.cs" Inherits="WebApp.zFrames.Top" %>

<!doctype html>
<html>
<head runat="server">
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        * { margin: 0px; padding: 0px; }
        html, body { width: 100%; height: 100%; }
    </style>
    <script>
        $(function () {
            $('#changePassword').click(function () {
                top.document.getElementById('ifrmain').src = '../zframes/ChangePassWord.aspx';
                return false;
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 100%; height: 100%;">
            <tbody>
                <tr>
                    <td style="width: 120px;"></td>
                    <td style="vertical-align: bottom; text-align: right;">欢迎 &nbsp; &nbsp; <%:Needs.Erp.ErpPlot.Current.UserName%> &nbsp; &nbsp; 登录 
                        &nbsp; | &nbsp; <a id="changePassword" href="ChangePassWord.aspx">修改密码</a>
                        &nbsp; | &nbsp; <a id="logout" href="../Logout.aspx">退出</a>
                    </td>
                </tr>
            </tbody>
        </table>
    </form>
</body>
</html>
