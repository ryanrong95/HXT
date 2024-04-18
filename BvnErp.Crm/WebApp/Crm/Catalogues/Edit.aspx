<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Catalogues.Edit" ValidateRequest="false"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>类目编辑</title>
    <UC:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            var data = eval('(<%=this.Model%>)');
            $("#Summary").textbox("setValue",data["Summary"]);
        });

        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server" method="post">
            <table id="table1">
                <tr style="height:50px">
                    <td class="lbl" style="width:80px">类目描述</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                             data-options="multiline:true,required:true,validType:'length[1,300]',tipPosition:'bottom'" style="width: 180px"/>
                    </td>
                </tr>
            </table>
            <div id="divSave" style="text-align:center">
                <asp:Button ID="btnSumit" Text="保存" runat="server" OnClientClick="return Valid();"  OnClick="btnSave_Click"  />
                <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()"  />
            </div>
        </form>
    </div>
</body>
</html>
