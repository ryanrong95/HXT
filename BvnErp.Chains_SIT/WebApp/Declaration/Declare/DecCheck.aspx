<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecCheck.aspx.cs" Inherits="WebApp.Declaration.Declare.DecCheck" %>

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
        var oInterval = "";
        var ModelCount = 0;
        var IsReturnd = "<%=this.Model.IsReturnd%>";
        var ContractUrl = "<%=this.Model.ContractUrl%>";
        var InvoiceUrl = "<%=this.Model.InvoiceUrl%>";
        var PackingListUrl = "<%=this.Model.PackingListUrl%>";
        var VoyageFileUrl = "<%=this.Model.VoyageFileUrl%>";
        var OrderID = "<%=this.Model.OrderID%>";
        var CCCWai = eval('(<%=this.Model.CCCWai%>)');
        var CertFileUrl = "";

        var Form = getQueryString("Form");
        $(function () {
            var DeclarationID = getQueryString("ID");
            if (Form == "Maker") {
                $("#Refuse").css('display', 'none');
            } else if (Form == "Maked") {
                $("#Refuse").css('display', 'none');
                $("#Submit").css('display', 'none');
                $("#CheckInfo").css('display', 'none');
            }

            $("#DeclarationID").val(DeclarationID);

            if (ContractUrl == null || ContractUrl == "") {
                $("#fileWarning").css('display', 'block');
                $("#Contract").css('display', 'none');
                $("#Invoice").css('display', 'none');
                $("#PackingList").css('display', 'none');
            }

            MaskUtil.mask();//遮挡层           
            $.post('?action=Print', { ID: DeclarationID }, function (data) {
                var result = JSON.parse(data);
                if (result.result) {
                    var CheckDocument = JSON.parse(result.info);
                    var htmlcontext = "";
                    htmlcontext += "<div id=\"printContainer\" style=\"margin:10px\">";
                    htmlcontext += "<table style=\"border:0.5px solid black;display:inline-block;\" cellspacing=\"0\">";

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
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CustomMaster + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CustomMasterName + "</td>";
                    htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">合同协议号</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.ContrNo + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">进境关别</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.IEPort + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.IEPortName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">备案号</td>";
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.ManualNo + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">进口日期</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.IEDate + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">申报日期</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.DDate + "</td>";
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
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TrafMode + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TrafModeName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">运输工具</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TrafName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">航次号</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.VoyNo + "</td>";
                    htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">提运单号</td>";
                    htmlcontext += "<td colspan=\"2\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.BillNo + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">监管方式</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TradeMode + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TradeModeName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">征免性质</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CutMode + "</td>";
                    htmlcontext += "<td colspan=\"4\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.CutModeName + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">许可证号</td>";
                    htmlcontext += "<td colspan=\"2\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.LicenseNo + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">启运国</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TradeCountry + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TradeCountryName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">经停港</td>"; 
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.DistinatePort + "</td>";
                    htmlcontext += "<td colspan=\"4\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.DistinatePortName + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">成交方式</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TransMode + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TransModeName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"></td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">启运港</td>"; 
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.DespPortCode + "</td>";
                    htmlcontext += "<td colspan=\"4\"  style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.DespPortCodeName + "</td>";
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
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.WrapType + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.WrapTypeName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">毛重</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.GrossWt + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">净重</td>";
                    htmlcontext += "<td colspan=\"2\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.NetWt + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">贸易国别(地区)</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TradeAreaCode + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.TradeAreaCodeName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">入境口岸</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.EntyPortCode + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.EntyPortCodeName + "</td>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">货物存放地点</td>"; 
                    htmlcontext += "<td colspan=\"5\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.GoodsPlace + "</td>";
                    htmlcontext += "</tr>";

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">报关单类型</td>"; 
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.EntryType + "</td>";
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

                    //htmlcontext += "<tr>";
                    //htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">备注</td>"; 
                    //htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.NoteS + "</td>";
                    //htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">(0字节)</td>";
                    //htmlcontext += "</tr>";

                    //htmlcontext += "<tr>";
                    //htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">标记唛码</td>";
                    //htmlcontext += "<td colspan=\"9\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.MarkNo + "</td>";
                    //htmlcontext += "<td colspan=\"3\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">(3字节)</td>";
                    //htmlcontext += "</tr>";

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

                    htmlcontext += "<tr>";
                    htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">标记唛码及备注</td>";
                    htmlcontext += "<td colspan=\"11\" style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\"> " + CheckDocument.MarkNo;
                    if (CheckDocument.Container.length > 0) {
                        htmlcontext += "     集装箱标箱数及号码：" + CheckDocument.Container;
                    } 
                    htmlcontext += "</td></tr>";

                    htmlcontext += "</table>";
                    //
                    htmlcontext += "<table style='display:inline-block;padding-left: 30px;'>";
                    if (CCCWai.length > 0) {
                        htmlcontext += "<tr><td>3C目录外鉴定结果单:</td></tr>";
                    }
                    for (var i = 0; i < CCCWai.length; i++) {
                        htmlcontext += "<tr><td><a href='" + CCCWai[i].Url + "' target='_blank'>" + CCCWai[i].FileName +"</a></td></tr>";
                    }
                    htmlcontext += "</table>";

                    htmlcontext += "</div>";

                    MaskUtil.unmask();//关闭遮挡层
                    $('#heads').append(htmlcontext);
                }
            });

            MaskUtil.mask();//遮挡层    
            $.post('?action=ListPrint', { ID: DeclarationID }, function (data) {
                var result = JSON.parse(data);
                if (result.result) {
                    var CheckDocument = JSON.parse(result.info);
                    var htmlcontext = "";
                    htmlcontext += "<div id=\"ListprintContainer\" style=\"margin:10px\">";
                    //htmlcontext += "<label>日期:" + CheckDocument.CheckDate.substring(0, 10) + "</label>";                   
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
                    //htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:2%\">最终目的国</th>";
                    //htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:3%\">境内目的地</th>";
                    //htmlcontext += "<th style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;width:3%\">征免</th>";
                    htmlcontext += "</tr>";

                    ModelCount = CheckDocument.lists.length;
                    for (var i = 0; i < CheckDocument.lists.length; i++) {
                        htmlcontext += "<tr>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GNo + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].CodeTS + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GName + "</td>";
                        //功能与许可证
                        var FunAndLimit = CheckDocument.lists[i].GModel;
                        if (CheckDocument.lists[i].Limits.length > 0) {
                            for (var j = 0; j < CheckDocument.lists[i].Limits.length; j++) {
                                FunAndLimit += "<br/><lable style='color:red;'>许可证信息：【" + CheckDocument.lists[i].Limits[j].BaseGoodsLimit.Code + "-" + CheckDocument.lists[i].Limits[j].BaseGoodsLimit.Name
                                    + "】  编号：" + "  <a href='" + CheckDocument.lists[i].Limits[j].FileUrl + "' target='_blank'>" + CheckDocument.lists[i].Limits[j].LicenceNo +"</a></lable>";
                            }
                            
                        }
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + FunAndLimit + "</td>";
                        if (CheckDocument.lists[i].GModel.toLowerCase().indexOf(CheckDocument.lists[i].GoodsBrand.toLowerCase()) < 0) {
                            htmlcontext += "<td style=\"background:red;border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GoodsBrand + "</td>";
                        } else {
                            htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GoodsBrand + "</td>";
                        }
                        if (CheckDocument.lists[i].GModel.toLowerCase().indexOf(CheckDocument.lists[i].GoodsModel.toLowerCase()) < 0) {
                            htmlcontext += "<td style=\"background:red;border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GoodsModel + "</td>";
                        } else {
                            htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GoodsModel + "</td>";
                        }
                        if (CheckDocument.lists[i].IsHOrigin == true) {
                            htmlcontext += "<td style=\"background:red;border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].OriginCountryName + "</td>";

                        } else {
                            htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].OriginCountryName + "</td>";
                        }
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].GQty + "</td><td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">";
                        htmlcontext += CheckDocument.lists[i].NetWt == "" ? "" : CheckDocument.lists[i].NetWt + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].DeclPrice + "</td>";
                        htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">" + CheckDocument.lists[i].DeclTotal + "</td>";
                        //htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">中国</td>";
                        //htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">深圳特区/深圳市龙岗区</td>";
                        //htmlcontext += "<td style=\"border-bottom:0.5px solid black;border-right:0.5px solid black;font-size: 8px !important;\">照章征税</td>";
                        htmlcontext += "</tr>";
                    }
                    htmlcontext += "</table>";

                    //htmlcontext += "<label>总件数:" + CheckDocument.PackNo + "&nbsp;CTNS</label>";
                    //htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总数量:" + CheckDocument.TotalQty + "&nbsp;PCS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总件数:" + CheckDocument.PackNo + "&nbsp;</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总净重:" + CheckDocument.NetWt + "&nbsp;KGS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总毛重:" + CheckDocument.GrossWt + "&nbsp;KGS</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>总金额:" + CheckDocument.DeclarePrice + "&nbsp;" + CheckDocument.Currency + "</label>";
                    htmlcontext += "<br/>";
                    htmlcontext += "<label>合同号:" + CheckDocument.ContrNo + "</label>";
                    htmlcontext += "&nbsp;&nbsp;&nbsp;&nbsp;";
                    htmlcontext += "<label>客户:" + CheckDocument.OwnerName + "</label>";                    
                    if (CheckDocument.LicenseDocus.length > 0 && CheckDocument.LicenseDocus[0].DocuCode == '0') {
                        //DocuCode
                        //htmlcontext += "<br/>";
                        //htmlcontext += "<label style=\"color:red;\">反制措施排除代码:" + CheckDocument.LicenseDocus[0].CertCode + "</label>";
                        CertFileUrl = CheckDocument.LicenseDocus[0].FileUrl;
                        $('#CertShow').show();
                        $('#certcode').append(CheckDocument.LicenseDocus[0].CertCode).show();
                    }
                    htmlcontext += "</div>";

                    MaskUtil.unmask();//关闭遮挡层
                    $('#lists').append(htmlcontext);
                    document.getElementById('openTime').innerHTML = CheckDocument.CheckDate.substring(0, 19).replace('T', ' ');
                }
            });

            oInterval = setInterval(CountDown, 1000);
        });
    </script>

    <script>
        function CountDown() {
            var count = document.getElementById('checkTime').innerHTML;
            count++;
            document.getElementById('checkTime').innerHTML = count;
        }

        function Submit() {
            //验证复核时间
            var ss = document.getElementById('checkTime').innerHTML;
            var needss = ModelCount * 1 + 5;
            if ((ss < needss) && IsReturnd != "True") {
                $.messager.alert('info', '核对时间太短，请仔细核对！');
                return;
            }

            var DeclarationID = $("#DeclarationID").val();
            var StartTime = document.getElementById('openTime').innerHTML;
            $("#approve-tip").show();
            $("#refuse-tip").hide();
            $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#AdditionSummary").textbox('getValue');
                        reason = reason.trim();
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Submit', {
                            DeclarationID: DeclarationID,
                            Reason: reason,
                            StartTime: StartTime,
                            Form: Form
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        NormalClose();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        function Refuse() {
            var DeclarationID = $("#DeclarationID").val();
            var StartTime = document.getElementById('openTime').innerHTML;
            $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;
            $("#approve-tip").hide();
            $("#refuse-tip").show();
            $("#cancel-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#ApproveSummary").textbox('getValue');
                        reason = reason.trim();
                        if (reason == "") {
                            $.messager.alert("提示", "拒绝原因不能为空！");
                            return;
                        }
                        $("#ApproveSummary").textbox('setValue', reason);
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Refuse', {
                            DeclarationID: DeclarationID,
                            Reason: reason,
                            StartTime: StartTime,
                            Form: Form
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        NormalClose();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
        }

        function Files(fileUrl) {
            top.$.myWindow({
                iconCls: "",
                url: fileUrl,
                noheader: false,
                title: '查看电子单据',
                width: '1024px',
                height: '600px'
            });
        }

        function Contract() {
            Files(ContractUrl);
        }

        function Invoice() {
            Files(InvoiceUrl);
        }

        function PackingList() {
            Files(PackingListUrl);
        }

        function VoyageShow() {
            Files(VoyageFileUrl);
        }

        function ClassifySow() {
            var url = location.pathname.replace(/DecCheck.aspx/ig, 'DecCheckClassify.aspx?OrderID=' + OrderID );
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '产品归类结果',
                width: '1200px',
                height: '500px'
            });
        }

        function CertShow() {
            Files(CertFileUrl);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="buttons">
            <table id="CheckInfo">
                <tr>
                    <td>打开时间：</td>
                    <td>
                        <label id="openTime"></label>
                    </td>
                    <td style="color: red">&nbsp 核对时间：</td>
                    <td>
                        <label id="checkTime" style="color: red">0</label>
                        <label style="color: red">S</label>
                        <input type="hidden" id="DeclarationID" />
                    </td>
                    <td>
                        <span id="fileWarning" style="color: red; display: none">&nbsp&nbsp&nbsp 随附单据还未生成 </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="btnCheck">
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Contract()" data-options="iconCls:'icon-edit'" id="Contract">合同</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Invoice()" data-options="iconCls:'icon-edit'" id="Invoice">发票</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="PackingList()" data-options="iconCls:'icon-edit'" id="PackingList">箱单</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="VoyageShow()" data-options="iconCls:'icon-edit'" id="VoyageShow">六联单</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="ClassifySow()" data-options="iconCls:'icon-edit'" id="ClassifySow">归类信息</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="CertShow()" data-options="iconCls:'icon-edit'" id="CertShow" style="display:none">反制措施排除代码</a>
            <label id="certcode" style="color:red;display:none;padding-left:20px"></label>
        </div>
        <div id="heads">
        </div>
        <div id="lists">
        </div>
        <div id="btn-area" class="view-location" style="width: 650px; height: 30px; float: left; margin: 5px">
            <span id="btn-submit">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-save'" id="Submit">复核通过</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'" id="Refuse">拒绝</a>
            </span>
        </div>
        <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
            <form id="form3">
                <div id="approve-tip" style="padding: 15px; display: none;">
                    <div>
                        <label>备注：</label>
                    </div>
                    <div style="margin-top: 3px;">
                        <input id="AdditionSummary" class="easyui-textbox" data-options="multiline:true," style="width: 300px; height: 62px;" />
                    </div>
                    <label style="font-size: 14px;">确定复核通过吗？</label>
                </div>

                <div id="refuse-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                    <div>
                        <label>拒绝原因：</label>
                    </div>
                    <div style="margin-top: 3px;">
                        <input id="ApproveSummary" class="easyui-textbox" data-options="required:true, multiline:true," style="width: 300px; height: 62px;" />
                    </div>
                </div>
            </form>
        </div>
    </form>
</body>
</html>
