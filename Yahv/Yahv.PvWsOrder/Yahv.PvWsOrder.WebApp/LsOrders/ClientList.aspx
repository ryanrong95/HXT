<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ClientList.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.LsOrders.ClientList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true,
                scrollbarSize: 0,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#CompanyName').textbox("setText", "");
                $('#ClientCode').textbox("setText", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                CompanyName: $.trim($('#CompanyName').textbox("getText")),
                ClientCode: $.trim($('#ClientCode').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        //新增申请
        function Add(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '新增租赁申请',
                minWidth: 1200,
                minHeight: 600,
                url: 'Add.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //我的申请
        function MyLsOrder(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '我的申请',
                minWidth: 1200,
                minHeight: 600,
                url: 'MyList.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-add\'" onclick="Add(' + index + ');return false;">新增申请</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="MyLsOrder(' + index + ');return false;">我的申请</a> '
                , '</span>'].join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户名称</td>
                <td>
                    <input id="CompanyName" data-options="prompt:'',validType:'length[1,75]'" class="easyui-textbox" style="width: 200px" />
                </td>
                <td style="width: 90px;">客户入仓号</td>
                <td>
                    <input id="ClientCode" data-options="prompt:'',validType:'length[1,10]'" class="easyui-textbox" style="width: 200px" />
                </td>
                <td style="width: 90px;">创建日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:''" class="easyui-datebox" />
                    &nbsp&nbsp&nbsp&nbsp<span>至</span>&nbsp&nbsp&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:''" class="easyui-datebox" />
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
    <table id="tab1" title="我的会员">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 40px;">创建日期</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 120px">客户名称</th>
                <th data-options="field:'CompanyCode',align:'left'" style="width: 80px;">统一社会信用代码</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 40px">客户入仓号</th>
                <th data-options="field:'Grade',align:'center'" style="width: 40px">客户等级</th>
                <th data-options="field:'ContactName',align:'center'" style="width: 40px;">联系人</th>
                <th data-options="field:'ContactTel',align:'center'" style="width: 50px;">联系电话</th>
                <th data-options="field:'Status',align:'center'" style="width: 40px;">状态</th>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 100px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
