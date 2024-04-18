<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="WebApp.Finance.DollarEquityApply.Payment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var DollarEquityApply = eval('(<%=this.Model.DollarEquityApply%>)');
        var PaymentType = eval('(<%=this.Model.PaymentType%>)');
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var CurrentAdminName = '<%=this.Model.CurrentAdminName%>';

        $(function () {
            $("#DollarEquityApplyCreateDate").html(DollarEquityApply.DollarEquityApplyCreateDate);
            $("#DollarEquityApplyExpectDate").html(DollarEquityApply.DollarEquityApplyExpectDate);
            $("#DollarEquityApplyAmount").html(DollarEquityApply.DollarEquityApplyAmount + "(" + DollarEquityApply.DollarEquityApplyCurrency + ")");

            $("#SupplierChnName").html(DollarEquityApply.SupplierChnName);
            $("#BankName").html(DollarEquityApply.BankName);
            $("#BankAddress").html(DollarEquityApply.BankAddress);
            $("#BankAccount").html(DollarEquityApply.BankAccount);
            $("#SwiftCode").html(DollarEquityApply.SwiftCode);
            $("#Payer").textbox('setValue', CurrentAdminName);

            $('#PayType').combobox({
                data: PaymentType,
            });

            $('#FinanceVault').combobox({
                data: FinanceVaultData,
                onSelect: function (record) {
                    $.post('?action=getAccounts', { VaultID: record.Value, Currency: DollarEquityApply.DollarEquityApplyCurrency, }, function (data) {
                        var accounts = JSON.parse(data);
                        $('#FinanceAccount').combobox({
                            data: accounts,
                        });
                    });
                }
            });

            //初始化支付凭证文件上传状态
            if (DollarEquityApply.FileURL != null && DollarEquityApply.FileURL != "") {
                //$("#NoticeFile").text(Notice.PayNoticeFile.FileName);
                $('#NoticeFile').attr('href', DollarEquityApply.FileURL);
                $("#uploadedProxyFile").show();
                $("#unUploadProxyFile").hide();
            }
            else {
                $("#uploadedProxyFile").hide();
                $("#unUploadProxyFile").show();
            }

            //注册文件上传的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if (val == '') {
                        return;
                    }

                    var $this = $(this);
                    //验证文件大小
                    if ($this.next().attr("class").indexOf("textbox-invalid") > 0) {
                        $.messager.alert('提示', '文件大小不能超过500kb！');
                        return;
                    }
                    //验证文件类型
                    var type = $this.filebox('options').accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    var ext = val.substr(val.lastIndexOf(".") + 1);
                    if (type.indexOf(ext.toLowerCase()) < 0) {
                        $this.filebox('setValue', '');
                        $.messager.alert('提示', "请选择" + type + "格式的文件！");
                        return;
                    }

                    var formData = new FormData($('#form1')[0]);
                    $.ajax({
                        url: '?action=UploadFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) { }
                    }).done(function (result) {
                        if (result.success) {
                            var data = result.data;
                            $("#NoticeFile").text(data.FileName);
                            $('#NoticeFile').attr('href', data.WebUrl);
                            $("#fileFormat").text(data.FileFormat);
                            $("#fileUrl").text(data.Url);

                            $("#uploadedProxyFile").show();
                            $("#unUploadProxyFile").hide();
                        }
                        else {
                            $.messager.alert('提示', result.data);
                        }
                    });
                   

                }
            });
        });

        //复制
        function Copy(IdName) {
            var obj = document.getElementById(IdName);
            $("#CopyContext").text(obj.innerText);
            var newobj = document.getElementById("CopyContext")
            newobj.select(); // 选择对象
            document.execCommand("Copy"); // 执行浏览器复制命令
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/Payment.aspx/ig, 'List.aspx');

            window.location.href = url;
        }

        //查看支付凭证
        function LookPay() {
            var fileUrl = $('#NoticeFile').attr("href");

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

        //确认付款
        function Confirm() {
            if (!Valid("form2")) {
                return;
            }

            var SeqNo = $("#SeqNo").textbox("getValue");
            var PayType = $("#PayType").combobox("getValue");
            var FinanceVaultID = $("#FinanceVault").combobox("getValue");
            var FinanceAccountID = $("#FinanceAccount").combobox("getValue");

            var FileName = $('#NoticeFile').text();
            var FileID = $('#fileID').text();
            var FileFormat = $('#fileFormat').text();
            var FileUrl = $('#fileUrl').text();

            if (FileUrl == null || FileUrl == 'undefined' || FileUrl == '') {
                $.messager.alert('提示', '请上传支付凭证！');
                return;
            }
            

            MaskUtil.mask();
            $.post(location.pathname + '?action=Confirm', {
                DollarEquityApplyID: DollarEquityApply.DollarEquityApplyID,
                SeqNo: SeqNo,
                PayType: PayType,
                FinanceVaultID: FinanceVaultID,
                FinanceAccountID: FinanceAccountID,

                FileName: FileName,
                FileID: FileID,
                FileFormat: FileFormat,
                FileUrl: FileUrl,
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert('消息', result.message, '', function () {
                        Back();
                    });
                } else {
                    $.messager.alert('错误', "保存付款错误");
                }
            });
        }
    </script>
    <style>
        #payment-container tr td:first-child {
            width: 90px;
            background-color: #f3f3f3;
        }

        .big-row-one .panel-body {
            height: 138px;
        }

        .big-row-two {
            margin-top: 5px;
        }

        #payeeman-container tr td:first-child {
            width: 110px;
            background-color: #f3f3f3;
        }

        #payeeman-container tr td:nth-child(3) {
            width: 40px;
        }

        .big-row-two .panel-body {
            height: 200px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="tt" class="easyui-tabs" style="width: auto;" data-options="border: false,">
        <div title="付款" style="display: none; padding: 5px;">

            <div data-options="region:'north',border:false," style="height: 41px; overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Confirm()" data-options="iconCls:'icon-save'">确认付款</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()" data-options="iconCls:'icon-back'" style="margin-left: 10px;">返回</a>
                </div>
            </div>

            <div data-options="border: false," style="width: 100%;">
                <div class="sec-container">
                    <div class="big-row-one" style="float: left; width: 30%;">
                        <div class="easyui-panel" title="基本信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">申请日期：</td>
                                        <td>
                                            <label class="lbl" id="DollarEquityApplyCreateDate"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款方式：</td>
                                        <td>
                                            <label class="lbl" id="">转账</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款日期：</td>
                                        <td>
                                            <label class="lbl" id="DollarEquityApplyExpectDate"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款金额：</td>
                                        <td>
                                            <label class="lbl" id="DollarEquityApplyAmount"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">费用类型：</td>
                                        <td>
                                            <label class="lbl" id="">货款</label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="big-row-one" style="float: left; width: calc(40% - 5px); margin-left: 5px;">
                        <div class="easyui-panel" title="付款">
                            <div class="sub-container" id="payment-container">
                                <form id="form2">
                                    <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="lbl">付款流水号：</td>
                                            <td>
                                                <input class="easyui-textbox" id="SeqNo" name="SeqNo" data-options="required:true,validType:'length[1,50]'," style="width: 300px;" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">付款方式：</td>
                                            <td class="lbl">
                                                <input class="easyui-combobox" id="PayType" name="PayType"
                                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:false," style="width: 300px;" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">付款金库：</td>
                                            <td>
                                                <label class="lbl">
                                                    <input class="easyui-combobox" id="FinanceVault" name="FinanceVault"
                                                        data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:false," style="width: 300px;" />
                                                </label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">付款账号：</td>
                                            <td>
                                                <input class="easyui-combobox" id="FinanceAccount" name="FinanceAccount"
                                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:false," style="width: 300px;" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="lbl">付款人：</td>
                                            <td>
                                                <input class="easyui-textbox" id="Payer" name="Payer" data-options="disabled:true," style="width: 300px" />
                                            </td>
                                        </tr>
                                    </table>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div class="big-row-one" style="float: left; width: calc(30% - 5px); margin-left: 5px;">
                        <div class="easyui-panel" title="支付凭证">
                            <div class="sub-container">
                                <form id="form1">
                                    <div>
                                        <table class="file-info" id="uploadedProxyFile">
                                            <tbody>
                                                <tr>
                                                    <td rowspan="2">
                                                        <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                    <td id="NoticeFile"></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <a href="#" onclick="LookPay();return false"><span>预览</span></a>
                                                        <label id="fileFormat" style="display: none"></label>
                                                        <label id="fileUrl" style="display: none"></label>
                                                        <label id="fileID" style="display: none"></label>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <table class="file-info" id="unUploadProxyFile">
                                            <tr>
                                                <td>
                                                    <a>未上传</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="upload" style="margin-top: 10px; margin-left: 5px;">
                                        <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                                    </div>
                                    <div class="text-container" style="margin-top: 10px;">
                                        <p>仅限图片、pdf格式的文件，且pdf文件不超过3M。</p>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div data-options="border: false," style="width: 100%;">
                <div class="sec-container">
                    <div class="big-row-two" style="float: left; width: 50%;">
                        <div class="easyui-panel" title="收款人">
                            <div class="sub-container" id="payeeman-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">收款人：</td>
                                        <td>
                                            <label class="lbl" id="SupplierChnName"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('SupplierChnName')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行名称：</td>
                                        <td>
                                            <label class="lbl" id="BankName"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('BankName')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行地址：</td>
                                        <td>
                                            <label class="lbl" id="BankAddress"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('BankAddress')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行账号：</td>
                                        <td>
                                            <label class="lbl" id="BankAccount"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('BankAccount')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行代码：</td>
                                        <td>
                                            <label class="lbl" id="SwiftCode"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('SwiftCode')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">其他相关资料：</td>
                                        <td colspan="2">
                                            <label class="lbl" id=""></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">备注：</td>
                                        <td colspan="2">
                                            <label class="lbl" id=""></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="big-row-two" style="float: left; width: calc(50% - 5px); margin-left: 5px;">
                        <div class="easyui-panel" title="附件">
                            <div class="sub-container">
                            </div>
                        </div>
                    </div>
                </div>
            </div>


        </div>
    </div>

    <div style="border: none; width: 1px; height: 1px; outline: none; opacity: 0;">
        <textarea id="CopyContext"></textarea>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 600px;">
        <img id="viewfileImg" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>

</body>
</html>
