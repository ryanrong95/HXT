<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Approval.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <title>风控审核</title>
    <style>
        #client p span {
            /*width: 50%;*/
            display: inline-block;
            margin-bottom: 12px;
            line-height: 30px;
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
    </style>


</head>
<script>
    var clientInfoData = eval('(<%=this.Model.ClientInfoData%>)');
    var serviceFileData = eval('(<%=this.Model.serviceFile%>)');
    var businessLicenceFile = eval('(<%=this.Model.businessLicenceFile%>)');
    var HKbusinessLicenceFile = eval('(<%=this.Model.HKbusinessLicenceFile%>)');
    var storageAgreementFile = eval('(<%=this.Model.StorageAgreementFile%>)');

    var ServiceManager = eval('(<%=this.Model.ServiceManager%>)');
    var Merchandiser = eval('(<%=this.Model.Merchandiser%>)');
    var ClientAssignData = eval('(<%=this.Model.ClientAssignData%>)');
    var Referrers = eval('(<%=this.Model.Referrers%>)');

    $(function () {

        initDom();

        initfile();

        if (clientInfoData.ClientStatus ==<%=Needs.Ccs.Services.Enums.ClientStatus.Confirmed.GetHashCode()%>) {
            $("#btn-approve").hide();
            $("#btn-refuse").hide();
        } else {
            $("#btn-approve").show();
            $("#btn-refuse").show();
        }


        $("#ServiceManager").combobox({
            data: ServiceManager
        });

        $("#Merchandiser").combobox({
            data: Merchandiser
        });
        $("#Referrers").combobox({
            data: Referrers
        });


        $.each(ClientAssignData, function (index, val) {
            if (val.Type == '<%=Needs.Ccs.Services.Enums.ClientAdminType.Merchandiser.GetHashCode()%>') {
                $("#Merchandiser").combobox('setValue', val.Admin.ID);
                $("#Summary").textbox('setValue', val.Summary);
            }
            else if (val.Type == '<%=Needs.Ccs.Services.Enums.ClientAdminType.ServiceManager.GetHashCode()%>') {
                $("#ServiceManager").combobox('setValue', val.Admin.ID);

            }
        });


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
        if (clientInfoData.ServiceType ==<%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>) {
            $(".dechead").show();
            $(".storage").hide();
            decLoadDecheadData();
        }
        else if (clientInfoData.ServiceType ==<%=Needs.Ccs.Services.Enums.ServiceType.Warehouse.GetHashCode()%>) {

            $(".dechead").hide();
            $(".storage").show();
        } else {

            $(".dechead").show();
            decLoadDecheadData();
            $(".storage").show();

        }
        $("#servicetype").html(clientInfoData.ServiceTypeDes);
        $("#storageType").html(clientInfoData.StorageType);
        $('#name').html(clientInfoData.CompanyName);
        $('#rank').html(clientInfoData.Rank);
        $('#Nature').html(clientInfoData.ClientNature);
        //  $("#ServiceManager").combobox('setValue', clientInfoData.ServiceManager.ID);


    }
    ///加载数据
    function decLoadDecheadData() {
        $('#PreAgency').html(clientInfoData.ClientAgreementData.PreAgency);
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
    //提交
    function Approve(id) {
        if (!$("#form2").form('validate')) {
            return;
        }
        $("#approve-tip").show();
        $("#refuse-tip").hide();
        var MerchandiserID = $("#Merchandiser").combobox('getValue');
        var referrer = $("#Referrers").combobox("getValue");
        $('#approve-dialog').dialog({
            title: '提示',
            width: 300,
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
                    MaskUtil.mask();
                    $("div[class*=window-mask]").css('z-index', '9005');
                    $.post(location.pathname + '?action=Approve', {
                        ID: clientInfoData.ID,
                        MerchandiserID: MerchandiserID,
                        Referrer: referrer
                    }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                NormalClose();

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

    function Refuse() {
        $("#approve-tip").hide();
        $("#refuse-tip").show();
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
                    var reason = $("#ApproveSummary").textbox('getValue');
                    reason = reason.trim();
                    //$("#ApproveSummary").textbox('setValue', reason);
                    MaskUtil.mask();
                    $("div[class*=window-mask]").css('z-index', '9005');
                    $.post(location.pathname + '?action=ApproveRefuse', {
                        ID: clientInfoData.ID,
                        Summary: reason
                    }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                NormalClose();

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


    function Cancel() {
        $.myWindow.close();
    }

    function initfile() {
        if (businessLicenceFile != null && businessLicenceFile != "") {
            $("#uploadedbusinessFile").show();
            $("#unUploadbusinessFile").hide();
            $("#businessFile").text(businessLicenceFile.FileName);
            $('#businessFile').attr('href', businessLicenceFile.WebUrl);
            //$("#FileDoc").attr("href", businessLicenceFile.WebUrl);
            $("#fileFormat").text(businessLicenceFile.FileFormat);
            $("#fileUrl").text(businessLicenceFile.Url);
            $("#fileID").text(businessLicenceFile.ID);
        }
        else {
            $("#unUploadbusinessFile").show();
            $("#uploadedbusinessFile").hide();
        }

        if (HKbusinessLicenceFile != null && HKbusinessLicenceFile != "") {
            $("#uploadHKbusinessFile").show();
            $("#unUploadHKbusinessFile").hide();
            $("#hkbusinessFile").text(HKbusinessLicenceFile.FileName);
            $('#hkbusinessFile').attr('href', HKbusinessLicenceFile.WebUrl);
            $("#fileFormat3").text(HKbusinessLicenceFile.FileFormat);
            $("#fileUrl3").text(HKbusinessLicenceFile.Url);
            $("#fileID3").text(HKbusinessLicenceFile.ID);
        } else {
            $("#unUploadHKbusinessFile").show();
            $("#uploadHKbusinessFile").hide();
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
            if (ext.toLowerCase().indexOf('docx') != -1 || ext.toLowerCase().indexOf('doc') != -1) {
                $("#view").hide();
            } else {

                $("#view").show();
            }

        } else {
            $("#unUploadserviceFile").show();
            $("#uploadserviceFile").hide();

        }
        if (storageAgreementFile != null && storageAgreementFile != "") {
            $("#uploadStorageFile").show();
            $("#unUploadStorageFile").hide();
            $("#storageFile").text(storageAgreementFile.FileName);
            $('#storageFile').attr('href', storageAgreementFile.WebUrl);
            $("#fileFormat2").text(storageAgreementFile.FileFormat);
            $("#fileUrl2").text(storageAgreementFile.Url);
            $("#fileID2").text(storageAgreementFile.ID);
        }
        else {
            $("#unUploadStorageFile").show();
            $("#uploadStorageFile").hide();
        }


    }

    //预览文件
    function View(filetype) {
        var fileUrl = "";
        if (filetype == "businessFile") {
            fileUrl = $('#businessFile').attr("href");
        }
        else if (filetype == "serviceFile") {
            fileUrl = $('#serviceFile').attr("href");

        } else if (filetype == "storageFile") {

            fileUrl = $('#storageFile').attr("href");
        }
        else if (filetype == "hkBusinessFile") {

            fileUrl = $('#hkbusinessFile').attr("href");
        }

        $('#viewfileImg').css('display', 'none');
        $('#viewfilePdf').css('display', 'none');
        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
            $('#viewfilePdf').attr('src', fileUrl);
            $('#viewfilePdf').css("display", "block");
            $('#viewFileDialog').window('open').window('center');
        }
        else if (fileUrl.toLowerCase().indexOf('doc') > 0 || fileUrl.toLowerCase().indexOf('docx') > 0) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = fileUrl;
            a.download = "";
            a.click();
        }
        else {
            $('#viewfileImg').attr('src', fileUrl);
            $('#viewfileImg').css("display", "block");
            $('#viewFileDialog').window('open').window('center');
        }
    }
</script>
<body class="easyui-layout">
    <div data-options="region:'center',border: false," style="width: 68%; float: left; margin-left: 2px;">
        <form id="form2" runat="server">
        <div class="sec-container">

            <div data-options="region:'south',border: false" style="text-align: left; padding: 5px;">
            <span id="btn-approve" style="display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Approve()"  data-options="iconCls:'icon-ok'">同意</a>
            </span>
            <span id="btn-refuse" style="margin-left: 15px; display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'">不同意</a>
            </span>
            <span id="btn-cancel" style="margin-left: 15px;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()" data-options="iconCls:'icon-back'">取消</a>
            </span>
        </div>


            <div style="margin-top: 5px; width: 100%;">
                <table id="table1" style="width: 100%; padding-right: 0;">
                    <tr>
                        <td style="vertical-align: top; width: 70%">
                            <div id="client" title="代报关会员信息" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'400px',">
                                <div class="sub-container">
                                    <p>
                                       <label style="font-weight:bold">会员名称：</label><span id="name"> </span>
                                        <label style="margin-left: 50px;font-weight:bold">会员等级:</label><span  id="rank"></span>
                                         <label  style="margin-left: 50px;font-weight:bold">业务类型：</label> <span id="servicetype"></span>
                                    </p>
                                    
                                    <p class="dechead">
                                       
                                        <label style="font-weight:bold">服务费率:</label>
                                        <span>
                                            <label id="AgencyRate"></label>
                                            <label style="margin-left: 20px; font-weight:bold">基础服务费:</label>
                                            <span id="PreAgency"></span>
                                            <label style="margin-left: 20px;font-weight:bold">最低服务费:</label><label id="minAgency"></label>
                                        </span>


                                        <span>
                                            <label style="margin-left: 20px;font-weight:bold">开票类型：</label>
                                            <label id="invoiceType"></label>
                                        </span>
                                         <label style="margin-left: 20px;font-weight:bold">会员类型:</label> <span id="Nature"></span>


                                    </p>
                                    <p class="dechead">
                                       <label style="font-weight:bold">换汇方式:</label> <span id="swapType">0.15%</span>
                                         <label style="margin-left: 20px;font-weight:bold">换汇汇率：</label>
                                        <span id="IsTenType"></span>
                                        <label style="margin-left: 20px;font-weight:bold">合同期限：</label><span id="StartDate"> </span>
                                        <label style="margin: 5px;font-weight:bold">至</label><span id="EndDate"></span>
                                    </p>
                                     <p class="storage">
                                         <label  style="font-weight:bold">仓储类型：</label> <span id="storageType"></span>
                                    </p>
                                </div>
                                <div style="width: 100%; padding-left: 2px;" class="table-a dechead">
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
                                            <td>服务费条款</td>
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
                            <div id="para-panel-2" class="easyui-panel" title="附件" data-options="iconCls:'icon-blue-fujian', height:'210px'", style="margin-bottom:5px">
                                <div class="sub-container dechead">
                                     
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
                                                            <a href="#" runat="server" id="FileDoc"  onclick="View('businessFile');return false"><span>预览</span></a>
                                                        <%--   <a href="#" id="FileDoc" onclick="downloadFile()"><span>下载</span></a>--%>
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
                                <div class="sub-container dechead">
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
                                                            <a href="#" onclick="View('serviceFile');return false" runat="server" id="FileDoc1"><span>预览</span></a>
                                                           <%-- <a href="#" runat="server" id="FileDoc2"  download=""><span>下载</span></a>--%>
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
               <%--                 <div class="sub-container storage">
                                            <label>仓储协议：</label>
                                            <table class="file-info" id="uploadStorageFile">
                                                <tbody>
                                                    <tr>
                                                        <td rowspan="2">
                                                            <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                        <td id="storageFile"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <a href="#" onclick="View('storageFile');return false"><span>预览</span></a>
                                                         
                                                            <label id="fileFormat2" style="display: none"></label>
                                                            <label id="fileUrl2" style="display: none"></label>
                                                            <label id="fileID2" style="display: none"></label>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <table class="file-info" id="unUploadStorageFile">
                                                <tr>
                                                    <td>
                                                        <a>未上传</a>
                                                    </td>
                                                </tr>
                                            </table>
                                </div>--%>
                                <div class="sub-container storage">
                                            <label>登记证：</label>
                                            <table class="file-info" id="uploadHKbusinessFile">
                                                <tbody>
                                                    <tr>
                                                        <td rowspan="2">
                                                            <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                        <td id="hkbusinessFile"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <a href="#" onclick="View('hkBusinessFile');return false"><span>预览</span></a>
                                                           <%-- <a href="#" runat="server" id="A2"  download=""><span>下载</span></a>--%>
                                                            <label id="fileFormat3" style="display: none"></label>
                                                            <label id="fileUrl3" style="display: none"></label>
                                                            <label id="fileID3" style="display: none"></label>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <table class="file-info" id="unUploadHKbusinessFile">
                                                <tr>
                                                    <td>
                                                        <a>未上传</a>
                                                    </td>
                                                </tr>
                                            </table>
                                </div>
                            </div>
                            <div id="para-panel-3" class="easyui-panel" title="分配代报关业务员" data-options="height:'184px' " >
                                <table id="editTable"   style="line-height:30px;padding-left: 5px;">
                                    <tr>
                                        <td class="lbl" >代报关业务员：</td>
                                        <td>
                                            <input class="easyui-combobox"  data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false,missingMessage:'请选择业务员'" id="ServiceManager" name="ServiceManager" style="width: 150px;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>代报关跟单员：</td>
                                        <td >
                                            <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',required:true, missingMessage:'请选择跟单员'" tipPosition:'top', id="Merchandiser" name="Merchandiser" style="width: 150px;" />
                                        </td>
                                    </tr>
                                       <tr>
                                       <td>代报关引荐人：</td>
                                      <td>
                                     <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:false,editable:false,missingMessage:'请选择引荐人'" id="Referrers" name="Referrers" style="width: 150px;" />
                                   </td>
                                      </tr>
                                </table>
                               
                            </div>
                        </td>
                       
                    </tr>
                    <tr>
                        <td>

                      
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
        
            </form>
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
