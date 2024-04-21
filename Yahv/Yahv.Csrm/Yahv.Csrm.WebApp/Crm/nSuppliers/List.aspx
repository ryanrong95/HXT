<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    //selStatus: $('#selStatus').combobox("getValue"),
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
                nowrap: false,
                rownumbers: true
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
        })

    </script>
    <script>
        var IsSuper = '<%= Yahv.Erp.Current.IsSuper%>' == 'True';
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)GeneralStatus.Normal%>') {
                //arry.push('<a id="btnUpd"  particle="Name:\'编辑\',jField:\'btnUpd\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDetail" particle="Name:\'详情\',jField:\'btnDetail\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showDetailPage(\'' + rowData.ID + '\')">详情</a> ');
                arry.push('<a id="btnConsignor" particle="Name:\'提货地址\',jField:\'btnConsignor\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showConsignor(\'' + rowData.ID + '\')">提货地址</a> ');
                arry.push('<a id="btnBenificiary" particle="Name:\'收款人\',jField:\'btnBenificiary\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showPayees(\'' + rowData.ID + '\')">收款人</a> ');
                arry.push('<a id="btnContact" particle="Name:\'联系人\',jField:\'btnContact\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showContacts(\'' + rowData.ID + '\')">联系人</a> ');
                arry.push('<a id="btnDel"  particle="Name:\'删除\',jField:\'btnDel\'"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        //打开收款人页面
        function showPayees(id) {
            $.myWindow({
                title: "收款人列表",
                url: 'nPayees/List.aspx?clientid=' + model.ID + "&supplierid=" + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function showContacts(id) {
            $.myWindow({
                title: "联系人列表",
                url: 'nContacts/List.aspx?clientid=' + model.ID + "&supplierid=" + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        //打开提货地址页面
        function showConsignor(id) {
            $.myWindow({
                title: "提货地址列表",
                url: 'nConsignors/List.aspx?clientid=' + model.ID + "&supplierid=" + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function showAddPage() {
            $.myWindowFuse({
                title: "新增代仓储供应商",
                url: 'Edit.aspx?clientid=' + model.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "700px",
                height: "500px",
            });
            return false;
        }
        function showEditPage(id) {
            $.myWindowFuse({
                title: "编辑代仓储供应商信息",
                url: 'Edit.aspx?clientid=' + model.ID + '&id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "700px",
                height: "500px",
            });
            return false;
        }
        function showDetailPage(id) {
            $.myWindow({
                title: "代仓储供应商详情",
                url: 'Details.aspx?clientid=' + model.ID + '&id=' + id, onClose: function () {
                },
                width: "700px",
                height: "500px",
            });
            return false;
        }
        function deleteItem(id, status) {

            $.messager.confirm('确认', '您确认想要删除该供应商吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { clientid: model.ID, supplierid: id }, function (result) {
                        if (result.success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除成功!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: result.msg,
                                type: "error"
                            });
                        }

                    });
                }
            });

        }
        function supplier_formatter(value, rec) {
            var result = "";
            result += rec.EnglishName;
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td colspan="7">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                       <%-- <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" particle="Name:'添加',jField:'btnCreator'" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <%-- <th data-options="field:'ID',width:100">ID</th>--%>
                <%--<th data-options="field:'Name',width:180,formatter:supplier_formatter">供应商名称</th>--%>
                <th data-options="field:'EnglishName',width:200,formatter:supplier_formatter">英文名称</th>
                <th data-options="field:'ChineseName',width:160">中文名称</th>
                <th data-options="field:'CHNabbreviation',width:100">中文简称</th>

                <th data-options="field:'Uscc',width:120">纳税人识别号</th>
                <th data-options="field:'Corporation',width:120">法人</th>
                <th data-options="field:'RegAddress',width:120">注册地址</th>
                <th data-options="field:'Place',width:120">国家/地区</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <th data-options="field:'UpdateDate',width:150">修改时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:425">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
