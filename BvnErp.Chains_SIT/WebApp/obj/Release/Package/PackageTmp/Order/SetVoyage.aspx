<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetVoyage.aspx.cs" Inherits="WebApp.Order.SetVoyage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <script>
        //保存
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            var value = $('#transportRequire').textbox("getValue");
            var ID = getQueryString("ID");
            //     MaskUtil.mask();
            $.post('?action=Save', { TransportRequire: escape(value), OrderId: ID },
                function (res) {
                    //        MaskUtil.unmask();
                    var result = JSON.parse(res);
                    if (result.success) {
                        $.messager.alert('', result.message, 'info', function () {

                            var ewindow = $.myWindow.getMyWindow("SetVoyage");
                            ewindow.SetSaveFlag(true);
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', result.message);
                    }
                });
        }

        function Close() {
            $.myWindow.close();
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
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true,closable:true,onClose:function(){$.myWindow.close();}" style="margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align: center">
                <input id="transportRequire" name="transportRequire" class="easyui-textbox" data-options="multiline:true,required:true,tipPosition:'bottom',validType:'length[1,250]'" style="width: 350px; height: 150px" />
            </div>

            <div id="dlg-buttons">
                <a id="A1" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" runat="server" onclick="Save()">保存</a>
                <a id="btnCancel" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </form>
    </div>
</body>
</html>
