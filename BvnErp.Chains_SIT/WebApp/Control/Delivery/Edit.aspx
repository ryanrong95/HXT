<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Control.Delivery.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单编辑</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="http://fix.szhxd.net/My/Scripts/area.data.js"></script>
    <script src="http://fix.szhxd.net/My/Scripts/areacombo.js"></script>
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var orderData = eval('(<%=this.Model.OrderData%>)');

        $(function () {
            //下拉框数据初始化
            var idType = eval('(<%=this.Model.IdType%>)');
            var payType = eval('(<%=this.Model.PayType%>)');
            var drivers = eval('(<%=this.Model.Drivers%>)');
            var vehicles = eval('(<%=this.Model.Vehicles%>)');
            var expressCompanies = eval('(<%=this.Model.ExpressCompanies%>)');

            //初始化证件类型下拉框
            $('#IDType').combobox({
                data: idType,
            });
            //初始化支付方式下拉框
            $('#PayType').combobox({
                data: payType,
            });
            //初始化快递公司下拉框
            $('#ExpressCompany').combobox({
                data: expressCompanies,
                onSelect: function (record) {
                    //非顺丰、跨越速运，只能到付
                    if (record.Name != '顺丰' && record.Name != '跨越速运') {
                        $('#PayType').combobox('setValue', '<%=Needs.Ccs.Services.Enums.PayType.CollectPay.GetHashCode()%>');
                        $("#PayType").combobox('readonly', true);
                    }
                    else {
                        $('#PayType').combobox('setValue', null);
                        $("#PayType").combobox('readonly', false);
                    }
                    $.post('?action=GetExpressType', { ExpressCompanyID: record.Name }, function (TypeData) {
                        $('#ExpressType').combobox('loadData', TypeData);
                        $('#ExpressType').combobox('setValue', null);
                    });
                }
            });
            //初始化司机下拉框
            $("#Driver").combogrid({
                idField: "ID",
                textField: "Name",
                data: drivers,
                panelWidth: 300,
                fitColumns: true,
                nowrap: false,
                mode: "local",
                columns: [[
                    { field: 'Name', title: '司机姓名', width: 80, align: 'left' },
                    { field: 'Mobile', title: '司机电话', width: 100, align: 'left' },
                    { field: 'CarrierName', title: '承运商', width: 120, align: 'left' },
                ]],
                onSelect: function () {
                    var grid = $("#Driver").combogrid('grid');
                    var row = grid.datagrid('getSelected');

                    $('#DriverMobile').textbox('setValue', row.Mobile);
                    $('#CarrierName').val(row.CarrierName)
                },
            });
            //初始化车辆下拉框
            $("#Vehicle").combogrid({
                idField: "ID",
                textField: "License",
                data: vehicles,
                panelWidth: 300,
                fitColumns: true,
                nowrap: false,
                mode: "local",
                columns: [[
                    { field: 'License', title: '车牌号', width: 80, align: 'left' },
                    { field: 'Type', title: '车辆类型', width: 100, align: 'left' },
                    { field: 'CarrierName', title: '承运商', width: 120, align: 'left' },
                ]],
            });

            //分拣信息列表初始化
            $('#sortings').myDatagrid({
                nowrap: false,
                pagination: false,
                fitcolumns: true,
                fit: true,
                onClickRow: onClickRow,
                loadFilter: function (data) {
                    $('#PackNo').numberbox('setValue', data.packNo);

                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        row['DeliveryQuantity'] = row['Quantity'];
                    }
                    return data;
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

            //提货时间不能为过去的时间
            $('#PickUpDate').datebox().datebox('calendar').calendar({
                validator: function (value) {
                    var curDate = new Date();
                    var date = new Date(value).toDateStr();
                    return date >= curDate.toDateStr();
                }
            });

            //送货时间不能为过去的时间
            $('#DeliverDate').datebox().datebox('calendar').calendar({
                validator: function (value) {
                    var curDate = new Date();
                    var date = new Date(value).toDateStr();
                    return date >= curDate.toDateStr();
                }
            });

            //订单国内交货信息初始化
            if (orderData != null && orderData != "") {
                if (orderData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.PickUpInStore.GetHashCode()%>") {
                    SZDelivery(1)
                    $("input[name='SZDeliveryType'][value=1]").attr("checked", true);
                    $("#Client").textbox("setValue", orderData["ClientName"]);
                    $("#Picker").textbox("setValue", orderData["Picker"]);
                    $("#PickerMobile").textbox("setValue", orderData["PickerMobile"]);
                    $("#IDType").combobox("setValue", orderData["IDType"]);
                    $("#IDNumber").textbox("setValue", orderData["IDNumber"]);

                    $.post('?action=GetDefaultConsignee', { ClientID: orderData["ClientID"] }, function (data) {
                        if (data != null) {
                            $("#Consignee").textbox("setValue", data["Name"]);
                            $("#Contact").textbox("setValue", data["Contact"]);
                            $("#ContactMobile").textbox("setValue", data["Mobile"]);
                            $("#Address").area("setValue", data.Address);
                        }
                    });
                } else if (orderData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.SentToClient.GetHashCode()%>" ||
                    orderData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.Shipping.GetHashCode()%>") {
                    if (orderData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.SentToClient.GetHashCode()%>") {
                        SZDelivery(2);
                        $("input[name='SZDeliveryType'][value=2]").attr("checked", true);
                    }
                    if (orderData["SZDeliveryType"] == "<%=Needs.Ccs.Services.Enums.SZDeliveryType.Shipping.GetHashCode()%>") {
                        SZDelivery(3);
                        $("input[name='SZDeliveryType'][value=3]").attr("checked", true);
                    }
                    $("#Consignee").textbox("setValue", orderData["Consignee"]);
                    $("#Contact").textbox("setValue", orderData["Contact"]);
                    $("#ContactMobile").textbox("setValue", orderData["ContactMobile"]);
                    $("#Address").area("setValue", orderData.Address);
                }
            }
        });

        //提交送货信息
        function Submit() {
            //验证表单数据
            //if (!Valid('form1')) {
            //    return;
            //}

            //验证客户名称、送货地址
            var value = $('input[name="SZDeliveryType"]:checked').val();
            var reg = /^(?!.*[\'\"#&+%\\<>])/g
            if (value == 1) {
                if (!reg.test($("#Client").textbox("getValue"))) {
                    $.messager.alert('提示', '公司名称不能含有特殊字符：\' \" # & + % \\ < >');
                    return;
                }
            }
            else if (value == 2 || value == 3) {
                if (!reg.test($("#Consignee").textbox("getValue"))) {
                    $.messager.alert('提示', '公司名称不能含有特殊字符：\' \" # & + % \\ < >');
                    return;
                }

                var addresses = $('#Address').area('getValue').split(' ');
                if (addresses.length < 3) {
                    $.messager.alert('提示', '请填写收货地址！');
                    return;
                }

                for (var i = 0; i < addresses.length; i++) {
                    if (!reg.test(addresses[i])) {
                        $.messager.alert('提示', '地址不能含有特殊字符：\' \" # & + % \\ < >');
                        return;
                    }
                }

                if (value == 3) {
                    if ($('#ExpressCompany').combobox('getText') == '顺丰' && $('#PackNo').numberbox('getValue') > 300) {
                        $.messager.alert('提示', '顺丰快递的件数不能超过300！');
                        return;
                    }
                }
            }

            //验证送货数量
            endEditing();
            var rows = $('#sortings').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                $('#sortings').datagrid('beginEdit', i);
                if ($('#sortings').datagrid('validateRow', i)) {
                    $('#sortings').datagrid('endEdit', i);
                    editIndex = undefined;
                } else {
                    $.messager.alert('提示', '请填写送货数量！');
                    editIndex = i;
                    return;
                }

                if (rows[i].DeliveryQuantity <= 0) {
                    $.messager.alert('提示', '送货数量必须大于0！');
                    return;
                }

                if (rows[i].DeliveryQuantity > rows[i].Quantity) {
                    $.messager.alert('提示', '送货数量不能大于剩余数量！');
                    return;
                }
            }
        
            //提交表单
            var data = new FormData($('#form1')[0]);
            var sortings = $('#sortings').datagrid('getRows');
            data.append('Sortings', JSON.stringify(sortings));
            data.append('OrderID', orderData['ID']);
            data.append('DriverName', $('#Driver').combobox('getText'));
            data.append('VehicleName', $('#Vehicle').combobox('getText'));
            data.append('ExpressCompanyName', $('#ExpressCompany').combobox('getText'));
            data.append('CarrierName', $('#CarrierName').val());

            MaskUtil.mask();
            $.ajax({
                url: '?action=SubmitDeliveryInfo',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $.messager.alert('', res.message, 'info', function () {
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        //设置国内交货方式中各个控件的显示/隐藏、是否必填
        function SZDelivery(option) {
            if (option == 1) {
                $('#PickUpTR').show();
                $('#PickerTR').show();
                $('#IDTR').show();
                $('#DeliverTR').hide();
                $('#ContactTR').hide();
                $('#AddressTR').hide();
                $('#DriverTR').hide();
                $('#VehicleTR').hide();
                $('#ExpressageTR').hide();
                $('#PayTypeTR').hide();

                $('#PickUpDate').datebox('textbox').validatebox('options').required = true;
                $('#Client').textbox('textbox').validatebox('options').required = true;
                $('#Picker').textbox('textbox').validatebox('options').required = true;
                $('#PickerMobile').textbox('textbox').validatebox('options').required = true;
                $('#IDType').combobox('textbox').validatebox('options').required = true;
                $('#IDNumber').textbox('textbox').validatebox('options').required = true;
                $('#DeliverDate').datebox('textbox').validatebox('options').required = false;
                $('#Consignee').textbox('textbox').validatebox('options').required = false;
                $('#Contact').textbox('textbox').validatebox('options').required = false;
                $('#ContactMobile').textbox('textbox').validatebox('options').required = false;
                $('#Driver').combogrid('textbox').validatebox('options').required = false;
                $('#DriverMobile').textbox('textbox').validatebox('options').required = false;
                $('#Vehicle').combogrid('textbox').validatebox('options').required = false;
                $('#ExpressCompany').combobox('textbox').validatebox('options').required = false;
                $('#ExpressType').combobox('textbox').validatebox('options').required = false;
                $('#PayType').textbox('textbox').validatebox('options').required = false;
            } else if (option == 2 || 3) {
                $('#PickUpTR').hide();
                $('#PickerTR').hide();
                $('#IDTR').hide();
                $('#DeliverTR').show();
                $('#ContactTR').show();
                $('#AddressTR').show();

                $('#PickUpDate').datebox('textbox').validatebox('options').required = false;
                $('#Client').textbox('textbox').validatebox('options').required = false;
                $('#Picker').textbox('textbox').validatebox('options').required = false;
                $('#PickerMobile').textbox('textbox').validatebox('options').required = false;
                $('#IDType').combobox('textbox').validatebox('options').required = false;
                $('#IDNumber').textbox('textbox').validatebox('options').required = false;
                $('#DeliverDate').datebox('textbox').validatebox('options').required = true;
                $('#Consignee').textbox('textbox').validatebox('options').required = true;
                $('#Contact').textbox('textbox').validatebox('options').required = true;
                $('#ContactMobile').textbox('textbox').validatebox('options').required = true;

                if (option == 2) {
                    $('#DriverTR').show();
                    $('#VehicleTR').show();
                    $('#ExpressageTR').hide();
                    $('#PayTypeTR').hide();

                    $('#Driver').combogrid('textbox').validatebox('options').required = true;
                    $('#DriverMobile').textbox('textbox').validatebox('options').required = true;
                    $('#Vehicle').combogrid('textbox').validatebox('options').required = true;
                    $('#ExpressCompany').combobox('textbox').validatebox('options').required = false;
                    $('#ExpressType').combobox('textbox').validatebox('options').required = false;
                    $('#PayType').textbox('textbox').validatebox('options').required = false;
                } else if (option == 3) {
                    $('#DriverTR').hide();
                    $('#VehicleTR').hide();
                    $('#ExpressageTR').show();
                    $('#PayTypeTR').show();

                    $('#Driver').combogrid('textbox').validatebox('options').required = false;
                    $('#DriverMobile').textbox('textbox').validatebox('options').required = false;
                    $('#Vehicle').combogrid('textbox').validatebox('options').required = false;
                    $('#ExpressCompany').combobox('textbox').validatebox('options').required = true;
                    $('#ExpressType').combobox('textbox').validatebox('options').required = true;
                    $('#PayType').textbox('textbox').validatebox('options').required = true;
                }
            }
            $('#form1').form('enableValidation').form('validate');
        }

        var editIndex = undefined;
        //结束编辑
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#sortings').datagrid('validateRow', editIndex)) {
                $('#sortings').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

        //触发事件
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#sortings').datagrid('selectRow', index)
                        .datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#sortings').datagrid('selectRow', editIndex);
                }
            }
        }
    </script>
    <style type="text/css">
        .datagrid-header-row,
        .datagrid-row {
            height: 30px;
        }
    </style>
</head>
<body>
        <div  id="content" style="text-align: center; margin: 5px;height:541.82px">
        <form id="form1" runat="server" method="post">
            <table id="SZDelivery" class="radioTable" style="font-size:12px">
                <tr>
                    <td>交货方式：</td>
                    <td style="text-align: left;">
                        <input type="radio" name="SZDeliveryType" value="1" id="PickUpInStore" class="radio" checked="checked" /><label for="PickUpInStore" style="margin-right: 30px">自提</label>
                        <input type="radio" name="SZDeliveryType" value="2" id="SentToClient" class="radio" /><label for="SentToClient" style="margin-right: 30px">送货上门</label>
                        <input type="radio" name="SZDeliveryType" value="3" id="Waybill" class="radio" /><label for="Waybill">国内快递</label>
                    </td>
                </tr>

                <tr id="PickUpTR">
                    <td>提货时间：</td>
                    <td>
                        <input class="easyui-datebox" id="PickUpDate" name="PickUpDate" value="" style="width: 300px" data-options="editable:false" />
                    </td>
                    <td>公司名称：</td>
                    <td>
                        <input class="easyui-textbox" id="Client" name="Client" value="" style="width: 300px" data-options="validType:'length[1,200]'" />
                    </td>
                </tr>
                <tr id="PickerTR">
                    <td>提货人：</td>
                    <td>
                        <input class="easyui-textbox" id="Picker" name="Picker" style="width: 300px" data-options="validType:'length[1,150]'" />
                    </td>
                    <td>联系方式：</td>
                    <td>
                        <input class="easyui-textbox" id="PickerMobile" name="PickerMobile" style="width: 300px" />
                    </td>
                </tr>
                <tr id="IDTR">
                    <td>证件类型：</td>
                    <td>
                        <input class="easyui-combobox" id="IDType" name="IDType" data-options="valueField:'Key',textField:'Value',editable:false" style="width: 300px" />
                    </td>
                    <td>证件号码：</td>
                    <td>
                        <input class="easyui-textbox" id="IDNumber" name="IDNumber" value="" style="width: 300px" data-options="validType:'idnumber'" />
                    </td>
                </tr>
                <tr id="DeliverTR">
                    <td>送货时间：</td>
                    <td>
                        <input class="easyui-datebox" id="DeliverDate" name="DeliverDate" value="" style="width: 300px" data-options="editable:false" />
                    </td>
                    <td>公司名称：</td>
                    <td>
                        <input class="easyui-textbox" id="Consignee" name="Consignee" value="" style="width: 300px" data-options="validType:'length[1,200]'" />
                    </td>
                </tr>
                <tr id="ContactTR">
                    <td>联系人：</td>
                    <td>
                        <input class="easyui-textbox" id="Contact" name="Contact" style="width: 300px" data-options="validType:'length[1,150]'" />
                    </td>
                    <td>联系方式：</td>
                    <td>
                        <input class="easyui-textbox" id="ContactMobile" name="ContactMobile" style="width: 300px" />
                    </td>
                </tr>
                <tr id="AddressTR">
                    <td>收货地址：</td>
                    <td colspan="3">
                        <table class="irtbaddress">
                            <tr>
                                <td style="text-align: left">
                                    <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:650" id="Address" name="Address"></div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>件数：</td>
                    <td>
                        <input class="easyui-numberbox" id="PackNo" name="PackNo" value="" data-options="precision:'0',required:true,validType:'packNo'" style="width: 300px" />
                    </td>
                </tr>
                <tr id="DriverTR">
                    <td>司机姓名：</td>
                    <td>
                        <input class="easyui-combogrid" id="Driver" name="Driver" data-options="valueField:'ID',textField:'Name',editable:false" style="width: 300px" />
                        <input type="hidden" id="CarrierName" />
                    </td>
                    <td>司机电话：</td>
                    <td>
                        <input class="easyui-textbox" id="DriverMobile" name="DriverMobile" style="width: 300px" readonly="readonly" data-options="validType:'mobile'" />
                    </td>
                </tr>
                <tr id="VehicleTR">
                    <td>车牌号：</td>
                    <td>
                        <input class="easyui-combogrid" id="Vehicle" name="Vehicle" data-options="valueField:'ID',textField:'License',editable:false" style="width: 300px" />
                    </td>
                </tr>
                <tr id="ExpressageTR">
                    <td>快递公司：</td>
                    <td>
                        <input class="easyui-combobox" id="ExpressCompany" name="ExpressCompany" data-options="valueField:'ID',textField:'Name',editable:false" style="width: 300px" />
                    </td>
                    <td>快递方式：</td>
                    <td>
                        <input class="easyui-combobox" id="ExpressType" name="ExpressType" data-options="valueField:'ID',textField:'TypeName',editable:false" style="width: 300px" />
                    </td>
                </tr>
                <tr id="PayTypeTR">
                    <td>付费方式：</td>
                    <td>
                        <input class="easyui-combobox" id="PayType" name="PayType" data-options="valueField:'Key',textField:'Value',editable:false" style="width: 300px" />
                    </td>
                </tr>
            </table>
        
            <div style="width:100%; height: 280px">
                <table id="sortings">
                    <thead>
                        <tr>
                            <th data-options="field:'BoxIndex',align:'center'" style="width:10%">箱号</th>
                            <th data-options="field:'Name',align:'left'" style="width: 20%">报关品名</th>
                            <th data-options="field:'Manufacturer',align:'left'" style="width:20%">品牌</th>
                            <th data-options="field:'Model',align:'left'" style="width:10%">规格型号</th>
                            <th data-options="field:'Origin',align:'center'" style="width: 20%">产地</th>
                            <th data-options="field:'Quantity',align:'center'" style="width:10%">剩余数量</th>
                            <th data-options="field:'DeliveryQuantity',align:'center',editor:{type:'numberbox',options:{precision:4,required:true}}" style="width:10%">送货数量</th>
                        </tr>
                    </thead>
                </table>
            </div>
    </form>
            </div>
    <div id="dlg-buttons" data-options="border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Submit()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
