<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowChart.aspx.cs" Inherits="WebApp.Finance.CostApply.FlowChart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>审批流程</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var CostApplyID = '<%=this.Model.CostApplyID%>';

        $(function () {
            ShowFlow();
        });

        function ShowFlow() {
            $.post(location.pathname + '?action=CostApplyLogs', {
                CostApplyID: CostApplyID,
            }, function (res) {
                var result = JSON.parse(res);

                $.each(result.rows, function (index, row) {
                    var $divNode = $('<div>');
                    $divNode.css('border', '1px solid black');
                    $divNode.css('width', '170px');
                    $divNode.css('margin-left', '0px');
                    $divNode.css('margin-top', '8px');
                    $divNode.css('border-radius', '5px');
                    $divNode.css('padding', '2px');
                    $divNode.css('background-color', row.Color);

                    var str = '';
                    if (row.IsCurrent) {
                        str += '<a name="current"></a>';
                    }
                    str += '<div><span>' + row.Title + '</span><span style="float: right;">' + row.Operation + '</span></div>';
                    str += '<div><span>' + row.Time + '</span></div>';
                    str += '<div><a name="summary" title="' + row.Summary + '" class="easyui-tooltip" href="javascript:void(0)" style="cursor: default;">' + row.Summary + '</a></div>';

                    $divNode.html(str);
                    $divNode.appendTo('#content');

                    if (index < result.rows.length - 1) {
                        var $divJiantou = $('<div>');
                        $divJiantou.css('border', '1px solid white');
                        $divJiantou.css('width', '80px');
                        $divJiantou.css('margin-left', '0px');
                        $divJiantou.css('margin-top', '8px');
                        $divJiantou.css('padding-left', '95px');
                        $divJiantou.html('<div class="to_bottom"></div>');

                        $divJiantou.appendTo('#content');
                    }
                });

                location.href = "#current";
            });
        }
    </script>
    <style>
        .to_bottom {
            width: 0;
            height: 0;
            border-top: 20px solid #c1c1c1;
            border-left: 30px solid transparent;
            border-right: 30px solid transparent;
        }

        #content a[name="summary"] {
            display: inline-block;
            white-space: nowrap; 
            width: 100%; 
            overflow: hidden;
            text-overflow:ellipsis;
        }
    </style>
</head>
<body>
    <div id="content">
    </div>
</body>
</html>
