<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Remark.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ClassifyHistory.Remark" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#partNumber').textbox('setValue', model.Other.PartNumber);
            $('#manufacturer').textbox('setValue', model.Other.Manufacturer);
            $('#summary').textbox('setValue', model.Other.Summary);
        });
    </script>

    <style>
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',border:false" style="text-align: center">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label" style="width: 100px">型号：</td>
                <td style="width: 200px">
                    <input id="partNumber" name="partNumber" class="easyui-textbox" style="width: 150px" data-options="disabled:true" />
                </td>
                <td class="liebiao-label" style="width: 100px">品牌：</td>
                <td style="width: 200px">
                    <input id="manufacturer" name="manufacturer" class="easyui-textbox" style="width: 150px" data-options="disabled:true" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <input id="summary" name="summary" class="easyui-textbox" data-options="multiline:true,required:true,tipPosition:'bottom',validType:'length[1,300]'" style="width: 450px; height: 150px" />
                </td>
            </tr>
        </table>
    </div>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>
