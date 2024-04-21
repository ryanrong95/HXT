<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_SealEngrave.Details" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        var ID = getQueryString("ID");
        var AdminID = getQueryString("AdminID");

        $(function () {
            $("#Staff").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $("#Department").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
            })
            $("#Manager").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $("#SealShape").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.SealShape,
                onChange: function () {
                    var value = $("#SealShape").combobox("getValue");
                    if (value == "其它") {
                        $(".Dec").css("display", 'table-row');
                    }
                    else {
                        $(".Dec").css("display", 'none');
                    }
                }
            })
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
                pagination: false,
                actionName: 'LoadLogs',
            });
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化申请
        function Init() {
            if (model.ApplicationData != null) {
                $("#Staff").combobox('setValue', model.ApplicationData.ApplicantID);
                $("#SealName").textbox('setValue', model.ApplicationData.SealName);
                $("#SealShape").combobox('setValue', model.ApplicationData.SealShape);
                $("#SealShapeDec").textbox('setValue', model.ApplicationData.SealShapeDec);
                $("#SealSize").textbox('setValue', model.ApplicationData.SealSize);
                $("#Reason").textbox('setValue', model.ApplicationData.Reason);

                $("#Manager").combobox('setValue', model.ApplicationData.ApproveID);
                $("#Department").combobox('setValue', model.ApplicationData.DepartmentName);
            }
            else {
                $("#btnSave").css("display", "inline-block");
            }
        }
    </script>
    <script>
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../../Content/Template/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>';
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
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">申请员工：</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">员工部门：</td>
                    <td>
                        <input id="Department" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">部门负责人：</td>
                    <td>
                        <input id="Manager" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">印章名称：</td>
                    <td>
                        <input id="SealName" class="easyui-textbox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">印章形状：</td>
                    <td>
                        <input id="SealShape" class="easyui-combobox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr class="Dec" style="display: none">
                    <td class="lbl">形状描述：</td>
                    <td>
                        <input id="SealShapeDec" class="easyui-textbox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">印章规格：</td>
                    <td>
                        <input id="SealSize" class="easyui-textbox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">刻章事由：</td>
                    <td>
                        <input id="Reason" class="easyui-textbox" style="width: 350px; height: 40px"
                            data-options="required:true,multiline:true,disabled: true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">印章模板：</td>
                    <td>
                        <div class="file" style="width: 500px">
                            <table id="file">
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
            <table id="logs" title="审批日志">
                <thead>
                    <tr>
                        <th data-options="field:'CreateDate',align:'left'" style="width: 80px">审批时间</th>
                        <th data-options="field:'VoteStepName',align:'left'" style="width: 50px">审批步骤</th>
                        <th data-options="field:'AdminID',align:'left'" style="width: 50px">审批人</th>
                        <th data-options="field:'Status',align:'left'" style="width: 50px;">审批结果</th>
                        <th data-options="field:'Summary',align:'left'" style="width: 200px">审批意见</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%">
            <img id="viewfileImg" src="" style="position: relative; zoom: 100%; cursor: move;" onmouseenter="mStart();" onmouseout="mEnd();" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>
