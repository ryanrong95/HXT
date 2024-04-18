<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Order.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单编辑</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>
    <script type="text/javascript">
        //表单是否已经提交标识，默认为false
        var global_isCommitted = false;
        var ids = eval('(<%=this.Model.IDs%>)');

        var ReplaceQuoteStr = "这是一个双引号";

        var SubmitPromptCloseUrl = "";

        $(function () {
            //最外面 Panel 标题
            var wholePanelTitle = "";
            if ("null" == ids["OrderID"]) {
                wholePanelTitle = "新增订单";
            } else {
                wholePanelTitle = "编辑订单";
            }
            $('#wholePanel').panel({
                title: wholePanelTitle,
            });

            //下拉框数据初始化
            var currData = eval('(<%=this.Model.CurrData%>)');
            var originData = eval('(<%=this.Model.OriginData%>)');
            var unitData = eval('(<%=this.Model.UnitData%>)');
            var idTypeData = eval('(<%=this.Model.IdType%>)');
            var warpTypeData = eval('(<%=this.Model.WarpType%>)');
            var suppliersData = eval('(<%=this.Model.Suppliers%>)');
            var consigneesData = eval('(<%=this.Model.Consignees%>)');
            var AllData = null;
            <%if (this.Model.AllData != null) %>
            <%{ %>
            AllData = eval('(<%=this.Model.AllData%>)');
            <%} %>

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////// 为修改提货文件上传增加 Begin ///////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            InitDeliveryFilePlusDisplay(AllData);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////// 为修改提货文件上传增加 End ////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //初始化币种下拉框
            $('#Currency').combobox({
                data: currData,
            });
            $('#Currency').combobox('setValue', 'USD');
            //初始化证件类型下拉框
            $('#IDType').combobox({
                data: idTypeData,
            });
            //初始化包装类型下拉框
            $('#WarpType').combobox({
                data: warpTypeData,
            });
            $('#WarpType').combobox('setValue', '22');
            //初始化供应商下拉框
            $('#Supplier').combobox({
                data: suppliersData,
                onSelect: function (record) {
                    $.post('?action=GetSupplierAddress', { SupplerID: record.ID }, function (AddressData) {
                        $('#SupplierAddress').combogrid('grid').datagrid('loadData', AddressData);

                        var isSetDefault = false;
                        if (AddressData != null && AddressData.length > 0) {
                            for (var i = 0; i < AddressData.length; i++) {
                                var address = AddressData[i];
                                if (address.IsDefault) {
                                    $("#SupplierAddress").combogrid("setValue", address.ID);
                                    $("#SupplierContact").textbox("setValue", address.Contact);
                                    $("#SupplierContactMobile").textbox("setValue", address.Mobile);
                                    isSetDefault = true;
                                    break;
                                }
                            }
                        }
                        if (!isSetDefault) {
                            $('#SupplierAddress').combogrid('clear');
                            $('#SupplierContact').textbox('clear');
                            $('#SupplierContactMobile').textbox('clear');
                        }
                    });
                }
            });
            //初始化客户收件人下拉框
            $("#ClientConsignee").combogrid({
                idField: "ID",
                textField: "Name",
                data: consigneesData,
                panelWidth: 500,
                fitColumns: true,
                nowrap: false,
                mode: "local",
                columns: [[
                    { field: 'Name', title: '收件单位', width: 150, align: 'left' },
                    { field: 'Contact', title: '联系人', width: 50, align: 'left' },
                    { field: 'Mobile', title: '手机号码', width: 100, align: 'left' },
                    { field: 'Address', title: '收货地址', width: 200, align: 'left' },
                ]],
                onSelect: function () {
                    var grid = $("#ClientConsignee").combogrid('grid');
                    var row = grid.datagrid('getSelected');

                    $('#ClientContact').textbox('setValue', row.Contact);
                    $('#ClientContactMobile').textbox('setValue', row.Mobile);
                    $('#ClientConsigneeAddress').textbox('setValue', row.Address);
                },
            });
            //设置默认收件人
            if (consigneesData != null && consigneesData.length > 0) {
                for (var i = 0; i < consigneesData.length; i++) {
                    var consignee = consigneesData[i];
                    if (consignee.IsDefault) {
                        $("#ClientConsignee").combogrid("setValue", consignee.ID);
                        $("#ClientConsigneeAddress").textbox("setValue", consignee.Address);
                        $("#ClientContact").textbox("setValue", consignee.Contact);
                        $("#ClientContactMobile").textbox("setValue", consignee.Mobile);
                        break;
                    }
                }
            }
            //初始化供应商提货地址下拉框
            $("#SupplierAddress").combogrid({
                idField: "ID",
                textField: "Address",
                panelWidth: 717,
                fitColumns: true,
                nowrap: false,
                mode: "local",
                columns: [[
                    { field: 'Address', title: '提货地址', width: 200, align: 'left' },
                    { field: 'Contact', title: '提货联系人', width: 100, align: 'left' },
                    { field: 'Mobile', title: '手机号码', width: 100, align: 'left' },
                ]],
                onSelect: function () {
                    var grid = $("#SupplierAddress").combogrid('grid');
                    var row = grid.datagrid('getSelected');

                    $('#SupplierContact').textbox('setValue', row.Contact);
                    $('#SupplierContactMobile').textbox('setValue', row.Mobile);
                },
            });
            //设置默认供应商及默认提货地址
            if (suppliersData != null && suppliersData.length > 0) {
                var supplierID = suppliersData[0].ID;
                $("#Supplier").combobox("setValue", supplierID);
                $.post('?action=GetSupplierAddress', { SupplerID: supplierID }, function (AddressData) {
                    $("#SupplierAddress").combogrid("grid").datagrid("loadData", AddressData);
                    if (AddressData != null && AddressData.length > 0) {
                        for (var i = 0; i < AddressData.length; i++) {
                            var address = AddressData[i];
                            if (address.IsDefault) {
                                $("#SupplierAddress").combogrid("setValue", address.ID);
                                $("#SupplierContact").textbox("setValue", address.Contact);
                                $("#SupplierContactMobile").textbox("setValue", address.Mobile);
                                break;
                            }
                        }
                    }
                });
            }
            //初始化付汇供应商下拉框
            $('#PayExchangeSupplier').combobox({
                data: suppliersData,
                onSelect: function () {
                    var values = $(this).combobox('getValues')
                    if (values.length > 3) {
                        $(this).combobox('unselect', values[values.length - 1]);
                        $.messager.alert('提示', '付汇供应商最多只能选择3个');
                    }
                }
            });


            //提货文件上传控件初始化
            InitDeliveryFileChainsupload();

            /*
            //提货文件上传控件初始化
            $('#DeliveryFile').chainsupload({
                required: false,
                multiple: false,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片、Word或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/msword',
                    'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/pdf'],
            });
            
            //改变 提货文件 按钮输入框宽度 Begin
            //$("#spanBL_DeliveryFile :not(:first-child)").width(56);
            //改变 提货文件 按钮输入框宽度 End
            
            $("input[name='DeliveryFile']").change(function(){
                var fullName = $("input[name='DeliveryFile']").val();
                
                if(fullName == null || fullName == "") {
                    DeleteHKPickUpFile();
                    return;
                }
                
                var pos = fullName.lastIndexOf("\\");
                var fileName = fullName.substring(pos+1);

                var content = '<img src="../App_Themes/xp/images/blue-fujian.png" />'
                    + '<a href="javascript:void(0);" style="margin-left: 2px; cursor: default;">' + fileName + '</a>'
                    + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteHKPickUpFile()">删除</a>';
                $("#HKPickUpFileDisplay").html(content);
            });
            */

            //提货时间的时间区间为一周内
            $('#PickupTime').datebox().datebox('calendar').calendar({
                validator: function (value) {
                    var curDate = new Date();
                    var endDate = new Date();
                    endDate.setDate(curDate.getDate() + 7);
                    var date = new Date(value).toDateStr();
                    return date >= curDate.toDateStr() && date < endDate.toDateStr();
                }
            });

            //如果会员协议是预付款，则下单时不允许选择垫款
            <%if (this.Model.IsPrePaid)%>
            <%{%>
            $("input[name='IsLoan']").attr("disabled", true);
            $("input[id='IsLoanfalse']").prop("checked", "checked");
            <%}%>

            //订单信息初始化
            if (AllData != null && AllData != "") {
                //香港交货信息
                if (AllData["HKDeliveryType"] == "<%=Needs.Ccs.Services.Enums.HKDeliveryType.SentToHKWarehouse.GetHashCode()%>") {
                    HKDelivery(1);
                    $("input[name='HKDeliveryType'][value=1]").attr("checked", true);
                    $("#Supplier").combobox("setValue", AllData["Supplier"]);
                    $("#WayBillNo").textbox("setValue", AllData["WayBillNo"]);
                } else {
                    HKDelivery(2);
                    $("input[name='HKDeliveryType'][value=2]").attr("checked", true);
                    $("#Supplier").combobox("setValue", AllData["Supplier"]);
                    $.post('?action=GetSupplierAddress', { SupplerID: AllData["Supplier"] }, function (AddressData) {
                        $("#SupplierAddress").combogrid("grid").datagrid("loadData", AddressData);
                        $("#SupplierAddress").combogrid("setValue", AllData["SupplierAddress"]);
                        $("#SupplierContact").textbox("setValue", AllData["SupplierContact"]);
                        $("#SupplierContactMobile").textbox("setValue", AllData["SupplierContactMobile"]);
                    });
                    if (AllData["DeliveryFile"] != null && AllData["DeliveryFile"] != "") {
                        AllData["DeliveryFile"].Url = AllData["DeliveryFileUrl"];
                        //$('#DeliveryFile').chainsupload("setValue", AllData["DeliveryFile"]);
                        fullFileNameToHKPickUpFileDisplay(AllData["DeliveryFileUrl"]);
                    }
                    if (AllData["PickupTime"] != null && AllData["PickupTime"] != "") {
                        var pickupTime = new Date(AllData["PickupTime"]).toDateStr();
                        $("#PickupTime").datebox("setValue", pickupTime);
                    }
                }

                //国内交货信息
                if (AllData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.PickUpInStore.GetHashCode()%>") {
                    SZDelivery(1)
                    $("input[name='SZDeliveryType'][value=1]").attr("checked", true);
                    $("#ClientPicker").textbox("setValue", AllData["ClientPicker"]);
                    $("#ClientPickerMobile").textbox("setValue", AllData["ClientPickerMobile"]);
                    $("#IDType").combobox("setValue", AllData["IDType"]);
                    $("#IDNumber").textbox("setValue", AllData["IDNumber"]);
                } else if (AllData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.SentToClient.GetHashCode()%>" ||
                    AllData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.Shipping.GetHashCode()%>") {
                    if (AllData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.SentToClient.GetHashCode()%>") {
                        SZDelivery(2);
                        $("input[name='SZDeliveryType'][value=2]").attr("checked", true);
                    }
                    if (AllData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.Shipping.GetHashCode()%>") {
                        SZDelivery(3);
                        $("input[name='SZDeliveryType'][value=3]").attr("checked", true);
                    }
                    $("#ClientConsignee").combogrid("setValue", AllData["ClientConsignee"]);
                    $("#ClientConsigneeAddress").textbox("setValue", AllData["ClientConsigneeAddress"]);
                    $("#ClientContact").textbox("setValue", AllData["ClientContact"]);
                    $("#ClientContactMobile").textbox("setValue", AllData["ClientContactMobile"]);
                }
                //付汇供应商
                $("#PayExchangeSupplier").combobox("setValue", AllData["PayExchangeSupplier"]);
                //订单基本信息
                $("#Currency").combobox("setValue", AllData["Currency"]);
                if (AllData["IsFullVehicle"] == true) {
                    $("input[name='IsFullVehicle'][value=true]").attr("checked", true);
                } else {
                    $("input[name='IsFullVehicle'][value=false]").attr("checked", true);
                }
                //如果会员协议不是预付款,则取之前保存的“是否垫付款”
                <%if (!this.Model.IsPrePaid)%>
                <%{%>
                if (AllData["IsLoan"] == true) {
                    $("input[name='IsLoan'][value=true]").attr("checked", true);
                } else {
                    $("input[name='IsLoan'][value=false]").attr("checked", true);
                }
                <%}%>
                $("#WarpType").combobox("setValue", AllData["WarpType"]);
                $("#PackNo").textbox("setValue", AllData["PackNo"]);
                $("#Summary").textbox("setValue", AllData["Summary"]);
            } else {
                HKDelivery(1);
                $("input[name='HKDeliveryType'][value=1]").attr("checked", true);
                SZDelivery(1)
                $("input[name='SZDeliveryType'][value=1]").attr("checked", true);
            }

            //产品列表初始化
            $('#dg').myDatagrid({
                pageSize: 50,
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'ProductUniqueCode', title: '物料号', width: 70, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Batch', title: '批号', width: 70, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Name', title: '品名', width: 130, align: 'left', editor: { type: 'textbox', options: { required: true, validType: 'length[1,150]' } } },
                    { field: 'Manufacturer', title: '品牌', width: 130, align: 'left', editor: { type: 'textbox', options: { required: true, validType: 'length[1,50]' } } },
                    { field: 'Model', title: '型号', width: 120, align: 'left', editor: { type: 'textbox', options: { required: true, validType: 'length[1,150]' } } },
                    {
                        field: 'Origin', title: '产地', width: 100, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < originData.length; i++) {
                                if (originData[i].OriginValue == value) return originData[i].OriginText;
                            }
                            return value;
                        },
                        editor: { type: 'combobox', options: { data: originData, valueField: "OriginValue", textField: "OriginText", required: true, hasDownArrow: false } }
                    },
                    {
                        field: 'Qty', title: '数量', width: 80, align: 'center',
                        editor: {
                            type: 'numberbox', options: {
                                min: 0, precision: 4, required: true, onChange: function (newValue, oldValue) {
                                    var unitPrice = $('#dg').datagrid('getEditor', { index: editIndex, field: 'UnitPrice' });
                                    if (unitPrice != null) {
                                        var qty = $('#dg').datagrid('getEditor', { index: editIndex, field: 'Qty' });
                                        var qtyVal = qty.target.numberbox('getValue');
                                        var totalPrice = $('#dg').datagrid('getEditor', { index: editIndex, field: 'TotalPrice' });
                                        var totalPriceVal = totalPrice.target.numberbox('getValue');
                                        if (qtyVal != '' && totalPriceVal != '') {
                                            $(unitPrice.target).numberbox('setValue', (totalPriceVal / qtyVal).toFixed(4));
                                        }
                                        else {
                                            $(unitPrice.target).numberbox('setValue', '');
                                        }
                                    }

                                    RemoveSubtotalRow();
                                    AddSubtotalRow();
                                }
                            }
                        }
                    },
                    {
                        field: 'Unit', title: '单位', width: 80, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < unitData.length; i++) {
                                if (unitData[i].UnitValue == value) return unitData[i].UnitText;
                            }
                            return value;
                        },
                        editor: {
                            type: 'combobox', options: {
                                data: unitData, valueField: "UnitValue", textField: "UnitText", required: true, hasDownArrow: false,
                                onSelect: function (value) {
                                    if (value.UnitValue != '007') {
                                        $.messager.alert('提示', '请仔细核对产品单位，如果产品单位选择错误，造成无法报关，芯达通将不承担任何责任');
                                    }
                                }
                            }
                        }
                    },
                    {
                        field: 'UnitPrice', title: '单价', width: 80, align: 'center',
                        formatter: function (value, row) {
                            if (value != undefined && value != null) {
                                if (value.toString().indexOf('<span class="subtotal">') != -1) {
                                    return value;
                                }
                            }
                            return (row.TotalPrice / row.Qty).toFixed(4);
                        },
                        editor: { type: 'numberbox', options: { min: 0, precision: 4, disabled: true, required: true } }
                    },
                    {
                        field: 'TotalPrice', title: '总价', width: 80, align: 'center',
                        formatter: function (value, row) {
                            if (value != undefined && value != null) {
                                if (value.toString().indexOf('<span class="subtotal">') != -1) {
                                    return value;
                                }
                            }
                            return parseFloat(value).toFixed(2);
                        },
                        editor: {
                            type: 'numberbox', options: {
                                min: 0, precision: 2, required: true, onChange: function (newValue, oldValue) {

                                    var unitPrice = $('#dg').datagrid('getEditor', { index: editIndex, field: 'UnitPrice' });
                                    if (unitPrice != null) {
                                        var qty = $('#dg').datagrid('getEditor', { index: editIndex, field: 'Qty' });
                                        var qtyVal = qty.target.numberbox('getValue');
                                        var totalPrice = $('#dg').datagrid('getEditor', { index: editIndex, field: 'TotalPrice' });
                                        var totalPriceVal = totalPrice.target.numberbox('getValue');
                                        if (qtyVal != '' && totalPriceVal != '') {
                                            $(unitPrice.target).numberbox('setValue', (totalPriceVal / qtyVal).toFixed(4));
                                        }
                                        else {
                                            $(unitPrice.target).numberbox('setValue', '');
                                        }
                                    }

                                    RemoveSubtotalRow();
                                    AddSubtotalRow();
                                }
                            }
                        }
                    },
                    {
                        field: 'GrossWeight', title: '毛重(KG)', width: 80, align: 'center',
                        formatter: function (value, row) {
                            if (value != undefined && value != null && value != '') {
                                if (value.toString().indexOf('<span class="subtotal">') != -1) {
                                    return value;
                                }
                                return parseFloat(value).toFixed(4);
                            }
                        },
                        editor: { type: 'numberbox', options: { min: 0, precision: 4 } }
                    },
                    { field: 'Btn', title: '操作', width: 100, align: 'center', formatter: Operation }
                ]],
                loadFilter: function (data) {
                    if (data.total == 0)
                        return data;

                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    //添加合计行
                    AddSubtotalRow();
                }
            });

             $('#dg').bvgrid('hideColumn', 'ProductUniqueCode');

            //原始PI列表初始化
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'dataPI',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'Name', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileFormat', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'VirtualPath', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: ShowInfo }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".pi");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });

            //注册香港交货方式radiobutton的点击事件
            $("input[name=HKDeliveryType]").click(function () {
                var value = $(this).val();
                if (value == 1) {
                    HKDelivery(1);
                }
                if (value == 2) {
                    HKDelivery(2);
                }
            });

            //注册国内交货方式radiobutton的点击事件
            $("input[name=SZDeliveryType]").click(function () {
                var value = $(this).val();
                if (value == 1) {
                    SZDelivery(1);
                }
                if (value == 2) {
                    SZDelivery(2);
                }
                if (value == 3) {
                    SZDelivery(3);
                }
            });

            //注册上传产品信息filebox的onChange事件
            $('#uploadExcel').filebox({
                onClickButton: function () {
                    $('#uploadExcel').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadExcel').filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    var rows = $('#dg').datagrid('getRows');
                    //去除合计行
                    formData.append('RowNum', rows.length - 1);
                    formData.append('uploadExcel', $("input[name='uploadExcel']").get(0).files[0]);
                    $.ajax({
                        url: '?action=ImportProductInfo',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            if (res.success) {
                                //移除合计行
                                RemoveSubtotalRow();

                                var data = res.data;
                                for (var index = 0; index < data.length; index++) {
                                    $('#dg').datagrid('insertRow', {
                                        row: {
                                            ProductUniqueCode: data[index].ProductUniqueCode,
                                            Batch: data[index].Batch,
                                            Name: data[index].Name,
                                            Manufacturer: data[index].Manufacturer,
                                            Model: data[index].Model,
                                            Origin: data[index].Origin,
                                            Qty: data[index].Qty,
                                            Unit: data[index].Unit,
                                            UnitPrice: data[index].UnitPrice,
                                            TotalPrice: data[index].TotalPrice,
                                            GrossWeight: data[index].GrossWeight
                                        }
                                    });
                                }

                                //添加合计行
                                AddSubtotalRow();
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            });

            //注册上传原始单据filebox的onChange事件
            $('#uploadPI').filebox({
                multiple: true,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $('#uploadPI').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadPI').filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    var files = $("input[name='uploadPI']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                formData.append('uploadPI', bl, fileName); // 文件对象
                                $.ajax({
                                    url: '?action=UploadPI',
                                    type: 'POST',
                                    data: formData,
                                    dataType: 'JSON',
                                    cache: false,
                                    processData: false,
                                    contentType: false,
                                    success: function (res) {
                                        if (res.success) {
                                            InsertPI(res.data);
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.append('uploadPI', file);
                            $.ajax({
                                url: '?action=UploadPI',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    if (res.success) {
                                        InsertPI(res.data);
                                    } else {
                                        $.messager.alert('提示', res.message);
                                    }
                                }
                            }).done(function (res) {

                            });
                        }
                    }
                }
            });

            function InsertPI(data) {
                var row = $('#pi').datagrid('getRows');
                for (var i = 0; i < data.length; i++) {
                    $('#pi').datagrid('insertRow', {
                        index: row.length + i,
                        row: {
                            Name: data[i].Name,
                            FileType: data[i].FileType,
                            FileFormat: data[i].FileFormat,
                            VirtualPath: data[i].VirtualPath,
                            Url: data[i].Url
                        }
                    });
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////// 为修改提货文件上传增加 Begin ///////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            $('#uploadDeliveryFilePlus').filebox({
                multiple: false,
                buttonText: '选择文件',
                buttonAlign: 'right',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf',
                    'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'],
                onClickButton: function () {
                    $('#uploadDeliveryFilePlus').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadDeliveryFilePlus').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='uploadDeliveryFilePlus']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var formData = new FormData();

                    if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                        photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                            var bl = convertBase64UrlToBlob(base64Codes);
                            formData.append('uploadDeliveryFilePlus', bl, fileName); // 文件对象
                            $.ajax({
                                url: '?action=UploadDeliveryFile',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    if (res.success) {
                                        AddDeliveryFilePlus(res.data[0]);
                                    } else {
                                        $.messager.alert('提示', res.message);
                                    }
                                }
                            }).done(function (res) {

                            });
                        });
                    } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                        $.messager.alert('提示', '上传的非图片文件大小不能超过3M!');
                    } else {
                        formData.append('uploadDeliveryFilePlus', file);
                        $.ajax({
                            url: '?action=UploadDeliveryFile',
                            type: 'POST',
                            data: formData,
                            dataType: 'JSON',
                            cache: false,
                            processData: false,
                            contentType: false,
                            success: function (res) {
                                if (res.success) {
                                    AddDeliveryFilePlus(res.data[0]);
                                } else {
                                    $.messager.alert('提示', res.message);
                                }
                            }
                        }).done(function (res) {

                        });
                    }
                }
            });



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////// 为修改提货文件上传增加 End ////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //设置只读输入框背景色
            SetReadonlyBgColor('easyui-textbox', '#EBEBE4');

            //上传产品信息按钮样式修改
            $("#uploadExcel + span").children(":first").css("border-left", "1px solid #f4f4f4");
            $("#uploadExcel + span").children(":nth-child(2)").children(":first").css("background-color", "#f4f4f4");
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            if (val != undefined && val != null) {
                if (val.toString().indexOf('<span class="subtotal">') != -1) {
                    return val;
                }
            }

            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Remove(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //列表框按钮加载
        function OperationFile(val, row, index) {
            var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" onclick="View(\'' + row.Url + '\')" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        var editIndex = undefined;
        var isRemoveRow = false;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                var ed = $('#dg').datagrid('getEditor', { index: editIndex, field: 'Origin' });
                var origin = $(ed.target).combobox('getText');
                $('#dg').datagrid('getRows')[editIndex]['OriginText'] = origin;

                ed = $('#dg').datagrid('getEditor', { index: editIndex, field: 'Unit' });
                var unit = $(ed.target).combobox('getText');
                $('#dg').datagrid('getRows')[editIndex]['UnitText'] = unit;

                $('#dg').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

        function onClickRow(index) {
            var lastIndex = $('#dg').datagrid('getRows').length - 1;
            if (index == lastIndex) {
                return;
            }

            if (isRemoveRow) {
                //移除合计行
                RemoveSubtotalRow();
                if (editIndex != undefined) {
                    $('#dg').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);
                    editIndex = undefined;
                }

                $('#dg').datagrid('deleteRow', index);
                //添加合计行
                AddSubtotalRow();
                isRemoveRow = false;
                return;
            }

            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index)
                        .datagrid('beginEdit', index);
                    var unitPrice = $('#dg').datagrid('getEditor', { index: index, field: 'UnitPrice' });
                    var row = $('#dg').datagrid('getSelected');
                    $(unitPrice.target).numberbox('setValue', (row.TotalPrice / row.Qty).toFixed(4));

                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
        }

        function Add() {
            if (endEditing()) {
                var rows = $('#dg').datagrid('getRows');
                //20个产品行+合计行
                if (rows.length >= 21) {
                    $.messager.alert('提示', '产品型号不能超过20个！');
                    return;
                }
                //移除合计行
                RemoveSubtotalRow();

                $('#dg').datagrid('appendRow', { Unit: '007' });
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex)
                    .datagrid('beginEdit', editIndex);
                //$('#dg').datagrid('endEdit', editIndex);

                //添加合计行
                AddSubtotalRow();
            }
        }

        function Remove(id) {
            isRemoveRow = true;
        }

        function AddSubtotalRow() {
            //添加合计行
            $('#dg').datagrid('appendRow', {
                Batch: '<span class="subtotal">合计：</span>',
                Name: '<span class="subtotal">--</span>',
                Manufacturer: '<span class="subtotal">--</span>',
                Model: '<span class="subtotal">--</span>',
                Origin: '<span class="subtotal">--</span>',
                Qty: '<span class="subtotal">' + compute('Qty') + '</span>',
                Unit: '<span class="subtotal">--</span>',
                UnitPrice: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
                GrossWeight: '<span class="subtotal">--</span>',
                Btn: '<span class="subtotal">--</span>',
            });
        }

        function RemoveSubtotalRow() {
            var lastIndex = $('#dg').datagrid('getRows').length - 1;
            $('#dg').datagrid('deleteRow', lastIndex);
        }

        function compute(colName) {
            var rows = $('#dg').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(rows[i][colName]);
                }
            }
            return total.toFixed(2);
        }

        //新增供应商
        function SupplierAdd() {
            var url = location.pathname.replace(/Edit.aspx/ig, '../Client/Supplier/Edit.aspx?ID=' + ids['ClientID']);
            top.$.myWindow({
                iconCls: "",
                closable: true,
                noheader: false,
                title: '供应商新增',
                url: url,
                width: '650px',
                height: '300px',
                onClose: function () {
                    $.post('?action=AddSupplier', { ClientID: ids['ClientID'] }, function (res) {
                        $('#PayExchangeSupplier').combobox('loadData', res.Suppliers);
                        $('#Supplier').combobox('loadData', res.Suppliers);
                    });
                }
            });
        }

        //新增客户收件信息
        function ClientConsigneeAdd() {
            var url = location.pathname.replace(/Edit.aspx/ig, '../Client/Consignee/Edit.aspx?ID=' + ids['ClientID']);
            top.$.myWindow({
                iconCls: "",
                closable: true,
                noheader: false,
                title: '提货人新增',
                url: url,
                width: '700px',
                height: '450px',
                onClose: function () {
                    $.post('?action=AddConsignee', { ClientID: ids['ClientID'] }, function (res) {
                        var rows = $('#ClientConsignee').combogrid('grid').datagrid('getRows');
                        if (res.Consignees.length == rows.length) {
                            return;
                        }
                        $('#ClientConsignee').combogrid('grid').datagrid('loadData', res.Consignees);
                        if (res.IsDefault) {
                            $('#ClientConsignee').combogrid('setValue', res.ID);
                            $('#ClientContact').textbox('setValue', res.Contact);
                            $('#ClientContactMobile').textbox('setValue', res.Mobile);
                            $('#ClientConsigneeAddress').textbox('setValue', res.Address);
                        }
                    });
                }
            });
        }

        //新增供应商交货地址（香港自提时使用）
        function SupplierAddressAdd() {
            var supplierID = $("#Supplier").combobox("getValue");
            var url = location.pathname.replace(/Edit.aspx/ig, '../Client/Supplier/Address/Edit.aspx?SupplierID=' + supplierID);
            top.$.myWindow({
                iconCls: "",
                closable: true,
                noheader: false,
                title: '提货地址新增',
                url: url,
                width: '750px',
                height: '400px',
                onClose: function () {
                    $.post('?action=AddSupplierAddress', { SupplierID: supplierID }, function (res) {
                        var rows = $('#SupplierAddress').combogrid('grid').datagrid('getRows');
                        if (res.SupplierAddresses.length == rows.length) {
                            return;
                        }
                        $('#SupplierAddress').combogrid('grid').datagrid('loadData', res.SupplierAddresses);
                        if (res.IsDefault) {
                            $('#SupplierAddress').combogrid('setValue', res.ID);
                            $('#SupplierContact').textbox('setValue', res.Contact);
                            $('#SupplierContactMobile').textbox('setValue', res.Mobile);
                        }
                    });
                }
            });
        }

        //验证录入的产品信息
        function ValidProducts() {
            var rows = $('#dg').datagrid('getRows');
            //仅有合计行
            if (rows.length == 1) {
                $.messager.alert('提示', '请至少添加一项产品！');
                return false;
            }
            //20个产品行+合计行
            if (rows.length > 21) {
                $.messager.alert('提示', '产品型号不能超过20个！');
                return false;
            }

            endEditing();
            for (var i = 0; i < rows.length - 1; i++) {
                //验证是否按要求录入数据
                $('#dg').datagrid('beginEdit', i);
                if ($('#dg').datagrid('validateRow', i)) {
                    $('#dg').datagrid('endEdit', i);
                    editIndex = undefined;
                } else {
                    $.messager.alert('提示', '请按提示输入数据！');
                    editIndex = i;
                    return false;
                }

                //验证计量单位是否有效
                var isValid = false;
                var unitData = eval('(<%=this.Model.UnitData%>)');
                for (var j = 0; j < unitData.length; j++) {
                    if (unitData[j].UnitValue == rows[i].Unit) {
                        isValid = true;
                        break;
                    }
                }
                if (!isValid) {
                    $.messager.alert('提示', rows[i].Unit + '不是有效的计量单位！');
                    return false;
                }

                //验证产地是否有效
                if (rows[i].Origin != null && rows[i].Origin != '') {
                    isValid = false;
                    var originData = eval('(<%=this.Model.OriginData%>)');
                    for (var j = 0; j < originData.length; j++) {
                        if (originData[j].OriginValue == rows[i].Origin) {
                            isValid = true;
                            break;
                        }
                    }
                    if (!isValid) {
                        $.messager.alert('提示', rows[i].Origin + '不是有效的产地！');
                        return false;
                    }
                }
            }

            return true;
        }

        //保存订单 - isConfirm:false 保存草稿; isConfirm:true 确认下单
        function SaveOrder(isConfirm) {
            //PI的文件 input file 清空
            $('#uploadPI').filebox('setValue', '');
            ///DeliveryFilePlus 的文件 input file 清空
            $('#uploadDeliveryFilePlus').filebox('setValue', '');

            if ("2" == $("input[name='HKDeliveryType']:checked").val()) {
                var deliveryFilePlusName = $("#DeliveryFilePlusName").val();
                if (deliveryFilePlusName.length <= 0) {
                    $('html,body').animate({ scrollTop: 0 }, 50);

                    $("#uploadDeliveryFilePlus").next().tooltip({
                        content: '<div>未选择提货文件</div>',
                        position: 'right',
                        hideDelay: 1000,
                        showEvent: null,
                        onShow: function () {
                            $(this).tooltip('tip').css({
                                backgroundColor: '#ffffcc',
                                borderColor: '#cc9933',
                            });
                        },
                    });

                    $('#uploadDeliveryFilePlus').next().tooltip('show');
                    window.setTimeout(function () {
                        $('#uploadDeliveryFilePlus').next().tooltip('hide');
                    }, 2000);
                    return;
                }
            }

            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证录入的产品信息
            if (!ValidProducts()) {
                return;
            }

            MaskUtil.mask();

            var data = new FormData($('#form1')[0]);
            var rows = $('#dg').datagrid('getRows');
            var products = [];
            //只保留产品信息
            for (var i = 0; i < rows.length - 1; i++) {
                rows[i].Model = rows[i].Model.replace(/\"/g, ReplaceQuoteStr);
                products.push(rows[i]);
            }
            data.append('Products', JSON.stringify(products));
            var invoices = $('#pi').datagrid('getRows');
            data.append('Invoices', JSON.stringify(invoices));
            data.append('SupplierAddressText', $('#SupplierAddress').combogrid("getText"));
            data.append('ClientConsigneeText', $('#ClientConsignee').combogrid("getText"));
            data.append('OrderID', ids['OrderID']);
            data.append('ClientID', ids['ClientID']);
            data.append('IsConfirm', isConfirm);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////// 为修改提货文件上传增加 Begin ///////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var deliveryFilePlus = {
                "Name": $("#DeliveryFilePlusName").val(),
                "FileType": $("#DeliveryFilePlusFileType").val(),
                "FileFormat": $("#DeliveryFilePlusFileFormat").val(),
                "VirtualPath": $("#DeliveryFilePlusVirtualPath").val(),
                "Url": $("#DeliveryFilePlusUrl").val()
            }
            data.append('DeliveryFilePlus', JSON.stringify(deliveryFilePlus));

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////// 为修改提货文件上传增加 End ////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            data.append('ReplaceQuoteStr', ReplaceQuoteStr);

            $.ajax({
                url: '?action=SaveOrder',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        var code = eval('(<%=this.Model.ClientCode%>)');
                        var isReturned = false;
                        <%if (this.Model.IsReturned)%>
                        <%{%>
                        isReturned = true;
                        <%}%>
                        var url = location.pathname.replace(/Edit.aspx/ig, 'SubmitPrompt.aspx?OrderID=' + res.ID + '&ClientCode=' + code['ClientCode'] + '&IsReturned=' + isReturned);

                        $.myWindow.setMyWindow("OrderEdit", window);
                        $.myWindow({
                            url: url,
                            width: '400px',
                            height: '150px',
                            onClose: function () {
                                window.location.href = SubmitPromptCloseUrl;
                            }
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }

        //查看原始PI
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
            }
            $('#viewFileDialog').window('open').window('center');
        }

        //删除原始PI
        var isRemovePIRow = false;
        function Delete(index) {
            $('#pi').datagrid('deleteRow', index);
            var data = $('#pi').datagrid('getData');
            $('#pi').datagrid('loadData', data);
            // isRemovePIRow = true;
        }
        function onClickPIRow(index) {
            if (isRemovePIRow) {
                $('#pi').datagrid('deleteRow', index);
                isRemovePIRow = false;
            }
        }

        //设置香港交货方式中各个控件的显示/隐藏、是否必填
        function HKDelivery(option) {
            if (option == 1) {
                $('#WayBillTR').show();
                $('#PickUpTimeTR').hide();
                $('#PickUpTR').hide();
                $('#PickUpTipTR').hide();
                $('#PickUpTipPlusTR').hide();
                $('#PickUpAddressTR').hide();
                $('#PickUpContactTR').hide();

                $('#PickupTime').datebox('textbox').validatebox('options').required = false;
                $('#SupplierAddress').combogrid('textbox').validatebox('options').required = false;
                $('#DeliveryFile').chainsupload('resetRequired', false);
            } else if (option == 2) {
                $('#WayBillTR').hide();
                $('#PickUpTimeTR').show();
                $('#PickUpTR').hide();
                $('#PickUpTipTR').hide();
                $('#PickUpTipPlusTR').show();
                $('#PickUpAddressTR').show();
                $('#PickUpContactTR').show();

                $('#PickupTime').datebox('textbox').validatebox('options').required = true;
                $('#SupplierAddress').combogrid('textbox').validatebox('options').required = true;
                //$('#DeliveryFile').chainsupload('resetRequired', true);
            }
            $('#form1').form('enableValidation').form('validate');
        }

        //设置国内交货方式中各个控件的显示/隐藏、是否必填
        function SZDelivery(option) {
            if (option == 1) {
                $('#ClientPickerTR').show();
                $('#ClientPickerIDTR').show();
                $('#ClientConsigneeTR').hide();
                $('#ClientConsigneeAddressTR').hide();
                $('#ClientContactTR').hide();

                $('#IDType').combobox('textbox').validatebox('options').required = true;
                $('#IDNumber').textbox('textbox').validatebox('options').required = true;
                $('#ClientPickerMobile').textbox('textbox').validatebox('options').required = true;
                $('#ClientPicker').textbox('textbox').validatebox('options').required = true;
                $('#ClientConsignee').combogrid('textbox').validatebox('options').required = false;
            } else if (option == 2 || 3) {
                $('#ClientPickerTR').hide();
                $('#ClientPickerIDTR').hide();
                $('#ClientConsigneeTR').show();
                $('#ClientConsigneeAddressTR').show();
                $('#ClientContactTR').show();

                $('#IDType').combobox('textbox').validatebox('options').required = false;
                $('#IDNumber').textbox('textbox').validatebox('options').required = false;
                $('#ClientPickerMobile').textbox('textbox').validatebox('options').required = false;
                $('#ClientPicker').textbox('textbox').validatebox('options').required = false;
                $('#ClientConsignee').combogrid('textbox').validatebox('options').required = true;
            }
            $('#form1').form('enableValidation').form('validate');
        }


        //显示附件图片
        function ShowInfo(val, row, index) {
            return '<img src="../App_Themes/xp/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;text-decoration:none" onclick="View(\'' + row.Url + '\')">' + row.Name + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
        }

        //删除香港交货方式提货文件
        function DeleteHKPickUpFile() {
            $("#HKPickUpFileDisplay").html("");

            $("#chainsuploadcss_DeliveryFile").remove();

            InitDeliveryFileChainsupload();


            //$('#DeliveryFile').chainsupload('resetRequired', true);

            $("#PickUpFileNotUploadTip").show();
            $("#PickUpFileNotUploadTip").html("未上传");
        }

        function InitDeliveryFileChainsupload() {
            //提货文件上传控件初始化
            $('#DeliveryFile').chainsupload({
                required: false,
                multiple: false,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片、Word或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/msword',
                    'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/pdf'],
            });

            //改变 提货文件 按钮输入框宽度 Begin
            $("#spanBL_DeliveryFile :not(:first-child)").width(56);
            //改变 提货文件 按钮输入框宽度 End

            $("#chainsuploadcss_DeliveryFile").tooltip({
                content: '<div>未选择提货文件</div>',
                position: 'right',
                hideDelay: 1000,
                showEvent: null,
            });

            $("input[name='DeliveryFile']").change(function () {
                var fullName = $("input[name='DeliveryFile']").val();
                if (fullName == null || fullName == "") {
                    //这是选择文件点了取消
                    return;
                }


                /*
                var fileSize = $("input[name='DeliveryFile']").get(0).files[0].size / 1024;
                if(fileSize > 500) {
                    $('#DeliveryFile').chainsupload({
                        required: false,
                        multiple: false,
                        //validType: ['fileSize[500,"KB"]'],
                        buttonText: '选择文件',
                        buttonAlign: 'right',
                        prompt: '请选择图片、Word或PDF类型的文件',
                        accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/msword',
                            'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/pdf'],
                    });

                    $.messager.alert('提示', '文件大小不能超过500KB！');
                    return;
                }
                */

                fullFileNameToHKPickUpFileDisplay(fullName);
            });
        }

        function fullFileNameToHKPickUpFileDisplay(fullName) {
            if (fullName == null || fullName == "") {
                //这是初始化的时候，没有已经上传的文件的情况。应该就是新增订单的时候
                DeleteHKPickUpFile();
                return;
            }

            var fileName = "";
            var pos = fullName.lastIndexOf("\\");
            if (pos != -1) {
                fileName = fullName.substring(pos + 1);
            } else {
                pos = fullName.lastIndexOf("/");
                fileName = fullName.substring(pos + 1);
            }

            var content = "";
            if (fullName.substring(0, 4) == "http") {
                content = '<img src="../App_Themes/xp/images/blue-fujian.png" />'
                    + '<a href="' + fullName + '" style="margin-left: 2px;" download>' + fileName + '</a>'
                    + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteHKPickUpFile()">删除</a>';
            } else {
                content = '<img src="../App_Themes/xp/images/blue-fujian.png" />'
                    + '<a href="javascript:void(0);" style="margin-left: 2px; cursor: default;">' + fileName + '</a>'
                    + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteHKPickUpFile()">删除</a>';
            }

            $("#HKPickUpFileDisplay").html(content);
            $("#PickUpFileNotUploadTip").hide();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////// 为修改提货文件上传增加 Begin ///////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        function AddDeliveryFilePlus(data) {
            var displayHtml = '<img src="../App_Themes/xp/images/blue-fujian.png" />';
            if (data.Name.toLowerCase().indexOf("doc") != -1 || data.Name.toLowerCase().indexOf("docx") != -1) {
                displayHtml = displayHtml + '<a href="' + data.Url + '" style="margin-left: 5px;" download>' + data.Name + '</a>';
            } else {
                displayHtml = displayHtml + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + data.Url + '\')">' + data.Name + '</a>';
            }
            displayHtml = displayHtml + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteDeliveryFilePlus()">删除</a>';

            $("#uploadDeliveryFilePlusDisplay").html(displayHtml);

            $("#DeliveryFilePlusName").val(data.Name);
            $("#DeliveryFilePlusFileType").val(data.FileType);
            $("#DeliveryFilePlusFileFormat").val(data.FileFormat);
            $("#DeliveryFilePlusVirtualPath").val(data.VirtualPath);
            $("#DeliveryFilePlusUrl").val(data.Url);
        }

        function DeleteDeliveryFilePlus() {
            var displayHtml = '<div style="color: red; display: block;">未上传</div>';
            $("#uploadDeliveryFilePlusDisplay").html(displayHtml);

            $("#DeliveryFilePlusName").val("");
            $("#DeliveryFilePlusFileType").val("");
            $("#DeliveryFilePlusFileFormat").val("");
            $("#DeliveryFilePlusVirtualPath").val("");
            $("#DeliveryFilePlusUrl").val("");
        }

        function InitDeliveryFilePlusDisplay(lastAllData) {
            if (lastAllData != null && lastAllData != "") {
                if (lastAllData["DeliveryFile"] != null && lastAllData["DeliveryFile"] != "") {
                    var virtualPath = lastAllData["DeliveryFile"].Url;
                    lastAllData["DeliveryFile"].Url = lastAllData["DeliveryFileUrl"];
                    var data = lastAllData["DeliveryFile"];

                    var displayHtml = '<img src="../App_Themes/xp/images/blue-fujian.png" />';
                    if (data.Name.toLowerCase().indexOf("doc") != -1 || data.Name.toLowerCase().indexOf("docx") != -1) {
                        displayHtml = displayHtml + '<a href="' + data.Url + '" style="margin-left: 5px;" download>' + data.Name + '</a>';
                    } else {
                        displayHtml = displayHtml + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + data.Url + '\')">' + data.Name + '</a>';
                    }
                    displayHtml = displayHtml + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteDeliveryFilePlus()">删除</a>';

                    $("#uploadDeliveryFilePlusDisplay").html(displayHtml);

                    $("#DeliveryFilePlusName").val(data.Name);
                    $("#DeliveryFilePlusFileType").val(data.FileType);
                    $("#DeliveryFilePlusFileFormat").val(data.FileFormat);
                    $("#DeliveryFilePlusVirtualPath").val(virtualPath);
                    $("#DeliveryFilePlusUrl").val(data.Url);
                } else {
                    DeleteDeliveryFilePlus();
                }
            } else {
                DeleteDeliveryFilePlus();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////// 为修改提货文件上传增加 End ////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    </script>
    <style type="text/css">
        .auto-style2 {
            width: 336px;
        }

        .auto-style3 {
            width: 83px;
        }

        .auto-style4 {
            height: 24px;
        }

        .auto-style5 {
            width: 336px;
            height: 24px;
        }

        .auto-style6 {
            width: 83px;
        }

        .datagrid-header-row,
        .datagrid-row {
            height: 26px;
        }

        .subtotal {
            font-weight: bold;
        }

        /* 20190507 修改增加 Begin */
        .big-title-container {
            margin-left: 20px;
            margin-top: 20px;
        }

            .big-title-container:first-child {
                margin-top: 10px;
            }

            .big-title-container label {
                font-size: 16px;
                font-weight: 600;
                color: #323232; /*orangered;*/
            }

        .content-container {
            margin-left: 30px;
        }

        #pi-fujian-table td {
            border-width: 0;
            padding: 0;
        }

            #pi-fujian-table td div {
                padding: 0;
            }

        #spanBL_DeliveryFile :not(:first-child) {
            border-color: #95B8E7;
        }
        /* 20190507 修改增加 End */
    </style>
</head>
<body>
    <div id="wholePanel" class="easyui-panel" data-options="border:false,">
        <form id="form1" runat="server" method="post">
            <div>
                <div class="big-title-container">
                    <label>产品录入</label>
                </div>
                <div class="content-container">
                    <div class="Productbtn">
                        <table id="operation" style="padding: 5px">
                            <tr>
                                <td style="text-align: right">交易币种：</td>
                                <td style="text-align: right">
                                    <input class="easyui-combobox" id="Currency" name="Currency" data-options="valueField:'CurrValue',textField:'CurrText',required:true,validType:'comboBoxEditValid[\'Currency\']'" style="width: 200px" />
                                </td>
                                <td>
                                    <a id="btnDownload" href="../Content/templates/报关委托书模板.xls" class="easyui-linkbutton" data-options="iconCls:'icon-save'" style="margin-left: 30px;">下载模板</a>
                                </td>
                                <td>
                                    <div style="margin-left: 10px;">
                                        <%--<a id="btnUpload" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="btnImport_ServerClick">上传产品信息</a>--%>
                                        <input id="uploadExcel" name="uploadExcel" class="easyui-filebox" style="width: 103px; height: 26px;"
                                            data-options="region:'center',buttonText:'上传产品信息',buttonIcon:'icon-add',
                                                            accept:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-left: 5px; margin-right: 5px">
                        <table id="dg">
                            <%--<thead>
                                    <tr>
                                        <th data-options="field:'Batch',width: 100,align:'center',editor:{type:'textbox'}" class="auto-style6">批号</th>
                                        <th data-options="field:'Name',width: 100,align:'center',editor:{type:'textbox',options:{required:true}}" class="auto-style6">品名</th>
                                        <th data-options="field:'Manufacturer',width: 100,align:'center',editor:{type:'textbox',options:{required:true}}" class="auto-style6">品牌</th>
                                        <th data-options="field:'Model',width: 100,align:'center',editor:{type:'textbox',options:{required:true}}" class="auto-style6">型号</th>
                                        <th data-options="field:'Origin',width: 100,align:'center',editor:{type:'textbox'}" class="auto-style6">产地</th>
                                        <th data-options="field:'Qty',width: 100,align:'center',editor:{type:'numberbox',options:{precision:4,required:true}}" class="auto-style6">数量</th>
                                        <th data-options="field:'Unit',width: 100,align:'center',editor:{type:'textbox'}" class="auto-style6">单位</th>
                                        <th data-options="field:'UnitPrice',width: 100,align:'center',editor:{type:'numberbox',options:{precision:4,required:true}}" class="auto-style6">单价</th>
                                        <th data-options="field:'TotalPrice',width: 100,align:'center',
                                            formatter: function (value, row) { 
                                                return new Number(row.UnitPrice * row.Qty).toFixed(5); 
                                            },
                                            editor:{
                                                type:'numberbox',
                                                options:{
                                                    precision:4,
                                                    disabled:true
                                                }   
                                            }"
                                            class="auto-style6">总价</th>
                                        <th data-options="field:'GrossWeight',width: 100,align:'center',editor:{type:'numberbox',options:{precision:4}}" class="auto-style6">毛重</th>
                                        <th data-options="field:'Btn',width: 100,align:'center',formatter:Operation" class="auto-style6">操作</th>
                                    </tr>
                                </thead>--%>
                        </table>
                    </div>
                    <div style="margin: 5px;">
                        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    </div>
                </div>
                <div class="big-title-container">
                    <a name="HKjiaohuofangshi">
                        <label>香港交货方式</label></a>
                </div>
                <div class="content-container">
                    <table id="HKDelivery" class="radioTable">
                        <tr>
                            <td class="auto-style4 auto-style6">交货方式：</td>
                            <td style="text-align: left;" class="auto-style5">
                                <input type="radio" name="HKDeliveryType" value="1" id="SentToHKWarehouse" title="供应商送货" class="radio" checked="checked" /><label for="SentToHKWarehouse" style="margin-right: 50px">供应商送货</label>
                                <input type="radio" name="HKDeliveryType" value="2" id="PickUp" title="提货" class="radio" /><label for="PickUp">提货</label>
                            </td>
                        </tr>
                        <tr id="WayBillTR">
                            <td>物流单号：</td>
                            <td class="auto-style2">
                                <input class="easyui-textbox" id="WayBillNo" name="WayBillNo" style="width: 300px" data-options="validType:'length[1,50]'" />
                            </td>
                        </tr>
                        <tr id="PickUpTimeTR">
                            <td>提货时间：</td>
                            <td class="auto-style2">
                                <input class="easyui-datebox" id="PickupTime" name="PickupTime" type="text" value="" maxlength="50" data-options="required:false,editable:false" style="width: 300px" />
                            </td>
                        </tr>
                        <tr>
                            <td>供应商：</td>
                            <td class="auto-style2">
                                <input class="easyui-combobox" id="Supplier" name="Supplier" data-options="valueField:'ID',textField:'Name',required:true,editable:false" style="width: 300px" />
                                <a id="btnSupplierAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="SupplierAdd(true)"></a>
                            </td>
                        </tr>
                        <tr id="PickUpAddressTR">
                            <td class="lbl">提货地址：</td>
                            <td class="auto-style2" colspan="3">
                                <input class="easyui-combogrid" id="SupplierAddress" name="SupplierAddress" style="width: 740px" data-options="editable:false" />
                                <a id="btnAddressAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="SupplierAddressAdd()"></a>
                            </td>
                        </tr>
                        <tr id="PickUpContactTR">
                            <td class="lbl">提货联系人：</td>
                            <td class="auto-style2">
                                <input class="easyui-textbox" id="SupplierContact" name="SupplierContact" readonly="readonly" style="width: 300px" />
                            </td>
                            <td class="lbl auto-style6">手机号码：</td>
                            <td class="auto-style2">
                                <input class="easyui-textbox" id="SupplierContactMobile" name="SupplierContactMobile" readonly="readonly" style="width: 300px" />
                            </td>
                        </tr>
                        <tr id="PickUpMobileTR">
                        </tr>
                        <tr id="PickUpTR">
                            <td>提货文件：</td>
                            <td class="auto-style2" colspan="3">
                                <%--文件类型参照 https://blog.csdn.net/m0_38000828/article/details/80494516 --%>
                                <div id="DeliveryFile"></div>
                            </td>
                        </tr>
                        <tr id="PickUpTipTR">
                            <td></td>
                            <td colspan="3">
                                <div>仅限图片、pdf、doc、docx格式的文件，且不超过500KB。</div>
                                <div style="margin-top: 15px;">
                                    <div id="HKPickUpFileDisplay">
                                    </div>
                                    <div id="PickUpFileNotUploadTip" style="color: red;">未上传</div>
                                </div>
                            </td>
                        </tr>
                        <!------------------------------------------------------------------------------------------------------------>
                        <!------------------------------------------------------------------------------------------------------------>
                        <!--------------------------------------- 为修改提货文件上传增加 Begin --------------------------------------->
                        <!------------------------------------------------------------------------------------------------------------>
                        <!------------------------------------------------------------------------------------------------------------>

                        <tr id="PickUpTipPlusTR">
                            <td style="vertical-align: top;">提货文件：</td>
                            <td colspan="3">
                                <div>
                                    <input id="uploadDeliveryFilePlus" name="uploadDeliveryFilePlus" class="easyui-filebox" style="width: 57px; height: 24px" />
                                </div>
                                <div style="margin-top: 12px;">仅限图片、pdf、doc、docx格式的文件，且非图片文件不超过3M。</div>
                                <div style="margin-top: 12px;">
                                    <div id="uploadDeliveryFilePlusDisplay">
                                    </div>

                                    <input id="DeliveryFilePlusName" type="hidden" value="" />
                                    <input id="DeliveryFilePlusFileType" type="hidden" value="" />
                                    <input id="DeliveryFilePlusFileFormat" type="hidden" value="" />
                                    <input id="DeliveryFilePlusVirtualPath" type="hidden" value="" />
                                    <input id="DeliveryFilePlusUrl" type="hidden" value="" />
                                </div>
                            </td>
                        </tr>

                        <!------------------------------------------------------------------------------------------------------------>
                        <!------------------------------------------------------------------------------------------------------------>
                        <!---------------------------------------- 为修改提货文件上传增加 End ---------------------------------------->
                        <!------------------------------------------------------------------------------------------------------------>
                        <!------------------------------------------------------------------------------------------------------------>
                    </table>
                </div>
                <div class="big-title-container">
                    <label>国内交货方式</label>
                </div>
                <div class="content-container">
                    <table id="SZDelivery" class="radioTable">
                        <tr>
                            <td class="auto-style6">交货方式：</td>
                            <td style="text-align: left;">
                                <input type="radio" name="SZDeliveryType" value="1" id="PickUpInStore" class="radio" checked="checked" /><label for="PickUpInStore" style="margin-right: 50px">自提</label>
                                <input type="radio" name="SZDeliveryType" value="2" id="SentToClient" class="radio" /><label for="SentToClient" style="margin-right: 50px">送货上门</label>
                                <input type="radio" name="SZDeliveryType" value="3" id="Waybill" class="radio" /><label for="Waybill">国内快递</label>
                            </td>
                        </tr>
                        <tr id="ClientPickerTR">
                            <td>提货人：</td>
                            <td class="auto-style2">
                                <input class="easyui-textbox" id="ClientPicker" name="ClientPicker" value="" style="width: 300px" data-options="required:true,validType:'length[1,150]'" />
                            </td>
                            <td class="auto-style6">手机号码：</td>
                            <td>
                                <input class="easyui-textbox" id="ClientPickerMobile" name="ClientPickerMobile" value="" style="width: 300px" data-options="required:true,validType:'mobile'" />
                            </td>
                        </tr>
                        <tr id="ClientPickerIDTR">
                            <td>证件类型：</td>
                            <td class="auto-style2">
                                <input class="easyui-combobox" id="IDType" name="IDType" data-options="valueField:'TypeValue',textField:'TypeText',required:true,editable:false" style="width: 300px" />
                            </td>
                            <td>证件号码：</td>
                            <td>
                                <input class="easyui-textbox" id="IDNumber" name="IDNumber" value="" style="width: 300px" data-options="required:true,validType:'idnumber'" />
                            </td>
                        </tr>
                        <tr id="ClientConsigneeTR">
                            <td>收货人：</td>
                            <td class="auto-style2">
                                <input class="easyui-combogrid" id="ClientConsignee" name="ClientConsignee" data-options="valueField:'ID',textField:'Name',required:false,editable:false" style="width: 300px" />
                                <a id="btnPersonAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="ClientConsigneeAdd()"></a>
                            </td>
                        </tr>
                        <tr id="ClientConsigneeAddressTR">
                            <td class="auto-style6">收货地址：</td>
                            <td colspan="3">
                                <input class="easyui-textbox" id="ClientConsigneeAddress" name="ClientConsigneeAddress" style="width: 740px" readonly="readonly" />
                            </td>
                        </tr>
                        <tr id="ClientContactTR">
                            <td>联系人：</td>
                            <td class="auto-style2">
                                <input class="easyui-textbox" id="ClientContact" name="ClientContact" readonly="readonly" style="width: 300px" />
                            </td>
                            <td class="auto-style6">手机号码：</td>
                            <td>
                                <input class="easyui-textbox" id="ClientContactMobile" name="ClientContactMobile" style="width: 300px" readonly="readonly" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="big-title-container">
                    <label>付汇供应商</label>
                </div>
                <div class="content-container">
                    <div class="PaymentSupplier">
                        <table id="PaymentSupplier" class="radioTable">
                            <tr>
                                <td class="auto-style3">供应商：</td>
                                <td>
                                    <input class="easyui-combobox" id="PayExchangeSupplier" name="PayExchangeSupplier" data-options="valueField:'ID',textField:'Name',required:true,multiple:true,editable:false" style="width: 300px" />
                                    <a id="btnPaymentsupplierAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="SupplierAdd(false)"></a>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <span style="color: #faa613;">提示：请选择付汇供应商，最多可选择3个，并确保与合同发票 （INVOICE LIST）一致</span>
                                </td>
                            </tr>
                            <tr>
                                <td>合同发票：</td>
                                <td>
                                    <input id="uploadPI" name="uploadPI" class="easyui-filebox" style="width: 57px; height: 24px" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <label>仅限图片、pdf格式的文件，且pdf文件不超过3M。</label>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
                                        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                                        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                                    </div>
                                    <div id="pi-fujian-table" class="ProductTable pi">
                                        <%--<table id="pi" data-options="pageSize:50,fitColumns:true,fit:false,pagination:false,queryParams:{ action: 'dataPI' }">
                                            <thead>
                                                <tr>
                                                    <th data-options="field:'Name',width: 100,align:'center'">文件名</th>
                                                    <th data-options="field:'FileType',width: 100,align:'center'">文件类型</th>
                                                    <th data-options="field:'FileFormat',width: 100,align:'center'">文件格式</th>
                                                    <th data-options="field:'Btn',width: 100,align:'center',formatter:OperationFile">操作</th>
                                                </tr>
                                            </thead>
                                        </table>--%>

                                        <table id="pi" data-options="pageSize:50,fitColumns:true,fit:false,pagination:false,queryParams:{ action: 'dataPI' }">
                                            <%--<thead>
                                                <tr>
                                                    <th data-options="field:'info',width: 100,align:'left',formatter:ShowInfo"">附件信息</th>
                                                </tr>
                                            </thead>--%>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="big-title-container">
                    <label>其他信息</label>
                </div>
                <div class="content-container">
                    <table id="OtherInfo" class="radioTable">
                        <tr>
                            <td class="auto-style6">是否包车：</td>
                            <td class="auto-style2">
                                <input type="radio" name="IsFullVehicle" value="true" id="IsFullVehicletrue" title="是" class="radio" /><label for="IsFullVehicletrue" style="margin-right: 30px">是</label>
                                <input type="radio" name="IsFullVehicle" value="false" id="IsFullVehiclefalse" title="否" class="radio" checked="checked" /><label for="IsFullVehiclefalse">否</label>
                            </td>
                            <td id="Isloadlbl" class="auto-style6">是否垫付款：</td>
                            <td id="Isloadradio">
                                <input type="radio" name="IsLoan" value="true" id="IsLoantrue" title="是" class="radio" /><label for="IsLoantrue" style="margin-right: 30px">是</label>
                                <input type="radio" name="IsLoan" value="false" id="IsLoanfalse" title="否" class="radio" checked="checked" /><label for="IsLoanfalse">否</label>
                            </td>
                        </tr>
                        <tr>
                            <td>包装类型：</td>
                            <td class="auto-style2">
                                <input class="easyui-combobox" id="WarpType" name="WarpType" data-options="valueField:'TypeValue',textField:'TypeText',required:true,validType:'comboBoxEditValid[\'WarpType\']'" style="width: 300px" />
                            </td>
                            <td>件数：</td>
                            <td>
                                <input class="easyui-numberbox" id="PackNo" name="PackNo" value="" data-options="precision:'0',validType:'packNo'" style="width: 300px" />
                            </td>
                        </tr>
                        <tr>
                            <td>备注：</td>
                            <td colspan="3">
                                <input class="easyui-textbox" id="Summary" name="Summary" value="" style="width: 100%; height: 100px" data-options="multiline:true,required:false,validType:'length[1,500]'" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divSave" class="content-container" style="margin-top: 20px; margin-bottom: 20px">
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="SaveOrder(true)" style="margin-left: 97px;">确认下单</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="SaveOrder(false)" style="margin-left: 10px;">保存为草稿</a>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
