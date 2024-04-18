<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Consignees.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>地址簿</title>
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

        //新增
        function Add() {
            var clientid = getQueryString('ClientID');
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + clientid;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '地址簿新增',
                width: '800px',
                height: '400px',
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
                    title: '地址簿编辑',
                    width: '800px',
                    height: '400px',
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
    <div title="地址簿列表" data-options="region:'north',border:false" style="height: 60px">
        <div>
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="CompanyName" data-options="align:'center'" style="width: 200px">公司</th>
                    <th field="ContactName" data-options="align:'center'" style="width: 100px">联系人</th>
                    <th field="Address" data-options="align:'center'" style="width: 300px">地址</th>
                    <th field="Zipcode" data-options="align:'center'" style="width: 100px">邮编</th>
                </tr>
            </thead>
        </table>
    </div>
    <div data-options="region:'south',border:false" style="display: none">
        <form id="form1" runat="server" style="display: none">
            <input type="hidden" id="hidID" runat="server" />
            <asp:Button runat="server" ID="btnDelete" OnClick="btnDelete_Click" />
        </form>
    </div>
</body>
</html>
