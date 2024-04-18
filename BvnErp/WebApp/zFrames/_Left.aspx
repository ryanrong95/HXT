<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_Left.aspx.cs" Inherits="WebApp.zFrames._Left" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        p {
            border-bottom: dashed 1px #333;
            padding: 5px;
            cursor: pointer;
        }
    </style>
    <!--功能菜单-->
    <script>
        $(function () {
            var senders = $('.easyui-accordion p');
            senders.click(function () {
                senders.removeClass('accordion-header-selected');
                $(this).addClass('accordion-header-selected');
            });
        });
    </script>

    <script>
        function redirect(url) {
            if (top == window) {
                alert('不能如此');
            }

            top.document.getElementById('ifrmain').src = url;
        }

        function allot() {
            var nodes = $('#tt').tree('getSelected').id;
            redirect('/_Faces/_Erps/_Staffs/Items.aspx?id=' + nodes + '&_' + Math.random());
            //window.newWin = window.open('/_Faces/_Erps/_Inners/_Admins/Allot.aspx?id=' + nodes + '&_' + Math.random(), '', 'width:700,height:400,location:no,depended:yes');
        }
        function showOrder() {
            var nodes = $('#tt').tree('getSelected').id;
            alert(nodes + " 的订单");
        }
        function granulate() {
            var nodes = $('#tt').tree('getSelected').id;
            redirect('/_Faces/_Erps/_Granulates/Edit.aspx?id=' + nodes + '&_' + Math.random());
        }
        function menu_gran() {
            var nodes = $('#tt').tree('getSelected').id;
            redirect('/_Faces/_Erps/_Granulates/Menu.aspx?id=' + nodes + '&_' + Math.random());
        }
        parent.treeload = function () {
            $('#tt').tree('reload');
        }
    </script>
</head>
<body <%--class="easyui-layout"--%>>
    <div class="easyui-tabs" data-options="fit:true">
        <div title="功能菜单">
            <div id="menu" class="easyui-accordion" data-options="border:false,multiple:true">

                <% 
                    var menus = this.Model as NtErp.Services.Models.Menu[];
                    if (menus != null && menus.Count() > 0)
                    {
                        foreach (var top in menus.Where(t => t.FatherID == null).OrderBy(t=>t.OrderIndex))
                        {
                %>
                <div title="<%=top.Name %>" data-options="iconCls:'icon-ok'" style="overflow: auto; padding: 10px;">
                    <%
                        foreach (var sub in menus.Where(item => item.FatherID == top.ID).OrderBy(t=>t.OrderIndex))
                        {
                    %>
                    <p onclick="redirect('<%=sub.Url %>');"><span><%=sub.Name %></span></p>
                    <%
                        }
                    %>
                </div>
                <%
                        }
                    }
                %>
            </div>
        </div>
        <%--<div title="我的员工">
            <div class="easyui-panel" style="padding: 5px" data-options="border:false">
                <ul id="tt" class="easyui-tree"></ul>
            </div>
        </div>--%>
    </div>

    <div id="mm" class="easyui-menu" style="width: 120px;">
        <div onclick="showOrder()">订单</div>
        <div class="menu-sep"></div>
        <div onclick="allot()">添加组员</div>
        <div onclick="granulate()">颗粒化</div>
        <div onclick="menu_gran()">菜单颗粒化</div>
    </div>

    <form id="form1" runat="server">
    </form>
</body>
</html>

