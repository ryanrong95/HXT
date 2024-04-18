<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocsLadingBill.aspx.cs" Inherits="WebApp.SZWarehouse.Exit.Docs.DocsLadingBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/jquery-barcode.js"></script>
    <script src="../../../Scripts/Ccs.js"></script>
    <script>
        var ExitNotice = eval('(<%=this.Model.ExitNotice%>)');

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
                    //HideBtn();
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });

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
           // $("#PakingDate").text(ExitNotice.SZPackingDate);
            $('#Purchaser').text(ExitNotice.Purchaser + '提货单');
        };

        function showData(data) {

            var str = "";//定义用于拼接的字符串
            // var PackingDate = data.rows[0].PackingDate;

            for (var index = 0; index < data.rows.length; index++) {
                var row = data.rows[index];
                var count = index + 1;
                //拼接表格的行和列
                str = "<tr><td>" + count + "</td><td>" + row.StockCode + "</td><td>" + row.CaseNumber + "</td><td>" + row.ProductName + "</td>" +
                    "<td>" + row.Model + "<td>" + row.Manufacturer + "</td><td>" + row.Qty + "</td><td>"+row.UpdateDate+"</td></tr>";
                //追加到table中
                $("#tab").append(str);
            }
            UniteTable("tab", 7)
            InitExitNotice();
        }

        //合并单元格
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
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
                </style>
                <div style="text-align: center; font-size: 18px; font-family: simsun">
                    <label id="Purchaser"></label>
                    <div id="barcodeTarget" style="float: right; padding: 0px; overflow: auto; width: 100px;">
                    </div>
                </div>
                <div class="print-row">
                    <p>
                        订单编号：<label id="OrderID"></label>
                        <span style="margin-left: 10px">提货日期：<label id="DeliveryTime"></label></span>
                        <%--<span style="margin-left: 10px">装箱日期：<label id="PakingDate"></label></span>--%>
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
                            <th style="width: 8%">库位号</th>
                            <th style="width: 8%">箱号</th>
                            <th style="width: 25%">品名</th>
                            <th style="width: 22%">型号</th>
                            <th style="width: 12%">品牌</th>
                            <th style="width: 8%">数量</th>
                            <th style="width: 12%">装箱日期</th>
                        </tr>
                    </table>
                    <%--客户评价--%>
                    <table id="table_info">
                        <tbody>
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
                    <div class="print-row" style="line-height: 20px; font-size: 14px; float: right; margin-right: 120px; margin-top: 15px">
                        <ul style="list-style: none">
                            <li>提货人签字/签章:</li>
                            <li>提货日期:  _____年 ____月 ____ 日</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
