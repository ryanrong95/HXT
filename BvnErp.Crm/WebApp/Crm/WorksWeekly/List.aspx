<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.WorksWeekly.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM工作管理';
        gvSettings.menu = '我的工作周报';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        var adminData = eval('(<%=this.Model.AdminData%>)');

        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        delete row.item;
                    }
                    return data;
                }
            });
            $("#CurWeek").textbox('textbox').bind('keyup', function (e) {
                $("#CurWeek").textbox('setValue', $(this).val().replace(/\D/g, ''));
            });
        });

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '我的工作周报新增',
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
                    title: '我的工作周报编辑',
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
                    title: '工作周报查看',
                    url: url,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
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
                buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            }
            buttons += '<button id="btnShow" onclick="Show(' + index + ')">查看</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var Admin = $("#Admin").combobox("getValue");
            var CurWeek = $("#CurWeek").val();
            $('#datagrid').bvgrid('flush', { Admin: Admin, CurWeek: CurWeek });
        }

        //查询
        function Search() {
            var Admin = $("#Admin").combobox("getValue");
            var CurWeek = $("#CurWeek").val();
            $('#datagrid').bvgrid('search', { Admin: Admin, CurWeek: CurWeek });
        }

    </script>
</head>
<body class="easyui-layout">
    <div title="我的工作周报" data-options="region:'north',border:false" style="height: 100px">
        <!--搜索按钮-->
        <table id="table1">
            <tr>
                <td class="lbl">报告人</td>
                <td>
                    <input class="easyui-combobox" id="Admin" style="width: 90%"
                        data-options="valueField:'ID',textField:'RealName',data:adminData" style="width: 95%" />
                </td>
                <td class="lbl">当前周</td>
                <td>
                    <input class="easyui-textbox" id="CurWeek" name="CurWeek" style="width: 95%" />
                    <input id="hdWeek" type="hidden" />
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
        <table id="datagrid" title="我的工作周报列表" data-options="fitColumns:true,border:false,fit:true, scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px">报告日期</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 50px">报告人</th>
                    <th field="WeekOfYear" data-options="align:'center'" style="width: 100px;">当前周</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px;">状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
