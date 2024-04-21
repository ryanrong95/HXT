<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Approvals.BusinessRelations.Edit" %>

<%@ Import Namespace="Yahv.CrmPlus.Service" %>
<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Rolls.BusinessRelation entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Rolls.BusinessRelation;
      
    %>
    <div id="aa" class="easyui-panel" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>供应商名称</td>
                <td><%=entity.MainName %></td>
            </tr>
            <tr>
                <td>关联公司</td>
                <td>
                    <%=entity.SubName %></td>
            </tr>
            <tr>
                <td>关联类型</td>
                <td>
                    <%=entity.BusinessRelationType.GetDescription() %></td>
            </tr>
            
        </table>
        <uc1:PcFiles runat="server" id="PcFiles" IsPc="false"/>
        <br />
         <input id="Result" runat="server" type="text" hidden="hidden" />
        <div style="text-align: center; padding: 5px; height: 30px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
            <a onclick="approval(true);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'">通过</a>
            <a onclick="approval(false);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'">否决</a>
        </div>
    </div>
</asp:Content>
