<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Credits.Add" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //$("#cbbCompany").companyCrmPlus({
            //    required: true,
            //})
            $("#cbbCurrency").fixedCombobx({
                required: true,
                type: "Currency",
                value: '<%=(int)Currency.CNY%>'
            })

            $("#cbbSubject").combobox({
                required: false,
                data: [{ "text": "货款", },
                    { "text": "代理费" },
                    { "text": "税款" },
                    { "text": "杂费" },
                    { "text": "其他" }],
                prompt: '可输入',
                valueField: 'text',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                editable: true
            })
            $("#cbbCatalog").combobox({
                required: false,
                data: [{ "text": "包装费", "selected": true },
                    { "text": "代付货款" },
                    { "text": "货款" },
                    { "text": "代理费" },
                    { "text": "代收货款" },
                    { "text": "垫付运费" },
                    { "text": "快递费" },
                    { "text": "消费税" },
                    { "text": "销售增值税" },
                    { "text": "自提服务费" }],
                prompt: '可输入',
                valueField: 'text',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                editable: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>我方公司:</td>
                <td>
                   <%=this.Model.CompanyName %>
                </td>
            </tr>
            <tr>
                <td style="width: 120px;">币种：</td>
                <td>
                    <input id="cbbCurrency" name="Currency" style="width: 350px;" />
                </td>
            </tr>
            <tr style="display: none;">
                <td style="width: 120px;">分类：</td>
                <td>
                    <input id="cbbSubject" name="Subject" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>
            <tr style="display: none;">
                <td style="width: 120px;">科目：</td>
                <td>
                    <input id="cbbCatalog" name="Catalog" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>
            <tr>
                <td>金额:</td>
                <td>
                    <input name="Price" id="Price" class="easyui-numberbox"
                        value="0" data-options="min:0,precision:2,required:true" style="width: 350px; text-align: right;">
                </td>
            </tr>

            <tr>
                <td>备注:</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,200]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>
            <%-- <tr>
                <td>附件:</td>
                <td>
                     <div>
                        <a id="File">上传</a>
                        <div id="fileMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileSuccess"></div>
                </td>
            </tr>--%>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
