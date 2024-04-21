<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Projects.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    client: $.trim($('#ClientName').textbox("getValue")),
                    projectName: $.trim($('#Name').textbox("getValue")),
                    startdate: $("#s_startdate").datebox("getValue"),
                    enddate: $("#s_enddate").datebox("getValue"),
                    orderclient: $("#OrderClient").textbox('getValue')
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

            //新增项目
            $("#btnProject").click(function () {
                $.myDialog({
                    title: '添加',
                    url: 'Add.aspx',
                    width: "80%",
                    height: "80%",
                    isHaveOk: true,
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })

        });

        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function showEditPage(id) {
            $.myDialog({
                title: "编辑",
                url: 'Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "80%",
                height: "80%",
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
                        <input id="ClientName" data-options="validType:'length[1,75]'" class="easyui-textbox" /></td>

                    <td style="width: 100px;">项目名称</td>
                    <td>
                        <input id="Name" name="Name" class="easyui-textbox" data-options="validType:'length[1,75]'" />
                    </td>
                    <td style="width: 100px;">下单客户 </td>
                    <td>
                        <input id="OrderClient" name="OrderClient" class="easyui-textbox" data-options="validType:'length[1,75]'" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px;">立项日期</td>
                    <td>
                        <input id="s_startdate" class="easyui-datebox" data-options="editable:false,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_enddate" class="easyui-datebox" data-options="editable:false,prompt:'结束时间',onSelect:onSelect2" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnProject" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增项目</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%" data-option="true">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'ClientName',width:200">客户名称</th>
                <th data-options="field:'Name',width:200">项目名称</th>
                <th data-options="field:'EstablishDate',width:150">立项日期</th>
                <th data-options="field:'RDDate',width:150">预计研发日期</th>
                <th data-options="field:'ProductDate',width:150">预计量产日期</th>
                <th data-options="field:'Contact',width:100">联系人</th>
                <th data-options="field:'Mobile',width:100">联系人电话</th>
                <%--<th data-options="field:'Creator',width:200">下单客户</th>--%>
                <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
