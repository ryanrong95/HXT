                                                                                                                                                                     <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Order.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
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
        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '订单编辑',
                width: '800px',
                height: '300px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '订单编辑',
                    width: '800px',
                    height: '300px',
                    onClose: function () {
                        $('#datagrid').datagrid('reload');
                    }
                }).open();
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="hhh" title="订单" data-options="region:'north',border:false " style="height: 60px">

        <!--搜索按钮-->
        <table class="liebiao">

            <tr>
                <td>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="订单列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 100px">客户名称</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 50px">客服人员</th>
                    <th field="CurrencyName" data-options="align:'center'" style="width: 100px;">币种</th>
                    <th field="BeneficiaryName" data-options="align:'center'" style="width: 100px;">收益人</th>
                    <th field="DeliveryAddress" data-options="align:'center'" style="width: 100px">交货地址</th>
                    <th field="Address" data-options="align:'center'" style="width: 50px">收货地址</th>
                    <th field="ConsigneeName" data-options="align:'center'" style="width: 100px;">收货人</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

