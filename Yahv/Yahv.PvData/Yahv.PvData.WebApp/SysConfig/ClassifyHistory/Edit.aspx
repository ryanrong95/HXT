﻿<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ClassifyHistory.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="../../Styles/pvdata.css" rel="stylesheet" />
    <script src="../../Scripts/pvdata.js"></script>

    <script>
        //为Easy-ui的只读输入框设置背景色
        function SetReadonlyBgColor(className, color) {
            $('.' + className).each(function () {
                if ($(this).attr('readonly')) {
                    if (className == 'easyui-textbox') {
                        $(this).textbox('textbox').css('background', color);
                    } else if (className == 'easyui-numberbox') {
                        $(this).numberbox('textbox').css('background', color);
                    } else if (className == 'easyui-combogrid') {
                        $(this).combogrid('textbox').css('background', color);
                    }
                }
            });
        }
        //获取数据的接口
        var PvDataApiUrl = '';
        //记录当前的海关编码，用于比较输入框中的海关编码是否发生了变化
        var global_curHSCode = '';
        //系统判断是否是禁运产品
        var global_isSysEmbargo = false;
        //系统判断是否是Ccc产品
        var global_isSysCcc = false;
        //是否修改了品牌
        var global_isMfrChanged = false;
        //计时器
        var global_interval = null;
        //归类产品数据
        var orderedProduct;

        $(function () {
            //初始化产品型号下拉框
            $('#partNumber').combogrid({
                idField: 'ID',
                textField: 'PartNumber',
                nowrap: false,
                panelWidth: $("input[name=partNumber]").parent().width(),
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                mode: "local",
                validType: 'length[1,75]',
                readonly: true,
                columns: [[
                    { field: 'PartNumber', title: '型号', width: 100, align: 'left' },
                    { field: 'Name', title: '报关品名', width: 100, align: 'left' },
                    { field: 'HSCode', title: '海关编码', width: 100, align: 'left' },
                    { field: 'Elements', title: '申报要素', width: 200, align: 'left' },
                ]],
                onChange: function (newValue, oldValue) {
                    var grid = $('#partNumber').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    var partNumber = $("#partNumber").combogrid("getText");
                    if (row == null || row.partNumber != partNumber) {
                        getDataFun(PvDataApiUrl + 'Classify/GetClassifiedPartNumberLogs', { partNumber: partNumber }, {
                            success: function (res) {
                                var dataForamt = [];
                                if (res.data && res.data.length) {
                                    dataForamt = res.data;
                                    for (var i = 0; i < dataForamt.length; i++) {
                                        //if (dataForamt[i].ExciseTaxRate == 0) {
                                        //    dataForamt[i].ExciseTaxRate = '';
                                        //}
                                        if (dataForamt[i].ImportPreferentialTaxRate == 0) {
                                            dataForamt[i].ImportPreferentialTaxRate = '';
                                        }
                                        if (dataForamt[i].OriginATRate == 0) {
                                            dataForamt[i].OriginATRate = '';
                                        }
                                        if (dataForamt[i].VATRate == 0) {
                                            dataForamt[i].VATRate = '';
                                        }
                                    }
                                }
                                $("#partNumber").combogrid("grid").datagrid("loadData", dataForamt);
                                $("#importPreferentialTaxRate").combogrid("grid").datagrid("loadData", dataForamt);
                                $("#originATRate").combogrid("grid").datagrid("loadData", dataForamt);
                                $("#vatRate").combogrid("grid").datagrid("loadData", dataForamt);
                                //$("#ExciseTaxRate").combogrid("grid").datagrid("loadData", dataForamt);
                                $("#ciqPrice").combogrid("grid").datagrid("loadData", dataForamt);
                            }
                        })
                    }
                },
                onSelect: function () {
                    var grid = $('#partNumber').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    $('#tariffName').combogrid('setValue', row.Name);
                    setHSCode(row.HSCode);
                    $('#elements').textbox('setValue', row.Elements);
                    verifyManufacturer();
                }
            });
            $('#partNumber').combogrid('textbox').bind('focus', function () {
                $('#partNumber').combogrid('showPanel');
            });

            //初始化海关编码下拉框
            $('#hsCode').combogrid({
                idField: 'ID',
                textField: 'HSCode',
                nowrap: false,
                panelWidth: 400,
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                mode: "remote",
                missingMessage: '请输入海关编码',
                columns: [[
                    { field: 'HSCode', title: '海关编码', width: 100, align: 'left' },
                    { field: 'Name', title: '报关品名', width: 300, align: 'left' },
                ]],
                onChange: function (record) {
                    var grid = $("#hsCode").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    var hsCode = $("#hsCode").combogrid("getText");

                    if (row == null || row.HSCode != hsCode) {
                        getDataFun(PvDataApiUrl + 'Classify/GetMatchedHSCodes', { hsCode: record }, {
                            success: function (res) {
                                if (res.data) {
                                    $("#hsCode").combogrid("grid").datagrid("loadData", res.data);
                                } else {
                                    $("#hsCode").combogrid("grid").datagrid("loadData", []);
                                }
                            }
                        });
                    }
                },
                onSelect: function () {
                    var grid = $('#hsCode').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    elementsQuery();
                },
            });
            $('#hsCode').combogrid('textbox').bind('focus', function () {
                $('#hsCode').combogrid('showPanel');
            });

            //初始化报关品名下拉框
            $("#tariffName").combogrid({
                idField: "ID",
                textField: "Name",
                nowrap: false,
                panelWidth: $("input[name=tariffName]").parent().width(),
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                mode: "remote",
                validType: 'length[1,250]',
                missingMessage: '请输入报关品名',
                columns: [[
                    { field: 'Name', title: '报关品名', width: 150, align: 'left' },
                    { field: 'PartNumber', title: '型号', align: 'left', hidden: true },
                    { field: 'TaxCode', title: '税务编码', width: 150, align: 'left' },
                    { field: 'TaxName', title: '税务名称', width: 180, align: 'left' },
                ]],
                onChange: function () {
                    var grid = $("#tariffName").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    var name = $("#tariffName").combogrid("getText");

                    if (row == null || row.Name != name) {
                        getDataFun(PvDataApiUrl + 'Classify/GetClassifiedTaxLogs', { name: name }, {
                            success: function (res) {
                                if (res.data) {
                                    $("#tariffName").combogrid("grid").datagrid("loadData", res.data);
                                } else {
                                    $("#tariffName").combogrid("grid").datagrid("loadData", []);
                                }
                            }
                        })
                    }
                },
                onSelect: function () {
                    var grid = $("#tariffName").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    $('#taxCode').textbox('setValue', row.TaxCode);
                    $('#taxName').textbox('setValue', row.TaxName);
                }
            });
            $('#tariffName').combogrid('textbox').bind('focus', function () {
                $('#tariffName').combogrid('showPanel');
            });

            //优惠税率历史记录
            $("#importPreferentialTaxRate").combogrid({
                idField: "ID",
                textField: "ImportPreferentialTaxRate",
                panelWidth: 400,
                fitColumns: true,
                //required: true,
                hasDownArrow: false,
                missingMessage: '请输入优惠税率',
                mode: "local",
                columns: [[
                    { field: 'ImportPreferentialTaxRate', title: '优惠税率', width: 100, align: 'left' },
                    { field: 'PartNumber', title: '型号', align: 'left', hidden: true },
                    { field: 'CreateDate', title: '归类时间', width: 200, align: 'left' },
                    { field: 'Quantity', title: '数量', width: 100, align: 'left' },
                ]],
                onSelect: function () {
                    var value = $("#importPreferentialTaxRate").combogrid("getValue");
                    var text = $("#importPreferentialTaxRate").combogrid("getText");
                    if (value == text) {
                        $("#importPreferentialTaxRate").combogrid("setValue", '');
                    }
                }
            });
            $('#importPreferentialTaxRate').combogrid('textbox').bind('focus', function () {
                $('#importPreferentialTaxRate').combogrid('showPanel');
            });

            //增值税率历史记录
            $("#vatRate").combogrid({
                idField: "ID",
                textField: "VATRate",
                panelWidth: 400,
                fitColumns: true,
                //required: true,
                hasDownArrow: false,
                missingMessage: '请输入增值税率',
                mode: "local",
                columns: [[
                    { field: 'VATRate', title: '增值税率', width: 100, align: 'left' },
                    { field: 'PartNumber', title: '型号', align: 'left', hidden: true },
                    { field: 'CreateDate', title: '归类时间', width: 200, align: 'left' },
                    { field: 'Quantity', title: '数量', width: 100, align: 'left' },
                ]],
                onSelect: function () {
                    var value = $("#vatRate").combogrid("getValue");
                    var text = $("#vatRate").combogrid("getText");
                    if (value == text) {
                        $("#vatRate").combogrid("setValue", '');
                    }
                }
            });
            $('#vatRate').combogrid('textbox').bind('focus', function () {
                $('#vatRate').combogrid('showPanel');
            });

            //消费税率历史记录
            //$("#ExciseTaxRate").combogrid({
            //    idField: "ID",
            //    textField: "ExciseTaxRate",
            //    panelWidth: 400,
            //    fitColumns: true,
            //    //required: true,
            //    hasDownArrow: false,
            //    missingMessage: '请输入消费税率',
            //    mode: "local",
            //    columns: [[
            //        { field: 'ExciseTaxRate', title: '消费税率', width: 100, align: 'left' },
            //        { field: 'PartNumber', title: '型号', align: 'left', hidden: true },
            //        { field: 'CreateDate', title: '归类时间', width: 200, align: 'left' },
            //        { field: 'Quantity', title: '数量', width: 100, align: 'left' },
            //    ]],
            //    onSelect: function () {
            //        var value = $("#ExciseTaxRate").combogrid("getValue");
            //        var text = $("#ExciseTaxRate").combogrid("getText");
            //        if (value == text) {
            //            $("#ExciseTaxRate").combogrid("setValue", '');
            //        }
            //    }
            //});
            //$('#ExciseTaxRate').combogrid('textbox').bind('focus', function () {
            //    $('#ExciseTaxRate').combogrid('showPanel');
            //});

            //商检费历史记录
            $("#ciqPrice").combogrid({
                idField: "ID",
                textField: "CIQprice",
                panelWidth: 400,
                fitColumns: true,
                hasDownArrow: false,
                mode: "local",
                min: 0,
                precision: '4',
                prompt: '输入商检费...',
                columns: [[
                    { field: 'PartNumber', title: '型号', width: 200, align: 'left' },
                    { field: 'CIQ', title: '是否商检', width: 100, align: 'left' },
                    { field: 'CIQprice', title: '商检费用', width: 100, align: 'left' },
                ]],
                onSelect: function () {
                    var grid = $("#ciqPrice").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    if (row.CIQ == '否') {
                        $('#ciqPrice').combogrid('setValue', null);
                        $('#isCiq').prop('checked', false);
                    } else {
                        $('#isCiq').prop('checked', true);
                    }
                    var value = $("#ciqPrice").combogrid("getValue");
                    var text = $("#ciqPrice").combogrid("getText");
                    if (value == text) {
                        $("#ciqPrice").combogrid("setValue", '');
                    }

                }
            });
            $('#ciqPrice').combogrid('textbox').bind('focus', function () {
                $('#ciqPrice').combogrid('showPanel');
            });

            //是否商检valuechange事件
            $('#isCiq').change(function () {
                if (this.checked) {
                    $('#ciqPrice').combogrid('textbox').css('background', 'white');
                    $('#ciqPrice').combogrid('textbox').attr('readonly', false);
                } else {
                    $('#ciqPrice').combogrid('textbox').css('background', '#EBEBE4');
                    $('#ciqPrice').combogrid('textbox').attr('readonly', true);
                    //清除商检费的输入框
                    $("#ciqPrice").textbox("setValue", '');
                }
            });

            orderedProduct = top.window['topdata'];

            if (orderedProduct != null) {

                PvDataApiUrl = orderedProduct['PvDataApiUrl'];

                $('#manufacturer').textbox('setValue', orderedProduct['Manufacturer']);
                $('#partNumber').combogrid('setValue', orderedProduct['PartNumber']);
                $('#taxCode').textbox('setValue', orderedProduct['TaxCode']);
                $('#taxName').textbox('setValue', orderedProduct['TaxName']);
                $('#hsCode').combogrid('setValue', orderedProduct['HSCode']);
                setHSCode(orderedProduct['HSCode']);
                $('#tariffName').combogrid('setValue', orderedProduct['TariffName']);
                $('#importPreferentialTaxRate').combogrid('setValue', parseFloat(orderedProduct['ImportPreferentialTaxRate']) == 0 ? '' : orderedProduct['ImportPreferentialTaxRate']);
                $('#vatRate').combogrid('setValue', parseFloat(orderedProduct['VATRate']) == 0 ? '' : orderedProduct['VATRate']);
                //$('#ExciseTaxRate').combogrid('setValue', parseFloat(orderedProduct['ExciseTaxRate']) == 0 ? '' : orderedProduct['ExciseTaxRate']);
                $('#legalUnit1').textbox('setValue', orderedProduct['LegalUnit1']);
                $('#legalUnit2').textbox('setValue', orderedProduct['LegalUnit2'])
                $('#ciqCode').combobox('setValue', orderedProduct['CIQCode']);
                $('#elements').textbox('setValue', orderedProduct['Elements']);

                var isCcc = orderedProduct['Ccc'];
                var isEmbargo = orderedProduct['Embargo'];
                var isHkControl = orderedProduct['HkControl'];
                var isCoo = orderedProduct['Coo'];
                var isCiq = orderedProduct['CIQ'];

                if (isCcc) {
                    $('#isCcc').prop('checked', true);
                } else {
                    $('#isCcc').prop('checked', false);
                }
                if (isEmbargo) {
                    $('#isEmbargo').prop('checked', true);
                } else {
                    $('#isEmbargo').prop('checked', false);
                }
                if (isHkControl) {
                    $('#isHkControl').prop('checked', true);
                } else {
                    $('#isHkControl').prop('checked', false);
                }
                if (isCoo) {
                    $('#isCoo').prop('checked', true);
                } else {
                    $('#isCoo').prop('checked', false);
                }
                if (isCiq) {
                    $('#isCiq').prop('checked', true);
                    $('#ciqPrice').combogrid('setValue', orderedProduct['CIQprice']);
                } else {
                    $('#isCiq').prop('checked', false);
                    $('#ciqPrice').combogrid('textbox').css('background', '#EBEBE4');
                    $('#ciqPrice').combogrid('textbox').attr('readonly', true);
                }
                //验证客户填写品牌与申报要素品牌是否一致
                verifyManufacturer();

                //获取产品管控信息
                getDataFun(PvDataApiUrl + 'Classify/GetSysControls', { partNumber: orderedProduct['PartNumber'] }, {
                    success: function (res) {
                        if (res.data) {
                            if (res.data.IsSysEmbargo) {
                                setIsSysEmbargo(res.data.IsSysEmbargo);
                            }
                            if (res.data.IsSysCcc) {
                                setIsSysCcc(res.data.IsSysCcc);
                                if (orderedProduct && orderedProduct.Step == 1) {
                                    $('#isCcc').prop('checked', true);
                                }
                            }
                        } else {
                            setIsSysEmbargo(global_isSysEmbargo);
                            setIsSysCcc(global_isSysCcc);
                        }
                    }
                });
            }
            $.parser.parse('#list');

            //设置只读输入框背景色
            setReadonlyBgColor('easyui-textbox', '#EBEBE4');
            setReadonlyBgColor('easyui-combogrid', '#EBEBE4');
            $('#partNumber').combogrid('textbox').css('background', '#EBEBE4');
            $('#ciqCode').combobox('setValue', orderedProduct['CIQCode']);

        });

        //申报要素查询
        function elementsQuery() {
            var elementsWin = orderedProduct['SetWindow'] + '_elements_' + Math.floor(Math.random() * 10000);
            $.myDialog.setMyDialog(elementsWin, window);
            var hsCode = $('#hsCode').combogrid('getText');
            var isHSCodeChanged = false;
            if (hsCode != global_curHSCode) {
                isHSCodeChanged = true;
            }
            if (hsCode == null || hsCode == undefined || hsCode == '') {
                $.messager.alert('提示', '请输入海关编码！');
                return;
            } else {
                var url = location.pathname.replace(/Edit.html/ig, '../ElementsQuery/Edit.html') + '?IsHSCodeChanged=' + isHSCodeChanged;
                $.myDialog({
                    url: url + '&setwindow=' + elementsWin + '&PvDataApiUrl=' + orderedProduct['PvDataApiUrl'],
                    noheader: false,
                    title: '申报要素查询',
                    closable: true,
                    width: '1100px',
                    height: '500px',
                    onClose: function () {

                    }
                });
            }
        }

        //设置海关编码
        function setHSCode(hsCode) {
            $('#hsCode').combogrid('setValue', hsCode);
            global_curHSCode = hsCode;
        }

        //系统判断是否需要商检
        function setIsSysCiq(isCiq) {
            if (isCiq) {
                $('#isCiq').prop('checked', true);
                $('#ciqPrice').combogrid('textbox').css('background', 'white');
                $('#ciqPrice').combogrid('textbox').attr('readonly', false);
            } else {
                $('#isCiq').prop('checked', false);
                $('#ciqPrice').combogrid('textbox').css('background', '#EBEBE4');
                $('#ciqPrice').combogrid('textbox').attr('readonly', true);
            }
        }

        //系统判断是否需要原产地证明
        function setIsSysCoo(isCoo) {
            if (isCoo) {
                $('#isCoo').prop('checked', true);
            } else {
                $('#isCoo').prop('checked', false);
            }
        }

        //系统判断是否是禁运产品
        function setIsSysEmbargo(isEmbargo) {
            if (isEmbargo) {
                global_isSysEmbargo = true;
                $('#embargo').css('display', 'block');
                $('#isEmbargo').prop('checked', true);
                $('#isEmbargo').prop('disabled', true);
            } else {
                global_isSysEmbargo = false;
                $('#embargo').css('display', 'none');
            }
        }

        //系统判断是否需要Ccc认证
        function setIsSysCcc(isCcc) {
            if (isCcc) {
                global_isSysCcc = true;
                $('#ccc').css('display', 'block');
            } else {
                global_isSysCcc = false;
                $('#ccc').css('display', 'none');
            }
        }

        //品牌变更
        function mfrChanged() {
            global_isMfrChanged = true;
            verifyManufacturer();
        }

        //验证客户填写的品牌与申报要素中的品牌是否一致
        function verifyManufacturer() {
            if ($('#elements').val() != '') {
                var hsCode = $('#hsCode').combogrid('getText');
                getDataFun(PvDataApiUrl + 'Classify/GetElementsFormat', {
                    hsCode: hsCode
                }, {
                    success: function (res) {
                        var data = res.data;
                        if (data && data != null && data != '') {
                            var array = data.split(';');
                            var isContainMfr = false;
                            for (var i = 0; i < array.length; i++) {
                                var arr = array[i].split(':');
                                if (arr[1] == '品牌') {
                                    isContainMfr = true;
                                    var ElementsArray = $('#elements').val().split('|');
                                    var mfr = $.trim($('#manufacturer').textbox('getValue'));
                                    if ((mfr.toLowerCase() + '牌') != elementsArray[i].toLowerCase()) {
                                        $('#mfrVerify').css('display', 'none');
                                    } else {
                                        if (global_isMfrChanged) {
                                            ElementsArray[i] = mfr + '牌';
                                            $('#elements').textbox('setValue', ElementsArray.join('|'));
                                            $('#mfrVerify').css('display', 'none');
                                            global_isMfrChanged = false;
                                        } else {
                                            $('#mfrVerify').css('display', 'block');
                                        }
                                    }
                                }
                            }
                            if (!isContainMfr) {
                                $('#mfrVerify').css('display', 'none');
                            }
                        }
                    }
                })
            } else {
                $('#mfrVerify').css('display', 'none');
            }
        }

        //================================================= 归类信息提交 Begin ===================================================

        //归类提交
        function classify() {

            //验证表单数据
            var isValid = $('#form1').form('enableValidation').form('validate');
            if (!isValid) {
                return;
            }

            //验证税务名称
            var taxName = $('#taxName').textbox('getValue');
            var tariffName = $("#tariffName").combogrid("getText");
            if (taxName.split('*')[2] != tariffName) {
                $.messager.alert('提示', '税务名称第二个*号后的内容必须与报关品名一致！');
                return;
            }

            var rate1 = parseFloat($("#importPreferentialTaxRate").combogrid("getText"));
            if (!isNaN(parseFloat(rate1)) && (rate1 < 0 || rate1 > 1)) {
                $.messager.alert('提示', '优惠税率应为0~1之间的小数！');
                return;
            }

            var rate2 = parseFloat($("#vatRate").combogrid("getText"));
            if (!isNaN(parseFloat(rate2)) && (rate2 < 0 || rate2 > 1)) {
                $.messager.alert('提示', '增值税率应为0~1之间的小数！');
                return;
            }

            //如果需要商检，必须填写商检费
            var isCiq = $('#isCiq').prop('checked');
            if (isCiq) {
                if (IsNotFloat($("#ciqPrice").combogrid("getText"))) {
                    $.messager.alert('提示', '请输入正确的商检费！');
                    return;
                }
            }

            ajaxLoading();
            //归类提交时，需要验证型号、品牌与申报要素中是否一致
            var hsCode = $('#hsCode').combogrid('getText');
            getDataFun(PvDataApiUrl + '/Classify/GetElementsFormat', {
                hsCode: hsCode
            }, {
                success: function (res) {
                    ajaxLoadEnd();

                    var data = res.data;
                    if (data && data != null && data != '') {
                        var array = data.split(';');
                        var elementsArray = $('#elements').val().split('|');
                        var isPassed = true;
                        for (var i = 0; i < array.length; i++) {
                            var arr = array[i].split(':');
                            if (arr[1] == '品牌') {
                                var mfr = $.trim($('#manufacturer').textbox('getValue'));
                                if ((mfr.toLowerCase() + '牌') != elementsArray[i].toLowerCase()) {
                                    isPassed = false;
                                    $.messager.alert('提示', '品牌与申报要素中不一致！');
                                    break;
                                }
                            }

                            if (arr[1] == '型号') {
                                var partNumber = $.trim($('#partNumber').combogrid('getText'));
                                if (('型号:' + partNumber.toLowerCase()) != elementsArray[i].toLowerCase()) {
                                    isPassed = false;
                                    $.messager.alert('提示', '型号与申报要素中不一致！');
                                    break;
                                }
                            }
                        }

                        if (isPassed) {
                            //提交归类信息

                        }
                    }
                }
            });
        }

        //================================================= 归类信息提交 End =====================================================

        //关闭页面
        function closeSelf() {
            $.myWindow.close();
        }
    </script>

    <style>
        .liebiao {
            border: 1px solid #ddd;
        }

            .liebiao tr td {
                padding: 4px;
                border: 1px solid #ddd;
            }

        tr td.lbl {
            padding-right: 3px;
            text-align: right;
        }

        .red {
            color: red;
        }

        .ml10 {
            margin-left: 10px !important;
        }

        .ml30 {
            margin-left: 30px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="margin-bottom: 5px">
        <table class="liebiao" style="margin: 0 auto; width: 95%;" id="list">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 40%"></th>
                <th style="width: 10%"></th>
                <th style="width: 40%"></th>
            </tr>
            <tr>
                <td class="lbl">品牌:</td>
                <td>
                    <input class="easyui-textbox" id="manufacturer" name="manufacturer" data-options="events:{blur:mfrChanged},required:true,validType:'length[1,50]',missingMessage:'请输入品牌'" style="width: 100%; height: 22px" />
                </td>
                <td class="lbl">型号:</td>
                <td>
                    <input id="partNumber" name="partNumber" style="width: 100%; height: 22px" readonly />
                </td>
            </tr>
            <tr>
                <td class="lbl">海关编码:</td>
                <td>
                    <input id="hsCode" name="hsCode" style="width: 59%; height: 22px" />
                    <a id="btnHSCode" href="javascript:void(0);" class="easyui-linkbutton" style="width: 40%; height: 22px" onclick="elementsQuery()">
                        <span class='l-btn-icon-left'><span class="l-btn-icon icon-search"></span><span>申报要素查询</span></span>
                    </a>
                </td>
                <td class="lbl">报关品名:</td>
                <td>
                    <input id="tariffName" name="tariffName" style="width: 100%; height: 22px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">税务编码:</td>
                <td>
                    <input class="easyui-textbox" id="taxCode" name="taxCode" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入税务编码'" style="width: 100%; height: 22px" />
                </td>
                <td class="lbl">税务名称:</td>
                <td>
                    <input class="easyui-textbox" id="taxName" name="taxName" data-options="required:true,validType:['taxName','length[1,50]'],missingMessage:'请输入税务名称'" style="width: 100%; height: 22px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">优惠税率%:</td>
                <td>
                    <input id="importPreferentialTaxRate" name="importPreferentialTaxRate" style="width: 100%; height: 22px" />
                </td>
                <td class="lbl">增值税率%:</td>
                <td>
                    <input id="vatRate" name="vatRate" style="width: 100%; height: 22px" />
                </td>

            </tr>
            <tr>
                <!--<td class="lbl">消费税率%:</td>
                    <td>
                        <input id="ExciseTaxRate" name="ExciseTaxRate" style="width: 100%; height:22px" />
                    </td>-->
            </tr>
            <tr>
                <td class="lbl">成交单位:</td>
                <td>
                    <input class="easyui-textbox" id="unit" name="unit" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入成交单位'" style="width: 100%; height: 22px" />
                </td>
                <td class="lbl">法定第一单位:</td>
                <td>
                    <input class="easyui-textbox" id="legalUnit1" name="legalUnit1" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入法定第一单位'" style="width: 100%; height: 22px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">法定第二单位:</td>
                <td>
                    <input class="easyui-textbox" id="legalUnit2" name="legalUnit2" data-options="validType:'length[1,50]'" style="width: 100%; height: 22px" />
                </td>
                <td class="lbl">检验检疫编码:</td>
                <td>
                    <select class="easyui-combobox" id="ciqCode" name="ciqCode" data-options="required:true,editable:false,hasDownArrow:false,missingMessage:'请选择检验检疫编码'" style="width: 100%; height: 22px">
                        <option value="101">101</option>
                        <option value="102">102</option>
                        <option value="103">103</option>
                        <option value="104">104</option>
                        <option value="105">105</option>
                        <option value="106">106</option>
                        <option value="150">150</option>
                        <option value="999">999</option>
                    </select>
                </td>

            </tr>
            <tr>
                <td class="lbl">申报要素:</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="elements" name="elements" data-options="multiline:true,required:true" readonly="readonly" style="width: 100%; height: 40px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">特殊类型:</td>
                <td colspan="3">
                    <input type="checkbox" id="isCiq" name="isCiq" class="checkbox" /><label for="isCiq" style="margin-right: 20px">商检</label>
                    <input id="ciqPrice" name="ciqPrice" style="height: 30px" />
                    <input type="checkbox" id="isCoo" name="isCoo" class="checkbox" /><label for="isCoo" style="margin-left: 20px; margin-right: 20px">原产地证明</label>
                    <input type="checkbox" id="isCcc" name="isCcc" class="checkbox" /><label for="isCcc" style="margin-right: 20px">CCC认证</label>
                    <input type="checkbox" id="isEmbargo" name="isEmbargo" class="checkbox" /><label for="isEmbargo" style="margin-right: 20px">禁运</label>
                    <input type="checkbox" id="isHkControl" name="isHkControl" class="checkbox" /><label for="isHkControl" style="margin-right: 20px">香港管制</label>
                </td>
            </tr>
            <tr>
                <td class="lbl"></td>
                <td colspan="3">
                    <div id="embargo" style="font-size: 14px; color: red; display: none">该型号为禁运型号，请仔细核实！</div>
                    <div id="ccc" style="font-size: 14px; color: red; display: none">该型号需Ccc认证，请仔细核实！</div>
                </td>
            </tr>
            <tr>
                <td class="lbl"></td>
                <td colspan="3">
                    <div id="divSave">
                        <a id="btnConfirm" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-confirm'" onclick="classify()">确认归类</a>
                        <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="closeSelf()" style="margin-left: 8px;">关闭</a>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
