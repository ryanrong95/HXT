<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Invoices.Edit" %>

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
            $('#selDistrict').combobox({
                data: model.District,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.District);  //全部
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#txtBank').textbox('readonly');
                $('#txtBankAddress').textbox('readonly');
                $('#txtTaxperNumber').textbox('readonly');
                $('#txtAccount').textbox('readonly');
                $('#selType').combobox({ readonly: true });;
                $('#form1').form('load', model.Entity);
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
                        <td style="width: 110px">开户行</td>
                        <td>
                            <input id="txtBank" name="Bank" class="easyui-textbox readonly_style"  style="width: 350px;"
                                data-options="prompt:'必填',required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr>
                        <td>开户行地址</td>
                        <td>
                            <input id="txtBankAddress" name="BankAddress" class="easyui-textbox readonly_style"  style="width: 350px;"
                                data-options="prompt:'必填',required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr>
                        <td>发票类型</td>
                        <td>
                            <select id="selType" name="Type" class="easyui-combobox readonly_style"  data-options="editable:false" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>银行账号</td>
                        <td>
                            <input id="txtAccount" name="Account" class="easyui-textbox readonly_style"  style="width: 350px;"
                                data-options="validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr>
                        <td>纳税人识别号</td>
                        <td>
                            <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox readonly_style" style="width: 350px;" data-options="prompt:'管理员自行保障正确性',validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td>收票地区
                        </td>
                        <td>
                            <select id="selDistrict" name="District" class="easyui-combobox" data-options="editable:false" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>收票地址</td>
                        <td>
                            <input id="txtAddress" name="Address" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td>邮编</td>
                        <td>
                            <input id="txtPostzip" name="Postzip" class="easyui-textbox" style="width: 350px;" data-options="prompt:'收货地址的邮编',validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td>联系人姓名</td>
                        <td>
                            <input id="txtContactName" name="Name" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td>电话</td>
                        <td>
                            <input id="txtTel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="validType:'telNum'">
                        </td>
                    </tr>
                    <tr>
                        <td>手机号</td>
                        <td>
                            <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 350px;" data-options="validType:'phoneNum'">
                        </td>
                    </tr>
                    <tr>
                        <td>邮箱</td>
                        <td>
                            <input id="txtEmai" name="Email" class="easyui-textbox" style="width: 350px;" data-options="validType:'email'">
                        </td>
                    </tr>

                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <%-- <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
