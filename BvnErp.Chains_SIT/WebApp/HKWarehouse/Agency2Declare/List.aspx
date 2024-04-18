<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.HKWarehouse.Agency2Declare.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script>
        var SelectedInfo = [];
        $(function () {
            $('#datagrid').myDatagrid({
                checkOnSelect: false,
                singleSelect: false,
                fitColumns: true,
                fit: true,
                toolbar: '#topBar',
                nowrap: false,
                rownumbers: true,
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
                onLoadSuccess: function (data) {
                    if (data.rows.length > 0) {
                        for (var i = 0; i < data.rows.length; i++) {
                            var index = findIndex(SelectedInfo, data.rows[i]["ID"]);
                            if (index > -1) {
                                $("input[type='checkbox']")[i + 1].checked = true;
                            }
                        }
                    }
                },
                onCheck: function (index, row) {
                    SelectedInfo.push({
                        ID: row["ID"],
                        PartNumber: row["PartNumber"],
                        Quantity: row["Quantity"],
                        SaleQuantity: row["Quantity"],
                        ClientID: row["ClientID"],
                        Currency:row["Currency"],
                    });
                },
                onUncheck: function (index, row) {
                    var index = findIndex(SelectedInfo, row["ID"]);
                    if (index > -1) {
                        SelectedInfo.splice(index, 1);
                    }
                },
                onCheckAll: function (rows) {                   
                    if (rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            var row = rows[i];
                            SelectedInfo.push({
                                ID: row["ID"],
                                PartNumber: row["PartNumber"],
                                Quantity: row["Quantity"],
                                SaleQuantity: row["Quantity"],
                                ClientID: row["ClientID"],
                                Currency:row["Currency"],
                            });
                        }
                    }
                },
                onUncheckAll: function (rows) {                   
                    if (rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            var index = findIndex(SelectedInfo, rows[i]["ID"]);
                            if (index > -1) {
                                SelectedInfo.splice(index, 1);
                            }
                        }
                    }
                },
            });
        });

        function findIndex(select, id) {
            var index = -1;
            for (i = 0; i < select.length; i++) {
                if (select[i]["ID"] == id) {
                    index = i;
                }
            }
            return index;
        }

        //查询
        function Search() {
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var ID = $('#ID').textbox('getValue');
            var PartNumber = $('#PartNumber').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var parm = {
                StartDate: StartDate,
                EndDate: EndDate,
                ID: ID,
                PartNumber: PartNumber,
                ClientName:ClientName
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#ID').textbox('setValue', null);
            $('#PartNumber').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            Search();
        }

        function A2Declare() {
            if (SelectedInfo.length.length < 1) {
                $.messager.alert('提示', "请选择库存");
                return;
            }
           
            var firstClientID = SelectedInfo[0].ClientID;
            var firstCurrecny = SelectedInfo[0].Currency;
            var cart = "";
            for (var i = 0; i < SelectedInfo.length; i++) {
                if(firstClientID != SelectedInfo[i].ClientID){
                     $.messager.alert('提示', "请选择同一客户的库存!");
                     return;
                }
                if (firstCurrecny != SelectedInfo[i].Currency) {
                     $.messager.alert('提示', "请选择币种相同的库存!");
                     return;
                }
                cart += SelectedInfo[i].ID + ",";
            }
          
            var url = location.pathname.replace(/List.aspx/ig, './ShoppingCart.aspx') + '?WindowName=StockData&ClientID='+firstClientID+'&CartInfo=' + cart ;

            $.myWindow.setMyWindow("StockData", window);

            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '购物车',
                width: 800,
                height: 500,
                closable: false,
                onClose: function () {

                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">入库时间:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" />
                    </td>
                    <td class="lbl">至:</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" />
                    </td>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox"  id="ClientName" data-options="height:26,width:200"/>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">入库单号:</td>
                    <td>
                        <input class="easyui-textbox" id="ID" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">型号:</td>
                    <td>
                        <input class="easyui-textbox" id="PartNumber" data-options="height:26,width:200" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="A2Declare()">转报关</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="库存列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 80px;">入库时间</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 80px;">公司名称</th>
                    <th data-options="field:'ID',align:'left'" style="width: 80px;">入库单号</th>
                    <th data-options="field:'PartNumber',align:'left'" style="width: 100px;">型号</th>
                    <th data-options="field:'Manufacturer',align:'left'" style="width: 100px;">品牌</th>
                    <th data-options="field:'Currency',align:'left'" style="width: 50px;">币种</th>
                    <th data-options="field:'UnitPrice',align:'left'" style="width: 50px;">单价</th>
                    <th data-options="field:'Quantity',align:'left'" style="width: 50px;">数量</th>
                    <th data-options="field:'TotalPrice',align:'left'" style="width: 50px;">金额</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
