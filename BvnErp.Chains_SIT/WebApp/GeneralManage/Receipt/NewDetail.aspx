<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewDetail.aspx.cs" Inherits="WebApp.GeneralManage.Receipt.NewDetail" %>

<%@ Import Namespace="Needs.Ccs.Services.Enums" %>
<%@ Import Namespace="Needs.Utils.Descriptions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看收款明细</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />

    <script>

        //客户信息
        var clientInfo = eval('(<%= this.Model.ClientInfo%>)');
        var orderInfo = eval('(<%= this.Model.OrderInfo%>)');
        //利润计算
        var profitInfo = eval('(<%= this.Model.Profit%>)');
        //收款
        var receipts = eval('(<%= this.Model.Receipts%>)');
        //付款
        var payments = eval('(<%= this.Model.Payments%>)');

        $(function () {

            //初始化页面
            Init();

            $('#tableReceipt').datagrid({
                data: receipts
            });

            $("#tablePayment").datagrid({
                data: payments
            });

            
           
        });

        function Init() {
            //初始化客户信息
            if (clientInfo != null) {
                $('#ClientCode').text(clientInfo.ClientCode);
                $('#ClientName').text(clientInfo.ClientName);
                $('#ClientRank').text(clientInfo.ClientRank);
                $('#Salesman').text(clientInfo.Salesman);
                $('#Merchandiser').text(clientInfo.Merchandiser);
            };
            //初始化订单信息

            if (orderInfo != null) {
                $('#OrderId').text(orderInfo.OrderId);
                $('#CreateDate').text(orderInfo.CreateDate);
                $('#DeclareDate').text(orderInfo.DeclareDate);
                $('#DeclarePrice').text(orderInfo.DeclarePrice);
                $('#IsLoan').text(orderInfo.IsLoan);
                $('#OrderStatus').text(orderInfo.OrderStatus);
            };

            //利润计算   已收款/未收款
            if (profitInfo != null) {
                $('#ReceiptStatus').text(profitInfo.ReceiptStatus);
                $('#TaxAgentAmount').text(profitInfo.TaxAgentAmount);
                $('#FeeAmount').text(profitInfo.FeeAmount);
                $('#Profit').text(profitInfo.Profit);
                $('#ProfitRate').text(profitInfo.ProfitRate);
            };
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.FeeType + '\',\'' + row.FeeTypeDesc + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //付款表货款按钮加载
        function OperationPay(val, row, index) {
            var buttons = '';
            if (row.PayFeeType == "<%=(int)OrderFeeType.Product%>") {
                buttons ='<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewPay(\'' +
                        row.PayFeeType +
                        '\',\'' +
                        row.PayFeeTypeDesc +
                        '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">查看</span>' +
                        '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
            }
               
            return buttons
        };
        
   
        //查看收款弹窗
        function View(feeType, feeTypeDesc) {
            //区分订单费用类型还是香港现金费用？
            var hkflag = feeTypeDesc == "现金费用" ? true : false;
            var url = location.pathname.replace(/NewDetail.aspx/ig,
                'ReceiptDetail.aspx?ID=' + orderInfo.OrderId + '&FeeType=' + feeType + '&Flag=' + hkflag);
            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '查看收款明细',
                width: 650,
                height: 350,
                onClose: function () {
                    //  $('#receiptRecords').datagrid('reload');
                }
            });
        }


        //查看货款付汇弹窗
        function ViewPay(feeType, feeTypeDesc) {
            var url = location.pathname.replace(/NewDetail.aspx/ig,
                'PaymentDetail.aspx?ID=' + orderInfo.OrderId + '&FeeType=' + feeType);
            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '查看货款付汇明细',
                width: 700,
                height: 350,
                onClose: function () {
                    //  $('#receiptRecords').datagrid('reload');
                }
            });
        }

        //查看订单协议
        function ViewClientAgreement() {
            var ClientID = orderInfo.ClientID;
            var url = location.pathname.replace('NewDetail.aspx', '../../Client/Agreement.aspx?ID=' + ClientID+'&Source=OrderView');
            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '查看订单补充协议',
                width: 1000,
                height:600,
                onClose: function () {
                    //  $('#receiptRecords').datagrid('reload');
                }
            });
        }

    </script>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" style="width: auto;" data-options="border: false,">
        <div data-options="region:'west',border:false" style="width: 27%; float: left;">
            <div class="sec-container" style="margin-top: 5px;">
                <div class="easyui-panel" title="客户信息">
                    <div class="sub-container">
                        <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="lbl">客户编号：</td>
                                <td id="ClientCode" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">客户名称：</td>
                                <td id="ClientName" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">客户级别：</td>
                                <td id="ClientRank" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">业务员：</td>
                                <td id="Salesman" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">跟单员：</td>
                                <td id="Merchandiser" class="lbl"></td>
                            </tr>
                        </table>
                    </div>
                </div>

                <div style="height: 5px;"></div>
                <div class="easyui-panel" title="订单信息">
                    <div class="sub-container">
                      
                        <div></div>
                        <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="lbl">订单编号：</td>
                                <td id="OrderId" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">下单日期：</td>
                                <td id="CreateDate" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">报关日期：</td>
                                <td id="DeclareDate" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">报关货值：</td>
                                <td id="DeclarePrice" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">是否代垫货款：</td>
                                <td id="IsLoan" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">订单状态：</td>
                                <td id="OrderStatus" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">订单协议：</td>
                                <td class="lbl">
                                    <a id="btn" href="javascript:void(0);" style="color: blue" onclick="ViewClientAgreement()">查看</a>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>

                <div style="height: 5px;"></div>
                <div class="easyui-panel" title="利润计算">
                    <div class="sub-container">
                        <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="lbl">收款状态：</td>
                                <td id="ReceiptStatus" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">税代费：</td>
                                <td id="TaxAgentAmount" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">费用：</td>
                                <td id="FeeAmount" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">利润：</td>
                                <td id="Profit" class="lbl"></td>
                            </tr>
                            <tr>
                                <td class="lbl">利润率：</td>
                                <td id="ProfitRate" class="lbl"></td>
                            </tr>
                        </table>
                    </div>
                </div>

            </div>
        </div>

        <div data-options="region:'center',border: false," style="width: 72%; float: left; margin-top: 3px; margin-left: 2px; padding-top: 3px;">
            <div style="width: 100%;">
                <table id="tableReceipt" title="收款" data-options="nowrap: false, fitColumns: true,pagination:false,scrollbarSize:0,singleSelect:true"
                    style="width: 100%;">
                    <thead>
                        <tr>
                            <th data-options="field:'FeeTypeDesc',align:'center'" style="width: 20%; background-color: #f0f0f0">费用类型</th>
                            <th data-options="field:'Receivable',align:'center'" style="width: 20%; background-color: #f8f8f8">应收</th>
                            <th data-options="field:'ReceivableDate',align:'center'" style="width: 25%; background-color: #f0f0f0">应收日期</th>
                            <th data-options="field:'Received',align:'center'" style="width: 20%; background-color: #f8f8f8">实收</th>
                            <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">查看</th>
                        </tr>
                    </thead>
                </table>
            </div>

            <div style="height: 5px;"></div>
            <div style="width: 100%">
                <table id="tablePayment" title="付款" data-options="nowrap: false, fitColumns: true,pagination:false,scrollbarSize:0"
                    style="width: 99%;">
                    <thead>
                        <tr>
                            <th data-options="field:'PayFeeTypeDesc',align:'center'" style="width: 25%; background-color: #f0f0f0">费用类型</th>
                            <th data-options="field:'PayReceived',align:'center'" style="width: 25%; background-color: #f8f8f8">实付</th>
                            <th data-options="field:'PayDate',align:'center'" style="width: 30%; background-color: #f0f0f0">付款日期</th>
                            <th data-options="field:'Btn',align:'center',formatter:OperationPay" style="width: 19%;">查看</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
</body>
</html>
