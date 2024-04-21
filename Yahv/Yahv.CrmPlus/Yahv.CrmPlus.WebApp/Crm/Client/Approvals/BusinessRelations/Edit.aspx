<%@ Page MasterPageFile="~/Uc/Works.Master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.BusinessRelations.Edit" %>

<%@ Import Namespace="Yahv.CrmPlus.Service" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        function approval(result) {
            $("#Result").val(result);
            var tips = result ? "确认审批通过？" : "确认审批不通过？";
            $.messager.confirm("操作提示", tips, function (r) {
                if (r) {
                    $('#btnSubmit').click()
                    //$('#form1').submit();
                }
            });

        }

        $(function () {
       
            $("#Name").text(model.Entity.MainName);
            $("#relition").text(model.Entity.SubName);
            $("#relationType").text( model.Entity.BusinessRelationType);

        })

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        //Yahv.CrmPlus.Service.Models.Origins.MapsEnterprise entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.MapsEnterprise;
        IEnumerable<Yahv.CrmPlus.Service.Models.Origins.FilesDescription> files = this.Model.Files as
            IEnumerable<Yahv.CrmPlus.Service.Models.Origins.FilesDescription>;
    %>
    <div id="aa" class="easyui-panel" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>客户名称</td>
                <td>
                    <label id="Name"></label>
                </td>
                <%-- <td id="Name"><%=entity.MainName %></td>--%>
            </tr>
            <tr>
                <td>关联公司</td>
                <td id="relition"></td>
            </tr>
            <tr>
                <td>关联类型</td>
                <td id="relationType"></td>
            </tr>
            <tr>
                <td>附件</td>
                <td>
                    <%
                        if (files.Count() > 0)
                        {
                            foreach (var item in files)
                            {
                    %>
                    <a href="<%=item.Url %>"><%=item.CustomName %></a><br />
                    <%
                        }
                    %>
                    <%
                        }
                    %>
                </td>
            </tr>
        </table>
        <br />
        <input id="Result" runat="server" type="text" hidden="hidden" />
        <div style="text-align: center; padding: 5px; height: 30px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <a onclick="approval(true);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'">通过</a>
            <a onclick="approval(false);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'">否决</a>
        </div>
    </div>
</asp:Content>
