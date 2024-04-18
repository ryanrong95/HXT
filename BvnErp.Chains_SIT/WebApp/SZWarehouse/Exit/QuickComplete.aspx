<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuickComplete.aspx.cs" Inherits="WebApp.SZWarehouse.Exit.QuickComplete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '出库通知(SZ)';
        gvSettings.menu = '签收单上传';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var RedirectFrom = getQueryString('RedirectFrom');

        var refreshCount = 0;
        var interval;

        var currentExitNoticeID = "";
        var currentFileUrl = "";

        function reinitIframe() {
            var iframe = document.getElementById("doc-iframe");
            try {
                var bHeight = iframe.contentWindow.document.body.scrollHeight;
                var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;
                var height = Math.max(bHeight, dHeight);
                iframe.height = height;
                //console.log(height);
                if (600 == refreshCount) {
                    window.clearInterval(interval);
                }
                refreshCount++;
            } catch (ex) { }
        }

        $.extend($.fn.textbox.methods, {
            addClearBtn: function(jq, iconCls){
                return jq.each(function(){
                    var t = $(this);
                    var opts = t.textbox('options');
                    opts.icons = opts.icons || [];
                    opts.icons.unshift({
                        iconCls: iconCls,
                        handler: function(e){
                            $(e.data.target).textbox('clear').textbox('textbox').focus();
                            $(this).css('visibility','hidden');
                        }
                    });
                    t.textbox();
                    if (!t.textbox('getText')){
                        t.textbox('getIcon',0).css('visibility','hidden');
                    }
                    t.textbox('textbox').bind('keyup', function(){
                        var icon = t.textbox('getIcon',0);
                        if ($(this).val()){
                            icon.css('visibility','visible');
                        } else {
                            icon.css('visibility','hidden');
                        }
                    });
                });
            }
        });

        $(function () {
            if (RedirectFrom == null || RedirectFrom == "") {
                var tab = $('#big-tab').tabs('getTab', 0);  //取得第一个tab
                $('#big-tab').tabs('update', {
                    tab: tab,
                    options: {
                        title: '签收单上传',
                    }
                });

                //修改输入提示标题
                $("#input-tip-title").html("请扫描或输入送\\提货单条码");
            } else if (RedirectFrom == "CustomerLading") {
                var tab = $('#big-tab').tabs('getTab', 0);  //取得第一个tab
                $('#big-tab').tabs('update', {
                    tab: tab,
                    options: {
                        title: '客户提货',
                    }
                });

                //修改输入提示标题
                $("#input-tip-title").html("请扫描或输入提货单条码");
            }

            $('#InputExitNoticeID').textbox('addClearBtn', 'icon-clear');
            //页面加载时，给输入框自动获得焦点
            $('#InputExitNoticeID').textbox('textbox').focus();

            //初始化上传按钮
            $("#upload-receipt-confirm-file").filebox({
                buttonText: '上传',
                //buttonIcon:'icon-add',
                buttonAlign: 'left',
                accept: 'image/gif, image/jp2, image/jpeg, image/png',
                multiple: false,
                width: '100px',
                height: '30px',
                onClickButton: function () {
                    $("#" + this.id).filebox('setValue', '');
                },
                onChange: function (newValue, oldValue) {
                    if ($("#" + this.id).filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    formData.append('exitnoticeid', currentExitNoticeID);

                    var files = $("input[id='" + this.id + "'] + span > input[type='file']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                        //检查文件类型
                        if (imgArr.indexOf(file.type) <= -1) {
                            $.messager.alert('提示', '请选择图片文件上传！');
                            return;
                        }

                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 0.8 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                formData.append('upload-receipt-confirm-file', bl, fileName); // 文件对象

                                SubmitFile(formData);
                            });
                        } else {
                            formData.append('upload-receipt-confirm-file', file);

                            SubmitFile(formData);
                        }

                                    
                    }

                },
            });

            $("#upload-receipt-confirm-file").next().children("a:first-child").width(100);
            $("#upload-receipt-confirm-file").next().children("span[class*='textbox-addon']").hide();

        });

        //查询
        function Search() {
            $("#tip-info-label").html(""); //清空提示
            $("#tip-info-div").hide(); //不显示提示

            
            $("#file-div").hide();

            currentExitNoticeID = "";
            currentFileUrl = "";

            $("#div-file-upload").hide();

            document.getElementById("doc-iframe").contentWindow.document.body.innerText = "";

            var InputExitNoticeID = $('#InputExitNoticeID').textbox('getValue');
            InputExitNoticeID = InputExitNoticeID.trim();
            $("#InputExitNoticeID").textbox('setValue', InputExitNoticeID);

            if (InputExitNoticeID.length <= 0) {
                $("#tip-info-label").html("请输入单号");
                $("#tip-info-div").show();
                window.setTimeout(function () {
                    $("#tip-info-label").html(""); //清空提示
                    $("#tip-info-div").hide(); //不显示提示
                }, 5000);
                return;
            }

            MaskUtil.mask();
            $.post('?action=QueryExitNotice', { ExitNoticeID: InputExitNoticeID }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {
                    currentExitNoticeID = resultJson.exitNoticeInfo.ExitNoticeID;
                    $("#div-file-upload").show();

                    //显示单号
                    $("#ExitNoticeID").html(resultJson.exitNoticeInfo.ExitNoticeID);

                    //显示制单时间
                    $("#CreateDate").html(resultJson.exitNoticeInfo.CreateDate);

                    //显示文件
                    if (resultJson.exitNoticeInfo.IsFileUploaded) {
                        //有文件
                        $("#FileStatus").html(resultJson.exitNoticeInfo.FileName + '<span style="color:red"> (已上传)</span>');
                        $("#file-view-button").show();

                        currentFileUrl = resultJson.exitNoticeInfo.FileUrl;
                    } else {
                        //没有文件
                        $("#FileStatus").html('<span style="color:red">(未上传)</span>');
                        $("#file-view-button").hide();

                        currentFileUrl = ""
                    }

                    $("#file-div").show();


                    //加载 iframe
                    switch (resultJson.exitNoticeInfo.ExitType) {
                    case "送货上门":
                        document.getElementById('doc-iframe').src = "./Docs/DocsDeliveryBill.aspx?ExitNoticeID=" + InputExitNoticeID; //送货上门
                        break;
                    case "快递":
                        document.getElementById('doc-iframe').src = "./Docs/DocsExpressBill.aspx?ExitNoticeID=" + InputExitNoticeID;  //快递
                        break;
                    case "自提":
                        document.getElementById('doc-iframe').src = "./Docs/DocsLadingBill.aspx?ExitNoticeID=" + InputExitNoticeID;   //自提
                        break;
                    }

                    interval = window.setInterval("reinitIframe()", 500);
                    refreshCount = 0;
                } else {
                    currentExitNoticeID = "";
                    $("#div-file-upload").hide();

                    $("#tip-info-label").html(resultJson.message);
                    $("#tip-info-div").show();
                    window.setTimeout(function () {
                        $("#tip-info-label").html(""); //清空提示
                        $("#tip-info-div").hide(); //不显示提示
                    }, 5000);
                }
            });

            //document.getElementById('doc-iframe').src = "./Docs/DocsDeliveryBill.aspx?ExitNoticeID=" + InputExitNoticeID;
        }

        //下载文件
        function Download() {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = currentFileUrl;
            a.download = "";
            a.click();
        }

        //预览文件
        function View() {
            $("#confirm-file-container").html("");

            $('<img/>',{
                src: currentFileUrl + '?time=' + new Date().getTime(),
                width: '1020px',
                //height: '600px',
                display: 'block',
            }).appendTo('#confirm-file-container');

            $('#view-receipt-confirm-file-dialog').dialog('open');
        }

        //上传
        function Upload() {

        }

        //提交文件
        function SubmitFile(formData) {
            MaskUtil.mask();
            //ajax 上传
            $.ajax({
                url: '?action=UploadReceiptConfirmFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $('#datagrid').datagrid('reload');
                        $.messager.alert('提示', "上传成功", 'info', function () {
                            var fileInfo = res.data[0];

                            currentFileName = fileInfo.Name;
                            currentFileUrl = fileInfo.Url;

                            //显示文件名 和 下载查看按钮
                            $("#FileStatus").html(fileInfo.Name + '<span style="color:red"> (已上传)</span>');
                            //$("#file-view-button").css("display", "block");
                            $("#file-view-button").show();
                        });
                        //$.messager.alert('提示', "上传成功");
                    } else {
                        $.messager.alert('错误', res.message);
                    }
                }
            }).done(function (res) {

            });
        }
    </script>
</head>
<body style="width: 100%; height: 100%; text-align: left; margin-left: 5px;">
    <div class="easyui-tabs" data-option="fit:true;" id="big-tab">
        <div title="" style="padding: 10px;">
            <form id="form1">
                <div style="width: 745px; margin: 20px auto 5px auto;">
                    <p id="input-tip-title" class="title" style="margin-bottom: 5px;"></p>
                    <input class="easyui-textbox" data-options="width:300, height:30," id="InputExitNoticeID" />
                    <a href="javascript:void(0);" class="easyui-linkbutton" style="margin-left: 10px; margin-right: 50px;"
                        data-options="<%--iconCls:'icon-search'--%> width:100, height:30," onclick="Search()">查询</a>
                    <%--<a href="javascript:void(0);" class="easyui-linkbutton" style="margin-left: 50px;" width:100, height:30," onclick="Upload()">上传</a>--%>
                    <span id="div-file-upload" style="display: none;">
                        <input id="upload-receipt-confirm-file" type="text" />
                    </span>
                </div>

                <div id="tip-info-div" style="width: 745px; margin: 5px auto; display: none;">
                    <span id="tip-info-label" style="font-size: 16px; color: red;"></span>
                </div>

                <div id="file-div" style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray; width: 735px; margin: 25px auto 5px auto; display: none;">
                    <p class="title"><span>单</span><span style="margin-left: 24px;">号</span>：<span id="ExitNoticeID" style="font-size: 14px"></span></p>
                    <p class="title">制单时间：<span id="CreateDate" style="font-size: 14px"></span></p>
                    <p class="title"><span>签</span><span style="margin-left: 6px;">收</span><span style="margin-left: 6px;">单</span>：<span id="FileStatus" style="font-size: 14px"></span></p>
                    <p class="title" id="file-view-button" style="display: none;">
                        <span style="margin-left: 52px;">&nbsp;</span>
                        <a href="javascript:void(0);" id="download" class="link" style="color: #0081d5;" data-options="iconCls:'icon-ok'" onclick="Download()">下载</a>
                        <a href="javascript:void(0);" id="view" class="link" style="color: #0081d5; margin-left: 5px;" data-options="iconCls:'icon-search'" onclick="View()">预览</a>
                    </p>
                    <p class="title" style="margin-top: 5px; margin-left: 60px">仅限图片格式的文件</p>
                </div>

                <div style="width: 770px; margin: 20px auto">
                    <iframe id="doc-iframe" style="width: 770px;" frameborder="no" scrolling="no" name="ifd" onload="this.height=ifd.document.body.scrollHeight"></iframe>
                </div>

            </form>
        </div>
    </div>

    <!------------------------------------------------------------ 查看上传的单据弹框 html Begin ------------------------------------------------------------>

    <div id="view-receipt-confirm-file-dialog" class="easyui-dialog" style="width: 1100px; height: 650px;" data-options="title: '查看客户确认单', iconCls:'icon-search', resizable:false, modal:true, closed: true,">
        <div id="confirm-file-container" style="margin: 15px 20px 0 25px; text-align: center;">

        </div>
    </div>

    <!------------------------------------------------------------ 查看上传的单据弹框 html End ------------------------------------------------------------>
</body>
</html>
