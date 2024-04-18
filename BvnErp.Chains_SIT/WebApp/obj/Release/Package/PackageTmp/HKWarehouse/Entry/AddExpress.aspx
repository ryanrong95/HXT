<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddExpress.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.AddExpress" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var CarrierData = eval('(<%=this.Model.CarrierData%>)');
        $(function () {
            //设置系统当前时间
            var curr_time = new Date();
            var str = curr_time.getMonth() + 1 + "/";
            str += curr_time.getDate() + "/";
            str += curr_time.getFullYear() + " ";
            str += curr_time.getHours() + ":";
            str += curr_time.getMinutes() + ":";
            str += curr_time.getSeconds();
            $('#ArrivalTime').datebox('setValue', str);

            $('#Carrier').combobox({
                data: CarrierData,
            });
        });

        //新增国际快递
        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var OrderID = getQueryString("OrderID");
            var data = new FormData($('#form1')[0]);
            data.append("OrderID", OrderID);
            $.ajax({
                url: '?action=AddWayBill',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = res.data;
                        $.messager.alert('提示', res.message, 'info', function () {
                            window.parent.$("#WaybillCodes").combobox({
                                disable: false,
                                data: eval(data),
                            })
                            self.parent.$('iframe').parent().window('close');
                        });
                    }
                }
            }).done(function (res) {
            });
        }

        //关闭
        function Close() {
            self.parent.$('iframe').parent().window('close');
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-top: 20px; margin: 0 auto; height: 150px">
                <tr>
                    <td class="lbl">到港日期：</td>
                    <td>
                        <input class="easyui-datebox input" id="ArrivalTime" name="ArrivalTime"
                            data-options="required:true,tipPosition:'bottom',height:26,width:200,missingMessage:'请输入到港时间'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">快递公司：</td>
                    <td>
                        <input class="easyui-combobox" id="Carrier" name="Carrier"
                            data-options="required:true,validType:'length[1,50]',height:26,width:200,tipPosition:'bottom',missingMessage:'请选择快递公司'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运单号：</td>
                    <td>
                        <input class="easyui-textbox input" id="WaybillCode" name="WaybillCode"
                            data-options="required:true,validType:'length[1,50]',height:26,width:200,tipPosition:'bottom',missingMessage:'请输入运单号'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
