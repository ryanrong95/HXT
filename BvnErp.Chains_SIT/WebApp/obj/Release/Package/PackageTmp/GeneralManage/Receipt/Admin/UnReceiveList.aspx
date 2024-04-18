<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnReceiveList.aspx.cs" Inherits="WebApp.GeneralManage.Receipt.Admin.UnReceiveList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待收款记录查询</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '综合管理(XDT)';
        gvSettings.menu = '未收款';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //业务员下拉框初始化

            var SalesmanData = eval('(<%=this.Model.Salesman%>)');
            var PeriodType = eval('(<%=this.Model.PeriodType%>)');

            //
            var typeAll = {
                Value: "",
                Text:"全部"
            };
            PeriodType.push(typeAll);

            $('#Salesman').combobox({
                data: SalesmanData,
            });

            $('#PeriodType').combobox({
                data: PeriodType,
                onSelect: function (record) {
                    if (record.Value == '<%=Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode()%>') {
                        $('#SettlementDate').textbox('setValue', null);
                        $("#SettlementDate").textbox('textbox').attr('readonly', true);
                    }
                    else {
                        $("#SettlementDate").textbox('textbox').attr('readonly', false);
                    }
                },
            });

            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            
            //列表初始化
            $('#datagrid').myDatagrid({
                queryParams: { action: 'data', StartDate: dateMonth(0), EndDate: dateMonth(1) },
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    //欠款合计
                    $('#Money').text(data.sumOverdraft);
                    return data;
                }
            });

        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + index + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function FmtExceedDay(val, row, index) {
           var color = ""
            if (row.ExceedDays < -10) {
                //绿色
                color = "limegreen";
            } else if (row.ExceedDays < 0) {
                //橙色
                color = "orange";
            } else {
                //逾期红色
                color = "red";
            }

            var str = '<label style="color:' + color +'">' + row.ExceedDays +'</label>';
            return str;
        }

        //查看
        function View(index) {

            $('#datagrid').datagrid('selectRow', index);
            var row = $('#datagrid').datagrid('getSelected');
            if (row) {
                var status = '';
                //需判断是未付款/部分付款
                if (row.Received == 0) {
                    status = '<%=Needs.Ccs.Services.Enums.OrderReceivedStatus.UnReceive.GetHashCode()%>';
                }
                else {
                    status = '<%=Needs.Ccs.Services.Enums.OrderReceivedStatus.PartReceived.GetHashCode()%>';
                }
                var url = location.pathname.replace(/UnReceiveList.aspx/ig, '../NewDetail.aspx?ID=' + row.OrderId + '&Status=' + status);
                top.$.myWindow({
                    iconCls: "icon-search",
                    url: url,
                    noheader: false,
                    title: '查看订单收款',
                    width: 1100,
                    height: 600,
                    onClose: function () {
                        $('#datagrid').datagrid('reload');
                    }
                });
            }
        }

        //导出欠款Excel
        function ExportExcel() {
            var clientCode = $('#ClientCode').textbox('getValue');
            var orderId = $('#OrderId').textbox('getValue');
            var salesman = $('#Salesman').combobox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var settlementDate = $('#SettlementDate').textbox('getValue');
            var periodType = $('#PeriodType').combobox('getValue');
            var parm = {
                ClientCode: clientCode,
                OrderId: orderId,
                Salesman: salesman,
                SettlementDate: settlementDate,
                PeriodType: periodType,
                StartDate: startDate,
                EndDate: endDate
            };

            MaskUtil.mask();
            $.post('?action=ExportExcel', parm, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                    let a = document.createElement('a');
                    document.body.appendChild(a);
                    a.href = result.url;
                    a.download = "";
                    a.click();
                } else {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                }
            })
        }

        //查询
        function Search() {
            var clientCode = $('#ClientCode').textbox('getValue');
            var orderId = $('#OrderId').textbox('getValue');
            var settlementDate = $('#SettlementDate').textbox('getValue');
            var salesman = $('#Salesman').combobox('getValue');
            var periodType = $('#PeriodType').combobox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {
                ClientCode: clientCode,
                OrderId: orderId,
                SettlementDate: settlementDate,
                Salesman: salesman,
                PeriodType: periodType,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#OrderId').textbox('setValue', null);
            $('#SettlementDate').textbox('setValue', null);
            $('Salesman').combobox('setValue', null);
            $('#PeriodType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            Search();
        }

        function dateMonth(number) {
            var nowdays = new Date();
            var year = nowdays.getFullYear();
            var month = nowdays.getMonth();
            if (month == 0) {
                month = 12;
                year = year - 1;
            }
            if (month < 10) {
                month = "0" + month;
            }
            if (number == 0) {
                return year + "-" + month + "-" + "01";
            }
            else {
                var myDate = new Date(year, month, 0);
                return year + "-" + month + "-" + myDate.getDate();
            }
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl" style="margin-left: 10px;">订单编号:</td>
                    <td>
                        <input class="easyui-textbox" id="OrderId" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">业务员:</td>
                    <td>
                        <input class="easyui-combobox" id="Salesman" data-options="valueField:'Value',textField:'Text',editable:false,height:26,width:200" />
                    </td>
                    <td class="lbl">结算方式:</td>
                    <td>
                        <input class="easyui-combobox" id="PeriodType" data-options="valueField:'Value',textField:'Text',editable:false,height:26,width:200" />
                    </td>
                    <td class="lbl">结算日:</td>
                    <td>
                         <input class="easyui-textbox" id="SettlementDate" data-options="height:26,width:200" />
                    </td>
                  </tr>
                    <tr>
                        <td class="lbl">报关日期:</td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                        </td>
                        <td class="lbl">至</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                        </td>
                        <td></td>
                        <td>
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                             <a href="javascript:void(0);" class="easyui-linkbutton" id="export" data-options="iconCls:'icon-save'" onclick="ExportExcel()">导出</a>
                        </td>
                    </tr>
                  <tr>
                    <td class="lbl">欠款合计</td>
                    <td>
                        <label id="Money"></label>
                    </td>
                   
                </tr>
            </table>
        </div>
    </div>

       <div id="data" data-options="region:'center',border:false">
           <table id="datagrid" title="待收款" data-options="
            nowrap:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:false,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 4%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 13%;">客户名称</th>
                    <th data-options="field:'OrderId',align:'left'" style="width: 8%;">订单编号</th>
                    <th data-options="field:'OrderStatus',align:'left'" style="width: 6%;">订单状态</th>
                    <th data-options="field:'Salesman',align:'center'" style="width: 6%;">业务员</th>
                    <th data-options="field:'Merchandiser',align:'center'" style="width: 6%;">客服</th>
                    <th data-options="field:'DeclareDate',align:'center'" style="width: 6%;">报关日期</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 6%;">报关货值</th>
                    <th data-options="field:'Receivable',align:'center'" style="width: 6%;">应收款</th>
                    <th data-options="field:'Received',align:'center'" style="width:5%;">已收款</th>
                    <th data-options="field:'Overdraft',align:'center',sortable:true" style="width: 7%;">欠款</th>
                    <th data-options="field:'PeriodTypeDesc',align:'center'" style="width:6%;">结算方式</th>
                    <th data-options="field:'SettlementDate',align:'center'" style="width:5%;">结算日</th>
                    <th data-options="field:'ExceedDays',align:'center',formatter:FmtExceedDay" style="width:5%;">逾期天数</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

</body>
</html>


