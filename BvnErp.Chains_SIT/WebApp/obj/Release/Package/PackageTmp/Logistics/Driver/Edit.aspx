<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Logistics.Driver.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>驾驶员-系统配置</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var id = getQueryString("ID");

        $(function () {
            var data = eval('(<%=this.Model.DriversInfo%>)');
            var carrierData = eval('(<%=this.Model.CarrierData%>)');
            $('#Mobile').textbox('setValue', data['Mobile']);
            $('#Name').textbox('setValue', data['Name']);
            $('#License').textbox('setValue', data['License']);
            $('#HSCode').textbox('setValue', data['HSCode']);
            $('#HKMobile').textbox('setValue', data['HKMobile']);
            $('#DriverCardNo').textbox('setValue', data['DriverCardNo']);
            $('#PortElecNo').textbox('setValue', data['PortElecNo']);
            $('#LaoPaoCode').textbox('setValue', data['LaoPaoCode']);
            $('#IsChcd').prop('checked', data['IsChcd']);
            $('#CarrierID').combobox({
                data: carrierData,
            });
            $('#CarrierID').combobox('setValue', data['CarrierID']);
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var mobile = $('#Mobile').textbox('getValue');
            //   var license = $('#License').textbox('getValue');

            $.post('?action=IsExitMobile', { ID: id, Mobile: mobile }, function (res) {
                if (!res) {
                    $.messager.alert('错误', "手机号已存在");
                    return;
                }
                //else {
                //    $.post('?action=IsExitIDCard', { ID: id, License: license }, function (res) {
                //        if (!res) {
                //            $.messager.alert('错误', "身份证号已存在");
                //            return;
                //        } else {
                //            SubmitDate();
                //        }
                //    });

                //}
                SubmitDate();
            });

        }

        function SubmitDate() {
            var data = new FormData($('#form1')[0]);
            data.append("ID", id);           
            data.append("IsChcdBack", $('#IsChcd').prop('checked'))
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
                $.messager.alert('提示', res.message, 'info', function () {
                    $.myWindow.close();
                });

            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <%-- <input class="easyui-textbox" type="hidden"  id="DriverID" />--%>
            <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">承运商:</td>
                    <td>
                        <input class="easyui-combobox input" id="CarrierID" name="CarrierID"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请选择承运商'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">姓名:</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'right',missingMessage:'请输入姓名'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">大陆手机号:</td>
                    <td>
                        <input class="easyui-textbox input" id="Mobile" name="Mobile"
                            data-options="validType:'length[1,50]',tipPosition:'right'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">海关编号:</td>
                    <td>
                        <input class="easyui-textbox input" id="HSCode" name="HSCode"
                            data-options="tipPosition:'right',validType:'length[1,50]'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">香港手机号:</td>
                    <td>
                        <input class="easyui-textbox input" id="HKMobile" name="HKMobile"
                            data-options="tipPosition:'right',validType:'length[1,50]'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">司机卡号:</td>
                    <td>
                        <input class="easyui-textbox input" id="DriverCardNo" name="DriverCardNo"
                            data-options="tipPosition:'right',validType:'length[1,50]'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">口岸电子编号:</td>
                    <td>
                        <input class="easyui-textbox input" id="PortElecNo" name="PortElecNo"
                            data-options="tipPosition:'right',validType:'length[1,50]'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">寮步密码:</td>
                    <td>
                        <input class="easyui-textbox input" id="LaoPaoCode" name="LaoPaoCode"
                            data-options="tipPosition:'right',validType:'length[1,50]'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">证件号码:</td>
                    <td>
                        <input class="easyui-textbox input" id="License" name="License"
                            data-options="tipPosition:'right',validType:'length[1,50]',missingMessage:'请输入证件号码'" style="width: 226px; height: 25px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">中港贸易:</td>
                    <td style="text-align: left">
                        <input type="checkbox" id="IsChcd" name="IsChcd"  checked="checked"/>
                        <label for="IsChcd" style="margin-right: 30px"></label>
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
