<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditCaseNumber.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.EditCaseNumber" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改箱号信息</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {
            //设置系统当前时间
            var curr_time = new Date();
            var str = curr_time.getMonth() + 1 + "/";
            str += curr_time.getDate() + "/";
            str += curr_time.getFullYear() + " ";
            str += curr_time.getHours() + ":";
            str += curr_time.getMinutes() + ":";
            str += curr_time.getSeconds();
            $('#NewPackingDate').datebox('setValue', str);
        });

        function Close() {
            $.myWindow.close();
        }



        //修改箱号
        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }

            var BoxIndex = $("#NewCaseNumber").textbox('getValue');
            //if (Number(Count) > 1 && BoxIndex.indexOf("-") != -1)
            //{
            //      $.messager.alert("消息", "输入箱号为连续箱号，只能勾选一个装箱产品");
            //        return;

            //}
            if (BoxIndex.split("-").length - 1 > 1 || BoxIndex.substring(0, 1) == '-' || BoxIndex.split("-")[0].substring(0, 3) != 'HXT' || (BoxIndex.split("-").length > 1 && BoxIndex.split("-")[1].substring(0, 3) != 'HXT')) {
                $("#NewCaseNumber").focus();
                $.messager.alert("消息", "请输入正确的装箱箱号");
                return;
            }
            var arry = BoxIndex.split('-');
            if (Number(arry[0].replace("HXT", "")) < 1 || (BoxIndex.split("-").length > 1 && Number(arry[1].replace("HXT", "")) < 1) || (BoxIndex.split("-").length > 1 && Number(arry[0].replace("HXT", "")) >= Number(arry[1].replace("HXT", "")))) {
                $("#NewCaseNumber").focus();
                $.messager.alert("消息", "请输入正确的装箱箱号");
                return;
            }

            var data = new FormData($('#form1')[0]);
            var OrderID = getQueryString('OrderID');
            var PackingID = getQueryString("PackingID");
            data.append("OrderID", OrderID);
            data.append("PackingID", PackingID);
            $.ajax({
                url: '?action=ChangeCaseNumber',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (rel) {
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            $.myWindow.close();
                    }
                    });
                }


            }).done(function (res) {
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="margin: 0px 20px; height: 120px">
                <tr>
                    <td class="lbl">装箱日期：</td>
                    <td>
                        <input class="easyui-datebox" id="NewPackingDate" name="NewPackingDate"
                            data-options="required:true,height:26,width:200" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">新箱号：</td>
                    <td>
                        <input class="easyui-textbox" id="NewCaseNumber" name="NewCaseNumber"
                            data-options="required:true,validType:'length[1,50]',height:26,width:200,missingMessage:'请输入新箱号'" />
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
