<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Invoice.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="http://fixed2.b1b.com/My/Scripts/area.data.js"></script>
    <script src="http://fixed2.b1b.com/My/Scripts/areacombo.js"></script>
    <script type="text/javascript">        
        var InvoiceType = eval('(<%=this.Model.InvoiceType%>)');
        var ClientName = eval('(<%=this.Model.ClientName%>)');
        var Invoice = eval('(<%=this.Model.Invoice%>)');
        var Contact = eval('(<%=this.Model.Contact%>)');
        var Consignee = eval('(<%=this.Model.Consignee%>)');

        //页面加载时
        $(function () {

            $("#InvoiceTypes").combobox({
                data: InvoiceType,
            });

            $("#ContactID").combobox({
                data: Contact,
            });

            $("#Consignee").combobox({
                data: Consignee,
                onSelect: function (record) {
                    $.post('?action=getPhoneAddress', { ID: record.ID }, function (data) {
                        if (data != "noResult") {
                            var values = data.split(',');
                            $("#ContactName").textbox("setValue", values[0]);
                            $("#ContactPhone").textbox("setValue", values[1]);
                            $("#ZipCode").textbox("setValue", values[2]);
                        }
                    })
                },
            });

            $("#CompanyName").textbox("setValue", ClientName);
            $("#ContactPhone").textbox("readonly", true);
            $("#ZipCode").textbox("readonly", true);
            $("#ContactName").textbox("readonly", true);

            if (Invoice != null) {
                $("#ContactID").combobox("setValue", Invoice["ContactID"]);
                $("#InvoiceTypes").combobox("setValue", Invoice["InvoiceTypes"]);
                $("#CompanyID").combobox("setValue", Invoice["CompanyID"]);
                $("#Consignee").combobox("setValue", Invoice["ConsigneeID"])
                $("#CompanyCode").textbox("setValue", Invoice["CompanyCode"]);
                $("#Address").textbox("setValue", Invoice["Address"]);
                $("#Phone").textbox("setValue", Invoice["Phone"]);
                $("#BankName").textbox("setValue", Invoice["BankName"]);
                $("#Account").textbox("setValue", Invoice["Account"]);
                $("#ZipCode").textbox("setValue", Invoice["ZipCode"]);
                $("#ContactName").textbox("setValue", Invoice["ContactName"]);
                $("#ContactPhone").textbox("setValue", Invoice["ContactPhone"]);

            }

            $("#InvoiceTypes").combobox("textbox").bind("blur", function () {
                var value = $("#InvoiceTypes").combobox("getValue");
                var data = $("#InvoiceTypes").combobox("getData");
                var valuefiled = $("#InvoiceTypes").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#InvoiceTypes").combobox("clear");
                }
            });
            $("#Consignee").combobox("textbox").bind("blur", function () {
                var value = $("#Consignee").combobox("getValue");
                var data = $("#Consignee").combobox("getData");
                var valuefiled = $("#Consignee").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Consignee").combobox("clear");
                }
            });
        });
    </script>
    <script>
        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server" method="post">
            <table id="table1">
                <tr>
                    <th style="width: 100px"></th>
                    <th style="width: 200px"></th>
                    <th></th>
                </tr>
                <tr>
                    <td class="lbl">发票类型</td>
                    <td>
                        <input class="easyui-combobox" id="InvoiceTypes" name="InvoiceTypes"
                            data-options="valueField:'value',textField:'text',required:true," style="width: 195px" />
                    </td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="lbl">公司名称</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" name="CompanyName"
                            data-options="required:true,tipPosition:'bottom',editable:false" style="width: 195px" />
                    </td>
                    <td>
                        <input type="hidden" id="CompanyID2" name="CompanyID2" />
                    </td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="lbl">税号</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyCode" name="CompanyCode"
                            data-options="required:true,validType:'length[1,100]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="lbl">地址</td>
                    <td>
                        <input class="easyui-textbox" id="Address" name="Address"
                            data-options="required:true,validType:'length[1,100]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="lbl">电话</td>
                    <td>
                        <input class="easyui-textbox" id="Phone" name="Phone"
                            data-options="required:true,validType:'length[1,20]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="lbl">开户行</td>
                    <td>
                        <input class="easyui-textbox" id="BankName" name="BankName"
                            data-options="required:true,validType:'length[1,100]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
                <tr></tr>
                <tr>
                    <td class="lbl">账号</td>
                    <td>
                        <input class="easyui-textbox" id="Account" name="Account"
                            data-options="required:true,validType:'length[1,100]',tipPosition:'bottom'" style="width: 195px" />
                    </td>
                </tr>
                <tr></tr>
            </table>
            <div id="consigneeafter">
                <table>
                    <tr>
                        <th style="width: 100px"></th>
                        <th style="width: 200px"></th>
                        <th></th>
                    </tr>
                    <tr>
                        <td class="lbl">邮寄地址</td>
                        <td>
                            <input class="easyui-combobox" id="Consignee" name="Consignee"
                                data-options="valueField:'ID',textField:'text',required:true," style="width: 195px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">邮编</td>
                        <td>
                            <input class="easyui-textbox" id="ZipCode" name="ZipCode"
                                data-options="validType:'length[1,10]',tipPosition:'bottom'" style="width: 195px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">联系人</td>
                        <td>
                            <input class="easyui-textbox" id="ContactName" name="ContactName"
                                data-options="validType:'length[1,10]',tipPosition:'bottom'" style="width: 195px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">收件人电话</td>
                        <td>
                            <input class="easyui-textbox" id="ContactPhone" name="ContactPhone"
                                data-options="validType:'length[1,12]',tipPosition:'bottom'" style="width: 195px" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divSave" style="text-align: center">
                <asp:Button runat="server" ID="btnSave" Text="保存" OnClick="btnSave_Click" OnClientClick="return Valid();" />
                <asp:Button runat="server" ID="Button1" Text="取消" OnClientClick="closeWin()" />
            </div>
        </form>
    </div>
</body>
</html>
