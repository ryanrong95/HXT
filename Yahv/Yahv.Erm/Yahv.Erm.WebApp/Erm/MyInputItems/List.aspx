<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.MyInputItems.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/handsontable.full.min.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/handsontable.full.min.css" rel="stylesheet">

    <script>
        var ht;
        var settings;
        var changeData = [];        //修改的数据

        $(function () {
            initDate();     //初始化日期
            initData();     //加载数据


            $("#btnImport").on("click",
                function () {
                    if ($("#wageDate").val() == '') {
                        top.$.messager.alert('提示', '请您先选择工资日期!');
                        return;
                    }

                    $("#<%=fileUpload.ClientID%>").click();
                });

            $("#<%=fileUpload.ClientID%>").on("change", function () {
                if (this.value === "") {
                    top.$.messager.alert('提示', '请选择要上传的Excel文件');
                    return;
                } else {
                    var index = this.value.lastIndexOf(".");
                    var extention = this.value.substr(index);
                    if (extention !== ".xls" && extention !== ".xlsx") {
                        top.$.messager.alert('提示', '请选择excel格式的文件!');
                        return;
                    }

                    $("#<%= btn_Import.ClientID %>").click();
                    this.value = '';        //清空值，确保每次导入都触发change事件
                }
            });

            $('#btnSave').click(function () {
                $('#btnSave').linkbutton('disable');

                if (changeData.length > 0) {
                    $.post('?action=save', {
                        source: JSON.stringify(changeData),
                        dateIndex: $("#<%= wageDate.ClientID %>").val()
                    }, function (result) {
                        var data = eval(result);
                        if (data.code === 200) {
                            //$.messager.alert("操作提示", "操作成功!", "info", function () {
                            //    //刷新页面
                            //    initData();
                            //});
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作成功!",
                                type: "success"
                            });
                            initData();//刷新页面
                            changeData = [];
                        } else if (data.code === 400) {
                            $.messager.alert("操作提示", "操作失败，" + data.message, "info", function () {
                                //刷新页面
                                initData();
                            });
                        }
                    });
                } else {
                    $.messager.alert("操作提示", "您没有修改任何项!", "info");
                }
                $('#btnSave').linkbutton('enable');
            });

            $('#btnSearch').click(function () {
                initData();
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });

            //提交按钮
            $('#btnSubmit').click(function () {
                $.messager.confirm("操作提示", "提交之后数据就不能修改，您确定提交吗？", function (result) {
                    if (result) {
                        $.post("?action=submit", { dateIndex: $("#<%= wageDate.ClientID %>").val() }, function (data) {
                            if (data.code === 200) {
                                //$.messager.alert("操作提示", "操作成功!", "info", function () {
                                //    //刷新页面
                                //    initData();
                                //});
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功!",
                                    type: "success"
                                });
                                //刷新页面
                                initData();
                                changeData = [];
                            } else {
                                $.messager.alert("操作提示", data.message, "error", function () {
                                    //刷新页面
                                    initData();
                                });
                            }
                        });
                    }
                });
                return false;
            });
        });

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
                            height: $(window).height() - 60,      //设置了高度以后就会出现横向滚动条
                            //隐藏列
                            hiddenColumns: {
                                columns: [0, 1],
                                indicators: false
                            },
                            afterChange: function (changes, source) {
                                if (changes) {
                                    $.each(changes, function (index) {
                                        var data = {
                                            "PayID": ht.getDataAtCell(changes[index][0], 0),
                                            "StaffID": "1",     //此处的staffid代表 修改或新增，这里都是修改
                                            "Name": changes[index][1],
                                            "Value": changes[index][3]
                                        };
                                        changeData.push(data);
                                    });
                                }
                            }
                        }

                        //如果已经封账或者提交，设置为只读
                        $.post("?action=isOnlyRead", { dateIndex: $("#<%= wageDate.ClientID %>").val() }, function (result) {
                            if (result) {
                                settings.readOnly = true;
                            }

                            //实例化handsontable
                            ht = new Handsontable(document.getElementById('list'), settings);
                            ht.getDataAtRow(0);
                        });

                        ////实例化handsontable
                        //ht = new Handsontable(document.getElementById('list'), settings);
                        //ht.getDataAtRow(0);
                    });
                });
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="form-group" style="display: none;">
        <asp:FileUpload ID="fileUpload" runat="server" />
        <input type="button" name="btn_Import" id="btn_Import" value="upload" runat="server" onserverclick="btnImport_Click" />
    </div>

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
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnImport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                    <a id="btnExport" class="easyui-linkbutton" runat="server" onserverclick="btnExport_Click" data-options="iconCls:'icon-yg-excelExport'">Excel导出</a>
                    <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'">保存</a>
                    <a id="btnSubmit" class="easyui-linkbutton" data-options="iconCls:'icon-yg-saveSubmit'">提交</a>
                </td>
                <td colspan="2">
                    <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>
                </td>
            </tr>
        </table>
        <div id="list"></div>
    </div>
</asp:Content>
