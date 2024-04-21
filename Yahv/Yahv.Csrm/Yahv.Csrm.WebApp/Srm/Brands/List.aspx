<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Brands.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText"))
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
                rownumbers: true,
                nowrap: false,
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
        ///操作按钮
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-assign\'" onclick="assign(\'' + rowData.ID + '\')">分配</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function agentformatter(value, rowData) {
            return value ? "是" : "否";
        }
        function assign(brandid) {
            $.myWindowFuse({
                title: "分配",
                url: 'Edit.aspx?BrandID=' + brandid,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "600px",
                height: "450px",
            });
            return false;
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">名称/简称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称/简称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>

                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'Name',width:'30%'">名称</th>
                <th data-options="field:'ChineseName',width:'20%'">中文名称</th>
                <th data-options="field:'ShortName',width:'20%'">简称</th>
                <th data-options="field:'IsAgent',width:'10%',formatter:agentformatter">是否代理</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:'10%'">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
