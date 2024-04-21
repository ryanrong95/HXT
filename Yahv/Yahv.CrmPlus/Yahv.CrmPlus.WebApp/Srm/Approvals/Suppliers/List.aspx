<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Approvals.Suppliers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#cbbSupplierType").fixedCombobx({
                editable: false,
                required: true,
                type: "SupplierType",
                isAll: true
            })
            $("#cbbGrade").fixedCombobx({
                editable: false,
                required: true,
                type: "SupplierGrade",
                isAll: true
            })
            var getQuery = function () {
                var params = {
                    action: 'data',
                    Name: $.trim($('#txtname').textbox("getText")),
                    SypplierType: $('#cbbSupplierType').combobox("getValue"),
                    Grade: $('#cbbGrade').combobox("getValue"),
                    StartDate: $('#s_startdate').datebox("getValue"),
                    EndDate: $('#s_enddate').datebox("getValue"),
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
            

        });
        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnApproval" href="#" particle="Name:\'审批\',jField:\'Approval\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="approval(\'' + rowData.ID + '\')">审批</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function nameformatter(value, rowData) {
            var result = "";
            result += rowData.Name;
            result += '<span class="level' + rowData.Grade + '"></span>';
            return result;
        }
        function approval(id) {
            $.myWindow({
                title: '审批',
                url: 'Edit.aspx?enterpriseid=' + id,
                width: '60%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function reasonformatter(value, rowData) {
            var span = "<span title='" + value + "'>";
            if (value == null) {
                span += '';
            }
            else if (value.length > 15) {
                span += value.substring(0, 14) + "......";
            }
            else {
                span += value;
            }
            span += "</span>"
            return span;
        }

    </script>

    <!--搜索时间-->
    <script type="text/javascript">
        function onSelect1(sd) {
            $('#s_enddate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }
        function onSelect2(ed) {
            $('#s_startdate').datebox('calendar').calendar({
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
                    <td style="width: 100px;">名称</td>
                    <td>
                        <input id="txtname" class="easyui-textbox" /></td>
                    <td>类型</td>
                    <td>
                        <input id="cbbSupplierType" class="easyui-combobox">
                    </td>
                    <td>等级</td>
                    <td>
                        <input id="cbbGrade" class="easyui-combobox">
                    </td>
                    <td>注册时间</td>
                    <td>
                        <input id="s_startdate" class="easyui-datebox" data-options="editable:false,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_enddate" class="easyui-datebox" data-options="editable:false,prompt:'结束时间',onSelect:onSelect2" />
                    </td>
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
                <th data-options="field:'Name',width:280,formatter:nameformatter,sortable:true">名称</th>
                <th data-options="field:'TypeDes',width:120">类型</th>
                <th data-options="field:'Area',width:120">国别地区</th>
                <th data-options="field:'OrderTypeDesc',width:120">下单方式</th>
                <th data-options="field:'Creator',width:120,sortable:true">注册人</th>
                <th data-options="field:'CreateDate',width:150,sortable:true">注册时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

