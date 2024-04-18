<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.StockIn.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>财务入库界面</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
</head>
<script type="text/javascript">
    $(function () {
        var decTaxStatus = eval('(<%=this.Model.DecTaxStatusData%>)');
        $('#DecTaxStatus').combobox({
            data: decTaxStatus,
            valueField: 'Key',
            textField: 'Value',
        })

        $('#datagrid').myDatagrid({
            fitColumns: true, fit: true, toolbar: '#topBar', nowrap: false, rownumbers: true,
            singleSelect: false,
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

    function btnbCheckPush() {
        var rows = $('#datagrid').datagrid('getChecked');
        if (rows.length < 1) {
            $.messager.alert('消息', '请先勾选要推送的数据！', 'info');
            return;
        }
        var ids = [];
        $.each(rows, function (index, value) {
            ids.push(value.ID)
        });
        var paramas = { IDs: ids.join() };
        var messageTotal = "";
        //统计勾选条数与报关金额 start
        $.post('?action=CheckAmount', paramas, function (res) {
            var result = JSON.parse(res);
            if (result.success) {
                messageTotal = '确定推送所勾选的数据吗？<br>' + '勾选条数：' + result.count + '<br>报关金额：' + result.TotalAmount;

                $.messager.confirm('确认', messageTotal, function (r) {
                    if (r) {
                        $.post('?action=BatchCheckPush', paramas, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                $.messager.alert('提示', '提交成功', 'info', function () {
                                    $('#datagrid').datagrid('reload');
                                });
                            } else {
                                $.messager.alert('错误', result.message, 'error');
                                return;
                            }

                        });
                    }
                });

            } else {
                messageTotal = "未能正确统计报关金额：" + result.message;
                $.messager.alert('错误', messageTotal, 'error');
                return;
            }
        });

        //统计勾选条数与报关金额 end

        
    };
    function btnQueryPush() {

        // var rows = $('#datagrid').datagrid('getRows');
        var ContrNo = $('#ContrNo').textbox('getValue');
        var OrderID = $('#OrderID').textbox('getValue');
        var EntryID = $('#EntryID').textbox('getValue');
        var DecTaxStatus = $('#DecTaxStatus').combobox('getValue');
        var StartDate = $('#StartDate').datebox('getValue');
        var EndDate = $('#EndDate').datebox('getValue');
        var ClientName = $('#ClientName').textbox('getValue');
        var paramas = {
            ContrNo: ContrNo,
            OrderID: OrderID,
            EntryID: EntryID,
            DecTaxStatus: DecTaxStatus,
            StartDate: StartDate,
            EndDate: EndDate,
            ClientName: ClientName,
        };
        //  var flag = ContrNo == "" && OrderID == "" && EntryID == "" && DecTaxStatus == "" && StartDate == "" && EndDate && ClientName == "";
        $.messager.confirm('确认', '确定推送当前查询出来的数据吗？', function (r) {
            if (r) {
                $.post('?action=BatchQueryPush', paramas, function (res) {
                    var result = JSON.parse(res);
                    if (result.success) {
                        $.messager.alert('提示', '提交成功', 'info', function () {
                            $('#datagrid').datagrid('reload');
                        });
                    } else {
                        $.messager.alert('错误', result.message, 'error');
                        return;
                    }

                });
            }
        });
    };


    //查询
    function Search() {
        var ContrNo = $('#ContrNo').textbox('getValue');
        var OrderID = $('#OrderID').textbox('getValue');
        var EntryID = $('#EntryID').textbox('getValue');
        var DecTaxStatus = $('#DecTaxStatus').combobox('getValue');
        var StartDate = $('#StartDate').datebox('getValue');
        var EndDate = $('#EndDate').datebox('getValue');
        var ClientName = $('#ClientName').textbox('getValue');
        var parm = {
            ContrNo: ContrNo,
            OrderID: OrderID,
            EntryID: EntryID,
            DecTaxStatus: DecTaxStatus,
            StartDate: StartDate,
            EndDate: EndDate,
            ClientName: ClientName,
        };
        $('#datagrid').myDatagrid('search', parm);
    }
    //重置
    function Reset() {
        $('#ContrNo').textbox('setValue', null);
        $('#OrderID').textbox('setValue', null);
        $('#EntryID').textbox('setValue', null);
        $('#InvoiceType').combobox('setValue', null);
        $('#DecTaxStatus').combobox('setValue', null);
        $('#StartDate').datebox('setValue', null);
        $('#EndDate').datebox('setValue', null);
        $('#ClientName').textbox('setValue', null);
        Search();
    }
</script>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnbCheckPush" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="btnbCheckPush()">批量勾选推送</a>
            <a id="btnQueryPush" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="btnQueryPush()">查询条件筛选推送</a>
        </div>
        <div id="search">

            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">合同号:</td>
                    <td>
                        <input class="easyui-textbox" id="ContrNo" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">报关单号:</td>
                    <td>
                        <input class="easyui-textbox" id="EntryID" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">缴税状态:</td>
                    <td>
                        <input class="easyui-combobox" id="DecTaxStatus" name="DecTaxStatus"
                            data-options="width:200,height:26 ,editable:false" />
                    </td>
                    <td class="lbl">申报日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" />
                    </td>
                    <td class="lbl" style="padding-left: 10px">至：</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" />
                    </td>
                </tr>

                <tr>
                    <td class="lbl">订单号:</td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">客户名称:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:200" />
                    </td>
                    <td></td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>

        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="财务入库列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true, singleSelect: false,">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNo',align:'center'" style="width: 100px;">合同号</th>
                    <th data-options="field:'EntryId',align:'center'" style="width: 100px;">报关单号</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 100px;">申报日期</th>
                    <th data-options="field:'DecTaxStatus',align:'center'" style="width: 100px;">缴税状态</th>
                    <th data-options="field:'ClientName',align:'center'" style="width: 100px;">客户名称</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单号</th>


                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
