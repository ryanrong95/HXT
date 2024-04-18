<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintInvoiceConfirm.aspx.cs" Inherits="WebApp.Finance.PrintInvoiceConfirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        var ConfirmInvoiceData = eval('(<%=this.Model.ConfirmInvoiceInfo%>)');
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

            var date = new Date();
            var year = date.getFullYear();
            $("#InvoiceConfirmTitle").html("您好！我公司于" + year + "年向贵公司开具增值税发票如下:");
        });

        function intData() {
            $("#InvoiceNo").text(ConfirmInvoiceData.InvoiceNo);
            $("#TotalAmout").text(ConfirmInvoiceData.TotalAmout);
            $("#Summry").text(ConfirmInvoiceData.Summry);
            $("#datetime").text(ConfirmInvoiceData.DataTime);
            $("#SealUrl").attr("src", InvoiceUrl);
        }
    </script>

</head>
<body>
    <%--<div class="easyui-tabs" data-option="fit:true;">
        <div title="确认单" style="padding: 10px;">--%>
    <div style="margin-top: 20px; padding-left: 20px">
        <a href="javascript:void(0);" class="easyui-linkbutton hidebtn" id="print"
            data-options="iconCls:'icon-print'">打印</a>
        <%--<a class="easyui-linkbutton" onclick="Back()"
                    data-options="iconCls:'icon-back'">返回</a>--%>
    </div>
    <div id="container" style="width: 700px; margin-left: 30px">
        <div style="text-align: center; font-size: 25px;">
            发票收受确认单
        </div>
        <div style="text-align: center; margin-top: 10px">
            <label id="InvoiceConfirmTitle" style="font-size: 15px; text-align: center;">您好！我公司于+2018+年向贵公司开具增值税发票如下:</label>
        </div>
        <div style="font-size: 15px;">
            <style>
                #tab {
                    margin-top: 5px;
                    font-family: simsun !important;
                    border: 1px solid #d4d7e9;
                    border-bottom: 1px solid #d4d7e9;
                    border-collapse: collapse; /*合并为单一的边线框*/
                }

                    #tab td {
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
                <table id="tab" style="font-size: 15px; line-height: 30px;" border="1" cellspacing="0">
                    <tr>
                        <th style="min-width: 400px;">发票号</th>
                        <th style="width: 150px">税费合计金额</th>
                        <th style="width: 100px">备注</th>
                    </tr>
                    <tr>
                        <td id="InvoiceNo" style="min-width: 400px; word-wrap: break-word; word-break: break-all;"></td>
                        <td id="TotalAmout"></td>
                        <td id="Summry"></td>
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
                    <li style="padding-top: 10px;">日期:<label id="datetime"></label>
                    </li>
                </ul>
            </div>
            <div style="position: relative; float: left; bottom: 34px; left: -30px;">
                <img id="SealUrl" style="width: 130px" />
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
    <%--</div>
    </div>--%>
</body>
</html>
