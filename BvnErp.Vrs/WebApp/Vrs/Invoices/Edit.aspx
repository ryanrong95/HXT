<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Vrs.Invoices.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            $("#select_type").combobox({
                onChange: function () {
                    var value = $("#select_type").combobox('getValue');
                    if (value == "2") {
                        $('.vat').hide();
                    }
                    if (value == "1") {
                        $('.vat').show();
                    }
                }
            });
            var getAjaxData = function () {
                var data = {
                    action: 'data'
                };
                return data;
            };
            $('#cbbCompany').combobox({
                url: '?action=selects_company',
                valueField: 'ID',
                textField: 'Name',
                onLoadSuccess: function () {
                    var val = $(this).combobox("getData");
                    var id = $('#cmID').val();
                    if (id) {
                        $('#cbbCompany').combobox('select', id);
                    }
                    else {
                       $('#cbbCompany').combobox('select', val[0].ID);
                    }
                }
            });
            $('#cbbContact').combobox({
                url: '?action=selects_contact',
                valueField: 'ID',
                textField: 'Name',
                onLoadSuccess: function () {
                    var val = $(this).combobox("getData");
                    var id = $('#cnID').val();
                    if (id) {
                        $('#cbbContact').combobox('select', id);
                    }
                    else {
                        $('#cbbContact').combobox('select', val[0].ID);
                    }
                }
            });
            $('.btn_company_add').click(function () {
                top.$.myWindow({
                    url: "/vrs/Vrs/Companies/Edit.aspx",
                    onClose: function () {
                        var data = getAjaxData();
                        data.action = "selects_company";
                        $('#cbbCompany').combobox('reload', '?' + $.param(data));
                    },
                    width: '600px',
                    height: '400px',
                }).open();
            })
            $('.btn_contact_add').click(function () {
                top.$.myWindow({
                    url: "/vrs/Vrs/Contacts/Edit.aspx",
                    onClose: function () {
                        var data = getAjaxData();
                        data.action = "selects_contact";
                        $('#cbbContact').combobox('reload', '?' + $.param(data));
                    },
                    width: '600px',
                    height: '400px',
                }).open();
            })

        });

    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="编辑">
        <form runat="server" class="easyui-layout" data-options="fit:true">
            
            <%
                var model = this.Model.Invoice as NtErp.Vrs.Services.Models.Invoice;
            %>
            <input type="hidden" id="cmID" value="<%=model?.CompanyID %>" />
            <input type="hidden" id="cnID" value="<%=model?.ContactID %>" />
            <table class="liebiao" id="table_invoices" data-options="rownumbers:true">
                <tr>
                    
                    <td>是否需要发票</td>
                    <td>
                        <input style="text-align: center;" type="radio" id="radioRequired" name="radioRequired" <%=model?.Required==true?"checked='checked'":"" %> value="true" checked="checked" />是
                        <input style="text-align: center;" type="radio" name="radioRequired" <%=model?.Required==false?"checked='checked' ":"" %> value="false" />否
                    </td>

                </tr>
                <tr>
                    <td>发票类型</td>
                    <td>

                        <select id="select_type" class="easyui-combobox" name="select_type" style="width: 180px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.InvoiceType)).Cast<NtErp.Vrs.Services.Enums.InvoiceType>())
                                {
                            %>
                            <option value="<%=(int)item %>" <%=model?.Type==item?"selected='selected'":"" %>><%=item %></option>
                            <%
                                }
                            %>
                        </select></td>
                </tr>
                <tr>
                    <td>公司</td>
                    <td>
                        <input class="easyui-combobox" id="cbbCompany" name="cbbCompany"
                            style="width: 178px" />
                        <a href="#" class="easyui-linkbutton btn_company_add" data-options="iconCls:'icon-add'">添加公司</a>
                    </td>
                </tr>
                <tr>
                    <td>联系人</td>
                    <td>
                        <input class="easyui-combobox" id="cbbContact" name="cbbContact"
                            style="width: 178px" />
                        <a id="btn" href="#" class="easyui-linkbutton btn_contact_add" data-options="iconCls:'icon-add'">添加联系人</a>
                    </td>
                </tr>

                <tr class="vat">
                    <td>开户银行</td>
                    <td>
                        <input id="txtBank" name="txtBank" class="easyui-textbox" data-options="required:true" value="<%=model?.Bank%>" /></td>
                </tr>
                <tr class="vat">
                    <td>开户地址</td>
                    <td>
                        <input id="txtAddress" name="txtAddress" class="easyui-textbox" data-options="required:true" missingmessage="地址不能为空！" value="<%=model?.BankAddress%>" /></td>
                </tr>
                <tr class="vat">
                    <td>账户</td>
                    <td>
                        <input id="txtAccount" name="txtAccount" class="easyui-textbox" data-options="required:true" value="<%=model?.Account%>" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="保存" />
                    </td>
                </tr>

            </table>
        </form>
    </div>
</body>
</html>
