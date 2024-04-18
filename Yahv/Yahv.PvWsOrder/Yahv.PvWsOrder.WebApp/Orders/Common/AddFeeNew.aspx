<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="AddFeeNew.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.AddFeeNew" %>

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
            $('#Subject').combobox({
                data: model.feeSubjectData,
                editable: false,
                required: true,
                valueField: 'value',
                textField: 'label',
                panelHeight: 'auto', //自适应
                onChange: function () {
                    $(".subject2").css('display', 'none')
                    $(".subject3").css('display', 'none')
                    $(".subject4").css('display', 'none')
                    $("#Subject2").combobox({ required: false });
                    $("#Subject3").combobox({ required: false });
                    $("#Subject4").combobox({ required: false });
                    SelectSubject();
                }
            });
            $('#Subject2').combobox({
                editable: false,
                required: true,
                valueField: 'value',
                textField: 'label',
                panelHeight: 'auto', //自适应
                onChange: function () {
                    $(".subject3").css('display', 'none')
                    $(".subject4").css('display', 'none')
                    $("#Subject3").combobox({ required: false });
                    $("#Subject4").combobox({ required: false });
                    SelectSubject2();
                }
            });
            $('#Subject3').combobox({
                editable: false,
                required: true,
                valueField: 'value',
                textField: 'label',
                panelHeight: 'auto', //自适应
                onChange: function () {
                    $(".subject4").css('display', 'none')
                    $("#Subject4").combobox({ required: false });
                    SelectSubject3();
                }
            });
            $('#Subject4').combobox({
                editable: false,
                required: true,
                valueField: 'value',
                textField: 'label',
                panelHeight: 'auto', //自适应
            });
            $("#Quantity").numberbox({
                onChange: function () {
                    var unitPrice = $("#UnitPrice").numberbox("getValue");
                    var quantity = $("#Quantity").numberbox("getValue");
                    var price = unitPrice * quantity;
                    $("#Price").numberbox("setValue", price);
                    $("#Amount").numberbox("setValue", price);
                }
            })
            //提交
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var amount = $("#Amount").numberbox("getValue");
                if (Number(amount) <= 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "应收金额必须大于零", type: "error" });
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Subject', $("#Subject").textbox("getValue"));
                data.append('Currency', $("#Currency").combobox("getValue"));
                data.append('Price', $("#Price").numberbox("getValue"));
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
        function SelectSubject() {
            var Subject = $("#Subject").combobox('getValue');
            if (CheckIsNullOrEmpty(Subject)) {
                $.post('?action=SelectSubject', { Subject: Subject }, function (result) {
                    var rel = JSON.parse(result);
                    if (rel.success) {
                        if (rel.data.children != null && rel.data.children.length > 0) {
                            $('#Subject2').combobox({
                                data: eval(rel.data.children)
                            });
                            $(".subject2").css('display', 'table-row')
                            $("#Subject2").combobox({ required: true });
                        }
                        else {
                            $(".subject2").css('display', 'none')
                            InitFee(rel.data);
                        }
                    }
                    else {
                        $.messager.alert('提示', rel.data);
                    }
                })
            }
        }
        function SelectSubject2() {
            var Subject = $("#Subject").combobox('getValue');
            var Subject2 = $("#Subject2").combobox('getValue');
            if (CheckIsNullOrEmpty(Subject) && CheckIsNullOrEmpty(Subject2)) {
                $.post('?action=SelectSubject2', { Subject: Subject, Subject2: Subject2 }, function (result) {
                    var rel = JSON.parse(result);
                    if (rel.success) {
                        if (rel.data.children != null && rel.data.children.length > 0) {
                            $('#Subject3').combobox({
                                data: eval(rel.data.children)
                            });
                            $(".subject3").css('display', 'table-row')
                            $("#Subject3").combobox({ required: true, });
                        }
                        else {
                            $(".subject3").css('display', 'none')
                            InitFee(rel.data);
                        }
                    }
                    else {
                        $.messager.alert('提示', rel.data);
                    }
                })
            }
        }
        function SelectSubject3() {
            var Subject = $("#Subject").combobox('getValue');
            var Subject2 = $("#Subject2").combobox('getValue');
            var Subject3 = $("#Subject3").combobox('getValue');
            if (CheckIsNullOrEmpty(Subject) && CheckIsNullOrEmpty(Subject2) && CheckIsNullOrEmpty(Subject3)) {
                $.post('?action=SelectSubject3', { Subject: Subject, Subject2: Subject2, Subject3: Subject3 }, function (result) {
                    var rel = JSON.parse(result);
                    if (rel.success) {
                        if (rel.data.children != null && rel.data.children.length > 0) {
                            $('#Subject4').combobox({
                                data: eval(rel.data.children)
                            });
                            $(".subject4").css('display', 'table-row')
                            $("#Subject4").combobox({ required: true });
                        }
                        else {
                            $(".subject4").css('display', 'none')
                            InitFee(rel.data);
                        }
                    }
                    else {
                        $.messager.alert('提示', rel.data);
                    }
                })
            }
        }
        function InitFee(data) {
            $("#Currency").combobox("setValue", data.currency)
            if (data.prices != null) {
                $("#UnitPrice").numberbox("setValue", data.prices);
                $("#Quantity").numberbox("setValue", 1);
                $("#Price").numberbox("setValue", data.prices);
                $("#Amount").numberbox("setValue", data.prices);
                if (data.Isquantity) {
                    $("#Quantity").numberbox({ required: true, disabled: false, editable: true });
                }
                else {
                    $("#Quantity").numberbox({ required: false, disabled: true });
                }

                $(".suggest").css('display', 'table-row')
            }
            else {
                $("#UnitPrice").numberbox({ required: false, disabled: true });
                $("#Quantity").numberbox({ required: false, disabled: true });
                $("#Price").numberbox({ required: false, disabled: true });

                $("#UnitPrice").numberbox("setValue", 0);
                $("#Quantity").numberbox("setValue", 0);
                $("#Price").numberbox("setValue", 0);
                $("#Amount").numberbox("setValue", 0);

                $(".suggest").css('display', 'none')
            }
        }
    </script>
    <style>
        .lbl {
            width: 120px;
        }

        .subject2 {
            display: none;
        }

        .subject3 {
            display: none;
        }

        .subject4 {
            display: none;
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
                        <input id="Subject" class="easyui-combobox" data-options="required:true,editable:false" style="width: 250px;" />
                    </td>
                </tr>
                <tr class="subject2">
                    <td class="lbl">二级科目：</td>
                    <td>
                        <input id="Subject2" class="easyui-combobox" data-options="required:false,editable:false" style="width: 250px;" />
                    </td>
                </tr>
                <tr class="subject3">
                    <td class="lbl">三级科目：</td>
                    <td>
                        <input id="Subject3" class="easyui-combobox" data-options="required:false,editable:false" style="width: 250px;" />
                    </td>
                </tr>
                <tr class="subject4">
                    <td class="lbl">四级科目：</td>
                    <td>
                        <input id="Subject4" class="easyui-combobox" data-options="required:false,editable:false" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">币种：</td>
                    <td>
                        <input id="Currency" class="easyui-combobox" data-options="required:true,disabled:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr class="suggest">
                    <td class="lbl">单价：</td>
                    <td>
                        <input id="UnitPrice" class="easyui-numberbox" data-options="editable:false,min:0,precision:0" style="width: 250px;" />
                    </td>
                </tr>
                <tr class="suggest">
                    <td class="lbl">数量：</td>
                    <td>
                        <input id="Quantity" class="easyui-numberbox" data-options="editable:false,min:0,precision:0" style="width: 250px;" />
                    </td>
                </tr>
                <tr class="suggest">
                    <td class="lbl">建议应收：</td>
                    <td>
                        <input id="Price" class="easyui-numberbox" data-options="editable:false,min:0,precision:2" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">实际应收：</td>
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

