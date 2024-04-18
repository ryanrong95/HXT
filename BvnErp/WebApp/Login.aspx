<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApp.Login" %>

<!doctype html>
<html>
<head runat="server">
    <script>
        //----------------浏览器判断 
        function myBrowser() {
            var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
            
            if (userAgent.indexOf("Firefox") > -1) {
                return "FF";
            } //判断是否Firefox浏览器
            if (userAgent.indexOf("Chrome") > -1 && userAgent.indexOf("Edge") == -1 && userAgent.indexOf("Browser")==-1) {
                return "Chrome";
            }
           
        }
        //以下是调用上面的函数
        var mb = myBrowser();
        //alert(navigator.userAgent);
        if (mb != "FF" && mb != "Chrome") {
            alert("建议你使用 FireFox(火狐) 或 Chrome(谷歌) 浏览器，其他浏览器可能存在兼容性问题");
        }
        //else { alert("ok");}
        //----------------------------- 

    </script>
    <title>远大芯城ERP系统</title>
    <uc:EasyUI runat="server" />
    <script src="http://fixed2.b1b.com/My/Scripts/jquery.md5.js"></script>
    <style>
        #login { width: 500px; overflow: hidden; margin: 13% auto; background: #fff; border: 8px solid #eee; -moz-border-radius: 5px; -webkit-border-radius: 5px; border-radius: 5px; -moz-box-shadow: 0 0 10px #4e707c; -webkit-box-shadow: 0 0 10px #4e707c; box-shadow: 0 0 10px #4e707c; text-align: left; }
            #login * { color: inherit; background-color: inherit; }
            #login h1 { width: 100%; background: #0092c8; color: #fff; text-shadow: #007dab 0 1px 0; font-size: 14px; padding: 18px 23px; margin: 0 0 1.5em 0; border-bottom: 1px solid #007dab; }
            #login .row { margin: .5em 25px; background: #eee; padding: 4px; -moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; text-align: right; position: relative; }
            #login label { float: left; line-height: 30px; padding-left: 10px; }
            #login .field { border: 1px solid #ccc; width: 280px; font-size: 12px; line-height: 1em; padding: 4px; -moz-box-shadow: inset 0 0 5px #ccc; -webkit-box-shadow: inset 0 0 5px #ccc; box-shadow: inset 0 0 5px #ccc; -moz-border-radius: 5px; -webkit-border-radius: 5px; border-radius: 5px; }
            #login div.submit { background: none; margin: 1em 25px; text-align: left; }
                #login div.submit label { float: none; display: inline; font-size: 11px; }
            #login button, #login input[type="submit"] { border: 0; padding: 0 30px; height: 30px; line-height: 30px; text-align: center; font-size: 14px; font-weight: bold; color: #fff; text-shadow: #007dab 0 1px 0; background: #0092c8; -moz-border-radius: 5px; -webkit-border-radius: 5px; border-radius: 5px; cursor: pointer; }
    </style>

    <script>
        $(function () {
            $('form,form input').prop('autocomplete', 'off');
            $('#txtUserName,#txtPassword').on('blur keyup', function () {
                var sender = $(this);
                var txt = $.trim(sender.val());
                if (txt.length == 0) {
                    sender.prop('placeholder', sender.attr('hitmessage'));
                }
                sender.data('__vif', txt.length > 0);
            }).each(function () {
                var sender = $(this);
                var forms = sender.parents('form');
                if (forms.length != 1) {
                    alert('error');
                }
                forms.submit(function () {
                    if (!sender.data('__vif')) {
                        sender.prop('placeholder', sender.attr('hitmessage'));
                        return false;
                    }
                    return true;
                });
            });
            $('form').submit(function () {
                $('#txtUserName,#txtPassword').blur();
                var password = $.trim($('#txtPassword').val());
                $('#hPassword').val($.md5(password));
                $('#txtPassword').val('');
            });
        });

    </script>
    
</head>
<body>
    <div id="login">
        <h1>
            <strong>管理员登录</strong>
        </h1>
        <form runat="server" id="form1" autocomplete="off">

      <%--      <input style="display: none">
            <input type="password" style="display: none">--%>

            <div class="row">
                <label for="txtUserName">
                    <strong>管理员名：</strong></label>
                <input id="txtUserName" runat="server" class="field"
                    placeholder="输入您的管理员登录名" hitmessage="管理员登录名必填" maxlength="50" />
            </div>
            <div class="row">
                <label for="txtPassword">
                    <strong>登录密码：</strong></label>
                <input id="txtPassword" runat="server" class="field"
                    placeholder="输入您的管理员密码" hitmessage="管理员密码必填" type="password" maxlength="50" />
                <input id="hPassword" type="hidden" value="" runat="server" />
            </div>
            <div class="row submit">
                <asp:Button ID="btnSubmit" runat="server" Text="登录" OnClick="btnSubmit_Click" />
            </div>
            <div><%Response.Write("IP地址："+Request.UserHostAddress);  %></div>
            <div><a href="http://www.firefox.com.cn/">FireFox浏览器下载地址：http://www.firefox.com.cn/</a></div>
        </form>
    </div>
    <input id="hLoginningFail" type="hidden" runat="server" value="您的用户名与密码错误，请您重新进行登录操作！" />
    
</body>
</html>
