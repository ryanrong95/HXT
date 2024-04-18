<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Documents.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var treedata = eval(<%=this.Model.TreeData%>);
        var admin = eval(<%=this.Model.Admin%>);
        var directoryid = null;

        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM工作管理';
        gvSettings.menu = '文档管理';
        gvSettings.summary = '';

        function down(id) {
            var h = location.href + "?action=download&id=" + id;
            location.href = location.href + "?action=download&id=" + id;
            return false;
        }
        //页面加载
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                toolbar: '#tb',
                loadFilter: function (data) {
                    document.getElementById("pathlab").innerText = "当前目录：" + escape2Html(data.path);

                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (!!row.CreateDate) {
                            row.CreateDate = new Date(row.CreateDate).toDateTimeStr();
                        }
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
                    $('#datagrid').bvgrid('reload');
                },
            });

            //区域树的初始化
            $('#directories').tree({
                data: treedata,
                onClick: function (node) {
                    directoryid = node.id;
                    $('#datagrid').bvgrid('search', { DirectoryID: directoryid });
                },
                onContextMenu: function (e, node) {
                    if (admin.JobType == 800) {
                        e.preventDefault();
                        // 查找节点
                        $('#directories').tree('select', node.target);
                        // 显示快捷菜单
                        $('#mm').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                    }
                },
                onAfterEdit: function (node) {
                    var parent = $(this).tree("getParent", node.target);
                    $.post("?action=SaveDirectory", { ID: node.id, FatherID: parent.id, Name: node.text }, function (data) {
                        window.location.href = window.location.href;
                    });
                },
            });
        });


        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            buttons += '<button id="btnMove" onclick="Move(' + index + ')">转移文件</button>';
            return buttons;
        }

        //查看文件
        function View(val, row, index) {
            var buttons = '<a href="javascript:void(0);"  style="color:Blue" onclick="down(\'' + row.ID + '\')">' + row.Name + '</a>';
            return buttons;
        }


        //转移文件夹页面弹出
        function Move(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                win.ID = rowdata.ID;
                win.window('open');
            }
        }

        //转移文件夹
        function MoveClick() {
            var DirectoryID = $("#DirectoryID").combobox("getValue");
            if (DirectoryID == "") {
                $.messager.alert('提示', '请先选择转移文件夹！');
                return;
            }
            $.post('?action=Move', { ID: win.ID, DirectoryID: DirectoryID }, function (result) {
                $.messager.alert('转移', '转移成功！');
                win.window('close');
            });
        }

        //关闭弹出框
        function closeWin() {
            win.window('close');
        }

        //新增
        function Add() {
            if (directoryid == null) {
                $.messager.alert("提示", "请选择一个文件夹新增文件!");
                return;
            }
            var url = location.pathname.replace(/List.aspx/ig, 'DocumentEdit.aspx') + "?DirectoryID=" + directoryid;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '文件信息新增',
                url: url,
                onClose: function () {
                    $('#datagrid').bvgrid('reload');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'DocumentEdit.aspx') + "?ID=" + rowdata.ID + "&DirectoryID=" + directoryid;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '文件信息编辑',
                    url: url,
                    onClose: function () {
                        $('#datagrid').bvgrid('reload');
                    }
                }).open();
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').bvgrid('reload');
                    })
                }
            });
        }

        //按钮点击触发事件
        function Append() {
            var node = $('#directories').tree('getSelected');
            var newid = new Date().Format("yyyyMMddhhmmssS") + node.id;
            $("#directories").tree("append", {
                parent: node.target,
                data: [{
                    id: newid,
                    text: '新目录'
                }]
            });
            var node = $("#directories").tree("find", newid);
            $('#directories').tree('beginEdit', node.target);
        }

        //开始编辑
        function BeginEdit() {
            var node = $('#directories').tree('getSelected');
            $('#directories').tree('beginEdit', node.target);
        }

        //删除目录
        function Remove() {
            var node = $('#directories').tree('getSelected');
            $.messager.confirm('确认', '本操作将删除' + node.text + ' 目录，如其下有子目录或文件也将一并被删除；本操作不可逆！', function (success) {
                if (success) {
                    var children = $('#directories').tree('getChildren', node.target);
                    var ids = node.id;
                    $.map(children, function (value) {
                        ids = ids + "," + value.id;
                    });
                    $.post('?action=DeleteDirectory', { ID: ids }, function () {
                        $.messager.alert('删除', '目录删除成功！');
                        $('#directories').tree('remove', node.target);
                    });
                }
            });
        }

        //时间格式化
        Date.prototype.Format = function (fmt) { //author: meizz 
            var o = {
                "M+": this.getMonth() + 1, //月份 
                "d+": this.getDate(), //日 
                "h+": this.getHours(), //小时 
                "m+": this.getMinutes(), //分 
                "s+": this.getSeconds(), //秒 
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
                "S": this.getMilliseconds() //毫秒 
            };
            if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }
    </script>
</head>
<body class="easyui-layout">

    <div title="文件夹目录" data-options="region:'west',split:true,border:false" style="width: 15%">
        <div id="div1">
            <ul id="directories" class="easyui-tree" data-options="method:'get'"></ul>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <div id="tb">
            <a id="btnAdd" href="javascript:void(0);" v_name="add" v_title="文件新增按钮" class="easyui-linkbutton"
                data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <label id="pathlab" style="margin-left: 50px; margin-bottom: 0px"></label>
        </div>
        <table id="datagrid" title="文件列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" style="height: 98%" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation"
                        g_name="Btn" v_title="列表项操作按钮组" style="width: 150px">操作</th>
                    <th field="Title" data-options="align:'center'" style="width: 100px">标题</th>
                    <th field="Name" data-options="align:'center',formatter:View" style="width: 100px">文件</th>
                    <th field="Size" data-options="align:'center'" style="width: 100px">文件大小</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px;">上传人</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 100px;">上传时间</th>
                    <th field="Summary" data-options="align:'center'" style="width: 100px;">描述</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="mm" class="easyui-menu" style="width: 120px;">
        <div onclick="Append()" data-options="name:'Add',iconCls:'icon-add'">新增</div>
        <div class="menu-sep"></div>
        <div onclick="BeginEdit()" data-options="name:'Edit',iconCls:'icon-edit'">编辑</div>
        <div class="menu-sep"></div>
        <div onclick="Remove()" data-options="name:'Remove',iconCls:'icon-remove'">删除</div>
    </div>
    <div id="win" class="easyui-window" title="转移文件夹页面" style="width: 430px; height: 160px">
        <table id="table2" style="width: 400px; text-align: center">
            <tr style="height: 20px">
                <td colspan="5"></td>
            </tr>
            <tr>
                <td style="width: 100px"></td>
                <td style="text-align: right">文件夹</td>
                <td style="width: 5px"></td>
                <td colspan="2" style="text-align: left">
                    <input class="easyui-combotree" id="DirectoryID" name="DirectoryID"
                        data-options="valueField:'ID',textField:'Name',required:true,data:treedata" style="width: 150px" />
                </td>
            </tr>
            <tr style="height: 20px">
                <td colspan="5"></td>
            </tr>
            <tr>
                <td colspan="5">
                    <button id="btnDistrbute" onclick="MoveClick()" style="text-align: center">确定</button>
                    <button id="btnCancel" onclick="closeWin()" style="text-align: center">取消</button>
                </td>
            </tr>
        </table>
    </div>



</body>
</html>
