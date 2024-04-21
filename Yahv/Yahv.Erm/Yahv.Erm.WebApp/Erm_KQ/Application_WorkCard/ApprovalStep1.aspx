<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ApprovalStep1.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_WorkCard.ApprovalStep1" %>

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
            $("#Position").combobox({
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostionData,
            })
            //借用日期（只能选当天或之后的日期）
            $('#AcceptDate').datebox('calendar').calendar({
                validator: function (date) {
                    var now = new Date();
                    var d1 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    return date >= d1;
                },
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
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Argument', $("#Argument").textbox("getValue"));
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));

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
            //驳回
            $("#btnReject").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Argument', $("#Argument").textbox("getValue"));

                ajaxLoading();
                $.ajax({
                    url: '?action=Reject',
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
                                $("#Position").combobox('setValue', result.data.position);
                                $("#SelCode").textbox('setValue', result.data.selcode);
                            }
                            else
                            {
                                $("#Department").combobox('setValue',"");
                                $("#Manager").combobox('setValue', "");
                                $("#Position").combobox('setValue', "");
                                $("#SelCode").textbox('setValue', "");
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
                                $("#Position").combobox('setValue', result.data.position);
                                $("#SelCode").textbox('setValue', result.data.selcode);
                            }
                            else
                            {
                                $("#Department").combobox('setValue',"");
                                $("#Manager").combobox('setValue', "");
                                $("#Position").combobox('setValue', "");
                                $("#SelCode").textbox('setValue', "");
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
                $("#AcceptDate").datebox('setValue', model.ApplicationData.AcceptDate);
                $("#Reason").textbox('setValue', model.ApplicationData.Reason);
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">补办员工：</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 350px;" />
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
                    <td class="lbl">岗位名称：</td>
                    <td>
                        <input id="Position" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">工牌编号：</td>
                    <td>
                        <input id="SelCode" class="easyui-textbox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">补办理由：</td>
                    <td>
                        <input id="Reason" class="easyui-textbox" style="width: 350px; height: 40px"
                            data-options="required:true,multiline:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">受理日期：</td>
                    <td>
                        <input id="AcceptDate" class="easyui-datebox" style="width: 350px;" data-options="disabled: true"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">文件信息：</td>
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
                <tr>
                    <td class="lbl" style="color: red">审批意见：</td>
                    <td>
                        <input id="Argument" class="easyui-textbox" style="width: 350px; height: 40px"
                            data-options="multiline:true" />
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
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">同意</a>
                <a id="btnReject" class="easyui-linkbutton" iconcls="icon-back">驳回</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%";>
            <img id="viewfileImg" src="" style="position:relative; zoom:100%; cursor:move;" onMouseEnter="mStart();" onMouseOut="mEnd();"/>
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>
