<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WrapTypeInfo.aspx.cs" Inherits="WebApp.BaseData.WrapTypeInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>包装信息</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {

        });

        function Close() {
            $.myWindow.close();
        }

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
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        //alert(res.message);
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
            <table id="editTable">
                <tr>
                    <td class="lbl">代码</td>
                    <td>
                        <input class="easyui-textbox input" id="Code" name="Code"
                            data-options="required:true,validType:'length[1,4]',tipPosition:'bottom',missingMessage:'请输入代码'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">名称</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入名称'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
