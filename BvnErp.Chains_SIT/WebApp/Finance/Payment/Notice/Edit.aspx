<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.Payment.Notice.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var Notice = eval('(<%=this.Model.Notice%>)');
        var PaymentType = eval('(<%=this.Model.PaymentType%>)');
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var FinanceAccountData = eval('(<%=this.Model.FinanceAccountData%>)');
        var RealExchangeRate = eval('(<%=this.Model.RealExchangeRate%>)');

        $(function () {
            if (Notice.BankAutoDisplay.length > 0) {
                $("#BankAutoDisplay").html(Notice.BankAutoDisplay);
                $("#BankAutoDisplay").show();
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

                    var formData = new FormData($('#form2')[0]);
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

            $('#PayType').combobox({
                data: PaymentType,
            });


            //绑定日志信息

            //先显示 CostApplyLogs
            ShowCostApplyLogs();

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
            InitPage();


            //显示文件选择 Begin

            //if (Notice.FeeTypeInt < 10000) {
            //    //显示付汇委托书
            //    $("#FuHuiWeiTuoShuFileArea").show();
            //} else {
            //    $("#CostApplyFilesArea").show();
            //}

            //显示文件逻辑修改，有costapplyID显示COSTAPPLY
            if (Notice.CostApplyID != "" && Notice.CostApplyID != null) {
                $("#CostApplyFilesArea").show();
            } else if (Notice.RefundApplyID != "" && Notice.RefundApplyID != null) {

            } else {
                //显示付汇委托书
                $("#FuHuiWeiTuoShuFileArea").show();
            }

            //显示文件选择 End

            ShowCostApplyFiles();

            //费用申请-申请列表初始化
            $('#ApproverList').myDatagrid({
                actionName: 'Feedata',
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        function Init() {
            if (Notice != null && Notice != "") {
                //基本信息
                $('#ApplyDate').text(Notice.ApplyDate);
                $('#ApplyPayType').text(Notice.ApplyPayType);
                $('#PayDate').text(Notice.PayDate);
                $('#Amount').text(Notice.Amount + "(" + Notice.Currency + ")");
                $('#PayFeeType').text(Notice.PayFeeTypeName);
                //付款人
                $('#PayeeName').text(Notice.PayeeName.replace("&#39", "'"));
                $('#BankName').text(Notice.BankName.replace("&#39", "'"));
                $('#BankAccount').text(Notice.BankAccount);
                $('#SwiftCode').text(Notice.SwiftCode);

                //$('#Poundage').text('setValue',Notice.Poundage);
                //$('#SeqNoPoundage').text('setValue',Notice.SeqNoPoundage);

                if (Notice.BankAddress != null) {
                    $('#BankAddress').text(Notice.BankAddress.replace("&#39", "'"));
                }
                if (Notice.OtherInfo != null) {
                    $('#OtherInfo').text(Notice.OtherInfo);
                }
                if (Notice.Summary != null) {
                    $('#Summary').text(Notice.Summary);
                }

                if (Notice.FeeTypeInt < 10000 && Notice.FeeTypeInt == "<%=Needs.Ccs.Services.Enums.FinanceFeeType.Product.GetHashCode()%>") {
                    if (Notice.PayExchangeApplyFile != null) {
                        $("#proxyFile").text(Notice.PayExchangeApplyFile.FileName);
                        $('#proxyFile').attr('href', Notice.PayExchangeApplyFile.Url);
                    }
                }

                $('#Payer').textbox('setValue', Notice.Payer.RealName);
                $('#PayType').combobox('setValue', Notice.PayType);
                if (Notice.ExchangeRate == null || Notice.ExchangeRate == "") {
                    $('#ExchangeRate').numberbox('setValue', RealExchangeRate.Rate);
                }
                else {
                    $('#ExchangeRate').numberbox('setValue', Notice.ExchangeRate);
                }

                if (Notice.PayNoticeFile.FileName != null && Notice.PayNoticeFile.FileName != "") {
                    $("#NoticeFile").text(Notice.PayNoticeFile.FileName);
                    $('#NoticeFile').attr('href', Notice.PayNoticeFile.Url);
                    $("#uploadedProxyFile").show();
                    $("#unUploadProxyFile").hide();
                }
                else {
                    $("#uploadedProxyFile").hide();
                    $("#unUploadProxyFile").show();
                }

                if (Notice.IsCash) {
                    $('#PayType').combobox('setValue', '<%=Needs.Ccs.Services.Enums.PaymentType.Cash.GetHashCode()%>');
                    $('#PayType').combobox('disable');
                    $('#SeqNo').combobox('disable');
                    $('#SeqNo').combobox('textbox').validatebox('options').required = false;
                    $("#CashTypeAreaaDisplay").show();
                }
            }
        }

        function InitPage() {
            if (Notice.Status == "<%=Needs.Ccs.Services.Enums.PaymentNoticeStatus.Paid.GetHashCode()%>") {
                //付款信息
                $('#FinanceVault').combobox({
                    data: FinanceVaultData,
                });
                $.post('?action=getAccounts', { VaultID: Notice.FinanceVault.ID, Currency: Notice.Currency }, function (data) {
                    var accounts = JSON.parse(data);
                    $('#FinanceAccount').combobox({
                        data: accounts,
                    });
                    $('#FinanceAccount').combobox('setValue', Notice.FinanceAccount.ID);
                });
                $('#SeqNo').textbox("setValue", Notice.SeqNo);
                $('#FinanceVault').combobox('setValue', Notice.FinanceVault.ID);
                $('#upload').css("display", "none");
                $('#btnSave').css("display", "none");
                $('#divActualPay').css("display", "none");

                $("#NoticeFile").text(Notice.PayNoticeFile.FileName);
                $('#NoticeFile').attr('href', Notice.PayNoticeFile.Url);
                $('#unUploadNoitceFile').css("display", "none");
                $('#uploadedNoitceFile').css("display", "block");

                $('#Poundage').textbox("setValue", Notice.Poundage);
                $('#SeqNoPoundage').textbox("setValue", Notice.SeqNoPoundage);
                $('#USDAmount').textbox("setValue", Notice.USDAmount);
            } else {
                $('#btnSavePoundage').css("display", "none");

                $('#FinanceVault').combobox({
                    data: FinanceVaultData,
                    onLoadSuccess: function () {
                        var opts = $(this).combobox('getData');
                        if (opts.length > 0) {
                            $(this).combobox('select', opts[0].Value); 
                        }
                    },
                    onSelect: function (record) {
                        $.post('?action=getAccounts', { VaultID: record.Value, Currency: Notice.Currency }, function (data) {
                            var accounts = JSON.parse(data);
                            $('#FinanceAccount').combobox({
                                data: accounts,
                            });
                        });
                    }
                });
                $('#FinanceAccount').combobox({
                    data: FinanceAccount,
                })


            }
        }

        //确认付款
        function Save() {
            if (!Valid("form1")) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            var FileName = $('#NoticeFile').text();
            var FileID = $('#fileID').text();
            var FileFormat = $('#fileFormat').text();
            var FileUrl = $('#fileUrl').text();
            var PayType = $("#PayType").combobox("getValue");
            var Poundage = $('#Poundage').textbox("getValue");
            var SeqNoPoundage = $('#SeqNoPoundage').textbox("getValue");
            var USDAmount = $('#USDAmount').textbox("getValue");
            var ActualPayDate = $('#ActualPayDate').datebox("getValue");

            data.append("ID", Notice.ID);
            data.append("FileID", FileID);
            data.append("FileName", FileName);
            data.append("FileFormat", FileFormat);
            data.append("FileUrl", FileUrl);
            data.append("FeeTypeInt", Notice.FeeTypeInt);
            data.append("CostApplyID", Notice.CostApplyID);
            data.append("Poundage", Poundage);
            data.append("SeqNoPoundage", SeqNoPoundage);
            data.append("USDAmount", USDAmount);
            data.append("RefundApplyID", Notice.RefundApplyID);
            data.append("ActualPayDate", ActualPayDate);

            if (Notice.IsCash) {
                data.append("PayType", PayType);
            }

            data.append("IsCash", Notice.IsCash);
            MaskUtil.mask();
            $.ajax({
                url: '?action=SavePaymentNotice',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $.messager.alert('消息', res.message, '', function () {
                            Back();
                        });
                    }
                    else {
                        $.messager.alert('错误', res.message);
                    }
                }
            }).done(function () {
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
        //复制
        function Copy(IdName) {
            var obj = document.getElementById(IdName);
            $("#CopyContext").text(obj.innerText);
            var newobj = document.getElementById("CopyContext")
            newobj.select(); // 选择对象
            document.execCommand("Copy"); // 执行浏览器复制命令
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

        function Back() {
            var url = '';
            if (Notice.Status == "<%=Needs.Ccs.Services.Enums.PaymentNoticeStatus.Paid.GetHashCode()%>") {
                url = location.pathname.replace(/Edit.aspx/ig, 'PaidList.aspx');
            }
            else {
                url = location.pathname.replace(/Edit.aspx/ig, 'UnPaidList.aspx');
            }
            window.location = url;
        }

        //CostApplyFiles Begin

        function ShowCostApplyFiles() {
            $('#datagrid_file').myDatagrid({
                actionName: 'CostApplyFiles',
                queryParams: { CostApplyID: Notice.CostApplyID },
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
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainerFile");
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

                    var heightValue = $("#datagrid_file").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                    $("#datagrid_file").prev().find(".datagrid-body").height(heightValue);
                    $("#datagrid_file").prev().height(heightValue);
                    $("#datagrid_file").prev().parent().height(heightValue);
                    $("#datagrid_file").prev().parent().parent().height(heightValue);

                    $("#datagrid_file").prev().parent().parent().height(heightValue + 35);
                }
            });
            //$('#datagrid_file').datagrid({
            //    queryParams: { action: 'CostApplyFiles', CostApplyID: Notice.CostApplyID, },
            //    border: false,
            //    showHeader: false,
            //    pagination: false,
            //    rownumbers: false,
            //    fitcolumns: true,
            //    rowStyler: function (index, row) {
            //        return 'background-color:white;';
            //    },
            //    loadFilter: function (data) {
            //        //$('#fileContainer').panel('setTitle', '合同发票(INVOICE LIST)(' + data.total + ')');
            //        if (data.total == 0) {
            //            $('#unUpload').css('display', 'block');
            //        } else {
            //            $('#unUpload').css('display', 'none');
            //        }
            //        return data;
            //    },
            //    onLoadSuccess: function (data) {
            //        var panel = $(this).datagrid('getPanel');
            //        var header = panel.find('div.datagrid-header');
            //        header.css({
            //            'visibility': 'hidden'
            //        });
            //        var tr = panel.find('div.datagrid-body tr');
            //        tr.each(function () {
            //            var td = $(this).children('td');
            //            td.css({
            //                'border-width': '0'
            //            });
            //        });
            //    }
            //});
        }

        function FileOperation(val, row, index) {
            var buttons = row.FileName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            return buttons;
        }
        function ShowImg(val, row, index) {
            return "<img src='../../../App_Themes/xp/images/wenjian.png' />";
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
        }

        //CostApplyFiles End

        function ShowCostApplyLogs() {
            if (Notice.CostApplyID != "") {
                $.post(location.pathname + '?action=CostApplyLogs', {
                    CostApplyID: Notice.CostApplyID,
                }, function (res) {
                    var result = JSON.parse(res);

                    $.each(result.rows, function (index, row) {
                        var str = '';
                        if (row.Summary != null) {
                            str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                            $("#LogContent").append(str);
                        }
                    });
                });
            }
            if (Notice.RefundApplyID != "") {
                $.post(location.pathname + '?action=RefundApplyLogs', {
                    RefundApplyID: Notice.RefundApplyID,
                }, function (res) {
                    var result = JSON.parse(res);

                    $.each(result.rows, function (index, row) {
                        var str = '';
                        if (row.Summary != null) {
                            str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                            $("#LogContent").append(str);
                        }
                    });
                });
            }

        }

        function SavePoundage() {
            var ID = Notice.ID;
            var Poundage = $('#Poundage').textbox("getValue");
            var SeqNoPoundage = $('#SeqNoPoundage').textbox("getValue");

            if (Poundage == '' || SeqNoPoundage == '') {
                $.messager.alert('错误', '请填写手续费和手续费流水号');
                return
            }
            MaskUtil.mask();
            $.post("?action=SavePoundage", { ID: ID, Poundage: Poundage, SeqNoPoundage: SeqNoPoundage }, function (res) {
                var result = JSON.parse(res);
                MaskUtil.unmask();
                if (result.success) {
                    $.messager.alert('成功', '保存成功');
                }
                else {
                    $.messager.alert('错误', result.message);
                }
            });
        }


    </script>
    <style type="text/css">
        .span {
            color: cornflowerblue;
        }

        .lab {
            word-break: break-all;
        }

        #CashTypeAreaaDisplay .label {
            background-color: #e00e0e;
            display: inline;
            padding: .2em .6em .3em;
            font-size: 75%;
            font-weight: 700;
            line-height: 1;
            color: #fff;
            text-align: center;
            white-space: nowrap;
            vertical-align: baseline;
            border-radius: .25em;
        }
    </style>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="付款通知" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 40px;">
                    <div id="divActualPay">
                        <span>实际付款日期：</span>
                        <input class="easyui-datebox" id="ActualPayDate" style="height: 26px; width: 200px" />
                    </div>
                    <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">确认付款</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                    <a id="btnSavePoundage" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="SavePoundage()">保存手续费</a>
                    <span id="BankAutoDisplay" style="font-size: 30px; margin-left: 240px; display: none; color: red;"></span>
                </div>
            </div>
            <div data-options="region:'west',border: false," style="width: 30%; float: left;">
                <div class="sec-container">
                    <div style="padding-top: 2px">
                        <div id="panel1" class="easyui-panel" title="基本信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">申请日期：</td>
                                        <td>
                                            <label class="lab" id="ApplyDate"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款方式：</td>
                                        <td>
                                            <label class="lab" id="ApplyPayType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款日期：</td>
                                        <td>
                                            <label class="lab" id="PayDate"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款金额：</td>
                                        <td>
                                            <label class="lab" id="Amount"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">费用类型：</td>
                                        <td>
                                            <label class="lab" id="PayFeeType"></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div id="panel2" class="easyui-panel" title="收款人">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="lbl">收款人：</td>
                                        <td>
                                            <label class="lab" id="PayeeName"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('PayeeName')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行名称：</td>
                                        <td>
                                            <label class="lab" id="BankName"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('BankName')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行地址：</td>
                                        <td>
                                            <label class="lab" id="BankAddress"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('BankAddress')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行账号：</td>
                                        <td>
                                            <label class="lab" id="BankAccount"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('BankAccount')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行代码：</td>
                                        <td>
                                            <label class="lab" id="SwiftCode"></label>
                                        </td>
                                        <td>
                                            <a href="#" style="float: right; min-width: 25px"><span class="span" onclick="Copy('SwiftCode')">复制</span></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">其它相关资料：</td>
                                        <td>
                                            <label class="lab" id="OtherInfo"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">备注：</td>
                                        <td>
                                            <label class="lab" id="Summary"></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div id="panel3" class="easyui-panel" title="附件">
                            <div class="sub-container" style="height: 150px; overflow: auto;">
                                <div id="FuHuiWeiTuoShuFileArea" style="display: none;">
                                    <table class="file-info">
                                        <tbody>
                                            <tr>
                                                <td colspan="2">付汇委托书</td>
                                            </tr>
                                            <tr>
                                                <td rowspan="2">
                                                    <img src="../../../App_Themes/xp/images/wenjian.png" /></td>
                                                <td id="proxyFile">123456789</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <a href="#" onclick="Look();return false"><span>预览</span></a>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div id="CostApplyFilesArea" style="display: none;">
                                    <div class="sub-container">
                                        <div id="unUpload" style="margin-left: 5px">
                                            <p>未上传</p>
                                        </div>
                                        <div id="fileContainerFile">
                                            <table id="datagrid_file" data-options=" ">
                                                <%--data-options="nowrap:false,"--%>
                                                <thead>
                                                    <tr>
                                                        <th data-options="field:'img',formatter:ShowImg">图片</th>
                                                        <th style="width: auto" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                        <div class="text-container" style="margin-top: 10px;">
                                            <p>仅限图片、pdf格式的文件，且pdf文件不超过3M。</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 69%; float: left;">
                <div class="sec-container">
                    <div style="width: 100%;">
                        <form id="form1">
                            <table id="table1" style="width: 100%; padding-right: 0;">
                                <tr>
                                    <td style="vertical-align: top; width: 50%">
                                        <!-- 费用信息 -->
                                        <div title="费用信息" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'270px',">
                                            <div style="height: 237px">
                                                <table id="ApproverList">
                                                    <thead>
                                                        <tr>
                                                            <th data-options="field:'FeeName',align:'left'" style="width: 50%;">费用类型</th>
                                                            <th data-options="field:'Price',align:'left'" style="width: 15%;">金额</th>
                                                            <th data-options="field:'FeeDesc',align:'left'" style="width: 35%;">描述</th>
                                                        </tr>
                                                    </thead>
                                                </table>
                                            </div>
                                        </div>
                                    </td>
                                    <td style="padding-left: 5px; vertical-align: top">
                                        <div id="fileContainer" title="付款" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'270px',">
                                            <div class="sub-container">
                                                <table class="row-info" style="width: 100%; height: 300px" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td class="lbl" style="width: 150px">付款流水号：</td>
                                                        <td>
                                                            <input class="easyui-textbox" id="SeqNo" name="SeqNo" data-options="required:true,validType:'length[1,50]'," style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">付款汇率：</td>
                                                        <td>
                                                            <input class="easyui-numberbox" id="ExchangeRate" name="ExchangeRate" style="width: 250px"
                                                                data-options="min:0,precision:4,required:true,validType:'length[1,18]',tipPosition:'bottom',missingMessage:'请输入汇率'" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">付款方式：</td>
                                                        <td>
                                                            <input class="easyui-combobox" id="PayType" name="PayType"
                                                                data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px" />

                                                            <span id="CashTypeAreaaDisplay" style="margin-left: 10px; display: none;">
                                                                <span class="label">现金</span>
                                                            </span>

                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">付款金库：</td>
                                                        <td>
                                                            <input class="easyui-combobox" id="FinanceVault" name="FinanceVault"
                                                                data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">付款账户：</td>
                                                        <td>
                                                            <input class="easyui-combobox" id="FinanceAccount" name="FinanceAccount"
                                                                data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">付款人：</td>
                                                        <td>
                                                            <input class="easyui-textbox" id="Payer" name="Payer" data-options="disabled:true" style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">手续费：</td>
                                                        <td>
                                                            <input class="easyui-textbox" id="Poundage" data-options="validType:'length[1,50]'" style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">手续费流水号：</td>
                                                        <td>
                                                            <input class="easyui-textbox" id="SeqNoPoundage" data-options="validType:'length[1,50]'" style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="lbl">美金金额：</td>
                                                        <td>
                                                            <input class="easyui-textbox" id="USDAmount" data-options="validType:'length[1,50]'" style="width: 250px" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </td>

                                </tr>
                            </table>
                        </form>
                    </div>
                    <div style="margin-top: 5px; margin-left: 2px;">
                        <form id="form2">
                            <table id="table2" style="width: 100%; padding-right: 0;">
                                <tr>
                                    <td style="vertical-align: top; width: 30%">
                                        <div id="para-panel-2" class="easyui-panel" title="支付凭证" data-options="iconCls:'icon-blue-fujian', height:'315px',">
                                            <div class="sub-container">
                                                <div>
                                                    <table class="file-info" id="uploadedProxyFile">
                                                        <tbody>
                                                            <tr>
                                                                <td rowspan="2">
                                                                    <img src="../../../App_Themes/xp/images/wenjian.png" /></td>
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
                                            </div>
                                        </div>
                                    </td>
                                    <td style="padding-left: 5px; vertical-align: top">
                                        <div class="easyui-panel" title="日志记录" style="width: 100%; min-height: 315px">
                                            <div class="sub-container">
                                                <div class="text-container" id="LogContent">
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 600px;">
        <img id="viewfileImg" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
    <div style="border: none; width: 1px; height: 1px; outline: none; opacity: 0;">
        <textarea id="CopyContext"></textarea>
    </div>

</body>
</html>
