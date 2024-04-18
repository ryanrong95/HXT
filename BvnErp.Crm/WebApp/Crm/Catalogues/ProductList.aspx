<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductList.aspx.cs" Inherits="WebApp.Crm.Catalogues.ProductList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        //页面加载时
        $(function () {
            $('#datagrid').bvgrid({
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

        //重置
        function Reset() {
            $("#table1").form('clear');
            $('#datagrid').datagrid('reload');
        }

        //新增
        function Add() {
            var cataloguesID = getQueryString("ID");
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?CatalogueID=" + cataloguesID;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '产品类目编辑',
                width: '800px',
                height: '200px',
                onClose: function () {
                    $('#datagrid').bvgrid('reload');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            var cataloguesID = getQueryString("ID");
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&&CatalogueID=" + cataloguesID;
                top.$.myWindow({
                    url: url,
                    iconCls: "",
                    noheader: false,
                    title: '产品类目编辑',
                    width: '800px',
                    height: '200px',
                    onClose: function () {
                        $('#datagrid').bvgrid('reload');
                    }
                }).open();
            }
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').bvgrid('reload');
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
    <div data-options="region:'north',border:false" style="height: 80px">
        <div>
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="ProductName" data-options="align:'center'" style="width: 100px">产品型号</th>
                    <th field="Amount" data-options="align:'center'" style="width: 100px">交易数量</th>
                    <th field="CurrencyName" data-options="align:'center'" style="width: 100px">币种</th>
                    <th field="UnitPrice" data-options="align:'center'" style="width: 100px">单价</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px">送样状态</th>
                    <th field="Count" data-options="align:'center'" style="width: 100px">送样数量</th>
                    <th field="btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
