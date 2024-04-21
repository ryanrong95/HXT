<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="File.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.File" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
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
            //文件类型
            $("#fileType").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.FileType,
            })
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
                    formData.append("StaffID", StaffID);
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
            })

        });
    </script>
    <script>
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-search\'" onclick="View(\'' + row.Url + '\');return false;">查看</a> ')
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;">删除</a> ')
            buttons.push('</span>')
            return buttons.join('');
        }
        //上传文件
        function UploadFile(formData) {
            $.ajax({
                url: '?action=uploadFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $("#tab1").myDatagrid('flush');
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
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
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('xls') > 0) {
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
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        $("#tab1").myDatagrid('flush');
                    })
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'north',height:40">
            <div style="float: left; margin-left: 5px; margin-top: 8px;">
                <input id="fileType" class="easyui-combobox" style="width: 150px;" data-options="prompt:'文件类型'" />
                <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
            </div>
        </div>
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
