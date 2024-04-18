<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectBank.aspx.cs" Inherits="WebApp.Finance.Declare.SelectBank" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var BankData = eval('(<%=this.Model.BankData%>)');
        var canClickNext = false;

        var IDs = getQueryString("IDs");
        //数据初始化
        $(function () {
            $("#Bank").combobox({
                data: BankData,
                required: true,
                valueField: 'value',
                textField: 'text',
                onChange: function (record) {
                    var ewindow = $.myWindow.getMyWindow("SwapApply2SelectBank");

                    ewindow.cleanIDs = "";

                    var BankID = $('#Bank').combobox('getValue');
                    var BankName = $("#Bank").combobox("getText");
                    ewindow.bankID = BankID;
                    ewindow.bankName = BankName;

                    var IDs = getQueryString("IDs");
                    MaskUtil.mask();

                    canClickNext = false;

                    $.post('?action=CheckLimitCountry', {
                        BankID: BankID,
                        BankName: BankName,
                        IDs: JSON.stringify(IDs),
                    }, function (result) {
                        MaskUtil.unmask();

                        canClickNext = true;

                        var resultJson = JSON.parse(result);
                        if (resultJson.success) {
                            $("#check-limit-country-result").html(resultJson.message);
                            ewindow.cleanIDs = resultJson.ids;
                        } else {
                            $.messager.alert('消息', resultJson.message, 'info', function () {
                                
                            });
                        }

                    });
                },
            });
        });

        //关闭窗口
        function Close() {
            var ewindow = $.myWindow.getMyWindow("SwapApply2SelectBank");

            ewindow.childParam = "onlyClose";
            $.myWindow.close();
        }

        //下一步
        function Next() {
            var ewindow = $.myWindow.getMyWindow("SwapApply2SelectBank");

            if (!$("#form1").form('validate')) {
                return;
            }

            if (!canClickNext) {
                $.messager.alert('提示消息', "校验银行是否存在黑名单未完成", 'info', function () {

                });
                return;
            }

            ewindow.childParam = "openEditSwapAmount";
            $.myWindow.close();
        }

        //验证黑名单国家
        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }
            var BankID = $("#Bank").combobox("getValue");
            var BankName = $("#Bank").combobox("getText");
            var IDs = getQueryString("IDs");
            MaskUtil.mask();
            $.post('?action=CheckLimitCountry', {
                BankID: BankID,
                BankName: BankName,
                IDs: JSON.stringify(IDs),
            }, function (result) {
                var rel = JSON.parse(result);
                 MaskUtil.unmask();
                if (!rel.success && rel.message.indexOf("申请失败") < 0) {
                    //弹框 有黑名单
                    top.$.messager.confirm({
                        width: 560,
                        title: '提示',
                        msg: rel.message + "是否过滤(排除)上述报关单，继续换汇？<br>" + "过滤后金额：" + rel.total,
                        fn: function (success) {
                            if (success) {
                                MaskUtil.mask();
                                if (rel.ids.length > 0) {
                                    $.post('?action=SubmitApply', { BankID: BankID, BankName: BankName, IDs: rel.ids.trim(','), }, function (res) {
                                        MaskUtil.unmask();
                                        var rst = JSON.parse(res);
                                        if (rst.success) {
                                            $.messager.alert('提示', rst.message, 'info', function () {
                                                Close();
                                            });
                                        } else {
                                            $.messager.alert('消息', rst.message);
                                        }
                                    });
                                }
                                else {
                                    $.messager.alert('提示', "无可换汇报关单，已取消", 'info', function () {
                                        Close();
                                    });
                                }
                            }
                        }
                    });
                }
                else {
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            Close();
                        }
                    });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin-top:50px;margin-left:100px">
                <tr>
                    <td>选择银行：</td>
                    <td>
                        <input class="easyui-combobox" id="Bank" name="Bank" panelHeight="120"
                            data-options="required:true,editable:false" style="height: 30px; width: 250px"" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td style="padding-top: 5px;">选择换汇银行，自动过滤换汇黑名单国家的报关单</td>
                </tr>
            </table>
            <table style="margin-top:10px;margin-left:10px;font-size:13px">
                <tbody>
                    <tr>
                        <td><div id="check-limit-country-result" style="word-break: break-all;width: 515px;"></div></td>
                    </tr>
                </tbody>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <%--<a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>--%>
        <a id="btnNext" class="easyui-linkbutton" data-options="iconCls:'icon-edit',width:70," onclick="Next()">下一步</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel',width:70," onclick="Close()">取消</a>
    </div>
</body>
</html>
