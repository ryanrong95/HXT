<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderBill.aspx.cs" Inherits="WebApp.Order.Bill.OrderBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>对账单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        //是否变更了汇率,弹出编辑汇率窗口时，如果点击了“保存”，则将该变量置为true
        var global_isChangeRate = false;
        var replaceQuotes = '<%=this.Model.ReplaceQuotes%>';
        var replaceSingleQuotes = '<%=this.Model.ReplaceSingleQuotes%>';
        var MainOrderID = '<%=this.Model.MainOrderID%>';
        var OrdersAdvanceMoney = '<%=this.Model.OrdersAdvanceMoney%>';

        <% if (this.Model.IsShowBill) %>
        <% { %>
        var bill = eval('(<%=this.Model.Bill%>)');
        <% } %>


        var totalQty = 0;
        var totalPrice = 0, totalCNYPrice = 0;
        var totalAgencyFee = 0, totalIncidentalFee = 0;
        var totalTraiff = 0, totalExciseTax = 0, totalAddedValueTax = 0;
        var totalTaxFee = 0, totalAmount = 0;

        var subtotalQty = 0;
        var subtotalPrice = 0, subtotalCNYPrice = 0;
        var subtotalAgencyFee = 0, subtotalIncidentalFee = 0;
        var subtotalTraiff = 0, subtotalExciseTax = 0, subtotalAddedValueTax = 0;
        var subtotalTaxFee = 0, subtotalAmount = 0;
        var backSubTotalAmount = 0;
        var backSubTotalTax = 0;
        var backSubTotalTariff = 0;
        var backSubTotalExciseTax = 0;
        var backSubTotalAddedValueTax = 0;
        var backSubTotalAgencyFee = 0;
        var backSubTotalIncidentalFee;

        //页面加载时
        $(function () {

            var IsHistory = eval('(<%=this.Model.IsHistory%>)');
            <% if (this.Model.IsShowBill) %>
            <% { %>
            var from = getQueryString('From');
            switch (from) {
                case 'UnUploaded':
                    $('#approve').hide();
                    $('#download').hide();
                    $('#view').hide();
                    break;
                case 'Auditing':
                    $('#export').hide();
                    $('#edit').hide();
                    $('#uploadFile').next().hide();
                    break;
                case 'Control':
                    $('#approve').hide();
                    $('#download').hide();
                    $('#view').hide();
                    break;
                case 'MerchandiserQuery':
                    if (bill['FileID'] == null || bill['FileID'] == '') {
                        $('#approve').hide();
                        $('#download').hide();
                        $('#view').hide();
                    } else if (bill['FileStatusValue'] == '<%=Needs.Ccs.Services.Enums.OrderFileStatus.Audited.GetHashCode()%>') {
                        $('#approve').hide();
                    }
                    break;
                case 'SalesQuery':
                    $('#approve').hide();
                    $('#edit').hide();
                    $('#uploadFile').next().hide();
                    if (bill['FileID'] == null || bill['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
                case 'AdminQuery':
                    $('#approve').hide();
                    $('#export').hide();
                    $('#edit').hide();
                    $('#uploadFile').next().hide();
                    if (bill['FileID'] == null || bill['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
                case 'OrderChange':
                    if (bill['FileID'] == null || bill['FileID'] == '') {
                        $('#approve').hide();
                        $('#download').hide();
                        $('#view').hide();
                    } else if (bill['FileStatusValue'] == '<%=Needs.Ccs.Services.Enums.OrderFileStatus.Audited.GetHashCode()%>') {
                        $('#approve').hide();
                    }
                    break;

                case 'DeclareOrderQuery':
                    $('#approve').hide();
                    $('#export').hide();
                    $('#edit').hide();
                    $('#uploadFile').next().hide();
                    if (bill['FileID'] == null || bill['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
            }

            $('#OrderID').html(bill['MainOrderID']);
            $('#CreateDate').html(bill['CreateDate']);
            $('#FileStatus').html(bill['FileName'] + '&nbsp;' + '<span style="color:red">(' + bill['FileStatus'] + ')</span>');
            backSubTotalAmount = bill.summaryPayAmount;
            backSubTotalTax = bill.summaryPay;
            backSubTotalTariff = bill.summaryTotalTariff;
            backSubTotalExciseTax = bill.summaryTotalExciseTax;
            backSubTotalAddedValueTax = bill.summaryTotalAddedValueTax;
            backSubTotalAgencyFee = bill.summaryTotalAgencyFee;
            backSubTotalIncidentalFee = bill.summaryTotalIncidentalFee;

            InitBaseInfo(bill);



            for (var i = 0; i < bill.Bills.length; i++) {
                var item = bill.Bills[i];
                var str = '';
                str += '<table id="products"' + i + ' class="border-table" style="margin-top: 5px; font-size: 12px;">';
                if (bill['ContrNo'] == '') {
                    str += '<tr><td class="content" style="text-align:left" colspan="7">订单编号：' + item['OrderID'] + '</td>';
                } else {
                    str += '<td class="content" style="text-align:left" colspan="7">订单编号：' + item['OrderID'] + ' 合同号：' + item['ContrNo'] + '</td>';
                }
                str += '<td class="content" style="text-align:left" colspan="8">实时汇率：' + item['RealExchangeRate'];
                if (bill.HasYBZ) {
                    str += ' 海关汇率：' + item['CustomsExchangeRate'];
                }
                str += '</td></tr>'
                str += '<tr style="background-color: whitesmoke">';
                str += '<th style="width: 5%;">序号</th>';
                str += '<th style="width: 10%; text-align: left">报关品名</th>';
                str += '<th style="width: 10%; text-align: left">规格型号</th>';
                str += '<th style="width: 5%;">数量</th>';
                str += '<th style="width: 7%;">报关单价(' + bill['Currency'] + ')</th>';
                str += '<th style="width: 7%;">报关总价(' + bill['Currency'] + ')</th>';
                str += '<th style="width: 5%;">关税率</th>';
                str += '<th style="width: 7%;">报关货值(CNY)</th>';
                str += '<th style="width: 6%;">关税(CNY)</th>';
                str += '<th style="width: 6%;">消费税(CNY)</th>';
                str += '<th style="width: 6%;">增值税(CNY)</th>';
                str += '<th style="width: 6%;">代理费(CNY)</th>';
                str += '<th style="width: 6%;">杂费(CNY)</th>';
                str += '<th style="width: 7%;">税费合计(CNY)</th>';
                str += '<th style="width: 7%;">报关总金额(CNY)</th>';
                str += '</tr>';

                var totalPrice = parseFloat(item['totalCNYPrice'] + item['totalTraiff'] + item['totalExciseTax'] + item['totalAddedValueTax'] + item['totalAgencyFee'] + item['totalIncidentalFee']).toFixed(2);
                var tariffIsZero = false;
                var exciseTaxZero = false;
                var addedValueTaxZero = false;
                if (item['totalTraiff'] == 0) {
                    tariffIsZero = true;
                }
                if (item['totalExciseTax'] == 0) {
                    exciseTaxZero = true;
                }
                if (item['totalAddedValueTax'] == 0) {
                    addedValueTaxZero = true;
                }
                str += InitProducts(item['Products'], IsHistory, item['OrderType'], item['AgencyFee'], totalPrice, tariffIsZero, exciseTaxZero, addedValueTaxZero);
                str += '</table>';
                $('#splitOrderBills').append(str);
            }


            //显示抵用券信息 Begin
            ShowVouchers();

            setRightHeight();
            //显示抵用券信息 End

            InitSubTotal(bill['Currency']);
            $('#agentInfo').text(bill['AgentName'] + ' ' + bill['AgentAddress'] + ' 电话：' + bill['AgentTel'] + ' 传真：' + bill['AgentFax']);
            $('PurchaserAccount').text('开户行：' + bill['Bank'] + ' 开户名：' + bill['Account'] + ' 账户：' + bill['AccountId']);
            //$("#sealUrl").attr("src", bill['SealUrl']);
            // InitAgreement(bill['IsLoan']);

            InitAgreement();//数据从垫资记录里面取得垫款金额 2020-01-22 by yeshuangshuang

            //注册上传对账单filebox的onChange事件
            $('#uploadFile').filebox({
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='uploadFile']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var formData = new FormData();
                    formData.append('ID', bill['MainOrderID']);
                    formData.append('FileID', bill['FileID']);

                    if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                        photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                            var bl = convertBase64UrlToBlob(base64Codes);
                            formData.append('uploadFile', bl, fileName); // 文件对象
                            $.ajax({
                                url: '?action=UploadBill',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    if (res.success) {
                                        $.messager.alert(
                                            {
                                                title: '',
                                                msg: res.message,
                                                icon: 'info',
                                                top: 300,
                                                fn: function () {
                                                    //Return();
                                                    bill['FileID'] = res.data.ID;
                                                    bill['FileStatus'] = res.data.FileStatus;
                                                    bill['Url'] = res.data.Url;
                                                    bill['FileName'] = res.data.Name;
                                                    $('#approve').hide();
                                                    $('#download').show();
                                                    $('#view').show();
                                                    $('#FileStatus').html(bill['FileName'] + '&nbsp;' + '<span style="color:red">(' + bill['FileStatus'] + ')</span>');

                                                    setRightHeight();
                                                }
                                            });
                                    } else {
                                        $.messager.alert({ title: '提示', msg: res.message, icon: 'info', top: 300 });
                                    }
                                }
                            }).done(function (res) {

                            });
                        });
                    } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                        $.messager.alert({ title: '提示', msg: 'pdf文件大小不能超过3M！', icon: 'info', top: 300 });
                    } else {
                        formData.append('uploadFile', file);
                        $.ajax({
                            url: '?action=UploadBill',
                            type: 'POST',
                            data: formData,
                            dataType: 'JSON',
                            cache: false,
                            processData: false,
                            contentType: false,
                            success: function (res) {
                                if (res.success) {
                                    $.messager.alert(
                                        {
                                            title: '',
                                            msg: res.message,
                                            icon: 'info',
                                            top: 300,
                                            fn: function () {
                                                //Return();
                                                bill['FileID'] = res.data.ID;
                                                bill['FileStatus'] = res.data.FileStatus;
                                                bill['Url'] = res.data.Url;
                                                bill['FileName'] = res.data.Name;
                                                $('#approve').hide();
                                                $('#download').show();
                                                $('#view').show();
                                                $('#FileStatus').html(bill['FileName'] + '&nbsp;' + '<span style="color:red">(' + bill['FileStatus'] + ')</span>');

                                                setRightHeight();
                                            }
                                        });
                                } else {
                                    $.messager.alert({ title: '提示', msg: res.message, icon: 'info', top: 300 });
                                }
                            }
                        }).done(function (res) {

                        });
                    }
                }
            });
            <% } %>

            setRightHeight();
        });


        function InitBaseInfo(bill) {
            var str = '';
            //拼接表格的行和列
            str = '<tr><td class="content" style="text-align:left">委托方：' + bill['ClientName'] + '</td><td class="content" style="text-align:left">被委托方：' + bill['AgentName'] + '</td></tr>' +
                '<tr><td class="content" style="text-align:left">电话：' + bill['ClientTel'] + '</td><td class="content" style="text-align:left">电话：' + bill['AgentTel'] + '</td></tr>';
            $('#baseInfo').append(str);
        }

        //报关商品明细
        function InitProducts(data, IsHistory, orderType, AgencyFee, baoguanzonghuozhi, tariffIsZero, exciseTaxZero, addedValueTaxZero) {
            var str = '';
            totalQty = 0;
            totalPrice = 0;
            totalCNYPrice = 0;
            totalAgencyFee = 0;
            totalIncidentalFee = 0;
            totalTraiff = 0;
            totalExciseTax = 0;
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
                        '<td>' + row.Traiff.toFixed(2) + '</td><td>' + row.ExciseTax.toFixed(2) + '</td><td>' + row.AddedValueTax.toFixed(2) + '</td>' +
                        '<td>' + row.AgencyFee.toFixed(2) + '</td><td>' + row.IncidentalFee.toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.Traiff.toFixed(2)) + parseFloat(row.ExciseTax.toFixed(2)) + parseFloat(row.AddedValueTax.toFixed(2)) + parseFloat(row.AgencyFee.toFixed(2)) + parseFloat(row.IncidentalFee.toFixed(2))).toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.TotalCNYPrice) + parseFloat(row.Traiff.toFixed(2)) + parseFloat(row.ExciseTax.toFixed(2)) + +parseFloat(row.AddedValueTax.toFixed(2)) + parseFloat(row.AgencyFee.toFixed(2)) + parseFloat(row.IncidentalFee.toFixed(2))).toFixed(2) + '</td></tr>';


                    totalQty += parseFloat(row.Quantity);
                    totalPrice += parseFloat(row.TotalPrice);
                    totalCNYPrice += parseFloat(row.TotalCNYPrice.toFixed(2));
                    totalTraiff += parseFloat(row.Traiff.toFixed(2));
                    totalExciseTax += parseFloat(row.ExciseTax.toFixed(2));
                    totalAddedValueTax += parseFloat(row.AddedValueTax.toFixed(2));
                    //totalAgencyFee += parseFloat(row.AgencyFee.toFixed(2));
                    totalIncidentalFee += parseFloat(row.IncidentalFee);

                } else {
                    //拼接表格的行和列
                    str += '<tr><td>' + count + '</td><td style="text-align:left">' + row.ProductName + '</td><td style="text-align:left">' +
                        row.Model.replace(replaceQuotes, '\"').replace(replaceSingleQuotes, '\'') + '</td>' +
                        '<td>' + row.Quantity + '</td><td>' + row.UnitPrice.toFixed(4) + '</td>' + '<td>' + row.TotalPrice.toFixed(2) + '</td>' +
                        '<td>' + row.TariffRate.toFixed(4) + '</td><td>' + row.TotalCNYPrice.toFixed(2) + '</td>' +
                        '<td>' + row.Traiff.toFixed(2) + '</td><td>' + row.ExciseTax.toFixed(2) + '</td><td>' + row.AddedValueTax.toFixed(2) + '</td>' +
                        '<td>' + row.AgencyFee.toFixed(2) + '</td><td>' + row.IncidentalFee.toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.Traiff) + parseFloat(row.ExciseTax) + parseFloat(row.AddedValueTax) + parseFloat(row.AgencyFee) + parseFloat(row.IncidentalFee)).toFixed(2) + '</td>' +
                        '<td>' + (parseFloat(row.TotalCNYPrice) + parseFloat(row.Traiff) + parseFloat(row.ExciseTax) + parseFloat(row.AddedValueTax) + parseFloat(row.AgencyFee) + parseFloat(row.IncidentalFee)).toFixed(2) + '</td></tr>';

                    //统计合计信息
                    totalQty += parseFloat(row.Quantity);
                    totalPrice += parseFloat(row.TotalPrice);
                    totalCNYPrice += parseFloat(row.TotalCNYPrice);
                    totalTraiff += parseFloat(row.Traiff);
                    totalExciseTax += parseFloat(row.ExciseTax);
                    totalAddedValueTax += parseFloat(row.AddedValueTax);
                    //totalAgencyFee += parseFloat(row.AgencyFee);
                    totalIncidentalFee += parseFloat(row.IncidentalFee);
                }
            }

            if (tariffIsZero) {
                totalTraiff = 0;
            }
            if (exciseTaxZero) {
                totalExciseTax = 0;
            }
            if (addedValueTaxZero) {
                totalAddedValueTax = 0;
            }

            subtotalQty += totalQty;
            subtotalPrice += totalPrice;
            subtotalCNYPrice += totalCNYPrice;
            subtotalTraiff += totalTraiff;
            subtotalExciseTax += totalExciseTax;
            subtotalAddedValueTax += totalAddedValueTax;
            //subtotalAgencyFee += totalAgencyFee;
            subtotalIncidentalFee += totalIncidentalFee;


            if (IsHistory == "1") {
                totalAgencyFee = parseFloat(AgencyFee).toFixed(2);
                totalTaxFee = parseFloat(totalTraiff.toFixed(2)) + parseFloat(totalExciseTax.toFixed(2)) + parseFloat(totalAddedValueTax.toFixed(2)) + parseFloat(totalAgencyFee) + parseFloat(totalIncidentalFee.toFixed(2));
                totalAmount = totalCNYPrice + parseFloat(totalTraiff.toFixed(2)) + parseFloat(totalExciseTax.toFixed(2)) + parseFloat(totalAddedValueTax.toFixed(2)) + parseFloat(totalAgencyFee)
                    + parseFloat(totalIncidentalFee.toFixed(2));
                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;
                str += '<tr><td colspan="3">合计：</td>' +
                    '<td>' + totalQty + '</td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td><td>' + totalCNYPrice.toFixed(2) + '</td>' +
                    '<td>' + totalTraiff.toFixed(2) + '</td><td>' + totalExciseTax.toFixed(2) + '</td><td>' + totalAddedValueTax.toFixed(2) + '</td>' +
                    '<td>' + totalAgencyFee.toFixed(2) + '</td><td>' + totalIncidentalFee.toFixed(2) + '</td>' +
                    '<td>' + totalTaxFee.toFixed(2) + '</td>' +
                    '<td>' + totalAmount.toFixed(2) + '</td></tr>';
            } else {
                totalAgencyFee = parseFloat(parseFloat(AgencyFee).toFixed(2));
                subtotalAgencyFee += totalAgencyFee;
                totalTaxFee = totalTraiff + totalExciseTax + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                totalAmount = totalCNYPrice + totalTraiff + totalExciseTax + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;

                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;
                str += '<tr><td colspan="3">合计：</td>' +
                    '<td>' + totalQty + '</td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td><td>' + totalCNYPrice.toFixed(2) + '</td>' +
                    '<td>' + totalTraiff.toFixed(2) + '</td><td>' + totalExciseTax.toFixed(2) + '</td><td>' + totalAddedValueTax.toFixed(2) + '</td>' +
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
            str = '<tr><td class="content" style="text-align:right;width:80%">货值小计</td><td style="text-align:left;width:20%;font-size:12px;">' + currency + ' ' + subtotalPrice.toFixed(2) + '<br/>CNY ' + subtotalCNYPrice.toFixed(2) + '</td></tr>' +
                '<tr><td class="content" style="text-align:right;width:80%">税代费小计</td><td style="text-align:left;width:20%;font-size:12px;">CNY ' + backSubTotalTax.toFixed(2) + '</td></tr>' +
                '<tr><td class="content" style="text-align:right;width:80%">应收总金额合计</td><td style="text-align:left;width:20%;font-size:12px;">CNY ' + backSubTotalAmount.toFixed(2) + '</td></tr>'
            $('#subTotal').append(str);
        }

        //备注协议
        function InitAgreement() {
            //代垫货款、应收费用
            var productFee = 0, receivableFee = 0;
            //if (isLoan) {
            //    productFee = subtotalCNYPrice;
            //    receivableFee = backSubTotalAmount;
            //}
            //else {
            //    productFee = 0;
            //    receivableFee = backSubTotalTax;
            //}
            productFee = Number(OrdersAdvanceMoney);
            receivableFee = backSubTotalTax + Number(OrdersAdvanceMoney);
            //$('#agreement').text('1、我司' + bill.Purchaser + '为委托方代垫本金(' + productFee.toFixed(2) + '元)' +
            //    '+关税(' + backSubTotalTariff.toFixed(2) + '元)+消费税(' + backSubTotalExciseTax.toFixed(2) + '元)+增值税(' + backSubTotalAddedValueTax.toFixed(2) + '元)' +
            //    '+代理费(' + backSubTotalAgencyFee.toFixed(2) + '元)' +
            //    '+杂费(' + subtotalIncidentalFee.toFixed(2) + '元),共计应收人民币(' + receivableFee.toFixed(2) + '元)，' +
            //    '委托方需在(' + bill['DueDate'] + ')前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。');
            $('#agreement').text('1、委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。');
        }

        //导出对账单
        function Export() {
            MaskUtil.mask();
            $.post('?action=ExportBill', { ID: bill['MainOrderID'] }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                    let a = document.createElement('a');
                    document.body.appendChild(a);
                    a.href = result.url;
                    a.download = "";
                    a.click();
                } else {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                }
            })
        }

        //打印对账单
        function Print() {
            $("#container").jqprint();
        }

        //编辑海关汇率/实时汇率
        function EditExchangeRate() {
            var id = bill['MainOrderID'];
            var url = location.pathname.replace(/OrderBill.aspx/ig, 'ExchangeRateEdit.aspx?ID=' + id);

            $.myWindow.setMyWindow("OrderBill", window);
            $.myWindow({
                iconCls: "icon-edit",
                url: url,
                noheader: false,
                title: '编辑汇率',
                closable: false,
                width: '800px',
                height: '500px',
                top: '200px',
                onClose: function () {
                    if (global_isChangeRate) {
                        window.location.reload();
                    }
                }
            });
        }

        //审核通过
        function Approve() {
            $.messager.confirm(
                {
                    title: '确认',
                    msg: '确认客户上传对账单无误，审核通过？',
                    icon: 'info',
                    top: 300,
                    fn: function (success) {
                        if (success) {
                            $.post('?action=ApproveBill', { ID: bill['MainOrderID'] }, function (res) {
                                var result = JSON.parse(res);
                                if (result.success) {
                                    $.messager.alert(
                                        {
                                            title: '',
                                            msg: result.message,
                                            icon: 'info',
                                            top: 300,
                                            fn: function () {
                                                //Return();
                                                bill['FileStatus'] = result.data.FileStatus;
                                                bill['FileName'] = result.data.Name;

                                                $('#approve').hide();
                                                $('#FileStatus').html(bill['FileName'] + '&nbsp;' + '<span style="color:red">(' + bill['FileStatus'] + ')</span>');
                                            }
                                        });
                                } else {
                                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                                }
                            })
                        }
                    }
                });
        }

        //下载对账单
        function Download() {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = bill['Url'];
            a.download = "";
            a.click();
        }

        //查看对账单
        function View() {
            var url = bill['Url'];

            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
            }
            $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
        }

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            if (from == 'UnUploaded') {
                url = location.pathname.replace(/OrderBill.aspx/ig, 'UnUploadedList.aspx');
                window.location = url;
            } else if (from == 'Auditing') {
                url = location.pathname.replace(/OrderBill.aspx/ig, 'AuditingList.aspx');
                window.location = url;
            } else if (from.indexOf('Query') != -1) {
                switch (from) {
                    case 'MerchandiserQuery':
                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/List.aspx');
                        break;
                    case 'SalesQuery':
                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/SalesList.aspx');
                        break;
                    case 'AdminQuery':
                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/AdminList.aspx');
                        break;
                    case 'InsideQuery':
                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/InsideList.aspx');
                        break;
                    case 'DeclareOrderQuery':
                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/DeclareOrderList.aspx');
                        break;
                    default:
                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/List.aspx');
                        break;
                }
                window.parent.location = url;
            } else if (from == 'Control') {
                var controlID = getQueryString('ControlID');
                url = location.pathname.replace(/OrderBill.aspx/ig, '../../Control/Merchandiser/OriginChangeDisplay.aspx?ID=' + controlID);
                window.location = url;
            } else if (from = 'OrderChange') {
                url = location.pathname.replace(/OrderBill.aspx/ig, '../OrderChange/List.aspx');
                window.location = url;

            }
            else if (from = 'DeclarOrderChange') {
                url = location.pathname.replace(/OrderBill.aspx/ig, '../../Declaration/OrderChange/List.aspx');
                window.location = url;

            }
        }


        function SetIsChangeRate(isChangeRate) {
            global_isChangeRate = isChangeRate;
        }

        Number.prototype.toFixed = function (s) {
            var changenum = (parseInt(this * Math.pow(10, s) + 0.5) / Math.pow(10, s)).toString();
            var index = changenum.indexOf(".");
            if (index < 0 && s > 0) {
                changenum = changenum + ".";
                for (i = 0; i < s; i++) {
                    changenum = changenum + "0";
                }

            } else {
                index = changenum.length - index;
                for (i = 0; i < (s - index) + 1; i++) {
                    changenum = changenum + "0";
                }

            }
            return changenum;
        }

        function setRightHeight() {
            var leftHeight = $("#left-zone").height();
            var rightHeight = $("#right-zone").height();
            if (leftHeight > rightHeight) {
                $("#left-zone").height(leftHeight);
                $("#right-zone").height(leftHeight);
            } else {
                $("#left-zone").height(rightHeight);
                $("#right-zone").height(rightHeight);
            }

            if (leftHeight < 143) {
                $("#left-zone").height(143);
                $("#right-zone").height(143);
            }
        }

        //显示抵用券
        function ShowVouchers() {
            $.post('?action=GetVouchers', { MainOrderID: MainOrderID, }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    var vouchersJson = JSON.parse(result.vouchers);
                    $("#vouchers").empty();
                    for (var i = 0; i < vouchersJson.length; i++) {
                        $("#vouchers").append('<div><span style="display: block; float: left;">' + vouchersJson[i].OrderID + '：</span>'
                            + '<span style="display: block; float: left; width: 155px;">' + vouchersJson[i].FinanceVoucherID + '</span>'
                            + '<span style="display: block; float: left;"> ： ' + vouchersJson[i].Amount + '</span></div>');
                        if (i != vouchersJson.length - 1) {
                            $("#vouchers").append('<br>');
                        }
                    }
                }
            });
        }

        //添加抵用券
        function AddVoucher() {
            var url = location.pathname.replace(/OrderBill.aspx/ig, 'AddVoucher.aspx') + "?MainOrderID=" + MainOrderID;

            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '添加抵用券',
                width: '410',
                height: '260',
                url: url,
                onClose: function () {
                    ShowVouchers();
                    setRightHeight();
                }
            });
        }
    </script>
    <style>
        .l-btn-left {
            margin-top: 0px !important;
        }
    </style>
</head>
<body>
    <% if (this.Model.IsShowBill) %>
    <% { %>
    <form id="form1" runat="server" method="post">
        <div title="委托进口货物报关对账单" style="padding: 10px;">
            <div id="container" style="margin: 20px 10px 20px 10px; background-color: white;">
                <%-- 行内样式 --%>
                <style>
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
                </style>

                <h3 style="text-align: left; font-size: 18px; font-weight: bold; margin-bottom: 10px">委托进口货物报关对账单</h3>
                <div style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray; margin-bottom: 5px">
                    <a href="javascript:void(0);" class="easyui-linkbutton" id="edit" data-options="iconCls:'icon-edit'" onclick="EditExchangeRate()">编辑汇率</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" id="approve" data-options="iconCls:'icon-man'" onclick="Approve()">审核通过</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
                </div>
                <div id="left-zone" style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray; width: calc(50% - 5px); float: left; margin-right: 5px; margin-bottom: 15px;">
                    <p class="content">订单编号：<span id="OrderID" style="font-size: 14px"></span></p>
                    <p class="content">下单日期：<span id="CreateDate" style="font-size: 14px"></span></p>
                    <p class="content">附&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;件：<span id="FileStatus" style="font-size: 14px"></span></p>
                    <p>
                        <a href="javascript:void(0);" id="download" class="link" style="margin-left: 65px" data-options="iconCls:'icon-ok'" onclick="Download()">下载</a>
                        <a href="javascript:void(0);" id="view" class="link" style="margin-left: 5px" data-options="iconCls:'icon-search'" onclick="View()">预览</a>
                    </p>
                    <div style="margin-left: 65px; margin-top: 5px; margin-bottom: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" id="export" data-options="iconCls:'icon-save'" onclick="Export()">导出</a>
                        <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                    </div>
                    <p class="content" style="margin-left: 65px">导出pdf格式文件，打印后盖章，扫描后上传</p>
                    <p class="content" style="margin-left: 65px">仅限图片或pdf格式的文件，且pdf文件不超过3M</p>
                </div>
                <div id="right-zone" style="font-size: 14px; background-color: whitesmoke; padding: 5px; border: solid 1px lightgray; width: 50%; float: left;">
                    <div>
                        <span>抵用券</span>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddVoucher()" style="margin-left: 15px;">添加</a>
                    </div>
                    <div id="vouchers" style="margin-top: 3px;"></div>
                </div>
                <br />

                <table id="baseInfo" class="border-table">
                </table>

                <div id="splitOrderBills"></div>

                <%-- <table id="rateInfo" class="border-table">

                </table>

                <table id="products" title="报关商品明细" class="border-table" style="margin-top: 5px">
                    <tr style="background-color: whitesmoke">
                        <th style="width: 5%;">序号</th>
                        <th style="width: 10%; text-align: left">报关品名</th>
                        <th style="width: 10%; text-align: left">规格型号</th>
                        <th style="width: 6%;">数量</th>
                        <th id="unitPrice" style="width: 7%;">报关单价</th>
                        <th id="totalPrice" style="width: 7%;">报关总价</th>
                        <th style="width: 7%;">关税率</th>
                        <th style="width: 7%;">报关货值(CNY)</th>
                        <th style="width: 6%;">关税(CNY)</th>
                        <th style="width: 7%;">增值税(CNY)</th>
                        <th style="width: 7%;">代理费(CNY)</th>
                        <th style="width: 6%;">杂费(CNY)</th>
                        <th style="width: 7%;">税费合计(CNY)</th>
                        <th style="width: 8%;">报关总金额(CNY)</th>
                    </tr>
                </table>--%>

                <table id="subTotal" title="费用合计明细" class="border-table" style="margin-top: 5px">
                </table>

                <table class="border-table" style="margin-top: 5px">
                    <tr>
                        <td id="agentInfo" class="content" style="text-align: left"></td>
                    </tr>
                    <tr>
                        <td class="content" style="text-align: left">
                            <label id="PurchaserAccount"></label>
                        </td>
                    </tr>
                </table>

                <table class="border-table" style="margin-top: 5px">
                    <tr>
                        <td colspan="2" style="text-align: left">
                            <ul>
                                <li class="content">备注：</li>
                                <li id="agreement" class="content">1、委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。</li>
                                <li class="content">2、委托方在90天内完成付汇，付汇汇率为报关协议约定的实际付汇当天的汇率。</li>
                                <li class="content">3、委托方应在收到帐单之日起二个工作日内签字和盖章确认回传。</li>
                                <li class="content">4、此传真件、扫描件、复印件与原件具有同等法律效力。</li>
                                <li class="content">5、如若对此帐单有发生争议的，双方应友好协商解决，如协商不成的，可通过被委托方所在地人民法院提起诉讼解决。</li>
                            </ul>
                        </td>
                    </tr>
                    <tr>
                        <td class="content" style="height: 40px; text-align: left">委托方确认签字或盖章：</td>
                        <td class="content" style="height: 40px; text-align: left">被委托方签字或盖章：</td>
                    </tr>
                </table>

                <div style="position: relative; float: right; bottom: 100px; right: 20%;">
                    <img id="sealUrl" />
                </div>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </form>
    <% } %>
    <% else %>
    <% { %>
    <div id="hint">
        <h3 style="text-align: center; font-size: 18px; font-weight: bold; margin-top: 10px">未报价订单不能生成对账单</h3>
    </div>
    <% } %>
</body>
</html>
