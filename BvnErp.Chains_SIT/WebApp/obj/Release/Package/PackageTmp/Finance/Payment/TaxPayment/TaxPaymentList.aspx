<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxPaymentList.aspx.cs" Inherits="WebApp.Finance.Payment.TaxPayment.TaxPaymentList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {

            $("#DDateStartDate").datebox("setValue", getLastMonthNowDate());
            $("#DDateEndDate").datebox("setValue", getNowFormatDate());

            var decTaxType = eval('(<%=this.Model.DecTaxType%>)');
            $('#TaxType').combobox({
                data: decTaxType
            });

            $('#datagrid').myDatagrid({
                queryParams: {
		            DDateStartDate: $('#DDateStartDate').datebox('getValue'),
                    DDateEndDate: $('#DDateEndDate').datebox('getValue'),
	            },
                border:false,
                fitColumns:true,
                fit:true,
                scrollbarSize: 0,
                rownumbers: true,
                singleSelect: false,
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
                onCheck: function (index, row) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    calcSomeSum($('#datagrid').datagrid('getChecked'));
                },
            });

        });

        //查询
        function Search() {
            var DDateStartDate = $('#DDateStartDate').datebox('getValue');
            var DDateEndDate = $('#DDateEndDate').datebox('getValue');
            var TaxType = $('#TaxType').combobox('getValue');

            var parm = {
                DDateStartDate: DDateStartDate,
                DDateEndDate: DDateEndDate,
                TaxType: TaxType,
            };

            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#DDateStartDate').datebox('setValue', getLastMonthNowDate());
            $('#DDateEndDate').datebox('setValue', getNowFormatDate());
            $('#TaxType').combobox('setValue', null);

            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';

            buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="PayWindow(\''
                + row.DecTaxFlowID + '\',\''
                + row.Amount + '\',\''
                + row.TaxTypeInt
                + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">付款</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            return buttons;
        }

        //点单个付款
        function PayWindow(decTaxFlowID, amount, taxTypeInt) {
            var url = location.pathname.replace(/TaxPaymentList.aspx/ig, 'Pay.aspx')
                + "?DecTaxFlowID=" + decTaxFlowID
                + "&Amount=" + amount
                + "&TaxTypeInt=" + taxTypeInt;

            $.myWindow.setMyWindow("TaxPaymentList2Pay", window);
            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '付款',
                width: '580',
                height: '400',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //点批量付款
        function PayWindowBatch() {
            var rows = $('#datagrid').myDatagrid('getChecked');
            if (rows.length <= 0) {
                $.messager.alert('Warning', '请选择需要付款的税务流水！');
                return;
            }

            var DecTaxFlowIDs = "";
            for (var i = 0; i < rows.length; i++) {
                DecTaxFlowIDs += rows[i].DecTaxFlowID + ",";
            }
            DecTaxFlowIDs = DecTaxFlowIDs.substr(0, DecTaxFlowIDs.length - 1);

            var url = location.pathname.replace(/TaxPaymentList.aspx/ig, 'PayBatch.aspx') + "?DecTaxFlowIDs=" + DecTaxFlowIDs;

            $.myWindow.setMyWindow("TaxPaymentList2PayBatch", window);
            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '付款',
                width: '580',
                height: '400',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //计算一些求和, 显示在界面上
        function calcSomeSum(rows) {
            var totalAmount = 0; //合计
            var addedValueTaxTotalAmount = 0; //增值税
            var tariffTotalAmount = 0; //关税

            for (var i = 0; i < rows.length; i++) {
                var currentTotalAmount = Number(Number(rows[i].Amount).toFixed(2));
                totalAmount += currentTotalAmount;

                if (rows[i].TaxTypeInt == '<%=Needs.Ccs.Services.Enums.DecTaxType.AddedValueTax.GetHashCode()%>') {
                    var currentAddedValueTaxTotalAmount = Number(Number(rows[i].Amount).toFixed(2));
                    addedValueTaxTotalAmount += currentAddedValueTaxTotalAmount;
                } else if(rows[i].TaxTypeInt == '<%=Needs.Ccs.Services.Enums.DecTaxType.Tariff.GetHashCode()%>') {
                    var currentTariffTotalAmount = Number(Number(rows[i].Amount).toFixed(2));
                    tariffTotalAmount += currentTariffTotalAmount;
                }
            }

            $("#TotalAmount").html(totalAmount.toFixed(2)); //合计
            $("#AddedValueTaxTotalAmount").html(addedValueTaxTotalAmount.toFixed(2)); //增值税
            $("#TariffTotalAmount").html(tariffTotalAmount.toFixed(2)); //关税
        }

        //获取今天日期
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        }

        //获取上个月今天日期
        function getLastMonthNowDate() {
            var now = new Date();
            var year = now.getFullYear();//getYear()+1900=getFullYear()
            var month = now.getMonth() + 1;//0-11表示1-12月
            var day = now.getDate();
            if (parseInt(month) < 10) {
                month = "0" + month;
            }
            if (parseInt(day) < 10) {
                day = "0" + day;
            }

            now = year + '-' + month + '-' + day;

            if (parseInt(month) == 1) {//如果是1月份，则取上一年的12月份
                return (parseInt(year) - 1) + '-12-' + day;
            }

            var preSize = new Date(year, parseInt(month) - 1, 0).getDate();//上月总天数
            if (preSize < parseInt(day)) {//上月总天数<本月日期，比如3月的30日，在2月中没有30
                return year + '-' + month + '-01';
            }

            if (parseInt(month) <= 10) {
                return year + '-0' + (parseInt(month) - 1) + '-' + day;
            } else {
                return year + '-' + (parseInt(month) - 1) + '-' + day;
            }
        }
    </script>
    <style>
        #sum-container label {
            font-size: 14px;
            color: brown;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table class="search-condition" style="line-height: 30px;">
                <tr>
                    <td class="lbl">报关日期:</td>
                    <td>
                        <input class="easyui-datebox" id="DDateStartDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 5px;">至</span></td>
                    <td>
                        <input class="easyui-datebox" id="DDateEndDate" data-options="width:200," />
                    </td>
                    <td class="lbl"><span style="padding-left: 10px;">费用类型:</span></td>
                    <td>
                        <select id="TaxType" class="easyui-combobox" data-options="valueField:'Key',textField:'Value',width:200,editable:false,">

                        </select>
                    </td>
                    <td>
                        <span style="padding-left: 10px;">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="PayWindowBatch()">付款</a>
                        <span id="sum-container" style="margin-left: 55px;">
                            <label>合计:</label>
                            <label id="TotalAmount">0</label>
                            <label style="margin-left: 25px;">其中 增值税:</label>
                            <label id="AddedValueTaxTotalAmount">0</label>
                            <label style="margin-left: 25px;">关税:</label>
                            <label id="TariffTotalAmount">0</label>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="付款-税金" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                    <th data-options="field:'DDate',align:'left'" style="width: 8%;">报关日期</th>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 10%;">合同号</th>
                    <th data-options="field:'TaxNumber',align:'left'" style="width: 12%;">海关发票号</th>
                    <th data-options="field:'TaxTypeName',align:'left'" style="width: 8%;">费用名称</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 8%;">金额</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 110px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
