<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="WsEdit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Invoices.WsEdit" %>

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
                $('#form1').form('load', model.Entity);
            }
            if (model.NotShowBtnSave || model.Nonstandard) {
                $("#btnSave").hide();
            }

        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
       <%-- <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">--%>
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">开户行</td>
                        <td>
                            <input id="txtBank" name="Bank" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="prompt:'必填',required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                     <tr>
                        <td style="width: 100px">发票地址</td>
                        <td>
                            <input id="txtInvoiceAddress" name="InvoiceAddress" class="easyui-textbox" style="width: 350px;"
                                data-options="prompt:'必填',required:true,validType:'length[1,150]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">开户行地址</td>
                        <td>
                            <input id="txtBankAddress" name="BankAddress" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="prompt:'必填',required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">发票类型</td>
                        <td>
                            <select id="selType" name="Type" class="easyui-combobox readonly_style" data-options="editable:false" style="width: 350px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">银行账号</td>
                        <td>
                            <input id="txtAccount" name="Account" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">纳税人识别号</td>
                        <td>
                            <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox readonly_style" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">企业电话</td>
                        <td>
                            <input id="txtCompanyTel" name="CompanyTel" class="easyui-textbox" style="width: 350px;" data-options="validType:'telNum',required:true">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">收票地区
                        </td>
                        <td>
                            <select id="selDistrict" name="District" class="easyui-combobox" data-options="editable:false,required:true" style="width: 350px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">收票地址</td>
                        <td>
                            <input id="txtAddress" name="Address" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">邮编</td>
                        <td>
                            <input id="txtPostzip" name="Postzip" class="easyui-textbox" style="width: 350px;" data-options="prompt:'收货地址的邮编',validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td>
                            <input id="txtContactName" name="Name" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]',required:true">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td>
                            <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 350px;" data-options="validType:'phoneNum',required:true">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">电话</td>
                        <td>
                            <input id="txtTel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="validType:'telNum'">
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td>
                            <input id="txtEmai" name="Email" class="easyui-textbox" style="width: 350px;" data-options="validType:'email'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">交付方式</td>
                        <td>
                            <select id="selDeliveryType" name="DeliveryType" class="easyui-combobox" data-options="editable:false" style="width: 350px"></select>
                        </td>
                    </tr>
                </table>
                <%--<div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <a id="btnSave" particle="Name:'保存',jField:'btnSave'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>--%>
            <%--</div>
        </div>--%>
    </div>
</asp:Content>
