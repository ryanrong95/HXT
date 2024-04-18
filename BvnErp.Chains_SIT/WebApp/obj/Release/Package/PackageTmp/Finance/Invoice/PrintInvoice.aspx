<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintInvoice.aspx.cs" Inherits="WebApp.Finance.PrintInvoice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>打印发票运单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-barcode.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <style>
        .kddiv {
            position: relative;
        }
    </style>
    <style>
        * {
            margin: 0;
            padding: 0;
            font-family: '黑体', 'Arial' !important;
        }



        .clearfix:after {
            content: '\20';
            display: block;
            height: 0;
            clear: both;
        }
    </style>

    <script type="text/javascript">
        var ExpressCompanyData = eval('(<%=this.Model.ExpressCompanyData%>)');

        $(function () {
            var invoiceNoticeIDs = getQueryString('IDs');
            //初始化快递信息
            $('#ExpressName').combobox({
                data: ExpressCompanyData,
                onLoadSuccess: function (data) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Text == "顺丰") {
                            $('#ExpressName').combobox('select', data[i].Value);
                        }
                    }
                },
                onSelect: function (record) {
                    $.post('?action=ExpressSelect', { ID: record.Value }, function (data) {
                        //更新快递方式
                        data = eval(data);
                        $('#ExpressType').combobox({
                            data: data,
                        });
                        //默认选择第一行
                        $('#ExpressType').combobox('setValue', data[0].Value);
                    })
                },
            });
            //
            $('#WaybillCode').textbox({
                readonly: true,
            });
            $('#InvoiceNotice').textbox({
                readonly: true,
                multiline: true,
            });
            $('#InvoiceNotice').textbox("setValue", (invoiceNoticeIDs + ''));
        });

        //生成快递面单
        function Print() {
            //验证表单数据
            //debugger;
            if (!Valid('form1')) {
                return;
            }
            var InvoiceNotice = $('#InvoiceNotice').textbox('getValue');
            var ExpressName = $('#ExpressName').combobox('getValue');
            var ExpressType = $('#ExpressType').combobox('getValue');
            $.ajax({
                type: "GET",
                url: "?action=ExpressSelf",
                data: { InvoiceNotice: InvoiceNotice, ExpressName: ExpressName, ExpressType: ExpressType },
                dataType: "json",
                success: function (data) {
                    if (data.success) {
                        $('#WaybillCode').textbox('setValue', data.LogisticCode);
                        if (data.ShipperCode == "SF") {

                            var $div1 = $("#expresskdd");
                            //var $divf = $("<div class='kddiv'></div>").append("<img src='"+data.PrintTemplate+"'  />");
                            var $divf = $("<img src='" + data.PrintTemplate + "' alt='SF' height='680' width='350' />");
                            $div1.append($divf);
                        }
                        if (data.ShipperCode == "EMS") {
                            //$('#WaybillCode').textbox('setValue', data.LogisticCode);
                            $("#expresskdd").html("");
                            $(".ems").show();

                            var waibill = data.message;
                            //填充内容
                            $("#etime").html("时间:" + timeStamp2String(new Date().getTime()));
                            //运单号
                            $.each($(".cwaybillno"), function (index, val) {
                                $(val).html(data.LogisticCode);
                            });
                            //条形码
                            //$("#tiaoxingma1").barcode(data.LogisticCode, "ean13");
                            //条形码
                            $('#tiaoxingma1').empty().barcode(data.LogisticCode, "code128", {
                                output: 'css',
                                color: '#000000',
                                addQuietZone: false,
                                showHRI: false
                            });
                            $('#tiaoxingma1').css("overflow", "hidden");
                            $('#tiaoxingma2').empty().barcode(data.LogisticCode, "code128", {
                                output: 'css',
                                color: '#000000',
                                barWidth: 1,
                                barHeight: 34,
                                addQuietZone: false,
                                showHRI: false
                            });
                            $('#tiaoxingma3').empty().barcode(data.LogisticCode, "code128", {
                                output: 'css',
                                color: '#000000',
                                barWidth: 1,
                                barHeight: 34,
                                addQuietZone: false,
                                showHRI: false
                            });
                            //路径
                            $("#eroute").html(waibill.RouteCode);
                            //收件人
                            $.each($(".cReceiverName"), function (index, val) {
                                $(val).html("收: " + waibill.RName + " " + waibill.RMobile);
                            });
                            //收件地址
                            $.each($(".cReceiverAddress"), function (index, val) {
                                $(val).html("&nbsp;&nbsp;&nbsp;&nbsp;" + waibill.RAddress);
                            });
                            //发件人
                            $.each($(".cSenderName"), function (index, val) {
                                $(val).html("寄: " + waibill.SName + " " + waibill.SMobile);
                            });
                            //发件地址
                            $.each($(".cSenderAddress"), function (index, val) {
                                $(val).html("&nbsp;&nbsp;&nbsp;&nbsp;" + waibill.SAddress);
                            });






                            //加入Dome
                            $("#expresskdd").append($(".ems"));

                        }
                        //hideKdd();
                    } else {
                        $.messager.alert('消息', "生成快递面单:" + data.message, 'info');
                    }
                },


            });
        }

        //打印快递面单
        function Printkdd() {
            var waybillCode = $('#WaybillCode').textbox('getValue');
            if (waybillCode == "" || waybillCode == null) {
                $.messager.alert('消息', "请先生成发票运单");
                return;
            }
            event.preventDefault();
            $("#expresskdd").jqprint();
        }

        //保存运单数据
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证运单号
            if ($('#WaybillCode').textbox('getValue') == "") {
                $.messager.alert('消息', "运单号为空，保存失败");
                return;
            }
            var data = new FormData($('#form1')[0]);
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }

        function timeStamp2String(time) {
            var datetime = new Date();
            datetime.setTime(time);
            var year = datetime.getFullYear();
            var month = datetime.getMonth() + 1 < 10 ? "0" + (datetime.getMonth() + 1) : datetime.getMonth() + 1;
            var date = datetime.getDate() < 10 ? "0" + datetime.getDate() : datetime.getDate();
            var hour = datetime.getHours() < 10 ? "0" + datetime.getHours() : datetime.getHours();
            var minute = datetime.getMinutes() < 10 ? "0" + datetime.getMinutes() : datetime.getMinutes();
            var second = datetime.getSeconds() < 10 ? "0" + datetime.getSeconds() : datetime.getSeconds();
            return year + "-" + month + "-" + date + " " + hour + ":" + minute + ":" + second;
        }
    </script>
</head>
<body>
    <div class="content" style="margin: 0 auto">
        <div style="float: left; padding-left: 10px">
            <form id="form1" runat="server" method="post">
                <div>
                    <table style="margin: 0 auto; line-height: 30px">
                        <tr>
                            <td class="lbl">开票通知：</td>
                            <td>
                                <input class="easyui-textbox" id="InvoiceNotice" name="InvoiceNotice" data-options="required:true,height:26,width:210" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">快递公司：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressName" name="ExpressName" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">快递方式：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressType" name="ExpressType" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">运单号：</td>
                            <td>
                                <input class="easyui-textbox" id="WaybillCode" name="WaybillCode" data-options="height:26,width:210,validType:'length[1,50]'" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style="text-align: right">
                                <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-ok'" onclick="Print()">生成发票运单</a>
                                <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-print'" onclick="Printkdd()">打印发票运单</a>
                            </td>
                        </tr>
                    </table>
                </div>
            </form>
        </div>
        <div style="padding-top: 10px; padding-right: 10px; float: right">
            <div id="expresskdd">
                <style>
                    #expresskdd * {
                        font-family: '微软雅黑' !important;
                    }
                </style>
            </div>
        </div>

        <div class="ems" style="display: none; width: 100mm; height: 208mm; padding-top: 2mm; background-color: #fff; margin: 0 auto;">
            <div style="width: 96mm; height: 206mm; border: 1px dashed #000; margin: 0 auto;">
                <!-- 第一联 -->
                <div>
                    <!-- 第一联第一行 -->
                    <div style="height: 19mm; border-bottom: 1px dashed #000;zoom: 1;">
                        <div style="float: left; height: 18mm; width: 40mm; padding-top: 7mm; border-right: 1px dashed #000;">
                            <p style="font-size: 24px; text-align: center;">
                                标准快递
                            </p>
                            <div style="text-align: left;">
                                <span id="etime" style="display: inline-block; padding-left: 1mm; font-size: 10px; transform: scale(0.75); width: 120%; margin-left: -6mm;">时间:2017-09-11 16:48:41</span>
                            </div>
                        </div>
                        <div style="float: left; width: 55mm; height: 18mm; text-align: center; padding-top: 1px;">
                            <%--<img src="./imgs/tiaoxingma.png" alt="" class="erweima">--%>
                            <div id="tiaoxingma1" style="margin: 0 auto; width: 53mm; height: 13mm;">
                            </div>
                            <p class="cwaybillno" style="margin-top: 2px; font-size: 14px;">TEST200024706</p>
                        </div>
                    </div>
                    <!-- 第一联第二行 -->
                    <div id="eroute" style="width: 100%; height: 9mm; line-height: 9mm; text-align: center; border-bottom: 1px dashed #000; font-size: 24px; font-weight: bold;">
                        天津-同城-天津站
                    </div>
                    <!-- 第一联第三行 -->
                    <div style="height: 19mm; border-bottom: 1px dashed #000; font-size: 16px; font-weight: bold; padding: 0 1mm;">
                        <p class="cReceiverName">收: 洪小杰 1390000009</p>
                        <p class="cReceiverAddress">&nbsp;&nbsp;&nbsp;&nbsp;海河东路88号</p>
                    </div>
                    <!-- 第一联第四行 -->
                    <div style="height: 9mm; border-bottom: 1px dashed #000; font-size: 12px;">
                        <p class="cSenderName">寄: 丽丽 1320000002</p>
                        <p class="cSenderAddress">&nbsp;&nbsp;&nbsp;&nbsp;海河东路88号</p>
                    </div>
                    <!-- 第一联第五行 -->
                    <div style="height: 15mm; border-bottom: 1px dashed #000; font-size: 12px;zoom: 1;">
                        <div style="float: left; width: 54mm; height: 14mm; padding-left: 1mm; padding-top: 2mm; border-right: 1px dashed #000;">
                            <p>付款方式：</p>
                            <p>计费重量(KG):</p>
                            <p>保价金额(元):</p>
                        </div>
                        <div style="float: left; height: 100%; width: 40mm;">
                            <p>收件人\代收人:</p>
                            <p>签收时间:&nbsp;&nbsp;年&nbsp;&nbsp;月&nbsp;&nbsp;时&nbsp;&nbsp;日</p>
                            <div>
                                <p style="font-size: 10px; transform: scale(0.75); margin-top: -1px; margin-left: -7mm; width: 139%;">
                                    快件递达收货人地址，经收件人或经收件人允许的代收人签字，视为递达。
                                </p>
                            </div>
                        </div>
                    </div>
                    <!-- 第一联第六行 -->
                    <div style="height: 16mm; border-bottom: 1px dashed #000; font-size: 12px;">
                        <p><span>件数:</span>&nbsp;&nbsp;&nbsp;<span>重量(KG):</span></p>
                        <p>配货信息：文件票据</p>
                    </div>
                </div>
                <!-- 分隔带 -->
                <div style="height: 3mm; border-bottom: 1px dashed #000;">
                </div>
                <!-- 第二联 -->
                <div class="page2">
                    <!-- 第二联第一行 -->
                    <div style="height: 15mm; border-bottom: 1px dashed #000;zoom: 1;">
                        <div style="margin-top: 1mm; float: left; width: 52mm; text-align: center;">
                            <%--<img src="../../Content/images/tiaoxingma.png" alt="" class="erweima">--%>
                            <div id="tiaoxingma2" style="margin: 0 auto; overflow: hidden; width: 53mm; height: 13mm; width: 48mm; height: 9mm;">
                            </div>
                            <p class="cwaybillno" style="font-size: 14px;">TEST200024706</p>
                        </div>
                        <div style="width: 43mm; float: left; text-align: center; padding-top: 4mm;">
                            <img src="../../Content/images/ems_logo.png" alt="" class="logo" style="width: 30mm; height: 7mm;">
                        </div>
                    </div>
                    <!-- 第二联第二行 -->
                    <div style="height: 18mm; border-bottom: 1px dashed #000; font-size: 12px;zoom: 1;">
                        <div  style="width: 55mm; height: 100%; border-right: 1px dashed #000;float: left;">
                            <p class="cReceiverName">收: 洪小杰 1390000009</p>
                            <p class="cReceiverAddress">&nbsp;&nbsp;&nbsp;&nbsp;海河东路88号</p>
                        </div>
                        <div style="float: left; width: 40mm; height: 100%;">
                            <p class="cSenderName">寄: 丽丽 1320000002</p>
                            <p class="cSenderAddress">&nbsp;&nbsp;&nbsp;&nbsp;海河东路88号</p>
                        </div>
                    </div>
                    <!-- 第二联第三行 -->
                    <div style="height: 17mm; border-bottom: 1px dashed #000; font-size: 12px;">
                        备注:
                    </div>
                    <!-- 第二联第四行 -->
                    <div style="height: 7mm; border-bottom: 1px dashed #000; font-size: 12px;zoom: 1;">
                        <div style="float: left; width: 68mm; line-height: 7mm; text-align: center; border-right: 1px dashed #000;">
                            <span>网址:www.ems.com.cn</span>
                            <span>客服电话:11180</span>
                        </div>
                        <div class="p2_esm_m4_right"></div>
                    </div>
                </div>
                <!-- 分隔带 -->
                <div style="height: 3mm; border-bottom: 1px dashed #000;">
                </div>
                <!-- 第三联 -->
                <div class="page2">
                    <!-- 第三联第一行 -->
                    <div style="height: 15mm; border-bottom: 1px dashed #000;zoom: 1;">
                        <div style="margin-top: 1mm; float: left; width: 52mm; text-align: center;">
                            <%--<img src="./imgs/tiaoxingma.png" alt="" class="erweima">--%>
                            <div id="tiaoxingma3" style="margin: 0 auto; overflow: hidden; width: 53mm; height: 13mm; width: 48mm; height: 9mm;">
                            </div>
                            <p class="cwaybillno" style="font-size: 14px;">TEST200024706</p>
                        </div>
                        <div style="width: 43mm; float: left;text-align: center; padding-top: 4mm;">
                            <img src="../../Content/images/ems_logo.png" alt="" class="logo" style="width: 30mm; height: 7mm;">
                        </div>
                    </div>
                    <!-- 第三联第二行 -->
                    <div style="height: 17mm; border-bottom: 1px dashed #000; font-size: 12px;zoom: 1;">
                        <div style="float: left; width: 55mm; height: 100%; border-right: 1px dashed #000;">
                            <p class="cReceiverName">收: 洪小杰 1390000009</p>
                            <p class="cReceiverAddress">&nbsp;&nbsp;&nbsp;&nbsp;海河东路88号</p>
                        </div>
                        <div style="float: left; width: 40mm; height: 100%;">
                            <p class="cSenderName">寄: 丽丽 1320000002</p>
                            <p class="cSenderAddress">&nbsp;&nbsp;&nbsp;&nbsp;海河东路88号</p>
                        </div>
                    </div>
                    <!-- 第三联第三行 -->
                    <div  style="height: 17mm; border-bottom: 1px dashed #000; font-size: 12px;">
                        备注:
                    </div>
                    <!-- 第三联第四行 -->
                    <div style="height: 7mm; border-bottom: 1px dashed #000; font-size: 12px;zoom: 1;">
                        <div style="float: left; width: 68mm; line-height: 7mm; text-align: center; border-right: 1px dashed #000;">
                            <span>网址:www.ems.com.cn</span>
                            <span>客服电话:11180</span>
                        </div>
                        <div style="float: left;"></div>
                    </div>
                </div>
            </div>
        </div>

    </div>
</body>
</html>
