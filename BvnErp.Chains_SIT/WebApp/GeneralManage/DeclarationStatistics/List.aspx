<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.GeneralManage.DeclarationStatistics.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>业务量统计</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '综合管理(XDT)';
        gvSettings.menu = '业务量统计';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            var ServiceManager = eval('(<%=this.Model.ServiceManager%>)');
            //初始化业务员下拉框
            $('#ServiceManager').combobox({
                data: ServiceManager,
            })

            window.grid = $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                toolbar: '#topBar',
                queryParams: { StartDate: dateMonth(0), EndDate: dateMonth(1), action: "data" },
                pagination: false,
                onLoadSuccess: function (data) {
                    $("#lblTotal").html(data.arr);
                }
            });
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            //Search();
        });

        //查询
        function Search() {
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var SaleManID = $("#ServiceManager").combobox('getValue');
            $('#datagrid').myDatagrid('search', { StartDate: StartDate, EndDate: EndDate, SaleManID: SaleManID });
        }

        //重置
        function Reset() {
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            $("#ServiceManager").combobox('setValue', "");
            Search();
        }

        function dateMonth(number) {
            var nowdays = new Date();
            var year = nowdays.getFullYear();
            var month = nowdays.getMonth();
            if (month == 0) {
                month = 12;
                year = year - 1;
            }
            if (month < 10) {
                month = "0" + month;
            }
            if (number == 0) {
                return year + "-" + month + "-" + "01";
            }
            else {
                var myDate = new Date(year, month, 0);
                return year + "-" + month + "-" + myDate.getDate();
            }
        }

        //导出Excel
        function ExportExcel() {
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var SaleManID = $("#ServiceManager").combobox('getValue');
            var parm = { StartDate: StartDate, EndDate: EndDate, SaleManID: SaleManID };

            MaskUtil.mask();
            $.post('?action=ExportExcel', parm, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                    let a = document.createElement('a');
                    document.body.appendChild(a);
                    a.href = result.url;
                    a.download = "";
                    a.click();
                } else {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                }
            })
        }

        function View(id,Currency) {
            if (id) {
                var StartDate = $("#StartDate").datebox('getValue');
                var EndDate = $("#EndDate").datebox('getValue');
                if (StartDate == "" || EndDate == "") {
                    $.messager.alert('提示', '请选择报关日期！');
                    return;
                }
                var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx?ID=' + id + '&StartDate=' + StartDate + "&EndDate=" + EndDate + "&Currency=" + Currency);
                window.location = url;
            }
        }

        function Operation(val, row, index) {

            var buttons = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="View(\'' + row.ID + '\',\'' + row.Currency + '\')"" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">报关日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="required:true,editable:false,missingMessage:'请选择报关开始日期'" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="required:true,editable:false,missingMessage:'请选择报关结束日期'" />
                    <span class="lbl">业务员: </span>
                    <input class="easyui-combobox" id="ServiceManager" data-options="valueField:'Key',textField:'Value',limitToList:true,editable:false,missingMessage:'请选择业务员'" name="ServiceManager" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportExcel()">导出Excel</a>
                </li>
                <li>
                    <span class="lbl" id="lblTotal" style="margin-left: 0; color: red;">&nbsp;</span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="业务量统计">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'center'" style="width: 50px">业务员</th>
                    <th field="Currency" data-options="align:'center'" style="width: 50px">币种</th>
                    <th field="TotalDeclarePrice" data-options="align:'center'" style="width: 50px">订单总金额</th>
                    <th data-options="field:'btnOpt',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
