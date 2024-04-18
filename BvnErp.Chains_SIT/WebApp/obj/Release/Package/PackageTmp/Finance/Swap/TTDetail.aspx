<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TTDetail.aspx.cs" Inherits="WebApp.Finance.TTDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        
        $(function () {
            debugger
            var txnRefId = getQueryString("txnRefId");           
            $("#txnRefId").val(txnRefId);
            debugger
            $.post('?action=TTDetailData', {txnRefId:txnRefId}, function (data) {
                var Result = JSON.parse(data);
                if (!Result.success) {
                    $.messager.alert('提示', Result.message);
                } else {
                    $("#senderName").textbox("setValue", Result.senderName);
                    $("#senderAccountNo").textbox("setValue", Result.senderAccountNo);
                    $("#senderSwiftBic").textbox("setValue", Result.senderSwiftBic);
                    $("#PartyNameEnglish").textbox("setValue", Result.PartyNameEnglish);
                    $("#AccountNo").textbox("setValue", Result.AccountNo);
                    $("#SwiftCode").textbox("setValue", Result.SwiftCode);
                    $("#chargeBearer").textbox("setValue", Result.chargeBearer);                    
                }
            });

        });
    </script>
    <script>
        //关闭窗口
        function Close() {
            $.myWindow.close();            
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form2" runat="server" method="post" onsubmit="return CheckSubmit()">
            <div>
                <input type="hidden" id="txnRefId" />              
            </div>
            <table id="editTable" style="margin-top: 30px">
                <tr>
                    <td class="lbl">付款人名称：</td>
                    <td>
                         <input class="easyui-textbox" id="senderName"  data-options="required:true,height:28,width:430,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款人账号：</td>
                    <td>
                         <input class="easyui-textbox" id="senderAccountNo"  data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款人银行代码：</td>
                    <td>
                         <input class="easyui-textbox" id="senderSwiftBic"  data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款人名称：</td>
                    <td>
                        <input class="easyui-textbox" id="PartyNameEnglish"  data-options="required:true,height:28,width:430,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>                               
                <tr>
                    <td class="lbl">收款人账号：</td>
                    <td>
                        <input class="easyui-textbox" id="AccountNo"data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款人银行代码：</td>
                    <td>
                        <input class="easyui-textbox" id="SwiftCode"  data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>                
                <tr>
                    <td class="lbl">银行费用：</td>
                    <td>
                       <input class="easyui-textbox" id="chargeBearer"  data-options="required:true,height:28,width:250,validType:'length[1,100]',editable:false" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">        
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">关闭</a>
    </div>
</body>
</html>
