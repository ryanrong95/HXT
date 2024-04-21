<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Invoices.List" %>

<%@ Import Namespace="Yahv.Underly" %>
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
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });


            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: "新增",
                    url: 'Edit.aspx?&enterpriseid=' + model.ID, onClose: function () {
                       location.reload();
                    },
                    width: "50%",
                    height: "60%",
                });
            })
            
        });

        //发票操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)DataStatus.Normal%>') {
                arry.push('<a id="btnInvoiceUnable" href="#" particle="Name:\'发票停用\',jField:\'btnInvoiceUnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            } else if (rowData.Status == '<%=(int)DataStatus.Closed%>') {
                arry.push('<a id="btnInvoiceEnable" particle="Name:\'发票启用\',jField:\'btnInvoiceEnable\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
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


        function BookAccountClosed(id) {
            $.messager.confirm('确认', '确定停用吗？', function (r) {
                if (r) {
                    $.post('?action=BookAccountClosed', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已停用!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            });
        }
        function BookAccountEnable(id) {
            $.messager.confirm('确认', '确定启用吗？', function (r) {
                if (r) {
                    $.post('?action=BookAccountEnable', { ID: id }, function () {
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
    <script>


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
            <div id="tb">
                <div>
                    <table class="liebiao-compact">
                        <tr>
                            <td colspan="8"><a id="btnCreator" particle="Name:'发票新增',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a></td>
                        </tr>
                        <tr></tr>
                    </table>
                </div>
            </div>
            <table id="dg" data-options="rownumbers:true">
                <thead>
                    <tr>
                        <th data-options="field:'Name',width:120">公司名称</th>
                        <th data-options="field:'Address',width:120">公司地址</th>
                        <th data-options="field:'Tel',width:120">联系电话</th>
                        <th data-options="field:'Bank',width:120">开户行</th>
                        <th data-options="field:'Account',width:120">开户行账号</th>
                        <th data-options="field:'StatusDes',width:80">状态</th>
                        <th data-options="field:'CreateDate',width:150">创建时间</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
                    </tr>
                </thead>
            </table>
</asp:Content>
