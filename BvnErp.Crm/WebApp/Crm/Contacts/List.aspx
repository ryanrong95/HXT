<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Contacts.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>客户联系人</title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        window.getQueryString = function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]);
            return "";
        }

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


        //新增
        function Add() {
            var clientid = getQueryString('ClientID');
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + clientid;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '联系人编辑',
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
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&ClientID=" + clientid;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '联系人编辑',
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
                    $("#hidID").val(ID);
                    $("#btnDelete").click();
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="Edit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="Delete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">

    <div title="客户联系人" data-options="region:'north',border:false" style="height: 55px">
        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Name" data-options="align:'center'" style="width: 100px">联系人</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 150px">客户名称</th>
                    <th field="Position" data-options="align:'center'" style="width: 100px;">公司职位</th>
                    <th field="Email" data-options="align:'center'" style="width: 100px;">邮箱</th>
                    <th field="Mobile" data-options="align:'center'" style="width: 100px;">手机</th>
                    <th field="Tel" data-options="align:'center'" style="width: 150px;">电话</th>
                </tr>
            </thead>
        </table>
    </div>
    <div data-options="region:'south',border:false" style="display: none">
        <form id="form1" runat="server" style="display: none">
            <input type="hidden" id="hidID" runat="server" />
            <asp:Button runat="server" ID="btnDelete" OnClick="btnDelete_Click"/>
        </form>
    </div>
</body>
</html>
