<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Adress.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHead">
    <script>
        $(function () {

            $("#AddressType").fixedCombobx({
                type: "AddressType",
                isAll: true
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    txtKey: $.trim($('#txtKey').textbox("getValue")),
                    AddressType: $("#AddressType").fixedCombobx("getValue")
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
                    url: './Edit.aspx?&enterpriseid=' + model.ID, onClose: function () {
                        location.reload();
                    },
                    width: "50%",
                    height: "60%",
                })
            });
              // 搜索按钮
            $('#btnSearch').click(function () {
                $("#dg").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

        });

        //收件地址操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status =='<%=(int)AuditStatus.Closed%>') {
                arry.push('<a id="btnConsineeEnable" href="#" particle="Name:\'地址启用\',jField:\'btnConsineeEnable\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            } else if (rowData.Status =='<%=(int)AuditStatus.Normal%>') {
                arry.push('<a id="btnConsineeUnable" href="#" particle="Name:\'地址停用\',jField:\'btnConsineeUnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            }

            arry.push('</span>');
            return arry.join('');
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
                    <td style="width: 100px;">收货人/详细地址</td>
                    <td>
                        <input id="txtKey" name="Key" style="width: 250px;" data-options="prompt:'收货人/详细地址',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">地址类型</td>

                    <td>
                        <input id="AddressType" name="AddressType" />
                    </td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a>
                       <%-- <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用选定的项</a>
                        <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用选定的项</a>--%>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'AddressType',width:120">地址类型</th>
                <th data-options="field:'Contact',width:120">收货人</th>
                <th data-options="field:'Phone',width:120">联系电话</th>
                <th data-options="field:'District',width:120">交货地</th>
                <th data-options="field:'Context',width:120">详细地址</th>
                <th data-options="field:'PostZip',width:120">邮政编码</th>
                <th data-options="field:'Creator',width:120">创建人</th>
                <th data-options="field:'AddressType',width:120">地址类型</th>
                <th data-options="field:'StatusDes',width:120">审批状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

