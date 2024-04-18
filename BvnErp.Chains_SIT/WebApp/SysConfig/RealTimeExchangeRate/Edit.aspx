<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.SysConfig.RealTimeExchangeRate.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑汇率</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ExchangeRateData = eval('(<%=this.Model.ExchangeRateData%>)');
        var CurrencyData = eval('(<%=this.Model.CurrencyData%>)');
        var loaddata = eval('(<%=this.Model.loaddata%>)');
        $(function () {
            $("#Code").combobox({
                data: CurrencyData,
                panelHeight:150
            });          
            if (ExchangeRateData != null) {
                $("#Code").combobox("setValue", ExchangeRateData["Code"]);
                $("#Rate").textbox("setValue", ExchangeRateData["Rate"]);
                $("#Summary").textbox("setValue", ExchangeRateData["Summary"]);
                $('#Code').textbox("readonly", true);
            }
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            data.append('ID', loaddata['ID']);
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
            <table id="editTable" style="margin-left:20px;">
                <tr>
                    <td>币种：</td>
                    <td>
                        <input class="easyui-combobox input" id="Code" name="Code" data-options="valueField:'value',textField:'text',tipPosition:'bottom',required:true,editable:false,missingMessage:'请选择币种类型'" style="height:25px;width:200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">汇率</td>
                    <td>
                        <input class="easyui-textbox input" id="Rate" name="Rate"
                            data-options="required:true,validType:'length[1,18]',tipPosition:'bottom',missingMessage:'请输入汇率'" style="height:25px;width:200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注</td>
                    <td>
                        <input class="easyui-textbox input" id="Summary" name="Summary"
                            data-options="required:false,validType:'length[0,200]'"  style="height:25px;width:200px"/>
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

