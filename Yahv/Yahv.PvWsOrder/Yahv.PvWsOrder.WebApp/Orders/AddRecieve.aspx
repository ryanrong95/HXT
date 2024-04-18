<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="AddRecieve.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.AddRecieve" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var currencyData = model.currencyData;
        var originData = model.originData;
        var packageData = model.packageData;
        var unitData = model.unitData;
        //var paymentType = model.paymentType;
        //客户供应商
        var supplierData = model.supplierData;
        var beneficiaryData = model.beneficiaryData;
        var companyData = model.companyData;
        var carrierData = model.carrierData;

        var ClientID = getQueryString("ID");
        var EnterCode = getQueryString("EnterCode");
        var firstLoad = true;
        var hk_DeliveryType = <%= (int)Yahv.Underly.WaybillType.PickUp%>;
        var hkgOrigin =<%=(int)Yahv.Underly.Origin.HKG %>;
        var SettlementCurrency =<%=(int)Yahv.Underly.Currency.CNY %>;
        var packageDefault =<%=(int)Yahv.Underly.Package.纸制或纤维板制盒%>;

        $(function () {
            //产品列表初始化
            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                rownumbers: true,
                fitColumns: true,
                fit: false,
                scrollbarSize: 0,
                pagination: false,
                checkOnSelect: false,
                selectOnCheck: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'DateCode', title: '批次号', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'PartNumber', title: '型号', width: 130, align: 'left',
                        editor: { 
                            type: 'textbox',
                            options: { 
                                required: true, validType: 'length[1,50]',
                                onChange:function(newValue,oldValue){
                                    if(editIndex!=undefined && newValue!=oldValue){ 
                                        var rows = $('#dg').datagrid('getRows');
                                        var row = rows[editIndex];
                                        if(CheckIsNullOrEmpty(row.StorageID)){
                                            row.PartNumber=oldValue;
                                            $('#dg').datagrid('refreshRow', editIndex);
                                            $.messager.alert('提示', '暂存导入产品不可编辑型号'); 
                                            onClickRow(rows.length-1);
                                        }
                                    }
                                }
                            }
                        } 
                    },
                    { field: 'Manufacturer', title: '品牌', width: 130, align: 'left',
                        editor: { 
                            type: 'textbox', 
                            options: { 
                                required: true, validType: 'length[1,50]',
                                onChange:function(newValue,oldValue){
                                    if(editIndex!=undefined && newValue!=oldValue){ 
                                        var rows = $('#dg').datagrid('getRows');
                                        var row = rows[editIndex];
                                        if(CheckIsNullOrEmpty(row.StorageID)){
                                            row.Manufacturer=oldValue;
                                            $('#dg').datagrid('refreshRow', editIndex);
                                            $.messager.alert('提示', '暂存导入产品不可编辑品牌'); 
                                            onClickRow(rows.length-1);
                                        }
                                    }
                                }
                            }
                        } 
                    },
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
                        field: 'Qty', title: '数量', width: 50, align: 'center',
                        editor: { 
                            type: 'numberbox', 
                            options: { 
                                min: 0, precision: 0, required: true,
                                onChange:function(newValue,oldValue){
                                    if(editIndex!=undefined && newValue!=oldValue){ 
                                        var rows = $('#dg').datagrid('getRows');
                                        var row = rows[editIndex];
                                        if(CheckIsNullOrEmpty(row.StorageID)){
                                            row.Qty=oldValue;
                                            $('#dg').datagrid('refreshRow', editIndex);
                                            $.messager.alert('提示', '暂存导入产品不可编辑数量'); 
                                            onClickRow(rows.length-1);
                                        }
                                    }
                                }
                            } 
                        }
                    },
                    {
                        field: 'Unit', title: '单位', width: 50, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < unitData.length; i++) {
                                if (unitData[i].UnitValue == value) return unitData[i].UnitText;
                            }
                            return value;
                        },
                        editor: {
                            type: 'combobox', options: {
                                data: unitData, valueField: "UnitValue", textField: "UnitText", required: true, hasDownArrow: true,
                                onSelect: function (value) {
                                    if (value.UnitValue != '007') {
                                        $.messager.alert('提示', '请仔细核对产品单位，如果产品单位选择错误，造成无法报关，芯达通将不承担任何责任');
                                    }
                                }
                            }
                        }
                    },
                    {
                        field: 'TotalPrice', title: '总价值', width: 50, align: 'center',
                        editor: { type: 'numberbox', options: { min: 0, precision: 2, required: true, } },
                    },
                    {
                        field: 'GrossWeight', title: '毛重(kg)', width: 50, align: 'center',
                        formatter: function (value, row) {
                            if (value != undefined && value != null && value != '') {
                                if (value.toString().indexOf('<span class="subtotal">') != -1) {
                                    return value;
                                }
                                return parseFloat(value).toFixed(4);
                            }
                        },
                        editor: { type: 'numberbox', options: { min: 0.02, precision: 4 } }
                    },
                    {
                        field: 'Volume', title: '体积(m³)', width: 50, align: 'center',
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
                    { field: 'TaxCode', title: '税务编码', width: 80, align: 'center', hidden: true, editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Currency', title: '币种', width: 80, align: 'center', hidden: true },
                    { field: 'StorageID', title: '暂存编号', width: 80, align: 'center', hidden: true },
                    { field: 'Btn', title: '操作', width: 80, align: 'center', formatter: Operation }
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddSubtotalRow();
                        firstLoad = false;
                    }
                    //获取require的列title,添加*
                    markTitle();
                }
            });

            $("#currency").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: currencyData,
                onChange: function () {
                    BandingBeneficiary();
                }
            })
            $("#SettlementCurrency").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: currencyData,
            })
            $("#SettlementCurrency").combobox('setValue', SettlementCurrency)//默认人民币
            $("#sourceDistrict").combobox({
                required: true,
                valueField: 'Value',
                textField: 'Text',
                data: originData,
            })
            $("#sourceDistrict").combobox('setValue', hkgOrigin)//默认中国香港
            $("#sourceDistrict").combobox('textbox').bind('blur', function (e) {
                var value = $("#sourceDistrict").combobox("getValue");
                var text = $("#sourceDistrict").combobox("getText");
                if (value === text && text != "") {
                    $.messager.alert('提示', "请选择有效的发货地。");
                    $("#sourceDistrict").combobox("setValue", "");
                    $("#sourceDistrict").combobox("setText", "");
                }
            });
            //包装类型
            $("#package").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: packageData,
            })
            $("#package").combobox("setValue", packageDefault)//默认纸箱
            //上传产品信息
            $('#btnUpload').filebox({
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传产品信息',
                buttonIcon: 'icon-yg-excelExport',
                width: 103,
                height: 22,
                accept: ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#btnUpload').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#btnUpload').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData();
                    formData.append('btnUpload', $("input[name='btnUpload']").get(0).files[0]);
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
                                RemoveSubtotalRow();
                                var data = res.data;
                                for (var index = 0; index < data.length; index++) {
                                    $('#dg').datagrid('insertRow', {
                                        row: {
                                            DateCode: data[index].DateCode,
                                            PartNumber: data[index].PartNumber,
                                            Manufacturer: data[index].Manufacturer,
                                            Origin: data[index].Origin,
                                            Qty: data[index].Qty,
                                            Unit: data[index].Unit,
                                            TotalPrice: data[index].TotalPrice,
                                            TaxCode: data[index].TaxCode,
                                            Currency: data[index].Currency,
                                            GrossWeight: data[index].GrossWeight,
                                            Volume: data[index].Volume,
                                        }
                                    });
                                }
                                AddSubtotalRow();
                                loadData();
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            })
            //上传合同发票
            $('#uploadInvoice').filebox({
                multiple: true,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '合同发票',
                buttonIcon: 'icon-yg-add',
                width: 80,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadInvoice').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadInvoice').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    var files = $("input[name='uploadInvoice']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadInvoice', bl, fileName);
                                //上传文件
                                UploadInvoice(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadInvoice', file);
                            //上传文件
                            UploadInvoice(formData);
                        }
                    }
                }
            })
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadInvoice',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationInvoice }
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
            //上传提货文件
            $('#uploadDelivery').filebox({
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '提货文件',
                buttonIcon: 'icon-yg-add',
                width: 80,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf','application/msword','application/vnd.openxmlformats-officedocument.wordprocessingml.document'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadDelivery').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadDelivery').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    var files = $("input[name='uploadDelivery']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadDelivery', bl, fileName);
                                //上传文件
                                UploadDelivery(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadDelivery', file);
                            //上传文件
                            UploadDelivery(formData);
                        }
                    }
                }
            })
            $('#delivery').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadDelivery',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationDelivery }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".delivery");
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
            $("#Supplier").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: supplierData,
                onChange: function () {
                    BandingBeneficiary();
                }
            })
            $("#Beneficiary").combogrid({
                editable: false,
                fitColumns: true,
                nowrap: false,
                idField: "Value",
                textField: "Text",
                data: beneficiaryData,
                panelWidth: 500,
                mode: "local",
                columns: [[
                    { field: 'Name', title: '公司名称', width: 100, align: 'left' },
                    { field: 'Text', title: '银行账号', width: 100, align: 'left' },
                    { field: 'BankName', title: '开户行', width: 120, align: 'left' },
                    { field: 'Currency', title: '币种', width: 50, align: 'center' },
                    { field: 'Method', title: '汇款方式', width: 50, align: 'center' },
                ]],
            })
            //上门自提
            $("#rec_Pick").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".pickUp").css("display", "table-row");
                        $(".send").css("display", "none");
                        $("#pickUpTime").textbox({ required: true });
                        $("#pickUpAddress").combogrid({ required: true });
                        $("#pickUpName").textbox({ required: true });
                        $("#pickUpTel").textbox({ required: true });

                        hk_DeliveryType = <%= (int)Yahv.Underly.WaybillType.PickUp%>;
                    }
                    else {
                        $("#pickUpTime").textbox({ required: false });
                        $("#pickUpAddress").combogrid({ required: false });
                        $("#pickUpName").textbox({ required: false });
                        $("#pickUpTel").textbox({ required: false });
                    }
                }
            })
            $("#rec_Send").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".pickUp").css("display", "none");
                        $(".send").css("display", "table-row");
                        hk_DeliveryType = <%= (int)Yahv.Underly.WaybillType.DeliveryToWarehouse%>;
                    }
                }
            })
            $("#rec_LocalExpress").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".pickUp").css("display", "none");
                        $(".send").css("display", "table-row");
                        hk_DeliveryType = <%= (int)Yahv.Underly.WaybillType.LocalExpress%>;
                    }
                }
            })
            $("#rec_InternationalExpress").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".pickUp").css("display", "none");
                        $(".send").css("display", "table-row");
                        $("#VoyageNumber").textbox({ disabled: false, })
                        hk_DeliveryType = <%= (int)Yahv.Underly.WaybillType.InternationalExpress%>;
                    }
                    else {
                        $("#VoyageNumber").textbox({ disabled: true, })
                    }
                }
            })
            //是否代付运费
            $("#isPayForFreight ").checkbox({
                onChange: function (record) {
                    var Carrier = $("#Carrier").combobox("getValue");
                    if (record) {
                        $("#Carrier").combobox({ disabled: false, required: true, })
                    }
                    else {
                        $("#Carrier").combobox({ required: false, })
                    }
                    $("#Carrier").combobox("setValue",Carrier);
                }
            })
            $("#Carrier").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: carrierData,
            })
            //提货时间（只能当前日期和其之后日期）
            $('#pickUpTime').datetimebox('calendar').calendar({
                validator: function (date) {
                    var now = new Date();
                    var d1 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    return date >= d1;
                }
            })
            $("#pickUpAddress").combogrid({
                fitColumns: true,
                idField: "Value",
                textField: "Text",
                panelWidth: 430,
                mode: "local",
                columns: [[
                    { field: 'Text', title: '收货地址', width: 250, align: 'left' },
                    { field: 'Contact', title: '联系人', width: 80, align: 'left' },
                    { field: 'Tel', title: '联系电话', width: 100, align: 'left' },
                ]],
                onSelect: function (data) {
                    var g = $('#pickUpAddress').combogrid('grid');	// get datagrid object
                    var res = g.datagrid('getSelected');	// get the selected row
                    $("#pickUpName").textbox("setValue", res.Contact);
                    $("#pickUpTel").textbox("setValue", res.Tel);
                }
            })
            //新增一行
            $("#btnAddRow").click(function () {
                if (endEditing()) {
                    var rows = $('#dg').datagrid('getRows');
                    if (rows.length > 100) {
                        $.messager.alert('提示', '产品型号不能超过100个！');
                        return;
                    }
                    RemoveSubtotalRow();

                    //设置默认数据
                    $('#dg').datagrid('appendRow', {
                        Unit: '007',
                        GrossWeight: 0.02,
                        Volume: 0.00
                    });

                    editIndex = $('#dg').datagrid('getRows').length - 1;
                    $('#dg').datagrid('selectRow', editIndex)
                        .datagrid('beginEdit', editIndex);

                    AddSubtotalRow();
                }
            })
            ////暂存导入
            //$("#btnImportStorage").click(function(){
            //    $.myWindow.setMyWindow('ParentWindow', window);
            //    $.myWindow({
            //        title: "我的暂存库存",
            //        url: location.pathname.replace('Orders/AddRecieve.aspx', 'Stocks/Temporary/MyList.aspx?EnterCode='+EnterCode),
            //    });
            //    return false;
            //});
            //提交订单
            $("#btnSubmit").click(function () {
                endEditing();
                //各种验证
                if (!ValidationOrder()) {
                    return;
                }
                var data = new FormData();
                //基本信息
                data.append('clientID', ClientID);
                data.append('enterCode', EnterCode);
                data.append('currency', $("#currency").combobox("getValue"));
                data.append('SettlementCurrency', $("#SettlementCurrency").combobox("getValue"));
                data.append('sourceDistrict', $("#sourceDistrict").combobox("getValue"));
                data.append('Supplier', $("#Supplier").combobox("getValue"));
                data.append('SupplierName', $("#Supplier").combobox("getText"));
                data.append('Beneficiary', $("#Beneficiary").combogrid("getValue"));
                //产品信息
                var rows = $('#dg').datagrid('getRows');
                var products = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    products.push(rows[i]);
                }
                data.append('products', JSON.stringify(products));
                data.append('totalPrice', compute2('TotalPrice'));
                //香港交货信息
                data.append('hk_DeliveryType', hk_DeliveryType);
                data.append('pickUpTime', $("#pickUpTime").datetimebox("getValue"));
                data.append('pickUpAddress', $("#pickUpAddress").combogrid("getText"));
                data.append('pickUpName', $("#pickUpName").textbox("getValue"));
                data.append('pickUpTel', $("#pickUpTel").textbox("getValue"));
                data.append('WaybillCode', $("#WaybillCode").textbox("getValue"));
                data.append('isPayForFreight', $('#isPayForFreight').checkbox('options').checked);
                data.append('Carrier', $("#Carrier").combobox("getValue"));
                data.append('VoyageNumber', $("#VoyageNumber").textbox("getValue"));
                //其它交货信息
                data.append('package', $("#package").combobox("getValue"));
                data.append('TotalPackages', $("#TotalPackages").numberbox("getValue"));
                data.append('TotalWeight', $("#TotalWeight").numberbox("getValue"));
                data.append('TotalVolume', $("#TotalVolume").numberbox("getValue"));
                data.append('Summary', $("#Summary").textbox("getValue"));
                data.append('isUnBox', $('#isUnBox').checkbox('options').checked);
                data.append('isDetection', $('#isDetection').checkbox('options').checked);
                //文件信息
                var piRows = $('#pi').datagrid('getRows');
                var invoices = [];
                for (var i = 0; i < piRows.length; i++) {
                    invoices.push(piRows[i]);
                }
                data.append('invoices', JSON.stringify(invoices));

                var deliveryRows = $('#delivery').datagrid('getRows');
                var deliverys = [];
                for (var i = 0; i < deliveryRows.length; i++) {
                    deliverys.push(deliveryRows[i]);
                }
                data.append('deliverys', JSON.stringify(deliverys));

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
            //关闭窗口
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
        });
    </script>
    <script>
        //产品操作
        function Operation(val, row, index) {
            if (val != undefined && val != null) {
                if (val.toString().indexOf('<span class="subtotal">') != -1) {
                    return val;
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
            //验证是否有提货文件
            if ($("#rec_Pick").radiobutton("options").checked) {
                var rows = $("#delivery").datagrid("getRows")
                if (rows.length == 0) {
                    $.messager.alert('提示', '请上传提货文件');
                    return false;
                }
            }
            //验证订单项数量(注：合计行)
            var rows = $("#dg").datagrid("getRows");
            if (rows.length < 2) {
                $.messager.alert('提示', '订单项数量不可为空');
                return false;
            }
            if (rows.length > 101) {
                $.messager.alert('提示', '订单项数量不能超过100项');
                return false;
            }
            //验证订单项必填项
            for (var i = 0; i < rows.length - 1; i++) {
                var row = rows[i];
                if(!(CheckIsNullOrEmpty(row.PartNumber)&&CheckIsNullOrEmpty(row.Manufacturer)&&
                    CheckIsNullOrEmpty(row.Qty)&&CheckIsNullOrEmpty(row.TotalPrice)))
                {
                    $.messager.alert('提示', '序号为'+(i+1)+'的订单项中必填项存在空值');
                    return false
                }
            }
            return true;
        }
        //绑定供应商收益人
        function BandingBeneficiary() {
            var SupplierID = $("#Supplier").combobox('getValue');
            var Currency = $("#currency").combobox('getValue');
            if (CheckIsNullOrEmpty(SupplierID) && CheckIsNullOrEmpty(Currency)) {
                //两筛选参数都不为空执行
                $.post('?action=SelectSupplier', { SupplierID: SupplierID, ClientID: ClientID, Currency: Currency }, function (result) {
                    var rel = JSON.parse(result);
                    if (rel.success) {
                        //绑定受益人数据
                        $('#Beneficiary').combogrid({
                            data: eval(rel.data)
                        });
                        //供应商收货地址
                        $('#pickUpAddress').combogrid({
                            data: eval(rel.consignee)
                        });
                    }
                    else {
                        $.messager.alert('提示', rel.data);
                    }
                })
            }
        }
        //发票文件操作
        function OperationInvoice(val, row, index) {
            return '<img src="../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteInvoice(' + index + ')">删除</a>';
        }
        //上传发票文件
        function UploadInvoice(formData) {
            $.ajax({
                url: '?action=UploadInvoice',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = eval(res.data);
                        for (var i = 0; i < data.length; i++) {
                            $('#pi').datagrid('insertRow', {
                                row: {
                                    ID: data[i].ID,
                                    CustomName: data[i].CustomName,
                                    FileName: data[i].FileName,
                                    FileType: data[i].FileType,
                                    Url: data[i].Url
                                }
                            });
                        }
                        var data = $('#pi').datagrid('getData');
                        $('#pi').datagrid('loadData', data);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            })
        }
        //删除合同发票
        function DeleteInvoice(index) {
            $('#pi').datagrid('deleteRow', index);
            var data = $('#pi').datagrid('getData');
            $('#pi').datagrid('loadData', data);
        }
        //提货文件操作
        function OperationDelivery(val, row, index) {
            return '<img src="../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteDelivery(' + index + ')">删除</a>';
        }
        //上传提货文件
        function UploadDelivery(formData) {
            $.ajax({
                url: '?action=UploadDelivery',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = eval(res.data);
                        for (var i = 0; i < data.length; i++) {
                            $('#delivery').datagrid('insertRow', {
                                row: {
                                    ID: data[i].ID,
                                    CustomName: data[i].CustomName,
                                    FileName: data[i].FileName,
                                    FileType: data[i].FileType,
                                    Url: data[i].Url
                                }
                            });
                        }
                        var data = $('#delivery').datagrid('getData');
                        $('#delivery').datagrid('loadData', data);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }
        //删除合同发票
        function DeleteDelivery(index) {
            $('#delivery').datagrid('deleteRow', index);
            var data = $('#delivery').datagrid('getData');
            $('#delivery').datagrid('loadData', data);
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0||url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
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
                DateCode: '<span class="subtotal">合计：</span>',
                PartNumber: '<span class="subtotal">--</span>',
                Manufacturer: '<span class="subtotal">--</span>',
                Origin: '<span class="subtotal">--</span>',
                Qty: '<span class="subtotal">' + compute('Qty') + '</span>',
                Unit: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
                GrossWeight: '<span class="subtotal">--</span>',
                Volume: '<span class="subtotal">--</span>',
                TaxCode: '<span class="subtotal">--</span>',
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
        //标记表格Title
        function markTitle() {
            var fields = $("#dg").datagrid('getColumnFields');
            var requiredHeaders = [];
            for (i = 0; i < fields.length - 3; i++) {
                var opt = $("#dg").datagrid('getColumnOption', fields[i]);
                var editor = opt.editor;
                var options = editor.options;
                if (options != null && options.required) {
                    requiredHeaders.push(opt.title);
                }
            }
            $(".datagrid-header-row td div span").each(function (i, th) {
                var val = $(th).text();
                if (requiredHeaders.indexOf(val) != -1) {
                    $(th).html(val + '<span style="color:red;font-weight:bold;vertical-align:middle;">' + " *" + '</span>');
                }
            });
        }
    </script>
    <style>
        .title {
            background-color: #F5F5F5;
            color: #575765;
            font-weight: 600;
        }

        .lbl {
            width: 100px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr>
                    <td class="lbl">订单产品币种</td>
                    <td>
                        <input id="currency" name="currency" class="easyui-combobox" style="width: 200px" />
                    </td>
                    <td class="lbl">订单结算币种</td>
                    <td>
                        <input id="SettlementCurrency" name="SettlementCurrency" class="easyui-combobox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">供应商名称</td>
                    <td>
                        <input id="Supplier" class="easyui-combobox" style="width: 200px"
                            data-options="prompt:'客户供应商'" />
                    </td>

                    <td class="lbl">供应商账户</td>
                    <td>
                        <input id="Beneficiary" class="easyui-combogrid" style="width: 200px; height: 22px"
                            data-options="prompt:'供应商受益人账户'" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <a id="btnDownload" href="../Content/templates/代仓储委托书模板.xls" class="easyui-linkbutton" iconcls="icon-yg-excelImport">下载导入模板</a>
                        <input id="btnUpload" name="btnUpload" class="easyui-filebox" />
                        <%--<a id="btnImportStorage" class="easyui-linkbutton" iconcls="icon-yg-add">暂存库存导入</a>--%>
                        <a id="btnAddRow" class="easyui-linkbutton" iconcls="icon-yg-add">新增一行</a>
                    </td>
                </tr>
            </table>
            <table id="dg">
            </table>
            <table class="liebiao">
                <tr>
                    <td colspan="8" class="title">香港交货方式</td>
                </tr>
                <tr>
                    <td class="lbl">发货地区</td>
                    <td colspan="7">
                        <input id="sourceDistrict" name="sourceDistrict" class="easyui-combobox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">交货类型</td>
                    <td colspan="7">
                        <input class="easyui-radiobutton" id="rec_Pick" name="HK_DeliveryType" data-options="labelPosition:'after',checked: true,label:'上门提货'">
                        <input class="easyui-radiobutton" id="rec_Send" name="HK_DeliveryType" data-options="labelPosition:'after',label:'送货上门'">
                        <input class="easyui-radiobutton" id="rec_LocalExpress" name="HK_DeliveryType" data-options="labelPosition:'after',label:'本港快递'">
                        <input class="easyui-radiobutton" id="rec_InternationalExpress" name="HK_DeliveryType" data-options="labelPosition:'after',label:'国际快递'">
                    </td>
                </tr>
                <tr class="pickUp">
                    <td class="lbl">提货时间</td>
                    <td>
                        <input id="pickUpTime" class="easyui-datetimebox" style="width: 200px; height: 22px"
                            data-options="required:true,editable:false" />
                    </td>
                    <td class="lbl">提货地址</td>
                    <td>
                        <input id="pickUpAddress" class="easyui-combogrid" style="width: 200px;height: 22px"
                            data-options="required:true," />
                    </td>
                </tr>
                <tr class="pickUp">
                    <td class="lbl">联系人</td>
                    <td>
                        <input id="pickUpName" class="easyui-textbox" style="width: 200px;"
                            data-options="required:true," />
                    </td>
                    <td class="lbl">联系电话</td>
                    <td>
                        <input id="pickUpTel" class="easyui-textbox" style="width: 200px"
                            data-options="required:true," />
                    </td>
                </tr>
                <tr class="pickUp">
                    <td class="lbl">提货文件</td>
                    <td colspan="7">
                        <div>
                            <input id="uploadDelivery" name="uploadDelivery" class="easyui-filebox" />
                        </div>
                        <div class="delivery" style="width: 1000px">
                            <table id="delivery">
                            </table>
                        </div>
                    </td>
                </tr>
                <tr class="send" style="display: none">
                    <td class="lbl">运单号</td>
                    <td>
                        <input id="WaybillCode" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">垫付运费</td>
                    <td>
                        <div style="width: 200px">
                            <input id="isPayForFreight" name="isPayForFreight" class="easyui-checkbox" value="true"
                                data-options="label:'是否垫付运费',labelPosition:'after'">
                        </div>
                    </td>
                </tr>
                <tr class="send" style="display: none">
                    <td class="lbl">承运商</td>
                    <td>
                        <input id="Carrier" class="easyui-combobox" style="width: 200px" data-options="prompt:'公司的承运商'" />
                    </td>
                    <td class="lbl">航次号</td>
                    <td>
                        <input id="VoyageNumber" class="easyui-textbox" style="width: 200px"
                            data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">合同发票(PI)</td>
                    <td colspan="7">
                        <div>
                            <input id="uploadInvoice" name="uploadInvoice" class="easyui-filebox" />
                        </div>
                        <div class="pi" style="width: 1000px">
                            <table id="pi">
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
            <table class="liebiao">
                <tr>
                    <td colspan="8" class="title">其它交货信息</td>
                </tr>
                <tr>
                    <td class="lbl">包装类型</td>
                    <td>
                        <input id="package" class="easyui-combobox" style="width: 200px" />
                    </td>
                    <td class="lbl">总件数</td>
                    <td>
                        <input id="TotalPackages" class="easyui-numberbox" style="width: 200px"
                            data-options="min:1" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">总毛重(kg)</td>
                    <td>
                        <input id="TotalWeight" class="easyui-numberbox" style="width: 200px"
                            data-options="min:0,precision:4" />
                    </td>
                    <td class="lbl">总体积(m³)</td>
                    <td>
                        <input id="TotalVolume" class="easyui-numberbox" style="width: 200px"
                            data-options="min:0,precision:4" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">特殊服务</td>
                    <td colspan="7">
                        <input id="isUnBox" name="isUnBox" class="easyui-checkbox" value="true"
                            data-options="label:'是否拆箱验货',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="isDetection" name="isDetection" class="easyui-checkbox" value="true"
                            data-options="label:'是否代检测',labelPosition:'after'">
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注信息</td>
                    <td colspan="7">
                        <input id="Summary" class="easyui-textbox" style="width: 200px; height: 50px"
                            data-options="multiline:true,validType:'length[1,300]'" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px;height:500px;min-width:70%;min-height:80%">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
