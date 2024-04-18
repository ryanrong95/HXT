<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Companys.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            var type = getQueryString("Type");
            var data = eval('(<%=this.Model.AllData%>)');
            var typevalue = <%=this.Model.Type%>;
            var typedata=eval('(<%=this.Model.TypeData%>)');
            $("#Type").combobox({
                data: typedata
            });
            if (data != null) {
                $("#Name").textbox("setValue", data["Name"]);
                $("#Code").textbox("setValue", data["Code"]);
                $("#Summary").textbox("setValue", data["Summary"]);
            }
            if (typevalue != null) {
                $("#Type").combobox("setValue", typevalue);
            }
            if(type=="M"){
                $("#NameLabel").html("品牌全称");
                $("#CodeLabel").html("品牌简称");
            }

            $("#Type").combobox("textbox").bind("blur", function () {
                var value = $("#Type").combobox("getValue");
                var data = $("#Type").combobox("getData");
                var valuefiled = $("#Type").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Type").combobox("clear");
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
        <table id="table1">
            <tr>
                <td class="lbl" style="text-align: center; width: 80px" colspan="1"><span id="NameLabel">名称</span></td>
                <td colspan="1">
                    <input class="easyui-textbox easyui-validatebox" id="Name" name="Name"
                        data-options="validType:'length[1,50]',required:true" style="width: 180px" />
                </td>
                <td class="lbl" style="text-align: center; width: 80px"><span id="CodeLabel">简码</span></td>
                <td>
                    <input class="easyui-textbox" id="Code" name="Code"
                        data-options="validType:'length[1,50]'" style="width: 180px" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="text-align: center; width: 80px">类型</td>
                <td>
                    <input class="easyui-combobox" id="Type" name="Type" 
                            data-options="valueField:'value',textField:'text',required:true,disabled:true," style="width: 178px"/>
                </td>
            </tr>
            <tr>
                <td class="lbl" colspan="1" style="text-align: center; width: 80px">描述</td>
                <td colspan="4">
                    <input class="easyui-textbox" id="Summary" name="Summary"
                        data-options="validType:'length[1,500]',multiline:true,tipPosition:'bottom'" style="width: 445px; height: 50px" />
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

