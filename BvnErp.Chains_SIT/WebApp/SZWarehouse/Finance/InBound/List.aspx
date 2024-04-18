<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.SZWarehouse.Finance.InBound.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '财务(SZ)';
        gvSettings.menu = '入库单';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({
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
        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var GoodsModel = $('#GoodsModel').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#datagrid').myDatagrid('search', { ContrNo: ContrNo, EntryId: EntryId, OwnerName: OwnerName, GoodsModel: GoodsModel, StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#EntryId').textbox('setValue', null);
            $('#OwnerName').textbox('setValue', null);
            $('#GoodsModel').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }
        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
        //查看订单
        function View(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Entry.aspx?ID=' + id);

            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '入库单',
                width: '800px',
                height: '600px'
            });
        }
        function Export() {

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Export()">导出</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />
                    <span class="lbl">报关号: </span>
                    <input class="easyui-textbox" id="EntryId" />
                    <span class="lbl">客户名称: </span>
                    <input class="easyui-textbox" id="OwnerName" />
                </li>
                <li>
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox" id="GoodsModel"/>
                    <span class="lbl">入库开始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">入库结束日期: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="入库单列表" data-options="singleSelect:false,fitColumns:true,fit:true,nowrap:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 40px">全选</th>
                    <th data-options="field:'ContrNO',align:'center'" style="width: 80px;">合同协议号</th>
                    <th data-options="field:'EntryId',align:'center'" style="width: 80px;">报关号</th>
                    <th data-options="field:'OwnerName',align:'center'" style="width: 80px;">客户名称</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 80px;">入库时间</th>
                    <th data-options="field:'TotalAmount',align:'center'" style="width: 80px;">总数量</th>
                    <th data-options="field:'DeclTotal',align:'center'" style="width: 80px;">总金额</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 80px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
