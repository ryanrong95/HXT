<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentOperatorLists.aspx.cs" Inherits="WebApp.Finance.Payment.PaymentOperators.PaymentOperatorLists" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {

            $('#datagrid').myDatagrid({
                border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                rownumbers: true,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

        });

        //添加窗口
        function AddWindow() {
            var url = location.pathname.replace(/PaymentOperatorLists.aspx/ig, 'Add.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '550px',
                height: '300px',
                title: '新增',
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //删除
        function Delete(financePaymentOperatorID, adminName ) {
            $.messager.confirm('确认', '是否删除 ' + adminName + ' ？', function (success) {
                if (success) {
                    $.post('?action=Delete', { FinancePaymentOperatorID: financePaymentOperatorID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }

        function Operation(val, row, index) {
            var buttons = '';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\''
                + row.FinancePaymentOperatorID + '\',\''
                + row.AdminName + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition">
                <tr>
                    <td>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddWindow()">添加</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="设置付款操作人" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'AdminName',align:'left'" style="width: 10%;">付款操作人</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 10%;">创建时间</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 30px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
