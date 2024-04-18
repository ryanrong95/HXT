<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecEdocRealation.aspx.cs" Inherits="WebApp.Declaration.Declare.DecEdocRealation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        $(function () {
            InitClientPage();
            $('#edocs').myDatagrid({
                toolbar:'#topBar',
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                fit: true,
                nowrap: false,
                onLoadSuccess: function () {
                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");
                        top.$.myWindow({
                            iconCls: "",
                            url: fileUrl,
                            noheader: false,
                            title: '查看电子单据',
                            width: '1024px',
                            height: '600px'
                        });
                    });
                }
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.FileUrl + '" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="DownLoad(\'' + row.FileUrl + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">下载</span>' +
                '<span class="l-btn-icon icon-yg-excelExport">&nbsp;</span>' +
                '</span>' +
                '</a>';

            if (window.parent.frames.Source == "Add" || window.parent.frames.Source == "Assign"||window.parent.frames.Source == "Edit") {
                buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">删除</span>' +
                    '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        function ReBuild() {
            $.messager.confirm("消息", "您确定要重新生成合同、发票、装箱单附件？", function (data) {
                if (data) {
                    //
                    MaskUtil.mask();//遮挡层
                    $.post('?action=ReBuildPDF', { ID: ID }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message,'info', function () {
                            if (result.success) {
                                $('#edocs').datagrid('reload');
                            } else {
                                 $.messager.alert('消息', result.info);
                            }
                        });
                    });
                } else {

                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/DecEdocRealation.aspx/ig, 'DecEdocEdit.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '上传电子随附单证',
                width: '750px',
                height: '450px',
                onClose: function () {
                    $('#edocs').datagrid('reload');
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除附件信息！', function (success) {
                if (success) {
                    $.post('?action=DeleteFile', { ID: id }, function (res) {
                        var result = JSON.parse(res);
                        $('#edocs').datagrid('reload');
                        $.messager.alert('消息', result.message, "info", function (r) {
                        });
                    })
                }
            });
        }

        function DownLoad(url) {
            try {
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            } catch (e) {
                console.log(e);
            }
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton ir-add" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <a id="btnReBuild" href="javascript:void(0);" class="easyui-linkbutton ir-reload" data-options="iconCls:'icon-redo'" onclick="ReBuild()">重新生成</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin:5px;">
        <table id="edocs" title="电子随附单据">
            <thead>
                <tr>
                    <th data-options="field:'EdocID',align:'left'" style="width: 25%">文件名</th>
                    <th data-options="field:'EdocName',align:'left'" style="width: 25%">单证类别</th>
                    <th data-options="field:'EdocCopId',align:'left'" style="width:25%">原文件名</th>
                    <%--<th data-options="field:'EdocFomatType',align:'center'" style="width:10%">格式</th>
                    <th data-options="field:'OpNote',align:'left'" style="width: 15%">操作说明(重传原因)</th>                    
                    <th data-options="field:'SignTime',align:'left'" style="width: 10%">签名时间</th>
                    <th data-options="field:'EdocOwnerName',align:'left'" style="width: 10%">所属单位名称</th>
                    <th data-options="field:'EdocSize',align:'center'" style="width: 10%">文件大小</th>--%>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width:15%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
