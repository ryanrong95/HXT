<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Protecteds.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.ApprovalRecords.Protecteds" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    Name: $.trim($("#txtname").textbox('getValue')),
                    RegisteStartDate: $.trim($('#s_RegisteStartDate').datebox("getValue")),
                    RegisteEndDate: $('#s_RegisteEndDate').datebox("getValue"),
                    ApproveStartDate: $.trim($('#s_ApproveStartDate').datebox("getValue")),
                    ApproveEndDate: $('#s_ApproveEndDate').datebox("getValue"),
                    ApplyAdmin: $.trim($("#txtApplyAdmin").textbox('getValue')),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        })
        function nameformatter(value, rowData) {
            var result = "";
            result += '<a href="#" onclick=detail(\'' + rowData.MainID + '\')>' + rowData.EnterpriseName + '</a>';
            return result;
        }
        function detail(id) {
            $.myWindow({
                title: '供应商详情',
                url: '../Suppliers/Details.aspx?id=' + id,
                width: '1100px',
                height: '600px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
    </script>
    <!--搜索时间-->
    <script type="text/javascript">
        function onSelect1(sd) {
            $('#s_RegisteEndDate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
            $('#s_ApproveEndDate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }
        function onSelect2(ed) {
            $('#s_RegisteStartDate').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
                }
            });
            $('#s_ApproveStartDate').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td>
                        <input id="txtname" class="easyui-textbox" data-options="prompt:'供应商名称'" /></td>
                    <td>申请时间</td>
                    <td>
                        <input id="s_RegisteStartDate" class="easyui-datebox" data-options="editable:true,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_RegisteEndDate" class="easyui-datebox" data-options="editable:true,prompt:'结束时间',onSelect:onSelect2" />
                    </td>
                    <td>审批时间</td>
                    <td>
                        <input id="s_ApproveStartDate" class="easyui-datebox" data-options="editable:true,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_ApproveEndDate" class="easyui-datebox" data-options="editable:true,prompt:'结束时间',onSelect:onSelect2" />
                    </td>
                    <td>
                        <input id="txtApplyAdmin" class="easyui-textbox" data-options="prompt:'注册人'" /></td>
                    <td>
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td>
                </tr>

            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'EnterpriseName',formatter:nameformatter,width:280,sortable:true">供应商名称</th>
                <th data-options="field:'Applyer',width:120,sortable:true">申请人</th>
                <th data-options="field:'ApplyDate',width:120,sortable:true">申请时间</th>
                <th data-options="field:'Approver',width:100,sortable:true,sortable:true">审批人</th>
                <th data-options="field:'ApproveDate',width:120,sortable:true">审批时间</th>
                <th data-options="field:'Status',width:200">审批结果</th>
            </tr>
        </thead>
    </table>
</asp:Content>
