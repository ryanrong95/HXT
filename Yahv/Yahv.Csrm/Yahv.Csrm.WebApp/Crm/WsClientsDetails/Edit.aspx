<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsClientsDetails.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        function Pic(url) {
            window.open(url, '', '', false)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.WsClient client = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.WsClient;
    %>
    <div style="padding: 10px 60px 20px 60px;">

        <table class="liebiao">
            <tr>
                <td style="width: 100px">客户名称</td>
                <td colspan="3"><%=client.Vip?"<span class='vip'></span>":"" %><%=client.Enterprise.Name.Replace("reg-","") %></td>
            </tr>
            <tr>
                <td style="width: 100px">业务</td>
                <td colspan="3"><%=client.ServiceType.GetDescription()%></td>
            </tr>
            <%if (client.ServiceType == ServiceType.Warehouse||client.ServiceType == ServiceType.Both) %>
            <%{ %>
            <tr>
                <td>仓储类型</td>
                <td colspan="3"><%=client.StorageType.GetDescription()%></td>
            </tr>
            <%} %>
            <tr>
                <td style="width: 100px">客户级别</td>
                <td colspan="3"><span class="level<%=(int)client.Grade%>"></span></td>
            </tr>
            <tr>
                <td style="width: 100px">入仓号</td>
                <td colspan="3"><%=client.EnterCode %></td>
            </tr>
            <tr>
                <td>性质</td>
                <td><%=client.Nature.GetDescription() %></td>
            </tr>

            <tr>
                <td style="width: 100px">海关编码</td>
                <td colspan="3"><%=client.CustomsCode %></td>
            </tr>
            <tr>
                <td style="width: 100px">管理员编码</td>
                <td colspan="3"><%=client.Enterprise.AdminCode %></td>
            </tr>
            <tr>
                <td style="width: 100px">国家/地区</td>
                <td colspan="3"><%= Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == client.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == client.Place).GetOrigin()?.ChineseName : null
                %></td>
            </tr>
            <tr>
                <td style="width: 100px">法人</td>
                <td colspan="3"><%=client.Enterprise.Corporation %> </td>
            </tr>
            <tr>
                <td style="width: 100px">统一社会信用代码</td>
                <td colspan="3"><%=client.Enterprise.Uscc %> </td>
            </tr>
            <tr>
                <td style="width: 100px">注册地址</td>
                <td colspan="3"><%=client.Enterprise.RegAddress %> </td>
            </tr>
            <tr>
                <td style="width: 100px">录入人</td>
                <td colspan="3"><%=client.Admin==null?null:client.Admin.RealName %> </td>
            </tr>
             <tr>
                <td style="width: 100px">是否收入仓费</td>
                <td colspan="3"><%=client.ChargeWHType.GetDescription() %> </td>
            </tr>
            <tr>
                <td style="width: 100px">创建时间</td>
                <td colspan="3"><%=client.CreateDate.ToString("yyyy-MM-dd HH:mm:ss") %> </td>
            </tr>
            <%if (client.StorageType == WsIdentity.HongKong) %>
            <%{ %>
            <tr>
                <td>登记证 </td>
                <td colspan="3"><a href="#" onclick="Pic('<%=client.HKBusinessLicense?.Url%>')"><%=client.HKBusinessLicense?.CustomName%></a>
                </td>
            </tr>
            <tr>
                <td>营业执照 </td>
                <td colspan="3"><a href="#" onclick="Pic('<%=client.BusinessLicense?.Url%>')"><%=client.BusinessLicense?.CustomName%></a>
                  
                </td>
            </tr>
            <%}%>
            <%else %>
            <%{ %>
            <tr>
                <td>营业执照 </td>
                <td colspan="3"><a href="#" onclick="Pic('<%=client.BusinessLicense?.Url%>')"><%=client.BusinessLicense?.CustomName%></a>
                  
                </td>
            </tr>
            <%}%>
        </table>
    </div>
</asp:Content>

