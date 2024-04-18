<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SysConfig.Referrer.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>引荐人</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({
                singleSelect: true, fit: true, scrollbarSize: 0, border: false,
                fitcolumns: true,
                toolbar: '#topBar',
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

            var buttons = '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }


        function Search() {
            var Name = $("#Name").val();
            $('#datagrid').myDatagrid('search', { Name: Name });
        }

        function Reset() {
            $("#Name").textbox("setValue", "");
            Search();
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                debugger;
                if (success) {
                    $.post('?action=DeleteReferrer', { ID: id }, function (res) {
                        debugger
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('提示', '删除成功', 'info', function () {
                                $('#datagrid').datagrid('reload');
                            });
                        } else {
                            $.messager.alert('错误', result.message, 'error');
                            return;
                        }
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增引荐人',
                width: '650px',
                height: '450px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
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
                    <span class="lbl">引荐人名称: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="引荐人列表">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 20%">引荐人</th>
                    <th data-options="field:'CreatorName',align:'center'" style="width: 20%;">添加人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 20%">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 20%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
