<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.HSCodeControl.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label"> 海关编码: </td>
                <td>
                    <input id="hsCode" name="hsCode" class="easyui-textbox" style="width: 200px" required/>
                </td>
            </tr>           
        </table>
        <div style="text-align:center; padding:5px">
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/> 

        </div>
    </div>
</asp:Content>
