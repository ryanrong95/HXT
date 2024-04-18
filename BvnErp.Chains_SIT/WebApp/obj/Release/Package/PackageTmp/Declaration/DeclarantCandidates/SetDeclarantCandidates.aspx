<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetDeclarantCandidates.aspx.cs" Inherits="WebApp.Declaration.Declare.SetDeclarantCandidates" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单信息</title>
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
            AddTab('candidates', '设置默认口岸', './SetDefaultPort.aspx', 'SetDefaultPort', 750);
            AddTab('candidates', '设置候选制单员', './SetCandidateDeclareCreator.aspx', 'SetCandidateDeclareCreator', 750);
            AddTab('candidates', '设置候选录入及申报员', './SetCandidateCustomSubmiter.aspx', 'SetCandidateCustomSubmiter', 750);
            AddTab('candidates', '设置候选报关单复核员', './SetCandidateDoubleChecker.aspx', 'SetCandidateDoubleChecker', 750);
            AddTab('candidates', '设置候选舱单复核员', './SetCandidateMainfestDoubleChecker.aspx', 'SetCandidateMainfestDoubleChecker', 750);
            AddTab('candidates', '设置核对人', './SetChecker.aspx', 'SetChecker', 750);
            AddTab('candidates', '调整制单员', './ModifyDeclareCreator.aspx', 'ModifyDeclareCreator', 750);
            AddTab('candidates', '调整录入及申报员', './ModifyCustomSubmiter.aspx', 'ModifyCustomSubmiter', 750);
            AddTab('candidates', '调整报关单复核员', './ModifyDoubleChecker.aspx', 'ModifyDoubleChecker', 750);
            AddTab('candidates', '调整舱单复核员', './ModifyManifestDoubleChecker.aspx', 'ModifyManifestDoubleChecker', 750);
        });
    </script>
</head>
<body>
    <div id="candidates" class="easyui-tabs">
    </div>
</body>
</html>
