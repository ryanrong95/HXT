<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CuttedList.aspx.cs" Inherits="WebApp.Logistics.ManifestVoyage.CuttedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '运输批次(已截单)';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#voyageGrid').myDatagrid({
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

            //设置系统当前时间
            var curr_time = new Date();
            var str = curr_time.getMonth() + 1 + "/";
            str += curr_time.getDate() + "/";
            str += curr_time.getFullYear() + " ";
            str += curr_time.getHours() + ":";
            str += curr_time.getMinutes() + ":";
            str += curr_time.getSeconds();
            $('#EndTime').datebox('setValue', str);
        });

        //查询
        function Search() {
            var VoyageNo = $("#VoyageNo").textbox('getValue');
            $('#voyageGrid').myDatagrid('search', { VoyageNo: VoyageNo });
        }

        //重置
        function Reset() {
            $("#VoyageNo").textbox('setValue', "");

            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1" style="margin: 5px 0">
                <tr>
                    <td class="lbl">货物运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:120" id="VoyageNo" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>

                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="voyageGrid" class="easyui-datagrid" title="已截单" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
       queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="VoyageNo" data-options="align:'center'" style="width: 50px">货物运输批次号</th>
                    <th field="Type" data-options="align:'center'" style="width: 50px">运输类型</th>
                    <th field="Carrier" data-options="align:'center'" style="width: 50px">承运商</th>
                    <th field="HKLicense" data-options="align:'center'" style="width: 50px">车牌号</th>
                    <th field="DriverName" data-options="align:'center'" style="width: 50px">驾驶员姓名</th>
                    <th field="CreateTime" data-options="align:'center'" style="width: 50px">创建时间</th>
                    <th field="CutStatus" data-options="align:'center'" style="width: 50px">截单状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
