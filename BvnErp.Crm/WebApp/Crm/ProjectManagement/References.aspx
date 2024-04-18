<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="References.aspx.cs" Inherits="WebApp.Crm.ProjectManagement.References" %>


<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品参考价查询</title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '产品参考价查询';
        gvSettings.summary = '';

    </script>
</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 100px">
        <table id="table1" style="margin-top: 10px; width: 100%">
             <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr>
                <td class="lbl">型号名称</td>
                <td>
                    <input class="easyui-textbox" id="s_name" data-options="prompt:'型号名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">品牌名称</td>
                <td>
                    <input class="easyui-textbox" id="s_manufacturer" data-options="prompt:'品牌名称',validType:'length[0,150]'" />
                </td>
            </tr>
        </table>

        <!--搜索按钮-->
        <table>
            <tr>
                <td>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'">查询</a>
                    <a id="btnClear" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="型号列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'center'", style="width:100px;">型号</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width:100px;">品牌</th>
                    <th data-options="field:'MOQ',align:'center'" style="width:100px;">最小起订量（MOQ）</th>
                    <th data-options="field:'MPQ',align:'center'" style="width:100px;">最小包装量（MPQ）</th>
                    <th data-options="field:'Validity',align:'center'" style="width:100px;">有效时间</th>
                    <th data-options="field:'ValidityCount',align:'center'"style="width:80px;">有效数量</th>
                    <th data-options="field:'CurrencyDes',align:'center'" style="width:80px;">币种</th>
                    <th data-options="field:'SalePrice',align:'center'" style="width:80px;">参考售价</th>
                    <th data-options="field:'Summary',align:'center'" style="width:200px;">备注</th>
                </tr>
            </thead>
        </table>
    </div>

    <script>
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                nowrap: false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                $('#datagrid').bvgrid('search', { s_name: $('#s_name').val(), s_manufacturer: $('#s_manufacturer').val() });
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $("#table1").form('clear');
                $('#datagrid').bvgrid('search', { s_name: $('#s_name').val(), s_manufacturer: $('#s_manufacturer').val() });
                return false;
            });

        });
    </script>
</body>
</html>
