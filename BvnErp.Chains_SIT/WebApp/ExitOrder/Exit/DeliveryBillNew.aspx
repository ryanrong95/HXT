<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryBillNew.aspx.cs" Inherits="WebApp.ExitOrder.Exit.DeliveryBillNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        var ExitNotice = eval('(<%=this.Model.ExitNotice%>)');
        var ExitNoticeItems = eval('(<%=this.Model.ExitNoticeItems%>)');
        var ExitStatus = getQueryString('ExitStatus');
        //页面加载时
        $(function () {
            var ExitNoticeID = getQueryString("ExitNoticeID")
            //var data = new FormData($('#form1')[0]);
            //data.append("ExitNoticeID", ExitNoticeID);
            //$.ajax({
            //    url: '?action=ProductData',
            //    type: 'POST',
            //    data: data,
            //    dataType: 'JSON',
            //    cache: false,
            //    processData: false,
            //    contentType: false,
            //    success: function (data) {
            //        /*这个方法里是ajax发送请求成功之后执行的代码*/
            //        showData(data);//我们仅做数据展示
            //    },
            //    error: function (msg) {
            //        alert("ajax连接异常：" + msg);
            //    }
            //});
            InitExitNotice();
            showData();

            HideBtn();
        });

        function InitExitNotice() {
            $("#ExitNoticeID").text(ExitNotice.ExitNoticeID);
            $("#OrderId").text(ExitNotice.OrderID);
            $("#Custumers").text(ExitNotice.ClientName);
            $("#DriverName").text(ExitNotice.DriverName == null ? "" : ExitNotice.DriverName);
            $("#Contactor").text(ExitNotice.Contactor == null ? "" : ExitNotice.Contactor);
            $("#ContantTel").text(ExitNotice.ContactTel == null ? "" : ExitNotice.ContactTel);
            $("#DeliveryAddress").text(ExitNotice.Address == null ? "" : ExitNotice.Address);
            $("#DriverTel").text(ExitNotice.DriverTel == null ? "" : ExitNotice.DriverTel);
            $("#License").text(ExitNotice.License == null ? "" : ExitNotice.License);
            $("#PackNo").text(ExitNotice.PackNo);
            $("#DeliveryTime").text(ExitNotice.DeliveryTime);
            $("#PakingDate").text(ExitNotice.SZPackingDate);
            $("#CompanyName").text(ExitNotice.Purchaser + "送货单");
            $("#SealUrl").attr("src", ExitNotice.SealUrl);
        };

        //返回
        function Back() {
            var url = location.pathname.replace(/DeliveryBillNew.aspx/ig, 'UnExitedNew.aspx');
            window.location = url;
        }

        function showData() {
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

        function HideBtn() {
            if (ExitStatus == 4) {
                $(".hidebtn").hide();
            } else {
                $(".hidebtn").show();
            }
        }

    </script>
</head>
<body style="width: 100%; height: 100%; text-align: left; margin-left: 5px;">
    <div class="easyui-tabs" data-option="fit:true;">
        <div title="送货单" style="padding: 10px;">
            <form id="form1">
                <div style="margin-top: 20px">
                    <a class="easyui-linkbutton" onclick="Back()"
                        data-options="iconCls:'icon-back'">返回</a>
                </div>
                <div id="container" style="width: 745px; margin: 20px auto">
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
                                border-color: black;
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
                    <div style="text-align: center;">
                        <span style="font-size: 18px; font-family: simsun;">
                            <label id="CompanyName"></label>
                        </span>
                    </div>
                    <div class="print-row">
                        <p>
                            订单编号：<label id="OrderId"></label>
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
                                        <label id="DeliveryAddress"></label>
                                    </p>
                                </td>
                                <td style="width: 150px">
                                    <p>联系人：<label id="Contactor"></label></p>
                                    <p>
                                        电话号码： 
                                        <label id="ContantTel"></label>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <p>送货人：<label id="DriverName"></label></p>
                                    <p>
                                        电话：
                                        <label id="DriverTel"></label>
                                        &nbsp&nbsp
                                        车牌号：<label id="License"></label>
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
