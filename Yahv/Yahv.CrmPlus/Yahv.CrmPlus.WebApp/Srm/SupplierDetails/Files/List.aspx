<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Files.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data'
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                //toolbar: '#tb',
                pagination: true,
                singleSelect: true,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
        })
        function btnformatter(value, row) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="preview(\'' + row.Url + '\')">预览</a> ');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="filedownLoad(\'' + row.Url + '\')">下载</a> ');
            arry.push('</span>');
            return arry.join(' ');
            //return value;
        }
         <% string virtualPath = HttpRuntime.AppDomainAppVirtualPath;%>;
        function preview(url) {
            var param = "?src=" + url;
            window.open('<%=virtualPath %>' + "/snapshot.html" + param);
        }
        function filedownLoad(url) {
            var $a = $("<a></a>").attr("href", url).attr("download", encodeURI(url));
            $a[0].click();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'Type',width:180">文件类型</th>
                <th data-options="field:'CustomName',width:300">文件</th>
                <th data-options="field:'CreatorName',width:100">创建人</th>
                <th data-options="field:'CreateDate',width:120">上传时间</th>
                <th data-options="field:'btn',width:150,formatter:btnformatter">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
