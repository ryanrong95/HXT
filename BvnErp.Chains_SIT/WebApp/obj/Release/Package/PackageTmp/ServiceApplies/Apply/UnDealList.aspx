<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnDealList.aspx.cs" Inherits="WebApp.Ccs.ServiceApplies.Apply.UnDealList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>注册申请</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '注册申请(XDT)';
        gvSettings.menu = '未处理';
        gvSettings.summary = '业务员处理注册申请';
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //下拉框数据初始化
            var status = eval('(<%=this.Model.Status%>)');
            $('#Status').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: status
            });
            //申请列表初始化
            $('#applies').myDatagrid({
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
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';

            if (row.Status == '待处理') {
                buttons += '<a id="btnDeal" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Deal(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">处理</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            }
            return buttons;
        }

        function View(ID) {
            var url = location.pathname.replace(/UnDealList.aspx/ig, 'Edit.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看注册申请',
                width: '750px',
                height: '400px',
                onClose: function () {
                    $('#applies').datagrid('reload');
                }
            });
        }

        function Deal(ID) {
            $.messager.confirm('确认', '确认已联系过该申请的联系人或公司？', function (r) {
                if (r) {
                    $.post('?action=SaveApplyHandle', { ID: ID }, function (res) {
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
                        });
                        Search();
                    });
                }
            });
        }

        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var parm = {
                CompanyName: CompanyName,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
            };
            $('#applies').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">公司名称: </span>
                    <input class="easyui-textbox search" id="CompanyName" />
                    <span class="lbl">申请日期: </span>
                    <input type="text" class="easyui-datebox search" id="CreateDateFrom" />
                    <span class="lbl">至: </span>
                    <input type="text" class="easyui-datebox search" id="CreateDateTo" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="applies" data-options="singleSelect:true,fit:true,nowrap:false,scrollbarSize:0" title="注册申请" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 18%;">公司名称</th>
                    <th data-options="field:'Address',align:'left'" style="width: 25%;">地址</th>
                    <th data-options="field:'Contact',align:'center'" style="width: 8%;">联系人</th>
                    <th data-options="field:'Mobile',align:'center'" style="width: 9%;">手机</th>
                    <th data-options="field:'Tel',align:'center'" style="width: 9%;">电话</th>
                    <th data-options="field:'Email',align:'center'" style="width: 10%;">邮箱</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">申请日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 9%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

