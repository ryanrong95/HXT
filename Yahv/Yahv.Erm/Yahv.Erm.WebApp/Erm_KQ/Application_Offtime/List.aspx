<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Offtime.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: true,
                nowrap:false,
            });
            $('#Status').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.Status,
            })
            $("#Department").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
            })
            $("#Staff").combobox({
                editable: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $('#ApprovalStatus').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.ApprovalStatus,
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#Status').textbox("setValue", "");
                $('#Staff').textbox("setValue", "");
                $('#Department').textbox("setValue", "");
                $('#Date').textbox("setValue", "");
                $('#ApprovalStatus').textbox("setValue", "")
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Status: $.trim($('#Status').combobox("getValue")),
                ApprovalStatus: $.trim($('#ApprovalStatus').combobox("getValue")),
                Staff: $.trim($('#Staff').combobox("getValue")),
                Department: $.trim($('#Department').combobox("getValue")),
                Date: $.trim($('#Date').datebox("getValue")),
            };
            return params;
        };
        var Draft = <%=Yahv.Erm.Services.ApplicationStatus.Draft.GetHashCode()%>;
        var Reject = <%=Yahv.Erm.Services.ApplicationStatus.Reject.GetHashCode()%>;
        var Closed = <%=Yahv.Underly.AdminStatus.Closed.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            if(row.Status == Draft || row.Status == Reject){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;" group>编辑</a> ');
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;" group>删除</a>');
            }
            else{
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Detail(\'' + row.ID + '\');return false;" group>详情</a> ');
            }
            if(row.Days >= 10 && row.AdminStatus == Closed){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="BackToWork (\'' + row.ID + '\');return false;" group>回岗确认</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Add.aspx');
            $.myWindow({
                title: "新增请假申请",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //编辑
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx')+ '?ID=' + id;
            $.myWindow({
                title: "编辑请假申请",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        $("#tab1").myDatagrid('flush');
                    })
                }
            });
        }
        //详情
        function Detail(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Details.aspx')+ '?ID=' + id;
            $.myWindow({
                title: "请假申请详情",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //回岗确认
        function BackToWork(id) {
            $.messager.confirm('确认', '请您再次确认员工是否回岗。', function (success) {
                if (success) {
                    $.post('?action=BackToWork', { ID: id }, function (res) {
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $("#tab1").myDatagrid('flush');
                    })
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">申请员工</td>
                <td style="width: 200px;">
                    <input id="Staff" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">员工部门</td>
                <td style="width: 200px;">
                    <input id="Department" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">申请状态</td>
                <td style="width: 200px;">
                    <input id="Status" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">审批状态</td>
                <td>
                    <input id="ApprovalStatus" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">请假日期</td>
                <td colspan="7">
                    <input id="Date" class="easyui-datebox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add" onclick="Add()">新增</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="请假申请列表">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center',width:50">申请日期</th>
                <th data-options="field:'ApplicantName',align:'center',width:50">请假人</th>
                <th data-options="field:'DepartmentType',align:'center',width:50">部门</th>
                <th data-options="field:'Manager',align:'center',width:50">部门负责人</th>
                <th data-options="field:'Dates',align:'left',width:100">请假日期</th>
                <th data-options="field:'Days',align:'center',width:50">请假时长(天)</th>
                <th data-options="field:'Type',align:'center',width:50">请假类型</th>
                <th data-options="field:'Reason',width:100">请假原因</th>
                <th data-options="field:'StatusDec',align:'center',width:50">申请状态</th>
                <th data-options="field:'StepName',align:'center',width:80">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:50">当前审批人</th>
                <th data-options="field:'btn',formatter:Operation,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
