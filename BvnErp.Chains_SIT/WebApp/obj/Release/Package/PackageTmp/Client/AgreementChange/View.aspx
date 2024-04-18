<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="WebApp.AgreementChange.View" %>

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
                    <span class="lbl">客户编号：<label id="ClientCode" style="width: 190px"></label></span>
                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
                    <span class="lbl">信用等级： 
                        <label id="ClientRank" style="width: 190px"></label>
                    </span>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
                    <span class="lbl">业务员： 
                        <label id="ServiceName" style="width: 190px"></label>
                    </span>
                </li>
            </ul>
        </div>
    </div>
    <div><span>变更内容</span> </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'ChangeType',align:'center'" style="width: 25%;">类型</th>
                    <th data-options="field:'OldValue',align:'center'" style="width: 37%;">变更前</th>
                    <th data-options="field:'NewValue',align:'center'" style="width: 37%;">变更后</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
