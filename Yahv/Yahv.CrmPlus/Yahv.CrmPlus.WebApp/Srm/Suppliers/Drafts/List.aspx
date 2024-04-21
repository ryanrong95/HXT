<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Suppliers.Drafts.List" %>

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
            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '注册',
                    url: 'Add.aspx',
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })

        });
        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)AuditStatus.Normal%>') {
                arry.push('<a id="btnDetails" href="#"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rowData.ID + '\',\'' + rowData.EnterpriseID + '\')">查看</a> ');
            }
            else {
                arry.push('<a id="btnEdit" href="#" particle="Name:\'编辑\',jField:\'btnEdit\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rowData.ID + '\')">编辑</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function nameformatter(value, rowData) {
            var result = "";
            result += rowData.Name;
            result += '<span class="level' + rowData.SupplierGrade + '"></span>';
            return result;
        }
        function edit(id) {
            $.myDialog({
                title: '编辑',
                url: 'Edit.aspx?enterpriseid=' + id,
                width: '60%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function detail(id) {
            $.myDialog({
                title: '注册信息',
                url: 'Details.aspx?enterpriseid=' + id,
                width: '60%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function submit(id) {
            $.post("?action=sumbit", { ID: id }, function (Success) {
                if (Success) {
                    //$("#SupplierName").textbox('setValue', model.Entity.Enterprise.Name);
                    top.$.timeouts.alert({ position: "TC", msg: result.message, type: "error" });
                }
                else {
                    return;
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

                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <a id="btnCreator" particle="Name:'注册',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">注册</a>
                        <%--<a id="btnMyCreators" particle="Name:'我的注册',jField:'btnMyCreators'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-details'">我的注册</a>--%>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <div data-options="region:'center'">
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field:'Name',width:'25%',formatter:nameformatter,sortable:true">名称</th>
                    <th data-options="field:'TypeDes',width:'10%'">类型</th>
                    <%--<th data-options="field:'Grade',width:120">等级</th>--%>
                    <%-- <th data-options="field:'SettlementDesc',width:120">结算类型</th>--%>
                    <th data-options="field:'Area',width:'10%'">国别地区</th>
                    <th data-options="field:'StatusDes',width:'10%'">审批状态</th>
                    <th data-options="field:'Summary',width:'15%',formatter:reasonformatter">审批意见</th>
                    <th data-options="field:'CreteDate',width:'15%'">注册时间</th>
                    <th data-options="field:'Btn',formatter:btnformatter,width:'15%'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
