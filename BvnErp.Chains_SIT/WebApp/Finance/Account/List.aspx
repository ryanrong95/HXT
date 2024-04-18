<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Account.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>金库账户查询界面</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <%-- <script>
        gvSettings.fatherMenu = '银行账户管理';
        gvSettings.menu = '账户管理';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">       
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var AccountSource = eval('(<%=this.Model.AccountSource%>)');       
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true, fit: true, toolbar: '#topBar', nowrap: false, rownumbers: true,
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
            //初始化Combobox
            $('#FinanceVault').combobox({
                data: FinanceVaultData,
            });
            //初始化Combobox
            $('#AccountSource').combobox({
                data: AccountSource,
            });

            $("#AccountSource").combobox('setValue', 1);
        });

        //查询
        function Search() {
            var FinanceVault = $('#FinanceVault').combobox('getValue');
            var BankAccount = $('#BankAccount').textbox('getValue');
            var AccountSource = $('#AccountSource').combobox('getValue');
            var parm = {
                FinanceVault: FinanceVault,
                BankAccount: BankAccount,
                AccountSource: AccountSource
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#FinanceVault').combobox('setValue', null);
            $('#BankAccount').textbox('setValue', null);
            $('#AccountSource').combobox('setValue', 1);
            Search();
        }

        //新增
        function BtnAdd() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增金库账户',
                width: '680',
                height: '450',
                url: url,
                onClose: function () {
                    Search();
                }
            });
        }

        //新增
        function BtnAddSimple() {
            var url = location.pathname.replace(/List.aspx/ig, 'EditPayee.aspx') + '?From=Add';
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增简易账户',
                width: '400',
                height: '200',
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
                var Width = 680;
                var Height = 450;
                if (rowdata.Source ==<%=Needs.Ccs.Services.Enums.AccountSource.easy.GetHashCode()%>) {
                    url = location.pathname.replace(/List.aspx/ig, 'EditPayee.aspx') + "?From=Edit&ID=" + rowdata.ID;
                    Width = 400;
                    Height = 200;
                }

                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '编辑金库账户',
                    width: Width,
                    height: Height,
                    url: url,
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
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BtnAdd()">新增</a>
                    <a id="btnAddSimple" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BtnAddSimple()">新增简易账户</a>
                </li>
            </ul>
        </div>
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">金库:</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">银行账号:</td>
                    <td>
                        <input class="easyui-textbox" id="BankAccount" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">是否简易:</td>
                    <td>
                        <input class="easyui-combobox" id="AccountSource" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
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
        <table id="datagrid" title="账户列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'FinanceVaultName',align:'left'" style="width: 80px;">金库名</th>
                    <th data-options="field:'AccountName',align:'left'" style="width: 80px;">账户名称</th>
                    <th data-options="field:'BankName',align:'left'" style="width: 80px;">银行名称</th>
                    <th data-options="field:'BankAddress',align:'left'" style="width: 100px;">银行地址</th>
                    <th data-options="field:'BankAccount',align:'left'" style="width: 130px;">银行账号</th>
                    <th data-options="field:'SwiftCode',align:'left'" style="width: 100px;">银行代码</th>
                    <th data-options="field:'CustomizedCode',align:'left'" style="width: 100px;">自定义代码</th>
                    <th data-options="field:'Currency',align:'left'" style="width: 50px;">币种</th>
                    <th data-options="field:'Balance',align:'left'" style="width: 90px;">余额</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
