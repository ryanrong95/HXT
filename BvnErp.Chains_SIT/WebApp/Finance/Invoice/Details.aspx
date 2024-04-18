<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="WebApp.Finance.Invoice.Details" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        table.row-info tr td:first-child {
            width: 100px;
        }
    </style>
    <script type="text/javascript">

        var InvoiceData = eval('(<%=this.Model.InvoiceData%>)');
        var MaileDate = eval('(<%=this.Model.MaileDate%>)');
        var OtherData = eval('(<%=this.Model.OtherData%>)');
        var InvoiceLog = eval('(<%=this.Model.InvoiceLog%>)');

        var fromOrder = getQueryString("From") == 'Order';

        //页面加载
        $(function () {
            if (fromOrder) {
                $('#btnPrint').hide();
            }

            $('#productGrid').myDatagrid({
                fitColumns: false,
                fit: false,
                nowrap: false,
                pagination: false,
                rownumbers: true,
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
            //绑定日志信息
            var id = getQueryString("ID");
            var data = new FormData($('#form1')[0]);
            data.append("ID", id);
            $.ajax({
                url: '?action=LoadInvoiceLogs',
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
            //数据初始化
            Init();

            //发票附件列表初始化
            $('#InvoiceFileTable').myDatagrid({
                queryParams:{ InvoiceNoticeID: getQueryString("ID"), },
                actionName: 'InvoiceFiles',
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#invoiceList').text('发票(' + data.total + ')');
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

                    
                    var heightValue = $("#InvoiceFileTable").prev().find(".datagrid-body").find(".datagrid-btable").height() + 20;
                    $("#InvoiceFileTable").prev().find(".datagrid-body").height(heightValue);
                    $("#InvoiceFileTable").prev().height(heightValue);
                    $("#InvoiceFileTable").prev().parent().height(heightValue);
                    $("#InvoiceFileTable").prev().parent().parent().height(heightValue);

                    $("#InvoiceFileTable").prev().find(".datagrid-header").height(0);

                    //调整“其它信息”和“发票附件”的高度
                    var panel3Height = $('#panel3').height();
                    var fileContainerHeight = $('#fileContainer').height();

                    var targetHeight = 0;
                    if (panel3Height >= fileContainerHeight) {
                        targetHeight = panel3Height;
                    } else {
                        targetHeight = fileContainerHeight;
                    }
                    targetHeight = targetHeight + 40;

                    $('#panel3').panel('resize',{
	                    height: targetHeight,
                    });

                    $('#fileContainer').panel('resize',{
	                    height: targetHeight,
                    });

                    $("#InvoiceFileTable").prev().find(".datagrid-header").height(0);

                    $("#panel3").children(":first").height($("#panel3").children(":first").height() - 35);
                    $("#fileContainer").children(":first").height($("#fileContainer").children(":first").height() - 35);

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

        //打印确认单
        function Print() {
            var InvoiceNoticeID = getQueryString("ID");
            var url = location.pathname.replace(/Details.aspx/ig, 'PrintInvoiceConfirm.aspx?ID=' + InvoiceNoticeID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印确认单',
                width: '750px',
                height: '550px',
                onClose: function () {
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/Details.aspx/ig, fromOrder ? '../../Order/Query/InvoicedList.aspx' : 'InvoicedList.aspx');
            window.location = url;
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + "&nbsp;&nbsp;" + row.CreateDate + "&nbsp;  " + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
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


        //查看文件
        function View(url) {
            var from = getQueryString('From');
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
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
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }

            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            return buttons;
        }
    </script>
    <style type="text/css">
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
                    <a class="easyui-linkbutton" id="btnPrint" data-options="iconCls:'icon-print'" onclick="Print()">打印确认单</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left;">
                <div class="sec-container">
                    <div style="display: block; float: left; width: 48%">
                        <div id="panel1" class="easyui-panel" title="开票信息">
                            <div class="sub-container" style="font-size: 12px;">
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
                            <div class="sub-container" style="font-size: 12px;">
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

            <div>
                <div data-options="region:'center',border: false," style="width: 48%; float: left; margin-top: 5px;">
                    <div class="sec-container">
                        <div id="panel3" class="easyui-panel" title="其它信息">
                            <div class="sub-container" style="font-size: 12px;">
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
                <div data-options="region:'center',border: false," style="width: 51%; float: left; margin-top: 5px; margin-left: 4px;">
                    <div class="sec-container">
                        <div id="fileContainer" class="easyui-panel" title="发票附件">
                            <div class="sub-container">
                                <div style="margin-bottom: 5px">
                                    <span>
                                        <img src="../../App_Themes/xp/images/blue-fujian.png" /></span>
                                    <span id="invoiceList" style="font-weight: bold">发票</span>
                                </div>
                                <p id="unUpload" style="display: none">未上传</p>
                                <table id="InvoiceFileTable">
                                    <thead>
                                        <tr>
                                            <th data-options="field:'img',formatter:ShowImg">图片</th>
                                            <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>
                        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
                            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                        </div>
                    </div>
                </div>
            </div>

            <div data-options="region:'center',border: false," style="width: 100%; float: left; margin-top: 5px;">
                <div data-options="region:'center',border: false,">
                    <div id="productGrid-container" class="sec-container">
                        <table id="productGrid" class="easyui-datagrid" title="商品信息" data-options="
                            fitColumns:false,
                            fit:false,
                            nowrap:false,
                            pagination:false,">
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
                                    <th data-options="field:'Amount',width: 100,align:'left'">含税金额</th>
                                    <th data-options="field:'Difference',width: 80,align:'left'">开票差额</th>
                                    <!--<th data-options="field:'InvoiceCode', width: 160,align:'left'">发票代码</th>-->
                                    <th data-options="field:'InvoiceNo', width: 160,align:'left'">发票号码</th>
                                    <th data-options="field:'InvoiceDate', width: 160,align:'left'">开票日期</th>
                                    <th data-options="field:'ProductName',width: 170,align:'left'">产品名称</th>
                                    <th data-options="field:'ProductModel',width: 180,align:'left'">规格型号</th>
                                </tr>
                            </thead>
                        </table>
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
</body>
</html>
