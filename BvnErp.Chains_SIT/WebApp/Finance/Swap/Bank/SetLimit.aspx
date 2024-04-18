<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetLimit.aspx.cs" Inherits="WebApp.Finance.Swap.Bank.SetLimit1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>受限国家</title>
    <uc:EasyUI runat="server" />
    <link href="../../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var BankData = eval('(<%=this.Model.BankData%>)');
        var HscodeData = eval('(<%=this.Model.HscodeData%>)');
        var IDdate = '<%=this.Model.IDdate%>';
        //数据初始化
        $(function () {
            $('#BankID').combobox({
                data: BankData,
            })
            $("#HSCode").combobox({
                data: HscodeData,
                onSelect: function (record) {
                    $.post('?action=getCountryName', { Code: record.value }, function (data) {
                        $("#Name").textbox("setValue", data);
                    });
                }
            });
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            data.append('ID', IDdate);
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
            <table id="editTable" style="margin:10px; line-height: 30px">
                <tr>
                    <td class="lbl">国家代码：</td>
                    <td>
                        <input class="easyui-combobox input" id="HSCode" name="HSCode"
                            data-options="valueField:'value',textField:'text',required:true,tipPosition:'bottom',missingMessage:'请选择国家代码',panelHeight:'120'" style="width:280px;"/>
                    </td>
                </tr>
                <tr>
                    <td  class="lbl">国家名：</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name"
                            data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',missingMessage:'请输入国家名'" style="width:280px;" />
                    </td>
                </tr>                
                <tr>
                    <td class="lbl">摘要：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="validType:'length[1,500]',tipPosition:'bottom',multiline:true,missingMessage:'请输入摘要'" style="width:280px;height:40px;" />
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