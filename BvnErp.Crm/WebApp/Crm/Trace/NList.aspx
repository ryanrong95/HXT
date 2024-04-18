<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NList.aspx.cs" Inherits="WebApp.Crm.Trace.NList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '客户跟踪记录';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        var TypeData = eval('(<%=this.Model.TypeData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var FollowerData = eval('(<%=this.Model.FollowerData%>)');
        //页面加载
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (!!row.Date) {
                            if (isNaN(Date(row.Date))) {
                                row.Date = row.Date.replace(/T.+$/, '');
                            } else {
                                row.Date = new Date(row.Date).toDateStr();
                            }
                        }
                        delete row.item;
                    }
                    return data;
                },
                scrollbarSize: 0
            });
        });

         //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/NList.aspx/ig, 'NAdd.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '跟踪记录新增',
                width: '900px',
                height: '500px',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //查看
        function Show(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            var url = location.pathname.replace(/NList.aspx/ig, 'List.aspx') + "?isadd=false&&ClientID=" + rowdata.ClientID + "&&IsRead=" + rowdata.IsRead;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '跟踪记录查看',
                url: url,
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnShow" onclick="Show(' + index + ' )">查看</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var ClientName = $("#Client").combobox("getValue");
            var AdminName = $("#Admin").combobox("getValue");
            var Type = $("#Type").combobox("getValue");
            $('#datagrid').bvgrid('flush', { ClientName: ClientName, AdminName: AdminName, Type: Type });
        }

        //查询
        function Search() {
            var ClientName = $("#Client").combobox("getValue");
            var AdminName = $("#Admin").combobox("getValue");
            var Type = $("#Type").combobox("getValue");
            $('#datagrid').bvgrid('search', { ClientName: ClientName, AdminName: AdminName, Type: Type });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="跟踪记录" data-options="region:'north',border:false" style="height: 100px">
        <table id="table1" style="width: 100%; margin-top: 5px" cellpadding="0" cellspacing="0">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr style="height: 25px">
                <td class="lbl">客户名称</td>
                <td>
                    <input class="easyui-combobox" id="Client" style="width: 90%"
                        data-options="valueField:'ID',textField:'Name',data: ClientData" />
                </td>
                <td class="lbl">跟进人</td>
                <td>
                    <input class="easyui-combobox" id="Admin" style="width: 90%"
                        data-options="valueField:'ID',textField:'RealName',data: FollowerData" />
                </td>
                <td class="lbl">跟进方式</td>
                <td>
                    <input class="easyui-combobox" id="Type" style="width: 90%"
                        data-options="valueField:'value',textField:'text',data: TypeData,panelMaxHeight:'100px'" />
                </td>
            </tr>
        </table>
        <!--搜索按钮-->
        <div style="margin-top: 5px">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="跟踪记录列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Date" data-options="align:'center'" style="width: 100px;">跟进日期</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 100px">客户名称</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 50px">跟进人</th>
                    <th field="TypeName" data-options="align:'center'" style="width: 100px;">跟进方式</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 100px;">更新时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
