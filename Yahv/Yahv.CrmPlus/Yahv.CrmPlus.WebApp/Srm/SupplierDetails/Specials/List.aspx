<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Specials.List" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function ()
        {
            var getQuery = function () {
                var params = {
                    action: 'data',
                };
                return params;
            };
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false, 
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx?id=' + model.EnterpriseID,
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            })
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)AuditStatus.Voted%>')
            {
                arry.push('<a id="btnDel" href="#" particle="Name:\'删除特色\',jField:\'btnDel\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('<a id="btnDetails" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rowData.ID + '\')">详情</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function detail(id)
        {
            $.myWindow({
                title: '详情',
                url: 'Detail.aspx?enterpriseid='+model.EnterpriseID+'&subid=' + id,
                width: "800px",
                height: '500px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function del(id)
        {
            $.post('?action=del', { id: id }, function (success) {
                if (success) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "删除成功",
                        type: "success"
                    });
                    grid.myDatagrid('flush');
                }
                else {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "删除失败",
                        type: "error"
                    });
                }
            });
          
            grid.myDatagrid('flush');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8"><a id="btnCreator" particle="Name:'新增特色',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'Brand',width:200">品牌</th>
                <th data-options="field:'PartNumber',width:200">型号</th>
                <th data-options="field:'Type',width:80">特色类型</th>
                <th data-options="field:'Summary',width:220">备注</th>
                <th data-options="field:'StatusDes',width:80">状态</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:180">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
