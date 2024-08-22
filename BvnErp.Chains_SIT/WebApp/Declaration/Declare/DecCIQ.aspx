<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecCIQ.aspx.cs" Inherits="WebApp.Declaration.Declare.DecCIQ" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <style type="text/css">
        .lbl {
            text-align: right;
        }

        .divContent {
            border: 1px solid #aaa;
        }
    </style>
    <script type="text/javascript">      
        var editIndex = undefined;
        var ID = '<%=this.Model.ID%>';
        var OrgCodes = eval('(<%=this.Model.OrgCodes%>)');
        var CustomMaster = eval('(<%=this.Model.CustomMaster%>)');
        var CorrelationReason = eval('(<%=this.Model.CorrelationReason%>)');
        var OrigBoxFlag = eval('(<%=this.Model.OrigBoxFlag%>)');
        if (ID != '') {
            var DecHead = eval('(<%=this.Model.DecHead%>)');
        }
        var check = false;
        $(function () {
            InitClientPage();
            //$('#OrgCode').combobox({
            //    data: OrgCodes,
            //    onChange:function (record) {
            //        $.post('?action=getDropdownlist', { value: record }, function (data) {
            //            $("#OrgCode").combobox('loadData', data);
            //        });
            //    },
            //});
            //$('#VsaOrgCode').combobox({
            //    data: OrgCodes,
            //    onChange:function (record) {
            //        $.post('?action=getDropdownlist', { value: record }, function (data) {
            //            $("#VsaOrgCode").combobox('loadData', data);
            //        });
            //    },
            //});
            //$('#InspOrgCode').combobox({
            //    data: OrgCodes,
            //    onChange:function (record) {
            //        $.post('?action=getDropdownlist', { value: record }, function (data) {
            //            $("#InspOrgCode").combobox('loadData', data);
            //        });
            //    },
            //});
            $('#PurpOrgCode').combobox({
                data: CustomMaster,
                onChange:function (record) {
                    $.post('?action=getCustomMasterlist', { value: record }, function (data) {
                        $("#PurpOrgCode").combobox('loadData', data);
                    });
                },
            });

            $('#CorrelationReasonFlag').combobox({
                data: CorrelationReason
            });
             $('#OrigBoxFlag').combobox({
                data: OrigBoxFlag
            });

            //证书列表初始化
            $('#docReqTable').myDatagrid({
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                singleSelect: false,
                checkOnSelect: false,
                loadFilter: function (data) {
                    if (ID != '' && DecHead.RequestCerts != null && DecHead.RequestCerts.length > 0) {
                        $.each(DecHead.RequestCerts, function (index, val) {
                            for (var index = 0; index < data.rows.length; index++) {
                                var row = data.rows[index];
                                if (row.AppCertCode == val.AppCertCode) {
                                    row.ApplOri = val.ApplOri;
                                    row.ApplCopyQuan = val.ApplCopyQuan;
                                }
                            }
                        });
                    }
                    return data;
                },
                onBeforeSelect: function (index, row) {
                    if (!check)
                        return false;
                },
                onBeforeUnselect: function (index, row) {
                    if (!check)
                        return false;
                },
                onCheck: function (index, row) {
                    check = true;
                    $('#docReqTable').datagrid('selectRow', index);
                    check = false;
                },
                onUncheck: function (index, row) {
                    check = true;
                    $('#docReqTable').datagrid('unselectRow', index);
                    check = false;
                },
                onClickRow: function (index) {
                    if (editIndex != index) {
                        if (endEditing()) {
                            $('#docReqTable').datagrid('selectRow', index)
                                .datagrid('beginEdit', index);
                            editIndex = index;
                        } else {
                            $('#docReqTable').datagrid('selectRow', editIndex);
                        }
                    }
                    check = false;
                },
                onLoadSuccess: function (data) {
                    if (ID != '' && DecHead.RequestCerts != null && DecHead.RequestCerts.length > 0) {
                        $.each(DecHead.RequestCerts, function (index, val) {
                            check = true;
                            for (var i = 0; i < data.rows.length; i++) {
                                if (data.rows[i].AppCertCode == val.AppCertCode) {
                                    $('.datagrid-btable').find("input[type='checkbox']")[i].checked = true;
                                    $('#docReqTable').datagrid('selectRow', i);
                                }
                            }
                            check = false;
                        });
                    }

                    var heightValue = $("#docReqTable").prev().find(".datagrid-body").find(".datagrid-btable").height() + 60;
                    $("#docReqTable").prev().find(".datagrid-body").height(heightValue);
                    $("#docReqTable").prev().height(heightValue);
                    $("#docReqTable").prev().parent().height(heightValue);
                    $("#docReqTable").prev().parent().parent().height(heightValue);
                }
            });         
            //编辑初始化
            if (DecHead != null && DecHead != "") {

                if (DecHead.IsInspection || DecHead.IsQuarantine) {
                    //是商检/检疫
                    //$('#OrgCode').combobox('setValue', DecHead.OrgCode);
                    //$('#VsaOrgCode').combobox('setValue', DecHead.VsaOrgCode);
                    //$('#InspOrgCode').combobox('setValue', DecHead.InspOrgCode);
                    $('#PurpOrgCode').combobox('setValue', DecHead.PurpOrgCode);
                    //特种业务标识
                    if (DecHead.SpecDeclFlag != null && DecHead.SpecDeclFlag.indexOf('1') >= 0) {
                        $.each(DecHead.SpecDeclFlag.split(''), function (index, val) {
                            if (val == '1') {
                                $('#SpecDeclFlag' + (index + 1)).prop('checked', true);
                            }
                        });
                    }
                    $("#UseOrgPersonCode").textbox("setValue", DecHead.UseOrgPersonCode);
                    $("#UseOrgPersonTel").textbox("setValue", DecHead.UseOrgPersonTel);
                    $('#DespDate').datebox('setValue', DecHead.DespDate);
                    $('#OrigBoxFlag').combobox('setValue', DecHead.OrigBoxFlag);
                    $("#BLNo").textbox("setValue", DecHead.BLNo);
                    $("#CorrelationNo").textbox("setValue", DecHead.CorrelationNo);
                    $('#CorrelationReasonFlag').combobox('setValue', DecHead.CorrelationReasonFlag);

                    //检验检疫申报要素
                    $("#DomesticConsigneeEname").textbox("setValue", DecHead.DomesticConsigneeEname);
                    $("#OverseasConsignorCname").textbox("setValue", DecHead.OverseasConsignorCname);
                    $("#OverseasConsignorAddr").textbox("setValue", DecHead.OverseasConsignorAddr);                   
                    $('#CmplDschrgDt').datebox('setValue', DecHead.CmplDschrgDt);                    
                }
                else {
                    $('#NoInspection').css('display', 'block');
                    $('#btnSave').hide();
                    $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
                }
            }
        });

        function Save() {
            if (!Valid("form1")) {
                return;
            }

            //特殊业务标识
            var SpecDeclFlag = "";
            for (var i = 1; i < 8; i++) {
                SpecDeclFlag += $('#SpecDeclFlag' + i)[0].checked ? '1' : '0';
            }

            //证书
            $.each($("#docReqTable").datagrid('getRows'), function (index, val) {
                $("#docReqTable").datagrid('endEdit', index);
            });
            var rows = $('#docReqTable').datagrid('getSelections');
            var values = FormValues("form1");
            values['ID'] = ID;
            values['Cert'] = rows;
            values['SpecDeclFlag'] = SpecDeclFlag;

            $.post('?action=Save', { Model: JSON.stringify(values) }, function (res) {
                var result = JSON.parse(res);
                $.messager.alert('消息', result.message, 'info', function () {
                    if (result.success) {

                    } else {
                         $.messager.alert('消息', result.info);
                    }
                });
            });
        }

        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#docReqTable').datagrid('validateRow', editIndex)) {
                var ed = $('#docReqTable').datagrid('getEditor', { index: editIndex, field: 'productid' });
                $('#docReqTable').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 8px;">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
            <%--<a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>--%>
            <label id="NoInspection" style="font-size: 18px; font-weight: 600; color: red; padding-left: 10%; display: none;">此单无需商检或检疫!</label>
        </div>
        <div style="margin: 5px">
            <div class="divContent">
                <div style="margin: 5px">
                    <label style="font-size: 18px; font-weight: 600; color: orangered">检验检疫信息</label>
                </div>
                <div>
                    <table class="oprationTable" style="margin: 10px; width: 98%">
                        <tr>
                            <th style="width: 15%"></th>
                            <th style="width: 20%"></th>
                            <th style="width: 15%"></th>
                            <th style="width: 30%"></th>
                        </tr>

                     <%--   <tr>
                            <td class="lbl">检验检疫受理机关：</td>
                            <td>
                                <input class="easyui-combobox" id="OrgCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                            <td class="lbl">领证机关：</td>
                            <td>
                                <input class="easyui-combobox" id="VsaOrgCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                        </tr>--%>
                        <tr>
                           <%-- <td class="lbl">口岸检验检疫机关：</td>
                            <td>
                                <input class="easyui-combobox" id="InspOrgCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>--%>
                            <td class="lbl">目的地海关：</td>
                            <td>
                                <input class="easyui-combobox" id="PurpOrgCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                        </tr>
                        <tr style="height: 35px;">
                            <td class="lbl">特殊业务标识：</td>
                            <td colspan="3" style="text-align: left">
                                <input type="checkbox" id="SpecDeclFlag1" name="SpecDeclFlag1" /><label for="SpecDeclFlag1" style="margin-right: 30px">国际赛事</label>
                                <input type="checkbox" id="SpecDeclFlag2" name="SpecDeclFlag2" /><label for="SpecDeclFlag2" style="margin-right: 30px">特殊进出军工物资</label>
                                <input type="checkbox" id="SpecDeclFlag3" name="SpecDeclFlag3" /><label for="SpecDeclFlag3" style="margin-right: 30px">国际援助物资</label>
                                <input type="checkbox" id="SpecDeclFlag4" name="SpecDeclFlag4" /><label for="SpecDeclFlag4" style="margin-right: 30px">国际会议</label>
                                <input type="checkbox" id="SpecDeclFlag5" name="SpecDeclFlag5" /><label for="SpecDeclFlag5" style="margin-right: 30px">直通放行</label>
                                <input type="checkbox" id="SpecDeclFlag6" name="SpecDeclFlag6" /><label for="SpecDeclFlag6" style="margin-right: 30px">外交礼遇</label>
                                <input type="checkbox" id="SpecDeclFlag7" name="SpecDeclFlag7" /><label for="SpecDeclFlag7" style="margin-right: 30px">转关</label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">使用单位联系人：</td>
                            <td>
                                <input class="easyui-textbox" id="UseOrgPersonCode" style="width: 200px" />
                            </td>
                            <td class="lbl">使用单位联系电话：</td>
                            <td>
                                <input class="easyui-textbox" id="UseOrgPersonTel" style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">启运日期：</td>
                            <td>
                                <input class="easyui-datebox" id="DespDate" data-options="required:true" style="width: 200px" />
                            </td>
                            <td class="lbl">原箱运输：</td>
                            <td>
                                <input class="easyui-combobox" id="OrigBoxFlag"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true" style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">B/L号：</td>
                            <td>
                                <input class="easyui-textbox" id="BLNo" style="width: 200px" />
                            </td>
                            <td class="lbl">关联号码及理由：</td>
                            <td>
                                <input class="easyui-textbox" id="CorrelationNo" style="width: 130px" />
                                <input class="easyui-combobox" id="CorrelationReasonFlag"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true" style="width: 150px" />
                            </td>
                        </tr>

                    </table>
                </div>
            </div>
            <br />
            <div class="divContent">
                <div style="margin: 5px">
                    <label style="font-size: 18px; font-weight: 600; color: orangered">检验检疫申报要素</label>
                </div>
                <div>
                    <table class="oprationTable" style="margin: 10px; width: 98%">
                        <tr>
                            <th style="width: 15%"></th>
                            <th style="width: 60%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">境内外发货人名称（英文）：</td>
                            <td>
                                <input class="easyui-textbox" id="DomesticConsigneeEname" data-options="required:true" style="width: 500px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">境外发货人名称（中文）：</td>
                            <td>
                                <input class="easyui-textbox" id="OverseasConsignorCname" data-options="required:true" style="width: 500px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">境外发货人地址（中文）：</td>
                            <td>
                                <input class="easyui-textbox" id="OverseasConsignorAddr" data-options="required:true" style="width: 500px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">卸毕日期：</td>
                            <td>
                                <input class="easyui-datebox" id="CmplDschrgDt" data-options="required:true" style="width: 500px" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <div style="display: block; margin-left: 10%;">
                        <table id="docReqTable" data-options="fit:false" style="width: 70%;height:auto">
                            <thead>
                                <tr>
                                    <th data-options="field:'ck',checkbox:true" style="width: 10px"></th>
                                    <th data-options="field:'AppCertCode',align:'center'" style="width: 120px">证书代码</th>
                                    <th data-options="field:'AppCertCodeName',align:'center'" style="width: 420px">证书名称</th>
                                    <th data-options="field:'ApplOri',align:'center',editor:'numberbox'" style="width: 120px">正本数量</th>
                                    <th data-options="field:'ApplCopyQuan',align:'center',editor:'numberbox'" style="width: 120px">副本数量</th>
                                </tr>
                            </thead>
                        </table>
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>
