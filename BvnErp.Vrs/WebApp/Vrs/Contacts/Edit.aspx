<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Vrs.Contacts.Edit" %>

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
        });
    </script>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="联系人编辑页" style="width: 100%;">
        <form runat="server" class="easyui-layout" data-options="fit:true">

            <%
                var model = this.Model as NtErp.Vrs.Services.Models.Contact;
            %>
               <input type="hidden" id="cmID" value="<%=model?.CompanyID %>" />
            <table class="liebiao">
                <tr>
                    <td style="width: 100px; text-align: center;">姓名:</td>
                    <td>
                        <input id="txtName" style="text-align: center;" type="text" name="txtName" value="<%=model?.Name%>" class=" easyui-textbox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">性别:</td>
                    <td>
                        <input style="text-align: center;" type="radio" id="radioSex" name="radioSex" <%=model?.Sex==true?"checked='checked'":"" %> value="true" />男
                        <input style="text-align: center;" type="radio" name="radioSex" <%=model?.Sex==false?"checked='checked' ":"" %> value="false" />女
                    </td>
                </tr>
                <tr>

                    <td style="width: 100px; text-align: center;">生日:</td>
                    <td>
                        <input style="text-align: center;" type="text" id="txtBirthday" name="txtBirthday" value="<%=model?.Birthday%>" class="easyui-datebox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">电话:</td>
                    <td>
                        <input style="text-align: center;" type="text" id="txtTel" name="txtTel" value="<%=model?.Tel %>" class=" easyui-textbox" maxlength="50" />
                    </td>

                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">邮件:</td>
                    <td>
                        <input style="text-align: center;" type="text" id="txtEmail" name="txtEmail" value="<%=model?.Email%>" class=" easyui-textbox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">手机:</td>
                    <td>
                        <input style="text-align: center;" type="text" id="txtMobile" name="txtMobile" value="<%=model?.Mobile%>" class=" easyui-textbox" maxlength="50" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">公司:</td>


                    <td>
                          <input class="easyui-combobox" id="txtCompanyID" name="txtCompanyID"  style="width: 178px" />
                    <%--    <input style="text-align: center;" type="text" id="txtCompanyID" name="txtCompanyID" value="<%=model?.Company.Name%>"/>--%>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px; text-align: center;">状态:</td>
                    <td>

                        <select id="select_grade" class="easyui-combobox" name="txtStatus" style="width: 200px;">
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

                    <td style="width: 100px; text-align: center;">职务:</td>
                    <td>
                         
                          <select id="select_job" class="easyui-combobox" name="txtJob" style="width: 200px;">
                            <%
                                foreach (var item in Enum.GetValues(typeof(NtErp.Vrs.Services.Enums.JobType)).Cast<NtErp.Vrs.Services.Enums.JobType>())
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
