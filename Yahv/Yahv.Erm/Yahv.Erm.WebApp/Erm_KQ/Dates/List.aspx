<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Dates.List" %>

<%@ Import Namespace="Yahv.Underly.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //加载日期
            initDate();
            //假期类型
            $("#s_method").combobox({
                data: model.Methods,
                valueField: 'Value',
                textField: 'Text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                    return false;
                }
            });
            //所属班别
            $('#s_scheduling').combobox({
                data: model.Schedulings,
                valueField: 'Value',
                textField: 'Text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                    return false;
                }
            });
            //所属区域
            $('#s_region').combobox({
                data: model.RegionData,
                valueField: 'Value',
                textField: 'Text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                    return false;
                }
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                //$('#s_date').textbox("setValue", "");
                $('#s_method').combobox("setValue", "");
                $('#s_scheduling').combobox("setValue", "");
                $('#s_region').combobox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //日期初始化
            $('#btnInitDate').click(function () {
                InitDate();
                return false;
            });
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery(),
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_date: $.trim($('#s_date').textbox("getText")),
                s_method: $.trim($('#s_method').combobox('getValue')),
                s_scheduling: $.trim($('#s_scheduling').combobox('getValue')),
                s_region: $.trim($('#s_region').combobox('getValue')),
            };
            return params;
        };

        function edit(id) {
            $.myDialog({
                title: "编辑信息",
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "1000px",
                height: "500px",
            });
            return false;
        }

        function detail(id) {
            $.myDialog({
                title: "详细信息",
                url: 'Edit.aspx?id=' + id,
                isHaveOk: false,
                width: "1000px",
                height: "500px",
            });
            return false;
        }

        //日期初始化
        function InitDate() {
            $.myWindow({
                title: "日期初始化",
                url: 'Init.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }

        function btn_formatter(value, rec) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');

            //之前日期不可修改
            if (new Date(rec.Date) > new Date(getDay(-1, '-'))) {
                arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.ID + '\');return false;">编辑</button>');
            } else {
                arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rec.ID + '\');return false;">查看</button>');
            }

            arry.push('</span>');
            return arry.join('');
        }

        function formatMethod(val, row) {
            if (row.MethodID == '<%= (int)ScheduleMethod.Work%>') {
                return val;
            } else {
                return '<span style="color:red;">' + val + '</span>';
            }
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
                                //得到年份
                                var year = /\d{4}/.exec(span.html())[0],
                                        //月份
                                        //之前是这样的month = parseInt($(this).attr('abbr'), 10) + 1; 
                                        month = parseInt($(this).attr('abbr'), 10);

                                //隐藏日期对象                     
                                $('#s_date').datebox('hidePanel')
                                    //设置日期的值
                                    .datebox('setValue', year + '-' + month);

                                //更新列表
                                grid.myDatagrid('search', getQuery());
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
                <td style="width: 90px;">日期</td>
                <td style="width: 200px;">
                    <input id="s_date" type="text" class="easyui-datebox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">所属班别</td>
                <td style="width: 200px;">
                    <input id="s_scheduling" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">所属区域</td>
                <td style="width: 200px;">
                    <input id="s_region" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">假期类型</td>
                <td>
                    <input id="s_method" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnInitDate" class="easyui-linkbutton" iconcls="icon-yg-add">初始化日期</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="日期列表">
        <thead>
            <tr>
                <th data-options="field:'Date',width:150">日期</th>
                <th data-options="field:'Week',width:150">星期几</th>
                <th data-options="field:'RegionName',width:150">所属区域</th>
                <th data-options="field:'ShiftName',width:150">所属班别</th>
                <th data-options="field:'SchedulingName',width:150">实际班别</th>
                <th data-options="field:'Method',width:150,formatter:formatMethod">日期类型</th>
                <%--<th data-options="field:'SalaryMultiple',width:50">薪资标准（倍数）</th>--%>
                <th data-options="field:'btn',formatter:btn_formatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
