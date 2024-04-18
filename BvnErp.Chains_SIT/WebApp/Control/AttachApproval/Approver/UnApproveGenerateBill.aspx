<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnApproveGenerateBill.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.UnApproveGenerateBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>附加审批-待审批重新生成对账单</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />    
    <script type="text/javascript">
        var replaceQuotes = '<%=this.Model.ReplaceQuotes%>';
        var replaceSingleQuotes = '<%=this.Model.ReplaceSingleQuotes%>';

        var approveGenerateBillInfo = eval('(<%=this.Model.ApproveGenerateBillInfo%>)');
        var bill = eval('(<%=this.Model.Bill%>)');


        var oldExchangeRateValue = eval('(<%=this.Model.OldExchangeRateValue%>)');
        var newExchangeRateValue = eval('(<%=this.Model.NewExchangeRateValue%>)');

        var approveLogs = eval('(<%=this.Model.ApproveLogs%>)');

        var subtotalQty = 0;
        var subtotalPrice = 0, subtotalCNYPrice = 0;
        var subtotalAgencyFee = 0, subtotalIncidentalFee = 0;
        var subtotalTraiff = 0, subtotalAddedValueTax = 0;
        var subtotalTaxFee = 0, subtotalAmount = 0;
        var backSubTotalAmount = 0;
        var backSubTotalTax = 0;
        var backSubTotalTariff = 0;
        var backSubTotalAddedValueTax = 0;
        var backSubTotalAgencyFee = 0;
        var backSubTotalIncidentalFee;

        $(function () {
            var IsHistory = eval('(<%=this.Model.IsHistory%>)');

            $("#ControlTypeDes").html(approveGenerateBillInfo.ControlTypeDes);
            $("#ApplicantName").html(approveGenerateBillInfo.ApplicantName);
            $("#ClientName").html(approveGenerateBillInfo.ClientName);
            $("#MainOrderID").html(approveGenerateBillInfo.MainOrderID);
            $("#Currency").html(approveGenerateBillInfo.Currency);
            $("#DeclarePrice").html(approveGenerateBillInfo.DeclarePrice);



            backSubTotalAmount = bill.summaryPayAmount;
            backSubTotalTax = bill.summaryPay;
            

            for (var i = 0; i < bill.Bills.length; i++) {
                var item = bill.Bills[i];
                var str = '';
                str += '<table id="products"' + i + ' class="border-table" style="margin-top: 5px">';
                if (bill['ContrNo'] == '') {
                    str += '<tr><td class="content" style="text-align:left" colspan="7">订单编号：' + item['OrderID'] + '</td>';
                } else {
                    str += '<td class="content" style="text-align:left" colspan="7">订单编号：' + item['OrderID'] + ' 合同号：' + item['ContrNo'] + '</td>';
                }
                str += '<td class="content" style="text-align:left" colspan="7">实时汇率：' + item['RealExchangeRate'] + ' 海关汇率：' + item['CustomsExchangeRate'] + '</td></tr>';
                str += '<tr style="background-color: whitesmoke">';
                str += '<th style="width: 5%;">序号</th>';
                str += '<th style="width: 10%; text-align: left">报关品名</th>';
                str += '<th style="width: 10%; text-align: left">规格型号</th>';
                str += '<th style="width: 6%;">数量</th>';
                str += '<th style="width: 7%;">报关单价(' + bill['Currency'] + ')</th>';
                str += '<th style="width: 7%;">报关总价(' + bill['Currency'] + ')</th>';
                str += '<th style="width: 7%;">关税率</th>';
                str += '<th style="width: 7%;">报关货值(CNY)</th>';
                str += '<th style="width: 6%;">关税(CNY)</th>';
                str += '<th style="width: 7%;">增值税(CNY)</th>';
                str += '<th style="width: 7%;">代理费(CNY)</th>';
                str += '<th style="width: 6%;">杂费(CNY)</th>';
                str += '<th style="width: 7%;">税费合计(CNY)</th>';
                str += '<th style="width: 8%;">报关总金额(CNY)</th>';
                str += '</tr>';

                var totalPrice = parseFloat(item['totalCNYPrice'] + item['totalTraiff'] + item['totalAddedValueTax'] + item['totalAgencyFee'] + item['totalIncidentalFee']).toFixed(2);
                var tariffIsZero = false;
                var addedValueTaxZero = false;
                if (item['totalTraiff'] == 0) {
                    tariffIsZero = true;
                }
                if (item['totalAddedValueTax'] == 0) {
                    addedValueTaxZero = true;
                }
                str += InitProducts(item['Products'], IsHistory, item['OrderType'], item['AgencyFee'], totalPrice, tariffIsZero, addedValueTaxZero);
                str += '</table>';
                $('#splitOrderBills').append(str);
            }


            InitSubTotal(bill['Currency']);

            //showRate(oldExchangeRateValue, $('#divOldRate'), 'old');
            //showRate(newExchangeRateValue, $('#divNewRate'), 'new');
            showStyleRate(oldExchangeRateValue, newExchangeRateValue);


            $(".CustomRate").numberbox({
                min: 0,
                precision: '4',
                disabled: true,
            });

            $(".RealRate").numberbox({
                min: 0,
                precision: '4',
                disabled: true,
            });

            $(".RealAgency").numberbox({
                min: 0,
                precision: '2',
                disabled: true,
            });

            //修改 divRate 宽度和 subTotal 一致
            $(".divRate").width($("#subTotal").width() - 20);
            $("#approveBtn").width($(".divRate").width() * 0.45);
            $("#cancelBtn").width($(".divRate").width() * 0.45);
            $("#logs").outerWidth($(".divRate").width() * 0.45);

            var from = GetQueryString("From");
            if (from == "Approver") {
                $("#approveBtn").show();
            } else if (from == "Applicant") {
                $("#cancelBtn").show();
            }

            showLogs();


        });

        //报关商品明细
        function InitProducts(data, IsHistory, orderType, AgencyFee, baoguanzonghuozhi, tariffIsZero, addedValueTaxZero) {
            var str = '';
            totalQty = 0;
            totalPrice = 0;
            totalCNYPrice = 0;
            totalAgencyFee = 0;
            totalIncidentalFee = 0;
            totalTraiff = 0;
            totalAddedValueTax = 0;
            totalTaxFee = 0;
            totalAmount = 0;

            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                var count = index + 1;

                if (IsHistory == "1") {

                    //拼接表格的行和列
                    str += '<tr><td>' + count + '</td><td style="text-align:left">' + row.ProductName + '</td><td style="text-align:left">' +
                        row.Model.replace(replaceQuotes, '\"').replace(replaceSingleQuotes, '\'') + '</td>' +
                        '<td>' + row.Quantity + '</td><td>' + row.UnitPrice.toFixed(4) + '</td>' + '<td>' + row.TotalPrice.toFixed(2) + '</td>' +
                        '<td>' + row.TariffRate.toFixed(4) + '</td><td>' + row.TotalCNYPrice.toFixed(2) + '</td>' +
                        '<td>' + row.Traiff.toFixed(2) + '</td><td>' + row.AddedValueTax.toFixed(2) + '</td>' +
                        '<td>' + row.AgencyFee.toFixed(2) + '</td><td>' + row.IncidentalFee.toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.Traiff.toFixed(2)) + parseFloat(row.AddedValueTax.toFixed(2)) + parseFloat(row.AgencyFee.toFixed(2)) + parseFloat(row.IncidentalFee.toFixed(2))).toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.TotalCNYPrice) + parseFloat(row.Traiff.toFixed(2)) + parseFloat(row.AddedValueTax.toFixed(2)) + parseFloat(row.AgencyFee.toFixed(2)) + parseFloat(row.IncidentalFee.toFixed(2))).toFixed(2) + '</td></tr>';


                    totalQty += parseFloat(row.Quantity);
                    totalPrice += parseFloat(row.TotalPrice);
                    totalCNYPrice += parseFloat(row.TotalCNYPrice.toFixed(2));
                    totalTraiff += parseFloat(row.Traiff.toFixed(2));
                    totalAddedValueTax += parseFloat(row.AddedValueTax.toFixed(2));
                    //totalAgencyFee += parseFloat(row.AgencyFee.toFixed(2));
                    totalIncidentalFee += parseFloat(row.IncidentalFee);

                } else {
                    //拼接表格的行和列
                    str += '<tr><td>' + count + '</td><td style="text-align:left">' + row.ProductName + '</td><td style="text-align:left">' +
                        row.Model.replace(replaceQuotes, '\"').replace(replaceSingleQuotes, '\'') + '</td>' +
                        '<td>' + row.Quantity + '</td><td>' + row.UnitPrice.toFixed(4) + '</td>' + '<td>' + row.TotalPrice.toFixed(2) + '</td>' +
                        '<td>' + row.TariffRate.toFixed(4) + '</td><td>' + row.TotalCNYPrice.toFixed(2) + '</td>' +
                        '<td>' + row.Traiff.toFixed(2) + '</td><td>' + row.AddedValueTax.toFixed(2) + '</td>' +
                        '<td>' + row.AgencyFee.toFixed(2) + '</td><td>' + row.IncidentalFee.toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.Traiff) + parseFloat(row.AddedValueTax) + parseFloat(row.AgencyFee) + parseFloat(row.IncidentalFee)).toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.TotalCNYPrice) + parseFloat(row.Traiff) + parseFloat(row.AddedValueTax) + parseFloat(row.AgencyFee) + parseFloat(row.IncidentalFee)).toFixed(2) + '</td></tr>';

                    //统计合计信息
                    totalQty += parseFloat(row.Quantity);
                    totalPrice += parseFloat(row.TotalPrice);
                    totalCNYPrice += parseFloat(row.TotalCNYPrice);
                    totalTraiff += parseFloat(row.Traiff);
                    totalAddedValueTax += parseFloat(row.AddedValueTax);
                    //totalAgencyFee += parseFloat(row.AgencyFee);
                    totalIncidentalFee += parseFloat(row.IncidentalFee);
                }
            }

            if (tariffIsZero) {
                totalTraiff = 0;
            }
            if (addedValueTaxZero) {
                totalAddedValueTax = 0;
            }

            subtotalQty += totalQty;
            subtotalPrice += totalPrice;
            subtotalCNYPrice += totalCNYPrice;
            subtotalTraiff += totalTraiff;
            subtotalAddedValueTax += totalAddedValueTax;
            //subtotalAgencyFee += totalAgencyFee;
            subtotalIncidentalFee += totalIncidentalFee;


            if (IsHistory == "1") {
                totalAgencyFee = parseFloat(AgencyFee).toFixed(2);
                totalTaxFee = parseFloat(totalTraiff.toFixed(2)) + parseFloat(totalAddedValueTax.toFixed(2)) + parseFloat(totalAgencyFee) + parseFloat(totalIncidentalFee.toFixed(2));
                totalAmount = totalCNYPrice + parseFloat(totalTraiff.toFixed(2)) + parseFloat(totalAddedValueTax.toFixed(2)) + parseFloat(totalAgencyFee)
                    + parseFloat(totalIncidentalFee.toFixed(2));
                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;
                str += '<tr><td colspan="3">合计：</td>' +
                    '<td>' + totalQty + '</td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td><td>' + totalCNYPrice.toFixed(2) + '</td>' +
                    '<td>' + totalTraiff.toFixed(2) + '</td><td>' + totalAddedValueTax.toFixed(2) + '</td>' +
                    '<td>' + totalAgencyFee.toFixed(2) + '</td><td>' + totalIncidentalFee.toFixed(2) + '</td>' +
                    '<td>' + totalTaxFee.toFixed(2) + '</td>' +
                    '<td>' + totalAmount.toFixed(2) + '</td></tr>';
            } else {
                totalAgencyFee = parseFloat(parseFloat(AgencyFee).toFixed(2));
                subtotalAgencyFee += totalAgencyFee;
                totalTaxFee = totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                totalAmount = totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;

                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;
                str += '<tr><td colspan="3">合计：</td>' +
                    '<td>' + totalQty + '</td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td><td>' + totalCNYPrice.toFixed(2) + '</td>' +
                    '<td>' + totalTraiff.toFixed(2) + '</td><td>' + totalAddedValueTax.toFixed(2) + '</td>' +
                    '<td>' + totalAgencyFee.toFixed(2) + '</td><td>' + totalIncidentalFee.toFixed(2) + '</td>' +
                    '<td>' + totalTaxFee.toFixed(2) + '</td>' +
                    '<td>' + baoguanzonghuozhi + '</td></tr>';
            }

            return str;


        }

        //费用合计明细
        function InitSubTotal(currency) {
            var str = '';
            //拼接表格的行和列
            str = '<tr><td class="content" style="text-align:right;width:80%">货值小计</td><td style="text-align:left;width:20%">' + currency + ' ' + subtotalPrice.toFixed(2) + '<br/>CNY ' + subtotalCNYPrice.toFixed(2) + '</td></tr>' +
                '<tr><td class="content" style="text-align:right;width:80%">税代费小计</td><td style="text-align:left;width:20%">CNY ' + backSubTotalTax.toFixed(2) + '</td></tr>' +
                '<tr><td class="content" style="text-align:right;width:80%">应收总金额合计</td><td style="text-align:left;width:20%">CNY ' + backSubTotalAmount.toFixed(2) + '</td></tr>'
            $('#subTotal').append(str);
        }

        // ------------------------------ 显示修改前后汇率信息 Begin ------------------------------

        function showRate(exchangeRateValue, $divRate, flag) {
            var normalchecked = '';
            var minchekced = '';
            var pointchecked = '';
            var pointedValue = '';
            var str = '';
            str += '<div>';
            for (var i = 0; i < exchangeRateValue.length; i++) {
                pointedValue = '';
                if (exchangeRateValue[i].OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.Normal.GetHashCode()%>) {
                    normalchecked = 'checked="checked"';
                    minchekced = '';
                    pointchecked = '';
                } else if (exchangeRateValue[i].OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.MinAgencyFee.GetHashCode()%>) {
                    normalchecked = '';
                    minchekced = 'checked="checked"';
                    pointchecked = '';
                } else if (exchangeRateValue[i].OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.Pointed.GetHashCode()%>) {
                    normalchecked = '';
                    minchekced = '';
                    pointchecked = 'checked="checked"';
                    pointedValue = 'value=' + exchangeRateValue[i].RealAgencyFee;
                } else {
                    normalchecked = 'checked="checked"';
                    minchekced = '';
                    pointchecked = '';
                }
                str += '<div id=order' + i + ' class="orderbill" orderid="' + exchangeRateValue[i].OrderID + '">';
                str += ' <div  style="text - align: center">';
                str += '<span>';
                str += '订单编号: ' + exchangeRateValue[i].OrderID + ' 海关汇率: ';
                str += '<input class="CustomRate" value=' + exchangeRateValue[i].CustomsExchangeRate + ' style="width:50px"/>  实时汇率: ';
                str += '<input class="RealRate" value=' + exchangeRateValue[i].RealExchangeRate + ' style="width:50px"/> 代理费类型 ';
                str += '<input disabled type="radio" name="OrderBillType' + i + flag +'" value="<%=Needs.Ccs.Services.Enums.OrderBillType.Normal.GetHashCode() %>" id="NormalType' + i + '" title="正常" class="checkbox checkboxlist"' + normalchecked + '/><label for="NormalType' + i + '" style="margin-right: 20px">正常</label>';
                str += '<input disabled type="radio" name="OrderBillType' + i + flag +'" value="<%=Needs.Ccs.Services.Enums.OrderBillType.MinAgencyFee.GetHashCode() %>" id="MinType' + i + '" title="实际费用" class="checkbox checkboxlist"' + minchekced + ' /><label for="MinType' + i + '" style="margin-right: 20px">实际费用</label>';
                str += '<input disabled type="radio" name="OrderBillType' + i + flag +'" value="<%=Needs.Ccs.Services.Enums.OrderBillType.Pointed.GetHashCode() %>" id="PointType' + i + '" title="指定费用" class="checkbox checkboxlist"' + pointchecked + '/><label for="PointType' + i + '" style="margin-right: 20px">指定费用</label>';
                str += '<input class="RealAgency"'+pointedValue+'  style="width:50px"/>';
                str += '</span>';
                str += '</div>';
                str += '</div>';
            }
            str += '</div>';
            $divRate.append(str);
        }

        //var oldExchangeRateValue = eval('([{"OrderID":"NL02020200121001-01","CustomsExchangeRate":7.0118,"RealExchangeRate":7.0154,"OrderBillType":1,"OrderBillTypeDes":"正常收取","RealAgencyFee":350.5900}])');
        //var newExchangeRateValue = eval('([{"OrderID":"NL02020200121001-01","CustomsExchangeRate":6.0000,"RealExchangeRate":5.0000,"OrderBillType":3,"OrderBillTypeDes":"指定代理费","RealAgencyFee":3.00}])');
        function showStyleRate(oldRateValue, newRateValue) {
            for (var i = 0; i < oldExchangeRateValue.length; i++) {
                var oneOld = oldExchangeRateValue[i];
                var oneNew = findByOrderID(newExchangeRateValue, oneOld.OrderID);
                if (oneNew == null) {
                    oneNew = eval('({"OrderID":"","CustomsExchangeRate":0,"RealExchangeRate":0,"OrderBillType":1,"OrderBillTypeDes":"","RealAgencyFee":0})');
                }

                //颜色 begin
                var customBackground = '#ffffff';
                var realBackground = '#ffffff';
                var ratetypeBackground = '#ffffff';

                if (oneOld.CustomsExchangeRate != oneNew.CustomsExchangeRate) {
                    customBackground = '#fbff0f';
                }
                if (oneOld.RealExchangeRate != oneNew.RealExchangeRate) {
                    realBackground = '#fbff0f';
                }
                if (oneOld.OrderBillType != oneNew.OrderBillType) {
                    ratetypeBackground = '#fbff0f';
                }
                //颜色 end

                var str = '';
                str += '<div>';
                str += '<span>订单编号：' + oneOld.OrderID + "</span>";

                str += '<span style="margin-left: 70px;background-color: ' + customBackground + ';">';
                str += '<span>海关汇率</span><span style="margin-right: 5px;">由</span><label>' + oneOld.CustomsExchangeRate.toFixed(4) + '</label>';
                str += '<span style="margin-left: 5px; margin-right: 5px;">改为</span><label>' + oneNew.CustomsExchangeRate.toFixed(4) + '</label>';
                str += '</span>';

                str += '<span style="margin-left: 70px;background-color: ' + realBackground + ';">';
                str += '<span>实时汇率</span><span style="margin-right: 5px;">由</span><label>' + oneOld.RealExchangeRate.toFixed(4) + '</label>';
                str += '<span style="margin-left: 5px; margin-right: 5px;">改为</span><label>' + oneNew.RealExchangeRate.toFixed(4) + '</label>';
                str += '</span>';

                str += '<span style="margin-left: 70px;background-color: ' + ratetypeBackground + ';">';
                str += '<span>代理费类型</span><span style="margin-right: 5px;">由</span><label>' + oneOld.OrderBillTypeDes + '</label>';
                if (oneOld.OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.Pointed.GetHashCode()%>) {
                    str += '<span>【' + oneOld.RealAgencyFee + '】</span>';
                }
                str += '<span style="margin-left: 5px; margin-right: 5px;">改为</span><label>' + oneNew.OrderBillTypeDes + '</label>';
                if (oneNew.OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.Pointed.GetHashCode()%>) {
                    str += '<span>【' + oneNew.RealAgencyFee + '】</span>';
                }
                str += '</span>';

                str += '</div>';
                $("#styleRate").append(str);
            }


        }

        function findByOrderID(rateValue, orderID) {
            for (var i = 0; i < rateValue.length; i++) {
                if (rateValue[i].OrderID == orderID) {
                    return rateValue[i];
                }
            }

            return null;
        }

        // ------------------------------- 显示修改前后汇率信息 End -------------------------------

        //审批通过
        function ApproveOK() {
            $("#approve-ok-tip").show();
            $("#approve-cancel-reason").hide();
            $("#approve-cancel-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var referenceInfo = $("#reference-info").html();

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveOk', {
                            OrderControlStepID: approveGenerateBillInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();

                                });
                                alert1.window({ modal:true, onBeforeClose:function() {
		                            NormalClose();
                                }});
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    
                                });
                            }
                        });
                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        //审批拒绝
        function ApproveRefuse() {
            $("#approve-ok-tip").hide();
            $("#approve-cancel-reason").show();
            $("#approve-cancel-tip").hide();

            $("#approve-cancel-reason-text").textbox('setValue', '');

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        if (!Valid('form1')) {
                            return;
                        }

                        var referenceInfo = $("#reference-info").html();
                        var reason = $("#approve-cancel-reason-text").textbox('getValue');
                        reason = reason.trim();
                        $("#approve-cancel-reason-text").textbox('setValue', reason);

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveRefuse', {
                            OrderControlStepID: approveGenerateBillInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
                            ApproveCancelReason: reason,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                });
                                alert1.window({ modal:true, onBeforeClose:function() {
		                            NormalClose();
                                }});
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    
                                });
                            }
                        });
                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        //撤销申请
        function CancelApplay() {
            $("#approve-ok-tip").hide();
            $("#approve-cancel-reason").hide();
            $("#approve-cancel-tip").show();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var referenceInfo = $("#reference-info").html();

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveCancel', {
                            OrderControlStepID: approveGenerateBillInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();

                                });
                                alert1.window({ modal:true, onBeforeClose:function() {
		                            NormalClose();
                                }});
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    
                                });
                            }
                        });
                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
        }

        function GetQueryString(name) {
             var reg = new RegExp("(^|&)"+ name +"=([^&]*)(&|$)");
             var r = window.location.search.substr(1).match(reg);
             if(r!=null)return  unescape(r[2]); return null;
        }

        function showLogs() {
            var str = '';
            for (var i = 0; i < approveLogs.length; i++) {
                str += '<div>';
                str += '<label>' + approveLogs[i].CreateDate + '</label><label style="margin-left: 20px;">' + approveLogs[i].Summary + '</label>';
                str += '</div>';
            }
            $("#logs").append(str);
        }
    </script>
    <style>
        #approve-generate-bill-info td {
            font-size: 14px;
        }

        #approve-generate-bill-info td:nth-child(odd) {
            background: #efefef;
            text-align: right;
        }


        .title {
            font: 14px Arial,Verdana,'微软雅黑','宋体';
            font-weight: bold;
        }

        .content {
            font: 14px Arial,Verdana,'微软雅黑','宋体';
            font-weight: normal;
        }

        .link {
            font: 14px Arial,Verdana,'微软雅黑','宋体';
            color: #0081d5;
            cursor: pointer;
        }

        ul li {
            list-style-type: none;
        }

        .border-table {
            line-height: 15px;
            border-collapse: collapse;
            border: 1px solid gray;
            width: 100%;
            text-align: center;
        }

            .border-table tr td {
                font-weight: normal;
                border: 1px solid gray;
                text-align: center;
            }

            .border-table tr th {
                font-weight: normal;
                border: 1px solid gray;
            }

        .noneborder-table {
            line-height: 20px;
            border: none;
            width: 100%;
        }

        #styleRate span {
            font-size: 14px;
        }

        #styleRate label {
            font-size: 14px;
        }
    </style>
</head>
<body class="easyui-layout" style="font-size: 12px;">
    <div style="margin-left: 10%; margin-top: 15px; padding: 10px; width: 80%; height: 80px; border: 1px dashed #808080; border-radius: 5px;">
        <table id="approve-generate-bill-info" style="width: 100%; border-collapse: separate; border-spacing: 2px 5px;">
            <tr>
                <th style="width: 15%;"></th>
                <th style="width: 20%;"></th>
                <th style="width: 10%;"></th>
                <th style="width: 20%;"></th>
                <th style="width: 10%;"></th>
                <th style="width: 20%;"></th>
            </tr>
            <tr>
                <td>审批类型：</td>
                <td id="ControlTypeDes"></td>
                <td>申请人：</td>
                <td id="ApplicantName"></td>
                <td>客户名称：</td>
                <td id="ClientName"></td>
            </tr>
            <tr>
                <td>订单编号：</td>
                <td id="MainOrderID"></td>
                <td>币种：</td>
                <td id="Currency"></td>
                <td>报关总价：</td>
                <td id="DeclarePrice"></td>
            </tr>
        </table>
    </div>

    <div id="reference-info" style="height: 270px; overflow:auto;">
        <div id="splitOrderBills" style="margin-left: 1%; width: 98%; margin-top: 15px;"></div>
        <table id="subTotal" title="费用合计明细" class="border-table" style="margin-left: 1%; width: 98%; margin-top: 5px;"></table>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 120px; overflow:auto;">
        <%--<label style="font-size: 14px; color: red;">原汇率为：</label>
        <div id="divOldRate" style="width: 800px;"></div>
        <div style="margin-top: 10px; margin-bottom: 10px; border-bottom: 1px solid red;"></div><!--用作分割线-->
        <label style="font-size: 14px; color: red;">申请将汇率修改为：</label>
        <div id="divNewRate" style="width: 800px;"></div>--%>
        <div id="styleRate"></div>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 65px; overflow:auto;">
        <div id="approveBtn" class="divRate" style="margin-left: 1%; margin-top: 10px; display: none; float: left;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="ApproveOK()">审批通过</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="ApproveRefuse()" style="margin-left: 10px;">审批拒绝</a>
        </div>
        <div id="cancelBtn" class="divRate" style="margin-left: 1%; margin-top: 10px; display: none; float: left;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="CancelApplay()">撤销</a>
        </div>
        <div id="logs" style="float: left; height: 40px; overflow:auto;">
        </div>
    </div>

    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form id="form1" runat="server">
            <div id="approve-ok-tip" style="padding: 10px; display: none;">
                <label style="font-size: 14px;">确定审批通过？</label>
            </div>
            <div id="approve-cancel-reason" style="margin-left: 15px; margin-top: 10px; display:none;">
                <div><label>拒绝原因：</label></div>
                <div style="margin-top: 3px;">
                    <input id="approve-cancel-reason-text" class="easyui-textbox" data-options="multiline:true, validType:'length[0,200]'," style="width:300px; height:56px;" />
                </div>            
            </div>
            <div id="approve-cancel-tip" style="padding: 10px; display: none;">
                <label style="font-size: 14px;">确定撤销吗？</label>
            </div>
        </form>
    </div>
</body>
</html>
