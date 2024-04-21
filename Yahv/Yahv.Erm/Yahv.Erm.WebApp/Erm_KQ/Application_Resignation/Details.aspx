<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Resignation.Details" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        $(function () {
            //文件信息
            $('#file').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadFile',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationFile }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".file");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });

            //审批日志
            $("#logs").myDatagrid({
                fitColumns: true,
                fit: false,
                rownumbers: true,
                pagination: false,
                actionName: 'LoadLogs',
                columns: [[
                    { field: 'CreateDate', title: '审批时间', width: fixWidth(15) },
                    { field: 'VoteStepName', title: '审批步骤', width: fixWidth(10) },
                    { field: 'AdminID', title: '审批人', width: fixWidth(10) },
                    { field: 'Status', title: '审批结果', width: fixWidth(10) },
                    { field: 'Summary', title: '审批意见', width: fixWidth(55) }
                ]]
            });

            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            });

            if (model.Data) {
                $("form").form("load", model.Data);
            }
        });
    </script>
    <script>
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../../Content/Template/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '（' + row.FileTypeDec + '）' + '</a>';
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
    <div style="display: none;">
        <input id="ID" name="ID" class="easyui-textbox">
    </div>
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none; width: 100%; height: 100%">
            <table class="liebiao">
                <tr>
                    <td>离职员工</td>
                    <td>
                        <input id="Applicant" name="Applicant" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                    <td>员工部门</td>
                    <td>
                        <input id="DeptName" name="DeptName" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <td>员工岗位</td>
                    <td>
                        <input id="PostionName" name="PostionName" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                    <td>离职日期</td>
                    <td>
                        <input id="ResignationDate" name="ResignationDate" class="easyui-datebox" style="width: 200px;" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <td>部门负责人</td>
                    <td>
                        <input id="DeptLeader" name="DeptLeader" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                    <td>总经理</td>
                    <td>
                        <input id="GeneralManager" name="GeneralManager" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <td>工作承接人</td>
                    <td colspan="3">
                        <input id="Handover" name="Handover" class="easyui-textbox" style="width: 200px;" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">文件信息：</td>
                    <td colspan="3">
                        <div style="display: none;">
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
                        </div>
                        <div class="file" style="width: 500px">
                            <table id="file">
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>离职原因</td>
                    <td colspan="3">
                        <input id="ReasonDescription" name="ReasonDescription" class="easyui-textbox" style="width: 350px; height: 40px;"
                            data-options="multiline:true" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <td>工作内容描述</td>
                    <td colspan="3">
                        <input id="JobDescription" name="JobDescription" class="easyui-textbox" style="width: 350px; height: 60px;"
                            data-options="multiline:true" readonly="readonly" />
                    </td>
                </tr>
            </table>
            <table id="logs" title="审批日志"></table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%";>
            <img id="viewfileImg" src="" style="position:relative; zoom:100%; cursor:move;" onMouseEnter="mStart();" onMouseOut="mEnd();"/>
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>
