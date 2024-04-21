<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var view = true;
            if (!jQuery.isEmptyObject(model)) {
                addTab('基本信息', 'Detail.aspx?id=' + model.ID + "&enterpriseid=" + model.ID, view);
                addTab('开票信息', 'Invoices/List.aspx?id=' + model.ID);
                addTab('付款信息', 'BookAccounts/List.aspx?id=' + model.ID);
                addTab('地址信息', 'Adress/List.aspx?id=' + model.ID);
                addTab('信用记录', 'Credits/List.aspx?TakerID=' + model.ID);
                addTab('客户联系人', 'Contacts/List.aspx?id=' + model.ID);
                addTab('客户所有人', 'Owner/List.aspx?id=' + model.ID);
                addTab('特殊要求', 'Specials/List.aspx?id=' + model.ID);
                addTab('原厂报备', 'ProjectReports/List.aspx?id=' + model.ID);
                addTab('关联关系', 'BusinessRelations/List.aspx?id=' + model.ID);

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
        var tabs = [{}];
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
