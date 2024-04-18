<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnCutList.aspx.cs" Inherits="WebApp.Logistics.ManifestVoyage.UnCutList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
  <%--  <script>
        gvSettings.fatherMenu = '物流管理';
        gvSettings.menu = '运输批次(未截单)';
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

        //新增
        function Add() {
            var url = location.pathname.replace(/UnCutList.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '520px',
                height: '410px',
                title: '新增',
                onClose: function () {
                    $('#voyageGrid').myDatagrid('reload');
                }
            });
        }


          //编辑
        function Edit(id) {
            var url = location.pathname.replace(/UnCutList.aspx/ig, 'Edit.aspx') + "?ID=" + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '520px',
                height: '410px',
                title: '编辑',
                onClose: function () {
                    $('#voyageGrid').myDatagrid('reload');
                }
            });
        }

        //截单
        function SureCut(id) {
            $.messager.confirm('确认', '请您再次确认是否截单。运输批次号：' + id, function (success) {
                if (success) {
                    $.post('?action=SureCut', { ID: id }, function () {
                        $.messager.alert('消息', '操作成功！');
                        $('#voyageGrid').myDatagrid('reload');
                    })
                }
            });
        }

        function Operation(val, row, index) {
           
          var  buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';

            buttons += '<a id="btnCut" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="SureCut(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">截单</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
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
        <table id="voyageGrid" class="easyui-datagrid" title="运输批次(未截单)" data-options="
            border: false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
       queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th field="VoyageNo" data-options="align:'center'" style="width: 50px">货物运输批次号</th>
                    <%--<th field="Type" data-options="align:'center'" style="width: 50px">运输类型</th>--%>
                    <th field="Carrier" data-options="align:'center'" style="width: 50px">承运商</th>
                    <th field="HKLicense" data-options="align:'center'" style="width: 50px">车牌号</th>
                    <th field="TransportTime" data-options="align:'center'" style="width: 50px">运输时间</th>
                    <th field="DriverName" data-options="align:'center'" style="width: 50px">驾驶员姓名</th>
                    <th field="VoyageType" data-options="align:'center'" style="width: 20px">运输类型</th>
                    <th field="CreateTime" data-options="align:'center'" style="width: 50px">创建时间</th>
                    <th data-options="field:'btnOpt',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
