<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Resignation.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        $(function () {
            //安排方式
            $('#PostionID').combobox({
                data: model.Postions,
                textField: 'text',
                valueField: 'value'
            });

            //承接人
            $("#HandoverID").combogrid({
                url: "?action=GetAdmins",
                required: false,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "id",
                textField: "name",
                panelWidth: 500,
                panelHeight: 200,
                mode: "local",
                prompt: "承接人",
                columns: [[
                    { field: 'id', title: 'AdminID', width: 100, align: 'left' },
                    { field: 'name', title: '名称', width: 120, align: 'left' },
                    { field: 'department', title: '部门', width: 100, align: 'left' }
                ]]
            });

            //提交
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                if ($("#DeptLeaderID").combobox("getText") == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "负责人不能为空", type: "error" });
                    return false;
                }

                var data = new FormData($('form')[0]);
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                data.set('ReasonDescription', $('#ReasonDescription').textbox('getValue').replace(/[\r\n]/g, "\\r\\n"));
                data.set('JobDescription', $('#JobDescription').textbox('getValue').replace(/[\r\n]/g, "\\r\\n"));

                ajaxLoading();
                $.post({
                    url: '?action=Submit',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.messager.alert('操作提示', '提交成功!', 'info', function () {
                                top.$.myWindow.close();
                            });
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });

                return false;
            });

            //保存草稿
            $('#btnSave').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                if ($("#DeptLeaderID").combobox("getText") == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "负责人不能为空", type: "error" });
                    return false;
                }

                var data = new FormData($('form')[0]);
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                data.set('ReasonDescription', $('#ReasonDescription').textbox('getValue').replace(/[\r\n]/g, "\\r\\n"));
                data.set('JobDescription', $('#JobDescription').textbox('getValue').replace(/[\r\n]/g, "\\r\\n"));

                ajaxLoading();
                $.post({
                    url: '?action=Save',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        if (result.success) {
                            top.$.messager.alert('操作提示', result.data, 'info', function () {
                                top.$.myWindow.close();
                            });
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                            ajaxLoadEnd();
                        }
                    }
                });

                return false;
            });

            //文件类型
            $("#fileType").combobox({
                //required: true,
                editable: false,
                valueField: 'value',
                textField: 'text',
                data: model.FileType
            });

            //上传文件
            $('#uploadFile').filebox({
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传',
                buttonIcon: 'icon-yg-add',
                width: 58,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadFile').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    if ($("#fileType").combobox("getValue") == '') {
                        $.messager.alert('提示', '上传失败，请先选择文件类型');
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    formData.append("fileType", $("#fileType").combobox("getValue"));
                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadFile', bl, fileName);
                                //上传文件
                                UploadFile(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
                            //上传文件
                            UploadFile(formData);
                        }
                    }
                }
            });

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
                    { field: 'FileType', title: '类型', width: 70, align: 'center', hidden: true },
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

                //if (!model.ID) {
                //    $("#logs").hide();
                //    $('#logs').panel('close');
                //}
            }
        });
    </script>
    <script>
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../../Content/Template/images/blue-fujian.png" /> '
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '（' + row.FileTypeDec + '）' + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteFile(' + index + ')">删除</a>';
        }
        //上传文件
        function UploadFile(formData) {
            $.ajax({
                url: '?action=UploadFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = eval(res.data);
                        for (var i = 0; i < data.length; i++) {
                            $('#file').datagrid('insertRow', {
                                row: {
                                    ID: data[i].ID,
                                    CustomName: data[i].CustomName,
                                    FileName: data[i].FileName,
                                    FileType: data[i].FileType,
                                    FileTypeDec: data[i].FileTypeDec,
                                    Url: data[i].Url
                                }
                            });
                        }
                        var data = $('#file').datagrid('getData');
                        $('#file').datagrid('loadData', data);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            })
        }
        //删除文件
        function DeleteFile(index) {
            $('#file').datagrid('deleteRow', index);
            var data = $('#file').datagrid('getData');
            $('#file').datagrid('loadData', data);
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
        <input id="DeptLeaderID" name="DeptLeaderID" class="easyui-textbox">
        <input id="GeneralManagerID" name="GeneralManagerID" class="easyui-textbox">
    </div>
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr>
                    <td>离职员工</td>
                    <td>
                        <input id="Applicant" name="Applicant" class="easyui-textbox" style="width: 200px;" readonly="readonly" data-options="required:true" />
                    </td>
                    <td>员工部门</td>
                    <td>
                        <input id="DeptName" name="DeptName" class="easyui-textbox" style="width: 200px;" readonly="readonly" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td>员工岗位</td>
                    <td>
                        <input id="PostionID" name="PostionID" class="easyui-combobox" style="width: 200px;" data-options="required:true" readonly="readonly" />
                    </td>
                    <td>期望离职日期</td>
                    <td>
                        <input id="ResignationDate" name="ResignationDate" class="easyui-datebox" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td>部门负责人</td>
                    <td>
                        <input id="DeptLeader" name="DeptLeader" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                    </td>
                    <td>总经理</td>
                    <td>
                        <input id="GeneralManager" name="GeneralManager" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                    </td>
                </tr>
                <tr style="display: none;">
                    <td>工作承接人</td>
                    <td colspan="3">
                        <input id="HandoverID" name="HandoverID" class="easyui-combogrid" style="width: 200px;" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">文件信息：</td>
                    <td colspan="3">
                        <div>
                            <input id="fileType" class="easyui-combobox" style="width: 150px;" data-options="prompt:'文件类型'" />
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
                            data-options="required:true,validType:'length[1,500]',multiline:true" />
                    </td>
                </tr>
                <tr>
                    <td>工作内容描述</td>
                    <td colspan="3">
                        <input id="JobDescription" name="JobDescription" class="easyui-textbox" style="width: 350px; height: 60px;"
                            data-options="required:true,validType:'length[1,500]',multiline:true" />
                    </td>
                </tr>
            </table>
            <table id="logs" title="审批日志"></table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save">保存草稿</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%";>
            <img id="viewfileImg" src="" style="position:relative; zoom:100%; cursor:move;" onMouseEnter="mStart();" onMouseOut="mEnd();"/>
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>
