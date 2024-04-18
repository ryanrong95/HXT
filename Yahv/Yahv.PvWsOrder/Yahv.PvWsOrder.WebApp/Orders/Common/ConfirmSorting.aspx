<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ConfirmSorting.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.ConfirmSorting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var originData = model.originData;
        var currencyData = model.currencyData;
        var firstLoad = true;

        $(function () {
            $("#currency").combobox({
                required: true,
                disabled: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: currencyData,
            })
            //页面初始化
            if (model.Info != null) {
                //$('#company').textbox('setValue', model.company);
                //$('#companyBeneficiary').textbox('setValue', model.companyBeneficiarie);
                $('#supplier').textbox('setValue', model.Info.Supplier);
                $('#clientName').textbox('setValue', model.Info.ClientName);
                $('#enterCode').textbox('setValue', model.Info.EnterCode);
                $('#sortingInfo').textbox('setValue', model.Info.SortingInfo);
            }

            //到货产品列表
            window.grid = $("#tab2").myDatagrid({
                singleSelect: true,
                fitColumns: true,
                pagination: false,
                fit: true,
                scrollbarSize: 0,
                nowrap: false,
                loadFilter: function (data) {
                    var data = model.deliveryData;
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

            //产品列表初始化
            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                rownumbers: true,
                fitColumns: true,
                //fit: false,
                scrollbarSize: 0,
                pagination: false,
                checkOnSelect: false,
                selectOnCheck: false,
                singleSelect: true,
                fit: true,
                nowrap: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'PartNumber', title: '型号', width: 130, align: 'left', editor: { type: 'textbox', options: { required: true, validType: 'length[1,50]' } } },
                    { field: 'Manufacturer', title: '品牌', width: 130, align: 'left', editor: { type: 'textbox', options: { required: true, validType: 'length[1,50]' } } },
                    { field: 'DateCode', title: '批次号', width: 80, align: 'left', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    {
                        field: 'Origin', title: '产地', width: 100, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < originData.length; i++) {
                                if (originData[i].Value == value) {
                                    return originData[i].Text;
                                }
                            }
                            return value;
                        },
                        editor: { type: 'combobox', options: { data: originData, valueField: "Value", textField: "Text", required: true, hasDownArrow: true } }
                    },
                    {
                        field: 'Quantity', title: '数量', width: 60, align: 'center',
                        editor: { type: 'numberbox', options: { min: 0, precision: 0, required: true, } }
                    },
                    {
                        field: 'TotalPrice', title: '总价值', width: 60, align: 'center',
                        editor: { type: 'numberbox', options: { min: 0, precision: 2, required: true, } },
                    },
                    { field: 'Btn', title: '操作', width: 80, align: 'center', formatter: Operation }
                ]],
                loadFilter: function (data) {
                    var data = model.itemData;
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    if (data.rows.length > 0) {
                        $('#currency').combobox('setValue', data.rows[0].Currency);
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddSubtotalRow();
                        firstLoad = false;
                    }
                },
                onSelect: function (rowIndex, rowData) {
                    var rows = $('#tab2').datagrid('getRows');
                    var exit = false;
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i].DeliveryPartNumber == rowData.PartNumber) {
                            $('#tab2').datagrid('selectRow', i);
                            exit = true;
                            break;
                        }
                    }
                    if (!exit) {
                        $('#tab2').datagrid('unselectAll');
                    }
                }
            });

            //新增一行
            $("#btnAddRow").click(function () {
                if (endEditing()) {
                    var rows = $('#dg').datagrid('getRows');
                    if (rows.length > 100) {
                        $.messager.alert('提示', '产品型号不能超过100个！');
                        return;
                    }
                    RemoveSubtotalRow();

                    //添加行，并设置默认值
                    $('#dg').datagrid('appendRow', {
                        Origin: 44,
                    });
                    editIndex = $('#dg').datagrid('getRows').length - 1;
                    $('#dg').datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
                    AddSubtotalRow();
                }
            })
            //关闭窗口
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //提交
            $("#btnSubmit").click(function () {
                endEditing();
                if (!ValidationOrder()) {
                    return;
                }
                var data = new FormData();
                data.append('orderID', id);
                data.append('currency', $("#currency").combobox("getValue"));
                //产品信息
                var rows = $('#dg').datagrid('getRows');
                var products = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    products.push(rows[i]);
                }
                data.append('products', JSON.stringify(products));

                ajaxLoading();
                $.ajax({
                    url: '?action=SubmitOrder',
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
            })
        });
    </script>
    <script>
        //产品操作
        function Operation(val, row, index) {
            //合计行不可删除
            if (val != undefined && val != null) {
                if (val.toString().indexOf('<span class="subtotal">') != -1) {
                    return val;
                }
            }
            //已经到货型号不可删除
            else {
                var rows = $('#tab2').datagrid('getRows');
                for (var m = 0; m < rows.length; m++) {
                    if (row.PartNumber == rows[m].DeliveryPartNumber) {
                        return "--";
                    }
                }
            }
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        //订单验证
        function ValidationOrder() {
            //验证必填项
            var isValid = $('#form1').form('enableValidation').form('validate');
            if (!isValid) {
                return false;
            }
            //验证修改的项是否与到货信息一致
            var tab1Rows = $('#dg').datagrid('getRows');
            var tab2Rows = $('#tab2').datagrid('getRows');
            for (var i = 0; i < tab1Rows.length; i++) {
                var row1 = tab1Rows[i];
                for (var k = 0; k < tab2Rows.length; k++) {
                    var row2 = tab2Rows[k];
                    if (row1.PartNumber == row2.DeliveryPartNumber) {
                        //if (row1.Manufacturer != row2.DeliveryManufacturer || row1.DateCode != row2.DeliveryDateCode ||
                        //  row1.Origin != row2.Origin || row1.Quantity != row2.DeliveryQuantity) {
                        //  $.messager.alert("提示","修改后型号为" + row1.PartNumber + "的下单信息与到货信息不匹配。");
                        //  return false;
                        //}
                    }
                }
            }
            return true;
        }
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);

                loadData();
                RemoveSubtotalRow();
                AddSubtotalRow();

                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            var lastIndex = $('#dg').datagrid('getRows').length - 1;
            if (index == lastIndex) {
                endEditing()
                return;
            }
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
        }
        //删除行
        function Delete(index) {
            RemoveSubtotalRow();//移除合计行
            if (editIndex != undefined) {
                $('#dg').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);
                editIndex = undefined;
            }
            $('#dg').datagrid('deleteRow', index);
            AddSubtotalRow();//添加合计行

            loadData()
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
                PartNumber: '<span class="subtotal">合计：</span>',
                Manufacturer: '<span class="subtotal">--</span>',
                DateCode: '<span class="subtotal">--</span>',
                Origin: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">' + compute('Quantity') + '</span>',
                Unit: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
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
    </script>
    <style>
        .lbl{
            width:100px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'north'" style="height: 107px; border: none">
            <table class="liebiao">
                <tr>
                    <td class="lbl">客户名称</td>
                    <td style="width:300px">
                        <input id="clientName" class="easyui-textbox" style="width: 250px;"
                            data-options="editable:false,multiline:true" />
                    </td>
                    <td class="lbl">入仓号</td>
                    <td>
                        <input id="enterCode" class="easyui-textbox" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">供应商名称</td>
                    <td style="width:300px">
                        <input id="supplier" class="easyui-textbox" style="width: 250px;"
                            data-options="editable:false,multiline:true" />
                    </td>
                    <td class="lbl">订单币种</td>
                    <td>
                        <input id="currency" name="currency" class="easyui-combobox" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">异常信息描述</td>
                    <td colspan="3">
                        <input id="sortingInfo" class="easyui-textbox" style="width: 650px; height: 40px"
                            data-options="editable:false,multiline:true" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center',title:'下单信息'" style="border: none">
            <table id="dg">
            </table>
        </div>
        <div data-options="region:'east',title:'到货信息',split:true," style="max-width: 40%; border: none">
            <table id="tab2">
                <thead>
                    <tr>
                        <th data-options="field:'DeliveryPartNumber',align:'left'" style="width: 150px">型号</th>
                        <th data-options="field:'DeliveryManufacturer',align:'left'" style="width: 150px">品牌</th>
                        <th data-options="field:'DeliveryDateCode',align:'left'" style="width: 100px;">批次号</th>
                        <th data-options="field:'DeliveryOrigin',align:'center'" style="width: 80px">产地</th>
                        <th data-options="field:'DeliveryQuantity',align:'center'" style="width: 80px;">数量</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: left; margin-left: 5px; margin-top: 8px">
                <a id="btnAddRow" class="easyui-linkbutton" iconcls="icon-yg-add">新增一行</a>
            </div>
            <div style="float: right; margin-right: 5px; margin-top: 8px">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
