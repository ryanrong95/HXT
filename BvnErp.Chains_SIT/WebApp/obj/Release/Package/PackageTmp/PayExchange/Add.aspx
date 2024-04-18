<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.PayExchange.Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增付汇申请</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>
    <script>
        var PaymentTypeData = eval('(<%=this.Model.PaymentTypeData%>)');
        var SupplierData = eval('(<%=this.Model.SupplierData%>)');

        var IsadvanceMoney = '<%=this.Model.IsadvanceMoney%>';//是否垫资 by 2020-12-28 yess

        var bankSensitiveFlag = 1; //银行敏感性标志  1-未检测, 2-已检测,是敏感, 3-已检测,不是敏感

        var AvailableProductFee = '<%=this.Model.AvailableProductFee%>';
        //页面加载时
        $(function () {
            if (IsadvanceMoney == "0") {
                document.getElementById('AdvanceMoney').checked = true;
            }
            else {
                document.getElementById('IsAdvanceMoney').checked = true;
            }
            document.getElementById('AdvanceMoneyProduct').innerHTML = AvailableProductFee;
            $('#datagrid').myDatagrid({
                actionName: 'data',
                onLoadSuccess: function (data) {
                    var total1 = 0;
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        total1 = total1 + Number(row.RmbAmount)
                    }
                    $("#ReceivableAmountTotal").text(total1.toFixed(4));
                    var heightValue = $("#datagrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 80;
                    $("#datagrid").prev().find(".datagrid-body").height(heightValue);
                    $("#datagrid").prev().height(heightValue);
                    $("#datagrid").prev().parent().height(heightValue);
                    $("#datagrid").prev().parent().parent().height(heightValue);
                }
            });
            //选择是否垫款时，验证可用垫资额度  by 2021-01-05 yess
            $('input[type=radio][name=AdvanceMoney]').change(function () {
                var rmbPrice = Number($("#ReceivableAmountTotal").html());
                if ($('input:radio[name="AdvanceMoney"]:checked').val() == "0") {
                    if (rmbPrice > Number(AvailableProductFee)) {//反向验证
                        //垫款
                        document.getElementById('IsAdvanceMoney').checked = true;
                        $.messager.alert('提示', '垫款额度不够，只能选择不垫款！');
                        return;
                    }
                }

            });
            //上传合同发票
            $('#uploadFile').filebox({
                multiple: true,
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $('#uploadFile').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
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
                                formData.set('uploadFile', bl, fileName); // 文件对象
                                $.ajax({
                                    url: '?action=UploadFiles',
                                    type: 'POST',
                                    data: formData,
                                    dataType: 'JSON',
                                    cache: false,
                                    processData: false,
                                    contentType: false,
                                    success: function (res) {
                                        if (res.success) {
                                            var row = $('#pi').datagrid('getRows');
                                            var data = res.data;
                                            for (var i = 0; i < data.length; i++) {
                                                $('#pi').datagrid('insertRow', {
                                                    index: row.length + i,
                                                    row: {
                                                        FileName: data[i].FileName,
                                                        FileFormat: data[i].FileFormat,
                                                        WebUrl: data[i].WebUrl,
                                                        Url: data[i].Url,
                                                    }
                                                });
                                            }
                                            var data = $('#pi').datagrid('getData');
                                            $('#pi').datagrid('loadData', data);
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
                            formData.set('uploadFile', file);
                            $.ajax({
                                url: '?action=UploadFiles',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    if (res.success) {
                                        var row = $('#pi').datagrid('getRows');
                                        var data = res.data;
                                        for (var i = 0; i < data.length; i++) {
                                            $('#pi').datagrid('insertRow', {
                                                index: row.length + i,
                                                row: {
                                                    FileName: data[i].FileName,
                                                    FileFormat: data[i].FileFormat,
                                                    WebUrl: data[i].WebUrl,
                                                    Url: data[i].Url,
                                                }
                                            });
                                            var data = $('#pi').datagrid('getData');
                                            $('#pi').datagrid('loadData', data);
                                        }
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

            //原始PI列表初始化
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'filedata',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileFormat', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'WebUrl', title: '', width: 70, align: 'center', hidden: true },
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

            //初始化付款方式下拉框
            $('#PaymentType').combobox({
                data: PaymentTypeData,
            });
            //初始化供应商下拉框
            $('#SupplierName').combobox({
                data: SupplierData,
                editable: false,
                onChange: function (newValue, oldValue) {
                    //ABA、IBAN 是否必填
                    var thePlaceType = '';
                    for (var i = 0; i < SupplierData.length; i++) {
                        if (SupplierData[i].ID == newValue) {
                            thePlaceType = SupplierData[i].PlaceType;
                            break;
                        }
                    }

                    if (thePlaceType != null) {
                        if (thePlaceType == '<%=Needs.Ccs.Services.Enums.RegionalType.ABA.GetHashCode()%>') {
                            $('#ABA').textbox({ required: true });
                        } else if (thePlaceType == '<%=Needs.Ccs.Services.Enums.RegionalType.IBAN.GetHashCode()%>') {
                            $('#IBAN').textbox({ required: true });
                        }
                    }


                    //先记下原先输入框中的值
                    var oldInputCurrentPaidAmounts = [];
                    var rows = $("#datagrid").datagrid('getRows');
                    for (var i = 0; i < rows.length; i++) {
                        oldInputCurrentPaidAmounts.push({
                            IsMatchSupplier: rows[i].IsMatchSupplier,
                            CurrentPaidAmount: rows[i].CurrentPaidAmount,
                        });
                    }

                    var ClientID = $(this).combobox('getValue');
                    var OrderIDs = '';

                    rows = $("#datagrid").datagrid('getRows');
                    for (var i = 0; i < rows.length; i++) {
                        OrderIDs += rows[i].ID;
                        if (i != rows.length - 1) {
                            OrderIDs += ',';
                        }
                    }

                    //访问后台
                    $.post('?action=SelectSupplier', { ClientID: ClientID, OrderIDs: OrderIDs, }, function (result) {
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            var data = rel.data;
                            $('#SupplierEnglishName').textbox('setValue', data.EnglishName);
                            //更新银行地址数据
                            var bankdata = eval(data.Banks);
                            var addressdata = eval(data.Address);
                            $('#BankName').combobox({
                                data: eval(bankdata)
                            });
                            $('#SupplierAddress').combobox({
                                data: eval(addressdata)
                            });
                            $('#BankAddress').textbox('setValue', "");
                            $('#BankAccount').textbox('setValue', "");
                            $('#SwiftCode').textbox('setValue', "");

                            //根据获得的每个订单的本次申请金额，对列表中的信息赋值
                            var rows = $("#datagrid").datagrid('getRows');
                            for (var i = 0; i < rows.length; i++) {
                                var thisOrderCurrentPayAmount = data.currentPayAmounts.filter(t => t.OrderID == rows[i].ID)[0];

                                var isMatchChangedToNotMatch = false;  //是否是由匹配变成不匹配
                                if (oldInputCurrentPaidAmounts[i].IsMatchSupplier == true && thisOrderCurrentPayAmount.IsMatchSupplier == false) {
                                    isMatchChangedToNotMatch = true;
                                }

                                rows[i].IsMatchSupplier = thisOrderCurrentPayAmount.IsMatchSupplier;
                                if (rows[i].IsMatchSupplier) {
                                    if (thisOrderCurrentPayAmount.CurrentPaidAmount <= rows[i].PaidAmount) {
                                        rows[i].CurrentPaidAmount = thisOrderCurrentPayAmount.CurrentPaidAmount;
                                    } else {
                                        rows[i].CurrentPaidAmount = rows[i].PaidAmount;
                                    }

                                    rows[i].MatchSupplierAmount = rows[i].CurrentPaidAmount; //如果匹配供应商，匹配的金额
                                } else {
                                    if (isMatchChangedToNotMatch) {
                                        //如果是由匹配变成不匹配, 则输入框中默认输入值为可申请金额。
                                        //否则不匹配变成不匹配，输入框中的值不变，使用一开始保存的 oldInputCurrentPaidAmounts
                                        rows[i].CurrentPaidAmount = rows[i].PaidAmount;
                                    } else {
                                        rows[i].CurrentPaidAmount = oldInputCurrentPaidAmounts[i].CurrentPaidAmount;
                                    }
                                }
                            }

                            $('#datagrid').datagrid('loadData', rows);




                            //rows = $("#datagrid").datagrid('getRows');
                            //for (var i = 0; i < rows.length; i++) {
                            //    $('#datagrid').datagrid('beginEdit', i);
                            //    var ed = $('#datagrid').datagrid('getEditor', { index: i, field: 'CurrentPaidAmount', });
                            //    if (rows[i].IsMatchSupplier) {
                            //        //$(ed.target).prop('readonly', true);
                            //        $(ed.target).prop('editable', false);
                            //    } else {
                            //        //$(ed.target).prop('readonly', false);
                            //        $(ed.target).prop('editable', true);
                            //    }
                            //    $('#datagrid').datagrid('endEdit', i);
                            //}


                        }
                        else {
                            $.messager.alert('提示', rel.data);
                        }
                    })
                }
            });
            //选择银行地址
            $('#BankName').combobox({
                onChange: function () {
                    bankSensitiveFlag = 1; //置为未检测银行是否敏感国家
                    $("#SensitiveTip").hide();

                    var BankID = $(this).combobox('getValue');
                    //访问后台
                    MaskUtil.mask();
                    $.post('?action=SelectBank', { BankID: BankID }, function (result) {
                        MaskUtil.unmask();
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            var data = rel.data;
                            $('#BankAddress').textbox('setValue', data.BankAddress);
                            $('#BankAccount').textbox('setValue', data.BankAccount);
                            $('#SwiftCode').textbox('setValue', data.SwiftCode);

                            if (data.IsSensitive) {
                                bankSensitiveFlag = 2; //置为已检测,是敏感
                                $("#SensitiveTip").show();
                            } else {
                                bankSensitiveFlag = 3; //置为已检测,不是敏感
                                $("#SensitiveTip").hide();
                            }
                        }
                        else {
                            $.messager.alert('提示', rel.data);
                        }
                    })
                }
            })
        });

        //删除合同发票， 
        // var isRemovePIRow = false;
        function Delete(index) {
            // isRemovePIRow = true;
            $("#pi").datagrid('deleteRow', index);
            //解决删除行后，行号错误问题
            var data = $('#pi').datagrid('getData');
            $('#pi').datagrid('loadData', data);

        }

        //function onClickPIRow(index) {
        //    if (isRemovePIRow) {
        //        $('#pi').datagrid('deleteRow', index);
        //        isRemovePIRow = false;
        //    }
        //}

        function DeleteDataGrid(Index) {
            $('#datagrid').datagrid('deleteRow', Index);
            //解决删除行后，行号错误问题
            var rows = $("#datagrid").datagrid('getRows');
            $("#datagrid").datagrid('loadData', rows);
        }

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

        //显示附件图片
        function ShowInfo(val, row, index) {
            return '<img src="../App_Themes/xp/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.WebUrl + '\')">' + row.FileName + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
        }
        //提交数据
        function Submit() {
            endEditing();
            var isValid = $("#form2").form("enableValidation").form("validate");
            var OrderData = $("#datagrid").datagrid("getRows");
            $('#uploadFile').filebox('setValue', '');
            var FileData = $("#pi").datagrid("getRows");

            if (!isValid) {
                return;
            }
            else if (OrderData.length == 0) {
                $.messager.alert('提示', '申请付汇的订单为空');
                return;
            }
            //else if (FileData.length == 0) {
            //    $.messager.alert('提示', '请上传合同发票文件');
            //    return;
            //}

            //银行敏感性标志  1-未检测, 2-已检测,是敏感, 3-已检测,不是敏感
            if (bankSensitiveFlag == 1) {
                $.messager.alert('提示', '此银行的敏感地区检测未完成');
                return;
            }
            if (bankSensitiveFlag == 2) {
                $.messager.alert('提示', '此银行涉及敏感地区，无法申请付汇');
                return;
            }

            var values = FormValues("form2");
            //验证成功
            values['SupplierAddress'] = $("#SupplierAddress").combobox('getText');
            values['SupplierEnglishName'] = $("#SupplierEnglishName").textbox('getValue').replace("'", "&#39");
            values['BankAddress'] = ($("#BankAddress").textbox('getValue')).replace("'", "&#39");
            if (($("#BankAddress").textbox('getValue')).replace("'", "&#39") == "") {
                $.messager.alert('提示', '银行地址为空，无法申请付汇，请先维护好银行地址！');
                return;
            }
            values['ABA'] = ($("#ABA").textbox('getValue')).replace("'", "&#39");
            values['IBAN'] = ($("#IBAN").textbox('getValue')).replace("'", "&#39");
            values['IsAdvanceMoney'] = $('input:radio[name="AdvanceMoney"]:checked').val();

            //values['SupplierID'] = $("#SupplierName").combobox('getValues')[0];  //获取选中的供应商的ID
            MaskUtil.mask();//遮挡层
            $.post('?action=Submit', {
                OrderData: encodeURI(JSON.stringify(OrderData)),
                FileData: encodeURI(JSON.stringify(FileData)),
                Model: encodeURI(JSON.stringify(values))
            }, function (result) {
                var rel = JSON.parse(result);
                MaskUtil.unmask();//关闭遮挡层
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //返回列表页
                        Back();
                    }
                });
            });

        }

        //返回
        function Back() {
            var url = location.pathname.replace(/Add.aspx/ig, '../Order/UnPayExchange/List.aspx');
            window.location = url;
        }
        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../App_Themes/xp/images/wenjian.png" />';
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="DeleteDataGrid(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">移除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#datagrid').datagrid('validateRow', editIndex)) {
                $('#datagrid').datagrid('endEdit', editIndex);
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
                    $('#datagrid').datagrid('selectRow', index);
                    $('#datagrid').datagrid('beginEdit', index);
                    $('#datagrid').datagrid('uncheckRow', index);
                    editIndex = index;
                } else {
                    $('#datagrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                VerifyQuantity(index);
                editIndex = undefined;
            }
        }
        function VerifyQuantity(Index) {
            if (Index != null) {
                $('#datagrid').datagrid('acceptChanges');
                $('#datagrid').datagrid('selectRow', Index);
                var row = $('#datagrid').datagrid('getSelected');

                if (row["IsMatchSupplier"]) {
                    $.messager.alert("消息", "订单 " + row.ID + " 中，已有型号与所选供应商匹配, 本次申请金额应为：" + row["MatchSupplierAmount"]);
                    row["CurrentPaidAmount"] = row["MatchSupplierAmount"];
                    var index = $('#datagrid').datagrid('getRowIndex', row);
                    $('#datagrid').datagrid('refreshRow', index);
                } else {
                    if (Number(row["PaidAmount"]) < Number(row["CurrentPaidAmount"])) {
                        $.messager.alert("消息", "申请金额不能大于可申请金额");
                        row["CurrentPaidAmount"] = row["PaidAmount"];
                        var index = $('#datagrid').datagrid('getRowIndex', row);
                        $('#datagrid').datagrid('refreshRow', index);
                    }
                    if (Number(row["CurrentPaidAmount"]) < 0) {
                        $.messager.alert("消息", "申请金额不能小于零");
                        row["CurrentPaidAmount"] = 0;
                        var index = $('#datagrid').datagrid('getRowIndex', row);
                        $('#datagrid').datagrid('refreshRow', index);
                    }
                }

                $('#datagrid').datagrid('acceptChanges');
            }
            else {
                //装箱数量不大于订单数量
                var rows = $('#datagrid').datagrid('getChanges');
                for (var i = 0; i < rows.length; i++) {
                    var row = rows[i];

                    if (row["IsMatchSupplier"]) {
                        $.messager.alert("消息", "订单 " + row.ID + " 中，已有型号与所选供应商匹配, 本次申请金额应为：" + row["MatchSupplierAmount"]);
                        row["CurrentPaidAmount"] = row["MatchSupplierAmount"];
                        var index = $('#datagrid').datagrid('getRowIndex', row);
                        $('#datagrid').datagrid('refreshRow', index);
                    } else {
                        if (Number(row["PaidAmount"]) < Number(row["CurrentPaidAmount"])) {
                            $.messager.alert("消息", "申请金额不能大于可申请金额");
                            row["CurrentPaidAmount"] = row["PaidAmount"];
                            var index = $('#datagrid').datagrid('getRowIndex', row);
                            $('#datagrid').datagrid('refreshRow', index);
                        }
                        if (Number(row["CurrentPaidAmount"]) < 0) {
                            $.messager.alert("消息", "申请金额不能小于零");
                            row["CurrentPaidAmount"] = 0;
                            var index = $('#datagrid').datagrid('getRowIndex', row);
                            $('#datagrid').datagrid('refreshRow', index);
                        }
                    }
                }

                $('#datagrid').datagrid('acceptChanges');
            }
        }

    </script>
    <style>
        #pi-fujian-table td {
            border-width: 0;
            padding: 0;
        }

            #pi-fujian-table td div {
                padding: 0;
            }
    </style>
</head>
<body class="easyui-layout">
    <div id="tt" class="easyui-tabs" style="width: auto;height:100%;" data-options="border: false,">
        <div title="付汇申请" style="display: none; padding: 5px;">
            <div data-options="region:'north',border:false," style="height: 41px; overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Submit()">提交</a>
                    <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                </div>
            </div>
            <div data-options="region:'north',border:false," style="height: 41px; overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <input type="radio" name="AdvanceMoney" value="0" id="AdvanceMoney" title="垫款" class="checkbox checkboxlist" /><label for="AdvanceMoney" style="margin-right: 20px">垫款</label>
                    <input type="radio" name="AdvanceMoney" value="1" id="IsAdvanceMoney" title="不垫款" class="checkbox checkboxlist" /><label for="IsAdvanceMoney">不垫款</label>
                    &nbsp; &nbsp; &nbsp; &nbsp;<span>可用垫款额度：<label id="AdvanceMoneyProduct" style="color: red;"></label></span>
                </div>
            </div>
            <div data-options="region:'west',border: false," style="width: 30%; float: left; min-width: 350px;">
                <div class="sec-container">
                    <form id="form2">
                        <div class="easyui-panel" title="付汇供应商">
                            <div class="sub-container">
                                <table style="width: 100%;" cellspacing="2" cellpadding="0">
                                    <tr>
                                        <td class="lbl">供应商名称：</td>
                                        <td>
                                            <input class="easyui-combobox" id="SupplierName" name="SupplierName"
                                                data-options="limitToList:true,required:true,valueField:'ID',textField:'ChineseName',height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">供应商地址：</td>
                                        <td>
                                            <input class="easyui-combobox" id="SupplierAddress" name="SupplierAddress"
                                                data-options="limitToList:true,valueField:'ID',textField:'Name',height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">英文名称：</td>
                                        <td>
                                            <input class="easyui-textbox" id="SupplierEnglishName" name="SupplierEnglishName"
                                                data-options="required:true,tipPosition:'bottom',height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行名称：</td>
                                        <td>
                                            <input class="easyui-combobox" id="BankName" name="BankName"
                                                data-options="limitToList:true,required:true,valueField:'ID',textField:'Name',height:26,width:250" />
                                            <br />
                                            <label id="SensitiveTip" style="color: red; display: none;">此银行涉及敏感地区，无法申请付汇</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行地址：</td>
                                        <td>
                                            <input class="easyui-textbox" id="BankAddress" name="BankAddress"
                                                data-options="limitToList:true,required:true,height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行账号：</td>
                                        <td>
                                            <input class="easyui-textbox" id="BankAccount" name="BankAccount"
                                                data-options="limitToList:true,required:true,height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">银行代码：</td>
                                        <td>
                                            <input class="easyui-textbox" id="SwiftCode" name="SwiftCode"
                                                data-options="limitToList:true,required:true,height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">ABA(付美国必填)：</td>
                                        <td>
                                            <input class="easyui-textbox" id="ABA" name="ABA"
                                                data-options="height:26,width:250,validType:'length[0,50]'," />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">IBAN(付欧盟必填)：</td>
                                        <td>
                                            <input class="easyui-textbox" id="IBAN" name="IBAN"
                                                data-options="height:26,width:250,validType:'length[0,50]'," />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">付款方式：</td>
                                        <td>
                                            <input class="easyui-combobox" id="PaymentType" name="PaymentType"
                                                data-options="limitToList:true,required:true,valueField:'Key',textField:'Value',height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">期望付汇日期：</td>
                                        <td>
                                            <input class="easyui-datebox" id="ExpectPayDate" name="ExpectPayDate"
                                                data-options="height:26,width:250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">其他相关资料：</td>
                                        <td>
                                            <input class="easyui-textbox" id="OtherInfo" name="OtherInfo"
                                                data-options="required:false,multiline:true,height:26,width:250,validType:'length[1,500]'" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="lbl">备注：</td>
                                        <td>
                                            <input class="easyui-textbox" id="Summary" name="Summary"
                                                data-options="required:false,multiline:true,height:50,width:250,validType:'length[1,250]'" />
                                            <label class="lbl" id="ReceivableAmountTotal" style="display: none">0</label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 68%; float: left; margin-left: 2px;">
                <div class="sec-container">
                    <div style="width: 100%;">
                        <table id="datagrid" class="mygrid" title="付汇订单" style="height: 50%" data-options="
                            fitColumns:true,
                            fit:false,
                            border:true,
                            scrollbarSize:0,
                            pagination:false,
                            onClickRow:onClickRow">
                            <thead>
                                <tr>
                                    <th data-options="field:'ID',width: 200,align:'center'">订单编号</th>
                                    <th data-options="field:'Currency',width: 130,align:'center'">币种</th>
                                    <th data-options="field:'DeclarePrice',width: 130,align:'center'">报关总价</th>
                                    <th data-options="field:'PaidExchangeAmount',width: 150,align:'center'">已付汇金额</th>
                                    <th data-options="field:'PaidAmount',width: 150,align:'center'">可申请付汇金额</th>
                                    <th data-options="field:'CurrentPaidAmount',width: 150,align:'center',editor:{type:'textbox'}">本次申请金额</th>
                                    <th data-options="field:'btn',width:150,formatter:Operation,align:'center'">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div style="margin-top: 5px; width: 100%;">
                        <table id="table1" style="width: 100%; padding-right: 0;">
                            <tr>
                                <td style="vertical-align: top; width: 100%">
                                    <div id="fileContainer" title="合同发票(INVOICE LIST)" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'300px',">
                                        <div class="sub-container">
                                            <form id="form1">
                                                <div style="margin-top: 5px; margin-left: 5px;">
                                                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />

                                                </div>
                                                <div class="text-container" style="margin-top: 10px;">
                                                    <label>仅限图片、pdf格式的文件，且pdf文件不超过3M。</label>
                                                </div>
                                                <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
                                                    <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                                                    <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                                                </div>
                                                <div id="pi-fujian-table" class="ProductTable pi">
                                                    <table id="pi" data-options="pageSize:50,fitColumns:true,fit:false,pagination:false,queryParams:{ action: 'filedata' }">
                                                        <%--<thead>
                                                <tr>
                                                    <th data-options="field:'info',width: 100,align:'left',formatter:ShowInfo"">附件信息</th>
                                                </tr>
                                            </thead>--%>
                                                    </table>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
