<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.GeneralManage.ClientDecStatistics.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关量统计</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));

            $('#datagrid').myDatagrid({
                queryParams: { DDataBegin: dateMonth(0), DDataEnd: dateMonth(1), action: "data" },
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,
                pageSize: 20,
                onLoadSuccess: function (data) {
                    GetTotal();
                }
            });
        });

        //查询
        function Search() {
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var DDataBegin = $("#StartDate").datebox('getValue');
            var DDataEnd = $("#EndDate").datebox('getValue');
            $('#datagrid').myDatagrid('search', {
                ClientCode: ClientCode,
                ClientName: ClientName,
                DDataBegin: DDataBegin,
                DDataEnd: DDataEnd,
            });
            GetTotal();
        }

        function GetTotal() {

            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var DDataBegin = $("#StartDate").datebox('getValue');
            var DDataEnd = $("#EndDate").datebox('getValue');

            $.post('?action=TotalData', {
                ClientCode: ClientCode,
                ClientName: ClientName,
                DDataBegin: DDataBegin,
                DDataEnd: DDataEnd,
            },
                function (res) {
                    $("#lblTotal").html(res);
                });
        }

        //重置
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            Search();
        }

        //导出Excel
        function ExportExcel() {
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var DDataBegin = $("#StartDate").datebox('getValue');
            var DDataEnd = $("#EndDate").datebox('getValue');
            var parm = {
                ClientCode: ClientCode,
                ClientName: ClientName,
                DDataBegin: DDataBegin,
                DDataEnd: DDataEnd,
            };

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
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 0;">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" style="width: 250px;" />
                    <span class="lbl" style="margin-left: 5px;">客户名称: </span>
                    <input class="easyui-textbox" id="ClientName" style="width: 250px;" />
                    <span class="lbl">报关日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="required:false,editable:true,missingMessage:'请选择下单开始日期'" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="required:false,editable:true,missingMessage:'请选择下单结束日期'" />

                    <a style="margin-left: 5px;" id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
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
        <table id="datagrid" title="报关量统计" toolbar="#topBar">
            <thead>
                <tr>
                    <th field="ClientCode" data-options="align:'center'" style="width: 8%">客户编号</th>
                    <th field="CompanyName" data-options="align:'left'" style="width: 20%">客户名称</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 8%">注册日期</th>
                    <th field="AgentRate" data-options="align:'center'" style="width: 8%">代理费率</th>
                    <th field="InvoiceType" data-options="align:'center'" style="width: 8%">开票类型</th>
                    <th field="DecPriceTotalStr" data-options="align:'center'" style="width: 12%">报关金额</th>
                    <th field="Currency" data-options="align:'center'" style="width: 8%">币种</th>
                    <th field="ServiceManagerName" data-options="align:'center'" style="width: 10%">业务员</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
