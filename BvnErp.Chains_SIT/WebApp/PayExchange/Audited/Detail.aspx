<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.PayExchange.Audited.Detail" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>审核付汇申请</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var PayExchangeApplyData = eval('(<%=this.Model.PayExchangeApplyData%>)');
                                var ProxyFileData = eval('(<%=this.Model.ProxyFileData%>) ');
    var ProductFeeLimitData = eval('(<%=this.Model.ProductFeeLimitData%>)');

    //页面加载时
    $(function () {
        $('#datagrid').myDatagrid({
            fitColumns: true,
            fit: false,
            pagination: false,
            scrollbarSize: 0,
            //onLoadSuccess: function (data) {
            //    var heightValue = $("#datagrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 80;
            //    $("#datagrid").prev().find(".datagrid-body").height(heightValue);
            //    $("#datagrid").prev().height(heightValue);
            //    $("#datagrid").prev().parent().height(heightValue);
            //    $("#datagrid").prev().parent().parent().height(heightValue);
            //},
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

                var unUploadHeight = data.total * 36 + 100;//ryan 根据附件个数 动态计算高度

                $("#unUpload").next().find(".datagrid-wrap").height(unUploadHeight);
                $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(unUploadHeight);
                $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(unUploadHeight);

            }
        });
        $("#file").filebox({
            onChange: function (e) {
                var formData = new FormData($('#form1')[0]);
                $.ajax({
                    url: '?action=UploadFiles',
                    type: 'POST',
                    data: formData,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) { }
                }).done(function (result) {
                    var row = $('#datagrid_file').datagrid('getRows');
                    if (result.success) {
                        var data = result.data;
                        for (var i = 0; i < data.length; i++) {
                            $('#datagrid_file').datagrid('insertRow', {
                                index: row.length + i,   // 索引从0开始
                                row: {
                                    FileName: data[i].FileName,
                                    FileFormat: data[i].FileFormat,
                                    WebUrl: data[i].WebUrl,
                                    Url: data[i].Url,
                                }
                            });
                            $("a[name=btnView]").on('click', function () {
                                var $this = $(this);
                                var fileUrl = $this.data("fileurl");
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
                            });
                        }
                    }
                });
            }
        })
        $("#file1").filebox({
            onChange: function (e) {
                var formData = new FormData($('#form2')[0]);
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
                        var data = result.data;
                        $("#proxyFile").text(data.FileName);
                        $('#proxyFile').attr('href', data.WebUrl);
                        $("#fileFormat").text(data.FileFormat);
                        $("#fileUrl").text(data.Url);
                    }
                });
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

        $.post('?action=isCanDelivery', { ID: getQueryString("ID") }, function (data) {
            var Result = JSON.parse(data);
            if (Result) {
                debugger;
                var htmlstr = '<span  style="color: red">' + Result.GetOrderId + '</span>';
                $("#OverDuePayment").html(Result.OverDuePayment ? "超期" + "(" + htmlstr + ")" : "未超期");
            }

        });

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
    });
    //初始化供应商信息
    function Init() {
        if (PayExchangeApplyData != null) {
            $('#SupplierName').text(PayExchangeApplyData.SupplierName);
            if (PayExchangeApplyData.SupplierAddress != null) {
                $('#SupplierAddress').text(PayExchangeApplyData.SupplierAddress);
            }
            // $('#SupplierEnglishName').text(PayExchangeApplyData.SupplierEnglishName);
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
            //$('#Currency').text(PayExchangeApplyData.Currency);
            $('#ExchangeRateType').text(PayExchangeApplyData.ExchangeRateType);
            $('#ExchangeRate').text(PayExchangeApplyData.ExchangeRate);
            $('#Price').text(PayExchangeApplyData.Price + "(" + PayExchangeApplyData.Currency + ")");
            $('#RmbPrice').text(PayExchangeApplyData.RmbPrice + "(RMB)");
            $('#Merchandiser').text(PayExchangeApplyData.Merchandiser);

            // AdvanceMoney=0 存在垫资申请，选择垫款， AdvanceMoney=1不存在 则选择不垫款 by 2020-12-23 yess
            if (PayExchangeApplyData.AdvanceMoney == 0) {
                document.getElementById('AdvanceMoney').checked = true;
            }
            else {
                document.getElementById('IsAdvanceMoney').checked = true;
            }
        }
        if (ProxyFileData != null) {
            $("#proxyFile").text(ProxyFileData.FileName);
            $('#proxyFile').attr('href', ProxyFileData.WebUrl);
            $("#fileFormat").text(ProxyFileData.FileFormat);
            $("#fileUrl").text(ProxyFileData.Url);
            $("#fileID").text(ProxyFileData.ID);
        }
        else {
            $("#fileID").text(ProxyFileData.ID);
        }
        if (ProductFeeLimitData != null) {
            //$("#PeriodType").text(ProductFeeLimitData.PeriodType);
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
        var url = location.pathname.replace(/Detail.aspx/ig, 'List.aspx');
        window.location = url;
    }
    //删除PI文件
    function DeleteFile(Index) {
        $("#datagrid_file").datagrid('deleteRow', Index);
        //解决删除行后，行号错误问题
        var rows = $("#datagrid_file").datagrid('getRows');
        $("#datagrid_file").datagrid('loadData', rows);
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
        return "<img src='../../App_Themes/xp/images/wenjian.png' />";
    }
    </script>
    <style>
        html {
            height: 100%;
        }

        body {
            min-height: 100%;
            height: auto;
        }

        .lab {
            word-break: break-all;
        }
    </style>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;" data-options="border: false,">
        <div title="付汇审核" style="display: none; padding: 5px;">
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
                                    <tr>
                                        <td class="lbl">是否垫款：</td>
                                        <td>
                                            <input id="AdvanceMoney" type="radio" name="IsAdvanceMoney"
                                                class="easyui-validatebox" value="0"><label>垫款</label></input>
                                            <input id="IsAdvanceMoney" type="radio" name="IsAdvanceMoney"
                                                class="easyui-validatebox" value="1"><label>不垫款</label></input>
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
                                        <td class="lbl">期望付汇日期：</td>
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
                                        <td class="lbl">汇率：</td>
                                        <td>
                                            <label class="lbl" id="ExchangeRate"></label>
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
                                    <th data-options="field:'DeclarePrice',width: 50,align:'center'">报关总价</th>
                                    <th data-options="field:'PaidPrice',width: 50,align:'center'">已付汇金额</th>
                                    <th data-options="field:'Amount',width: 50,align:'center'">本次申请金额</th>
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
                                <td style="padding-left: 5px; vertical-align: top">
                                    <div id="para-panel-2" class="easyui-panel" title="付汇委托书" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                                        <div class="sub-container">
                                            <form id="form2">
                                                <div>
                                                    <table class="file-info">
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
