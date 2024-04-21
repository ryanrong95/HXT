<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_WorkCard.List" %>

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
                if (row.StepName == "部门负责人审核") {
                    arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Approval(' + index + ');return false;" group>部门审核</a> ');
                }
                else if (row.StepName == "行政部受理") {
                    arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Approval(' + index + ');return false;" group>行政受理</a> ');
                }
                else if(row.StepName == "行政部通知工牌领取"){
                    arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Approval(' + index + ');return false;" group>通知领卡</a> ');
                }
                else {
                    arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Approval(' + index + ');return false;" group>员工领卡</a> ');
                }
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
            var url = null;
            if (row.StepName == "部门负责人审核") {
                url = location.pathname.replace(/List.aspx/ig, 'ApprovalStep1.aspx') + '?ID=' + row.ID;
            }
            else if (row.StepName == "行政部受理") {
                url = location.pathname.replace(/List.aspx/ig, 'ApprovalStep2.aspx') + '?ID=' + row.ID;
            }
            else if(row.StepName == "行政部通知工牌领取"){
                url = location.pathname.replace(/List.aspx/ig, 'ApprovalStep3.aspx') + '?ID=' + row.ID;
            }
            else {
                url = location.pathname.replace(/List.aspx/ig, 'ApprovalStep4.aspx') + '?ID=' + row.ID;
            }
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
                <td style="width: 200px;">
                    <input id="Status" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">审批状态</td>
                <td>
                    <input id="ApprovalStatus" class="easyui-combobox" style="width: 150px;" />
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
    <table id="tab1" title="工牌补办申请列表">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">补办人</th>
                <th data-options="field:'DepartmentType',align:'center',width:100">所属部门</th>
                <th data-options="field:'Manager',align:'center',width:100">部门负责人</th>
                <th data-options="field:'Reason',align:'left',width:200">补办理由</th>
                <th data-options="field:'StatusDec',align:'center',width:100">申请状态</th>
                <th data-options="field:'StepName',align:'center',width:150">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
                <th data-options="field:'btn',align:'center',formatter:Operation,width:120">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
