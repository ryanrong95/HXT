<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="EditFee.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.PaymentApply.EditFee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //提交
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                var data = new FormData($('form')[0]);
                ajaxLoading();
                $.post({
                    url: '?action=Submit&&id=' + getQueryString('id'),
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.data, type: "success" });
                            top.$.myDialog.close();
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });
                return false;
            });

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

            //付款类型
            $("#AccountCatalogID").combotree({
                data: eval(model.AccountCatalogs),
                onLoadSuccess: function (node, data) {
                    if (model.Data) {
                        $('#AccountCatalogID').combotree('setValue', { id: model.Data.PayerLeft.AccountCatalogID });
                    }
                }
            });

            if (model.Data) {
                $('form').form('load', model.Data);

                $('#tr_rate').hide();
                if (model.Data.PayeeCurrency != model.Data.PayerCurrency) {
                    $('#tr_rate').show();
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
            <input type="submit" id='btnSubmit' />
            <input type="hidden" id="FormCode_Hidden" name="FormCode_Hidden" />
            <input type="hidden" id="PaymentDate_Hidden" name="PaymentDate_Hidden" />
            <input type="hidden" id="PaymentMethord_Hidden" name="PaymentMethord_Hidden" />
        </div>
        <table class="liebiao">
            <tr>
                <td>收款人
                </td>
                <td>
                    <input id="PayeeAccountID" name="PayeeAccountID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayeeCurrency" name="PayeeCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>收款账号</td>
                <td>
                    <input id="PayeeCode" name="PayeeCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>收款银行</td>
                <td>
                    <input id="PayeeBank" name="PayeeBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款公司
                </td>
                <td>
                    <input id="PayerAccountID" name="PayerAccountID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayerCurrency" name="PayerCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款账号</td>
                <td>
                    <input id="PayerCode" name="PayerCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款银行</td>
                <td>
                    <input id="PayerBank" name="PayerBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <%--<tr>
                <td>银行自动扣除</td>
                <td>
                    <input id="IsPaid" name="IsPaid" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款日期</td>
                <td>
                    <input id="PaymentDate" name="PaymentDate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>--%>
            <tr>
                <td>付款日期</td>
                <td>
                    <input id="PaymentDate" name="PaymentDate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款方式 
                </td>
                <td>
                    <input id="PaymentMethord" name="PaymentMethord" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款类型
                </td>
                <td>
                    <input id="AccountCatalogID" name="AccountCatalogID" style="width: 200px;" class="easyui-combotree" disabled="disabled" />
                </td>
                <td>付款金额
                </td>
                <td>
                    <input id="Price" name="Price" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr id="tr_rate">
                <td>汇率
                </td>
                <td colspan="3">
                    <input id="TargetRate" name="TargetRate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>流水号
                </td>
                <td>
                    <input id="FormCode" name="FormCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>手续费</td>
                <td>
                    <input id="ServiceCharge" name="ServiceCharge" class="easyui-textbox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>申请人</td>
                <td colspan="3">
                    <input id="ApplierID" name="ApplierID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>摘要</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" />
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
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
