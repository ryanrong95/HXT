<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnApprovedList.aspx.cs" Inherits="WebApp.Control.AttachApproval.Applicant.UnApprovedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-跟单员已审批列表</title>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script>
        //gvSettings.fatherMenu = '订单审批(跟单员)';
        //gvSettings.menu = '待审批';
        //gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        $(function () {
            //待审批列表初始化
            $('#UnApprovedList').myDatagrid({
                actionName: 'UnApprovedListData',
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

            //待审批列表初始化
            $('#UnApprovedList').myDatagrid({
                actionName: 'UnApprovedListData',
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
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="showGenDanUnproveDetailWindow(\''
                + row.OrderControlStepID + '\',\'' + row.ControlTypeInt + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //显示跟单员查看未审核详情弹框
        function showGenDanUnproveDetailWindow(orderControlStepID, controlTypeInt) {
            var url = '';
            var width = 0;
            var height = 0;
            if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.GenerateBillApproval.GetHashCode()%>') {
                url = location.pathname.replace(/UnApprovedList.aspx/ig, '../Approver/UnApproveGenerateBill.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Applicant';
                width = 1600;
                height = 650;
            } else if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.DeleteModelApproval.GetHashCode()%>') {
                url = location.pathname.replace(/UnApprovedList.aspx/ig, '../Approver/UnApproveDeleteModel.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Applicant';
                width = 1600;
                height = 650;
            } else if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.ChangeQuantityApproval.GetHashCode()%>') {
                url = location.pathname.replace(/UnApprovedList.aspx/ig, '../Approver/UnApproveChangeQuantity.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Applicant';
                width = 1600;
                height = 650;
            } else if (controlTypeInt == '<%=Needs.Ccs.Services.Enums.OrderControlType.SplitOrderApproval.GetHashCode()%>') {
                url = location.pathname.replace(/UnApprovedList.aspx/ig, '../Approver/UnApproveSplitOrder.aspx')
                    + '?OrderControlStepID=' + orderControlStepID
                    + '&From=Applicant';
                width = 1600;
                height = 650;
            }
            
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
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
        <table id="UnApprovedList" title="待审批列表" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'ShowOrderID',align:'left'" style="width: 12%;">订单编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 18%;">客户名称</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 8%;">币种</th>
                    <th data-options="field:'DeclarePrice',align:'left'" style="width: 10%;">报关总价</th>
                    <th data-options="field:'ControlTypeDes',align:'left'" style="width: 15%;">审批类型</th>
                    <th data-options="field:'ApplicantName',align:'left'" style="width: 8%;">申请人</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 10%;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
