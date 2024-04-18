<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintKdd.aspx.cs" Inherits="WebApp.AuxiliaryFunction.PrintKdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="http://gerpfixed.for-ic.net/My/Scripts/area.data.js"></script>
    <script src="http://gerpfixed.for-ic.net/My/Scripts/areacombo.js"></script>
    <script src="../Scripts/Ccs.js"></script>
    <link href="../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script src="../Scripts/jquery.jqprint-0.3.js"></script>
    <script src="../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script type="text/javascript">
        var ExpressCompanyData = eval('(<%=this.Model.ExpressCompanyData%>)');
        var payType = eval('(<%=this.Model.PayType%>)');
        var ExpressData = eval('(<%=this.Model.ExpressData%>)');
        var id = getQueryString("ID");

        $(function () {
            $('#PayType').combobox({
                data: payType,
                onLoadSuccess: function (data) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Value == "月结") {
                            $('#PayType').combobox('select', data[i].Key);
                        }
                    }
                }
            });
            $('#ExpressName').combobox({
                data: ExpressCompanyData,
                onLoadSuccess: function (data) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Text == "顺丰") {
                            $('#ExpressName').combobox('select', data[i].Value);
                        }
                    }
                },
                onSelect: function (record) {
                    $.post('?action=ExpressSelect', { name: record.Text }, function (data) {
                        console.log(data);
                        //更新快递方式
                        data = eval(data);
                        $('#ExpressType').combobox({
                            data: data,
                        });
                        if (ExpressData.ExpressType != null)
                            $("#ExpressType").combobox("setValue", ExpressData.ExpressType);
                        //else
                        //    //默认选择第一行
                        //    $('#ExpressType').combobox('setValue', data[0].Value);
                    })
                },
            });

            $('#WaybillCode').textbox({
                readonly: true,
            });

            if (id != '') {
                $("#ReceiverComp").textbox("setValue", ExpressData.ReceiverComp);
                $("#Receiver").textbox("setValue", ExpressData.Receiver);
                $("#ReveiveMobile").textbox("setValue", ExpressData.ReveiveMobile);
                $("#ReveiveAddress").area("setValue", ExpressData.ReveiveAddress);
                $("#SenderComp").textbox("setValue", ExpressData.SenderComp);
                $("#Sender").textbox("setValue", ExpressData.Sender);
                $("#SenderMobile").textbox("setValue", ExpressData.SenderMobile);
                $("#SenderAddress").area("setValue", ExpressData.SenderAddress);
                $("#ExpressName").combobox("setValue", ExpressData.ExpressCompany);
                $("#ExpressType").combobox("setValue", ExpressData.ExpressType);
                $("#PayType").combobox("setValue", ExpressData.PayType);
            }

            $('#btnSave').on('click', function () {
                if (!$("#form1").form('validate')) {
                    return;
                }

                if ($("#SenderAddress").area("getValue").split(' ').length < 3) {
                    $.messager.alert("消息", "请选择寄件地址");
                    return;
                }
                if ($("#ReveiveAddress").area("getValue").split(' ').length < 3) {
                    $.messager.alert("消息", "请选择收件地址");
                    return;
                }
                var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
                values["ID"] = id;
                //提交后台
                $.post('?action=Save', { Model: JSON.stringify(values) }, function (res) {
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });

        });

        //生成快递面单
        function Print() {
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }

            if ($("#SenderAddress").area("getValue").split(' ').length < 3) {
                $.messager.alert("消息", "请选择寄件地址");
                return;
            }
            if ($("#ReveiveAddress").area("getValue").split(' ').length < 3) {
                $.messager.alert("消息", "请选择收件地址");
                return;
            }
            var values = FormValues("form1");//可继续添加其它参数:values[id] = $this.val();
            ////提交后台
            $.post('?action=GenerateExpress', { Model: JSON.stringify(values) }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {

                    $('#WaybillCode').textbox('setValue', result.LogisticCode);
                    $("#expresskdd").html("");
                    $("#expresskdd").html(result.PrintTemplate);
                }
                else {
                    $.messager.alert('消息', result.message);
                }
            });
        }

        //打印快递面单
        function Printkdd() {
            var waybillCode = $('#WaybillCode').textbox('getValue');
            if (waybillCode == "" || waybillCode == null) {
                $.messager.alert('消息', "请先生成快递运单");
                return;
            }
            event.preventDefault();
            $("#expresskdd").jqprint();
        }

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content" style="margin: 0 auto; overflow: auto; width: 650px; height: 550px;">
        <div>
            <form id="form1" runat="server">
                <table id="editTable">
                    <tr>
                        <td class="lbl">收货单位：</td>
                        <td>
                            <input class="easyui-textbox" id="ReceiverComp" data-options="validType:'length[1,200]',tipPosition:'bottom'" />
                        </td>
                        <td class="lbl">运单号：</td>
                        <td>
                            <input class="easyui-textbox" id="WaybillCode" name="WaybillCode" data-options="validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">收件人：</td>
                        <td>
                            <input class="easyui-textbox" id="Receiver" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" />
                        </td>
                        <td class="lbl">收件人电话：</td>
                        <td>
                            <input class="easyui-textbox" id="ReveiveMobile" data-options="required:true,tipPosition:'bottom'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">收件地址：</td>
                        <td colspan="4">
                            <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:360" id="ReveiveAddress" name="ReveiveAddress"></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">寄件单位：</td>
                        <td>
                            <input class="easyui-textbox" id="SenderComp" data-options="validType:'length[1,200]',tipPosition:'bottom'" />
                        </td>
                        <td class="lbl">寄件人：</td>
                        <td>
                            <input class="easyui-textbox" id="Sender" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">寄件人电话：</td>
                        <td>
                            <input class="easyui-textbox" id="SenderMobile" data-options="required:true,tipPosition:'bottom'" />
                        </td>
                        <td class="lbl">付费方式：</td>
                        <td>
                            <input class="easyui-combobox" id="PayType" name="PayType" data-options="required:true,valueField:'Key',textField:'Value'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">寄件地址：</td>
                        <td colspan="4">

                            <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:360" id="SenderAddress" name="SenderAddress"></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">快递公司：</td>
                        <td>
                            <input class="easyui-combobox" id="ExpressName" name="ExpressName" data-options="required:true,valueField:'Value',textField:'Text'" />
                        </td>
                        <td class="lbl">快递方式：</td>
                        <td>
                            <input class="easyui-combobox" id="ExpressType" name="ExpressType" data-options="required:true,valueField:'Value',textField:'Text'"  />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style="text-align: left">
                            <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-ok'" onclick="Print()">生成快递面单</a>
                            <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-print'" onclick="Printkdd()">打印快递面单</a>
                        </td>
                    </tr>
                </table>
            </form>
            <hr style="margin: 3px 0" />
            <div style="padding-bottom: 20px; margin: 2px">
                <div id="expresskdd" style="position: relative;">
                </div>
            </div>
        </div>
    </div>
</body>
</html>
