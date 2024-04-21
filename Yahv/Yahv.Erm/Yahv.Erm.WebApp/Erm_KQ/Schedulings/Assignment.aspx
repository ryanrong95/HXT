<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Assignment.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Schedulings.Assignment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //员工
            window.grid = $("#dg").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                fit: false,
                pagination: false,
                rownumbers: true,
                singleSelect: false,
                rowStyler: function (index, row) {
                    if (row.SchedulingOld != row.SchedulingNew) {
                        return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                    }
                },
                columns: [[
                    { field: 'ck', checkbox: true },
                    { field: 'Code', title: '工号', width: 100, sortable: true },
                    { field: 'Name', title: '姓名', width: 100, sortable: true, sorter: SorterOpetation },
                    { field: 'DepartmentCode', title: '部门', width: 100, sortable: true },
                    { field: 'PostionName', title: '岗位', width: 100 },
                    {
                        field: 'SchedulingOld', title: '原班别', width: 100,
                        formatter: function (value) {
                            for (var i = 0; i < model.Schedulings.length; i++) {
                                if (model.Schedulings[i].Value == value) {
                                    return model.Schedulings[i].Text;
                                }
                            }
                            return value;
                        },
                        editor: { type: 'combobox', options: { data: model.Schedulings, valueField: "Value", textField: "Text", hasDownArrow: false } }
                    },
                    {
                        field: 'SchedulingNew', title: '新班别', width: 100,
                        formatter: function (value) {
                            for (var i = 0; i < model.Schedulings.length; i++) {
                                if (model.Schedulings[i].Value == value) {
                                    return model.Schedulings[i].Text;
                                }
                            }
                            return value;
                        },
                        editor: { type: 'combobox', options: { data: model.Schedulings, valueField: "Value", textField: "Text", hasDownArrow: false } }
                    },
                    { field: 'EntryDate', title: '入职日期', width: 100, sortable: true }
                ]]
            });
            $('#Position').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostionData,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            })
            $('#DepartmentType').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            })
            $('#PostType').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostType,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            })
            $('#Schedule').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.Schedulings,
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#Name').textbox("setValue", "")
                $('#Position').textbox("setValue", "")
                $('#DepartmentType').textbox("setValue", "")
                $('#PostType').textbox("setValue", "")
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //设置班别
            $("#btnSetSchedule").click(function () {
                var row = $("#dg").datagrid('getChecked');
                if (row.length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "请勾选员工信息", type: "error" });
                    return;
                }
                var schedule = $("#Schedule").combobox('getValue');
                if (schedule == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择员工班别", type: "error" });
                    return;
                }
                for (var i = 0; i < row.length; i++) {
                    row[i].SchedulingNew = schedule;
                }
                var data = $('#dg').datagrid('getData');
                $('#dg').datagrid('loadData', data);
                top.$.timeouts.alert({ position: "TC", msg: "设置成功", type: "success" });
            })
            //提交
            $("#btnSave").click(function () {
                //工作经历
                var schedule = $('#dg').datagrid('getRows');
                var schedules = [];
                for (var i = 0; i < schedule.length; i++) {
                    if (schedule[i].SchedulingNew != schedule[i].SchedulingOld) {
                        schedules.push(schedule[i]);
                    }
                }
                if (schedules.length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "员工班别未发生变更", type: "error" });
                    $.myWindow.close();
                    return;
                }
                var data = new FormData();
                data.append('schedules', JSON.stringify(schedules));
                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
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
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#Name').textbox("getText")),
                Position: $.trim($('#Position').combobox("getValue")),
                DepartmentType: $.trim($('#DepartmentType').combobox("getValue")),
                PostType: $.trim($('#PostType').combobox("getValue")),
            };
            return params;
        };
        //中文排序
        function SorterOpetation(a, b) {
            return a.localeCompare(b, 'zh');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div id="topper">
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 80px;">姓名或编号</td>
                    <td style="width: 130px">
                        <input id="Name" data-options="isKeydown:true" class="easyui-textbox" style="width: 120px;" />
                    </td>
                    <td style="width: 80px;">所在部门</td>
                    <td style="width: 130px">
                        <input id="DepartmentType" data-options="isKeydown:true" class="easyui-combobox" style="width: 120px;" />
                    </td>
                    <td style="width: 80px;">岗位名称</td>
                    <td style="width: 130px">
                        <input id="Position" data-options="isKeydown:true" class="easyui-combobox" style="width: 120px;" />
                    </td>
                    <td style="width: 80px;">职务类型</td>
                    <td style="width: 130px">
                        <input id="PostType" data-options="isKeydown:true" class="easyui-combobox" style="width: 120px;" />
                    </td>
                    <td>
                        <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    </td>
                </tr>
                <tr>
                    <td style="width: 90px;">员工班别</td>
                    <td colspan="9">
                        <input id="Schedule" class="easyui-combobox" style="width: 120px;" />
                        <a id="btnSetSchedule" class="easyui-linkbutton" iconcls="icon-yg-assign">设置班别</a>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center'" style="border: none">
            <table id="dg" title="">
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save">提交</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
