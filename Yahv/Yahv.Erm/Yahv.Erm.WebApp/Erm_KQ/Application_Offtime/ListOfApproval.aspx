<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfApproval.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Offtime.ListOfApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
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
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#Status').textbox("setValue", "")
                $('#Staff').textbox("setValue", "")
                $('#Department').textbox("setValue", "")
                $('#Date').textbox("setValue", "")
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
                Staff: $.trim($('#Staff').combobox("getValue")),
                Department: $.trim($('#Department').combobox("getValue")),
                Date: $.trim($('#Date').datebox("getValue")),
            };
            return params;
        };
        var Draft = <%=Yahv.Erm.Services.ApplicationStatus.Draft.GetHashCode()%>;
        var Reject = <%=Yahv.Erm.Services.ApplicationStatus.Reject.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="Approval(\'' + row.ID + '\');return false;" group>审批</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        //新增
        function Add() {
            var url = location.pathname.replace(/ListOfApproval.aspx/ig, 'Add.aspx')+ '?AdminID=';
            $.myWindow({
                title: "新增请假申请",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //审批
        function Approval(id) {
            var url = location.pathname.replace(/ListOfApproval.aspx/ig, 'Approval.aspx') + '?ID=' + id;
            $.myWindow({
                title: "申请审批",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
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
                <td style="width: 90px;">请假日期</td>
                <td style="width: 200px;">
                    <input id="Date" class="easyui-datebox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">申请状态</td>
                <td>
                    <input id="Status" class="easyui-combobox" style="width: 150px;" />
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
    <table id="tab1" title="请假审批列表">
        <thead>
            <tr>
                <th data-options="field:'btn',align:'center',formatter:Operation,width:100">操作</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">请假人</th>
                <th data-options="field:'DepartmentType',align:'center',width:100">所属部门</th>
                <th data-options="field:'Manager',align:'center',width:100">部门负责人</th>
                <th data-options="field:'Type',align:'center',width:100">请假类型</th>
                <th data-options="field:'Days',align:'center',width:100">请假时长(天)</th>
                <th data-options="field:'Dates',align:'left',width:300">请假日期</th>
                <th data-options="field:'Reason',width:200">请假原因</th>
                <th data-options="field:'StatusDec',align:'center',width:100">申请状态</th>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'Name',align:'center',width:100">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
