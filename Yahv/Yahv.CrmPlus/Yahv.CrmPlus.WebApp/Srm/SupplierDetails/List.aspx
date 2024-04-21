<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/mytabs.js"></script>
    <script>
        $(function () {
            addTab('基础信息', "Edit.aspx?enterpriseid=" + model.SupplierID);
            addTab('收款信息', "BookAccounts/List.aspx?id=" + model.EnterpriseID);
            addTab('信用记录', 'Credits/List.aspx?MakerID='+model.EnterpriseID);
            //addTab('结算管理', '');
            addTab('联系人', "Contacts/List.aspx?id=" + model.EnterpriseID);
            addTab('代理品牌', "AgentBrands/List.aspx?id=" + model.EnterpriseID);
            addTab('特色维护', "Specials/List.aspx?id=" + model.EnterpriseID);
            addTab('固定渠道', "FixedSuppliers/List.aspx?enterpriseid=" + model.EnterpriseID + '&subid=' + model.EnterpriseID);
            addTab('返款信息', "Commissions/List.aspx?supplierid=" + model.SupplierID);
            addTab('关联公司', "BusinessRelations/List.aspx?id=" + model.EnterpriseID);
            //addTab('下单公司', "");
            addTab('文件信息', "Files/List.aspx?enterpriseid=" + model.EnterpriseID);

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
    <div id="tt" class="easyui-tabs" data-options="fit:true"></div>
</asp:Content>
