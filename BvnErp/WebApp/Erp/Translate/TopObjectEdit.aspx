<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TopObjectEdit.aspx.cs" Inherits="WebApp.Erp.Translate.TopObject" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>国际化语言翻译</title>
    <uc:EasyUI runat="server" />

    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '系统管理';
        gvSettings.menu = '翻译管理';
        gvSettings.summary = '';

    </script>

    <script type="text/javascript">
        var name;
        var editId;
        $(function () {
            $.get("?action=treedata", {}, function (data) {
                var sort = function (data) {
                    if (data instanceof Array) {
                        data = data.sort(function (left, rigth) {
                            if (left.text > rigth.text) {
                                return 1;
                            } else if (left.text < rigth.text) {
                                return -1;
                            } else {
                                return 0;
                            }
                        });
                        for (var index in data) {
                            if (data[index].children) {
                                sort(data[index].children);
                            }
                        }
                    };
                };
                sort(data);
                $("#tt").tree({
                    data: data,
                    onClick: function (node) {
                        name = getParentNodes(node);
                        loadRight(name);
                    }
                });
            }, 'json');
        });
        function collapseAll() {
            $('#tt').tree('collapseAll');
        }
        function expandAll() {
            $('#tt').tree('expandAll');
        }
        function getParentNodes(node) {
            var str = "";
            var parentAll = "";
            parentAll = node.text;
            parentAll = parentAll.replace(/\[[^\)]*\]/g, ""); //获得所需的节点文本
            var parent = $('#tt').tree('getParent', node.target); //获取选中节点的父节点 
            while (parent != null) {
                if (parent.tag) {
                    parentAll = "#!@%".concat(parentAll);
                }
                else {
                    parentAll = ".".concat(parentAll);
                }
                str = (parent.text).replace(/\[[^\)]*\]/g, "");
                parentAll = (str).concat(parentAll);
                var parent = $('#tt').tree('getParent', parent.target);
            }
            return parentAll;
        }
        function loadRight(name) {
            $('#tb1').datagrid({
                url: "?action=getConfig",
                method: "post",
                queryParams: {
                    "name": name,
                },
                onClickCell: function (rowIndex, field, value) {
                    if (editId != undefined) {
                        $('#tb1').datagrid('endEdit', editId);
                    }
                    if (field == "Value") {
                        $('#tb1').datagrid('beginEdit', rowIndex);
                        editId = rowIndex;
                        var ed = $('#tb1').datagrid('getEditor', { index: rowIndex, field: field });
                        var obj = ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target));
                        obj.focus();
                        obj.blur(function () {
                            if ($.trim(obj.val()) == value) {
                                $('#tb1').datagrid('cancelEdit', rowIndex);
                            }
                            //else {
                            //    $('#tb1').treegrid('endEdit', editId);
                            //}
                        });
                    }

                },
                onAfterEdit: function (rowIndex, rowData, changes) {
                    $.post("?action=updCofig",
                          {
                              id: rowData.ID,
                              value: $('<div>').html(changes.Value).html()
                          },
                          function (data) {
                              rowData.Other = data;
                              $('#tb1').datagrid('refreshRow', rowIndex);
                          });
                }
            })
        }
        function onRowContextMenu(e, rowIndex, rowData) {
            e.preventDefault();
            var selected = $("#tb1").datagrid('getRows'); //获取所有行集合对象
            selected[rowIndex].id; //index为当前右键行的索引，指向当前行对象
            $('#mm').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        }

        function del() {
            var selects = $('#tb1').datagrid('getSelections');
            if (selects.length == 0) {
                $.messager.alert('提示', '请选择要删除的信息！', 'info');
                return false;
            }
            $.messager.confirm('删除提示', '您确定要删除选中的记录吗?', function (r) {
                if (r) {
                    var ids = '';
                    for (var item in selects) {
                        ids += selects[item].ID;
                        ids += '|';
                    }
                    $.post("?action=delConfig", { ID: ids }, function (text) {
                        $.messager.alert('提示', text);
                        $('#tb1').datagrid('reload');
                    })
                }
            })
        }
        function renewLang() {
            $.ajax({
                url: 'TopobjectConfig.ashx',
                type: 'get',
                dataType: 'json',
                data: { action: 'readLang' },
                success: function (text) {

                },
                error: function () {
                }
            });

        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',collapsible:false" title="国际化语言翻译" style="height: 55px">
        <a href="#" class="easyui-linkbutton" onclick="renewLang()">查看翻译</a>
        <a href="#" class="easyui-linkbutton" onclick="collapseAll()">收起</a>
        <a href="#" class="easyui-linkbutton" onclick="expandAll()">展开</a>
        <a href="#" class="easyui-linkbutton" onclick="top.$.myWindow({            
                    url: location.href.replace(/TopObjectEdit/i, 'importlangConfigs')
                }).open();">批量处理</a>
    </div>
    <div data-options="region:'west',split:true" style="width: 20%;">
        <div id="tt" class="easyui-tree">
        </div>
    </div>
    <div data-options="region:'center',iconCls:'icon-ok'">
        <table id="tb1" class="easyui-datagrid"
            data-options="fitColumns:true,
                    striped:true,
                    onRowContextMenu: onRowContextMenu,
                    fit:true,
                    border:false
                    ">
            <thead>
                <tr>
                    <th data-options="field:'Language',width:40">语言</th>
                    <th data-options="field:'Name',width:80">配置名</th>
                    <th data-options="field:'Value',width:120,editor:{type:'text'}">值</th>
                    <th data-options="field:'Other',width:20"></th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="mm" class="easyui-menu" style="width: 20px;">
        <button onclick="del()" class="easyui-linkbutton" data-options="iconCls:'icon-remove'">删除</button>
    </div>
</body>
</html>

