<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileTest.aspx.cs" Inherits="WebApp.Temp.FileTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日志记录</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/ccs.log-1.0.js"></script>
    <script src="../Scripts/ccs.file-0.3.js"></script>
    <script src="../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            var logs = eval('(<%=this.Model.Logs%>)');
            var files = eval('(<%=this.Model.Files%>)');

            //日志控件初始化
            $('#ccslogContainer').ccslog({
                data: logs
            });

            //文件控件初始化
            $('#ccsfileContainer').ccsfile({
                data: files
            });

            //文件列表初始化
            $('#files').myDatagrid({
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#fileContainer').panel('setTitle', '订单附件(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $(this).datagrid('getPanel');
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });
                }
            });

            //注册文件上传的onChange事件
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

                    var formData = new FormData($('#form1')[0]);
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
                                var data = res.data;
                                for (var i = 0; i < data.length; i++) {
                                    $('#files').datagrid('appendRow', {
                                        Name: data[i].Name,
                                        FileFormat: data[i].FileFormat,
                                        VirtualPath: data[i].VirtualPath,
                                        Url: data[i].Url
                                    });
                                }
                                var data = $('#files').datagrid('getData');
                                $('#files').datagrid('loadData', data);
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            });
        });

        //预览文件
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
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

        //删除文件
        function Delete(index) {
            $.messager.confirm('确认', '请您再次确认是否删除所选文件！', function (success) {
                if (success) {
                    // ...
                    $('#files').datagrid('deleteRow', index);
                    var data = $('#files').datagrid('getData');
                    $('#files').datagrid('loadData', data);
                }
            });
        }

        //导出文件
        function Export() {
            // ...
            var rows = $('#files').datagrid('getRows');
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            buttons += '<a href="#"><span style="color: cornflowerblue; margin-left: 10px;" onclick="Delete(' + index + ')">删除</span></a>';
            return buttons;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div style="margin-top: 10px; margin-left: 5px; width: 40%">
            <%--日志--%>
            <div id="ccslogContainer" title="日志记录" class="easyui-panel">
            </div>
            <br />

            <%--文件--%>
            <div id="ccsfileContainer" title="订单附件" class="easyui-panel">
            </div>
            <br />

            <%--文件--%>
            <div id="fileContainer" title="订单附件" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                <div class="sub-container">
                    <div id="unUpload" style="margin-left: 5px">
                        <p>未上传</p>
                    </div>
                    <div>
                        <table id="files" data-options="nowrap:false,queryParams:{ action: 'dataFiles' }">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>

                    <div style="margin-top: 5px; margin-left: 5px;">
                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Export()">导出</a>
                        <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                    </div>
                    <div class="text-container" style="margin-top: 10px;">
                        <p>导出pdf格式的文件，交给客户盖章后上传</p>
                        <p>仅限图片或pdf格式的文件,并且不超过500kb</p>
                    </div>
                </div>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </form>
</body>
</html>
