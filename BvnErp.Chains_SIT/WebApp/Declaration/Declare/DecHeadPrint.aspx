<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecHeadPrint.aspx.cs" Inherits="WebApp.Declaration.Declare.DecHeadPrint" %>

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
                    htmlcontext += "<table style=\"border:0.5px solid black;\" cellspacing=\"0\">";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:110px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:60px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:95px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:75px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:90px;font-size: 8px !important;\"></td>";

                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:130px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:85px;font-size: 8px !important;\"></td>";  
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:60px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:70px;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:60px;font-size: 8px !important;\"></td>";

                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:60px;font-size: 8px !important;\"></td>";  
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;width:70px;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";



                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">申报地海关</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CusPort + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CusPortName + "</td>";  
                    htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">合同协议号</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.ContrNo + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">进境关别</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CusPort + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CusPortName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">备案号</td>";
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">进口日期</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">申报日期</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"7\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">境内收发货人</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CustomsCode + "</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.Code + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CiqCode + "</td>";
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CompanyName + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">境外收发货人</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";                   
                    htmlcontext += "<td colspan=\"6\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.VendorCompanyName + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">消费使用单位</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.OwnerCusCode + "</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.OwnerScc + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.OwnerName + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">申报单位</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CustomsCode + "</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.Code + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CiqCode + "</td>";
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CompanyName + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">运输方式</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">4</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">公路运输</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">运输工具</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">航次号</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.VoyNo + "</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">提运单号</td>";
                    htmlcontext += "<td colspan=\"2\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.BillNo + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">监管方式</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">0110</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">一般贸易</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">征免性质</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">101</td>";
                    htmlcontext += "<td colspan=\"4\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">一般征税</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">许可证号</td>";
                    htmlcontext += "<td colspan=\"2\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">启运国</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">HKG</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">香港</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">经停港</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">HKG000</td>";
                    htmlcontext += "<td colspan=\"4\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">中国香港</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">成交方式</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">1</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">CIF</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">启运港</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">HKG000</td>";
                    htmlcontext += "<td colspan=\"4\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">中国香港</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">运费</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"8\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";                           
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">保险费</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"8\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";                           
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">杂费</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"8\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";                           
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">件数</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.PackNo + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">包装</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">22</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">纸质或纤维板制盒/箱</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">毛重</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.GrossWt + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">净重</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.NetWt + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">贸易国别(地区)</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">HKG</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">香港</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">入境口岸</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.InPort + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CusPortName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">货物存放地点</td>";
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">深圳市龙华区龙华街道富康社区天汇大厦C栋212</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">报关单类型</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">M</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">通关无纸化</td>";
                    htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">特殊确认关系</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">价格影响确认</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">支付特许权使用费确认</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">备注</td>";
                    htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">(0字节)</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">标记唛码</td>";
                    htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">N/M</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">(3字节)</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">税单无纸化</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">自主报税</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">自报自缴</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">担保验放</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">否</td>";                   
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">是否需做商检</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.IsInspection + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">详细信息</td>";
                    htmlcontext += "<td colspan=\"8\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";



                    htmlcontext += "</table>";
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
        
    </form>
</body>
</html>
