<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfApproval.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Resignation.ListOfApproval" %>

<%@ Import Namespace="Yahv.Underly.Enums" %>
<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery,
                columns: [[
                    { field: 'CreateDate', title: '申请日期', align: 'center', sortable: true, width: 100 },
                    { field: 'Applicant', title: '离职人', align: 'center', width: 100, formatter: function (value, row) { return row.Entity.Applicant; } },
                    { field: 'DeptName', title: '所属部门', align: 'center', width: 100, formatter: function (value, row) { return row.Entity.DeptName; } },
                    { field: 'PostionName', title: '岗位名称', align: 'center', width: 100, formatter: function (value, row) { return row.Entity.PostionName; } },
                    { field: 'Handover', title: '工作承接人', align: 'center', width: 100, formatter: function (value, row) { return row.Entity.Handover; } },
                    {
                        field: 'ResignationDate', title: '离职日期', align: 'center', sortable: true, width: 100,
                        formatter: function (value, row) {
                            if (row.Entity.ResignationDate) {
                                var base = new Date("2019-03-01");//芯达通公司成立
                                var date = new Date(row.Entity.ResignationDate);
                                if (date < base) {
                                    return "--"
                                }
                                var dateStr = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
                                return dateStr;
                            }
                            else {
                                return "--"
                            }
                        }
                    },
                    { field: 'StatusDec', title: '申请状态', align: 'center', width: 100 },
                    { field: 'VoteStepName', title: '当前审批步骤', align: 'center', width: 150 },
                    { field: 'ApproveName', title: '当前审批人', align: 'center', width: 100 },
                    { field: 'btn', title: '操作', align: 'center', width: 120, formatter: btn_formatter }
                ]]

            });
            //查询条件
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
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#Staff').textbox("setValue", "")
                $('#Department').textbox("setValue", "")
                grid.myDatagrid('search', getQuery());
                return false;
            }); 
        });
    </script>
    <script>
        // 搜索条件
        var getQuery = function () {
            var params = {
                action: 'data',
                Staff: $.trim($('#Staff').combobox("getValue")),
                Department: $.trim($('#Department').combobox("getValue")),
            };
            return params;
        };

        // 审批
        function approval(id, uri) {
            var url = location.pathname.replace(/ListOfApproval.aspx/ig, uri) + '?id=' + id;

            $.myWindow({
                title: "申请审批",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });

            return false;
        }

        //操作按钮格式化
        function btn_formatter(value, row) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="approval(\'' + row.ApplicationID + '\',\'' + row.Uri + '\');return false;" group>审批</a> ');
            arry.push('</span>');
            return arry.join('');
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
                <td>
                    <input id="Department" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="离职审批列表"></table>
</asp:Content>
