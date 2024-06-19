<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.User.List" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var clientID = JSON.parse(ID);
        //数据初始化
        $(function () {
            //下拉框数据初始化

            //订单列表初始化
            $('#useraccounts').myDatagrid({
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

            if (InitClientPage()) {
                $('#useraccounts').datagrid('hideColumn', 'Btn');
            }
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDeal" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Reset(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">重置密码</span>' +
                '<span class="l-btn-icon icon-reload">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        function Edit(id) {

            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + clientID.ID + '&UserID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑会员账号',
                width: '650px',
                height: '360px',
                onClose: function () {
                    $('#useraccounts').datagrid('reload');
                }
            });
        }

        function Reset(userid) {
            $.messager.confirm('确认', '确认重置密码？', function (r) {
                if (r) {
                    $.post('?action=ResetPassword', { ID: clientID.ID, UserID: userid }, function () {
                        $.messager.alert('消息', "密码已重置为：HXT123");
                    });
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除会员账号！', function (success) {
                if (success) {
                    $.post('?action=DeleteUserAccount', { ID: id }, function () {
                        $.messager.alert('消息', '删除成功！');
                        $('#useraccounts').myDatagrid('reload');
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + clientID.ID + '&UserID=');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增会员账号',
                width: '650px',
                height: '360px',
                onClose: function () {
                    $('#useraccounts').datagrid('reload');
                }
            });
        }

        function Return() {
            var source = window.parent.frames.Source;
            var u = "";
            switch (source) {
                case 'Add':
                    u = '../New/List.aspx';
                    break;
                case 'Assign':
                    u = '../Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = '../New/List.aspx';
                    break;
                case 'ApproveView':
                    u = '../Approval/List.aspx';
                    break;
                case 'QualifiedView':
                    u = '../Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = '../ServiceManagerView/List.aspx';
                    break;
                default:
                    u = '../View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/List.aspx/ig, u);
            window.parent.location = url;
        }
    </script>
</head>

<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 5px">
        <table id="useraccounts" data-options="singleSelect:true,border:true,fit:true,nowrap:false,scrollbarSize:0" title="会员账号" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left'" style="width: 15%;">账号</th>
                    <%--   <th data-options="field:'RealName',align:'left'" style="width: 15%;">真实姓名</th>--%>
                    <th data-options="field:'Mobile',align:'left'" style="width: 8%;">手机</th>
                    <th data-options="field:'Email',align:'left'" style="width: 12%;">邮箱</th>
                    <th data-options="field:'IsMain',align:'center'" style="width: 8%;">是否主账号</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 10%;">备注</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 18%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
