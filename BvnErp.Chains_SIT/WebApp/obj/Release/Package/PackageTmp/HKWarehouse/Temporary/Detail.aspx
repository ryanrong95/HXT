<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.HKWarehouse.Temporary.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑暂存</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var Temporary = eval('(<%=this.Model.Temporary%>)');
        var WrapTypeData = eval('(<%=this.Model.WrapTypeData%>)');

        var ID = getQueryString('ID');

        //页面加载
        $(function () {
            //文件列表初始化
            $('#files').myDatagrid({
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#fileContainer').panel('setTitle', '暂存附件(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainer");
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });

                    //$("#unUpload").next().height(600);
                    $("#unUpload").next().find(".datagrid-wrap").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(600);
                }
            });
            // 初始化包装类型
            $("#WrapType").combogrid({
                idField: "Code",
                textField: "Name",
                data: WrapTypeData,
                fitColumns: true,
                mode: "local",
                columns: [[
                    { field: 'Code', title: 'Code', width: 50, align: 'center', sortable: true },
                    { field: 'Name', title: 'Name', width: 120, align: 'center' },
                ]],
                onSelect: function () {
                    var grid = $("#WrapType").combogrid('grid');
                    var row = grid.datagrid('getSelected');
                },
                keyHandler: {
                    up: function () { },
                    down: function () { },
                    enter: function () { },
                    query: function (data) {
                        //动态搜索 
                        $("#WrapType").combogrid("grid").datagrid("reload", { 'keyword': data });
                        $("#WrapType").combogrid("setValue", data);
                    }
                }
            });
            //绑定日志信息
            var data = new FormData($('#form2')[0]);
            data.append("ID", ID);
            $.ajax({
                url: '?action=LoadLogs',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    showLogContent(data);
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });
            Init();
        });

        function Init() {
            if (Temporary != null) {
                $("#EntryNumber").textbox('setValue', Temporary.EntryNumber);
                $("#CompanyName").textbox('setValue', Temporary.CompanyName);
                $("#ShelveNumber").textbox('setValue', Temporary.ShelveNumber);
                $("#EntryDate").datebox('setValue', Temporary.EntryDate);
                $("#WaybillCode").textbox('setValue', Temporary.WaybillCode);
                $("#PackNo").numberbox('setValue', Temporary.PackNo);
                if (Temporary.WrapType == 0 || Temporary.WrapType == 1 || Temporary.WrapType == 4 || Temporary.WrapType == 6) {
                    $("#WrapType").combogrid('setValue', "0" + Temporary.WrapType);
                }
                else {
                    $("#WrapType").combogrid('setValue', Temporary.WrapType);
                }
                $("#Summary").textbox('setValue', Temporary.Summary);
            }
        }

        //预览文件
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
             $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //关闭
        function Close() {
            $.myWindow.close();
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //暂存照片操作
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',border:false,collapsible:false,split:true" title="" style="width: 350px; min-width: 330px">
        <form id="form1" runat="server" method="post">
            <table id="table1" style="line-height: 30px">
                <tr>
                    <td class="lbl">入仓号：</td>
                    <td>
                        <input class="easyui-textbox" id="EntryNumber" name="EntryNumber"
                            data-options="required:false,validType:'length[1,50]',width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">公司名称：</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" name="CompanyName"
                            data-options="required:true,validType:'length[1,50]',width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">库位号：</td>
                    <td>
                        <input class="easyui-textbox" id="ShelveNumber" name="ShelveNumber"
                            data-options="required:true,validType:'length[1,50]',width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">入库时间：</td>
                    <td>
                        <input class="easyui-datetimebox" id="EntryDate" name="ShelveNumber"
                            data-options="required:true,validType:'length[1,50]',width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">运单号：</td>
                    <td>
                        <input class="easyui-textbox" id="WaybillCode" name="WaybillCode"
                            data-options="required:false,validType:'length[1,50]',width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">件数：</td>
                    <td>
                        <input type="text" id="PackNo" name="PackNo" class="easyui-numberbox"
                            data-options="min:0,required:true,validType:'length[1,3]',width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr style="height: 30px">
                    <td class="lbl">包装类型：</td>
                    <td>
                        <input class="easyui-combogrid" id="WrapType" name="WrapType"
                            data-options="required:true,width:250,height:26,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="multiline:true,required:false,validType:'length[1,500]',height:50,width:250,disabled:true" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div data-options="region:'center',border:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center',border:false">
                <form id="form2">
                    <div id="fileContainer" title="" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'auto',border:false">
                        <div class="sub-container">
                            <div class="text-container" style="margin-top: 10px;">
                                <p>仅限图片或pdf格式的文件,并且pdf文件大小不能超过3M</p>
                            </div>
                            <div id="unUpload" style="margin-left: 5px">
                                <p>未上传</p>
                            </div>
                            <div>
                                <table id="files" data-options="nowrap:false,queryParams:{ action: 'data' }">
                                    <thead>
                                        <tr>
                                            <th data-options="field:'img',formatter:ShowImg">图片</th>
                                            <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>                     
                        </div>
                    </div>
                </form>
            </div>
            <div data-options="region:'south',border:false,collapsible:false,split:true" style="min-height: 150px" title="日志记录">
                <div class="sub-container">
                    <div class="text-container" id="LogContent">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
