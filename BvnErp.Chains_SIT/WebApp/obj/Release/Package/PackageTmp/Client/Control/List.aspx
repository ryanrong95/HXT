<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Control.List" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>会员列表-风控</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--  <script>
        gvSettings.fatherMenu = '风控(XDT)';
        gvSettings.menu = '会员列表';
        gvSettings.summary = '科目明细报表';
    </script>--%>
    <script type="text/javascript">
        var RealName = '<%=this.Model.RealName%>';
        //数据初始化
        $(function () {
            //下拉框数据初始化
            var status = eval('(<%=this.Model.Status%>)');
            var Ranks = eval('(<%=this.Model.ClientRanks%>)');

            $('#Status').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: status
            });

            //下拉框数据初始化
            $('#Rank').combobox({
                data: Ranks
            });
            //订单列表初始化
            $('#clients').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {

            var buttons = "";
            if (row.StatusValue ==<%=Needs.Ccs.Services.Enums.ClientStatus.Confirmed.GetHashCode()%>) {
                buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

            }
            else {
                //if ((row.DepartmentCode == '业务二部' && RealName == '张令金') || (row.DepartmentCode == '业务一部' && RealName == '张庆永') || RealName == '风控') {
                buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">审核</span>' +
                    '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                buttons += '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditRank(\'' + row.ID + '\',\'' + row.ClientRankValue + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">修改会员等级</span>' +
                    '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                //}

            };

            return buttons;
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, './Edit.aspx')
                + '?ID=' + id + '&Source=List';
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '审核',
                width: 1000,
                height: 780,
                onClose: function () {
                    Search();
                }
            });
        }

        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var Status = $('#Status').combobox('getValue');
            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Status: Status
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
        //整行关闭一系列弹框
        function NormalClose() {
            $('#rank-dialog').window('close');
            $.myWindow.close();

        }
        //修改会员等级
        function EditRank(id, value) {
            $("#rank-tip").show();
            $('#Rank').textbox('textbox').validatebox('options').required = true;
            $('#Rank').combobox('setValue', value);
            $('#rank-dialog').dialog({
                title: '修改会员等级',
                width: 450,
                height: 350,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '保存',
                    width: 70,
                    handler: function () {
                        if (!Valid('form1')) {
                            return;
                        }
                        MaskUtil.mask();
                        var rank = $("#Rank").textbox('getValue');
                        rank = rank.trim();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=EditRank', {
                            ID: id,
                            Rank: rank,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    //修改成功后刷新datagrid
                                    $("#clients").myDatagrid("reload");
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        NormalClose();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#rank-dialog').window('close');
                    }
                }],
            });
            $('#rank-dialog').window('center');
        }


    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
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
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="clients" data-options="singleSelect:true,border:false,fit:true,nowrap:false,scrollbarSize:0" title="会员列表" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 25%;">公司名称</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 10%;">会员编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 6%;">会员等级</th>
                    <th data-options="field:'ContactName',align:'center'" style="width: 6%;">联系人</th>
                    <th data-options="field:'ContactTel',align:'center'" style="width: 9%;">手机号码</th>
                    <%--<th data-options="field:'ServiceManagerName',align:'center'" style="width: 10%;">业务员</th>--%>
                    <th data-options="field:'ServiceManagerName',align:'center'" style="width: 10%;">报关业务员</th>
                    <th data-options="field:'DepartmentCode',align:'center'" style="width: 10%;">业务员部门</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 7%;">注册时间</th>
                    <th data-options="field:'Status',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="rank-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false" style="overflow: hidden">
        <form id="form1">
            <div id="rank-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                <div style="display: inline-block">
                    <label>会员等级：</label>
                </div>
                <div style="margin-top: 3px; display: inline-block">
                    <input class="easyui-combobox" style="width: 200px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" id="Rank" name="Rank" />
                </div>
            </div>
        </form>
    </div>
</body>
</html>
