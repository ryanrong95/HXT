﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllList.aspx.cs" Inherits="WebApp.Client.New.AllList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>会员列表</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%-- <script>
        gvSettings.fatherMenu = '会员管理(XDT)';
        gvSettings.menu = '我的会员';
        gvSettings.summary = '业务员管理会员的菜单';
    </script>--%>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            //下拉框数据初始化
            var status = eval('(<%=this.Model.Status%>)');
            $('#Status').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: status
            });

            $('#Status').combobox("setValue", 2);
            //$('#Status').combobox('disable');
            //订单列表初始化
            $('#clients').myDatagrid({
                queryParams: {
                    Status: 2
                },
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },

            });

            $("#All").click(function () {
                if ($(this).is(":checked")) {
                    $("#Normal").prop("checked", false);
                    $("#AbNormal").prop("checked", false);
                    Search();
                }
            });
            $("#Normal").click(function () {
                if ($(this).is(":checked")) {
                    $("#All").prop("checked", false);
                    $("#AbNormal").prop("checked", false);
                    Search();
                }
            });
            $("#AbNormal").click(function () {
                if ($(this).is(":checked")) {
                    $("#All").prop("checked", false);
                    $("#Normal").prop("checked", false);
                    Search();
                }
            });
          
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            var NormalOperation = "标为异常";
            if (row.isNormal == false) {
                NormalOperation = "标为正常";
            }

            //if (row.statusValue != '<%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%>' && row.statusValue !='<%=Needs.Ccs.Services.Enums.ClientStatus.Returned.GetHashCode() %>') {
            buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            //} 

            buttons += '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetControl(\'' + row.ID + '\',\'' + row.IsDownloadDecTax + '\',\'' + row.DecTaxExtendDate + '\',\'' + row.IsApplyInvoice + '\',\'' + row.InvoiceExtendDate + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">管控设置</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetNormal(\'' + row.ID + '\',\'' + row.isNormal + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">' + NormalOperation + '</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }


        //提交
        function Submit(id, code) {
            //客户编号为空不允许提交
            if (code == "null" | code == '') {
                $.messager.alert("消息", "请先保存客户编号");
                return;
            }
            $.messager.confirm('确认', '请您再次确认否提交', function (success) {
                if (success) {
                    $.post('?action=Submit', { ID: id }, function (res) {
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, "info", function () {
                            if (result.success) {
                                $('#clients').myDatagrid('reload');
                            }
                        });


                    })
                }
            });
        }
        //详情
        function View(id) {
            if (id) {
                var url = location.pathname.replace(/New\/AllList.aspx/ig, 'Risk.aspx') + '?Source=RiskView&ID=' + id;
                window.location = url;
            }
        }


        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var Status = $('#Status').combobox('getValue');
            var NormalType = 2;
            if ($('#Normal').is(':checked')) {
                NormalType = 1;
            }
            if ($('#AbNormal').is(':checked')) {
                NormalType = 0;
            }
            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Status: Status,
                NormalType: NormalType
            };
            $('#clients').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#Status').combobox('setValue', null);
            Search();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/New\/List.aspx/ig, 'Index.aspx');
            window.location = url + '?Source=Add';
        }

        function ViewSummary(val, row, index) {
            var status = parseInt(row.statusValue);
            if (status ==<%=Needs.Ccs.Services.Enums.ClientStatus.Returned.GetHashCode()%>) {
                return '<a href="javascript:void(0);" onclick="Reason(\'' + row.Summary + '\')" >' + val + '</a>'

            } else {
                return val;
            }
        }

        function ViewNormal(val, row, index) {
            if (row.isNormal == true) {
                return '正常';

            } else {
                return '异常';
            }
        }

        function ViewDecTax(val, row, index) {
            if (row.IsDownloadDecTax == true) {
                return '否';

            } else {
                return '<label style="color:red;">是</label>';
            }
        }

        function ViewInvoice(val, row, index) {
            if (row.IsApplyInvoice == true) {
                return '否';

            } else {
                return '<label style="color:red;">是</label>';
            }
        }

        function Reason(Summary) {
            $.messager.alert("退回原因", Summary);
        }

        function SetNormal(ClientID, IsNormal) {
            var para = {
                ClientID: ClientID,
                IsNormal: IsNormal,
            };
            var msgAlert = "正常";
            if (IsNormal == "true") {
                msgAlert = "异常";
            }

            $.messager.confirm('确认', '请您再次确认是否把该客户设置成 ' + msgAlert, function (success) {
                if (success) {
                    $.post('?action=SetNormal', para, function (res) {
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, "info", function () {
                            if (result.success) {
                                Search();
                            }
                        });
                    })
                }
            });
        }

        //设置管控
        function SetControl(ClientID, IsDownloadDecTax, DecTaxExtendDate, IsApplyInvoice, InvoiceExtendDate) {

            var url = location.pathname.replace(/AllList.aspx/ig, 'SetControl.aspx?ClientID=' + ClientID + '&IsDownloadDecTax=' + IsDownloadDecTax + '&DecTaxExtendDate=' + DecTaxExtendDate + '&IsApplyInvoice=' + IsApplyInvoice + '&InvoiceExtendDate=' + InvoiceExtendDate);

            $.myWindow.setMyWindow("AlllistToSet", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '管控设置',
                width: '800px',
                height: '600px',
                onClose: function () {
                    Search();
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <%--<a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>--%>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户名称:</span>
                    <input class="easyui-textbox search" id="CompanyName" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox search" id="Status" />
                    <br />
                    <span class="lbl">创建日期:</span>
                    <input type="text" class="easyui-datebox search" id="CreateDateFrom" />
                    <span class="lbl">至: </span>
                    <input type="text" class="easyui-datebox search" id="CreateDateTo" />
                    <input type="checkbox" name="Order" value="2" id="All" title="全部客户" class="checkbox checkboxlist" checked="checked" /><label for="All" style="margin-right: 20px">全部客户</label>
                    <input type="checkbox" name="Order" value="1" id="Normal" title="正常客户" class="checkbox checkboxlist" /><label for="Normal" style="margin-right: 20px">正常客户</label>
                    <input type="checkbox" name="Order" value="0" id="AbNormal" title="异常客户" class="checkbox checkboxlist" /><label for="AbNormal">异常客户</label>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="clients" data-options="singleSelect:true,fit:true,border:false,nowrap:false,scrollbarSize:0,rownumbers:true," title="会员列表" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 13%;">客户名称</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 5%;">信用等级</th>
                    <th data-options="field:'CompanyCode',align:'center'" style="width: 10%;">统一社会信用代码</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 5%;">创建日期</th>
                    <th data-options="field:'SalesName',align:'center'" style="width: 5%;">业务员</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 5%;">跟单员</th>
                    <th data-options="field:'ClientStatus',align:'center'" style="width: 5%;">状态</th>
                    <th data-options="field:'IsDownloadDecTax',align:'center',formatter:ViewDecTax" style="width: 5%;">限制海关票</th>
                    <th data-options="field:'DecTaxExtendDate',align:'center'" style="width: 7%;">海关票宽限日期</th>
                    <th data-options="field:'IsApplyInvoice',align:'center',formatter:ViewInvoice" style="width: 5%;">限制开票</th>
                    <th data-options="field:'InvoiceExtendDate',align:'center'" style="width: 7%;">开票宽限日期</th>
                    <th data-options="field:'IsNormal',align:'center',formatter:ViewNormal" style="width: 5%;">是否异常</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

   
</body>
</html>
