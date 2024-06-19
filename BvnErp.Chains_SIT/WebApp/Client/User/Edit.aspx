<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.User.Edit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var UserID = '<%=this.Model.UserID%>';
        var Tel = '<%=this.Model.Tel%>';
        if (UserID != '') {
            UserData = eval('(<%=this.Model.UserData != null ? this.Model.UserData:""%>)');
        }

        $(function () {
            if (UserID != '') {
                $("#Name").textbox("setValue", UserData.Name);
                $("#RealName").textbox("setValue", UserData.RealName);
                $("#Mobile").textbox("setValue", UserData.Mobile);
                $("#Email").textbox("setValue", UserData.Email);
                $("#Summary").textbox("setValue", UserData.Summary);
                $('#IsMain').prop('checked', UserData.IsMain);
                $("#trPass").css("display", "none");
            }
            else {
                $("#Password").textbox("setValue", "HXT123");
                $("#trPass").css("display", "table-row");
                $("#Mobile").textbox("setValue", Tel);
            }

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values['ClientID'] = ID;//会员ID
                values['UserID'] = UserID;//收件地址ID

                //提交后台
                $.post('?action=IsExitName', { ID: UserID, Name:values["Name"] }, function (res) {
                    if (!res) {
                        $.messager.alert('错误', "用户名已存在！");
                        return;
                    } else {
                        $.post('?action=SaveUserAccount', { Model: JSON.stringify(values) }, function (res) {
                            var result = JSON.parse(res);
                            $.messager.alert('消息', result.message, 'info', function () {
                                if (result.success) {
                                    closeWin();
                                }
                            });
                        });
                    }
                });
              
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });
        });

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">用户名：</td>
                    <td>
                        <input class="easyui-textbox" id="Name" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入用户名'" style="width: 450px;" />
                    </td>
                </tr>
                <tr id="trPass">
                    <td class="lbl">初始密码：</td>
                    <td>
                        <input class="easyui-textbox" id="Password" data-options="validType:'length[1,50]',tipPosition:'bottom'" readonly="true" style="width: 450px;" />
                    </td>
                </tr>
                <tr style="display: none">
                    <td class="lbl">真实姓名：</td>
                    <td>
                        <input class="easyui-textbox" id="RealName" data-options="validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入真实姓名'" style="width: 450px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">手机号码：</td>
                    <td>
                        <input class="easyui-textbox" id="Mobile" data-options="validType:'mobile',tipPosition:'bottom',required:true,missingMessage:'请输入手机号码'" style="width: 450px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">邮箱：</td>
                    <td>
                        <input class="easyui-textbox" id="Email" data-options="validType:'email',tipPosition:'bottom',required:false,missingMessage:'请输入邮箱'" style="width: 450px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" data-options="validType:'length[1,250]',tipPosition:'bottom',multiline:true" style="width: 450px; height: 60px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl"></td>
                    <td style="text-align: left">
                        <input type="checkbox" id="IsMain" name="IsMain" checked="checked" /><label for="IsMain" style="margin-right: 30px">设为主账号</label>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
