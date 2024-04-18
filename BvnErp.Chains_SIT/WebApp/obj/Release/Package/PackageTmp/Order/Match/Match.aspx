<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Match.aspx.cs" Inherits="WebApp.Order.Match.Match" %>

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
        var SplitInfo = [];
        var SplitTotalPrice = 0;//需要拆分的订单金额
        var totalSelectAmount = 0;//已经勾选的总金额
        var DeclarePrice = 0;//01订单的总金额
        var PaidExchangeAmount = 0;//01订单的预付汇金额
        $(function () {
            var ID = getQueryString("ID");
            $("#OrderID").val(ID);

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
                                var qty = row.Qty;
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
 
            SplitInfo = [];
            SplitTotalPrice = 0;
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

                if (Rows[i]["Qty"] == 0) {
                    $.messager.alert('提示', '到货数量不能为0');
                    return;
                }

                SplitInfo.push({
                    OrderID: orderID,
                    ID: selectedRows[i]["ID"],
                    CaseNo: selectedRows[i]["CaseNo"],                    
                    Brand: selectedRows[i]["Brand"],
                    Model: selectedRows[i]["Model"],
                    Origin: selectedRows[i]["Origin"],
                    Qty: selectedRows[i]["Qty"],
                    UnitPrice: selectedRows[i]["UnitPrice"],
                    TotalPrice: selectedRows[i]["TotalPrice"],
                    Unit: selectedRows[i]["Unit"],
                    Name: selectedRows[i]["Name"],
                    BatchNo: selectedRows[i]["BatchNo"],
                    OrderItemID: selectedRows[i]["OrderItemID"],
                    OrderItemQty:selectedRows[i]["OrderItemQty"],
                });

                SplitTotalPrice += selectedRows[i]["TotalPrice"];
            }

            //拆分订单处理付汇
            MaskUtil.mask();
            $.post('?action=PayExchangeCheck', { OrderID: orderID, SplitTotalPrice: SplitTotalPrice }, function (data) {
                var Result = JSON.parse(data);
                MaskUtil.unmask();
                if (Result.success && Result.check) {
                    //被拆分订单已有付汇，并且拆分后金额小于已付汇金额  需要弹出处理付汇页面

                    //$('#setPayExchange-datagrid').datagrid('load');
                    //$('#setPayExchange-dialog').dialog('open');

                    DeclarePrice = Result.DeclarePrice;
                    PaidExchangeAmount = Result.PaidExchangeAmount;

                    ShowPayExchangeDialog(orderID);
                } else {
                    PrePostSplit('');
                }
            });
        }

        //订单拆分的动作
        function PrePostSplit(splitPayexchange) {
            var msg = '';
            if (splitPayexchange != '') {
                msg = '<br/>同时调整付汇金额？';
            }

            $.messager.confirm('确认', '请您再次确认将已勾选箱号拆分报关？' + msg, function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=SplitCheck', { Model: JSON.stringify(SplitInfo) }, function (data) {
                        var Result = JSON.parse(data);
                        MaskUtil.unmask();
                        if (Result.canContinue) {                           
                            PostSplit(JSON.stringify(SplitInfo), splitPayexchange);
                        } else {
                            $.messager.alert('提示', Result.info);
                        }
                    });
                }
                else {
                    $("#btnPESplit").linkbutton("enable");
                    $("#btnPESkip").linkbutton("enable");
                }
            });
        }

        //拆分
        function PostSplit(PostData, SplitPE) {
            MaskUtil.mask();
            //拆分
            $.post('?action=SplitDeclare', { Model: PostData, SplitPE: SplitPE }, function (data) {
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

        //需要处理付汇申请
        function PESplit() {

            $("#btnPESplit").linkbutton("disable");
            $("#btnPESkip").linkbutton("disable");

            //付汇拆分的数据
            var checkedItems = $('#setPayExchange-datagrid').datagrid('getChecked');
            var splitPayexchange = '';
            if (checkedItems.length < 1) {
                $.messager.alert('消息', '请勾选需要拆分的付汇申请！', 'info', function () {
                    $("#btnPESplit").linkbutton("enable");
                    $("#btnPESkip").linkbutton("enable");
                });
                return;
            } else {

                //订单01的金额
                var remainAmount = DeclarePrice - SplitTotalPrice;
                //需要分给02的付汇金额
                var needSplitAmount = PaidExchangeAmount - remainAmount;

                //勾选不够，不允许拆分
                if (Number(totalSelectAmount) < Number(needSplitAmount)) {
                    $.messager.alert('消息', '勾选的付汇金额需大于等于 ' + needSplitAmount.toFixed(2), 'info', function () {
                        $("#btnPESplit").linkbutton("enable");
                        $("#btnPESkip").linkbutton("enable");
                    });
                    return;
                }
                $.each(checkedItems, function (index, item) {
                    splitPayexchange += item.ID + ',';
                });
            }

            PrePostSplit(splitPayexchange);
        }

        //不需要处理付汇申请
        function PESkip() {

            $("#btnPESplit").linkbutton("disable");
            $("#btnPESkip").linkbutton("disable");
            $('#setPayExchange-dialog').dialog('close');
            PrePostSplit('');
        }

        //显示付汇弹框
        function ShowPayExchangeDialog(orderId) {

            $("#btnPESplit").linkbutton("enable");
            $("#btnPESkip").linkbutton("enable");

            //订单01的金额
            var remainAmount = DeclarePrice - SplitTotalPrice;
            //需要分给02的付汇金额
            var needSplitAmount = PaidExchangeAmount - remainAmount;
            $('#needsplitSpan').html(Number(needSplitAmount).toFixed(2));

            if (orderId) {
                $('#setPayExchange-datagrid').datagrid({
                    url: location.pathname.replace(/Match.aspx/ig, 'Match.aspx?action=PayExchangeRecord&OrderID=' + orderId),
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
                    onCheck: function (rowIndex, rowData) {
                        var allSelections = $(this).datagrid('getSelections');
                        totalSelectAmount = 0;
                        for (var i = 0; i < allSelections.length; i++) {
                            totalSelectAmount += allSelections[i].Amount;
                        }

                        //勾选的总金额还不够需要拆分的，可以勾选，否则不能再勾选（已经够分给02了）
                        if ((Number(totalSelectAmount) <= (Number(needSplitAmount) + rowData.Amount)) || allSelections.length == 1) {
                            //$(this).datagrid('selectRow', rowIndex);
                            $('#totalSelectSpan').html(totalSelectAmount.toFixed(2));
                        }
                        else {
                            $(this).datagrid('unselectRow', rowIndex);
                        }
                    },
                    //取消选中一行
                    onUncheck: function (rowIndex, rowData) {

                        totalSelectAmount -= rowData.Amount;
                    },
                });

                $('#setPayExchange-dialog').dialog('open');
            }
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

                if (Rows[i]["Qty"] == 0) {
                    $.messager.alert('提示', '到货数量不能为0');
                    return;
                }

                ConfirmInfo.push({
                    OrderID: orderID,
                    ID: Rows[i]["ID"],
                    CaseNo: Rows[i]["CaseNo"],                    
                    Brand: Rows[i]["Brand"],
                    Model: Rows[i]["Model"],
                    Origin: Rows[i]["Origin"],
                    Qty: Rows[i]["Qty"],
                    UnitPrice: Rows[i]["UnitPrice"],
                    TotalPrice: Rows[i]["TotalPrice"],
                    Unit: Rows[i]["Unit"],
                    Name: Rows[i]["Name"],
                    BatchNo: Rows[i]["BatchNo"],
                    OrderItemID: Rows[i]["OrderItemID"],
                    OrderItemQty:Rows[i]["OrderItemQty"],
                });
            }

            $.messager.confirm('确认', '请您再次确认该订单是否继续报关？', function (success) {
                if (success) {
                    $.post('?action=ConfirmCheck', { Model: JSON.stringify(ConfirmInfo) }, function (data) {
                        var Result = JSON.parse(data);
                        if (Result.canContinue) {
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
            <label style="font-size: 16px; font-weight: 600; color: orangered">到货信息</label>
        </div>
        <div style="text-align: center; height: 80%; margin: 5px;">
            <table id="datagrid">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true" style="width: 20px"></th>
                        <th data-options="field:'CaseNo',align:'center'" style="width: 100px">箱号</th>                       
                        <th data-options="field:'BatchNo',align:'center'" style="width: 100px">批号</th>
                        <th data-options="field:'Name',align:'center', editor:'textbox'," style="width: 150px">品名</th>
                        <th data-options="field:'Brand',align:'center'" style="width: 150px">品牌</th>
                        <th data-options="field:'Model',align:'center'" style="width: 150px">型号</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 150px">产地</th>
                        <th data-options="field:'OrderItemQty',align:'center'" style="width: 100px">订单数量</th>
                        <th data-options="field:'Qty',align:'center'" style="width: 100px">到货数量</th>
                        <th data-options="field:'Unit',align:'center',editor:'textbox'" style="width: 150px">单位</th>
                        <th data-options="field:'UnitPrice',align:'center',editor:'textbox'" style="width: 150px">单价</th>
                        <th data-options="field:'TotalPrice',align:'center',editor:'textbox'" style="width: 150px">总价</th>
                        <%-- <th data-options="field:'OrderItemID',align:'center'" style="display: none">ID</th>--%>
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

<!------------------------------------------------------------------------------------------------->
<div id="setPayExchange-dialog" class="easyui-dialog" title="处理付汇" style="width: 850px; height: 500px;"
    data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
    <div style="display: block; height: 82%;border-bottom: 1px solid lightgray;">
        <table id="setPayExchange-datagrid" data-options="
            nowrap:false,
            border:false,
            autoRowHeight:true,
            checkOnSelect:true,
            selectOnCheck:true,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            rownumbers:false">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 5px;"></th>
                    <th data-options="field:'PayExchangeApplyID',align:'center'" style="width: 15px;">申请编号</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 13px;">申请日期</th>
                    <th data-options="field:'SupplierEnglishName',align:'left'" style="width: 18px;">供应商名称</th>
                    <th data-options="field:'BankName',align:'left'" style="width: 18px;">银行名称</th>
                    <th data-options="field:'BankAccount',align:'left'" style="width: 15px;">银行账号</th>
                    <th data-options="field:'PayExchangeApplyStatus',align:'center'" style="width: 10px;">状态</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 10px;">付汇金额</th>
                    <%--                    <th data-options="field:'IsAdvanceMoney',align:'center'" style="width: 10px;">是否垫资</th>
                    <th data-options="field:'FatherID',align:'center'" style="width: 10px;">父级ID</th>--%>
                </tr>
            </thead>
        </table>
    </div>
    <div style="display:block;height:15%;margin: 6px;">
        <div>
            <p>该订单已经有付汇申请，请勾选付汇申请点击<span style="color: limegreen">【继续】</span>，同步拆分付汇申请；
                需拆分金额 <span id="needsplitSpan" style="color:red;">0</span>； 
                已勾选金额 <span id="totalSelectSpan" style="color:darkgreen;">0</span>；
            </p>
            <p>点击<span style="color: orangered">【跳过】</span>，不对付汇申请进行任何操作，只拆分订单；</p>
        </div>
        <div style="text-align:right;margin-right:30px;">
            <a id="btnPESplit" href="javascript:void(0);" class="easyui-linkbutton" style="margin-right: 20px;" data-options="iconCls:'icon-cut'" onclick="PESplit()">继续</a>
            <a id="btnPESkip" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="PESkip()">跳过</a>
        </div>
    </div>

</div>


</html>
