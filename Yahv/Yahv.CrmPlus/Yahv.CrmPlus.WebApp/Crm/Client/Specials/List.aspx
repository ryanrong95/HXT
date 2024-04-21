<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Specials.List" %>

<%@ Import Namespace="Yahv.Underly" %>
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
                toolbar: '#tb',
                queryParams: getQuery(),
                pagination: false,
                fit: true,
                nowrap: false,
                singleSelect: false,
                rownumbers: true

            });
            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: "新增",
                    url: 'Edit.aspx?&enterpriseid=' + model.ID, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "50%",
                    height: "60%",
                });
            });

        });

        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }

        }
        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)DataStatus.Normal%>') {
                arry.push('<a id="btnUnable" href="#" particle="Name:\'停用\',jField:\'btnUnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            } else if (rowData.Status == '<%=(int)DataStatus.Closed%>') {
                arry.push('<a id="btnEnable" href="#" particle="Name:\'启用\',jField:\'btnEnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }

        function operation(value, row, index) {

            if (value != null) {
                return '<a id="btnView" name="btnView" href="javascript:void(0);"  onclick="View(\'' + row.Url + '\')">' + value + '</a>';

            }
        }

        function Closed(id) {
            $.messager.confirm('确认', '确定停用吗？', function (r) {
                if (r) {
                    $.post('?action=Closed', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已停用!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                        // grid.myDatagrid('search', getQuery());
                    });
                }
            });
        }
        function Enable(id) {
            $.messager.confirm('确认', '确定启用吗？', function (r) {
                if (r) {
                    $.post('?action=Enable', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "启用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8"><a id="btnCreator" particle="Name:'新增',jField:'btnCreator'"  class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'SpecialType',width:200">特殊类型</th>
                <th data-options="field:'Content',width:200">说明</th>
                <th data-options="field:'FileName',width:180,formatter:operation">上传附件</th>
                <th data-options="field:'Creator',width:220">创建人</th>
                <th data-options="field:'CreteDate',width:220">创建时间</th>
                <th data-options="field:'StatusDes',width:80">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:180">操作</th>
            </tr>
        </thead>
    </table>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 550px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
