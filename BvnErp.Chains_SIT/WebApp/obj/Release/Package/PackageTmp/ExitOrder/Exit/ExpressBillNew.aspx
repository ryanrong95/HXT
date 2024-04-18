<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpressBillNew.aspx.cs" Inherits="WebApp.ExitOrder.Exit.ExpressBillNew" %>

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
           //获取出库产品数据
            var ExitNoticeID = getQueryString("ExitNoticeID")
           
            InitExitNotice();
            showData();

            //隐藏打印按钮
            HideBtn();
        });

        //初始化出库信息
        function InitExitNotice() {
            $("#OrderID").text(ExitNotice.OrderID);
            $("#Custumers").text(ExitNotice.ClientName);
            $("#ExpressComp").text(ExitNotice.ExpressComp);
            $("#ExpressCode").text(ExitNotice.ExpressCode)
            $("#Contactor").text(ExitNotice.Contactor);
            $("#Tel").text(ExitNotice.ContactTel);
            $("#ExpressTy").text(ExitNotice.ExpressTy);
            $("#SendAddress").text(ExitNotice.Address);
            $("#ExpressPayType").text(ExitNotice.ExpressPayType);
            $("#PackNo").text(ExitNotice.PackNo);
            $("#DeliveryTime").text(ExitNotice.DeliveryTime);
            $("#PakingDate").text(ExitNotice.SZPackingDate);
            $("#CompanyName").text(ExitNotice.Purchaser + "送货单");
            $("#SealUrl").attr("src", ExitNotice.SealUrl);
        };



        //返回
        function Back() {            
                var url = location.pathname.replace(/ExpressBillNew.aspx/ig, 'UnExitedNew.aspx');
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
    <div class="easyui-tabs" data-option="fit:true;">
        <div title="送货单" style="padding: 10px;">
            <form id="form1">
                <div style="margin-top: 10px">               
                    <a class="easyui-linkbutton" onclick="Back()"
                        data-options="iconCls:'icon-back'">返回</a>
                </div>
                <div id="container" style="width: 745px; margin: 20px auto">
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
                    </div>
                    <div class="print-row">
                        <p>
                            订单编号：<label id="OrderID"></label>
                            <span style="margin-left: 10px">送货日期：<label id="DeliveryTime"></label></span>
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
                                <td>
                                    <p>收货地址：</p>
                                    <p>
                                        <label id="SendAddress"></label>
                                    </p>
                                </td>
                                <td style="width: 150px">
                                    <p>联系人：<label id="Contactor"></label></p>
                                    <p>电话：<label id="Tel"></label></p>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <p>
                                        快递公司：<label id="ExpressComp"></label>
                                        快递单号：<label id="ExpressCode"></label>
                                    </p>
                                    <p>
                                        快递方式：<label id="ExpressTy"></label>
                                        付费方式：<label id="ExpressPayType"></label>
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
