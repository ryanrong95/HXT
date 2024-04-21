<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.BookAccounts.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {


<%--           $("#cbbBookAccountMethord").fixedCombobx({
                type: "BookAccountMethord",
                required: true,
                value: '<%=(int)Yahv.Underly.BookAccountMethord.Bank%>',

                 onChange: function (newValue, oldValue) {
                    if (newValue == '<%=(int)Yahv.Underly.BookAccountMethord.Bank%>') {
                        $(".bank").show();
                        $("#Bank").textbox({ required: true })
                        $("#BankCode").textbox({ required: true })
                        $("#BankAddress").textbox({ required: true })
                    }
                    else {
                        $(".bank").hide();
                        $("#Bank").textbox({ required: false })
                        $("#BankCode").textbox({ required: false })
                        $("#BankAddress").textbox({ required: false })
                    }
                }


            });--%>
             $('#cbbCurrency').fixedCombobx({
                type: "Currency",
                value: '<%=(int)Yahv.Underly.Currency.CNY%>',
                required: false,
            })

            $('#cbbBookAccountMethord').combobox({
                data: model.BookAccountMethord,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '<%=(int)Yahv.Underly.BookAccountMethord.Bank%>');
                   }
               },
               onChange: function (newValue, oldValue) {
                   if (newValue == '<%=(int)Yahv.Underly.BookAccountMethord.Bank%>') {
                        $(".bank").show();
                        $("#Bank").textbox({ required: true })
                        $("#BankCode").textbox({ required: true })
                        $("#SwiftCode").textbox({ required: true })
                        $("#BankAddress").textbox({ required: true })
                    }
                    else {
                        // $(".bank").hide();
                        $("#Bank").textbox({ required: false })
                        $("#BankCode").textbox({ required: false })
                        $("#SwiftCode").textbox({ required: false })
                        $("#BankAddress").textbox({ required: false })
                    }
                }
            });
           
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td style="width: 120px;">类型：</td>
            <td>
                <%--<input id="cbbBookAccountMethord" name="BookAccountMethord" style="width: 350px;"  />--%>
                <input id="cbbBookAccountMethord" name="BookAccountMethord" class="easyui-combobox" style="width: 350px;" data-options="required:true" />
            </td>
        </tr>
        <tr>
            <td>是否为个人</td>
            <td>
                <input id="IsP" class="easyui-checkbox" name="IsP" /><label for="IsP" style="margin-right: 30px">是</label>
            </td>
        </tr>
        <tr>
            <td>账号</td>
            <td>
                <input id="Account" name="Account" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" />
            </td>
        </tr>

        <tr class="bank">
            <td>开户行：</td>
            <td>
                <input id="Bank" name="Bank" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,1500]'" />
            </td>
        </tr>
        <tr class="bank">
            <td>银行地址</td>
            <td>
                <input id="BankAddress" name="BankAddress" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" /></td>
        </tr>

        <tr class="bank">
            <td>银行代码</td>
            <td>
                <input id="SwiftCode" name="SwiftCode" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" /></td>
        </tr>
        <tr class="bank">
            <td>行号</td>
            <td>
                <input id="BankCode" name="BankCode" class="easyui-textbox" style="width: 350px;" data-options="required:false" /></td>
        </tr>
        <tr class="bank">
            <td>中转银行</td>
            <td>
                <input id="Transfer" name="Transfer" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'" /></td>
        </tr>
        <tr class="bank">
            <td>币种</td>
            <td>
                <input id="cbbCurrency" name="Currency" style="width: 350px;" /></td>
        </tr>

    </table>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>
