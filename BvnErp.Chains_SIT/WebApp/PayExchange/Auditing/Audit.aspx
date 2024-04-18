<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Audit.aspx.cs" Inherits="WebApp.PayExchange.Auditing.Audit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>审核付汇申请</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var PayExchangeApplyData = eval('(<%=this.Model.PayExchangeApplyData%>)');
        var ProxyFileData = eval('(<%=this.Model.ProxyFileData%>)');
        var ProductFeeLimitData = eval('(<%=this.Model.ProductFeeLimitData%>)');
        var ID = '<%=this.Model.ID%>';
        var IsadvanceMoney = '<%=this.Model.IsadvanceMoney%>';
        var IsTen = '<%=this.Model.IsTen%>';

        var AvailableProductFee ='<%=this.Model.AvailableProductFee%>';

        //页面加载时
        $(function () {
            if (PayExchangeApplyData.FatherID != null) {
                $('#Split').css('display', 'none');
                $('#Cancel').css('display', 'none');
                if (PayExchangeApplyData.IsadvanceMoney == "0") {
                    document.getElementById('AdvanceMoney').checked = true;
                } else {
                    document.getElementById('IsAdvanceMoney').checked = true;
                }
                document.getElementById('AdvanceMoney').disabled = true;
                document.getElementById('IsAdvanceMoney').disabled = true;
            } else {
                if (IsadvanceMoney == "0") {
                    document.getElementById('AdvanceMoney').checked = true;
                }
                else {
                    document.getElementById('IsAdvanceMoney').checked = true;
                    //document.getElementById('AdvanceMoney').disabled = true;
                }
            }
            document.getElementById('AdvanceMoneyProduct').innerHTML = AvailableProductFee;

            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                scrollbarSize: 0
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


                    //var unUploadHeight = 900;
                    //if (data.total > 100) {
                    //    unUploadHeight = 7000;
                    //}

                    var unUploadHeight = data.total * 36 + 100;//ryan 根据附件个数 动态计算高度

                    $("#unUpload").next().find(".datagrid-wrap").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(unUploadHeight);
                }
            });

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
                        url: '?action=UploadFiles',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            if (res.success) {
                                var data = res.data;
                                for (var i = 0; i < data.length; i++) {
                                    $('#datagrid_file').datagrid('appendRow', {
                                        FileName: data[i].FileName,
                                        FileFormat: data[i].FileFormat,
                                        WebUrl: data[i].WebUrl,
                                        Url: data[i].Url
                                    });
                                }
                                var data = $('#datagrid_file').datagrid('getData');
                                $('#datagrid_file').datagrid('loadData', data);
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            });
            //注册文件上传的onChange事件
            $('#uploadProxyFile').filebox({
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
                    //if ($this.next().attr("class").indexOf("textbox-invalid") > 0) {
                    //    $.messager.alert('提示', '文件大小不能超过500kb！');
                    //    return;
                    //}
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
                    var ID = getQueryString('ID');
                    formData.append("ID", ID);
                    $.ajax({
                        url: '?action=UploadProxyFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) { }
                    }).done(function (result) {
                        if (result.success) {
                            debugger;
                            var data = result.data;
                            $("#proxyFile").text(data.FileName);
                            $('#proxyFile').attr('href', data.WebUrl);
                            $("#fileFormat").text(data.FileFormat);
                            $("#fileUrl").text(data.Url);
                            $("#fileID").text("");  //使得可以向中心传新付汇委托书

                            $("#uploadedProxyFile").show();
                            $("#unUploadProxyFile").hide();
                        }
                        else {
                            $.messager.alert('提示', result.data);
                        }
                    });
                }
            });
            $('#ExchangeRate').numberbox({
                onChange: function () {
                    var rate = $('#ExchangeRate').numberbox('getValue');
                    var rmbPrice = Number(PayExchangeApplyData.Price) * Number(rate);
                    $('#RmbPrice').text(rmbPrice + "(RMB)");
                }
            })
            //绑定日志信息
            var ApplyID = getQueryString("ID");
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

            $('input[type=radio][name=AdvanceMoney]').change(function () {
                var rate = $('#ExchangeRate').numberbox('getValue');
                var rmbPrice = Number(PayExchangeApplyData.Price) * Number(rate);
                if ($('input:radio[name="AdvanceMoney"]:checked').val() == "0") {
                    //if (rmbPrice > Number(AvailableProductFee)) {
                    //    //垫款
                    //    document.getElementById('IsAdvanceMoney').checked = true;
                    //    $.messager.alert('提示', '垫款额度不够，只能选择不垫款！');
                    //    return;
                    //}
                    //有些客户偶尔或超过额度。通过邮件审批的，所以系统允许超过额度选择垫款 20230628 ryan
                    //if (Number(AvailableProductFee) <= 0) {
                    //    document.getElementById('IsAdvanceMoney').checked = true;
                    //    $.messager.alert('提示', '无垫款额度，只能选择不垫款！');
                    //    return;
                    //}
                }
            });



            $.post('?action=isCanDelivery', { ID: getQueryString("ID") }, function (data) {
                var Result = JSON.parse(data);
                if (Result) {
                    var htmlstr = '<span  style="color: red">' + Result.GetOrderId + '</span>';
                    $("#OverDuePayment").html(Result.OverDuePayment ? "超期" + "(" + htmlstr + ")" : "未超期");
                }

            });
        });
        //初始化供应商信息
        function Init() {
            if (PayExchangeApplyData != null && PayExchangeApplyData != "") {
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

                $('#ClientName').text(PayExchangeApplyData.ClientName);
                $('#ClientCode').text(PayExchangeApplyData.ClientCode);
                $('#ApplyDate').text(PayExchangeApplyData.CreateDate);
                $('#PayMode').text(PayExchangeApplyData.PaymentType);
                $('#PayDate').text(PayExchangeApplyData.SettlemenDate);
                $('#ExchangeRateType').text(PayExchangeApplyData.ExchangeRateType);
                $('#ExchangeRate').numberbox('setValue', PayExchangeApplyData.ExchangeRate);
                $('#Price').text(PayExchangeApplyData.Price + "(" + PayExchangeApplyData.Currency + ")");
                $('#RmbPrice').text(PayExchangeApplyData.RmbPrice + "(RMB)");
                $('#Merchandiser').text(PayExchangeApplyData.Merchandiser);


                //20230621 ryan 付汇审核显示手续费情况
                if (PayExchangeApplyData.HandlingFeePayerType != '' && PayExchangeApplyData.HandlingFeePayerType != null) {
                    var ss = "";
                    if (PayExchangeApplyData.HandlingFeePayerType == 1) {
                        ss = "收款方";
                    }
                    if (PayExchangeApplyData.HandlingFeePayerType == 2) {
                        ss = "付款方";
                    }
                    if (PayExchangeApplyData.HandlingFeePayerType == 3) {
                        ss = "双方承担";
                    }
                    $('#HandleFeeMsg').text("手续费承担方：" + ss + "/金额：" + PayExchangeApplyData.HandlingFee + "/汇率：" + PayExchangeApplyData.USDRate);
                }
                

                //// AdvanceMoney=0 存在垫资申请，选择垫款， AdvanceMoney=1不存在 则选择不垫款 by 2020-12-23 yess
                //if (PayExchangeApplyData.AdvanceMoney == 0) {
                //    document.getElementById('AdvanceMoney').checked = true;
                //}
                //else {
                //    document.getElementById('IsAdvanceMoney').checked = true;
                //}

                if (PayExchangeApplyData.BankIsSensitive) {
                    if (PayExchangeApplyData.BankSensitiveType == '<%=Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Forbid.GetHashCode()%>') {
                        $("#SensitiveBankTip-Forbid").show();
                        $("#SensitiveBankTip-Sensitive").hide();
                    } else if (PayExchangeApplyData.BankSensitiveType == '<%=Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Sensitive.GetHashCode()%>') {
                        $("#SensitiveBankTip-Forbid-Forbid").hide();
                        $("#SensitiveBankTip-Sensitive").show();
                    } else {
                        $("#SensitiveBankTip-Forbid").hide();
                        $("#SensitiveBankTip-Sensitive").hide();
                    }
                } else {
                    $("#SensitiveBankTip-Forbid").hide();
                    $("#SensitiveBankTip-Sensitive").hide();
                }

                //使用十点汇率提示语
                if (IsTen == "True") {
                    $("#RateType").html("中国银行10:00");
                }
                else {
                    $("#RateType").html("中国银行09:30");
                }

            }
            if (ProxyFileData != null && ProxyFileData != "") {
                $("#uploadedProxyFile").show();
                $("#unUploadProxyFile").hide();
                $("#proxyFile").text(ProxyFileData.FileName);
                $('#proxyFile').attr('href', ProxyFileData.WebUrl);
                $("#fileFormat").text(ProxyFileData.FileFormat);
                $("#fileUrl").text(ProxyFileData.Url);
                $("#fileID").text(ProxyFileData.ID);
            }
            else {
                $("#unUploadProxyFile").show();
                $("#uploadedProxyFile").hide();
            }

            if (ProductFeeLimitData != null && ProductFeeLimitData != "") {
                //$("#PeriodType").text(ProductFeeLimitData.PeriodType);
            }
        }
        //审核通过
        function Submit() {
            var isValid = $("#form2").form("enableValidation").form("validate");
            var FileData = $("#datagrid_file").datagrid("getRows");
            var FileID = $('#fileID').text();
            var FileName = $('#proxyFile').text();
            var FileFormat = $('#fileFormat').text();
            var FileUrl = $('#fileUrl').text();
            var ExchangeRate = $('#ExchangeRate').numberbox('getValue');
            var ID = getQueryString('ID');
            var AdvanceMoney = $('input:radio[name="AdvanceMoney"]:checked').val();

            if (!isValid) {
                return;
            }
            else if (FileData.length == 0) {
                $.messager.alert('提示', '请上传PI文件！');
                return;
            }
            else if (FileUrl == "") {
                $.messager.alert('提示', '请上传付汇委托书！');
                return;
            }
            else if (ExchangeRate == "" || Number(ExchangeRate) == 0) {
                $.messager.alert('提示', '请输入今日汇率！');
                return;
            }
            else {
                MaskUtil.mask();//遮挡层
                //验证成功
                $.post('?action=Submit', {
                    ID: ID,
                    FileID: FileID,
                    FileName: FileName,
                    FileFormat: FileFormat,
                    FileUrl: FileUrl,
                    ExchangeRate: ExchangeRate,
                    FileData: JSON.stringify(FileData),
                    AdvanceMoney: AdvanceMoney
                }, function (result) {
                    MaskUtil.unmask();//关闭遮挡层
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            //返回列表页
                            Back();
                        }
                    });
                })
            }
        }
        //取消审核
        function Cancel() {
            var ID = getQueryString('ID');
            $.messager.confirm('确认', '是否取消审核该申请！', function (success) {
                if (success) {
                    $.post('?action=Cancel', {
                        ID: ID,
                    }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                //返回列表页
                                Back();
                            }
                        });
                    })
                }
            });
        }
        function Split() {
            var FileUrl = $('#fileUrl').text();
            if (FileUrl == "") {
                $.messager.alert('提示', '请上传付汇委托书！');
                return;
            }
            var AdvanceMoney = $('input:radio[name="AdvanceMoney"]:checked').val();
            var url = location.pathname.replace(/Audit.aspx/ig, 'Split.aspx') + '?ID=' + ID + '&FileUrl=' + FileUrl + '&AdvanceMoney=' + AdvanceMoney;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '拆分信息',
                width: '980px',
                height: '600px',
                onClose: function () {
                    //   if (true) {
                    // $('#datagrid').myDatagrid('reload');
                    // }
                    // else {
                    Back();
                    // }
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

            $('.window-mask').css('display', 'none');
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

            $('.window-mask').css('display', 'none');
        }
        //打印付汇委托书
        function Print() {
            var href = $('#proxyFile').attr("href");
            if (href.toLowerCase().indexOf('pdf') > 0) {
                $('#pdfPrint').attr("src", href);
                setTimeout(function () {
                    $("#print_button").click();
                }, 500);
            }
            else {
                $('#imgPrint').attr("src", href);
                setTimeout(function () {
                    $('#image').jqprint();
                }, 500);
            }
        }
        //导出付汇委托书
        function Export() {
            var ID = getQueryString('ID');
            MaskUtil.mask();
            $.post('?action=ExportProxyFile', {
                ID: ID,
            }, function (result) {
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
        //返回
        function Back() {
            var url = location.pathname.replace(/Audit.aspx/ig, 'List.aspx');
            window.location = url;
        }
        //删除PI文件
        function DeleteFile(Index) {
            $("#datagrid_file").datagrid('deleteRow', Index);
            //解决删除行后，行号错误问题
            var data = $('#datagrid_file').datagrid('getData');
            $('#datagrid_file').datagrid('loadData', data);
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
            buttons += '<a href="#"><span style="color: cornflowerblue; margin-left: 10px;" onclick="DeleteFile(' + index + ')">删除</span></a>';
            return buttons;
        }
        function ShowImg(val, row, index) {
            return "<img src='../../App_Themes/xp/images/wenjian.png' />";
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
    <style>
        html {
            height: 100%;
        }

        body {
            min-height: 100%;
        }

        .easyui-numberbox {
            border: none;
            border-radius: 0;
            box-shadow: 0px 0px 0px 0px;
        }

        .lab {
            word-break: break-all;
        }

        input[type="radio"]:checked + label::before {
            padding: 0
        }
    </style>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;" data-options="border: false,">
        <div title="付汇审核" style="display: none; padding: 5px;">
            <div data-options="region:'north',border:false," style="height: 41px; overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()"
                        data-options="iconCls:'icon-save'">审核通过</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()"
                        data-options="iconCls:'icon-cancel'" id="Cancel">审核取消</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Split()"
                        data-options="iconCls:'icon-cut'" id="Split">拆分</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()"
                        data-options="iconCls:'icon-back'">返回</a>
                </div>
            </div>
            <div data-options="region:'north',border:false," style="height: 41px; overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <input type="radio" name="AdvanceMoney" value="0" id="AdvanceMoney" title="垫款" class="checkbox checkboxlist" /><label for="AdvanceMoney" style="margin-right: 20px">垫款</label>
                    <input type="radio" name="AdvanceMoney" value="1" id="IsAdvanceMoney" title="不垫款" class="checkbox checkboxlist" /><label for="IsAdvanceMoney">不垫款</label>
                    &nbsp; &nbsp; &nbsp; &nbsp;<span>可用垫款额度：<label id="AdvanceMoneyProduct" style="color: red;"></label></span>
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
                                            <label id="HandleFeeMsg" style="color:red"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">期望付款日期：</td>
                                        <td class="lbl" id="ExpectPayDate"></td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款金额：</td>
                                        <td>
                                            <label class="lbl" id="Price"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">汇率类型：</td>
                                        <td>
                                            <label class="lbl" id="ExchangeRateType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">今日汇率：</td>
                                        <td style="padding-left: 0px;">
                                            <input type="text" id="ExchangeRate" class="easyui-numberbox" data-options="min:0,precision:6,required:true,border:false" style="width: 50%; height: 23px;"></input>
                                            <label class="lbl" style="color:red;margin-left:10px;" id="RateType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">应收款：</td>
                                        <td>
                                            <label class="lbl" id="RmbPrice"></label>
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
                    <div style="width: 100%; padding-left: 2px;">
                        <table id="datagrid" title="付汇订单" style="width: 100%;" data-options="
                            fitColumns:true,
                            fit:false,
                            pagination:false,
                            scrollbarSize:0">
                            <thead>
                                <tr>
                                    <th data-options="field:'OrderID',width:200,align:'left'">订单编号</th>
                                    <th data-options="field:'CreateDate',width: 150,align:'center'">申请时间</th>
                                    <th data-options="field:'Currency',width: 130,align:'center'">币种</th>
                                    <th data-options="field:'DeclarePrice',width: 150,align:'center'">报关总价</th>
                                    <th data-options="field:'PaidPrice',width: 150,align:'center'">已付汇金额</th>
                                    <th data-options="field:'Amount',width: 150,align:'center'">本次申请金额</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div style="margin-top: 5px; width: 100%;">
                        <table id="table1" style="width: 100%; padding-right: 0;">
                            <tr>
                                <td style="vertical-align: top; width: 50%">
                                    <div id="fileContainer" title="合同发票(INVOICE LIST)" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'200px',">
                                        <div class="sub-container">
                                            <form id="form1">
                                                <div id="unUpload" style="margin-left: 5px">
                                                    <p>未上传</p>
                                                </div>
                                                <table id="datagrid_file" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
                                                    <thead>
                                                        <tr>
                                                            <th data-options="field:'img',formatter:ShowImg">图片</th>
                                                            <th style="width: auto;" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                                        </tr>
                                                    </thead>
                                                </table>
                                                <div style="margin-top: 5px; margin-left: 5px;">
                                                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                                                </div>
                                                <div class="text-container" style="margin-top: 10px;">
                                                    <p>仅限图片或pdf格式的文件,并且不超过500kb</p>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                </td>
                                <td style="padding-left: 5px; vertical-align: top">
                                    <div id="para-panel-2" class="easyui-panel" title="付汇委托书" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                                        <div class="sub-container">
                                            <form id="form2">
                                                <div>
                                                    <table class="file-info" id="uploadedProxyFile">
                                                        <tbody>
                                                            <tr>
                                                                <td rowspan="2">
                                                                    <img src="../../App_Themes/xp/images/wenjian.png" /></td>
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
                                                <div style="margin-top: 10px; margin-left: 5px;">
                                                    <a id="export" href="javascript:void(0);" class="easyui-linkbutton" onclick="Export()"
                                                        data-options="iconCls:'icon-yg-excelExport'">导出</a>
                                                    <input id="uploadProxyFile" name="uploadProxyFile" class="easyui-filebox" style="width: 54px; height: 26px" />
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
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
