<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QualifiedList.aspx.cs" Inherits="WebApp.Client.Control.Qualified" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>合格会员列表</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var CurrentName = '<%=this.Model.CurrentName%>';
        var ClientID = "";

        //数据初始化
        $(function () {
            //下拉框数据初始化
            var serviceManager = eval('(<%=this.Model.ServiceManager%>)');
            var DepartmentType = eval('(<%=this.Model.DepartmentType%>)');


            //业务员
            $('#ServiceManager').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: serviceManager
            });

            //业务员部门
            $('#DepartmentType').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: DepartmentType
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
                },
            });

            //评估时间弹框, 一些设置
            $('#assessDate-dialog').dialog({
                buttons: [
                    {
                        text: '保存',
                        width: '52px',
                        handler: function () {
                            MaskUtil.mask();
                            $.post('?action=SaveAssessDate', {
                                ClientID: ClientID,
                                AssessDate: $('#tbAssessDate').datebox('getValue')
                            }, function (res) {
                                MaskUtil.unmask();
                                var result = JSON.parse(res);
                                if (result.success) {
                                    $.messager.alert('', result.message, 'info', function () {
                                        Search();
                                        $('#assessDate-dialog').dialog('close');
                                    });
                                } else {
                                    $.messager.alert('提示', result.message);
                                }
                            });
                        }
                    },
                    {
                        text: '关闭',
                        width: '52px',
                        handler: function () {
                            $('#assessDate-dialog').dialog('close');
                        }
                    }
                ]
            });
        });



        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Remove(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">移除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a id="btnAssess" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Assess(\'' + row.ID + '\',\'' + row.AssessDate + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">评估日期</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }


        function Add() {
            var url = location.pathname.replace(/QualifiedList.aspx/ig, 'AddQualified.aspx');
            top.$.myWindow({
                iconCls: "icon-man",
                url: url,
                noheader: false,
                title: '添加合格客户',
                width: '700px',
                height: '500px',
                onClose: function () {
                    $('#clients').datagrid('reload');
                }
            });
        }


        //详情
        function View(id) {
            if (id) {
                var url = location.pathname.replace(/Control\/QualifiedList.aspx/ig, 'Index.aspx') + '?Source=QualifiedView&ID=' + id;
                window.location = url;
            }
        }

        //移除
        function Remove(id) {
            $.messager.confirm('确认', '请您再次确认是否移除该合格客户！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '移除成功成功！');
                        $('#clients').myDatagrid('reload');
                    })
                }
            });
        }

        //修改评估日期
        function Assess(id, assessdate) {
            ClientID = id;
            $('#tbAssessDate').datebox('setValue', assessdate);
            $('#assessDate-dialog').dialog('open');
        }

        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var servicemanager = $("#ServiceManager").combobox('getValue');
            var DepartmentType = $("#DepartmentType").combobox('getValue');

            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Servicemanager: servicemanager,
                DepartmentType: DepartmentType,
            };
            $('#clients').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#ServiceManager').combobox('setValue', null);
            $('#DepartmentType').combobox('setValue', null);
            //$('#Unclaim').attr('checked', false);
            //$('#HasReturn').attr('checked', false);
            //$('#IsSAEleUpload').attr('checked', false);
            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户名称:</span>
                    <input class="easyui-textbox search" id="CompanyName" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">业务员:</span>
                    <input class="easyui-combobox search" id="ServiceManager" />
                    <br />
                    <span class="lbl">创建日期:</span>
                    <input type="text" class="easyui-datebox search" id="CreateDateFrom" />
                    <span class="lbl">至: </span>
                    <input type="text" class="easyui-datebox search" id="CreateDateTo" />

                    <span class="lbl">业务员部门:</span>
                    <input class="easyui-combobox search" id="DepartmentType" />

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
                    <th data-options="field:'CompanyName',align:'left'" style="width: 15%;">公司名称</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 4%;">会员编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 4%;">会员等级</th>
                    <th data-options="field:'ContactName',align:'center'" style="width: 6%;">联系人</th>
                    <th data-options="field:'ContactTel',align:'center'" style="width: 6%;">手机号码</th>
                    <th data-options="field:'DepartmentCode',align:'center'" style="width: 6%;">业务员部门</th>
                    <th data-options="field:'ServiceManagerName',align:'center'" style="width: 6%;">业务员</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 6%;">跟单员</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 6%;">风控审批时间</th>
                    <th data-options="field:'AssessDate',align:'center',sortable:true" style="width: 6%;">评估日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <!-----------------------------------------------------------编辑评估日期------------------------------------------------------------------------------------>
    <div id="assessDate-dialog" class="easyui-dialog" title="评估日期" style="width: 550px; height: 350px;"
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <div style="padding:20px 0px 0px 20px">
            <span class="lbl">评估日期:</span>
            <input type="text" class="easyui-datebox search" id="tbAssessDate" />
        </div>
    </div>

</body>
</html>
