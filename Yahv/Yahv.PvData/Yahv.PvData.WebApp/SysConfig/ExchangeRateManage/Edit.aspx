<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ExchangeRateManage.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#from').combobox({
                data: model.ExchangeCurrency,
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
                editable: false,
            });

            $('#to').combobox({
                data: model.ExchangeCurrency,
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
                editable: false,
            });

            $('#district').combobox({
                data: model.ExchangeDistrict,
                valueField: "value",
                textField: "text",
                panelHeight: 'auto',
                editable: false,
            });

            $('#district').combobox('select', model.ExchangeDistrict[0].value);

            if (model.ExchangeRate) {
                $('#district').combobox('setValue', model.ExchangeRate.District);
                $('#from').combobox('setValue', model.ExchangeRate.From);
                $('#to').combobox('setValue', model.ExchangeRate.To);
                $('#rate').textbox('setValue', model.ExchangeRate.Value);
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label"> 汇率类型：</td>
                <td>
                    <input id="type" name="type" value="固定汇率" class="easyui-textbox" disabled/>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">区域: </td>
                <td>
                    <select id="district" name="district" class="easyui-combobox"></select>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 币种: </td>
                <td>
                    <select id="from" name="from" class="easyui-combobox"></select>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 兑换币种: </td>
                <td>
                    <select id="to" name="to" class="easyui-combobox"></select>
                </td>
            </tr>
            <tr>
                <td class="liebiao-label"> 汇率:</td>
                <td>
                    <input id="rate" name="rate" class="easyui-numberbox" data-options="prompt:'汇率', min:0.00001,precision:5" required/>
                </td>
            </tr>            
        </table>
        <div style="text-align:center; padding:5px">
            <%--<asp:Button ID="btnSubmit" runat="server" Visible="false" Text="保存" OnClick="btnSubmit_Click"/> --%>
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/> 

        </div>
    </div>
</asp:Content>
