<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="PrintInvoiceConfirm.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Invoice.PrintInvoiceConfirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../Content/Themes/Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../Content/Themes/Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../Content/Themes/Styles/jquery.jqprint.css" rel="stylesheet" />
    <script>
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
            $("#InvoiceNo").text(model.ConfirmInvoiceInfo.InvoiceNo);
            $("#TotalAmout").text(model.ConfirmInvoiceInfo.TotalAmout);
            $("#Summry").text(model.ConfirmInvoiceInfo.Summry);
            $("#datetime").text(model.ConfirmInvoiceInfo.DataTime);
            $("#SealUrl").attr("src", model.InvoiceUrl);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="margin-top: 20px; padding-left: 20px">
        <a href="javascript:void(0);" class="easyui-linkbutton hidebtn" id="print" data-options="iconCls:'icon-print'">打印</a>
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
            <label style="font-size: 15px; text-align: center;">收到发票核实无误后请扫描发送至邮箱:sunny@wl.net.cn</label>
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
                    <li style="padding-top: 10px;">经办人(签字)：曹丽萍
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
            <label style="font-size: 15px; text-align: center;">☆如开票有误，请拨打0755-83995933联系,错误发票当月及时更换，跨月概不退换。</label>
        </div>
    </div>
</asp:Content>
