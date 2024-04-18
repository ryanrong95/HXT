<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductControlList.aspx.cs" Inherits="WebApp.SysConfig.ProductControlList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管控产品-系统配置</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>       
        gvSettings.fatherMenu = '系统配置（xdt）';
        gvSettings.menu = '管控产品';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript"> 
        $(function () {
            var ProductControlTypeData = eval('(<%=this.Model.ProductControlType%>)');
            $('#Type').combobox({
                data: ProductControlTypeData,
                options: {
                    valueField: 'Value',
                    textField: 'Text',
                    required: true,
                    editable: false
                }
            });

            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
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
                idField: 'ID',
                remoteSort: true
            });
        });

        function Operation(val, row, index) {

            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    });
                }
            });
        }

        function Search() {
            var Type = $("#Type").combobox('getValue');
            var Model = $("#Model").val();
            var Name = $("#Name").val();

            $('#datagrid').myDatagrid('search', { ControlType: Type, Name: Name, Model: Model });
        }

        function Reset() {
            $("#Type").textbox("setValue", "");
            $("#Model").textbox("setValue", "");
            $("#Name").textbox("setValue", "");
            Search();
        }

        function Add() {
            var url = location.pathname.replace(/ProductControlList.aspx/ig, 'ProductControlInfo.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增管控产品',
                url: url,
                width: '420px',
                height: '250px',
                onClose: function () {
                    Search();
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">管控类型:</span>
                    <input class="easyui-combobox search" id="Type" data-options="valueField:'ID',textField:'Name'" />
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox search" id="Model" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <span class="lbl">品名: </span>
                    <input class="easyui-textbox search" id="Name" data-options="validType:'length[1,150]',tipPosition:'bottom'" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="管控产品" class="easyui-datagrid" style="width: 100%; height: 100%" data-options="nowrap:false,border:false,fitColumns:true,fit:true,toolbar:'#topBar'"
            singleselect="true" remotesort="true">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'left'" style="width: 150px" sortable="true">品名</th>
                    <th field="Model" data-options="align:'left'" style="width: 150px">型号</th>
                    <th field="Manufacturer" data-options="align:'left'" style="width: 150px">品牌</th>
                    <th field="Type" data-options="align:'center'" style="width: 100px">管控类型</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px" sortable="true">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 150px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
