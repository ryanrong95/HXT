<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderProfit.aspx.cs" Inherits="WebApp.GeneralManage.Profit.OrderProfit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单利润</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var clients = eval('(<%=this.Model.Clients%>)');
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                queryParams: { StartDate: dateMonth(0), EndDate: dateMonth(1), action: "data" },
            });

            $('#Customer').combobox({
                data: clients
            });
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            //IntData();
        });

        //function IntData() {
        //    if (profits != null) {
        //        $("#Salesman").text(profits.Salesman);
        //        $("#TotalProfits").text(profits.Profits);
        //        $("#TotalCommission").text(profits.BusinessCommission);
        //        $("#StartDate").datebox('setValue', profits.StartDate);
        //        $("#EndDate").datebox('setValue', profits.EndDate);
        //    }
        //}

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

        //查询
        function Search() {
            var Customer = $('#Customer').combobox('getValue');
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var parm = {
                ClientID: Customer,
                StartDate: StartDate,
                EndDate: EndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            $('#Customer').combobox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {
            var Customer = $('#Customer').combobox('getValue');
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                StartDate: StartDate,
                EndDate: EndDate,
                ClientID: Customer
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }
    </script>

    <script type="text/javascript">
        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['ClientName'] == data.rows[i - 1]['ClientName']) {
                    mark += 1;
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'ClientName',
                        rowspan: mark
                    });
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'CustomProfits',
                        rowspan: mark
                    });
                }
                else {
                    mark = 1;
                }
            }
            compute();
        }

        //指定列求和
        function compute() {//计算函数
            var rows = $('#datagrid').datagrid('getRows')//获取当前的数据行
            var ptotal = 0//计算listprice的总和
            for (var i = 0; i < rows.length; i++) {
                ptotal += parseFloat(rows[i]['OrderProfits']);
            }
            //新增一行显示统计信息
            $('#datagrid').datagrid('appendRow', { ClientName: '<b>合计：</b>', OrderProfits: ptotal.toFixed(2) });
        }
    </script>
    <style>
        .divstyle {
            display: inline;
            margin: 20px 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
        </div>
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">报关日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200,editable:false," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">选择客户:</td>
                    <td>
                        <input class="easyui-combobox search" id="Customer" data-options="valueField:'Key',textField:'Value',height:26,width:300" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="订单利润" data-options="
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            toolbar:'#topBar',
            onLoadSuccess: onLoadSuccess,
            pagination:false,
            rownumbers:false">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 10%;">报关日期</th>
                    <th data-options="field:'ReceiveDate',align:'center'" style="width: 10%;">收款日期</th>
                    <th data-options="field:'RMBDeclarePrice',align:'center'" style="width: 10%;">报关货值</th>
                    <th data-options="field:'TaxGeneratTotal',align:'center'" style="width: 10%;">代杂费合计</th>
                    <th data-options="field:'FeeTotal',align:'center'" style="width: 10%;">费用成本</th>
                    <th data-options="field:'HKFeeReceived',align:'center'" style="width: 10%;">香港收款</th>
                    <th data-options="field:'OrderProfit',align:'center'" style="width: 5%;">订单利润</th>
                    <th data-options="field:'proportion',align:'center'" style="width: 5%;">提成比例</th>
                    <th data-options="field:'Commission',align:'center'" style="width: 5%;">提成</th>
                    <th data-options="field:'Referrer',align:'center'" style="width: 5%;">引荐人</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
