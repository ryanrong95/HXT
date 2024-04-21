<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table  style="padding: 20px 20px 0px 20px; margin-top: 2px;">
            <tr>
                <td>客户名称：</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 250px; height: 35px" data-options="required:true,tipPosition:'bottom', validType:'length[1,50]'" /></td>
            </tr>
        </table>

               <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>

</asp:Content>
