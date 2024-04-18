<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Handle.aspx.cs" Inherits="Yahv.PvOms.WebApp.Stocks.Temporary.Handle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ID = getQueryString("ID");
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                pagination: false,
                rownumbers: true,
            });
            $('#file').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadFiles',
                rowStyler: function (index, row) {
                    return 'background-color:#ffffff;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationFile }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".file");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            //审批通过
            $("#btnSave").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var enterCode = $("#EnterCode").textbox('getValue');
                $.messager.confirm('确认', '请您确认是否修改客户入仓号', function (success) {
                    if (success) {
                        ajaxLoading();
                        $.post('?action=Submit', { ID: ID, EnterCode: enterCode }, function (result) {
                            ajaxLoadEnd();
                            var res = JSON.parse(result);
                            if (res.success) {
                                top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            }
                            else {
                                top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                            }
                        })
                    }
                });
            })
            //初始化
            Init();
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                EnterCode: $.trim($('#EnterCode').textbox("getText")),
                WaybillCode: $.trim($('#WaybillCode').textbox("getText")),
                Supplier: $.trim($('#Supplier').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        function Init() {
            $("#EnterCode").textbox('setValue', model.waybillData.EnterCode);
            $("#WaybillCode").textbox('setValue', model.waybillData.WaybillCode);
            $("#Supplier").textbox('setValue', model.waybillData.Supplier);
            $("#Place").textbox('setValue', model.waybillData.Place);
            $("#Address").textbox('setValue', model.waybillData.Address);
        }
        //文件操作
        function OperationFile(val, row, index) {
            return '<img src="../../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>';
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户入仓号</td>
                <td>
                    <input id="EnterCode" class="easyui-textbox" style="width: 250px"
                        data-options="required:true" />
                    <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'">保存</a>
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">供应商</td>
                <td>
                    <input id="Supplier" class="easyui-textbox" style="width: 250px"
                        data-options="disabled:true" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">运单号</td>
                <td>
                    <input id="WaybillCode" class="easyui-textbox" style="width: 250px"
                        data-options="disabled:true" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">发货地</td>
                <td>
                    <input id="Place" class="easyui-textbox" style="width: 250px"
                        data-options="disabled:true" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">详细地址</td>
                <td>
                    <input id="Address" class="easyui-textbox" style="width: 250px;height:40px"
                        data-options="disabled:true,multiline:true" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">暂存文件</td>
                <td>
                    <div class="file">
                        <table id="file">
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">到货日期</th>
                <th data-options="field:'Manufacturer',align:'left'" style="width: 150px;">品牌</th>
                <th data-options="field:'PartNumber',align:'left'" style="width: 150px;">型号</th>
                <th data-options="field:'DateCode',align:'left'" style="width: 100px;">批次号</th>
                <th data-options="field:'Origin',align:'center'" style="width: 100px">原产地</th>
                <th data-options="field:'Quantity',align:'center'" style="width: 100px">暂存数量</th>
            </tr>
        </thead>
    </table>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px;height:500px;min-width:70%;min-height:80%">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>

