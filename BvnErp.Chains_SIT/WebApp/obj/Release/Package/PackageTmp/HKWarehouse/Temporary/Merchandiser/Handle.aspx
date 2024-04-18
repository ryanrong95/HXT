<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Handle.aspx.cs" Inherits="WebApp.HKWarehouse.Temporary.Merchandiser.Handle" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑暂存</title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script src="../../../Scripts/chainsupload.js"></script>
  
    <script type="text/javascript">

        var Temporary = eval('(<%=this.Model.Temporary%>)');
        var WrapTypeData = eval('(<%=this.Model.WrapTypeData%>)');
        var Clients = eval('(<%=this.Model.Clients%>)');
        var ID = getQueryString('ID');

        //页面加载
        $(function () {
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
                    $('#fileContainer').panel('setTitle', '暂存附件(' + data.total + ')');
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

            $('#uploadFile').filebox({
                multiple: true,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $('#uploadFile').filebox('setValue', '');
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
                                formData.set('uploadFile', bl, fileName); // 文件对象
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
                                            var row = $('#pi').datagrid('getRows');
                                            var data = res.data;
                                            for (var i = 0; i < data.length; i++) {
                                                $('#pi').datagrid('insertRow', {
                                                    index: row.length + i,
                                                    row: {
                                                        Name: data[i].Name,
                                                        FileType: data[i].FileType,
                                                        FileFormat: data[i].FileFormat,
                                                        WebUrl: data[i].WebUrl,
                                                        Url: data[i].Url
                                                    }
                                                });
                                            }
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
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
                                        var row = $('#pi').datagrid('getRows');
                                        var data = res.data;
                                        for (var i = 0; i < data.length; i++) {
                                            $('#pi').datagrid('insertRow', {
                                                index: row.length + i,
                                                row: {
                                                    Name: data[i].Name,
                                                    FileType: data[i].FileType,
                                                    FileFormat: data[i].FileFormat,
                                                    WebUrl: data[i].WebUrl,
                                                    Url: data[i].Url
                                                }
                                            });
                                        }
                                    } else {
                                        $.messager.alert('提示', res.message);
                                    }
                                }
                            }).done(function (res) {

                            });
                        }
                    }

                }
            });

            //原始PI列表初始化
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'data',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'Name', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileFormat', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'WebUrl', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: ShowInfo }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".pi");
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

            // 初始化包装类型
            $("#WrapType").combogrid({
                idField: "Code",
                textField: "Name",
                data: WrapTypeData,
                fitColumns: true,
                mode: "local",
                columns: [[
                    { field: 'Code', title: 'Code', width: 50, align: 'center', sortable: true },
                    { field: 'Name', title: 'Name', width: 120, align: 'center' },
                ]],
                onSelect: function () {
                    var grid = $("#WrapType").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                },
                keyHandler: {
                    up: function () { },
                    down: function () { },
                    enter: function () { },
                    query: function (data) {
                        //动态搜索 
                        $("#WrapType").combogrid("grid").datagrid("reload", { 'keyword': data });
                        $("#WrapType").combogrid("setValue", data);
                    }
                }
            });
            //绑定日志信息
            var data = new FormData($('#form2')[0]);
            data.append("ID", ID);
            $.ajax({
                url: '?action=LoadLogs',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    showLogContent(data);
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });

            $("#EntryNumber").combobox({
                data: Clients,
                onSelect: function (record) {
                    $("#CompanyName").textbox('setValue', record.Text.split('-')[1]);
                }
            });
            Init();
            $("#EntryNumber").combobox("textbox").bind("blur", function () {
                var data = $("#EntryNumber").combobox("getData");
                var value = $("#EntryNumber").combobox("getText");
                var textField = $("#EntryNumber").combobox("options").textField;
                var index = $.easyui.indexOfArray(data, textField, value);
                if (index < 0) {
                    $("#EntryNumber").combobox("clear");
                }
                $("#CompanyName").textbox('setValue', value.split('-')[1]);
            });

        });

        function Init() {
            if (Temporary != null && Temporary != "") {
                $("#EntryNumber").textbox('setValue', Temporary.EntryNumber);
                $("#CompanyName").textbox('setValue', Temporary.CompanyName);
                $("#ShelveNumber").textbox('setValue', Temporary.ShelveNumber);
                $("#EntryDate").datebox('setValue', Temporary.EntryDate);
                $("#WaybillCode").textbox('setValue', Temporary.WaybillCode);
                $("#PackNo").numberbox('setValue', Temporary.PackNo);
                if (Temporary.WrapType == 0 || Temporary.WrapType == 1 || Temporary.WrapType == 4 || Temporary.WrapType == 6) {
                    $("#WrapType").combogrid('setValue', "0" + Temporary.WrapType);
                }
                else {
                    $("#WrapType").combogrid('setValue', Temporary.WrapType);
                }

                $("#Summary").textbox('setValue', Temporary.Summary);
            }
            else {
                //设置默认包装类型
                $("#WrapType").combogrid('setValue', 22);
            }
        }

        //预览文件
        // var isRemovePIRow = false;
        function Delete(index) {
            // isRemovePIRow = true;
            $('#pi').datagrid('deleteRow', index);
            var data = $('#pi').datagrid('getData');
            $('#pi').datagrid('loadData', data);
        }

        //function onClickPIRow(index) {
        //    if (isRemovePIRow) {
        //        $('#pi').datagrid('deleteRow', index);
        //        isRemovePIRow = false;
        //    }
        //}

        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
            }
            $('#viewFileDialog').window('open').window('center');
        }

        //显示附件图片
        function ShowInfo(val, row, index) {
            return '<img src="../../../App_Themes/xp/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.WebUrl + '\')">' + row.Name + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
        }

        //保存暂存信息
        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                return;
            }
            else {
                var id = getQueryString('ID');
                $('#uploadFile').filebox('setValue', '');
                var FileData = $("#pi").datagrid("getRows");
                if (FileData.length == 0) {
                    $.messager.alert('消息', "请上传暂存附件！");
                    return;
                }
                //验证成功
                var entryNumber = $("#EntryNumber").textbox('getValue');
                var companyName = $("#CompanyName").textbox('getValue');
                var ShelveNumber = $("#ShelveNumber").textbox('getValue');
                var entryDate = $("#EntryDate").datebox('getValue');
                var WaybillCode = $("#WaybillCode").textbox('getValue');
                var packNo = $("#PackNo").numberbox('getValue');
                var wrapType = $("#WrapType").combobox('getValue');
                var Summary = $("#Summary").textbox('getValue');
                $.post('?action=Save', {
                    ID: id,
                    EntryNumber: entryNumber,
                    CompanyName: companyName,
                    ShelveNumber: ShelveNumber,
                    EntryDate: entryDate,
                    WaybillCode: WaybillCode,
                    PackNo: packNo,
                    Summary: Summary,
                    WrapType: wrapType,
                    FileData: JSON.stringify(FileData),
                }, function (result) {
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            Close();
                        }
                    });
                })
            }
        }

        //关闭
        function Close() {
            $.myWindow.close();
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }

    </script>
     <style>
         #pi-fujian-table td {
             border-width: 0;
             padding: 0;
         }

             #pi-fujian-table td div {
                 padding: 0;
             }
     </style>
</head>
<body class="easyui-layout">
    <div  id="content" data-options="region:'center',border:false,collapsible:false" >
        <form id="form1" runat="server" method="post">
                 <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">入仓号：</td>
                    <td>
                       <input class="easyui-combobox" id="EntryNumber" name="EntryNumber"
                            data-options="valueField:'Value',textField:'Text'" style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">公司名称：</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" name="CompanyName"
                            data-options="required:true,validType:'length[1,50]',width:250,height:26" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">库位号：</td>
                    <td>
                        <input class="easyui-textbox" id="ShelveNumber" name="ShelveNumber"
                            data-options="required:true,validType:'length[1,50]',width:250,height:26" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">入库时间：</td>
                    <td>
                        <input class="easyui-datetimebox" id="EntryDate" name="ShelveNumber"
                            data-options="required:true,validType:'length[1,50]',width:250,height:26" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运单号：</td>
                    <td>
                        <input class="easyui-textbox" id="WaybillCode" name="WaybillCode"
                            data-options="required:false,validType:'length[1,50]',width:250,height:26" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">件数：</td>
                    <td>
                        <input type="text" id="PackNo" name="PackNo" class="easyui-numberbox"
                            data-options="min:0,required:true,validType:'length[1,3]',width:250,height:26" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td class="lbl">包装类型：</td>
                    <td>
                        <input class="easyui-combogrid" id="WrapType" name="WrapType"
                            data-options="required:true,width:250,height:26" />
                    </td>
                </tr>
              
                   <tr>
                                <td>费用附件：</td>
                                <td>
                                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 24px" />
                                </td>
                            </tr>
                     <tr>
                                <td></td>
                                <td>
                                    <label>仅限图片、pdf格式的文件，且pdf文件不超过3M。</label>
                                </td>
                            </tr>
                     <tr>
                                <td></td>
                                <td>
                                    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 650px; height: 450px;">
                                        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                                        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                                    </div>
                                    <div id="pi-fujian-table" class="ProductTable">
                                        <table id="pi" data-options="pageSize:50,fitColumns:true,fit:false,pagination:false,queryParams:{ action: 'data' }">
                                          <%--  <thead>
                                                <tr>
                                                    <th data-options="field:'info',width: 100,align:'left',formatter:ShowInfo"">附件信息</th>
                                                </tr>
                                            </thead>--%>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                       <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="multiline:true,required:false,validType:'length[1,300]',height:50,width:250" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
     <div data-options="region:'south',border:false,collapsible:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center',border:false,collapsible:false,split:true" style="min-height: 100px" title="日志记录">
                <div class="sub-container">
                    <div class="text-container" id="LogContent">
                    </div>
                </div>
            </div>
            <div id="dlg-buttons" data-options="region:'south',border:false">
                <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
                <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </div>
    </div>
</body>
</html>
