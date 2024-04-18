<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReturnReason.aspx.cs" Inherits="WebApp.Control.Merchandiser.MatchReturnReason" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {

        });

        //关闭窗口
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
<body class="easyui-layout">
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true,closable:true,onClose:function(){$.myWindow.close();}" style="margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align:center">
                <input id="reason" name="reason" class="easyui-textbox" data-options="multiline:true,required:true,tipPosition:'bottom',prompt:'请详细描述退回原因，以便客户或跟单员修改订单',validType:'length[1,400]'" style="width: 350px; height: 150px" />
            </div>

            <div id="dlg-buttons">
                <a id="btnSave" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" runat="server" onclick="return Valid()" onserverclick="btnSave_ServerClick">保存</a>
               <%-- <a id="btnCancel" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>--%>
            </div>
        </form>
    </div>
</body>
</html>
