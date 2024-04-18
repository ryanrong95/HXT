<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MatchOld.aspx.cs" Inherits="WebApp.Order.Match.MatchOld" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script>
        /*字典 Dictionary类*/
        function Dictionary() {
            this.add = add;
            this.datastore = new Array();
            this.find = find;
        }

        function add(key, value) {
            this.datastore[key] = value;
        }

        function find(key) {
            return this.datastore[key];
        }
    </script>
    <script>
        var CaseNoCount = new Dictionary();
        var editIndex = undefined;
        var check = false;
        $(function () {
            var ID = getQueryString("ID");
            $("#OrderID").val(ID);
            //订单信息初始化
            $('#products').myDatagrid({
                actionName: 'data',
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: false, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
            });

            //订单信息初始化
            $('#datagrid').myDatagrid({
                actionName: 'dataInputs',
                pagination: false, //启用分页
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: true,
                onCheck: onCheck,
                onUncheck: onUncheck,
                onClickRow: onClickRow,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        if (data.rows[index].Unit == "" || data.rows[index].Unit == null || data.rows[index].Unit == undefined) {
                            data.rows[index].Unit = "007";
                        }
                        if (data.rows[index].UnitPrice == 0) {
                            data.rows[index].UnitPrice = "";
                        }
                        if (data.rows[index].TotalPrice == 0) {
                            data.rows[index].TotalPrice = "";
                        }
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    MergeCells('datagrid', 'CaseNo', 'CaseNo,ck');
                }
            });
        });

        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#datagrid').datagrid('validateRow', editIndex)) {
                $('#datagrid').datagrid('endEdit', editIndex);
                MergeCells('datagrid', 'CaseNo', 'CaseNo,ck');
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#datagrid').datagrid('selectRow', index);
                    $('#datagrid').datagrid('beginEdit', index);
                    editIndex = index;
                    var edUnitPrice = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'UnitPrice' });
                    if (edUnitPrice) {
                        $(edUnitPrice.target).textbox({
                            readonly: true,
                        });
                    }
                    var ed = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'TotalPrice' });
                    if (ed) {
                        $(ed.target).textbox({
                            onChange: function (newValue, oldValue) {
                                if (editIndex == undefined)
                                    return;
                                var rows = $("#datagrid").datagrid("getRows");                              
                                var row = rows[index]
                                var qty = row.Quantity;
                                var unitPrice = newValue / qty;
                                var unitPriceShow = Number(unitPrice).toFixed(4);                               
                                //$(edUnitPrice.target).textbox("setValue", unitPriceShow);

                                $('#datagrid').datagrid('updateRow', {
                                    index: index,
                                    row: {
                                        UnitPrice: unitPriceShow,
                                        TotalPrice: newValue
                                    }
                                });                              
                                MergeCells('datagrid', 'CaseNo', 'CaseNo,ck');
                            }
                        });                        
                    }
                    var edRowNo = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'RowNo' });
                    if (edRowNo) {
                        $(edRowNo.target).textbox({
                            onChange: function (newValue, oldValue) {
                                //if (editIndex == undefined)
                                //    return;
                                if (newValue == null || newValue == "" || newValue == undefined) {
                                    $('#datagrid').datagrid('updateRow', {
                                        index: index,
                                        row: {
                                            RowNo: newValue
                                        }
                                    });
                                    return
                                } else {
                                    var rows = $("#datagrid").datagrid("getRows");
                                    var newModel = rows[index]["Model"];
                                    var newOriginal = rows[index]["Origin"];
                                    for (var irow = 0; irow < rows.length; irow++) {
                                        if (irow != index) {
                                            if (rows[irow]["RowNo"] == newValue) {
                                                var oldModel = rows[irow]["Model"];
                                                var oldOriginal = rows[irow]["Origin"];
                                                if (newModel != oldModel) {
                                                    $.messager.alert('提示', '该条到货记录的型号，与之前匹配的到货记录型号不一致');
                                                    $(edRowNo.target).textbox("setValue", "");
                                                    return;
                                                } else {
                                                    if (newOriginal != oldOriginal) {
                                                        $.messager.alert('提示', '该条到货记录的产地，与之前匹配的到货记录产地不一致');
                                                        $(edRowNo.target).textbox("setValue", "");
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                $('#datagrid').datagrid('updateRow', {
                                    index: index,
                                    row: {
                                        RowNo: newValue
                                    }
                                });

                                MergeCells('datagrid', 'CaseNo', 'CaseNo,ck');
                            }
                        });
                    }

                    var edName = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'Name' });
                    if (edName) {
                        $(edName.target).textbox({
                            onChange: function (newValue, oldValue) {
                                $('#datagrid').datagrid('updateRow', {
                                    index: index,
                                    row: {
                                        Name: newValue
                                    }
                                });
                                MergeCells('datagrid', 'CaseNo', 'CaseNo,ck');
                            }
                        });
                    }
                } else {
                    $('#datagrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                editIndex = undefined;
            }
        }
    </script>
    <script type="text/javascript">
        var index = -1;
        function onCheck(rowIndex, rowData) {
            if (index == -1) {
                index = rowIndex;
                var productid = rowData["CaseNo"];
                var rows = $("#datagrid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i]["CaseNo"] == productid && i != rowIndex) {
                        $("#datagrid").datagrid("checkRow", i);
                    }
                }
                index = -1;
            }
        }
        function onUncheck(rowIndex, rowData) {
            if (index == -1) {
                index = rowIndex;
                var productid = rowData["CaseNo"];
                var rows = $("#datagrid").datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i]["CaseNo"] == productid && i != rowIndex) {
                        $("#datagrid").datagrid("uncheckRow", i);
                    }
                }
                index = -1;
            }
        }
    </script>
    <script>
        /**
  * EasyUI DataGrid根据字段动态合并单元格
  * @param fldList 要合并table的id
  * @param fldList 要合并的列,用逗号分隔(例如："name,department,office");
  */
        function MergeCells(tableID, baseCol, fldList) {
            var dg = $('#' + tableID);
            var fldName = baseCol;
            var RowCount = dg.datagrid("getRows").length;
            var span;
            var PerValue = "";
            var CurValue = "";
            for (row = 0; row <= RowCount; row++) {
                if (row == RowCount) {
                    CurValue = "";
                }
                else {
                    CurValue = dg.datagrid("getRows")[row][fldName];
                }
                if (PerValue == CurValue) {
                    span += 1;
                }
                else {
                    var index = row - span;
                    $.each(fldList.split(","), function (i, val) {
                        dg.datagrid('mergeCells', {
                            index: index,
                            field: val,
                            rowspan: span,
                            colspan: null
                        });
                    });
                    CaseNoCount.add(PerValue, span);
                    span = 1;
                    PerValue = CurValue;
                }
            }
        }
    </script>
    <script>       
        Number.prototype.toFixed = function (s) {
            var changenum = (parseInt(this * Math.pow(10, s) + 0.5) / Math.pow(10, s)).toString();
            var index = changenum.indexOf(".");
            if (index < 0 && s > 0) {
                changenum = changenum + ".";
                for (i = 0; i < s; i++) {
                    changenum = changenum + "0";
                }

            } else {
                index = changenum.length - index;
                for (i = 0; i < (s - index) + 1; i++) {
                    changenum = changenum + "0";
                }

            }
            return changenum;
        }
    </script>
    <script>
        function SplitOrder() {
            var orderID = $("#OrderID").val();
            var selectedRows = $("#datagrid").datagrid("getChecked");
            var Rows = $("#datagrid").datagrid("getRows");
            if (selectedRows.length < 1) {
                $.messager.alert('提示', '请勾选箱号！');
                return;
            }          
            var SplitInfo = [];
            for (var i = 0; i < selectedRows.length; i++) {

                if (selectedRows[i]["UnitPrice"] == null || selectedRows[i]["UnitPrice"] == "") {
                    $.messager.alert('提示', '请填写单价');
                    return;
                }

                if (selectedRows[i]["TotalPrice"] == null || selectedRows[i]["TotalPrice"] == "") {
                    $.messager.alert('提示', '请填写总价');
                    return;
                }

                if (selectedRows[i]["Name"] == null || selectedRows[i]["Name"] == "") {
                    $.messager.alert('提示', '请填写品名');
                    return;
                }

                if (selectedRows[i]["Unit"] == null || selectedRows[i]["Unit"] == "") {
                    $.messager.alert('提示', '请填写单位');
                    return;
                }

                if (Rows[i]["Quantity"] == 0) {
                    $.messager.alert('提示', '到货数量不能为0');
                    return;
                }

                SplitInfo.push({
                    OrderID:orderID,
                    ID: selectedRows[i]["ID"],
                    CaseNo: selectedRows[i]["CaseNo"],
                    RowNo: selectedRows[i]["RowNo"],
                    Manufacturer: selectedRows[i]["Manufacturer"],
                    Model: selectedRows[i]["Model"],
                    Origin: selectedRows[i]["Origin"],
                    Quantity: selectedRows[i]["Quantity"],
                    UnitPrice: selectedRows[i]["UnitPrice"],
                    TotalPrice: selectedRows[i]["TotalPrice"],
                    Unit: selectedRows[i]["Unit"],
                    Name: selectedRows[i]["Name"],
                    Batch: selectedRows[i]["Batch"],
                });
            }

            $.messager.confirm('确认', '请您再次确认将已勾选箱号拆分报关？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=SplitCheck', { Model: JSON.stringify(SplitInfo) }, function (data) {
                        var Result = JSON.parse(data);
                        MaskUtil.unmask();
                        if (Result.canContinue) {
                            if (Result.result) {
                                $.messager.confirm('确认', Result.info, function (success) {
                                    if (success) {
                                        PostSplit(JSON.stringify(SplitInfo));
                                    }
                                });
                            } else {
                                PostSplit(JSON.stringify(SplitInfo));
                            }
                        } else {
                            $.messager.alert('提示', Result.info);
                        }
                    });
                }
            });
        }

        function PostSplit(PostData) {
            MaskUtil.mask();
            $.post('?action=SplitDeclare', { Model: PostData }, function (data) {
                var result = JSON.parse(data);
                MaskUtil.unmask();
                $.messager.alert('消息', result.message, 'info', function () {
                    if (result.success) {
                        closeWin();
                    } else {

                    }
                });
            });
        }
    </script>
    <script>
        function ConfirmOrder() {
            var orderID = $("#OrderID").val();
            var Rows = $("#datagrid").datagrid("getRows");



            var ConfirmInfo = [];
            for (var i = 0; i < Rows.length; i++) {

                if (Rows[i]["UnitPrice"] == null || Rows[i]["UnitPrice"] == "") {
                    $.messager.alert('提示', '请填写单价');
                    return;
                }

                if (Rows[i]["TotalPrice"] == null || Rows[i]["TotalPrice"] == "") {
                    $.messager.alert('提示', '请填写总价');
                    return;
                }

                if (Rows[i]["Name"] == null || Rows[i]["Name"] == "") {
                    $.messager.alert('提示', '请填写品名');
                    return;
                }

                if (Rows[i]["Unit"] == null || Rows[i]["Unit"] == "") {
                    $.messager.alert('提示', '请填写单位');
                    return;
                }

                if (Rows[i]["Quantity"] == 0) {
                    $.messager.alert('提示', '到货数量不能为0');
                    return;
                }

                ConfirmInfo.push({
                    OrderID:orderID,
                    ID: Rows[i]["ID"],
                    CaseNo: Rows[i]["CaseNo"],
                    RowNo: Rows[i]["RowNo"],
                    Manufacturer: Rows[i]["Manufacturer"],
                    Model: Rows[i]["Model"],
                    Origin: Rows[i]["Origin"],
                    Quantity: Rows[i]["Quantity"],
                    UnitPrice: Rows[i]["UnitPrice"],
                    TotalPrice: Rows[i]["TotalPrice"],
                    Unit: Rows[i]["Unit"],
                    Name: Rows[i]["Name"],
                    Batch: Rows[i]["Batch"],
                });
            }

            $.messager.confirm('确认', '请您再次确认该订单是否继续报关？', function (success) {
                if (success) {
                    $.post('?action=ConfirmCheck', { Model: JSON.stringify(ConfirmInfo) }, function (data) {
                        var Result = JSON.parse(data);
                        if (Result.canContinue) {
                            if (Result.result) {
                                $.messager.confirm('确认', Result.info, function (success) {
                                    if (success) {
                                        MaskUtil.mask();
                                        $.post('?action=OrderDeliveryConfirm', { Model: JSON.stringify(ConfirmInfo) }, function (data) {
                                            MaskUtil.unmask();
                                            var result = JSON.parse(data);
                                            $.messager.alert('消息', result.message, 'info', function () {
                                                if (result.success) {
                                                    closeWin();
                                                } else {

                                                }
                                            });
                                        });
                                    }
                                });
                            } else {
                                MaskUtil.mask();
                                $.post('?action=OrderDeliveryConfirm', { Model: JSON.stringify(ConfirmInfo) }, function (data) {
                                    MaskUtil.unmask();
                                    var result = JSON.parse(data);
                                    $.messager.alert('消息', result.message, 'info', function () {
                                        if (result.success) {
                                            closeWin();
                                        } else {

                                        }
                                    });
                                });
                            }
                        } else {
                            $.messager.alert('提示', Result.info);
                        }

                    });
                }
            });
        }

        function ReturnCheck() {
            var orderID = $("#OrderID").val();
            $.post('?action=IsOrderReturned', { OrderID: orderID }, function (isReturned) {
                if (isReturned) {
                    $.messager.alert('提示', '该订单已退回!');
                } else {
                    Return(orderID);
                }
            })
        }

        function Return(OrderID) {
            var url = location.pathname.replace(/Match.aspx/ig, 'ReturnReason.aspx') + '?ID=' + OrderID;
            top.$.myWindow({
                iconCls: "icon-man",
                url: url,
                noheader: false,
                title: '订单退回原因',
                width: '400px',
                height: '260px',
                onClose: function () {

                }
            });
        }

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div class="easyui-panel" style="width: 100%; height: 100%; border: 0px;">
        <div style="margin-left: 5px; margin-top: 10px">
            <label style="font-size: 16px; font-weight: 600; color: orangered">订单信息</label>
        </div>
        <div style="text-align: center; height: 40%; margin: 5px;">
            <table id="products">
                <thead>
                    <tr>
                        <th data-options="field:'RowNo',align:'center'" style="width: 150px">序号</th>
                        <th data-options="field:'Name',align:'center'" style="width: 150px">品名</th>
                        <th data-options="field:'Manufacturer',align:'center'" style="width: 150px">品牌</th>
                        <th data-options="field:'Model',align:'center'" style="width: 150px">型号</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 150px">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 150px">数量</th>
                        <th data-options="field:'UnitPrice',align:'center'" style="width: 150px">单价</th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width: 150px">总价</th>
                        <th data-options="field:'Batch',align:'center'" style="width: 150px">批次号</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div style="margin-left: 5px; margin-top: 10px">
            <label style="font-size: 16px; font-weight: 600; color: orangered">到货信息</label>
        </div>
        <div style="text-align: center; height: 40%; margin: 5px;">
            <table id="datagrid">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true" style="width: 20px"></th>
                        <th data-options="field:'CaseNo',align:'center'" style="width: 100px">箱号</th>
                        <th data-options="field:'RowNo',align:'center', editor:'textbox'," style="width: 100px">序号</th>
                        <th data-options="field:'Batch',align:'center'" style="width: 100px">批号</th>
                        <th data-options="field:'Name',align:'center', editor:'textbox'," style="width: 150px">品名</th>
                        <th data-options="field:'Manufacturer',align:'center'" style="width: 150px">品牌</th>
                        <th data-options="field:'Model',align:'center'" style="width: 150px">型号</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 150px">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 150px">数量</th>
                        <th data-options="field:'Unit',align:'center',editor:'textbox'" style="width: 150px">单位</th>
                        <th data-options="field:'UnitPrice',align:'center',editor:'textbox'" style="width: 150px">单价</th>
                        <th data-options="field:'TotalPrice',align:'center',editor:'textbox'" style="width: 150px">总价</th>
                        <th data-options="field:'ID',align:'center'" style="display: none">ID</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div class="sub-container" style="height: 20px;">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ConfirmOrder()">确认</a>
            <a id="btnSplit" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cut'" onclick="SplitOrder()">拆分订单</a>
            <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="ReturnCheck()">退回</a>
            <input type="hidden" id="OrderID" />
        </div>
    </div>
</body>
</html>
