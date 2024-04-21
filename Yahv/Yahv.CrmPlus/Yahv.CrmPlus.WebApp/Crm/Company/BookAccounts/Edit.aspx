<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.BookAccounts.Edit" %>
<%@ import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
             $('#BookAccountType').fixedCombobx({
                 type: "BookAccountType",
                 value:<%= (int)BookAccountType.Payee%>
            });

            //$('#BookAccountType').combobox({
            //    data: model.BookAccountType,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto',
            //    multiple: false,
            //   onLoadSuccess: function (data) {
            //        if (data.length > 0) {
            //            $(this).combobox('select', '1');
            //        }
            //    }
            //});
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">

            <tr>
                <td style="width: 120px;">账户类型：</td>
                <td>
                    <input id="BookAccountType" name="BookAccountType" class="eeasyui-combobox" style="width: 350px;" data-options="required:true," /></td>
            </tr>
            <%-- <tr>
                <td style="width: 120px;">支付方式：</td>
                <td>
                    <input id="BookAccountMethord" name="BookAccountMethord" class="easyui-textbox" style="width: 350px;" data-options="required:true," /></td>
         </tr>--%>

            <tr>
                <td>开户行：</td>
                <td>
                    <input id="Bank" name="Bank" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,1500]'" />
                </td>
            </tr>
            <tr>
                <td>银行地址</td>
                <td>
                    <input id="BankAddress" name="BankAddress" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" /></td>
            </tr>

            <tr>
                <td>账号</td>
                <td>
                    <input id="Account" name="Account" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" />
                </td>
            </tr>


            <tr>
                <td>银行代码</td>
                <td>
                    <input id="SwiftCode" name="SwiftCode" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'" /></td>
            </tr>
            <tr>
                <td>行号</td>
                <td>
                    <input id="BankCode" name="BankCode" class="easyui-textbox" style="width: 350px;" data-options="required:false" /></td>
            </tr>
            <tr>
                <td>中转银行</td>
                <td>
                    <input id="Transfer" name="Transfer" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'" /></td>
            </tr>
            <tr>
                <td>是否为个人</td>
                <td>
                    <input id="IsP"   class="easyui-checkbox" name="IsP" /><label for="IsP" style="margin-right: 30px">是</label>
                </td>
            </tr>



        </table>
        <div style="text-align: center; padding: 5px">
            <asp:button id="btnSubmit" runat="server" text="保存" style="display: none;" onclick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
