<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Vrs.Beneficiaries.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:easyui runat="server" />
      <script>
        $(function () {
            var getAjaxData = function () {
                var data = {
                    action: 'data'
                };
                return data;
            };
       
            $('#txtCompanyID').combobox({
                url: '?action=selects_company',
                valueField: 'ID',
                textField: 'Name',
                onLoadSuccess: function () {
                    var val = $(this).combobox("getData");
                    var id = $('#cmID').val();
                    if (id) {
                        $('#txtCompanyID').combobox('select', id);
                    }
                    else {
                        $('#txtCompanyID').combobox('select', val[0].ID);
                    }
                }
            });
            $('#txtContactID').combobox({
                url: '?action=selects_contact',
                valueField: 'ID',
                textField: 'Name',
                onLoadSuccess: function () {
                    var val = $(this).combobox("getData");
                    var id = $('#cnID').val();
                    if (id) {
                        $('#txtContactID').combobox('select', id);
                    }
                    else {
                        $('#txtContactID').combobox('select', val[0].ID);
                    }
                }
            });
        });
    </script>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="受益人编辑页" style="width: 100%;">
        <form runat="server" class="easyui-layout" data-options="fit:true">

            <%
                var model = this.Model as NtErp.Vrs.Services.Models.Beneficiary;
            %>
              <input type="hidden" id="cmID" value="<%=model?.CompanyID %>" />
               <input type="hidden" id="cnID" value="<%=model?.ContactID%>" />
            <table class="liebiao">
                <tr>
                    <td style="width: 100px; text-align: center;">银行名称:</td>
                    <td>
                        <input id="txtBank" style="text-align: center;" type="text" name="txtBank" value="<%=model?.Bank%>" class=" easyui-textbox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">支付方式:</td>
                    <td>
                        <select id="select_PayMethod" class="easyui-combobox" name="txtMethod" style="width: 200px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.PayMethod)).Cast<NtErp.Vrs.Services.Enums.PayMethod>())
                                {
                            %>
                            <option value="<%=(int)item %>"><%=item %></option>
                            <%
                                }
                            %>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">货币类型:</td>
                    <td>
                        <select id="select_Currency" class="easyui-combobox" name="txtCurrency" style="width: 200px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(Needs.Underly.Currency)).Cast<Needs.Underly.Currency>())
                                {
                            %>
                            <option value="<%=(int)item %>"><%=item %></option>
                            <%
                                }
                            %>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">送达地址:</td>
                    <td>
                        <input id="txtAddress" style="text-align: center;" type="text" name="txtAddress" value="<%=model?.Address%>" class=" easyui-textbox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">银行查询:</td>
                    <td>
                        <input id="txtSwiftCode" style="text-align: center;" type="text" name="txtSwiftCode" value="<%=model?.SwiftCode%>" class=" easyui-textbox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">联系人:</td>
                    <td>
                         <input class="easyui-combobox" id="txtContactID" name="txtContactID"
                            style="width: 178px" />
                       <%-- <input id="txtContactID" style="text-align: center;" type="text" name="txtContactID" value="<%=model?.ContactID%>" class=" easyui-textbox" maxlength="50" />--%>
                    </td>
                </tr>
                 <tr>
                    <td style="width: 100px; text-align: center;">公司名称:</td>
                    <td>
                         <input class="easyui-combobox" id="txtCompanyID" name="txtCompanyID"  style="width: 178px" />
                       <%-- <input id="txtCompanyID" style="text-align: center;" type="text" name="txtCompanyID" value="<%=model?.Company.Name%>" class=" easyui-textbox" maxlength="50" />--%>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">状态:</td>
                    <td>

                        <select id="select_Status" class="easyui-combobox" name="txtStatus" style="width: 200px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.Status)).Cast<NtErp.Vrs.Services.Enums.Status>())
                                {
                            %>
                            <option value="<%=(int)item %>"><%=item %></option>
                            <%
                                }
                            %>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button name="submitbtn" ID="Button1" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="保存" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
