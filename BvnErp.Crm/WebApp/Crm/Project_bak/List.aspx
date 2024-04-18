<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Project_bak.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <%-- <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '我的销售机会';
        gvSettings.summary = '';

    </script>--%>
    <script type="text/javascript">

        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var Admin = eval('(<%=this.Model.Admin%>)');
        var Manufacture = eval('(<%=this.Model.Manufacture%>)');
        var Current = eval('(<%=this.Model.Current%>)');
        var ProjectType = eval('(<%=this.Model.ProjectType%>)');
        var StatusData = eval('(<%=this.Model.Status%>)');

        //页面加载时
        $(function () {

            //解决特殊字符的问题
            $("#ManufactureID").combobox({
                onChange: function (newValue, oldValue) {
                    var text = escape2Html($(this).combobox('getText'));
                    $(this).combobox('setText', text);
                },
            });

            //状态
            $("#Status").combobox({
                onChange: function (newValue, oldValue) {
                    if (newValue != "") {
                        $("#StartDate").datebox("enable");
                        $("#EndDate").datebox("enable");
                    } else {
                        $("#StartDate").datebox("disable");
                        $("#EndDate").datebox("disable");
                    }
                },
            });

            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                    }
                    return data;
                }
            });
        });

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '销售机会新增',
                url: url,
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'EditTabs.aspx') + "?ProjectID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: ' ',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //产品信息
        function View(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'ProductList.aspx') + "?ID=" + rowdata.ID + "&&CatalogueID=" + rowdata.CatelogueID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '产品查看',
                    url: url,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            };
        }

        //Tab页
        function Detail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Tabs.aspx') + "?ProjectID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: ' ',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnDetail" onclick="Detail(\'' + index + '\')">详情</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var ID = $("#ID").val();
            var ProjectType = $("#ProjectType").combobox("getValue");
            var ClientID = $("#ClientID").combobox("getValue");
            var AdminID = $("#AdminID").combobox("getValue");
            var ManufactureID = $("#ManufactureID").combobox("getValue");
            var Status = $("#Status").combobox("getValue");
            var StartDate = $("#StartDate").datebox("getValue");
            var EndDate = $("#EndDate").datebox("getValue");
            $('#datagrid').bvgrid('flush', {
                ID: ID, ProjectType: ProjectType, ClientID: ClientID, AdminID: AdminID,
                ManufactureID: ManufactureID, Status: Status, StartDate: StartDate, EndDate: EndDate,
            });
        }

        //查询
        function Search() {
            var ID = $("#ID").val();
            var ProjectType = $("#ProjectType").combobox("getValue");
            var ClientID = $("#ClientID").combobox("getValue");
            var AdminID = $("#AdminID").combobox("getValue");
            var ManufactureID = $("#ManufactureID").combobox("getValue");
            var Status = $("#Status").combobox("getValue");
            var StartDate = $("#StartDate").datebox("getValue");
            var EndDate = $("#EndDate").datebox("getValue");
            if (new Date(StartDate) > new Date(EndDate)) {
                $.messager.alert('提示', '结束时间应该大于开始时间！');
                return;
            }
            $('#datagrid').bvgrid('search', {
                ID: ID, ProjectType: ProjectType, ClientID: ClientID, AdminID: AdminID,
                ManufactureID: ManufactureID, Status: Status, StartDate: StartDate, EndDate: EndDate,
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="项目列表" data-options="region:'north',border:false" style="height: 150px; margin-left: 10px">
        <table id="table1" style="margin-top: 10px; width: 100%">
            <tr>
                <th style="width: 8%"></th>
                <th style="width: 24%"></th>
                <th style="width: 8%"></th>
                <th style="width: 24%"></th>
                <th style="width: 8%"></th>
                <th style="width: 24%"></th>
            </tr>
            <tr>
                <td class="lbl">机会编号</td>
                <td>
                    <input class="easyui-textbox" id="ID" name="ID" style="width: 95%" />
                </td>
                <td class="lbl">客户名称</td>
                <td>
                    <input class="easyui-combobox" id="ClientID" name="ClientID"
                        data-options="valueField:'ID',textField:'Name',data: ClientData" style="width: 95%" />
                </td>
                <td class="lbl">所有人</td>
                <td>
                    <input class="easyui-combobox" id="AdminID" name="AdminID"
                        data-options="valueField:'ID',textField:'RealName',data: Admin" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">品牌</td>
                <td>
                    <input class="easyui-combobox" id="ManufactureID" name="ManufactureID"
                        data-options="valueField:'ID',textField:'Name',data: Manufacture" style="width: 95%" />
                </td>
                <td class="lbl">机会类型</td>
                <td>
                    <input class="easyui-combobox" id="ProjectType" name="ProjectType"
                        data-options="valueField:'value',textField:'text',data: ProjectType" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">销售状态</td>
                <td>
                    <input class="easyui-combobox" id="Status" name="Status"
                        data-options="valueField:'value',textField:'text',data: StatusData" style="width: 95%" />
                </td>
                <td class="lbl">状态开始时间</td>
                <td>
                    <input class="easyui-datebox" id="StartDate" name="StartDate" data-options="editable:false,disabled:true," style="width: 95%" />
                </td>
                <td class="lbl">状态结束时间</td>
                <td>
                    <input class="easyui-datebox" id="EndDate" name="EndDate" data-options="editable:false,disabled:true," style="width: 95%" />
                </td>
            </tr>
        </table>
        <div style="margin-top: 5px">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 150px">机会编号</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 150px">客户名称</th>
                    <th field="CurrencyName" data-options="align:'center'" style="width: 80px">币种</th>
                    <th field="RefValuation" data-options="align:'center'" style="width: 80px">总金额(万元)</th>
                    <th field="ExpectValuation" data-options="align:'center'" style="width: 80px">预期成交(万元)</th>
                    <th field="ClientAdminName" data-options="align:'center'" style="width: 80px">客户所有人</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 80px">机会建立人</th>
                    <th field="TypeName" data-options="align:'center'" style="width: 100px">机会类型</th>
                    <th field="CompanyName" data-options="align:'center'" style="width: 150px">公司</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 150px">更新时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
