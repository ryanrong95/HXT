<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsConsignees.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selStatus').combobox({
                data: model.Status,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '0');
                    }
                }
            });
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                rownumbers: true,
                singleSelect: false,
                fit: true,
                nowrap: false,
                queryParams: getQuery()
            });

            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                address: $.trim($('#txtaddress').textbox("getText")),
                contactname: $.trim($('#txtcontact').textbox("getText")),
                tel: $.trim($('#txttel').textbox("getText"))
            };
            return params;
        };
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)ApprovalStatus.Normal%>') {
                //arry.push('<a id="btnUpd" particle="Name:\'编辑\',jField:\'btnUpd\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDetail" particle="Name:\'详情\',jField:\'btnDetail\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showDetailPage(\'' + rowData.ID + '\')">详情</a> ');
                arry.push('<a id="btnDel" href="#" particle="Name:\'删除\',jField:\'btnDel\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该收件地址吗？', function (r) {
                if (r) {
                    $.post('?action=del', { clientid: model.Client.ID, id: id }, function () {
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
        function showDetailPage(id) {
            $.myWindow({
                title: "收件地址信息详情",
                url: 'Details.aspx?id=' + id + '&clientid=' + model.Client.ID + '&enterprisetype=' + model
                .EnterpriseType, onClose: function () {
                },
                width: "50%",
                height: "65%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myDialog({
                title: "编辑收件地址信息",
                url: 'Edit.aspx?id=' + id + '&clientid=' + model.Client.ID + '&enterprisetype=' + model
                .EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "65%",
            });
            return false;
        }
        function showAddPage() {
            $.myDialog({
                title: "添加收件地址",
                url: 'Edit.aspx?&clientid=' + model.Client.ID + '&enterprisetype=' + model
                .EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "65%",
            });
            return false;
        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">地址</td>
                <td>
                    <input id="txtaddress" data-options="prompt:'地址',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                <td style="width: 90px;">联系人</td>
                <td>
                    <input id="txtcontact" data-options="prompt:'联系人姓名',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">电话/手机号</td>
                <td colspan="3">
                    <input id="txttel" data-options="prompt:'联系人电话或手机号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <%--<a id="btnCreator" particle="Name:'添加',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>--%>
                </td>
            </tr>
        </table>
    </div>
    <!-- 表格 -->
    <table id="dg" data-options="fit:true" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'StatusName',width:50">状态</th>
                

                <th data-options="field:'Title',width:100">收货单位</th>
                <th data-options="field:'Place',width:80">国家/地区</th>
                <th data-options="field:'Address',width:280">地址</th>
                <th data-options="field:'Postzip',width:50">邮编</th>
                <th data-options="field:'DyjCode',width:80">大赢家编码</th>
                <th data-options="field:'ContactName',width:80">联系人</th>
                <th data-options="field:'Tel',width:100">电话</th>

                <th data-options="field:'Mobile',width:100">手机号</th>
                <th data-options="field:'IsDefault',width:100">是否默认</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>
                <th data-options="field:'Btn',formatter:btnformatter,width:210">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
