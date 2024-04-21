<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Admins.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.StandardBrand.Admins" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#cbbAdmins").combobox({
                required: false,
                data: model.Admins,
                valueField: 'ID',
                textField: 'Text',
                panelHeight: 'auto',
                multiple: false,
                editable: false,
                limitToList: true,
            })
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
   <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao">
            <tr>
                <td>
                   人员：
                </td>
               <td> <input id="cbbAdmins"  name="AdminID" style="width:350px" class="easyui-combobox" data-options="required:true,"/></td>
            </tr>
        </table>

    <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
       </div>
</asp:Content>
