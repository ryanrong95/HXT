<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Commissions.Add" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/radio.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>

    <script>
        $(function () {
            $("#span_none").hide();
            $('#rb_Rebate').radiobutton({
                labelPosition: 'after',
                checked: true,
                value: 1,
                label: '返点',
                onChange: function (checked) {
                    if (checked) {
                        $("#span_none").hide();
                        $("#SpanMethord").show();
                    }
                }
            });
            $('#rb_Discount').radiobutton({
                labelPosition: 'after',
                checked: false,
                value: 2,
                label: '折扣',
                onChange: function (checked) {
                    if (checked) {
                        $("#span_none").show();
                        $("#SpanMethord").hide();
                    }
                }
            });
           
             $("#cbbCurrency").fixedCombobx({
                required: true,
                type: "Currency",
                value: '<%=(int)Currency.CNY%>'
             })
            $("#cbbMethord").fixedCombobx({
                required: true,
                type: "CommissionMethod",
                value: '<%=(int)CommissionMethod.Deduction%>'
            })
           
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tt" class="easyui-panel" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>类型</td>
                <td>
                    <input id="rb_Rebate" class="easyui-radiobutton" name="Type">
                    <input id="rb_Discount" class="easyui-radiobutton" name="Type">
                </td>
            </tr>
            <tr>
                <td>方式</td>
                <td>
                    <span id="span_none">
                        <input class="easyui-textbox" data-options="value:'无',readonly:true"  style="width: 200px;" />
                    </span>
                    <span id="SpanMethord">
                        <input id="cbbMethord" name="Methord" class="easyui-combobox" style="width: 200px;" />
                    </span>
                </td>

            </tr>
            <tr>
                <td>率值</td>
                <td>
                    <input id="Radio" name="Radio" class="easyui-numberbox" data-options="min:0,max:1,precision:2,required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td>币种</td>
                <td>
                    <input id="cbbCurrency" class="easyui-combobox" name="Currency" style="width: 200px" />
                </td>

            </tr>
            <tr>
                <td>金额最小值</td>
                <td>
                    <input id="cbbMsp" class="easyui-numberbox" name="Msp" data-options="min:0, precision:2,required:true" style="width: 200px" />
                </td>
            </tr>

        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
