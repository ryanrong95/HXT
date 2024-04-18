<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryBill.aspx.cs" Inherits="WebApp.ExitOrder.Exit.DeliveryBill" %>

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
        var ExitStatus = getQueryString('ExitStatus');
        //页面加载时
        $(function () {
            $('#barcodeTarget').empty().barcode(ExitNotice.ExitNoticeID, "code128", {//code128为条形码样式
                output: 'css',
                color: '#000000',
                barWidth: 2,        //单条条码宽度
                barHeight: 30,     //单体条码高度
                addQuietZone: false,
                showHRI: true
            });
            //打印
            $('#print').click(function () {
                $("#container").jqprint();
            })

            var ExitNoticeID = getQueryString("ExitNoticeID")
            var data = new FormData($('#form1')[0]);
            data.append("ExitNoticeID", ExitNoticeID);
            $.ajax({
                url: '?action=ProductData',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    /*这个方法里是ajax发送请求成功之后执行的代码*/
                    showData(data);//我们仅做数据展示
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });
            InitExitNotice();

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
            var ExitStatus = getQueryString("ExitStatus");
            if (ExitStatus == 4) {
                var url = location.pathname.replace(/DeliveryBill.aspx/ig, 'Exited.aspx');
                window.location = url;
            }
            else {
                var url = location.pathname.replace(/DeliveryBill.aspx/ig, 'UnExited.aspx');
                window.location = url;
            }
        }

        function showData(data) {
            var str = "";//定义用于拼接的字符串
            for (var index = 0; index < data.rows.length; index++) {
                var row = data.rows[index];
                var count = index + 1;
                //拼接表格的行和列
                str = "<tr><td>" + count + "</td><td>" + row.StockCode + "</td><td>" + row.CaseNumber + "</td><td>" + row.ProductName + "</td>" +
                    "<td>" + row.Model + "<td>" + row.Manufacturer + "</td><td>" + row.Qty + "</td></tr>";
                //追加到table中
                $("#tab").append(str);
            }
            UniteTable("tab", 7)
        }

        function UniteTable(tableId, colLength) {//表格ID，表格列数
            var tb = document.getElementById(tableId);
            tb.style.display = '';
            var i = 0;
            var j = 0;
            var rowCount = tb.rows.length; //   行数 
            var colCount = tb.rows[0].cells.length - 4; //   列数 
            var obj1 = null;
            var obj2 = null;
            //为每个单元格命名 
            for (i = 0; i < rowCount; i++) {
                for (j = 0; j < colCount; j++) {
                    tb.rows[i].cells[j].id = "tb__" + i.toString() + "_" + j.toString();
                }
            }
            //合并行 
            for (i = 0; i < colCount; i++) {
                if (i == colLength) break;
                obj1 = document.getElementById("tb__0_" + i.toString())
                for (j = 1; j < rowCount; j++) {
                    obj2 = document.getElementById("tb__" + j.toString() + "_" + i.toString());
                    if (obj1.innerText == obj2.innerText && ((obj2.innerText != "" || obj1.innerText != "") && (obj1.innerText != "-" || obj2.innerText != "-"))) {
                        obj1.rowSpan++;
                        obj2.parentNode.removeChild(obj2);
                    } else {
                        obj1 = document.getElementById("tb__" + j.toString() + "_" + i.toString());
                    }
                }
            }
            //合并列
            for (i = 0; i < rowCount; i++) {
                colCount = tb.rows[i].cells.length - 4;
                obj1 = document.getElementById(tb.rows[i].cells[0].id);
                for (j = 1; j < colCount; j++) {
                    if (j >= colLength) break;
                    if (obj1.colSpan >= colLength) break;
                    obj2 = document.getElementById(tb.rows[i].cells[j].id);
                    if (obj1.innerText == obj2.innerText && ((obj2.innerText != "" || obj1.innerText != "") && (obj1.innerText != "-" || obj2.innerText != "-"))) {
                        obj1.colSpan++;
                        obj2.parentNode.removeChild(obj2);
                        j = j - 1;
                    }
                    else {
                        obj1 = obj2;
                        j = j + obj1.rowSpan;
                    }
                }
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
                    <a href="javascript:void(0);" class="easyui-linkbutton hidebtn" id="print"
                        data-options="iconCls:'icon-print'">打印</a>
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
                            line-height: 20px;
                            font-family: simsun !important;
                        }

                        p, p label, p span {
                            font-size: 10px;
                            line-height: 20px;
                            font-family: simsun !important;
                        }
                    </style>
                    <div style="text-align: center;">
                        <span style="font-size: 18px; font-family: simsun;">
                            <label id="CompanyName"></label>
                        </span>
                        <div id="barcodeTarget" style="float: right; padding: 0; overflow: auto; width: 100px;"></div>
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
                                <th style="width: 8%">库位号</th>
                                <th style="width: 8%">箱号</th>
                                <th style="width: 22%">品名</th>
                                <th style="width: 25%">型号</th>
                                <th style="width: 10%">品牌</th>
                                <th style="width: 10%">数量</th>
                            </tr>
                        </table>
                        <%--客户评价--%>
                        <table id="table_info">
                            <tbody>
                                <%--   <tr>
                                    <td colspan="2">
                                        <span>收款方式：电汇</span>
                                        <span style="margin-left: 32px">结算方式：约定期限</span>
                                        <span style="margin-left: 32px">发票交付方式：邮寄</span>
                                    </td>
                                </tr>--%>
                                <tr>
                                    <td style="width: 120px">对本次服务评价：</td>
                                    <td>
                                        <span>口</span>&nbsp;优秀&nbsp;&nbsp;&nbsp; 
                                        <span>口</span>&nbsp;一般&nbsp;&nbsp;&nbsp;
                                        <span>口</span>&nbsp;差
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 120px">客户意见或建议：</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="2">本公司已如数收到上述货物和发票，无货物数量损失，无货物损坏。</td>
                                </tr>
                            </tbody>
                        </table>
                        <%--客户签收--%>
                        <div class="print-row" style="line-height: 30px; font-size: 14px; float: right; margin-right: 120px; margin-top: 15px">
                            <ul style="list-style: none">
                                <li>收货人签字/签章:</li>
                                <li>收货日期:  _____年 ____月 ____ 日</li>
                            </ul>
                        </div>
                        <%--印章--%>
                        <div style="float: left; position: relative; left: 100px; top: 1px;">
                            <img id="SealUrl" style="width: 120px;" />
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
