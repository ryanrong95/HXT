<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetDefaultPort.aspx.cs" Inherits="WebApp.Declaration.DeclarantCandidates.SetDefaultPort" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>设置核对人</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var OrgCodes = eval('(<%=this.Model.OrgCodes%>)');
        var CustomMaster = eval('(<%=this.Model.CustomMaster%>)');
        var EntryPort = eval('(<%=this.Model.EntryPort%>)');
        $(function () {
            $('#DefaultPort').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,

            });


            $('#CustomMaster').combobox({
                data: CustomMaster,
                onChange: function (record) {
                    $.post('?action=getCustomMasterlist', { value: record }, function (data) {
                        $("#CustomMaster").combobox('loadData', data);
                    });
                },
            });
            $('#IEPortCode').combobox({
                data: CustomMaster,
                onChange: function (record) {
                    $.post('?action=getCustomMasterlist', { value: record }, function (data) {
                        $("#IEPortCode").combobox('loadData', data);
                    });
                },
            });
            $('#EntyPortCode').combobox({
                data: EntryPort,
                onChange: function (record) {
                    $.post('?action=getEntryPortlist', { value: record }, function (data) {
                        $("#EntyPortCode").combobox('loadData', data);
                    });
                },
            });
            $('#OrgCode').combobox({
                data: OrgCodes,
                onChange: function (record) {
                    $.post('?action=getDropdownlist', { value: record }, function (data) {
                        $("#OrgCode").combobox('loadData', data);
                    });
                },
            });
            $('#VsaOrgCode').combobox({
                data: OrgCodes,
                onChange: function (record) {
                    $.post('?action=getDropdownlist', { value: record }, function (data) {
                        $("#VsaOrgCode").combobox('loadData', data);
                    });
                },
            });
            $('#InspOrgCode').combobox({
                data: OrgCodes,
                onChange: function (record) {
                    $.post('?action=getDropdownlist', { value: record }, function (data) {
                        $("#InspOrgCode").combobox('loadData', data);
                    });
                },
            });
            $('#PurpOrgCode').combobox({
                data: CustomMaster,
                onChange: function (record) {
                    $.post('?action=getCustomMasterlist', { value: record }, function (data) {
                        $("#PurpOrgCode").combobox('loadData', data);
                    });
                },
            });
        });

        function Operation(val, row, index) {
            var buttons = '';

            buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\''
                + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function Edit(ID) {

            if (ID != undefined) {
                var rows = $('#DefaultPort').datagrid("getRows");
                var row;
                $.each(rows, function (index, val) {
                    if (val.ID == ID) {
                        row = val;
                    }
                });

                $('#CustomMaster').combobox('setValue', row.Code);
                $('#IEPortCode').combobox('setValue', row.IEPortCode);
                $('#EntyPortCode').combobox('setValue', row.EntyPortCode);
                $('#OrgCode').combobox('setValue', row.OrgCode);
                $('#VsaOrgCode').combobox('setValue', row.VsaOrgCode);
                $('#InspOrgCode').combobox('setValue', row.InspOrgCode);
                $('#PurpOrgCode').combobox('setValue', row.PurpOrgCode);
                $('#IsDefault').prop('checked', row.IsDefault);
            }
            else {

                $('#CustomMaster').combobox('setValue', '');
                $('#IEPortCode').combobox('setValue', '');
                $('#EntyPortCode').combobox('setValue', '');
                $('#OrgCode').combobox('setValue', '');
                $('#VsaOrgCode').combobox('setValue', '');
                $('#InspOrgCode').combobox('setValue', '');
                $('#PurpOrgCode').combobox('setValue', '');
                $('#IsDefault').prop('checked', false);
            }


            $('#edit-dialog').dialog({
                title: ID != undefined ? '编辑' : '新增',
                width: 850,
                height: 480,
                closed: false,
                closable: true,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-delete-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        if (!Valid("form1")) {
                            return;
                        }
                        var values = FormValues("form1");
                        values['ID'] = ID;

                        MaskUtil.mask();
                        $.post('?action=Save', { Model: JSON.stringify(values) }, function (res) {
                            MaskUtil.unmask();//关闭遮挡层
                            var result = JSON.parse(res);
                            $.messager.alert('消息', result.message, 'info', function () {
                                $('#edit-dialog').dialog('close');
                                $('#DefaultPort').myDatagrid('reload');
                            });
                        });
                    }
                }, {
                    id: 'btn-delete-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#edit-dialog').dialog('close');
                    }
                }],
            });

            $('#edit-dialog').window('center'); //dialog 居中
        }


        function FrmtCode(val, row, index) {
            return row.CodeName + " " + row.Code;
        }

        function FrmtIEPortCode(val, row, index) {
            return row.IEPortCodeName + " " + row.IEPortCode;
        }

        function FrmtEntyPortCode(val, row, index) {
            return row.EntyPortCodeName + " " + row.EntyPortCode;
        }

        function FrmtOrgCode(val, row, index) {
            return row.OrgCodeName + " " + row.OrgCode;
        }

        function FrmtVsaOrgCode(val, row, index) {
            return row.VsaOrgCodeName + " " + row.VsaOrgCode;
        }

        function FrmtInspOrgCode(val, row, index) {
            return row.InspOrgCodeName + " " + row.InspOrgCode;
        }

        function FrmtPurpOrgCode(val, row, index) {
            return row.PurpOrgCodeName + " " + row.PurpOrgCode;
        }

        function FrmtIsDefault(val, row, index) {
            return row.IsDefault ? "是" : "否";
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div style="margin: 5px 0 0px 15px;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Edit()">新增</a>
        </div>


    </div>
    <table id="DefaultPort" title="设置默认口岸" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false,rownumbers:true," toolbar="#topBar">
        <thead>
            <tr>
                <th data-options="field:'Code',align:'left',formatter:FrmtCode" style="width: 10%">申报地海关</th>
                <th data-options="field:'IEPortCode',align:'left',formatter:FrmtIEPortCode" style="width: 10%">进境关别</th>
                <th data-options="field:'EntyPortCode',align:'left',formatter:FrmtEntyPortCode" style="width: 10%">入境口岸</th>
<%--                <th data-options="field:'OrgCode',align:'left',formatter:FrmtOrgCode" style="width: 12%">检验检疫受理机关</th>
                <th data-options="field:'VsaOrgCode',align:'left',formatter:FrmtVsaOrgCode" style="width: 12%">领证机关</th>
                <th data-options="field:'InspOrgCode',align:'left',formatter:FrmtInspOrgCode" style="width: 12%">口岸检验检疫机关</th>--%>
                <th data-options="field:'PurpOrgCode',align:'left',formatter:FrmtPurpOrgCode" style="width: 12%">目的地海关</th>
                <th data-options="field:'IsDefault',align:'center',formatter:FrmtIsDefault" style="width: 6%">是否默认</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%">操作</th>
            </tr>
        </thead>
    </table>
    <div id="edit-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div>
            <form id="form1" runat="server">
                <div>
                    <table class="oprationTable" style="margin: 10px; width: 98%">
                        <tr>
                            <th style="width: 15%"></th>
                            <th style="width: 70%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">申报地海关：</td>
                            <td colspan="3">
                                <input class="easyui-combobox" id="CustomMaster"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">进境关别：</td>
                            <td colspan="3">
                                <input class="easyui-combobox" id="IEPortCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">入境口岸：</td>
                            <td colspan="3">
                                <input class="easyui-combobox" id="EntyPortCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                        </tr>
                       <%-- <tr>
                            <td class="lbl">检验检疫受理机关：</td>
                            <td>
                                <input class="easyui-combobox" id="OrgCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,editable:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                            <td class="lbl">领证机关：</td>
                            <td>
                                <input class="easyui-combobox" id="VsaOrgCode"
                                    data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,tipPosition:'bottom',missingMessage:'请输入编码,下拉框一次显示十条'," style="width: 200px" />
                            </td>
                        </tr>--%>
                        <tr>
                            <%--<td class="lbl">口岸检验检疫机关：</td>
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
                        <tr>
                            <td class="lbl"></td>
                            <td colspan="3">
                                <input type="checkbox" id="IsDefault" name="IsDefault" checked="checked" /><label for="IsDefault" style="margin-right: 30px">是否默认</label>
                            </td>
                        </tr>
                    </table>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
