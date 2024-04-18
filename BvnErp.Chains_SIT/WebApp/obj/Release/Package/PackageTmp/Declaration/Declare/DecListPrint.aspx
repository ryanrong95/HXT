<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecListPrint.aspx.cs" Inherits="WebApp.Declaration.Declare.DecListPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <style type="text/css">
        /*.lkprintTb {
            border:0.5px solid black;
            font-family:'Times New Roman', Times, serif;
            font-size:6px !important;
        }
            .lkprintTb td{
                border-bottom:0.5px solid black;
                border-right:0.5px solid black;
            }
            .lkprintTb th{
                border-bottom:0.5px solid black;
                border-right:0.5px solid black;
            }*/

       /*body {
            -webkit-transform: scale(1,0.75,0.75);
            font-size:0.2em;
        }*/
    </style>
    <script> 	
        jQuery.browser = {};
        (function () {
            jQuery.browser.msie = false;
            jQuery.browser.version = 0;
            if (navigator.userAgent.match(/MSIE ([0-9]+)./)) {
                jQuery.browser.msie = true; jQuery.browser.version = RegExp.$1;
            }
        })();
    </script>
    <script>
        $(function () {
            var DeclarationID = getQueryString("ID");
           MaskUtil.mask();//遮挡层
            $.post('?action=Print', { ID: DeclarationID }, function (data) {
                var result = JSON.parse(data);
                if (result.result) {
                    var CheckDocument = JSON.parse(result.info);
                    var htmlcontext = "";
                    htmlcontext += "<div>";
                    htmlcontext += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Print()" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">打印</span>' +
                        '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                    htmlcontext += "</div>"
                    htmlcontext += "<div id=\"printContainer\" style=\"margin:10px\">";
                    htmlcontext += "<label>日期:" + CheckDocument.CheckDate.substring(0, 10) + "</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>合同号:" + CheckDocument.ContrNo + "</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>航次号:" + CheckDocument.VoyNo + "</label>";
                    htmlcontext += "<table style=\"width:100%;border:0.5px solid black;\" cellspacing=\"0\">";
                    htmlcontext += "<tr>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:2%\">序号</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:5%\">商品编码</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:10%\">品名</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:30%\">功能</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:10%\">品牌</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:10%\">规格型号</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:3%\">产地</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:2%\">数量<br/>(PCS)</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:2%\">净重<br/>(KGS)</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:3%\">报关单价<br/>(" + CheckDocument.Currency + ")</th>";
                    htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:3%\">报关总价<br/>(" + CheckDocument.Currency + ")</th>";
                    htmlcontext += "</tr>";

                    for (var i = 0; i < CheckDocument.lists.length; i++) {
                        htmlcontext += "<tr>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GNo + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].CodeTS + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GName + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GModel + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GoodsBrand + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GoodsModel + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].OriginCountryName + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GQty + "</td><td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">";
                        htmlcontext += CheckDocument.lists[i].NetWt == "" ? "" : CheckDocument.lists[i].NetWt + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].DeclPrice + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].DeclTotal + "</td>";
                        htmlcontext += "</tr>";
                    }
                    htmlcontext += "</table>";

                    htmlcontext += "<label>总件数:" + CheckDocument.PackNo + "&nbsp;CTNS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总数量:" + CheckDocument.TotalQty + "&nbsp;PCS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总净重:" + CheckDocument.NetWt + "&nbsp;KGS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总毛重:" + CheckDocument.GrossWt + "&nbsp;KGS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总金额:" + CheckDocument.DeclarePrice + "&nbsp;" + CheckDocument.Currency + "</label>";
                    htmlcontext += "</div>";

                    MaskUtil.unmask();//关闭遮挡层
                    $('body').append(htmlcontext);
                }
            });
        });

        function Print() {
            $("#printContainer").jqprint({
               
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height: 1px;">
        <div>
        </div>
    </form>
</body>
</html>
