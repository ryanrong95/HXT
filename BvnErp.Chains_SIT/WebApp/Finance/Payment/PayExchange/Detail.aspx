<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Finance.Payment.PayExchange.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>

        var NoticeData = eval('(<%=this.Model.NoticeData%>)');
        var PayExchangeApplyData = eval('(<%=this.Model.PayExchangeApplyData%>)');
        var ProxyFileData = eval('(<%=this.Model.ProxyFileData%>)');
        var ProductFeeLimitData = eval('(<%=this.Model.ProductFeeLimitData%>)');
        var FatherId = '';
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                scrollbarSize: 0,
                onLoadSuccess: function (data) {
                    var total1 = 0;
                    var total2 = 0;
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        total1 = total1 + Number(row.ReceivableAmount)
                        total2 = total2 + Number(row.ReceivedAmount)
                    }
                    $("#ReceivableAmountTotal").text(total1.toFixed(2));
                    $("#ReceivedAmountTotal").text(total2.toFixed(2));
                    var leftTrs = $(".datagrid-view1>.datagrid-body tr");
                    var rightTrs = $(".datagrid-view2>.datagrid-body tr");

                    for (var i = 0; i < leftTrs.length; i++) {
                        var useHeight = 0;

                        if ($(leftTrs[i]).height() > $(rightTrs[i]).height()) {
                            useHeight = $(leftTrs[i]).height();
                        } else {
                            useHeight = $(rightTrs[i]).height();
                        }

                        $(leftTrs[i]).height(useHeight);
                        $(rightTrs[i]).height(useHeight);
                    }

                    var heightValue = $("#datagrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 60;
                    $("#datagrid").prev().find(".datagrid-body").height(heightValue);
                    $("#datagrid").prev().height(heightValue);
                    $("#datagrid").prev().parent().height(heightValue);
                    $("#datagrid").prev().parent().parent().height(heightValue);
                }
            });
            $('#datagrid_file').myDatagrid({
                actionName: 'filedata',
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#fileContainer').panel('setTitle', '合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainer");
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
            var ApplyID = getQueryString("ApplyID");
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
            Init();
            AdjustPanel();
            $.post('?action=isCanDelivery', { ID: ApplyID }, function (data) {
                var Result = JSON.parse(data);
                if (Result) {
                    var htmlstr = '<span  style="color: red">' + Result.GetOrderId + '</span>';
                    $("#OverDuePayment").html(Result.OverDuePayment ? "超期" + "(" + htmlstr + ")" : "未超期");
                }

            });
        });
        //初始化供应商信息
        function Init() {
            if (PayExchangeApplyData != null) {
                $('#SupplierName').text(PayExchangeApplyData.SupplierName);
                if (PayExchangeApplyData.SupplierAddress != null) {
                    $('#SupplierAddress').text(PayExchangeApplyData.SupplierAddress);
                }
                $('#SupplierEnglishName').text(PayExchangeApplyData.SupplierEnglishName);
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
                $('#ExchangeRateType').text(PayExchangeApplyData.ExchangeRateType + ' ' + PayExchangeApplyData.ExchangeRate);
                //$('#ExchangeRate').text(PayExchangeApplyData.ExchangeRate);
                $('#Price').text(PayExchangeApplyData.Price + "(" + PayExchangeApplyData.Currency + ")");
                $('#RmbPrice').text(PayExchangeApplyData.RmbPrice + "(RMB)");
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
                //if (ProductFeeLimitData.RemainAdvances != null && ProductFeeLimitData.RemainAdvances != "") {
                //    $("#RemainAdvances").text(ProductFeeLimitData.RemainAdvances.toFixed(4));
                //}
            }
            if (NoticeData != null) {
                $("#Payer").text(NoticeData.Payer);
                $("#PaySummary").text(NoticeData.Summary);
            }

            FatherId = PayExchangeApplyData.FatherID;
            if (FatherId != '') {
                $("#IsFather").text('拆分付汇,实收金额');

                //查询拆分前付汇实收
                $.post('?action=FatherReceipts', { FatherID: FatherId  }, function (data) {
                    var Result = JSON.parse(data);
                    if (Result) {
                        $("#IsFather").text('拆分付汇,实收金额' + Result.Amount);
                    }

                });
            }
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
        //返回
        function Back() {
            var url = location.pathname.replace(/Detail.aspx/ig, 'ApprovedList.aspx');
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
                var url = location.pathname.replace(/Detail.aspx/ig, 'ApprovedList.aspx');
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
                                            <label id="lbldyjid" style="padding-left:20px;font-size: 12px;"></label>
                                            <label id="IsFather" style="padding-left:20px;font-size: 12px;"></label>
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
                                        <td class="lbl lab" id="BankName"></td>
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
                        <table id="datagrid" title="付汇订单" style="width: 100%;" data-options="
                            fitColumns:true,
                            fit:false,
                            pagination:false,
                            scrollbarSize:0">
                            <thead>
                                <tr>
                                    <th data-options="field:'OrderID',width: 70,align:'center'">订单编号</th>
                                    <th data-options="field:'CreateDate',width: 50,align:'center'">申请时间</th>
                                    <th data-options="field:'Currency',width: 30,align:'center'">币种</th>
                                    <th data-options="field:'DeclarePrice',width: 40,align:'center'">报关总价</th>
                                    <th data-options="field:'PaidPrice',width: 40,align:'center'">已付汇金额</th>
                                    <th data-options="field:'Amount',width: 50,align:'center'">本次申请金额</th>
                                    <th data-options="field:'ReceivableAmount',width: 50,align:'center'">应收货款(RMB)</th>
                                    <th data-options="field:'ReceivedAmount',width: 50,align:'center'">实收货款(RMB)</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div style="margin-top: 5px; width: 100%;">
                        <table id="table1" style="width: 100%;">
                            <tr>
                                <td style="vertical-align: top; width: 50%">
                                    <div id="fileContainer" title="合同发票(INVOICE LIST)" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'200px',">
                                        <div class="sub-container">
                                            <div id="unUpload" style="margin-left: 5px">
                                                <p>未上传</p>
                                            </div>
                                            <div>
                                                <table id="datagrid_file" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
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
                                            <div>
                                                <table class="file-info">
                                                    <tr>
                                                        <td rowspan="2">
                                                            <img src="../../../App_Themes/xp/images/wenjian.png" />
                                                        </td>
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
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-top: 5px;">
                        <div class="easyui-panel" title="审批信息">
                            <div class="sub-container">
                                <form id="form1">
                                    <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="lbl">审批时间：</td>
                                            <td>
                                                <label class="lbl" id="Merchandiser"></label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">付款人：</td>
                                            <td>
                                                <label class="lbl" id="Payer"></label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">审批备注：</td>
                                            <td>
                                                <label class="lbl" id="PaySummary"></label>
                                            </td>
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
