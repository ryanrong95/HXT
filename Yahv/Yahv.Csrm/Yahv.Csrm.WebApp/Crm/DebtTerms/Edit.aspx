<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.DebtTerms.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Data && model.Data.ExchangeType != 0) {
                $("form").form("load", model.Data);
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>结算方式</td>
                    <td>
                        <input id="SettlementType" name="SettlementType" class="easyui-combobox" style="width: 200px;"
                            data-options="required:true,limitToList:true,valueField:'value',textField:'text',data: model.SettlementType," />
                    </td>
                    <td>汇率类型</td>
                    <td>
                        <input id="ExchangeType" name="ExchangeType" class="easyui-combobox" style="width: 200px;"
                            data-options="required:true,limitToList:true,valueField:'value',textField:'text',data: model.ExchangeType," />
                    </td>
                </tr>
                <tr>
                    <td>月数</td>
                    <td>
                        <input id="Months" name="Months" class="easyui-numberbox" data-options="min:1" style="width: 200px;" />
                    <td>还款日</td>
                    <td>
                        <input id="Days" name="Days" class="easyui-numberbox" data-options="min:1,max:31" style="width: 200px;" />

                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSave" runat="server" Text="保存" Style="display: none;" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
    <input type="hidden" runat="server" id="hScussMsg" value="保存成功" />
</asp:Content>
