<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.MyClients.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '我的客户';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        var admin = eval('(<%=this.Model.CurrentAdmin%>)');
        var ClientOwner = eval('(<%=this.Model.ClientOwner%>)');
        var clientBrand = eval('(<%=this.Model.Manufacture%>)');
        var clientIndustry = eval(<%=this.Model.DrpCategory %>);
        var ReIndustry = eval(<%=this.Model.ReIndustry %>);
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var StatusData = eval('(<%=this.Model.StatusData%>)');
        var importantlevel = eval('(<%=this.Model.ImportantLevelData%>)');
        //根据角色控制按钮是否开放
        var power = admin.JobType == 100 || admin.JobType == 200 || admin.JobType == 400 || admin.JobType == 500;

        $(function () {

            //解决特殊字符的问题
            $("#ClientBrand").combobox({
                onChange: function (newValue, oldValue) {
                    var text = escape2Html($(this).combobox('getText'));
                    $(this).combobox('setText', text);
                },
            });

          
            if (power) {
                $("#btnAdd").show();
            }
            else {
                $("#btnAdd").hide();
            }
            //if (admin.JobType == 100 || admin.JobType == 500 || admin.JobType == 800 ) {
            //    $("#btnAdd").hide();
            //    $("#btnFileUpload").show();
            //    $("#btnFileDownload").show();
            //    if (admin.JobType == 100 || admin.JobType == 500) {
            //        $("#btnAdd").show();
            //    }
            //}
            //else {
            //    $("#btnAdd").hide();
            //    $("#btnFileUpload").hide();
            //    $("#btnFileDownload").hide();
            //}

            $('#datagrid').bvgrid({
                pageSize: 20,
                rowStyler: function (index, row) {
                    if (row.IsWarning) {
                        return 'background-color:#FF0000;color:red;';
                    }
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
            //        $('#datagrid').bvgrid('reload');
            //    },
            //});

            $("#ClientIndustry").combotree('tree').tree("collapseAll");
        });

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=";
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '客户编辑',
                width: '850px',
                height: '580px',
                onClose: function () {
                    CloseLoad();
                }
            }).open();

        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '客户编辑',
                    width: '850px',
                    height: '580px',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //客户信息详情
        function Show(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Show.aspx') + "?ID=" + rowdata.ID + "&Status=" + rowdata.Status;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '客户详情',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //Tab页
        function Detail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Tabs.aspx') + "?ClientID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: ' ',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //客户加入公海
        function Allote(ID) {
            $.messager.confirm('确认', '请您再次确认是否分配该客户为公海客户！', function (success) {
                if (success) {
                    $.post('?action=Allote', { ID: ID }, function () {
                        $.messager.alert('分配', '分配成功！');
                        CloseLoad();
                    })
                }
            });
        }

        //弹出分配页面
        function Distribute(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            $.post('?action=GetAdmin', function (result) {
                var admins = JSON.parse(result);
                $("#SalesMan").combobox({
                    data: admins,
                });
                var sales = [];
                //debugger;
                $.map(rowdata.AdminID.split(','), function (value) {
                    var index = $.easyui.indexOfArray(admins, "ID", value);
                    if (index >= 0) {
                        sales.push(value);
                    }
                })
                $("#SalesMan").combobox("setValues", sales)
                window.bindingSaleID = sales.join(',');
                window.publicClientID = rowdata.ID;
                win.window('open');
                //校验填入数据是否是数据源中
                $("#SalesMan").combobox("textbox").bind("blur", function () {
                    var values = [];
                    $.map($("#SalesMan").combobox("getValues"), function (value) {
                        var data = $("#SalesMan").combobox("getData");
                        var valuefiled = $("#SalesMan").combobox("options").valueField;
                        var index = $.easyui.indexOfArray(data, valuefiled, value);
                        if (index >= 0) {
                            values.push(value);
                        }
                    });
                    $("#SalesMan").combobox("setValues", values);
                });
            });

        }

        //分配
        function apply() {
            var saleman = $("#SalesMan").combobox('getValues').join(',');
            if (saleman == null || saleman == "") {
                $.messager.alert("提示", "请选择要分配的人员！")
            } else {
                $.post('?action=Distribute', { ID: publicClientID, BindingID: bindingSaleID, Sale: saleman }, function () {
                    $.messager.alert('分配', '分配成功！');
                    win.window('close');
                });
            }
        }

        //重置
        function Reset() {
            $("#table1").form('clear');
            CloseLoad();
        }

        //查看销售机会
        function Project(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/MyClients/ig, 'Project') + "?ClientID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '客户的销售机会列表',
                    width: '95%',
                    height: '95%',
                    noheader: false,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        };

        // 报备信息管理
        function ProjectInfo(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/MyClients/ig, 'ProjectManagement') + "?id=" + rowdata.ID + "&name=" + rowdata.Name;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '销售机会报备管理',
                    width: '95%',
                    height: '95%',
                    noheader: false,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.Status == 500) {
                if (power) {
                    if (row.IsOwn) {
                        buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
                        buttons += '<button id="btnDistrbute" onclick="Distribute(' + index + ')">分配</button>';
                    }
                }
                if (admin.JobType == 800) {
                    buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
                    buttons += '<button id="btnDistrbute" onclick="Distribute(' + index + ')">分配</button>';
                    buttons += '<button id="btnPublic" onclick="Allote(\'' + row.ID + '\')">加入公海</button>';
                    buttons += '<button id="btnProjectInfo" onclick="ProjectInfo('+ index +')">报备信息</button>';
                }
                buttons += '<button id="btnDetail" onclick="Detail(\'' + index + '\')">详情</button>';
                buttons += '<button id="btnDetail" onclick="Project(\'' + index + '\')">销售机会</button>';
            }
            else if (row.Status == 600) {
                if ((power) && row.IsOwn) {
                    buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
                }
                buttons += '<button id="btnShow" onclick="Show(\'' + index + '\')">客户详情</button>';
            }
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var ClientID = $("#ClientID").combobox("getValue");
            var Admincode = $("#AdminCode").val();
            var Owner = $("#ClientOwner").combobox("getValue");
            var importantlevel = $("#ImportantLevel").combobox("getValue");
            var clientBrand = $("#ClientBrand").combobox("getValues").join(',');
            var clientIndustry = $("#ClientIndustry").combotree("getValues").join(',');
            var clientReIndustry = $("#ClientReIndustry").combobox("getValues").join(',');
            var status = $("#Status").combobox("getValue");
            $('#datagrid').bvgrid('flush', {
                ClientID: ClientID, ClientOwner: Owner, ClientBrand: clientBrand,
                ClientIndustry: clientIndustry, AdminCode: Admincode, ClientReIndustry: clientReIndustry,
                Status: status, ImportantLevel: importantlevel,
            });
        }

        //查询
        function Search() {
            var ClientID = $("#ClientID").combobox("getValue");
            var Admincode = $("#AdminCode").val();
            var Owner = $("#ClientOwner").combobox("getValue");
            var importantlevel = $("#ImportantLevel").combobox("getValue");
            var clientBrand = $("#ClientBrand").combobox("getValues").join(',');
            var clientIndustry = $("#ClientIndustry").combotree("getValues").join(',');
            var clientReIndustry = $("#ClientReIndustry").combobox("getValues").join(',');
            var status = $("#Status").combobox("getValue");
            if (clientIndustry.length > 900) {
                $.messager.alert('错误', '行业项目选择过多');
                return;
            };
            $('#datagrid').bvgrid('search', {
                ClientID: ClientID, ClientOwner: Owner, ClientBrand: clientBrand,
                ClientIndustry: clientIndustry, AdminCode: Admincode, ClientReIndustry: clientReIndustry,
                Status: status, ImportantLevel: importantlevel,
            });
        }

        //查询一周内进入公海客户
        function Public() {
            $('#datagrid').bvgrid('search', { IsWarning: "True" });
        }

        //关闭弹出框
        function closeWin() {
            win.window('close');
        }

        //上传客户
        function Upload() {
            winFile.window('open');
        }

        //保存文件
        function FileUpload() {
            $.post('?action=FileUpload', function () {
                $('#datagrid').datagrid('reload');
                winFile.window('close');
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 150px">
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
                <td class="lbl">所有人</td>
                <td>
                    <input class="easyui-combobox" id="ClientOwner" style="width: 95%"
                        data-options="valueField:'ID',textField:'RealName',data: ClientOwner," />
                </td>
                <td class="lbl">代理品牌</td>
                <td>
                    <input class="easyui-combobox" id="ClientBrand" style="width: 95%"
                        data-options="valueField:'ID',textField:'Name',data: clientBrand,multiple:true," />
                </td>
            </tr>
            <tr>
                <td class="lbl">自定义客户编号</td>
                <td>
                    <input class="easyui-textbox" id="AdminCode" name="AdminCode" style="width: 95%" />
                </td>

                <td class="lbl">所属行业</td>
                <td>
                    <input class="easyui-combobox" id="ClientReIndustry" style="width: 95%"
                        data-options="valueField:'ID',textField:'Name',editable:false,data: ReIndustry,multiple:true," />
                </td>
                <td class="lbl">主要产品</td>
                <td>
                    <select id="ClientIndustry" name="ClientIndustry" class="easyui-combotree"
                        data-options="valueField: 'id',textField: 'text',data: clientIndustry,multiple:true" style="width: 95%;">
                    </select>
                </td>
            </tr>
            <tr>
                <td class="lbl">客户状态</td>
                <td>
                    <input class="easyui-combobox" id="Status" style="width: 95%"
                        data-options="valueField:'value',textField:'text',data: StatusData," />
                </td>
                <td>重点客户</td>
                <td>
                    <input class="easyui-combobox" id="ImportantLevel" name="ImportantLevel"
                        data-options="valueField:'value',textField:'text',data: importantlevel," style="width: 95%" />
                </td>
            </tr>
        </table>

        <!--搜索按钮-->
        <table>
            <tr>
                <td>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <%--<a id="btnFileUpload" name="btnFileUpload" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-excel'" onclick="Upload()">导入客户</a>
                    <a id="btnFileDownload" name="btnFileDownload" href="../../UploadFiles/客户导入模板.xlsx" class="easyui-linkbutton" data-options="iconCls:'icon-excel'">下载模板</a>--%>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
                    <a id="btnpublic" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Public()">一周内进入公海</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="我的客户列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 230px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 50px">客户ID</th>
                    <th field="Name" data-options="align:'center'" style="width: 100px">客户名称</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 50px;">状态</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px;">所有人</th>
                    <th field="IsSafeName" data-options="align:'center'" style="width: 50px">是否保护</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px;">创建时间</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 100px;">更新时间</th>
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
                        data-options="valueField:'ID',textField:'RealName',required:true,multiple:true," style="width: 150px" />
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
