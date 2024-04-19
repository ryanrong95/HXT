<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Vrs.Companies.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:easyui runat="server" />
    <script>
        var getAjaxData = function () {
            var data = {
                action: 'data'
            };
            return data;
        };
        $(function () {
            $('#txtName').textbox({
                onChange: function (value) {
                    var name = $('#txtName').textbox('getValue');
                    $.post("?action=GetCompany", { txtName: name }, function (data) {
                        data = eval('(' + data + ')');
                        $('#txtAddress').textbox('setText', data.Address);
                        $('#txtRegisteredCapital').textbox('setText', data.RegisteredCapital);
                        $('#txtCorporateRepresentative').textbox('setText', data.CorporateRepresentative);
                        $('#txtCode').textbox('setText', data.Code);
                        $('#txtSummary').textbox('setText', data.Summary);
                        $('#select_type').combobox('select', data.Type);
                    });
                }
            });
        })
    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="编辑">
        <form runat="server" class="easyui-layout" data-options="fit:true">
            <%
                var model = this.Model as NtErp.Vrs.Services.Models.Company;
            %>
            <table class="liebiao">
                <tr>
                    <td>公司名称</td>
                    <td colspan="3">
                        <input id="txtName" name="txtName" class="easyui-textbox" data-options="required:true" missingmessage="公司名称不能为空！" style="width: 250px;" value="<%=model?.Name%>" /></td>

                </tr>
                <tr>
                    <td>公司类型</td>
                    <td colspan="3">
                        <select id="select_type" class="easyui-combobox" name="select_type" style="width: 180px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.ComapnyType)).Cast<NtErp.Vrs.Services.Enums.ComapnyType>())
                                {
                            %>
                            <option value="<%=(int)item %>" <%=model?.Type==item?"selected='selected'":"" %>><%=item %></option>
                            <%
                                }
                            %>
                        </select></td>
                </tr>
                <tr>
                    <td>地址</td>
                    <td>
                        <input id="txtAddress" name="txtAddress" class="easyui-textbox" data-options="required:true" style="width:200px;" missingmessage="公司地址不能为空！" value="<%=model?.Address%>" />
                    </td>
                    <td>纳税人识别号</td>
                    <td>
                        <input id="txtCode" name="txtCode" class="easyui-textbox" data-options="required:true" missingmessage="纳税人识别号不能为空！" value="<%=model?.Code%>" /></td>
                </tr>
                <tr>
                    <td>注册资金</td>
                    <td>
                        <input id="txtRegisteredCapital" name="txtRegisteredCapital" class="easyui-numberbox" style="width: 150px;"
                            data-options="min:0,editable:true" value="<%=model?.RegisteredCapital%>" />
                    </td>
                    <td>法人代表</td>
                    <td>
                        <input id="txtCorporateRepresentative" name="txtCorporateRepresentative" class="easyui-textbox" value="<%=model?.CorporateRepresentative%>" />
                    </td>
                </tr>
                <tr>
                    <td>描述</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="txtSummary" name="txtSummary"
                            data-options="validType:'length[1,500]',multiline:true,tipPosition:'bottom'" style="width: 445px; height: 50px" value="<%=model?.Summary%>" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="保存" />
                    </td>
                </tr>

            </table>
        </form>
    </div>
</body>
</html>
