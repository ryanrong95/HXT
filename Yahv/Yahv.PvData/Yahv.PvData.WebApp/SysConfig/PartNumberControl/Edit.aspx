<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.PartNumberControl.Edit" %>

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
                <td class="liebiao-label"> 型号: </td>
                <td>
                    <input id="partNumber" name="partNumber" class="easyui-textbox" style="width: 200px" required/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 品名: </td>
                <td>
                    <input id="name" name="name" class="easyui-textbox" style="width: 200px" required/>
                </td>
            </tr>         
        </table>
        <div style="text-align:center; padding:5px">
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/> 

        </div>
    </div>
</asp:Content>

