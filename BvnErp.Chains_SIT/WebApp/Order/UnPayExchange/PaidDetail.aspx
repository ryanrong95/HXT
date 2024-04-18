<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaidDetail.aspx.cs" Inherits="WebApp.Order.UnPayExchange.PaidDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {

            $('#paiddetails').myDatagrid({
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
        <table id="paiddetails" title="付汇记录" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar" style="width: 100%; height: auto">
            <thead>
                <tr>
                    <th data-options="field:'SupplierName',align:'left'" style="width: 35%;">供应商名称</th>
                    <th data-options="field:'ApplyTime',align:'center'" style="width: 15%;">申请时间</th>
                    <th data-options="field:'Applier',align:'left'" style="width: 15%;">申请人</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 10%;">付汇金额</th>
                    <th data-options="field:'Status',align:'left'" style="width: 10%;">状态</th>
                    <th data-options="field:'FatherID',align:'left'" style="width: 10%;">付汇类型</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
