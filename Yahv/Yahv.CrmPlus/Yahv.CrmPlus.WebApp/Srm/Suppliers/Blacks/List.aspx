<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Suppliers.Blacks.List" %>

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
                    SupplierType: $('#cbbSupplierType').combobox("getValue"),
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
            arry.push('<a id="btnDetails" href="#" particle="Name:\'查看\',jField:\'btnDetails\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rowData.ID + '\',\'' + rowData.EnterpriseID + '\')">查看</a> ');
            arry.push('<a id="btnCancelBlack" href="#" particle="Name:\'撤销黑名单\',jField:\'btnCancelBlack\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-returned\'" onclick="cancel(\'' + rowData.EnterpriseID + '\')">撤销黑名单</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function nameformatter(value, rowData) {
            var result = "";
            result += rowData.Name;
            result += '<span class="level' + rowData.SupplierGrade + '"></span>';
            return result;
        }
        function detail(id, enterpriseid) {
            $.myWindow({
                title: '供应商详情',
                url: '../../SupplierDetails/Edit.aspx?id=' + id,
                width: '80%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        //撤销黑名单
        function cancel(id) {
            $.messager.confirm('确认', '确定撤销黑名单吗？', function (r) {
                if (r) {
                    $.post('?action=Cancel', { ID: id }, function (result) {
                        if (result.success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: result.message,
                                type: "success"
                            });
                            window.grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: result.message,
                                type: "error"
                            });
                        }
                    });

                }
            });
        }
        function clientformatter(value, rowData) {
            return rowData.IsClient ? "是" : "否";
        }
        function acountformatter(value, rowData) {
            return rowData.IsAccount ? "是" : "否";
        }
        function specialformatter(value, rowData) {
            return rowData.IsSpecial ? "是" : "否";
        }
        function summaryformatter(value, rowData) {
            var span = "<span title='" + value + "'>";
            if (value == null) {
                return null;
            }
            if (value.length > 15) {
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
                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>

                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div data-options="region:'center'">
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field:'Name',width:280,formatter:nameformatter,sortable:true">名称</th>
                    <th data-options="field:'TypeDes',width:120">类型</th>
                    <th data-options="field:'SettlementDesc',width:120">结算类型</th>
                    <th data-options="field:'Area',width:120">国别地区</th>
                    <th data-options="field:'IsAccount',width:120,formatter:acountformatter">是否有账期</th>
                    <th data-options="field:'IsSpecial',width:120,formatter:specialformatter">是否特色</th>
                    <th data-options="field:'IsClient',width:120,formatter:clientformatter">是否同为客户</th>
                    <th data-options="field:'Summary',width:120,formatter:summaryformatter">黑名单原因</th>
                    <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
