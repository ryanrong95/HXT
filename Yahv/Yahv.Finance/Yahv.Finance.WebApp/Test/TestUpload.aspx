<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="TestUpload.aspx.cs" Inherits="Yahv.Finance.WebApp.Test.TestUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#uploadFile').filebox({
                multiple: true,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if (val == '') {
                        return;
                    }

                    var $this = $(this);
                    //验证文件大小
                    if ($this.next().attr("class").indexOf("textbox-invalid") > 0) {
                        $.messager.alert('提示', '文件大小不能超过500kb！');
                        return;
                    }
                    //验证文件类型
                    var type = $this.filebox('options').accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    var ext = val.substr(val.lastIndexOf(".") + 1);
                    if (type.indexOf(ext.toLowerCase()) < 0) {
                        $this.filebox('setValue', '');
                        $.messager.alert('提示', "请选择" + type + "格式的文件！");
                        return;
                    }
                    
                    var files = $("#uploadFile").filebox("files");
                    var formData = new FormData();
                    for (var i = 0; i < files.length; i++) {
                        formData.append("Filedata" + i, files[i]);
                    }

                    $.ajax({
                        url: 'http://hv.erp.b1b.com/FinanceApi/Upload/UploadFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            var resString = '';
                            for(i = 0; i < res.length; i++) {
                                resString += "FileID = " + res[i].FileID + ", FileName = " + res[i].FileName 
                                                + ", SessionID = " + res[i].SessionID + ", Url = " + res[i].Url + "<br>";
                            }
                            $.messager.alert('提示', resString);
                        }
                    }).done(function (res) {

                    });

                    /*
                    var formData = new FormData($('#form1')[0]);
                    $.ajax({
                        url: 'http://hv.erp.b1b.com/FinanceApi/Upload/UploadFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            //var resJson = JSON.parse(res);
                            $.messager.alert('提示', "FileID = " + res[0].FileID + ", FileName = " + res[0].FileName 
                                                + ", SessionID = " + res[0].SessionID + ", Url = " + res[0].Url);
                        }
                    }).done(function (res) {

                    });
                    */

                }
            });


        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="margin-left: 20px; margin-top: 20px;">
        <div>
            <label>sdsf234</label>
        </div>
        <div>
            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
        </div>
    </div>
</asp:Content>
