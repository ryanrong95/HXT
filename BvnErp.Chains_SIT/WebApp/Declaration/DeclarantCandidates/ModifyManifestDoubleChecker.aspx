<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModifyManifestDoubleChecker.aspx.cs" Inherits="WebApp.Declaration.DeclarantCandidates.ModifyManifestDoubleChecker" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调整报关单复核员</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            //订单列表初始化
             $('#datagrid').myDatagrid({
                fitColumns:true,border:false,fit:true,singleSelect:false,toolbar:'#topBar',
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

            $('#MyDecHead').change(function () {
                Search();
            });
        });

        //查询
        function Search() {
            var VoyageNo = $('#VoyageNo').textbox('getValue');
            var BillNo = $('#BillNo').textbox('getValue');            
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#datagrid').myDatagrid('search', { VoyageNo: VoyageNo, BillNo: BillNo,  StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {            
            $('#VoyageNo').textbox('setValue', null);
            $('#BillNo').textbox('setValue', null);          
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        function Operation(val, row, index) {
            var buttons = '';

            buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ModifyCustomSubmiter(\''
                + row.BillNo + '\',\'' + row.VoyageNo + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">修改复核员</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //单个修改复核人
        function ModifyCustomSubmiter(DecHeadID, OrderID) {
            var params = DecHeadID + "|" + OrderID;

            var url = location.pathname.replace(/ModifyManifestDoubleChecker.aspx/ig, 'ModifyManifestDoubleCheckerWindow.aspx') + "?Params=" + params;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改复核员',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //批量修改复核人
        function BatchModify() {
            var checkedItems = $('#datagrid').datagrid('getChecked');
            if (checkedItems == null || checkedItems.length <= 0) {
                $.messager.alert('提示','未选择报关单！','info');
                return;
            }

            var params = "";
            for (var i = 0; i < checkedItems.length; i++) {
                params = params + checkedItems[i].BillNo + "|" + checkedItems[i].VoyageNo + ",";
            }

            params = params.substring(0, params.length - 1);

            var url = location.pathname.replace(/ModifyManifestDoubleChecker.aspx/ig, 'ModifyManifestDoubleCheckerWindow.aspx') + "?Params=" + params;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改复核员',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div style="margin: 5px 0 0px 15px;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="BatchModify()">修改复核员</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">货物运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageNo" />
                    <%--<span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />--%>
                    <span class="lbl">提运单号: </span>
                    <input class="easyui-textbox" id="BillNo" />
                </li>
                <li>
                    <span class="lbl">录单起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">录单结束日期: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <%--                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox" id="Status" name="Status" data-options="valueField:'Value',textField:'Text'" />--%>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <table id="datagrid" title="调整舱单复核员" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false," toolbar="#topBar">
        <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'VoyageNo',align:'center'" style="width: 15%">货物运输批次号</th>
                    <th data-options="field:'BillNo',align:'center'" style="width: 12%">提运单号</th>
                    <th data-options="field:'Port',align:'center'" style="width: 9%">口岸</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 8%">件数</th>
                    <th data-options="field:'ConsigneeName',align:'center'" style="width: 25%">境内收货人</th>
                    <th data-options="field:'DoubleCheckerName',align:'center'" style="width: 8%">复核员</th>
                    <th data-options="field:'CreateTime',align:'center'" style="width: 8%">录单时间</th>                    
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%">操作</th>
                </tr>
            </thead>
    </table>
</body>
</html>
