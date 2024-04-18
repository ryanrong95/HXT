<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ElementsQuery.aspx.cs" Inherits="WebApp.Classify.ElementsQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>申报要素查询</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            var hsCode = window.parent.$('#HSCode').combogrid('getText');
            var model = window.parent.$('#Model').combogrid('getText');
            var manufacture = window.parent.$('#Manufacturer').textbox('getValue');
            $('#HSCode').textbox('setValue', hsCode);
            $('#Model').textbox('setValue', model);
            $('#Manufacturer').textbox('setValue', manufacture);

            $('#btnHSCode').trigger('click');
            $('#divSave').css('display', 'none');
        });

        //获取申报要素
        function GetElements() {
            var hsCode = $('#HSCode').val();
            var origin = window.parent.$('#Origin').textbox('getValue');

            //从归类界面打开申报要素查询窗口时,判断该海关编码与之前归类过的海关编码是否发生变更
            var isHSCodeChanged = getQueryString('IsHSCodeChanged') == 'false' ? false : true;
            //在申报要素查询界面，输入新的海关编码进行查询时，判断该海关编码与归类界面输入的海关编码是否变更
            var parentHSCode = window.parent.$('#HSCode').textbox('getValue');
            if (hsCode != parentHSCode) {
                isHSCodeChanged = true;
            }

            //根据海关编码和原产地查询申报要素
            $.post('?action=GetElements', { HSCode: hsCode, Origin: origin }, function (data) {
                if (data && data != null && data != '') {  
                    window.parent.$('#TariffName').combogrid('setValue', data.TariffName);
                    //if (window.parent.$('#TariffName').combogrid('getValue') == null || window.parent.$('#TariffName').combogrid('getValue') == '') {
                        
                    //}                    
                    window.parent.$('#TariffRate').combogrid('setValue', data.TariffRate);
                    window.parent.$('#ValueAddTaxRate').combogrid('setValue', data.ValueAddTaxRate);
                    window.parent.$('#Unit1').textbox('setValue', data.Unit1);
                    window.parent.$('#Unit2').textbox('setValue', data.Unit2);
                    window.parent.$('#CIQCode').combobox('setValue', data.CIQCode);
                    //系统判断是否需要商检
                    if (data.InspectionCodeFlag) {
                        window.parent.SetIsSysInspection(true);
                    } else {
                        window.parent.SetIsSysInspection(false);
                    }
                    //系统判断是否需要原产地证明
                    if (data.RegulatoryCode != null && data.RegulatoryCode.toUpperCase().indexOf('Y') != -1) {
                        window.parent.SetIsSysOriginProof(true);
                    } else {
                        window.parent.SetIsSysOriginProof(false);
                    }

                    $('#Elements').empty();
                    var array = data.DeclareElements.split(';');
                    var arrayIndex = array.length - 1;

                    var isSetDefalut = $('#Elements', window.parent.document).val() == undefined;
                    var ElementsArrayText = isSetDefalut ? '' : $('#Elements', window.parent.document).val();
                    var ElementsArray = isSetDefalut ? '' : $('#Elements', window.parent.document).val().split('|');

                    var temp = '';
                    var value = '';
                    var isRequired = true;
                    var otherDefault = '';

                    var htmlArray = new Array();
                    var looptimes;

                    for (var i = 0; i <= arrayIndex ; i++) {
                        if (array[i] != null && array[i] != '') {
                            var name = array[i].replace((i + 1).toString(), '').replace(':', '');
                            var nametooltip = name
                            if (name.length >= 6) {
                                nametooltip = name.substring(0, 5) + '...';
                            }
                            temp += '<td class="lbl"><label class="lbl"  title="' + name + '">' + nametooltip;
                            if ($.trim(name) == 'GTIN' || $.trim(name) == 'CAS') {
                                isRequired = false;
                            } else {
                                temp += '<span style="color:red">*</span>';
                            }

                            //设置默认值
                            if (ElementsArrayText == '' || isHSCodeChanged) {
                                $.each(data.ElementsDefaults, function (index, element) {
                                    if (element.ElementName == name) {
                                        value = element.DefaultValue;
                                    }
                                    if (element.ElementName == '其他') {
                                        otherDefault = element.DefaultValue;
                                    }
                                });
                            } else {
                                value = ElementsArray[i];
                                otherDefault = ElementsArray[i + 1];
                            }

                            //特殊值设置
                            if (ElementsArrayText == '' || isHSCodeChanged) {
                                if (name == '品牌') {
                                    value = $('#Manufacturer').val() + '牌';
                                }
                                if (name == '型号') {
                                    value = '型号:' + $('#Model').val();
                                }

                                if (name == '品牌类型') {
                                    value = '4';
                                }
                                if (name == '出口享惠情况') {
                                    value = '3';
                                }
                            }

                            temp += '</label></td>';
                            temp += '<td>';
                            temp += '<input class="easyui-textbox" name="Elements" data-options="required: ' + isRequired + '" value="' + value + '" style="width: 350px; height: 30px"/>';
                            temp += '</td>';
                        }

                        value = "";
                        isRequired = 1;
                        htmlArray[i] = temp;
                        temp = "";
                    }

                    temp += '<td class="lbl"><label class="lbl" title="其他">其他<span style="color:red">*</span></label></td>';
                    temp += '<td>';
                    temp += '<input class="easyui-textbox" name="Elements" data-options="required:true" value="' + otherDefault + '" style="width: 350px; height: 30px"/>';
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

                    $('#Elements').empty();
                    $('#Elements').html(tempall);
                    //重新渲染，使js添加的easyui样式生效
                    $.parser.parse('#Elements');
                    $('#divSave').css('display', 'block');
                } else {
                    $('#Elements').empty();
                    $.messager.alert('消息', '未查询到该海关编码的申报要素！');
                    $('#divSave').css('display', 'none');
                }
            })
        }

        //确认
        function Confirm() {
            if (!Valid()) {
                return;
            }

            var elements = document.getElementsByName('Elements');
            var temp = '';
            for (var i = 0; i < elements.length; i++) {
                temp += elements[i].value + '|';
            }
            temp = temp.substring(0, temp.length - 1);
            window.parent.$('#Elements').textbox('setValue', temp);
            window.parent.SetHSCode($('#HSCode').val());
            window.parent.SetIsConfirm(true);
            window.parent.VerifyManufacturer();
            //关闭当前弹框
            self.parent.$('iframe').parent().window('close');
        }

        //关闭
        function Close() {
            window.parent.SetIsConfirm(false);
            self.parent.$('iframe').parent().window('close');
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
                        <td class="lbl">型号</td>
                        <td>
                            <input class="easyui-textbox" id="Model" name="Model" data-options="disabled:true" style="width: 350px; height: 30px" />
                        </td>
                        <td class="lbl">品牌</td>
                        <td>
                            <input class="easyui-textbox" id="Manufacturer" name="Manufacturer" data-options="disabled:true" style="width: 350px; height: 30px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">海关编码</td>
                        <td>
                            <input class="easyui-textbox" id="HSCode" name="HSCode" style="width: 200px; height: 30px" />
                            <a id="btnHSCode" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="width: 135px; height: 30px" onclick="GetElements()">申报要素查询</a>
                        </td>
                    </tr>
                </table>
            </div>
            <hr style="margin-top: 10px" />
            <div id="Elements" style="margin-top: 10px"></div>
        </form>
    </div>

    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnConfirm" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Confirm()">确认</a>
        <a id="btnClose" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
