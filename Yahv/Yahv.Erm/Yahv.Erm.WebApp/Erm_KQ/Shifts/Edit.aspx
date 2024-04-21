<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Shifts.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ID = getQueryString("id");
        $(function () {
            $("#CurrentSch").combobox({
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.SchedulingData,
            })
            $("#Shifts").combobox({
                required: true,
                editable: false,
                multiple: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.SchedulingData,
                onChange: function () {
                    var values = $("#Shifts").combobox("getValues");
                    if (values.length > 2) {
                        $("#Shifts").combobox("unselect", values[values.length - 1]);
                        $.messager.alert('提示', "请选择两个轮换班别");
                    }
                }
            })
            $("#NextSch").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.SchedulingData,
            })
            //保存
            $("#btnSave").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                //验证轮转班别
                var shifts = $("#Shifts").combobox("getValues");
                var nextSch = $("#NextSch").combobox("getValue");
                if (shifts.length != 2) {
                    $.messager.alert('提示', "请选择两个轮换班别");
                    return false;
                }
                //验证下次班别
                if ($.inArray(nextSch, shifts) == -1) {
                    $.messager.alert('提示', "请从轮换班别中选择下次班别");
                    return false;
                }

                //基本信息
                var data = new FormData();
                data.append('ID', ID);
                data.append('Staff', $("#Staff").combobox('getValue'));
                data.append('Shifts', $("#Shifts").combobox('getValues'));
                data.append('NextSch', $("#NextSch").combobox('getValue'));
                ajaxLoading();
                $.ajax({
                    url: '?action=Save',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })

            //初始化
            Init();
        });
    </script>
    <script>
        function Init() {
            if (model.Data != null) {
                $("#Staff").combobox({
                    required: true,
                    disabled: true,
                    valueField: 'Value',
                    textField: 'Text',
                    data: model.StaffData,
                    onChange: function () {
                        StaffChange();
                    }
                })
                $("#Staff").combobox('setValue', model.Data.ID);
                $("#Shifts").combobox('setValues', model.Data.Shifts)
                $("#NextSch").combobox('setValue', model.Data.nextSch)
            }
            else {
                $("#Staff").combobox({
                    required: true,
                    editable: false,
                    valueField: 'Value',
                    textField: 'Text',
                    data: model.StaffData,
                    onChange: function () {
                        StaffChange();
                    }
                })
            }
        }
        function StaffChange() {
            var staffId = $("#Staff").combobox('getValue');
            $.post('?action=StaffChange', { ID: staffId }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $("#CurrentSch").combobox('setValue', result.data.scheduling);
                    $("#SelCode").textbox('setValue', result.data.selcode)
                }
                else {
                    $("#CurrentSch").combobox('setValue', "");
                    $("#SelCode").textbox('setValue', "")
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr>
                    <td style="width: 120px">员工名称</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px">员工工号</td>
                    <td>
                        <input id="SelCode" class="easyui-textbox" style="width: 250px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px">当前班别</td>
                    <td>
                        <input id="CurrentSch" class="easyui-combobox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px">轮换班别</td>
                    <td>
                        <input id="Shifts" class="easyui-combobox" style="width: 250px;" data-options="prompt:'请选择两个轮换班别'" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px">下次班别</td>
                    <td>
                        <input id="NextSch" class="easyui-combobox" style="width: 250px;" data-options="prompt:'请从轮换班别中选择'" />
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save">保存</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
