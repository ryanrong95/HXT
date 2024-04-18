<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.AcceptanceBill.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>承兑汇票界面</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>    
    <%-- <script>
        gvSettings.fatherMenu = '银行账户管理';
        gvSettings.menu = '账户管理';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">       
       var BillStatus = eval('(<%=this.Model.BillStatus%>)');       
        
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true, fit: true, toolbar: '#topBar', nowrap: false, rownumbers: true, singleSelect: false,
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
            ////初始化Combobox
            $('#BillStatus').combobox({
                data: BillStatus,
            });

             //注册上传原始单据filebox的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['application/pdf'],
                onClickButton: function () {
                    $('#uploadFile').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                            MaskUtil.mask();               
                            formData.append('uploadFile', file);
                            $.ajax({
                                url: '?action=UploadFile',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    //var rel = JSON.parse(res);
                                    MaskUtil.unmask();
                                    if (res.success) {
                                        
                                    } else {
                                        $.messager.alert('提示', res.message);
                                    }
                                }
                            }).done(function (res) {

                            });
                     }
                    
                }
            });
           
        });

        //查询
        function Search() {
            var Code = $('#Code').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');           
            var BillStatus = $('#BillStatus').combobox('getValue');
            var CreateStartDate = $('#CreateStartDate').datebox('getValue');
            var CreateEndDate = $('#CreateEndDate').datebox('getValue');
            var parm = {
                Code: Code,
                StartDate: StartDate,
                EndDate: EndDate,
                BillStatus: BillStatus,
                CreateStartDate: CreateStartDate,
                CreateEndDate:CreateEndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Code').textbox('setValue', '');
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#BillStatus').combobox('setValue', null);
            $('#CreateStartDate').datebox('setValue', null);
            $('#CreateEndDate').datebox('setValue', null);
            Search();
        }

        //新增
        function BtnAdd() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx')+ "?Source=Add";
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增承兑汇票',
                width: '1050',
                height: '600',
                url: url,
                onClose: function () {
                    Search();
                }
            });
        }

        function BtnUpload() {

        }

       
        //编辑
        function Show(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID+"&Source=View";
                var Width = 1050;
                var Height = 600;
               
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '查看票据',
                    width: Width,
                    height: Height,
                    url: url,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        function Edit(index) {
            $('#datagrid').datagrid('selectRow', index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID+"&Source=Edit";
                var Width = 1050;
                var Height = 600;
               
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '编辑票据',
                    width: Width,
                    height: Height,
                    url: url,
                    onClose: function () {
                        Search();
                    }
                });
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.ExchangeDate != null) {
                 buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Show(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">编辑</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
                       
            return buttons;
        }

        function Generate() {
            var data = $('#datagrid').myDatagrid('getSelections');
            if (data.length == 0) {
                $.messager.alert('提示', '请先勾选要生成收款的数据！');
                return;
            }

            var AccInfo = [];
            for (i = 0; i < data.length; i++) {
                if (data[i].AcceptedDate == "" || data[i].AcceptedDate == null) {
                    $.messager.alert('提示', '不能勾选签收日期为空的数据！');
                    return;
                }
                AccInfo.push({
                    ID: data[i].ID                  
                });
            };

            MaskUtil.mask();//遮挡层
            //验证成功
            $.post('?action=GenFinanceReceipt', {
                Model: JSON.stringify(AccInfo)
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', "生成收款成功", 'info', function () {
                        $('#datagrid').myDatagrid('reload');
                    });
                }
                else {
                    $.messager.alert('消息', "生成收款失败", 'info', function () { });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <%--<a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BtnAdd()">新增</a> --%>       
                    <input id="uploadFile" name="uploadFile" class="easyui-filebox"  style="width: 57px; height: 24px" />
                </li>
            </ul>
        </div>
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">票据号码</td>
                    <td>
                        <input class="easyui-textbox" id="Code" data-options="height:26,width:200" />                        
                    </td>
                    <td class="lbl">票据到期日:</td>
                    <td>
                       <input class="easyui-datebox" id="StartDate" />
                    </td>
                     <td class="lbl">至:</td>
                    <td>
                       <input class="easyui-datebox" id="EndDate" />
                    </td>
                    <td class="lbl">状态:</td>
                    <td>
                        <input class="easyui-combobox" id="BillStatus" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                     <td class="lbl">创建日期:</td>
                    <td>
                       <input class="easyui-datebox" id="CreateStartDate" />
                    </td>
                     <td class="lbl">至:</td>
                    <td>
                       <input class="easyui-datebox" id="CreateEndDate" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnGenerate" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Generate()">生成收款</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="账户列表" data-options="fitColumns:true,fit:true,toolbar:'#topBar',nowrap:false,rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'InAccountName',align:'left'" style="width: 150px;">收款人账户</th>
                    <th data-options="field:'Code',align:'left'" style="width: 200px;">票据号码</th>
                    <th data-options="field:'Price',align:'left'" style="width: 80px;">金额</th>
                    <th data-options="field:'OutAccountName',align:'left'" style="width: 150px;">出票人</th>
                    <th data-options="field:'StartDate',align:'left'" style="width: 60px;">出票日期</th>
                    <th data-options="field:'EndDate',align:'left'" style="width: 60px;">汇票到期日</th>
                    <th data-options="field:'AcceptedDate',align:'left'" style="width: 60px;">签收日期</th>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 100px;">创建日期</th>
                    <th data-options="field:'Status',align:'left'" style="width: 90px;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

