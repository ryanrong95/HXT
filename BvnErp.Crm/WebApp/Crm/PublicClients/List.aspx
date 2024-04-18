<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.PublicClients.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '公海客户';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">  
        window.admintype = eval(<%=this.Model.AdminType %>);
        var ClientData = eval('(<%=this.Model.ClientData%>)');

        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
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

            win = $("#win").window({
                collapsible: false,
                minimizable: false,
                maximizable: false,
                closed: true,
                onClose: function () {
                    CloseLoad();
                },
            });

            //winFile = $("#winFile").window({
            //    collapsible: false,
            //    minimizable: false,
            //    maximizable: false,
            //    closed: true,
            //    onclose: function () {
            //        CloseLoad();
            //    },
            //});
        });

        //查看
        function Show(id) {
            var url = location.pathname.replace(/List.aspx/ig, '../PublicClients/Show.aspx') + "?ID=" + id;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '公海客户',
                width: '800px',
                height: '180px',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //认领
        //function Protect(ID) {
        //    $.messager.confirm('确认', '请您再次确认是否保护该公司！', function (success) {
        //        if (success) {
        //            $.post('?action=Protect', { ID: ID }, function () {
        //                CloseLoad();
        //            })
        //        }
        //    });
        //}

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //分配页面弹出
        function Distribute(id) {
            $.post('?action=GetAdmin', function (result) {
                var admins = JSON.parse(result);
                $("#SalesMan").combobox({
                    data: admins
                });
                window.publicClientID = id;
                win.window('open');
            });
        }

        //分配销售人员
        function apply() {
            var saleman = $("#SalesMan").combobox('getValue');
            if (saleman == null || saleman == "") {
                $.messager.alert("提示", "请选择要分配的人员！")
            } else {
                $.post('?action=Distribute', { ID: publicClientID, Sale: saleman }, function () {
                    $.messager.alert('分配', '分配成功！');
                    CloseLoad();
                    win.window('close');
                });
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnDelete" onclick="Show(\'' + row.ID + '\')">查看</button>';
            // weipan@20190510  领导要求
            //if (admintype == 100 || admintype == 500) {
            //    buttons += '<button id="btnDelete" onclick="Protect(\'' + row.ID + '\')">认领</button>';
            //}
            if (admintype == 800) {
                buttons += '<button id="btnDelete" onclick="Distribute(\'' + row.ID + '\')">分配</button>';
            }
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var ClientID = $("#ClientID").val();
            $('#datagrid').bvgrid('flush', { ClientID: ClientID });
        }

        //查询
        function Search() {
            var ClientID = $("#ClientID").combobox("getValue");
            $('#datagrid').bvgrid('search', { ClientID: ClientID });
        }

        //function Upload() {
        //    winFile.window('open');
        //}

        ////关闭窗口
        //function closeWin() {
        //    win.window('close');
        //}
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
                <th style="width: 30%"></th>
            </tr>
            <tr>
                <td class="lbl">客户名称</td>
                <td>
                    <input class="easyui-combobox" id="ClientID" name="ClientID"
                        data-options="valueField:'ID',textField:'Name',data: ClientData" style="width: 95%" />
                </td>
            </tr>
        </table>
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
                    <%-- weipan@20190510  领导要求--%>
                    <%-- <a id="btnFileUpload" href="javascript:void(0);" class="easyui-linkbutton" onclick="Upload()" data-options="iconCls:'icon-excel'">导入客户</a>
                    <a id="btnFileDownload" href="../../UploadFiles/客户导入模板.xlsx" class="easyui-linkbutton" data-options="iconCls:'icon-excel'">下载模板</a>--%>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="我的客户列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Name" data-options="align:'center'" style="width: 100px">客户名称</th>
                    <th field="IsSafeName" data-options="align:'center'" style="width: 50px">是否保护</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px;">状态</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px;">创建时间</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 100px;">更新时间</th>
                    <th field="Summary" data-options="align:'center'" style="width: 200px;">描述</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="win" class="easyui-window" title="分配页面" style="width: 430px; height: 160px">
        <table id="table2" style="width: 400px; text-align: center">
            <tr style="height: 20px">
                <td colspan="5"></td>
            </tr>
            <tr>
                <td style="width: 100px"></td>
                <td style="text-align: right">销售
                </td>
                <td style="width: 5px"></td>
                <td colspan="2" style="text-align: left">
                    <input class="easyui-combobox" id="SalesMan" name="SalesMan"
                        data-options="valueField:'ID',textField:'RealName',required:true" style="width: 150px" />
                </td>
            </tr>
            <tr style="height: 20px">
                <td colspan="5"></td>
            </tr>
            <tr>
                <td colspan="5">
                    <button id="btnDistrbute" onclick="apply()" style="text-align: center">确定</button>
                    <button id="btnCancel" onclick="closeWin()" style="text-align: center">取消</button>
                </td>
            </tr>
        </table>
    </div>

    <%--<div id="winFile" class="easyui-window" title="上传客户" style="width: 430px; height: 160px">
        <form id="importFileForm" method="post" enctype="multipart/form-data" runat="server">
            <table id="table3" style="width: 400px; text-align: center">
                <tr style="height: 20px">
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td style="width: 120px"></td>
                    <td style="text-align: right"></td>
                    <td colspan="3" style="text-align: left">

                        <input type="file" name="MyFileUploadInput" runat="server" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel" />
                    </td>
                </tr>
                <tr style="height: 20px">
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td colspan="5">
                        <asp:Button ID="InputFileUploadButton" runat="server" Text="上传" OnClick="InputFileUploadButton_Click" />
                    </td>
                </tr>
            </table>
        </form>
    </div>--%>

</body>
</html>
