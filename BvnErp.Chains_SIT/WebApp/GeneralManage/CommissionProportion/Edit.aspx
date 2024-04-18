<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.GeneralManage.CommissionProportion.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>提成比例</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />--%>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        var isCommitted = false;//表单是否已经提交标识，默认为false
        $(function () {
            //初始化赋值
            if (AllData != null && AllData != "") {
                $("#RegeisterMonth").textbox("setValue", AllData["RegeisterMonth"]);
                $("#CommissionProportion").textbox("setValue", AllData["CommissionProportion"]);
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
        //保存校验
        function Save() {
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }
            var r = /^\+?[1-9][0-9]*$/;
            var RegeisterMonth = $('#RegeisterMonth').textbox('getValue');
            if (r.test(RegeisterMonth)) {
            } else {
                $.messager.alert('消息', '月数为正整数');
                return;
            }
            var regu = "^([0-9]*[.0-9])$";
            var re = new RegExp(regu);
            var CommissionProportion = $('#CommissionProportion').textbox('getValue');
            if (CommissionProportion == "" || isNaN(CommissionProportion) || isNaN(RegeisterMonth)) {//进行数字校验，如果不是数字填出对话框进行提示
                $.messager.alert('消息', '请填写数字');
            }
            if (CommissionProportion >= 1) {
                $.messager.alert('消息', '提成比例要小于1');
                return;
            }
            if (CommissionProportion.search(re) != -1) {
                $.messager.alert('消息', '提成比例为小数');
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('ID', AllData["ID"])
            }
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
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
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">注册月数：</td>
                    <td>
                        <input class="easyui-textbox" id="RegeisterMonth" name="RegeisterMonth" data-options="required:true,height:30,width:250,validType:'length[1,50]'" />
                        <p>请输入月数，月数为正整数</p>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">提成比例：</td>
                    <td>
                        <input class="easyui-textbox" id="CommissionProportion" name="CommissionProportion" data-options="required:true,height:30,width:250,validType:'length[1,5]'" />
                        <p>
                            请输入小数，不要输入百分比
                        </p>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="required:false,height:30,width:250,validType:'length[1,500]',tipPosition:'bottom',missingMessage:'请输入备注'" />
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
