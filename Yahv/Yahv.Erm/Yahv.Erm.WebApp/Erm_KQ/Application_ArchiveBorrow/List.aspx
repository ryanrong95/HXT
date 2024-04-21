<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_ArchiveBorrow.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false
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
                $('#Status').textbox("setValue", "")
                $('#Staff').textbox("setValue", "")
                $('#Department').textbox("setValue", "")
                $('#ApprovalStatus').textbox("setValue", "")
                $('#ArchiveName').textbox("setValue", "")
                $('#StartDate').datebox("setValue", "")
                $('#EndDate').datebox("setValue", "")
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
                ArchiveName: $.trim($('#ArchiveName').textbox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getValue")),
                EndDate: $.trim($('#EndDate').datebox("getValue")),
            };
            return params;
        };
        var Draft = <%=Yahv.Erm.Services.ApplicationStatus.Draft.GetHashCode()%>;
        var Reject = <%=Yahv.Erm.Services.ApplicationStatus.Reject.GetHashCode()%>;
        var UnderApproval = <%=Yahv.Erm.Services.ApplicationStatus.UnderApproval.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            if(row.Status == UnderApproval && model.CurrentAdmin.ID == row.AdminID){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Approval(' + index + ');return false;" group>审批</a> ');
            }
            else{
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Detail(\'' + row.ID + '\');return false;" group>详情</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        //详情
        function Detail(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Details.aspx')+ '?ID=' + id;
            $.myWindow({
                title: "申请详情",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //审批
        function Approval(index) {
            var row = $("#tab1").myDatagrid('getRows')[index];
            var url = location.pathname.replace(/List.aspx/ig, 'ApprovalStep.aspx') + '?ID=' + row.ID;
            $.myWindow({
                title: row.StepName,
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
                <td style="width: 90px;">申请状态</td>
                <td>
                    <input id="Status" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">单证档案名称</td>
                <td style="width: 200px;">
                    <input id="ArchiveName" class="easyui-textbox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">审批状态</td>
                <td style="width: 200px;">
                    <input id="ApprovalStatus" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">申请日期</td>
                <td>
                    <input id="StartDate" class="easyui-datebox" style="width: 150px;" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" class="easyui-datebox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="单证档案借阅申请列表">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'ArchiveName',align:'left',width:150">单证档案名称</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">借阅人</th>
                <th data-options="field:'DepartmentType',align:'center',width:100">借阅部门</th>
                <th data-options="field:'Manager',align:'center',width:100">部门负责人</th>
                <th data-options="field:'Reason',align:'left',width:200">借阅原因</th>
                <th data-options="field:'BorrowDate',align:'center',width:100">借阅日期</th>
                <th data-options="field:'ReturnDate',align:'center',width:100">归还日期</th>
                <th data-options="field:'StatusDec',align:'center',width:100">申请状态</th>
                <th data-options="field:'StepName',align:'center',width:150">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
                <th data-options="field:'btn',align:'center',formatter:Operation,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
