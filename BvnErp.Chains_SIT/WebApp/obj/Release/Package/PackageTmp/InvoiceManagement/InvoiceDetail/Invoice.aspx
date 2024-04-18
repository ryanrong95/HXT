<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="WebApp.InvoiceManagement.InvoiceDetail.Invoice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var InvoiceBack = '<%=this.Model.Invoice%>';
        var InvoiceItemsBack = '<%=this.Model.InvoiceItems%>';
        var Invoice = JSON.parse(InvoiceBack);
        var InvoiceItems = JSON.parse(InvoiceItemsBack);

        $(function () {

            var InvoiceContext = "";
            InvoiceContext += "<div id=\"InvoiceHead\" style=\"margin:10px\">";
            InvoiceContext += "<table>";

            InvoiceContext += "<tr>";
            InvoiceContext += "<td colspan=\"5\" style=\"text-align:center;width:1000px\">" + Invoice.invoiceTypeName + "</td>";
            InvoiceContext += "</tr>";

            InvoiceContext += "<tr>";
            InvoiceContext += "<td style=\"font-size: 8px !important;width:200px\">发票代码：" + Invoice.invoiceDataCode + "</td>";
            InvoiceContext += "<td style=\"font-size: 8px !important;width:200px\">发票号码：" + Invoice.invoiceNumber + "</td>";
            InvoiceContext += "<td style=\"font-size: 8px !important;width:200px\">开票日期：" + Invoice.billingTime + "</td>";
            InvoiceContext += "<td style=\"font-size: 8px !important;width:200px\">校验码：" + Invoice.checkCode + "</td>";
            InvoiceContext += "<td style=\"font-size: 8px !important;width:200px\">机器编号：" + Invoice.taxDiskCode + "</td>";
            InvoiceContext += "</tr>";

            InvoiceContext += "</table>";
            InvoiceContext += "</div>";
            $('#InvoiceHead').append(InvoiceContext);

            var PurchaseContext = ""
            PurchaseContext += "<div id=\"InvoiceHead\" style=\"margin:10px\">";
            PurchaseContext += "<table style=\"border:0.5px solid black;\" cellspacing=\"0\">";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:200px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:130px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:40px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:90px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:90px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:100px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:100px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:150px;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\">购买方</td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">名称：" + Invoice.purchaserName + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">密码区</td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">纳税人识别号：" + Invoice.taxpayerNumber + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">地址、电话：" + Invoice.taxpayerAddressOrId + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">开户行及账号：" + Invoice.taxpayerBankAccount + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">货物或应税劳务、服务名称</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">规格型号</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">单位</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">数量</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">单价</td>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">金额</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">税率</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\">税额</td>";
            PurchaseContext += "</tr>";

            if (InvoiceItems.length == 1) {
                var taxnumber = "";
                var taxprice = "";
                if (InvoiceItems[0].number != null) {
                    taxnumber = InvoiceItems[0].number;
                }
                if (InvoiceItems[0].price != null) {
                    taxprice = InvoiceItems[0].price;
                }

                PurchaseContext += "<tr>";
                PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + InvoiceItems[0].goodserviceName + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + InvoiceItems[0].model + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + InvoiceItems[0].unit + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + taxnumber +"</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + taxprice + "</td>";
                PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + InvoiceItems[0].sum + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + InvoiceItems[0].taxRate + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\">" + InvoiceItems[0].tax + "</td>";
                PurchaseContext += "</tr>";                
            } else {
                PurchaseContext += "<tr>";
                PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"><a  href=\"javascript:void(0);\"   onClick=ViewItems()>详见销货清单</a></td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + Invoice.totalAmount + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + InvoiceItems[0].taxRate + "</td>";
                PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\">" + Invoice.totalTaxNum + "</td>";
                PurchaseContext += "</tr>";               
            }


            PurchaseContext += "<tr>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\">&nbsp</td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">合计</td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">¥" + Invoice.totalAmount + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td style=\"border-bottom:0.5px solid black;border-right:0px solid black;font-size: 8px !important;\">¥" + Invoice.totalTaxNum + "</td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">价税合计(大写)</td>";
            PurchaseContext += "<td colspan=\"4\" style=\"border-bottom:0.5px solid black;border-right:0px solid black;font-size: 8px !important;\">" + NoToChinese(Invoice.totalTaxSum) + "</td>";
            PurchaseContext += "<td colspan=\"4\" style=\"border-bottom:0.5px solid black;border-right:0px solid black;font-size: 8px !important;\">(小写)¥" + Invoice.totalTaxSum + "</td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\">销售方</td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">名称：" + Invoice.salesName + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">备注</td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\">" + Invoice.invoiceRemarks + "</td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">纳税人识别号：" + Invoice.salesTaxpayerNum + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">地址、电话：" + Invoice.salesTaxpayerAddress + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "<tr>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;width:50px;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"5\" style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\">开户行及账号：" + Invoice.salesTaxpayerBankAccount + "</td>";
            PurchaseContext += "<td style=\"border-bottom:0px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "<td colspan=\"3\" style=\"border-bottom:0px solid black;border-right:0px solid black;font-size: 8px !important;\"></td>";
            PurchaseContext += "</tr>";

            PurchaseContext += "</table>";
            PurchaseContext += "</div>";
            $('#PurchaserInfo').append(PurchaseContext);
        });

        function ViewItems() {
            var url = location.pathname.replace(/Invoice.aspx/ig, '/InvoiceItems.aspx?ID=' + Invoice.ID);
            debugger
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '发票详情',
                width: '1000px',
                height: '500px',
                onClose: function () {
                    Search();
                }
            });
        }
    </script>
    <script>
        //阿拉伯数字转中文数字
        function NoToChinese(num) {
            if (!/^\d*(\.\d*)?$/.test(num)) {
                alert("Number is wrong!");
                return "Number is wrong!";
            }
            var AA = new Array("零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖");
            var BB = new Array("", "拾", "佰", "仟", "万", "亿", "圆", "");
            var a = ("" + num).replace(/(^0*)/g, "").split("."),
                k = 0,
                re = "";
            for (var i = a[0].length - 1; i >= 0; i--) {
                switch (k) {
                    case 0:
                        re = BB[7] + re;
                        break;
                    case 4:
                        if (!new RegExp("0{4}\\d{" + (a[0].length - i - 1) + "}$").test(a[0]))
                            re = BB[4] + re;
                        break;
                    case 8:
                        re = BB[5] + re;
                        BB[7] = BB[5];
                        k = 0;
                        break;
                }
                if (k % 4 == 2 && a[0].charAt(i + 2) != 0 && a[0].charAt(i + 1) == 0) re = AA[0] + re;
                if (a[0].charAt(i) != 0) re = AA[a[0].charAt(i)] + BB[k % 4] + re;
                k++;
            }
            if (a.length > 1) //加上小数部分(如果有小数部分) 
            {
                re += BB[6];
                for (var i = 0; i < a[1].length; i++) {
                    if (i == 0) {
                        re += AA[a[1].charAt(i)]+"角";
                    }
                    if (i == 1) {
                        re += AA[a[1].charAt(i)]+"分";
                    }                    
                } 
                
            }
            return re;
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="InvoiceHead">
        </div>
        <div id="PurchaserInfo">
        </div>
    </form>
</body>
</html>
