<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ApplyInvoice.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Clients.ApplyInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var invoiceData = model.InvoiceData.Invoice;
        var firstLoad = true;
        var clientid = invoiceData[0].ClientID;
        var isPersonal = invoiceData[0].IsPersonal;
        //发票类型
        var invoiceType = "服务费发票";
        var invoiceTypeDate = eval('(<%=this.Model.InvoiceTypeOption%>)');
        //邮寄类型
        var deliveryType = invoiceData[0].DeliveryType;
        $(function () {
            //商品信息表格初始化
            window.grid = $("#tab1").myDatagrid({
                rownumbers: true,
                fit: false,
                scrollbarSize: 0,
                pagination: false,
                singleSelect: false,
                fitColumns: false,
                nowrap: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'ID', title: '账单ID', width: 150, align: 'center', hidden: true },
                    { field: 'ProductName', title: '产品名称', width: 150, align: 'center' },
                    { field: 'ProductModel', title: '规格型号', width: 70, align: 'center' },
                    { field: 'Quantity', title: '数量', width: 70, align: 'center' },
                    { field: 'Price', title: '单价', width: 100, align: 'center' },
                    { field: 'TotalPrice', title: '金额', width: 100, align: 'center' },
                    { field: 'InvoiceTaxRate', title: '税率', width: 50, align: 'center' },
                    { field: 'UnitPrice', title: '含税单价', width: 100, align: 'center' },
                    { field: 'Amount', title: '含税金额', width: 100, align: 'center' },
                    {
                        field: 'Difference', title: '开票差额', width: 100, align: 'center',
                        editor: { type: 'numberbox', options: { precision: 2 } }
                    },
                    { field: 'TaxName', title: '税务名称', width: 150, align: 'center' },
                    { field: 'TaxCode', title: '税务编码', width: 150, align: 'center' },
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddSubtotalRow();
                        firstLoad = false;
                    }
                }
            });

            $("#Title").combogrid({
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "Value",
                textField: "Text",
                data: invoiceData,
                panelWidth: 500,
                mode: "local",
                columns: [[
                    // { field: 'InvoiceType', title: '开票类型', width: 100, align: 'left', hidden: true, },
                    { field: 'DeliveryTypeDesc', title: '交付方式', width: 100, align: 'left', hidden: true },
                    { field: 'CompanyName', title: '公司名称(抬头)', width: 100, align: 'left' },
                    { field: 'TaxCode', title: '纳税人识别号', width: 100, align: 'left', hidden: true },
                    { field: 'BankName', title: '开户行', width: 100, align: 'left', hidden: true },
                    { field: 'BankAccount', title: '账号', width: 100, align: 'left', hidden: true },
                    { field: 'Address', title: '地址', width: 100, align: 'left', hidden: true },
                    { field: 'Tel', title: '电话', width: 120, align: 'left', hidden: true },
                    { field: 'ReceipterCompany', title: '收件单位', width: 50, align: 'center', hidden: true },
                    { field: 'ReceipterName', title: '收件人', width: 50, align: 'center', hidden: true },
                    { field: 'ReceipterTel', title: '手机号', width: 50, align: 'center', hidden: true },
                    { field: 'DetailAddress', title: '详细邮寄地址', width: 50, align: 'center', hidden: true },
                ]],
                onLoadSuccess: function () {


                },
                onSelect: function (index, item) {
                    var g = $('#Title').combogrid('grid');	// get datagrid object
                    var res = g.datagrid('getSelected');	// get the selected row

                    isPersonal = res.IsPersonal;
                    deliveryType = res.DeliveryType;
                    //开票信息初始化
                    $("#DeliveryType").html(res.DeliveryTypeDesc);
                    $("#TaxCode").html(res.TaxCode);
                    $("#BankName").html(res.BankName);
                    $("#BankAccount").html(res.BankAccount);
                    $("#Address").html(res.Address);
                    $("#Tel").html(res.Tel);

                    //邮寄信息初始化
                    $("#ReceipterCompany").html(res.ReceipterCompany);
                    $("#ReceipterName").html(res.ReceipterName);
                    $("#ReceipterTel").html(res.ReceipterTel);
                    $("#DetailAddress").html(res.DetailAddress);
                }
            })
            Init();
        });
        function Init() {
            //开票信息初始化
            //2023-05-13 深圳 鲁亚慧要求 由跟单自己选择开哪种类型的发票
            //$("#InvoiceType").html(invoiceType);
            $('#InvoiceType').combobox({
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                panelWidth: 500,
                data: invoiceTypeDate,
            });
            $("#DeliveryType").html(invoiceData[0].DeliveryTypeDesc);
            //默认设置combogrid
            $('#Title').combogrid('setValue', invoiceData[0]);
            // $("#InvoiceType").html(invoice.);
            $("#TaxCode").html(invoiceData[0].TaxCode);
            $("#BankName").html(invoiceData[0].BankName);
            $("#BankAccount").html(invoiceData[0].BankAccount);
            $("#Address").html(invoiceData[0].Address);
            $("#Tel").html(invoiceData[0].Tel);

            //邮寄信息初始化
            $("#ReceipterCompany").html(invoiceData[0].ReceipterCompany);
            $("#ReceipterName").html(invoiceData[0].ReceipterName);
            $("#ReceipterTel").html(invoiceData[0].ReceipterTel);
            $("#DetailAddress").html(invoiceData[0].DetailAddress);
        }
    </script>
    <script>
        var editIndex = undefined;
        function onClickRow(index) {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            if (index == lastIndex) {
                endEditing()
                return;
            }
            if (editIndex != index) {
                if (endEditing()) {
                    $('#tab1').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#tab1').datagrid('selectRow', editIndex);
                }
            }
        }
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#tab1').datagrid('validateRow', editIndex)) {
                $('#tab1').datagrid('endEdit', editIndex);
                //验证差额
                ConfimDifference(editIndex);
                RemoveSubtotalRow();
                AddSubtotalRow();
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        //添加合计行
        function AddSubtotalRow() {
            //添加合计行
            $('#tab1').datagrid('appendRow', {
                Amount: '<span class="subtotal">' + compute('Amount') + '</span>',
                Difference: '<span class="subtotal">' + compute('Difference') + '</span>',
                ProductName: '<span class="subtotal">合计:</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
            });
        }
        //删除合计行
        function RemoveSubtotalRow() {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            $('#tab1').datagrid('deleteRow', lastIndex);
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //验证开票差额
        function ConfimDifference(editIndex) {
            var rows = $('#tab1').datagrid('getRows');
            var row = rows[editIndex];
            if (Number(row["Difference"]) > Number(row["Amount"])) {
                $.messager.alert('提示', '开票差额应小于含税金额');
                $('#tab1').datagrid('rejectChanges');
            }
        }
        //提交申请
        function SubmitApply() {
            endEditing();
            //验证必填项
            var isValid = $('#form1').form('enableValidation').form('validate');
            if (!isValid) {
                return false;
            }
            //邮寄信息，开票信息 商品信息
            var data = new FormData();
            //基本信息
            data.append('InvoiceType', $('#InvoiceType').combobox("getValue"));
            data.append('DeliveryType', deliveryType);
            data.append('Title', $("#Title").combogrid("getText"));
            data.append('TaxCode', $("#TaxCode").text());
            data.append('BankName', $("#BankName").text());
            data.append('BankAccount', $("#BankAccount").text());
            data.append('Address', $("#Address").text());
            data.append('Tel', $("#Tel").text());
            data.append('ReceipterCompany', $("#ReceipterCompany").text());
            data.append('ReceipterName', $("#ReceipterName").text());
            data.append('ReceipterTel', $("#ReceipterTel").text());
            data.append('DetailAddress', $("#DetailAddress").text());
            data.append('ClientId', clientid);
            data.append('Summary', $("#Summary").textbox("getText"));
            data.append('IsPersonal', isPersonal);
            //产品信息
            var rows = $('#tab1').datagrid('getRows');
            var products = [];
            for (var i = 0; i < rows.length - 1; i++) {
                products.push(rows[i]);
            }
            data.append('products', JSON.stringify(products));

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
        };
    </script>
    <style>
        .lbl {
            width: 100px;
            background-color: #f3f3f3;
        }

        .title {
            font-weight: 800;
            background-color: #f3f3f3;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'north',border: false," style="height: 35px; padding: 5px;">
            <a id="btnApply" class="easyui-linkbutton" data-options="iconCls:'icon-yg-confirm'" onclick="SubmitApply()">提交申请</a>
        </div>
        <div data-options="region:'center',border: false">
            <div style="float: left; width: 50%; padding-left: 5px; border: none">
                <table class="liebiao">
                    <tr>
                        <td colspan="2" class="title">开票信息</td>
                    </tr>
                    <tr>
                        <td class="lbl">开票类型：</td>
                        <td>
                            <input id="InvoiceType" class="easyui-combobox" style="width: 150px" />
                           <%-- <label id="InvoiceType"></label>--%>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">交付方式：</td>
                        <td>
                            <label id="DeliveryType"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">公司名称(抬头)：</td>
                        <td>
                            <input id="Title" class="easyui-combogrid" style="width: 200px; height: 22px"
                                data-options="prompt:'公司抬头'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">纳税人识别号：</td>
                        <td>
                            <label id="TaxCode"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">开户行及账号：</td>
                        <td>
                            <label id="BankName"></label>
                            <label id="BankAccount"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">地址 电话：</td>
                        <td>
                            <label id="Address"></label>
                            <label id="Tel"></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="float: left; width: 50%; padding-left: 5px; padding-right: 5px; border: none">
                <table class="liebiao">
                    <tr>
                        <td colspan="2" class="title">邮寄信息</td>
                    </tr>
                    <tr>
                        <td class="lbl">收件单位：</td>
                        <td>
                            <label id="ReceipterCompany"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">收件人：</td>
                        <td>
                            <label id="ReceipterName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">手机号：</td>
                        <td>
                            <label id="ReceipterTel"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">详细邮寄地址：</td>
                        <td>
                            <label id="DetailAddress"></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="float: left; width: 100%; padding: 5px;">
                <table id="tab1" class="easyui-datagrid" title="商品信息">
                </table>
                <table style="margin-top: 5px; line-height: 40px">
                    <tr style="color: #b0aeae">
                        <td>注： </td>
                        <td>
                            <p style="font-size: 14px;">跟单员需填写好开票差额。</p>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-size: 14px; color: red">备注：</td>
                        <td>
                            <input class="easyui-textbox" id="Summary" data-options="multiline:true,validType:'length[1,250]',height:50,width:400" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
