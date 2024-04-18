<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvanceRecord.aspx.cs" Inherits="WebApp.AdvanceMoney.AdvanceRecord" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>
    <script type="text/javascript">
        var AdvanceMoneyApply = eval('(<%=this.Model.AdvanceMoneyApply%>)');

        $(function () {
            if (AdvanceMoneyApply != "") {
                document.getElementById("ClientName").innerText = AdvanceMoneyApply.ClientName;
                document.getElementById("Amount").innerText = AdvanceMoneyApply.Amount;
                document.getElementById("AmountUsed").innerText = AdvanceMoneyApply.AmountUsed;
                document.getElementById("LimitDays").innerText = AdvanceMoneyApply.LimitDays;
                document.getElementById("InterestRate").innerText = AdvanceMoneyApply.InterestRate;
            }
            else {
                document.getElementById("ClientName").innerText = "";
                document.getElementById("Amount").innerText = "";
                document.getElementById("AmountUsed").innerText = "";
                document.getElementById("LimitDays").innerText = "";
                document.getElementById("InterestRate").innerText = "";
            }
            //列表初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            if (row[checkOverdue] == "是") {

                            }
                        }
                    }
                    return data;
                }
            });
        });

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li style="height: 40px">
                    <span class="lbl">客户名称： 
                        <label id="ClientName"></label>
                    </span>
                </li>
            </ul>
            <ul>
                <li style="height: 40px">
                    <span class="lbl">垫款金额：<label id="Amount" style="width: 190px"></label></span> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
                    <span class="lbl">已使用金额： 
                        <label id="AmountUsed" style="width: 190px"></label>
                    </span>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
                    <span class="lbl">垫资期限： 
                        <label id="LimitDays" style="width: 190px"></label>
                    </span>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
                    <span class="lbl">月利率： 
                        <label id="InterestRate" style="width: 190px"></label>
                    </span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <%--<th data-options="field:'ClientCode',align:'left'" style="width: 16%;">序号</th>--%>
                    <th data-options="field:'OrderID',align:'center'" style="width: 18%;">订单编号</th>
                    <th data-options="field:'PayExchangeID',align:'center'" style="width: 16%;">付汇申请编号</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 10%;">垫款金额</th>
                    <th data-options="field:'PaidAmount',align:'center'" style="width: 8%;">还款金额</th>
                    <th data-options="field:'AdvanceTime',align:'center'" style="width: 16%;">垫资日期</th>
                    <th data-options="field:'InterestAmount',align:'center'" style="width: 8%;">当前利息</th>
                    <th data-options="field:'checkOverdue',align:'center'" style="width: 8%;">是否逾期</th>
                    <th data-options="field:'OverdueInterestAmount',align:'center'" style="width: 16%;">逾期利息</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
