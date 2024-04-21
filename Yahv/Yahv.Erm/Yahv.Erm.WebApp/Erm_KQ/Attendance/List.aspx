<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Attendance.List" %>

<%@ Import Namespace="Yahv.Underly.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var firstLoad = true;
        $(function () {
            //考勤记录
            $("#Staff").combobox({
                required: false,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.StaffData,
                onChange: function () {
                    $('#btnSearch').click();
                },
                onLoadSuccess: function (data) {
                    console.log(data);
                    if (data) {
                        $.trim($('#Staff').combobox("setValue", data[0].Value));

                        $("#tab1").myDatagrid({
                            toolbar: '#topper',
                            singleSelect: true,
                            rownumbers: true,
                            fitColumns: true,
                            pagination: false,
                            fit: false,
                            queryParams: getQuery()
                        });
                    }
                }
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                var Date = $.trim($('#s_date').textbox("getText"));
                var AdminID = $.trim($('#Staff').combobox("getValue"));

                $("#tab1").myDatagrid({
                    toolbar: '#topper',
                    singleSelect: true,
                    rownumbers: true,
                    fitColumns: true,
                    pagination: false,
                    fit: false,
                    queryParams: getQuery()
                });
                return false;
            });

            $('#btnClear').click(function () {
                $('#Staff').combobox("setValue", "");
                return false;
            });

            //加载日期
            initDate();
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Date: $.trim($('#s_date').textbox("getText")),
                AdminID: $.trim($('#Staff').textbox("getValue"))
            };
            return params;
        };

        function formatResult(val, row, index) {
            var result = row.AttendResult.split(',');
            var arry = [];
            for (var i = 0; i < result.length; i++) {
                if (result[i].indexOf("正常") == -1) {
                    arry.push('<span style="color:red;">' + result[i] + '</span>');
                }
                else {
                    arry.push('<span">' + result[i] + '</span>');
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

        function btn_formatter(value, rec) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="calc(\'' + rec.Date + '\',\'' + rec.StaffID + '\');return false;">计算考勤</a> '
                , '</span>'].join('');
        }

        function calc(date, staffId) {
            $.post("?action=Calc", { date: date, staffId: staffId }, function (result) {
                if (result.success) {
                    $.messager.alert('操作提示', result.data, 'info', function () {
                        $("#tab1").myDatagrid({
                            toolbar: '#topper',
                            singleSelect: true,
                            rownumbers: true,
                            fitColumns: true,
                            pagination: false,
                            fit: false,
                            queryParams: getQuery()
                        });
                    });
                } else {
                    $.messager.alert('操作提示', result.data, 'error');
                }

            });
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
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="员工考勤记录">
        <thead>
            <tr>
                <th data-options="field:'Date',align:'center',width:100">日期</th>
                <th data-options="field:'Week',align:'center',width:100">星期几</th>
                <th data-options="field:'Name',align:'center',width:100">员工姓名</th>
                <th data-options="field:'Code',align:'center',width:100">员工编号</th>
                <th data-options="field:'DepartmentCode',align:'center',width:100">部门</th>
                <th data-options="field:'Scheduling',align:'center',width:100">班别名称</th>
                <th data-options="field:'AttendTime',align:'center',width:150">打卡时间</th>
                <th data-options="field:'AttendResult',align:'center',width:200,formatter:formatResult">考勤结果</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
