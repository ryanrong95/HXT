﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="WebApp.Finance.Invoice.Confirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var InvoiceData = eval('(<%=this.Model.InvoiceData%>)');
        var MaileDate = eval('(<%=this.Model.MaileDate%>)');
        var OtherData = eval('(<%=this.Model.OtherData%>)');

        $(function () {
            $('#productGrid').myDatagrid({
                rownumbers: true,
                fitColumns: false,
                fit: false,
                nowrap: false,
                pagination: false,
                onClickRow: onClickRow,
                onAfterEdit: onAfterEdit,
                onCancelEdit: onCancelEdit,
                nowrap: false,
                actionName: 'ProductData',
                onLoadSuccess: function (data) {
                    $('#productGrid').datagrid('appendRow', {
                        Amount: data.totaldata.Amount,
                        Difference: data.totaldata.Difference,
                        Quantity: data.totaldata.Quantity,
                        SalesTotalPrice: data.totaldata.SalesTotalPrice,
                        InvoiceNo: "",
                    });

                    //"合计"标题
                    setHejiTitle();

                    //"其它信息"中"含税金额"显示(已经这个为准,不要使用OtherData中的,因为后来加了含税金额的计算方法)
                    $("#NoticeAmount").text(data.totaldata.Amount);

                    var heightValue = $("#productGrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 60;
                    $("#productGrid").prev().find(".datagrid-body").height(heightValue);
                    $("#productGrid").prev().height(heightValue);
                    $("#productGrid").prev().parent().height(heightValue);
                    $("#productGrid").prev().parent().parent().height(heightValue);

                },
            });
            Init();

            //绑定日志信息
            var id = getQueryString("ID");
            var data = new FormData($('#form1')[0]);
            data.append("ID", id);
            MaskUtil.mask();
            $.ajax({
                url: '?action=LoadInvoiceLogs',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    MaskUtil.unmask();
                    showLogContent(data);
                },
                error: function (msg) {
                    MaskUtil.unmask();
                    alert("ajax连接异常：" + msg);
                }
            });
        });

        function Init() {
            if (InvoiceData != null) {
                $("#InvoiceType").text(InvoiceData.InvoiceType);
                $("#DeliveryType").text(InvoiceData.DeliveryType);
                $("#CompanyName").text(InvoiceData.CompanyName);
                $("#TaxCode").text(InvoiceData.TaxCode);
                $("#BankInfo").text(InvoiceData.BankInfo);
                $("#AddressTel").text(InvoiceData.AddressTel);
            };
            if (MaileDate != null) {
                $("#ReceipCompany").text(MaileDate.ReceipCompany);
                $("#ReceiterName").text(MaileDate.ReceiterName);
                $("#ReceiterTel").text(MaileDate.ReceiterTel);
                $("#DetailAddres").text(MaileDate.DetailAddres);
                $("#WaybillCode").text(MaileDate.WaybillCode);
            }
            if (OtherData != null) {
                //$("#NoticeAmount").text(OtherData.Amount);
                $("#NoticeDifference").text(OtherData.Difference);
                $("#Summary").text(OtherData.Summary);
            }
        }

        //完成开票
        function ConfirmInvoice() {

            //结束表单编辑状态
            endEditing()

            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证 发票代码、发票号码
            if (!VerifyOneInfo('InvoiceCode')) {
                $.messager.alert('提示', "发票代码不能为空");
                return;
            }
            if (!VerifyInvoiceCodeFormat()) {
                return;
            }
            if (!VerifyOneInfo('InvoiceNo')) {
                $.messager.alert('提示', "发票号码不能为空");
                return;
            }
            if (!VerifyInvoiceNoFormat()) {
                return;
            }
            if (!VerifyOneInfo('InvoiceDate')) {
                $.messager.alert('提示', "开票日期不能为空");
                return;
            }
            if (!VerifyInvoiceDateFormat()) {
                return;
            }

            //提交表单
            var data = new FormData($('#form1')[0]);
            var products = $('#productGrid').datagrid('getRows');
            var realProducts = [];
            for (var i = 0; i < products.length - 1; i++) {
                realProducts.push(products[i]);
            }

            var InvoiceNoticeID = getQueryString("ID");

            CalcInvoiceNumber(InvoiceNoticeID, realProducts, function () {
                data.append('Data', JSON.stringify(realProducts));
                data.append('InvoiceNoticeID', InvoiceNoticeID);
                //data.append("InvoiceTypeValue",InvoiceData.InvoiceTypeValue)
                MaskUtil.mask();
                $.ajax({
                    url: '?action=ConfirmInvoice',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        MaskUtil.unmask();
                        if (res.success) {
                            $.messager.alert('', res.message, 'info', function () {
                                Back();
                            });
                        } else {
                            $.messager.alert('提示', res.message);
                        }
                    }
                }).done(function (res) {
                    //if (res.success) {
                    //    $.messager.alert('', res.message, 'info', function () {
                    //        Back();
                    //    });
                    //} else {
                    //    $.messager.alert('提示', res.message);
                    //}
                });
            });
        }

        // 计算发票号
        function CalcInvoiceNumber(InvoiceNoticeID, realProducts, callback) {
            $.post('?action=CalcInvoiceNumber', {
                InvoiceNoticeID: InvoiceNoticeID,
                Data: JSON.stringify(realProducts),
            }, function (res) {
                var resJson = JSON.parse(res);
                if (resJson.success) {
                    var content = '';
                    content += '<div';
                    content += '<h3>总共<strong style="font-size:25px; color:red;">' + resJson.invoicecount + '</strong>张发票，请您确认：</h3>';
                    content += '<table style="border-collapse: collapse; border: 1px solid black; width: 100%;">';
                    content += '<tr><th>发票代码</th><th>发票号码</th><th>开票日期</th></tr>';
                    for (var i = 0; i < resJson.invoiceinfos.length; i++) {
                        content += '<tr><td style="border: 1px solid black;">' + resJson.invoiceinfos[i].InvoiceCode
                            + '</td><td style="border: 1px solid black;">'
                            + resJson.invoiceinfos[i].InvoiceNo
                            + '</td><td style="border: 1px solid black;">'
                            + resJson.invoiceinfos[i].InvoiceDate + '</td></tr>';
                    }
                    content += '</table></div>';

                    $("#comfirm-dialog-content").html(content);
                    $('#comfirm-dialog').dialog({
                        title: '提示',
                        width: 550,
                        height: 350,
                        closed: false,
                        modal: true,
                        buttons: [{
                            id: 'btn-submit-comfirm-ok',
                            text: '确定',
                            width: 70,
                            handler: function () {
                                $('#comfirm-dialog').dialog('close');
                                if (callback != null) {
                                    callback();
                                }
                            }
                        }, {
                            id: 'btn-submit-comfirm-cancel',
                            text: '取消',
                            width: 70,
                            handler: function () {
                                $('#comfirm-dialog').dialog('close');
                            }
                        }],
                    });
                    $('#comfirm-dialog').window('center');
                } else {
                    $.messager.alert('提示', res.message);
                }

            });
        }

        //打印发票运单
        function PrintWaybill() {
            //结束表单编辑状态
            endEditing()
            //验证 发票代码、发票号码
            if (!VerifyOneInfo('InvoiceCode')) {
                $.messager.alert('提示', "发票代码不能为空");
                return;
            }
            if (!VerifyInvoiceCodeFormat()) {
                return;
            }
            if (!VerifyOneInfo('InvoiceNo')) {
                $.messager.alert('提示', "发票号码不能为空");
                return;
            }
            if (!VerifyInvoiceNoFormat()) {
                return;
            }
            if (!VerifyOneInfo('InvoiceDate')) {
                $.messager.alert('提示', "开票日期不能为空");
                return;
            }
            if (!VerifyInvoiceDateFormat()) {
                return;
            }

            var InvoiceNoticeID = getQueryString("ID");
            var url = location.pathname.replace(/Confirm.aspx/ig, 'PrintInvoice.aspx?IDs=' + InvoiceNoticeID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印发票运单',
                width: '750px',
                height: '550px',
                onClose: function () {
                    //刷新页面
                    location.reload();
                }
            });
        }

        //打印确认单
        function Print() {
            //结束表单编辑状态
            endEditing()
            //验证 发票代码、发票号码
            if (!VerifyOneInfo('InvoiceCode')) {
                $.messager.alert('提示', "发票代码不能为空");
                return;
            }
            if (!VerifyInvoiceCodeFormat()) {
                return;
            }
            if (!VerifyOneInfo('InvoiceNo')) {
                $.messager.alert('提示', "发票号码不能为空");
                return;
            }
            if (!VerifyInvoiceNoFormat()) {
                return;
            }
            if (!VerifyOneInfo('InvoiceDate')) {
                $.messager.alert('提示', "开票日期不能为空");
                return;
            }
            if (!VerifyInvoiceDateFormat()) {
                return;
            }

            var InvoiceNoticeID = getQueryString("ID");
            var url = location.pathname.replace(/Confirm.aspx/ig, 'PrintInvoiceConfirm.aspx?ID=' + InvoiceNoticeID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印确认单',
                width: '750px',
                height: '550px',
                onClose: function () {
                    location.reload();
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/Confirm.aspx/ig, 'InvoicingList.aspx');
            window.location = url;
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + "&nbsp;&nbsp;" + row.CreateDate + "&nbsp;," + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }

    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#productGrid').datagrid('validateRow', editIndex)) {
                var rows = $('#productGrid').datagrid('getRows');

                var oldInvoiceCode = rows[editIndex].InvoiceCode;
                var oldInvoiceNo = rows[editIndex].InvoiceNo;
                var oldInvoiceDate = rows[editIndex].InvoiceDate;

                $('#productGrid').datagrid('endEdit', editIndex);
                $('#productGrid').datagrid('acceptChanges');

                var newInvoiceCode = rows[editIndex].InvoiceCode;
                if (newInvoiceCode != oldInvoiceCode) {
                    UpdateInvoiceCode(editIndex);
                }
                var newInvoiceNo = rows[editIndex].InvoiceNo;
                if (newInvoiceNo != oldInvoiceNo) {
                    UpdateInvoiceNo(editIndex);
                }
                var newInvoiceDate = rows[editIndex].InvoiceDate;
                if (newInvoiceDate != oldInvoiceDate) {
                    UpdateInvoiceDate(editIndex);
                }

                editIndex = undefined;

                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            var dataLength = $("#productGrid-container .datagrid-cell-rownumber").length;
            if (index == (dataLength - 1)) {
                return;
            }

            if (editIndex != index) {
                if (endEditing()) {
                    $('#productGrid').datagrid('selectRow', index)
                    $('#productGrid').datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#productGrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                var rows = $('#productGrid').datagrid('getRows');

                var oldInvoiceCode = rows[editIndex].InvoiceCode;
                var oldInvoiceNo = rows[editIndex].InvoiceNo;
                var oldInvoiceDate = rows[editIndex].InvoiceDate;

                $('#productGrid').datagrid('acceptChanges');

                var newInvoiceCode = rows[editIndex].InvoiceCode;
                if (newInvoiceCode != oldInvoiceCode) {
                    UpdateInvoiceCode(editIndex);
                }
                var newInvoiceNo = rows[editIndex].InvoiceNo;
                if (newInvoiceNo != oldInvoiceNo) {
                    UpdateInvoiceNo(editIndex);
                }
                var newInvoiceDate = rows[editIndex].InvoiceDate;
                if (newInvoiceDate != oldInvoiceDate) {
                    UpdateInvoiceDate(editIndex);
                }

                editIndex = undefined;
            }

            //"合计"标题
            setHejiTitle();
        }

        //验证发票代码/发票号码/开票日期是否为空
        function VerifyOneInfo(fieldName) {
            $('#productGrid').datagrid('acceptChanges');
            var rows = $('#productGrid').datagrid('getRows');

            for (var i = 0; i < rows.length - 1; i++) {
                var row = rows[i];
                if (row[fieldName] == "" || row[fieldName] == null) {
                    return false;
                }
            }
            return true;
        }

        //验证发票代码格式
        function VerifyInvoiceCodeFormat() {
            $('#productGrid').datagrid('acceptChanges');
            var rows = $('#productGrid').datagrid('getRows');

            var allOk = true;
            var showmessage = '';
            for (var i = 0; i < rows.length - 1; i++) {
                var InvoiceCode = rows[i].InvoiceCode;
                InvoiceCode = InvoiceCode.trim();
                if (!(/(^[1-9]\d*$)/.test(InvoiceCode))) {
                    showmessage += '第' + (i + 1) + '行发票代码 ' + InvoiceCode + ' 格式不正确。<br>';
                    allOk = false;
                }
            }

            if (allOk == false) {
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '提示',
                    width: '340',
                    height: '220',
                    content: '<div style="overflow-y:scroll; height: 180px;">' + showmessage + '</div>',
                    onClose: function () {

                    }
                });
            }
            return allOk;
        }

        //验证发票号码格式
        function VerifyInvoiceNoFormat() {
            $('#productGrid').datagrid('acceptChanges');
            var rows = $('#productGrid').datagrid('getRows');

            var allOk = true;
            var showmessage = '';
            for (var i = 0; i < rows.length - 1; i++) {
                var InvoiceNo = rows[i].InvoiceNo;
                InvoiceNo = InvoiceNo.trim();

                //第一步验证
                if (!(/(^\d+([-,]\d+)*(\d)*$)/.test(InvoiceNo))) {
                    showmessage += '第' + (i + 1) + '行发票号码 ' + InvoiceNo + ' 格式不正确。<br>';
                    allOk = false;
                    continue;
                }

                //第二部验证, 根据逗号切开之后, 查看每个部分格式是否正确
                var invoiceNosByComma = InvoiceNo.split(","); // 根据逗号切开
                var isByCommaOk = true;
                for (var j = 0; j < invoiceNosByComma.length; j++) {
                    if (!(/^\d+([-]\d+)?$/.test(invoiceNosByComma[j]))) {
                        isByCommaOk = false;
                        break;
                    }
                    if (invoiceNosByComma[j].indexOf('-') > -1) {
                        var numBegin = invoiceNosByComma[j].split("-")[0];
                        var numEnd = invoiceNosByComma[j].split("-")[1];
                        if (!(parseInt(numBegin) < parseInt(numEnd))) {
                            isByCommaOk = false;
                            break;
                        }
                    }
                }
                if (isByCommaOk == false) {
                    showmessage += '第' + (i + 1) + '行发票号码 ' + InvoiceNo + ' 格式不正确。<br>';
                    allOk = false;
                    continue;
                }
            }

            if (allOk == false) {
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '提示',
                    width: '340',
                    height: '220',
                    content: '<div style="overflow-y:scroll; height: 180px;">' + showmessage + '</div>',
                    onClose: function () {

                    }
                });
            }
            return allOk;
        }

        //验证开票日期格式
        function VerifyInvoiceDateFormat() {
            $('#productGrid').datagrid('acceptChanges');
            var rows = $('#productGrid').datagrid('getRows');

            var allOk = true;
            var showmessage = '';
            for (var i = 0; i < rows.length - 1; i++) {
                var InvoiceDate = rows[i].InvoiceDate;
                InvoiceDate = InvoiceDate.trim();
                if (!(/(^\d{4}-\d{2}-\d{2}$)/.test(InvoiceDate))) {
                    showmessage += '第' + (i + 1) + '行开票日期 ' + InvoiceDate + ' 格式不正确。<br>';
                    allOk = false;
                }
            }

            if (allOk == false) {
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '提示',
                    width: '340',
                    height: '220',
                    content: '<div style="overflow-y:scroll; height: 180px;">' + showmessage + '</div>',
                    onClose: function () {

                    }
                });
            }
            return allOk;
        }

        //更新发票代码到数据库
        function UpdateInvoiceCode(Index) {
            UpdateOneInfo(Index, 'InvoiceCode', 'UpdateInvoiceCode');
        }

        //更新发票号码到数据库
        function UpdateInvoiceNo(Index) {
            UpdateOneInfo(Index, 'InvoiceNo', 'UpdateInvoiceNo');
        }

        //更新开票日期到数据库
        function UpdateInvoiceDate(Index) {
            UpdateOneInfo(Index, 'InvoiceDate', 'UpdateInvoiceDate');
        }

        //更新发票代码/发票号码/开票日期到数据库
        function UpdateOneInfo(Index, fieldName, urlAction) {
            //提交表单
            var InvoiceNoticeID = getQueryString("ID");
            var data = new FormData($('#form1')[0]);
            var products = $('#productGrid').datagrid('getRows');
            var product = products[Index];
            var select = new Array();
            for (var i = Index; i < products.length - 1; i++) {
                products[i][fieldName] = product[fieldName];
                select.push(products[i]);
                $('#productGrid').datagrid('refreshRow', i);
            }

            data.append('Data', JSON.stringify(select));
            data.append('InvoiceNoticeID', InvoiceNoticeID);
            $.ajax({
                url: '?action=' + urlAction,
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                }
            });
        }

        //"合计"标题
        function setHejiTitle() {
            var rownumberCells = $("#productGrid-container .datagrid-cell-rownumber");
            if (rownumberCells.length > 0) {
                $(rownumberCells[rownumberCells.length - 1]).html("合计");

                $("#productGrid-container tr[datagrid-row-index=" + (rownumberCells.length - 1) + "] td[field=InvoiceNo]").html("")
            }
        }

        function onAfterEdit() {
            setHejiTitle();
        }

        function onCancelEdit() {
            setHejiTitle();
        }
    </script>
    <style type="text/css">
        table.row-info tr td:first-child {
            width: 100px;
        }

        #productGrid-container .datagrid-header-rownumber, #productGrid-container .datagrid-cell-rownumber {
            width: 50px;
        }
    </style>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="开票确认" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ConfirmInvoice()">完成开票</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="PrintWaybill()">打印发票运单</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="Print()">打印确认单</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left;">
                <div class="sec-container">
                    <div style="display: block; float: left; width: 48%">
                        <div id="panel1" class="easyui-panel" title="开票信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>
                                            <label>开票类型：</label></td>
                                        <td>
                                            <label id="InvoiceType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>交付方式：</label></td>
                                        <td>
                                            <label id="DeliveryType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>公司名称：</label></td>
                                        <td>
                                            <label id="CompanyName"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>纳税人识别号：</label></td>
                                        <td>
                                            <label id="TaxCode"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>开户行及账号：</label></td>
                                        <td>
                                            <label id="BankInfo"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>地址 电话：</label></td>
                                        <td>
                                            <label id="AddressTel"></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div style="display: block; float: left; width: 51%; margin-left: 5px;">
                        <div id="panel2" class="easyui-panel" title="邮寄信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>收件单位：</td>
                                        <td>
                                            <label id="ReceipCompany"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>收件人：</td>
                                        <td>
                                            <label id="ReceiterName"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>手机号：</td>
                                        <td>
                                            <label id="ReceiterTel"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>详细邮寄地址：</td>
                                        <td>
                                            <label id="DetailAddres"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>发票运单：</td>
                                        <td>
                                            <label id="WaybillCode"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <label></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left; margin-top: 5px;">
                <div class="sec-container">
                    <div id="panel3" class="easyui-panel" title="其它信息">
                        <div class="sub-container">
                            <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td>含税金额：</td>
                                    <td>
                                        <label id="NoticeAmount"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>开票差额：</td>
                                    <td>
                                        <label id="NoticeDifference"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>备注信息：</td>
                                    <td>
                                        <label id="Summary"></label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left; margin-top: 5px;">
                <div data-options="region:'center',border: false,">
                    <div id="productGrid-container" class="sec-container">
                        <form id="form1">
                            <table id="productGrid" class="easyui-datagrid" title="商品信息" data-options="
                                fitColumns:false,
                                fit:false,
                                nowrap:false,
                                pagination:false,
                                onClickRow:onClickRow,
                                onAfterEdit:onAfterEdit,
                                onCancelEdit:onCancelEdit">
                                <thead>
                                    <tr>
                                        <th data-options="field:'UnitName',width: 80,align:'left'">单位</th>
                                        <th data-options="field:'Quantity',width: 100,align:'center'">数量</th>
                                        <th data-options="field:'SalesUnitPrice',width: 100,align:'center'">单价</th>
                                        <th data-options="field:'SalesTotalPrice',width: 100,align:'center'">金额</th>
                                        <th data-options="field:'InvoiceTaxRate',width: 100,align:'center'">税率</th>
                                        <th data-options="field:'UnitPrice',width: 100,align:'center'">含税单价</th>
                                        <th data-options="field:'TaxName',width: 170,align:'left'">税务名称</th>
                                        <th data-options="field:'TaxCode',width: 170,align:'left'">税务编码</th>
                                    </tr>
                                </thead>
                                <thead data-options="frozen:true">
                                    <tr>
                                        <th data-options="field:'Amount',width: 100,align:'center'">含税金额</th>
                                        <th data-options="field:'Difference',width: 80,align:'center'">开票差额</th>
                                        <th data-options="field:'InvoiceCode', width: 160,align:'left',editor:{type:'textbox',options: {validType: 'length[1,50]'}}">发票代码</th>
                                        <th data-options="field:'InvoiceNo', width: 160,align:'left',editor:{type:'textbox',options: {validType: 'length[1,100]'}}">发票号码</th>
                                        <th data-options="field:'InvoiceDate', width: 120,align:'left',editor:{type:'datebox',options: {}}">开票日期</th>
                                        <th data-options="field:'ProductName',width: 170,align:'left'">产品名称</th>
                                        <th data-options="field:'ProductModel',width: 180,align:'left'">规格型号</th>
                                    </tr>
                                </thead>
                            </table>
                        </form>
                    </div>
                    <div style="margin-top: 5px; margin-left: 2px;">
                        <div class="easyui-panel" title="日志记录" style="width: 100%;">
                            <div class="sub-container">
                                <div class="text-container">
                                    <div id="LogContent" title="日志记录">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="comfirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="comfirm-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>
</body>
</html>
