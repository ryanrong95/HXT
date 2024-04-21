<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.TraceComments.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        <%@ Import Namespace="Yahv.Underly" %>
        <%@ Import Namespace="System" %>
        $(function () {
            window.grid = $('#dg').myDatagrid({
                pageSize: 50,
                toolbar: '#tb',
                nowrap: false,
                fitColumns: true,
                fit: true,
                pagination: false,
            });

            $('#file').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#fileMessge',
                successTarget: '#fileSuccess',
                multiple: true
            });
            if (!jQuery.isEmptyObject(model.files)) {
                var msgr = $("#fileSuccess");
                var ul = $("<ul></ul>");
                for (var index = 0; index < model.files.length; index++) {
                    var item = model.files[index];
                    var li = $("<li><a href='" + item.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + item.CustomName + "</em> </a></li>");
                    ul.append(li);
                }
                msgr.html(ul);
            };

            $('#Contact').contactCrmPlus({
                required: true,
                isAdd: false
            });

            $("#Name").clientCrmPlus({
                onChange: function (Value) {
                    ////获取当前选中的值，返回json
                    var json = $('#Name').clientCrmPlus('getValue');
                    $('#Contact').contactCrmPlus('setEnterpriseID', json.id);
                }
            });

            $('#Readers').combobox({
                data: model.Readers,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
            });


            //新增
            $("#btnCreator").click(function () {
                $.myDialogFuse({
                    title: '添加',
                    url: 'Add.aspx?TraceID=' + model.Entity.ID,
                    width: "750px",
                    height: "400px",
                    isHaveOk: true,
                    onClose: function () {
                        $('#dg').myDatagrid('flush');
                    }
                });
            })

        })

        //function showCommentContent(data) {
        //    var str = "";//定义用于拼接的字符串
        //    $.each(data.rows, function (index, row) {
        //        if (row.Reader != null) {
        //            str = "<p> 点评人：" + row.Reader + "，&nbsp点评时间：" + row.CreateDate + ",&nbsp;&nbsp;点评内容：" + row.Comments + "</p>"
        //        }
        //        $("#Comment").append(str);
        //    });
        //}
    </script>
    <script type="text/javascript">
        function onSelect1(sd) {
            $('#NextDate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }
        function onSelect2(ed) {
            $('#TraceDate').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
                }
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.TraceRecord entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.TraceRecord;

    %>
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: 213px">

            <table class="liebiao">
                <tr>
                    <td style="width:110px">客户名称：</td>
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
                    <td>本公司陪同人员：</td>
                    <td colspan="3">
                        <%=entity.CompanyStaffs %></td>
                </tr>
                <tr>
                    <td>指定阅读人：</td>
                    <td colspan="3">
                        <%=this.Model.ReaderIDs %></td>
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
                <tr>
                    <td>附件</td>
                    <td colspan="3">
                        <div id="fileSuccess"></div>

                    </td>
                </tr>
            </table>
        </div>

        <div data-options="region:'center'">
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
    </div>
</asp:Content>
