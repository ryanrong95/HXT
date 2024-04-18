<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ElementsManufacturer.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.ElementsManufacturer) {
                $('#Manufacturer').textbox('setValue', model.ElementsManufacturer.Manufacturer);
                $('#MfrMapping').textbox('setValue', model.ElementsManufacturer.MfrMapping);

                $('#Manufacturer').textbox('readonly', true);
                $('#Manufacturer').textbox('textbox').css('background', '#EBEBE4');
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label" style="width: 200px"> 归类品牌: </td>
                <td>
                    <input id="Manufacturer" name="Manufacturer" class="easyui-textbox" style="width: 150px" required/>
                </td>
            </tr>   
            <tr>
                <td class="liebiao-label" style="width: 200px"> 申报要素需要的中文或外文品牌: </td>
                <td>
                    <input id="MfrMapping" name="MfrMapping" class="easyui-textbox" style="width: 150px" required/>
                </td>
            </tr>        
        </table>
        <div style="text-align:center; padding:5px">
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/> 

        </div>
    </div>
</asp:Content>
