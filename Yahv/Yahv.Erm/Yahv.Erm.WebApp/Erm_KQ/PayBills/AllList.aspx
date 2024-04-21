﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="AllList.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.PayBills.AllList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                s_name: $.trim($('#s_name').textbox("getText")),
                s_wageDate: $.trim($('#s_wageDate').textbox("getText"))
            };
            return params;
        };

        $(function () {
            initDate();
            $.post("?action=getColNames", getQuery(), function (result) {
                window.grid = $("#tab1").myDatagrid({
                    toolbar: '#topper',
                    pagination: true,
                    columns: [JSON.parse(result)],
                    queryParams: getQuery()
                });
            });


            // 搜索按钮
            $('#btnSearch').click(function () {
                //重新获取列
                $.get("?action=getColNames", getQuery(), function (result) {
                    var array = JSON.parse(result);
                    //array.push({ field: 'btn', title: '操作', formatter: Operation, width: 80 });

                    $("#tab1").myDatagrid({
                        toolbar: '#topper',
                        pagination: true,
                        columns: [array],
                        queryParams: getQuery()
                    });
                });

                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });
        });

        //获取当前月
        function getCurrentMonth(date) {
            //获取年份
            var y = date.getFullYear();
            //获取月份
            var m = date.getMonth() + 1;
            return y + '-' + m;
        }

        //初始化年月日期选择
        function initDate() {
            $('#s_wageDate').datebox({
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
                                $('#s_wageDate').datebox('hidePanel')
                                    //设置日期的值
                                    .datebox('setValue', year + '-' + month);
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
            var p = $('#s_wageDate').datebox('panel'),
                //日期选择对象中月份
                tds = false,
                //显示月份层的触发控件
                span = p.find('span.calendar-text');

            //设置前当月
            $("#s_wageDate").datebox("setValue", getCurrentMonth(new Date()));
        }

        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.DateIndex + '\',\'' + row.StaffID + '\');return false;" group>个税详情</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function Edit(dateIndex, staffId) {
            $.myDialog({ title: '个税详情', isHaveOk: false, url: '/Erm/Erm/PayBills/IncomeTax.aspx?dateIndex=' + dateIndex + '&staffId=' + staffId });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">工资日期</td>
                <td style="width: 250px;">
                    <input id="s_wageDate" type="text" class="easyui-datebox" style="width: 200px">
                </td>
                <td style="width: 90px;">姓名/工号</td>
                <td style="width: 250px;">
                    <input id="s_name" data-options="prompt:'姓名/工号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" style="width: 200px" />
                </td>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">刷新</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="员工工资列表"></table>
</asp:Content>
