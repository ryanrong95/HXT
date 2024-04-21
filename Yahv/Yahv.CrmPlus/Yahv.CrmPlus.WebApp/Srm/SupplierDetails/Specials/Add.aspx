<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Specials.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $('#File').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png'.split(','),
                progressbarTarget: '#fileMessge',
                successTarget: '#fileSuccess',
                multiple: false
            });
            $("#cbbType").fixedCombobx({
                required: true,
                type: "nBrandType",
            })
            $("#cbbPn").standardPartNumberCrmPlus({
                required: true
            })
          
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>型号:</td>
                <td>
                    <input id="cbbPn" name="Pn" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>
           <%-- <tr>
                <td style="width: 120px;">品牌：</td>
                <td>
                    <input id="cbbBrand" name="Brand" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>--%>
            <tr>
                <td>特色类型:</td>
                <td>
                    <input id="cbbType" name="Type" class="easyui-combobox" style="width: 350px;" /></td>
            </tr>

            <tr>
                <td>备注:</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,300]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>
            <tr>
                <td>附件:</td>
                <td>
                    <div>
                        <a id="File">上传</a>
                        <div id="fileMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileSuccess"></div>
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
