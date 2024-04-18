<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecFileEdit.aspx.cs" Inherits="WebApp.Declaration.Declare.DecFileEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/chainsupload.js"></script>
    <style type="text/css">
        .lbl {
            text-align: right;
        }
    </style>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var DecFileType = eval('(<%=this.Model.DecFileType%>)');
        $(function () {
            $("#DecFileType").combobox({
                data: DecFileType
            });

            //文件上传控件初始化
            $('#DecFile').chainsupload({
                required: true,
                multiple: false,
                validType: ['fileSize[4,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],

            });

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var data = new FormData($('#form1')[0]);
                data.append("ID", ID);
                $.ajax({
                    url: '?action=SaveDecFile',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        $.messager.alert('消息', res.message, 'info', function () {
                            if (res.success) {
                                closeWin();
                            }
                        });
                    }
                }).done(function (res) {

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
                    <td class="lbl">附件类型：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:true,tipPosition:'right',missingMessage:'请选择附件类型'" id="DecFileType" name="DecFileType" style="width: 400px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">选择附件：</td>
                    <td>
                        <div id="DecFile" style="width: 400px;"></div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,255]',tipPosition:'buttom',multiline:true" style="width: 400px; height: 60px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
