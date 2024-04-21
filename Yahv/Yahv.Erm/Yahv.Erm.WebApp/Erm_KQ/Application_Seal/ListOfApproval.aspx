<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfApproval.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Seal.ListOfApproval" %>

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
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#Status').textbox("setValue", "")
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
            };
            return params;
        };
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="Approval(' + index + ');return false;" group>审批</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        //审批
        function Approval(index) {
            var row = $("#tab1").myDatagrid('getRows')[index];
            var url = null;
            if (row.StepName == "总经理审批") {
                url = location.pathname.replace(/ListOfApproval.aspx/ig, 'ApprovalStep1.aspx') + '?ID=' + row.ID;
            }
            else if (row.StepName == "行政部安排盖章") {
                url = location.pathname.replace(/ListOfApproval.aspx/ig, 'ApprovalStep2.aspx') + '?ID=' + row.ID;
            }
            else {
                url = location.pathname.replace(/ListOfApproval.aspx/ig, 'ApprovalStep3.aspx') + '?ID=' + row.ID;
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
                <td style="width: 90px;">状态</td>
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
    <table id="tab1" title="印章使(借)用审批列表">
        <thead>
            <tr>
                <th data-options="field:'btn',align:'center',formatter:Operation,width:100">操作</th>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">使(借)用人</th>
                <th data-options="field:'DepartmentType',align:'center',width:100">所在部门</th>
                <th data-options="field:'Manager',align:'center',width:100">部门负责人</th>
                <th data-options="field:'SealType',align:'center',width:100">印章类型</th>
                <th data-options="field:'BorrowDate',align:'center',width:100">使(借)用日期</th>
                <th data-options="field:'ReturnDate',align:'center',width:100">归还日期</th>
                <th data-options="field:'Reason',align:'left',width:200">使(借)用事由</th>
                <th data-options="field:'StatusDec',align:'center',width:100">申请状态</th>
                <th data-options="field:'StepName',align:'center',width:150">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
