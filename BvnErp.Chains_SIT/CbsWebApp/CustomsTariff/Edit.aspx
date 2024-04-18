<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Needs.Cbs.WebApp.CustomsTariff.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Cbs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var tariff = eval('(<%=this.Model.TariffData%>)');
        //数据初始化
        $(function () {
            if (tariff != null) {
                $('#HSCode').textbox('setValue', tariff['HSCode']);
                $('#Name').textbox('setValue', tariff['Name']);
                $('#Elements').textbox('setValue', tariff['Elements']);
                $('#Unit1').textbox('setValue', tariff['Unit1']);
                $('#Unit2').textbox('setValue', tariff['Unit2']);
                $('#MFN').textbox('setValue', tariff['MFN']);
                $('#General').textbox('setValue', tariff['General']);
                $('#AddedValue').textbox('setValue', tariff['AddedValue']);
                $('#Consume').textbox('setValue', tariff['Consume']);
                $('#RegulatoryCode').textbox('setValue', tariff['RegulatoryCode']);
                $('#CIQCode').textbox('setValue', tariff['CIQCode']);
                $('#Summary').textbox('setValue', tariff['Summary']);
                $('#InspectionCode').textbox('setValue', tariff['InspectionCode']);

                $('#HSCode').textbox('readonly', true);
            }
        });

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }

            var data = new FormData($('#form1')[0]);
            if (tariff != null) {
                data.append("ID", tariff['ID']);
            }

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

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable">
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">海关编码</td>
                    <td>
                        <input class="easyui-textbox" id="HSCode" name="HSCode" data-options="validType:'length[1,50]',required:true" style="width: 250px" />
                    </td>
                    <td class="lbl" style="text-align: center; width: 80px">报关品名</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" data-options="validType:'length[1,250]',required:true" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">申报要素</td>
                    <td colspan="3" style="width: 580px">
                        <input class="easyui-textbox" id="Elements" name="Elements" data-options="required:true,tipPosition:'bottom'" style="width: 585px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">法定第一单位</td>
                    <td>
                        <input class="easyui-textbox" id="Unit1" name="Unit1" data-options="validType:'length[1,50]',required:true" style="width: 250px" />
                    </td>
                    <td class="lbl" style="text-align: center; width: 80px">法定第二单位</td>
                    <td>
                        <input class="easyui-textbox" id="Unit2" name="Unit2" data-options="validType:'length[1,50]'" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">最惠国税率</td>
                    <td>
                        <input class="easyui-numberbox" id="MFN" name="MFN" data-options="precision:'2',required:true" style="width: 250px" />
                    </td>
                    <td class="lbl" style="text-align: center; width: 80px">普通税率</td>
                    <td>
                        <input class="easyui-numberbox" id="General" name="General" data-options="precision:'2',required:true" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">增值税率</td>
                    <td>
                        <input class="easyui-numberbox" id="AddedValue" name="AddedValue" data-options="precision:'2',required:true" style="width: 250px" />
                    </td>
                    <td class="lbl" style="text-align: center; width: 80px">消费税率</td>
                    <td>
                        <input class="easyui-numberbox" id="Consume" name="Consume" data-options="precision:'2'" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">监管代码</td>
                    <td>
                        <input class="easyui-textbox" id="RegulatoryCode" name="RegulatoryCode" data-options="precision:'5'" style="width: 250px" />
                    </td>
                    <td class="lbl" style="text-align: center">检验检疫编码</td>
                    <td>
                        <input class="easyui-textbox" id="CIQCode" name="CIQCode" data-options="validType:'length[1,3]',required:true" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center; width: 80px">商检监管条件</td>
                     <td>
                        <input class="easyui-textbox" id="InspectionCode" name="InspectionCode"  style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl" style="text-align: center">摘要备注</td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,150]'" style="width: 585px" />
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
