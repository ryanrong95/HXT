<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListNew.aspx.cs" Inherits="WebApp.Logistics.ManifestVoyage.ListNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '运输批次';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var currentAction;
        var editMode;

        var initPageNumber = 1;
        var initPageSize = 20;
        var initUrl = '';

        var ajson;
        var loadCarrierInVoyage = 0;

        //列表中下拉选项
        var carriersForList = eval('(<%=this.Model.CarriersForList%>)');
        var cutStatus = eval('(<%=this.Model.CutStatus%>)');

        //高级编辑框选项
        var carrierTypeData = eval('(<%=this.Model.CarrierTypeData%>)');
        var vehicleType = eval('(<%=this.Model.VehicleType%>)');

        $(function () {
            if ('' == initUrl) {
                initUrl = location.pathname;
            }

            //列表, 下拉框数据初始化
            $('#CarrierForList').combobox({
                data: carriersForList
            });
            $('#CutStatus').combobox({
                data: cutStatus
            });

            //高级编辑框, 下拉框数据初始化
            //承运商类型
            $('#complicated-edit-carrierType').combobox({
                data: carrierTypeData,
            });
            $('#complicated-edit-carrierType').combobox({
                onSelect: function (record) {
                    loadComplicatedEditCarrierCodeAndName(record.TypeValue);
                    showOrHideVehicleHKLicense(record.IsInteLogistics);
                }
            });
            $('#complicated-edit-carrierType').combobox('setValue', carrierTypeData[0].TypeValue);

            //车辆类型
            $("#complicated-edit-vehicleType").combobox({
                data: vehicleType
            });

            //初始化车辆 datagrid
            $("#complicated-edit-vehicleLicense").combogrid({
                idField: "License",
                textField: "Name",
                panelWidth: 400,
                fitColumns: true,
                required: false,
                nowrap: false,
                mode: "local",
                columns: [[
                    { field: 'License', title: '车牌号', align: 'left', width: 80 },
                    { field: 'HKLicense', title: '香港车牌', width: 80, align: 'left' },
                    { field: 'VehicleType', title: '车辆类型', width: 80, align: 'left' },
                ]],
                onSelect: function (rowIndex, rowData) {
                    //加载 车辆类型、车牌号、车重、香港车牌号
                    $("#complicated-edit-vehicleType").textbox('setValue', rowData.VehicleType);
                    $("#complicated-edit-vehicleLicense").textbox('setValue', rowData.License);
                    $("#complicated-edit-vehicleWeight").textbox('setValue', rowData.Weight);
                    $("#complicated-edit-vehicleSize").textbox('setValue', rowData.Size);
                    $("#complicated-edit-vehicleHKLicense").textbox('setValue', rowData.HKLicense);
                },
            });

            //初始化司机 datagrid
            $("#complicated-edit-driverName").combogrid({
                idField: "DriverName",
                textField: "Name",
                panelWidth: 400,
                fitColumns: true,
                required: false,
                nowrap: false,
                mode: "local",
                columns: [[
                    { field: 'DriverName', title: '姓名', width: 60, align: 'left' },
                    { field: 'Mobile', title: '手机号', align: 'left', width: 90, },
                    { field: 'License', title: '证件号码', width: 150, align: 'left', },
                ]],
                onSelect: function (rowIndex, rowData) {
                    //加载 姓名、大陆手机号、海关编号、香港手机号、司机卡号、口岸电子编号、寮步密码、证件号码
                    $("#complicated-edit-driverName").textbox('setValue', rowData.DriverName);
                    $("#complicated-edit-driverMobile").textbox('setValue', rowData.Mobile);
                    $("#complicated-edit-driverHSCode").textbox('setValue', rowData.HSCode);
                    $("#complicated-edit-driverHKMobile").textbox('setValue', rowData.HKMobile);

                    $("#complicated-edit-driverDriverCardNo").textbox('setValue', rowData.DriverCardNo);
                    $("#complicated-edit-driverPortElecNo").textbox('setValue', rowData.PortElecNo);
                    $("#complicated-edit-driverLaoPaoCode").textbox('setValue', rowData.LaoPaoCode);
                    $("#complicated-edit-driverLicense").textbox('setValue', rowData.License);
                },
            });

            //承运商 简称 或 名称 选择后，加载对应的 名称 或 简称, 加载车辆和司机信息
            $('#complicated-edit-carrierCode').combobox({
                onSelect: function (record) {
                    loadComplicatedEditVehicleInfoAndDriverInfo(record.TypeValue);
                },
                onChange: function (newValue, oldValue) {
                    //var carrierCodeGrid = $('#complicated-edit-carrierCode').combobox('getValues');
                    //console.log("carrierCodeGrid ====   " + carrierCodeGrid);
                },
            });
            $('#complicated-edit-carrierName').combobox({
                onSelect: function (record) {
                    loadComplicatedEditVehicleInfoAndDriverInfo(record.TypeValue);
                },
                onChange: function (newValue, oldValue) {
                    //var carrierNameGrid = $('#complicated-edit-carrierName').combobox('getValues');
                    //console.log("carrierNameGrid =====----" + carrierNameGrid);
                },
            });














            //订单列表初始化
            $('#voyageGrid').myDatagrid({
                pageNumber: initPageNumber,
                pageSize: initPageSize,
                actionName:'VoyageListData',
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

            //精简编辑窗口初始化
            $('#simple-edit-dialog').dialog({
                buttons: [{
                    id: 'simple-edit-button-complicated',
                    text: '高级 >>',
                    width: '60px',
                    handler: function () {
                        //将精简框中的 运输批次号、运输类型、备注 复制到 高级框中
                        $('#complicated-edit-voyageNo').textbox('setValue', $('#simple-edit-voyageNo').textbox('getValue'));
                        $('#complicated-edit-transportTime').datebox('setValue', $('#simple-edit-transportTime').datebox('getValue'));
                        $('#complicated-edit-voyageType').combobox('select', $('#simple-edit-voyageType').textbox('getValue'));
                        $('#complicated-edit-voyageSummary').textbox('setValue', $('#simple-edit-voyageSummary').textbox('getValue'));

                        $('#simple-edit-dialog').dialog('close');
                        $('#complicated-edit-dialog').dialog('open');
                        editMode = '<%=WebApp.Logistics.ManifestVoyage.EditMode.Complicated.GetHashCode()%>';
                    }
                }, {
                        text: '保存',
                        iconCls: 'icon-save',
                        width: '60px',
                        handler: function () {
                            Save();
                        }
                    }, {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        width: '60px',
                        handler: function () {
                            $('#simple-edit-dialog').dialog('close');
                        }
                    }]
            });

            //高级按钮左对齐
            $("#simple-edit-button-complicated").css('float', 'left');

            //高级编辑窗口初始化
            $('#complicated-edit-dialog').dialog({
                buttons: [{
                    id: 'complicated-edit-button-simple',
                    text: '精简 <<',
                    width: '60px',
                    handler: function () {
                        //将高级框中的 运输批次号、运输类型、备注 复制到 精简框中
                        $('#simple-edit-voyageNo').textbox('setValue', $('#complicated-edit-voyageNo').textbox('getValue'));
                        $('#simple-edit-transportTime').datebox('setValue', $('#complicated-edit-transportTime').datebox('getValue'));
                        $('#simple-edit-voyageType').combobox('select', $('#complicated-edit-voyageType').textbox('getValue'));
                        $('#simple-edit-voyageSummary').textbox('setValue', $('#complicated-edit-voyageSummary').textbox('getValue'));

                        $('#complicated-edit-dialog').dialog('close');
                        $('#simple-edit-dialog').dialog('open');
                        editMode = '<%=WebApp.Logistics.ManifestVoyage.EditMode.Simple.GetHashCode()%>';
                    }
                }, {
                        text: '保存',
                        iconCls: 'icon-save',
                        width: '60px',
                        handler: function () {
                            Save();
                        }
                    }, {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        width: '60px',
                        handler: function () {
                            $('#complicated-edit-dialog').dialog('close');
                        }
                    }]
            });

            //精简按钮左对齐
            $("#complicated-edit-button-simple").css('float', 'left');








        });

        //查询
        function Search() {
            var voyageNo = $("#VoyageNo").textbox('getValue');
            var carrier = $("#CarrierForList").combobox('getValue');
            var cutStatus = $("#CutStatus").combobox('getValue');

            var opts = $("#voyageGrid").datagrid("options");
            opts.url = initUrl;

            $('#voyageGrid').datagrid('reload', {
                action: 'VoyageListData',
                VoyageNo: voyageNo,
                Carrier: carrier,
                CutStatus: cutStatus
            });
        }

        //重置
        function Reset() {
            $("#VoyageNo").textbox('setValue', "");
            $("#CarrierForList").combobox('setValue', "");
            $("#CutStatus").combobox('setValue', "");
            Search();
        }

        //新增
        function Add() {
            //新增框打开的时候，使用第一个承运商类型，加载 承运商简称、承运上名称
            loadComplicatedEditCarrierCodeAndName(carrierTypeData[0].TypeValue);
            //新增框打开的时候，使用第一个承运商类型，显示或隐藏香港车牌号输入框
            showOrHideVehicleHKLicense(carrierTypeData[0].IsInteLogistics);

            $('#complicated-edit-dialog').window({ title: "新增", });
            $('#complicated-edit-button-simple').show();
            $('#complicated-edit-voyageNo').textbox('textbox').attr('readonly', false);
            $('#complicated-edit-voyageNo').textbox('enable');

            //清除精简编辑框的内容
            $('#simple-edit-voyageNo').textbox('setValue', '');
            $('#simple-edit-transportTime').datebox('setValue', '');
            $('#simple-edit-voyageType').combobox('select', '1');
            $('#simple-edit-voyageSummary').textbox('setValue', '');

            //清除高级编辑框的内容 运输批次
            $('#complicated-edit-voyageNo').textbox('setValue', '');
            $('#complicated-edit-transportTime').datebox('setValue', '');
            $('#complicated-edit-voyageType').combobox('select', '1');
            $('#complicated-edit-voyageSummary').textbox('setValue', '');

            //清除高级编辑框的内容 承运商
            $('#complicated-edit-carrierType').combobox('setValue', carrierTypeData[0].TypeValue);
            $('#complicated-edit-carrierCode').textbox('setValue', '');
            $('#complicated-edit-carrierName').textbox('setValue', '');
            $('#complicated-edit-queryMark').textbox('setValue', '');
            $('#complicated-edit-contactMobile').textbox('setValue', '');
            $('#complicated-edit-carrierAddress').textbox('setValue', '');
            $('#complicated-edit-contactName').textbox('setValue', '');
            $('#complicated-edit-fax').textbox('setValue', '');

            //清除高级编辑框的内容 车辆
            $('#complicated-edit-vehicleLicense').textbox('setValue', '');
            $('#complicated-edit-vehicleType').textbox('setValue', '');
            $('#complicated-edit-vehicleWeight').textbox('setValue', '');
            $('#complicated-edit-vehicleSize').textbox('setValue', '');
            $('#complicated-edit-vehicleHKLicense').textbox('setValue', '');

            //清除高级编辑框的内容 司机
            $("#complicated-edit-driverName").textbox('setValue', '');
            $("#complicated-edit-driverMobile").textbox('setValue', '');
            $("#complicated-edit-driverHSCode").textbox('setValue', '');
            $("#complicated-edit-driverHKMobile").textbox('setValue', '');

            $("#complicated-edit-driverDriverCardNo").textbox('setValue', '');
            $("#complicated-edit-driverPortElecNo").textbox('setValue', '');
            $("#complicated-edit-driverLaoPaoCode").textbox('setValue', '');
            $("#complicated-edit-driverLicense").textbox('setValue', '');

            $('#simple-edit-dialog').dialog('open');
            currentAction = '<%=WebApp.Logistics.ManifestVoyage.CurrentAction.Add.GetHashCode()%>';
            editMode = '<%=WebApp.Logistics.ManifestVoyage.EditMode.Simple.GetHashCode()%>';
        }


        //编辑
        function Edit(id) {
            $('#complicated-edit-dialog').window({ title: "编辑", });
            $('#complicated-edit-button-simple').hide();
            $('#complicated-edit-voyageNo').textbox('textbox').attr('readonly', true);
            $('#complicated-edit-voyageNo').textbox('disable');

            $.post('?action=GetAllInfoByVoyageNo', { VoyageNo: id }, function (res) {
                var resJson = JSON.parse(res);
                if (resJson.success) {
                    //编辑框打开的时候，使用已有的承运商类型，加载 承运商简称、承运上名称
                    //loadComplicatedEditCarrierCodeAndName(resJson.voyageInfo.CarrierTypeInt);
                    //编辑框打开的时候，使用已有的承运商类型，显示或隐藏香港车牌号输入框
                    showOrHideVehicleHKLicense(resJson.voyageInfo.IsInteLogistics);

                    $.post('?action=GetCarrierCodeAndName', { CarrierTypeValue: resJson.voyageInfo.CarrierTypeInt, }, function (res) {
                        var res1 = JSON.parse(res);
                        if (res1.success) {
                            $('#complicated-edit-carrierCode').combobox({
                                data: res1.carriersCodeInfo,
                            });
                            $('#complicated-edit-carrierName').combobox({
                                data: res1.carriersNameInfo,
                            });

                            loadCarrierInVoyage = 1;
                            ajson = resJson;

                            $('#complicated-edit-carrierCode').combobox('select', resJson.voyageInfo.CarrierID);
                            //$('#complicated-edit-carrierName').textbox('setValue', resJson.voyageInfo.CarrierName);


                        }
                    });

                    //清除高级编辑框的内容 运输批次
                    $('#complicated-edit-voyageNo').textbox('setValue', resJson.voyageInfo.VoyageNo);
                    $('#complicated-edit-transportTime').datebox('setValue', resJson.voyageInfo.VoyageTransportTime);
                    $('#complicated-edit-voyageType').combobox('select', resJson.voyageInfo.VoyageTypeInt);
                    $('#complicated-edit-voyageSummary').textbox('setValue', resJson.voyageInfo.VoyageSummary);

                    //清除高级编辑框的内容 承运商
                    $('#complicated-edit-carrierType').textbox('setValue', resJson.voyageInfo.CarrierType);
                    //$('#complicated-edit-carrierCode').combobox('select', resJson.voyageInfo.CarrierID);
                    //$('#complicated-edit-carrierName').textbox('setValue', resJson.voyageInfo.CarrierName);
                    $('#complicated-edit-queryMark').textbox('setValue', resJson.voyageInfo.CarrierQueryMark);
                    $('#complicated-edit-contactMobile').textbox('setValue', resJson.voyageInfo.ContactMobile);
                    $('#complicated-edit-carrierAddress').textbox('setValue', resJson.voyageInfo.CarrierAddress);
                    $('#complicated-edit-contactName').textbox('setValue', resJson.voyageInfo.ContactName);
                    $('#complicated-edit-fax').textbox('setValue', resJson.voyageInfo.ContactFax);

                    //清除高级编辑框的内容 车辆
                    $('#complicated-edit-vehicleLicense').textbox('setValue', resJson.voyageInfo.VehicleLicence);
                    $('#complicated-edit-vehicleType').textbox('setValue', resJson.voyageInfo.VehicleType);
                    $('#complicated-edit-vehicleWeight').textbox('setValue', resJson.voyageInfo.VehicleWeight);
                    $('#complicated-edit-vehicleSize').textbox('setValue', resJson.voyageInfo.VehicleSize);
                    $('#complicated-edit-vehicleHKLicense').textbox('setValue', resJson.voyageInfo.VehicleHKLicense);

                    //清除高级编辑框的内容 司机
                    $("#complicated-edit-driverName").textbox('setValue', resJson.voyageInfo.DriverName);
                    $("#complicated-edit-driverMobile").textbox('setValue', resJson.voyageInfo.DriverMobile);
                    $("#complicated-edit-driverHSCode").textbox('setValue', resJson.voyageInfo.DriverHSCode);
                    $("#complicated-edit-driverHKMobile").textbox('setValue', resJson.voyageInfo.DriverHKMobile);

                    $("#complicated-edit-driverDriverCardNo").textbox('setValue', resJson.voyageInfo.DriverCardNo);
                    $("#complicated-edit-driverPortElecNo").textbox('setValue', resJson.voyageInfo.DriverPortElecNo);
                    $("#complicated-edit-driverLaoPaoCode").textbox('setValue', resJson.voyageInfo.DriverLaoPaoCode);
                    $("#complicated-edit-driverLicense").textbox('setValue', resJson.voyageInfo.DriverLicence);

                    $('#complicated-edit-dialog').dialog('open');
                    currentAction = '<%=WebApp.Logistics.ManifestVoyage.CurrentAction.Edit.GetHashCode()%>';
                    editMode = '<%=WebApp.Logistics.ManifestVoyage.EditMode.Complicated.GetHashCode()%>';
                }
            });
        }

        //截单
        function SureCut(id) {
            $.messager.confirm('确认', '请您再次确认是否截单。运输批次号：' + id, function (success) {
                if (success) {
                    $.post('?action=SureCut', { ID: id }, function (res) {
                        var resJson = JSON.parse(res);
                        $.messager.alert('消息', resJson.message);
                        $('#voyageGrid').myDatagrid('reload');
                    });
                }
            });
        }

        //查看
        function View(id) {
            var url = location.pathname.replace(/ListNew.aspx/ig, 'Detail.aspx') + "?ID=" + id + '&From=Logistics';
            window.location = url;
        }

        //舱单
        function ViewManifest(id) {
            var url = location.pathname.replace(/ListNew.aspx/ig, 'ManifestDetail.aspx') + "?ID=" + id;
            window.location = url;
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            if (row.CutStatusValue == '<%=Needs.Wl.Models.Enums.CutStatus.UnCutting.GetHashCode()%>') {
                buttons += '<a id="btnCut" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="SureCut(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">截单</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } //else {
                buttons += '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">详情</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            //}
            buttons += '<a id="btnCut" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="ViewManifest(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">舱单</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //加载高级编辑框, 根据 承运商类型 Value 加载 承运商"简称"和"名称"
        function loadComplicatedEditCarrierCodeAndName(carrierTypeValue) {
            $.post('?action=GetCarrierCodeAndName', { CarrierTypeValue: carrierTypeValue, }, function (res) {
                var resJson = JSON.parse(res);
                if (resJson.success) {
                    $('#complicated-edit-carrierCode').combobox({
                        data: resJson.carriersCodeInfo,
                    });
                    $('#complicated-edit-carrierName').combobox({
                        data: resJson.carriersNameInfo,
                    });
                }
            });
        }

        //显示或隐藏香港车牌号
        function showOrHideVehicleHKLicense(carrierTypeIsInteLogistics) {
            if (carrierTypeIsInteLogistics) {
                $('#position-complicated-edit-vehicleHKLicense').show();
                $('#position-title-complicated-edit-vehicleHKLicense').show();
            } else {
                $('#position-complicated-edit-vehicleHKLicense').hide();
                $('#position-title-complicated-edit-vehicleHKLicense').hide();
            }
        }

        //加载高级编辑框, 根据 承运商 ID 加载 车辆信息和司机信息
        function loadComplicatedEditVehicleInfoAndDriverInfo(carrierId) {
            $.post('?action=GetVehicleInfoAndDriverInfo', { CarrierId: carrierId, }, function (res) {
                var resJson = JSON.parse(res);
                //简称、名称 选择对应的
                //$('#complicated-edit-carrierCode').combobox('select', carrierId);
                //$('#complicated-edit-carrierName').combobox('select', carrierId);
                $('#complicated-edit-carrierCode').combobox('setValue', resJson.Carrier.CarrierCode);
                $('#complicated-edit-carrierName').combobox('setValue', resJson.Carrier.CarrierName);

                //加载对应的承运商及联系人信息
                //$('#complicated-edit-queryMark').textbox('setValue', resJson.Carrier.CarrierQueryMark);
                //$('#complicated-edit-contactMobile').textbox('setValue', resJson.Contact.ContactMobile);
                //$('#complicated-edit-carrierAddress').textbox('setValue', resJson.Carrier.CarrierAddress);
                //$('#complicated-edit-contactName').textbox('setValue', resJson.Contact.ContactName);
                //$('#complicated-edit-fax').textbox('setValue', resJson.Contact.ContactFax);
                

                
                if (1 == loadCarrierInVoyage) {
                    $('#complicated-edit-queryMark').textbox('setValue', ajson.voyageInfo.CarrierQueryMark);
                    $('#complicated-edit-contactMobile').textbox('setValue', ajson.voyageInfo.ContactMobile);
                    $('#complicated-edit-carrierAddress').textbox('setValue', ajson.voyageInfo.CarrierAddress);
                    $('#complicated-edit-contactName').textbox('setValue', ajson.voyageInfo.ContactName);
                    $('#complicated-edit-fax').textbox('setValue', ajson.voyageInfo.ContactFax);

                    loadCarrierInVoyage = 0;
                } else {
                    $('#complicated-edit-queryMark').textbox('setValue', resJson.Carrier.CarrierQueryMark);
                    $('#complicated-edit-contactMobile').textbox('setValue', resJson.Contact.ContactMobile);
                    $('#complicated-edit-carrierAddress').textbox('setValue', resJson.Carrier.CarrierAddress);
                    $('#complicated-edit-contactName').textbox('setValue', resJson.Contact.ContactName);
                    $('#complicated-edit-fax').textbox('setValue', resJson.Contact.ContactFax);
                }


                //加载车辆的 datagrid， 并清空车辆信息
                $('#complicated-edit-vehicleLicense').combogrid('grid').datagrid('loadData', resJson.Vehicles);

                var isNeedClearVehicle = 1;
                var currentVehicleType = $('#complicated-edit-vehicleType').combobox('getValue');
                var currentLicense = $('#complicated-edit-vehicleLicense').combobox('getValue');
                for (var i = 0; i < resJson.Vehicles.length; i++) {
                    if (resJson.Vehicles[i].VehicleType == currentVehicleType && resJson.Vehicles[i].License == currentLicense) {
                        isNeedClearVehicle = 0;
                        break;
                    }
                }

                if (1 == isNeedClearVehicle) {
                    $('#complicated-edit-vehicleLicense').textbox('setValue', '');
                    $('#complicated-edit-vehicleType').textbox('setValue', '');
                    $('#complicated-edit-vehicleWeight').textbox('setValue', '');
                    $('#complicated-edit-vehicleSize').textbox('setValue', '');
                    $('#complicated-edit-vehicleHKLicense').textbox('setValue', '');
                }                

                //加载司机的 datagrid， 并清空司机信息
                $('#complicated-edit-driverName').combogrid('grid').datagrid('loadData', resJson.Drivers);

                var isNeedClearDriver = 1;
                var currentDriverName = $('#complicated-edit-driverName').textbox('getValue');
                var currentDriverLicense = $('#complicated-edit-driverLicense').textbox('getValue');
                for (var i = 0; i < resJson.Drivers.length; i++) {
                    if (resJson.Drivers[i].DriverName == currentDriverName && resJson.Drivers[i].License == currentDriverLicense) {
                        isNeedClearDriver = 0;
                        break;
                    }
                }

                if (1 == isNeedClearDriver) {
                    $('#complicated-edit-driverName').textbox('setValue', '');
                    $('#complicated-edit-driverMobile').textbox('setValue', '');
                    $('#complicated-edit-driverHSCode').textbox('setValue', '');
                    $('#complicated-edit-driverHKMobile').textbox('setValue', '');

                    $('#complicated-edit-driverDriverCardNo').textbox('setValue', '');
                    $('#complicated-edit-driverPortElecNo').textbox('setValue', '');
                    $('#complicated-edit-driverLaoPaoCode').textbox('setValue', '');
                    $('#complicated-edit-driverLicense').textbox('setValue', '');
                }
            });
        }

        //提交保存
        function Save() {
            //currentAction = '<%=WebApp.Logistics.ManifestVoyage.CurrentAction.Edit.GetHashCode()%>';
            if (editMode == '<%=WebApp.Logistics.ManifestVoyage.EditMode.Simple.GetHashCode()%>') {
                //精简输入框保存
                if (!Valid('form1')) {
                    return;
                }

                MaskUtil.mask();
                $.post('?action=SaveSimple', {
                    VoyageNo: $('#simple-edit-voyageNo').textbox('getValue'),
                    TransportTime: $('#simple-edit-transportTime').datebox('getValue'),
                    VoyageType: $('#simple-edit-voyageType').textbox('getValue'),
                    VoyageSummary: $('#simple-edit-voyageSummary').textbox('getValue'),
                }, function (res) {
                    MaskUtil.unmask();
                    var resJson = JSON.parse(res);
                    if (resJson.success) {
                        $.messager.alert('提示', resJson.msg, 'info', function () {
                            $('#simple-edit-dialog').dialog('close');
                            Search();
                        });
                    } else {
                        $.messager.alert('错误', resJson.msg);
                    }
                });
            } else {
                //高级输入框保存
                if (!Valid('form2')) {
                    return;
                }

                var inputVehicleLicence = $('#complicated-edit-vehicleLicense').textbox('getValue').trim();
                if (inputVehicleLicence.length <= 0) {
                    $.messager.alert('提示', "请输入车牌号！");
                    return;
                }
                var inputDriveName = $("#complicated-edit-driverName").textbox('getValue').trim();
                if (inputDriveName.length <= 0) {
                    $.messager.alert('提示', "请输入司机姓名！");
                    return;
                }

                MaskUtil.mask();
                $.post('?action=SaveComplicated', {
                    //高级编辑框的内容 运输批次
                    VoyageNo: $('#complicated-edit-voyageNo').textbox('getValue'),
                    VoyageType: $('#complicated-edit-voyageType').textbox('getValue'),
                    TransportTime: $('#complicated-edit-transportTime').datebox('getValue'),
                    VoyageSummary: $('#complicated-edit-voyageSummary').textbox('getValue'),

                    //高级编辑框的内容 承运商
                    CarrierType: $('#complicated-edit-carrierType').combobox('getValue'),
                    CarrierCode: $('#complicated-edit-carrierCode').combobox('getText'),
                    CarrierName: $('#complicated-edit-carrierName').combobox('getText'),
                    QueryMark: $('#complicated-edit-queryMark').textbox('getValue'),

                    ContactMobile: $('#complicated-edit-contactMobile').textbox('getValue'),
                    CarrierAddress: $('#complicated-edit-carrierAddress').textbox('getValue'),
                    ContactName: $('#complicated-edit-contactName').textbox('getValue'),
                    Fax: $('#complicated-edit-fax').textbox('getValue'),

                    //高级编辑框的内容 车辆
                    VehicleLicense: $('#complicated-edit-vehicleLicense').textbox('getValue'),
                    VehicleType: $('#complicated-edit-vehicleType').combobox('getValue'),
                    VehicleWeight: $('#complicated-edit-vehicleWeight').textbox('getValue'),
                    VehicleSize: $('#complicated-edit-vehicleSize').textbox('getValue'),
                    VehicleHKLicense: $('#complicated-edit-vehicleHKLicense').textbox('getValue'),

                    //高级编辑框的内容 司机
                    DriverName: $("#complicated-edit-driverName").textbox('getValue'),
                    DriverMobile: $("#complicated-edit-driverMobile").textbox('getValue'),
                    DriverHSCode: $("#complicated-edit-driverHSCode").textbox('getValue'),
                    DriverHKMobile: $("#complicated-edit-driverHKMobile").textbox('getValue'),

                    DriverDriverCardNo: $("#complicated-edit-driverDriverCardNo").textbox('getValue'),
                    DriverPortElecNo: $("#complicated-edit-driverPortElecNo").textbox('getValue'),
                    DriverLaoPaoCode: $("#complicated-edit-driverLaoPaoCode").textbox('getValue'),
                    DriverLicense: $("#complicated-edit-driverLicense").textbox('getValue'),
                }, function (res) {
                    MaskUtil.unmask();
                    var resJson = JSON.parse(res);
                    if (resJson.success) {
                        $.messager.alert('提示', resJson.msg, 'info', function () {
                            $('#complicated-edit-dialog').dialog('close');
                            $('#voyageGrid').datagrid('reload');
                        });
                    } else {
                        $.messager.alert('错误', resJson.msg);
                    }
                });
            }





        }
    </script>
    <style>
        #simple-edit-dialog .lbl {
            padding-left: 5px;
            width: 100px;
            text-align: right;
            vertical-align: top;
        }

        #simple-edit-dialog table {
            border-collapse: separate;
            border-spacing: 0px 10px;
        }

        #complicated-edit-dialog .lbl {
            padding-left: 5px;
            width: 100px;
            text-align: right;
            vertical-align: top;
        }

        #complicated-edit-dialog table {
            border-collapse: separate;
            border-spacing: 0px 10px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <table style="margin: 5px 0">
                <tr>
                    <td class="lbl">货物运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" id="VoyageNo" data-options="width:200" />
                    </td>
                    <td class="lbl" style="padding-left: 10px;">承运商：</td>
                    <td>
                        <input class="easyui-combobox" id="CarrierForList" data-options="width:200,valueField:'Code',textField:'Name',editable:false" />
                    </td>
                    <td class="lbl" style="padding-left: 10px;">截单状态：</td>
                    <td>
                        <input class="easyui-combobox" id="CutStatus" data-options="width:200,valueField:'Key',textField:'Value',editable:false" />
                    </td>
                    <td style="padding-left: 10px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="voyageGrid" class="easyui-datagrid" title="运输批次" data-options="
            nowrap: false,
            border: false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'VoyageListData' },">
            <thead>
                <tr>
                    <th field="VoyageNo" data-options="align:'left'" style="width: 10%">货物运输批次号</th>
                    <th field="Carrier" data-options="align:'left'" style="width: 15%">承运商</th>
                    <th field="HKLicense" data-options="align:'left'" style="width: 10%">车牌号</th>
                    <th field="TransportTime" data-options="align:'center'" style="width: 8%">运输时间</th>
                    <th field="DriverName" data-options="align:'left'" style="width: 8%">驾驶员姓名</th>
                    <th field="VoyageType" data-options="align:'center'" style="width: 8%">运输类型</th>
                    <th field="CutStatus" data-options="align:'center'" style="width: 8%">截单状态</th>
                    <th field="CreateTime" data-options="align:'center'" style="width: 8%">创建日期</th>
                    <th data-options="field:'btnOpt',formatter:Operation,align:'left'" style="width: 20%">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 精简运输批次编辑框 html Begin ------------------------------------------------------------>

    <div id="simple-edit-dialog" class="easyui-dialog" style="width: 400px; height: 380px;" data-options="title: '新增', iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <form id="form1" runat="server">
            <table style="margin: 15px 20px 0 25px; font-size: 12px;">
                <tr>
                    <td class="lbl">货物运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" id="simple-edit-voyageNo" name="simple-edit-voyageNo" data-options="width:200,required:true,validType:'voyNo',missingMessage:'请输入运输批次号'," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运输时间：</td>
                    <td>
                        <input class="easyui-datebox" id="simple-edit-transportTime" name="simple-edit-transportTime" data-options="width:200,required:true,missingMessage:'请选择运输时间'," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运输类型：</td>
                    <td>
                        <select id="simple-edit-voyageType" class="easyui-combobox" data-options="width:200,required:true,editable:false,">
                            <option value="1">普通</option>
                            <option value="2">包车</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="simple-edit-voyageSummary" data-options="multiline:true,width:200,validType:'length[1,200]',tipPosition:'left'," style="height: 120px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>

    <!------------------------------------------------------------ 精简运输批次编辑框 html End ------------------------------------------------------------>

    <!------------------------------------------------------------ 高级运输批次编辑框 html Begin ------------------------------------------------------------>

    <div id="complicated-edit-dialog" class="easyui-dialog" style="width: 1000px; height: 520px;" data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <form id="form2">
            <table style="margin: 10px 20px 0 25px; font-size: 12px;">
                <tr>
                    <td class="lbl">货物运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" id="complicated-edit-voyageNo" name="complicated-edit-voyageNo" data-options="width:200,valueField:'TypeValue',textField:'TypeText',
                            validType:'voyNo',required:true,missingMessage:'请输入运输批次号'," />
                    </td>
                    <td class="lbl">运输时间：</td>
                    <td>
                        <input class="easyui-datebox" data-options="width:200," id="complicated-edit-transportTime" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运输类型：</td>
                    <td style="vertical-align: top;">
                        <select id="complicated-edit-voyageType" class="easyui-combobox" data-options="width:200,required:true,editable:false,">
                            <option value="1">普通</option>
                            <option value="2">包车</option>
                        </select>
                    </td>
                    <td class="lbl">备注：</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="complicated-edit-voyageSummary" name="complicated-edit-voyageSummary" data-options="multiline:true,width:506,
                            validType:'length[1,200]',tipPosition:'left'," style="height: 55px;" />
                    </td>
                </tr>

                <tr>
                    <td colspan="6" style="border-top: 1px dashed #ccc;"></td>
                </tr>

                <tr>
                    <td class="lbl">承运商类型：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-carrierType" name="complicated-edit-carrierType" data-options="width:200,
                            valueField:'TypeValue',textField:'TypeText',required:true,editable:false,missingMessage:'请选择承运商类型'," />
                    </td>
                    <td class="lbl">查询标记：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-queryMark" name="complicated-edit-queryMark" data-options="width:200, validType:'length[1,50]',
                            tipPosition:'right',missingMessage:'请输入查询标记'" />
                    </td>
                    <td class="lbl">联系人：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-contactName" name="complicated-edit-contactName" data-options="width:200, validType:'length[1,50]',
                            tipPosition:'right',missingMessage:'请输入联系人'" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">简称：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-carrierCode" name="complicated-edit-carrierCode" data-options="width:200, valueField:'TypeValue',textField:'TypeText',validType:'length[1,50]',
                            tipPosition:'right',required:true,missingMessage:'请输入简称'" />
                    </td>
                    <td class="lbl">联系电话：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-contactMobile" data-options="width:200, tipPosition:'right',missingMessage:'请输入联系电话'" />
                    </td>
                    <td class="lbl">传真：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-fax" data-options="width:200, tipPosition:'right'" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">名称：</td>
                    <td style="vertical-align: top;">
                        <input class="easyui-textbox input" id="complicated-edit-carrierName" name="complicated-edit-carrierName" data-options="width:200, 
                            valueField:'TypeValue',textField:'TypeText',required:true,validType:'length[1,100]',tipPosition:'right', missingMessage:'请输入名称'" />
                    </td>
                    <td class="lbl">承运商地址：</td>
                    <td colspan="3">
                        <input class="easyui-textbox input" id="complicated-edit-carrierAddress" name="complicated-edit-carrierAddress" data-options="width:506, 
                            tipPosition:'right' ,multiline:true,validType:'length[1,500]'" style="height: 55px;" />
                    </td>
                </tr>

                <tr>
                    <td colspan="6" style="border-top: 1px dashed #ccc;"></td>
                </tr>

                <tr>
                    <td class="lbl">车牌号：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-vehicleLicense" name="complicated-edit-vehicleLicense" data-options="width:200, required:true,
                            valueField:'TypeValue',textField:'TypeText',validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入车牌号'" />
                    </td>
                    <td class="lbl">车重：</td>
                    <td>
                        <input class="easyui-numberbox input" id="complicated-edit-vehicleWeight" name="complicated-edit-vehicleWeight" data-options="width:172, required:true, validType:'length[1,50]',
                            tipPosition:'right',missingMessage:'请输入车重'" />
                        <span>KGS</span>
                    </td>
                    <td class="lbl">尺寸：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-vehicleSize" name="complicated-edit-vehicleSize" data-options="width:200, validType:'length[1,50]',
                            tipPosition:'right',missingMessage:'请输入尺寸'" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">车辆类型：</td>
                    <td>
                        <input class="easyui-combobox input" id="complicated-edit-vehicleType" name="complicated-edit-vehicleType" data-options="width:200, required:true,valueField:'Key',
                            textField:'Value',editable:false, missingMessage:'请选择车辆类型'" />
                    </td>
                    <td class="lbl">
                        <div id="position-title-complicated-edit-vehicleHKLicense">香港车牌号：</div>
                    </td>
                    <td>
                        <div id="position-complicated-edit-vehicleHKLicense">
                            <input class="easyui-textbox input" id="complicated-edit-vehicleHKLicense" name="complicated-edit-vehicleHKLicense" data-options="width:200, 
                                validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入香港车牌号'" />
                        </div>
                    </td>
                </tr>

                <tr>
                    <td colspan="6" style="border-top: 1px dashed #ccc;"></td>
                </tr>

                <tr>
                    <td class="lbl">姓名：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverName" name="complicated-edit-driverName" data-options="width:200, required:true,
                            validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入姓名'" />
                    </td>
                    <td class="lbl">海关编号：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverHSCode" name="complicated-edit-driverHSCode" data-options="width:200, tipPosition:'right',
                            validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">口岸电子编号：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverPortElecNo" name="complicated-edit-driverPortElecNo" data-options="width:200, 
                            tipPosition:'right',validType:'length[1,50]'" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">大陆手机号：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverMobile" name="complicated-edit-driverMobile" data-options="width:200, required:true,
                            validType:'length[1,50]',tipPosition:'right'" />
                    </td>
                    <td class="lbl">香港手机号：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverHKMobile" name="complicated-edit-driverHKMobile" data-options="width:200, 
                            tipPosition:'right',validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">寮步密码：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverLaoPaoCode" name="complicated-edit-driverLaoPaoCode" data-options="width:200, 
                            tipPosition:'right',validType:'length[1,50]'" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl"></td>
                    <td></td>
                    <td class="lbl">司机卡号：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverDriverCardNo" name="complicated-edit-driverDriverCardNo" data-options="width:200, 
                            tipPosition:'right',validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">证件号码：</td>
                    <td>
                        <input class="easyui-textbox input" id="complicated-edit-driverLicense" name="complicated-edit-driverLicense" data-options="width:200, 
                            tipPosition:'right',validType:'length[1,50]',missingMessage:'请输入证件号码'" />
                    </td>
                </tr>


            </table>
        </form>




    </div>

    <!------------------------------------------------------------ 高级运输批次编辑框 html End ------------------------------------------------------------>

</body>
</html>
