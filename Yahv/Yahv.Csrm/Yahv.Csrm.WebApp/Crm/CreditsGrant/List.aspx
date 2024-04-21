<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.CreditsGrant.List" %>

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
                        $(this).combobox('select', '-100');
                    }
                }
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    selStatus: $('#selStatus').combobox("getValue"),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: true,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });

            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            });

            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        })

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnAccount"  particle="Name:\'欠款批复\',jField:\'btnAccount\'" href="#"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-sealing\'" onclick="showAccountList(\'' + rowData.ID + '\',\'' + rowData.Name + '\')">欠款批复</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function showAccountList(id, name) {
            $.myDialog({
                title: "欠款批复",
                url: '../CreditsGrant/Edit.aspx?payerId=' + id + "&payeeId=" + model.CompanyID + "&payer=" + name,
                width: "60%",
                height: "80%"
            });
            return false;
        }

        function client_formatter(value, rec) {
            var result = "";
            if (rec.Vip) {
                result += "<span class='vip'></span>";
            }
            result += rec.Name
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
                        <input id="s_name" data-options="prompt:'名称或入仓号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 90px;">状态</td>

                    <td colspan="5">
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>

                </tr>
                <tr>
                    <td colspan="8">
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
                <th data-options="field:'Name',width:350,formatter:client_formatter">名称</th>
                <th data-options="field:'EnterCode',width:100">入仓号</th>
                <th data-options="field:'Total',width:100">信用额度</th>
                <th data-options="field:'Cost',width:100">信用花费</th>
                <th data-options="field:'ServiceManager',width:80">业务员</th>
                <th data-options="field:'Merchandiser',width:80">跟单员</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

