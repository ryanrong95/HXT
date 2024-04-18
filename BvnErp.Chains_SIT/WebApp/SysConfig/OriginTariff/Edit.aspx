<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.SysConfig.OriginTariff.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>原产地税则</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var CustomsRateTypeData = eval('(<%=this.Model.CustomsRateType%>)');
        var OriginData = eval('(<%=this.Model.OriginData%>)');
        var OriginTariffData = eval('(<%=this.Model.OriginTariffData%>)');
        var HscodeData = eval('(<%=this.Model.HscodeData%>)');
        $(function () {
            $("#CustomsRateType").combobox({
                data: CustomsRateTypeData
            });
            $("#HSCode").combobox({
                data: HscodeData,
                onChange:function (record) {
                    $.post('?action=getDropdownlist', { value: record }, function (data) {
                        $("#HSCode").combobox('loadData', data);
                    });
                },
                onSelect: function (record) {
                    $.post('?action=getTariffName', { Code: record.value }, function (data) {
                        $("#Name").textbox("setValue", data);
                    });
                }
            });
            $("#Origin").combobox({
                data: OriginData
            });
            $('#Name').textbox("readonly", true);

            if (OriginTariffData != null) {
                $("#HSCode").combobox("setValue", OriginTariffData["HSCode"]);
                $("#Name").textbox("setValue", OriginTariffData["Name"]);
                $("#Origin").combobox("setValue", OriginTariffData["Origin"]);
                $("#CustomsRateType").combobox("setValue", OriginTariffData["CustomsRateType"]);
                $("#Rate").textbox("setValue", OriginTariffData["Rate"]);
                $("#StartDate").textbox("setValue", OriginTariffData["StartDate"]);
                $("#EndDate").textbox("setValue", OriginTariffData["EndDate"]);
            }
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var rate = $("#Rate").textbox("getValue");
            if (/^\d+(\.\d{1,2})?$/.test(rate)) {
            } else {
                $.messager.alert('提示',"税率应为精度两位以内的小数！");
                return;
            }
            if (rate >= 100) {
                $.messager.alert('提示',"税率为100以内整数或小数");
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (OriginTariffData != null) {
                data.append('id',OriginTariffData["id"])
            }
            data.append('HSCodeValue', $("#HSCode").combobox('getText'));
            MaskUtil.mask();
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
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
            <table id="editTable"  style="margin-left: 20px; width: 70% ;">
                <tr>
                    <td class="lbl">商品编码:</td>
                    <td>
                        <input class="easyui-combobox input" id="HSCode" name="HSCode"
                            data-options="valueField:'value',textField:'text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入商品编码,下拉框一次显示十条',panelHeight:'120'" style="width: 320px; height: 28px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">商品名称:</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name" style="width: 320px; height: 28px"
                            data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入商品名称'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">加征开始日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" name="StartDate" style="width: 320px; height: 28px" data-options="editable:false,required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">加征结束日期:</td>
                    <td>
                    <input class="easyui-datebox" id="EndDate" name="EndDate" style="width: 320px; height: 28px" data-options="editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">原产地:</td>
                    <td>
                        <input class="easyui-combobox input" id="Origin" name="Origin"
                            data-options="valueField:'value',textField:'text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入原产地',panelHeight:'120'" style="width: 320px; height: 28px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">征税类型:</td>
                    <td>
                        <input class="easyui-combobox input" id="CustomsRateType" name="CustomsRateType"
                            data-options="valueField:'value',textField:'text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入征税类型',panelHeight:'120'" style="width: 320px; height: 28px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">税率:</td>
                    <td>
                        <input class="easyui-textbox input" id="Rate" name="Rate" style="width: 320px; height: 28px"
                            data-options="min:0,required:true,validType:'length[1,50]',tipPosition:'bottom',tipPosition:'bottom',missingMessage:'请输入税率'" />
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

