<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.RiskFile.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var FileType = eval('(<%=this.Model.FileType%>)');

        //数据初始化
        $(function () {

            //下拉框数据初始化
            $('#FileType').combobox({
                data: FileType
            });

            //订单列表初始化
            $('#files').myDatagrid({
                fitColumns: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function () {
                    $("a[name=btnView]").on('click', function () {
                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");

                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");
                        }

                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }

                        $("#viewFileDialog").window('open').window('center');
                    });
                    //如果是doc 文档只能下载查看
                    $("#btndownload").click(function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");
                        ////if (fileUrl.toLowerCase().indexOf('docx') > 0) {
                        //    $("#btndownload").attr('href', fileUrl);
                        //    $("#btndownload").attr("download", fileUrl);
                        //    $("#btndownload").click();
                        ////}
                        let a = document.createElement('a');
                        document.body.appendChild(a);
                        a.href = fileUrl;
                        a.download = "";
                        a.click();

                    });

                }
            });
            InitClientPage();
            $('#btnAdd').show();
            var from = window.parent.frames.Source;
            debugger
            switch (from) {
                case 'ServiceManagerView':
                    $('#btnAdd').hide();
                    break;
                default:
                    break;
            }
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            //获取最后一个.的位置
            var index = row.Name.lastIndexOf(".");
            //获取后缀
            var ext = row.Name.substr(index + 1);

            var buttons = ""
            if (ext.toLowerCase().indexOf('docx') != -1 || ext.toLowerCase().indexOf('doc') != -1 || ext.toLowerCase().indexOf('mp4') != -1) {
                buttons = '<a id="btndownload" name="btndownload" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.Url + '" style="margin:3px" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">下载</span>' +
                    '<span class="l-btn-icon icon-download">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

            } else {
                buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.Url + '" style="margin:3px" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            if (window.parent.frames.Source == 'Add' || window.parent.frames.Source == 'Assign') {
                buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">删除</span>' +
                    '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除附件信息！', function (success) {
                if (success) {
                    $.post('?action=DeleteFile', { ID: id }, function (res) {
                        var result = JSON.parse(res);
                        $('#files').myDatagrid('reload');
                        $.messager.alert('消息', result.message, "info", function (r) {
                        });
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增附件信息',
                width: '700px',
                height: '300px',
                onClose: function () {
                    $('#files').datagrid('reload');
                }
            });
        }

        function Return() {
            var source = window.parent.frames.Source;
            var u = "";
            switch (source) {
                case 'Add':
                    u = '../New/List.aspx';
                    break;
                case 'Assign':
                    u = '../Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = '../New/List.aspx';
                    break;
                case 'ApproveView':
                    u = '../Approval/List.aspx';
                    break;
                case 'QualifiedView':
                    u = '../Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = '../ServiceManagerView/List.aspx';
                    break;
                default:
                    u = '../View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/List.aspx/ig, u);
            window.parent.location = url;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 700px;">
        <img id="viewfileImg" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 5px">
        <table id="files" data-options="singleSelect:true,border:true,fit:true,scrollbarSize:0" title="附件信息列表" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left'" style="width: 15%;">名称</th>
                    <th data-options="field:'FileType',align:'center'" style="width: 12%;">文件类型</th>
                    <%-- <th data-options="field:'FileFormat',align:'center'" style="width: 12%;">文件格式</th>--%>
                    <th data-options="field:'Status',align:'center'" style="width: 10%;">状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">创建日期</th>
                    <th data-options="field:'Summary',align:'center'" style="width: 15%;">摘要备注</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
