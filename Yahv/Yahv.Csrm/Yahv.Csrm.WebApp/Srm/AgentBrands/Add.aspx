<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.AgentBrands.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script>
        $(function () {

            $('#cbo_Admins').combobox({
                data: model.Admin,
                valueField: 'ID',
                textField: 'RealName',
                panelHeight: 'auto', //自适应
                multiple: true,
            });
            //$('#cbo_Supplier').combobox({
            //    data: model.Supplisers,
            //    valueField: 'ID',
            //    textField: 'Name',
            //    panelHeight: 'auto', 
            //    multiple: false,
            //    limitToList: true
            //});
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">

        <table class="liebiao">
            <tr>
                <td style="width: 100px">代理品牌</td>
                <td colspan="3">
                    <input id="txt_standardBrand" class="easyui-crmStandardBrand" name="StandardBrand" data-options="width:350" />
                </td>
            </tr>
            <%--<tr>
                <td style="width: 100px">代理公司</td>
                <td colspan="3">
                    <input id="txt_InternalCompany" class="easyui-InternalCompany" name="InternalCompany" data-options="required:true,width:350" value="" />
                </td>
            </tr>--%>
            
            <%--<tr>
                <td style="width: 100px">负责人</td>
                <td colspan="3">
                    <input id="cbo_Admins" name="Admins" class="easyui-combobox" style="width: 350px;" data-options="width:350">
                </td>
            </tr>--%>

        </table>
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>
