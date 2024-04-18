<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.ServiceManagerView.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>会员查询</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        //数据初始化
        $(function () {
            //订单列表初始化
            $('#clients').myDatagrid({
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

            });


        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\',\'' + row.ClientAdminID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }


        function View(id, ClientAdminID) {
            debugger
            if (id) {
                var url = location.pathname.replace(/ServiceManagerView\/List.aspx/ig, 'Index.aspx') + '?Source=ServiceManagerView&ID=' + id + "&adminId=" + ClientAdminID;
                window.location = url;
            }
        }


        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');

            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
            };
            $('#clients').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            Search();
        }

        function ViewSummary(val, row, index) {
            var status = parseInt(row.statusValue);
            if (status ==<%=Needs.Ccs.Services.Enums.ClientStatus.Returned.GetHashCode()%>) {
                return '<a href="javascript:void(0);" onclick="Reason(\'' + row.Summary + '\')" >' + val + '</a>'

            } else {
                return val;
            }
        }



        function Reason(Summary) {
            $.messager.alert("退回原因", Summary);
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户名称:</span>
                    <input class="easyui-textbox search" id="CompanyName" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <%-- <span class="lbl">状态: </span>
                    <input class="easyui-combobox search" id="Status" />--%>

                    <span class="lbl">注册日期:</span>
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
        <table id="clients" data-options="singleSelect:true,fit:true,border:false,nowrap:false,scrollbarSize:0,rownumbers:true," title="会员查询" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 13%;">客户名称</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 5%;">信用等级</th>
                    <th data-options="field:'ServiceTypeDes',align:'center'" style="width: 6%;">业务类型</th>
                    <th data-options="field:'ServiceManagerName',align:'center'" style="width: 6%;">业务员</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 6%;">跟单员</th>
                    <th data-options="field:'Referrer',align:'center'" style="width: 6%;">引荐人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 6%;">注册日期</th>
                    <th data-options="field:'Status',align:'center',formatter:ViewSummary" style="width: 6%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
