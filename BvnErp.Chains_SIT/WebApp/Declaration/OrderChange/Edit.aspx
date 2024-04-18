<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Declaration.OrderChange.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/ccs.log-1.0.js"></script>
    <style type="text/css">
        .border-table {
            line-height: 15px;
            border-collapse: collapse;
            border: 1px solid lightgray;
            width: 100%;
            text-align: center;
        }

            .border-table tr {
                height: 25px;
            }

                .border-table tr td {
                    font-weight: normal;
                    border: 1px solid lightgray;
                    text-align: left;
                    padding: 5px;
                }

                .border-table tr th {
                    font-weight: normal;
                    border: 1px solid lightgray;
                    text-align: left;
                    padding: 5px;
                }
    </style>
    <script type="text/javascript">
        var orderChange = eval('(<%=this.Model.OrderChange%>)');
        // var orderid = getQueryString("ID");
        var orderid = orderChange.ID;
        var tjRowIndex;
        var importTaxTotal = 0;
        var exciseTaxTotal = 0;
        var addedValueTaxTotal = 0;
        var arr2 = [];
        var arrImport = [];
        var arrExciseTax = [];
        var arrAddedValue = [];
             $(function () {

            var lastIndex;
            $('#productGrid').myDatagrid({
                nowrap: false,
                pagination: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        //因香港分开装箱报关单中会有多条，所以需要重新计算单条型号的税费金额 ryan 20210811//////////////////////
                        var decimalFortotalprice = 0;
                        if (orderChange.IsTwoStep) {
                            decimalFortotalprice = 2;
                        }
                        //RMB 总金额
                        totalprice = accMul(row.TotalPrice, row.CustomsExchangeRate).toFixed(decimalFortotalprice);
                        //关税率
                        var importRate = row.ImportRate;
                        //消费税率
                        var exciseTaxRate = row.ExciseTaxRate;
                        //增值税率
                        var addedValueRate = row.AddedValueRate;
                        //关税
                        var currenrImportTax = accMul(totalprice, importRate).toFixed(2);
                        var temp1 = accAdd(totalprice, currenrImportTax); //完税价格+关税
                        var temp2 = accMul(accDiv(temp1, (1 - exciseTaxRate)), exciseTaxRate); //(完税价格＋关税)÷(1－消费税税率)×消费税税率
                        //消费税
                        var currenrExciseTax = temp2.toFixed(2);
                        //增值税
                        var currenrAddValue = accMul(accAdd(temp1, temp2), addedValueRate).toFixed(2);

                        row.ImportTax = currenrImportTax;
                        row.ExciseTax = currenrExciseTax;
                        row.AddedValueTax = currenrAddValue;
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                actionName: 'ProductData',
                 onClickRow: onClickRow,
                onLoadSuccess: function () {
                    //RemoveSubtotalRow();
                    AddSubtotalRow();

                    $.each($("#productGrid").datagrid("getData").rows, function (index, val) {
                        arr2.push({
                            ID: val.ID,
                            ImportRate: val.ImportRate,
                            ExciseTaxRate: val.ExciseTaxRate,
                            AddedValueRate: val.AddedValueRate,
                            ImportTax: val.ImportTax,
                            ExciseTax: val.ExciseTax,
                            AddedValueTax: val.AddedValueTax
                        });
                    });
                    var heightValue = $("#productGrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 60;
                    $("#productGrid").prev().find(".datagrid-body").height(heightValue);
                    $("#productGrid").prev().height(heightValue);
                    $("#productGrid").prev().parent().height(heightValue);
                    $("#productGrid").prev().parent().parent().height(heightValue);
                }
            });
            Init();
        });

        //toFixed精度解决
        Number.prototype.toFixed = function (len) {
            var add = 0;
            var s, temp;
            var s1 = this + "";
            var start = s1.indexOf(".");
            if (start != -1) {
            if (s1.substr(start + len + 1, 1) >= 5) add = 1;
            }
            var temp = Math.pow(10, len);
            s = accAdd(Math.floor(accMul(this, temp)), add);
            return accDiv(s, temp);
        }
       
        ///JS 页面计算精度丢失,解决方案
        //加法
        function accAdd(arg1, arg2) {
            var r1, r2, m;
            try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
            try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
            m = Math.pow(10, Math.max(r1, r2))
            return (Math.round(arg1 * m) + Math.round(arg2 * m)) / m
        }
        //减法
        function accSub(arg1, arg2) {
            var r1, r2, m, n;
            try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
            try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
            m = Math.pow(10, Math.max(r1, r2));
            n = (r1 >= r2) ? r1 : r2;
            return ((arg1 * m - arg2 * m) / m).toFixed(n);
        }
        //乘法
        function accMul(arg1, arg2) {
            var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
            try { m += s1.split(".")[1].length } catch (e) { }
            try { m += s2.split(".")[1].length } catch (e) { }
            return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m)
        }


        //除法
        function accDiv(arg1, arg2) {
            var t1 = 0, t2 = 0, r1, r2;
            try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
            try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
            with (Math) {
                r1 = Number(arg1.toString().replace(".", ""))
                r2 = Number(arg2.toString().replace(".", ""))
                return accMul((r1 / r2), pow(10, t2 - t1));
            }
        }

        function RemoveSubtotalRow() {
            var lastIndex = $('#productGrid').datagrid('getRows').length - 1;
            $('#productGrid').datagrid('deleteRow', lastIndex);
        }
        //追加合计
        function AddSubtotalRow() {
            //添加合计行
            $('#productGrid').datagrid('appendRow', {
                ProductName: '<span class="subtotal">合计：</span>',
                TotalPrice: '<span class="subtotal">' + computeTotal('TotalPrice') + '</span>',
                ImportTax: '<span class="subtotal">' + computeTotal('ImportTax') + '</span>',
                ExciseTax: '<span class="subtotal">' + computeTotal('ExciseTax') + '</span>',
                AddedValueTax: '<span class="subtotal">' + computeTotal('AddedValueTax') + '</span>',
            });
        }
        //求总价 \关税\ 增值税的合计
        function computeTotal(colName) {
            var rows = $('#productGrid').datagrid('getRows');
            var total = 0.00;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    var ss = rows[i][colName].toString();
                    total = accAdd(ss, total);
                }
            }
            //判断如果关税小于50，则将关税的合计，置为零
            if (colName == "ImportTax") {
                if (orderChange.TariffValue < 50) {
                    total = 0;
                } else {
                    importTaxTotal = total
                }
            }
            if (colName == "ExciseTax") {
                if (orderChange.ExciseTaxValue < 50) {
                    total = 0;
                } else {
                    exciseTaxTotal = total
                }
            }
            if (colName == "AddedValueTax") {
                addedValueTaxTotal = total;
            }
            return total.toFixed(2);
        }
    //初始化数据
        function Init() {
            if (orderChange != null) {
                $("#OrderID").text(orderChange.OrderID);
                $("#ContrNo").text(orderChange.ContrNo);
                $("#EntryId").text(orderChange.EntryId);
                $("#DDate").text(orderChange.DDate);
                $("#CreateDate").text(orderChange.CreateDate);
                $("#Currency").text(orderChange.Currency);
                // $(".Currency").text("(" + orderChange.Currency + ")");
                $("#DecAmount").text(orderChange.DecAmount.toFixed(2));
                $("#CustomsExchangeRate").text(orderChange.CustomsExchangeRate);
                $("#RealExchangeRate").text(orderChange.RealExchangeRate);
                $("#TariffValue").text(orderChange.TariffValue);
                $("#AddedValue").text(orderChange.AddedValue);
                $("#ExciseTaxValue").text(orderChange.ExciseTaxValue);
            };
            var from = getQueryString('From');
            if (from == "ViewDeclareChange") {
                $("#btnSubmit").hide();

                $("#TaxRateChangeLog").parent().show();

                //请求税费修改记录
                $.post('?action=GetTaxRateChangeLogs', { OrderID: orderChange.OrderID, }, function (res) {
                    if (res != null && res.length > 0) {
                        $('#TaxRateChangeLog').ccslog({
                            data: res
                        });
                    } else {
                        $('#TaxRateChangeLog').append('<div style="margin:10px"><p style="margin:5px">无税率修改记录</p></div>');
                    }
                });

            } else {
                $("#TaxRateChangeLog").parent().hide();

                $("#btnSubmit").show();
            }

        }
        //提交申请
        function Submit() {
            if (importTaxTotal != Number(orderChange.TariffValue) || exciseTaxTotal != Number(orderChange.ExciseTaxValue) || addedValueTaxTotal != Number(orderChange.AddedValue)) {
                $.messager.alert("消息", "关税或消费税或增值税与实缴的值不相等，不能提交！");
                return;
            }
            if (!$("#form1").form('validate')) {
                return;
            }
            else {
                //验证成功
                var arr1 = $('#productGrid').datagrid('getChanges', 'updated');
                GetChange(arr1);
                MaskUtil.mask();//遮挡层
                $.post('?action=Submit', {
                    ArrImport: JSON.stringify(arrImport),
                    ArrExciseTax: JSON.stringify(arrExciseTax),
                    ArrAddedValue: JSON.stringify(arrAddedValue),
                    CusTariffValue: orderChange.TariffValue,
                    //CusAddedValue: orderChange.AddedValue,
                    OrderID: orderid
                }, function (result) {
                    MaskUtil.unmask();//关闭遮挡层
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            Back();
                        }
                    });
                })
            }
        }
        // 获取关税或消费税或增值税率修改的数据
        function GetChange(arr1) {
            // 清空数据，保证每次进来是新数据
            arrImport = [];
            arrExciseTax = [];
            arrAddedValue = [];
            var orderItemIds = []; // 定义OrderItemId数组，处理去重
            for (var i = 0; i < arr1.length; i++) {
                var orderItemID = arr1[i]["OrderItemID"]; // 取出OrderItemId 判断去重
                var index = orderItemIds.findIndex(f => f == orderItemID); // 判断如果数组中是否存在当前OrderItemid
                if (index == -1) { // 不存在
                    var id = arr1[i]["ID"]; // 取出当前ID
                    var sourceImportRate = arr2.find(f => f.ID == id).ImportRate; // 取出原ID中的关税率
                    var sourceExciseTaxRate = arr2.find(f => f.ID == id).ExciseTaxRate; // 取出原ID中的消费税率
                    var sourceAddedValueRate = arr2.find(f => f.ID == id).AddedValueRate; // 取出原ID中的增值税率

                    var sourceImportTax = arr2.find(f => f.ID == id).ImportTax; // 取出原ID中的关税
                    var sourceExciseTax = arr2.find(f => f.ID == id).ExciseTax; // 取出原ID中的消费税
                    var sourceAddedValueTax = arr2.find(f => f.ID == id).AddedValueTax; // 取出原ID中的增值税

                    if (sourceImportRate != arr1[i]["ImportRate"] || sourceImportTax != arr1[i]["ImportTax"]) { // 当前关税率不一致，则push
                        arr1[i]["OldImportRate"] = sourceImportRate;
                        arr1[i]["OldExciseTaxRate"] = sourceExciseTaxRate;
                        arr1[i]["OldAddedValueRate"] = sourceAddedValueRate;
                        arrImport.push(arr1[i]);
                    }
                    if (sourceExciseTaxRate != arr1[i]["ExciseTaxRate"] || sourceExciseTax != arr1[i]["ExciseTax"]) {
                        arr1[i]["OldImportRate"] = sourceImportRate;
                        arr1[i]["OldExciseTaxRate"] = sourceExciseTaxRate;
                        arr1[i]["OldAddedValueRate"] = sourceAddedValueRate;
                        arrExciseTax.push(arr1[i]);
                    }
                    if (sourceAddedValueRate != arr1[i]["AddedValueRate"] || sourceAddedValueTax != arr1[i]["AddedValueTax"]) {
                        arr1[i]["OldImportRate"] = sourceImportRate;
                        arr1[i]["OldExciseTaxRate"] = sourceExciseTaxRate;
                        arr1[i]["OldAddedValueRate"] = sourceAddedValueRate;
                        arrAddedValue.push(arr1[i]);
                    }
                    //orderItemIds.push(orderItemID); // 记录当前OrderItmeId,为其他数据去重
                }
            };
        }
        //返回
        function Back() {
            var from = getQueryString('From');
            var url;
            if (from == 'EditDeclareChange') {
                var url = location.pathname.replace(/Edit.aspx/ig, 'List.aspx')
                window.location = url;
            } else if (from == 'ViewDeclareChange') {
                url = location.pathname.replace(/Edit.aspx/ig, 'HandledList.aspx');
                window.location = url;
            }
        }

    </script>

    <script>
         var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#productGrid').datagrid('validateRow', editIndex)) {
                $('#productGrid').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(rowIndex) {
            if (editIndex != rowIndex) {
                if (endEditing()) {
                    $('#productGrid').datagrid('selectRow', rowIndex);
                    $('#productGrid').datagrid('beginEdit', rowIndex);
                    editIndex = rowIndex;
                    var editors = $('#productGrid').datagrid('getEditors', editIndex);
                    //获取产地的值
                    var editors = $('#productGrid').datagrid('getEditors', rowIndex);
                    var importRate = $(editors[0].target);
                    var exciseTaxRate = $(editors[1].target);
                    var addedValueRate = $(editors[2].target);
                    arr = $("#productGrid").datagrid("getData");
                    var decimalFortotalprice = 0;
                    if (orderChange.IsTwoStep) {
                        decimalFortotalprice = 2;
                    }
                    totalprice = accMul(arr.rows[rowIndex].TotalPrice, orderChange.CustomsExchangeRate).toFixed(decimalFortotalprice);
                    //计算关税
                    importRate.numberbox({
                        onChange: function () {
                            reCalcTax(importRate, exciseTaxRate, addedValueRate, totalprice, rowIndex);
                        }
                    });
                    //计算消费税
                    exciseTaxRate.numberbox({
                        onChange: function () {
                            reCalcTax(importRate, exciseTaxRate, addedValueRate, totalprice, rowIndex);
                        }
                    });
                    //计算增值税
                    addedValueRate.numberbox({
                        onChange: function () {
                            reCalcTax(importRate, exciseTaxRate, addedValueRate, totalprice, rowIndex);
                        }
                    });
                } else {
                    $('#productGrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                editIndex = undefined;
            }
        }
        //修改税率重新计算税费
        function reCalcTax(importRate, exciseTaxRate, addedValueRate, totalprice, rowIndex) {
            var newValue1 = importRate.numberbox('getValue');
            var newValue2 = exciseTaxRate.numberbox('getValue');
            var newValue3 = addedValueRate.numberbox('getValue');
            var currenrImportTax = accMul(totalprice, newValue1).toFixed(2);
            var temp1 = accAdd(totalprice, currenrImportTax); //完税价格+关税
            var temp2 = accMul(accDiv(temp1, (1 - newValue2)), newValue2); //(完税价格＋关税)÷(1－消费税税率)×消费税税率
            var currenrExciseTax = temp2.toFixed(2);
            var currenrAddValue = accMul(accAdd(temp1, temp2), newValue3).toFixed(2);

            $('#productGrid').datagrid('updateRow', {
                index: rowIndex,
                row: {
                    ImportRate: newValue1,
                    ExciseTaxRate: newValue2,
                    AddedValueRate: newValue3,
                    ImportTax: currenrImportTax,//关税
                    ExciseTax: currenrExciseTax,//消费税
                    AddedValueTax: currenrAddValue//增值税
                }
            });
            RemoveSubtotalRow();
            AddSubtotalRow();
        }

    </script>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="修改订单税费" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a id="btnSubmit" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Submit()">提交</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                </div>
            </div>
            <div data-options="region:'north',border: false,fit:true">
                <table class="border-table" style="width: 100%; margin-top: 5px; margin-bottom: 5px">
                    <tr>
                        <td class="content" style="background-color: whitesmoke; width: 20%">订单编号</td>
                        <td class="content" id="OrderID" style="width: 30%"></td>
                        <td class="content" style="background-color: whitesmoke; width: 20%">合同号</td>
                        <td class="content" id="ContrNo" style="width: 30%"></td>
                    </tr>
                    <tr>
                        <td class="content" style="background-color: whitesmoke; width: 20%">报关单号</td>
                        <td class="content" id="EntryId" style="width: 30%"></td>
                        <td class="content" style="background-color: whitesmoke; width: 20%">下单日期</td>
                        <td class="content" id="CreateDate" style="width: 30%"></td>
                    </tr>
                    <tr>
                        <td class="content" style="background-color: whitesmoke; width: 20%">报关日期</td>
                        <td class="content" id="DDate" style="width: 30%"></td>
                        <td class="content" style="background-color: whitesmoke; width: 20%">币种</td>
                        <td class="content" id="Currency" style="width: 30%"></td>
                    </tr>
                    <tr>
                        <td class="content" style="background-color: whitesmoke; width: 20%">报关总价</td>
                        <td class="content" id="DecAmount" style="width: 30%"></td>
                        <td class="content" style="background-color: whitesmoke; width: 20%">海关汇率</td>
                        <td class="content" id="CustomsExchangeRate" style="width: 30%"></td>
                    </tr>
                    <tr>
                        <td class="content" style="background-color: whitesmoke; width: 20%">实缴关税</td>
                        <td class="content" id="TariffValue" style="width: 30%"></td>
                        <td class="content" style="background-color: whitesmoke; width: 20%">实缴增值税</td>
                        <td class="content" id="AddedValue" style="width: 30%"></td>
                    </tr>
                    <tr>
                        <td class="content" style="background-color: whitesmoke; width: 20%">实缴消费税</td>
                        <td class="content" id="ExciseTaxValue" style="width: 30%"></td>
                        <td class="content" style="background-color: whitesmoke; width: 20%"></td>
                        <td class="content" id="" style="width: 30%"></td>
                    </tr>
                </table>
            </div>
            <div data-options="region:'center',border: false,fit:true">
                <div data-options="region:'center',border: false,">
                    <div id="productGrid-container" class="sec-container">
                        <form id="form1">
                            <table id="productGrid" title="商品信息">
                                <thead>
                                    <tr>
                                        <th data-options="field:'HSCode',align:'center'" style="width: 10%;">商品编码</th>
                                        <th data-options="field:'ProductName',align:'left'" style="width: 20%;">品名</th>
                                        <th data-options="field:'ProductModel',align:'left'" style="width: 15%;">型号</th>
                                        <th data-options="field:'Origin',align:'left'" style="width: 7%;">产地</th>
                                        <th data-options="field:'TotalPrice',align:'left'" style="width: 7%;">总价</th>
                                        <th data-options="field:'ImportRate',align:'left',editor:{type:'numberbox',options:{precision:4}}" style="width: 7%;">关税率</th>
                                        <th data-options="field:'ImportTax',align:'left'" style="width: 7%;">关税</th>
                                        <th data-options="field:'ExciseTaxRate',align:'left',editor:{type:'numberbox',options:{precision:4}}" style="width: 7%;">消费税率</th>
                                        <th data-options="field:'ExciseTax',align:'left'" style="width: 7%;">消费税</th>
                                        <th data-options="field:'AddedValueRate',align:'left',editor:{type:'numberbox',options:{precision:4}}" style="width: 7%;">增值税率</th>
                                        <th data-options="field:'AddedValueTax',align:'left'" style="width: 7%;">增值税</th>
                                    </tr>
                                </thead>
                            </table>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div>
        <div id="TaxRateChangeLog" title="税费修改记录" class="easyui-panel">
        </div>
    </div>

</body>
</html>

