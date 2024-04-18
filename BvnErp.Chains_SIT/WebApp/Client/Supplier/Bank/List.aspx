<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Supplier.Bank.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';

        //数据初始化
        $(function () {
            //下拉框数据初始化

            //订单列表初始化
            $('#accounts').myDatagrid({
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

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?SupplierID=' + ID + '&SupplierBankID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑银行账户',
                width: '750px',
                height: '450px',
                onClose: function () {
                    $('#accounts').datagrid('reload');
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除银行账户！', function (success) {
                if (success) {
                    $.post('?action=DeleteClientSupplierAccount', { ID: id }, function () {
                        $.messager.alert('消息', '删除成功！');
                        $('#accounts').myDatagrid('reload');
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?SupplierID=' + ID + '&SupplierBankID=');;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增银行账户',
                width: '750px',
                height: '450px',
                onClose: function () {
                    $('#accounts').datagrid('reload');
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div title="" class="easyui-panel" data-options="region:'north',border:false" style="height: 50px">
        <div style="margin: 10px;">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="width: 100%; height: 100%;">
        <table id="accounts" data-options="nowrap:false,border:true,fit:true,scrollbarSize:0" title="银行账户">
            <thead>
                <tr>
                    <th data-options="field:'BankName',align:'left'" style="width: 15%;">银行名称</th>
                    <th data-options="field:'SwiftCode',align:'left'" style="width: 10%;">银行代码</th>
                    <th data-options="field:'Account',align:'left'" style="width: 12%;">银行账户</th>
                    <th data-options="field:'BankAddress',align:'left'" style="width: 15%;">银行地址</th>
                    <th data-options="field:'Status',align:'center'" style="width: 5%;">状态</th>
                    <th data-options="field:'Place',align:'center'" style="width: 8%;">国家/地区</th>
                    <th data-options="field:'Methord',align:'center'" style="width:7%;">汇款方式</th>
                  <%--  <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>--%>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
