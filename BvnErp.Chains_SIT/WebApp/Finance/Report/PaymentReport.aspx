<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentReport.aspx.cs" Inherits="WebApp.Finance.Report.PaymentReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>付汇报表</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {




            $('#datagrid').myDatagrid({
                //fitColumns: true,
                fit: true,
                rownumbers: true,
                onLoadSuccess: function () {
                    //var merges = [{
                    //    index: 2,
                    //    rowspan: 2
                    //}, {
                    //    index: 6,
                    //    rowspan: 2
                    //}, {
                    //    index: 7,
                    //    rowspan: 2
                    //}];
                    //var merges = [{
                    //    index: 3,
                    //    rowspan: 2
                    //}];
                    //for (var i = 0; i < merges.length; i++) {
                    //    $('#datagrid').datagrid('mergeCells', {
                    //        index: merges[i].index,
                    //        field: 'productid',
                    //        rowspan: merges[i].rowspan
                    //    });
                    //}

                    //调整表格右侧表头的宽度
                    var trs1 = $("#datagrid").parent().find(".datagrid-view2").find(".datagrid-header").find("tr:nth-child(1)");
                    var trs2 = $("#datagrid").parent().find(".datagrid-view2").find(".datagrid-header").find("tr:nth-child(2)");

                    for (var i = 1; i <= 7; i++) {
                        $(trs2[0]).find("td:nth-child(" + i + ")").width(55);
                        $(trs2[0]).find("td:nth-child(" + i + ")").find("div").width(55);
                        $(trs2[0]).find("td:nth-child(" + (i * 2) + ")").width(55);
                        $(trs2[0]).find("td:nth-child(" + (i * 2) + ")").find("div").width(55);
                        $(trs2[0]).find("td:nth-child(" + (i * 3) + ")").width(55);
                        $(trs2[0]).find("td:nth-child(" + (i * 3) + ")").find("div").width(55);

                        $(trs1[0]).find("td:nth-child(" + i + ")").width(155);
                        $(trs1[0]).find("td:nth-child(" + i + ")").find("div").width(155);
                    }

                    //调整表格每行的高度
                    var leftTrs = $("#datagrid").parent().find(".datagrid-view1>.datagrid-body tr");
                    var rightTrs = $("#datagrid").parent().find(".datagrid-view2>.datagrid-body tr");

                    for (var i = 0; i < leftTrs.length; i++) {
                        var useHeight = 0;

                        if ($(leftTrs[i]).height() > $(rightTrs[i]).height()) {
                            useHeight = $(leftTrs[i]).height();
                        } else {
                            useHeight = $(rightTrs[i]).height();
                        }

                        $(leftTrs[i]).height(useHeight);
                        $(rightTrs[i]).height(useHeight);
                    }

                    //调整表格右侧的宽度
                    for (var i = 0; i < rightTrs.length; i++) {
                        for (var j = 1; j <= 7; j++) {
                            $(rightTrs[i]).find("td:nth-child(" + j + ")").find("div").width(55);
                            $(rightTrs[i]).find("td:nth-child(" + j + ")").width(55);
                            $(rightTrs[i]).find("td:nth-child(" + (j * 2) + ")").find("div").width(55);
                            $(rightTrs[i]).find("td:nth-child(" + (j * 2) + ")").width(55);
                            $(rightTrs[i]).find("td:nth-child(" + (j * 3) + ")").find("div").width(55);
                            $(rightTrs[i]).find("td:nth-child(" + (j * 3) + ")").width(55);
                        }
                    }

                    //显示横向滚动条
                    $(".datagrid-view2 .datagrid-body").css("overflow-x","scroll");

                }
            });

        });
    </script>
</head>
<body class="easyui-layout">
    
        <div id="topBar">
            <div id="search">
                <table class="search-condition">
                    <tr>
                        <td class="lbl">报关日期: </td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="editable:false" style="width: 200px;" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">至</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="editable:false" style="width: 200px;" />
                        </td>                        
                        <td style="padding-left: 5px">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                            <a id="btnExportExcel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()" style="margin-left: 15px;">导出Excel</a>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div data-options="region:'center',border:false">
            <table id="datagrid" title="付汇报表" style="height: 1250px" data-options="toolbar:'#topBar',">
                <thead frozen="true">
                    <tr>
                        <th field="PayDate" width="80">付汇日期</th>
                        <th field="PayExchangeAmount" width="80">付汇金额</th>
                        <th field="ExchangeRate" width="80">汇率</th>
                        <th field="CNYAmount" width="80">人民币金额</th>
                        <th field="IncomeStatement" width="80">换汇损益</th>
                        <th field="RealTimeExchangeRate" width="80">实时汇率</th>
                    </tr>
                </thead>
                <thead>
                    <tr>
                        <th colspan="3">安达</th>
                        <th colspan="3">環宇</th>
                        <th colspan="3">聯創</th>
                        <th colspan="3">鸿圖</th>
                        <th colspan="3">IC360 電子</th>
                        <th colspan="3">IC360 GROUP</th>
                        <th colspan="3">纽威</th>
                    </tr>
                    <tr>
                        <th field="AndaPayable" align="left">应付账款</th>
                        <th field="AndaPayment" align="left">实付金额</th>
                        <th field="AndaLeft" align="left">余额</th>

                        <th field="HuanYuPayable" align="left">应付账款</th>
                        <th field="HuanYuPayment" align="left">实付金额</th>
                        <th field="HuanYuLeft" align="left">余额</th>

                        <th field="LianChuangPayable" align="left">应付账款</th>
                        <th field="LianChuangPayment" align="left">实付金额</th>
                        <th field="LianChuangLeft" align="left">余额</th>

                        <th field="HongTuPayable" align="left">应付账款</th>
                        <th field="HongTuPayment" align="left">实付金额</th>
                        <th field="HongTuLeft" align="left">余额</th>

                        <th field="IC360ElecPayable" align="left">应付账款</th>
                        <th field="IC360ElecPayment" align="left">实付金额</th>
                        <th field="IC360ElecLeft" align="left">余额</th>

                        <th field="IC360GroupPayable" align="left">应付账款</th>
                        <th field="IC360GroupPayment" align="left">实付金额</th>
                        <th field="IC360GroupLeft" align="left">余额</th>

                        <th field="NewMayPayable" align="left">应付账款</th>
                        <th field="NewMayPayment" align="left">实付金额</th>
                        <th field="NewMayLeft" align="left">余额</th>
                    </tr>
                </thead>
            </table>
        </div>
    
</body>
</html>
