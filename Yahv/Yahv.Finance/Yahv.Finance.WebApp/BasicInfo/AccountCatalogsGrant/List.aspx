<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.AccountCatalogsGrant.List" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        /*body { background-color: #eee; }
        .datagrid-cell { padding: 1px; }
        .datagrid-row { height: 22px; line-height: 22px; }
        .datagrid-btable * { border-spacing: 1px; }*/
    </style>

    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };

        function allot(id, name) {
            $.myDialog({
                title: "分配收付款类型",
                url: 'Edit.aspx?id=' + id + '&name=' + name,
                width: "50%",
                height: "80%",
            });
            return false;
        }
        var igons = [parseInt('<%=(int)Yahv.Underly.AdminStatus.Super%>'), parseInt('<%=(int)Yahv.Underly.AdminStatus.Npc%>')]

        function Staff_formatter(value, rec) {
            //console.log(rec.StaffStatus);
            if (rec) {
                if (!rec.StaffID) {
                    return "添加用户";
                } else {
                    if (rec.StaffStatus == "<%=(int)StaffStatus.Normal %>") {
                        return rec.StaffID;
                    }
                    else if (rec.StaffStatus == "<%=(int)StaffStatus.Departure %>") {
                        return "已离职";
                    }
                    else if (rec.StaffStatus == "<%=(int)StaffStatus.Delete %>") {
                        return "已废弃";
                    }
                }
            }
        }

        function btn_formatter(value, rec) {
            //if ($.inArray(rec.Status, igons) > -1) {
            //    return '';
            //}

            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="allot(\'' + rec.ID + '\',\'' + rec.RealName + '\');return false;">分配</a> '
                , '</span>'].join('');
        }
       
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">名称</td>
                <td colspan="3">
                    <input id="s_name" data-options="prompt:'用户名或真实名',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="管理员列表">
        <thead>
            <tr>
                <%--<th data-options="field:'ck',checkbox:true"></th>--%>
                <th data-options="field:'ID',width:109">ID</th>
                <th data-options="field:'UserName',width:150">用户名</th>
                <th data-options="field:'RealName',width:150">真实名</th>
                <th data-options="field:'Staff',width:150,formatter:Staff_formatter">员工信息</th>
                <%--<th data-options="field:'DyjCode',width:150">大赢家ID</th>--%>
                <th data-options="field:'SelCode',width:150">编码</th>
                <th data-options="field:'RoleName',width:150">角色</th>
                <%--<th data-options="field:'CreateDate',width:106">创建日期</th>--%>
                <%--<th data-options="field:'LastLoginDate',width:170">最后登入</th>--%>
                <%--<th data-options="field:'StatusName',width:111">状态</th>--%>
                <th data-options="field:'btn',align:'center',formatter:btn_formatter,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
