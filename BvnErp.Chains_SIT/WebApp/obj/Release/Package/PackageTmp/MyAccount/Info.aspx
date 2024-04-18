<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Info.aspx.cs" Inherits="WebApp.AdminInfo.Info" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理员信息</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>       
        gvSettings.fatherMenu = '账户信息';
        gvSettings.menu = '我的账户';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript">
        var AdminData = eval('(<%=this.Model.AdminData%>)');

        $(function () {
            $('#UserName').textbox("readonly", true);
            $('#RealName').textbox("readonly", true);

            if (AdminData != null) {
                $("#UserName").textbox("setValue", AdminData["UserName"]);
                $("#RealName").textbox("setValue", AdminData["RealName"]);
                $("#Tel").textbox("setValue", AdminData["Tel"]);
                $("#Mobile").textbox("setValue", AdminData["Mobile"]);
                $("#Email").textbox("setValue", AdminData["Email"]);
                $("#Summary").textbox("setValue", AdminData["Summary"]);
            }
        });

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }

            var data = new FormData($('#form1')[0]);
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false
            }).done(function (res) {
                $.messager.alert('消息', res.message, 'info', function () {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info');
                    }
                });
            });
        }
    </script>
</head>
<body class="easyui-panel">
    <div class="easyui-tabs" data-options="border:false" style="width: 100%; height: 500px;">
        <div title="账户信息">
            <div id="topBar">
                <div id="tool">
                    <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
                </div>
            </div>
            <div id="content">
                <form id="form1" runat="server">
                    <table id="table1" style="line-height: 30px; width: 60%" cellspacing="8" cellpadding="10">
                        <tr>
                            <td align="right">账户名称</td>
                            <td>
                                <input class="easyui-textbox input" id="UserName" name="UserName" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">*真实姓名</td>
                            <td>
                                <input class="easyui-textbox input" id="RealName" name="RealName" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">*电话</td>
                            <td>
                                <input class="easyui-textbox input" id="Tel" name="Tel"
                                    data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入电话'" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">*手机号码</td>
                            <td>
                                <input class="easyui-textbox input" id="Mobile" name="Mobile"
                                    data-options="required:true,validType:'length[1,11]',tipPosition:'bottom',missingMessage:'请输入手机号码'" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">*邮箱</td>
                            <td>
                                <input class="easyui-textbox input" id="Email" name="Email"
                                    data-options="limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入邮箱'" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">备注</td>
                            <td>
                                <input class="easyui-textbox input" id="Summary" name="Summary"
                                    data-options="limitToList:true,required:false" />
                            </td>
                        </tr>
                    </table>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
