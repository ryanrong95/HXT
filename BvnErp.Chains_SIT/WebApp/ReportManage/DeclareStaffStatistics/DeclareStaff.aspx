<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeclareStaff.aspx.cs" Inherits="WebApp.ReportManage.DeclareStaffStatistics.DeclareStaff" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>工作量统计</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#candidates').tabs({
                border: false,
                tabWidth: 150,
            });

            //var id = getQueryString('ID');
            //var from = getQueryString('From');

            AddTab('candidates', '统计', './List.aspx', 'List', 750);
            AddTab('candidates', '明细信息', './DetailList.aspx', 'DetailList', 750);
        });
    </script>
</head>
<body>
    <div id="candidates" class="easyui-tabs">
    </div>
</body>
</html>