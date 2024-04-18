<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchPrintInvoiceConfirm.aspx.cs" Inherits="WebApp.Finance.Invoice.BatchPrintInvoiceConfirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        var ConfirmInvoiceData = eval('(<%=this.Model.ConfirmInvoiceInfos%>)');
        var InvoiceUrl = eval('(<%=this.Model.InvoiceUrl%>)');
        //页面加载时
        $(function () {
            $('#print').click(function () {
                $("#container").jqprint({
                    importCSS: false, //将此属性改为false；直接在style中写入样式即可
                    printContainer: true,
                });
            });
            intData();
        });

        function intData() {
            var date = new Date();
            var year = date.getFullYear();

            if (ConfirmInvoiceData != null && ConfirmInvoiceData.length > 0) {
                for (var i = 0; i < ConfirmInvoiceData.length; i++) {
                    var strHtml = $("#prepare").html();
                    var $div = $('<div/>', {
                        id: 'invoice_block_' + i,
                        html: strHtml,
                    });
                    $div.width(700);
                   
                    var margin_bottom = 1000;
                    //var invoiceNos = ConfirmInvoiceData[i].InvoiceNo.split(',');
                    //if (invoiceNos.length > 30) {
                    //    margin_bottom = 500;
                    ////}

                    if (i == ConfirmInvoiceData.length - 1) {
                        margin_bottom = 10
                    }

                    $div.css("marginBottom", margin_bottom);
                    $div.css("borderTop", "1px dashed #F00");
                    $div.appendTo('#container');

                    $("#invoice_block_" + i + " [nid='InvoiceNo']").text(ConfirmInvoiceData[i].InvoiceNo);
                    $("#invoice_block_" + i + " [nid='TotalAmout']").text(ConfirmInvoiceData[i].TotalAmout);
                    $("#invoice_block_" + i + " [nid='Summry']").text(ConfirmInvoiceData[i].Summry);
                    $("#invoice_block_" + i + " [nid='datetime']").text(ConfirmInvoiceData[i].DataTime);
                    $("#invoice_block_" + i + " [nid='SealUrl']").attr("src", InvoiceUrl);
                    $("#invoice_block_" + i + " [nid='InvoiceConfirmTitle']").html("您好！我公司于" + year + "年向贵公司("+ConfirmInvoiceData[i].ClientName+")开具增值税发票如下:");
                }
            }




        }
    </script>
</head>
<body>
    <div style="margin-top: 20px; padding-left: 20px">
        <a href="javascript:void(0);" class="easyui-linkbutton hidebtn" id="print" data-options="iconCls:'icon-print'">打印</a>
    </div>

    <div id="container" style="width: 700px; margin-left: 30px">



    </div>

</body>

<div id="prepare" style="width: 700px; margin-left: 30px; display: none;">
    <div style="text-align: center; font-size: 25px;">
        发票收受确认单
    </div>
    <div style="text-align: center; margin-top: 10px">
        <label nid="InvoiceConfirmTitle" style="font-size: 15px; text-align: center;">您好！我公司于+2018+年向贵公司开具增值税发票如下:</label>
    </div>
    <div style="font-size: 15px;">
        <style>
            .tab {
                margin-top: 5px;
                font-family: simsun !important;
                border: 1px solid #d4d7e9;
                border-bottom: 1px solid #d4d7e9;
                border-collapse: collapse; /*合并为单一的边线框*/
            }

                .tab td {
                    border-left: 1px solid #d4d7e9;
                    border-top: 1px solid #d4d7e9;
                }

            table tr th {
                font-weight: 300;
                font-size: 12px;
            }
        </style>
        <!-- 表格 -->
        <div>
            <table class="tab" style="font-size: 15px; line-height: 30px;" border="1" cellspacing="0">
                <tr>
                    <th style="min-width: 400px;">发票号</th>
                    <th style="width: 130px">税费合计金额</th>
                    <th style="width: 80px">备注</th>
                </tr>
                <tr>
                    <td nid="InvoiceNo" style="min-width: 400px; word-wrap: break-word; word-break: break-all;"></td>
                    <td nid="TotalAmout" style="vertical-align:top;text-align:center"></td>
                    <td nid="Summry" style="vertical-align:top;text-align:center"></td>
                </tr>
            </table>
        </div>
    </div>
    <div style="text-align: center; margin-top: 10px">
        <label style="font-size: 15px; text-align: center;">收到发票核实无误后请扫描发送至邮箱:zhanyf@for-ic.net</label>
    </div>
    <div style="text-align: center; margin-top: 10px">
        <label style="font-size: 20px; text-align: center;">若不回传，一律视为已收到发票</label>
    </div>
    <div style="text-align: center; margin-top: 10px">
        <label style="font-size: 15px; text-align: center;">谢谢合作!</label>
    </div>
    <br />
    <div style="font-size: 15px;">
        <div style="float: left">
            <ul style="padding-left: 50px; list-style: none">
                <li style="padding-top: 10px;">出票公司(盖章)
                </li>
                <li style="padding-top: 10px;">经办人(签字)：鲁亚慧
                </li>
                <li style="padding-top: 10px;">日期:<label nid="datetime"></label>
                </li>
            </ul>
        </div>
        <div style="position: relative; float: left; bottom: 34px; left: -30px;">
            <img nid="SealUrl" style="width: 130px" />
        </div>
        <div style="float: right">
            <ul style="padding-right: 160px; list-style: none">
                <li style="padding-top: 10px;">收票公司(盖章)
                </li>
                <li style="padding-top: 10px;">经办人(签字)
                </li>
                <li style="padding-top: 10px;">日期:
                </li>
            </ul>
        </div>
    </div>
    <div style="text-align: center; position: relative; top: 10px; margin-left: 55px; float: left">
        <label style="font-size: 15px; text-align: center;">☆如开票有误，请拨打0755-28503067联系,错误发票当月及时更换，跨月概不退换。</label>
    </div>
</div>

</html>
