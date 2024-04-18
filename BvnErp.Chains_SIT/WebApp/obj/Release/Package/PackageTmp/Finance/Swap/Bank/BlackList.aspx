<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlackList.aspx.cs" Inherits="WebApp.Finance.Swap.LimitCountry.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var IDdate = '<%=this.Model.IDdate%>';
        //数据初始化
        $(function () {
            //银行列表初始化
            $('#datagrid').myDatagrid({
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
            var name = $('#Name').textbox('getValue');
            $('#datagrid').myDatagrid('search', { Name: name });
        }

        //重置查询条件
        function Reset() {
            $('#Name').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }


        //删除限制
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

        //新增
        function Add() {
            var url = location.pathname.replace(/BlackList.aspx/ig, 'SetLimit.aspx') + "?ID=" + IDdate;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增',
                width: '420',
                height: '300',
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
            <div id="search">
                <ul>
                    <li>
                        <span class="lbl">名称: </span>
                        <input class="easyui-textbox search" id="Name" />
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </li>
                </ul>
            </div>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fit:true,scrollbarSize:0,border:false" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 100px;">国家名称</th>
                    <th data-options="field:'Code',align:'left'" style="width: 80px;">代码</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 100px;">摘要</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 80px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
