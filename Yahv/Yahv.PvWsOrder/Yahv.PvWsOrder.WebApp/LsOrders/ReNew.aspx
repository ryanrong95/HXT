<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ReNew.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.ReNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var firstLoad = true;
        var companyData = model.companyData;
        var ClientID = getQueryString('ClientID');
        var FatherID = getQueryString('ID');
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: true,
                fit: false,
                singleSelect: true,
                pagination: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'Name', title: '库位名称', width: 100, align: 'center' },
                    { field: 'SpecID', title: '库位级别', width: 100, align: 'center' },
                    { field: 'StartDate', title: '开始日期', width: 100, align: 'center' },
                    { field: 'EndDate', title: '结束日期', width: 100, align: 'center' },

                    { field: 'Quantity', title: '数量', width: 100, align: 'center' },
                    {
                        field: 'UnitPrice', title: '单价(元/月)', width: 100, align: 'center',
                        editor: {
                            type: 'numberbox', options: {
                                required: true,
                                min: 1,
                                precision: 0,
                            }
                        },
                    },
                    { field: 'TotalPrice', title: '总价', width: 100, align: 'center' },
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddTotalRow();
                        firstLoad = false;
                    }
                }
            });
            // 查看租赁价格表
            $('#btn').click(function () {
                $.myWindow({
                    title: "库位租赁价格配置表",
                    url: location.pathname.replace('ReNew.aspx', 'BasePrice/ViewList.aspx'),
                });
            });
            //默认租赁开始时间
            $('#StartDate').datebox('setValue', model.StartDate);
            //租赁时长
            $('#Month').numberbox({
                onChange: function () {
                    $("#btnCalculate").click();
                }
            })
            // 费用计算
            $("#btnCalculate").click(function () {
                endEditing();
                var month = $('#Month').numberbox('getValue');
                var rows = $('#tab1').datagrid('getRows');
                for (var i = 0; i < rows.length - 1; i++) {
                    var price = rows[i].UnitPrice;
                    rows[i].TotalPrice = price * rows[i].Quantity * month;
                };
                loadData();
                RemoveSubtotalRow();
                AddTotalRow();
            })
            // 提交订单
            $("#btnSubmit").click(function () {
                //防止编辑状态提交获取不到值
                endEditing();
                var data = new FormData();
                //基本信息
                data.append('clientId', ClientID);
                data.append('fatherid', FatherID);
                data.append('company', $("#company").combobox("getValue"));
                data.append('beneficiary', $("#beneficiary").combobox("getValue"));
                data.append('startDate', $("#StartDate").datebox("getValue"));
                data.append('month', $("#Month").numberbox("getValue"));
                //产品信息
                var rows = $('#tab1').datagrid('getRows');
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
            });
            // 关闭窗口
            $("#btnClose").click(function () {
                $.myWindow.close();
            });
        });
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#tab1').datagrid('validateRow', editIndex)) {
                $('#tab1').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
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
        //添加合计行
        function AddTotalRow() {
            $('#tab1').datagrid('appendRow', {
                Name: '<span class="subtotal">合计：</span>',
                SpecID: '<span class="subtotal">--</span>',
                StartDate: '<span class="subtotal">--</span>',
                EndDate: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">--</span>',
                UnitPrice: '--',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
            });
        }
        //删除合计行
        function RemoveSubtotalRow() {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            $('#tab1').datagrid('deleteRow', lastIndex);
        }
        //重新加载数据
        function loadData() {
            var data = $('#tab1').datagrid('getData');
            $('#tab1').datagrid('loadData', data);
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
    </script>
    <style>
        .lbl {
            width: 120px;
            max-width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'">
            <div id="topper">
                <table class="liebiao">
                    <tr>
                        <td class="lbl">内部公司</td>
                        <td>
                            <input id="company" class="easyui-combobox" style="width: 250px;"
                                data-options="prompt:'内部受益公司'" />
                        </td>
                        <td class="lbl">受益账号</td>
                        <td>
                            <input id="beneficiary" class="easyui-combogrid" style="width: 250px; height: 22px"
                                data-options="prompt:'内部公司受益账户'," />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <a id="btn" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">查看租赁价格表</a>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="tab1">
            </table>
            <table class="liebiao" style="margin-top: 1px">
                <tr>
                    <td class="lbl">租赁开始时间</td>
                    <td>
                        <input id="StartDate" class="easyui-datebox" style="width: 250px;"
                            data-options="required:true,disabled:true" />
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
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">保存</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
