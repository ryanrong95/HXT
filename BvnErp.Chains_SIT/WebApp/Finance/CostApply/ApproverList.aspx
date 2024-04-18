<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproverList.aspx.cs" Inherits="WebApp.Finance.CostApply.ApproverList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>费用申请-待审批(经理)</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
 <%--   <script>
        gvSettings.fatherMenu = '费用申请';
        gvSettings.menu = '待审批（经理）';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            var feeType = eval('(<%=this.Model.FeeType%>)');
            $('#FeeType').combobox({
                data: feeType
            });

            var payers = eval('(<%=this.Model.Payers%>)');
            
            <%--var costStatus = eval('(<%=this.Model.CostStatus%>)');
            $('#CostStatus').combobox({
                data: costStatus
            });--%>

            $('#Payers').combobox({
                data: payers,
                editable: false,
                valueField: 'PayerID',
                textField: 'PayerName'
            });

            //费用申请-申请列表初始化
            $('#ApproverList').myDatagrid({
                actionName: 'ApproverListData',
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
            
            //批量审批通过/拒绝选中事件
            $("input[name='approve-result']").change(function() {
                var radioVal = $("input[name='approve-result']:checked").val();
                if(radioVal == '1') {
                    $('#Payers').combobox('textbox').validatebox('options').required = true;
                    $('#ApproveSummary').textbox('textbox').validatebox('options').required = false;
                    $('#Payers').combobox('enable');
                }
                if(radioVal == '2') {
                    $('#Payers').combobox('textbox').validatebox('options').required = false;
                    $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;
                    $('#Payers').combobox('setValue', null);
                    $('#Payers').combobox('disable');

                }
            });
        });

        //查询
        function Search() {
            var PayeeName = $('#PayeeName').textbox('getValue');
            var FeeType = $('#FeeType').combobox('getValue');
            //var CostStatus = $('#CostStatus').combobox('getValue');
            var CreateDateBegin = $('#CreateDateBegin').datebox('getValue');
            var CreateDateEnd = $('#CreateDateEnd').datebox('getValue');

            var parm = {
                PayeeName: PayeeName,
                FeeType: FeeType,
                //CostStatus: CostStatus,
                CreateDateBegin: CreateDateBegin,
                CreateDateEnd: CreateDateEnd,
            };

            //费用申请-申请列表初始化
            $('#ApproverList').myDatagrid({
                actionName: 'ApproverListData',
                queryParams:parm,
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
            $('#FeeType').combobox('setValue', null);
            //$('#CostStatus').combobox('setValue', null);
            $('#CreateDateBegin').datebox('setValue', null);
            $('#CreateDateEnd').datebox('setValue', null);
            Search();
        }

        //查看费用申请详情
        function View(costApplyID) {
            var url = location.pathname.replace(/ApproverList.aspx/ig, './View.aspx')
                + '?CostApplyID=' + costApplyID
                + '&From=Approver';

            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 1000,
                height: 580,
                onClose: function () {
                    Search();
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';
            if (row.CostStatusInt == '<%=Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove.GetHashCode()%>') {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\''
                    + row.CostApplyID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">审批</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\''
                    + row.CostApplyID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }


            return buttons;
        }

        //批量审批
        function ShowBatchApproveWindow() {
            var docData = $('#ApproverList').datagrid('getChecked');
            var arr = new Array();
            for (var i = 0; i < docData.length; i++) {
                arr[i] = docData[i].CostApplyID;
            }

            if (arr.length == 0) {
                $.messager.alert('提示', '请至少选择一个要审批的项！');
                return;
            }
            var jsonString = JSON.stringify(arr);            


            $("#approve-result-pass").removeAttr("checked");
            $("#approve-result-reject").removeAttr("checked");
            $('#Payers').combobox('setValue', null);
            $('#Payers').combobox('enable');

            $('#Payers').combobox('textbox').validatebox('options').required = false;
            $('#ApproveSummary').textbox('textbox').validatebox('options').required = false;

            $('#batch-approve-dialog').dialog({
                title: '批量审批',
                width: 380,
                height: 240,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        //检查是否选中radio
                        var radioVal = $("input[name='approve-result']:checked").val();
                        if (radioVal == 'undefined' || (radioVal != 1 && radioVal != 2)) {
                            $.messager.alert('提示', '请选择同意/拒绝！');
                            return;
                        }

                        var payer = $("#Payers").combobox('getValue');
                        var approveSummary = $("#ApproveSummary").textbox('getValue').trim();
                        
                        if(radioVal == '1') {
                            if(payer.length <= 0) {
                                $.messager.alert('提示', '请选择财务！');
                                return;
                            }
                        }
                        if(radioVal == '2') {
                            if(approveSummary.length <= 0) {
                                $.messager.alert('提示', '请输入备注！');
                                return;
                            }
                        }    

                        $.messager.confirm('确认', '确定要' + (radioVal == 1 ? '通过':'拒绝') + '这些申请？', function(r){
                            if (r){
                                MaskUtil.mask();
                                $("div[class*=window-mask]").css('z-index', '9005');
                                $.post(location.pathname + '?action=BatchApprove', {
                                    CostApplyIDs: jsonString,
                                    RadioVal: radioVal,
                                    Payer: payer,
                                    ApproveSummary: approveSummary,
                                }, function (res) {
                                    MaskUtil.unmask();
                                    var result = JSON.parse(res);
                                    if (result.success) {
                                        var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                            NormalClose();

                                        });
                                        alert1.window({
                                            modal: true, onBeforeClose: function () {
                                                NormalClose();
                                            }
                                        });
                                    } else {
                                        $.messager.alert('提示', result.message, 'info', function () {

                                        });
                                    }
                                });
                            }
                        });
                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#batch-approve-dialog').window('close');
                    }
                }],
            });

            $('#batch-approve-dialog').window('center');

        }

        //整行关闭一系列弹框
        function NormalClose() {
            $('#batch-approve-dialog').window('close');
            $('#ApproverList').datagrid('reload');
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <a id="btnBatchApprove" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" style="margin-left: 10px;" 
                        onclick="ShowBatchApproveWindow()">批量审批</a>
                </li>
            </ul>
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 22px;">收款方：</span>
                    <input class="easyui-textbox" id="PayeeName" data-options="width:160,validType:'length[1,50]'" />
                    <span class="lbl">费用类型：</span>
                    <input class="easyui-combobox" id="FeeType" data-options="width:160,valueField:'Key',textField:'Value',editable:false," />
                    <%--<span class="lbl">状态：</span>
                    <input class="easyui-combobox" id="CostStatus" data-options="valueField:'Key',textField:'Value',editable:false," />--%>
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
        <table id="ApproverList" title="费用申请(待审批)" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'CostApplyID',align:'left'" style="width: 18%;">申请编号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 18%;">收款方</th>
                    <th data-options="field:'CostType',align:'left'" style="width: 8%;">费用类型</th>
                    <th data-options="field:'FeeTypeName',align:'left'" style="width: 8%;">费用名称</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 5%;">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 8%;">币种</th>
                    <th data-options="field:'CostStatus',align:'left'" style="width: 10%;">状态</th>
                    <th data-options="field:'PayTime',align:'center'" style="width: 10%;">付款日期</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">申请日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="batch-approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form>
            <div style="margin-left: 25px; margin-top: 15px;">
                <span>
                    <input id="approve-result-pass" name="approve-result" type="radio" value="1" /><label for="approve-result-pass">通过</label>
                </span>
                <span style="margin-left: 10px;">
                    <input id="approve-result-reject" name="approve-result" type="radio" value="2" /><label for="approve-result-reject">拒绝</label>
                </span>
            </div>
            <div style="margin-left: 15px; margin-top: 15px;">      
                <form id="form1" runat="server">
                    <div class="sub-container">
                        <div id="payer-area" style="display: block;">
                            <table>
                                <tr>
                                    <td style="width: 45px;"><span>财务：</span></td>
                                    <td>
                                        <input class="easyui-combobox" id="Payers" data-options="valueField:'Key',textField:'Value',editable:false,width:200," />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="margin-top: 5px;">
                            <table>
                                <tr>
                                    <td style="width: 45px; vertical-align: top;"><span>备注：</span></td>
                                    <td>
                                        <input class="easyui-textbox" id="ApproveSummary" data-options="multiline:true,validType:'length[1,100]',tipPosition:'top'," style="width: 260px; height: 60px">
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </form>
            </div>
        </form>
    </div>

</body>
</html>
