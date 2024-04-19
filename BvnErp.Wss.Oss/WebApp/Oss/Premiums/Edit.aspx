<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Premiums.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>附加价值</title>
    <uc:EasyUI runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <table class="liebiao">
            <tr>
                <th style="width: 120px;">附加费名称</th>
                <td>
                    <input type="text" id="_name" class="easyui-validatebox" data-options="required:true" name="_name" value="" />
                </td>
            </tr>
            <tr>
                <th>数量：</th>
                <td>
                    <input type="text" id="_count" name="_count" style="width: 200px;" class="easyui-numberbox" data-options="precision:0,required:true,min:1,max:10000," />
                </td>
                <td></td>
            </tr>
            <tr>
                <th>金额</th>
                <td>
                    <input type="text" id="_price" class="easyui-numberbox" name="_price" data-options="precision:4,min:-100000,max:100000" style="text-align: left;" />
                </td>
            </tr>
            <tr>
                <th>说明</th>
                <td>
                    <input class="easyui-textbox" id="_summary" name="_summary" data-options="multiline:true" style="width: 300px; height: 100px;" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;">
                    <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" Text="保存" OnClick="btnSubmit_Click" />
                    <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                </td>
            </tr>
        </table>
    </form>
    <input runat="server" type="hidden" id="hSuccess" value="保存成功" />
</body>
</html>
