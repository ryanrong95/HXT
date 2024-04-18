<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SysConfig.CustomsQuarantine.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '检疫地区';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({
                singleSelect:true,fit:true,scrollbarSize:0,border:false,
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

        //查询
        function Search() {
            var Origin = $('#Origin').textbox('getValue');
            $('#datagrid').myDatagrid('search', { Origin: Origin });
        }

        //重置查询条件
        function Reset() {
            $('#Origin').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        //编辑地区
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '编辑地区',
                width: '510',
                height: '410',
                url: url,
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //删除地区
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除成功！');
                        $('#datagrid').myDatagrid('reload');
                    })
                }
            });
        }

        //新增地区
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增地区',
                width: '510',
                height: '410',
                url: url,
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">产地: </span>
                    <input class="easyui-textbox search" id="Origin" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid"  title="地区" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Origin',align:'center'" style="width: 80px;">需要检疫的产地</th>
                    <th data-options="field:'StartDate',align:'left'" style="width: 100px;">检疫起始日期</th>
                    <th data-options="field:'EndDate',align:'left'" style="width: 100px;">检疫结束日期</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">创建时间</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 100px;">摘要（备注）</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 80px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
