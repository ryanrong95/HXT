<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PsWms.SzApp.Clients.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#clientName').textbox("setText", "");
                $('#startDate').datebox("setText", "");
                $('#endDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#clientName').textbox("getText")),
                StartDate: $.trim($('#startDate').datebox("getText")),
                EndDate: $.trim($('#endDate').datebox("getText")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Details(' + index + ');return false;">查看</a> '
                , '</span>'].join('');
        }
        //详情
        function Details(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '客户信息',
                minWidth: 1200,
                minHeight: 600,
                url: 'Details.aspx?ID=' + data.ID,
                onClose: function () {
                },
            });
            return false;
        }     
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户名称</td>
                <td style="width: 200px;">
                    <input id="clientName" data-options="prompt:'',validType:'length[1,75]'" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">创建日期</td>
                <td>
                    <input id="startDate" data-options="prompt:''" class="easyui-datebox" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="endDate" data-options="prompt:''" class="easyui-datebox" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="客户列表">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">创建日期</th>
                <th data-options="field:'Name',align:'left'" style="width: 300px">客户名称</th>
                <th data-options="field:'Username',align:'left'" style="width: 150px;">登录账号</th>
                <th data-options="field:'Password',align:'left'" style="width: 150px">登录密码</th>
                <th data-options="field:'LoginDate',align:'left'" style="width: 150px">最后登录时间</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
