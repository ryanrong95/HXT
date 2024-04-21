<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Brand.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>

        $(function () {

            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                };
                return params;
            };
         window.grid =  $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
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



            //$("#btnAdd").click(function () {
            //    $.myDialog({
            //        title: '添加',
            //        url: './Edit.aspx',
            //        width: "50%",
            //        height: "60%",
            //        isHaveOk: false,
            //        onClose: function () {
            //            $("#dg").myDatagrid('search', getQuery());
            //        }
            //    });
            //});

        });
       
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)GeneralStatus.Normal%>') {
                arry.push('<a id="btnDetails" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Owners(\'' + rowData.ID + '\')">负责人</a> ');
                    arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                    arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="deleteItem(\'' + rowData.ID + '\')">停用</a> ');
                }
           else
               {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="deleteItem(\'' + rowData.ID + '\')">启用</a> ');

                    }
        
            arry.push('</span>');
            return arry.join('');
        }
        function Owners( id) {
            $.myWindow({
                title: '负责人',
                url: 'Admins.aspx?id=' + id ,
                width: '60%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function showAddPage() {
            $.myWindow({
                title: "新增",
                url: 'Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }

          function showEditPage(id) {
            $.myWindow({
                title: "编辑",
                url: 'Edit.aspx?ID='+id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }


        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该品牌吗？', function (r) {
                if (r) {
                    $.post('?action=del', { id: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">输入搜索</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称/简称/中文名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <%-- 表格--%>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'Name',width:300">品牌名称</th>
                <th data-options="field:'Code',width:100">简称</th>
                <th data-options="field:'ChineseName',width:200">品牌中文名称</th>
                <th data-options="field:'WebSite',width:100">网址</th>
                <th data-options="field:'Summary',width:200">备注</th>
                <th data-options="field:'Creator',width:100">创建人</th>
                <th data-options="field:'CreateDate',width:200">创建时间</th>
                <%--<th data-options="field:'CreateDate',width:200">修改时间</th>--%>
                <th data-options="field:'StatusDes',width:100">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:300">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
