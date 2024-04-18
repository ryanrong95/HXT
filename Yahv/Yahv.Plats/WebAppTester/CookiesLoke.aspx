<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="/javascripts/jquery.min.js"></script>
    <script src="/javascripts/jquery.cookie-1.4.1.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <script>
                document.write($.cookie('MyCook'));
            </script>
        </div>
    </form>
</body>
</html>
