<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.PayExchange.PrepaymentApplyRecord.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预付汇记录</title>
    <uc:EasyUI runat="server" />
    <%--    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />--%>
    <link href="../../App_Themes/xp/Style.css?v=1" rel="stylesheet" />
    <%--  <script src="../../Scripts/Ccs.js"></script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                border: false,
                scrollbarSize: 0,
                toolbar: '#topBar',
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
                }
            });
        });

        function Search() {
            var ID = $("#ID").textbox('getValue');
            var ClientName = $("#ClientName").textbox('getValue');
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            $('#datagrid').myDatagrid('search', { ID: ID, ClientName: ClientName, StartDate: StartDate, EndDate: EndDate });
        }

        function Reset() {
            $("#ID").textbox('setValue', "");
            $("#ClientName").textbox('setValue', "");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //匹配
        function Match(OrderID, PayExchangeApplyID, ClientID, SupplierEnglishName, Currency,TotalAmount) {/*OrderID, ID, ClientID, SupplierEnglishName, Currency*/  
            //OrderID=' + OrderID + '&ID=' + ID + '&ClientID=' + ClientID + '&SupplierID=' + SupplierEnglishName + '&Currency=' + Currency;
            var url = location.pathname.replace(/List.aspx/ig, 'Match.aspx') + '?OrderID=' + OrderID + '&ID=' + PayExchangeApplyID + '&ClientID=' + ClientID + '&SupplierID=' + SupplierEnglishName + '&Currency=' + Currency + '&Amount=' + TotalAmount;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: '1200px',
                height: '600px',
                onClose: function () {
                    Search();
                }
            });

        }

        //操作
        function Operation(val, row, index) { /*onclick="Match(\'' + row.OrderID + '\',\'' + row.ID + '\',\'' + row.ClientID + '\',\'' + row.SupplierEnglishName + '\',\'' + row.Currency + '\')"*/
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Match(\'' + row.OrderID + '\',\'' + row.PayExchangeApplyID + '\',\'' + row.ClientID + '\',\'' + row.SupplierEnglishName + '\',\'' + row.Currency + '\',\'' + row.TotalAmount + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">匹配</span>' +
                '<span class="l-btn-icon icon-sum">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1" style="margin: 5px 0 5px 0">
                <tr>
                    <td class="lbl">申请编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="ID" />
                    </td>
                    <td class="lbl">客户名称：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="ClientName" />
                    </td>
                    <td class="lbl">申请时间：</td>
                    <td>
                        <input class="easyui-datebox" data-options="height:26,width:150" id="StartDate" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" data-options="height:26,width:150" id="EndDate" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="预付汇申请记录">
            <thead>
                <tr>
                    <th field="PayExchangeApplyID" data-options="align:'center'" style="width: 120px">申请编号</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 120px">申请日期</th>
                    <th field="OrderID" data-options="align:'center'" style="width: 120px">订单编号</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 200px">客户名称</th>
                    <th field="ClientID" data-options="align:'center', hidden: true">客户编号</th>
                    <th field="SupplierEnglishName" data-options="align:'center', hidden: true">供应商编号</th>
                    <th field="SupplierName" data-options="align:'center'" style="width: 200px">供应商</th>
                    <th field="Currency" data-options="align:'center'" style="width: 80px">币种</th>
                    <th field="ExchangeRate" data-options="align:'left'" style="width: 80px">付汇汇率</th>
                    <th field="TotalAmount" data-options="align:'center'" style="width: 80px">预付金额</th>
                    <th data-options="field:'btn',width:150,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
