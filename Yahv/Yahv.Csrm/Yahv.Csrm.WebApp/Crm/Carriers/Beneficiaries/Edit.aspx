<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Beneficiaries.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selMethord').combobox({
                data: model.Methord,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    var data = $(this).combobox('getData');
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Methord);  //全部
                    }
                }
            });
            $('#selCurrency').combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    var data = $(this).combobox('getData');
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Currency);  //全部
                    }
                }
            });
            $('#selDistrict').combobox({
                data: model.District,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    var data = $(this).combobox('getData');
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.District);  //全部
                    }
                }
            });
            $('#selInvoiceType').combobox({
                data: model.InvoiceType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    var data = $(this).combobox('getData');
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.InvoiceType);  //全部
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', {
                    RealName: model.Entity.RealName,
                    Bank: model.Entity.Bank,
                    Account: model.Entity.Account,
                    BankAddress: model.Entity.BankAddress,
                    SwiftCode: model.Entity.SwiftCode,
                    Name: model.Entity.Name,
                    Tel: model.Entity.Tel,
                    Mobile: model.Entity.Mobile,
                    Email: model.Entity.Email
                });
                $('#txtRealName').textbox('readonly');
                $('#txtBank').textbox('readonly');
                $('#txtAccount').textbox('readonly');
                $('#txtBankAddress').textbox('readonly');
                $('#txtSwiftCode').textbox('readonly');
                $('#selMethord').combobox({ readonly: true });
                $('#selCurrency').combobox({ readonly: true });
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 110px">实际的企业名称</td>
                        <td>
                            <input id="txtRealName" name="RealName" class="easyui-textbox readonly_style"
                                data-options="validType:'length[1,75]'" style="width: 350px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>开户行</td>
                        <td>
                            <input id="txtBank" name="Bank" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="prompt:'开户银行',required:true,validType:'length[1,100]'" />
                        </td>
                    </tr>
                    <tr>
                        <td>账号</td>
                        <td>
                            <input id="txtAccount" name="Account" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="prompt:'银行账号',required:true,validType:'length[1,75]'" />
                        </td>
                    </tr>
                    <tr>
                        <td>开户行地址</td>
                        <td>
                            <input id="txtBankAddress" name="BankAddress" class="easyui-textbox readonly_style"
                                data-options="prompt:'开户银行地址',required:true,validType:'length[1,200]'" style="width: 350px;" />
                        </td>
                    </tr>

                    <tr>
                        <td>银行编码</td>
                        <td>
                            <input id="txtSwiftCode" name="SwiftCode" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="prompt:'银行编码',required:true,validType:'length[1,75]'" />
                        </td>
                    </tr>
                    <tr>
                        <td>支付方式</td>
                        <td>
                            <select id="selMethord" name="Methord" class="easyui-combobox readonly_style" data-options="editable:false,panelheight:'auto'" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr id="trcode">
                        <td>支付币种</td>
                        <td>
                            <select id="selCurrency" name="Currency" class="easyui-combobox readonly_style" data-options="editable:false,panelheight:'auto'" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>地区</td>
                        <td>
                            <select id="selDistrict" name="selDistrict" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>发票</td>
                        <td>
                            <select id="selInvoiceType" name="InvoiceType" class="easyui-combobox" data-options="editable:false,panelheight:'auto' " style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>联系人姓名</td>
                        <td>
                            <input id="txtName" name="Name" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td>电话</td>
                        <td>
                            <input id="txtTel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="validType:'telNum'" />
                        </td>
                    </tr>
                    <tr>
                        <td>手机号</td>
                        <td>
                            <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 350px;" data-options="validType:'phoneNum'" />
                        </td>
                    </tr>
                    <tr>
                        <td>邮箱</td>
                        <td>
                            <input id="txtEmail" name="Email" class="easyui-textbox" style="width: 350px;" data-options="validType:'email',required:false">
                        </td>
                    </tr>
                    <%-- <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input class="easyui-checkbox" id="IsDefault" name="IsDefault" /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>--%>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <%-- <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

