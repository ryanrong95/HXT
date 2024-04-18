<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.PvCustom_ConsultClassify.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>咨询产品编辑</title>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <script type="text/javascript">
       
        var isCommitted = false;//表单是否已经提交标识，默认为false

        $(function () {
            $("#Currency").textbox("setValue", "USD");
        })
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
            if (!CheckSubmit()) {
                 $.messager.alert('提示', '不能重复提交');
                return;
            }

            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            //if (AllData != null) {
            //    data.append('ID', AllData["ID"])
            //    data.append('BankAccount', AllData["BankAccount"])
            //}
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
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">               
                <tr>
                    <td class="lbl">型号：</td>
                    <td>
                        <input class="easyui-textbox" id="Model" name="Model" data-options="required:true,height:28,width:250,validType:'length[1,250]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">品牌：</td>
                     <td>
                        <input class="easyui-textbox" id="Brand" name="Brand" data-options="required:true,height:28,width:250,validType:'length[1,250]'" />
                    </td>           
                </tr>
                <tr>
                    <td class="lbl">数量：</td>
                   <td>
                        <input class="easyui-numberbox" id="Qty" name="Qty" data-options="required:true,height:28,width:250,validType:'length[1,150]',precision:2" />
                    </td>     
                </tr>
                <tr>                    
                    <td class="lbl">单价：</td>
                    <td>
                        <input class="easyui-numberbox" id="UnitPrice" name="UnitPrice" data-options="required:true,height:28,width:250,validType:'length[1,150]',precision:4" />
                    </td>      
                </tr>
                <tr>
                    <td class="lbl">币种：</td>
                    <td>
                        <input class="easyui-textbox" id="Currency" name="Currency" data-options="required:true,height:28,width:250,validType:'length[1,250]'" />
                    </td>                  
                </tr>
                <tr>
                    <td class="lbl">自定义编码：</td>
                    <td>
                       <input class="easyui-textbox" id="ProductUniqueCode" name="ProductUniqueCode" data-options="height:28,width:250,validType:'length[1,250]'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" id="SaveButton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" id="CancelButton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>     
    </div>

</body>
</html>
