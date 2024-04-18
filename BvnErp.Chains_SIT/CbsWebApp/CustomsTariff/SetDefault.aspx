<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetDefault.aspx.cs" Inherits="Needs.Cbs.WebApp.CustomsTariff.SetDefault" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Cbs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            var HSCode = getQueryString('HSCode');
            $("#HSCode").textbox("setValue", HSCode);

            $("#btnHSCode").trigger("click");
            $(".dlg-buttons").hide();
            SetReadonlyBgColor('easyui-textbox', '#EBEBE4');
        });

        function getElements() {
            var HSCode = $("#HSCode").val();

            if (HSCode == null || HSCode == '') {
                $.messager.alert('提示', "请输入海关编码", 'info');
                return;
            }

            $.post('?action=GetElements', { HSCode: HSCode }, function (data) {
                if (data && data != null && data != '') {
                    $("#Elements").empty();
                    var array = data.DeclareElements.split(';');
                    var arrayIndex = array.length - 1;

                    var temp = '';
                    var ismust = 1;
                    var value = '';
                    var otherDefault = '';

                    var htmlArray = new Array();
                    var looptimes;

                    for (var i = 0; i <= arrayIndex; i++) {
                        if (array[i] != null && array[i] != '') {
                            var name = array[i].replace((i + 1).toString(), "").replace(":", "");
                            var nametooltip = name
                            if (name.length >= 6) {
                                nametooltip = name.substring(0, 5) + "...";
                            }
                            temp += '<td class="lbl"><label class="lbl"  title="' + name + '">' + nametooltip;
                            if (name == "GTIN" || name == "CAS") {
                                ismust = 0;
                            } else {
                                temp += '<span style="color:red">*</span> ';
                            }

                            $.each(data.ElementsDefaults, function (index, element) {
                                if (element.ElementName == name) {
                                    value = element.DefaultValue;
                                }
                                if (element.ElementName == '其他') {
                                    otherDefault = element.DefaultValue;
                                }
                            });

                            temp += '</label></td>';
                            temp += '<td>';
                            temp += '<input class="easyui-textbox" id="' + name + '" name="' + name + '" value="' + value + '" style="width: 250px"/>';
                            temp += '</td>';
                        }

                        value = "";
                        ismust = 1;
                        htmlArray[i] = temp;
                        temp = "";
                    }

                    temp += '<td class="lbl"><label class="lbl" title="其他">其他<span style="color:red">*</span></label></td>';
                    temp += '<td>';
                    temp += '<input class="easyui-textbox" id="其他" name="其他" value="' + otherDefault + '" style="width: 250px"/>';
                    temp += '</td>';

                    htmlArray[array.length] = temp;

                    var tempall = '<table style="margin: 0 auto; width: 90%; border-spacing: 0px 5px;">' +
                        '<tr>' +
                        '<th style="width:10%"></th>' +
                        '<th style="width:20%"></th>' +
                        '<th style="width:10%"></th>' +
                        '<th style="width:20%"></th>' +
                        '</tr>';

                    if (htmlArray.length % 2 == 0) {
                        looptimes = htmlArray.length;
                        for (var j = 0; j < looptimes; j = j + 2) {
                            tempall += '<tr>';
                            tempall += htmlArray[j] + htmlArray[j + 1];
                            tempall += '</tr>';
                        }
                    } else {
                        looptimes = htmlArray.length - 1;
                        for (var j = 0; j < looptimes; j = j + 2) {
                            tempall += '<tr>';
                            tempall += htmlArray[j] + htmlArray[j + 1];
                            tempall += '</tr>';
                        }

                        tempall += '<tr>';
                        tempall += htmlArray[htmlArray.length - 1];
                        tempall += '</tr>';
                    }

                    tempall += '</table>'

                    $("#Elements").empty();
                    $("#Elements").html(tempall);
                    //重新渲染，使js添加的easyui样式生效
                    $.parser.parse('#Elements');
                    $(".dlg-buttons").css('display', 'block');
                } else {
                    $("#Elements").empty();
                    $.messager.alert('提示', "未查询到该海关编码的申报要素", 'info');
                    $(".dlg-buttons").hide();
                }
            })
        }

        //保存默认值
        function Save() {
            var values = FormValues("form1");

            $.post('?action=SaveDefaults', { Model: encodeURI(JSON.stringify(values)).replace(/\+/g, '%2B') }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert('提示', result.message, 'info', function () {
                        Close();
                    });
                } else {
                    $.messager.alert('提示', result.message);
                }
            });
        }

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <div>
                <table style="margin: 0 auto; width: 90%; border-spacing: 0px 5px;">
                    <tr>
                        <th style="width: 10%"></th>
                        <th style="width: 20%"></th>
                        <th style="width: 10%"></th>
                        <th style="width: 20%"></th>
                    </tr>
                    <tr>
                        <td class="lbl">海关编码</td>
                        <td>
                            <input class="easyui-textbox" id="HSCode" name="HSCode" readonly="readonly" style="width: 250px" />
                        </td>
                        <td class="lbl"></td>
                        <td>
                            <input readonly="readonly" style="width: 250px; border: none" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <a id="btnHSCode" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="display: none" onclick="getElements()">申报要素查询</a>
                        </td>
                    </tr>
                </table>
            </div>
            <hr style="margin-top: 10px" />
            <div id="Elements" style="margin-top: 10px"></div>
        </form>
    </div>

    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>

