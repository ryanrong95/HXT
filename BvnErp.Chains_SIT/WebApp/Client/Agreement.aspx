<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Agreement.aspx.cs" Inherits="WebApp.Client.Agreement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        .lbl {
            text-align: right;
            font-size: 14px;
        }

        .addlink {
            color: #0081d5;
            text-decoration: none;
            padding-left: 20px;
            outline: 0;
            cursor: pointer;
            font-size: 14px;
            font: 13px/1.2 Arial,Verdana,"微软雅黑","宋体";
        }

        .divAll {
            display: table;
            padding-bottom: 12px;
        }

        .spanTitle {
            display: table-cell;
            vertical-align: middle;
            width: 20%;
            padding-left: 11%;
            font-size: 14px;
        }

        .divContent {
            border: 1px solid #aaa;
            width: 800px;
        }

        .radioSpan input {
            margin: 0 10px;
        }

        input[type="radio"] {
            position: inherit;
        }
    </style>
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>

    <script type="text/javascript">

        //$.ajaxSettings.async = false;//设置为同步加载
        var ID = '<%=this.Model.ID%>';
        var AgreementID = '<%=this.Model.AgreementID%>';

        var PeriodType = eval('(<%=this.Model.PeriodType%>)');
        var InvoiceType = eval('(<%=this.Model.InvoiceType%>)');
        var ExchangeRateType = eval('(<%=this.Model.ExchangeRateType%>)');
        var ExchangeRateTypeOther = eval('(<%=this.Model.ExchangeRateTypeOther%>)');
        var ExchangeRateTypeTax = eval('(<%=this.Model.ExchangeRateTypeTax%>)');
        var ExchangeRateTypeGood = eval('(<%=this.Model.ExchangeRateTypeGood%>)');
        var ExchangeRateTypeAgree = eval('(<%=this.Model.ExchangeRateTypeAgree%>)');
        var InvoiceRate = eval('(<%=this.Model.InvoiceRate%>)');
        var PEIsTen = eval('(<%=this.Model.PEIsTen%>)');

        if ('<%=this.Model.ServiceFile != null%>' == 'True') {
            ServiceFile = eval('(<%=this.Model.ServiceFile != null ? this.Model.ServiceFile:""%>)');
        }

        if ('<%=this.Model.ClientAgreementData != null%>' == 'True') {
            ClientAgreementData = eval('(<%=this.Model.ClientAgreementData != null ? this.Model.ClientAgreementData:""%>)');
        }

        if ('<%=this.Model.AdvanceMoneyApply != null%>' == 'True') {
            AdvanceMoneyApply = eval('(<%=this.Model.AdvanceMoneyApply != null ? this.Model.AdvanceMoneyApply:""%>)');
        }
        var ClientStatus = '<%=this.Model?.ClientStatus%>';

        //返回按钮显示or隐藏  this.Model.From  
        var from = eval('(<%=this.Model.From%>)');

        //数据初始化
        $(function () {
            //  debugger;
            $('#btnSaveApply').hide();
            //来自订单收款查看协议页面
            if (from == "OrderView") {
                $('#btnReturn').hide();
                // $('#btnSaveApply').hide();
            }

            //文件上传控件初始化
            $('#ServiceAgreement').chainsupload({
                required: false,
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择PDF类型的文件',
                accept: ['application/pdf'],
            });

            //chainsuploadcss
            $('span .l-btn-left').css("margin-top", "0px");

            //下拉框数据初始化
            $('#InvoiceRate').combobox({
                data: InvoiceRate,
                onLoadSuccess: function () {
                    $('#InvoiceRate').combobox("setValue", '<%=Needs.Ccs.Services.Enums.InvoiceRate.SixPercent.GetHashCode()%>');
                }
            });

            $('#PEIsTen').combobox({
                data: PEIsTen,
                onLoadSuccess: function () {
                    if ('<%=this.Model.ClientAgreementData != null%>' == 'True') {
                        $('#PEIsTen').combobox("setValue", ClientAgreementData.IsTen);
                    }
                    else {
                        $('#PEIsTen').combobox("setValue", '<%=Needs.Ccs.Services.Enums.PEIsTen.Ten.GetHashCode()%>');
                    }
                }
            });

            $('#InvoiceType').combobox({
                data: InvoiceType,
                onSelect: function (record) {
                    if (record.Key == '<%=Needs.Ccs.Services.Enums.InvoiceType.Full.GetHashCode()%>') {
                        $('span[name=SPInvoiceAdd]').css("display", "block");
                        $('span[name=SPInvoiceRate]').css("display", "none");
                    }
                    else {
                        $('span[name=SPInvoiceAdd]').css("display", "none");
                        $('span[name=SPInvoiceRate]').css("display", "block");
                    }
                },
                onLoadSuccess: function () {
                    if ('<%=this.Model.ClientAgreementData != null%>' == 'True') {
                        $('#InvoiceType').combobox("setValue", ClientAgreementData.InvoiceType);
                        if (ClientAgreementData.InvoiceType == '<%=Needs.Ccs.Services.Enums.InvoiceType.Full.GetHashCode()%>') {
                            $('span[name=SPInvoiceAdd]').css("display", "block");
                            $('span[name=SPInvoiceRate]').css("display", "none");
                        }
                        else {
                            $('#InvoiceRate').combobox("setValue", ClientAgreementData.InvoiceTaxRate * 100);
                            $('span[name=SPInvoiceAdd]').css("display", "none");
                            $('span[name=SPInvoiceRate]').css("display", "block");
                        }
                    }
                    else {
                        $('#InvoiceType').combobox("setValue", '<%=Needs.Ccs.Services.Enums.InvoiceType.Full.GetHashCode()%>');
                        $('span[name=SPInvoiceAdd]').css("display", "block");
                        $('span[name=SPInvoiceRate]').css("display", "none");
                    }
                }
            });
            //报关协议，默认预付款，不允许编辑;垫资申请审批生效时，修改报关协议中的货款部分：垫款上限、约定期限（日） by 2020-12-23  yess
            //$('#AgencyFeePeriodType').combobox('disable');
            //$('#TaxPeriodType').combobox('disable');
            $('#GoodsPeriodType').combobox('disable');
            //初始化协议
            if ('<%=this.Model.ClientAgreementData != null%>' == 'True') {
                //协议附件
                if ('<%=this.Model.ServiceFile != null%>' == 'True') {
                    $('#ServiceAgreement').chainsupload("setValue", ServiceFile);
                }

                $('#StartDate').datebox('setValue', ClientAgreementData.StartDate);
                $('#EndDate').datebox('setValue', ClientAgreementData.EndDate);

                $('#PreAgency').textbox('setValue', ClientAgreementData.PreAgency);
                $('#AgencyRate').textbox('setValue', ClientAgreementData.AgencyRate);
                $('#MinAgencyFee').textbox('setValue', ClientAgreementData.MinAgencyFee);

                if (ClientAgreementData.IsPrePayExchange) {
                    document.getElementById('IsPrePayExchange').checked = true;
                }
                if (ClientAgreementData.IsLimitNinetyDays) {
                    document.getElementById('IsLimitNinetyDays').checked = true;
                }

                $('#Summary').textbox('setValue', ClientAgreementData.Summary);

                if (ClientAgreementData.ProductFeeClause || ClientAgreementData.TaxFeeClause || ClientAgreementData.AgencyFeeClause || ClientAgreementData.ProductFeeClause) {
                    //四种款项文本框赋值
                    //判断垫资申请是否有值，如果存在则赋值给当前已完善的报关协议 by 2020-12-23 yess
                    if ('<%=this.Model.AdvanceMoneyApply != null%>' == 'True') {
                        $('#GoodsDaysLimit').textbox('setValue', AdvanceMoneyApply.LimitDays);
                        $('#GoodsMonthlyDay').textbox('setValue', AdvanceMoneyApply.LimitDays);
                        $('#GoodsUpperLimit').textbox('setValue', AdvanceMoneyApply.Amount);
                    } else {
                        $('#GoodsDaysLimit').textbox('setValue', ClientAgreementData.ProductFeeClause.DaysLimit);
                        $('#GoodsMonthlyDay').textbox('setValue', ClientAgreementData.ProductFeeClause.MonthlyDay);
                        $('#GoodsUpperLimit').textbox('setValue', ClientAgreementData.ProductFeeClause.UpperLimit);
                    }
                    $('#GoodsExchangeRateValue').textbox('setValue', ClientAgreementData.ProductFeeClause.ExchangeRateValue);

                    $('#TaxDaysLimit').textbox('setValue', ClientAgreementData.TaxFeeClause.DaysLimit);
                    $('#TaxMonthlyDay').textbox('setValue', ClientAgreementData.TaxFeeClause.MonthlyDay);
                    $('#TaxUpperLimit').textbox('setValue', ClientAgreementData.TaxFeeClause.UpperLimit);
                    $('#TaxExchangeRateValue').textbox('setValue', ClientAgreementData.TaxFeeClause.ExchangeRateValue);

                    $('#AgencyFeeDaysLimit').textbox('setValue', ClientAgreementData.AgencyFeeClause.DaysLimit);
                    $('#AgencyFeeMonthlyDay').textbox('setValue', ClientAgreementData.AgencyFeeClause.MonthlyDay);
                    $('#AgencyFeeUpperLimit').textbox('setValue', ClientAgreementData.AgencyFeeClause.UpperLimit);
                    $('#AgencyFeeExchangeRateValue').textbox('setValue', ClientAgreementData.AgencyFeeClause.ExchangeRateValue);

                    $('#IncidentalDaysLimit').textbox('setValue', ClientAgreementData.IncidentalFeeClause.DaysLimit);
                    $('#IncidentalMonthlyDay').textbox('setValue', ClientAgreementData.IncidentalFeeClause.MonthlyDay);
                    $('#IncidentalUpperLimit').textbox('setValue', ClientAgreementData.IncidentalFeeClause.UpperLimit);
                    $('#IncidentalExchangeRateValue').textbox('setValue', ClientAgreementData.IncidentalFeeClause.ExchangeRateValue);

                    //四种款项下拉框初始化 并赋值
                    if ('<%=this.Model.AdvanceMoneyApply != null%>' == 'True') {
                        initDom("Goods", '<%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>', ClientAgreementData.ProductFeeClause.ExchangeRateType);
                    }
                    else {
                        initDom("Goods", ClientAgreementData.ProductFeeClause.PeriodType, ClientAgreementData.ProductFeeClause.ExchangeRateType);
                    }
                    initDom("Tax", ClientAgreementData.TaxFeeClause.PeriodType, ClientAgreementData.TaxFeeClause.ExchangeRateType);
                    initDom("AgencyFee", ClientAgreementData.AgencyFeeClause.PeriodType, ClientAgreementData.AgencyFeeClause.ExchangeRateType);
                    initDom("Incidental", ClientAgreementData.IncidentalFeeClause.PeriodType, ClientAgreementData.IncidentalFeeClause.ExchangeRateType);
                } else {
                    //四种款项赋初值
                    initDom("Goods", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode()%>');
                    initDom("Tax", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.Custom.GetHashCode()%>');
                    initDom("AgencyFee", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode()%>');
                    initDom("Incidental", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.None.GetHashCode()%>');

                }
            }
            else {
                //换汇方式赋初值
                document.getElementById('IsLimitNinetyDays').checked = true;
                //换汇汇率赋初值
                //document.getElementById('Ten').checked = true;
                //四种款项赋初值
                initDom("Goods", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode()%>');
                initDom("Tax", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.Custom.GetHashCode()%>');
                initDom("AgencyFee", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode()%>');
                initDom("Incidental", '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>', '<%=Needs.Ccs.Services.Enums.ExchangeRateType.None.GetHashCode()%>');
            }

            //保存、申请
            $('.ir-save').on('click', function () {
                if (!Valid("form1")) {
                    return;
                }
                //垫款上限不能为0 
                if (!CheckUpperLimitSum()) {
                    var message = "垫款上限不能为 0"
                    $.messager.alert("消息", message);
                    return;
                }
            //根据客户等级，限制协议额度上限  2020-08-31 by yeshuangshuang
            //与庆永经理商定，会存在大客户的情况：默认显示限额，业务可以手动修改，由风控及经理审批额度
                <%--if (window.parent.frames.Source == "Add") {
                    //会员等级
                    var Rank = "九级";

                    if (ID != "") {
                        Rank = '<%=this.Model.ClientRank%>';
                    }
                    var upperLimit = getUpperLimit(Rank);
                    if (!validateUpperLimitSum(upperLimit)) {
                        var message = "您的客户等级是" + Rank + ",垫款上限为" + upperLimit;
                        $.messager.alert("消息", message);
                        return;
                    }
                }--%>
                //end

                MaskUtil.mask();//遮挡层
                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values["ID"] = ID;

                // values["AgreementFile"] = $("input[name='ServiceAgreement']")[0].files[0];

                //提交后台
                $.post('?action=SaveClientAgreement', { Model: JSON.stringify(values) }, function (res) {
                    MaskUtil.unmask();//关闭遮挡层
                    var result = JSON.parse(res);
                    if ($("input[name='ServiceAgreement']")[0].files[0] != undefined) {
                        //上传附件
                        var data = new FormData($('#form1')[0]);
                        data.append("ID", ID);
                        $.ajax({
                            url: '?action=SaveServiceAgreement',
                            type: 'POST',
                            data: data,
                            dataType: 'JSON',
                            cache: false,
                            processData: false,
                            contentType: false,
                            success: function (res) {
                                //if (!res.success) {
                                //    $.messager.alert('错误', res.message);
                                //}
                            }
                        }).done(function () {
                            //MaskUtil.unmask();//关闭遮挡层
                        });
                    }

                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            //保存成功
                            MaskUtil.unmask();//关闭遮挡层
                            document.location.reload();
                        } else {
                            $.messager.alert('错误', res.message);
                        }
                    });
                });

            });


            //导出协议
            $('#btnExport').on('click', function () {
                if (ClientStatus == '<%=Needs.Ccs.Services.Enums.ClientStatus.Verifying.GetHashCode()%>') {
                    $.messager.alert("消息", "此客户等待风控审批，不能导出协议！");
                    return;
                }
                else if (ClientStatus == '<%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%>') {
                    $.messager.alert("消息", "此客户未完善资料，不能导出协议！");
                    return;
                }
                else if (ClientStatus == '<%=Needs.Ccs.Services.Enums.ClientStatus.Returned.GetHashCode()%>') {
                    $.messager.alert("消息", "此客户已被退回，不能导出协议！");
                    return;
                }
                else if (ClientStatus != '<%=Needs.Ccs.Services.Enums.ClientStatus.Verifying.GetHashCode()%>' && ClientStatus != '<%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%>') {
                    var param = { AgreementID: AgreementID };
                    MaskUtil.mask();
                    $.post('?action=ExportAgreementNew', { AgreementID: AgreementID }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            MaskUtil.unmask();
                            if (rel.success) {
                                //下载文件
                                try {
                                    let a = document.createElement('a');
                                    a.href = rel.url;
                                    a.download = "";
                                    a.click();
                                } catch (e) {
                                    console.log(e);
                                }
                            }
                        });
                    })
                }
            });

            InitClientPage();
            if (window.parent.frames.Source != 'Add' && window.parent.frames.Source != 'Assign' && window.parent.frames.Source != 'Edit') {
                $('#btnApply').hide();
                $('#btnSaveApply').hide();
            }
            if (window.parent.frames.Source == 'ClerkView') {
                $('#btnApply').show();
            }
            if (window.parent.frames.Source == 'Add') {
                $('#btnApply').hide();
            }
            if (ClientStatus == '<%=Needs.Ccs.Services.Enums.ClientStatus.Confirmed.GetHashCode()%>') {
                $('#btnApply').show();
            }
            else {
                $('#btnApply').hide();
            }
            //陈红燕诉求，临时放开已完善还导出协议功能 20200629 ryan
            $('#btnExport').show();
        });

        //客户等级对应的垫款上限
        function getUpperLimit(Rank) {
            switch (Rank) {
                case '一级':
                    upperLimit = 500000;
                    break;
                case '二级':
                    upperLimit = 300000;
                    break;
                case '三级':
                    upperLimit = 200000;
                    break;
                case '四级':
                    upperLimit = 150000;
                    break;
                case '五级':
                    upperLimit = 50000;
                    break;
                default:
                    upperLimit = 0;
                    break;
            }
            return upperLimit;
        }
        //客户等级对应各类型垫款上限
        function getFeeTypeLimit(obj) {
            var upper = getUpperLimit('<%=this.Model.ClientRank%>');
            switch (obj) {
                case 'Tax':
                    upperLimit = upper * 0.9;
                    break;
                case 'AgencyFee':
                    upperLimit = upper * 0.08;
                    break;
                case 'Incidental':
                    upperLimit = upper * 0.02;
                    break;
                default:
                    upperLimit = 0;
                    break;
            }
            return upperLimit;
        }
        function validateUpperLimitSum(upperLimit) {
            var taxUpperLimit = $("#TRTax").css("display") == "none" ? 0 : parseInt($("#TaxUpperLimit").textbox('getValue'));
            var agencyFeeUpperLimit = $("#TRAgencyFee").css("display") == "none" ? 0 : parseInt($("#AgencyFeeUpperLimit").textbox('getValue'));
            var incidentalUpperLimit = $("#TRIncidental").css("display") == "none" ? 0 : parseInt($("#IncidentalUpperLimit").textbox('getValue'));
            var upperLimitSum = taxUpperLimit + agencyFeeUpperLimit + incidentalUpperLimit;
            if (upperLimitSum != 0) {
                if (upperLimitSum > upperLimit) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        }
        //垫款上限不能为0
        function CheckUpperLimitSum() {
            if ($("#TRGoods").css("display") != "none") {
                var goodsUpperLimit = parseInt($("#GoodsUpperLimit").textbox('getValue'));
            }
            if ($("#TRTax").css("display") != "none") {
                var taxUpperLimit = parseInt($("#TaxUpperLimit").textbox('getValue'));
            }
            if ($("#TRAgencyFee").css("display") != "none") {
                var agencyFeeUpperLimit = parseInt($("#AgencyFeeUpperLimit").textbox('getValue'));
            }
            if ($("#TRIncidental").css("display") != "none") {
                var incidentalUpperLimit = parseInt($("#IncidentalUpperLimit").textbox('getValue'));
            }
            if (taxUpperLimit != 0 && agencyFeeUpperLimit != 0 && incidentalUpperLimit != 0 && goodsUpperLimit != 0) {
                return true;
            }
            else {
                return false;
            }

        }

        //设置输入框是否可见/必填；账期类型下拉框初始化
        function initDom(obj, FeeType, RateType) {
            $('#' + obj + 'PeriodType').combobox({
                data: PeriodType,
                onSelect: function (record) {
                    if (record.Key == '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>') {
                        //预付款
                        $('#TR' + obj).css("display", "none");
                        $('#' + obj + 'DaysLimit').textbox({ required: false });
                        $('#' + obj + 'MonthlyDay').textbox({ required: false });
                        $('#' + obj + 'UpperLimit').textbox({ required: false });

                    }
                    else if (record.Key == '<%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>') {
                        //约定期限
                        $('#TR' + obj).css("display", "table-row");
                        $('span[name=SP' + obj + 'Limit]').css("display", "block");
                        $('span[name=SP' + obj + 'Monthly]').css("display", "none");
                        $('#' + obj + 'DaysLimit').textbox({ required: true });
                        $('#' + obj + 'MonthlyDay').textbox({ required: false });
                        $('#' + obj + 'UpperLimit').textbox({ required: true });
                        //非货款垫款方式默认显示限额，业务可编辑
                        if (obj != 'Goods' && $('#' + obj + 'UpperLimit').textbox('getValue') == '') {
                            $('#' + obj + 'UpperLimit').textbox('setValue', getFeeTypeLimit(obj));
                        }
                    }
                    else {
                        //月结
                        $('#TR' + obj).css("display", "table-row");
                        $('span[name=SP' + obj + 'Limit]').css("display", "none");
                        $('span[name=SP' + obj + 'Monthly]').css("display", "block");
                        $('#' + obj + 'DaysLimit').textbox({ required: false });
                        $('#' + obj + 'MonthlyDay').textbox({ required: true });
                        $('#' + obj + 'UpperLimit').textbox({ required: true });
                        //非货款垫款方式默认显示限额，业务可编辑
                        if (obj != 'Goods' && $('#' + obj + 'UpperLimit').textbox('getValue') == '') {
                            $('#' + obj + 'UpperLimit').textbox('setValue', getFeeTypeLimit(obj));
                        }
                    }
                    $.parser.parse($('#TR' + obj));
                },
                onLoadSuccess: function () {
                    if (FeeType == '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>') {
                        //预付款
                        $('#TR' + obj).css("display", "none");
                        $('#' + obj + 'DaysLimit').textbox({ required: false });
                        $('#' + obj + 'MonthlyDay').textbox({ required: false });
                        $('#' + obj + 'UpperLimit').textbox({ required: false });
                    }
                    else if (FeeType == '<%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>') {
                        //约定期限
                        $('#TR' + obj).css("display", "table-row");
                        $('span[name=SP' + obj + 'Limit]').css("display", "block");
                        $('span[name=SP' + obj + 'Monthly]').css("display", "none");
                        $('#' + obj + 'DaysLimit').textbox({ required: true });
                        $('#' + obj + 'MonthlyDay').textbox({ required: false });
                        $('#' + obj + 'UpperLimit').textbox({ required: true });
                        //非货款垫款方式默认显示限额，业务可编辑
                        if (obj != 'Goods' && $('#' + obj + 'UpperLimit').textbox('getValue') == '') {
                            $('#' + obj + 'UpperLimit').textbox('setValue', getFeeTypeLimit(obj));
                        }
                    }
                    else {
                        //月结
                        $('#TR' + obj).css("display", "table-row");
                        $('span[name=SP' + obj + 'Limit]').css("display", "none");
                        $('span[name=SP' + obj + 'Monthly]').css("display", "block");
                        $('#' + obj + 'DaysLimit').textbox({ required: false });
                        $('#' + obj + 'MonthlyDay').textbox({ required: true });
                        $('#' + obj + 'UpperLimit').textbox({ required: true });
                        //非货款垫款方式默认显示限额，业务可编辑
                        if (obj != 'Goods' && $('#' + obj + 'UpperLimit').textbox('getValue') == '') {
                            $('#' + obj + 'UpperLimit').textbox('setValue', getFeeTypeLimit(obj));
                        }
                    }
                    $('#' + obj + 'PeriodType').combobox('setValue', FeeType);
                    $.parser.parse($('#TR' + obj));
                }
            });
            $('#' + obj + 'ExchangeRateType').combobox({
                data: FilterExchangeRateType(obj),
                onSelect: function (record) {
                    if (record.Key == '<%=Needs.Ccs.Services.Enums.ExchangeRateType.Agreed.GetHashCode()%>') {
                        //约定汇率
                        $('span[name=SP' + obj + 'AgreeRate]').css("display", "block");
                        $('#' + obj + 'ExchangeRateValue').textbox({ required: true });
                    }
                    else {
                        //实时汇率or海关汇率
                        $('span[name=SP' + obj + 'AgreeRate]').css("display", "none");
                        $('#' + obj + 'ExchangeRateValue').textbox({ required: false });
                    }
                },
                onLoadSuccess: function () {
                    if (RateType == '<%=Needs.Ccs.Services.Enums.ExchangeRateType.Agreed.GetHashCode()%>') {
                        //约定汇率
                        $('span[name=SP' + obj + 'AgreeRate]').css("display", "block");
                        $('#' + obj + 'ExchangeRateValue').textbox({ required: true });
                    }
                    else {
                        //实时汇率or海关汇率
                        $('span[name=SP' + obj + 'AgreeRate]').css("display", "none");
                        $('#' + obj + 'ExchangeRateValue').textbox({ required: false });
                    }
                    $('#' + obj + 'ExchangeRateType').combobox('setValue', RateType);
                }
            });
        }

        function FilterExchangeRateType(obj) {
            if (obj == "Incidental") {
                return ExchangeRateTypeOther;
            }
            if (obj == "AgencyFee") {
                return ExchangeRateTypeAgree;
            }
            if (obj == "Tax") {
                return ExchangeRateTypeTax;
            }
            if (obj == "Goods") {
                return ExchangeRateTypeGood;
            }

            return ExchangeRateType;
        }

        function Return() {
            var source = window.parent.frames.Source;//$('#ClientInfo').data('source');
            var u = "";
            switch (source) {
                case 'Add':
                    u = 'New/List.aspx';
                    break;
                case 'Assign':
                    u = 'Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = 'New/List.aspx';
                    break;
                case 'ApproveView':
                    u = 'Approval/List.aspx';
                    break;
                case 'QualifiedView':
                    u = 'Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = 'ServiceManagerView/List.aspx';
                    break;
                default:
                    u = 'View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/Agreement.aspx/ig, u);
            window.parent.location = url;
        }

        /////////////////////////////////////////////////////////////协议变更  by  yeshuangshuang 2021-02-04  ///////////////////////////////

        function Apply() {

            //判断是否已存在变更协议申请（状态不为已生效或作废）
            if ('<%=this.Model.AgreementChangeApply == null%>' == 'True') {
                $('#AgencyFeePeriodType').combobox('enable');
                $('#TaxPeriodType').combobox('enable');
                $('#IncidentalPeriodType').combobox('enable');
                $('#StartDate').datebox('enable');
                $('#EndDate').datebox('enable');
                $('#PreAgency').textbox('enable');
                $('#PreAgency').textbox('readonly', false);	// 禁用只读模式
                $('#AgencyRate').textbox('enable');
                $('#AgencyRate').textbox('readonly', false);	// 禁用只读模式
                $('#MinAgencyFee').textbox('enable');
                $('#MinAgencyFee').textbox('readonly', false);	// 禁用只读模式
                $('#InvoiceRate').combobox('enable');
                $('#InvoiceType').combobox('enable');
                $('#AgencyFeeExchangeRateType').combobox('enable');
                $('#TaxExchangeRateType').combobox('enable');
                $('#IncidentalExchangeRateType').combobox('enable');
                $('#btnApply').hide();
                $('#btnSaveApply').show();
                $('#TaxDaysLimit').textbox('enable');
                $('#TaxDaysLimit').textbox('readonly', false);
                $('#TaxMonthlyDay').textbox('enable');
                $('#TaxMonthlyDay').textbox('readonly', false);
                $('#TaxUpperLimit').textbox('enable');
                $('#TaxUpperLimit').textbox('readonly', false);
                $('#AgencyFeeDaysLimit').textbox('enable');
                $('#AgencyFeeDaysLimit').textbox('readonly', false);
                $('#AgencyFeeMonthlyDay').textbox('enable');
                $('#AgencyFeeMonthlyDay').textbox('readonly', false);
                $('#AgencyFeeUpperLimit').textbox('enable');
                $('#AgencyFeeUpperLimit').textbox('readonly', false);
                $('#IncidentalDaysLimit').textbox('enable');
                $('#IncidentalDaysLimit').textbox('readonly', false);
                $('#IncidentalMonthlyDay').textbox('enable');
                $('#IncidentalMonthlyDay').textbox('readonly', false);
                $('#IncidentalUpperLimit').textbox('enable');
                $('#IncidentalUpperLimit').textbox('readonly', false);
            }
            else {
                $.messager.alert("消息", "此客户存在协议变更申请，不能再次申请！");
                return;
            }
        }

        //垫款上限不能为0
        function CheckUpperLimitSum() {
            if ($("#TRGoods").css("display") != "none") {
                var goodsUpperLimit = parseInt($("#GoodsUpperLimit").textbox('getValue'));
            }
            if ($("#TRTax").css("display") != "none") {
                var taxUpperLimit = parseInt($("#TaxUpperLimit").textbox('getValue'));
            }
            if ($("#TRAgencyFee").css("display") != "none") {
                var agencyFeeUpperLimit = parseInt($("#AgencyFeeUpperLimit").textbox('getValue'));
            }
            if ($("#TRIncidental").css("display") != "none") {
                var incidentalUpperLimit = parseInt($("#IncidentalUpperLimit").textbox('getValue'));
            }
            if (taxUpperLimit != 0 && agencyFeeUpperLimit != 0 && incidentalUpperLimit != 0 && goodsUpperLimit != 0) {
                return true;
            }
            else {
                return false;
            }

        }

        function validateUpperLimitSum(upperLimit) {
            var taxUpperLimit = $("#TRTax").css("display") == "none" ? 0 : parseInt($("#TaxUpperLimit").textbox('getValue'));
            var agencyFeeUpperLimit = $("#TRAgencyFee").css("display") == "none" ? 0 : parseInt($("#AgencyFeeUpperLimit").textbox('getValue'));
            var incidentalUpperLimit = $("#TRIncidental").css("display") == "none" ? 0 : parseInt($("#IncidentalUpperLimit").textbox('getValue'));
            var upperLimitSum = taxUpperLimit + agencyFeeUpperLimit + incidentalUpperLimit;
            if (upperLimitSum != 0) {
                if (upperLimitSum > upperLimit) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        }
        //客户等级对应的垫款上限
        function getUpperLimit(Rank) {
            switch (Rank) {
                case '一级':
                    upperLimit = 500000;
                    break;
                case '二级':
                    upperLimit = 300000;
                    break;
                case '三级':
                    upperLimit = 200000;
                    break;
                case '四级':
                    upperLimit = 150000;
                    break;
                case '五级':
                    upperLimit = 50000;
                    break;
                default:
                    upperLimit = 0;
                    break;
            }
            return upperLimit;
        }

        function SaveApply() {
            if (!Valid("form1")) {
                return;
            }
            //垫款上限不能为0 
            if (!CheckUpperLimitSum()) {
                var message = "垫款上限不能为 0"
                $.messager.alert("消息", message);
                return;
            }
            if (window.parent.frames.Source == "Add") {
                //会员等级
                var Rank = "九级";

                if (ID != "") {
                    Rank = '<%=this.Model.ClientRank%>';
                }
                var upperLimit = getUpperLimit(Rank);
                if (!validateUpperLimitSum(upperLimit)) {
                    var message = "您的客户等级是" + Rank + ",垫款上限为" + upperLimit;
                    $.messager.alert("消息", message);
                    return;
                }
            }
            //end

            //填写备注信息
            $("#approve-tip").show();
            $("#AdditionSummary").textbox('textbox').attr("disabled", false);
            $('#AdditionSummary').textbox('textbox').attr('readonly', false);
            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 220,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#AdditionSummary").textbox('getValue');
                        reason = reason.trim();
                        if (reason == "") {
                            $.messager.alert('提示', "备注不能为空！");
                            return;
                        }
                        MaskUtil.mask();//遮挡层
                        $("div[class*=window-mask]").css('z-index', '9005');
                        var values = FormValues("form1");
                        values["ID"] = ID;
                        values["reason"] = reason;

                        //提交后台
                        $.post('?action=SaveClientAgreementApply',
                            { Model: JSON.stringify(values) }, function (res) {
                                MaskUtil.unmask();//关闭遮挡层
                                var result = JSON.parse(res);//
                                if (result.success) {
                                    var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                        NormalClose();
                                        //MaskUtil.unmask();
                                        document.location.reload();

                                    });
                                    alert1.window({
                                        modal: true, onBeforeClose: function () {
                                            NormalClose();
                                        }
                                    });
                                } else {
                                    $.messager.alert('提示', result.message, 'info', function () {

                                    });
                                }
                            });
                    }
                }, {
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    </script>
</head>
<body>
    <form id="form1">
        <div style="margin: 8px;">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'">保存</a>
            <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'">导出协议</a>
            <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
            <a id="btnApply" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="Apply()">协议变更</a>
            <a id="btnSaveApply" href="javascript:void(0);" class="easyui-linkbutton " data-options="iconCls:'icon-save'" onclick="SaveApply()">提交协议变更</a>
        </div>
        <div>
            <div class="divAll">
                <span class="spanTitle"></span>
                <div class="divContent" style="border: 0">
                    <table class="oprationTable" style="margin: 10px; width: 100%">
                        <tr>
                            <th style="width: 12%"></th>
                            <th style="width: 20%"></th>
                            <th style="width: 12%"></th>
                            <th style="width: 20%"></th>
                            <th style="width: 12%"></th>
                            <th style="width: 20%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">开始日期：</td>
                            <td>
                                <input class="easyui-datebox" id="StartDate" data-options="valueField:'value',textField:'text',editable:false,required:true" style="width: 150px" />
                            </td>
                            <td class="lbl">结束日期：</td>
                            <td>
                                <input class="easyui-datebox" id="EndDate" data-options="valueField:'value',textField:'text',editable:false,required:true" style="width: 150px" />
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td class="lbl">代理费率：</td>
                            <td>
                                <input class="easyui-textbox" id="AgencyRate"
                                    data-options="required:true,validType:'agencyrate'" style="width: 150px" />
                            </td>

                            <td class="lbl">基础代理费：</td>
                            <td>
                                <input class="easyui-textbox" id="PreAgency"
                                    data-options="validType:'numbercheck'" style="width: 150px" />
                            </td>
                            <td class="lbl">最低代理费：</td>
                            <td>
                                <input class="easyui-textbox" id="MinAgencyFee"
                                    data-options="required:true,validType:'numbercheck'" style="width: 150px" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td class="lbl">换汇方式：</td>
                            <td colspan="2">
                                <span class="radioSpan">
                                    <input type="radio" class="ircheckbox" id="IsLimitNinetyDays" name="IsPrePayExchange" value="0">90天内换汇</input>
                                    <input type="radio" class="ircheckbox" id="IsPrePayExchange" name="IsPrePayExchange" value="1">预换汇</input>
                                </span>
                            </td>
                            <td class="lbl">换汇汇率：</td>
                            <td colspan="2">
                                <input class="easyui-combobox" id="PEIsTen"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 150px" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>


            <!-- 货款start -->
            <div class="divAll">
                <span class="spanTitle">货款条款</span>
                <div class="divContent">
                    <table class="oprationTable" style="margin: 10px; width: 100%">
                        <tr>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 30%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">结算方式：</td>
                            <td>
                                <input class="easyui-combobox" id="GoodsPeriodType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                        </tr>
                        <tr id="TRGoods">
                            <td class="lbl">
                                <span name="SPGoodsLimit">约定期限(天)：
                                </span>
                                <span name="SPGoodsMonthly">结算日期(次月)：
                                </span>
                            </td>
                            <td>
                                <span name="SPGoodsLimit">
                                    <input class="easyui-textbox" id="GoodsDaysLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                                </span>
                                <span name="SPGoodsMonthly">
                                    <input class="easyui-textbox" id="GoodsMonthlyDay" data-options="validType:'numbercheck'" style="width: 200px;" />
                                </span>
                            </td>
                            <td class="lbl">垫款上限(元)：</td>
                            <td>
                                <input class="easyui-textbox" id="GoodsUpperLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">汇率类型：</td>
                            <td>
                                <input class="easyui-combobox" id="GoodsExchangeRateType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                            <td class="lbl">
                                <span name="SPGoodsAgreeRate">约定汇率：
                                </span>
                            </td>
                            <td>
                                <span name="SPGoodsAgreeRate">
                                    <input class="easyui-textbox" id="GoodsExchangeRateValue" data-options="validType:'exchangerate'" style="width: 200px" />
                                </span>

                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <!-- 货款end -->

            <!-- 税款start -->
            <div class="divAll">
                <span class="spanTitle">税款条款</span>
                <div class="divContent">
                    <table class="oprationTable" style="margin: 10px; width: 100%">
                        <tr>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 30%"></th>
                        </tr>

                        <tr>
                            <td class="lbl">结算方式：</td>
                            <td>
                                <input class="easyui-combobox" id="TaxPeriodType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                        </tr>
                        <tr id="TRTax">
                            <td class="lbl">
                                <span name="SPTaxLimit">约定期限(天)：
                                </span>
                                <span name="SPTaxMonthly">结算日期(次月)：
                                </span>
                            </td>
                            <td>
                                <span name="SPTaxLimit">
                                    <input class="easyui-textbox" id="TaxDaysLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                                </span>
                                <span name="SPTaxMonthly">
                                    <input class="easyui-textbox" id="TaxMonthlyDay" data-options="validType:'numbercheck'" style="width: 200px;" />
                                </span>
                            </td>
                            <td class="lbl">垫款上限(元)：</td>
                            <td>
                                <input class="easyui-textbox" id="TaxUpperLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">汇率类型：</td>
                            <td>
                                <input class="easyui-combobox" id="TaxExchangeRateType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                            <td class="lbl">
                                <span name="SPTaxAgreeRate">约定汇率：
                                </span>
                            </td>
                            <td>
                                <span name="SPTaxAgreeRate">
                                    <input class="easyui-textbox" id="TaxExchangeRateValue" data-options="validType:'exchangerate'" style="width: 200px" />
                                </span>

                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <!-- 税款end -->


            <!-- 代理费start -->
            <div class="divAll">
                <span class="spanTitle">代理费条款</span>
                <div class="divContent">
                    <table class="oprationTable" style="margin: 10px; width: 100%">
                        <tr>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 30%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">结算方式：</td>
                            <td>
                                <input class="easyui-combobox" id="AgencyFeePeriodType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                        </tr>
                        <tr id="TRAgencyFee">
                            <td class="lbl">
                                <span name="SPAgencyFeeLimit">约定期限(天)：
                                </span>
                                <span name="SPAgencyFeeMonthly">结算日期(次月)：
                                </span>
                            </td>
                            <td>
                                <span name="SPAgencyFeeLimit">
                                    <input class="easyui-textbox" id="AgencyFeeDaysLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                                </span>
                                <span name="SPAgencyFeeMonthly">
                                    <input class="easyui-textbox" id="AgencyFeeMonthlyDay" data-options="validType:'numbercheck'" style="width: 200px;" />
                                </span>
                            </td>
                            <td class="lbl">垫款上限(元)：</td>
                            <td>
                                <input class="easyui-textbox" id="AgencyFeeUpperLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">汇率类型：</td>
                            <td>
                                <input class="easyui-combobox" id="AgencyFeeExchangeRateType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                            <td class="lbl">
                                <span name="SPAgencyFeeAgreeRate">约定汇率：
                                </span>
                            </td>
                            <td>
                                <span name="SPAgencyFeeAgreeRate">
                                    <input class="easyui-textbox" id="AgencyFeeExchangeRateValue" data-options="validType:'exchangerate'" style="width: 200px" />
                                </span>

                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <!-- 代理费end -->

            <!-- 杂费start -->
            <div class="divAll">
                <span class="spanTitle">杂费条款</span>
                <div class="divContent">
                    <table class="oprationTable" style="margin: 10px; width: 100%">
                        <tr>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 30%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">结算方式：</td>
                            <td>
                                <input class="easyui-combobox" id="IncidentalPeriodType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                        </tr>
                        <tr id="TRIncidental">
                            <td class="lbl">
                                <span name="SPIncidentalLimit">约定期限(天)：
                                </span>
                                <span name="SPIncidentalMonthly">结算日期(次月)：
                                </span>
                            </td>
                            <td>
                                <span name="SPIncidentalLimit">
                                    <input class="easyui-textbox" id="IncidentalDaysLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                                </span>
                                <span name="SPIncidentalMonthly">
                                    <input class="easyui-textbox" id="IncidentalMonthlyDay" data-options="validType:'numbercheck'" style="width: 200px;" />
                                </span>
                            </td>
                            <td class="lbl">垫款上限(元)：</td>
                            <td>
                                <input class="easyui-textbox" id="IncidentalUpperLimit" data-options="validType:'numbercheck'" style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">汇率类型：</td>
                            <td>
                                <input class="easyui-combobox" id="IncidentalExchangeRateType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                            <td class="lbl">
                                <span name="SPIncidentalAgreeRate">约定汇率：
                                </span>
                            </td>
                            <td>
                                <span name="SPIncidentalAgreeRate">
                                    <input class="easyui-textbox" id="IncidentalExchangeRateValue" data-options="validType:'exchangerate'" style="width: 200px" />
                                </span>

                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <!-- 杂费end -->

            <!--开票方式-->
            <div class="divAll">
                <span class="spanTitle"></span>
                <div class="divContent" style="border: 0">
                    <table class="oprationTable" style="margin: 10px; width: 100%">
                        <tr>
                            <th style="width: 11%"></th>
                            <th style="width: 8%"></th>
                            <th style="width: 10%"></th>
                            <th style="width: 30%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">开票类型：</td>
                            <td>
                                <input class="easyui-combobox" id="InvoiceType"
                                    data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                            </td>
                            <td class="lbl">对应税点：</td>
                            <td>
                                <span name="SPInvoiceAdd">
                                    <input class="easyui-textbox" id="InvoiceRateAdd" value="13%" readonly="true" style="width: 200px" />
                                </span>
                                <span name="SPInvoiceRate">
                                    <input class="easyui-combobox" id="InvoiceRate"
                                        data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                                </span>

                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td class="lbl">摘要备注：</td>
                            <td colspan="3">
                                <input class="easyui-textbox" id="Summary" data-options="validType:'length[1,200]',tipPosition:'bottom',multiline:true" style="width: 550px; height: 60px;" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td class="lbl">服务协议：</td>
                            <td colspan="3">
                                <div id="ServiceAgreement" style="width: 200px"></div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </form>
    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="approve-tip" style="padding: 15px; display: none;">
            <div>
                <label>备注：</label>
            </div>
            <div style="margin-top: 3px;">
                <input id="AdditionSummary" class="easyui-textbox" data-options="required:true,multiline:true," style="width: 300px; height: 62px;" />
            </div>
        </div>
    </div>
</body>
</html>
