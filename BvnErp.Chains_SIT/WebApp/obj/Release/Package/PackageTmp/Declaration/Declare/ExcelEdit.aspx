<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcelEdit.aspx.cs" Inherits="WebApp.Declaration.Declare.ExcelEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
     <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            var DeclarationID = getQueryString("ID");
            $("#DeclarationID").val(DeclarationID);
        });

        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                //$.messager.alert('提示', '请按提示输入数据！');
                return;
            }
            MaskUtil.mask();//遮挡层
            $('#btnSave').linkbutton('disable');
            var DeclarationID = $("#DeclarationID").val();
            var EntryID = $("#EntryID").textbox("getValue");
            var SeqNo = $("#SeqNo").textbox("getValue");
            $.post('?action=UpdateEntryID', { ID: DeclarationID, EntryID: EntryID, SeqNo: SeqNo }, function (data) {
                var result = JSON.parse(data);
                MaskUtil.unmask();//关闭遮挡层
                $('#btnSave').linkbutton('enable');
                $.messager.alert('消息', result.info, 'info', function () {
                    Search();
                    Cancel();
                });
            });
        }

        function Search() {
            var ewindow = $.myWindow.getMyWindow("ExcelList2ExcelEdit");
            ewindow.Search();
        }

        function Cancel() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">海关编号：</td>
                    <td>
                        <input class="easyui-textbox" id="EntryID" name="EntryID" data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入海关编号'" style="width: 300px;" />
                        <input type="hidden" id="DeclarationID" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">统一编号：</td>
                    <td>
                        <input class="easyui-textbox" id="SeqNo" name="SeqNo" style="width: 300px;"  data-options="validType:'length[1,50]',tipPosition:'bottom'"/>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>
