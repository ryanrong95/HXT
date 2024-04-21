<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/_Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.vInvoices.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selType').combobox({
                data: model.InvoiceType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Type);  //全部
                    }
                }
            });
            $("#selDeliveryType").combobox({
                data: model.DeliveryType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.DeliveryType);  //全部
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', {
                    Title: model.Entity.Title,
                    TaxNumber: model.Entity.TaxNumber,
                    RegAddress: model.Entity.RegAddress,
                    Tel: model.Entity.Tel,
                    BankName: model.Entity.BankName,
                    BankAccount: model.Entity.BankAccount,
                    PostAddress: model.Entity.PostAddress,
                    PostRrecipient: model.Entity.PostRecipient,
                    PostTel: model.Entity.PostTel,
                    PostZipCode: model.Entity.PostZipCode,
                });
                if (model.Entity.IsDefault) {
                    $("#IsDefault").checkbox('check');
                }
                if (model.Entity.IsPersonal) {
                    $(".company").hide();
                    $("#txtTaxperNumber").textbox({ required: false })
                }
                else {
                    $(".company").show();
                    $("#txtTaxperNumber").textbox({ required: true })
                }

            }
            $("#ispersonal").radiobutton({
                checked: model.Entity.IsPersonal,
                onChange: function (checked) {
                    if (checked) {
                        $(".company").hide();
                        $("#txtTaxperNumber").textbox({ required: false })
                    }
                    else {
                        $(".company").show();
                        $("#txtTaxperNumber").textbox({ required: true })
                    }
                }
            })
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>抬头类型</td>
                <td>
                    <input class="easyui-radiobutton" name="IsPersonal" value="false" data-options="checked:true"><label for="IsPersonals" style="margin-right: 30px">企业</label>
                    <input id="ispersonal" class="easyui-radiobutton" name="IsPersonal" value="true"><label for="IsPersonal" style="margin-right: 30px">个人/非企业单位</label>
                </td>
            </tr>
            <tr>
                <td style="width: 100px">抬头</td>
                <td>
                    <input id="txtTitle" name="Title" class="easyui-textbox" style="width: 350px;"
                        data-options="prompt:'必填',required:true,validType:'length[1,150]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">发票类型</td>
                <td>
                    <select id="selType" name="Type" class="easyui-combobox" data-options="editable:false" style="width: 350px"></select>
                </td>
            </tr>
            <tr class="company">
                <td style="width: 100px">纳税人识别号</td>
                <td>
                    <input id="txtTaxperNumber" name="TaxNumber" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                </td>
            </tr>

            <tr class="company">
                <td style="width: 100px">注册地址</td>
                <td>
                    <input id="txtRegAddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'">
                </td>
            </tr>
            <tr class="company">
                <td style="width: 100px">企业电话</td>
                <td>
                    <input id="txtTel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">开户行</td>
                <td>
                    <input id="txtBankName" name="BankName" class="easyui-textbox readonly_style" style="width: 350px;"
                        data-options="required:false,validType:'length[1,100]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">账号</td>
                <td>
                    <input id="txtBankAccount" name="BankAccount" class="easyui-textbox" style="width: 350px;"
                        data-options="required:false,validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">收票地址</td>
                <td>
                    <input id="txtPostAddress" name="PostAddress" class="easyui-textbox" style="width: 350px;"
                        data-options="required:false,validType:'length[1,150]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">收票人</td>
                <td>
                    <input id="txtPostRrecipient" name="PostRrecipient" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]',required:false">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">联系电话</td>
                <td>
                    <input id="txtPostTel" name="PostTel" class="easyui-textbox" style="width: 350px;" data-options="required:false">
                    <%--validType:'phoneNum',--%>
                </td>
            </tr>

            <tr>
                <td style="width: 100px">邮编</td>
                <td>
                    <input id="txtPostZipCode" name="PostZipCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                </td>
            </tr>


            <tr>
                <td style="width: 100px">交付方式</td>
                <td>
                    <select id="selDeliveryType" name="DeliveryType" class="easyui-combobox" data-options="editable:false" style="width: 350px"></select>
                </td>
            </tr>
            <tr>
                <td style="width: 100px"></td>
                <td>
                    <input id="IsDefault" class="easyui-checkbox" name="IsDefault" /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />

        </div>
        <%--</div>
        </div>--%>
    </div>
</asp:Content>
