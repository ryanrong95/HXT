<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivingList.aspx.cs" Inherits="WebApp.Finance.Receipt.Notice.ReceivingList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待收款通知查询</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        //gvSettings.fatherMenu = '财务报表(XDT)';
        //gvSettings.menu = '收款明细';
        //gvSettings.summary = '';

        var pageNumber = getQueryString("pageNumber");
        var pageSize = getQueryString("pageSize");

        var initClientName = '<%=this.Model.ClientName%>';  //getQueryString("ClientName");
        var initQuerenStatus = getQueryString("QuerenStatus");
        var initStartDate = getQueryString("StartDate");
        var initEndDate = getQueryString("EndDate");

        var lastQuerenStatus = "0";
    </script>
    <script type="text/javascript">
        $(function () {
            if (pageNumber == null || pageNumber == "") {
                pageNumber = 1;
            }
            if (pageSize == null || pageSize == "") {
                pageSize = 20;
            }

            //初始化查询参数（返回来的）放入查询条件输入框内
            $('#ClientName').textbox('setValue', initClientName);
            if (initQuerenStatus == null || initQuerenStatus == "") {
                initQuerenStatus = "0";
            }
            $("#comboboxQuerenStatus").combobox("select", initQuerenStatus);
            $('#StartDate').datebox('setValue', initStartDate);
            $('#EndDate').datebox('setValue', initEndDate);

            //订单列表初始化
            $('#datagrid').myDatagrid({
                //url: location.pathname + "?ClientName=" + initClientName + "&QuerenStatus=" + initQuerenStatus + "&StartDate=" + initStartDate + "&EndDate=" + initEndDate,
                queryParams: {
                    ClientName: initClientName,
                    QuerenStatus: initQuerenStatus,
                    StartDate: initStartDate,
                    EndDate: initEndDate,
                },
                border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                rownumbers: true,
                pageNumber: pageNumber,
                pageSize: pageSize,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onBeforeLoad: function (param) {
                    pageNumber = param.page;
                    pageSize = param.rows;
                },
            });

            $('#comboboxQuerenStatus').combobox({
                onChange: function (newValue, oldValue) {
                    Search();
                }
            });

            //一笔款项的收款记录 弹框, 一些设置
            $('#receiptRecordFinane-dialog').dialog({
                buttons: [{
                    text: '关闭',
                    width: '52px',
                    handler: function () {
                        $('#receiptRecordFinane-dialog').dialog('close');
                    }
                }]
            });
        });

        //查询
        function Search() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

            $('#datagrid').myDatagrid({
                //url: location.pathname + "?ClientName=" + ClientName + "&QuerenStatus=" + QuerenStatus + "&StartDate=" + StartDate + "&EndDate=" + EndDate,
                queryParams: {
                    ClientName: ClientName,
                    QuerenStatus: QuerenStatus,
                    StartDate: StartDate,
                    EndDate: EndDate,
                    LastReceiptDateStartDate: LastReceiptDateStartDate,
                    LastReceiptDateEndDate: LastReceiptDateEndDate,
                },
            });
        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#comboboxQuerenStatus').textbox('setValue', "全部");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#LastReceiptDateStartDate').datebox('setValue', null);
            $('#LastReceiptDateEndDate').datebox('setValue', null);
            Search();
        }

        function ShowReceiptRecordFinance(financeReceiptId) {
            $('#receiptRecordFinane-datagrid').myDatagrid({
                actionName: 'dataDetail',
                queryParams: { FinanceReceiptId: financeReceiptId, },
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                pageSize: 300,
                fit: true,
                singleSelect: false,
                rownumbers: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function () {
                    var trs = $("#receiptRecordFinane-dialog").find(".datagrid-view").find(".datagrid-view2").find("tr");
                    for (var i = 0; i < trs.length; i++) {
                        $(trs[i]).find("td:nth-child(1)").find("div").width(160);
                        $(trs[i]).find("td:nth-child(2)").find("div").width(140);
                        $(trs[i]).find("td:nth-child(3)").find("div").width(60);
                        $(trs[i]).find("td:nth-child(4)").find("div").width(70);
                        $(trs[i]).find("td:nth-child(5)").find("div").width(70);
                        $(trs[i]).find("td:nth-child(6)").find("div").width(80);

                        $(trs[i]).find("td:nth-child(7)").find("div").width(80);
                        $(trs[i]).find("td:nth-child(8)").find("div").width(80);
                        $(trs[i]).find("td:nth-child(9)").find("div").width(80);
                        $(trs[i]).find("td:nth-child(10)").find("div").width(60);
                        $(trs[i]).find("td:nth-child(11)").find("div").width(80);

                        $(trs[i]).find("td:nth-child(12)").find("div").width(80);
                    }
                },
            });

            $('#receiptRecordFinane-dialog').dialog('open');
        }



        //列表框按钮加载
        function Operation(val, row, index) {
            var buttonsAddReceipt = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ShowReceiptRecordFinance(\''
                + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttonsAddReceipt;
        }

        //导出收款明细报表
        function Export() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

            if (!(StartDate != "" || EndDate != "" || ClientName != "" || LastReceiptDateStartDate != "" || LastReceiptDateEndDate != ""||QuerenStatus!='0')) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }


            var queryParams = {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
                LastReceiptDateStartDate: LastReceiptDateStartDate,
                LastReceiptDateEndDate: LastReceiptDateEndDate,
            };
            MaskUtil.mask();
            $.post('?action=ExportDetail', queryParams, function (res) {
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

        //导出收款做账报表
        function ExportFinanceReport() {
            var ClientName = $('#ClientName').textbox('getValue');
            var QuerenStatus = $("#comboboxQuerenStatus").combobox("getValue");
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var LastReceiptDateStartDate = $('#LastReceiptDateStartDate').datebox('getValue');
            var LastReceiptDateEndDate = $('#LastReceiptDateEndDate').datebox('getValue');

            if (!(StartDate != "" || EndDate != "" || ClientName != "" || QuerenStatus != "0")) {
                $.messager.alert({ title: '提示', msg: "请至少选择一种筛选条件", icon: 'info', top: 300 });
                return;
            }

            if (!(LastReceiptDateStartDate != "" || LastReceiptDateEndDate != "" )) {
                $.messager.alert({ title: '提示', msg: "请选择核销日期导出做账报表", icon: 'info', top: 300 });
                return;
            }

            var queryParams = {
                ClientName: ClientName,
                QuerenStatus: QuerenStatus,
                StartDate: StartDate,
                EndDate: EndDate,
                LastReceiptDateStartDate: LastReceiptDateStartDate,
                LastReceiptDateEndDate: LastReceiptDateEndDate,
            };
            MaskUtil.mask();
            $.post('?action=ExportFinanceReport', queryParams, function (res) {
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

    </script>

    <style>
        table.search-condition td {
            padding-left: 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl">付款人:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">状态:</td>
                    <td>
                        <select id="comboboxQuerenStatus" class="easyui-combobox" data-options="height:26,width:200,required:true,editable:false,">
                            <option value="0">全部</option>
                            <option value="1">未确认</option>
                            <option value="2">已确认</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">最近核销日期:</td>
                    <td>
                        <input class="easyui-datebox" id="LastReceiptDateStartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="LastReceiptDateEndDate" data-options="height:26,width:200," />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" id="export" data-options="iconCls:'icon-save'" onclick="Export()">导出</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" id="exportReport" data-options="iconCls:'icon-save'" onclick="ExportFinanceReport()">导出做账</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="收款明细" data-options="
            border:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 7%;">收款日期</th>
                    <th data-options="field:'LastReceiptDate',align:'center'" style="width: 7%;">最近核销日期</th>
                    <th data-options="field:'AccountName',align:'center'" style="width: 15%;">账户名称</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 7%;">金额</th>
                    <th data-options="field:'ClearAmount',align:'center'" style="width: 7%;">已确认金额</th>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 8%;">流水号</th>
                    <th data-options="field:'VaultName',align:'left'" style="width: 7%;">金库</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">付款人</th>
                    <th data-options="field:'InvoiceTypeName',align:'center'" style="width: 7%;">客户类型</th>
                    <th data-options="field:'FinanceReceiptFeeType',align:'center'" style="width: 7%;">类型</th>
                    <th data-options="field:'QuerenStatus',align:'center'" style="width: 7%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 7%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 一笔款项的收款记录 html Begin ------------------------------------------------------------>

    <div id="receiptRecordFinane-dialog" class="easyui-dialog" title="收款记录" style="width: 1200px; height: 600px;"
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <table id="receiptRecordFinane-datagrid" data-options="
            nowrap:false,
            border:false,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'center'" style="width: 20px;">订单编号</th>
                    <th data-options="field:'ContrNo',align:'center'" style="width: 12px;">合同号</th>
                    <th data-options="field:'AddedValueTax',align:'center'" style="width: 12px;">增值税</th>
                    <th data-options="field:'ExciseTax',align:'center'" style="width: 12px;">消费税</th>
                    <th data-options="field:'Tariff',align:'center'" style="width: 12px;">关税</th>
                    <th data-options="field:'ShowAgencyFee',align:'center'" style="width: 12px;">代理费</th>
                    <th data-options="field:'GoodsAmount',align:'center'" style="width: 12px;">货款</th>
                    <th data-options="field:'PaymentExchangeRate',align:'center'" style="width: 12px;">付款汇率</th>
                    <th data-options="field:'FCAmount',align:'center'" style="width: 12px;">外币金额</th>
                    <th data-options="field:'RealExchangeRate',align:'center'" style="width: 12px;">实时汇率</th>
                    <th data-options="field:'DueGoods',align:'center'" style="width: 12px;">应收账款-货款</th>
                    <th data-options="field:'Gains',align:'center'" style="width: 12px;">损益</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
