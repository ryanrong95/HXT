<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Reports.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />

    <script>
        <%--var DrpFollowUpMethod = eval('(<%=this.Model.DrpFollowUpMethod%>)');
        var ItemData = eval('(<%=this.Model.ItemData%>)');


        $(function () {

            $("#FollowUpMethod").combobox({
                data: DrpFollowUpMethod
            });

            if (ItemData != "") {
                $("#FollowUpMethod").combobox("setValue", ItemData["FollowUpMethod"]);
                $("#FollowUpDate").textbox("setValue", new Date(ItemData["FollowUpDate"]).toDateStr());
                $("#NextFollowUpDate").textbox("setValue", new Date(ItemData["NextFollowUpDate"]).toDateStr());
                $("#FollowUpContent").textbox("setValue", ItemData["FollowUpContent"]);
            }

        });--%>
</script>
    <script>
        function closeWin() {
            $.myWindow.close();
        }
    </script>

</head>
<body>
    <%-- <form id="form1" runat="server">
        <table id="table1">
            <tr>
                <td class="lbl">跟进方式</td>
                <td>
                    <input class="easyui-combobox" id="FollowUpMethod" name="FollowUpMethod"
                        data-options="valueField:'value',textField:'text',required:true" style="width: 95%" />
                </td>
                <td class="lbl">跟踪日期</td>
                <td>
                    <input type="text" class="easyui-datebox" id="FollowUpDate" name="FollowUpDate"
                        data-options="required:true,editable:false" style="width: 150px" />
                </td>
                <td class="lbl">下次跟踪日期</td>
                <td>
                    <input type="text" class="easyui-datebox" id="NextFollowUpDate" name="NextFollowUpDate"
                        data-options="required:true,editable:false" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">跟进内容</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="FollowUpContent" name="FollowUpContent" style="width: 95%; height: 80px;" data-options="multiline:true,validType:'length[1,300]',tipPosition:'bottom'" />
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center">
            <asp:Button ID="btnSumit" data-options="iconCls:'icon-save'" class="easyui-linkbutton" Text="保存" runat="server" OnClientClick="return Valid();" OnClick="btnSumit_Click" />
            <asp:Button ID="Button1" data-options="iconCls:'icon-save'" class="easyui-linkbutton" Text="取消" runat="server" OnClientClick="closeWin()" />
        </div>
    </form>--%>
</body>
</html>
