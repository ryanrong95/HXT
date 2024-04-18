<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecEdocEdit.aspx.cs" Inherits="WebApp.Declaration.Declare.DecEdocEdit" %>

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
        var ConsigneeName = '<%=this.Model.ConsigneeName%>';
        var ConsigneeCusCode = '<%=this.Model.ConsigneeCusCode%>';
        var EdocCode = eval('(<%=this.Model.EdocCode%>)');
        var CustomMaster = eval('(<%=this.Model.CustomMaster%>)');
        $(function () {
            $("#EdocCode").combobox({
                data: EdocCode
            });

            $("#EdocOwnerCode").textbox("setValue", ConsigneeCusCode);
            $("#EdocOwnerName").textbox("setValue", ConsigneeName);

            $('#EdocOwnerCode').textbox('readonly');
            $('#EdocOwnerName').textbox('readonly');
            $('#EdocSize').textbox('readonly');
            $('#EdocCopId').textbox('readonly');


            //文件上传控件初始化
            $('#EdocFile').chainsupload({
                required: true,
                multiple: false,
                validType: ['fileSize[4,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择PDF类型的文件',
                accept: ['application/pdf'],
                onChange: function (newValue, oldValue) {                   
                    // file对象
                    var files = $('#EdocFile').next().find('input[id^="filebox_file_id_"]')["0"].files;

                    // 上传的文件大小
                    var fileSzie = files[0].size;
                    var fileName = files[0].name;
                    $('#EdocSize').textbox('setValue', fileSzie);
                    $('#EdocCopId').textbox('setValue', fileName);
                }
            });

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                var data = new FormData($('#form1')[0]);
                data.append("ID", ID);
                data.append("CustomMaster", CustomMaster);
                $.ajax({
                    url: '?action=SaveEdocFiles',
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
                    <td class="lbl">随附单据类别：</td>
                    <td>
                        <input class="easyui-combobox" data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:true,tipPosition:'right',missingMessage:'请选择随附单据类别'" id="EdocCode" name="EdocCode" style="width: 500px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">选择文件：</td>
                    <td>
                        <div id="EdocFile" style="width: 500px;"></div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">随附单据文件企业名：</td>
                    <td>
                        <input class="easyui-textbox" id="EdocCopId" name="EdocCopId" data-options="validType:'length[1,64]',tipPosition:'right'" style="width: 500px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">随附单据文件大小：</td>
                    <td>
                        <input class="easyui-textbox" id="EdocSize" name="EdocSize" data-options="validType:'length[1,12]',tipPosition:'right'" style="width: 500px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">所属单位名称：</td>
                    <td>
                        <input class="easyui-textbox" id="EdocOwnerName" name="EdocOwnerName" data-options="validType:'length[1,100]',tipPosition:'right'" style="width: 500px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">所属单位海关编号：</td>
                    <td>
                        <input class="easyui-textbox" id="EdocOwnerCode" name="EdocOwnerCode" data-options="validType:'length[1,10]',tipPosition:'right'" style="width: 500px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">操作说明(重传原因)：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,255]',tipPosition:'buttom',multiline:true" style="width: 500px; height: 60px;" />
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


