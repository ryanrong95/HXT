<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EMSExpressTest.aspx.cs" Inherits="WebApp.Express100.EMSExpressTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
     <link href="../Content/Ccs.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">


        $(function () {



            $('#btnSave').on('click', function () {

                var param = { ID: "", ClientNo: "" };
 
                //提交后台
                MaskUtil.mask();//遮挡层
                $.post('?action=EmsTestAction', { Model: JSON.stringify(param) }, function (res) {
                    MaskUtil.unmask();//关闭遮挡层
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, "info", function () {
                        if (result.success) {
                            console(result);
                        }
                    });
                });
            });



        });

    </script>




</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 8px;">
            <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        </div>
        <div>

            <table class="irtbwrite" style="margin: 25px;">
                <tr>
                    <td class="lbl">公司(个人)名称：
                    </td>
                    <td>
                        <input class="easyui-textbox" style="width: 350px;" id="CompanyName" name="CompanyName" />
                    </td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
