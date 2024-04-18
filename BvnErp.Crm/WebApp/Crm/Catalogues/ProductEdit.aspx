<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductEdit.aspx.cs" Inherits="WebApp.Crm.Catalogues.ProductEdit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>产品信息编辑</title>
    <UC:EasyUI runat="server" />
    <script type="text/javascript">
        var Standard = eval('(<%=this.Model.Standard%>)');
        var ObjectType = eval('(<%=this.Model.ObjectType%>)');
        var Currency = eval('(<%=this.Model.Currency%>)');
        var Sended = eval('(<%=this.Model.Sended%>)');
        var DeclareProducts = eval('(<%=this.Model.DeclareProducts%>)');
        var Status = eval('(<%=this.Model.Status%>)')

        $(function () {
            
            $("#StandardID").combobox("setValue", DeclareProducts["StandardID"]);
            $("#ObjectType").combobox("setValue", DeclareProducts["ObjectType"]);
            $("#Currency").combobox("setValue", DeclareProducts["Currency"]);
            $("#Amount").numberbox("setValue", DeclareProducts["Amount"]);
            $("#UnitPrice").textbox("setValue", DeclareProducts["UnitPrice"]);
            $("#Delivery").textbox("setValue", DeclareProducts["Delivery"]);
            $("#Count").numberbox("setValue", DeclareProducts["Count"]);

            //校验输入框内容
            $("#StandardID").combobox("textbox").bind("blur", function () {
                var value = $("#StandardID").combobox("getValue");
                var data = $("#StandardID").combobox("getData");
                var valuefiled = $("#StandardID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#StandardID").combobox("clear");
                }
            });
            $("#ObjectType").combobox("textbox").bind("blur", function () {
                var value = $("#ObjectType").combobox("getValue");
                var data = $("#ObjectType").combobox("getData");
                var valuefiled = $("#ObjectType").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ObjectType").combobox("clear");
                }
            });
            $("#Currency").combobox("textbox").bind("blur", function () {
                var value = $("#Currency").combobox("getValue");
                var data = $("#Currency").combobox("getData");
                var valuefiled = $("#Currency").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Currency").combobox("clear");
                }
            });
            $("#Status").combobox("textbox").bind("blur", function () {
                var value = $("#Status").combobox("getValue");
                var data = $("#Status").combobox("getData");
                var valuefiled = $("#Status").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Status").combobox("clear");
                }
            });
        });

        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server" >
            <table id="table1" cellpadding="0" cellspacing="0">
                 <tr>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                </tr>
                <tr style="height:30px">
                    <td class="lbl">产品型号</td>
                    <td>
                        <input class="easyui-combobox" id="StandardID" name="StandardID"
                             data-options="valueField:'ID',textField:'Name',required:true,tipPosition:'right',data: Standard," style="width: 95%"/>
                    </td>
                    <td class="lbl">谈判类型</td>
                    <td>
                        <input class="easyui-combobox" id="ObjectType" name="ObjectType"
                            data-options="valueField:'value',textField:'text',required:true,data: ObjectType," style="width: 95%" />
                    </td>
                    <td class="lbl">币种</td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency"
                            data-options="valueField:'value',textField:'text',required:true,data: Currency," style="width: 95%" />
                    </td>
                </tr>
                <tr style="height:30px">
                    <td class="lbl">数量</td>
                    <td>
                        <input class="easyui-numberbox" id="Amount" name="Amount" 
                            data-options="min:0,required:true,validType:'length[1,10]',tipPosition:'bottom'" style="width: 95%" />
                    </td>
                    <td class="lbl">单价</td>
                    <td>
                        <input class="easyui-numberbox" id="UnitPrice" name="UnitPrice"
                            data-options="min:0,precision:2,required:true,validType:'length[1,10]'" style="width: 95%" />
                    </td>
                    <td class="lbl">状态</td>
                    <td>
                        <input class="easyui-combobox" id="Status" name="Status" 
                            data-options="valueField:'value',textField:'text',required:true,data: Status," style="width: 95%"/>
                    </td>
                </tr>
                <tr style="height:30px">
                    <td class="lbl">送样数量</td>
                    <td>
                        <input class="easyui-numberbox" id="Count" name="Count" data-options="min:0,required:true,validType:'length[1,10]'" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">交货地址</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Delivery" name="Delivery" 
                            data-options="required:true,validType:'length[1,100]',tipPosition:'bottom'"  style="width: 98%"/>
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
