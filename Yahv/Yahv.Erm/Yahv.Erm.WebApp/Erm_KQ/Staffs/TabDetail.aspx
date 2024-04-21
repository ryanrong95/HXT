<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="TabDetail.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.TabDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var Source = "";
        $(function () {
            $('#tt').tabs({
                border: false,
                tabWidth: 100,
                onSelect: function (title) {
                }
            });

            addTab("基本信息", "Detail/Details.aspx?ID=" + id, "Personal", false);
            addTab("公司信息", "Detail/CompanyDetails.aspx?ID=" + id, "Company", false);
            addTab("劳资信息", "Detail/LabourDetails.aspx?ID=" + id, "Labour", false);
            addTab("文件信息", "Detail/FileDetails.aspx?ID=" + id, "File", false);
            $('#tt').tabs('select', 0);//第一个选中
            $('#Personal').parent().css("overflow", "hidden");
            $('#Company').parent().css("overflow", "hidden");
            $('#Labour').parent().css("overflow", "hidden");
            $('#File').parent().css("overflow", "hidden");
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
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
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
