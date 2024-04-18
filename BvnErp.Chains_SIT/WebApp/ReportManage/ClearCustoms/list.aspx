<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="list.aspx.cs" Inherits="WebApp.ReportManage.ClearCustoms.list" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>清关数据</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '报表管理(XDT)';
        gvSettings.menu = '每日报关数据';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                //queryParams: { StartTime: GetToday(), EndTime: GetToday(), action: "data" },
                nowrap: false,
                pageSize: 20,
                pageList: [20, 50, 100, 150, 200, 500]
            });

            //$('#StartTime').datebox('setValue', GetToday());
            //$('#EndTime').datebox('setValue', GetToday());
        });

        //查询
        function Search() {
            //var StartTime = $('#StartTime').datebox('getValue');
            //var EndTime = $('#EndTime').datebox('getValue');         
            var VoyNo = $('#VoyNo').textbox('getValue');
            if (VoyNo == "") {
                 $.messager.alert('提示', '请输入运输批次号');
                return;
            }

            var parm = {
                //StartTime: StartTime,
                //EndTime: EndTime,            
                VoyNo: VoyNo,             
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {          
            //$('#StartTime').datebox('setValue', null);
            //$('#EndTime').datebox('setValue', null);        
            $('#VoyNo').textbox('setValue', null);
        
            Search();
        }

        //导出Excel
        function Export() {          
            //var StartTime = $('#StartTime').datebox('getValue');
            //var EndTime = $('#EndTime').datebox('getValue');        
            var VoyNo = $('#VoyNo').textbox('getValue');
              if (VoyNo == "") {
                 $.messager.alert('提示', '请输入运输批次号');
                return;
            }
                   
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                //StartTime: StartTime,
                //EndTime: EndTime,            
                VoyNo: VoyNo,          
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        if (rel.url.length > 1) {
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
                    }
                });
            })
        }

        function GetToday() {
            var mydate = new Date();
            var str = "" + mydate.getFullYear() + "-";
            str += (mydate.getMonth() + 1) + "-";
            str += mydate.getDate();
            return str;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>                    
                    <td class="lbl">运输批次号: </td>
                    <td>
                        <input class="easyui-textbox search" id="VoyNo" data-options="height:26,width:150" />
                    </td>
                   <%-- <td class="lbl">报关日期: </td>
                    <td colspan="3">
                        <input class="easyui-datebox search" id="StartTime" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox search" id="EndTime" data-options="height:26,width:150" />
                    </td>--%>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>                   
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="报关产品" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>                  
                    <th data-options="field:'GName',align:'left',width:160">品名</th>
                    <th data-options="field:'HSCode',align:'center',width:100">海关编码</th> 
                    <th data-options="field:'UnitName',align:'center',width:100">单位</th>
                    <th data-options="field:'UnitShow',align:'center',width:100">单位码</th>
                    <th data-options="field:'FirstQty',align:'center',width:80">法一数量</th>
                    <th data-options="field:'Qty',align:'center',width:80">数量</th>
                    <th data-options="field:'DecTotal',align:'center',width:100">金额</th>
                    <th data-options="field:'Currency',align:'center',width:80">币制</th>                    
                    <th data-options="field:'NetWt',align:'center',width:80">净重</th>                   
                </tr>
            </thead>            
        </table>
    </div>
</body>
</html>