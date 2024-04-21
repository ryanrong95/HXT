<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.AcceptanceApply.Detail" %>

<%@ Import Namespace="Yahv.Finance.Services.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //附件列表展示
            $('#file').myDatagrid({
                actionName: 'filedata',
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
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

                    var unUploadHeight = data.total * 36 + 100;//ryan 根据附件个数 动态计算高度

                    $("#unUpload").next().find(".datagrid-wrap").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(unUploadHeight);
                }
            });

            //审批日志
            $("#tabLogs").myDatagrid({
                fitColumns: true,
                fit: false,
                rownumbers: true,
                pagination: false,
                actionName: 'getLogs',
                columns: [[
                    { field: 'CreateDate', title: '审批时间', width: fixWidth(15) },
                    { field: 'ApproverName', title: '审批人', width: fixWidth(10) },
                    { field: 'Status', title: '审批结果', width: fixWidth(10) },
                    { field: 'Summary', title: '审批意见', width: fixWidth(55) }
                ]]
            });

            //付款类型
            //$("#AccountCatalogID").combotree({
            //    data: eval(model.AccountCatalogs),
            //    onLoadSuccess: function (node, data) {
            //        if (model.Data) {
            //            $('#AccountCatalogID').combotree('setValue', { id: model.Data.AccountCatalogID });
            //        }
            //    }
            //});
            $('#AccountCatalogID').combobox({
                data: model.AccountCatalogs,
                textField: 'text',
                valueField: 'value',
                required: true
            });

            if (model.Data) {
                $('form').form('load', model.Data);

                $('#tr_mo').hide();
                $('#tr_decors').hide();

                if (model.Data.Type == '<%= (int)AcceptanceType.Endorsement%>') {
                    $('#tr_decors').show();
                } else {
                    $('#tr_mo').show();
                }
            }
        });
    </script>
    <script>
        function FileOperation(val, row, index) {
            var buttons = row.CustomName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            return buttons;
        }

        function ShowImg(val, row, index) {
            return "<img src='../../Content/Images/wenjian.png' />";
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

            $('.window-mask').css('display', 'none');
        }
    </script>
    <style>
        #unUpload + div td {
            border: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
        </div>
        <table class="liebiao">
            <tr>
                <td>类型
                </td>
                <td>
                    <input id="TypeName" name="TypeName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>承兑汇票
                </td>
                <td>
                    <input id="MoneyOrderCode" name="MoneyOrderCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>调出账户
                </td>
                <td colspan="3">
                    <input id="PayerAccountName" name="PayerAccountName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>调出账号</td>
                <td>
                    <input id="PayerCode" name="PayerCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>调出银行</td>
                <td>
                    <input id="PayerBank" name="PayerBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>调出币种 
                </td>
                <td>
                    <input id="PayerCurrency" name="PayerCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>调出金额</td>
                <td>
                    <input id="PayerPrice" name="PayerPrice" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>调入账户
                </td>
                <td colspan="3">
                    <input id="PayeeAccountName" name="PayeeAccountName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>调入账号</td>
                <td>
                    <input id="PayeeCode" name="PayeeCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>调入银行</td>
                <td>
                    <input id="PayeeBank" name="PayeeBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>调入币种</td>
                <td>
                    <input id="PayeeCurrency" name="PayeeCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>调入金额</td>
                <td>
                    <input id="PayeePrice" name="PayeePrice" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款方式 
                </td>
                <td>
                    <input id="PaymentMethord" name="PaymentMethord" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款日期</td>
                <td>
                    <input id="PaymentDate" name="PaymentDate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>

            </tr>
            <tr>
                <td>调出流水号
                </td>
                <td>
                    <input id="OutFormCode" name="OutFormCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>调入流水号
                </td>
                <td>
                    <input id="InFormCode" name="InFormCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr id="tr_mo">
                <td>承兑日期</td>
                <td colspan="3">
                    <input id="ExchangeDate" name="ExchangeDate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr id="tr_decors">
                <td>背书日期</td>
                <td>
                    <input id="EndorseDate" name="EndorseDate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>是否允许转让</td>
                <td>
                    <input id="IsTransfer" name="IsTransfer" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>申请人</td>
                <td>
                    <input id="ApplierID" name="ApplierID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>指定审批人</td>
                <td>
                    <input id="ApproverName" name="ApproverName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>上传附件</td>
                <td colspan="3">
                    <div style="height: 40%; width: 80%; border: 1px solid #d3d3d3; border-radius: 5px; padding: 3px; overflow-y: auto;">
                        <div id="unUpload" style="margin-left: 5px;">
                            <p>未上传</p>
                        </div>
                        <table id="file" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto;" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <table id="tabLogs" title="审批日志"></table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
