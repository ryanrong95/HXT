<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddWaybill.aspx.cs" Inherits="WebApp.Finance.Invoice.AddWaybill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>打印发票运单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-barcode.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <style>
        .kddiv {
            position: relative;
        }
    </style>
    <style>
        * {
            margin: 0;
            padding: 0;
            font-family: '黑体', 'Arial' !important;
        }



        .clearfix:after {
            content: '\20';
            display: block;
            height: 0;
            clear: both;
        }
    </style>

    
    <script type="text/javascript">
        var ExpressCompanyData = eval('(<%=this.Model.ExpressCompanyData%>)');

        $(function () {
            var invoiceNoticeIDs = getQueryString('IDs');
            //初始化快递信息
            $('#ExpressName').combobox({
                data: ExpressCompanyData,
                onLoadSuccess: function (data) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Text == "顺丰") {
                            $('#ExpressName').combobox('select', data[i].Value);
                        }
                    }
                }
            });

            $('#InvoiceNotice').textbox({
                readonly: true,
                multiline: true,
            });
            $('#InvoiceNotice').textbox("setValue", (invoiceNoticeIDs + ''));
        });

        //保存运单数据
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证运单号
            if ($('#WaybillCode').textbox('getValue') == "") {
                $.messager.alert('消息', "运单号为空，保存失败");
                return;
            }
            var data = new FormData($('#form1')[0]);
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
                            $.myWindow.close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }

    </script>

</head>
<body>
    <form id="form1" runat="server" method="post">
                <div>
                    <table style="margin: 20px; line-height: 30px">
                        <tr>
                            <td class="lbl">开票通知：</td>
                            <td>
                                <input class="easyui-textbox" id="InvoiceNotice" name="InvoiceNotice" data-options="required:true,height:56,width:410" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">快递公司：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressName" name="ExpressName" data-options="required:true,height:26,width:410,valueField:'Value',textField:'Text'" />
                            </td>
                        </tr>
       
                        <tr>
                            <td class="lbl">运单号：</td>
                            <td>
                                <input class="easyui-textbox" id="WaybillCode" name="WaybillCode" data-options="height:26,width:410,validType:'length[1,50]'" />
                            </td>
                        </tr>
                        <tr>
                            <td> &nbsp;</td>
                        </tr>
                        <tr>
                            <td style="text-align: center" colspan="2">
                                <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-ok'" onclick="Save()">保存</a>
                            </td>
                        </tr>
                    </table>
                </div>
            </form>
</body>
</html>
