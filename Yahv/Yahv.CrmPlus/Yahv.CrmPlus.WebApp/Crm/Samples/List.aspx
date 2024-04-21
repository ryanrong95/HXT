<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Samples.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#Status").fixedCombobx({
                type: "AuditStatus",
                isAll: true
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    clientName: $.trim($('#ClientName').textbox("getValue")),
                    projectName: $.trim($('#Name').textbox("getValue")),
                    startdate: $("#s_startdate").datebox("getValue"),
                    enddate: $("#s_enddate").datebox("getValue"),
                    Status: $("#Status").combobox("getValue"),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            });
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            //新增
            $("#btnSample").click(function () {
                $.myDialog({
                    title: '新增送样信息',
                    url: 'Add.aspx',
                    width: "60%",
                    height: "70%",
                    isHaveOk: true,
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })

           

        });
          function Close(id) {
            $.messager.confirm('确认', '您确认想要删除该送样信息吗？', function (r) {
                if (r) {
                    $.post('?action=Closed', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已删除!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            });
        }
        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.AuditStatus == '<%=(int)Yahv.Underly.AuditStatus.Normal%>') {
                arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="addWaybillCode(\'' + rowData.ID + '\')">运单维护</a> ');
            } else {
                //arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnSuccess" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="Close(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }

       
        function showEditPage(id) {
            $.myWindow({
                title: "编辑送样信息",
                url: 'Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }

        function addWaybillCode(id) {
            $.myDialog({
                title: "维护运单号",
                url: 'AddWayBillNo.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "650px",
                height: "500px",
            });
            return false;

        }

    </script>

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
                    <td style="width: 100px;">客户名称</td>
                    <td>
                        <input id="ClientName" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">项目名称</td>
                    <td>
                        <input id="Name" name="Name" class="easyui-textbox" data-options="editable:false,panelheight:'auto'" />
                    </td>
                    <td style="width: 100px;">审核状态 </td>
                    <td>
                        <input id="Status" name="Status" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px;">寄送日期</td>
                    <td>
                        <input id="s_startdate" class="easyui-datebox" data-options="editable:false,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_enddate" class="easyui-datebox" data-options="editable:false,prompt:'结束时间',onSelect:onSelect2" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                      <%--  <a id="btnSample" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">样品申请</a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%" data-option="true">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field: 'ClientName',width:200">客户名称</th>
                <th data-options="field: 'ProjectName',width:200">项目名称</th>
                <th data-options="field: 'DeliveryDate',width:150">寄送日期</th>
                <th data-options="field: 'Address',width:150">寄送地址</th>
                <th data-options="field: 'WaybillCode',width:150">运单号</th>
                <th data-options="field: 'Contact',width:100">联系人</th>
                <th data-options="field:'Tel',width:100">联系人电话</th>
                <th data-options="field:'AuditStatusDes',width:200">审核状态</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
