<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WaybillList.aspx.cs" Inherits="WebApp.HKWarehouse.Sorting.WaybillList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>运单列表</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
</head>
<script>
    var EntryNoticeStatus = getQueryString('EntryNoticeStatus');
    $(function () {
        $('#wayBillGrid').myDatagrid({
            fitColumns: true,
            rownumbers: false,
            scrollbarSize: 0,
            fit: true,
            border: false,
            nowrap: false,
            pagination: false,
            toolbar: '#tb',
            loadFilter: function (data) {
                for (var index = 0; index < data.rows.length; index++) {
                    var row = data.rows[index];
                    for (var name in row.item) {
                        row[name] = row.item[name];
                    }
                    delete row.item;
                }
                return data;
            },
            nowrap: false
        });
    });

    //国际快递操作
    function ExpressOperate(val, row, index) {
        var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton" style="margin:3px;color:#0094ff" onclick="DeleteWayBill(\'' + row.ID + '\')">删除</a>';
        return buttons;
    }
    //删除国际快递
    function DeleteWayBill(id) {
        if (EntryNoticeStatus == 3) {
            $.messager.alert("提示", "已封箱不能删除运单");
            return;
        }
        $.messager.confirm('确认', '是否删除所选数据！', function (success) {
            if (success) {
                $.post('?action=DeleteWayBill', { ID: id }, function () {
                    $.messager.alert('消息', '删除成功！');
                    $('#wayBillGrid').myDatagrid('reload');
                })
            }
        });
    }

</script>
<body class="easyui-layout">
    <table id="wayBillGrid" class="easyui-datagrid" data-options="
                    fitColumns:true,
                    rownumbers:false,
                    scrollbarSize:0,
                    fit:true,      
                    border:false,     
                    nowrap: false,
                    pagination:false,
                    toolbar:'#tb'">
        <thead>
            <tr>
                <th field="CompanyName" data-options="align:'left'" style="width: 50px">快递公司</th>
                <th field="WaybillCode" data-options="align:'left'" style="width: 60px">运单编号</th>
                <th field="ArrivalDate" data-options="align:'left'" style="width: 30px">到港日期</th>
                <th data-options="field:'btnOpt',width:50,formatter:ExpressOperate,align:'center'">操作</th>
            </tr>
        </thead>
    </table>
</body>
</html>
