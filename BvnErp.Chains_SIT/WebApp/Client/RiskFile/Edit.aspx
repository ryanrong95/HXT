<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.RiskFile.Edit" %>

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
        var ClientID = '<%=this.Model.ClientID%>';
        var FileID = '<%=this.Model.FileID%>';//附件ID
        var FileType = eval('(<%=this.Model.FileType%>)');

        if (FileID != '') {
            ClientFileData = eval('(<%=this.Model.ClientFileData != null ? this.Model.ClientFileData:""%>)');
        }

        $(function () {
            $("#FileType").combobox({
                data: FileType
            });

            //文件上传控件初始化
            $('#ClientFile').chainsupload({
                required: true,
                tipPosition:'bottom',
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf','video/mp4'],
                //onChangeFile: function (newValue, oldValue) {
                //    debugger;
                //    alert("aa" + newValue);
                //}
            });

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var data = new FormData($('#form1')[0]);
                data.append("ClientID", ClientID);
                data.append("FileID", FileID);
                $.ajax({
                    url: '?action=SaveClientFiles',
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
                    <td class="lbl">文件类型：</td>
                    <td>
                        <input class="easyui-textbox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false,tipPosition:'bottom',missingMessage:'请选择文件类型'" id="FileType" name="FileType" style="width: 450px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">选择文件：</td>
                    <td>
                        <div id="ClientFile" style="width: 450px"></div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">摘要备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,300]',tipPosition:'bottom',multiline:true" style="width: 450px; height: 60px;" />
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
