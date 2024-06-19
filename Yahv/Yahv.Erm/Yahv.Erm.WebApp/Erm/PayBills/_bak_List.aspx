<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="_bak_List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.PayBills._bak_List" %>

<%@ Import Namespace="Yahv.Utils.Serializers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/handsontable.full.min.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/handsontable/dist/handsontable.full.min.css" rel="stylesheet">

    <script>
        var ht;

        $(function () {
            initDate();

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
                }
            });

            var dynamicColumns = <%=GetColNames()%>;
            var list = document.getElementById('list');
            var changeData = [];        //修改的数据
            var settings = {
                licenseKey: 'non-commercial-and-evaluation',
                data: model,
                columns: dynamicColumns,
                rowHeaders: true,
                manualColumnMove: true, //true/false 当值为true时，列可拖拽移动到指定列
                manualColumnResize: true, //true/false//当值为true时，允许拖动，当为false时禁止拖动
                autoColumnSize: true, //true/false //当值为true且列宽未设置时，自适应列大小
                minRows: 0, //最小行数
                minSpareRows: 0, //最小行空间，不足则添加空行
                filters: true,
                //readOnly:true,
                //colWidths: [200, 100],
                //dropdownMenu: true,
                //copyable: false,
                //wordWrap: true
                //contextMenu: true,
                //contextMenu: ["remove_row"],
                //隐藏列
                hiddenColumns: {
                    columns: [0],
                    indicators: false
                },
                afterChange:function(changes,source) {
                    //console.log('afterChange');
                    if (changes) {
                        var data = {
                            "PayID": ht.getDataAtCell(changes[0][0], 0),
                            "Name":changes[0][1],
                            "Value": changes[0][3]
                        };
                        changeData.push(data);
                    }

                    //console.log(changeData);
                },
                afterInit :function() {     
                    //实例被初始化后调用
                    //console.log("afterInit");
                    changeData = [];   
                }
            }

            //如果已经封账，设置为只读
            $.post("?action=isClosed",{dateIndex: $("#<%= wageDate.ClientID %>").val()},function(result) {
                var data = eval(result);
                if (data.code === 200) {
                    settings.readOnly = true;
                }

                //实例化handsontable
                ht = new Handsontable(list, settings);
                ht.getDataAtRow(0);
            });

           

            $('#btnSave').click(function() {
                $('#btnSave').linkbutton('disable');

                //console.log(changeData.length);
                if (changeData.length > 0) {
                    $.post('?action=save', {
                        source: JSON.stringify(changeData),
                        dateIndex: $("#<%= wageDate.ClientID %>").val()
                    }, function(result) {
                        var data = eval(result);
                        if (data.code === 200) {
                            <%--$.messager.alert("操作提示", "操作成功!", "info",function() {
                                //刷新页面
                                __doPostBack('<%=btnSearch.UniqueID%>', ""); 
                            });--%>
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作成功!",
                                type: "success"
                            });
                            //刷新页面
                            __doPostBack('<%=btnSearch.UniqueID%>', ""); 
                            changeData = [];
                        } else if (data.code === 400) {
                            $.messager.alert("操作提示", "操作失败" + data.message, "info");
                        }

                    });
                } else {
                    $.messager.alert("操作提示", "您没有修改任何项!", "info");
                }

                $('#btnSave').linkbutton('enable');
            });

            //封账
            $('#btnClosed').click(function() {
                $('#btnClosed').linkbutton('disable');

                $.messager.confirm("操作提示", "封账之后数据就不能修改，您确定封账吗？",function(result) {
                    if (result) {
                        $.post("?action=closed", {dateIndex: $("#<%= wageDate.ClientID %>").val()},function(result) {
                            var data = eval(result);
                            if (data.code === 200) {
                                <%--$.messager.alert("操作提示", "操作成功!", "info",function() {
                                    //刷新页面
                                    __doPostBack('<%=btnSearch.UniqueID%>', ""); 
                                });--%>
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功!",
                                    type: "success"
                                });
                                __doPostBack('<%=btnSearch.UniqueID%>', ""); 
                            } else {
                                $.messager.alert("操作提示", "操作失败" + data.message, "info");
                            }
                        });
                    }
                });

                $('#btnClosed').linkbutton('enable');
            });

            //导出
            $('#btnExport').click(function() {
                //var exportPlugin = ht.getPlugin('exportFile');
                //exportPlugin.downloadFile('csv', {filename: 'MyFile'});


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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="form-group" style="display: none;">
        <asp:FileUpload ID="fileUpload" runat="server" />
        <input type="button" name="btn_Import" id="btn_Import" value="upload" runat="server" onserverclick="btnImport_Click" />
    </div>

    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">工资日期</td>
                <td>
                    <input id="wageDate" type="text" class="easyui-datebox" runat="server" required="required">
                    <asp:HiddenField ID="hid_wageDate" runat="server" />
                </td>
                <td>所属公司：</td>
                <td>
                    <input id="CompanyId" runat="server" data-options="editable: false, url:'?action=getCompanies',valueField:'Value',textField:'Text'," class="easyui-combobox" style="width: 200px" />
                </td>
                <td>岗位：</td>
                <td>
                    <input id="PostionId" runat="server" data-options="editable: false, url:'?action=getPostions',valueField:'Value',textField:'Text'," class="easyui-combobox" style="width: 200px" />
                </td>
                <td style="width: 120px;">姓名/工号：</td>
                <td>
                    <input id="txt_name" data-options="prompt:'姓名/工号',validType:'length[1,75]'" runat="server" class="easyui-textbox" />
                </td>

            </tr>
            <tr>
                <td colspan="8">
                    <div style="text-align: left; float: left;">
                        <%--<a id="btnCreator" class="easyui-linkbutton" iconcls="icon-add">添加</a>--%>
                        <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-search" runat="server" onserverclick="btnSearch_Click">搜索</a>
                        <%--<a id="btnInit" class="easyui-linkbutton" data-options="iconCls:'icon-back'">初始化默认值</a>
                     <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-man'">启用</a>
                         <a id="btnClear" class="easyui-linkbutton" iconcls="icon-reload">清空</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-lock'">停用</a>--%>
                        <a id="btnImport" class="easyui-linkbutton" data-options="iconCls:'icon-save'">Excel导入</a>
                        <a id="btnExport" class="easyui-linkbutton" runat="server" OnServerClick="btnExport_Click" data-options="iconCls:'icon-save'">Excel导出</a>
                    </div>

                    <div style="text-align: right; float: right;">
                        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-ok'">保存</a>
                        <a id="btnClosed" class="easyui-linkbutton" data-options="iconCls:'icon-lock'">封账</a>
                    </div>
                </td>
            </tr>
        </table>
        <div id="list"></div>
    </div>
</asp:Content>
