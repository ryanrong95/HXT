<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.GeneralManage.Profit.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单收款明细</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var clients = eval('(<%=this.Model.Clients%>)');
        var profits = eval('(<%=this.Model.ProfitDetails%>)');

        $(function () {
            debugger;
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                loadFilter: function (data) {
                    var mark = 1;
                    var totalProfits = 0;
                    for (var i = 1; i < data.rows.length; i++) {
                        if (data.rows[i]['ClientName'] == data.rows[i - 1]['ClientName']) {
                            mark += 1;
                            data.rows[i + 1 - mark]['CustomProfits'] = parseFloat(data.rows[i + 1 - mark]['CustomProfits']) + parseFloat(data.rows[i]['CustomProfits']);
                        }
                        else {
                            mark = 1;
                            totalProfits = 0
                        }
                    }
                    return data;
                }
            });

            $('#Customer').combobox({
                data: clients
            });
            IntData();
        });

        function IntData() {
            if (profits != null) {
                $("#Salesman").text(profits.Salesman);
                $("#TotalProfits").text(profits.Profits);
                $("#TotalCommission").text(profits.BusinessCommission);
                $("#StartDate").datebox('setValue', profits.StartDate);
                $("#EndDate").datebox('setValue', profits.EndDate);
            }
        }

        //查询
        function Search() {
            var Customer = $('#Customer').combobox('getValue');
            var parm = {
                ClientID: Customer,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Customer').combobox('setValue', null);
            Search();
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
            var utotal = 0;//统计unitcost的总和
            for (var i = 0; i < rows.length; i++) {
                ptotal += parseFloat(rows[i]['OrderProfits']);
                utotal += parseFloat(rows[i]['BusinessCommission']);
            }
            //新增一行显示统计信息
            $('#datagrid').datagrid('appendRow', { ClientName: '<b>合计：</b>', OrderProfits: ptotal.toFixed(2), BusinessCommission: utotal.toFixed(2) });
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
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">报关日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" disabled="disabled" data-options="height:26,width:200,editable:false," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" disabled="disabled" data-options="height:26,width:200," />
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

            <div style="margin-top: 15px">
                <div class="divstyle">
                    <span style="color: red; font-size: 14px">业务员：</span>
                    <label id="Salesman" style="color: red; font-size: 14px"></label>

                </div>
                <div class="divstyle">
                    <span style="color: red; font-size: 14px">总利润：</span>
                    <label id="TotalProfits" style="color: red; font-size: 14px">0.00</label>
                </div>
                <div class="divstyle">
                    <span style="color: red; font-size: 14px">总提成：</span>
                    <label id="TotalCommission" style="color: red; font-size: 14px">0.00</label>
                </div>
            </div>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="提成明细" data-options="
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
                    <th data-options="field:'ClientName',align:'left'" style="width: 100px;">客户名称</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'RMBDeclarePrice',align:'center'" style="width: 100px;">报关货值</th>
                    <th data-options="field:'OrderDate',align:'center'" style="width: 100px;">下单日期</th>
                    <th data-options="field:'ReceiveDate',align:'center'" style="width: 100px;">收款日期</th>
                    <th data-options="field:'TaxGeneratTotal',align:'center'" style="width: 100px;">税代合计</th>
                    <th data-options="field:'FeeTotal',align:'center'" style="width: 100px;">费用合计</th>
                    <th data-options="field:'OrderProfits',align:'center'" style="width: 100px;">订单利润</th>
                    <th data-options="field:'Proportion',align:'center'" style="width: 100px;">比例</th>
                    <th data-options="field:'BusinessCommission',align:'center'" style="width: 100px;">业务提成</th>
                    <th data-options="field:'CustomProfits',align:'center'" style="width: 100px;">客户利润</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
