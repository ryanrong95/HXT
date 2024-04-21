<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsClientsDetails.List" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var tabs = [{}];//有个默认项目，建议把“客户基本信息”也开发为一个iframe 项
        $(function () {
            if (!jQuery.isEmptyObject(model.Entity)) {
                addTab('基本信息', 'Edit.aspx?id=' + model.Entity.ID, false);

                var disServiceManager = { dis: model.Entity.ServiceManager == null, tip: "尚未分配业务员" };
                var disContact = { dis: disServiceManager.dis ? true : (model.Entity.Contact == null), tip: disServiceManager.dis ? disServiceManager.tip : (model.Entity.Contact == null ? "联系人信息不全" : "") };

                var disContract = { dis: disServiceManager.dis || disContact.dis || model.Entity.Invoice == null, tip: disServiceManager.dis ? disServiceManager.tip : (disContact ? disContact.tip : model.Entity.Invoice == null ? "发票信息不全" : "") };

                addTab('联系人管理', '../Contacts/WsEdit.aspx?id=' + model.Entity.ID + '&enterprisetype=<%=(int)EnterpriseType.WsClient%>', disServiceManager.dis || model.Standard, disServiceManager.tip);


                addTab('发票管理', '../Invoices/WsEdit.aspx?id=' + model.Entity.ID + '&enterprisetype=<%=(int)EnterpriseType.WsClient%>', disContact.dis || model.Standard, disContact.tip);
                if (model.IsPersonal)
                {
                     addTab('个人客户发票', '../vInvoices/List.aspx?id=' + model.Entity.ID, false, disContact.tip);
                }


                addTab('代报关协议', '../Contracts/List.aspx?id=' + model.Entity.ID + "&CompanyID=" + model.Entity.Company.ID, disContract.dis || model.Standard, disContract.tip);
                addTab('代仓储协议', '../WsContracts/List1.aspx?id=' + model.Entity.ID + "&CompanyID=" + model.Entity.Company.ID, disContract.dis || model.Standard, disContract.tip);

                addTab('收件地址管理', '../WsConsignees/List.aspx?id=' + model.Entity.ID + '&enterprisetype=' + '<%=(int)EnterpriseType.WsClient%>', disContact.dis || model.Standard, disContact.tip);

                addTab('代仓储供应商', '../nSuppliers/List.aspx?id=' + model.Entity.ID, disContact.dis || model.Standard);

                addTab('会员账号', '../SiteUsers/List.aspx?id=' + model.Entity.ID + '&enterprisetype=<%=(int)EnterpriseType.WsClient%>', false, null);

                addTab('付款人', '../Payers/List.aspx?id=' + model.Entity.ID, disContact.dis || model.Standard, null);

            }

            var sender = $('#tt');
            sender.tabs({
                onSelect: function (title, index) {
                    var id = tabs[index + 1].id;
                    var src = $('#' + id).prop('src');
                    if (src == null || src.length == 0) {
                        $('#' + id).attr('src', tabs[index + 1].src);
                    }
                    //if (tabs[index + 1].dis) {
                    //    $.messager.alert("提示", tabs[index + 1].tip);
                    //}
                    //else {
                    //    // 判断 src 是否有值？ 如果为真，就跳出。否则，就做如下操作
                    //    if (src == null || src.length == 0) {
                    //        $('#' + id).attr('src', tabs[index + 1].src);
                    //    }
                    //}
                }
            });
        })

        function addTab(title, url, dis, tip) {
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
                    cache: false,
                    //disabled: dis
                    //tools: [{
                    //    iconCls: 'icon-help',
                    //    handler: function () {
                    //        $.messager.alert("提示", tip);
                    //    }
                    //}]
                });


                tabs.push({
                    id: id,
                    src: url,
                    dis: dis,
                    tip: tip
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
