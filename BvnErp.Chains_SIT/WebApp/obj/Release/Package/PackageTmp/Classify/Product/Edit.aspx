<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Classify.Product.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品归类</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script src="../../Scripts/ccs.log-1.0.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //表单是否已经提交标识，默认为false
        var global_isCommitted = false;
        //用于记录申报要素查询窗口的关闭操作
        //如果是点击“确认”按钮关闭，将isConfirm置为true，如果是点击“取消”按钮关闭，则将isConfirm置为false
        var global_isConfirm = true;
        //记录当前的海关编码，用于比较输入框中的海关编码是否发生了变化
        var global_curHSCode = '';
        //系统判断是否是禁运产品
        var global_isSysForbid = false;
        //系统判断是否是CCC产品
        var global_isSysCCC = false;
        //是否修改了品牌
        var global_isMfrChanged = false;

        //订单项数据
        //var orderItem = eval('(<%=this.Model.OrderItem%>)');
        var orderItem = eval('(' + '<%=this.Model.OrderItem%>'.replace(/\\/g, '解决斜杠转移字符') + ')');

        //产品变更信息 Begin

        var isReClassify = '<%=this.Model.IsReClassify%>';
        var mfrChangeTextJson = eval('(<%=this.Model.MfrChangeTextJson%>)');
        var modelChangeTextJson = eval('(<%=this.Model.ModelChangeTextJson%>)');
        var originChangeTextJson = eval('(<%=this.Model.OriginChangeTextJson%>)');
        var hsCodeChangeTextJson = eval('(<%=this.Model.HsCodeChangeTextJson%>)');
        var tariffNameChangeTextJson = eval('(<%=this.Model.TariffNameChangeTextJson%>)');

        //产品变更信息 End

        var currentSc = eval('(<%=this.Model.CurrentSc%>)');

        $(function () {
            if (orderItem['Model'] != null) {
                orderItem['Model'] = orderItem['Model'].replace('解决斜杠转移字符', '\\').replace('&amp;', '&');
            }
            if (orderItem['Elements'] != null) {
                orderItem['Elements'] = orderItem['Elements'].replace('解决斜杠转移字符', '\\').replace('&amp;', '&');
            }
            //初始化产品型号下拉框
            $('#Model').combogrid({
                idField: 'ID',
                textField: 'Model',
                nowrap: false,
                panelWidth: $("input[name=Model]").parent().width(),
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                mode: "local",
                columns: [[
                    { field: 'Model', title: '型号', width: 100, align: 'left' },
                    { field: 'Name', title: '报关品名', width: 100, align: 'left' },
                    { field: 'HSCode', title: '海关编码', width: 100, align: 'left' },
                    { field: 'Elements', title: '申报要素', width: 200, align: 'left' },
                ]],
                onChange: function () {
                    var grid = $('#Model').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    var name = $("#Model").combogrid("getText");
                    if (row == null || row.Name != name) {
                        $.post('?action=GetProductCategories', { Model: name }, function (res) {
                            $("#Model").combogrid("grid").datagrid("loadData", res.data);
                        });
                    }
                },
                onSelect: function () {
                    var grid = $('#Model').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    $('#TariffName').combogrid('setValue', row.Name);
                    SetHSCode(row.HSCode);
                    $('#Elements').textbox('setValue', row.Elements);
                    VerifyManufacturer();
                }
            });
            $('#Model').combogrid('textbox').bind('focus', function () {
                $('#Model').combogrid('showPanel');
            });

            //初始化海关编码下拉框
            $('#HSCode').combogrid({
                idField: 'ID',
                textField: 'HSCode',
                nowrap: false,
                panelWidth: 400,
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                mode: "local",
                columns: [[
                    { field: 'HSCode', title: '海关编码', width: 100, align: 'left' },
                    { field: 'Name', title: '报关品名', width: 300, align: 'left' },
                ]],
                onChange: function (record) {
                    $.post('?action=GetHSCodes', { HSCode: record, Origin: orderItem['Origin'], }, function (data) {
                        if (data == null || data == '') {
                            $("#HSCode").combogrid("grid").datagrid("loadData", []);
                        } else {
                            $("#HSCode").combogrid("grid").datagrid("loadData", data);
                        }
                    });

                    checkHighVlaueShowTipByHsCode(record);
                },
                onSelect: function () {
                    var grid = $('#HSCode').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    SetHSCode(row.HSCode);
                    $('#TariffName').combogrid('setValue', row.Name);
                    $('#TariffRate').combogrid('setValue', row.TariffRate);
                    $('#ValueAddTaxRate').combogrid('setValue', row.ValueAddTaxRate);
                    $('#Unit1').textbox('setValue', row.Unit1);
                    $('#Unit2').textbox('setValue', row.Unit2);

                    checkHighVlaueShowTipByHsCode(row.HSCode);

                    ElementsQuery();
                },
            });
            $('#HSCode').combogrid('textbox').bind('focus', function () {
                $('#HSCode').combogrid('showPanel');
            });

            //初始化报关品名下拉框
            $("#TariffName").combogrid({
                idField: "ID",
                textField: "Name",
                nowrap: false,
                panelWidth: $("input[name=TariffName]").parent().width(),
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                mode: "local",
                columns: [[
                    { field: 'Name', title: '报关品名', width: 150, align: 'left' },
                    { field: 'Model', title: '型号', align: 'left', hidden: true },
                    { field: 'TaxCode', title: '税务编码', width: 150, align: 'left' },
                    { field: 'TaxName', title: '税务名称', width: 180, align: 'left' },
                ]],
                onChange: function () {
                    var grid = $("#TariffName").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    var name = $("#TariffName").combogrid("getText");

                    //判断高价值提示显示
                    checkHighVlaueShowTip(name);

                    if (row == null || row.Name != name) {
                        $.post('?action=GetProductTaxCategories', { ClientID: orderItem['ClientID'], Name: name }, function (res) {
                            if (res == '') {
                                $("#TariffName").combogrid("grid").datagrid("loadData", []);
                            } else {
                                $("#TariffName").combogrid("grid").datagrid("loadData", res);
                            }
                        });
                    }
                },
                onSelect: function () {
                    var grid = $("#TariffName").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    $('#TaxCode').textbox('setValue', row.TaxCode);
                    $('#TaxName').textbox('setValue', row.TaxName);

                    if (row != null) {
                        //判断高价值提示显示
                        checkHighVlaueShowTip(row.Name);
                    }
                }
            });
            $('#TariffName').combogrid('textbox').bind('focus', function () {
                $('#TariffName').combogrid('showPanel');
            });

            //关税率历史记录
            $("#TariffRate").combogrid({
                idField: "ID",
                textField: "TariffRate",
                panelWidth: 400,
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                missingMessage: '请输入关税率',
                mode: "local",
                columns: [[
                    { field: 'TariffRate', title: '关税率', width: 100, align: 'left' },
                    { field: 'Model', title: '型号', align: 'left', hidden: true },
                    { field: 'CreateDate', title: '归类时间', width: 200, align: 'left' },
                    { field: 'Qty', title: '数量', width: 100, align: 'left' },
                ]],
                onShowPanel: function () {
                    var model = $("#Model").combogrid("getText");
                    $.post('?action=GetHistoryCategories', { Model: model }, function (res) {
                        $("#TariffRate").combogrid("grid").datagrid("loadData", res);
                    });
                },
                onSelect: function () {
                    var value = $("#TariffRate").combogrid("getValue");
                    var text = $("#TariffRate").combogrid("getText");
                    if (value == text) {
                        $("#TariffRate").combogrid("setValue", 0);
                    }
                }
            });
            $('#TariffRate').combogrid('textbox').bind('focus', function () {
                $('#TariffRate').combogrid('showPanel');
            });

            //增值税率历史记录
            $("#ValueAddTaxRate").combogrid({
                idField: "ID",
                textField: "AddedValueRate",
                panelWidth: 400,
                fitColumns: true,
                required: true,
                hasDownArrow: false,
                missingMessage: '请输入增值税率',
                mode: "local",
                columns: [[
                    { field: 'AddedValueRate', title: '增值税率', width: 100, align: 'left' },
                    { field: 'Model', title: '型号', align: 'left', hidden: true },
                    { field: 'CreateDate', title: '归类时间', width: 200, align: 'left' },
                    { field: 'Qty', title: '数量', width: 100, align: 'left' },
                ]],
                onShowPanel: function () {
                    var model = $("#Model").combogrid("getText");
                    $.post('?action=GetHistoryCategories', { Model: model }, function (res) {
                        $("#ValueAddTaxRate").combogrid("grid").datagrid("loadData", res);
                    });
                },
                onSelect: function () {
                    var value = $("#ValueAddTaxRate").combogrid("getValue");
                    var text = $("#ValueAddTaxRate").combogrid("getText");
                    if (value == text) {
                        $("#ValueAddTaxRate").combogrid("setValue", 0);
                    }
                }
            });
            $('#ValueAddTaxRate').combogrid('textbox').bind('focus', function () {
                $('#ValueAddTaxRate').combogrid('showPanel');
            });

            //单价历史记录
            $("#UnitPrice").combogrid({
                idField: "ID",
                textField: "UnitPrice",
                panelWidth: 400,
                fitColumns: true,
                required: true,
                hasDownArrow: true,
                mode: "local",
                columns: [[
                    { field: 'UnitPrice', title: '单价', width: 100, align: 'left' },
                    { field: 'Model', title: '型号', align: 'left', hidden: true },
                    { field: 'CreateDate', title: '归类时间', width: 200, align: 'left' },
                    { field: 'Qty', title: '数量', width: 100, align: 'left' },
                ]],
                onShowPanel: function () {
                    var model = $("#Model").combogrid("getText");
                    $.post('?action=GetHistoryCategories', { Model: model }, function (res) {
                        $("#UnitPrice").combogrid("grid").datagrid("loadData", res);

                        //单价与历史价格浮动超过10%，需要有明显的提示
                        unitPrirceTip(orderItem['UnitPrice'], res.avgUnitPrice);
                    });
                },
                onSelect: function () {
                    $('#UnitPrice').combogrid('setValue', orderItem['UnitPrice']);
                }
            });
            $('#UnitPrice').combogrid('textbox').bind('focus', function () {
                $('#UnitPrice').combogrid('showPanel');
            });

            //商检费历史记录
            $("#InspFee").combogrid({
                idField: "ID",
                textField: "InspectionFee",
                panelWidth: 400,
                fitColumns: true,
                hasDownArrow: false,
                mode: "local",
                columns: [[
                    { field: 'Model', title: '型号', width: 200, align: 'left' },
                    { field: 'IsInspection', title: '是否商检', width: 100, align: 'left' },
                    { field: 'InspectionFee', title: '商检费用', width: 100, align: 'left' },
                ]],
                onShowPanel: function () {
                    var model = $("#Model").combogrid("getText");
                    $.post('?action=GetHistoryCategories', { Model: model }, function (res) {
                        $("#InspFee").combogrid("grid").datagrid("loadData", res);
                    });
                },
                onSelect: function () {
                    var grid = $("#InspFee").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    if (row.IsInspection == '否') {
                        $('#InspFee').combogrid('setValue', null);
                    }
                }
            });
            $('#InspFee').combogrid('textbox').bind('focus', function () {
                $('#InspFee').combogrid('showPanel');
            });

            //原始PI列表初始化
            $('#pitable').myDatagrid({
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    //console.log(data)
                    var panel = $(this).datagrid('getPanel');
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });
                }
            });

            //产品归类信息初始化
            if (orderItem != null) {
                $('#ClientName').html(orderItem['ClientName']);
                $('#ClientCode').html(orderItem['ClientCode']);
                $('#ClientCode').html(orderItem['ClientCode']);
                $('#CreateDate').html(orderItem['CreateDate']);
                $('#OrderID').html(orderItem['OrderID']);
                $('#Manufacturer').textbox('setValue', orderItem['Manufacturer']);
                $('#Model').combogrid('setValue', orderItem['Model']);
                $('#Origin').textbox('setValue', orderItem['Origin']);
                $('#Currency').html(orderItem['Currency']);
                $('#TaxCode').textbox('setValue', orderItem['TaxCode']);
                $('#TaxName').textbox('setValue', orderItem['TaxName']);
                $('#HSCode').combogrid('setValue', orderItem['HSCode']);
                $('#TariffName').combogrid('setValue', orderItem['TariffName']);
                $('#TariffRate').combogrid('setValue', orderItem['TariffRate']);
                //$('#TariffValue').textbox('setValue', orderItem['TariffValue']);
                $('#ValueAddTaxRate').combogrid('setValue', orderItem['ValueAddTaxRate']);
                //$('#ValueAddTaxValue').textbox('setValue', orderItem['ValueAddTaxValue']);
                $('#Qty').html(orderItem['Qty']);
                $('#Unit').textbox('setValue', orderItem['Unit']);
                $('#UnitPriceValue').html(orderItem['UnitPrice']);
                $('#UnitPrice').combogrid('setValue', orderItem['UnitPrice']);
                $('#TotalPrice').html(orderItem['TotalPrice']);
                $('#Unit1').textbox('setValue', orderItem['Unit1']);
                $('#Unit2').textbox('setValue', orderItem['Unit2'])
                $('#CIQCode').combobox('setValue', orderItem['CIQCode']);
                $('#Elements').textbox('setValue', orderItem['Elements']);
                $('#Summary').textbox('setValue', orderItem['Summary']);

                SetIsSysForbid(orderItem['IsSysForbid']);
                SetIsSysCCC(orderItem['IsSysCCC']);

                var isCCC = orderItem['IsCCC'];
                var isForbid = orderItem['IsForbid'];
                var isOriginProof = orderItem['IsOriginProof'];
                var isInsp = orderItem['IsInsp'];
                var isHighValue = orderItem['IsHighValue'];

                //var OrderIsCharterBus = orderItem['OrderIsCharterBus'];
                //var OrderIsHighValue = orderItem['OrderIsHighValue'];
                //var OrderIsInspection = orderItem['OrderIsInspection'];
                //var OrderIsQuarantine = orderItem['OrderIsQuarantine'];
                //var OrderIsCCC = orderItem['OrderIsCCC'];


                if (isCCC) {
                    $('#IsCCC').prop('checked', true);
                }
                if (isForbid) {
                    $('#IsForbid').prop('checked', true);
                }
                if (isOriginProof) {
                    $('#IsOriginProof').prop('checked', true);
                }
                if (isInsp) {
                    $('#InspFee').combogrid('setValue', orderItem['InspFee']);
                } else {
                    $('#IsInsp').prop('checked', false);
                    $('#InspFee').combogrid('textbox').css('background', '#EBEBE4');
                    $('#InspFee').combogrid('textbox').attr('readonly', true);
                }
                if (isHighValue) {
                    $('#IsHighValue').prop('checked', true);
                }


                //显示订单特殊类型 Begin
                //var specialArr = [];
                //if (OrderIsCharterBus) {
                //    specialArr.push("包车");
                //}
                //if (OrderIsHighValue) {
                //    specialArr.push("高价值");
                //}
                //if (OrderIsInspection) {
                //    specialArr.push("商检");
                //}
                //if (OrderIsQuarantine) {
                //    specialArr.push("检疫");
                //}
                //if (OrderIsCCC) {
                //    specialArr.push("CCC");
                //}
                //$("#SpecialType").html(specialArr.join('|'));

                $("#SpecialType").html(orderItem['OrderSpecialTypeDisplay']);
                //显示订单特殊类型 End

                global_curHSCode = orderItem['HSCode'];
                //验证客户填写品牌与申报要素品牌是否一致
                VerifyManufacturer();
                //高价值产品警示 已移到报关品名 change 事件
                //if (orderItem['UnitPrice'] > 10) {
                //    $('#UnitPriceWarning').css('display', 'block');
                //}

                //单价与历史价格浮动超过10%，需要有明显的提示
                unitPrirceTip(orderItem['UnitPrice'], orderItem['AvgUnitPrice']);

                //显示单价的输入框始终是不可编辑状态 Begin
                $("input[name^='UnitPrice']").prev().attr('disabled', true);
                //显示单价的输入框始终是不可编辑状态 End
            }

            //是否商检valuechange事件
            $('#IsInsp').change(function () {
                if (this.checked) {
                    $('#InspFee').combogrid('textbox').css('background', 'white');
                    $('#InspFee').combogrid('textbox').attr('readonly', false);
                } else {
                    $('#InspFee').combogrid('textbox').css('background', '#EBEBE4');
                    $('#InspFee').combogrid('textbox').attr('readonly', true);
                    //清除商检费的输入框
                    $("#InspFee").textbox("setValue", '');
                }
            });

            //如果来自完成列表
            var from = getQueryString('From');
            if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.Done.GetHashCode()%>' || from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.ReClassified.GetHashCode()%>') {
                SetDisable();
            }

            //设置只读输入框背景色
            SetReadonlyBgColor('easyui-textbox', '#EBEBE4');
            SetReadonlyBgColor('easyui-combogrid', '#EBEBE4');

            //产品归类日志
            $.post('?action=GetProductClassifyLogs', { ID: orderItem['ID'] }, function (res) {
                if (res != null && res.length > 0) {
                    $('#ClassifyLog').ccslog({
                        data: res
                    });
                } else {
                    $('#ClassifyLog').append('<div style="margin:10px"><p style="margin:5px">无产品归类记录</p></div>');
                }
            });

            //产品归类变更日志
            $.post('?action=GetProductClassifyChangeLogs', { Model: orderItem['Model'] }, function (res) {
                if (res != null && res.length > 0) {
                    $('#ClassifyChangeLog').ccslog({
                        data: res
                    });
                } else {
                    $('#ClassifyChangeLog').append('<div style="margin:10px"><p style="margin:5px">无产品归类变更记录</p></div>');
                }
            });

            $("#Clock").click(function () {
                console.log("111");
                $("#UnitPrice").click();
            });


            //初始化 品牌、型号、原产地 变更记录的页面 Begin

            //品牌 文本框id Manufacturer , 图片 id ManufacturerImg , 
            if (isReClassify == 'True' && mfrChangeTextJson.length > 0) {
                //console.log("是重新归类 并且 品牌变更了");
                //console.log(mfrChangeTextJson);

                //修改输入框的宽度
                $("#Manufacturer").textbox('resize', $("#Manufacturer").next().width() - 28);

                //初始化变更历史的 combogrid panel
                $("#ManufacturerChangeHistory").combogrid({
                    idField: "OrderItemChangeLogID",
                    textField: "ShowText",
                    panelWidth: 400,
                    fitColumns: true,
                    nowrap: false,
                    mode: "local",
                    columns: [[
                        { field: 'ShowText', title: '内容', width: 300, align: 'left' },
                        { field: 'UpdateDate', title: '时间', width: 100, align: 'left' },
                    ]],
                    onShowPanel: function () {
                        $("#ManufacturerChangeHistory").combogrid("grid").datagrid("loadData", mfrChangeTextJson);
                    },
                });

                //显示图片
                $('<img style="height:22px; margin-left: 3px; vertical-align: middle;" src="../../App_Themes/xp/images/wenjian.png" />')
                    .prop('id', 'ManufacturerImg')
                    .insertAfter($("#ManufacturerChangeHistory").next());

                $("#ManufacturerImg").mouseover(function () {
                    $('#ManufacturerChangeHistory').combogrid('showPanel');
                });

                $("#ManufacturerImg").mouseout(function () {
                    $('#ManufacturerChangeHistory').combogrid('hidePanel');
                });

                $('#ManufacturerChangeHistory').next().css('border-color', '#ffff');
            }

            //型号 文本框id Model , 图片 id ModelImg , 
            if (isReClassify == 'True' && modelChangeTextJson.length > 0) {
                //console.log("是重新归类 并且 型号变更了");
                //console.log(modelChangeTextJson);

                //修改输入框的宽度
                $("#Model").textbox('resize', $("#Model").next().width() - 28);

                //初始化变更历史的 combogrid panel
                $("#ModelChangeHistory").combogrid({
                    idField: "OrderItemChangeLogID",
                    textField: "ShowText",
                    panelWidth: 400,
                    fitColumns: true,
                    nowrap: false,
                    mode: "local",
                    columns: [[
                        { field: 'ShowText', title: '内容', width: 300, align: 'left' },
                        { field: 'UpdateDate', title: '时间', width: 100, align: 'left' },
                    ]],
                    onShowPanel: function () {
                        $("#ModelChangeHistory").combogrid("grid").datagrid("loadData", modelChangeTextJson);
                    },
                });

                //显示图片
                $('<img style="height:22px; margin-left: 3px; vertical-align: middle;" src="../../App_Themes/xp/images/wenjian.png" />')
                    .prop('id', 'ModelImg')
                    .insertAfter($("#ModelChangeHistory").next());

                $("#ModelImg").mouseover(function () {
                    $('#ModelChangeHistory').combogrid('showPanel');
                });

                $("#ModelImg").mouseout(function () {
                    $('#ModelChangeHistory').combogrid('hidePanel');
                });

                $('#ModelChangeHistory').next().css('border-color', '#ffff');
            }

            //原产地 文本框id Origin , 图片 id OriginImg , 
            if (isReClassify == 'True' && originChangeTextJson.length > 0) {
                //console.log("是重新归类 并且 原产地变更了");
                //console.log(originChangeTextJson);

                //修改输入框的宽度
                $("#Origin").textbox('resize', $("#Origin").next().width() - 28);

                //初始化变更历史的 combogrid panel
                $("#OriginChangeHistory").combogrid({
                    idField: "OrderItemChangeLogID",
                    textField: "ShowText",
                    panelWidth: 400,
                    fitColumns: true,
                    nowrap: false,
                    mode: "local",
                    columns: [[
                        { field: 'ShowText', title: '内容', width: 300, align: 'left' },
                        { field: 'UpdateDate', title: '时间', width: 100, align: 'left' },
                    ]],
                    onShowPanel: function () {
                        $("#OriginChangeHistory").combogrid("grid").datagrid("loadData", originChangeTextJson);
                    },
                });

                //显示图片
                $('<img style="height:22px; margin-left: 3px; vertical-align: middle;" src="../../App_Themes/xp/images/wenjian.png" />')
                    .prop('id', 'OriginImg')
                    .insertAfter($("#OriginChangeHistory").next());

                $("#OriginImg").mouseover(function () {
                    $('#OriginChangeHistory').combogrid('showPanel');
                });

                $("#OriginImg").mouseout(function () {
                    $('#OriginChangeHistory').combogrid('hidePanel');
                });

                $('#OriginChangeHistory').next().css('border-color', '#ffff');
            }

            //海关编码 文本框id HSCode , 图片 id HSCodeImg , 
            if (isReClassify == 'True' && hsCodeChangeTextJson.length > 0) {
                //修改输入框的宽度
                $("#HSCode").textbox('resize', $("#HSCode").next().width() - 28);

                //初始化变更历史的 combogrid panel
                $("#HSCodeChangeHistory").combogrid({
                    idField: "OrderItemChangeLogID",
                    textField: "ShowText",
                    panelWidth: 400,
                    fitColumns: true,
                    nowrap: false,
                    mode: "local",
                    columns: [[
                        { field: 'ShowText', title: '内容', width: 300, align: 'left' },
                        { field: 'UpdateDate', title: '时间', width: 100, align: 'left' },
                    ]],
                    onShowPanel: function () {
                        $("#HSCodeChangeHistory").combogrid("grid").datagrid("loadData", hsCodeChangeTextJson);
                    },
                });

                //显示图片
                $('<img style="height:22px; margin-left: 3px; vertical-align: middle;" src="../../App_Themes/xp/images/wenjian.png" />')
                    .prop('id', 'HSCodeImg')
                    .insertAfter($("#HSCodeChangeHistory").next());

                $("#HSCodeImg").mouseover(function () {
                    $('#HSCodeChangeHistory').combogrid('showPanel');
                });

                $("#HSCodeImg").mouseout(function () {
                    $('#HSCodeChangeHistory').combogrid('hidePanel');
                });

                $('#HSCodeChangeHistory').next().css('border-color', '#ffff');
            }

            //报关品名 文本框id TariffName , 图片 id TariffNameImg , 
            if (isReClassify == 'True' && tariffNameChangeTextJson.length > 0) {
                //修改输入框的宽度
                $("#TariffName").textbox('resize', $("#TariffName").next().width() - 28);

                //初始化变更历史的 combogrid panel
                $("#TariffNameChangeHistory").combogrid({
                    idField: "OrderItemChangeLogID",
                    textField: "ShowText",
                    panelWidth: 400,
                    fitColumns: true,
                    nowrap: false,
                    mode: "local",
                    columns: [[
                        { field: 'ShowText', title: '内容', width: 300, align: 'left' },
                        { field: 'UpdateDate', title: '时间', width: 100, align: 'left' },
                    ]],
                    onShowPanel: function () {
                        $("#TariffNameChangeHistory").combogrid("grid").datagrid("loadData", tariffNameChangeTextJson);
                    },
                });

                //显示图片
                $('<img style="height:22px; margin-left: 3px; vertical-align: middle;" src="../../App_Themes/xp/images/wenjian.png" />')
                    .prop('id', 'TariffNameImg')
                    .insertAfter($("#TariffNameChangeHistory").next());

                $("#TariffNameImg").mouseover(function () {
                    $('#TariffNameChangeHistory').combogrid('showPanel');
                });

                $("#TariffNameImg").mouseout(function () {
                    $('#TariffNameChangeHistory').combogrid('hidePanel');
                });

                $('#TariffNameChangeHistory').next().css('border-color', '#ffff');
            }


            //初始化 品牌、型号、原产地 变更记录的页面 End
        });

        //申报要素查询
        function ElementsQuery() {
            var hsCode = $('#HSCode').combogrid('getText');
            var isHSCodeChanged = false;
            if (hsCode != global_curHSCode) {
                isHSCodeChanged = true;
            }
            if (hsCode == null || hsCode == undefined || hsCode == '') {
                $.messager.alert('提示', '请输入海关编码！');
                return;
            } else {
                var url = location.pathname.replace(/Edit.aspx/ig, '../ElementsQuery.aspx') + '?IsHSCodeChanged=' + isHSCodeChanged;
                self.$.myWindow({
                    url: url,
                    noheader: false,
                    title: '申报要素查询',
                    closable: true,
                    width: '1000px',
                    height: '480px',
                    onClose: function () {
                        if (global_isConfirm) {
                            //CalcTariff();
                        }
                    }
                });
            }
        }

        //归类
        function Classify() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证税务名称
            var taxName = $('#TaxName').textbox('getValue');
            var tariffName = $("#TariffName").combogrid("getText");
            if (taxName.split('*')[2] != tariffName) {
                $.messager.alert('提示', '税务名称第二个*号后的内容必须与报关品名一致！');
                return;
            }

            //数值验证
            if (IsNotFloat($("#TariffRate").combogrid("getText"))) {
                $.messager.alert('提示', '请输入正确的关税率！');
                return;
            } else {
                var rate = parseFloat($("#TariffRate").combogrid("getText"));
                if (rate < 0 || rate > 1) {
                    $.messager.alert('提示', '关税率应为0~1之间的小数！');
                    return;
                }
            }
            if (IsNotFloat($("#ValueAddTaxRate").combogrid("getText"))) {
                $.messager.alert('提示', '请输入正确的增值税率！');
                return;
            } else {
                var rate = parseFloat($("#ValueAddTaxRate").combogrid("getText"));
                if (rate < 0 || rate > 1) {
                    $.messager.alert('提示', '增值税率应为0~1之间的小数！');
                    return;
                }
            }

            //如果需要商检，必须填写商检费
            var isInsp = $('#IsInsp').prop("checked");
            var inspFee = $('#InspFee').combogrid("getValue");;
            if (isInsp) {
                if (IsNotFloat($("#InspFee").combogrid("getText"))) {
                    $.messager.alert('提示', '请输入正确的商检费！');
                    return;
                }
            }

            //检验"品牌"与"型号"是否与申报要素一致 Begin
            //var manufacturer = $('#Manufacturer').textbox('getValue');
            //var model = $("#Model").combogrid("getText");
            //var elements = $('#Elements').textbox('getValue');

            //var isManufacturerRight = 0; //品牌是否一致（1 - 一致， 0 - 不一致）
            //var isModelRight = 0; //型号是否一致（1 - 一致， 0 - 不一致）
            //if (elements.toLowerCase().indexOf(manufacturer.toLowerCase() + '牌') >= 0) {
            //    isManufacturerRight = 1;
            //}
            //if (elements.indexOf('型号:' + model + '|') >= 0) {
            //    isModelRight = 1;
            //}

            //if (isManufacturerRight != 1 || isModelRight != 1) {
            //    var display = "";
            //    if (isManufacturerRight != 1) {
            //        display += "品牌与申报要素中不一致<br>";
            //    }
            //    if (isModelRight != 1) {
            //        display += "型号与申报要素中不一致<br>";
            //    }

            //    $.messager.alert('提示', display);
            //    return;
            //}
            //检验"品牌"与"型号"是否与申报要素一致 End

            //报关员判断是否需要CCC认证
            var isCCC = $('#IsCCC').prop("checked");
            //报关员判断是否禁运
            var isForbid = $('#IsForbid').prop("checked");
            //报关员判断是否需要原产地证明
            var isOriginProof = $('#IsOriginProof').prop("checked");
            //报关员判断是否高价值产品
            var isHighValue = $('#IsHighValue').prop("checked");

            var values = FormValues("form1");
            values['ID'] = getQueryString('ID');
            values['From'] = getQueryString('From');
            values["OrderID"] = orderItem['OrderID'];
            values['ModelText'] = $("#Model").combogrid("getText");
            values['HSCode'] = $("#HSCode").combogrid("getText");
            values['TariffNameText'] = $("#TariffName").combogrid("getText");
            values['TariffRateText'] = $("#TariffRate").combogrid("getText");
            values['ValueAddTaxRateText'] = $("#ValueAddTaxRate").combogrid("getText");
            values['InspFeeText'] = $("#InspFee").combogrid("getText");
            values['CIQCodeText'] = $("#CIQCode").combobox("getText");
            values['IsSysForbid'] = global_isSysForbid;
            values['IsSysCCC'] = global_isSysCCC;
            values['IsCCC'] = isCCC;
            values['IsForbid'] = isForbid;
            values['IsOriginProof'] = isOriginProof;
            values['IsInsp'] = isInsp;
            values['IsHighValue'] = isHighValue;

            MaskUtil.mask();

            $.post('?action=GetElements', { HSCode: $('#HSCode').combogrid('getText') }, function (data) {
                var ismfrok = true;
                if (data && data != null && data != '') {
                    var array = data.split(';');
                    for (var i = 0; i < array.length; i++) {
                        var arr = array[i].split(':');
                        if (arr[1] == '品牌') {
                            var ElementsArray = $('#Elements').val().split('|');
                            var mfr = $('#Manufacturer').textbox('getValue').trim();
                            if ((mfr.toLowerCase() + '牌') != ElementsArray[i].toLowerCase()) {
                                ismfrok = false;
                                MaskUtil.unmask();
                                $.messager.alert('提示', '品牌与申报要素中不一致！');
                                break;
                            }
                        }

                        if (arr[1] == '型号') {
                            var ElementsArray = $('#Elements').val().split('|');
                            var model = $("#Model").combogrid("getText").trim();
                            if (('型号:' +  model.toLowerCase()) != ElementsArray[i].toLowerCase()) {
                                ismfrok = false;
                                MaskUtil.unmask();
                                $.messager.alert('提示', '型号与申报要素中不一致！');
                                break;
                            }
                        }

                    }
                }

                if (ismfrok) {
                    $.post('?action=ModelAndHSCodeCheck', { OrderItemID: orderItem['ID'], OrderID: orderItem['OrderID'], Model: values['ModelText'], HScode: values['HSCode'] }, function (res) {
                        var result = JSON.parse(res);
                        if (result.matched) {
                            $.post('?action=ClassifyCheck', { Model: encodeURI(JSON.stringify(values)).replace(/\+/g, '%2B') }, function (res) {
                                var result = JSON.parse(res);
                                if (result.pass) {
                                    DoClassify(values);

                                } else {
                                    MaskUtil.unmask();
                                    $.messager.confirm({
                                        width: 700,
                                        title: '提示',
                                        msg: result.message,
                                        fn: function (success) {
                                            if (success) {
                                                MaskUtil.mask();
                                                DoClassify(values);

                                            }
                                        }
                                    });
                                }
                            });
                        } else {
                            MaskUtil.unmask();
                            $.messager.alert({
                                width: 700,
                                title: '提示',
                                msg: result.message,
                                fn: function (success) {

                                }
                            });
                        }
                    });
                }
            });
        }

        //执行归类操作
        function DoClassify(values) {
            $.post('?action=Classify', { Model: encodeURI(JSON.stringify(values)).replace(/\+/g, '%2B') }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    //$.messager.alert('提示', result.message, 'info', function () {
                    //    Return();
                    //});

                    $("#classifySuccess-dialog-content").html(result.message);

                    $('#classifySuccess-dialog').dialog({
                        title: '提示',
                        width: 350,
                        height: 180,
                        closed: false,
                        //cache: false,
                        modal: true,
                        buttons: [{
                            id: 'btn-classifySuccess-ok',
                            text: '确定',
                            width: 70,
                            handler: function () {
                                Return();
                            }
                        }, {
                            id: 'btn-classifySuccess-continueClassify',
                            text: '继续归类',
                            width: 70,
                            handler: function () {
                                $('#classifySuccess-dialog').dialog('close');
                                MaskUtil.mask();
                                $.post('?action=ContinueEdit', { From: getQueryString('From'), }, function (resContinueEdit) {

                                    var resultContinueEdit = JSON.parse(resContinueEdit);

                                    if (resultContinueEdit.success) {
                                        //开始锁定 + 跳转到归类详情

                                        $.post(location.pathname.replace(/Edit.aspx/ig, 'FirstList.aspx') + '?action=Lock', { ID: resultContinueEdit.OrderItemID, }, function (resLock) {
                                            MaskUtil.unmask();
                                            var resultLock = JSON.parse(resLock);
                                            if (resultLock.success) {
                                                var url = location.pathname + '?ID=' + resultContinueEdit.OrderItemID + '&From=' + getQueryString('From');
                                                window.location = url;
                                            } else {
                                                $.messager.alert('提示', resultLock.message, 'info', function () {
                                                    Return();
                                                });
                                            }
                                        });


                                    } else {
                                        //查询下一个可归类型号失败（可能是 已没有 未被他人锁定或被您锁定的型号），弹框让点击确定，执行返回到列表

                                        MaskUtil.unmask();
                                        $.messager.alert('提示', resultContinueEdit.message, 'info', function () {
                                            Return();
                                        });

                                    }


                                });
                            }
                        }],
                    });

                    $('#classifySuccess-dialog').window('center'); //dialog 居中

                    //如果不是预处理一和预处理二进来的, 就隐藏继续归类按钮
                    var from = getQueryString('From');
                    if (from != '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step1.GetHashCode()%>' && from != '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step2.GetHashCode()%>') {
                        $("#btn-classifySuccess-continueClassify").hide();
                    }

                } else {
                    //$.messager.alert('归类', result.message);
                    $.messager.alert('归类', result.message, 'info', function () {
                        if (result.needReturn == true) {
                            Return();
                        }
                    });
                }
            });
        }

        //归类异常
        function Anomaly() {
            var id = getQueryString('ID');
            var from = getQueryString('From');
            var url = location.pathname.replace(/Edit.aspx/ig, 'Anomaly.aspx?ID=' + id + '&From=' + from);
            self.$.myWindow({
                url: url,
                noheader: false,
                title: '产品归类异常',
                closable: true,
                width: '400px',
                height: '260px',
                onClose: function () {

                }
            });
        }

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step1.GetHashCode()%>') {
                url = location.pathname.replace(/Edit.aspx/ig, 'FirstList.aspx');
            } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.Step2.GetHashCode()%>') {
                url = location.pathname.replace(/Edit.aspx/ig, 'SecondList.aspx');
            } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.Done.GetHashCode()%>') {
                url = location.pathname.replace(/Edit.aspx/ig, 'DoneList.aspx');
            } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.ReClassify.GetHashCode()%>') {
                url = location.pathname.replace(/Edit.aspx/ig, '../ProductChange/UnProcessList.aspx');
            } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.ReClassified.GetHashCode()%>') {
                url = location.pathname.replace(/Edit.aspx/ig, '../ProductChange/ProcessList.aspx');
            } else if (from == '<%=Needs.Ccs.Services.Enums.ClassifyStep.DoneEdit.GetHashCode()%>') {
                url = location.pathname.replace(/Edit.aspx/ig, 'DoneList.aspx');
            }
            window.location = url + "?" + parseParams(currentSc);
        }

        //系统判断是否需要商检
        function SetIsSysInspection(isInspection) {
            if (isInspection) {
                $('#IsInsp').prop('checked', true);
                $('#InspFee').combogrid('textbox').css('background', 'white');
                $('#InspFee').combogrid('textbox').attr('readonly', false);
            } else {
                $('#IsInsp').prop('checked', false);
                $('#InspFee').combogrid('textbox').css('background', '#EBEBE4');
                $('#InspFee').combogrid('textbox').attr('readonly', true);
            }
        }

        //系统判断是否需要原产地证明
        function SetIsSysOriginProof(isOriginProof) {
            if (isOriginProof) {
                $('#IsOriginProof').prop('checked', true);
            } else {
                $('#IsOriginProof').prop('checked', false);
            }
        }

        //记录申报要素查询窗口的关闭操作（“确认”还是“取消”）
        function SetIsConfirm(isConfirm) {
            global_isConfirm = isConfirm;
        }

        //设置海关编码
        function SetHSCode(hsCode) {
            $('#HSCode').combogrid('setValue', hsCode);
            global_curHSCode = hsCode;
        }

        //系统判断是否是禁运产品
        function SetIsSysForbid(isForbid) {
            if (isForbid) {
                global_isSysForbid = true;
                $('#Forbid').css('display', 'block');
                $('#IsForbid').prop('checked', true);
                $("input[name='IsForbid']").click(
                    function () {
                        this.checked = !this.checked;
                    }
                );
            } else {
                global_isSysForbid = false;
                $('#Forbid').css('display', 'none');
            }
        }

        //系统判断是否需要CCC认证
        function SetIsSysCCC(isCCC) {
            if (isCCC) {
                global_isSysCCC = true;
                $('#CCC').css('display', 'block');
            } else {
                global_isSysCCC = false;
                $('#CCC').css('display', 'none');
            }
        }

        //计算关税
        function CalcTariff() {
            var tariffRate = $('#TariffRate').val();
            if (tariffRate == 0) {
                $('#TariffValue').combogrid('setValue', 0);
                CalcValueAddedTax();
                return;
            }

            var totalPrice = $('#TotalPrice').val();
            var currency = $('#Currency').val();

            //公式: Round(报关总价 * 海关汇率 * 1.002, 0) * 关税率
            $.post('?action=CalcTariff', {
                TotalPrice: totalPrice, Currency: currency, TariffRate: tariffRate
            }, function (data) {
                if (data < 0) {
                    $.messager.alert('提示', '关税计算异常,请检查币种的海关汇率是否正确！');
                } else {
                    $('#TariffValue').textbox('setValue', data);
                    CalcValueAddedTax();
                }
            });
        }

        //计算增值税
        function CalcValueAddedTax() {
            //如果关税没有正确计算，则不再计算增值税
            var tariffValue = $("#TariffValue").val();
            if (tariffValue == null || tariffValue == '') {
                return;
            }

            var valueAddTaxRate = $('#ValueAddTaxRate').val();
            if (valueAddTaxRate == 0) {
                $('#ValueAddTaxValue').combogrid('setValue', 0);
                return;
            }

            var totalPrice = $('#TotalPrice').val();
            var currency = $('#Currency').val();

            //公式: Round(报关总价 * 海关汇率 * 1.002 + 关税, 0) * 增值税率
            $.post('?action=CalcValueAddedTax', {
                TotalPrice: totalPrice, Currency: currency,
                TariffValue: tariffValue, ValueAddTaxValue: valueAddTaxRate
            }, function (data) {
                if (data < 0) {
                    $.messager.alert('提示', '增值税计算异常,请检查币种的海关汇率是否正确！');
                } else {
                    $('#ValueAddTaxValue').textbox('setValue', data);
                }
            });
        }

        //判断输入的文本是否为浮点数 
        function IsNotFloat(text) {
            var len = text.length;
            var dotNum = 0;
            if (text.length == 0) {
                return true;
            }

            for (var i = 0; i < len; i++) {
                var oneNum = text.substring(i, i + 1);
                if (oneNum == ".") {
                    dotNum++;
                }
                if (((oneNum < "0" || oneNum > "9") && oneNum != ".") || dotNum > 1) {
                    return true;
                }
            }
            if (len > 1 && text.substring(0, 1) == "0") {
                if (text.substring(1, 2) != ".") {
                    return true;
                }
            }
            return false;
        }

        //品牌变更
        function MfrChanged() {
            global_isMfrChanged = true;
            VerifyManufacturer();
        }

        //验证客户填写的品牌与申报要素中的品牌是否一致
        function VerifyManufacturer() {
            if ($('#Elements').val() != '') {
                $.post('?action=GetElements', { HSCode: $('#HSCode').combogrid('getText') }, function (data) {
                    if (data && data != null && data != '') {
                        var array = data.split(';');
                        var isContainMfr = false;
                        for (var i = 0; i < array.length; i++) {
                            var arr = array[i].split(':');
                            if (arr[1] == '品牌') {
                                isContainMfr = true;
                                var ElementsArray = $('#Elements').val().split('|');
                                var mfr = $('#Manufacturer').textbox('getValue');
                                if ((mfr + '牌') == ElementsArray[i]) {
                                    $('#MfrVerify').css('display', 'none');
                                } else {
                                    if (global_isMfrChanged) {
                                        ElementsArray[i] = mfr + '牌';
                                        $('#Elements').textbox('setValue', ElementsArray.join('|'));
                                        $('#MfrVerify').css('display', 'none');
                                        global_isMfrChanged = false;
                                    } else {
                                        $('#MfrVerify').css('display', 'block');
                                    }
                                }
                            }
                        }
                        if (!isContainMfr) {
                            $('#MfrVerify').css('display', 'none');
                        }
                    }
                });
            } else {
                $('#MfrVerify').css('display', 'none');
            }
        }

        //隐藏操作按钮，disable输入框
        function SetDisable() {
            $('#btnHSCode').hide();
            $("#btnConfirm").css("display", "none");
            $("#btnAnomaly").css("display", "none");
            $('input[class*=textbox-text]').attr('disabled', true);
            $('input[class*=combobox]').attr('disabled', true);
            $('input[class*=combogrid]').attr('disabled', true);
            $('input[type=radio]').attr('disabled', true);
            $('input[type=checkbox]').attr('disabled', true);
        }

        function checkHighVlaueShowTip(currentValue) {
            if ((orderItem['UnitPrice'] > 10)
                && (currentValue.indexOf("二极管") >= 0 || currentValue.indexOf("三极管") >= 0
                    || currentValue.indexOf("二極管") >= 0 || currentValue.indexOf("三極管") >= 0)) {
                $('#UnitPriceWarning').css('display', 'block');
            } else {
                $('#UnitPriceWarning').css('display', 'none');
            }
        }

        //20190927目前电阻、电容类产品，请在系统提示：当归类时价格超过20美元以上的，系统提示“高价值”,电阻 8533； 电容 8532
        function checkHighVlaueShowTipByHsCode(HsCode) {
            if ((orderItem['UnitPrice'] > 20)
                && (HsCode.indexOf("8533") >= 0 || HsCode.indexOf("8532") >= 0)) {
                $('#UnitPriceWarning').css('display', 'block');
            } else {
                $('#UnitPriceWarning').css('display', 'none');
            }
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            return buttons;
        }

        //查看附件
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

        //单价与历史价格浮动超过10%，需要有明显的提示
        function unitPrirceTip(strCurrentUnitPrice, strHistoryAvgUnitPrice) {
            var currentUnitPrice = Number(strCurrentUnitPrice);
            var historyAvgUnitPrice = Number(strHistoryAvgUnitPrice);

            if (historyAvgUnitPrice <= 0) {
                return;
            }

            if (currentUnitPrice >= historyAvgUnitPrice * 0.9 && currentUnitPrice <= historyAvgUnitPrice * 1.1) {
                return;
            }

            $("input[name^='UnitPrice']").prev().css('color', 'red');
        }
    </script>
    <style>
        tr td.lbl {
            padding-right: 30px;
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

        .lh30 {
            line-height: 30px;
        }

        .clock {
            background: url(../Content/images/clock.png);
            display: inline-block;
            width: 14px;
            height: 14px;
            background-size: cover;
            margin-left: 5px;
            position: relative;
            top: 2px;
            cursor: pointer;
        }

        .combo-arrow {
            background: url(../../Content/images/clock.png) no-repeat center center;
        }

        .textbox-addon a {
            cursor: pointer;
        }

        .combo-arrow:hover {
            background-color: #fff;
            cursor: pointer;
        }

        .textbox-icon:hover {
            opacity: 1;
        }

        #UnitPrice + .combo {
            border: 0px;
        }

        #UnitPrice + .textbox-focused {
            box-shadow: none;
        }

        #UnitPrice + .combo .textbox-text {
            color: transparent;
            text-shadow: 0 0 0 #000;
        }

        #ManufacturerChangeHistory + span {
            border-color: #fff;
        }

        #ModelChangeHistory + span {
            border-color: #fff;
        }

        #OriginChangeHistory + span {
            border-color: #fff;
        }

        #HSCodeChangeHistory + span {
            border-color: #fff;
        }

        #TariffNameChangeHistory + span {
            border-color: #fff;
        }
    </style>
</head>
<body>
    <div class="easyui-tabs" data-option="fit:true;border:false" style="margin-bottom: 5px">
        <div title="产品归类" style="padding: 10px;" data-option="border:false">
            <form id="form1" runat="server">
                <div>
                    <table style="margin: 0 auto; width: 95%; border-spacing: 0px 5px">
                        <tr>
                            <th style="width: 10%"></th>
                            <th style="width: 40%"></th>
                            <th style="width: 10%"></th>
                            <th style="width: 40%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">客户编号:</td>
                            <td class="lh30" colspan="3">
                                <span id="ClientCode"></span><span class="ml10" id="ClientName"></span>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">订单编号:</td>
                            <td class="lh30" colspan="3">
                                <span id="OrderID"></span>
                                <span class="ml30">下单日期:</span><span class="ml10" id="CreateDate"></span>
                                <span class="ml30">币种:</span><span class="red ml10" id="Currency"></span>
                                <span class="ml30">特殊类型:</span><span class="red ml10" id="SpecialType"></span>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl lh30" valign="top">合同发票:</td>
                            <td class="lh30" colspan="3">
                                <p id="unUpload" style="display: none">未上传</p>
                                <table id="pitable" data-options="queryParams:{ action: 'dataFiles' }">
                                    <thead>
                                        <tr>
                                            <th data-options="field:'img',formatter:ShowImg">图片</th>
                                            <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                        </tr>
                                    </thead>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">品牌:</td>
                            <td>
                                <input class="easyui-textbox" id="Manufacturer" name="Manufacturer" data-options="events:{blur:MfrChanged},required:true,validType:'length[1,50]',missingMessage:'请输入品牌'" style="width: 100%; height: 30px" />
                                <input id="ManufacturerChangeHistory" style="width: 1px; height: 30px; display: none;" />
                            </td>
                            <td class="lbl">型号:</td>
                            <td>
                                <input class="easyui-combogrid" id="Model" name="Model" readonly="readonly" data-options="required:true,validType:'length[1,75]'" style="width: 100%; height: 30px" />
                                <input id="ModelChangeHistory" style="width: 1px; height: 30px; display: none;" />
                            </td>
                        </tr>
                        <% if (this.Model.IsReClassify && (this.Model.MfrChange != null || this.Model.ModelChange != null)) %>
                        <% { %>
                        <tr>
                            <% if (this.Model.MfrChange != null) %>
                            <% { %>
                            <td class="lbl"></td>
                            <td>
                                <div style="color: red; display: block" class="easyui-tooltip" title="<%= this.Model.MfrChangeSummary %>">
                                    <%= this.Model.MfrChangeSummary.Length > 45 ? this.Model.MfrChangeSummary.Substring(0, 45) + "...." : this.Model.MfrChangeSummary %>
                                </div>
                            </td>
                            <% } %>
                            <% else %>
                            <% { %>
                                <td class="lbl"></td>
                                <td></td>
                            <% } %>
                            <% if (this.Model.ModelChange != null) %>
                            <% { %>
                            <td class="lbl"></td>
                            <td>
                                <div style="color: red; display: block" class="easyui-tooltip" title="<%= this.Model.ModelChangeSummary %>">
                                    <%= this.Model.ModelChangeSummary.Length > 45 ? this.Model.ModelChangeSummary.Substring(0, 45) + "...." : this.Model.ModelChangeSummary %>
                                </div>
                            </td>
                            <% } %>
                        </tr>
                        <% } %>
                        <tr>
                            <td class="lbl">原产地:</td>
                            <td>
                                <input class="easyui-textbox" id="Origin" name="Origin" readonly="readonly" style="width: 100%; height: 30px" />
                                <input id="OriginChangeHistory" style="width: 1px; height: 30px; display: none;" />
                            </td>
                            <td class="lbl">单价:</td>
                            <td>
                                <input id="UnitPrice" name="UnitPrice" style="width: 75px; height: 30px" />
                                <span class="ml30">数量:</span>
                                <span id="Qty"></span>
                                <span class="ml30">总价:</span>
                                <span id="TotalPrice"></span>
                            </td>
                        </tr>
                        <% if (this.Model.IsReClassify && (this.Model.OriginChange != null)) %>
                        <% { %>
                        <tr>
                            <td class="lbl"></td>
                            <td>
                                <div style="color: red; display: block" class="easyui-tooltip" title="<%= this.Model.OriginChangeSummary %>">
                                    <%= this.Model.OriginChangeSummary.Length > 45 ? this.Model.OriginChangeSummary.Substring(0, 45) + "...." : this.Model.OriginChangeSummary %>
                                </div>
                            </td>
                        </tr>
                         <% } %>
                        <% else %>
                        <% { %>
                            <td class="lbl"></td>
                            <td></td>
                        <% } %>
                        <tr>
                            <td class="lbl">海关编码:</td>
                            <td>
                                <input class="easyui-combogrid" id="HSCode" name="HSCode" data-options="required:true,missingMessage:'请输入海关编码'" style="width: 59%; height: 30px" />
                                <input id="HSCodeChangeHistory" style="width: 1px; height: 30px; display: none;" />
                                <a id="btnHSCode" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="width: 40%; height: 30px" onclick="ElementsQuery()">申报要素查询</a>
                            </td>
                            <td class="lbl">报关品名:</td>
                            <td>
                                <input class="easyui-combogrid" id="TariffName" name="TariffName" data-options="required:true,validType:'length[1,250]',missingMessage:'请输入报关品名'" style="width: 100%; height: 30px" />
                                <input id="TariffNameChangeHistory" style="width: 1px; height: 30px; display: none;" />
                            </td>
                        </tr>
                        <% if (this.Model.IsReClassify && (this.Model.HsCodeChange != null || this.Model.TariffNameChange != null)) %>
                        <% { %>
                        <tr>
                            <% if (this.Model.HsCodeChange != null) %>
                            <% { %>
                            <td class="lbl"></td>
                            <td>
                                <div style="color: red; display: block" class="easyui-tooltip" title="<%= this.Model.HsCodeChangeSummary %>">
                                    <%= this.Model.HsCodeChangeSummary.Length > 45 ? this.Model.HsCodeChangeSummary.Substring(0, 45) + "...." : this.Model.HsCodeChangeSummary %>
                                </div>
                            </td>
                            <% } %>
                            <% else %>
                            <% { %>
                                <td class="lbl"></td>
                                <td></td>
                            <% } %>
                            <% if (this.Model.TariffNameChange != null) %>
                            <% { %>
                            <td class="lbl"></td>
                            <td>
                                <div style="color: red; display: block" class="easyui-tooltip" title="<%= this.Model.TariffNameChangeSummary %>">
                                    <%= this.Model.TariffNameChangeSummary.Length > 45 ? this.Model.TariffNameChangeSummary.Substring(0, 45) + "...." : this.Model.TariffNameChangeSummary %>
                                </div>
                            </td>
                            <% } %>
                        </tr>
                        <% } %>
                        <tr>
                            <td class="lbl">税务编码:</td>
                            <td>
                                <input class="easyui-textbox" id="TaxCode" name="TaxCode" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入税务编码'" style="width: 100%; height: 30px" />
                            </td>
                            <td class="lbl">税务名称:</td>
                            <td>
                                <input class="easyui-textbox" id="TaxName" name="TaxName" data-options="required:true,validType:['taxName','length[1,50]'],missingMessage:'请输入税务名称'" style="width: 100%; height: 30px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">关税率%:</td>
                            <td>
                                <input class="easyui-combogrid" id="TariffRate" name="TariffRate" style="width: 100%; height: 30px" />
                            </td>
                            <td class="lbl">增值税率%:</td>
                            <td>
                                <input class="easyui-combogrid" id="ValueAddTaxRate" name="ValueAddTaxRate" data-options="min:0,max:1,precision:'4',required:true" style="width: 100%; height: 30px" />
                            </td>

                        </tr>
                        <tr>
                            <td class="lbl">成交单位:</td>
                            <td>
                                <input class="easyui-textbox" id="Unit" name="Unit" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入成交单位'" style="width: 100%; height: 30px" />
                            </td>
                            <td class="lbl">法定第一单位:</td>
                            <td>
                                <input class="easyui-textbox" id="Unit1" name="Unit1" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入法定第一单位'" style="width: 100%; height: 30px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">法定第二单位:</td>
                            <td>
                                <input class="easyui-textbox" id="Unit2" name="Unit2" data-options="validType:'length[1,50]'" style="width: 100%; height: 30px" />
                            </td>
                            <td class="lbl">检验检疫编码:</td>
                            <td>
                                <select class="easyui-combobox" id="CIQCode" name="CIQCode" data-options="required:true,editable:false,hasDownArrow:false,missingMessage:'请选择检验检疫编码'" style="width: 100%; height: 30px">
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
                                <input class="easyui-textbox" id="Elements" name="Elements" data-options="multiline:true,required:true" readonly="readonly" style="width: 100%; height: 40px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">摘要备注:</td>
                            <td colspan="3">
                                <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,250]'" style="width: 100%; height: 30px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">特殊类型:</td>
                            <td colspan="3">
                                <input type="checkbox" id="IsInsp" name="IsInsp" checked="checked" class="checkbox" /><label for="IsInsp" style="margin-right: 20px">商检</label>
                                <input class="easyui-combogrid" id="InspFee" name="InspFee" style="height: 30px" data-options="min:0,precision:'4',prompt:'输入商检费...'" />
                                <input type="checkbox" id="IsOriginProof" name="IsOriginProof" class="checkbox" /><label for="IsOriginProof" style="margin-left: 20px; margin-right: 20px">原产地证明</label>
                                <input type="checkbox" id="IsCCC" name="IsCCC" class="checkbox" /><label for="IsCCC" style="margin-right: 20px">CCC认证</label>
                                <input type="checkbox" id="IsHighValue" name="IsHighValue" class="checkbox" /><label for="IsHighValue" style="margin-right: 20px">高价值产品</label>
                                <input type="checkbox" id="IsForbid" name="IsForbid" class="checkbox" /><label for="IsForbid" style="margin-right: 20px">禁运</label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl"></td>
                            <td colspan="3">
                                <div id="Forbid" style="font-size: 16px; color: red; margin-bottom: 10px; display: none">该型号为禁运型号，请仔细核实！</div>
                                <div id="CCC" style="font-size: 16px; color: red; margin-bottom: 10px; display: none">该型号需CCC认证，请仔细核实！</div>
                                <div id="MfrVerify" style="font-size: 16px; color: red; margin-bottom: 10px; display: none">客户填写的品牌与申报要素中的品牌不一致，请仔细核对！</div>
                                <div id="UnitPriceWarning" style="font-size: 16px; color: red; margin-bottom: 10px; display: none">该产品单价大于10，请仔细核对是否属于高价值产品！</div>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl"></td>
                            <td colspan="3">
                                <div id="divSave">
                                    <a id="btnConfirm" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Classify()">确认归类</a>
                                    <a id="btnAnomaly" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-no'" onclick="Anomaly()" style="margin-left: 8px;">归类异常</a>
                                    <input type="hidden" id="reason" name="reason" />
                                    <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()" style="margin-left: 8px;">返回</a>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </form>
        </div>
    </div>
    <div>
        <div id="ClassifyLog" class="easyui-panel" title="产品归类记录" style="margin-bottom: 5px">
        </div>
        <div id="ClassifyChangeLog" title="产品归类变更记录" class="easyui-panel">
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
    <div id="classifySuccess-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="classifySuccess-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>
</body>
</html>
