<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Prm.Payers.Edit" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>

        $(function () {
            $('#selMethord').combobox({
                data: model.Methord,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                onChange: function (newValue, oldValue) {
                    var required = (newValue != '<%=(int)Methord.Cash%>');
                    var readonly = (newValue == '<%=(int)Methord.Cash%>');
                    var options = {};
                    options['required'] = required;
                    options['readonly'] = readonly;
                    if (readonly) {
                        $('#txtBank').textbox('setValue', '');
                        $('#txtBankAddress').textbox('setValue', '');
                        $('#txtAccount').textbox('setValue', '');
                        $('#txtSwiftCode').textbox('setValue', '');
                    }
                    $('#txtBank').textbox(options);
                    $('#txtBankAddress').textbox(options);
                    $('#txtAccount').textbox(options);
                    $('#txtSwiftCode').textbox(options);
                },
                onLoadSuccess: function () {
                    $(this).combobox('select', model.Entity == null ? '<%=(int)Methord.Cash %>' : model.Entity.Methord);
                }
            });
            $('#selCurrency').combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                onLoadSuccess: function () {
                    $(this).combobox('select', model.Entity == null ? '<%=(int)Currency.Unknown %>' : model.Entity.Currency);
                }
            });
            $('#selDistrict').combobox({
                data: model.District,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                onLoadSuccess: function () {
                    $(this).combobox('select', model.Entity == null ? '<%=(int)District.Unknown %>' : model.Entity.District);
                }
            });
            $('#selInvoiceType').combobox({
                data: model.InvoiceType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    $(this).combobox('select', model.Entity == null ? '<%=(int)InvoiceType.Unkonwn %>' : model.Entity.InvoiceType);
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load',
                   {
                       Bank: model.Entity.Bank,
                       BankAddress: model.Entity.BankAddress,
                       Account: model.Entity.Account,
                       SwiftCode: model.Entity.SwiftCode,
                       Name: model.Entity.Contact,
                       Tel: model.Entity.Tel,
                       Mobile: model.Entity.Mobile,
                       Email: model.Entity.Email
                   });
                if (model.Entity.RealID != null) {
                    $("#selEnterprise").Enterprise('setValbyID', model.Entity.RealID)
                }
                $('#selOrigin').originPlace('setVal', model.Entity.Place);
                $("#selMethord").combobox({ 'readonly': true, 'required': false });
                $("#selCurrency").combobox({ 'readonly': true, 'required': false });
                $("#txtAccount").textbox({ 'readonly': true, 'required': false });
                $("#txtBank").textbox({ 'readonly': true, 'required': false });
                $("#txtBankAddress").textbox({ 'readonly': true, 'required': false });
                $("#txtSwiftCode").textbox({ 'readonly': true, 'required': false });
            }
             else {
                $('#selOrigin').originPlace('setVal', '<%=Origin.CHN.GetOrigin().Code%>')
            }
        })

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 800px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td colspan="3">
                            <input id="selOrigin" class="easyui-originPlace" name="Place" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">实际付款人</td>
                        <td colspan="3">
                            <input id="selEnterprise" class="easyui-Enterprise" name="Enterprise" data-options="required:false,width:350,valueField: 'Name',textField: 'Name'" value="" />
                        </td>

                    </tr>

                    <tr>
                        <td style="width: 100px">支付方式</td>
                        <td>
                            <select id="selMethord" name="Methord" class="easyui-combobox readonly_style" data-options="editable:false,panelheight:'auto'" style="width: 200px"></select>
                        </td>
                        <td style="width: 100px">币种</td>
                        <td>
                            <select id="selCurrency" name="Currency" class="easyui-combobox readonly_style" data-options="editable:false,panelheight:'auto'" style="width: 200px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">开户行 </td>
                        <td>
                            <input id="txtBank" name="Bank" class="easyui-textbox readonly_style" style="width: 200px;" data-options="required:true,validType:'length[1,200]'">
                        </td>
                        <td style="width: 100px">账号</td>
                        <td>
                            <input id="txtAccount" name="Account" class="easyui-textbox readonly_style" style="width: 200px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">开户行地址</td>
                        <td>
                            <input id="txtBankAddress" name="BankAddress" class="easyui-textbox readonly_style" style="width: 200px;" data-options="required:true,validType:'length[1,200]'">
                        </td>
                        <td style="width: 100px">SwiftCode</td>
                        <td>
                            <input id="txtSwiftCode" name="SwiftCode" class="easyui-textbox readonly_style" style="width: 200px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td colspan="3">
                            <input name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                        </td>

                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td colspan="3">
                            <input name="Mobile" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'phoneNum'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">电话</td>
                        <td colspan="3">
                            <input name="Tel" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'telNum'">
                        </td>

                    </tr>
                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td colspan="3">
                            <input name="Email" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'email'">
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>