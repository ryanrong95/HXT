<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfMy.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Resignation.ListOfMy" %>

<%@ Import Namespace="Yahv.Underly.Enums" %>
<%@ Import Namespace="Yahv.Erm.Services" %>
<%@ Import Namespace="Yahv" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                $('#s_keyword').textbox("setValue", "");
                $('#s_status').combobox('setValue', '');
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 添加
            $('#btnCreator').click(function () {
                edit('');
            });

            // 状态
            $('#s_status').combobox({
                editable: false,
                valueField: 'value',
                textField: 'text',
                data: model.Status
            });

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
                    { field: 'PostionName', title: '岗位名称', align: 'center', width: 120, formatter: function (value, row) { return row.Entity.PostionName; } },
                    { field: 'Handover', title: '工作承接人', align: 'center', width: 100, formatter: function (value, row) { return row.Entity.Handover; } },
                    {
                        field: 'ResignationDate', title: '离职日期', align: 'center', width: 100,
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
                    { field: 'StatusName', title: '状态', sortable: true, align: 'center', width: 100 },
                    { field: 'StepName', title: '当前审批步骤', align: 'center', width: 120 },
                    { field: 'AdminName', title: '当前审批人', align: 'center', width: 100 },
                    { field: 'btn', title: '操作', width: 150, formatter: btn_formatter }
                ]]

            });
        });
    </script>
    <script>
        // 搜索条件
        var getQuery = function () {
            var params = {
                action: 'data',
                s_keyword: $.trim($('#s_keyword').textbox("getText")),
                s_status: $.trim($('#s_status').combobox('getValue'))
            };
            return params;
        };

        // 添加或编辑
        function edit(id) {
            var url = location.pathname.replace(/ListOfMy.aspx/ig, 'Edit.aspx') + '?id=' + id;
            var title = "新增申请";
            if (id) {
                title = "编辑申请";
            }

            $.myWindow({
                title: title,
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
            return false;
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
                    });
                }
            });
        }

        // 详细信息
        function detail(id) {
            var url = location.pathname.replace(/ListOfMy.aspx/ig, 'Details.aspx') + '?id=' + id;
            $.myWindow({
                title: "申请信息",
                url: url,
            });

            return false;
        }

        //操作按钮格式化
        function btn_formatter(value, rec) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            //草稿 or 驳回 可编辑
            if (rec.ApplicantID == '<%= Erp.Current.ID %>' && (rec.ApplicationStatus == '<%= (int)ApplicationStatus.Draft%>' || rec.ApplicationStatus == '<%= (int)ApplicationStatus.Reject%>')) {
                arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.ID + '\');return false;" group>编辑</button>');
                arry.push(' <button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + rec.ID + '\');return false;" group>删除</button>');
            } else {
                arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rec.ID + '\');return false;">查看</button>');
            }
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
                <td style="width: 90px;">标题</td>
                <td style="width: 200px;">
                    <input id="s_keyword" type="text" class="easyui-textbox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">状态</td>
                <td>
                    <input id="s_status" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <em class="toolLine"></em>
                    <a id="btn_apply" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_apply_Click">申请表模板</a>
                    <a id="btn_handover" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btn_handover_Click">交接表模板</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="我的离职申请列表"></table>
</asp:Content>
