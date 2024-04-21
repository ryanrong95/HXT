<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Contacts.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHead">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data'
                };
                return params;
            };

            //$('#selStatus').combobox({
            //    data: model.Status,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    onLoadSuccess: function (data) {
            //        if (data.length > 0) {
            //            $(this).combobox('select', '0');
            //        }
            //    }
            //});
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
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
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: "新增",
                    url: 'Edit.aspx?&id=' + model.ID, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "50%",
                    height: "60%",
                });
            })

        });
        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDetails" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + rowData.ID + '\')">详情</a> ');
            if (rowData.Status == '<%=(int)DataStatus.Normal%>') {
                arry.push('<a id="btnUnable" href="#" particle="Name:\'停用\',jField:\'btnUnable\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            } else if (rowData.Status == '<%=(int)DataStatus.Closed%>') {
                arry.push('<a id="btnEnable" href="#" particle="Name:\'启用\',jField:\'btnEnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            }

            arry.push('</span>');
            return arry.join('');
        }
        function Details(id) {
            $.myWindow({
                title: "详情",
                url: 'Detail.aspx?&id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
        }

        function Closed(id) {
            $.messager.confirm('确认', '确定停用吗？', function (r) {
                if (r) {
                    $.post('?action=Closed', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已停用!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                        // grid.myDatagrid('search', getQuery());
                    });
                }
            });
        }
        function Enable(id) {
            $.messager.confirm('确认', '确定启用吗？', function (r) {
                if (r) {
                    $.post('?action=Enable', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "启用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphForm">

    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8"><a id="btnCreator" particle="Name:'新增',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a></td>
                </tr>
                <tr></tr>
            </table>
        </div>
    </div>
    <table id="dg" data-options="rownumbers:true">
        <thead>
            <tr>

                <th data-options="field:'Name',width:120">联系人姓名</th>
                <th data-options="field:'Department',width:120">部门</th>
                <th data-options="field:'Positon',width:120">职位</th>
                <th data-options="field:'Mobile',width:120">手机</th>
                <th data-options="field:'Email',width:120">邮箱</th>
                <th data-options="field:'Tel',width:120">电话</th>
                <th data-options="field:'Gender',width:50">性别</th>
                <th data-options="field:'QQ',width:120">QQ</th>
                <th data-options="field:'Wx',width:120">微信</th>
                <th data-options="field:'StatusDes',width:80">状态</th>
                <th data-options="field:'CreteDate',width:150">创建时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>


</asp:Content>
