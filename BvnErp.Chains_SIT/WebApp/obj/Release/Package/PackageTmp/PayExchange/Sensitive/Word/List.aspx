<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.PayExchange.Sensitive.Word.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var areaID = '<%=this.Model.AreaID%>';
        var editWordID = '';

        $(function () {
            $('#datagrid').myDatagrid({
                pageList: [10,20,30,40,50, 50000],
                pageSize: 50000,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

            $('#edit-word-dialog').dialog({
                buttons: [{
                    text: '提交',
                    iconCls:'icon-save',
                    handler: function () {
                        if (!$('#form1').form('validate')) {
                            return;
                        }

                        var wordContent = $('#WordContent').textbox('getValue');

                        MaskUtil.mask();

                        var url = location.pathname.replace(/List.aspx/ig, 'List.aspx?action=EditWord');                        
                        var params = {
                            ID: editWordID,
                            AreaID: areaID,
                            Content: wordContent,
                        };
                        $.post(url, params, function (res) {
                            MaskUtil.unmask();
                            var resData = JSON.parse(res);
                            if (resData.success == true) {
                                $.messager.alert('提示', resData.message, 'info', function () {
                                    $('#edit-word-dialog').dialog('close');
                                    $('#datagrid').datagrid('reload');
                                });
                            } else {
                                $.messager.alert('提示', resData.message);
                            }
                        });
                    }
                }, {
                    text: '取消',
                    iconCls:'icon-cancel',
                    handler: function () {
                        $('#edit-word-dialog').dialog('close');
                    }
                }]
            });
        });

        //操作
        function Operation(val, row, index) {
            var buttons = '';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" ' +
                'onclick="EditWindow(\'' + row.WordID + '\',\'' +  row.WordContent + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.WordID + '\',\'' + row.WordContent + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        function AddWindow() {
            editWordID = '';
            $('#WordContent').textbox('setValue', '');
            $('#edit-word-dialog').dialog('open');
        }

        function EditWindow(wordID, wordContent) {
            editWordID = wordID;
            $('#WordContent').textbox('setValue', wordContent);
            $('#edit-word-dialog').dialog('open');
        }

        function Delete(wordID, wordName) {
            $.messager.alert('提示', '确定要删除 ' + wordName + ' 吗？', 'info', function () {

                var url = location.pathname.replace(/List.aspx/ig, 'List.aspx?action=DeleteWord');                        
                var params = {
                    ID: wordID,
                };

                MaskUtil.mask();
                $.post(url, params, function (res) {
                    MaskUtil.unmask();
                    var resData = JSON.parse(res);
                    if (resData.success == true) {
                        $.messager.alert('提示', resData.message, 'info', function () {
                            $('#edit-word-dialog').dialog('close');
                            $('#datagrid').datagrid('reload');
                        });
                    } else {
                        $.messager.alert('提示', resData.message);
                    }
                });
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1">
                <tr>
                    <td>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddWindow()">新增</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            toolbar:'#topBar',
            showHeader:false,
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="WordContent" data-options="align:'left'" style="width: 50px">关键词</th>
                    <th data-options="field:'btn',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 新增 Word html Begin ------------------------------------------------------------>

    <div id="edit-word-dialog" class="easyui-dialog" title="编辑关键词" style="width:400px;height:120px;" 
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">关键词:</td>
                    <td>
                        <input class="easyui-textbox input" id="WordContent" name="WordContent" data-options="valueField:'TypeValue',textField:'TypeText',required:true," />
                    </td>
                </tr>
            </table>
        </form>
    </div>

    <!------------------------------------------------------------ 新增 Word html End ------------------------------------------------------------>

</body>
</html>
