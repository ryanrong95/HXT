<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WeeklyComment.aspx.cs" Inherits="WebApp.Crm.WorksWeekly.WeeklyComment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var admins = eval('(<%=this.Model.Admins%>)');

        $(function () {
            var Type = getQueryString("Type");
            if (Type == "30") {
                $("#read").show();
            }else {
                $("#read").hide();
            }

            $("#Reader").combobox("textbox").bind("blur", function () {
                var values = [];
                $.map($("#Reader").combobox("getValues"), function (value) {
                    var data = $("#Reader").combobox("getData");
                    var valuefiled = $("#Reader").combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index >= 0) {
                        values.push(value);
                    }
                });
                $("#Reader").combobox("setValues", values);
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; width: 100%; height: 100%">
            <tr>
                <td class="lbl"><strong>点评人</strong></td>
                <td>
                    <asp:Label ID="AdminName" runat="server" Text=""></asp:Label>
                </td>
                <td class="lbl"><strong>点评时间</strong></td>
                <td>
                    <asp:Label ID="CreateDate" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr id="read">
                <td class="lbl">指定阅读人</td>
                <td>
                    <input class="easyui-combobox" id="Reader" name="Reader"
                        data-options="valueField:'ID',textField:'RealName',data: admins,multiple:true" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="width: 80px">点评内容</td>
                <td colspan="3">
                    <input class="easyui-textbox" data-options="multiline:true,required:true,tipPosition:'bottom'" name="CommentValue" id="CommentValue" style="width: 300px; height: 100px" />
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSumit" Text="提交" runat="server" OnClientClick="return Valid();" OnClick="btnSave_Click" />
        </div>
    </form>
</body>
</html>
