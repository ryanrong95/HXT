<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LadingBillNew.aspx.cs" Inherits="WebApp.ExitOrder.Exit.LadingBillNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-barcode.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        var ExitNotice = eval('(<%=this.Model.ExitNotice%>)');
        var ExitNoticeItems = eval('(<%=this.Model.ExitNoticeItems%>)');
        var ExitStatus = getQueryString('ExitStatus');
        //页面加载时
        $(function () {
            var ExitNoticeID = getQueryString("ExitNoticeID")

            InitExitNotice();
            showData();

            HideBtn();
        });

        function InitExitNotice() {
            $("#ExitNoticeID").text(ExitNotice.ExitNoticeID);
            $("#OrderID").text(ExitNotice.OrderID);
            $("#Custumers").text(ExitNotice.ClientName);
            $("#DeliveryName").text(ExitNotice.DeliveryName);
            $("#DeliveryTel").text(ExitNotice.DeliveryTel);
            $("#IDType").text(ExitNotice.IDType == null ? "" : ExitNotice.IDType);
            $("#IDCard").text(ExitNotice.IDCard == null ? "" : ExitNotice.IDCard);
            $("#PackNo").text(ExitNotice.PackNo == null ? "" : ExitNotice.PackNo);
            $("#DeliveryTime").text(ExitNotice.DeliveryTime);
            $("#PakingDate").text(ExitNotice.SZPackingDate);
            $("#CompanyName").text(ExitNotice.Purchaser + "提货单");
        };

        //返回
        function Back() {
            var url = location.pathname.replace(/LadingBillNew.aspx/ig, 'UnExitedNew.aspx');
            window.location = url;
        }

        function showData(data) {
            var data = ExitNoticeItems;   
            var str = "";//定义用于拼接的字符串
            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                var count = index + 1;
                //拼接表格的行和列
                str = "<tr><td>" + count + "</td><td>" + row.Name + "</td><td>" + row.Model + "</td><td>" + row.Brand + "</td>" +
                    "<td>" + row.Qty + "<td></tr>";
                //追加到table中
                $("#tab").append(str);
            }
        }

      

        // 隐藏打印按钮；
        function HideBtn() {
            if (ExitStatus == 4) {
                $(".hidebtn").hide();
            } else {
                $(".hidebtn").show();
            }
        }
    </script>
</head>
<body style="width: 100%; height: 100%; text-align: center; margin-left: 5px;">
    <div class="easyui-tabs" data-option="fit:true" style="border-width: 0px; border-style: none;">
        <div title="提货单" style="padding: 10px;">
            <form id="form1">
                <div style="margin-top: 10px">                 
                    <a class="easyui-linkbutton" onclick="Back()"
                        data-options="iconCls:'icon-back'">返回</a>
                </div>
                <div id="container" style="width: 745px; margin: 20px auto; border: none;">
                    <%--样式--%>
                    <style>
                        table {
                            width: 100%;
                        }

                            table, table tr th, table tr td {
                                border-spacing: 0;
                                border: 1px solid;
                                border-collapse: collapse;
                                border-spacing: 0;
                                font-size: 10px;
                                border-color: grey;
                            }

                                table tr td p {
                                    margin-left: 5px;
                                    vertical-align: top;
                                    vertical-align: text-top;
                                }

                        .print-row:before, .print-row:after {
                            content: '';
                            display: block;
                            clear: both;
                        }

                        #tab {
                            line-height: 12px !important;
                            margin-top: 5px;
                            font-family: simsun !important;
                        }

                            #tab tr {
                                text-align: center;
                            }

                        #table_info {
                            margin-top: 5px;
                            font-size: 10px;
                            line-height: 20px;
                            font-family: simsun !important;
                        }

                        p, p label, p span {
                            font-size: 10px;
                            line-height: 20px;
                            font-family: simsun !important;
                        }

                          .panel-body {
                            color: black;
                        }
                    </style>
                    <div style="text-align: center; font-size: 18px; font-family: simsun">
                        <label id="CompanyName"></label>
                        <div id="barcodeTarget" style="float: right; padding: 0px; overflow: auto; width: 100px;">
                        </div>
                    </div>
                    <div class="print-row">
                        <p>
                            订单编号：<label id="OrderID"></label>
                            <span style="margin-left: 10px">提货日期：<label id="DeliveryTime"></label></span>
                            <span style="margin-left: 10px">装箱日期：<label id="PakingDate"></label></span>
                            <span style="float: right">总件数：<label id="PackNo"></label></span>
                        </p>
                        <div style="width: 36%; float: left;"></div>
                        <table style="margin-top: 5px; font-size: 12px; line-height: 20px;">
                            <tr>
                                <td>
                                    <p>收货人：</p>
                                    <p>
                                        <label id="Custumers"></label>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <p>提货人：<label id="DeliveryName"></label></p>
                                    <p>
                                        证件类型：<label id="IDType"></label>&nbsp&nbsp
                                        证件号码：<label id="IDCard"></label>&nbsp&nbsp
                                        电话：<label id="DeliveryTel"></label>
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="print-row">
                        <%--产品信息--%>
                        <table id="tab">
                            <tr>
                                <th style="width: 5%">序号</th>                               
                                <th style="width: 25%">品名</th>
                                <th style="width: 30%">型号</th>
                                <th style="width: 30%">品牌</th>
                                <th style="width: 10%">数量</th>
                            </tr>
                        </table>
                                            
                    </div>
                </div>
            </form>
        </div>
    </div>
</body>
</html>

