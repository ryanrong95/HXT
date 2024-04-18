<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ClientID = getQueryString("ID");
        var firstLoad = true;
       // var companyData = model.companyData;

        $(function () {
            //页面初始化
            window.grid = $("#dg").myDatagrid({
                toolbar: '#topper',
                fitColumns: true,
                fit: false,
                singleSelect: true,
                pagination: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'Name', title: '库位名称', width: 100, align: 'center' },
                    { field: 'SpecID', title: '库位级别', width: 100, align: 'center' },
                    { field: 'Load', title: '承重(kg)', width: 100, align: 'center' },
                    { field: 'Volume', title: '容积(cm³)', width: 100, align: 'center' },
                    { field: 'Quantity', title: '可租赁个数', width: 100, align: 'center' },
                    {
                        field: 'ApplyQty', title: '本次申请数', width: 100, align: 'center',
                        formatter: formatApplyQty,
                        editor: { type: 'numberbox', options: { required: true, min: 0, precision: 0 } }
                    },
                    { field: 'UnitPrice', title: '租赁单价', width: 100, align: 'center' },
                    { field: 'TotalPrice', title: '租赁总价(数量*单价*时长)', width: 200, align: 'center' },
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddSubtotalRow();
                        firstLoad = false;
                    }
                }
            });
            //限制租赁开始时间
            $('#StartDate').datetimebox('calendar').calendar({
                validator: function (date) {
                    var now = new Date();
                    var end = new Date((+now) + 7 * 24 * 3600 * 1000);
                    var d1 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    var d2 = new Date(end.getFullYear(), end.getMonth(), end.getDate());
                    return date >= d1 && date <= d2;
                }
            })
            $('#Month').numberbox({
                onChange: function () {
                    $("#btnCalculate").click();
                }
            })
            // 查看租赁价格表
            $('#btn').click(function () {
                $.myWindow({
                    title: "库位租赁价格配置表",
                    url: location.pathname.replace('Add.aspx', 'BasePrice/ViewList.aspx'),
                });
            });
            // 费用计算
            $("#btnCalculate").click(function () {
                endEditing();
                var month = $('#Month').numberbox('getValue');
                ajaxLoading();
                $.post('?action=Calculate', { Month: month }, function (result) {
                    ajaxLoadEnd();
                    var res = JSON.parse(result);
                    if (res.success) {
                        var data = res.data;
                        var rows = $('#dg').datagrid('getRows');
                        for (var i = 0; i < data.length; i++) {
                            var productID = data[i].ProductID
                            var price = data[i].Price
                            for (var k = 0; k < rows.length; k++) {
                                if (rows[k].ID == productID) {
                                    var month = $("#Month").numberbox("getValue")
                                    rows[k].UnitPrice = price;
                                    rows[k].TotalPrice = price * rows[k].ApplyQty * month;
                                }
                            }
                        }
                        RemoveSubtotalRow();
                        loadData();
                        AddSubtotalRow();
                    }
                })
            })
            // 提交订单
            $("#btnSubmit").click(function () {
                //防止编辑状态提交获取不到值
                endEditing();
                $("#btnCalculate").click();
                if (!ValidationOrder()) {
                    return;
                }
                var data = new FormData();
                //基本信息
                data.append('clientID', ClientID);
                data.append('startDate', $("#StartDate").datebox("getValue"));
                data.append('month', $("#Month").numberbox("getValue"));
                //产品信息
                var rows = $('#dg').datagrid('getRows');
                var products = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    if (rows[i].ApplyQty != 0) {
                        products.push(rows[i]);
                    }
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
            // 关闭窗口
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
        });
    </script>
    <script>
        //订单验证
        function ValidationOrder() {
            //验证必填项
            var isValid = $('#form1').form('enableValidation').form('validate');
            if (!isValid) {
                return false;
            }
            var rows = $('#dg').datagrid('getRows');
            var total = 0;
            //rows.length-1 排除掉合并行的数量
            for (var i = 0; i < rows.length-1; i++) {
                if (rows[i]["ApplyQty"] != undefined) {
                    total += Number(rows[i]["ApplyQty"]);
                }
            }
            if (total == 0||total==NaN) {
                $.messager.alert('提示', "租赁库位数量不能为零。")
                return false;
            }
            return true;
        }
        function formatApplyQty(val, row) {
            return '<span style="color:red;font-weight:bold">' + val + '</span>';
        }
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);
                ConfimQuantity(editIndex);

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
        //验证申报数量
        function ConfimQuantity(editIndex) {
            var rows = $('#dg').datagrid('getRows');
            var row = rows[editIndex];
            if (Number(row["Quantity"]) < Number(row["ApplyQty"])) {
                $.messager.alert('提示', '申报数量不能大于可租赁数量');
                $('#dg').datagrid('rejectChanges');
            }
        }
        //添加合计行
        function AddSubtotalRow() {
            //添加合计行
            $('#dg').datagrid('appendRow', {
                Name: '<span class="subtotal">合计：</span>',
                SpecID: '<span class="subtotal">--</span>',
                Load: '<span class="subtotal">--</span>',
                Volume: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">--</span>',
                ApplyQty: '<span class="subtotal">--</span>',
                UnitPrice: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
            });
        }
        //删除合计行
        function RemoveSubtotalRow() {
            var lastIndex = $('#dg').datagrid('getRows').length - 1;
            $('#dg').datagrid('deleteRow', lastIndex);
        }
        //重新加载数据
        function loadData() {
            var data = $('#dg').datagrid('getData');
            $('#dg').datagrid('loadData', data);
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
    </script>
    <style>
        .lbl {
            width: 120px;
            max-width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none">
            <!--搜索按钮-->
            <div id="topper">
                <table class="liebiao">
                    <tr>
                        <td colspan="7">
                            <a id="btn" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">查看租赁价格表</a>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="dg">
            </table>
            <table class="liebiao" style="margin-top: 1px">
                <tr>
                    <td class="lbl">租赁开始时间</td>
                    <td>
                        <input id="StartDate" class="easyui-datebox" style="width: 250px;"
                            data-options="required:true,editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">租赁时长(月)</td>
                    <td>
                        <input id="Month" class="easyui-numberbox" style="width: 250px" data-options="required:true,min:1,precision:0" />
                        <a id="btnCalculate" class="easyui-linkbutton" data-options="iconCls:'icon-yg-assign'">费用计算</a>
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

