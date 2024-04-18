<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Charges.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">

        //页面加载时
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

        //重置
        function Reset() {
            $("#table1").form('clear');
            $("#datagrid").datagrid("reload");
        }

        //新增
        function Add() {
            var clientid = getQueryString('ClientID');
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + clientid;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '费用编辑',
                width: '800px',
                height: '500px',
                onClose: function () {
                    $("#datagrid").datagrid("reload");
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '费用编辑',
                    width: '800px',
                    height: '500px',
                    onClose: function () {
                        $("#datagrid").datagrid("reload");
                    }
                }).open();
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            return buttons;
        }

        //格式化单元格提示信息  
        function formatCellTooltip(value) {
            return "<span title='" + value + "'>" + value + "</span>";
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="客户费用" data-options="region:'north',border:false" style="height: 55px">

        <div>
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="ID" data-options="align:'center'" style="width: 50px">ID</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 100px">客户名称</th>
                    <th field="Name" data-options="align:'center'" style="width: 50px">费用名称</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 50px">填写人</th>
                    <th field="Count" data-options="align:'center'" style="width: 50px">件数</th>
                    <th field="Price" data-options="align:'center'" style="width: 50px">价格</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 50px">日期</th>
                    <th field="Summary" data-options="align:'center',formatter:formatCellTooltip" style="width: 250px">描述</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
