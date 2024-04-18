<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Comfirm.aspx.cs" Inherits="WebApp.AdvanceMoney.Auditing.comfirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var applyID = '<%=this.Model.ApplyID%>';
        var From = '<%=this.Model.From%>';
        var ClientID = '<%=this.Model.ClientID%>';

        $(function () {

        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
        function Save() {

            var reason = $('#reason').textbox('getValue').trim();
            //if (reason == "") {
            //    $.messager.alert('提示', '审批备注不能为空！');
            //    return;
            //}
            var data = new FormData($('#form1')[0]);
            data.append("Reason", reason);
            data.append("ApplyID", applyID);
            data.append("From", From);
            data.append("ClientID", ClientID);
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
                        //window.parent.location.reload();
                        $.messager.alert('提示', "保存成功");
                        $.myWindow.close();
                    }
                    else {
                        $.messager.alert('错误', res.message);
                    }
                }
            })
        }
    </script>
    <style>
        #dlg-buttons {
            height: 40px;
            text-align: right;
            margin-top: 30px;
            padding-right: 40px;
            background-color: #F3F3F3;
            vertical-align: central;
        }

            #dlg-buttons a {
                margin-top: 5px;
                margin-bottom: 5px;
            }
    </style>
</head>
<body class="easyui-layout">
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true,closable:true,onClose:function(){$.myWindow.close();}" style="margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align: center">
                <input id="reason" class="easyui-textbox" data-options="multiline:true,tipPosition:'bottom',prompt:'请描述审核备注',validType:'length[1,40]'" style="width: 350px; height: 150px" />
            </div>

            <div id="dlg-buttons">
                <a id="btnSave" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" runat="server" onclick="Save()">保存</a>
                <a id="btnCancel" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </form>
    </div>
</body>
</html>
