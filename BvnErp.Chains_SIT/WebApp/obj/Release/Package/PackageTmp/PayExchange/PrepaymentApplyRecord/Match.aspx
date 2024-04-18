<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Match.aspx.cs" Inherits="WebApp.PayExchange.PrepaymentApplyRecord.Match" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var PaidID = '<%=this.Model.ID%>';
        var amount = '<%=this.Model.Amount%>';
        var OldOrderID = '<%=this.Model.OrderID%>';

        $(function () {
            $('#paiddetails').myDatagrid({
                singleSelect: false,
                autoRowWidth: true,
                pagination: true, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                nowrap: false,
            });
        });

        //修改预申请金额
        function Edit(ID, DeclarePrice, PaidExchangeAmount) {
            //var RpaidExchangeAmount = row.DeclarePrice - row.PaidExchangeAmount;
            var url = location.pathname.replace(/match.aspx/ig, 'Edit.aspx') + '?orderID=' + ID + '&PaidID=' + PaidID + '&Amount=' + amount + '&PaidExchangeAmount=' + PaidExchangeAmount + '&DeclarePrice=' + DeclarePrice + '&OldOrderID=' + OldOrderID;

            top.$.myWindow({
                iconCls: "icon-man",
                url: url,
                noheader: false,
                title: '匹配金额',
                width: '350px',
                height: '145px',
                onClose: function () {
                    $('#paiddetails').datagrid('reload');
                }
            });
        }
        //列表框按钮加载
        function Operation(val, row, index) {

            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\',\'' + row.DeclarePrice + '\',\'' + row.PaidExchangeAmount + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">操作</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }
    </script>
</head>

<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false" style="margin: 5px;">
        <table id="paiddetails" title="订单详情" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar" style="width: 100%; height: auto">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'center'" style="width: 20%;">订单编号</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 15%;">下单日期</th>
                    <th data-options="field:'SupplierName',align:'left'" style="width: 20%;">供应商</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 10%;">订单金额</th>
                    <th data-options="field:'PaidExchangeAmount',align:'center'" style="width: 10%;">已申请付汇金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 10%;">币种</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

</body>
</html>
