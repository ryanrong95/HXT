<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sorting.aspx.cs" Inherits="WebApp.HKWarehouse.Sorting.Sorting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库房分拣</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>

        var originData = eval('(<%=this.Model.OriginData%>)');
        var sortingRequireData = eval('(<%=this.Model.SortingRequireData%>)');
        var ID = getQueryString('ID');
        var BaseInfo = eval('(<%=this.Model.BaseInfo%>)');
        var timer;
        //页面加载时
        $(function () {
            $('#noticeItemGrid').myDatagrid({
                fitColumns: true,
                scrollbarSize: 0,
                fit: false,
                border: true,
                nowrap: false,
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: true,
                onClickRow: onClickRow,
                pagination: false,
                toolbar: '#topBar',
                rowStyler: function (index, row) {
                    if (row.IsMatched == true) {
                        return 'background-color:pink;color:blue;font-weight:bold;';
                    }
                },
                columns: [[
                    { field: 'ck', width: 70, align: 'center', checkbox: true },
                    {
                        field: 'ProductModel', title: '型号', width: 100, align: 'left', editor: {
                            type: 'textbox', options: {
                                required: true, validType: 'length[1,50]',
                                onChange: function (newValue, oldValue) {
                                    if (editIndex == undefined)
                                        return;
                                    var row = $('#noticeItemGrid').datagrid('getRows')[editIndex];
                                    $.post('?action=ChangeProductModel',
                                        {
                                            ProductModel: newValue,
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
                        field: 'SpecialType', title: '特殊类型', width: 80, align: 'center', formatter: function (value, row, index) {
                            return '<span style="color:red;font-family: Microsoft YaHei; font-size: 15px; font-weight: bold;">' + value + '</span>';
                        }
                    },
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
                                data: originData, valueField: "OriginValue", required: true, missingMessage: '请选择产地', validType: 'OriginValid', textField: "OriginText", onSelect: function (newValue, oldValue) {
                                    if (timer) {
                                        clearTimeout(timer);
                                    }
                                    updateOriginData(newValue.OriginValue, 'onSelect');
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
                actionName: 'LoadNoticeItems',
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        //  $(this).datagrid('appendRow', { SpecialType: '<div style="text-align:center;color:red" >已经全部装箱完成</div>' }).datagrid('mergeCells', { index: 0, field: 'SpecialType', colspan: 8 });
                        //  $('.datagrid-body').find("input[type='checkbox']")[0].hidden = true;

                        $(".nodata").show();
                        $('.divSeal').show();
                        $(".divPacking").hide();
                        return;
                    } else {
                        $(".nodata").hide()
                        $(".divPacking").show();
                        $('.divSeal').hide();
                    }
                }

            });

            $.extend($.fn.validatebox.defaults.rules, {
                OriginValid: {
                    validator: function (value) {
                        return checkOrigin(value);
                    },
                    message: '产地不正确!'

                }
            });


            $('#packedGrid').myDatagrid({
                actionName: 'LoadPackedProduct',
                fitColumns: true,
                scrollbarSize: 0,
                fit: false,
                nowrap: false,
                pagination: false,
                onLoadSuccess: onLoadSuccess,
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
                
            });

            InitSortingRequire();
            int();

            $('#ProductModel').textbox({
                onChange: function (value) {
                    Search();
                },
            });
        });

        function checkOrigin(value) {
            var val = $.trim(value);
            var oriarr = val.split(' ');
            if ('(<%=this.Model.OriginData%>)'.indexOf('"' + oriarr[0] + '"') < 0) {
                return false;
            }
            else
                return true;
        }

        function int() {
            //初始化订单编号
            $('#OrderID').text(BaseInfo['OrderID']);
            $('#ClientCode').text(BaseInfo['ClientCode']);
            $("#OrderDate").text(BaseInfo['CreateDate']);
            $("#SpecialOrderType").text(BaseInfo['orderVoyageType']);
            $("#DeliverType").text(BaseInfo['deliveryType']);

            if (BaseInfo["EntryNoticeStatus"] == '<%=Needs.Ccs.Services.Enums.EntryNoticeStatus.Sealed.GetHashCode()%>') {
                $(".hidestyle").hide();
                // $("#SortingException").hide();
            }

            if (BaseInfo["Type"] == '<%=Needs.Ccs.Services.Enums.HKDeliveryType.PickUp.GetHashCode()%>') {
                $("#DeliverTime").text(BaseInfo['PickUpDate']);
                $("#deliverFile").text(BaseInfo['FileName']);
                $("#FileDoc").attr("href", BaseInfo['PickUpFile']);
                $("#Print").attr("href", BaseInfo['PickUpFile']);
                $(".deliver").show();
            } else {
                $(".deliver").hide()
            }
        }

        // 下载提货文件
        function DowmloadAddress() {
            var link = document.getElementById("FileDoc");
            //设置下载的文件名
            link.download = BaseInfo['FileName'];
            // link.style.display = 'none';
            //设置下载路径
            link.href = BaseInfo.PickUpURL;
        }

        function InitSortingRequire() {

            if (sortingRequireData != null) {
                $("#SortingRequire").text(sortingRequireData["SortingRequireText"]);
                if (sortingRequireData["SortingRequireValue"] == '<%=Needs.Ccs.Services.Enums.SortingRequire.Packed.GetHashCode()%>') {
                    $("#SortingException").hide();
                }
                else {
                    $("#SortingException").show();
                }
            }
        }

        function Search() {
            var model = $('#ProductModel').textbox('getValue');
            var rows = $('#noticeItemGrid').datagrid('getRows');

            for (var i = 0; i < rows.length; i++) {
                if (model == null || model == '') {
                    rows[i].IsMatched = false;
                    $('#noticeItemGrid').datagrid('refreshRowStyle', i);
                    continue;
                }

                if (rows[i].ProductModel.toLowerCase().indexOf(model.toLowerCase()) >= 0) {
                    rows[i].IsMatched = true;
                } else {
                    rows[i].IsMatched = false;
                }
                $('#noticeItemGrid').datagrid('refreshRowStyle', i);
            }
        }
        //装箱
        function Packing(Index) {
            if (!$("#form1").form('validate')) {
                return;
            }

            //提交修改
            $('#noticeItemGrid').datagrid('acceptChanges');
            //验证是否勾选
            var Data = $('#noticeItemGrid').datagrid('getChecked');
            if (Data.length == 0) {
                $.messager.alert("消息", "请勾选装箱产品");
                return;
            }
            var quantity = 0;
            for (var i = 0; i < Data.length; i++) {
                if (Data[i].Origin == "" || Data[i].Origin == null) {
                    $.messager.alert("消息", "装箱失败，产品" + Data[i].ProductModel + "原产地为空");
                    return;
                }
                if (Number(Data[i].Quantity) == 0) {
                    $.messager.alert("消息", "装箱产品数量为0");
                    return;
                }

                quantity += Number(Data[i].Quantity);
            }
            //验证装箱数量
            if (quantity == 0) {
                $.messager.alert("消息", "装箱产品总数量为0");
                return;
            }

            MaskUtil.mask();
            $.post('?action=CheckIsHasUnApproved', { TinyOrderID: BaseInfo['OrderID'] }, function (res) {
                MaskUtil.unmask();
                var resJson = JSON.parse(res);
                if (resJson.isHas) {
                    $.messager.alert("消息", resJson.message);
                    return;
                }

                var url = location.pathname.replace(/Sorting.aspx/ig, 'Packing.aspx?ID=' + Data[0].EntryNoticeID + "&Quantity=" + quantity + "&OrderID=" + BaseInfo['OrderID']);
                self.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '装箱',
                    width: '600px',
                    height: '500px',
                    onClose: function () {
                        $('#packedGrid').bvgrid('reload');
                        $('#noticeItemGrid').bvgrid('reload');
                    }
                }).open();
            });
        }

        // 拆分型号
        function SplitModel(OrderItemID, Quantity) {
            var Data = $('#noticeItemGrid').datagrid('getRows');
            if (Data.length == 50) {
                $.messager.alert("消息", "型号数量已达上限50个，不能继续拆分！");
                return;
            }

            MaskUtil.mask();
            $.post('?action=CheckIsHasUnApproved', { TinyOrderID: BaseInfo['OrderID'] }, function (res) {
                MaskUtil.unmask();
                var resJson = JSON.parse(res);
                if (resJson.isHas) {
                    $.messager.alert("消息", resJson.message);
                    return;
                }

                var url = location.pathname.replace(/Sorting.aspx/ig, 'SplitModel.aspx') + '?OrderItemID=' + OrderItemID + '&Qty=' + Quantity;
                self.$.myWindow({
                    iconCls: '',
                    url: url,
                    noheader: false,
                    title: '拆分型号',
                    width: '550px',
                    height: '400px',
                    onClose: function () {
                        $('#packedGrid').bvgrid('reload');
                        $('#noticeItemGrid').bvgrid('reload');
                    }
                }).open();
            });
        }

        //待装箱产品操作
        function NoticeItemOperate(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton" style="margin:3px;color:red; font-weight: bold;" onclick="SplitModel(\'' + row.OrderItemID + '\',\'' + row.Quantity + '\')" >拆分型号</a>';
            if (row.IsSportCheck) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton" style="margin:3px;color:red; font-weight: bold;" onclick="CheckProduct(\'' + row.EntryNoticeItemID + '\')" >抽检异常</a>';
            }
            return buttons;
        }

        //已装箱产品操作
        function PackedOperate(val, row, index) {

            if (row["StatusValue"] == '<%=Needs.Ccs.Services.Enums.PackingStatus.UnSealed.GetHashCode()%>') {
                var buttons = '<a href="javascript:void(0);"  class="easyui-linkbutton deletePacking" style="margin:3px;color:red; font-weight: bold;" onclick="DeletePacking(' + index + ')">取消装箱</a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton " style="margin:3px;color:red; font-weight: bold;" onclick="EditPacking(' + index + ')">修改箱号</a>';
                return buttons;
            }
            if (row["StatusValue"] == '<%=Needs.Ccs.Services.Enums.PackingStatus.Sealed.GetHashCode()%>') {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton " style="margin:3px;color:red; font-weight: bold;" onclick="CancelSealed(' + index + ')">取消封箱</a>';
                return buttons;
            }
        }

        //新增费用
        function AddFee() {
            var OrderID = getQueryString('OrderID');
            var url = location.pathname.replace(/Sorting.aspx/ig, '../Fee/FeeAdd.aspx') + '?OrderID=' + BaseInfo['OrderID'];
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '新增费用',
                width: '820px',
                height: '540px',
            });
        }

        //分拣异常
        function AbnormalSorting() {
            var ID = getQueryString('ID');
            var url = location.pathname.replace(/Sorting.aspx/ig, 'Anomaly.aspx') + '?ID=' + ID + "&OrderID=" + BaseInfo['OrderID'];
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '分拣异常',
                width: '400px',
                height: '300px',
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
        //查看日志
        function ViewLog() {
            var url = location.pathname.replace(/Sorting.aspx/ig, 'Log.aspx') + "?OrderID=" + BaseInfo['OrderID'] + "&EntryNoticeID=" + ID;
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '分拣日志',
                width: '720px',
                height: '540px',
            });
        }
        //查看费用
        function ViewFeeList() {
            var url = location.pathname.replace(/Sorting.aspx/ig, '../Fee/OrderFeeList.aspx') + "?OrderID=" + BaseInfo['OrderID'];
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '查看费用',
                width: '850px',
                height: '520px',
            });
        }
        //查看运单
        function ViewWayBill() {
            var OrderID = getQueryString('OrderID');
            var url = location.pathname.replace(/Sorting.aspx/ig, 'WayBillList.aspx') + '?OrderID=' + BaseInfo['OrderID'] + '&EntryNoticeStatus=' + BaseInfo['EntryNoticeStatus'];
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '运单',
                width: '720px',
                height: '540px',
            });
        }
        //更新装箱数量
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
                    //获取产地的值
                    var editor = $(editors[3].target);
                    editor.combobox("textbox").bind("blur", function () {
                        timer = setTimeout(function () {
                            var originValue = editor.combobox("getValue");
                            if (!checkOrigin(originValue)) {
                                message: '产地不正确!'
                                return false;
                            }
                            updateOriginData(originValue, 'onBlur');
                        }, 500)

                    });
                } else {
                    $('#noticeItemGrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                VerifyQuantity(index);
                editIndex = undefined;
            }
        }

        function updateOriginData(value, type) {
            console.log(type)
            if (editIndex == undefined)
                return;
            var row = $('#noticeItemGrid').datagrid('getRows')[editIndex];
            $.post('?action=ChangeOrigin',
                {
                    OriginValue: value,
                    OrderItemID: row.OrderItemID
                },
                function (result) {
                    var rel = JSON.parse(result);
                    if (!rel.success)
                        $.messager.alert('消息', result.message, 'info', function () {

                            //if (rel.success) {

                            //}
                        });
                });
        }

        //验证装箱数量是否合法
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

        //封箱
        function Sealed() {
            MaskUtil.mask();
            $.post('?action=CheckIsHasUnApproved', { TinyOrderID: BaseInfo['OrderID'] }, function (res) {
                MaskUtil.unmask();
                var resJson = JSON.parse(res);
                if (resJson.isHas) {
                    $.messager.alert("消息", resJson.message);
                    return;
                }

                $.messager.confirm('确认', '再次确认是否封箱？', function (success) {
                    if (success) {
                        MaskUtil.mask();
                        $.post('?action=Sealed',
                            {
                                ID: ID,
                            },
                            function (result) {
                                MaskUtil.unmask();
                                var rel = JSON.parse(result);
                                $.messager.alert('消息', rel.message, 'info', function () {
                                    if (rel.success) {
                                        var url = location.pathname.replace(/Sorting.aspx/ig, '../Entry/UnSortingList.aspx');
                                        // window.parent.location.href = url;
                                        window.location.href = url;
                                    }
                                });
                            })
                    }
                });
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
                                    $('#noticeItemGrid').myDatagrid('reload');

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
            AddSubtotalRow();
        }

        //删除装箱结果
        function DeletePacking(Index) {
            MaskUtil.mask();
            $.post('?action=CheckIsHasUnApproved', { TinyOrderID: BaseInfo['OrderID'] }, function (res) {
                MaskUtil.unmask();
                var resJson = JSON.parse(res);
                if (resJson.isHas) {
                    $.messager.alert("消息", resJson.message);
                    return;
                }

                $('#packedGrid').datagrid('selectRow', Index);
                var rowdata = $('#packedGrid').datagrid('getSelected');
                $.messager.confirm('确认', '是否取消所选数据！', function (success) {
                    if (success) {
                        $.post('?action=DeletePacking', { PackingID: rowdata.PackingID, EntryNoticeID: ID, }, function (result) {
                            var rel = JSON.parse(result);
                            $.messager.alert('消息', rel.message, 'info', function () {
                                if (rel.success) {
                                    $('#packedGrid').bvgrid('reload');
                                    $('#noticeItemGrid').bvgrid('reload');

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
            });
        }

        //修改箱号
        function EditPacking(Index) {
            MaskUtil.mask();
            $.post('?action=CheckIsHasUnApproved', { TinyOrderID: BaseInfo['OrderID'] }, function (res) {
                MaskUtil.unmask();
                var resJson = JSON.parse(res);
                if (resJson.isHas) {
                    $.messager.alert("消息", resJson.message);
                    return;
                }

                $('#packedGrid').datagrid('selectRow', Index);
                var rowdata = $('#packedGrid').datagrid('getSelected');
                var PackingID = rowdata.PackingID;
                var url = location.pathname.replace(/Sorting.aspx/ig, '../Entry/EditCaseNumber.aspx?PackingID=' + PackingID);
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '修改箱号',
                    width: '400px',
                    height: '300px',
                    onClose: function () {
                        $('#packedGrid').bvgrid('reload');
                    }
                }).open();
            });
        }

        //追加合计
        function AddSubtotalRow() {
            //添加合计行
            $('#packedGrid').datagrid('appendRow', {
                BoxIndex: '<span class="subtotal">合计：</span>',
                Quantity: '<span class="subtotal">' + computeTotal('Quantity') + '</span>',
                GrossWeight: '<span class="subtotal">' + computeTotal('GrossWeight') + '</span>',
                Status: '<span class="subtotal">' + compute('BoxIndex') + ' 件</span>',

            });
        }

        //求和
        function computeTotal(colName) {
            var rows = $('#packedGrid').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(rows[i][colName]);
                }
            }
            if (colName == "Quantity")
                return total.toFixed(0);
            else
                return total.toFixed(2);
        }

        //统计件数
        function compute(colName) {
            var rows = $('#packedGrid').datagrid('getRows');
            var total = 0;
            //统计件数
            //获取所有的箱号 ，去掉重复的箱号
            var arr = [];
            for (var i = 0; i < rows.length; i++) {
                if (colName == "BoxIndex") {
                    if (arr.indexOf(rows[i]["BoxIndex"]) == -1) {
                        arr.push(rows[i]["BoxIndex"]);
                    }
                }
            }
            for (var i = 0; i < arr.length; i++) {
                if (arr[i].indexOf("-") != -1 && arr[i].indexOf("WL") != -1) {
                    total += Number(arr[i].split("-")[1].replace("WL", "")) - Number(arr[i].split("-")[0].replace("WL", "")) + 1;
                } else {
                    total += 1;
                }
            }
            return total;
        }


        function Specialstyle(value, record, index) {
            if (value) {
                return '<span style="color:red;font-family: Microsoft YaHei; font-size: 15px; font-weight: bold;">' + value + '</span>';
            }
        };

        //function Specialstyle(value, row, index) {
        //    var strStyle = "";
        //    if (value) {
        //        strstyle = '<span>' + row.Model + '</span >&nbsp;&nbsp;&nbsp;'
        //        strstyle += '<button class="button" style=" border: 2px; color: white;  background-color:#FF0000;padding: 5px 12px;">' + row.SpecialType + '</button>';
        //        return strstyle;
        //    }
        //};
    </script>

    <style>
        #baseinfo {
            line-height: 10px;
            width: 100%;
            text-align: center;
        }

            #baseinfo tr {
                height: 10px;
            }

                #baseinfo tr td {
                    font-weight: normal;
                    text-align: left;
                    padding: 6px;
                    font: 14px Arial,Verdana,'微软雅黑','宋体';
                }

        .FontColor {
            color: red;
            font-family: 'Microsoft YaHei';
            font-size: 15px;
            line-height: 20px;
            font-weight: bold;
        }
        /*.nodata {
           display:block;
        }*/
        .auto-style1 {
            height: 20px;
        }
    </style>
</head>
<body class="easyui-layout" data-options="border:false">
    <div data-options="region:'center',border:false,fit:true">
        <div class="easyui-panel" data-options="border:false" title="基础数据" style="width: 100%; margin-bottom: 8px; border: 0px">
            <div id="baseinfo">
                <table>
                    <tr>
                        <td>入仓号：</td>
                        <td id="ClientCode"></td>
                        <td>订单编号:</td>
                        <td id="OrderID"></td>
                        <td>下单时间:</td>
                        <td id="OrderDate"></td>
                        <td>订单特殊类型:
                            <label id="SpecialOrderType" class="FontColor"></label>
                        </td>
                    </tr>
                    <tr>
                        <td>交货方式:</td>
                        <td>
                            <label id="DeliverType" class="FontColor"></label>
                        </td>
                        <td class="deliver">提货时间:</td>
                        <td id="DeliverTime"></td>
                        <td class="deliver">提货文件:</td>
                        <td class="deliver">
                            <span id="deliverFile"></span>
                            <a id="FileDoc" style="color: #0094ff" href="#" onclick="DowmloadAddress()" download="#">下载</a>
                        </td>
                        <td>分拣要求:
                            <label id="SortingRequire" style="margin-left: 2px" class="FontColor"></label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="topBar">
            <div id="tool">
                <div id="search">
                    <table>
                        <tr>
                            <td class="lbl">型号：</td>
                            <td>
                                <input class="easyui-textbox" data-options="height:30,width:300,prompt:'请输入型号'" id="ProductModel" />
                            </td>
                            <td style="padding-left: 5px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            </td>
                            <td rowspan="2" style="padding-left: 90px" class="hidestyle">
                                <a href="javascript:void(0);" class="easyui-linkbutton divPacking" data-options="iconCls:'icon-ok',width:100" onclick="Packing()">装箱</a>
                                <a href="javascript:void(0);" class="easyui-linkbutton divSeal" data-options="iconCls:'icon-ok',width:100" id="Sealed" onclick="Sealed()">封箱</a>
                            </td>
                            <td style="padding-left: 5px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add',width:100" onclick="AddFee()">新增费用</a>
                            </td>
                            <td style="padding-left: 5px" class="hidestyle">
                                <a href="javascript:void(0);" id="SortingException" class="easyui-linkbutton " data-options=" iconCls:'icon-help',width:100" onclick="AbnormalSorting()">分拣异常</a>
                            </td>
                            <td style="padding-left: 5px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',width:100" onclick="ViewWayBill()">国际运单</a>
                            </td>
                            <td style="padding-left: 5px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',width:100" onclick="ViewFeeList()">查看费用</a>
                            </td>
                            <td style="padding-left: 5px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',width:100" onclick="ViewLog()">查看日志</a>
                            </td>
                        </tr>
                    </table>
                </div>


            </div>
        </div>
        <%--待装箱列表--%>
        <form id="form1">
            <div id="pakinglist" style="margin-bottom:100px">
                <table id="noticeItemGrid" title="待装箱产品"  style="height:10%; margin-bottom:5px">
                </table>
                <div class="nodata" style="text-align: center; color: red;">已经全部装箱完成</div>
            </div>

       
        <%--已装箱产品列表--%> 
        <table id="packedGrid"  title="已装箱(装箱单)" style="height:20%;">
            <thead>
                <tr>
                    <th data-options="field:'BoxIndex',align:'center',width:'50px'" class="auto-style1">箱号</th>
                    <th data-options="field:'Model',align:'left',width:'80px'" class="auto-style1">型号</th>
                    <th data-options="field:'SpecialType',align:'left',width:'50px',formatter:Specialstyle" class="auto-style1">特殊类型</th>
                    <th data-options="field:'Name',align:'left',width:'100px'" class="auto-style1">品名</th>
                    <th data-options="field:'Manufacturer',align:'left',width:'50px'" class="auto-style1">品牌</th>
                    <th data-options="field:'Origin',align:'left',width:'30px'" class="auto-style1">产地</th>
                    <th data-options="field:'Quantity',align:'left',width:'40px'" class="auto-style1">数量</th>
                    <th data-options="field:'GrossWeight',align:'center',width:'45px'" class="auto-style1">毛重</th>
                    <th data-options="field:'Status',align:'left',width:'30px'" class="auto-style1">状态</th>
                    <th data-options="field:'btnRemove',formatter:PackedOperate,align:'center',width:'75px'" class="auto-style1">操作</th>
                </tr>
            </thead>
        </table>
             </form>
    </div>
</body>
</html>

