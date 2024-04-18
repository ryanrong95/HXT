<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.GeneralManage.Profit.List" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>利润提成查询</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '综合管理(XDT)';
        gvSettings.menu = '业务提成';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            //debugger;
            var ServiceManager = eval('(<%=this.Model.ServiceManager%>)');
            var DepartmentType = eval('(<%=this.Model.DepartmentType%>)');
            var Department = eval('(<%=this.Model.Department%>)');

            //初始化业务员下拉框
            $('#ServiceManager').combobox({
                data: ServiceManager,
            });

            //初始化部门下拉框
            $('#DepartmentType').combobox({
                data: Department,
            });

            window.grid = $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                toolbar: '#topBar',
                queryParams: { StartDate: dateMonth(0), EndDate: dateMonth(1), action: "data" },
                pagination: false,
            });
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            //Search();
        });

        //查询
        function Search() {
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var SaleManID = $("#ServiceManager").combobox('getValue');
            var DepartmentType = $("#DepartmentType").combobox('getText');
            $('#datagrid').myDatagrid('search', { StartDate: StartDate, EndDate: EndDate, SaleManID: SaleManID, DepartmentType: DepartmentType, });
        }

        //重置
        function Reset() {
            $('#StartDate').datebox('setValue', dateMonth(0));
            $('#EndDate').datebox('setValue', dateMonth(1));
            $("#ServiceManager").combobox('setValue', "");
            $("#DepartmentType").combobox('setValue', "");
            Search();
        }

        //设上月月底跟月头时间
        //function time(number) {
        //    var date = new Date();
        //    var strDate = date.getFullYear() + "-";
        //    //0 为月底
        //    if (number == 0) {
        //        strDate += date.getMonth() + 1 + "-";
        //        strDate += number;
        //    } else {
        //        strDate += date.getMonth() + "-";
        //        strDate += number;
        //    }
        //    return strDate;
        //}

        function dateMonth(number) {
            var nowdays = new Date();
            var year = nowdays.getFullYear();
            var month = nowdays.getMonth();
            if (month == 0) {
                month = 12;
                year = year - 1;
            }
            if (month < 10) {
                month = "0" + month;
            }
            if (number == 0) {
                return year + "-" + month + "-" + "01";
            }
            else {
                var myDate = new Date(year, month, 0);
                return year + "-" + month + "-" + myDate.getDate();
            }
        }

        //导出Excel
        function Export() {
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var SaleManID = $("#ServiceManager").combobox('getValue');
            var DepartmentType = $("#DepartmentType").combobox('getText');
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                StartDate: StartDate,
                EndDate: EndDate,
                SaleManID: SaleManID,
                DepartmentType: DepartmentType,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }

        //查看
        function View(id) {
            if (id) {
                var StartDate = $("#StartDate").datebox('getValue');
                var EndDate = $("#EndDate").datebox('getValue');
                if (StartDate == "" || EndDate == "") {
                    $.messager.alert('提示', '请选择报关日期！');
                    return;
                }
                var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx?ID=' + id + '&StartDate=' + StartDate + "&EndDate=" + EndDate);
                //window.location = url;
                //var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx')
                //    + '?ID=' + id + '&StartDate=' + StartDate + '&EndDate=' + EndDate;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '查看',
                    width: 1200,
                    height: 780,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        function Operation(val, row, index) {

            var buttons = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="View(\'' + row.ID + '\')"" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">报关日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="required:true,editable:false,missingMessage:'请选择报关开始时间'" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="required:true,editable:false,missingMessage:'请选择报关结束时间'" />
                    <span class="lbl">业务员: </span>
                    <input class="easyui-combobox" id="ServiceManager" data-options="valueField:'Key',textField:'Value',limitToList:true,editable:false,missingMessage:'请选择业务员'" name="ServiceManager" />
                    <span class="lbl">部门: </span>
                    <input class="easyui-combobox" id="DepartmentType" data-options="valueField:'Key',textField:'Value',limitToList:true,editable:false," name="DepartmentType" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="利润提成列表">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'center'" style="width: 50px">业务员</th>
                    <th field="DepartmentCode" data-options="align:'center'" style="width: 50px">部门</th>
                    <th field="Profits" data-options="align:'center'" style="width: 50px">利润</th>
                    <th field="BusinessCommission" data-options="align:'center'" style="width: 50px">提成</th>
                    <th data-options="field:'btnOpt',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
