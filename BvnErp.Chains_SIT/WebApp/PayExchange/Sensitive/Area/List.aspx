<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.PayExchange.Sensitive.Area.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        var areaType = eval('(<%=this.Model.AreaType%>)');
        var editAreaID = '';

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

            $("#AreaType").combobox({
                data: areaType
            });

            $('#edit-area-dialog').dialog({
                buttons: [{
                    text: '提交',
                    iconCls:'icon-save',
                    handler: function () {
                        if (!$('#form1').form('validate')) {
                            return;
                        }

                        var areaID = editAreaID;
                        var areaName = $('#AreaName').textbox('getValue');
                        var areaType = $('#AreaType').combobox('getValue');

                        MaskUtil.mask();

                        var url = location.pathname.replace(/List.aspx/ig, 'List.aspx?action=EditArea');                        
                        var params = {
                            ID: areaID,
                            Name: areaName,
                            Type: areaType,
                        };
                        $.post(url, params, function (res) {
                            MaskUtil.unmask();
                            var resData = JSON.parse(res);
                            if (resData.success == true) {
                                $.messager.alert('提示', resData.message, 'info', function () {
                                    $('#edit-area-dialog').dialog('close');
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
                        $('#edit-area-dialog').dialog('close');
                    }
                }]
            });
        });

        //操作
        function Operation(val, row, index) {
            var buttons = '';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="WordWindow(\'' + row.AreaID + '\',\'' + row.AreaName + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">关键词</span>' +
                '<span class="l-btn-icon icon-tip">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" ' +
                'onclick="EditWindow(\'' + row.AreaID + '\',\'' + row.AreaTypeCode + '\',\'' +  row.AreaName + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.AreaID + '\',\'' + row.AreaName + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        function AddWindow() {
            editAreaID = '';
            $('#AreaName').textbox('setValue', '');
            $('#AreaType').combobox('setValue', '');
            $('#edit-area-dialog').dialog('open');
        }

        function EditWindow(areaID, areaTypeCode, areaName) {
            editAreaID = areaID;
            $('#AreaName').textbox('setValue', areaName);
            $('#AreaType').combobox('setValue', areaTypeCode);
            $('#edit-area-dialog').dialog('open');
        }

        function Delete(areaID, areaName) {
            $.messager.alert('提示', '确定要删除 ' + areaName + ' 吗？', 'info', function () {

                var url = location.pathname.replace(/List.aspx/ig, 'List.aspx?action=DeleteArea');                        
                var params = {
                    ID: areaID,
                };

                MaskUtil.mask();
                $.post(url, params, function (res) {
                    MaskUtil.unmask();
                    var resData = JSON.parse(res);
                    if (resData.success == true) {
                        $.messager.alert('提示', resData.message, 'info', function () {
                            $('#edit-area-dialog').dialog('close');
                            $('#datagrid').datagrid('reload');
                        });
                    } else {
                        $.messager.alert('提示', resData.message);
                    }
                });
            });
        }

        function WordWindow(areaID, areaName) {
            var url = location.pathname.replace(/Area\/List.aspx/ig, 'Word/List.aspx')
                + "?AreaID=" + areaID
                + "&AreaName=" + areaName;
            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: areaName + ' 关键词',
                width: '500',
                height: '350',
                url: url,
                onClose: function () {

                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1" style="margin: 5px 0 5px 0">
                <tr>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddWindow()">新增</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title=" 付汇银行敏感地区列表" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="AreaName" data-options="align:'left'" style="width: 50px">地区名称</th>
                    <th field="AreaTypeDes" data-options="align:'center'" style="width: 50px">类型</th>
                    <th data-options="field:'btn',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 新增 Area html Begin ------------------------------------------------------------>

    <div id="edit-area-dialog" class="easyui-dialog" title="编辑敏感地区" style="width:400px;height:150px;" 
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <form id="form1" runat="server">
            <table id="editTable" style="margin-left: 20px">
                <tr>
                    <td class="lbl">地区名称:</td>
                    <td>
                        <input class="easyui-textbox input" id="AreaName" name="AreaName" data-options="valueField:'TypeValue',textField:'TypeText',required:true," />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">类型:</td>
                    <td>
                        <input class="easyui-combobox input" id="AreaType" name="AreaType" data-options="validType:'length[1,50]',tipPosition:'right',editable:false,
                            valueField:'Key',textField:'Value',required:true," />
                    </td>
                </tr>
            </table>
        </form>
    </div>

    <!------------------------------------------------------------ 新增 Area html End ------------------------------------------------------------>

</body>
</html>
