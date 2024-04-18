<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetCandidateDeclareCreator.aspx.cs" Inherits="WebApp.Declaration.DeclarantCandidates.SetCandidateDeclareCreator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>设置候选制单员</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            $('#CandidateDeclareCreators').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,

            });



        });

        function Operation(val, row, index) {
            var buttons = '';

            buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\''
                + row.DeclarantCandidateID + '\',\'' + row.AdminName + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">删除</span>' +
                    '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

            return buttons;
        }

        function Add() {
            var url = location.pathname.replace(/SetCandidateDeclareCreator.aspx/ig, 'AddDeclarantCandidate.aspx')
                + "?From=<%=Needs.Ccs.Services.Enums.DeclarantCandidateType.DeclareCreator.ToString()%>";

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '设置候选制单员',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#CandidateDeclareCreators').datagrid('reload');
                }
            });
        }

        function Delete(declarantCandidateID, adminName) {
            $("#delete-dialog-content").html("确定要删除 " + adminName + "吗？");

            $('#delete-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-delete-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();

                        $.post('?action=Delete', {
                            DeclarantCandidateID: declarantCandidateID
                        }, function (result) {
                            var rel = JSON.parse(result);
                            MaskUtil.unmask();
                            $.messager.alert('提示', rel.message, 'info', function () {
                                $('#delete-dialog').dialog('close');
                            });
                            $('#CandidateDeclareCreators').datagrid('reload');
                        });
                    }
                }, {
                    id: 'btn-delete-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#delete-dialog').dialog('close');
                    }
                }],
            });

            $('#delete-dialog').window('center'); //dialog 居中
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div style="margin: 5px 0 0px 15px;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>


    </div>
    <table id="CandidateDeclareCreators" title="候选制单员列表" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false," toolbar="#topBar">
        <thead>
            <tr>
                <th data-options="field:'AdminName',align:'left'" style="width: 10%">候选制单员</th>
                <th data-options="field:'CreateTime',align:'left'" style="width: 10%">添加时间</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 16%">操作</th>
            </tr>
        </thead>
    </table>
    <div id="delete-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="delete-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>
</body>
</html>
