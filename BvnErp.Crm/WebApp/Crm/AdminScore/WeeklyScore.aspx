<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WeeklyScore.aspx.cs" Inherits="WebApp.Crm.AdminScore.WeeklyScore" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '当月员工工作考核';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        var Admins = eval(<%=this.Model.Admins%>);
        var JobTypeData = eval(<%=this.Model.JobTypeData%>);
        var year = eval(<%=this.Model.Year%>);
        var month = eval(<%=this.Model.Month%>);

        //数据初始化
        $(function () {
            var date = new Date();
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1).toString() : (date.getMonth() + 1).toString();
            $("#Year").combobox("setValue", date.getFullYear().toString());
            $("#Month").combobox("setValue", month);

            $('#datagrid').bvgrid({
                queryParams: { action: 'data', Year: date.getFullYear().toString(), Month: month },
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

        //导出
        function Export() {
            var query = {};
            query.AdminID = $("#AdminID").combobox("getValue");
            query.JobType = $("#JobType").combobox("getValue");
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

        function CloseLoad() {
            var AdminID = $("#AdminID").combobox("getValue");
            var JobType = $("#JobType").combobox("getValue");
            var Year = $("#Year").combobox("getValue");
            var Month = $("#Month").combobox("getValue");
            $('#datagrid').bvgrid('flush', { AdminID: AdminID, JobType: JobType, Year: Year, Month: Month });
        }

        //查询
        function Search() {
            var AdminID = $("#AdminID").combobox("getValue");
            var JobType = $("#JobType").combobox("getValue");
            var Year = $("#Year").combobox("getValue");
            var Month = $("#Month").combobox("getValue");
            $('#datagrid').bvgrid('search', { AdminID: AdminID, JobType: JobType, Year: Year, Month: Month });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="管理员列表" data-options="region:'north',border:false" style="height: 80px; margin-top: 5px;">
        <table id="table1" cellpadding="0" cellspacing="0">
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
                <td class="lbl">考核角色</td>
                <td>
                    <input class="easyui-combobox" id="JobType" name="JobType"
                        data-options="valueField:'value',textField:'text',data: JobTypeData" style="width: 95%" />
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
        <table id="datagrid" data-options="fit:true" class="mygrid">
            <thead>
                <tr>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px">用户名</th>
                    <th field="ScoreTypeName" data-options="align:'center'" style="width: 100px">考核角色</th>
                    <th field="DistrictName" data-options="align:'center'" style="width: 100px">地区</th>
                    <th field="ReportScore" data-options="align:'center'" style="width: 200px">客户拜访数</th>
                    <th field="ClientScore" data-options="align:'center'" style="width: 200px">新客户数/销售机会数</th>
                    <th field="DIScore" data-options="align:'center'" style="width: 100px">DI个数</th>
                    <th field="DWScore" data-options="align:'center'" style="width: 100px">DW个数</th>
                    <th field="TotalScore" data-options="align:'center'" style="width: 100px">绩效分</th>
                    <th field="Bonus" data-options="align:'center'" style="width: 100px">绩效工资</th>
                    <th field="Year" data-options="align:'center'" style="width: 100px">年份</th>
                    <th field="Month" data-options="align:'center'" style="width: 100px">月份</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
