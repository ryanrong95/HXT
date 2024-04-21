<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.TraceRecords.Detail" %>


<%@ Import Namespace="Yahv.Underly" %>
<%@ Import Namespace="System" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
            });

            $("#btnCreator").click(function () {
                $.myDialogFuse({
                    title: '点评',
                    url: './TraceComments/Edit.aspx?TraceID=' + model.Entity.ID,
                    width: "750px",
                    height: "400px",
                    isHaveOk: true,
                    onClose: function () {
                        $('#dg').myDatagrid('flush');
                    }
                })
            });
        })
        ////操作
        //function btnformatter(value, rowData) {
        //    var arry = ['<span class="easyui-formatted">'];
        //    arry.push('<a id="btnCreator" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="comment(\'' + model.Entity.ID + '\')">点评</a> ');
        //    arry.push('</span>');
        //    return arry.join('');
        //}
        ////点评
        //function comment(id) {
        //    $.myWindow({
        //        title: "点评",
        //        url: 'Edit.aspx?ID=' + id, onClose: function () {
        //            window.grid.myDatagrid('flush');
        //        },
        //        width: "60%",
        //        height: "80%",
        //    });
        //    return false;
        //}
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.TraceRecord entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.TraceRecord;

    %>
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: 200px">
            <table class="liebiao">
                <tr>
                    <td style="width: 110px">客户名称：</td>
                    <td>
                        <%=entity.Enterprise.Name %></td>
                    <td>跟进方式：</td>
                    <td>
                        <%=entity.FollowWay.GetDescription() %>
                    </td>
                </tr>
                <tr>
                    <td>跟进日期：</td>
                    <td>
                        <%=entity.TraceDate.ToString("yyyy-MM-dd") %>
                    </td>
                    <td>下次跟进日期:</td>
                    <td>
                        <%=entity.NextDate?.ToString("yyyy-MM-dd") %>
                    </td>
                </tr>
                <tr>
                    <td>原厂陪同人员：</td>
                    <td colspan="3">
                        <%=entity.SupplierStaffs %></td>
                </tr>
                
                <tr>
                    <td>指定阅读人：</td>
                    <td colspan="3">
                        <%=this.Model.Readers %></td>
                </tr>
                <%if (entity.Owner.ID == Yahv.Erp.Current.ID)
                    {
                        Yahv.CrmPlus.Service.Models.Origins.Contact contact = this.Model.Contact as Yahv.CrmPlus.Service.Models.Origins.Contact;
                %>
                <tr>
                    <td>客户联系人：</td>
                    <td colspan="3">
                        <%=contact.Name %>-<%=contact.Mobile %>
                    </td>
                </tr>
                <%
                    }
                %>
                <tr>
                    <td>跟进内容：</td>
                    <td colspan="3">
                        <%=entity.Context %>
                    </td>
                </tr>
                <tr>
                    <td>下一步计划：</td>
                    <td colspan="3">
                        <%=entity.NextPlan %>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center'" style="height: 553px">
            <div id="tb">

                <div>
                    <table class="liebiao-compact">
                        <tr>
                            <td colspan="8">
                                <a id="btnCreator" particle="jField:'btnCreator'" class="easyui-linkbutton"
                                    data-options="iconCls:'icon-yg-add'">点评</a>
                            </td>
                        </tr>
                        <tr></tr>
                    </table>
                </div>
            </div>
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto" data-options="toolbar: '#tb'" title="点评记录">
                <thead>
                    <tr>
                        <th data-options="field:'Reader',width:100">点评人</th>
                        <th data-options="field:'CreateDate',width:100">点评时间</th>
                        <th data-options="field:'Comments',width:300">点评内容</th>
                    </tr>
                </thead>
            </table>

        </div>
        <div data-options="region:'south'" style="height: 200px">
            <uc1:PcFiles runat="server" id="PcFiles" />
        </div>
    </div>

</asp:Content>
