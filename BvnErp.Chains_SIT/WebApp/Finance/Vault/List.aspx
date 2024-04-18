<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Vault.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>金库查询界面</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '银行账户管理';
        gvSettings.menu = '金库管理';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,
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
        });

        //查询
        function Search() {
            var Name = $('#Name').textbox('getValue');
            var Leader = $('#Leader').textbox('getValue');
            var parm = {
                Name: Name,
                Leader: Leader
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Name').textbox('setValue', null);
            $('#Leader').textbox('setValue', null);
            Search();
        }

        //新增
        function BtnAdd() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增金库',
                width: '450px',
                height: '220px',
                url: url,
                onClose: function () {
                    Search();
                }
            });
        }

        //编辑
        function Edit(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '编辑金库',
                    url: url,
                    width: '450px',
                    height: '230px',
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            //buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">删除</span>' +
            //    '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BtnAdd()">新增</a>
        </div>
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">名称:</td>
                    <td>
                        <input class="easyui-textbox" id="Name" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">负责人:</td>
                    <td>
                        <input class="easyui-textbox" id="Leader" data-options="height:26,width:200" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="金库列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'" style="width: 100px;">名称</th>
                    <th data-options="field:'Leader',align:'center'" style="width: 100px;">负责人</th>
                    <th data-options="field:'Summary',align:'center'" style="width: 100px;">备注</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
