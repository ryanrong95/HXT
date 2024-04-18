<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="SetDefault.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.Tariff.SetDefault" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var hsCode = model.HSCode;
            $("#hsCode").textbox("setValue", hsCode);

            $("#btnHSCode").trigger("click");
            $('#hsCode').textbox('textbox').css('background', '#EBEBE4');
        });

        function getElements() {
            var hsCode = $("#hsCode").val();

            if (hsCode == null || hsCode == '') {
                $.messager.alert('提示', "请输入海关编码", 'info');
                return;
            }

            $.post('?action=getElements', { hsCode: hsCode }, function (data) {
                if (data && data != null && data != '') {
                    $("#elements").empty();
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
                            if ($.trim(name) == 'GTIN' || $.trim(name) == 'CAS') {
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

                    $("#elements").empty();
                    $("#elements").html(tempall);
                    //重新渲染，使js添加的easyui样式生效
                    $.parser.parse('#elements');
                } else {
                    $("#elements").empty();
                    $.messager.alert('提示', "未查询到该海关编码的申报要素", 'info');
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
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
                        <input class="easyui-textbox" id="hsCode" name="hsCode" readonly="readonly" style="width: 250px" />
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
        <div id="elements" style="margin-top: 10px"></div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click"/>
        </div>
    </div>
</asp:Content>

