<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.AgentBrands.Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            
            $("#cbbBrand").standardBrandCrmPlus({
                required: true
            })
            //$("#cbbCompany").combobox({
            //    required: true,
            //    data: model.Companies,
            //    valueField: 'ID',
            //    textField: 'Name',
            //    panelHeight: 'auto',
            //    multiple: false,
            //    editable: true,
            //    limitToList: true,
            //})
            //$("#cbbPM").combobox({
            //    required: false,
            //    data: model.PM,
            //    valueField: 'ID',
            //    textField: 'Name',
            //    panelHeight: 'auto',
            //    multiple: true,
            //    editable: true,
            //    limitToList: true,
            //})
            //$("#cbbPMA").combobox({
            //    required: false,
            //    data: model.PMA,
            //    valueField: 'ID',
            //    textField: 'Name',
            //    panelHeight: 'auto',
            //    multiple: true,
            //    editable: true,
            //    limitToList: true,
            //})
            //$("#cbbFAE").combobox({
            //    required: false,
            //    data: model.FAE,
            //    valueField: 'ID',
            //    textField: 'Name',
            //    panelHeight: 'auto',
            //    multiple: true,
            //    editable: true,
            //    limitToList: true,
            //})
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>品牌名称</td>
                <td>
                    <input id="cbbBrand" class="easyui-combobox" name="Brand" style="width: 350px;" />
                </td>
            </tr>
          
            <tr>
                <td>备注:</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                            data-options="multiline:true,validType:'length[1,300]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
