<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Supplier.Address.List" %>

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
            $('#address').myDatagrid({
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
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?SupplierID=' + ID + '&SupplierAddressID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑提货地址',
                width: '750px',
                height: '400px',
                onClose: function () {
                    $('#address').datagrid('reload');
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除提货地址！', function (success) {
                if (success) {
                    $.post('?action=DeleteClientSupplierAddress', { ID: id }, function () {
                        $.messager.alert('消息', '删除成功！');
                        $('#address').myDatagrid('reload');
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?SupplierID=' + ID + '&SupplierAddressID=');;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增提货地址',
                width: '750px',
                height: '400px',
                onClose: function () {
                    $('#address').datagrid('reload');
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
    <div data-options="region:'center',border:false" style="margin-left: 8px; margin: 5px; padding-right: 10px; margin-bottom: 15px; padding-bottom: 10px;">
        <table id="address" data-options="fit:true,nowrap:false,scrollbarSize:0" title="提货地址">
            <thead>
                <tr>
                    <th data-options="field:'ContactName',align:'left'" style="width: 10%;">联系人</th>
                    <th data-options="field:'Mobile',align:'left'" style="width: 12%;">联系电话</th>
                    <th data-options="field:'Address',align:'left'" style="width: 20%;">提货地址</th>
                    <th data-options="field:'Status',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'IsDefault',align:'center'" style="width: 10%;">是否默认</th>
                    <th data-options="field:'Place',align:'center'" style="width: 10%;">国家/地区</th>
                    <th data-options="field:'Summary',align:'center'" style="width: 15%;">备注</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
