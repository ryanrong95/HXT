<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Vrs.Venders.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        var getAjaxData = function () {
            var data = {
                action: 'data'
            };
            return data;
        };
        $(function () {
            $('#t_companies').bvgrid({ queryParams: getAjaxData() });
            $('#txtName').textbox({
                onChange: function (value) {
                    var name = $('#txtName').textbox('getValue');
                    $.post("?action=GetCompany", { txtName: name }, function (data) {
                        data = eval('(' + data + ')');
                        $('#txtAddress').textbox('setText', data.Address);
                        $('#txtRegisteredCapital').textbox('setText', data.RegisteredCapital);
                        $('#txtCorporateRepresentative').textbox('setText', data.CorporateRepresentative);
                        $('#select_grade').combobox('select', data.Grade);
                        $('#select_type').combobox('select', data.Type);
                    });
                }
            });
        })
        function AddCompany() {
            top.$.myWindow({
                url: location.pathname.replace(/Venders\/Edit.aspx/ig, '/Companies/Edit.aspx'),
                width: '400px',
                height: '200px',
                onClose: function () {
                    $("#t_vender").bvgrid('reload');
                }
            }).open();
        }
        function CompaniesInfor() {
            top.$.myWindow({
                width: '1000px',
                height: '500px',
                url: location.pathname.replace(/Venders\/Edit.aspx/ig, '/Companies/List.aspx')
            }).open();
        }

    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="供应商编辑">
        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddCompany()">添加公司</a>
        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="CompaniesInfor()">查看公司</a>

        <form id="form1" runat="server" class="easyui-layout" data-options="fit:true">
            <%
                var model = this.Model as NtErp.Vrs.Services.Models.Vender;
            %>
            <table class="liebiao">
                <tr>
                    <td>公司名称</td>
                    <td>
                        <input id="txtName" name="txtName" class="easyui-textbox" data-options="prompt:'必填',required:true" missingmessage="公司名称不能为空！" value="<%=model?.Name%>" /></td>
                    <td>级别</td>
                    <td>
                        <select id="select_grade" class="easyui-combobox" name="select_grade" style="width: 200px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.Grade)).Cast<NtErp.Vrs.Services.Enums.Grade>())
                                {
                            %>
                            <option value="<%=(int)item %>" <%=model?.Grade == item ?"selected='selected'":"" %>><%=item %></option>
                            <%
                                }
                            %>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>性质</td>
                    <td colspan="3">
                        <select id="select_type" class="easyui-combobox" name="select_type" style="width: 200px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.ComapnyType)).Cast<NtErp.Vrs.Services.Enums.ComapnyType>())
                                {
                            %>
                            <option value="<%=(int)item %>" <%=model?.Type==item?"selected='selected'":"" %>><%=item %></option>
                            <%
                                }
                            %>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>地址</td>
                    <td>
                        <input id="txtAddress" name="txtAddress" class="easyui-textbox" data-options="prompt:'必填',required:true" missingmessage="公司名称不能为空！" value="<%=model?.Address%>" style="width: 400px;" />
                    </td>
                    <td>注册资金</td>
                    <td>
                        <input id="txtRegisteredCapital" name="txtRegisteredCapital" class="easyui-numberbox" style="width: 150px;"
                            data-options="min:0,editable:true" value="<%=model?.RegisteredCapital%>" />
                    </td>
                </tr>
                <tr>
                    <td>法人代表</td>
                    <td colspan="3">
                        <input id="txtCorporateRepresentative" name="txtCorporateRepresentative" class="easyui-textbox" value="<%=model?.CorporateRepresentative%>" />
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
