<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.SalesChances.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    clientName: $.trim($('#ClientName').textbox("getValue")),
                    projectName: $.trim($('#Name').textbox("getValue")),
                    partNumber: $("#PartNumber").textbox("getValue"),
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
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            });
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

        });

        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">审批</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function showEditPage(id) {
            $.myWindow({
                title: "销售机会状态审批",
                url: 'Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
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
                        <input id="ClientName" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">项目名称</td>
                    <td>
                        <input id="Name" name="Name" class="easyui-textbox" data-options="editable:false,panelheight:'auto'" />
                    </td>
                    <td style="width: 100px;">产品型号 </td>
                    <td>
                        <input id="PartNumber" name="PartNumber" class="easyui-textbox" data-options="panelheight:'auto'" />
                    </td>
                      <td>
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
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
                <th data-options="field:'PartNumber',width:150">型号</th>
                <th data-options="field:'Brand',width:150">品牌</th>
                  <th data-options="field:'ProjectStatusDes',width:200">变更状态</th>
                <th data-options="field:'Contact',width:100">联系人</th>
                <th data-options="field:'Mobile',width:100">联系人电话</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
