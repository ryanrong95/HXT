<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfApproval.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Recruit.ListOfApproval" %>

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
            };
            return params;
        };
        var Draft = <%=Yahv.Erm.Services.ApplicationStatus.Draft.GetHashCode()%>;
        var Reject = <%=Yahv.Erm.Services.ApplicationStatus.Reject.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            if(row.StepName == '行政部招聘'){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Complete(\'' + row.ID + '\');return false;" group>招聘结果</a> ');
            }
            else{
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="Approval(\'' + row.ID + '\');return false;" group>审批</a> ');
            }       
            arry.push('</span>');
            return arry.join('');
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
        //招聘结果
        function Complete(id) {
            var url = location.pathname.replace(/ListOfApproval.aspx/ig, 'Complete.aspx') + '?ID=' + id;
            $.myWindow({
                title: "招聘结果",
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
                <td style="width: 90px;">申请部门</td>
                <td style="width: 200px;">
                    <input id="Department" class="easyui-combobox" style="width: 150px;" />
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
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="招聘审批列表">
        <thead>
            <tr>
                <th data-options="field:'btn',align:'center',formatter:Operation,width:100">操作</th>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">申请人</th>
                <th data-options="field:'DepartmentName',align:'center',width:100">申请部门</th>
                <th data-options="field:'PostionName',align:'center',width:150">岗位名称</th>
                <th data-options="field:'NumberOfNeeds',align:'center',width:100">需求人数</th>
                <th data-options="field:'NumberOfRecruiters',align:'center',width:100">拟招人数</th>
                <th data-options="field:'PeriodSalary',align:'center',width:100">试用期薪资</th>
                <th data-options="field:'NormalSalary',align:'center',width:100">转正后薪资</th>
                <th data-options="field:'ExpectedArrivalTime',align:'center',width:100">期望到岗日期</th>
                <th data-options="field:'StepName',align:'center',width:150">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
