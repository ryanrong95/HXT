<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderLog.aspx.cs" Inherits="WebApp.Order.Query.OrderLog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {
            //代理订单列表初始化
            $('#datagraid').myDatagrid({
                nowrap: false,
                pagination: true,
                fitColumns: true,
                fit: true,
                toolbar: '#topBar',
            });
        });
        function Close() {
            $.myWindow.close();
        };

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            switch (from) {
                case 'MerchandiserQuery':
                    url = location.pathname.replace(/OrderLog.aspx/ig, '../Query/List.aspx');
                    break;
                case 'SalesQuery':
                    url = location.pathname.replace(/OrderLog.aspx/ig, '../Query/SalesList.aspx');
                    break;
                case 'AdminQuery':
                    url = location.pathname.replace(/OrderLog.aspx/ig, '../Query/AdminList.aspx');
                    break;
                case 'InsideQuery':
                    url = location.pathname.replace(/OrderLog.aspx/ig, '../Query/InsideList.aspx');
                    break;
                case 'DeclareOrderQuery':
                    url = location.pathname.replace(/OrderLog.aspx/ig, '../Query/DeclareOrderList.aspx');
                    break;
                default:
                    url = location.pathname.replace(/OrderLog.aspx/ig, '../Query/List.aspx');
                    break;
            }
            window.parent.location = url;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
        </div>
    </div>
    <div class="easyui-panel" data-options="region:'center',border:false,fit:true" style="padding: 5px">
        <table id="datagraid" title="订单日志">
            <thead>
                <tr>
                    <%--                    <th data-options="field:'People',align:'center'" style="width: 40px;">操作人</th>
                    <th data-options="field:'Orderstatus',align:'center'" style="width: 50px;">订单状态</th>--%>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 260px;">时间</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 380px;">内容</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
