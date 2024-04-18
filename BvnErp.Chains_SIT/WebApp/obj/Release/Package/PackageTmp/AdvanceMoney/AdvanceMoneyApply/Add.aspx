<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.AdvanceMoney.AdvanceMoneyApply.Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/chainsupload.js"></script>
    <script type="text/javascript">

        $(function () {
            debugger;
            $("#ClientID").next().hide();
            $("#OverdueInterestRate").textbox('setValue', '0.05');

            $('#datagrid_file').datagrid();

            //注册上传原始单据filebox的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                buttonText: '选择文件',
                buttonAlign: 'right',
                onClickButton: function () {
                    $('#uploadFile').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        formData.append('uploadFile', file);
                    }
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
                                InsertFile(res.data);
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });

                    $("#datagrid_file").parent().parent().height(600);
                    $("#datagrid_file").parent().parent().find(".datagrid-view").height(600);
                    $("#datagrid_file").parent().parent().find(".datagrid-view").find(".datagrid-view2").height(600);

                }
            });

            $("#AdvanceAmount + span :first-child").keyup(function () {
                formatAmt(this);
            });
        });

        //验证客户名称 Begin
        function Search() {
            var ClientCode = $("#ClientCode").textbox('getText');
            $.post('?action=CheckClientCode', { ClientCode: ClientCode }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $("#ClientName").textbox('setText', result.data.ClientName);
                    $("#ClientID").textbox('setText', result.data.ClientID);
                    return;
                }
                else {
                    $("#ClientCode").textbox('setValue', "");
                    $.messager.alert('错误', result.message);
                    return;
                }
            });
        }
        // End

        function FileOperation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            buttons += '<a href="javascript:void(0);" style="margin-left: 12px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
            return buttons;
        }
        function ShowImg(val, row, index) {
            return "<img src='../../App_Themes/xp/images/wenjian.png' />";
        }

        //删除文件
        function Delete(index) {
            $("#datagrid_file").datagrid('deleteRow', index);
            //解决删除行后，行号错误问题
            var data = $('#datagrid_file').datagrid('getData');
            $('#datagrid_file').datagrid('loadData', data);
        }

        function onClickFileRow(index) {
            if (isRemoveFileRow) {
                $('#datagrid_file').datagrid('deleteRow', index);
                isRemoveFileRow = false;
            }
        }

        //预览文件
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0 || url.toLowerCase().indexOf('jpg') > 0
                || url.toLowerCase().indexOf('png') > 0 || url.toLowerCase().indexOf('bmp') > 0
                || url.toLowerCase().indexOf('jpeg') > 0 || url.toLowerCase().indexOf('gif') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('mp4') || url.toLowerCase().indexOf('xlsx') || url.toLowerCase().indexOf('xls')) {
                let a = document.createElement('a');
                document.body.appendChild(a);
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

        //提交申请
        function Submit() {
            //验证表单数据
            var require = Valid('form1');
            if (!require) {
                return;
            }
            var ClientID = $("#ClientID").textbox("getText").trim();
            var ClientCode = $("#ClientCode").textbox("getValue").trim();//客户编号
            var ClientName = $("#ClientName").textbox("getText").trim(); //客户名称
            var AdvanceAmount = $("#AdvanceAmount").textbox("getValue").trim(); //垫资金额
            var LimitDay = $("#LimitDay").textbox("getValue").trim(); //垫资期限
            var InterestRate = $("#InterestRate").numberbox("getValue").trim(); //利率
            var OverdueInterestRate = $("#OverdueInterestRate").numberbox("getValue").trim(); //逾期利率
            //2021-06-25 张庆永 业务员可以自己谈，但不能低于公司规定
            if (OverdueInterestRate < 0.05)
            {
                $.messager.alert('提示', '逾期利率不得低于公司规定的0.05%！');
                return;
            }
            var FileData = $('#datagrid_file').datagrid('getRows');
            var Summary = $("#Summary").textbox("getValue").trim(); //备注
            if (FileData.length == 0) {
                $.messager.alert('提示', '请上传附件！');
                return;
            }
            $("#comfirm-dialog-content").html("<label>确定提交申请吗？</label>");
            $('#comfirm-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                modal: true,
                buttons: [{
                    id: 'btn-submit-comfirm-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post('?action=Submit', {
                            ClientID: ClientID,
                            ClientCode: ClientCode,
                            ClientName: ClientName,
                            AdvanceAmount: AdvanceAmount,
                            LimitDay: LimitDay,
                            InterestRate: InterestRate,
                            OverdueInterestRate: OverdueInterestRate,
                            Files: JSON.stringify(FileData),
                            Summary: Summary
                        }, function (res) {
                            MaskUtil.unmask();
                            var resJson = JSON.parse(res);
                            if (resJson.success) {
                                $.messager.alert('提示', resJson.message, 'info', function () {
                                    $('#comfirm-dialog').dialog('close');
                                    $.myWindow.close();
                                });
                            } else {
                                $.messager.alert('提示', resJson.message, 'info', function () {
                                    $('#comfirm-dialog').dialog('close');

                                });
                            }
                        });
                    }
                }, {
                    id: 'btn-submit-comfirm-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#comfirm-dialog').dialog('close');
                    }
                }],
            });

            $('#comfirm-dialog').window('center');
        }

        //格式化金额
        function formatAmt(obj) {
            obj.value = obj.value.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符
            obj.value = obj.value.replace(/^\./g, "");  //验证第一个字符是数字而不是.
            obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的.
            obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
        }

        function InsertFile(data) {
            var row = $('#datagrid_file').datagrid('getRows');
            for (var i = 0; i < data.length; i++) {
                $('#datagrid_file').datagrid('insertRow', {
                    index: row.length + i,
                    row: {
                        Name: data[i].Name,
                        FileType: data[i].FileType,
                        FileFormat: data[i].FileFormat,
                        VirtualPath: data[i].VirtualPath,
                        WebUrl: data[i].Url,
                    }
                });
            }
        }
    </script>
    <style type="text/css">
        .easyui-panel panel-body {
            height: 260px
        }

        .panel window panel-htop {
            top: -1px
        }
    </style>
</head>
<body class="easyui-layout">
    <div style="margin-top: 20px; margin-left: 2%; float: left; width: 800px;">
        <!-- 按钮 -->
        <div id="btn-area" class="view-location" style="width: 750px; height: 30px; float: left;">
            <span id="btn-submit">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-ok'">提交申请</a>
            </span>

        </div>

        <!-- 申请信息列 -->
        <div style="float: left; width: 420px; height: 250px">
            <div class="big-row-one view-location">
                <div class="easyui-panel" title="申请信息">
                    <form id="form1">
                        <div class="sub-container left-block-one">
                            <table class="row-info" style="width: 100%; height: 248px" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="lbl" style="text-align: right">客户编号：</td>
                                    <td>
                                        <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]',width: 190,required:true" />
                                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="color: #0081d5; cursor: pointer; margin: 0 8px; font: 12px/1.2 Arial,Verdana,'微软雅黑','宋体';">查询</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">客户名称：</td>
                                    <td>
                                        <input class="easyui-textbox" id="ClientName" data-options="validType:'length[1,50]',width: 190,required:true,readonly:true," />
                                        <input class="easyui-textbox" id="ClientID" data-options="validType:'length[1,50]',width: 190" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">垫资金额（元）：</td>
                                    <td>
                                        <input class="easyui-textbox" id="AdvanceAmount" data-options="validType:'length[1,50]',width: 190,required:true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">垫资期限（日）：</td>
                                    <td>
                                        <input class="easyui-textbox" id="LimitDay" data-options="validType:'length[1,50]',width: 190,required:true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">月利率（%）：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="InterestRate" data-options="min:0,precision:4,width: 190,required:true,border:false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">逾期利率（日/%）：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="OverdueInterestRate" data-options="min:0,precision:4,width: 190,required:true,border:false," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right; vertical-align: initial">备注：</td>
                                    <td>
                                        <input class="easyui-textbox" id="Summary" data-options="width: 190, height:70,multiline:true" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <!-- 附件列 -->
        <div style="margin-left: 10px; float: left;">
            <div id="file-area" class="big-row-two view-location" style="width: 350px; height: 248px; float: left;">
                <div class="easyui-panel" title="附件" style="height: 280px;">
                    <div class="sub-container" style="height: 248px;">
                        <div id="unUpload" style="margin-left: 5px">
                            <p>未上传</p>
                        </div>
                        <div>
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 24px" />
                            <div style="margin-top: 5px;">
                                <label></label>
                            </div>
                        </div>
                        <div>
                            <table id="datagrid_file" data-options="nowrap:false," style="height: 248px;">
                                <thead>
                                    <tr>
                                        <th data-options="field:'img',formatter:ShowImg" style="width: 10%">图片</th>
                                        <th style="width: 100%" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 350px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 60%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>

    <div id="comfirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="comfirm-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>
</body>
</html>
