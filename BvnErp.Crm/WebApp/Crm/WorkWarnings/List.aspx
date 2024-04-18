<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.WorkWarnings.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM工作管理';
        gvSettings.menu = '我的工作提醒';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        var Status = eval('(<%=this.Model.Status%>)');

        //初始化加载数据
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

        //超链接
        function View(val, row, index) {
            var buttons = '<a href="javascript:void(0);" style="color:blue" onclick="Main(' + index + ')">' + row.MainID + '</a>';
            return buttons;
        };

        //查看页面
        function Main(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = "";
                var title = "";
                if (rowdata.Type == "80") {
                    url = location.pathname.replace(/WorkWarnings/ig, 'MyClients');
                    url = url.replace(/List.aspx/ig, 'Show.aspx') + "?ID=" + rowdata.MainID;
                    title = "一周内进入公海客户信息查看";
                }
                //工作计划
                if (rowdata.Type == "50") {
                    url = location.pathname.replace(/WorkWarnings/ig, 'WorksOther');
                    url = url.replace(/List.aspx/ig, 'Show.aspx') + "?id=" + rowdata.MainID;
                    title = "工作计划点评查看";
                }
                //工作周报
                if (rowdata.Type == "40") {
                    url = location.pathname.replace(/WorkWarnings/ig, 'WorksWeekly');
                    url = url.replace(/List.aspx/ig, 'Show.aspx') + "?id=" + rowdata.MainID;
                    title = "工作周报点评查看";
                }
                //客户追踪记录
                if (rowdata.Type == "30" || rowdata.Type == "60" || rowdata.Type == "90" || rowdata.Type == "100") {
                    url = location.pathname.replace(/WorkWarnings/ig, 'Trace');
                    url = url.replace(/List.aspx/ig, 'ShowList.aspx') + "?id=" + rowdata.MainID;
                    title = "客户跟踪记录查看";
                }
                //销售机会追踪记录
                if (rowdata.Type == "20" || rowdata.Type == "70") {
                    url = location.pathname.replace(/WorkWarnings/ig, 'Project');
                    url = url.replace(/List.aspx/ig, 'ReportShow.aspx') + "?ReportID=" + rowdata.MainID;
                    title = "销售机会跟踪记录查看";
                }
                //销售机会
                if (rowdata.Type == "10") {
                    url = location.pathname.replace(/WorkWarnings/ig, 'Project');
                    url = url.replace(/List.aspx/ig, 'ShowList.aspx') + "?ID=" + rowdata.MainID;
                    title = "销售机会查看";
                }
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: title,
                    url: url,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
            $.post('?action=UpdateStatus', { ID: rowdata.ID }, function (data) {   //更新状态为已读
            })
        };

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var Status = $("#Status").combobox("getValue");
            var Resource = $("#Resource").textbox("getValue");
            var SDate = $("#SDate").datebox('getValue');
            var EDate = $("#EDate").datebox('getValue');
            $('#datagrid').bvgrid('flush', { Status: Status, Resource: Resource, SDate: SDate, EDate: EDate });
        }

        //查询
        function Search() {
            var Status = $("#Status").combobox("getValue");
            var Resource = $("#Resource").textbox("getValue");
            var SDate = $("#SDate").datebox('getValue');
            var EDate = $("#EDate").datebox('getValue');
            $('#datagrid').bvgrid('search', { Status: Status, Resource: Resource, SDate: SDate, EDate: EDate });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="查询列表" data-options="region:'north',border:false" style="height: 100px">
        <table id="table1" style="width: 100%; margin-top: 5px" cellpadding="0" cellspacing="0">
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
            <tr style="height: 25px">
                <td class="lbl">状态</td>
                <td>
                    <input class="easyui-combobox" id="Status" style="width: 90%"
                        data-options="valueField:'value',textField:'text',data: Status" />
                </td>
                <td class="lbl">来源</td>
                <td>
                    <input class="easyui-textbox" id="Resource" style="width: 90%" />
                </td>
                <td class="lbl">开始日期</td>
                <td>
                    <input class="easyui-datebox" id="SDate" name="SDate" style="width: 95%" />
                </td>
                <td class="lbl">结束日期</td>
                <td>
                    <input class="easyui-datebox" id="EDate" name="EDate" style="width: 95%" />
                </td>
            </tr>
        </table>
        <div style="margin-top: 5px">
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="数据列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Summary" data-options="align:'center'" style="width: 100px">提醒内容</th>
                    <th field="TypeName" data-options="align:'center'" style="width: 100px">提醒项目</th>
                    <th field="MainID" data-options="align:'center',formatter:View" style="width: 100px">关联ID</th>
                    <th field="Resource" data-options="align:'center'" style="width: 80px">来源</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 50px">状态</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 100px">更新日期</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
