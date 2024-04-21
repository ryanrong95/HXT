<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.BlackLists.Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model.Licenses)) {
                var msgr = $("#licenseSuccess");
                var ul = $("<ul></ul>");
                for (var index = 0; index < model.Licenses.length; index++) {
                    var item = model.Licenses[index];
                    var li = $("<li><a href='" + item.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + item.FileName + "</em> </a></li>");
                    ul.append(li);
                }
                msgr.html(ul);
            };

            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#txtName").text(model.Entity.Name);
                $("#area").text(model.Entity.District);
                $("#Source").text(model.Entity.Source);
                $("#logo").attr("src", model.LogoFile);
                if (model.Entity.ClientTypeValue == '<%=Yahv.Underly.CrmPlus.ClientType.University.GetHashCode()%>') {

                    $(".domestic").hide();
                    $(".IsInternation").hide();
                    $(".university").show();


                }
                else {
                    if (!model.Entity.IsInternational) {
                        $(".IsInternation").hide();
                        $(".Unicersity").hide();
                        $(".domestic").show();
                    }
                    else {

                        $(".IsInternation").show();
                        $(".Unicersity").hide();
                        $(".domestic").hide();
                    }
                }

                $('#IsInternational').text(model.Entity.IsInternationDes);
                $(".Place").text(model.Entity.Place);
                $(".Currency").text(model.Entity.Currency);
                $(".adderss").text(model.Entity.RegAddress);
                $("#clientType").text(model.Entity.ClientType);
                $("#nature").text(model.Entity.Nature);
                $("#industry").text(model.Entity.Industry);
                $("#Product").text(model.Entity.product);
                $("#Grade").text(model.Entity.ClientGrade);
                $("#Vip").text(model.Entity.Vip);
                $("#IsMajor").text(model.Entity.IsMajor);
                $("#Status").text(model.Entity.Status);
                $("#IsSpecial").text(model.Entity.IsSpecial);
                $("#ProfitRate").text(model.Entity.ProfitRate);
                $("#Owner").text(model.Entity.Owner);
                if (model.Entity.Protector) {

                    $("#IsProtect").text("是");
                    $("#btnProtect").hide();
                    $("#btnCancelProtect").show();
                } else {
                    $("#IsProtect").text("否");
                    $("#btnProtect").show();
                    $("#btnCancelProtect").hide();
                }
                if (model.Entity.EnterPriseStatus == '<%=Yahv.Underly.AuditStatus.Black.GetHashCode()%>') {
                    $("#btnBlack").hide();
                }
                else {
                    $("#btnBlack").show();
                }
                $("#IsSuplier").text(model.Entity.IsSuplier);
                $(".Uscc").text(model.Entity.Uscc);
                $("#Corperation").text(model.Entity.Corperation);
                $("#RegistDate").text(model.Entity.RegistDate);
                $("#Employees").text(model.Entity.Employees);
                $("#RegistCurrency").text(model.Entity.RegistCurrency)
                $("#RegistFund").text(model.Entity.RegistFund);
                $("#RegAddress").text(model.Entity.RegAddress);
                $("#Nature").text(model.Entity.Nature);
                $("#BusinessState").text(model.Entity.BusinessState);
                $("#Website").text(model.Entity.WebSite);

            }
        })
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao">

            <tr>
                <td colspan="6" class="csrmtitle">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td colspan="2" rowspan="4">
                    <img src="" id="logo" style="width: 150px; height: 110px" />
                </td>
                <td class="csrmtitle">客户名称：</td>
                <td>
                    <label id="txtName"></label>
                </td>
                <td class="csrmtitle">是否国际</td>
                <td>
                    <label id="IsInternational"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle">国别地区：</td>
                <td>
                    <label id="area"></label>
                </td>
                <td class="csrmtitle">客户来源</td>
                <td>
                    <label id="Source"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle">客户类型</td>
                <td>
                    <label id="clientType"></label>
                </td>
                <td class="csrmtitle">所属行业 </td>
                <td>
                    <label id="industry"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle">主要产品</td>
                <td colspan="3">
                    <label id="Product"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle" style="width:50px">客户等级</td>
                <td style="width:52px">
                    <label id="Grade"></label>
                </td>
                <td class="csrmtitle">Vip等级 </td>
                <td>
                    <label id="Vip"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle">是否重点</td>
                <td>
                    <label id="IsMajor"></label>
                </td>
                <td class="csrmtitle">客户状态</td>
                <td>
                    <label id="Status"></label>
                </td>
                <td class="csrmtitle">证照</td>
                <td>
                    <label id="licenseSuccess"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle">是否特殊</td>
                <td>
                    <label id="IsSpecial"></label>
                </td>
                <td class="csrmtitle">网址</td>
                <td>
                    <label id="Website"></label>
                </td>
                <td class="csrmtitle">核定净毛利率</td>
                <td>
                    <label id="ProfitRate"></label>
                </td>
            </tr>
            <tr>
                <td class="csrmtitle">是否保护</td>
                <td>
                    <label id="IsProtect"></label>
                </td>
                <td class="csrmtitle">保护人</td>
                <td>
                    <label id="Owner"></label>
                </td>
                <td class="csrmtitle">是否同为供应商</td>
                <td>
                    <label id="IsSuplier"></label>
                </td>
            </tr>
        </table>
        <table class="liebiao" id="BusinessInfo">
            <tr>
                <td colspan="6" class="csrmtitle">
                    <p>工商信息</p>
                </td>
            </tr>
            <tr class="domestic">
                <td class="csrmtitle">统一社会信用编码</td>
                <td>
                    <label class="Uscc"></label>
                </td>
                <td class="csrmtitle">法人代表</td>
                <td>
                    <label id="Corperation"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="csrmtitle">公司成立日期</td>
                <td>
                    <label id="RegistDate"></label>
                </td>
                <td class="csrmtitle">员工人数</td>
                <td>
                    <label id="Employees"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="csrmtitle">注册币种</td>
                <td>
                    <label id="RegistCurrency"></label>
                </td>
                <td class="csrmtitle">注册资金（万）</td>
                <td>
                    <label id="RegistFund"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="csrmtitle">注册地址</td>
                <td colspan="3">
                    <label id="RegAddress"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="csrmtitle">公司类型</td>
                <td>
                    <label id="Nature"></label>
                </td>
                <td class="csrmtitle">经营状态</td>
                <td>
                    <label id="BusinessState"></label>
                </td>
            </tr>

            <tr class="IsInternation">
                <td  colspan="2" class="csrmtitle">国家地区</td>
                <td>
                    <label class="Place"></label>
                </td>
                <td class="csrmtitle">币种</td>
                <td>
                    <label class="Currency"></label>
                </td>
            </tr>
            <tr class="IsInternation">
                <td class="csrmtitle" colspan="2">详细地址</td>
                <td colspan="4">
                    <label class="adderss"></label>
            </tr>
            <tr class="Unicersity">
                <td class="csrmtitle">统一社会信用编码</td>
                <td colspan="4">
                    <label class="Uscc"></label>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
