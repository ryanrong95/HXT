<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.Receipt.Order.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var noticeId = getQueryString("NoticeId");
          var clientID = getQueryString("ClientID");
        var orderId = getQueryString("ID");
        var unReceiveAmount = getQueryString("UnReceiveAmount");
        var orderReceivedDetail = eval('(<%=this.Model.orderReceivedDetail%>)');
        var receivedFees = orderReceivedDetail.ReceivedFees;
        var unReceiveFees = orderReceivedDetail.UnReceiveFees;

        var productHtml = '';
        var productIndex = 0;
        var tariffHtml = '';
        var tariffIndex = 0;
        var addedValueTaxHtml = '';
        var addedValueTaxIndex = 0;
        var agencyFeeHtml = '';
        var agencyFeeIndex = 0;
        var incidentalHtml = '';
        var incidentalIndex = 0;

        $(function () {
            Init();
        });

        function Init() {
            if (receivedFees != null) {
                getData(receivedFees);
            }

            if (unReceiveFees != null&& unReceiveFees != '') {
                getData(unReceiveFees, true);
                $(".showdiv").show();
            } else {
                $(".showdiv").hide();
            }

            $('.product').after(productHtml);
            $('.tariff').after(tariffHtml);
            $('.addedValueTax').after(addedValueTaxHtml);
            $('.agencyFee').after(agencyFeeHtml);
            $('.incidental').after(incidentalHtml);
        }

        function getData(data, noHead) {
            $.each(data, function (i, item) {
                switch (item.Type) {
                    case 1:
                        productHtml += getHtml(item, productIndex);
                        productIndex++;
                        break;
                    case 2:
                        tariffHtml += getHtml(item, tariffIndex);
                        tariffIndex++;
                        break;
                    case 3:
                        addedValueTaxHtml += getHtml(item, addedValueTaxIndex);
                        addedValueTaxIndex++;
                        break;
                    case 4:
                        agencyFeeHtml += getHtml(item, agencyFeeIndex);
                        agencyFeeIndex++;
                        break;
                    case 5:
                        incidentalHtml += getHtml(item, incidentalIndex);
                        incidentalIndex++;
                        break;
                }
            });
        }

        var seed = 0;
        function getHtml(item, index) {
            seed++;

            var payType = item.Ispaid ? '已收款' : '未收款';
            var receiptDate = item.Ispaid ? item.ReceiptDate : '';
            var Name = item.Type == '<%=Needs.Ccs.Services.Enums.OrderFeeType.Incidental.GetHashCode()%>' ? item.Name : '';

            var html = '';
            if (index > 0) {
                html += '<div class="divHead" style="width: 10%">&nbsp;</div>';
            }
            html += '<div class="divHead" style="width: 75%" >' + ' <div style="width: 20%;min-height: 25px">';
            if (item.Ispaid) {
                html +=
                    '<input type="checkbox" name="chkpayment" id="chkpayment' + seed + '" class="checkbox" disabled="disabled" />' +
                    '<label for="chkpayment' + seed + '" id="payment" class="payment">' +
                    item.Amount.toFixed(2) +
                    '</label> </div >' +
                    '<div style="width: 20%">&nbsp;</div>' +
                    '<div style="width: 20%"><label class="paid">' + Name + '</label></div>' +
                    '<div style="width: 20%;min-height: 25px"><label class="ReceiptDate">' +
                    fmtDate(receiptDate) +
                    '</label></div>' +
                    '<div style="width: 20%;min-height: 25px"><label class="paid">' +
                    payType +
                    '</label></div> </div ><br/> ';
            } else {
                html +=
                    '<input type="checkbox" name="chkunpayment" id="chkunpayment' + seed + '" class="checkbox" onchange="SumReceiveValue(this)"  onclick="getAmount(this)" />' +
                    '<label for="chkunpayment' + seed + '" class="unpayment">' +
                    item.Amount.toFixed(2) +
                    '</label> </div >' +
                    '<div style="width: 20%"><input type="textbox"  onchange="SumReceiveValue(this)" data-amount="' + item.Amount + '" data-feesourceid="' + item.FeeSourceID + '"  data-type="' + item.Type + '"  class="receive"  value="" style="height:30,width:160"/></div>' +
                    '<div style="width: 20%"><label class="unpaid">' + Name + '</label></div>' +
                    '<div style="width: 20%;min-height: 25px"><label class="ReceiptDate">' +
                    fmtDate(receiptDate) +
                    '</label></div>' +
                    '<div style="width: 20%;min-height: 25px"><label class="unpaid">' +
                    payType +
                    '</label></div> </div ><br/> ';
            }
            return html;
        }


        function fmtDate(obj) {
            if (obj != '') {
                var date = new Date(obj);
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                m = m < 10 ? ('0' + m) : m;
                var d = date.getDate();
                d = d < 10 ? ('0' + d) : d;
                var h = date.getHours();
                h = h < 10 ? ('0' + h) : h;
                var minute = date.getMinutes();
                var second = date.getSeconds();
                minute = minute < 10 ? ('0' + minute) : minute;
                second = second < 10 ? ('0' + second) : second;
                return y + '-' + m + '-' + d + ' ' + h + ':' + minute + ':' + second;
            } else {
                return '';
            }


        }

        function getAmount(obj) {
            var check = $(obj)[0].checked;
            var value = $(obj).next().text();
            $(obj).parent().next().find('input').val(check ? value : "");
        }

        //合计
        function SumReceiveValue() {
            var sumTotal = 0;
            $(".receive").each(function () {
                sumTotal += Number($(this).val());
                $('.totalReceipt').text(sumTotal.toFixed(2));
            });

        };

        function Back() {
           var url = location.pathname.replace(/Edit.aspx/ig, '/List.aspx?ID=' + noticeId+"&ClientID="+clientID);
           //top.document.getElementById('ifrmain').src = url;    
           window.location.href = url;
        }
        //提交申请
        function Submit() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }

            var data = new FormData($('#form1')[0]);
            data.append("NoticeId", noticeId);
            data.append("OrderId", orderId);
            var sources = [];
            var flag = 0;
            var sumTotal = 0;
            $(".receive").each(function () {
                debugger;
                if ($(this).val().length > 0) {

                    if (Number($(this).val()) > Number($(this).data("amount").toFixed(2))) {
                        flag = flag + 1;

                    }
                    sumTotal += Number($(this).val());
                    sources.push({ FeeSourceID: $(this).data("feesourceid"), Type: $(this).data("type"), Amount: $(this).val() });
                }
            })
            if (sumTotal == 0.00) {
                $.messager.alert('提示', "请填写需要添加的收款明细！");
                return;
            }
            if (flag > 0) {
                $.messager.alert('提示', "收款金额必须小于应收款金额！");
                return;
            }
            if (sumTotal > Number(unReceiveAmount).toFixed(2)) {
                $.messager.alert('提示', "收款总计应小于待收款金额！");
                return;
            }

            data.append("Sources", JSON.stringify(sources));
            $.ajax({
                url: '?action=Submit',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                            Back();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }
    </script>
    <style>
        .divHead,
        .divHead > div {
            /*width: 100px;*/
            display: inline-block;
            font-family: 'Microsoft YaHei', 'Arial', 'Courier New';
            font-size: 14px;
        }

        .link-top {
            width: 80%;
            height: 1px;
            border-top: solid darkgray 1px;
            margin-bottom: 5px;
        }

        .paid {
            color: darkgray;
            margin-left: 20px;
        }

        .unpaid {
            margin-left: 20px;
        }

        .payment, .ReceiptDate {
            color: darkgray;
        }

        .receive {
            border: solid darkgray 1px;
            height: 20px;
            width: 150px;
        }

        label, span, input {
            font-family: 'Microsoft YaHei', 'Arial', 'Courier New';
            font-size: 14px;
        }

        .checkbox + label::before {
            content: "\a0";
            display: inline-block;
            vertical-align: middle;
            font-size: 14px;
            width: 1em;
            height: 1em;
            margin-right: .35em;
            margin-bottom: .25em;
            border-radius: .2em;
            border: 1px solid darkgray;
            text-indent: .15em;
            line-height: .65;
        }

        .checkbox:checked + label::before {
            content: "\2713";
            background-color: #01cd78;
            color: white;
        }

        .checkbox {
            position: absolute;
            clip: rect(0, 0, 0, 0);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div style="margin: 10px 0px 0px 20px">
            <div class="divHead" style="width: 10%"></div>
            <div class="divHead" style="width: 15%">应收款金额</div>
            <div class="divHead" style="width: 15%">本次收款金额</div>
            <div class="divHead" style="width: 15%">描述</div>
            <div class="divHead" style="width: 15%;">收款时间</div>
            <div class="divHead" style="width: 15%;">是否收款</div>
            <br />
            <div class="divHead product" style="width: 10%; min-height: 30px; margin-top: 15px">应收货款：</div>
            <br />
            <div class="link-top"></div>
            <div class="divHead tariff" style="width: 10%; min-height: 30px">应收关税：</div>
            <br />
            <div class="link-top"></div>
            <div class="divHead addedValueTax" style="width: 10%; min-height: 30px">应收增值税：</div>
            <br />
            <div class="link-top"></div>
            <div class="divHead agencyFee" style="width: 10%; min-height: 30px">应收代理费：</div>
            <br />
            <div class="link-top"></div>
            <div class="divHead incidental" style="width: 10%; min-height: 30px">应收杂费：</div>
            <br />
            <div class="link-top"></div>

        </div>

        <div style="margin-left: 20px; line-height: 40px; display: inline-block">
            <span>合计：</span>
            <label class="totalReceipt" style="margin-left: 20px;">0.00</label>
        </div>


        <div style="text-align: center; margin-top: 20px; margin-left: 15px;" >
            <a class="easyui-linkbutton showdiv"  data-options="height:30,width:60,iconCls:'icon-save'" onclick="Submit()">保存</a>
            <a class="easyui-linkbutton" style="margin-left:10px" onclick="Back()" data-options=" height:30,width:60,iconCls:'icon-back'">返回</a>
        </div>
    </form>
</body>
</html>
