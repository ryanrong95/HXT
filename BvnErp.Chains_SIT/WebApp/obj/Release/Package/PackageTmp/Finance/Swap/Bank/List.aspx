<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Swap.Bank.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%-- <script>
        gvSettings.fatherMenu = '银行账户管理';
        gvSettings.menu = '换汇银行';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
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
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetBlackList(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">设置黑名单国家</span>' +
                '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ReceiptHead(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">日志</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }


        ////删除
        //function Delete(id) {
        //    $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
        //        if (success) {
        //            $.post('?action=Delete', { ID: id }, function () {
        //                $.messager.alert('消息', '删除银行成功！');
        //                $('#datagrid').myDatagrid('reload');
        //            })
        //        }
        //    });
        //}

        //编辑
        function Edit(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '编辑银行',
                    width: '400',
                    height: '300',
                    url: url,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        //设置黑名单国家
        function SetBlackList(id) {
            var url = location.pathname.replace(/List.aspx/ig, '/BlackList.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '设置黑名单国家',
                width: '1000px',
                height: '550px'
            });
        }


        //查看日志
        function ReceiptHead(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Log.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '日志信息',
                width: '1000px',
                height: '550px'
            });
        }

        //新增银行
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增银行',
                width: '500',
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
        </div>
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
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fit:true,scrollbarSize:0,border:false" title="换汇银行" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 80px;">名称</th>
                    <th data-options="field:'Code',align:'left'" style="width: 100px;">系统代码</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 100px;">摘要</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 80px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
