<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Delivery.Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var Source = "";

        $(function () {
            $('#tt').tabs({
                border: false,
                tabWidth: 100,
            });

            addTab("基本信息", "Details/Product.aspx?ID=" + id, "Product", false);
            addTab("运单信息", "../Common/OutWayBill.aspx?ID=" + id, "WayBill", false);
            addTab("开票信息", "../Common/Invoice.aspx?ID=" + id, "Invoice", false);
            addTab("文件信息", "../Common/File.aspx?ID=" + id, "File", false);
            addTab("账单明细", "../Common/Bill.aspx?ID=" + id, "Bill", false);

            $('#tt').tabs('select', 0);//第一个选中

            $('#Product').parent().css("overflow", "hidden");
            $('#WayBill').parent().css("overflow", "hidden");
            $('#Invoice').parent().css("overflow", "hidden");
            $('#File').parent().css("overflow", "hidden");
            $('#Bill').parent().css("overflow", "hidden");
        });

        function addTab(title, href, id, dis) {
            var tt = $('#tt');
            if (tt.tabs('exists', title)) {
                //如果tab已经存在,则选中并刷新该tab                   
                tt.tabs('select', title);
                refreshTab({ tabTitle: title, url: href });
            } else {
                var content = "";
                if (href) {
                    content = '<iframe id="' + id + '" scrolling="no" frameborder="0" data-source="' + Source + '"  src="' + href + '"style="width:100%;height:100%;padding: 2px"></iframe>';
                }
                else {
                    content = '未实现';
                }
                tt.tabs('add', {
                    title: title,
                    closable: false,
                    content: content,
                });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" id="tt" class="easyui-tabs" style="border:none">
        </div>
    </div>
</asp:Content>
