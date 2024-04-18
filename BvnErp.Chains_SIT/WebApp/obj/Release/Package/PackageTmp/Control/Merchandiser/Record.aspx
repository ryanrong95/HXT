<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Record.aspx.cs" Inherits="WebApp.Control.Merchandiser.Record" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>跟单员管控审批记录</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '管控审批(跟单员)';
        gvSettings.menu = '审批记录';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //待审批列表初始化
            $('#records').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        if (row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.CCC.GetHashCode()%>' ||
                            row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.OriginCertificate.GetHashCode()%>') {
                            row["AuditResult"] = '资料已上传';
                        }
                        if (row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.Forbid.GetHashCode()%>' ||
                            row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.ClassifyAnomaly.GetHashCode()%>' ||
                            row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.CheckingAbnomaly.GetHashCode()%>' ||
                            row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.SortingAbnomaly.GetHashCode()%>') {
                            row["AuditResult"] = '订单退回';
                        }
                        if (row["ControlTypeValue"] == '<%=Needs.Ccs.Services.Enums.OrderControlType.OriginChange.GetHashCode()%>') {
                            if (row["ControlStatus"] == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Approved.GetHashCode()%>') {
                                row["AuditResult"] = '取消挂起';
                            } else if (row["ControlStatus"] == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Rejected.GetHashCode()%>') {
                                row["AuditResult"] = '订单退回';
                            }
                        } 
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            $('#records').myDatagrid('search', { OrderID: orderID, ClientCode: clientCode });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="records" title="审批记录" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 13%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 13%;">客户名称</th>
                    <th data-options="field:'Name',align:'left'" style="width: 10%;">报关品名</th>
                    <th data-options="field:'Model',align:'center'" style="width: 6%;">型号</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 5%;">品牌</th>
                    <th data-options="field:'HSCode',align:'center'" style="width: 6%;">商品编码</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 5%;">产地</th>
                    <th data-options="field:'ControlType',align:'center'" style="width: 6%;">管控类型</th>
                    <th data-options="field:'AuditResult',align:'center'" style="width: 6%;">审批结果</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 5%;">审批人</th>
                    <th data-options="field:'AuditDate',align:'center'" style="width: 10%;">审批时间</th>
                    <th data-options="field:'ApproveSummary',align:'center'" style="width: 10%;">备注</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
