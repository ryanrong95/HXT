<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.FundTransferApplies.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调拨查询界面</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '银行账户管理';
        gvSettings.menu = '账户管理';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        var FundTransferApplyStatus = eval('(<%=this.Model.FundTransferApplyStatus%>)');
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true,
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
            //初始化Combobox
            $('#FundTransferApplyStatus').combobox({
                data: FundTransferApplyStatus,
            })
        });

          //查询
        function Search() {
            var FundTransferApplyStatus = $('#FundTransferApplyStatus').combobox('getValue');
            var ApplyNo = $('#ApplyNo').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {
                FundTransferApplyStatus: FundTransferApplyStatus,
                ApplyNo: ApplyNo,
                StartDate: StartDate,
                EndDate : EndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#FundTransferApplyStatus').combobox('setValue', null);
            $('#ApplyNo').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //新增
        function BtnAdd() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增调拨',
                width: '680',
                height: '450',
                url: url,
                onClose: function () {
                    Search();
                }
            });
        }

        //编辑
        function Edit(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + "?ID=" + rowdata.ID+"&PageFunction=View";
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '查看调拨',
                    width: '680',
                    height: '600',
                    url: url,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            if (row.ApplyStatus == "待审批") {
                 buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BtnAdd()">新增</a>
                </li>
            </ul>
        </div>
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">申请编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ApplyNo" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">状态:</td>
                    <td>
                        <input class="easyui-combobox" id="FundTransferApplyStatus" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td> 
                     <td class="lbl">完成时间:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false,height:26,width:200" />
                    </td>
                    <td class="lbl">至:</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false,height:26,width:200" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="调拨列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 80px;">申请编号</th>
                    <th data-options="field:'OutAccountID',align:'left'" style="width: 80px;">调出账户</th>
                    <th data-options="field:'OutAmount',align:'left'" style="width: 80px;">调出金额</th>
                    <th data-options="field:'InAccountID',align:'left'" style="width: 100px;">调入账户</th>
                    <th data-options="field:'InAmount',align:'left'" style="width: 100px;">调入金额</th>
                    <th data-options="field:'Admin',align:'left'" style="width: 130px;">申请人</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">申请时间</th>
                    <th data-options="field:'ApplyStatus',align:'left'" style="width: 100px;">状态</th>                   
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
