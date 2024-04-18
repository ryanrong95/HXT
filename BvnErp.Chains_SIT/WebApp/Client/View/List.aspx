<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.View.List" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>我的会员</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--  <script>
        gvSettings.fatherMenu = '会员管理(XDT)';
        gvSettings.menu = '我的会员-跟单';
        gvSettings.summary = '跟单员查看会员的菜单';
    </script>--%>
    <script type="text/javascript">
        var PvWsOrderUrl = '<%=this.Model.PvWsOrderUrl%>';
        //数据初始化
        $(function () {
            //下拉框数据初始化
            var status = eval('(<%=this.Model.Status%>)');

            $('#Status').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: status
            });
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
                }
            });

            $("#All").click(function () {
                if ($(this).is(":checked")) {
                    $("#Normal").prop("checked", false);
                    $("#AbNormal").prop("checked", false);
                    Search();
                }
            });
            $("#Normal").click(function () {
                if ($(this).is(":checked")) {
                    $("#All").prop("checked", false);
                    $("#AbNormal").prop("checked", false);
                    Search();
                }
            });
            $("#AbNormal").click(function () {
                if ($(this).is(":checked")) {
                    $("#All").prop("checked", false);
                    $("#Normal").prop("checked", false);
                    Search();
                }
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="AddOrder(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">新增订单</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\',\'' + row.ClientAdminID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-Search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //超期未换汇
        function ViewUnPayExchange(val, row, index) {
            if (row.ServiceType == '代仓储') {
                return "-";
            }
            else {
                if (val >= 500000) {
                    return "<label style='color:red'>" + val + "</label>";
                }
                return val;
            }
        }
        //近期报关金额
        function ViewDeclare(val, row, index) {
            if (row.ServiceType == '代仓储') {
                return "-";
            }
            else {
                return val;
            }
        }
        //近期付汇金额
        function ViewPayExchange(val, row, index) {
            if (row.ServiceType == '代仓储') {
                return "-";
            }
            else {
                return val;
            }
        }

        //详情
        function View(id) {
            if (id) {
                var url = location.pathname.replace(/View\/List.aspx/ig, 'Index.aspx') + '?Source=View&ID=' + id;
                window.location = url;
            }
        }

        //新增订单
        function AddOrder(index) {
            $('#clients').datagrid('selectRow', index);
            var rowdata = $('#clients').datagrid('getSelected');
            <%--if (rowdata) {
                if (rowdata.StatusValue == '<%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%>') {
                    $.messager.alert('提示', '客户资料未完善，不能下单！');
                    return;
                }
                if (rowdata.ClientRankValue == '<%=Needs.Ccs.Services.Enums.ClientRank.ClassSix.GetHashCode()%>') {
                    $.messager.alert('提示', '六级客户，不能下单！');
                    return;
                }

                var url = location.pathname.replace(/List.aspx/ig, '../../Order/Edit.aspx') + '?ClientID=' + rowdata.ID;
                window.location = url;
            }--%>

            $.myWindow({
                title: '新增报关订单',
                minWidth: 1200,
                minHeight: 600,
                url: PvWsOrderUrl + 'Orders/AddDeclare.aspx?Name=' + rowdata.CompanyName + '&EnterCode=' + rowdata.ClientCode,
                onClose: function () {
                },
            });
            return false;
        }

        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var UnPayExchangeAmount = $('#UnPayExchangeAmount').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var Status = $('#Status').combobox('getValue');
            var NormalType = 2;
            if ($('#Normal').is(':checked')) {
                NormalType = 1;
            }
            if ($('#AbNormal').is(':checked')) {
                NormalType = 0;
            }
            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                UnPayExchangeAmount: UnPayExchangeAmount,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Status: Status,
                NormalType: NormalType
            };
            $('#clients').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#UnPayExchangeAmount').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#Status').combobox('setValue', null);
            Search();
        }

        function ViewNormal(val, row, index) {
            if (row.isNormal == true) {
                return '正常'

            } else {
                return '异常';
            }
        }


    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户名称: </span>
                    <input class="easyui-textbox" id="CompanyName" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox" id="Status" />
                    <span class="lbl">超期未换汇金额: </span>
                    <input class="easyui-textbox" id="UnPayExchangeAmount" />
                    <br />
                    <span class="lbl">创建日期: </span>
                    <input class="easyui-datebox" id="CreateDateFrom" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="CreateDateTo" data-options="editable:false" />
                    <input type="checkbox" name="Order" value="2" id="All" title="全部客户" class="checkbox checkboxlist" checked="checked" /><label for="All" style="margin-right: 20px">全部客户</label>
                    <input type="checkbox" name="Order" value="1" id="Normal" title="正常客户" class="checkbox checkboxlist" /><label for="Normal" style="margin-right: 20px">正常客户</label>
                    <input type="checkbox" name="Order" value="0" id="AbNormal" title="异常客户" class="checkbox checkboxlist" /><label for="AbNormal">异常客户</label>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="clients" data-options="fitColumns:true,border:false,fit:true,nowrap:false,toolbar:'#topBar',rownumbers:true," title="会员列表">
            <thead>
                <tr>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 14%;">客户名称</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 5%;">信用等级</th>
                    <th data-options="field:'ContactName',align:'center'" style="width: 6%;">联系人</th>
                    <th data-options="field:'ContactTel',align:'center'" style="width: 7%;">联系人电话</th>
                    <th data-options="field:'ServiceManagerName',align:'center'" style="width: 5%;">报关业务员</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 5%;">报关跟单员</th>
<%--                    <th data-options="field:'StorageServiceManagerName',align:'center'" style="width: 8%;">代仓储业务员</th>
                    <th data-options="field:'StorageMerchandiserName',align:'center'" style="width: 8%;">代仓储跟单员</th>--%>
                    <th data-options="field:'ClientNature',align:'center'" style="width: 4%;">客户类型</th>
                    <th data-options="field:'ServiceType',align:'center'" style="width: 5%;">业务类型</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 5%;">创建日期</th>
                    <th data-options="field:'Status',align:'center'" style="width: 5%;">状态</th>
                    <th data-options="field:'UnPayExchangeAmount',align:'center',formatter:ViewUnPayExchange" style="width: 5%;">超期未换汇</th>
                    <th data-options="field:'DeclareAmount',align:'center',formatter:ViewDeclare" style="width: 5%;">近期报关</th>
                    <th data-options="field:'PayExchangeAmount',align:'center',formatter:ViewPayExchange" style="width: 5%;">近期付汇</th>
                    <th data-options="field:'IsNormal',align:'center',formatter:ViewNormal" style="width: 6%;">是否异常</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 12%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

