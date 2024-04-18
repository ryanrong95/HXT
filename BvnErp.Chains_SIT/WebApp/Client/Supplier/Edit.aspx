<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Client.Supplier.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <style type="text/css">
        .lbl {
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        var supplierid = getQueryString("SupplierID");
        var places = eval('(<%=this.Model.Places%>)');
        $(function () {
            var ID = '<%=this.Model.ID%>';
            var ClientSupplierID = '<%=this.Model.ClientSupplierID%>';
             //下拉框数据初始化
            //$("#Rank").combobox({
            //    data: ranks
            //});
            $("#Place").combobox({
                data: places,
                onLoadSuccess: function () {
                    $('#Place').combobox("setValue", "HKG");
                }
            });
            if (ClientSupplierID != '') {
                var ClientSupplierData = eval('(<%=this.Model.ClientSupplierData != null ? this.Model.ClientSupplierData:""%>)');
            }
            if (ClientSupplierData != null) {
                $("#ChineseName").textbox("setValue", ClientSupplierData.ChineseName);
                $("#Name").textbox("setValue", ClientSupplierData.Name.replace(new RegExp("#39;", "g"), "'"));
                $("#Summary").textbox("setValue", ClientSupplierData.Summary);
                $("#Rank").combobox("setValue", ClientSupplierData.Rank);
                $("#Place").combobox("setValue", ClientSupplierData.Place);

            }

           

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }
                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values["ClientID"] = ID;
                values["SuplierID"] = supplierid;

                values["Name"] = values["Name"].trim();
                if (values["Name"] == null || values["Name"] == '') {
                    $.messager.alert('错误', "请输入英文名称");
                    return;
                }

                var reg = /[\u4e00-\u9fa5]/g;
                if (reg.test(values["Name"])) {
                    $.messager.alert('错误', "英文名称中不能输入汉字");
                    return;
                }

                //提交后台
                $.post('?action=IsExitName', { ID: supplierid, ClientID: ID, Name:values["Name"] }, function (res) {
                    if (!res) {
                        $.messager.alert('错误', "供应商名称已存在");
                        return;
                    } else {
                        $.post('?action=SaveClientSupplier', { Model: JSON.stringify(values) }, function (res) {
                            var result = JSON.parse(res);
                            $.messager.alert('消息', result.message, 'info', function () {
                                if (result.success) {
                                    closeWin();
                                }
                            });
                        });
                    }
                });
            //    $.post('?action=SaveClientSupplier', { Model: JSON.stringify(values) }, function (res) {
            //        var result = JSON.parse(res);
            //        $.messager.alert('消息', result.message, 'info', function () {
            //            if (result.success) {
            //                closeWin();
            //            }
            //        });
            //    });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });
        });

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                 <tr>
                    <td class="lbl">供应商英文名称:</td>
                    <td>
                        <%--                        <input class="easyui-textbox" id="Name"
                            data-options="validType:'supplierEname',tipPosition:'right',required:true" style="width: 400px" />--%>
                        <input class="easyui-textbox" id="Name"
                            data-options="tipPosition:'bottom',required:true" style="width: 400px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">供应商中文名称:</td>
                    <td>
                        <input class="easyui-textbox" id="ChineseName"
                            data-options="validType:'length[1,150]',tipPosition:'bottom',required:false" style="width: 400px" />
                    </td>
                </tr>
               
               <%-- <tr>
                    <td class="lbl">级别:</td>
                    <td>
                        <input class="easyui-combobox" style="width: 400px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" id="Rank" name="Rank" /></td>
                </tr>--%>
                <tr>
                    <td class="lbl">国家/地区:</td>
                    <td>
                         <input class="easyui-combobox" style="width: 400px;" data-options="valueField:'Code',textField:'Name',limitToList:true,required:true,tipPosition:'bottom'" id="Place" name="Place" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">是否给客户显示:</td>
                    <td>
                         <input type="checkbox" id="IsShowClient" name="IsShowClient" checked="checked" /><label for="IsShowClient" style="margin-right: 30px">是</label>
                    </td>
                </tr>

                <tr>
                    <td class="lbl">备注:</td>
                    <td>
                        <input class="easyui-textbox" id="Summary"
                            data-options="validType:'length[1,400]',tipPosition:'bottom',multiline:true," style="width: 400px; height: 60px;" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>

