<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForbidList.aspx.cs" Inherits="WebApp.Control.PreProduct.ForbidList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预归类产品禁运管控审批</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {
            //待审批列表初始化
            $('#controls').myDatagrid({
                singleSelect: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onSelect: function (index, row) {
                    if (!row.IsCanClassify) {
                        $('#products').datagrid('unselectRow', index);

                        if (IsCheckAll()) {
                            $("table input[type='checkbox']")[0].checked = true;
                        }
                    }
                },
                onCheck: function (index, row) {
                    if (IsCheckAll()) {
                        $("table input[type='checkbox']")[0].checked = true;
                    }
                },
                onCheckAll: function (rows) {
                    for (var index = 0; index < rows.length; index++) {
                        var row = rows[index];
                        if (!row.IsCanClassify) {
                            $('#products').datagrid('unselectRow', index);
                        }
                    }
                    $("table input[type='checkbox']")[0].checked = true;
                },
            });
        });

        //查询
        function Search() {
            var model = $('#Model').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            $('#controls').myDatagrid('search', { Model: model, ClientCode: clientCode });
        }

        //重置查询条件
        function Reset() {
            $('#Model').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            Search();
        }

        //同意报关员，产品无需3C认证
        function Approve(index) {
            $('#controls').datagrid('selectRow', index);
            var data = $('#controls').datagrid('getSelected');
            $.messager.confirm('确认', '同意报关员，产品为非禁运型号？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=Approve', { ID: data.ID }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('审批', result.message);
                            $('#controls').datagrid('reload');
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //不同意报关员，产品需要3C认证
        function Veto(index) {
            $('#controls').datagrid('selectRow', index);
            var data = $('#controls').datagrid('getSelected');
            $.messager.confirm('确认', '不同意报关员，产品为禁运型号？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=Veto', { ID: data.ID }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('审批', result.message);
                            $('#controls').datagrid('reload');
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //一键同意
        function BatchApprove() {
            var docData = $('#controls').datagrid('getChecked');
            if (docData.length <= 0) {
                $.messager.alert('提示', '请至少勾选一个产品！');
                return;
            }

            var arr = new Array();
            for (var i = 0; i < docData.length; i++) {
                arr[i] = docData[i].ID;
            }

            $.messager.confirm('确认', '同意报关员，产品为非禁运型号？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=BatchApprove', { IDs: arr.join() }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('审批', result.message);
                            $('#controls').datagrid('reload');
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //一键否决
        function BatchVeto() {
            var docData = $('#controls').datagrid('getChecked');
            if (docData.length <= 0) {
                $.messager.alert('提示', '请至少勾选一个产品！');
                return;
            }

            var arr = new Array();
            for (var i = 0; i < docData.length; i++) {
                arr[i] = docData[i].ID;
            }

            $.messager.confirm('确认', '不同意报关员，产品为禁运型号？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=BatchVeto', { IDs: arr.join() }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('审批', result.message);
                            $('#controls').datagrid('reload');
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Approve(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">同意</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Veto(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">否决</span>' +
                '<span class="l-btn-icon icon-no">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //是否勾选全选框
        function IsCheckAll() {
            var isCheckAll = true;
            var rows = $('#controls').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (!$('.datagrid-btable').find("input[type='checkbox']")[i].checked) {
                    isCheckAll = false;
                    break;
                };
            }

            return isCheckAll;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnApprove" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="BatchApprove()">一键同意</a>
            <a id="btnVeto" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-no'" onclick="BatchVeto()" style="margin-left: 10px;">一键否决</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
                <li>
                    <span style="font-style: italic; color: orangered; font-size: 13px">*点击“同意”表示产品为非禁运型号，可以报关；点击“否决”表示产品为禁运型号。</span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="controls" title="禁运产品信息（报关员归类为非禁运产品）" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true">全选</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 8%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'ProductUnionCode',align:'center'" style="width: 8%;">单据号</th>
                    <th data-options="field:'Model',align:'left'" style="width: 10%;">型号</th>
                    <th data-options="field:'Manufacturer',align:'left'" style="width: 10%;">品牌</th>
                    <th data-options="field:'HSCode',align:'center'" style="width: 8%;">商品编码</th>
                    <th data-options="field:'ProductName',align:'left'" style="width: 10%;">报关品名</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center'" style="width: 8%;">预处理一操作人</th>
                    <th data-options="field:'ClassifySecondOperatorName',align:'center'" style="width: 8%;">预处理二操作人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建时间</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
