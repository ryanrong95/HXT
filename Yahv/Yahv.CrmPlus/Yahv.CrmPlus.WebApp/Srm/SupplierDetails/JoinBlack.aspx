<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="JoinBlack.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.JoinBlack" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="aa" class="easyui-panel" data-options="fit:true">

        <table class="liebiao">
            <tr>
                <td>原因:</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="required:true,multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 400px; height: 100px" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
