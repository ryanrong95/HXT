<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadedTaxFlow.aspx.cs" Inherits="WebApp.Declaration.Declare.UploadedTaxFlow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已上传缴税流水</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
 <%--   <script>
        gvSettings.fatherMenu = '报关单(XTD)';
        gvSettings.menu = '已上传缴税流水';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            $('#uploadedTaxFlow-table').myDatagrid({
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
                pageSize: 20,
            });
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var StrDDateBegin = $('#StrDDateBegin').textbox('getValue');
            var StrDDateEnd = $('#StrDDateEnd').textbox('getValue');

            $('#uploadedTaxFlow-table').myDatagrid('search', {
                ContrNo: ContrNo,
                EntryId: EntryId,
                OrderID: OrderID,
                OwnerName: OwnerName,
                StrDDateBegin: StrDDateBegin,
                StrDDateEnd: StrDDateEnd,
            });
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#EntryId').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#OwnerName').textbox('setValue', null);
            $('#StrDDateBegin').textbox('setValue', null);
            $('#StrDDateEnd').textbox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1" runat="server">
                <ul>
                    <li>
                        <span class="lbl" style="margin-left: 22px;">合同号: </span>
                        <input class="easyui-textbox" id="ContrNo" data-options="validType:'length[1,50]'" />
                        <span class="lbl">海关编号: </span>
                        <input class="easyui-textbox" id="EntryId" data-options="validType:'length[1,50]'" />
                        <span class="lbl">订单编号: </span>
                        <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    </li>
                    <li>
                        <span class="lbl">公司名称: </span>
                        <input class="easyui-textbox" id="OwnerName" data-options="validType:'length[1,50]'" />
                        <span class="lbl">报关日期: </span>
                        <input class="easyui-datebox" data-options="" id="StrDDateBegin" />
                        <span class="lbl" style="margin-left: 48px;">至 </span>
                        <input class="easyui-datebox" data-options="" id="StrDDateEnd" />
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 10px;">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </li>
                </ul>
            </form>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="uploadedTaxFlow-table" title="已上传缴税流水" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar',
            border:false,">
            <thead>
                <tr>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 175px;">合同号</th>
                    <th data-options="field:'EntryId',align:'left'" style="width: 185px;">海关编号</th>
                    <th data-options="field:'DDate',align:'left'" style="width: 150px;">报关日期</th>
                    <th data-options="field:'TariffTaxNumber',align:'left'" style="width: 150px;">关税税费单号</th>
                    <th data-options="field:'TariffAmount',align:'left'" style="width:100px;">关税金额</th>
                    <th data-options="field:'ExciseTaxNumber',align:'left'" style="width: 150px;">消费税税费单号</th>
                    <th data-options="field:'ExciseTaxAmount',align:'left'" style="width:100px;">消费税金额</th>
                    <th data-options="field:'AddedValueTaxTaxNumber',align:'left'" style="width: 150px;">增值税税费单号</th>
                    <th data-options="field:'AddedValueTaxAmount',align:'left'" style="width: 100px;">增值税金额</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 150px;">订单编号</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 230px;">公司名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
