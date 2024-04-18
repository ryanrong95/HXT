<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="WebApp.Finance.Payment.PayExchange.Approval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>

        var PayExchangeApplyData = eval('(<%=this.Model.PayExchangeApplyData%>)');
        var ProxyFileData = eval('(<%=this.Model.ProxyFileData%>)');
        var ProductFeeLimitData = eval('(<%=this.Model.ProductFeeLimitData%>)');
        var AvailableProductFee = '<%=this.Model.AvailableProductFee%>';
        var CurrentName = '<%=this.Model.CurrentName%>';
        var ApplyID = getQueryString("ApplyID");
        var FatherId = '';
        var ConfirmBackUrl = '';
        //页面加载时
        $(function () {
            window.grid = $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                scrollbarSize: 0,
                actionName: 'data',
                onLoadSuccess: function (data) {
                    var total1 = 0;
                    var total2 = 0;
                    var dyjids = '';
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        total1 = total1 + Number(row.ReceivableAmount);
                        total2 = total2 + Number(row.ReceivedAmount);
                        dyjids += row.DyjIDs + ',';
                    }
                    $("#ReceivableAmountTotal").text(total1.toFixed(2));
                    $("#ReceivedAmountTotal").text(total2.toFixed(2));

                    var ids = $.unique(dyjids.split(',').sort());
                    //if (ids.length > 0) {
                    //    $("#lbldyjid").text($.unique(ids.sort()));
                    //}

                    var a = []; //用于接收去除重复元满后的数组
                    for (var i = 0; i < ids.length; i++) { //循环遍历数组
                        if (jQuery.inArray(ids[i], a) < 0) { //判farr数组 中的元者是否存在于a数组神
                            a.push(ids[i]); //不存在则将该元病存放Fa数组中
                        }
                    }
                    $("#lbldyjid").text(a);

                }
            });
            $('#datagrid_file').myDatagrid({
                actionName: 'filedata',
                nowrap: false,
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    console.log("loadFiilter", data)
                    $('#fileContainer').panel('setTitle', '合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;

                },
                onLoadSuccess: function (data) {
                    console.log("onLoadSuccess", data)
                    var panel = $('#fileContainer').panel();
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });
                    $("#unUpload").next().find(".datagrid-wrap").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(600);
                }
            });

            //绑定日志信息           
            var data = new FormData($('#form1')[0]);
            data.append("ApplyID", ApplyID);
            $.ajax({
                url: '?action=LoadLogs',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    showLogContent(data);
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });

            //验证是否有银行敏感地址 Begin

            $.post('?action=CheckIsSensitive', { ApplyID: ApplyID, }, function (data) {

                $("#btnApproveOK").show();
                $("#btnApproveCancel").show();

                var data = JSON.parse(data);
                if (data.BankIsSensitive) {
                    if (data.BankSensitiveType == '<%=Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Forbid.GetHashCode()%>') {
                        $("#SensitiveBankTip-Forbid").show();
                        $("#SensitiveBankTip-Sensitive").hide();
                    } else if (data.BankSensitiveType == '<%=Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Sensitive.GetHashCode()%>') {
                        $("#SensitiveBankTip-Forbid").hide();
                        $("#SensitiveBankTip-Sensitive").show();
                    } else {
                        $("#SensitiveBankTip-Forbid").hide();
                        $("#SensitiveBankTip-Sensitive").hide();
                    }
                } else {
                    $("#SensitiveBankTip-Forbid").hide();
                    $("#SensitiveBankTip-Sensitive").hide();
                }
            });

            //验证是否有银行敏感地址 End

            Init();
            AdjustPanel();



        });

        //初始化供应商信息
        function Init() {
            if (PayExchangeApplyData != null) {
                $('#SupplierName').text(PayExchangeApplyData.SupplierName.replace('&#39', '\''));
                if (PayExchangeApplyData.SupplierAddress != null) {
                    $('#SupplierAddress').text(PayExchangeApplyData.SupplierAddress);
                }
                $('#SupplierEnglishName').text(PayExchangeApplyData.SupplierEnglishName.replace('&#39', '\''));
                $('#BankName').text(PayExchangeApplyData.BankName);
                $('#BankAddress').text(PayExchangeApplyData.BankAddress.replace('&#39', '\''));
                $('#BankAccount').text(PayExchangeApplyData.BankAccount);
                $('#SwiftCode').text(PayExchangeApplyData.SwiftCode);
                $('#ABA').text(PayExchangeApplyData.ABA);
                $('#IBAN').text(PayExchangeApplyData.IBAN);
                $('#PaymentType').text(PayExchangeApplyData.PaymentType);
                if (PayExchangeApplyData.ExpectPayDate != null) {
                    $('#ExpectPayDate').text(PayExchangeApplyData.ExpectPayDate);
                }
                if (PayExchangeApplyData.OtherInfo != null) {
                    $('#OtherInfo').text(PayExchangeApplyData.OtherInfo);
                }
                if (PayExchangeApplyData.Summary != null) {
                    $('#Summary').text(PayExchangeApplyData.Summary);
                }
                $('#Merchandiser').text(PayExchangeApplyData.Merchandiser);

                $('#ClientName').text(PayExchangeApplyData.ClientName);
                $('#ClientCode').text(PayExchangeApplyData.ClientCode);
                $('#ApplyDate').text(PayExchangeApplyData.CreateDate);
                $('#PayMode').text(PayExchangeApplyData.PaymentType);
                $('#PayDate').text(PayExchangeApplyData.SettlemenDate);
                //$('#Currency').text("汇率(" + PayExchangeApplyData.Currency + ")");
                $('#ExchangeRateType').text(PayExchangeApplyData.ExchangeRateType);
                //$('#ExchangeRate').text(PayExchangeApplyData.ExchangeRate);
                $('#Price').text(PayExchangeApplyData.Price + "(" + PayExchangeApplyData.Currency + ")");
                $('#RmbPrice').text(PayExchangeApplyData.RmbPrice + "(RMB)");

                FatherId = PayExchangeApplyData.FatherID;
                if (FatherId != null) {
                    $("#IsFather").text('拆分付汇,实收金额');

                    //查询拆分前付汇实收
                    $.post('?action=FatherReceipts', { FatherID: FatherId }, function (data) {
                        var Result = JSON.parse(data);
                        if (Result) {
                            $("#IsFather").text('拆分付汇,实收金额' + Result.Amount);
                        }

                    });
                }

                <%--if (PayExchangeApplyData.BankIsSensitive) {
                    if (PayExchangeApplyData.BankSensitiveType == '<%=Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Forbid.GetHashCode()%>') {
                        $("#SensitiveBankTip-Forbid").show();
                        $("#SensitiveBankTip-Sensitive").hide();
                    } else if (PayExchangeApplyData.BankSensitiveType == '<%=Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Sensitive.GetHashCode()%>') {
                        $("#SensitiveBankTip-Forbid").hide();
                        $("#SensitiveBankTip-Sensitive").show();
                    } else {
                        $("#SensitiveBankTip-Forbid").hide();
                        $("#SensitiveBankTip-Sensitive").hide();
                    }
                } else {
                    $("#SensitiveBankTip-Forbid").hide();
                    $("#SensitiveBankTip-Sensitive").hide();
                }--%>
            }
            if (ProxyFileData != null) {
                $("#proxyFile").text(ProxyFileData.FileName);
                $('#proxyFile').attr('href', ProxyFileData.WebUrl);
                $("#fileFormat").text(ProxyFileData.FileFormat);
                $("#fileUrl").text(ProxyFileData.Url);
                $("#fileID").text(ProxyFileData.ID);
            }
            if (ProductFeeLimitData != null) {
                //$("#PeriodType").text(ProductFeeLimitData.PeriodType);
                //if (ProductFeeLimitData.RemainAdvances != null) {
                //    $("#RemainAdvances").text(Number(ProductFeeLimitData.RemainAdvances).toFixed(4));
                //}
            }

            $.post('?action=isCanDelivery', { ID: ApplyID }, function (data) {
                var Result = JSON.parse(data);
                if (Result) {
                    var htmlstr = '<span  style="color: red">' + Result.GetOrderId + '</span>';
                    $("#OverDuePayment").html(Result.OverDuePayment ? "超期" + "(" + htmlstr + ")" : "未超期");
                }

            });
        }
        //查看付汇委托书
        function Look() {
            var fileUrl = $('#proxyFile').attr("href");

            $('#viewfileImg').css("display", "none");
            $('#viewfilePdf').css("display", "none");
            if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', fileUrl);
                $('#viewfilePdf').css("display", "block");

            }
            else {
                $('#viewfileImg').attr('src', fileUrl);
                $('#viewfileImg').css("display", "block");
            }
            $("#viewFileDialog").window('open').window('center');
        }
        //预览文件
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }
        //审批通过
        function Approval() {
            var PeriodType = ProductFeeLimitData.PeriodType;
            var Amount = $("#ReceivedAmountTotal").html();//实收
            var PaidAmount = $("#ReceivableAmountTotal").html();//应收

            //张庆永 有特殊权限 ryan 20210122 电话要求
            if (CurrentName != '张庆永' && CurrentName != '张令金' && FatherId =='') {
                if (PeriodType == "预付款" && Number(Amount) == 0) {
                    $.messager.alert('提示', "预付款且未收到货款，不能审批通过。");
                    return;
                }

                //判断是否是约定期限且此申请是垫款类型  by 2020-12-28 yess
                if (PeriodType == "约定期限" && PayExchangeApplyData.IsAdvanceMoney == 0) {
                    if (Number(PayExchangeApplyData.RmbPrice) > AvailableProductFee) {//Number(PaidAmount) - Number(Amount) 
                        $.messager.alert('提示', "约定期限且垫款金额不足，不能审批通过。");
                        return;
                    }
                }

                //判断是否是约定期限且此申请不是垫款类型  by 2020-12-28 yess
                if (PayExchangeApplyData.IsAdvanceMoney != 0) {
                    if (Number(PaidAmount) == 0) {
                        $.messager.alert('提示', "未收到货款，不能审批通过。");
                        return;
                    }
                }
            }

            var ReceivableAmountTotal = $("#ReceivableAmountTotal").html();
            var ReceivedAmountTotal = $("#ReceivedAmountTotal").html();

            var url = location.pathname.replace(/Approval.aspx/ig, 'Confirm.aspx')
                + "?ID=" + ApplyID + "&IsPass=true&ClientID=" + PayExchangeApplyData.ClientID
                + "&ReceivableAmountTotal=" + ReceivableAmountTotal
                + "&ReceivedAmountTotal=" + ReceivedAmountTotal
                + "&WaiBiPrice=" + PayExchangeApplyData.Price
                + "&RmbPrice=" + PayExchangeApplyData.RmbPrice
                + "&IsAdvanceMoney=" + PayExchangeApplyData.IsAdvanceMoney;

            $.myWindow.setMyWindow("Approval2Confirm", window);
            $.myWindow({
                url: url,
                noheader: false,
                title: '审批通过',
                width: '500px',
                height: '300px',
                onClose: function () {
                    //Back();
                    if (null != ConfirmBackUrl && 'undefined' != ConfirmBackUrl && '' != ConfirmBackUrl) {
                        window.location.href = ConfirmBackUrl;
                    }
                }
            });
        }
        //审批退回
        function Cancel() {
            var ReceivableAmountTotal = $("#ReceivableAmountTotal").html();
            var ReceivedAmountTotal = $("#ReceivedAmountTotal").html();

            var url = location.pathname.replace(/Approval.aspx/ig, 'Confirm.aspx')
                + "?ID=" + ApplyID + "&IsPass=false&ClientID=" + PayExchangeApplyData.ClientID
                + "&ReceivableAmountTotal=" + ReceivableAmountTotal
                + "&ReceivedAmountTotal=" + ReceivedAmountTotal
                + "&WaiBiPrice=" + PayExchangeApplyData.Price
                + "&RmbPrice=" + PayExchangeApplyData.RmbPrice
                + "&IsAdvanceMoney=" + PayExchangeApplyData.IsAdvanceMoney;

            $.myWindow.setMyWindow("Approval2Confirm", window);
            $.myWindow({
                url: url,
                noheader: false,
                title: '审批退回',
                width: '500px',
                height: '300px',
                onClose: function () {
                    //Back();
                    if (null != ConfirmBackUrl && 'undefined' != ConfirmBackUrl && '' != ConfirmBackUrl) {
                        window.location.href = ConfirmBackUrl;
                    }

                }
            });
        }
        //返回
        function Back() {
            var url = location.pathname.replace(/Approval.aspx/ig, 'UnApprovedList.aspx');
            window.location = url;
        }
        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }
        //操作
        function FileOperation(val, row, index) {
            var buttons = row.FileName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            return buttons;
        }
        function ShowImg(val, row, index) {
            return "<img src='../../../App_Themes/xp/images/wenjian.png' />";
        }
        function AdjustPanel() {
            //修改并排的两个 panel 的高度
            var par1Height = $("#fileContainer").parent().height();
            var par2Height = $("#para-panel-2").parent().height();

            if (par1Height != par2Height) {
                if (par1Height > par2Height) {
                    $('#para-panel-2').panel('resize', {
                        height: par1Height
                    });
                } else {
                    $('#fileContainer').panel('resize', {
                        height: par2Height
                    });
                }
            }
        }
    </script>
    <script>
        //约定示例
        top.$.backLiebiao = {
            kkk: 1,
            Ruturn: function () {
                var url = location.pathname.replace(/Approval.aspx/ig, 'UnApprovedList.aspx');
                window.location = url;
            }
        };
    </script>
    <style>
        html {
            height: 100%;
        }

        body {
            min-height: 100%;
        }

        .lab {
            word-break: break-all;
        }
    </style>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="审批付汇申请" style="display: none; padding: 5px;">
            <div data-options="region:'north',border:false," style="height: 41px; overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a id="btnApproveOK" href="javascript:void(0);" class="easyui-linkbutton" onclick="Approval()"
                        data-options="iconCls:'icon-ok'" style="display: none;">审批通过</a>
                    <a id="btnApproveCancel" href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()"
                        data-options="iconCls:'icon-cancel'" style="display: none;">审批退回</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()"
                        data-options="iconCls:'icon-back'">返回</a>
                </div>
            </div>
            <div data-options="region:'west',border: false," style="width: 30%; float: left;">
                <div class="sec-container">
                    <div>
                        <div class="easyui-panel" title="客户信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">申请日期：</td>
                                        <td>
                                            <label class="lbl" id="ApplyDate"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">客户编号：</td>
                                        <td>
                                            <label class="lbl" id="ClientCode"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">客户名称：</td>
                                        <td>
                                            <label class="lbl" id="ClientName"></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div style="margin-top: 5px;">
                        <div class="easyui-panel" title="付款信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <%--<tr>
                                        <td class="lbl">账期类型：</td>
                                        <td>
                                            <label class="lbl" id="PeriodType"></label>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td class="lbl">付款方式：</td>
                                        <td>
                                            <label class="lbl" id="PayMode"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付汇金额：</td>
                                        <td>
                                            <label class="lbl" id="Price"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款金额：</td>
                                        <td>
                                            <label class="lab" id="RmbPrice"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl" id="Currency">汇率类型：</td>
                                        <td>
                                            <label class="lbl" id="ExchangeRateType"></label>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td class="lbl">剩余可垫货款(RMB)：</td>
                                        <td>
                                            <label class="lab" id="RemainAdvances"></label>
                                        </td>
                                    </tr>--%>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div style="margin-top: 5px;">
                        <div class="easyui-panel" title="应收实收总额（RMB）">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">应收总额：</td>
                                        <td>
                                            <label class="lbl" id="ReceivableAmountTotal">0</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">实收总额：</td>
                                        <td>
                                            <label class="lbl" id="ReceivedAmountTotal">0</label>
                                            <label id="lbldyjid" style="padding-left:20px;font-size: 12px;color: brown;"></label>
                                            <label id="IsFather" style="padding-left:20px;font-size: 12px;color: brown;"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">税代费超期</td>
                                        <td>
                                            <label class="lbl" id="OverDuePayment"></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div style="margin-top: 5px;">
                        <div class="easyui-panel" title="付汇供应商">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">供应商名称：</td>
                                        <td class="lbl lab" id="SupplierName"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">供应商地址：</td>
                                        <td class="lbl lab" id="SupplierAddress"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">英文名称：</td>
                                        <td class="lbl lab" id="SupplierEnglishName"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行名称：</td>
                                        <td class="lbl lab">
                                            <label id="BankName"></label>
                                            <label id="SensitiveBankTip-Forbid" style="display: none; color: red;">此银行涉及禁止地区，请仔细核实</label>
                                            <label id="SensitiveBankTip-Sensitive" style="display: none; color: red;">此银行涉及敏感地区，请仔细核实</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行地址：</td>
                                        <td class="lbl lab" id="BankAddress"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行账号：</td>
                                        <td class="lbl lab" id="BankAccount"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行代码：</td>
                                        <td class="lbl lab" id="SwiftCode"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">ABA(付美国必填)：</td>
                                        <td class="lbl lab" id="ABA"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">IBAN(付欧盟必填)：</td>
                                        <td class="lbl lab" id="IBAN"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">其他相关资料：</td>
                                        <td class="lbl lab" id="OtherInfo"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">备注：</td>
                                        <td class="lbl lab" id="Summary"></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 68%; float: left; margin-left: 2px;">
                <div class="sec-container">
                    <div style="width: 100%;">
                        <table id="datagrid" title="付汇订单" style="width: 100%;">
                            <thead>
                                <tr>
                                    <th data-options="field:'OrderID',width: 170,align:'center'">订单编号</th>
                                    <th data-options="field:'CreateDate',width: 150,align:'center'">申请时间</th>
                                    <th data-options="field:'Currency',width: 130,align:'center'">币种</th>
                                    <th data-options="field:'DeclarePrice',width: 140,align:'center'">报关总价</th>
                                    <th data-options="field:'PaidPrice',width: 140,align:'center'">已付汇金额</th>
                                    <th data-options="field:'Amount',width: 150,align:'center'">本次申请金额</th>
                                    <th data-options="field:'ReceivableAmount',width: 150,align:'center'">应收货款(RMB)</th>
                                    <th data-options="field:'ReceivedAmount',width: 150,align:'center'">实收货款(RMB)</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div style="margin-top: 5px; width: 100%;">
                        <table id="table1" style="width: 100%;">
                            <tr>
                                <td style="vertical-align: top; width: 50%">
                                    <div id="fileContainer" title="合同发票(INVOICE LIST)" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'auto'," style="height: 200px">
                                        <div class="sub-container">
                                            <div id="unUpload" style="margin-left: 5px">
                                                <p>未上传</p>
                                            </div>
                                            <div>
                                                <table id="datagrid_file" style="width: 100%; height: auto">
                                                    <thead>
                                                        <tr>
                                                            <th data-options="field:'img',formatter:ShowImg">图片</th>
                                                            <th style="width: auto" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                                        </tr>
                                                    </thead>
                                                </table>
                                            </div>
                                            <div class="text-container" style="margin-top: 10px;">
                                                <p>仅限图片或pdf格式的文件,并且不超过500kb</p>
                                            </div>

                                        </div>
                                    </div>
                                </td>
                                <td style="padding-left: 3px; vertical-align: top;">
                                    <div id="para-panel-2" class="easyui-panel" title="付汇委托书" data-options="iconCls:'icon-blue-fujian', height:'auto'," style="height: 180px">
                                        <div class="sub-container">
                                            <form id="form2">
                                                <div>
                                                    <table class="file-info">
                                                        <tr>
                                                            <td rowspan="2">
                                                                <img src="../../../App_Themes/xp/images/wenjian.png" /></td>
                                                            <td id="proxyFile"></td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <a href="#" onclick="Look();return false"><span>预览</span></a>
                                                                <label id="fileFormat" style="display: none"></label>
                                                                <label id="fileUrl" style="display: none"></label>
                                                                <label id="fileID" style="display: none"></label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="text-container" style="margin-top: 10px;">
                                                    <p>导出pdf格式文件后，交给客户盖章后上传；</p>
                                                    <p>仅限图片或pdf格式的文件,并且不超过500k</p>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>

                    </div>
                    <div style="margin-top: 5px; display: none">
                        <div class="easyui-panel" title="审批信息">
                            <div class="sub-container">
                                <form id="form1">
                                    <table class="row-info" style="width: 100%;">
                                        <tr>
                                            <td class="lbl">跟单员：</td>
                                            <td>
                                                <label class="lbl" id="Merchandiser"></label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">付款人：</td>
                                            <td class="lbl" id="Payer"></td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">审批备注：</td>
                                            <td class="lbl" id="PaySummary"></td>
                                        </tr>
                                    </table>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div style="margin-top: 5px; margin-left: 2px;">
                        <div class="easyui-panel" title="日志记录" style="width: 100%;">
                            <div class="sub-container">
                                <div class="text-container" id="LogContent">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
