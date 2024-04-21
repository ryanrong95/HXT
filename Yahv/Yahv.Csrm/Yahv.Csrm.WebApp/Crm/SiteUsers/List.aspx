<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.SiteUsers.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data'
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false
            });
            $("#btnCreator").click(function () {
                if (model.WsClient.ServiceManager == null) {
                    top.$.messager.alert('操作失败', "请先分配业务员");
                }
                else if (model.Count < 5) {
                    $.myDialog({
                        title: "新增会员",
                        url: 'Edit.aspx?wsclientid=' + model.WsClient.Enterprise.ID, onClose: function () {
                            window.grid.myDatagrid('flush');
                        },
                        width: "48%",
                        height: "60%",
                    });
                }
                else {
                    top.$.messager.alert('操作失败', "会员账号数量已达到上限");
                }
                return false;
            })
        })
        function btnformatter(value, rowData, index) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUpd"  particle="Name:\'编辑\',jField:\'btnUpd\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
            arry.push('<a id="btnReset"  particle="Name:\'初始化密码\',jField:\'btnReset\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-initPass\'" onclick="reset(\'' + rowData.ID + '\')">初始化密码</a> ');
            arry.push('<a id="btnDel"  particle="Name:\'删除\',jField:\'btnDel\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function showEditPage(id) {
            if (model.WsClient.ServiceManager == null) {
                top.$.messager.alert('操作失败', "请先分配业务员");
            }
            else {
                $.myDialog({
                    title: "编辑会员账号信息",
                    url: 'Edit.aspx?wsclientid=' + model.WsClient.Enterprise.ID + '&userid=' + id, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "48%",
                    height: "60%",
                });
            }
            return false;
        }
        function del(id) {
            $.messager.confirm('确认', '您确认要删除吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { userid: id, clientid: model.WsClient.Enterprise.ID }, function () {
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
        function reset(id) {
            $.messager.confirm('确认', '您确认初始化密吗？', function (r) {
                if (r) {
                    $.post('?action=resetPwd', { userid: id, enterpriseid: model.WsClient.Enterprise.ID }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "密码已初始化为 CXHY123",
                            type: "success"
                        });
                    });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8">
                        <a id="btnCreator" particle="Name:'新增',jField:'btnCreator'"  class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a>
                    </td>
                </tr>
            </table>

        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field:'UserName',width:180,nowrap:false">用户名</th>
                   <%-- <th data-options="field:'RealName',width:80">真实姓名</th>--%>
                    <th data-options="field:'Mobile',width:100">手机号</th>
                    <th data-options="field:'Email',width:150">邮箱</th>
                    <th data-options="field:'QQ',width:80">QQ</th>
                    <th data-options="field:'Wx',width:100">微信</th>
                    <th data-options="field:'Summary',width:100">备注</th>
                    <th data-options="field:'Status',width:50">状态</th>
                    <th data-options="field:'IsMain',width:50">主账号</th>
                    <th data-options="field:'Btn',formatter:btnformatter,width:250">操作</th>

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
