<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Commissions.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            $("#cbbType").radio({
                data: model.Type,
                valueField: 'value',
                labelField: 'text'
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tt" class="easyui-panel" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>类型</td>
                <td>
                    <select id="radio_Type" name="Type"></select>
                </td>
            </tr>
            <tr>
                <td>方式</td>
                <td>
                    <select id="cbbMethord" name="Methord" class="easyui-combobox" style="width: 200px"></select></td>
            </tr>
            <tr>
                <td>率值</td>
                <td><input id="File" name="Radio" class="easyui-numnerbox" data-options="min:0,max:1,precision:2" style="width: 200px"></td>
            </tr>
            <tr>
                <td>
                <input id="cbbCurrency" class="easyui-combobox" name="" />

                </td>
            </tr>
        </table>
          <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
