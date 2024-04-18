<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Client.Control.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
       <link href="../../Scripts/js/css/viewer.css" rel="stylesheet" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
        <script src="../../Scripts/js/js/viewer.js"></script>
    <script src="../../Scripts/Ccs.js"></script>
    <title>风控审核</title>
    <style>
       
        #client p span {
            /*width: 50%;*/
            display: inline-block;
            margin-bottom: 12px;
        }

        #client {
            width: 60%;
            display: inline-block;
        }

        .table-a table {
            border-right: 1px solid;
            border-bottom: 1px solid
        }

            .table-a table td {
                border-left: 1px solid;
                border-top: 1px solid;
                line-height: 25px;
            }
                    .viewer-transition {
            z-index: 9015 !important;
        }
    </style>


</head>
<script>
    var clientInfoData = eval('(<%=this.Model.ClientInfoData%>)');
    var serviceFileData = eval('(<%=this.Model.serviceFile%>)');
    var businessLicenceFile = eval('(<%=this.Model.businessLicenceFile%>)');


    $(function () {
        initDom();
        initfile();



        var ID = getQueryString("ID");
        var data = new FormData($('#form4')[0]);
        data.append("ID", ID);
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




    });

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

    function initDom() {
        $('#name').html(clientInfoData.CompanyName);
        $('#rank').html(clientInfoData.Rank);
        $('#AgencyRate').html(clientInfoData.ClientAgreementData.AgencyRate);
        $("#minAgency").html(clientInfoData.ClientAgreementData.MinAgencyFee);
        $("#invoiceType").html(clientInfoData.InvoiceType);
        if (clientInfoData.ClientAgreementData.IsPrePayExchange) {
            $("#swapType").html("预换汇");
        }
        if (clientInfoData.ClientAgreementData.IsLimitNinetyDays) {
            $("#swapType").html("90天内换汇");
        }
        if (clientInfoData.ClientAgreementData.IsTen) {
            $("#IsTenType").html("10:00");
        }
        else {
            $("#IsTenType").html("09:30");
        }
        var ClientAgreementData = clientInfoData.ClientAgreementData;
        $("#StartDate").html(clientInfoData.StartDate);
        $("#EndDate").html(clientInfoData.EndDate);

        switch (ClientAgreementData.ProductFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#GoodsPeriodType').html(clientInfoData.GoodsPeriodTypeDec);
                $('#GoodsSettleDate').html("-");
                $('#GoodsUpperLimit').html("-");
                $('#GoodsExchangeRateValue').html(clientInfoData.GoodsExchangeRateTypeDec);
                break;


            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#GoodsPeriodType').html(clientInfoData.GoodsPeriodTypeDec);
                $('#GoodsSettleDate').html(ClientAgreementData.ProductFeeClause.DaysLimit);
                $('#GoodsUpperLimit').html(ClientAgreementData.ProductFeeClause.UpperLimit);
                $('#GoodsExchangeRateValue').html(clientInfoData.GoodsExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#GoodsPeriodType').html(clientInfoData.GoodsPeriodTypeDec);
                $('#GoodsSettleDate').html(ClientAgreementData.ProductFeeClause.MonthlyDay);
                $('#GoodsUpperLimit').html(ClientAgreementData.ProductFeeClause.UpperLimit);
                $('#GoodsExchangeRateValue').html(clientInfoData.GoodsExchangeRateTypeDec);
                break;
        }

        switch (ClientAgreementData.TaxFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#TaxPeriodType').html(clientInfoData.TaxPeriodTypeDec);
                $('#TaxSettleDate').html("-");
                $('#TaxUpperLimit').html("-");
                $('#TaxExchangeRateValue').html(clientInfoData.TaxExchangeRateTypeDec);
                break;
            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#TaxPeriodType').html(clientInfoData.TaxPeriodTypeDec);
                $('#TaxSettleDate').html(ClientAgreementData.TaxFeeClause.DaysLimit);
                $('#TaxUpperLimit').html(ClientAgreementData.TaxFeeClause.UpperLimit);
                $('#TaxExchangeRateValue').html(clientInfoData.TaxExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#TaxPeriodType').html(clientInfoData.TaxPeriodTypeDec);
                $('#TaxSettleDate').html(ClientAgreementData.TaxFeeClause.MonthlyDay);
                $('#TaxUpperLimit').html(ClientAgreementData.TaxFeeClause.UpperLimit);
                $('#TaxExchangeRateValue').html(clientInfoData.TaxExchangeRateTypeDec);
                break;
        }

        switch (ClientAgreementData.AgencyFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#AgencyFeePeriodType').html(clientInfoData.AgencyPeriodType);
                $('#AgencyFeeSettleDate').html("-");
                $('#AgencyFeeDaysLimit').html("-");
                $('#AgencyFeeExchangeRateValue').html(clientInfoData.AgencyExchangeRateTypeDec);
                break;
            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#AgencyFeePeriodType').html(clientInfoData.AgencyPeriodType);
                $('#AgencyFeeSettleDate').html(ClientAgreementData.AgencyFeeClause.DaysLimit);
                $('#AgencyFeeUpperLimit').html(ClientAgreementData.AgencyFeeClause.UpperLimit);
                $('#AgencyFeeExchangeRateValue').html(clientInfoData.AgencyExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#AgencyFeePeriodType').html(clientInfoData.AgencyPeriodType);
                $('#AgencyFeeSettleDate').html(ClientAgreementData.AgencyFeeClause.MonthlyDay);
                $('#AgencyFeeUpperLimit').html(ClientAgreementData.AgencyFeeClause.UpperLimit);
                $('#AgencyFeeExchangeRateValue').html(clientInfoData.AgencyExchangeRateTypeDec);
                break;
        }


        switch (ClientAgreementData.IncidentalFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#IncidentalPeriodType').html(clientInfoData.IncidentalPeriodTypeDec);
                $('#IncidentalSettleDate').html("-");
                $('#IncidentalUpperLimit').html("-");
                $('#IncidentalExchangeRateValue').html(clientInfoData.IncidentalExchangeRateTypeDec);
                break;
            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#IncidentalPeriodType').html(clientInfoData.IncidentalPeriodTypeDec);
                $('#IncidentalSettleDate').html(ClientAgreementData.IncidentalFeeClause.DaysLimit);
                $('#IncidentalUpperLimit').html(ClientAgreementData.IncidentalFeeClause.UpperLimit);
                $('#IncidentalExchangeRateValue').html(clientInfoData.IncidentalExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#IncidentalPeriodType').html(clientInfoData.IncidentalPeriodTypeDec);
                $('#IncidentalSettleDate').html(ClientAgreementData.IncidentalFeeClause.MonthlyDay);
                $('#IncidentalUpperLimit').html(ClientAgreementData.IncidentalFeeClause.UpperLimit);
                $('#IncidentalExchangeRateValue').html(clientInfoData.IncidentalExchangeRateTypeDec);
                break;
        }
    }

    ////提交
    //function Approve(id) {
    //    $("#approve-tip").show();
    //    $("#refuse-tip").hide();
    //    $('#approve-dialog').dialog({
    //        title: '提示',
    //        width: 350,
    //        height: 180,
    //        closed: false,
    //        //cache: false,
    //        modal: true,
    //        closable: true,
    //        buttons: [{
    //            //id: '',
    //            text: '确定',
    //            width: 70,
    //            handler: function () {
    //                MaskUtil.mask();
    //                $("div[class*=window-mask]").css('z-index', '9005');
    //                $.post(location.pathname + '?action=ToAudit', {
    //                    ID: clientInfoData.ID,
    //                }, function (res) {
    //                    MaskUtil.unmask();
    //                    var result = JSON.parse(res);
    //                    if (result.success) {
    //                        var alert1 = $.messager.alert('提示', result.message, 'info', function () {
    //                            NormalClose();

    //                        });
    //                        alert1.window({
    //                            modal: true, onBeforeClose: function () {
    //                                NormalClose();
    //                            }
    //                        });
    //                    } else {
    //                        $.messager.alert('提示', result.message, 'info', function () {

    //                        });
    //                    }
    //                });

    //            }
    //        }, {
    //            //id: '',
    //            text: '取消',
    //            width: 70,
    //            handler: function () {
    //                $('#approve-dialog').window('close');
    //            }
    //        }],
    //    });

    //    $('#approve-dialog').window('center');
    //}


    //整行关闭一系列弹框
    function NormalClose() {
        $('#approve-dialog').window('close');
        $.myWindow.close();
    }

    //function Refuse() {
    //    $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;
    //    $("#approve-tip").hide();
    //    $("#refuse-tip").show();
    //    $("#cancel-tip").hide();

    //    $('#approve-dialog').dialog({
    //        title: '提示',
    //        width: 450,
    //        height: 280,
    //        closed: false,
    //        modal: true,
    //        closable: true,
    //        buttons: [{
    //            text: '确定',
    //            width: 70,
    //            handler: function () {
    //                if (!Valid('form1')) {
    //                    return;
    //                }
    //                var reason = $("#ApproveSummary").textbox('getValue');
    //                    reason = reason.trim();
    //                    $("#ApproveSummary").textbox('setValue', reason);
    //                MaskUtil.mask();
    //                $("div[class*=window-mask]").css('z-index', '9005');
    //                $.post(location.pathname + '?action=Refuse', {
    //                    ID: clientInfoData.ID,
    //                    ApproveSummary: reason,
    //                }, function (res) {
    //                    MaskUtil.unmask();
    //                    var result = JSON.parse(res);
    //                    if (result.success) {
    //                        var alert1 = $.messager.alert('提示', result.message, 'info', function () {
    //                            NormalClose();

    //                        });
    //                        alert1.window({
    //                            modal: true, onBeforeClose: function () {
    //                                NormalClose();
    //                            }
    //                        });
    //                    } else {
    //                        $.messager.alert('提示', result.message, 'info', function () {

    //                        });
    //                    }
    //                });

    //            }
    //        }, {
    //            //id: '',
    //            text: '取消',
    //            width: 70,
    //            handler: function () {
    //                $('#approve-dialog').window('close');
    //            }
    //        }],
    //    });

    //    $('#approve-dialog').window('center');
    //}
    function Cancel() {
        $.myWindow.close();
    }

    function initfile() {
        if (businessLicenceFile != null && businessLicenceFile != "") {
            $("#uploadedbusinessFile").show();
            $("#unUploadbusinessFile").hide();
            $("#businessFile").text(businessLicenceFile.FileName);
            $('#businessFile').attr('href', businessLicenceFile.WebUrl);
            $("#fileFormat").text(businessLicenceFile.FileFormat);
            $("#fileUrl").text(businessLicenceFile.Url);
            $("#fileID").text(businessLicenceFile.ID);
        }
        else {
            $("#unUploadbusinessFile").show();
            $("#uploadedbusinessFile").hide();
        }

        if (serviceFileData != null && serviceFileData != "") {
            $("#uploadserviceFile").show();
            $("#unUploadserviceFile").hide();
            $("#serviceFile").text(serviceFileData.FileName);
            $('#serviceFile').attr('href', serviceFileData.WebUrl);
            $("#fileFormat1").text(serviceFileData.FileFormat);
            $("#fileUrl1").text(serviceFileData.Url);
            $("#fileID1").text(serviceFileData.ID);

              //获取最后一个.的位置
            var index = serviceFileData.FileName.lastIndexOf(".");
            //获取后缀
            var ext = serviceFileData.FileName.substr(index + 1);
            if (ext.toLowerCase().indexOf('docx') != -1 || ext.toLowerCase().indexOf('doc')!= -1)
            {
                $("#view").hide();
            } else {

                  $("#view").show();
            }



        } else {
            $("#unUploadserviceFile").show();
            $("#uploadserviceFile").hide();

        }


    }

    function Look() {
        var fileUrl = $('#businessFile').attr("href");

        $('#viewfileImg').css("display", "none");
        $('#viewfilePdf').css("display", "none");
        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
            $('#viewfilePdf').attr('src', fileUrl);
            $('#viewfilePdf').css("display", "block");

        }
        else {
            $('#viewfileImg').attr('src', fileUrl);
            $('#viewfileImg').css("display", "block");
            viewer = new Viewer(document.getElementById('viewfileImg'), {
                url: 'data-original',
                navbar: false//点开查看的时候不显示缩略图

            });
        }
        $("#viewFileDialog").window('open').window('center');
    }
    //预览文件
    function View(url) {
        var fileUrl = $('#serviceFile').attr("href");
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

    function Dowmload() {
        $("#FileDoc1").attr("href", serviceFileData.WebUrl);
    }
</script>
<body class="easyui-layout">
    <div data-options="region:'center',border: false," style="width: 68%; float: left; margin-left: 2px;">
        <div class="sec-container">
            <div style="margin-top: 5px; width: 100%;">
                <table id="table1" style="width: 100%; padding-right: 0;">
                    <tr>
                        <td style="vertical-align: top; width: 70%">
                            <div id="client" title="会员信息" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'400px',">
                                <div class="sub-container">
                                    <p>
                                        <label>会员名称：</label><span id="name"></span>
                                        <label style="margin-left: 30px">会员等级:</label>
                                        <span id="rank"></span>
                                    </p>
                                    <p>
                                        <label>代理费率:</label>
                                        <span>
                                            <label id="AgencyRate"></label>
                                            <label style="margin-left: 5px;">最低代理费:</label><label id="minAgency"></label>
                                        </span>


                                        <span>
                                            <label style="margin-left: 30px">开票类型：</label>
                                            <label id="invoiceType"></label>
                                        </span>


                                    </p>
                                    <p>
                                        换汇方式:<span id="swapType">0.15%</span>
                                        <label style="margin-left: 30px;">换汇汇率：</label>
                                        <span id="IsTenType"></span>
                                        <label style="margin-left: 30px;">合同期限：</label>
                                        <span id="StartDate"> </span>
                                        <label style="margin: 5px">至</label>
                                        <span id="EndDate"></span>
                                    </p>
                                    <table></table>
                                </div>
                                <div style="width: 100%; padding-left: 2px;" class="table-a">
                                    <table width="95%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="155">条款</td>
                                            <td width="181">结算方式</td>
                                            <td width="182">结算日/天</td>
                                            <td width="182">垫款上线</td>
                                            <td width="198">汇率类型</td>
                                        </tr>
                                        <tr>
                                            <td>货款条款</td>
                                            <td id="GoodsPeriodType"></td>
                                            <td id="GoodsSettleDate"></td>
                                            <td id="GoodsUpperLimit"></td>
                                            <td id="GoodsExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>税款条款</td>
                                            <td id="TaxPeriodType"></td>
                                            <td id="TaxSettleDate"></td>
                                            <td id="TaxUpperLimit"></td>
                                            <td id="TaxExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>代理费条款</td>
                                            <td id="AgencyFeePeriodType"></td>
                                            <td id="AgencyFeeSettleDate"></td>
                                            <td id="AgencyFeeUpperLimit"></td>
                                            <td id="AgencyFeeExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>杂费条款</td>
                                            <td id="IncidentalPeriodType"></td>
                                            <td id="IncidentalSettleDate"></td>
                                            <td id="IncidentalUpperLimit"></td>
                                            <td id="IncidentalExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>代仓储条款</td>
                                            <td id=""></td>
                                            <td id=""></td>
                                            <td id=""></td>
                                            <td id=""></td>
                                        </tr>
                                    </table>
                                </div>

                            </div>


                        </td>
                        <td style="padding-left: 5px; vertical-align: top">
                            <div id="para-panel-2" class="easyui-panel" title="附件" data-options="iconCls:'icon-blue-fujian', height:'400px',">
                                <div class="sub-container">
                                    <form id="form2">
                                        <div>
                                            <label>营业执照：</label>
                                            <table class="file-info" id="uploadedbusinessFile">
                                                <tbody>
                                                    <tr>
                                                        <td rowspan="2">
                                                            <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                        <td id="businessFile"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <a href="#" onclick="Look();return false"><span>预览</span></a>
                                                            <a href="#" runat="server" id="FileDoc1" download=""><span>下载</span></a>
                                                            <label id="fileFormat" style="display: none"></label>
                                                            <label id="fileUrl" style="display: none"></label>
                                                            <label id="fileID" style="display: none"></label>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <table class="file-info" id="unUploadbusinessFile">
                                                <tr>
                                                    <td>
                                                        <a>未上传</a>
                                                    </td>

                                                </tr>
                                            </table>
                                        </div>
                                    </form>
                                </div>

                                <div class="sub-container">
                                    <form id="form3">
                                        <div>
                                            <label>服务协议：</label>
                                            <table class="file-info" id="uploadserviceFile">
                                                <tbody>
                                                    <tr>
                                                        <td rowspan="2">
                                                            <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                        <td id="serviceFile"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <a href="#" id="view"  style="display: none;" onclick="View();return false"><span>预览</span></a>
                                                            <a href="#" runat="server" id="FileDoc2" download=""><span>下载</span></a>
                                                            <label id="fileFormat1" style="display: none"></label>
                                                            <label id="fileUrl1" style="display: none"></label>
                                                            <label id="fileID1" style="display: none"></label>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <table class="file-info" id="unUploadserviceFile">
                                                <tr>
                                                    <td>
                                                        <a>未上传</a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
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


        <div data-options="region:'south',border: false" style="text-align: right; margin-right: 200px; padding: 20px;">
            <span id="btn-cancel" style="margin-left: 15px;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()" data-options="iconCls:'icon-back'">取消</a>
            </span>
        </div>
    </div>



    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 550px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>

    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form id="form1">
            <div id="approve-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定通过该申请吗？</label>
            </div>

            <div id="refuse-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                <div>
                    <label>拒绝原因：</label></div>
                <div style="margin-top: 3px;">
                    <input id="ApproveSummary" class="easyui-textbox" data-options="required:true, multiline:true, validType:'length[0,200]'," style="width: 300px; height: 62px;" />
                </div>
            </div>
        </form>
    </div>

</body>
</html>
