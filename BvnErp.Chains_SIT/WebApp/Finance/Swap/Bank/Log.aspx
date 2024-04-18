<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="WebApp.Finance.Swap.Bank.Log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
<uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {

            $('#datagrid').myDatagrid({
                //autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: true, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                nowrap: false,
                fit: true,
                scrollbarSize: 0,
                singleSelect: true,
            });
        });

    </script>
</head>

<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false" style="margin:5px;">
        <table id="datagrid" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar" style="width: 90%; height: auto">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 20%;">操作时间</th>
                    <th data-options="field:'Admin',align:'left'" style="width: 20%;">操作人员</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 55%;">备注</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

