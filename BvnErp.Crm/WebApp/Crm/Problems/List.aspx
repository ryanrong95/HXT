<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Problems.List" ValidateRequest="false" %>

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
            var StandardID = getQueryString("StandardID");
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?StandardID=" + StandardID;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '问题新增',
                width: "880px",
                height: "500px",
                url: url,
                onClose: function () {
                    $('#datagrid').bvgrid('reload');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            var StandardID = getQueryString("StandardID");
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&&StandardID=" + StandardID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '问题编辑',
                    width: "820px",
                    height: "500px",
                    url: url,
                    onClose: function () {
                        $('#datagrid').bvgrid('reload');
                    }
                }).open();
            }
        }

        //查看
        function Show(Index) {
            var StandardID = getQueryString("StandardID");
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Show.aspx') + "?ID=" + rowdata.ID + "&&StandardID=" + StandardID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '问题查看',
                    width: "820px",
                    height: "500px",
                    url: url,
                    onClose: function () {
                        $('#datagrid').bvgrid('reload');
                    }
                }).open();
            }
        }

        //删除
        function Delete(id) {
            var StandardID = getQueryString("StandardID");
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id, StandardID: StandardID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').bvgrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnShow" onclick="Show(\'' + index + '\')">查看</button>';
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
    <div title="问题" id="hh"  data-options="region:'north',border:false" style="height: 60px">
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
        <table id="datagrid" title="问题列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Context" data-options="align:'center',formatter:formatCellTooltip" style="width: 100px;">内容</th>
                    <th field="Answer" data-options="align:'center',formatter:formatCellTooltip" style="width: 100px">回答</th>
                    <th field="ContactName" data-options="align:'center'" style="width: 50px;">联系人</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 50px;">解答人</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

