<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBSAccountFlow.aspx.cs" Inherits="WebApp.Finance.DBSAccountFlow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var StartDate = '<%=this.Model.StartDate%>';
        var EndDate = '<%=this.Model.EndDate%>';

        $(function () {            
            $('#StartDate').datebox("setValue", StartDate);            
            $('#EndDate').datebox("setValue", EndDate);

            $('#AccountNo').combobox('setValue', "30015588588");
            $('#drCrInd').combobox('setValue', "C");

            //账户流水列表初始化
            $('#AccountFlows').myDatagrid({
                queryParams: {
                    AccountNo: "30015588588",
                    drCrInd: "C",
                    StartDate: StartDate,
                    EndDate: EndDate,
                },
                fitColumns: true,
                fit: true,
                nowrap: false,
                toolbar: '#topBar',
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

        function Search() {
            var AccountNo = $('#AccountNo').combobox('getValue');
            var drCrInd = $('#drCrInd').combobox('getValue');           
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');

            var parm = {
                AccountNo: AccountNo,
                drCrInd: drCrInd,
                StartDate: startDate,
                EndDate: endDate,
            };
            $('#AccountFlows').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $("#AccountNo").combobox('setValue', "30015588588");
            $('#drCrInd').combobox('setValue', "C");
            $('#StartDate').datebox('setValue', StartDate);
            $('#EndDate').datebox('setValue', EndDate);

            Search();
        }       
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <form id="form1">
            <div id="search">
                <ul>
                    <li>
                        <span style="width: 70px; margin-left: 10px; display: inline-block">账户:</span>
                        <select class="easyui-combobox" id="AccountNo" name="AccountNo" data-options="width:255,editable:false">
                            <option value="" style="display: none"></option>
                            <option value="30015588588">人民币</option>
                            <option value="30015589288">美金</option>
                        </select>
                        <span style="width: 70px; margin-left: 10px; display: inline-block">收款/付款:</span>
                        <select class="easyui-combobox" id="drCrInd" name="drCrInd" data-options="width:255,editable:false">
                            <option value="" style="display: none"></option>
                            <option value="C">收款</option>
                            <option value="D">付款</option>
                        </select>
                        <br />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">发生日期:</span>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false,width:255" />
                        <span style="width: 70px; margin-left: 10px; display: inline-block">至 </span>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false,width:255" />

                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" style="margin-left: 10px;" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </li>
                </ul>
            </div>
        </form>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="AccountFlows" title="星展流水列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'AccountNo',align:'left'" style="width: 80px;">账号</th>
                    <th data-options="field:'initiatingPartyName',align:'left'" style="width: 80px;">对方户名</th>
                    <th data-options="field:'drCrInd',align:'center'" style="width: 50px;">收款/付款</th>
                    <th data-options="field:'txnAmount',align:'center'" style="width: 50px;">金额</th>
                    <th data-options="field:'txnCcy',align:'left'" style="width: 80px;">币制</th>
                    <th data-options="field:'Balance',align:'center'" style="width: 50px;">账户余额</th>
                    <th data-options="field:'txnDate',align:'center'" style="width: 50px;">发生日期</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
