<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.ClientDetails.List" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var tabs = [{}];//有个默认项目，建议把“客户基本信息”也开发为一个iframe 项
        $(function () {
            if (!jQuery.isEmptyObject(model.Entity)) {
                addTab('基本信息', 'Edit.aspx?id=' + model.Entity.ID);
                addTab('到货地址管理', '../Consignees/List.aspx?id=' + model.Entity.ID + '&enterprisetype=<%=(int)EnterpriseType.Client%>');
                addTab('发票管理', '../Invoices/List.aspx?id=' + model.Entity.ID + '&enterprisetype=<%=(int)EnterpriseType.Client%>');
                addTab('联系人管理', '../Contacts/List.aspx?id=' + model.Entity.ID + '&enterprisetype=<%=(int)EnterpriseType.Client%>');
                <%-- addTab('合作公司(线上销售业务)', '../Companies/Selected_Companies.aspx?id=' + model.ID + '&type=' + '<%=(int)CooperType.Online %>');
                addTab('合作公司(传统贸易)', '../Companies/Selected_Companies.aspx?id=' + model.ID + '&type=' + '<%=(int)CooperType.OldTrade %>');--%>
                if (model.IsAssignSale) {
                    addTab('分配销售人', '../Admins/Selected_Admins.aspx?id=' + model.Entity.ID);
                    //addTab('分配销售', '../Admins/List.aspx?id=' + model.Entity.ID);
                }

            }

            var sender = $('#tt');
            sender.tabs({
                onSelect: function (title, index) {
                    var id = tabs[index + 1].id;
                    var src = $('#' + id).prop('src');
                    // 判断 src 是否有值？ 如果为真，就跳出。否则，就做如下操作
                    if (src == null || src.length == 0) {
                        $('#' + id).attr('src', tabs[index + 1].src);
                    }
                }
            });
        })

        function addTab(title, url) {
            var sender = $('#tt');
            if (sender.tabs('exists', title)) {
                sender.tabs('select', title);
            } else {
                var id = '_' + Math.random().toString().substring(2);
                var content = '<iframe id="' + id + '" scrolling="auto" frameborder="0"'
                     + ' style="width:100%;height:100%;"></iframe>';

                sender.tabs('add', {
                    title: title,
                    content: content,
                    closable: false,
                    selected: false,
                    cache: false
                });

                tabs.push({
                    id: id,
                    src: url,
                });


                $('#' + id).parent('div').css({ 'overflow': 'hidden' });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-tabs" id="tt" data-options="fit:true">
    </div>
</asp:Content>
