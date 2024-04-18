<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="WebApp.zFrames.ChangePassword" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>账户安全</title>
    <uc:EasyUI runat="server" />
    <script src="http://fixed2.b1b.com/My/Scripts/jquery.md5.js"></script>
    <script type="text/javascript">
        var check_password_len_msg = '密码最短不能小于8个字符，最长不能超过32个字符';
        var check_password_msg = '密码必须包涵：大小写字母、数字';

        function check_password_len(val) {
            return 8 <= val.length && val.length <= 32;
        }

        // 判断是否含有大写字母
        function hasCapital(str) {
            var result = str.match(/^.*[A-Z]+.*$/);
            if (result == null) return false;
            return true;
        }

        // 判断是否含有小写字母
        function hasLowercase(str) {
            var result = str.match(/^.*[a-z]+.*$/);
            if (result == null) return false;
            return true;
        }

        // 判断是否含有数字
        function hasNumber(str) {
            var result = str.match(/^.*[0-9]+.*$/);
            if (result == null) return false;
            return true;
        }

        $(function () {
            $('#form1').submit(function () {
                var oldPassword = $.trim($('#txtOldPassword').val());
                if (oldPassword.length == 0) {
                    alert('请输入原密码', '');
                    return false;
                }

                var password1 = $.trim($('#txtPassword1').val());
                var password2 = $.trim($('#txtPassword2').val());

                if (password1.length == 0 || password2.length == 0 || password1 != password2) {
                    alert('如果您想修改密码，请保证密码输入正确！并保证两次输入密码完全一致！', '');
                    return false;
                }

                if (!check_password_len(password1) || !check_password_len(password2)) {
                    alert(check_password_len_msg);
                    return false;
                }

                if (!hasCapital(password1) || !hasLowercase(password1) || !hasNumber(password1)
                    || !hasCapital(password2) || !hasLowercase(password2) || !hasNumber(password2)) {
                    alert(check_password_msg);
                    return false;
                }

                $('#holdpassword').val($.md5(oldPassword));
                $('#hpassword').val($.md5(password1));
                //$('#txtOldPassword').val('');
                //$('#txtPassword1').val('');
                //$('#txtPassword2').val('');

                return true;
            });
        });
    </script>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:false" title="修改密码" style="width: 100%;">
        <form id="form1" runat="server">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">登录名
                    </td>
                    <td>
                        <asp:Literal ID="txtUserName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">真实姓名
                    </td>
                    <td>
                        <asp:Literal ID="txtRealName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td>原密码
                    </td>
                    <td>
                        <input id="txtOldPassword"  type="password"  runat="server" placeholder="原密码" style="width: 40%" />
                        <input id="holdpassword" type="hidden" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>新密码
                    </td>
                    <td>
                        <input id="txtPassword1" type="password" runat="server" placeholder="新密码" minlength="8" maxlength="32" style="width: 40%" />
                    </td>
                </tr>
                <tr>
                    <td>确认新密码
                    </td>
                    <td>
                        <input id="txtPassword2" type="password" runat="server" placeholder="确认新密码" minlength="8" maxlength="32" style="width: 40%" />
                        <input id="hpassword" type="hidden" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnSubmit" Text="提交" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <input id="hSuccessMsg" type="hidden" runat="server" value="修改密码成功！" />
    <input type="hidden" id="OldPassError" value="原密码错误！" runat="server" />
    <input type="hidden" id="NoUpdatePassowrd" value="Sa不能修改密码！" runat="server" />
</body>
</html>
