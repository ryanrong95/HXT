<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Logistics.Vehicle.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>车辆-系统配置</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var id = getQueryString("ID");
        $(function () {
            var data = eval('(<%=this.Model.VehiclesInfo%>)');
            var carrierData = eval('(<%=this.Model.CarrierData%>)')
            var vehicleType = eval('(<%=this.Model.VehicleType%>)');
            $('#HKLicense').textbox('setValue', data['HKLicense']);
            $('#License').textbox('setValue', data['License']);
            $("#Weight").textbox('setValue', data['Weight']);
            $("#Size").textbox('setValue', data['Size']);
            $('#CarrierID').combobox({
                data: carrierData,
                onChange: function () {
                    var carrierId = $("#CarrierID").combobox('getValue');
                    $.post('?action=GetCarrierType', { ID: carrierId, }, function (res) {
                        if (!res) {
                            $(".ShowTr").hide();
                        } else {
                            $(".ShowTr").show();
                        }

                    });
                }
            });
            $('#CarrierID').combobox('setValue', data['CarrierID']);

            //初始化车辆类型
            $("#VehicleType").combobox({
                data: vehicleType
            });
            $('#VehicleType').combobox('setValue', data['VehicleType']);
        });



        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            //var id = getQueryString("ID");
            var license = $('#License').textbox('getValue');
            var hklicense = $("#HKLicense").textbox('getValue');
            $.post('?action=IsExitLicense', { ID: id, License: license }, function (res) {
                if (!res) {
                    $.messager.alert('错误', "车牌号已存在");
                    return;
                }
                else {
                    $.post('?action=IsExitHKLicense', { ID: id, HKLicense: hklicense }, function (res) {
                        if (!res) {
                            $.messager.alert('错误', "香港车牌号已存在");
                            return;
                        } else {
                            SubmitData();
                        }
                    });
                }
            });
        }

        function SubmitData() {
            var data = new FormData($('#form1')[0]);
            data.append("ID", id)
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

        function GetCarrier() {

            debugger;
            var value = $("#CarrierID").combobox("getValue");
            alert(value)
            console.log(value)
        }
    </script>
    <style>
        .ShowTr {
            display:none;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">承运商:</td>
                    <td>
                        <input class="easyui-combobox input" id="CarrierID" name="CarrierID"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请选择承运商'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">车辆类型:</td>
                    <td>
                        <input class="easyui-combobox input" id="VehicleType" name="VehicleType" data-options="required:true,valueField:'Key',textField:'Value',missingMessage:'请选择车辆类型'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">车牌号:</td>
                    <td>
                        <input class="easyui-textbox input" id="License" name="License"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入车牌号'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">车重:</td>
                    <td>
                        <input class="easyui-textbox input" id="Weight" name="Weight"
                            data-options="validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入车重'"   style="width: 226px; height: 25px"/>
                        <span>KGS</span>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">尺寸:</td>
                    <td>
                        <input class="easyui-textbox input" id="Size" name="Size"
                            data-options="validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入尺寸'"   style="width: 226px; height: 25px"/>
                    </td>
                </tr>
                <tr class="ShowTr">
                    <td class="lbl">香港车牌号:</td>
                    <td>
                        <input class="easyui-textbox input" id="HKLicense" name="HKLicense"
                            data-options="validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入香港车牌号'"   style="width: 226px; height: 25px"/>
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
