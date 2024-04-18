<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Claim.List" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>非正式会员-待认领</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        gvSettings.fatherMenu = '待认领(XDT)';
        gvSettings.menu = '非正式会员列表';
    </script>
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
        });

        //列表框按钮加载
        function Operation(val, row, index) {

           var buttons   = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Deal(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">处理</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

          //处理
        function Deal(id) {
            $.messager.confirm('确认', '请确认是否联系此客户', function (success) {
                if (success) {
                    $.post('?action=ToDeal', { ID: id }, function (res) {
                           var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, "info", function () {
                        if (result.success) {
                            $('#clients').myDatagrid('reload');
                        }
                    });

                       
                    })
                }
            });
        }
        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var Status = $('#Status').combobox('getValue');
            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Status: Status
            };
            $('#clients').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#Status').combobox('setValue', null);
            Search();
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
                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox search" id="Status" />
                    <br />
                    <span class="lbl">创建日期:</span>
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
        <table id="clients" data-options="singleSelect:true,border:false,fit:true,nowrap:false,scrollbarSize:0" title="会员列表" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 25%;">客户名称</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 7%;">客户编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 6%;">信用等级</th>
                    <th data-options="field:'ContactName',align:'center'" style="width: 6%;">联系人</th>
                    <th data-options="field:'ContactTel',align:'center'" style="width: 9%;">联系人电话</th>
                    <th data-options="field:'ServiceManagerName',align:'center'" style="width: 8%;">业务员</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 8%;">跟单员</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 7%;">注册时间</th>
                    <th data-options="field:'Status',align:'center'" style="width: 6%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

