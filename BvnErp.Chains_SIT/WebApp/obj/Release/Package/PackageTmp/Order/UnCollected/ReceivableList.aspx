<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivableList.aspx.cs" Inherits="WebApp.Order.UnCollected.ReceivableList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待收款订单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script type="text/javascript">
        $(function () {
            //下拉框数据初始化

            var currencies = eval('(<%=this.Model.Currencies%>)');

            $('#Currency').combobox({
                data: currencies,
            });

            //代理订单列表初始化
            $('#orders').myDatagrid({
                fitColumns: true,
                fit: true,
                singleSelect: false,
                //queryParams: { ClientType:'<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>', action: "data" },
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var OriginData = data.rows[index]["CreateDate"].split("T");
                        var showData = OriginData[0];
                        data.rows[index]["CreateDate"] = showData;                       
                    }
                    return data;
                },
            });
        });

        //查询
        function Search() {

            //var currency = $('#Currency').combobox("getValue");

            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');

            $('#orders').myDatagrid('search', { StartDate: startDate, EndDate: endDate, });
        }

        //重置查询条件
        function Reset() {

            //$('#Currency').combobox('setValue', null);

            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //申请付汇
        function ApplyPay(index) {
            var rows = $('#orders').datagrid('getRows');
            var row = rows[index];
            if (!isAllowPayment(row)) {
                return;
            }

            var url = location.pathname.replace(/List.aspx/ig, '../../PayExchange/Add.aspx') + '?ids=' + row.ID;
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SingleReceive(\'' + row.ID + '\',\'' + row.DeclarePrice + '\',\'' + row.Currency + '\',\'' + row.PaidAmount + '\',\'' + row.CollectedAmouont + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">收款</span>' +
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

                    <span class="lbl">日期: </span>
                    <input class="easyui-datebox input" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox input" id="EndDate" data-options="editable:false" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton ml10" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>

                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="收款列表" data-options="border:false,nowrap:false,fitColumns:true,fit:true,singleSelect:false,toolbar:'#topBar'">
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 10%;">编号</th>
                    <%--<th data-options="field:'ClientCode',align:'left'" style="width: 6%;">客户编号</th> --%>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 7%;">日期</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 6%;">外币金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'CNYAmount',align:'center'" style="width: 6%;">RMB金额</th>
                    <th data-options="field:'MatchStatus',align:'center'" style="width: 7%;">收款状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
