<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Attends.List" %>

<%@ Import Namespace="Yahv.Underly.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var firstLoad = true;
        $(function () {
            //考勤记录
            $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: false,
                rownumbers: true,
                fitColumns: false,
                pagination: true,
                pageSize: 50,
                fit: false,
                queryParams: getQuery(),
            });
            //考勤统计
            $("#dg").myDatagrid({
                fitColumns: false,
                fit: false,
                pagination: false,
                actionName: 'LoadDate',
                onLoadSuccess: function () {
                    if (firstLoad) {
                        if (model.AttendData != null) {
                            //清空统计数据
                            var item = $('#dg').datagrid('getRows');
                            if (item) {
                                for (var i = item.length - 1; i >= 0; i--) {
                                    var index = $('#dg').datagrid('getRowIndex', item[i]);
                                    $('#dg').datagrid('deleteRow', index);
                                }
                            }
                            //添加统计数据
                            $('#dg').datagrid('appendRow', {
                                Date: model.AttendData.Data,
                                WorkDays: model.AttendData.WorkDays,
                                NormalDays: model.AttendData.NormalDays,
                                OvertimeDays: model.AttendData.OvertimeDays,
                                LaterOrEarlyTimes: model.AttendData.LaterOrEarlyTimes,
                                ResignTimes: model.AttendData.ResignTimes,
                                CasualLeavedays: model.AttendData.CasualLeavedays,
                                SickLeavedays: model.AttendData.SickLeavedays,
                                Absenteeismdays: model.AttendData.Absenteeismdays,
                                OfficialBusinessdays: model.AttendData.OfficialBusinessdays,
                                BusinessTripdays: model.AttendData.BusinessTripdays,
                                PaidLeavedays: model.AttendData.PaidLeavedays,
                            });
                        }
                        firstLoad = false;
                    }
                }
            });
            $("#IsWorkDate").checkbox({
                onChange: function () {
                    $('#btnSearch').click();
                }
            })
            $("#Staff").combobox({
                required: false,
                editable: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
                //onChange: function () {
                //    $('#btnSearch').click();
                //}
            })
            // 搜索按钮
            $('#btnSearch').click(function () {
                var Date = $.trim($('#s_date').textbox("getText"));
                var AdminID = $.trim($('#Staff').textbox("getValue"));
                $.post('?action=DateChange', { Date: Date, AdminID: AdminID }, function (res) {
                    var result = JSON.parse(res);
                    if (result.success) {
                        var item = $('#dg').datagrid('getRows');
                        if (item) {
                            for (var i = item.length - 1; i >= 0; i--) {
                                var index = $('#dg').datagrid('getRowIndex', item[i]);
                                $('#dg').datagrid('deleteRow', index);
                            }
                        }
                        //清空统计数据
                        $('#dg').datagrid('appendRow', {
                            Date: result.data.Data,
                            WorkDays: result.data.WorkDays,
                            NormalDays: result.data.NormalDays,
                            OvertimeDays: result.data.OvertimeDays,
                            LaterOrEarlyTimes: result.data.LaterOrEarlyTimes,
                            ResignTimes: result.data.ResignTimes,
                            CasualLeavedays: result.data.CasualLeavedays,
                            SickLeavedays: result.data.SickLeavedays,
                            Absenteeismdays: result.data.Absenteeismdays,
                            OfficialBusinessdays: result.data.OfficialBusinessdays,
                            BusinessTripdays: result.data.BusinessTripdays,
                            PaidLeavedays: result.data.PaidLeavedays,
                        });
                    }
                })
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });
            $('#btnClear').click(function () {
                $('#Staff').combobox("setValue", "");
                $('#btnSearch').click();
                return false;
            });
            //修正考勤班别
            $('#btnChangeSch').click(function () {
                var staffid = $("#Staff").combobox('getValue');
                if (staffid == "" || staffid == undefined) {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择员工后点击该按钮!", type: "error" });
                    return;
                }
                var rows = $("#tab1").datagrid('getChecked');
                if (rows.length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "请勾选考勤记录", type: "error" });
                    return;
                }
                if (rows.length > 7) {
                    top.$.timeouts.alert({ position: "TC", msg: "最多勾选7条考勤记录", type: "error" });
                    return;
                }

                var dates = [];
                for (var i = 0; i < rows.length; i++) {
                    dates.push(rows[i].DateStr);
                }

                $.myDialog({
                    title: "修正员工考勤班别",
                    url: 'ChangeScheduling.aspx?id=' + staffid + '&dates=' + dates.join(','),
                    onClose: function () {
                        $('#btnSearch').click();
                    },
                    width: "600px",
                    height: "300px",
                });
            })
            //系统授权
            $('#btnSysNormal').click(function () {
                var staffid = $("#Staff").combobox('getValue');
                if (staffid == "" || staffid == undefined) {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择员工后点击该按钮!", type: "error" });
                    return;
                }
                var rows = $("#tab1").datagrid('getChecked');
                if (rows.length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "请勾选考勤记录", type: "error" });
                    return;
                }
                $.messager.confirm('确认', '请您再次确认是否进行系统授权！', function (success) {
                    if (success) {
                        var dates = [];
                        for (var i = 0; i < rows.length; i++) {
                            dates.push(rows[i].DateStr);
                        }
                        ajaxLoading();
                        $.post('?action=SysNormal', { AdminID: staffid, Dates: dates.join(',') },
                            function (res) {
                                ajaxLoadEnd();
                                var result = JSON.parse(res);
                                if (result.success) {
                                    top.$.timeouts.alert({ position: "TC", msg: "授权成功!", type: "success" });
                                }
                                else {
                                    top.$.timeouts.alert({ position: "TC", msg: "授权失败!", type: "error" });
                                }
                            })
                        $('#btnSearch').click();
                    }
                });
            })
            //加载日期
            initDate();
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Date: $.trim($('#s_date').textbox("getText")),
                AdminID: $.trim($('#Staff').textbox("getValue")),
                IsWorkDate: $('#IsWorkDate').checkbox('options').checked,
            };
            return params;
        };
        function formatResult(val, row, index) {
            var result = row.AttendResult.split(',');
            var arry = [];
            for (var i = 0; i < result.length; i++) {
                if (result[i].indexOf("正常") != -1) {
                    arry.push('<span">' + result[i] + '</span>');
                }
                else if (result[i].indexOf("系统授权") != -1) {
                    arry.push('<span style="color:green;">' + result[i] + '</span>');
                }
                else {
                    arry.push('<span style="color:red;">' + result[i] + '</span>'); 
                }
            }
            return arry.join(',');
        }
        //初始化年月日期选择
        function initDate() {
            $('#s_date').datebox({
                editable: false,
                //显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
                onShowPanel: function () {
                    //触发click事件弹出月份层
                    span.trigger('click');
                    if (!tds)
                        //延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                        setTimeout(function () {
                            tds = p.find('div.calendar-menu-month-inner td');
                            tds.click(function (e) {
                                //禁止冒泡执行easyui给月份绑定的事件
                                e.stopPropagation();
                                //获取年份
                                var year = /\d{4}/.exec(span.html())[0],
                                //获取月份
                                month = parseInt($(this).attr('abbr'), 10);
                                //隐藏日期对象    //设置日期的值                 
                                $('#s_date').datebox('hidePanel').datebox('setValue', year + '-' + month);
                            });
                        }, 0);
                },
                //配置parser，返回选择的日期
                parser: function (s) {
                    if (!s) return new Date();
                    var arr = s.split('-');
                    return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
                },
                //配置formatter，只返回年月 之前是这样的d.getFullYear() + '-' +(d.getMonth()); 
                formatter: function (d) {
                    var currentMonth = (d.getMonth() + 1);
                    var currentMonthStr = currentMonth < 10 ? ('0' + currentMonth) : (currentMonth + '');
                    return d.getFullYear() + '-' + currentMonthStr;
                },
                onChange: function () {
                    if (!firstLoad) {
                        $('#btnSearch').click();
                    }
                }
            });

            //日期选择对象
            var p = $('#s_date').datebox('panel'),
                //日期选择对象中月份
                tds = false,
                //显示月份层的触发控件
                span = p.find('span.calendar-text');

            //设置前当月
            $("#s_date").datebox("setValue", getCurrentMonth(new Date()));
        }
        //获取当前月份
        function getCurrentMonth(date) {
            //获取年份
            var y = date.getFullYear();
            //获取月份
            var m = date.getMonth() + 1;
            return y + '-' + m;
        }
        //获取天数
        function getDay(num, str) {
            var today = new Date();
            var nowTime = today.getTime();
            var ms = 24 * 3600 * 1000 * num;
            today.setTime(parseInt(nowTime + ms));
            var oYear = today.getFullYear();
            var oMoth = (today.getMonth() + 1).toString();
            if (oMoth.length <= 1) oMoth = '0' + oMoth;
            var oDay = today.getDate().toString();
            if (oDay.length <= 1) oDay = '0' + oDay;
            return oYear + str + oMoth + str + oDay;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">考勤月份</td>
                <td style="width: 250px;">
                    <input id="s_date" name="s_date" type="text" class="easyui-datebox" style="width: 200px;" />
                </td>
                <td style="width: 90px;">员工名称</td>
                <td>
                    <input id="Staff" name="Staff" class="easyui-combobox" style="width: 200px;" />&nbsp&nbsp&nbsp&nbsp
                    <input id="IsWorkDate" name="IsWorkDate" class="easyui-checkbox" data-options="label:'显示非工作日',labelPosition:'after',checked:false">
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnChangeSch" class="easyui-linkbutton" data-options="iconCls:'icon-redo'">修正员工考勤班别</a>
                    <em class="toolLine"></em>
                    <a id="btnSysNormal" class="easyui-linkbutton" data-options="iconCls:'icon-ok'">系统授权考勤正常</a>
                    <em class="toolLine"></em>
                    <a id="btnExport" class="easyui-linkbutton" runat="server" onserverclick="btnExport_Click" data-options="iconCls:'icon-yg-excelExport'">导出考勤扣款Excel</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="员工考勤记录">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <th data-options="field:'Date',align:'center',sortable:true,width:160">日期</th>
                <th data-options="field:'Name',align:'center',width:100">员工姓名</th>
                <th data-options="field:'Code',align:'center',width:100">员工编号</th>
                <th data-options="field:'DepartmentCode',align:'center',width:100">部门</th>
                <th data-options="field:'Scheduling',align:'center',width:100">实际班别</th>
                <th data-options="field:'AttendTime',align:'center',width:182">打卡时间</th>
                <th data-options="field:'WordHours',align:'center',width:100">工作时长</th>
                <th data-options="field:'AttendResult',align:'center',width:300,formatter:formatResult">考勤结果</th>
            </tr>
        </thead>
    </table>
    <div style="margin-top: 2px">
        <table id="dg" title="">
            <thead>
                <tr>
                    <th data-options="field:'Date',align:'center',width:100">考勤月份</th>
                    <th data-options="field:'WorkDays',align:'center',width:100">工作日</th>
                    <th data-options="field:'NormalDays',align:'center',width:100">正常</th>
                    <th data-options="field:'BusinessTripdays',align:'center',width:100">公差</th>
                    <th data-options="field:'OfficialBusinessdays',align:'center',width:100">公务</th>
                    <th data-options="field:'PaidLeavedays',align:'center',width:100">带薪假</th>
                    <th data-options="field:'CasualLeavedays',align:'center',width:100">事假</th>
                    <th data-options="field:'SickLeavedays',align:'center',width:100">病假</th>
                    <th data-options="field:'Absenteeismdays',align:'center',width:100">旷工</th>
                    <th data-options="field:'ResignTimes',align:'center',width:100">补签</th>
                    <th data-options="field:'LaterOrEarlyTimes',align:'center',width:100">迟到早退</th>
                    <th data-options="field:'OvertimeDays',align:'center',width:100">加班</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
