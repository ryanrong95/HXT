<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.Vault.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>金库编辑</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        var Admins = eval('(<%=this.Model.AllAdmin%>)');
        var isCommitted = false;//表单是否已经提交标识，默认为false
        $(function () {
            $("#Leader").combobox({
                data: Admins
            });

            //初始化赋值
            if (AllData != null && AllData != "") {
                $("#Name").textbox("setValue", AllData["Name"]);
                $("#Leader").combobox("setValue", AllData["Leader"]);
                $("#Summary").textbox("setValue", AllData["Summary"]);

            }
        });
        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }
        //校验重复提交
        function CheckSubmit() {
            if (isCommitted == false) {
                isCommitted = true;//提交表单后，将表单是否已经提交标识设置为true
                return true;//返回true让表单正常提交
            } else {
                return false;//返回false那么表单将不提交
            }
        }
        //校验金库名称
        function CheckName() {
            //验证表单数据
            var ValutName = $("#Name").textbox("getValue");
            MaskUtil.mask();           
            $.post('?action=CheckValutName', {ID:AllData["ID"],Name:ValutName}, function (res) {
                MaskUtil.unmask();
                var Result = JSON.parse(res)              
                if (Result.success) {
                       Save();
                    } else {
                        $.messager.alert('提示', Result.message);
                    }
            });
        }
        //保存校验
        function Save() {
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }

            var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('ID', AllData["ID"])
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
                        $.messager.alert('消息', res.message, 'info', function () {
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server" method="post" onsubmit="return CheckSubmit()">
            <table id="editTable">
                <tr>
                    <td class="lbl">名称：</td>
                    <td>
                        <input class="easyui-textbox input" id="Name" name="Name" data-options="required:true,validType:'length[1,50]',tipPosition:'bottom',missingMessage:'请输入名称'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">负责人：</td>
                    <td>
                         <input class="easyui-combobox" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false,tipPosition:'bottom',missingMessage:'请选择负责人'" id="Leader" name="Leader"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox input" id="Summary" name="Summary" data-options="required:false,validType:'length[1,500]'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="CheckName()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
