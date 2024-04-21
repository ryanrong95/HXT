<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="_bak_Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.ProductsFee._bak_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var firstLoad = true;

        $(function () {
            if (model.Data) {
                $('form').form('load', model.Data);
            }

            if (getQueryString('id')) {
                $('#msg').hide();
            }

            $("#PayeeAccountID").combogrid({
                data: model.PayeeAccounts,
                required: true,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "Name",
                panelWidth: 500,
                //panelHeight: 200,
                mode: "local",
                prompt: "收款账户",
                columns: [[
                    { field: 'Name', title: '账户名称', width: 100, align: 'left' },
                    { field: 'OpeningBank', title: '开户行', width: 100, align: 'left' },
                    { field: 'Code', title: '银行账号', width: 100, align: 'left' },
                    { field: 'Currency', title: '币种', width: 120, align: 'left' }
                ]],
                onChange: function (now, old) {
                    var row = $('#PayeeAccountID').combogrid('grid').datagrid('getSelected');
                    $('#PayeeCurrency').textbox('setValue', row.Currency);
                    $('#PayeeCode').textbox('setValue', row.Code);
                    $('#PayeeBank').textbox('setValue', row.OpeningBank);
                }
            });

            $('#PayerAccountID').combogrid({
                data: model.PayerAccounts,
                required: true,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "Name",
                panelWidth: 500,
                mode: "local",
                prompt: "付款账户",
                columns: [[
                    { field: 'Name', title: '账户名称', width: 100, align: 'left' },
                    { field: 'OpeningBank', title: '开户行', width: 100, align: 'left' },
                    { field: 'Code', title: '银行账号', width: 100, align: 'left' },
                    { field: 'Currency', title: '币种', width: 120, align: 'left' }
                ]],
                onChange: function (now, old) {
                    var row = $('#PayerAccountID').combogrid('grid').datagrid('getSelected');
                    $('#PayerCurrency').textbox('setValue', row.Currency);
                    $('#PayerCode').textbox('setValue', row.Code);
                    $('#PayerBank').textbox('setValue', row.OpeningBank);
                }
            });

            //申请人
            $('#ApplierID').combobox({
                url: '?action=getAdmins',
                valueField: "value",
                textField: "text",
                multiple: false,
                required: true,
                editable: true,
                onLoadSuccess: function (data) {
                    //if (data.length > 0 && model.Data != null) {
                    //    $(this).combobox('select', model.Data.OwnerID);
                    //}
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

            //审批人
            $('#ApproverID').combobox({
                url: '?action=getAdmins',
                valueField: "value",
                textField: "text",
                multiple: false,
                editable: true,
                required: true,
                onLoadSuccess: function (data) {
                    //if (data.length > 0 && model.Data != null) {
                    //    $(this).combobox('select', model.Data.OwnerID);
                    //}
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

            //产品列表初始化
            $('#dg').myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'data',
                onClickRow: onClickRow,
                rownumbers: true,
                columns: [[
                    {
                        field: 'AccountCatalogID', title: '付款类型', width: 200, align: 'center',
                        formatter: function (value) {
                            var array = model.AccountCatalogs;
                            for (var i = 0; i < array.length; i++) {
                                if (array[i].id == value) {
                                    return array[i].text;
                                }
                            }
                            return value;
                        },
                        editor: {
                            type: 'combotree', options: {
                                data: eval(model.AccountCatalogsJson), required: true, onBeforeSelect: function (node) {
                                    if (node.children != null) {
                                        top.$.messager.alert('操作提示', "请您选择子节点!", 'error');
                                        return false;
                                    }
                                }
                            }
                        }
                    },
                    {
                        field: 'Price', title: '金额', width: 200, align: 'center', editor: { type: 'numberbox', options: { min: 0, precision: 2, required: true } }
                    },
                    { field: 'Btn', title: '操作', width: 150, align: 'center', formatter: Operation }
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        addNewRow();        //新增一行 
                        AddSubtotalRow();
                        firstLoad = false;
                    }
                }
            });

            //新增一条 申请项
            $("#btnAdd").click(function () {
                addNewRow();
            });

            //提交
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                var data = new FormData($('form')[0]);
                ajaxLoading();
                $.post({
                    url: '?action=Submit&&id=' + getQueryString('id') + '&&fatherid=' + getQueryString('fatherid'),
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.data, type: "success" });
                            top.$.myDialogFuse.close();
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });

                return false;
            });
        });
    </script>
    <script>
        function Operation(val, row, index) {
            if (val != undefined && val != null) {
                if (val.toString().indexOf('<span class="subtotal">') != -1) {
                    return val;
                }
            }

            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            } else {
                endEditing();
                loadData();
            }
        }
        //删除行
        function Delete(index) {
            if (editIndex != undefined) {
                $('#dg').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);
                editIndex = undefined;
            }
            $('#dg').datagrid('deleteRow', index);
            loadData();
        }
        //重新加载数据，作用：刷新列表操作按钮的样式，并触发onLoadSuccess事件
        function loadData() {
            var data = $('#dg').datagrid('getData');
            $('#dg').datagrid('loadData', data);
        }
        //添加合计行
        function AddSubtotalRow() {
            //添加合计行
            $('#dg').datagrid('appendRow', {
                AccountCatalogID: '<span class="subtotal">合计：</span>',
                LeftPrice: '<span class="subtotal">' + compute('LeftPrice') + '</span>',
                Btn: '<span class="subtotal">--</span>',
            });
        }
        //删除合计行
        function RemoveSubtotalRow() {
            var lastIndex = $('#dg').datagrid('getRows').length - 1;
            $('#dg').datagrid('deleteRow', lastIndex);
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#dg').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //合计值
        function compute2(colName) {
            var rows = $('#dg').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length - 1; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //新增一行
        function addNewRow() {
            if (endEditing()) {
                //设置默认数据
                $('#dg').datagrid('appendRow', {});
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
            }
        }
        //重新加载数据，作用：刷新列表操作按钮的样式，并触发onLoadSuccess事件
        function loadData() {
            var data = $('#dg').datagrid('getData');
            $('#dg').datagrid('loadData', data);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input id="FatherID" name="FatherID" class="easyui-textbox">
            <input type="submit" id='btnSubmit' />
        </div>
        <div id="topper" style="padding: 5px">
            <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">新增</a>
        </div>
        <table class="liebiao">
            <tr>
                <td>收款账户
                </td>
                <td>
                    <input id="PayeeAccountID" name="PayeeAccountID" class="easyui-combogrid" style="width: 200px;" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayeeCurrency" name="PayeeCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>收款账号</td>
                <td>
                    <input id="PayeeCode" name="PayeeCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>收款银行</td>
                <td>
                    <input id="PayeeBank" name="PayeeBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款账户
                </td>
                <td>
                    <input id="PayerAccountID" name="PayerAccountID" class="easyui-combogrid" style="width: 200px;" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayerCurrency" name="PayerCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款账号</td>
                <td>
                    <input id="PayerCode" name="PayerCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款银行</td>
                <td>
                    <input id="PayerBank" name="PayerBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款日期</td>
                <td>
                    <input id="PaymentDate" name="PaymentDate" class="easyui-datebox" data-options="required:true" style="width: 200px;" editable="false" />
                </td>
                <td>是否已付款</td>
                <td>
                    <input id="IsPaid" name="IsPaid" class="easyui-checkbox" />
                </td>
            </tr>
            <tr>
                <td>申请人</td>
                <td>
                    <input id="ApplierID" name="ApplierID" class="easyui-combobox" style="width: 200px;" />
                </td>
                <td>指定审批人</td>
                <td>
                    <input id="ApproverID" name="ApproverID" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>上传附件
                </td>
                <td colspan="3">
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>付款类型
                </td>
                <td colspan="3">
                    <input id="AccountCatalog" name="AccountCatalog" style="width: 200px;" class="easyui-combotree" />
                </td>
            </tr>
            <tr>
                <td>摘要</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 100px;" />
                </td>
            </tr>
        </table>
        <div id="tt" class="easyui-tabs" style="border: none;">
            <div title="货款申请项" style="border: none">
                <table id="dg" title="">
                </table>
            </div>
        </div>
    </div>
</asp:Content>
