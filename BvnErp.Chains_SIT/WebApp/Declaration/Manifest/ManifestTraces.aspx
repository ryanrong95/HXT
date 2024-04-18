<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManifestTraces.aspx.cs" Inherits="WebApp.Declaration.Manifest.DecTrace" %>

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
                fitColumns: true,
                fit: false,
                nowrap: false,
                singleSelect: true,
                pagination: false,
            });
        });
        function Close() {
            $.myWindow.close();
        };
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <div>
                <div id="data" data-options="region:'center',border:false">
                    <table id="datagraid" title="" data-options="fitColumns:true,fit:false,nowrap:false,singleSelect:true,pagination:false" style="width: 100%; height: 450px">
                        <thead>
                            <tr>
                                <th data-options="field:'NoticeDate',align:'center'" style="width: 100px;">回执时间</th>
                                <th data-options="field:'BillNo',align:'center'" style="width: 100px;">提(运)单号</th>
                                <th data-options="field:'StatementCode',align:'left'" style="width: 100px;">处理结果</th>
                                <th data-options="field:'Message',align:'left'" style="width: 380px;">海关备注</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">关闭</a>
    </div>
</body>
</html>
