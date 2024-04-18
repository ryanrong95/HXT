<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Industries.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        //页面加载时
        $(function () {
            //初始化赋值
            if (AllData != null) {
                $("#Name").textbox("setValue", AllData["Name"]);
                $("#EnglishName").textbox("setValue", escape2Html(AllData["EnglishName"]));
            }
        });

        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body> 
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto;padding-top:10px">
            <tr>
                <td class="lbl" style="width: 80px">中文名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" name="Name"
                        style="width: 250px" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">英文名称</td>
                <td>
                    <input class="easyui-textbox" id="EnglishName" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" name="EnglishName"
                        style="width: 250px" />
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSumit" Text="保存" runat="server" OnClientClick="return Valid();" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
    </form>
</body>
</html>
