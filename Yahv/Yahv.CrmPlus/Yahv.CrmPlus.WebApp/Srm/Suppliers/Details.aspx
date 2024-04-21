<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Suppliers.Details" %>

<%@ Import Namespace="Yahv.CrmPlus.Service" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.Supplier supplier = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.Supplier;
        string LogoUrl = this.Model.LogoUrl;
        IEnumerable<Yahv.CrmPlus.Service.Models.Origins.FilesDescription> licenses = this.Model.Licenses as
            IEnumerable<Yahv.CrmPlus.Service.Models.Origins.FilesDescription>;
    %>


    <div>
        <table class="liebiao">
            <tr>
                <td colspan="6" class="csrmtitle">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td colspan="2" rowspan="4">
                    <img src="<%=LogoUrl %>" style="width: 190px; height: 110px" /></td>
                <td class="tdtitle">供应商名称</td>
                <td><%=supplier.Name %></td>
                <td class="tdtitle">是否国际</td>
                <td><%=supplier.EnterpriseRegister.IsInternational?"是":"否" %></td>
            </tr>
            <tr>
                <td class="tdtitle">国别</td>
                <td><%=supplier.DistrictDes %></td>
                <td class="tdtitle">企业性质</td>
                <td><%=supplier.EnterpriseRegister.Nature %></td>
            </tr>
            <tr>
                <td class="tdtitle">供应商性质</td>
                <td><%=supplier.Type.GetDescription() %></td>
                <td class="tdtitle">所属行业</td>
                <td><%=supplier.EnterpriseRegister.Industry %></td>
            </tr>
            <tr>
                <td class="tdtitle">供应商等级</td>
                <td><span class="level<%=supplier.SupplierGrade %>"></span></td>
                <td class="tdtitle">开票类型</td>
                <td><%=supplier.InvoiceType.GetDescription() %></td>
            </tr>
            <tr>
                <td class="tdtitle">是否返款</td>
                <td>否</td>
                <td class="tdtitle">状态</td>
                <td><%=supplier.Status.GetDescription() %></td>
                <td class="tdtitle">下单方式</td>
                <td><%=supplier.OrderType.GetDescription() %></td>
            </tr>
            <tr>
                <td class="tdtitle">是否特色</td>
                <td><%=supplier.IsSpecial?"是":"否"%></td>
                <td class="tdtitle">网址</td>
                <td><%=supplier.EnterpriseRegister.WebSite %></td>
                <td class="tdtitle">证照</td>
                <td>
                    <ul>
                        <%
                            if (licenses.Count() > 0)
                            {
                                foreach (var item in licenses)
                                {
                        %>
                        <li><a href="<%=item.Url %>" target='_blank'><%=item.CustomName %></a>
                        </li>
                        <%
                                }
                            }
                        %>
                    </ul>

                </td>
            </tr>
            <tr>
                <td class="tdtitle">是否同为客户</td>
                <td><%=supplier.IsClient?"是":"否"%></td>
                <td class="tdtitle">是否有账期</td>
                <td><%=supplier.IsAccount?"是":"否" %></td>
                <td class="tdtitle">上班时间</td>
                <td><%=supplier.WorkTime %></td>
            </tr>
            <tr>
                <td class="tdtitle">是否保护</td>
                <td><%=supplier.IsProtected?"是":"否"%>
                    <%if (supplier.IsProtected)
                        { %>
                    (<%=supplier.OwnerAdmin?.RealName %>)
                    <%} %>
                </td>
                <td class="tdtitle">是否取得代理权</td>
                <td><%=supplier.IsAgent?"是":"否" %></td>
                <td>指定下单公司</td>
                <td><%=supplier.OrderCompany?.Name %></td>
            </tr>
        </table>


        <table class="liebiao">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>工商信息</p>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">统一社会信用代码</td>
                <td><%=supplier.EnterpriseRegister.Uscc %></td>
                <td class="tdtitle">法人代表</td>
                <td><%=supplier.EnterpriseRegister.Corperation %></td>
            </tr>
            <tr>
                <td class="tdtitle">公司成立日期</td>
                <td colspan="3"><%=supplier.EnterpriseRegister.RegistDate?.ToString("yyyy-MM-dd") %></td>
            </tr>
            <tr>
                <td class="tdtitle">注册币种</td>
                <td><%=supplier.EnterpriseRegister.RegistCurrency.GetDescription() %></td>
                <td class="tdtitle">注册资金</td>
                <td><%=supplier.EnterpriseRegister.RegistFund %></td>
            </tr>
            <tr>
                <td class="tdtitle">注册地址</td>
                <td colspan="3"><%=supplier.EnterpriseRegister.RegAddress %></td>
            </tr>
            <tr>
                <td class="tdtitle">员工人数</td>
                <td><%=supplier.EnterpriseRegister.Employees %></td>
                <td class="tdtitle">经营状态</td>
                <td><%=supplier.EnterpriseRegister.BusinessState %></td>
            </tr>
        </table>
    </div>
</asp:Content>
