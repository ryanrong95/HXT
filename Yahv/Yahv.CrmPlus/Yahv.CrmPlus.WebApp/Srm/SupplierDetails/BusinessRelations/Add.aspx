<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BusinessRelations.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            //$("#cbbEnterprises").combobox({
            //    data: model.Suppliers,
            //    valueField: 'ID',
            //    textField: 'Name',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    limitToList: true,
            //    collapsible: true,
            //    required: true
            //})
            $("#cbbEnterprises").supplierCrmPlus({
                required: true,
                exceptitem: model.ID
            })
            $("#cbbType").fixedCombobx({
                required: true,
                type: "BusinessRelationType",
            })

            $('#RelationFile').fileUploader({
                required: true,
                type: 'RelationFiles',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#fileRelationFileMessge',
                successTarget: '#fileRelationFileSuccess',
                multiple: true,
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="aa" class="easyui-panel" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>关联公司</td>
                <td>
                    <select id="cbbEnterprises" name="SubID" class="easyui-combobox" style="width: 200px"></select></td>
            </tr>
            <tr>
                <td>关联类型</td>
                <td>
                    <select id="cbbType" name="Type" class="easyui-combobox" style="width: 200px"></select></td>
            </tr>
            <tr>
                <td>附件</td>
                <td>
                    <div>
                        <a id="RelationFile">上传</a>
                        <div id="fileRelationFileMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileRelationFileSuccess"></div>
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
