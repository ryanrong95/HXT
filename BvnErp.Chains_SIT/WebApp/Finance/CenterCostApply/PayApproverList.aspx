<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayApproverList.aspx.cs" Inherits="WebApp.Finance.CenterCostApply.PayApproverList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>费用申请-付款申请</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
           <%-- var feeType = eval('(<%=this.Model.FeeType%>)');
            $('#FeeType').combobox({
                data: feeType
            });--%>
            var costStatus = eval('(<%=this.Model.CostStatus%>)');
            $('#CostStatus').combobox({
                data: costStatus
            });

            //费用申请-申请列表初始化
            $('#PayApproverList').myDatagrid({
                actionName: 'PayApproverListData',
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
            var PayeeName = $('#PayeeName').textbox('getValue');
            //var FeeType = $('#FeeType').combobox('getValue');
            var CostStatus = $('#CostStatus').combobox('getValue');
            var CreateDateBegin = $('#CreateDateBegin').datebox('getValue');
            var CreateDateEnd = $('#CreateDateEnd').datebox('getValue');

            var parm = {
                PayeeName: PayeeName,
                //FeeType: FeeType,
                CostStatus: CostStatus,
                CreateDateBegin: CreateDateBegin,
                CreateDateEnd: CreateDateEnd,
            };

            //费用申请-申请列表初始化
            $('#PayApproverList').myDatagrid({
                actionName: 'PayApproverListData',
                queryParams: parm,
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
            $('#PayeeName').textbox('setValue', null);
            //$('#FeeType').combobox('setValue', null);
            $('#CostStatus').combobox('setValue', null);
            $('#CreateDateBegin').datebox('setValue', null);
            $('#CreateDateEnd').datebox('setValue', null);
            Search();
        }

        //查看费用申请详情
        function View(costApplyID) {
            var url = location.pathname.replace(/PayApproverList.aspx/ig, 'View.aspx')
                + '?CostApplyID=' + costApplyID
                + '&From=Approvered';

            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 1200,
                height: 600,
                onClose: function () {
                    Search();
                }
            });
        }

        //编辑是否收到纸质票据
        function Edit(CostApplyID) {
            var url = location.pathname.replace(/PayApproverList.aspx/ig, '../CostApply/PayEditView.aspx') + '?CostApplyID=' + CostApplyID;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '编辑是否收到纸质票据',
                width: '400',
                height: '180',
                url: url,
                onClose: function () {
                    $('#PayApproverList').myDatagrid('reload');
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\''
                + row.CostApplyID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            if (row.PaperNotesStatusInt == '<%=Needs.Ccs.Services.Enums.CheckPaperNotesEnum.UnPaperNotes.GetHashCode()%>') {
                buttons += '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.CostApplyID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">修改</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            }


            return buttons;
        }

        function CostStatusOperation(val, row, index) {
            var des = '';
            if (row.CostStatusInt == '<%=Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit.GetHashCode()%>') {
                des = '<span style="color: red;">' + row.CostStatus + '</span>';
            } else {
                des = '<span>' + row.CostStatus + '</span>';
            }

            return des;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul style="margin-top: 5px;">
                <li>
                    <span class="lbl" style="margin-left: 22px;">收款方：</span>
                    <input class="easyui-textbox" id="PayeeName" data-options="width:160,validType:'length[1,50]'" />
                    <%--<span class="lbl">费用类型：</span>
                    <input class="easyui-combobox" id="FeeType" data-options="width:160,valueField:'Key',textField:'Value',editable:false," />--%>
                    <span class="lbl">状态：</span>
                    <input class="easyui-combobox" id="CostStatus" data-options="width:160,valueField:'Key',textField:'Value',editable:false," />
                </li>
            </ul>
            <ul>
                <li>
                    <span class="lbl">申请日期：</span>
                    <input class="easyui-datebox" id="CreateDateBegin" data-options="width:160,validType:'length[1,50]'" />
                    <span class="lbl">至</span>
                    <input class="easyui-datebox" id="CreateDateEnd" data-options="width:160,validType:'length[1,50]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="PayApproverList" title="付款申请" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'CostApplyID',align:'left'" style="width: 18%;">申请编号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 18%;">收款方</th>
                    <th data-options="field:'CostType',align:'left'" style="width: 8%;">费用类型</th>
                    <th data-options="field:'FeeTypeName',align:'left'" style="width: 8%;">费用名称</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 5%;">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'CostStatus',align:'left',formatter:CostStatusOperation" style="width: 10%;">状态</th>
                    <th data-options="field:'PayTime',align:'center'" style="width: 10%;">付款日期</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">申请日期</th>
                    <th data-options="field:'PaperNotesStatus',align:'center'" style="width: 10%;">是否收到纸质票据</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
