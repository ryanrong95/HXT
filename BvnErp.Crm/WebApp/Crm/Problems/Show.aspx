<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.Problems.Show" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        var ContactData = eval('(<%=this.Model.ContactData%>)');

        //页面加载时
        $(function () {
            $("#ContactID").combobox({
                data: ContactData
            });
            //初始化赋值
            if (AllData != null) {
                $("#Context").textbox("setValue", escape2Html(AllData["Context"]));
                $("#Answer").textbox("setValue", escape2Html(AllData["Answer"]));
                $("#ContactID").combobox("setValue", AllData["ContactID"]);
            }
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; width: 800px">
            <tr>
                <td class="lbl" style="width: 80px">内容</td>
                <td colspan="5">
                    <input class="easyui-textbox" data-options="required:true,multiline:true,validType:'length[1,500]',tipPosition:'bottom'" id="Context" name="Context" style="width: 95%; height: 180px;" readonly="true" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">回答</td>
                <td colspan="5">
                    <input class="easyui-textbox" data-options="multiline:true,validType:'length[0,500]',tipPosition:'bottom'" id="Answer" name="Answer" style="width: 95%; height: 180px;" readonly="true" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">联系人</td>
                <td>
                    <input class="easyui-combobox" id="ContactID" data-options="valueField:'value',textField:'text',required:true" name="ContactID" style="width: 38%" readonly="true" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
