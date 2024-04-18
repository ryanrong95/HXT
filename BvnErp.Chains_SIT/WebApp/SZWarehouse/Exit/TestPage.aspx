<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="WebApp.SZWarehouse.Exit.TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        $(function () {

        });

        function Upload() {
            MaskUtil.mask();
            $.post('?action=Upload', { ID: "11" }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function BarchInsert() {
            MaskUtil.mask();
            $.post('?action=BarchInsert', { IDs: "ooo,ppp,qqq" }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function Delete() {
            var DeleteID = $('#DeleteID').textbox('getValue');
            DeleteID = DeleteID.trim();
            $('#DeleteID').textbox('setValue', DeleteID);

            MaskUtil.mask();
            $.post('?action=Delete', { ID: DeleteID }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function Update() {
            var UpdateID = $('#UpdateID').textbox('getValue');
            UpdateID = UpdateID.trim();
            $('#UpdateID').textbox('setValue', UpdateID);

            var UpdateAdminID = $('#UpdateAdminID').textbox('getValue');
            UpdateAdminID = UpdateAdminID.trim();
            $('#UpdateAdminID').textbox('setValue', UpdateAdminID);

            var UpdateName = $('#UpdateName').textbox('getValue');
            UpdateName = UpdateName.trim();
            $('#UpdateName').textbox('setValue', UpdateName);

            MaskUtil.mask();
            $.post('?action=Update', { ID: UpdateID, AdminID: UpdateAdminID, Name: UpdateName, }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function UpdateChangeds() {
            var UpdateID = $('#UpdateID').textbox('getValue');
            UpdateID = UpdateID.trim();
            $('#UpdateID').textbox('setValue', UpdateID);

            var UpdateAdminID = $('#UpdateAdminID').textbox('getValue');
            UpdateAdminID = UpdateAdminID.trim();
            $('#UpdateAdminID').textbox('setValue', UpdateAdminID);

            var UpdateName = $('#UpdateName').textbox('getValue');
            UpdateName = UpdateName.trim();
            $('#UpdateName').textbox('setValue', UpdateName);

            MaskUtil.mask();
            $.post('?action=UpdateChangeds', { ID: UpdateID, AdminID: UpdateAdminID, Name: UpdateName, }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function UpdateProblem() {
            MaskUtil.mask();
            $.post('?action=UpdateProblem', { }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function UpdateObjects() {
            MaskUtil.mask();
            $.post('?action=UpdateObjects', { }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function DeleteBoolean() {
            MaskUtil.mask();
            $.post('?action=DeleteBoolean', { }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }

        function TestExceptionLog() {
            MaskUtil.mask();
            $.post('?action=TestExceptionLog', { }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {

                    $.messager.alert('提示', resultJson.message, 'info', function () {
                        
                    });
                } else {
                    $.messager.alert('错误', resultJson.message, 'error', function () {
                        
                    });
                }

            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="Upload()">上传</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="BarchInsert()" style="margin-left: 50px;">批量插入</a>
        </div>
        <div>
            <input class="easyui-textbox search" id="DeleteID" style="width: 250px;" />
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="Delete()">删除</a>
        </div>
        <div>
            <input class="easyui-textbox search" id="UpdateID" style="width: 250px;" />
            <input class="easyui-textbox search" id="UpdateAdminID" style="width: 100px;" />
            <input class="easyui-textbox search" id="UpdateName" style="width: 100px;" />
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="Update()">更新</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="UpdateChangeds()">更新Changeds</a>
        </div>
        <div>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="UpdateProblem()">更新Problem</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="UpdateObjects()" style="margin-left: 50px;">更新Objects</a>
        </div>
        <div>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="DeleteBoolean()">删除Boolean</a>
        </div>
        <div>
            <a href="javascript:void(0);" class="easyui-linkbutton" width:100, height:30," onclick="TestExceptionLog()">TestExceptionLog</a>
        </div>
    </form>
</body>
</html>
