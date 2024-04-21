<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_ArchiveDestroy.Edit" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        var ID = getQueryString("ID");
        var AdminID = getQueryString("AdminID");

        $(function () {
            InitStaff();
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
                data: model.ManageData,
            })
            $("#Supervisor").combobox({
                required: false,
                editabled: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
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
            $('#uploadFile').filebox({
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传',
                buttonIcon: 'icon-yg-add',
                width: 58,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadFile').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
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
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
                            //上传文件
                            UploadFile(formData);
                        }
                    }
                }
            })
            //档案内容
            $("#archives").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadArchives',
                onClickRow: onClickRow,
                columns: [[
                    { field: 'Year', title: '档案年度', width: 150, align: 'center', editor: { type: 'textbox', options: { validType: 'length[4,4]', required: true } } },
                    { field: 'Type', title: '档案类别', width: 150, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]', required: true } } },
                    { field: 'Code', title: '档案编号', width: 150, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]', required: true } } },
                    { field: 'Context', title: '档案内容', width: 200, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]', required: true } } },
                    { field: 'Count', title: '档案数量', width: 150, align: 'center', editor: { type: 'numberbox', options: { min: 1, required: true } } },
                    { field: 'Btn', title: '操作', width: 150, align: 'center', formatter: OperationDate }
                ]],
            });
            //审批日志
            $("#logs").myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadLogs',
            });
            //提交
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                if ($("#Manager").combobox("getText") == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "负责人不能为空", type: "error" });
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Staff', $("#Staff").combobox("getValue"));
                data.append('Manager', $("#Manager").combobox("getValue"));
                data.append('ManagerName', $("#Manager").combobox("getText"));
                data.append('Supervisor', $("#Supervisor").combobox("getValue"));
                data.append('SupervisorName', $("#Supervisor").combobox("getText"));
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                //档案名称
                var archive = $('#archives').datagrid('getRows');
                var archives = [];
                for (var i = 0; i < archive.length; i++) {
                    archives.push(archive[i]);
                }
                data.append('archives', JSON.stringify(archives));

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            //保存草稿
            $("#btnSave").click(function () {
                endEditing();
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                if ($("#Manager").combobox("getText") == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "负责人不能为空", type: "error" });
                    return false;
                }
                if ($("#Staff").combobox("getText") == $("#Supervisor").combobox("getText")) {
                    top.$.timeouts.alert({ position: "TC", msg: "监销人不能是申请人", type: "error" });
                    return false;
                }
                if ($('#archives').datagrid('getRows').length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "请填写销毁档案内容", type: "error" });
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('Staff', $("#Staff").combobox("getValue"));
                data.append('Manager', $("#Manager").combobox("getValue"));
                data.append('ManagerName', $("#Manager").combobox("getText"));
                data.append('Supervisor', $("#Supervisor").combobox("getValue"));
                data.append('SupervisorName', $("#Supervisor").combobox("getText"));
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
                //销毁档案内容
                var archive = $('#archives').datagrid('getRows');
                var archives = [];
                for (var i = 0; i < archive.length; i++) {
                    archives.push(archive[i]);
                }
                data.append('archives', JSON.stringify(archives));

                ajaxLoading();
                $.ajax({
                    url: '?action=SaveDraft',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //添加行程
            $("#btnAddArchive").click(function () {
                AddArchive();
            })
            //清除行程
            $("#btnClearArchive").click(function () {
                var rows = $("#archives").myDatagrid("getRows");
                if (rows.length > 0) {
                    for (var i = rows.length - 1; i >= 0; i--) {
                        $('#archives').datagrid('deleteRow', i);
                    }
                }
            })
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化申请员工
        function InitStaff() {
            if (CheckIsNullOrEmpty(AdminID)) {
                $("#Staff").combobox({
                    required: true,
                    disabled: true,
                    valueField: 'Value',
                    textField: 'Text',
                    data: model.StaffData,
                    onChange: function () {
                        var name = $("#Staff").combobox('getValue');
                        $.post('?action=StaffChange', { Name: name }, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                $("#Department").combobox('setValue', result.data.department);
                                $("#Manager").combobox('setValue', result.data.manager);
                            }
                            else {
                                $("#Department").combobox('setValue', "");
                                $("#Manager").combobox('setValue', "");
                            }
                        })
                    }
                })
            }
            else {
                $("#Staff").combobox({
                    required: true,
                    editable: false,
                    valueField: 'Value',
                    textField: 'Text',
                    data: model.StaffData,
                    onChange: function () {
                        var name = $("#Staff").combobox('getValue');
                        $.post('?action=StaffChange', { Name: name }, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                $("#Department").combobox('setValue', result.data.department);
                                $("#Manager").combobox('setValue', result.data.manager);
                            }
                            else {
                                $("#Department").combobox('setValue', "");
                                $("#Manager").combobox('setValue', "");
                            }
                        })
                    }
                })
            }
            $("#Staff").combobox('setValue', AdminID);
        }
        //初始化申请
        function Init() {
            if (model.ApplicationData != null) {
                $("#Staff").combobox('setValue', model.ApplicationData.ApplicantID);
                $("#Supervisor").combobox('setValue', model.ApplicationData.Supervisor);
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
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>'
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
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#archives').datagrid('validateRow', editIndex)) {
                $('#archives').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#archives').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#archives').datagrid('selectRow', editIndex);
                }
            }
            else {
                endEditing();
                loadData();
            }
        }
        function OperationDate(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        //删除行
        function Delete(index) {
            if (editIndex != undefined) {
                $('#archives').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);
                editIndex = undefined;
            }
            $('#archives').datagrid('deleteRow', index);
            loadData()
        }
        //添加行
        function AddArchive() {
            if (endEditing()) {
                $('#archives').datagrid('appendRow', {});
                editIndex = $('#archives').datagrid('getRows').length - 1;
                $('#archives').datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
            }
        }
        //重新加载数据，作用：刷新列表操作按钮的样式
        function loadData() {
            var data = $('#archives').datagrid('getData');
            $('#archives').datagrid('loadData', data);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div id="topper" style="padding: 5px">
            <a id="btnAddArchive" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
            <a id="btnClearArchive" class="easyui-linkbutton" iconcls="icon-yg-clear">清除</a>
        </div>
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">申请人：</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">申请部门：</td>
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
                    <td class="lbl">监销人：</td>
                    <td>
                        <input id="Supervisor" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">相关文件：</td>
                    <td>
                        <div>
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
                        </div>
                        <div class="file" style="width: 500px">
                            <table id="file">
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
            <table id="archives" title="销毁档案内容">
            </table>
            <br />
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
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save" style="display: none">保存草稿</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%">
            <img id="viewfileImg" src="" style="position: relative; zoom: 100%; cursor: move;" onmouseenter="mStart();" onmouseout="mEnd();" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>
