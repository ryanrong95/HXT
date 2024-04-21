<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Specials.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $('#file').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#fileMessge',
                successTarget: '#fileSuccess',
                multiple: false
            });
            $("#SpecialType").combobox({
                data: model.SpecialType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
            });
        })
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao">
            <tr>
                <td>特殊类型：
                </td>
                <td>
                    <input id="SpecialType" name="SpecialType" class="easyui-combobox" style="width: 350px;" data-options="required:true"></td>
            </tr>

            <tr>
                <td>说明：</td>
                <td colspan="2">
                    <input class="easyui-textbox" id="Content" name="Content"
                        data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 350px; height: 100px" />
            </tr>
            <tr>
                <td>上传附件：</td>
                <td colspan="2">
                    <div>
                        <a id="file">上传</a>
                        <div id="fileMessge" style="display: inline-block; width: 350px;"></div>
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
