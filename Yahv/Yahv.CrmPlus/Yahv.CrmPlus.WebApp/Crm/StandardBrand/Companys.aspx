<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Companys.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.StandardBrand.Companys" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
   <script>
       $(function () {
           $("#cbbcompany").companyCrmPlus({
             required:true
           });

       })
   </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
     <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao">
            <tr>
                <td>
                   合作公司：
                </td>
               <td> <input id="cbbcompany"  name="Company" style="width:350px"  /></td>
            </tr>
        </table>

    <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
       </div>
</asp:Content>
