<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Offtime.Details" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        var ID = getQueryString("ID");
        var AdminID = getQueryString("AdminID");
        var BusinessTrip = <%=Yahv.Underly.Enums.SchedulePrivateType.BusinessTrip.GetHashCode()%>;
        var OfficialBusiness = <%=Yahv.Underly.Enums.SchedulePrivateType.OfficialBusiness.GetHashCode()%>;
        var OtherBusiness = <%=BusinessTripReason.Others.GetHashCode()%>;

        $(function () {
            $("#Staff").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $("#Department").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
            })
            $("#Manager").combobox({
                required: true,
                disabled: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $("#Type").combobox({
                required: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.SchedulePrivateType,
                onChange: function () {
                    var value = $("#Type").combobox("getValue");
                    if(value == OfficialBusiness||value==BusinessTrip){
                        $(".normal").css("display","none")
                        $(".tolerance").css("display","table-row")
                        $(".tabs-last").css("display","block")
                    }
                    else{
                        $(".normal").css("display","table-row")
                        $(".tolerance").css("display","none")
                        $(".tabs-last").css("display","none")
                    }
                }
            })
            $("#SwapStaff").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
            })
            $("#BusinessReason").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.BusinessTripReason,
                onChange:function(){
                    var business =  $("#BusinessReason").combobox('getValue');
                    if(business==OtherBusiness ){
                        $("#schedule").datagrid("hideColumn", "CompanyName");
                        $("#schedule").datagrid("hideColumn", "Person");
                        $("#schedule").datagrid("hideColumn", "Department");
                        $("#schedule").datagrid("hideColumn", "Position");
                        $("#schedule").datagrid("hideColumn", "Phone");
                    }
                    else{  
                        $("#schedule").datagrid("showColumn", "CompanyName");
                        $("#schedule").datagrid("showColumn", "Person");
                        $("#schedule").datagrid("showColumn", "Department");
                        $("#schedule").datagrid("showColumn", "Position");
                        $("#schedule").datagrid("showColumn", "Phone");
                    }
                    loadData2();
                }
            })
            $("#LoanOrNot").combobox({
                valueField: 'Value',
                textField: 'Text',
                data: model.LoanOrNot,
            })
            //请假日期
            $("#dg").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadDate',
                columns: [[
                    { field: 'Date', title: '请假日期', width: 150, align: 'center' },
                    {
                        field: 'Type', title: '请假时长', width: 150, align: 'center',
                        formatter: function (value) {
                            for (var i = 0; i < model.DateType.length; i++) {
                                if (model.DateType[i].Value == value) {
                                    return model.DateType[i].Text;
                                }
                            }
                            return value;
                        },
                        editor: { type: 'combobox', options: { data: model.DateType, valueField: "Value", textField: "Text", required: true, hasDownArrow: true } }
                    },
                ]],
            });
            //行程安排
            $("#schedule").myDatagrid({
                toolbar: '#scheduletopper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadSchedule',
                columns: [[
                    { field: 'StartDate', title: '出发日期', width: 120, align: 'center', editor: { type: 'datebox' ,options: { required: true}} },
                    { field: 'StartPlace', title: '出发地点', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]',required: true} } },
                    { field: 'EndDate', title: '到达日期', width: 120, align: 'center', editor: { type: 'datebox' ,options: { required: true}} },
                    { field: 'EndPlace', title: '到达地点', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' ,required: true} } },
                    { field: 'Vehicle', title: '交通工具', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'VehicleCost', title: '预计交通花费', width: 80, align: 'center', editor: { type: 'numberbox', options: { min: 0, precision: 2, } } },
                    { field: 'StayDay', title: '住宿天数', width: 80, align: 'center', editor: { type: 'numberbox', options: { min: 0, precision: 0, } } },
                    { field: 'CompanyName', title: '客户（供应商）名称', width: 120, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Person', title: '拜访人员', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Department', title: '部门名称', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Position', title: '岗位名称', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                    { field: 'Phone', title: '联系电话', width: 80, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,11]' } } },
                    { field: 'BusinessReason', title: '出差事由简述', width: 180, align: 'center', editor: { type: 'textbox', options: { validType: 'length[1,50]' } } },
                ]],
            });
            //文件信息
            $('#file').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadFile',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationFile }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".file");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            //审批日志
            $("#logs").myDatagrid({
                fitColumns: false,
                fit: false,
                pagination: false,
                actionName: 'LoadLogs',
            });
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化申请
        function Init() {
            $(".tabs-last").css("display","none");
            if (model.ApplicationData != null) {
                $("#Staff").combobox('setValue', model.ApplicationData.ApplicantID);
                $("#SwapStaff").combobox('setValue', model.ApplicationData.SwapStaff);
                $("#WorkContext").textbox('setValue', model.ApplicationData.WorkContext);
                $("#Type").combobox('setValue', model.ApplicationData.Type);
                $("#Reason").textbox('setValue', model.ApplicationData.Reason);
                $("#Type").combobox('setValue', model.ApplicationData.Type);
                $("#BusinessReason").combobox('setValue', model.ApplicationData.BusinessReason);
                $("#Entourage").textbox('setValue', model.ApplicationData.Entourage);
                $("#LoanOrNot").combobox('setValue', model.ApplicationData.LoanOrNot);

                $("#Manager").combobox('setValue', model.ApplicationData.ApproveID);
                $("#Department").combobox('setValue', model.ApplicationData.DepartmentName);

                $.each(model.StaffData, function (i, j) {
                    if (model.StaffData[i].Value == model.ApplicationData.ApplicantID) {
                        $("#StaffCode").textbox('setValue', model.StaffData[i].SelCode);
                        return;
                    }
                })
            }
        }
    </script>
    <script>
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../../Content/Template/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>';
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }

        function loadData2() {
            var data = $('#schedule').datagrid('getData');
            $('#schedule').datagrid('loadData', data);
        }
    </script>
    <style>
        .tabs-header .tabs-panels {
            border: none;
        }

        div.datagrid-wrap {
            border: none;
        }

        .tabs-last {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">请假员工：</td>
                    <td>
                        <input id="Staff" class="easyui-combobox" style="width: 350px;" />
                    </td>
                    <td class="lbl">员工编号：</td>
                    <td>
                        <input id="StaffCode" class="easyui-textbox" style="width: 350px;" data-options="disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">员工部门：</td>
                    <td>
                        <input id="Department" class="easyui-combobox" style="width: 350px;" />
                    </td>
                    <td class="lbl">部门负责人：</td>
                    <td>
                        <input id="Manager" class="easyui-combobox" style="width: 350px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">工作交接：</td>
                    <td colspan="3">
                        <input id="SwapStaff" class="easyui-combobox" style="width: 350px;"
                            data-options="prompt:'承接人',disabled:true" />
                        <div style="width: 350px; padding-top: 5px">
                            <input id="WorkContext" class="easyui-textbox" style="width: 350px; height: 40px"
                                data-options="prompt:'工作内容描述',multiline:true,disabled:true" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">文件信息：</td>
                    <td colspan="3">
                        <div class="file" style="width: 500px">
                            <table id="file">
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">请假类型：</td>
                    <td colspan="3">
                        <input id="Type" class="easyui-combobox" style="width: 350px;" data-options="disabled:true"/>
                    </td>
                </tr>
                <tr class="normal">
                    <td class="lbl">请假原因：</td>
                    <td colspan="3">
                        <input id="Reason" class="easyui-textbox" style="width: 350px; height: 40px"
                            data-options="multiline:true,disabled:true" />
                    </td>
                </tr>
                <tr class="tolerance" style="display: none">
                    <td class="lbl">出差事由：</td>
                    <td colspan="3">
                        <input id="BusinessReason" class="easyui-combobox" style="width: 350px;" data-options="disabled:true"/>
                    </td>
                </tr>
                <tr class="tolerance" style="display: none">
                    <td class="lbl">随行人员：</td>
                    <td>
                        <input id="Entourage" class="easyui-textbox" style="width: 350px;" data-options="disabled:true"/>
                    </td>
                    <td class="lbl">是否借款：</td>
                    <td>
                        <input id="LoanOrNot" class="easyui-combobox" style="width: 350px;" data-options="disabled:true"/>
                    </td>
                </tr>
            </table>
            <div id="tt" class="easyui-tabs" style="border: none;">
                <div title="请假日期" style="display: none; border: none;">
                    <table id="dg" title="">
                    </table>
                </div>
                <div title="审批日志" style="display: none; border: none">
                    <table id="logs" style="display: none">
                        <thead>
                            <tr>
                                <th data-options="field:'CreateDate',align:'left'" style="width: 130px">审批时间</th>
                                <th data-options="field:'VoteStepName',align:'left'" style="width: 120px">审批步骤</th>
                                <th data-options="field:'AdminID',align:'left'" style="width: 100px">审批人</th>
                                <th data-options="field:'Status',align:'left'" style="width: 100px;">审批结果</th>
                                <th data-options="field:'Summary',align:'left'" style="width: 200px">审批意见</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div title="日程安排" style="display: none; border: none">
                    <table id="schedule" title="" style="display: none">
                    </table>
                </div>
            </div>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 70%; height: 80%";>
        <img id="viewfileImg" src="" style="position:relative; zoom:100%; cursor:move;" onMouseEnter="mStart();" onMouseOut="mEnd();"/>
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
