<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PalletsList.aspx.cs" Inherits="WebApp.Declaration.Notice.PalletsList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预估卡板数</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%-- <script>       
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '承运商';
        gvSettings.summary = '仅限芯达通业务库房的物流使用的车辆管理'
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            var BaseWarehouse = eval('(<%=this.Model.Warehouse%>)');
            debugger
            $("#Warehouse").combobox({
                data: BaseWarehouse
            });

            setCurrentDate();
            var StartDateLoad = $('#StartDate').datebox('getValue');
            var EndDateLoad = $('#EndDate').datebox('getValue');

            $('#carriersGrid').myDatagrid({
                queryParams: { action: 'data', StartDate: StartDateLoad, EndDate: EndDateLoad },
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
            var StartDate = $('#StartDate').textbox('getValue');
            var EndDate = $('#EndDate').textbox('getValue');
            var Warehouse = $('#Warehouse').combobox('getValue');
            $('#carriersGrid').myDatagrid('search', { StartDate: StartDate, EndDate: EndDate, Warehouse: Warehouse });
        }

        //重置查询条件
        function Reset() {
            $('#StartDate').textbox('setValue', null);
            $('#EndDate').textbox('setValue', null);
            $('#Warehouse').combobox('setValue', null);
            Search();
        }

        function ConsigneeAddress(val, row, index) {
            var warehouse = "";
            var stock = row["Stock"];

            if (stock=="yisheng") {
              warehouse = "怡生";

            }
            else if(stock=="zhongmei"){                
                warehouse = "中美";
            }

            return warehouse;
        }

    </script>

    <script>
        function setCurrentDate() {
            var CurrentDate = getNowFormatDate();
            $("#StartDate").datebox("setValue", CurrentDate);
            $("#EndDate").datebox("setValue", CurrentDate);
        }

        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">库房: </span>
                    <input class="easyui-combobox" id="Warehouse" name="Warehouse"
                        data-options="valueField:'Value',textField:'Text'" style="width: 150px" />
                    <span class="lbl">申报起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至: </span>
                    <input class="easyui-datebox" id="EndDate" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="carriersGrid" data-options="singleSelect:true,fit:true,scrollbarSize:0" title="卡板数" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'Stock',align:'center',formatter:ConsigneeAddress" style="width: 80px;">库房</th>
                    <th data-options="field:'Pallet',align:'center'" style="width: 100px;">卡板预测</th>
                    <th data-options="field:'NoticeTime',align:'center'" style="width: 100px;">报关时间</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 80px;">创建时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
