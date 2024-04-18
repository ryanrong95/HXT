<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="AddFee.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.AddFee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");
        $(function () {
            $('#Currency').combobox({
                data: model.currencyData,
                editable: false,
                required: true,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            //提交
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Subject', $("#Subject").textbox("getValue"));
                data.append('Currency', $("#Currency").combobox("getValue"));
                data.append('Amount', $("#Amount").numberbox("getValue"));
                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            });
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            Init();
        });
    </script>
    <script>
        function Init() {
            if (model.orderData != null) {
                $("#PayerCompany").textbox("setValue", model.orderData.payerName);
                $("#PayeeCompany").textbox("setValue", model.orderData.payeeName);
            }
        }
    </script>
    <style>
        .lbl {
            width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">付款公司：</td>
                    <td>
                        <input id="PayerCompany" class="easyui-textbox" data-options="disabled:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款公司：</td>
                    <td>
                        <input id="PayeeCompany" class="easyui-textbox" data-options="disabled:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">科目：</td>
                    <td>
                        <input id="Subject" class="easyui-textbox" data-options="required:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">币种：</td>
                    <td>
                        <input id="Currency" class="easyui-combobox" data-options="required:true,editable:false" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">金额：</td>
                    <td>
                        <input id="Amount" class="easyui-numberbox" data-options="required:true,min:0,precision:2" style="width: 250px;" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>

