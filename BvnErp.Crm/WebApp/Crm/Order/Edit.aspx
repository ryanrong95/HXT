<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Order.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var CurrencyData = eval('(<%=this.Model.CurrencyData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var BeneficiaryData = eval('(<%=this.Model.BeneficiaryData%>)');
        var ConsigneeData = eval('(<%=this.Model.ConsigneeData%>)');
        var AllData = eval('(<%=this.Model.AllData%>)');
        //页面加载时
        $(function () {
            //下拉框加载数据
            $("#Currency").combobox({
                data: CurrencyData
            });
            $("#ClientID").combobox({
                data: ClientData
            });
            $("#BeneficiaryID").combobox({
                data: GetBenefit(BeneficiaryData)
            });
            $("#ConsigneeID").combobox({
                data: ConsigneeData
            });
            //初始化赋值
            if (AllData != null) {
                $("#Address").textbox("setValue", escape2Html(AllData["Address"]));
                $("#DeliveryAddress").textbox("setValue", escape2Html(AllData["DeliveryAddress"]));
                $("#Currency").combobox("setValue", AllData["Currency"]);
                $("#ClientID").combobox("setValue", AllData["ClientID"]);
                $("#BeneficiaryID").combobox("setValue", AllData["BeneficiaryID"]);
                $("#ConsigneeID").combobox("setValue", AllData["ConsigneeID"]);
            }
        });
        function GetBenefit(arg) {
            var arr = new Array();
            for (var i = 0; i < arg.length; i++) {
                var json = {
                    text: arg[i].bank + "/" + arg[i].bankcode,
                    value: arg[i].value
                }
                arr[i] = json;
            }
            return arr;
        }
        function Close() {
            $.myWindow.close();
        }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 320px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <table id="table1" style="margin: 0 auto; height: 50px">
            <tr>
                <td class="lbl" style="text-align: center; width: 80px" colspan="1">币种</td>
                <td colspan="1">
                    <input class="easyui-combobox" id="Currency" data-options="valueField:'value',textField:'text',required:true, panelMaxHeight:'150px'" name="Currency" style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 80px">收益人</td>
                <td>
                    <input class="easyui-combobox" id="BeneficiaryID" data-options="valueField:'value',textField:'text',required:true, panelMaxHeight:'150px'" name="BeneficiaryID" style="width: 300px" />
                </td>
            </tr>
            <tr>
                <td class="lbl" style="text-align: center; width: 80px" colspan="1">客户</td>
                <td colspan="1">
                    <input class="easyui-combobox" id="ClientID" data-options="valueField:'value',textField:'text',required:true, panelMaxHeight:'150px'" name="ClientID" style="width: 150px" />
                </td>
                <td class="lbl" style="text-align: center; width: 80px">收货人</td>
                <td>
                    <input class="easyui-combobox" id="ConsigneeID" data-options="valueField:'value',textField:'text',required:true, panelMaxHeight:'150px'" name="ConsigneeID" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td class="lbl" colspan="1" style="text-align: center; width: 80px">交货地址</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="DeliveryAddress" name="DeliveryAddress"
                        data-options="validType:'length[1,500]'" style="width: 500px" />
                </td>
            </tr>
            <tr>
                <td class="lbl" colspan="1" style="text-align: center; width: 80px">收货地址</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Address" name="Address"
                        data-options="validType:'length[1,500]',required:true" style="width: 500px" />
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
