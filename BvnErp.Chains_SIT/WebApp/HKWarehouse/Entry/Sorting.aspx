<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sorting.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.Sorting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库房分拣</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var WaybillCodeData = eval('(<%=this.Model.OrderWaybillData%>)');
        var wrapTypeData = eval('(<%=this.Model.WarpTypeData%>)');
        var SortingRequireData = eval('(<%=this.Model.SortingRequireData%>)');
        var WraptypeValue = eval('(<%=this.Model.WraptypeValue%>)');
        var originData = eval('(<%=this.Model.OriginData%>)');
        var ID = getQueryString('ID');
        var OrderID = getQueryString('OrderID');
        var EntryStatus = getQueryString('EntryStatus');
        //页面加载时
        $(function () {

            $('#wayBillGrid').myDatagrid({
                actionName: 'LoadWayBill'
            });

            $('#noticeItemGrid').myDatagrid({
                columns: [[
                    { field: 'ck', width: 70, align: 'center', checkbox: true },
                    { field: 'ProductModel', title: '型号', width: 100, align: 'center' },
                    { field: 'ProductName', title: '品名', width: 100, align: 'left' },
                    { field: 'OrderQuantity', title: '数量', width: 50, align: 'left' },
                    { field: 'Quantity', title: '装箱数量', width: 50, align: 'left', editor: { type: 'textbox', options: { required: true, validType: 'length[1,50]' } } },
                    {
                        field: 'Manufacturer', title: '品牌', width: 50, align: 'left', editor: {
                            type: 'textbox', options: {
                                required: true, validType: 'length[1,50]',
                                onChange: function (newValue, oldValue) {
                                    if (editIndex == undefined)
                                        return;
                                    var row = $('#noticeItemGrid').datagrid('getRows')[editIndex];
                                    $.post('?action=ChangeManufacturer',
                                        {
                                            Manufacturer: newValue,
                                            OrderItemID: row.OrderItemID
                                        },
                                        function (result) {
                                            var rel = JSON.parse(result);
                                            if (!rel.success) {
                                                $.messager.alert('消息', rel.message, 'info', function () {
                                                    if (rel.success) {

                                                    }
                                                });
                                            }
                                        });
                                }
                            }
                        }
                    },
                    {
                        field: 'Origin', title: '产地', width: 100, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < originData.length; i++) {
                                if (originData[i].OriginValue == value) return originData[i].OriginText;
                            }
                            return value;
                        },
                        editor: {
                            type: 'combobox', options: {
                                data: originData, valueField: "OriginValue", required: true,missingMessage:'请选择产地', validType: 'OriginValid', textField: "OriginText", onSelect: function (newValue, oldValue) {
                                    if (editIndex == undefined)
                                        return;
                                    var row = $('#noticeItemGrid').datagrid('getRows')[editIndex];
                                    $.post('?action=ChangeOrigin',
                                        {
                                            OriginValue: newValue.OriginValue,
                                            OrderItemID: row.OrderItemID
                                        },
                                        function (result) {
                                            var rel = JSON.parse(result);
                                            $.messager.alert('消息', rel.message, 'info', function () {
                                                if (rel.success) {

                                                }
                                            });
                                        });
                                }
                            }
                        }
                    },
                    {
                        field: 'Batch', title: '批次号', width: 50, align: 'left', editor: {
                            type: 'textbox', options: {
                                validType: 'length[1,50]',
                                onChange: function (newValue, oldValue) {
                                    if (editIndex == undefined || newValue == "" || newValue == null)
                                        return;
                                    var row = $('#noticeItemGrid').datagrid('getRows')[editIndex];
                                    $.post('?action=ChangeBatch',
                                        {
                                            Batch: newValue,
                                            OrderItemID: row.OrderItemID
                                        },
                                        function (result) {
                                            var rel = JSON.parse(result);
                                            if (!rel.success) {
                                                $.messager.alert('消息', rel.message, 'info', function () {
                                                    if (rel.success) {

                                                    }
                                                });
                                            }
                                        });
                                }
                            }
                        }
                    },
                    { field: 'optEdit', title: '操作', width: 100, align: 'center', formatter: NoticeItemOperate }
                ]],
                loadFilter: function (data1) {
                    if (data1.rows.length > 0) {
                        $('#pakinglist').show();
                        $(".divPacking").show();
                        $('.divSeal').hide();
                        for (var index = 0; index < data1.rows.length; index++) {
                            var row = data1.rows[index];
                            for (var name in row.item) {
                                row[name] = row.item[name];
                            }
                            delete row.item;
                        }
                        return data1;
                    }
                    else {
                        $('#pakinglist').hide();
                        $('.divSeal').show();
                        $(".divPacking").hide();
                        if (EntryStatus == 3) {
                            $('.divSeal').hide();
                            $('.divSealed').hide();
                        } else {
                            $('.divSeal').show();
                        }
                    }
                },
                onCheck: function () {
                    UpdateQuantity();
                },
                onUncheck: function () {
                    UpdateQuantity();
                },
                onCheckAll: function () {
                    UpdateQuantity();
                },
                onUncheckAll: function () {
                    UpdateQuantity();
                },
            });

            $.extend($.fn.validatebox.defaults.rules, {
                OriginValid: {
                    validator: function (value) {
                        var val = $.trim(value);
                        var oriarr = val.split(' ');
                        if ('(<%=this.Model.OriginData%>)'.indexOf('"' + oriarr[0] + '"') < 0) {
                            return false;
                        }
                        else
                            return true;
                    },
                    message: '产地不正确!'
                }
            });

            $('#packedGrid').myDatagrid({
                pagination: false,
                nowrap: false,
            });
            //隐藏和显示国际快递信息
            $('#panel2').hide();
            $('input[name="radioExpress"]').change(function () {
                if ($('input[name="radioExpress"][value="1"]').prop("checked")) {
                    $('#panel2').show();
                    $('#WaybillCodes').combobox({
                        required: true,
                        disabled: false,
                    });
                    $('#wayBillGrid').myDatagrid('reload');
                } else {
                    $('#panel2').hide();
                    $('#WaybillCodes').combobox({
                        required: false,
                        disabled: true,
                    });
                }
            })

            //初始化订单编号
            $('#OrderID').text(OrderID);

            //初始化运单下拉框数据
            $('#WaybillCodes').combobox({
                disabled: true,
                data: WaybillCodeData,
            });

            //设置系统当前时间
            var curr_time = new Date();
            var str = curr_time.getMonth() + 1 + "/";
            str += curr_time.getDate() + "/";
            str += curr_time.getFullYear() + " ";
            str += curr_time.getHours() + ":";
            str += curr_time.getMinutes() + ":";
            str += curr_time.getSeconds();
            $('#PackingDate').datebox('setValue', str);
            $('#NewPackingDate').datebox('setValue', str);
            // 初始化包装类型
            $("#PackingType").combogrid({
                idField: "Code",
                textField: "Name",
                data: wrapTypeData,
                fitColumns: true,
                mode: "local",
                columns: [[
                    { field: 'Code', title: 'Code', width: 50, align: 'center', sortable: true },
                    { field: 'Name', title: 'Name', width: 120, align: 'center' },
                ]],
                onSelect: function () {
                    var grid = $("#PackingType").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                },
                //设置默认值代码
                onLoadSuccess: function () {
                    $("#PackingType").combogrid("setValue", WraptypeValue);
                },

                keyHandler: {
                    up: function () { },
                    down: function () { },
                    enter: function () { },
                    query: function (data) {
                        //动态搜索 
                        $("#PackingType").combogrid("grid").datagrid("reload", { 'keyword': data });
                        $("#PackingType").combogrid("setValue", data);
                    }
                }
            });

            ShowWayBill();
            InitSortingRequire();
        });

        function ShowWayBill() {
            if (EntryStatus == 3) {
                $("#panel2").show();
            }
        }

        function InitSortingRequire() {
            if (SortingRequireData != null) {
                $("#SortingRequire").text(SortingRequireData["SortingRequireText"]);
                if (SortingRequireData["SortingRequireValue"] == '<%=Needs.Ccs.Services.Enums.SortingRequire.Packed.GetHashCode()%>') {
                    $("#SortingException").hide();
                }
                else {
                    $("#SortingException").show();
                }
            }
        }

        //删除国际快递
        function DeleteWayBill(Index) {
            if (EntryStatus == 3) {
                $.messager.alert("提示", "已封箱不能删除运单");
                return;
            }
            $('#wayBillGrid').datagrid('selectRow', Index);
            var rowdata = $('#wayBillGrid').datagrid('getSelected');

            $.messager.confirm('确认', '是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=DeleteWayBill', { ID: rowdata.ID, OrderID: OrderID }, function (data) {
                        $('#wayBillGrid').myDatagrid('reload');
                        //更新运单数据
                        data = eval(data);
                        $('#WaybillCodes').combobox({
                            disabled: false,
                            data: eval(data)
                        });
                    })
                }
            });
        }

        //新增国际快递
        function AddExpress(id) {
            var url = location.pathname.replace(/Sorting.aspx/ig, 'AddExpress.aspx?ID=' + ID + "&OrderID=" + OrderID);
            self.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增国际快递',
                width: '400px',
                height: '300px',
                onClose: function () {
                    $('#wayBillGrid').myDatagrid('reload');
                }
            });
        }

        //国际快递操作
        function ExpressOperate(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton" style="margin:3px;color:#0094ff" onclick="DeleteWayBill(' + index + ')">删除</a>';
            return buttons;
        }

        //待装箱产品操作
        function NoticeItemOperate(val, row, index) {
            if (row.IsSportCheck) {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton" style="margin:3px;color:#0094ff" onclick="CheckProduct(\'' + row.ID + '\')" >抽检异常</a>';
                return buttons;
            }
        }

        //已装箱产品操作
        function PackedOperate(val, row, index) {
            if (row["StatusValue"] == '<%=Needs.Ccs.Services.Enums.PackingStatus.UnSealed.GetHashCode()%>') {
                var buttons = '<a href="javascript:void(0);"  class="easyui-linkbutton deletePacking" style="margin:3px;color:#0094ff" onclick="DeletePacking(' + index + ')">删除</a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton " style="margin:3px;color:#0094ff" onclick="EditPacking(' + index + ')">修改箱号</a>';
                return buttons;
            }
            if (row["StatusValue"] == '<%=Needs.Ccs.Services.Enums.PackingStatus.Sealed.GetHashCode()%>') {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton " style="margin:3px;color:#0094ff" onclick="CancelSealed(' + index + ')">取消封箱</a>';
                return buttons;
            }
        }

        function UpdateQuantity() {
            var rows = $('#noticeItemGrid').datagrid('getChecked');
            if (rows) {
                var quantity = 0;
                for (var index = 0; index < rows.length; index++) {
                    var row = rows[index];
                    quantity += Number(row["Quantity"]);
                }
                $("#Quantity").textbox('setValue', quantity);
            }


        }




    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#noticeItemGrid').datagrid('validateRow', editIndex)) {
                $('#noticeItemGrid').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    VerifyQuantity();

                    $('#noticeItemGrid').datagrid('selectRow', index);
                    $('#noticeItemGrid').datagrid('beginEdit', index);
                    editIndex = index;
                    var editors = $('#noticeItemGrid').datagrid('getEditors', editIndex);
                    var editor = $(editors[2].target);
                    editor.combobox("textbox").bind("blur", function (newValue, oldValue) {
                        if (editIndex == undefined)
                            return;
                        var row = $('#noticeItemGrid').datagrid('getRows')[editIndex];
                        var originValue = editor.combobox("getValue");

                        $.post('?action=ChangeOrigin',
                            {
                                OriginValue: originValue,
                                OrderItemID: row.OrderItemID
                            },
                            function (result) {
                                var rel = JSON.parse(result);
                                $.messager.alert('消息', rel.message, 'info', function () {
                                    if (rel.success) {

                                    }
                                });
                            });
                    });
                    //$("#noticeItemGrid").datagrid('endEdit',index);
                } else {
                    $('#noticeItemGrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                VerifyQuantity(index);
                editIndex = undefined;
            }
        }

        function VerifyQuantity(Index) {
            if (Index != null) {
                $('#noticeItemGrid').datagrid('acceptChanges');
                $('#noticeItemGrid').datagrid('selectRow', Index);
                var row = $('#noticeItemGrid').datagrid('getSelected');
                if (Number(row["Quantity"]) > Number(row["OrderQuantity"])) {
                    $.messager.alert("消息", "装箱数量不能大于可装箱数量");
                    row["Quantity"] = row["OrderQuantity"];
                    var index = $('#noticeItemGrid').datagrid('getRowIndex', row);
                    $('#noticeItemGrid').datagrid('refreshRow', index);
                }
                if (Number(row["Quantity"]) < 0) {
                    $.messager.alert("消息", "装箱数量不小于0");
                    row["Quantity"] = row["OrderQuantity"];
                    var index = $('#noticeItemGrid').datagrid('getRowIndex', row);
                    $('#noticeItemGrid').datagrid('refreshRow', index);
                }
            }
            else {
                //装箱数量不大于订单数量  
                var rows = $('#noticeItemGrid').datagrid('getChanges');
                for (var i = 0; i < rows.length; i++) {
                    var row = rows[i];
                    if (Number(row["Quantity"]) > Number(row["OrderQuantity"])) {
                        $.messager.alert("消息", "装箱数量不能大于可装箱数量");
                        row["Quantity"] = row["OrderQuantity"];
                        var index = $('#noticeItemGrid').datagrid('getRowIndex', row);
                        $('#noticeItemGrid').datagrid('refreshRow', index);
                    }
                    if (Number(row["Quantity"]) < 0) {
                        $.messager.alert("消息", "装箱数量不小于0");
                        row["Quantity"] = row["OrderQuantity"];
                        var index = $('#noticeItemGrid').datagrid('getRowIndex', row);
                        $('#noticeItemGrid').datagrid('refreshRow', index);
                    }
                }
            }

            $('#noticeItemGrid').datagrid('acceptChanges');
            //更新装箱数量
            UpdateQuantity();
        }

     <%--   function VerifyOrigin(index) {
            $('#noticeItemGrid').datagrid('acceptChanges');
            $('#noticeItemGrid').datagrid('selectRow', index);
            var row = $('#noticeItemGrid').datagrid('getSelected');
            if ('(<%=this.Model.OriginData%>)'.indexOf('"' + row.Origin + '"') < 0) {
                return false;
            }
            else
                return true;
        }--%>

        //修改产品
        function EditProduct(OrderItemID) {
            var url = location.pathname.replace(/Sorting.aspx/ig, 'EditProduct.aspx?OrderItemID=' + OrderItemID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '修改产品',
                width: '420px',
                height: '300px',
                onClose: function () {
                    $('#noticeItemGrid').myDatagrid('reload');
                }
            });
        }

        ///抽检异常
        function CheckProduct(ID) {
            $.messager.confirm('确认', '是否抽检异常？', function (success) {
                if (success) {
                    $.post('?action=CheckProduct',
                        {
                            ID: ID
                        },
                        function (result) {
                            var rel = JSON.parse(result);
                            $.messager.alert('消息', rel.message, 'info', function () {
                                if (rel.success) {
                                    //TODO:
                                }
                            });
                        })
                }
            });
        }

        //分拣异常
        function AbnormalSorting() {
            $.messager.confirm('确认', '是否分拣异常？', function (success) {
                if (success) {
                    $.post('?action=AbnormalSorting',
                        {
                            ID: ID,
                            OrderID: OrderID
                        },
                        function (result) {
                            var rel = JSON.parse(result);
                            $.messager.alert('消息', rel.message, 'info', function () {
                                if (rel.success) {

                                }
                            });
                        })
                }
            });
        }

        //装箱
        function Packing() {
            if (!$("#form1").form('validate')) {
                return;
            }
            if (!$("#form5").form('validate')) {
                return;
            }
            else {
                //验证是否勾选
                var Data = $('#noticeItemGrid').datagrid('getChecked');
                if (Data.length == 0) {
                    $.messager.alert("消息", "请勾选装箱产品");
                    return;
                }
                for (var i = 0; i < Data.length; i++) {
                    if (Data[i].Origin == "" || Data[i].Origin == null) {
                        $.messager.alert("消息", "装箱失败，产品" + Data[i].ProductModel + "原产地为空");
                        return;
                    }
                }

                //验证装箱数量
                var Quantity = $("#Quantity").textbox('getValue');
                if (Quantity == "0" || Quantity == "") {
                    $.messager.alert("消息", "装箱产品总数量为0");
                    return;
                }
                //验证正式箱号
                var BoxIndex = $("#BoxIndex").textbox('getValue');
                if (BoxIndex.split("-").length == 2) {
                    if (Data.length > 1) {
                        $.messager.alert("消息", "输入箱号为连续箱号，只能勾选一个装箱产品");
                        return;
                    }
                }
                if (BoxIndex.split("-").length - 1 > 1 || BoxIndex.substring(0, 1) == '-' || BoxIndex.split("-")[0].substring(0, 2) != 'WL' || (BoxIndex.split("-").length > 1 && BoxIndex.split("-")[1].substring(0, 2) != 'WL')) {
                    $("#BoxIndex").focus();
                    $.messager.alert("消息", "请输入正确的装箱箱号");
                    return;
                }
                var arry = BoxIndex.split('-');
                if (Number(arry[0].replace("WL", "")) < 1 || (BoxIndex.split("-").length > 1 && Number(arry[1].replace("WL", "")) < 1) || (BoxIndex.split("-").length > 1 && Number(arry[0].replace("WL", "")) >= Number(arry[1].replace("WL", "")))) {
                    $("#BoxIndex").focus();
                    $.messager.alert("消息", "请输入正确的装箱箱号");
                    return;
                }

                var ShelveNumber = $("#ShelveNumber").textbox('getValue');
                var Weight = $("#Weight").textbox('getValue');
                var WaybillCode = $("#WaybillCodes").combobox('getValue');
                var PackingType = $("#PackingType").combobox('getValue');
                var PackingDate = $("#PackingDate").datebox('getValue');
                MaskUtil.mask();//遮挡层
                $.post('?action=Packing', {
                    EntryNoticeID: ID,
                    OrderID: OrderID,
                    BoxIndex: BoxIndex,
                    ShelveNumber: ShelveNumber,
                    Weight: Weight,
                    Quantity: Quantity,
                    WaybillCode: WaybillCode,
                    PackingType: PackingType,
                    PackingDate: PackingDate,
                    Data: JSON.stringify(Data),
                }, function (result) {
                    MaskUtil.unmask();//关闭遮挡层
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            $('#noticeItemGrid').myDatagrid('reload');
                            $('#packedGrid').myDatagrid('reload');
                        }
                    });
                })
            }
        }
        //封箱
        function Sealed() {

            $.messager.confirm('确认', '再次确认是否封箱？', function (success) {
                if (success) {
                    $.post('?action=Sealed',
                        {
                            ID: ID,
                        },
                        function (result) {
                            var rel = JSON.parse(result);
                            $.messager.alert('消息', rel.message, 'info', function () {
                                if (rel.success) {
                                    var url = location.pathname.replace(/Sorting.aspx/ig, 'UnSortingList.aspx');
                                    //top.document.getElementById('ifrmain').src = url;
                                    window.parent.location.href = url;
                                }
                            });
                        })
                }
            });
        }
        //取消封箱
        function CancelSealed(Index) {
            $('#packedGrid').datagrid('selectRow', Index);
            var rowdata = $('#packedGrid').datagrid('getSelected');

            $.messager.confirm('确认', '再次确认是否取消封箱？', function (success) {
                if (success) {
                    $.post('?action=CancelSealed',
                        {
                            PackingID: rowdata.PackingID
                        },
                        function (result) {
                            var rel = JSON.parse(result);
                            $.messager.alert('消息', rel.message, 'info', function () {
                                if (rel.success) {
                                    $('#packedGrid').myDatagrid('reload');
                                }
                            });
                        })
                }
            });
        }
    </script>
    <script>
        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['BoxIndex'] == data.rows[i - 1]['BoxIndex']) {
                    mark += 1;
                    $("#packedGrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'BoxIndex',
                        rowspan: mark
                    });
                    $("#packedGrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'btnRemove',
                        rowspan: mark
                    });
                    $("#packedGrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'ck',
                        rowspan: mark
                    });
                }
                else {
                    mark = 1;
                }
            }
        }

        //删除装箱结果
        function DeletePacking(Index) {
            $('#packedGrid').datagrid('selectRow', Index);
            var rowdata = $('#packedGrid').datagrid('getSelected');
            $.messager.confirm('确认', '是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=DeletePacking', { PackingID: rowdata.PackingID, EntryNoticeID: ID, }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                $('#packedGrid').myDatagrid('reload');
                                $('#noticeItemGrid').myDatagrid('reload');

                                var data = $('#noticeItemGrid').datagrid('getData');
                                if (data.rows.length > 0) {
                                    $('#divPacking').show();
                                    $('.divSeal').hide();
                                } else {
                                    $('#divPacking').hide();
                                    $('.divSeal').show();

                                }
                            }
                        });
                    })
                }

            });
        }

        //修改箱号
        function EditPacking(Index) {
            $('#packedGrid').datagrid('selectRow', Index);
            var rowdata = $('#packedGrid').datagrid('getSelected');
            var PackingID = rowdata.PackingID;
            var url = location.pathname.replace(/Sorting.aspx/ig, 'EditCaseNumber.aspx?PackingID=' + PackingID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '修改箱号',
                width: '400px',
                height: '300px',
                onClose: function () {
                    $('#packedGrid').myDatagrid('reload');
                }
            });
        }
    </script>
    <style>
        input[type="radio"] {
            display: none;
        }

            input[type="radio"] + span {
                display: inline-block;
                width: 12px;
                height: 14px;
                vertical-align: middle;
                border-radius: 50%;
                border: 1px solid #808080;
                background-color: #f7eeee;
            }

            input[type="radio"]:checked + span {
                border: 1px solid #808080;
                background-color: #4cff00;
            }
    </style>
</head>
<body class="easyui-layout">
    <div id="data" class="easyui-layout" data-options="fit:true" style="margin-left: 5px; margin-top: 5px">
        <div data-options="region:'west',border:false,split:true" style="width: 73%; overflow: auto">
            <%--待装箱列表--%>
            <form id="form1">
                <div id="pakinglist" style="margin-bottom: 20px">
                    <table id="noticeItemGrid" title="待装箱产品" class="easyui-datagrid" style="width: 100%;" data-options="
                     fitColumns:true,
                    scrollbarSize:0,
                    fit:false,
                    border:true,
                    nowrap: false,
                    singleSelect:false,
                    checkOnSelect: false,
                    selectOnCheck: true,
                    onClickRow:onClickRow,
                    pagination: false,
                    queryParams:{ action: 'LoadNoticeItems' }">
                    </table>
                </div>
            </form>
            <%--已装箱产品列表--%>
            <table id="packedGrid" class="easyui-datagrid" title="已装箱" data-options="
                fitColumns:true,
                scrollbarSize:0,
                fit:true,
                nowrap: false,
                onLoadSuccess: onLoadSuccess,
                queryParams:{ action: 'LoadPackedProduct' }">
                <thead>
                    <tr>
                        <th data-options="field:'BoxIndex',align:'center',width:'50px'">箱号</th>
                        <th data-options="field:'Model',align:'left',width:'100px'">型号</th>
                        <th data-options="field:'Name',align:'left',width:'100px'">品名</th>
                        <th data-options="field:'Manufacturer',align:'left',width:'50px'">品牌</th>
                        <th data-options="field:'Quantity',align:'left',width:'40px'">数量</th>
                        <th data-options="field:'Origin',align:'left',width:'30px'">产地</th>
                        <%--<th data-options="field:'NetWeight',align:'center',width:'50px'">净重</th>--%>
                        <th data-options="field:'GrossWeight',align:'center',width:'45px'">毛重</th>
                        <th data-options="field:'Status',align:'left',width:'30px'">状态</th>
                        <th data-options="field:'btnRemove',formatter:PackedOperate,align:'center',width:'75px'">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'center',split:true" style="min-width: 20%; height: 100%" title="装箱">
            <form id="form5" style="margin-bottom: 20px">
                <div id="divPack">
                    <table class="packing" style="margin-left: 10px; line-height: 30px;">
                        <tr>
                            <td>分拣要求:
                            </td>
                            <td style="padding-left: 10px">
                                <label id="SortingRequire"></label>
                                <span class="divSealed">
                                    <a id="SortingException" style="padding-left: 10px; color: #0094ff" href="#" onclick="AbnormalSorting()">分拣异常</a>
                                </span>
                            </td>
                        </tr>
                        <tr class="divSealed">
                            <td><span style="white-space: nowrap;">国际快递：</span></td>
                            <td style="padding-left: 10px">
                                <label>
                                    <input type="radio" name="radioExpress" value="1" />
                                    <span></span><span style="padding-left: 10px;">是</span>
                                </label>
                                <label style="padding-left: 10px">
                                    <input type="radio" name="radioExpress" value="0" checked="checked" />
                                    <span></span><span style="padding-left: 10px;">否</span>
                                </label>
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td>装箱日期：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-datebox" data-options="required:true,height:26,width:200,missingMessage:'请选择装箱日期'" id="PackingDate" />
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td>箱号：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-textbox" id="BoxIndex" data-options="required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入箱号'" />
                            </td>
                        </tr>
                        <%-- <tr style="line-height: 15px;" class="divPacking">
                            <td></td>
                            <td style="padding-left: 10px">
                                <label style="font-size: 11px; color: red;">输入连续箱号时，请确保每个箱子内该型号的数量相等</label>
                            </td>
                        </tr>--%>
                        <tr class="divPacking">
                            <td>重量：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-textbox" id="Weight" data-options="required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入重量'" />
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td>数量：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-textbox" id="Quantity" data-options="editable:false,height:26,width:200" />
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td>包装类型：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-combogrid" id="PackingType" name="PackingType" data-options="required:true,height:26,width:200" />
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td>运单编号：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-combobox" id="WaybillCodes" data-options="editable:false,validType:'length[1,50]',height:26,width:200" />
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td>库位号：</td>
                            <td style="padding-left: 10px">
                                <input class="easyui-textbox" id="ShelveNumber" data-options="required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入库位号'" />
                            </td>
                        </tr>
                        <tr class="divPacking">
                            <td></td>
                            <td style="padding-left: 10px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok',height:30,width:200" onclick="Packing()">装箱</a>
                            </td>
                        </tr>
                        <tr class="divSeal">
                            <td></td>
                            <td style="padding-left: 10px; text-align: center">已经完成装箱分拣，确定封箱</td>
                        </tr>
                        <tr class="divSeal">
                            <td></td>
                            <td style="padding-left: 10px"><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok',height:30,width:200" id="Sealed" onclick="Sealed()">确认封箱</a></td>
                        </tr>
                    </table>
                </div>
            </form>
            <div id="panel2" class="easyui-panel" title="国际快递" data-options="fit:true">
                <table id="wayBillGrid" class="easyui-datagrid" data-options="
                    fitColumns:true,
                    rownumbers:false,
                    scrollbarSize:0,
                    fit:true,      
                    border:false,     
                    nowrap: false,
                    pagination:false,
                    toolbar:'#tb'">
                    <thead>
                        <tr>
                            <th field="CompanyName" data-options="align:'left'" style="width: 50px">快递公司</th>
                            <th field="WaybillCode" data-options="align:'left'" style="width: 60px">运单编号</th>
                            <th field="ArrivalDate" data-options="align:'left'" style="width: 30px">到港日期</th>
                            <th data-options="field:'btnOpt',width:25,formatter:ExpressOperate,align:'center'">操作</th>
                        </tr>
                    </thead>
                </table>
                <div id="tb" style="padding: 1px; height: 35px;" class="divSealed">
                    <div id="tool">
                        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddExpress()">新增</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>

