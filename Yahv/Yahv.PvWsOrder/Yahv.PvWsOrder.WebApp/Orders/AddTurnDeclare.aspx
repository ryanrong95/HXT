<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="AddTurnDeclare.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.AddTurnDeclare" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ClientID = getQueryString("ClientID");
        var EnterCode = getQueryString("EnterCode");
        var sz_DeliveryType = <%= (int)Yahv.Underly.WaybillType.PickUp%>;
        var hkgOrigin=<%=(int)Yahv.Underly.Origin.HKG %>;
        var firstLoad = true;
        
        var currencyData = model.currencyData;
        var originData = model.originData;
        var packageData = model.packageData; 
        var fileType = model.fileType; 
        var paymentType = model.paymentType;
        var idType = model.idType;
        var supplierData = model.supplierData;//客户供应商
        var beneficiaryData = model.beneficiaryData;
        var companyData = model.companyData;
        var consigeeData = model.consigeeData;
        $(function () {           
            //产品列表初始化
            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                rownumbers:true,
                fitColumns: true,
                fit: false,
                scrollbarSize: 0,
                pagination: false,
                checkOnSelect:false,
                selectOnCheck:false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'DateCode', title: '批次号', width: 80, align: 'center'},
                    { field: 'CustomName', title: '海关品名', width: 80, align: 'center', editor: { type: 'textbox', options: {required: true, validType: 'length[1,50]' } } },
                    { field: 'PartNumber', title: '型号', width: 130, align: 'left'},
                    { field: 'Manufacturer', title: '品牌', width: 130, align: 'left'},
                    { field: 'OriginDec', title: '产地', width: 50, align: 'center'},
                    { field: 'TotalQuantity', title: '库存数量', width: 50, align: 'center' },
                    {
                        field: 'Quantity', title: '发货数量', width: 50, align: 'center',
                        editor: { type: 'numberbox', options: { min: 0, precision: 0, required: true, }}
                    },
                    { field: 'UnitDec', title: '单位', width: 50, align: 'center'},
                    { field: 'CurrencyDec', title: '库存币种', width: 50, align: 'center'},
                    {
                        field: 'TotalPrice', title: '总价值', width: 50, align: 'center',
                        editor: { type: 'numberbox', options: { min: 0, precision: 2, required: true, } },
                    },
                    { field: 'GrossWeight', title: '毛重(kg)', width: 50, align: 'center'},
                    { field: 'Volume', title: '体积(m³)', width: 50, align: 'center'},
                    { field: 'Btn', title: '操作', width: 80, align: 'center', formatter: Operation },
                    { field: 'Currency', title: '', width: 50, align: 'center',hidden: true },
                    { field: 'Origin', title: '', width: 50, align: 'center',hidden: true },
                    { field: 'Unit', title: '', width: 50, align: 'center',hidden: true },
                    { field: 'StorageID', title: '', width: 50, align: 'center',hidden: true },
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddSubtotalRow();
                        firstLoad = false;
                    }
                    //获取require的列title,添加*
                    markTitle();
                    $("#currency").combobox("setValue",data.rows[0].Currency);
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
            $("#sourceDistrict").combobox({
                required: true,
                disabled:true,
                valueField: 'Value',
                textField: 'Text',
                data: originData,
            })          
            $("#sourceDistrict").combobox('setValue',hkgOrigin)//默认中国香港
            $("#IDCardType").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',                
                data: idType,
            })
            //是否代付货款        
            $("#supplier").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: supplierData,
                onChange: function () {
                    BandingBeneficiary();
                }
            })
            $("#paySupplier").combobox({
                required: true,
                editable: false,
                multiple:true,
                multiline:true,
                valueField: 'Value',
                textField: 'Text',
                data: supplierData,
                onChange : function(){
                    var values = $("#paySupplier").combobox("getValues");
                    if(values.length>3){
                        $("#paySupplier").combobox("unselect",values[values.length-1]);
                        $.messager.alert('提示', "客户付汇供应商不能超过3个.");
                    }
                }
            })
            $("#beneficiary").combogrid({
                editable: false,
                required:false,
                multiple:false,
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
            //国内交货信息
            SZ_Delivery();
            $("#companyName").combobox({
                required: false,
                valueField: 'Value',
                textField: 'Text',                
                data: consigeeData,
                onChange: function () {
                    var consigeeID = $(this).combobox('getValue');
                    $.post('?action=SelectConsigee', { ConsigeeID: consigeeID }, function (result) {
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            //绑定数据
                            $("#address").textbox("setValue",rel.data[0].Address);
                            $("#contacts").textbox("setValue",rel.data[0].Name);
                            $("#phone").textbox("setValue",rel.data[0].Mobile);
                        }
                    })
                }
            })
            //其它信息
            //上传文件
            $("#fileType").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data:fileType,
                onChange:function(){
                    $("#uploadFile").filebox({
                        disabled:false
                    })
                }
            })
            $('#file').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName:'LoadFile',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center',hidden: true,},
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100,align: 'left', formatter: OperationFile }
                ]],
                onLoadSuccess: function (data) {
                    var obj =$(".file");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border':'0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height':data.rows.length*32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display':'none',
                    });                     
                    var tr = obj.find('div.datagrid-body tr');                    
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding':'0',
                        });
                    });      
                },
            });
            $('#uploadFile').filebox({
                multiple: true,
                disabled:true,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传',
                buttonIcon: 'icon-yg-add',
                width: 58,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadFile').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    if(!CheckIsNullOrEmpty($("#fileType").combobox("getValue"))){
                        $.messager.alert('提示','请选择文件类型');
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    formData.append("fileType",$("#fileType").combobox("getValue"));
                    var files = $("input[name='uploadFile']").get(0).files;
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
                                formData.set('uploadFile', bl, fileName); 
                                //上传文件
                                UploadFile(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
                            //上传文件
                            UploadFile(formData);
                        }
                    }   
                }
            })
            $("#package").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: packageData,
            })
            $("#package").combobox("setValue", "22")//默认纸箱

            //关闭窗口
            $("#btnClose").click(function(){
                $.myWindow.close();
            })
            //提交订单
            $("#btnSubmit").click(function() {
                endEditing();
                //各种验证
                if(!ValidationOrder()){
                    return;
                }
            
                var data = new FormData();            
                //基本信息
                data.append('clientID', ClientID);
                data.append('enterCode', EnterCode);
                data.append('currency', $("#currency").combobox("getValue"));
                data.append('supplier', $("#supplier").combobox("getValue"));
                data.append('paySupplier', $("#paySupplier").combobox("getValues"));
                data.append('isPayForGoods', $('#isPayForGoods').checkbox('options').checked);
                data.append('beneficiary', $("#beneficiary").combogrid("getValues"));
                //产品信息
                var rows = $('#dg').datagrid('getRows');
                var products = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    products.push(rows[i]);
                }
                data.append('products', JSON.stringify(products));
                data.append('totalPrice', compute2('TotalPrice'));
                //国内交货方式
                data.append('sz_DeliveryType', sz_DeliveryType);

                data.append('s_pickUpTime', $("#s_pickUpTime").datetimebox("getValue"));
                data.append('s_pickUpAddress', $("#s_pickUpAddress").combobox("getText"));
                data.append('pickUpName', $("#pickUpName").textbox("getValue"));
                data.append('pickUpTel', $("#pickUpTel").textbox("getValue"));
                data.append('IDCardType', $("#IDCardType").combobox("getValue"));
                data.append('IDCardNumber', $("#IDCardNumber").textbox("getValue"));               
                
                data.append('companyName', $("#companyName").combobox("getText"));
                data.append('address', $("#address").textbox("getValue"));
                data.append('contacts', $("#contacts").textbox("getValue"));
                data.append('phone', $("#phone").textbox("getValue"));
                //其它交货信息
                data.append('package', $("#package").combobox("getValue"));
                data.append('TotalPackages', $("#TotalPackages").numberbox("getValue"));
                data.append('TotalWeight', $("#TotalWeight").numberbox("getValue"));
                data.append('TotalVolume', $("#TotalVolume").numberbox("getValue"));
                data.append('summary', $("#summary").textbox("getValue"));           
                
                data.append('isUnBox', $('#isUnBox').checkbox('options').checked);
                data.append('isDetection', $('#isDetection').checkbox('options').checked);
                data.append('isCustomLabel', $('#isCustomLabel').checkbox('options').checked);
                data.append('isRepackaging', $('#isRepackaging').checkbox('options').checked);
                data.append('isVacuumPackaging', $('#isVacuumPackaging').checkbox('options').checked);
                data.append('isWaterproofPackaging', $('#isWaterproofPackaging').checkbox('options').checked);
                data.append('isCharterBus', $('#isCharterBus').checkbox('options').checked);
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                
                //ajax请求
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
                        if(res.success){
                            top.$.timeouts.alert({position: "TC",msg: res.message,type: "success"});
                            $.myWindow.close();
                        }
                        else{
                            top.$.timeouts.alert({position: "TC",msg: res.message,type: "error"});
                        }
                    }
                })
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
        //绑定供应商收益人
        function BandingBeneficiary()
        {
            var SupplierID = $("#supplier").combobox('getValue');
            var Currency = $("#currency").combobox('getValue');
            if(CheckIsNullOrEmpty(SupplierID)&&CheckIsNullOrEmpty(Currency))
            {
                //两筛选参数都不为空执行
                $.post('?action=SelectSupplier', { SupplierID: SupplierID ,ClientID:ClientID,Currency:Currency}, function (result) {
                    var rel = JSON.parse(result);
                    if (rel.success) {
                        //绑定受益人数据
                        $('#beneficiary').combogrid({
                            data: eval(rel.data)
                        });
                    }
                    else {
                        $.messager.alert('提示', rel.data);
                    }
                })
            }
        }
        //订单验证
        function ValidationOrder()
        {
            //验证必填项
            var isValid = $('#form1').form('enableValidation').form('validate');
            if(!isValid){
                return false;
            }
            //验证订单项数量(注：合计行)
            var rows = $("#dg").datagrid("getRows");
            if(rows.length < 2){
                $.messager.alert('提示', '订单项数量不可为空');
                return false;
            }
            if(rows.length > 101){
                $.messager.alert('提示', '订单项数量不能超过100项');
                return false;
            }
            //验证订单项必填项
            for (var i = 0; i < rows.length-1; i++) {
                var row = rows[i];
                if(!(CheckIsNullOrEmpty(row.PartNumber)&&CheckIsNullOrEmpty(row.Manufacturer)&&
                    CheckIsNullOrEmpty(row.Quantity)&&CheckIsNullOrEmpty(row.TotalPrice)))
                {
                    $.messager.alert('提示', '序号为'+(i+1)+'的订单项中必填项存在空值');
                    return false
                }
            }
            return true;
        }
        //深圳发货方式
        function SZ_Delivery()
        {
            $("#send_Pick").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".s_pickUp").css("display", "table-row");
                        $(".s_send").css("display", "none");
                        sz_DeliveryType = <%= (int)Yahv.Underly.WaybillType.PickUp%>;
                        PickInfoRequired();
                    }
                    else
                    {
                        SendInfoRequired();
                    }
                }
            });
            $("#send_Send").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".s_pickUp").css("display", "none");
                        $(".s_send").css("display", "table-row");
                        sz_DeliveryType = <%= (int)Yahv.Underly.WaybillType.DeliveryToWarehouse%>;
                        SendInfoRequired();
                    }
                }
            });
            $("#send_LocalExpress").radiobutton({
                onChange: function (record) {
                    if (record) {
                        $(".s_pickUp").css("display", "none");
                        $(".s_send").css("display", "table-row");
                        sz_DeliveryType = <%= (int)Yahv.Underly.WaybillType.LocalExpress%>;
                        SendInfoRequired();
                    }
                }
            });
        }
        function PickInfoRequired()
        {
            $("#s_pickUpTime").textbox({ required: true });
            $("#s_pickUpAddress").combobox({ required: true });
            $("#pickUpName").textbox({required:true});
            $("#pickUpTel").textbox({required:true});
            $("#IDCardType").combobox({required:true});
            $("#IDCardNumber").textbox({required:true});
                        
            $("#companyName").combobox({required:false});
            $("#address").textbox({required:false});
            $("#contacts").textbox({required:false});
            $("#phone").textbox({required:false});
        }
        function SendInfoRequired()
        {
            $("#s_pickUpTime").textbox({ required: false });
            $("#s_pickUpAddress").combobox({ required: false });
            $("#pickUpName").textbox({required:false});
            $("#pickUpTel").textbox({required:false});
            $("#IDCardType").combobox({required:false});
            $("#IDCardNumber").textbox({required:false});

            $("#companyName").combobox({required:true});
            $("#address").textbox({required:true});
            $("#contacts").textbox({required:true});
            $("#phone").textbox({required:true});
        }
    </script>
    <script>   
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName+'----'+row.FileTypeDec + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteFile(' + index + ')">删除</a>';
        }
        //上传文件
        function UploadFile(formData)
        {
            $.ajax({
                url: '?action=UploadFile',
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
                            $('#file').datagrid('insertRow', {
                                row: {
                                    ID:data[i].ID,
                                    CustomName: data[i].CustomName,
                                    FileName: data[i].FileName,
                                    FileType: data[i].FileType,
                                    FileTypeDec: data[i].FileTypeDec,
                                    Url: data[i].Url
                                }
                            });
                        }
                        var data = $('#file').datagrid('getData');
                        $('#file').datagrid('loadData', data);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            })
        }    
        //删除文件
        function DeleteFile(index) {
            $('#file').datagrid('deleteRow', index);
            var data = $('#file').datagrid('getData');
            $('#file').datagrid('loadData', data);
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
                ConfimQuantity(editIndex);              
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
                CustomName: '<span class="subtotal">--</span>',
                PartNumber: '<span class="subtotal">--</span>',
                Manufacturer: '<span class="subtotal">--</span>',
                Origin: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">' + compute('Quantity') + '</span>',
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
        //获取合计值
        function compute2(colName) {
            var rows = $('#dg').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length-1; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //验证申报数量
        function ConfimQuantity(editIndex)
        {
            var rows = $('#dg').datagrid('getRows');
            var row = rows[editIndex];
            if(Number(row["TotalQuantity"])<Number(row["Quantity"])){
                $.messager.alert('提示', '申报数量不能大于库存数量');
                $('#dg').datagrid('rejectChanges');
            }
        }
        //标记表格Title
        function markTitle()
        {
            var fields =$("#dg").datagrid('getColumnFields');
            var requiredHeaders=[];
            for (i = 0; i < fields .length-3; i++) { 
                var opt=$("#dg").datagrid('getColumnOption',fields[i]);
                var editor=opt.editor;
                if(editor == undefined){
                    continue;
                }
                var options=editor.options;
                if(options!=null&&options.required){
                    requiredHeaders.push(opt.title);
                } 
            }
            $(".datagrid-header-row td div span").each(function(i,th){
                var val=$(th).text();
                if(requiredHeaders.indexOf(val) != -1){
                    $(th).html(val+'<span style="color:red;font-weight:bold;vertical-align:middle;">' + " *" + '</span>');
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
        <div data-options="region:'center'" style="border:none">
            <table class="liebiao">
                <tr>
                    <td class="lbl">订单产品币种</td>
                    <td>
                        <input id="currency" name="currency" class="easyui-combobox" style="width: 220px" />
                    </td>
                    <td class="lbl">发货地</td>
                    <td>
                        <input id="sourceDistrict" name="sourceDistrict" class="easyui-combobox" style="width: 220px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">供应商名称</td>
                    <td>
                        <input id="supplier" class="easyui-combobox" style="width: 220px" />
                    </td>
                    <td class="lbl">供应商账户</td>
                    <td>
                        <input id="beneficiary" class="easyui-combogrid" style="width: 220px; height: 22px"
                            data-options="prompt:'供应商受益人'" />
                    </td>
                </tr>
            </table>
            <table id="dg" title="产品信息">
            </table>
            <table class="liebiao">
                <tr>
                    <td colspan="8" class="title">国内交货方式</td>
                </tr>
                <tr>
                    <td class="lbl">交货类型</td>
                    <td colspan="3">
                        <input class="easyui-radiobutton" id="send_Pick" name="SZ_DeliveryType" data-options="labelPosition:'after',checked: true,label:'客户自提'">
                        <input class="easyui-radiobutton" id="send_Send" name="SZ_DeliveryType" data-options="labelPosition:'after',label:'送货上门'">
                        <input class="easyui-radiobutton" id="send_LocalExpress" name="SZ_DeliveryType" data-options="labelPosition:'after',label:'国内快递'">
                    </td>
                </tr>
                 <tr class="s_pickUp">
                    <td class="lbl">提货时间</td>
                    <td>
                        <input id="s_pickUpTime" class="easyui-datetimebox" style="width: 220px; height: 20px; font-size: larger"
                            data-options="required:true,editable:false" />
                    </td>
                    <td class="lbl">提货地址</td>
                    <td>
                        <select id="s_pickUpAddress" class="easyui-combobox" style="width: 220px;" data-options="required:true,editable:false">
                            <option>英达丰(深圳市龙岗区吉华路393号英达丰科技园A栋101)</option>
                            <option>华强北(深圳市福田区中航路7-1号鼎诚大厦一楼)</option>
                        </select>
                    </td>
                </tr>
                <tr class="s_pickUp">
                    <td class="lbl">联系人</td>
                    <td>
                        <input id="pickUpName" class="easyui-textbox" style="width: 220px;"
                            data-options="required:true," />
                    </td>
                    <td class="lbl">联系电话</td>
                    <td>
                        <input id="pickUpTel" class="easyui-textbox" style="width: 220px"
                            data-options="required:true," />
                    </td>
                </tr>
                <tr class="s_pickUp">
                    <td class="lbl">证件类型</td>
                    <td>
                        <input id="IDCardType" class="easyui-combobox" style="width: 220px"
                            data-options="required:true," />
                    </td>
                    <td class="lbl">证件号码</td>
                    <td>
                        <input id="IDCardNumber" class="easyui-textbox" style="width: 220px"
                            data-options="required:true," />
                    </td>
                </tr>
                <tr class="s_send" style="display: none">
                    <td class="lbl">收货人</td>
                    <td>
                        <input id="companyName" class="easyui-combobox" style="width: 220px;"
                            data-options="prompt:'公司名称'" />
                    </td>
                    <td class="lbl">收货地址</td>
                    <td>
                        <input id="address" class="easyui-textbox" style="width: 220px" />
                    </td>
                </tr>
                <tr class="s_send" style="display: none">
                    <td class="lbl">联系人</td>
                    <td>
                        <input id="contacts" class="easyui-textbox" style="width: 220px" />
                    </td>
                    <td class="lbl">联系电话</td>
                    <td>
                        <input id="phone" class="easyui-textbox" style="width: 220px" />
                    </td>
                </tr>
            </table>
            <table class="liebiao">
                <tr>
                    <td colspan="4" class="title">其它信息</td>
                </tr>
                <tr>
                    <td class="lbl">上传文件</td>
                    <td colspan="3">
                        <div>
                            <input id="fileType" class="easyui-combobox" data-options="prompt:'文件类型'," style="width: 220px;" />
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
                        </div>
                        <div class="file" style="width: 1000px">
                            <table id="file">
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">包装类型</td>
                    <td>
                        <input id="package" class="easyui-combobox" style="width: 220px" />
                    </td>
                    <td class="lbl">总件数</td>
                    <td>
                        <input id="TotalPackages" class="easyui-numberbox" style="width: 220px"
                            data-options="min:1" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">总毛重(kg)</td>
                    <td>
                        <input id="TotalWeight" class="easyui-numberbox" style="width: 220px"
                            data-options="min:0,precision:4" />
                    </td>
                    <td class="lbl">总体积(m³)</td>
                    <td>
                        <input id="TotalVolume" class="easyui-numberbox" style="width: 220px"
                            data-options="min:0,precision:4" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">货款服务</td>
                    <td>
                        <div style="width: 220px">
                            <input id="isPayForGoods" name="isPayForGoods" class="easyui-checkbox" value="true"
                                data-options="label:'是否代付货款',labelPosition:'after'">
                        </div>
                    </td>
                    <td class="lbl">付汇供应商</td>
                    <td>
                        <input id="paySupplier" class="easyui-combobox" style="width: 220px;height:36px"
                            data-options="prompt:'客户付汇供应商,最多选择3个'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">其它服务</td>
                    <td colspan="3">
                        <input id="isUnBox" name="isUnBox" class="easyui-checkbox" value="true"
                            data-options="label:'是否拆箱验货',labelPosition:'after'">&nbsp&nbsp
                        <input id="isDetection" name="isDetection" class="easyui-checkbox" value="true"
                            data-options="label:'是否检测产品',labelPosition:'after'">&nbsp&nbsp
                        <input id="isCustomLabel" name="isCustomLabel" class="easyui-checkbox" value="true"
                            data-options="label:'是否重贴标签',labelPosition:'after'">&nbsp&nbsp
                        <input id="isRepackaging" name="isRepackaging" class="easyui-checkbox" value="true"
                            data-options="label:'是否重新包装',labelPosition:'after'">&nbsp&nbsp
                        <input id="isVacuumPackaging" name="isVacuumPackaging" class="easyui-checkbox" value="true"
                            data-options="label:'是否真空包装',labelPosition:'after'">&nbsp&nbsp
                        <input id="isWaterproofPackaging" name="isWaterproofPackaging" class="easyui-checkbox" value="true"
                            data-options="label:'是否防水包装',labelPosition:'after'">&nbsp&nbsp
                        <input id="isCharterBus" name="isCharterBus" class="easyui-checkbox" value="true"
                            data-options="label:'是否包车',labelPosition:'after'">
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注信息</td>
                    <td colspan="3">
                        <input id="summary" class="easyui-textbox" style="width: 220px; height: 50px"
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
