<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.AgentBrands.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
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
                    width: '600px',
                    height: '300px',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            })
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUnable" href="#" particle="Name:\'废弃代理品牌\',jField:\'btnUnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="abandon(\'' + rowData.ID + '\')">废弃</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function abandon(id) {
            $.messager.confirm('确认', '确定废弃吗？', function (r) {
                if (r) {
                    $.post('?action=abandon', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "废弃成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        }
        //function Owners(brandid,id) {
        //    $.myWindow({
        //        title: '负责人',
        //        url: 'Edit.aspx?brandid=' + brandid + '&id=' + id,
        //        width: '60%',
        //        height: '80%',
        //        onClose: function () {
        //            window.grid.myDatagrid('flush');
        //        }
        //    });
        //}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8"><a id="btnCreator" particle="Name:'新增代理品牌',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'BrandName',width:'40%'">品牌</th>
                <th data-options="field:'Summary',width:'40%'">备注</th>
               <%-- <th data-options="field:'CompanyName',width:300">代理公司</th>--%>
                <th data-options="field:'Btn',formatter:btnformatter,width:'20%'">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
