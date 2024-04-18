<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateOrder.aspx.cs" Inherits="WebApp.Order.IcgooOrder.CreateOrder" %>

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
            
           
        });

        function Save() {
          
            var DeclarationID = $("#DeclarationID").textbox("getValue");;           
            $.post('?action=UpdateEntryID', { ID: DeclarationID}, function (data) {
                var result = JSON.parse(data);                          
                $.messager.alert('消息', result.info, 'info', function () {
                  
                });
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <input class="easyui-textbox" id="DeclarationID"  style="width: 300px;" />
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>       
    </div> 
</body>
</html>
