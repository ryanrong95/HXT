<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Consignees.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://fixed2.b1b.com/My/Scripts/area.data.js"></script>
    <script src="http://fixed2.b1b.com/My/Scripts/areacombo.js"></script>
    <script type="text/javascript">
        var Contact = eval('(<%=this.Model.Contact%>)');
        var Consignee = eval('(<%=this.Model.Consignee%>)');

        $(function () {

            $("#ContactID").combobox({
                data: Contact
            });

            $("#ContactID").combobox({
                onSelect: function (record) {
                    $.post('?action=getPhoneAddress', { ID: record.ID }, function (data) {
                        if (data != "noResult") {
                            $("#ContactPhone").textbox("setValue", data);
                        }
                    })
                },
            });

            $("#ContactPhone").textbox("readonly", true);


            if (Consignee != null) {
                $("#ContactID").combobox("setValue", Consignee["ContactID"]);
                $("#ContactPhone").textbox("setValue", Consignee["ContactPhone"]);
                $("#ZipCode").numberbox("setValue", Consignee["Zipcode"]);
                $("#Address").area('setValue', Consignee["Address"]);
                $("#CompanyID").textbox("setValue", Consignee["CompanyID"]);
            }

            $("#btnAdd").on('click', function () {
                $("#tradd").hide();
                $("#trback").show();
                $("#ContactPhone").textbox("readonly", false);
                $("#ContactID").combobox("setValue", "");
                $("#ContactPhone").textbox("setValue", "");
                $("#ContactName").textbox("setValue", "");
                $('#ContactName').textbox({ required: true });
                $('#ContactID').combobox({ required: false });
            });

            $("#btnBack").on('click', function () {
                $("#tradd").show();
                $("#trback").hide();
                $("#ContactPhone").textbox("readonly", true);
                $("#ContactID").combobox("setValue", "");
                $("#ContactName").textbox("setValue", "");
                $("#ContactPhone").textbox("setValue", "");
                $('#ContactName').textbox({ required: false });
                $('#ContactID').combobox({ required: true });
                if (Consignee != null) {
                    $("#ContactID").combobox("setValue", Consignee["ContactID"]);
                    $("#ContactPhone").textbox("setValue", Consignee["ContactPhone"]);
                }

            });

            $("#ContactID").combobox("textbox").bind("blur", function () {
                var value = $("#ContactID").combobox("getValue");
                var data = $("#ContactID").combobox("getData");
                var valuefiled = $("#ContactID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ContactID").combobox("clear");
                }
            });
        });
    </script>
    <script>
        function closeWin() {
            $.myWindow.close();
        }

        function ConsigneeValid() {

            var address = $("#Address").area("getValue");
            if (address == "" || address == undefined) {
                $.messager.alert('提示', '请输入地址！');
                return false;
            }

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
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server" method="post">
            <table id="table1"  style="border-collapse: separate; border-spacing: 0px 10px; margin-top: 0px">
                <tr>
                    <th style="width: 100px"></th>
                    <th style="width: 700px"></th>
                </tr>
                <tr>
                    <td class="lbl">公司名称</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyID" name="CompanyID"
                            data-options="validType:'length[1,50]'" style="width: 195px" />
                    </td>
                </tr>
                <tr id="tradd">
                    <td class="lbl">联系人</td>
                    <td>
                        <input class="easyui-combobox" id="ContactID" name="ContactID"
                            data-options="valueField:'ID',textField:'Name',required:true," style="width: 195px" />
                        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'">新增</a>
                    </td>
                </tr>
                <tr id="trback" style="display:none">
                    <td class="lbl">联系人</td>
                    <td>
                        <input class="easyui-textbox" id="ContactName" name="ContactName"
                            data-options="validType:'length[1,10]',tipPosition:'bottom'" style="width: 195px" />
                        <a id="btnBack" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'">返回</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">手机号码</td>
                    <td>
                        <input class="easyui-numberbox" id="ContactPhone" name="ContactPhone"
                            data-options="required:true,validType:'length[1,11]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">地址</td>
                    <td>
                        <div class="easyui-area" data-options="country:'中国',required:true,validType:'length[1,50]'," id="Address" name="Address"></div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">邮编</td>
                    <td>
                        <input class="easyui-numberbox" id="ZipCode" name="ZipCode"
                            data-options="required:true,validType:'length[1,10]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
            </table>
            <div id="divSave" style="text-align: center">
                <asp:Button runat="server" ID="btnSave" Text="保存" OnClick="btnSave_Click" OnClientClick="return ConsigneeValid();" />
                <asp:Button runat="server" ID="Button1" Text="取消" OnClientClick="closeWin()" />
            </div>
        </form>
    </div>
</body>
</html>
