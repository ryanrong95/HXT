<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnSwapList.aspx.cs" Inherits="WebApp.Finance.UnSwapList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待换汇</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%-- <script>
        gvSettings.fatherMenu = '换汇通知';
        gvSettings.menu = '待换汇';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var BankData = eval('(<%=this.Model.BankData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                rownumbers: true,
                singleSelect: true,
                toolbar: '#topBar'
            });

            $("#Bank").combobox({
                data: BankData,
                valueField: 'value',
                textField: 'text',
            })
        });

        //查询
        function Search() {
            var BankName = $('#Bank').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                BankName: BankName,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Bank').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //导出
        function Export(id) {
            MaskUtil.mask();
            //验证成功
            $.post('?action=ExportSwapFiles', {
                ID: id,
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

        //确认换汇
        function SwapConfirm(id) {
            var url = location.pathname.replace(/UnSwapList.aspx/ig, 'Complete.aspx') + "?ID=" + id;
            window.location = url;
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否取消换汇通知！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function (res) {
                        var result = JSON.parse(res);
                        $('#datagrid').myDatagrid('reload');
                        $.messager.alert('消息', result.message, "info", function (r) {
                        });
                    })
                }
            });
        }

        //编辑该次换汇中的信息（包括银行、增加删除报关单以及编辑金额）
        function EditInfo(id) {
            var url = location.pathname.replace(/UnSwapList.aspx/ig, 'EditInfo.aspx') + "?ID=" + id;
            window.location = url;
        }

        function FXButton(ID, TotalAmount, uid) {      
            if (TotalAmount >= 1000) {
                 var url = location.pathname.replace(/UnSwapList.aspx/ig, 'FXList.aspx') + '?SwapNoticeID=' + ID+'&txnAmount='+TotalAmount+'&UID='+uid;
                    // window.location = url;
                    top.$.myWindow({
                        iconCls: "",
                        url: url,
                        noheader: false,
                        title: '锁汇购汇',
                        width: '50%',
                        height: '80%'
                    });

            } else {
                $.messager.alert('提示', '换汇金额必须大于等于1000美元!');
            }           
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Export(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">导出文件</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';

            if (row.BankName == '星展银行') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="FXButton(\'' + row.ID + '\',\'' + row.TotalAmount + '\',\'' + row.uid + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">锁汇换汇</span>' +
                    '<span class="l-btn-icon icon-lock">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            if (row.SwapStatusInt == '<%=Needs.Ccs.Services.Enums.SwapStatus.Auditing.GetHashCode()%>') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SwapConfirm(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">完成换汇</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            //buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditInfo(\'' + row.ID + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">编辑</span>' +
            //    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';


            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">取消</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">换汇银行: </td>
                    <td>
                        <input class="easyui-combobox" id="Bank" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">申请日期: </td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:150,editable:false" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:150,editable:false" />
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
        <table id="datagrid" title="换汇通知-待换汇" data-options="
            fitColumns:true,
            fit:true,
            rownumbers:true,
            singleSelect:true,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'Creator',align:'center'" style="width: 60px">申请人</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 60px">币种</th>
                    <th data-options="field:'TotalAmount',align:'center'" style="width: 60px">换汇金额</th>
                    <th data-options="field:'BankName',align:'center'" style="width: 60px">换汇银行</th>
                    <th data-options="field:'ConsignorCode',align:'center'" style="width: 80px">境外发货人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 60px">申请日期</th>
                    <th data-options="field:'SwapStatus',align:'center'" style="width: 60px">换汇状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 160px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
