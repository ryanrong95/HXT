<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Order.OrderChange.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <style type="text/css">
        table.row-info tr td:first-child {
            width: 100px;
        }

        table.row-info tr td:first-child {
            width: 100px;
        }

        #productGrid-container .datagrid-header-rownumber, #productGrid-container .datagrid-cell-rownumber {
            width: 50px;
        }

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
        var orderid = getQueryString("ID")
        var tjRowIndex;
        var importTaxTotal = 0;
        var addedValueTaxTotal = 0;
        var arr2;
        var arrImport = [];
        var arrAddedValue = [];
        $(function () {
            var lastIndex;
            $('#productGrid').myDatagrid({
                nowrap: false,
                pagination: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                actionName: 'ProductData',
                onLoadSuccess: function () {
                    arr2 = $("#productGrid").datagrid("getData");
                    AddSubtotalRow();
                    //compute();

                    var heightValue = $("#productGrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                    $("#productGrid").prev().find(".datagrid-body").height(heightValue);
                    $("#productGrid").prev().height(heightValue);
                    $("#productGrid").prev().parent().height(heightValue);
                    $("#productGrid").prev().parent().parent().height(heightValue);
                },
                onClickRow: function (rowIndex) {
                    if (rowIndex == tjRowIndex) {
                        return;
                    }

                    if (lastIndex != rowIndex) {
                        $(this).datagrid('endEdit', lastIndex);
                        $(this).datagrid('beginEdit', rowIndex);
                    }
                    lastIndex = rowIndex;
                },
                onBeginEdit: function (rowIndex) {
                    var editors = $('#productGrid').datagrid('getEditors', rowIndex);
                    var importRate = $(editors[0].target);
                    var addedValueRate = $(editors[1].target);
                    arr = $("#productGrid").datagrid("getData");
                    var totalprice1 = (arr.rows[0].TotalPrice * orderChange.TransPremiumInsurance).toFixed(2);
                    totalprice = (totalprice1 * orderChange.CustomsExchangeRate).toFixed(0);
                    //计算关税
                    importRate.numberbox({
                        onChange: function () {
                            var newValue1 = importRate.numberbox('getValue');
                            var newValue2 = addedValueRate.numberbox('getValue');
                            var currenrAddValue = ((Number(totalprice) + Number(totalprice * newValue1)) * newValue2).toFixed(2)
                            $('#productGrid').datagrid('updateRow', {
                                index: rowIndex,
                                row: {
                                    ImportRate: newValue1,
                                    AddedValueRate: newValue2,
                                    ImportTax: (totalprice * newValue1).toFixed(2),//关税
                                    AddedValueTax: currenrAddValue  //(总价*运保杂.tofix(2)* 海关汇率.tofix(0)+关税）*增值税率
                                }
                            });
                            RemoveSubtotalRow();
                            AddSubtotalRow();
                        }
                    });
                    //计算增值税
                    addedValueRate.numberbox({
                        onChange: function () {
                            var newValue1 = importRate.numberbox('getValue');
                            var newValue2 = addedValueRate.numberbox('getValue');
                            $('#productGrid').datagrid('updateRow', {
                                index: rowIndex,
                                row: {
                                    ImportRate: newValue1,
                                    AddedValueRate: newValue2,
                                    ImportTax: (totalprice * newValue1).toFixed(2),//关税
                                    AddedValueTax: ((Number(totalprice) + Number(totalprice * newValue1)) * newValue2).toFixed(2)  //(总价*运保杂.tofix(2)* 海关汇率.tofix(0)+关税）*增值税率
                                }
                            });
                            RemoveSubtotalRow();
                            AddSubtotalRow();
                            //  compute(rowIndex);
                        }
                    });
                }
            });
            Init();
        });

        function RemoveSubtotalRow() {
            var lastIndex = $('#productGrid').datagrid('getRows').length - 1;
            $('#productGrid').datagrid('deleteRow', lastIndex);
        }

        function AddSubtotalRow() {
            //添加合计行
            $('#productGrid').datagrid('appendRow', {
                ProductName: '<span class="subtotal">合计：</span>',
                TotalPrice: '<span class="subtotal">' + computeTotal('TotalPrice') + '</span>',
                ImportTax: '<span class="subtotal">' + computeTotal('ImportTax') + '</span>',
                AddedValueTax: '<span class="subtotal">' + computeTotal('AddedValueTax') + '</span>',

            });
        }

        function computeTotal(colName) {
            var rows = $('#productGrid').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(rows[i][colName]);
                }
            }
            if (colName == "ImportTax") {
                importTaxTotal = total.toFixed(2)
            }
            if (colName == "AddedValueTax") {
                addedValueTaxTotal = total.toFixed(2);
            }
            return total.toFixed(2);
        }

        function Init() {
            if (orderChange != null) {
                $("#OrderID").text(orderChange.OrderID);
                $("#ContrNo").text(orderChange.ContrNo);
                $("#EntryId").text(orderChange.EntryId);
                $("#DDate").text(orderChange.DDate);
                $("#CreateDate").text(orderChange.CreateDate);
                $("#Currency").text(orderChange.Currency);
                $(".Currency").text("(" + orderChange.Currency + ")");
                $("#DecAmount").text(orderChange.DecAmount.toFixed(2));
                $("#CustomsExchangeRate").text(orderChange.CustomsExchangeRate);
                $("#RealExchangeRate").text(orderChange.RealExchangeRate);
                $("#TariffValue").text(orderChange.TariffValue);
                $("#AddedValue").text(orderChange.AddedValue);
            };
        }
        //提交申请

        function GetChange(arr1, arr2) {
            var len = arr1.length;
            while (len--) {
                for (var i = 0; i < arr2.rows.length; i++) {
                    if (arr2.rows[i]["ID"] == arr1[len]["ID"]) {
                        if (arr2.rows[i]["ImportRate"] != arr1[len]["ImportRate"]) {
                            arrImport.push(arr1[len]);
                        }
                        if (arr2.rows[i]["AddedValueRate"] != arr1[len]["AddedValueRate"]) {
                            arrAddedValue.push(arr1[len])
                        }
                    }
                }
            }
        };
        //返回
        function Back() {
            var url = location.pathname.replace(/Edit.aspx/ig, 'List.aspx')
            window.location = url;
        }
    </script>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="修改订单税费" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
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
                        <td class="content" style="background-color: whitesmoke; width: 20%">报关总价<label class="Currency"></label></td>
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
                </table>
            </div>
            <div data-options="region:'center',border: false,fit:true">
                <div data-options="region:'center',border: false,">
                    <div id="productGrid-container" class="sec-container">
                        <form id="form1">
                            <table id="productGrid" class="easyui-datagrid" title="商品信息" data-options="
                                nowrap:false,
                                pagination:false,
                                ">
                                <thead>
                                    <tr>
                                        <th data-options="field:'HSCode',width: 200,align:'center'">商品编码</th>
                                        <th data-options="field:'ProductName',width: 300,align:'left'">品名</th>
                                        <th data-options="field:'ProductModel',width: 250,align:'left'">型号</th>
                                        <th data-options="field:'Origin',width: 100,align:'left'">产地</th>
                                        <th data-options="field:'TotalPrice',width: 150,align:'left'">总价</th>
                                        <th data-options="field:'ImportRate',width: 150,align:'left',editor:{type:'numberbox',options:{precision:4}}">关税率</th>
                                        <th data-options="field:'ImportTax',width: 150,align:'left'">关税</th>
                                        <th data-options="field:'AddedValueRate',width: 250,align:'left',editor:{type:'numberbox',options:{precision:4}}">增值税率</th>
                                        <th data-options="field:'AddedValueTax',width: 150,align:'left'">增值税</th>
                                    </tr>
                                </thead>
                            </table>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>

