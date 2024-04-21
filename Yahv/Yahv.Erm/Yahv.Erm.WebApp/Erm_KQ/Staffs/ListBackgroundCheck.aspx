<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListBackgroundCheck.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ListBackgroundCheck" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
                rownumbers: true,
                onCheck: function () {
                    $("#btnBackgroundInvestigation").linkbutton('enable');//启用
                    $("#uploadFile").filebox('enable');//启用
                }
            });
            //员工背景调查报告
            $("#btnBackgroundInvestigation").click(function () {
                var row = $("#tab1").datagrid('getChecked');
                if (row.length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "请勾选员工信息", type: "error" });
                    return;
                }

                var formData = new FormData($('#form1')[0]);
                formData.append("ID", row[0].ID);
                ajaxLoading();
                $.ajax({
                    url: '?action=ExportBackgroundInvestigation',
                    type: 'POST',
                    data: formData,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('消息', res.message, 'info', function () {
                            if (res.success) {
                                //下载文件
                                try {
                                    let a = document.createElement('a');
                                    a.href = res.fileUrl;
                                    a.download = "";
                                    a.click();
                                } catch (e) {
                                    console.log(e);
                                }
                            }
                        });
                    }
                })
            });
            $('#uploadFile').filebox({
                validType: ['fileSize[3,"MB"]'],
                buttonText: '上传背景调查报告',
                buttonIcon: 'icon-yg-add',
                width: 130,
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
                    var row = $("#tab1").datagrid('getChecked');
                    formData.append("StaffID", row[0].ID);
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
        });
    </script>
    <script>
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
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        $.myWindow.close();
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                    $("#tab1").myDatagrid('flush');
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td> 
                    <a id="btnBackgroundInvestigation" class="easyui-linkbutton" data-options="disabled:true" iconcls="icon-yg-excelExport">导出背景调查报告</a>
                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" data-options="disabled:true" />
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="员工背景调查提醒列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <th data-options="field:'EntryDate',align:'center',width:100">入职日期</th>
                <th data-options="field:'LastBackgroundCheck',align:'center',width:100">上次调查日期</th>
                <th data-options="field:'Code',align:'center',width:100">编号</th>
                <th data-options="field:'SelCode',align:'center',width:100">工号</th>
                <th data-options="field:'Name',align:'center',width:100">姓名</th>
                <th data-options="field:'DepartmentCode',align:'center',width:100">部门</th>
                <th data-options="field:'PostionName',align:'center',width:100">岗位</th>
                <th data-options="field:'Gender',align:'center',width:100">性别</th>
                <th data-options="field:'BirthDate',align:'center',width:100">出生日期</th>
                <th data-options="field:'Volk',align:'center',width:100">民族</th>
                <th data-options="field:'IsMarry',align:'center',width:100">婚姻状况</th>
                <th data-options="field:'IDCard',align:'center',width:150">身份证号</th>
                <th data-options="field:'Education',align:'center',width:100">学历</th>
                <th data-options="field:'GraduatInstitutions',align:'center',width:150">毕业院校</th>
                <th data-options="field:'Major',align:'center',width:100">专业</th>
                <th data-options="field:'Mobile',align:'center',width:100">联系电话</th>
                <th data-options="field:'PassAddress',align:'left',width:250">户口所在地</th>
                <th data-options="field:'HomeAddress',align:'left',width:250">现居地</th>
                <th data-options="field:'EmergencyContact',align:'center',width:100">紧急联系人</th>
                <th data-options="field:'EmergencyMobile',align:'center',width:100">紧急联系人电话</th>
                <th data-options="field:'ContractPeriod',align:'center',width:100">合同期限</th>
                <th data-options="field:'StatusDec',align:'center',width:100">状态</th>
            </tr>
        </thead>
    </table>
</asp:Content>
