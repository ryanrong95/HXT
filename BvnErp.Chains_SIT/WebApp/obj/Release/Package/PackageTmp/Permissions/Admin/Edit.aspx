<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Permissions.Edit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色管理</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var admin = eval('(<%=this.Model.AdminData%>)');
        //数据初始化
        $(function () {
            $('#UserName').textbox("readonly", true);
            $('#RealName').textbox("readonly", true);           
            if (admin != null) {
                $("#UserName").textbox("setValue", admin["UserName"]);
                $("#RealName").textbox("setValue", admin["RealName"]);
                $("#Tel").textbox("setValue", admin["Tel"]);
                $("#Mobile").textbox("setValue", admin["Mobile"]);
                $("#Email").textbox("setValue", admin["Email"]);
                $("#Summary").textbox("setValue", admin["Summary"]);
            }
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            data.append('ID', admin["ID"]);
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    }
                }
            }).done(function (res) {
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="padding-left:15px;">
                <tr>
                    <td align="right">账号：</td>
                    <td>
                        <input class="easyui-textbox input" id="UserName" name="UserName" />
                    </td>
                </tr>
                <tr>
                    <td align="right">*姓名：</td>
                    <td>
                        <input class="easyui-textbox input" id="RealName" name="RealName" />
                    </td>
                </tr>
                <tr>
                    <td align="right">*电话：</td>
                    <td>
                        <input class="easyui-textbox input" id="Tel" name="Tel"
                            data-options="required:true,validType:'length[1,20]',tipPosition:'bottom',missingMessage:'请输入电话'" />
                    </td>
                </tr>
                <tr>
                    <td align="right">*手机：</td>
                    <td>
                        <input class="easyui-textbox input" id="Mobile" name="Mobile"
                            data-options="required:true,validType:'mobile',tipPosition:'bottom',missingMessage:'请输入手机号码'" />
                    </td>
                </tr>
                <tr>
                    <td align="right">*邮箱：</td>
                    <td>
                        <input class="easyui-textbox input" id="Email" name="Email"
                            data-options="limitToList:true,required:true,validType:'email',tipPosition:'bottom',missingMessage:'请输入邮箱'" />
                    </td>
                </tr>
                <tr>
                    <td align="right">备注：</td>
                    <td>
                        <input class="easyui-textbox input" id="Summary" name="Summary"
                            data-options="limitToList:true,required:false,validType:'length[1,200]',multiline:true,tipPosition:'bottom'" style="height:40px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
