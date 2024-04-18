<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Logistics.Voyage.Edit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {
            var carrierData = eval('(<%=this.Model.CarrierData%>)');
            var ManifestInfoData = eval('(<%=this.Model.ManifestInfo%>)');

            $('#Voyage').textbox('setValue', ManifestInfoData['Voyage']);
            $("#VoyageType").combobox("select", ManifestInfoData['VoyageType']);

            $("#CarrierID").combobox({
                data: carrierData,
                onSelect: function (record) {
                    $.post('?action=GetVehicleDriverbyID', { CarrierID: record.ID }, function (data) {
                        $('#Vehicle').combogrid('grid').datagrid('loadData', data.Vehicles);
                        $('#driver').combogrid('grid').datagrid('loadData', data.Drivers);

                        if ($('#CarrierID').combobox('getValue') == ManifestInfoData["CarrierID"]) {
                            //如果 选择的CarrierID 和 原CarrierID 相等
                            $('#driver').combogrid('setValue', ManifestInfoData['Driver']);
                            $('#Vehicle').combogrid('setValue', ManifestInfoData['Vehicle']);
                        } else {
                            $('#driver').combogrid('setValue', '');
                            $('#Vehicle').combogrid('setValue', '');
                        }

                    });
                },
            });

            //司机
            $("#driver").combogrid({
                idField: "DriverName",
                textField: "Name",
                panelWidth: 300,
                fitColumns: true,
                required: false,

                mode: "local",
                columns: [[
                    { field: 'DriverName', title: '姓名', width: 50, align: 'left' },
                    { field: 'Mobile', title: '手机号', align: 'left', width: 50, },
                    { field: 'License', title: '证件号码', width: 150, align: 'left', hidden: 'true' },
                ]],
                onSelect: function () {
                    //var grid = $("#driver").combogrid('grid');
                    //var row = grid.datagrid('getSelected');
                },
            });

            //车辆
            $("#Vehicle").combogrid({
                idField: "HKLicense",
                textField: "Name",
                panelWidth: 400,
                fitColumns: true,
                required: false,
                mode: "local",
                columns: [[
                    { field: 'License', title: '车牌号', align: 'left', width: 80 },
                    { field: 'HKLicense', title: '香港车牌', width: 80, align: 'left' },
                    { field: 'VehicleType', title: '车辆类型', width: 80, align: 'left' },
                ]],
                onSelect: function () {
                    //var grid = $("#Vehicle").combogrid('grid');
                    //var row = grid.datagrid('getSelected');
                    //$('#Vehicle').combogrid('setValue', row.HKLicense);
                }
            });

            if (undefined != ManifestInfoData['CarrierID'] && "" != ManifestInfoData['CarrierID']) {
                $('#CarrierID').combobox('select', ManifestInfoData['CarrierID']);
            }

            //运输时间
            $('#TransportTime').textbox('setValue', ManifestInfoData['TransportTime']);
            //备注
            $('#Summary').textbox('setValue', ManifestInfoData['Summary']);

            intData();
        });

        function intData() {
            var id = getQueryString("ID");
            if (id) {
                $("#Voyage").textbox('disable');
            }

        }

        function Close() {
            $.myWindow.close();
        }



        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            var vehicle = $('#Vehicle').combogrid('getValue');
            var driver = $('#driver').combogrid('getValue');
            var g = $('#driver').combogrid('grid');	
            var r = g.datagrid('getSelected');	
            data.append("ID", $("#Voyage").textbox('getValue'));
            
            data.append("HKLicense", vehicle);
            data.append("DriverName", driver);
            if (r != null) {
            data.append("License",r.License);
            }

            var transportTime = $("#TransportTime").textbox('getValue');
            transportTime = transportTime.trim();
            data.append("TransportTime", transportTime);

            var summary = $("#Summary").textbox('getValue');
            summary = summary.trim();
            data.append("Summary", summary);

            var voyageType = $("#VoyageType").combobox("getValue");
            data.append("VoyageType", voyageType);

            SubmitDate(data);

        }

        function SubmitDate(data) {
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    }
                }
            }).done(function (res) {
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-left: 20px">

                <tr>
                    <td class="lbl">货物运输批次号:</td>
                    <td>
                        <input class="easyui-textbox input" id="Voyage" name="Voyage" data-options="required:true,validType:'voyNo',tipPosition:'right',required:true,missingMessage:'请输入货物运输批次号'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">承运商:</td>
                    <td>
                        <input class="easyui-combobox input" id="CarrierID" name="CarrierID"
                            data-options="valueField:'ID',textField:'Name',editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">车辆:</td>
                    <td>
                        <input class="easyui-combogrid input" id="Vehicle" name="Vehicle"
                            data-options="validType:'length[1,50]',tipPosition:'right',missingMessage:'请选择车辆', scrollbarSize:0, editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运输时间:</td>
                    <td>
                        <input class="easyui-datebox" data-options="" id="TransportTime" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">司机:</td>
                    <td>
                        <input class="easyui-combogrid input" id="driver" name="driver"
                            data-options="validType:'length[1,50]',tipPosition:'right',editable:false,missingMessage:'请选择司机', scrollbarSize:0," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运输类型:</td>
                    <td>
                        <select id="VoyageType" class="easyui-combobox" data-options="width:255,required:true,editable:false,">
                            <option value="1">普通</option>
                            <option value="2">包车</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="vertical-align: top;">备注:</td>
                    <td>
                        <input id="Summary" class="easyui-textbox" data-options="multiline:true,validType:'length[1,200]',tipPosition:'left'," style="height: 120px;">
                    </td>
                </tr>

            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
