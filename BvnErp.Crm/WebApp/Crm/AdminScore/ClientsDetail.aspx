<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientsDetail.aspx.cs" Inherits="WebApp.Crm.AdminScore.ClientsDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var Admins = eval(<%=this.Model.Admins%>);
        var year = eval(<%=this.Model.Year%>);
        var month = eval(<%=this.Model.Month%>);

        //页面加载数据
        $(function () {
            var date = new Date();
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1).toString() : (date.getMonth() + 1).toString();
            $("#Year").combobox("setValue", date.getFullYear().toString());
            $("#Month").combobox("setValue", month);

            $('#datagrid').bvgrid({
                pageSize: 20,
                queryParams: { action: 'data', Year: date.getFullYear().toString(), Month: month },
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

        //导出
        function Export() {
            var query = {};
            query.AdminID = $("#AdminID").combobox("getValue");
            query.Year = $("#Year").combobox("getValue");
            query.Month = $("#Month").combobox("getValue");
            $.post('?action=Export', query, function (data) {
                window.location.href = data;
                //$.messager.alert('提示', '导出成功！');
            })
        }

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //查询
        function Search() {
            var AdminID = $("#AdminID").combobox("getValue");
            var Year = $("#Year").combobox("getValue");
            var Month = $("#Month").combobox("getValue");
            $('#datagrid').bvgrid('search', { AdminID: AdminID, Year: Year, Month: Month });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="查询列表" data-options="region:'north',border:false" style="height: 80px; margin-top: 5px;">
        <table id="table1" cellpadding="0" cellspacing="0" style="width:100%">
            <tr>
                <th style="width: 8%"></th>
                <th style="width: 15%"></th>
                <th style="width: 8%"></th>
                <th style="width: 15%"></th>
                <th style="width: 8%"></th>
                <th style="width: 15%"></th>
                <th style="width: 8%"></th>
                <th style="width: 15%"></th>
            </tr>
            <tr>
                <td class="lbl">用户名</td>
                <td>
                    <input class="easyui-combobox" id="AdminID" style="width: 95%"
                        data-options="valueField:'ID',textField:'RealName',data: Admins" />
                </td>
                <td class="lbl">年份</td>
                <td>
                    <input class="easyui-combobox" id="Year" name="Year"
                        data-options="valueField:'value',textField:'text',data: year" style="width: 95%" />
                </td>
                <td class="lbl">月份</td>
                <td>
                    <input class="easyui-combobox" id="Month" name="Month"
                        data-options="valueField:'value',textField:'text',data: month" style="width: 95%" />
                </td>
            </tr>
        </table>
        <div id="toolbar">
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
            <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-excel'" onclick="Export()">导出</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="我的客户列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="ID" data-options="align:'center'" style="width: 50px">客户ID</th>
                    <th field="Name" data-options="align:'center'" style="width: 100px">客户名称</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 50px;">状态</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px;">创建时间</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 100px;">更新时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
