<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="FileDetails.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail.FileDetails" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../../Content/Script/file.js"></script>
    <script>
        var StaffID = getQueryString("ID");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                rownumbers: true,
                singleSelect: true,
                fitColumns: true,
                onLoadSuccess: function (data) {
                }
            });
        });
    </script>
    <script>
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-search\'" onclick="View(\'' + row.Url + '\');return false;">查看</a> ')
            buttons.push('</span>')
            return buttons.join('');
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1">
                <thead>
                    <tr>
                        <th data-options="field:'ID',align:'center'" style="width: 50px">编号</th>
                        <th data-options="field:'CustomName',align:'left'" style="width: 100px">文件名称</th>
                        <th data-options="field:'FileType',align:'center'" style="width: 50px">文件类型</th>
                        <th data-options="field:'CreateDate',align:'center'" style="width: 50px">上传日期</th>
                        <th data-options="field:'Creater',align:'center'" style="width: 50px">上传人</th>
                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 50px;">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%;min-width:700px;min-height:450px">
            <img id="viewfileImg" src="" style="position: relative; zoom: 100%; cursor: move;" onmouseenter="mStart();" onmouseout="mEnd();" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>
