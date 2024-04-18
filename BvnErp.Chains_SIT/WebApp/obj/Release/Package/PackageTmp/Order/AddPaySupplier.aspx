<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddPaySupplier.aspx.cs" Inherits="WebApp.Order.AddPaySupplier" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script>
        var SuppliersData = eval('<%=this.Model.suppliers%>');
        $(function () {
            $("#Supplier").combobox({
                data: SuppliersData
            });

        });
        //保存
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            var value = $('#Supplier').combobox("getValue");
            var ID = getQueryString("ID");
            //     MaskUtil.mask();
            $.post('?action=Save', { SupplierID: value, OrderId: ID },
                function (res) {
                    //        MaskUtil.unmask();
                    var result = JSON.parse(res);
                    if (result.success) {
                        $.messager.alert('', result.message, 'info', function () {
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', result.message);
                    }
                });
        }

        function Close() {
            $.myWindow.close();
        }


    </script>

</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">客户供应商：</td>
                    <td>
                        <input class="easyui-combobox" id="Supplier"
                            data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,tipPosition:'right'" style="width: 250px" />
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
