<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Edit" %>

<%@ Import Namespace="Yahv.CrmPlus.Service" %>
<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .tdtitle {
            background-color: #F9FAFC;
            width: 150px;
        }
    </style>
    <script>
        $(function () {
            $("#btnEdit").click(function () {
                $.myDialog({
                    title: '编辑',
                    url: '../Suppliers/Edit.aspx?enterpriseid=' + model.Entity.ID,
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        location.reload();
                    }
                });
            })
            $("#btnProtect").click(function () {
                $.messager.confirm('确认', '确定要对该供应商申请保护吗？', function (r) {
                    if (r) {
                        $.post('?action=Protect', { ID: model.Entity.ID }, function (result) {
                            if (result.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "申请成功,等待审批!",
                                    type: "success"
                                });
                            }
                            else {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: result.message,
                                    type: "error"
                                });
                            }
                        });

                    }
                });
            })
            $("#btnBlack").click(function () {
                $.myDialogFuse({
                    title: '加入黑名单',
                    url: 'JoinBlack.aspx?id=' + model.Entity.ID,
                    width: '500px',
                    height: '200px',
                    onClose: function () {
                        window.location.reload();
                    }
                });
            })
            $("#btnCancelProtect").click(function () {
                $.messager.confirm('确认', '确定要撤销当前保护人吗？', function (r) {
                    if (r) {
                        $.post('?action=CancelProtect', { ID: model.Entity.ID }, function (success) {
                            if (success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "已撤销",
                                    type: "success"
                                });
                                window.location.reload()
                            }
                            else {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "撤销失败",
                                    type: "error"
                                });
                            }
                        });

                    }
                });
            })

            $("#btnLogs").click(function () {
                $.myWindow({
                    title: "我的保护申请记录",
                    url: "../ApprovalRecords/MyProtecteds.aspx?supplierid=" + model.Entity.ID,
                    width: '800px',
                    height: '600px'
                });
            })
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.Supplier supplier = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.Supplier;
        string LogoUrl = this.Model.LogoUrl;
    %>
    <div class="easyui-panel" data-options="fit:true">

        <table class="liebiao">
            <tr>
                <td colspan="5">
                    <div style="padding-bottom: 5px">
                        <%
                            bool isBlack = supplier.Status == AuditStatus.Black;
                            string btnEdit = isBlack ? "style=\"display:none\"" : "";
                            string btnCancelProtect = supplier.IsProtected ? "" : "style=\"display:none\"";
                            string btnProtect = isBlack || supplier.IsProtected ? "style=\"display:none\"" : "";
                            string btnBlack = isBlack ? "style=\"display:none\"" : "";
                        %>
                        <a id="btnEdit" particle="Name:'编辑',jField:'btnEdit'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" <%=btnEdit %>>编辑资料</a>

                        <a id="btnCancelProtect" particle="Name:'撤销保护',jField:'btnCancelProtect'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-returned'" <%=btnCancelProtect %>>撤销保护</a>

                        <a id="btnProtect" particle="Name:'申请保护',jField:'btnProtect'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-claim'" <%=btnProtect %>>申请保护</a>

                        <a id="btnBlack" particle="Name:'加入黑名单',jField:'btnBlack'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-blacklist'" <%=btnBlack %>>加入黑名单</a>


                        <span style="font-size: 14px" <%=isBlack?"":"hidden='hidden'" %>>黑名单原因:<%=supplier.Summary %></span>


                    </div>
                </td>
                <td>
                    <%-- <a id="btnLogs" href="#" data-options="iconCls:'icon-yg-details'" onclick="Logs('<%=supplier.ID %>')">保护申请记录</a>--%>
                    <a id="btnLogs" class="easyui-linkbutton" data-options="iconCls:'icon-yg-details'">保护申请记录</a>
                </td>
            </tr>
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
                <td><span class="level<%=supplier.Grade %>"></span></td>
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
                <td colspan="2"><%=supplier.EnterpriseRegister.WebSite %></td>

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
                    (<%=supplier.OwnerAdmin.RealName %>)
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
        <uc1:PcFiles runat="server" id="PcFiles" IsPc="false" />

    </div>
</asp:Content>
