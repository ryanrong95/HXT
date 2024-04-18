<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CookiesTester.aspx.cs" Inherits="WebAppTester.CookiesTester" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="/javascripts/jquery.min.js"></script>
    <script src="/javascripts/jquery.cookie-1.4.1.min.js"></script>
    <%
        if (isOpen)
        {
    %>
    <script>
        window.open('http://erp8.ic360.cn', 'newwindow', 'height=100, width=400, top=0, left=0, toolbar=no, menubar=no, scrollbars=no,resizable=no,location=no, status=no')
    </script>
    <%
        }
    %>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <a href="//www.cb1b.com/CookiesTester.aspx" target="_blank">cb1b</a><br />
            <a href="//www.bb1b.com/CookiesTester.aspx" target="_blank">bb1b</a><br />
            <script>
                document.write($.cookie('MyCook'));
            </script>
            <asp:Button ID="btnSubmit" runat="server" Text="提交" OnClick="btnSubmit_Click" /><br />
            <iframe src="//www.cb1b.com/CookiesIframe.aspx" style="width: 100%; height: 100px;"></iframe>
        </div>
    </form>
</body>
</html>
