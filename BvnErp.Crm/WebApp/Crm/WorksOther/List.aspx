<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.WorksOther.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM工作管理';
        gvSettings.menu = '我的工作计划';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        var adminData = eval('(<%=this.Model.AdminData%>)');

        //页面加载
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (!!row.StartDate) {
                            if (isNaN(Date(row.StartDate))) {
                                row.StartDate = row.StartDate.replace(/T/, ' ');
                            } else {
                                row.StartDate = new Date(row.StartDate).toDateStr();
                            }
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
                noheader: false,
                title: '我的工作计划新增',
                url: url,
                IsEsc: false,
                onBeforeClose: function () {
                    var iframes = top.document.getElementsByTagName("iframe");
                    var addwindow = iframes[iframes.length - 1].contentWindow;
                    if (addwindow.Istempsave) {
                        addwindow.win.window('open');
                        return false;
                    }
                },
                onClose: function () {
                    CloseLoad();
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
                    noheader: false,
                    title: '我的工作计划编辑',
                    url: url,
                    IsEsc: false,
                    onBeforeClose: function () {
                        var iframes = top.document.getElementsByTagName("iframe");
                        var addwindow = iframes[iframes.length - 1].contentWindow;
                        if (addwindow.Istempsave) {
                            addwindow.win.window('open');
                            return false;
                        }
                    },
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //查看
        function Show(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Show.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '工作计划查看',
                    url: url,
                    onClose: function () {
                        CloseLoad();
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
                        CloseLoad();
                    })
                }
            });
        }

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.isShow) {
                buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
                buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            }
            buttons += '<button id="btnShow" onclick="Show(' + index + ')">查看</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var Name = $("#Admin").combobox("getValue");
            $('#datagrid').bvgrid('flush', { Name: Name });
        }

        //查询
        function Search() {
            var Name = $("#Admin").combobox("getValue");
            $('#datagrid').bvgrid('search', { Name: Name });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="我的工作计划" data-options="region:'north',border:false" style="height: 100px">
        <!--搜索按钮-->
        <table id="table1">
            <tr>
                <td class="lbl">计划人</td>
                <td>
                    <input class="easyui-combobox" id="Admin" style="width: 95%" data-options="valueField:'ID',textField:'RealName',data: adminData" />
                </td>
            </tr>
        </table>
        <table class="liebiao">
            <tr>
                <td>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="我的工作计划列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 50px">操作</th>
                    <th field="StartDate" data-options="align:'center'" style="width: 50px">开始时间</th>
                    <th field="Subject" data-options="align:'center'" style="width: 50px">计划主题</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 50px">计划人</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px;">状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
