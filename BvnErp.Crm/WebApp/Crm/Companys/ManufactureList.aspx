<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManufactureList.aspx.cs" Inherits="WebApp.Crm.Companys.ManufactureList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>公司列表</title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '品牌管理';
        gvSettings.summary = '';

    </script>
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
            Search();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/ManufactureList.aspx/ig, 'Edit.aspx') + "?Type=M";
            if (true) {
                top.$.myWindow({
                    iconCls: "",
                    width: '600px',
                    height: '200px',
                    url: url,
                    noheader: false,
                    title: '品牌新增',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/ManufactureList.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&Type=M";
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '品牌编辑',
                    url: url,
                    width: '600px',
                    height: '200px',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            // buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var Name = $("#Name").val();
            $('#datagrid').bvgrid('flush', { Name: Name });
        }

        //查询
        function Search() {
            var Name = $("#Name").val();
            $('#datagrid').bvgrid('search', { Name: Name });
        }
        //格式化单元格提示信息  
        function formatCellTooltip(value) {
            return "<span title='" + value + "'>" + value + "</span>";
        }

    </script>
</head>
<body class="easyui-layout">
    <div title="品牌列表" data-options="region:'north',border:false" style="height: 80px">
        <table id="table1" cellpadding="0" cellspacing="0">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 30%"></th>
            </tr>
            <tr>
                <td class="lbl">品牌名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" style="width: 90%" />
                </td>
            </tr>
        </table>
        <div>
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Name" data-options="align:'center',formatter:formatCellTooltip" style="width: 100px">品牌全称</th>
                    <th field="Code" data-options="align:'center'" style="width: 100px">品牌简称</th>
                    <th field="Summary" data-options="align:'center'" style="width: 200px">品牌描述</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
