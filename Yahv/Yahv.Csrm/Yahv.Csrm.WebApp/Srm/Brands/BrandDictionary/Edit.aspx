<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Brands.BrandDictionary.Edit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td style="width: 100px">名称</td>
            <td>
                <input id="txtName" name="Name" style="width: 200px;" class="easyui-textbox"
                    data-options="prompt:'名称',required:true,validType:'length[1,50]'">
            </td>

        </tr>
        <tr>
            <td style="width: 100px">简称</td>
            <td>
                <input id="txtShortName" name="ShortName" style="width: 200px;" class="easyui-textbox"
                    data-options="prompt:'名称',required:true,validType:'length[1,50]'">
            </td>
        </tr>

    </table>

    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
    </div>



</asp:Content>
