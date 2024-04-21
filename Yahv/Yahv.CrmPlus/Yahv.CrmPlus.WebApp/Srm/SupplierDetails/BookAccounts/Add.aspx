<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BookAccounts.Add" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#cbbCurrency").fixedCombobx({
                required: false,
                type: "Currency",
                value: '<%=(int)Currency.CNY%>'
            })
            //onChange
            $('#cbbBookAccountMethord').fixedCombobx({
                required: true,
                type: "BookAccountMethord",
                value: '<%=(int)BookAccountMethord.Bank%>'
            })
            $("#cbbBookAccountMethord").combobox({
                onChange: function (newValue, oldValue) {
                    if (newValue == '<%=(int)BookAccountMethord.Bank%>') {
                    $(".bank").show();
                    $("#textBank").textbox({ required: true })
                    $("#textBankCode").textbox({ required: true })
                    $("#textBankAddress").textbox({ required: true })
                }
                else {
                    $(".bank").hide();
                    $("#textBank").textbox({ required: false })
                    $("#textBankCode").textbox({ required: false })
                    $("#textBankAddress").textbox({ required: false })
                }
            }
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>是否个人</td>
                <td>
                    <input id="IsPersonal" class="easyui-checkbox" name="IsPersonal" /><label for="IsPersonal" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td style="width: 120px;">类型：</td>
                <td>
                    <input id="cbbBookAccountMethord" name="BookAccountMethord" style="width: 350px;" /></td>
            </tr>
            <tr>
                <td>账号</td>
                <td>
                    <input id="textAccount" name="Account" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" />
                </td>
            </tr>
            <tr class="bank">
                <td>开户行：</td>
                <td>
                    <input id="textBank" name="Bank" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,150]'" />
                </td>
            </tr>
            <tr class="bank">
                <td>行号</td>
                <td>
                    <input id="textBankCode" name="BankCode" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,25]'" /></td>
            </tr>

            <tr class="bank">
                <td>币种</td>
                <td>
                    <input id="cbbCurrency" name="Currency" style="width: 350px;" /></td>
            </tr>
            <tr class="bank">
                <td>SwiftCode</td>
                <td>
                    <input id="textSwiftCode" name="SwiftCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,25]'" /></td>
            </tr>
            <tr class="bank">
                <td>中转银行</td>
                <td>
                    <input id="textTransfer" name="Transfer" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'" /></td>
            </tr>
            <tr class="bank">
                <td>银行地址</td>
                <td>
                    <input id="textBankAddress" name="BankAddress" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,200]'" /></td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
