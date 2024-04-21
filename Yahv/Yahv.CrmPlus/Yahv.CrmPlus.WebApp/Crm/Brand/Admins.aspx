<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Admins.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Brand.Admins" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                };
                return params;
            };
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: false,
                rownumbers: true,
                nowrap: false,
            });
            $("#btnCreator").click(function () {
                var adminid = $("#cbbAdmins").combobox('getValue');
                if (adminid.length > 0) {
                    $.post('?action=add', { AdminID: adminid, BrandID: model.BrandID }, function (data) {
                        if (data.success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: data.msg,
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: data.msg,
                                type: "error"
                            });
                        }
                    });
                }
                else {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "先选择负责人!",
                        type: "infor"
                    });
                }
            })
            $("#cbbAdmins").combobox({
                required: false,
                data: model.Admins,
                valueField: 'ID',
                textField: 'Text',
                panelHeight: 'auto',
                multiple: false,
                editable: false,
                limitToList: true,
            })
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDetails" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="abandon(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function abandon(id) {
            $.messager.confirm('确认', '确定删除吗？', function (r) {
                if (r) {
                    $.post('?action=abandon', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    
    <div>
        <table class="liebiao">
            <tr>
                <td>品牌名称</td>
                <td><%=this.Model.BrandName %></td>
            </tr>
          
        </table>
        <div id="tb">
            <table class="liebiao-compact">

                <tr>
                    <td colspan="2">
                        <input id="cbbAdmins" name="AdminIDs" class="easyui-combobox" style="width: 350px;" />
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
        <table id="dg">
            <thead>
                <tr>
                    <th data-options="field:'RealName',width:'30%'">管理员姓名</th>
                    <th data-options="field:'RoleName',width:'30%'">角色</th>
                    <th data-options="field:'Btn',formatter:btnformatter,width:'40%'">操作</th>

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
