<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VoyageList.aspx.cs" Inherits="WebApp.HKWarehouse.Exit.VoyageList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '出库通知(HK)';
        gvSettings.menu = '待出库';
        gvSettings.summary = '按照运输批次出库';
    </script>--%>
    <script>
        $(function () {
            //下拉框数据初始化
            var carriers = eval('(<%=this.Model.Carriers%>)');
            $('#Carrier').combobox({
                data: carriers
            });

            //运输批次列表初始化
            $('#voyageGrid').myDatagrid({
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
            var voyageNo = $("#VoyageNo").textbox('getValue');
            var carrier = $("#Carrier").combobox('getValue');
            $('#voyageGrid').myDatagrid('search', { VoyageNo: voyageNo, Carrier: carrier });
        }

        //重置
        function Reset() {
            $("#VoyageNo").textbox('setValue', "");
            $("#Carrier").combobox('setValue', "");
            Search();
        }

        //查看
        function View(id) {
            var url = location.pathname.replace(/VoyageList.aspx/ig, '../../Logistics/ManifestVoyage/Detail.aspx') + '?ID=' + id + '&From=HKWarehouse';
            window.location = url;
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                  '<span class =\'l-btn-left l-btn-icon-left\'>' +
                  '<span class="l-btn-text">详情</span>' +
                  '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';

            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1" style="margin: 5px 0">
                <tr>
                    <td class="lbl">货物运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" id="VoyageNo" data-options="width:200" />
                    </td>
                    <td class="lbl">承运商：</td>
                    <td>
                        <input class="easyui-combobox" id="Carrier" data-options="width:200,valueField:'Code',textField:'Name',editable:false" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="voyageGrid" class="easyui-datagrid" title="运输批次" data-options="
            nowrap:false,
            border: false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="VoyageNo" data-options="align:'center'" style="width: 50px">货物运输批次号</th>
                    <th field="Carrier" data-options="align:'left'" style="width: 50px">承运商</th>
                    <th field="HKLicense" data-options="align:'center'" style="width: 50px">车牌号</th>
                    <th field="TransportTime" data-options="align:'center'" style="width: 50px">运输时间</th>
                    <th field="DriverName" data-options="align:'center'" style="width: 50px">驾驶员姓名</th>
                    <th field="VoyageType" data-options="align:'center'" style="width: 20px">运输类型</th>
                    <th field="CutStatus" data-options="align:'center'" style="width: 20px">截单状态</th>
                    <th field="CreateTime" data-options="align:'center'" style="width: 50px">创建日期</th>
                    <th data-options="field:'btnOpt',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
