<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecTrace.aspx.cs" Inherits="WebApp.Declaration.Declare.DecTrace" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {

            $('#orders').myDatagrid({
                singleSelect: false,
                //autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: true, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                nowrap: false,
            });
        });

    </script>
</head>

<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false" style="margin:5px;">
        <table id="orders" title="操作记录" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar" style="width: 100%; height: auto">
            <thead>
                <tr>
                    <th data-options="field:'ContrNO',align:'center'" style="width: 15%;">合同号</th>
                    <th data-options="field:'NoticeDate',align:'center'" style="width: 15%;">回执时间</th>
                    <th data-options="field:'Channel',align:'left'" style="width: 15%;">操作内容</th>
                    <th data-options="field:'Message',align:'left'" style="width: 55%;">备注</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
