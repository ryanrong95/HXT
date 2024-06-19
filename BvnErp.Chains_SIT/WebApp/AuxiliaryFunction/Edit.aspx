<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.AuxiliaryFunction.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="http://fix.szhxd.net/My/Scripts/area.data.js"></script>
    <script src="http://fix.szhxd.net/My/Scripts/areacombo.js"></script>
    <script src="../Scripts/Ccs.js"></script>
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script type="text/javascript">
        var ExpressCompanyData = eval('(<%=this.Model.ExpressCompanyData%>)');
        var payType = eval('(<%=this.Model.PayType%>)');
        var ExpressData = eval('(<%=this.Model.ExpressData%>)');
        var id = getQueryString("ID");

      <%--  var ClientConsigneeID = '<%=this.Model.ClientConsigneeID%>';
        if (ClientConsigneeID != '') {
            ClientConsigneeData = eval('(<%=this.Model.ClientConsigneeData != null ? this.Model.ClientConsigneeData:""%>)');
        }--%>

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
                    if (data.findIndex(x=>x.Value == ExpressData.ExpressCompany) == -1) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Text == "顺丰") {
                            $('#ExpressName').combobox('select', data[i].Value);
                        }
                        GetTypeByName()
                        $('#ExpressType').combobox('setValue', null);
                    }
                    }
                },
                onChange: function (record) {
                    $('#ExpressType').combobox('setValue', null);
                    GetTypeByName();
                },
            });


            function GetTypeByName() {
                var selectedValue = $("#ExpressName").combobox("getValue");
                 var selectedText = $("#ExpressName").combobox("getText");
                if (selectedValue != null) {
                    $.post('?action=ExpressSelect',
                        { ID: selectedText },
                        function (data) {
                            data = eval(data);
                            if (ExpressData && data.findIndex(f => f.Value == ExpressData.ExpressType) == -1) {
                                $('#ExpressType').combobox('setValue', null); // 此代码解决，每次下拉列表数据更新时，在还没有更新完成之前， 默认选择项就自动匹配的问题
                            }
                            $("#ExpressType").combobox("loadData", data);
                            bindType(data);
                        });
                }
            }

            function bindType(data) {

                if (data.length > 0
                    && ExpressData
                    && ExpressData.ExpressType
                    && data.findIndex(f => f.Value == ExpressData.ExpressType) != -1) {
                    $('#ExpressType').combobox('setValue', ExpressData.ExpressType);
                }
            }
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

                // GetTypeByName();

                //$("#ExpressType").combobox("setValue", ExpressData.ExpressType);
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
                debugger;
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

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table class="irtbwrite" id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">收货单位：</td>
                    <td>
                        <input class="easyui-textbox" id="ReceiverComp" data-options="validType:'length[1,200]',tipPosition:'bottom',height:26,width:210" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收件人：</td>
                    <td>
                        <input class="easyui-textbox" id="Receiver" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',height:26,width:210" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">收件人电话：</td>
                    <td>
                        <input class="easyui-textbox" id="ReveiveMobile" data-options="required:true,tipPosition:'bottom',height:26,width:210," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收件地址：</td>
                    <td>
                        <table class="irtbaddress">
                            <tr>
                                <td>
                                    <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:360" id="ReveiveAddress" name="ReveiveAddress"></div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">寄件单位：</td>
                    <td>
                        <input class="easyui-textbox" id="SenderComp" data-options="validType:'length[1,200]',tipPosition:'bottom',height:26,width:210" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">寄件人：</td>
                    <td>
                        <input class="easyui-textbox" id="Sender" data-options="required:true,validType:'length[1,150]',tipPosition:'bottom',height:26,width:210" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">寄件人电话：</td>
                    <td>
                        <input class="easyui-textbox" id="SenderMobile" data-options="required:true,tipPosition:'bottom',height:26,width:210," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">寄件地址：</td>
                    <td>
                        <table class="irtbaddress">
                            <tr>
                                <td>
                                    <div class="easyui-area" data-options="country:'中国',newline:true,newlinewidth:360" id="SenderAddress" name="SenderAddress"></div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">快递公司：</td>
                    <td>
                        <input class="easyui-combobox" id="ExpressName" name="ExpressName" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">快递方式：</td>
                    <td>
                        <input class="easyui-combobox" id="ExpressType" name="ExpressType" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付费方式：</td>
                    <td>
                        <input class="easyui-combobox" id="PayType" name="PayType" data-options="required:true,height:26,width:210,valueField:'Key',textField:'Value'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'">保存</a>
        <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'">取消</a>
    </div>
</body>
</html>
