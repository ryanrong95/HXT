<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveGenerateBill.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.ApproveGenerateBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-已审批重新生成对账单</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var approveGenerateBillInfo = eval('(<%=this.Model.ApproveGenerateBillInfo%>)');
        var referenceInfo = '<%=this.Model.ReferenceInfo%>';

        var oldExchangeRateValue = eval('(<%=this.Model.OldExchangeRateValue%>)');
        var newExchangeRateValue = eval('(<%=this.Model.NewExchangeRateValue%>)');

        var approveLogs = eval('(<%=this.Model.ApproveLogs%>)');

        $(function () {
            $("#ControlTypeDes").html(approveGenerateBillInfo.ControlTypeDes);
            $("#ApplicantName").html(approveGenerateBillInfo.ApplicantName);
            $("#ClientName").html(approveGenerateBillInfo.ClientName);
            $("#MainOrderID").html(approveGenerateBillInfo.MainOrderID);
            $("#Currency").html(approveGenerateBillInfo.Currency);
            $("#DeclarePrice").html(approveGenerateBillInfo.DeclarePrice);
            $("#ApproveAdminName").html(approveGenerateBillInfo.ApproveAdminName);
            $("#OrderControlStatusDes").html(approveGenerateBillInfo.OrderControlStatusDes);
            $("#ApproveReason").html(approveGenerateBillInfo.ApproveReason);

            //通过/拒绝字样换颜色
            if (approveGenerateBillInfo.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Approved.GetHashCode()%>') {
                $("#OrderControlStatusDes").css('color', 'green');
            } else if (approveGenerateBillInfo.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Rejected.GetHashCode()%>') {
                $("#OrderControlStatusDes").css('color', 'red');
            }


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
            $(".divRate").width($("#zhanwei").width() - 20);
            $("#approve-result").width($(".divRate").width() * 0.45);
            $("#logs").outerWidth($(".divRate").width() * 0.45);

            $("#reference-info").html(HTMLDecode(referenceInfo));

            showLogs();

        });

        function HTMLEncode(html) {
            var temp = document.createElement("div");
            (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
            var output = temp.innerHTML;
            temp = null;
            return output;
        }

        function HTMLDecode(text) { 
	        var temp = document.createElement("div"); 
	        temp.innerHTML = text; 
	        var output = temp.innerText || temp.textContent; 
	        temp = null; 
	        return output; 
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

    <div style="margin-left: 15px;">
        <label style="font-size: 14px; color: red;">参考信息快照：</label>
    </div>
    <div id="reference-info" style="height: 260px; overflow:auto;">
    </div>

    <table id="zhanwei" class="border-table" style="margin-left: 1%; width: 98%; margin-top: 5px;"></table><!--此处只是用来占位，得到一个宽度-->

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 120px; overflow:auto;">
        <%--<label style="font-size: 14px; color: red;">原汇率为：</label>
        <div id="divOldRate" style="width: 800px;"></div>
        <div style="margin-top: 10px; margin-bottom: 10px; border-bottom: 1px solid red;"></div><!--用作分割线-->
        <label style="font-size: 14px; color: red;">申请将汇率修改为：</label>
        <div id="divNewRate" style="width: 800px;"></div>--%>
        <div id="styleRate"></div>
    </div>
    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 65px; overflow:auto;">
        <div id="approve-result" style="float: left; height: 40px;">
            <div>
                <label style="font-size: 14px; background: #efefef;">审批结果：</label>
                <label id="OrderControlStatusDes" style="font-size: 14px;"></label>
                <label style="font-size: 14px; background: #efefef; margin-left: 30px;">审批人：</label>
                <label id="ApproveAdminName" style="font-size: 14px;"></label>
            </div>
            <div>
                <label style="font-size: 14px; background: #efefef;">拒绝说明：</label>
                <label id="ApproveReason"></label>
            </div>
        </div>
        <div id="logs" style="float: left; height: 40px; overflow:auto;">
        </div>
    </div>
</body>
</html>
