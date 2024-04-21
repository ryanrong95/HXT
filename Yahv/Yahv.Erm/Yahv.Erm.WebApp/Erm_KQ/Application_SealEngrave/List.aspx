<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_SealEngrave.List" %>

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
    <table id="tab1" title="印章刻制申请列表">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">申请人</th>
                <th data-options="field:'DepartmentType',align:'center',width:100">所在部门</th>
                <th data-options="field:'Manager',align:'center',width:100">部门负责人</th>
                <th data-options="field:'SealName',align:'center',width:150">印章名称</th>
                <th data-options="field:'SealShape',align:'center',width:100">印章形状</th>
                <th data-options="field:'SealSize',align:'center',width:150">尺寸规则</th>
                <th data-options="field:'Reason',align:'left',width:150">刻章事由</th>
                <th data-options="field:'StatusDec',align:'center',width:100">申请状态</th>
                <th data-options="field:'StepName',align:'center',width:150">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
                <th data-options="field:'btn',align:'center',formatter:Operation,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
