<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Details.List" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var tabs = [{}];//有个默认项目，建议把“客户基本信息”也开发为一个iframe 项
        $(function () {
            if (!jQuery.isEmptyObject(model.Carrier)) {
                addTab('基本信息', 'Edit.aspx?id=' + model.Carrier.ID);
                addTab('司机', '../Drivers/List.aspx?id=' + model.Carrier.ID);
                addTab('运输工具', '../Transports/List.aspx?id=' + model.Carrier.ID);
                addTab('收款人', '../Beneficiaries/List.aspx?id=' + model.Carrier.ID);
                addTab('联系人', '../Contacts/List.aspx?id=' + model.Carrier.ID);
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
