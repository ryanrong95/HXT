<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecContainer.aspx.cs" Inherits="WebApp.Declaration.Declare.DecContainer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            InitClientPage();
            var DeclarationID = getQueryString("ID");
            $("#DeclarationID").val(DeclarationID);
            $('#orders').myDatagrid();
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            if (window.parent.frames.Source == "Add" || window.parent.frames.Source == "Assign"||window.parent.frames.Source == "Edit") {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
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
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SeachContainer(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }

        function Add() {
            var DeclarationID = $("#DeclarationID").val();
            var url = location.pathname.replace(/DecContainer.aspx/ig, 'DecContainerEdit.aspx?DeclarationID=' + DeclarationID + '&SourceCon=Add');
            $.myWindow.setMyWindow("DecContainer2DecContainerEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '集装箱',
                width: '1000px',
                height: '500px'
            });
        }

        function Edit(ID) {
            var DeclarationID = $("#DeclarationID").val();
            var url = location.pathname.replace(/DecContainer.aspx/ig, 'DecContainerEdit.aspx?DeclarationID=' + DeclarationID + '&DecConID=' + ID + '&SourceCon=Edit');
            $.myWindow.setMyWindow("DecContainer2DecContainerEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '集装箱',
                width: '1000px',
                height: '500px'
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

        function SearchButton() {
            var parm = {
                DeclarationID: ""
            };
            $('#orders').myDatagrid("search", parm);
        }

        function SeachContainer(ID) {
            var DeclarationID = $("#DeclarationID").val();
            var url = location.pathname.replace(/DecContainer.aspx/ig, 'DecContainerEdit.aspx?DeclarationID=' + DeclarationID + '&DecConID=' + ID + '&SourceCon=Search');
            $.myWindow.setMyWindow("DecContainer2DecContainerEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '集装箱',
                width: '900px',
                height: '400px'
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <input type="hidden" id="DeclarationID" />
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin:5px;">
        <table id="orders" title="集装箱" data-options="fitColumns:true,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar" style="width: 100%; height: auto">
            <thead>
                <tr>
                    <th data-options="field:'ContainerID',align:'center'" style="width: 20%">集装箱号</th>
                    <th data-options="field:'ContainerMd',align:'center'" style="width: 20%">集装箱规格</th>
                    <th data-options="field:'GoodsNo',align:'center'" style="width: 11%">商品项号</th>
                    <th data-options="field:'LclFlag',align:'center'" style="width: 11%">拼箱标识</th>
                    <th data-options="field:'GoodsContaWt',align:'center'" style="width:11%">装箱重量(KG)</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 14%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
