<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecRequestCert.aspx.cs" Inherits="WebApp.Declaration.Declare.DecRequestCert" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        $(function () {
            InitClientPage();
            var DeclarationID = getQueryString("ID");
            $("#DeclarationID").val(DeclarationID);
            $('#orders').myDatagrid({
                fitColumns: true,
                fit: true,
                toolbar: '#topBar',
                onLoadSuccess: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        //
                        $('[id^=uploadFile]').filebox({
                            validType: ['fileSize[10240,"KB"]'],
                            buttonText: '上传文件',
                            buttonAlign: 'right',
                            prompt: '请选择图片或PDF类型的文件',
                            accept: ['application/pdf'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                formData.append('ID', this.id.split("-")[1]);
                                MaskUtil.mask();
                                $.ajax({
                                    url: '?action=UploadFile',
                                    type: 'POST',
                                    data: formData,
                                    dataType: 'JSON',
                                    cache: false,
                                    processData: false,
                                    contentType: false,
                                    success: function (res) {
                                        MaskUtil.unmask();
                                        if (res.success) {
                                            //$('#AgreementChangeList').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                    }
                },
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            if (window.parent.frames.Source == "Add" || window.parent.frames.Source == "Assign" || window.parent.frames.Source == "Edit") {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\',\'' + row.DocuCode + '\',\'' + row.CertCode + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">编辑</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>' +
                    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">删除</span>' +
                    '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            else {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SearchEdoc(\'' + row.ID + '\',\'' + row.DocuCode + '\',\'' + row.CertCode + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            buttons += '<input id="uploadFile-' + row.ID + '" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 26px" />';
            return buttons;
        }

        function ViewFile(val, row, index) {
            var fmt = "";
            if (row.FileUrl != "") {
                fmt += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 10px;" onclick="ViewSAEle(\''
                    + row.FileUrl + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return fmt;
        }

        //查看文件
        function ViewSAEle(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('doc') > 0) {
                var a = document.createElement("a");
                //a.download = name + ".xls";
                a.href = url;
                $("body").append(a); // 修复firefox中无法触发click
                a.click();
                $(a).remove();
            } else if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            } else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            }
        }

        function Add() {
            var DeclarationID = $("#DeclarationID").val();
            var url = location.pathname.replace(/DecRequestCert.aspx/ig, 'DecRequestCertEdit.aspx?DeclarationID=' + DeclarationID + '&EdocSource=Add');
            $.myWindow.setMyWindow("DecRequestCert2DecRequestCertEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增随附单证',
                width: '500px',
                height: '280px',
                onClose: function () {
                    $('#orders').myDatagrid('flush');
                }
            });
        }

        function Delete(ID) {
            var model = {
                ID: ID
            };
            $.messager.confirm('消息', '确定删除该条记录', function (r) {
                if (r) {
                    $.post('?action=Delete', model, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('消息', '删除成功');
                            SearchButton();
                        } else {
                            $.messager.alert('消息', '删除失败');
                        }
                    });
                } else {

                }
            });
        }

        function Edit(ID, DocuCode, CertCode) {
            var url = location.pathname.replace(/DecRequestCert.aspx/ig, 'DecRequestCertEdit.aspx?ID=' + ID + '&DocuCode=' + DocuCode + '&CertCode=' + CertCode + '&EdocSource=Edit');
            $.myWindow.setMyWindow("DecRequestCert2DecRequestCertEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑随附单证',
                width: '500px',
                height: '280px'
            });
        }

        function SearchEdoc(ID, DocuCode, CertCode) {
            var url = location.pathname.replace(/DecRequestCert.aspx/ig, 'DecRequestCertEdit.aspx?ID=' + ID + '&DocuCode=' + DocuCode + '&CertCode=' + CertCode + '&EdocSource=Search');
            $.myWindow.setMyWindow("DecRequestCert2DecRequestCertEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑随附单证',
                width: '500px',
                height: '280px'
            });
        }

        function SearchButton() {
            var parm = {
                DeclarationID: ""
            };
            $('#orders').myDatagrid("search", parm);
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server" method="post">
        <div id="topBar">
            <div id="tool">
                <ul>
                    <li>
                        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                        <input type="hidden" id="DeclarationID" />
                    </li>
                </ul>
            </div>
        </div>
        <div id="data" data-options="region:'center',border:false" style="margin: 5px;">
            <table id="orders" title="随附单证" data-options="fitColumns:true,fit:true,toolbar:'#topBar'">
                <thead>
                    <tr>
                        <th data-options="field:'DocuCode',align:'center'" style="width: 120px;">单证代码</th>
                        <th data-options="field:'CertCode',align:'center'" style="width: 120px;">单证编号</th>
                        <th data-options="field:'FileUrl',align:'center',formatter:ViewFile" style="width: 120px;">附件</th>
                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
                    </tr>
                </thead>
            </table>
        </div>

         <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </form>
</body>
</html>
