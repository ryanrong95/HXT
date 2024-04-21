<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Admins.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model && model.UserName) {
                $('form').form('load', model);

                $('#txtUserName').textbox('readonly');
                $('#txtRealName').textbox('readonly');

                if (model.Status == '<%=(int)Yahv.Underly.AdminStatus.Super%>') {
                    $('#slctRole').combobox('readonly');
                }
            }

            $('#txtPassword1,#txtPassword2').passwordbox({ required: model == null });
            $('#txtPassword1').passwordbox({
                onChange: function () {
                    if (false) {
                        return;
                    }
                    $('#txtPassword2').passwordbox({ required: true });
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div>
        <table class="liebiao">
            <tr>
                <td>用户名</td>
                <td>
                    <input id="txtUserName" name="UserName" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,75]'" />
                </td>
                <td>真实名</td>
                <td>
                    <input id="txtRealName" name="RealName" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,75]'" />
                </td>
            </tr>
            <%-- <tr>
                    <td>大赢家ID</td>
                    <td colspan="3">
                        <input id="txtSelCode" name="DyjCode" class="easyui-textbox" style="width: 200px;"
                            data-options="prompt:'',required:true,validType:'length[1,50]'" />
                    </td>
                </tr>--%>

            <tr>
                <td>设置密码</td>
                <td>
                    <input id="txtPassword1" name="Password" class="easyui-passwordbox" style="width: 200px;"
                        data-options="prompt:'',validType:'length[1,50]'" /></td>

                <td>密码确认</td>
                <td>
                    <input id="txtPassword2" class="easyui-passwordbox" style="width: 200px;"
                        data-options="prompt:'',validType:['equalTo[\'#txtPassword1\']','length[1,50]'],invalidMessage:'两次输入密码不匹配'" />
                </td>
            </tr>

            <tr>
                <td>角色</td>
                <td colspan="3">
                    <select id="slctRole" name="RoleID" class="easyui-combobox"
                        data-options="editable: true, url:'?action=roles', valueField: 'id', textField: 'name',panelHeight:'160px'"
                        style="width: 200px">
                    </select>
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
</asp:Content>
