<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Invoice.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>  
    <title>开票信息</title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        //页面加载时
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
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

        function Add() {
            var clientid = getQueryString('ClientID');                
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + clientid ;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '发票编辑',
                width: '500px',
                height: '500px',
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            var clientid = getQueryString('ClientID');       
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&ClientID=" + clientid ;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '发票编辑',
                    width: '500px',
                    height: '500px',
                    onClose: function () {
                        $('#datagrid').datagrid('reload');
                    }
                }).open();
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
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
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
  
    <div title="开票信息" data-options="region:'north',border:false" style="height: 55px" >
        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
    </div>
    <div data-options="region:'center',border:false" >
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Type" data-options="align:'center'" style="width: 100px">发票类型</th>
                    <th field="CompanyName" data-options="align:'center'" style="width: 100px">公司名称</th>
                    <th field="CompanyCode" data-options="align:'center'" style="width: 150px">税号</th>
                    <th field="Phone" data-options="align:'center'" style="width: 100px;">联系电话</th>
                    <th field="Bank" data-options="align:'center'" style="width: 100px;">开户行</th>
                    <th field="Account" data-options="align:'center'" style="width: 100px;">账号</th>
                    <th field="Contact" data-options="align:'center'" style="width: 100px;">联系人</th>
                </tr>
            </thead>
        </table>
    </div>   
</body>
</html>
