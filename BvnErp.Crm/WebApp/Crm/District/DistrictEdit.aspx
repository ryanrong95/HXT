<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DistrictEdit.aspx.cs" Inherits="WebApp.Crm.District.DistrictEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var distincts = eval(<%=this.Model.Distinct%>);

        //页面加载时
        $(function () {
            $("#Father").combobox({
                data: distincts,
                onSelect: function (record) {
                    var a = record;
                    $("#hidlevel").val(record["Level"]);
                }
            });

            $("#Father").combobox("textbox").bind("blur", function () {
                var value = $("#Father").combobox("getValue");
                var data = $("#Father").combobox("getData");
                var valuefiled = $("#Father").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Father").combobox("clear");
                }
            });
        });

        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin-top: 10px; width: 100%">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 30%"></th>
            </tr>
            <tr>
                <td class="lbl">区域名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name"  data-options="required:true,validType:'length[1,50]'" style="width: 95%" />
                </td>
                <td class="lbl">上级区域</td>
                <td>
                    <input type="hidden" id="hidlevel" name="hidlevel" />
                    <input class="easyui-combobox" id="Father" name="Father" style="width: 95%"
                        data-options="required:true,valueField:'ID',textField:'Name'," />
                </td>
            </tr>
        </table>
        <div id="divSave" style="text-align: center; margin-top: 30px">
            <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return Valid();" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
    </form>
</body>
</html>
