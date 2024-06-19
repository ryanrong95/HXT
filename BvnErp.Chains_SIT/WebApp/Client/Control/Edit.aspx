<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Control.Edit" %>

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
        /*.big-row-one {
            height: 119px;
        }

        .big-row-three {
            height: 143px;
        }

            .big-row-three .easyui-panel {
                height: calc(143px - 28px);
            }*/

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

    <style>
        .irtd {
            width: 120px;
        }

            .irtd span {
                margin: 10px;
                font-size: larger;
            }

        .irrisk {
            font-size: large;
            margin: 10px;
            font-weight: bold;
        }

            .irrisk .LvH {
                color: red;
            }

            .irrisk .LvM {
                color: orangered;
            }

            .irrisk .LvL {
                color: forestgreen;
            }

        .irtable {
            border-collapse: collapse;
            border-left: 1px solid slategray;
            border-top: 1px solid slategray;
        }

            .irtable td {
                border-right: 1px solid slategray;
                border-bottom: 1px solid slategray;
            }
    </style>

</head>
<script>
    //有两个来源，一个是/Client/Control/List,一个是/Client/RiskIndex
    var Source = getQueryString("Source");
    var clientInfoData = eval('(<%=this.Model.ClientInfoData%>)');
    var serviceFileData = eval('(<%=this.Model.serviceFile%>)');
    var businessLicenceFile = eval('(<%=this.Model.businessLicenceFile%>)');
    var Ranks = eval('(<%=this.Model.ClientRanks%>)');
    var ClientNature = eval('(<%=this.Model.ClientNature%>)');
    var HKbusinessLicenceFile = eval('(<%=this.Model.HKbusinessLicenceFile%>)');
    var storageAgreementFile = eval('(<%=this.Model.StorageAgreementFile%>)');
    $(function () {
        $('#RankComb').combobox({
            data: Ranks,
        });

        $('#NatureComb').combobox({
            data: ClientNature,
        });

        initDom();
        initfile();

        if (Source == "List") {
            if (clientInfoData.ClientStatus ==<%=Needs.Ccs.Services.Enums.ClientStatus.Confirmed.GetHashCode()%>) {
                $("#btn-approve").hide();
                $("#btn-refuse").hide();
            } else {
                $("#btn-approve").show();
                $("#btn-refuse").show();
            }
        } else {
            $("#btn-cancel").hide();
            $("#btn-save").show();
        }

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
        if (clientInfoData.ServiceType ==<%=Needs.Ccs.Services.Enums.ServiceType.Both.GetHashCode()%>) {
            $(".storage").show();
            if (clientInfoData.StorageType ==<%=Needs.Ccs.Services.Enums.StorageType.HKCompany.GetHashCode()%>) {
                $(".hkStorage").show();

            } else {
                $(".hkStorage").hide();
            }
        }
        else {
            $(".storage").hide();
            $(".hkStorage").hide();

        }
        $("#servicetype").html(clientInfoData.ServiceTypeDes);
        $('#name').html(clientInfoData.CompanyName);
        $('#RankComb').combobox("setValue", clientInfoData.Rank);
        $('#NatureComb').combobox("setValue", clientInfoData.ClientNature);
        $('#AgencyRate').html(clientInfoData.ClientAgreementData.AgencyRate);
        $('#PreAgency').html(clientInfoData.ClientAgreementData.PreAgency);
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
                $('#GoodsUpperLimit').numberbox("setValue", "-");
                $('#GoodsUpperLimit').numberbox("disable", true);
                $('#GoodsExchangeRateValue').html(clientInfoData.GoodsExchangeRateTypeDec);
                break;


            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#GoodsPeriodType').html(clientInfoData.GoodsPeriodTypeDec);
                $('#GoodsSettleDate').html(ClientAgreementData.ProductFeeClause.DaysLimit);
                $('#GoodsUpperLimit').numberbox("setValue", ClientAgreementData.ProductFeeClause.UpperLimit);
                $('#GoodsExchangeRateValue').html(clientInfoData.GoodsExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#GoodsPeriodType').html(clientInfoData.GoodsPeriodTypeDec);
                $('#GoodsSettleDate').html(ClientAgreementData.ProductFeeClause.MonthlyDay);
                $('#GoodsUpperLimit').numberbox("setValue", ClientAgreementData.ProductFeeClause.UpperLimit);
                $('#GoodsExchangeRateValue').html(clientInfoData.GoodsExchangeRateTypeDec);
                break;
        }

        switch (ClientAgreementData.TaxFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#TaxPeriodType').html(clientInfoData.TaxPeriodTypeDec);
                $('#TaxSettleDate').html("-");
                $('#TaxUpperLimit').numberbox("setValue", "-");
                $('#TaxUpperLimit').numberbox("disable", true);
                $('#TaxExchangeRateValue').html(clientInfoData.TaxExchangeRateTypeDec);
                break;
            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#TaxPeriodType').html(clientInfoData.TaxPeriodTypeDec);
                $('#TaxSettleDate').html(ClientAgreementData.TaxFeeClause.DaysLimit);
                $('#TaxUpperLimit').numberbox("setValue", ClientAgreementData.TaxFeeClause.UpperLimit);
                $('#TaxExchangeRateValue').html(clientInfoData.TaxExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#TaxPeriodType').html(clientInfoData.TaxPeriodTypeDec);
                $('#TaxSettleDate').html(ClientAgreementData.TaxFeeClause.MonthlyDay);
                $('#TaxUpperLimit').numberbox("setValue", ClientAgreementData.TaxFeeClause.UpperLimit);
                $('#TaxExchangeRateValue').html(clientInfoData.TaxExchangeRateTypeDec);
                break;
        }

        switch (ClientAgreementData.AgencyFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#AgencyFeePeriodType').html(clientInfoData.AgencyPeriodType);
                $('#AgencyFeeSettleDate').html("-");
                $('#AgencyFeeUpperLimit').numberbox("setValue", "-");
                $('#AgencyFeeUpperLimit').numberbox("disable", true);
                $('#AgencyFeeExchangeRateValue').html(clientInfoData.AgencyExchangeRateTypeDec);
                break;
            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#AgencyFeePeriodType').html(clientInfoData.AgencyPeriodType);
                $('#AgencyFeeSettleDate').html(ClientAgreementData.AgencyFeeClause.DaysLimit);
                $('#AgencyFeeUpperLimit').numberbox("setValue", ClientAgreementData.AgencyFeeClause.UpperLimit);
                $('#AgencyFeeExchangeRateValue').html(clientInfoData.AgencyExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#AgencyFeePeriodType').html(clientInfoData.AgencyPeriodType);
                $('#AgencyFeeSettleDate').html(ClientAgreementData.AgencyFeeClause.MonthlyDay);
                $('#AgencyFeeUpperLimit').numberbox("setValue", ClientAgreementData.AgencyFeeClause.UpperLimit);
                $('#AgencyFeeExchangeRateValue').html(clientInfoData.AgencyExchangeRateTypeDec);
                break;
        }


        switch (ClientAgreementData.IncidentalFeeClause.PeriodType) {
            case <%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>:
                $('#IncidentalPeriodType').html(clientInfoData.IncidentalPeriodTypeDec);
                $('#IncidentalSettleDate').html("-");
                $('#IncidentalUpperLimit').numberbox("setValue", "-");
                $('#IncidentalUpperLimit').numberbox("disable", true);
                $('#IncidentalExchangeRateValue').html(clientInfoData.IncidentalExchangeRateTypeDec);
                break;
            case <%=Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode()%>:
                $('#IncidentalPeriodType').html(clientInfoData.IncidentalPeriodTypeDec);
                $('#IncidentalSettleDate').html(ClientAgreementData.IncidentalFeeClause.DaysLimit);
                $('#IncidentalUpperLimit').numberbox("setValue", ClientAgreementData.IncidentalFeeClause.UpperLimit);
                $('#IncidentalExchangeRateValue').html(clientInfoData.IncidentalExchangeRateTypeDec);
                break;

            case <%=Needs.Ccs.Services.Enums.PeriodType.Monthly.GetHashCode()%>:
                $('#IncidentalPeriodType').html(clientInfoData.IncidentalPeriodTypeDec);
                $('#IncidentalSettleDate').html(ClientAgreementData.IncidentalFeeClause.MonthlyDay);
                $('#IncidentalUpperLimit').numberbox("setValue", ClientAgreementData.IncidentalFeeClause.UpperLimit);
                $('#IncidentalExchangeRateValue').html(clientInfoData.IncidentalExchangeRateTypeDec);
                break;
        }
    }

    //提交
    function Approve(id) {
        $("#approve-tip").show();
        $("#refuse-tip").hide();
        $('#approve-dialog').dialog({
            title: '提示',
            width: 450,
            height: 280,
            closed: false,
            //cache: false,
            modal: true,
            closable: true,
            buttons: [{
                //id: '',
                text: '确定',
                width: 70,
                handler: function () {
                    var reason = $("#AdditionSummary").textbox('getValue');
                    reason = reason.trim();
                    MaskUtil.mask();
                    $("div[class*=window-mask]").css('z-index', '9005');
                    $.post(location.pathname + '?action=ToAudit', {
                        ID: clientInfoData.ID,
                        Rank: $('#RankComb').combobox("getValue"),
                        Nature: $('#NatureComb').combobox("getValue"),
                        GoodsUpperLimit: $('#GoodsUpperLimit').numberbox("getValue"),
                        TaxUpperLimit: $('#TaxUpperLimit').numberbox("getValue"),
                        AgencyFeeUpperLimit: $('#AgencyFeeUpperLimit').numberbox("getValue"),
                        IncidentalUpperLimit: $('#IncidentalUpperLimit').numberbox("getValue"),
                        Reason: reason
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

    function Modify(id) {
        $("#approve-tip").show();
        $("#refuse-tip").hide();
        $('#approve-dialog').dialog({
            title: '提示',
            width: 450,
            height: 280,
            closed: false,
            //cache: false,
            modal: true,
            closable: true,
            buttons: [{
                //id: '',
                text: '确定',
                width: 70,
                handler: function () {
                    var reason = $("#AdditionSummary").textbox('getValue');
                    reason = reason.trim();
                    MaskUtil.mask();
                    $("div[class*=window-mask]").css('z-index', '9005');
                    $.post(location.pathname + '?action=ToModify', {
                        ID: clientInfoData.ID,
                        Rank: $('#RankComb').combobox("getValue"),
                        Nature: $('#NatureComb').combobox("getValue"),
                        GoodsUpperLimit: $('#GoodsUpperLimit').numberbox("getValue"),
                        TaxUpperLimit: $('#TaxUpperLimit').numberbox("getValue"),
                        AgencyFeeUpperLimit: $('#AgencyFeeUpperLimit').numberbox("getValue"),
                        IncidentalUpperLimit: $('#IncidentalUpperLimit').numberbox("getValue"),
                        Reason: reason
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
        $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;
        $("#approve-tip").hide();
        $("#refuse-tip").show();
        $("#cancel-tip").hide();

        $('#approve-dialog').dialog({
            title: '提示',
            width: 450,
            height: 280,
            closed: false,
            modal: true,
            closable: true,
            buttons: [{
                text: '确定',
                width: 70,
                handler: function () {
                    if (!Valid('form1')) {
                        return;
                    }
                    var reason = $("#ApproveSummary").textbox('getValue');
                    reason = reason.trim();
                    $("#ApproveSummary").textbox('setValue', reason);
                    MaskUtil.mask();
                    $("div[class*=window-mask]").css('z-index', '9005');
                    $.post(location.pathname + '?action=Refuse', {
                        ID: clientInfoData.ID,
                        ApproveSummary: reason,
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

        } else {
            $("#unUploadserviceFile").show();
            $("#uploadserviceFile").hide();

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
        //仓储协议
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
            viewer = new Viewer(document.getElementById('viewfileImg'), {
                url: 'data-original',
                navbar: false//点开查看的时候不显示缩略图

            });
        }
    }

    function checkCompanyRisk() {
        var companyName = clientInfoData.CompanyName;
        companyName = companyName.trim();
        if (companyName.length <= 0) {
            $.messager.alert('错误', '公司名称');
            return;
        }

        MaskUtil.mask();//遮挡层
        $.post('?action=CheckCompanyRisk', { CompanyName: companyName }, function (res) {
            MaskUtil.unmask();//关闭遮挡层
            var resJson = JSON.parse(res);
            if (!resJson.success) {
                $.messager.alert('错误', resJson.message);
            } else {
                //风险弹框初始化
                var risk = resJson.riskinfo;
                $('#riskdiv').html("");
                switch (risk.riskLevel) {
                    case ('高'):
                        $('#riskdiv').append("<p class=\"irrisk\">风险等级：<span class=\"LvH\">" + risk.riskLevel + "</span></p>");
                        break;
                    case ('中'):
                        $('#riskdiv').append("<p class=\"irrisk\">风险等级：<span class=\"LvM\">" + risk.riskLevel + "</span></p>");
                        break;
                    case ('低'):
                        $('#riskdiv').append("<p class=\"irrisk\">风险等级：<span class=\"LvL\">" + risk.riskLevel + "</span></p>");
                        break;
                    default:
                        $('#riskdiv').append("<p class=\"irrisk\">风险等级：<span class=\"LvM\">" + risk.riskLevel + "</span></p>");
                        break;
                }

                //
                var tbstr = "";
                $('#risktb').html("");
                $.each(risk.riskList, function (index, val) {
                    tbstr += "<tr><td rowspan=\"" + (val.list.length + 1) + "\" class=\"irtd\"><span>" + val.name + val.count + "</span></td>" + (val.list.length == 0 ? "<td></td><td></td>" : "") + "</tr>";

                    //审批时候给出审批人员提示。风险等级，法人信息 严重违法，被执行，限制消费 1.3.5 三项 
                    $.each(val.list, function (index, valitem) {
                        //严重违法:高风险5;警示6;提示信息1
                        var isred = valitem.title == '严重违法' || valitem.title == '失信被执行人（公司）' || valitem.title == '被执行人（公司）';

                        tbstr += "<tr" + (isred ? " style=\"color: red;\"" : "") + "><td style=\"padding: 5px;\">[" + valitem.tag + "]<br>" + valitem.title + valitem.total + "次</td><td>";
                        $.each(valitem.list, function (index, valcell) {
                            tbstr += "<p>" + valcell.title + valcell.riskCount + "次</p>";
                        });
                        tbstr += "</td></tr>";
                    });
                });

                $('#risktb').append(tbstr)

                //风险弹框显示
                $('#riskinfo-dialog').dialog({
                    title: '风险信息',
                    //width: 450,
                    //height: 280,
                    closed: true,
                    //cache: false,
                    modal: true,
                    closable: true,
                    buttons: [{
                        //id: '',
                        text: '确定',
                        width: 70,
                        handler: function () {

                            $('#riskinfo-dialog').window('close');
                        }
                    }],
                });
                $('#riskinfo-dialog').dialog('open');
                $('#riskinfo-dialog').window('center');
            }
        });
    }

</script>
<body class="easyui-layout">
    <div data-options="region:'center',border: false," style="width: 1148px; margin-left: 2px;">
        <div class="sec-container">
            <div style="margin-top: 5px; width: 100%;">
                <table id="table1" style="width: 100%; padding-right: 0;">
                    <tr>
                        <td style="vertical-align: top; width: 800px">
                            <div id="client" title="会员信息" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'400px',width:'800px'">
                                <div class="sub-container">
                                    <p>
                                        <label>会员名称：</label><span id="name"></span>
                                        <a href="javascript:void(0);" onclick="checkCompanyRisk()" style="color: red; cursor: pointer; margin: 0 8px; font: 12px/1.2 Arial,Verdana,'微软雅黑','宋体';">查询风险信息</a>
                                    </p>
                                    <p>
                                        <label>会员等级:</label>
                                        <input class="easyui-combobox" style="width: 100px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true" id="RankComb" />
                                        <label style="margin-left: 30px">会员类型:</label>
                                        <input class="easyui-combobox" style="width: 100px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true" id="NatureComb" />
                                    </p>
                                    <p>
                                        <label>服务费率:</label>
                                        <span>
                                            <label id="AgencyRate"></label>
                                            <label style="margin-left: 30px">基础服务费:</label>
                                            <span id="PreAgency"></span>
                                            <label style="margin-left: 50px;">最低服务费:</label><label id="minAgency"></label>
                                        </span>

                                         
                                        <span>
                                            <label style="margin-left: 30px">开票类型：</label>
                                            <label id="invoiceType"></label>
                                        </span>
                                        <label style="margin-left: 50px;">业务类型：</label>
                                        <span id="servicetype"></span>
                                    </p>
                                    <p>
                                        <label>换汇方式:</label><span id="swapType">0.15%</span>
                                        <label style="margin-left: 30px;">换汇汇率：</label>
                                        <span id="IsTenType"></span>
                                        <label style="margin-left: 50px;">合同期限：</label><span id="StartDate"> </span>
                                        <label style="margin: 10px">至</label><span id="EndDate"></span>
                                    </p>
                                </div>
                                <div style="width: 100%; padding-left: 2px;" class="table-a">
                                    <table width="95%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="155">条款</td>
                                            <td width="181">结算方式</td>
                                            <td width="182">结算日/天</td>
                                            <td width="182">垫款上限</td>
                                            <td width="198">汇率类型</td>
                                        </tr>
                                        <tr>
                                            <td>货款条款</td>
                                            <td id="GoodsPeriodType"></td>
                                            <td id="GoodsSettleDate"></td>
                                            <td>
                                                <input class="easyui-numberbox" id="GoodsUpperLimit" />
                                            </td>
                                            <td id="GoodsExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>税款条款</td>
                                            <td id="TaxPeriodType"></td>
                                            <td id="TaxSettleDate"></td>
                                            <td>
                                                <input class="easyui-numberbox" id="TaxUpperLimit" />
                                            </td>
                                            <td id="TaxExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>服务费条款</td>
                                            <td id="AgencyFeePeriodType"></td>
                                            <td id="AgencyFeeSettleDate"></td>
                                            <td>
                                                <input class="easyui-numberbox" id="AgencyFeeUpperLimit" />
                                            </td>
                                            <td id="AgencyFeeExchangeRateValue"></td>
                                        </tr>
                                        <tr>
                                            <td>杂费条款</td>
                                            <td id="IncidentalPeriodType"></td>
                                            <td id="IncidentalSettleDate"></td>
                                            <td>
                                                <input class="easyui-numberbox" id="IncidentalUpperLimit" />
                                            </td>
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
                            <div id="para-panel-2" class="easyui-panel" title="附件" data-options="iconCls:'icon-blue-fujian', height:'400px',width:'348px'">
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
                                                            <a href="#" onclick="View('businessFile');return false" runat="server" id="FileDoc1"><span>预览</span></a>
                                                            <%--    <a href="#" runat="server" id="FileDoc1" download=""><span>下载</span></a>--%>
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
                                                            <a href="#" runat="server" id="FileDoc2" onclick="View('serviceFile');return false"><span>预览</span></a>
                                                            <%--  <a href="#" runat="server" id="FileDoc2" download=""><span>下载</span></a>--%>
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

                                <div class="sub-container storage">
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
                                                    <%--  <a href="#" runat="server" id="A1"  download=""><span>下载</span></a>--%>
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
                                </div>
                                <div class="sub-container hkStorage">
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
                        </td>
                    </tr>
                </table>
            </div>
            <div style="margin-top: 5px; margin-left: 2px;">
                <div class="easyui-panel" title="日志记录" style="width: 1148px; height: 250px">
                    <div class="sub-container">
                        <div class="text-container" id="LogContent">
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div data-options="region:'south',border: false" style="text-align: right; margin-right: 200px; padding: 20px;">
            <span id="btn-approve" style="display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Approve()" data-options="iconCls:'icon-ok'">通过</a>
            </span>
            <span id="btn-refuse" style="margin-left: 15px; display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'">不通过</a>
            </span>
            <span id="btn-cancel" style="margin-left: 15px;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()" data-options="iconCls:'icon-back'">取消</a>
            </span>
            <span id="btn-save" style="margin-left: 15px; display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Modify()" data-options="iconCls:'icon-ok'">修改</a>
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
                <div>
                    <label>备注：</label>
                </div>
                <div style="margin-top: 3px;">
                    <input id="AdditionSummary" class="easyui-textbox" data-options="multiline:true, validType:'length[0,200]'," style="width: 300px; height: 62px;" />
                </div>
                <label style="font-size: 14px;">确定通过该申请吗？</label>
            </div>

            <div id="refuse-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                <div>
                    <label>拒绝原因：</label>
                </div>
                <div style="margin-top: 3px;">
                    <input id="ApproveSummary" class="easyui-textbox" data-options="required:true, multiline:true, validType:'length[0,200]'," style="width: 300px; height: 62px;" />
                </div>
            </div>
        </form>
    </div>

    <div id="riskinfo-dialog" class="easyui-dialog" title="风险信息" style="width: 900px; height: 550px;"
        data-options="iconCls:'icon-search', resizable:false, modal:true, closed: true,">
        <div style="display: block;">
            <div id="riskdiv" style="display: block;">
            </div>
            <table id="risktb" class="irtable">
            </table>
        </div>
    </div>
</body>
</html>
