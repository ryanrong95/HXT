<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetControl.aspx.cs" Inherits="WebApp.Client.New.SetControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
</head>
<script type="text/javascript">

    var ClientID = '<%=this.Model.ClientID%>';
    var IsDownloadDecTax = '<%=this.Model.IsDownloadDecTax%>';
    var DecTaxExtendDate = '<%=this.Model.DecTaxExtendDate%>';
    var IsApplyInvoice = '<%=this.Model.IsApplyInvoice%>';
    var InvoiceExtendDate = '<%=this.Model.InvoiceExtendDate%>';


    //数据初始化
    $(function () {

        //控件赋值
        if (IsDownloadDecTax == 'true') {
            //不限制，可以下载
            $("input[name='dectaxRD'][value=1]").attr("checked", true);
            //$("input[name='dectaxRD'][value=0]").attr("checked", "");
            $("td[name=tdDecDate]").css("display", "none");
            $("#dectaxDate").datebox("setValue", "");

        }
        else {
            //限制，不能下载
            $("input[name='dectaxRD'][value=0]").attr("checked", true);
            //$("input[name='dectaxRD'][value=1]").attr("checked", "");
            $("td[name=tdDecDate]").css("display", "table-cell");
            $("#dectaxDate").datebox("setValue", DecTaxExtendDate);
        }

        if (IsApplyInvoice == 'true') {
            //不限制，可以下载
            $("input[name='invoiceRD'][value=1]").attr("checked", true);
            //$("input[name='invoiceRD'][value=0]").attr("checked", "");
            $("td[name=tdInvoice]").css("display", "none");
            $("#invoiceDate").datebox("setValue", "");
        }
        else {
            //限制，不能下载
            $("input[name='invoiceRD'][value=0]").attr("checked", true);
            //$("input[name='invoiceRD'][value=1]").attr("checked", "");
            $("td[name=tdInvoice]").css("display", "table-cell");
            $("#invoiceDate").datebox("setValue", InvoiceExtendDate);
        }

        $("#summary").textbox("setValue", "");

        //控件事件
        //限制海关发票radiobutton的点击事件
        $("input[name=dectaxRD]").click(function () {
            var value = $(this).val();
            if (value == 0) {
                //限制
                $("td[name=tdDecDate]").css("display", "table-cell");
                //$('#dectaxDate').datebox('textbox').validatebox('options').required = true;
            }
            else {
                //不限制
                $("td[name=tdDecDate]").css("display", "none");
            }
        });

        //限制申请开票radiobutton的点击事件
        $("input[name=invoiceRD]").click(function () {
            var value = $(this).val();
            if (value == 0) {
                //限制
                $("td[name=tdInvoice]").css("display", "table-cell");
            }
            else {
                //不限制
                $("td[name=tdInvoice]").css("display", "none");
            }
        });

        //列表初始化
        $('#operatelog').myDatagrid({
            //actionName: 'GetOperateLogs',
            fitColumns: true,
            fit: true,
            scrollbarSize: 0,
            rownumbers: false,
            nowrap: false,
            pagination: false
        });
    });


    function SaveControlEdit() {

        //保存数据库

        MaskUtil.mask();//遮挡层
        var formData = new FormData($('#form1')[0]);

        formData.append("ClientID", ClientID);
        formData.append("dectaxDate", $("#dectaxDate").datebox("getValue"));
        formData.append("invoiceDate", $("#invoiceDate").datebox("getValue"));
        $.ajax({
            url: '?action=SavePayExControl',
            type: 'POST',
            data: formData,
            dataType: 'JSON',
            cache: false,
            processData: false,
            contentType: false,
            success: function (res) {
                MaskUtil.unmask();
                $.messager.alert('提示', res.message, 'info',function (r) {
                    
                        //执行目标操作
                        $.myWindow.close();
                    
                });
            }
        });
    }

    function Cancel() {
        $.myWindow.close();
    }

</script>

<body>
    <form id="form1" runat="server">
        <!------------------------------------------------------------ 确认框 html Begin ------------------------------------------------------------>


        <div data-toggle="topjui-radio">
            <table>
                <tr style="height: 50px;">
                    <td>
                        <span class="lbl" style="padding-left: 20px;">限制海关发票：</span>
                    </td>
                    <td>
                        <input type="radio" name="dectaxRD" value="0" id="dectaxRD_xianzhi" class="radio" checked="checked" /><label for="dectaxRD_xianzhi" style="margin-right: 30px">限制</label>
                        <input type="radio" name="dectaxRD" value="1" id="dectaxRD_buxianzhi" class="radio" /><label for="dectaxRD_buxianzhi">不限制</label>
                    </td>
                    <td style="width: 100px;" name="tdDecDate">
                        <span class="lbl" style="padding-left: 20px;">宽限日期：</span>
                    </td>
                    <td name="tdDecDate">
                        <input type="text" class="easyui-datebox" style="width: 180px;" id="dectaxDate" data-options="editable:false" />
                    </td>
                </tr>
                <tr style="height: 50px;">
                    <td>
                        <span class="lbl" style="padding-left: 20px;">限制申请开票：</span>
                    </td>
                    <td>
                        <input type="radio" name="invoiceRD" value="0" id="invoiceRD_xianzhi" class="radio" checked="checked" /><label for="invoiceRD_xianzhi" style="margin-right: 30px">限制</label>
                        <input type="radio" name="invoiceRD" value="1" id="invoiceRD_buxianzhi" class="radio" /><label for="invoiceRD_buxianzhi">不限制</label>
                    </td>
                    <td name="tdInvoice">
                        <span class="lbl" style="padding-left: 20px;">宽限日期：</span>
                    </td>
                    <td name="tdInvoice">
                        <input type="text" class="easyui-datebox" style="width: 180px;" id="invoiceDate" data-options="editable:false" />
                    </td>
                </tr>
                <tr style="height: 80px;">
                    <td>
                        <span class="lbl" style="padding-left: 20px;">备注：</span>
                    </td>
                    <td colspan="3">
                        <input class="easyui-textbox" id="summary" name="summary" style="width: 600px; height: 80px;" data-options="multiline:true" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="height: 335px; width: 798px;">
            <table id="operatelog" title="管控记录" data-options="fitColumns: true,fit: true,scrollbarSize: 0,rownumbers: false,nowrap: false, pagination: false">
                <thead>
                    <tr>
                        <th data-options="field:'AdminName',align:'center'" style="width: 10%">操作人</th>
                        <th data-options="field:'CreateDate',align:'center'" style="width: 20%">操作时间</th>
                        <th data-options="field:'Summary',align:'left'" style="width: 70%">操作备注</th>

                    </tr>
                </thead>
            </table>
        </div>


        <div id="dlg-buttons" data-options="region:'south',border:false">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="SaveControlEdit()">保存</a>
            <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">关闭</a>
        </div>
        <!------------------------------------------------------------ 确认框 html End ------------------------------------------------------------>

    </form>
</body>
</html>
