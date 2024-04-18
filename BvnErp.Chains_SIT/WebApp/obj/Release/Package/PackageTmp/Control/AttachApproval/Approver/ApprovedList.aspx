<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovedList.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.ApprovedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-北京已审批列表</title>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script>
        //gvSettings.fatherMenu = '订单审批(北京)';
        //gvSettings.menu = '已审批';
        //gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        $(function () {
            //已审批列表初始化
            $('#ApprovedList').myDatagrid({
                actionName: 'ApprovedListData',
                fitColumns: true,
                fit: true,
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
            var orderID = $('#OrderID').textbox('getValue');

            $('#ApprovedList').myDatagrid({
                actionName: 'ApprovedListData',
                queryParams:{ 'OrderID': orderID },
                fitColumns: true,
                fit: true,
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
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="showApproveDetailWindow(\''
                + row.OrderControlStepID + '\',\'' + row.ControlTypeInt + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //显示审批结果
        function OrderControlStatusDesOperation(val, row, index) {
            var des = '';

            if (row.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Approved.GetHashCode()%>') {
                des = '<lable style="color: green;">' + row.OrderControlStatusDes + '</label>'
            } else if (row.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Rejected.GetHashCode()%>') {
                des = '<lable style="color: red;">' + row.OrderControlStatusDes + '</label>'
            } else {
                des = '<lable>' + row.OrderControlStatusDes + '</label>'
            }

            return des;
        }

        //显示审批详细信息窗口
        function showApproveDetailWindow(orderControlStepID, controlTypeInt) {
            var url = '';
            var width = 0;
            var height = 0;
            if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.GenerateBillApproval.GetHashCode()%>') {
                url = location.pathname.replace(/ApprovedList.aspx/ig, 'ApproveGenerateBill.aspx?OrderControlStepID=' + orderControlStepID);
                width = 1600;
                height = 650;
            } else if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.DeleteModelApproval.GetHashCode()%>') {
                url = location.pathname.replace(/ApprovedList.aspx/ig, 'ApproveDeleteModel.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Approver';
                width = 1600;
                height = 650;
            } else if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.ChangeQuantityApproval.GetHashCode()%>') {
                url = location.pathname.replace(/ApprovedList.aspx/ig, 'ApproveChangeQuantity.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Approver';
                width = 1600;
                height = 650;
            } else if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.SplitOrderApproval.GetHashCode()%>') {
                url = location.pathname.replace(/ApprovedList.aspx/ig, 'ApproveSplitOrder.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Approver';
                width = 1600;
                height = 650;
            }
            
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '已审批',
                width: width,
                height: height,
                onClose: function () {
                    Search();
                }
            });
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

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="ApprovedList" title="已审批列表" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'ShowOrderID',align:'left'" style="width: 12%;">订单编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 18%;">客户名称</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 8%;">币种</th>
                    <th data-options="field:'DeclarePrice',align:'left'" style="width: 10%;">报关总价</th>
                    <th data-options="field:'ControlTypeDes',align:'left'" style="width: 15%;">审批类型</th>
                    <th data-options="field:'ApplicantName',align:'center'" style="width: 8%;">申请人</th>
                    <th data-options="field:'OrderControlStatusDes',align:'center',formatter:OrderControlStatusDesOperation" style="width: 8%;">审批结果</th>
                    <th data-options="field:'ApproveAdminName',align:'center'" style="width: 8%;">审批人</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
