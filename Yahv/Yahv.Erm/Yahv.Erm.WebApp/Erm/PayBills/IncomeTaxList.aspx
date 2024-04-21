<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="IncomeTaxList.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.PayBills.IncomeTaxList" %>

<%@ Import Namespace="Yahv.Utils.Serializers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.css" rel="stylesheet">
    <script>
        var ht;
        var settings;

        $(function () {
            initDate();     //加载日期
            initData();     //加载数据

            $('#btnSearch').click(function () {
                initData();
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });
        });

        //获取上个月
        function getLastMonth(date) {
            //获取年份
            var y = date.getFullYear();
            //获取月份
            var m = date.getMonth();
            return y + '-' + m;
        }

        //初始化年月日期选择
        function initDate() {
            $('#wageDate').datebox({
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
                                $('#wageDate').datebox('hidePanel')
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
            var p = $('#wageDate').datebox('panel'),
                //日期选择对象中月份
                tds = false,
                //显示月份层的触发控件
                span = p.find('span.calendar-text');

            //设置前当月
            //$("#wageDate").datebox("setValue", getLastMonth(new Date()));
        }

        //查询条件
        var getQuery = function () {
            var params = {
                dateIndex: $("#<%= wageDate.ClientID %>").val(),
                name: $("#<%= txt_name.ClientID %>").val(),
                company: $("#CompanyId").val(),
                postion: $("#PostionId").val(),
                status: $("#Status").val()
            };
            return params;
        };

        //加载数据
        function initData() {
            try {
                if (ht != undefined)
                    ht.destroy();       //销毁
            } catch (e) {
                console.log(e);
            }

            $.post("?action=getData", getQuery(),
                function (result) {
                    var data = eval(result);
                    $.post("?action=getColNames", getQuery(), function (result) {
                        var dynamicColumns = eval(result);
                        settings = {
                            licenseKey: 'non-commercial-and-evaluation',
                            data: data,
                            columns: dynamicColumns,
                            rowHeaders: true,
                            manualColumnMove: true, //true/false 当值为true时，列可拖拽移动到指定列
                            manualColumnResize: true, //true/false//当值为true时，允许拖动，当为false时禁止拖动
                            autoColumnSize: true, //true/false //当值为true且列宽未设置时，自适应列大小
                            minRows: 0, //最小行数
                            minSpareRows: 0, //最小行空间，不足则添加空行
                            filters: true,
                            stretchH: 'none',
                            wordWrap: false,    //自动换行
                            width: '100%',
                            readOnly: true,        //只读
                            height: $(window).height() - 60,      //设置了高度以后就会出现横向滚动条
                            //隐藏列
                            hiddenColumns: {
                                columns: [0, 1],
                                indicators: false
                            },
                            contextMenu: {//右键菜单
                                items: {
                                    "about": { // Own custom option
                                        name: function () { // `name` can be a string or a function
                                            return '<b>个税详情</b>'; // Name can contain HTML
                                        },
                                        callback: function (key, selection, clickEvent) { // Callback for specific option
                                            setTimeout(function () {
                                                var staffId = ht.getDataAtRow(selection[0].start.row)[1];
                                                var dateIndex = $("#<%= wageDate.ClientID %>").val();

                                                $.myDialog({ title: '个税详情', isHaveOk: false, url: 'IncomeTax.aspx?dateIndex=' + dateIndex.replace('-', '') + '&staffId=' + staffId });
                                            }, 0);
                                        }
                                    },
                                    //"hsep1": "---------",
                                    //remove_row: {
                                    //    name: "<b>删除</b>"
                                    //}
                                }
                            }
                        }


                        //实例化handsontable
                        ht = new Handsontable(document.getElementById('list'), settings);
                        ht.getDataAtRow(0);
                    });
                });
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">工资日期</td>
                <td>
                    <input id="wageDate" type="text" class="easyui-datebox" runat="server" required="required" />
                    <asp:HiddenField ID="hid_wageDate" runat="server" />
                </td>
                <td style="width: 90px;">所属公司</td>
                <td>
                    <input id="CompanyId" runat="server" data-options="editable: true, url:'?action=getCompanies',valueField:'Value',textField:'Text'," class="easyui-combobox" />
                </td>
                <td style="width: 90px;">岗位</td>
                <td>
                    <input id="PostionId" runat="server" data-options="editable: true, url:'?action=getPostions',valueField:'Value',textField:'Text'," class="easyui-combobox" />
                </td>
                <td style="width: 90px;">姓名/工号</td>
                <td>
                    <input id="txt_name" data-options="prompt:'姓名/工号',validType:'length[1,75]',isKeydown:true" runat="server" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnExport" class="easyui-linkbutton" runat="server" onserverclick="btnExport_Click" data-options="iconCls:'icon-yg-excelExport'">Excel导出</a>
                </td>
            </tr>
        </table>
        <div id="list"></div>
    </div>
</asp:Content>
