<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="Tab.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Tab" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var Source = "";

        $(function () {
            $('#tt').tabs({
                border: false,
                tabWidth: 100,
                onSelect: function (title) {
                    //var iframe = null;// 
                    //if (title == "劳资信息") {
                    //    iframe = $('#Labour')[0];
                    //}
                    //if (iframe) {
                    //    iframe = (iframe.contentWindow || iframe.contentDocument);// 得到iframe窗口内容
                    //    iframe.location.reload(true); // 刷新整个页面列表
                    //}
                }
            });

            addTab("基本信息", "Edit.aspx?ID=" + id, "Personal", false);
            addTab("公司信息", "Company.aspx?ID=" + id, "Company", false);
            addTab("劳资信息", "Labour.aspx?ID=" + id, "Labour", false);
            addTab("文件信息", "File.aspx?ID=" + id, "File", false);

            $('#tt').tabs('select', 0);//第一个选中

            $('#Personal').parent().css("overflow", "hidden");
            $('#Company').parent().css("overflow", "hidden");
            $('#Labour').parent().css("overflow", "hidden");
            $('#File').parent().css("overflow", "hidden");
        });
    </script>
    <script>
        function addTab(title, href, id, dis) {
            var tt = $('#tt');
            if (tt.tabs('exists', title)) {
                //如果tab已经存在,则选中并刷新该tab                   
                tt.tabs('select', title);
                refreshTab({ tabTitle: title, url: href });
            } else {
                var content = "";
                if (href) {
                    content = '<iframe id="' + id + '" scrolling="no" frameborder="0" data-source="' + Source + '"  src="' + href + '"style="width:100%;height:100%;"></iframe>';
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
        //刷新tab
        function RefreshTab(parent, title, href, id, height) {
            if ($('#' + parent).tabs('exists', title)) {
                $('#' + parent).tabs('close', title);
            }
            AddTab(parent, title, href, id, height);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" id="tt" class="easyui-tabs" style="border: none">
        </div>
    </div>
</asp:Content>
