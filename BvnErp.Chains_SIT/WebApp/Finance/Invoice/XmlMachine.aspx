<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="XmlMachine.aspx.cs" Inherits="WebApp.Finance.Invoice.XmlMachine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var InvoiceNoticeIDs = getQueryString("InvoiceNoticeIDs");
        var ExpressCompanyData = eval('(<%=this.Model.ExpressCompanyData%>)');
        $(function () {
             //初始化快递信息
            $('#ExpressName').combobox({
                data: ExpressCompanyData,
                onLoadSuccess: function (data) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Text == "顺丰") {
                            $('#ExpressName').combobox('select', data[i].Value);
                        }
                    }
                },
                onSelect: function (record) {
                    $.post('?action=ExpressSelect', { ID: record.Value }, function (data) {
                        //更新快递方式
                        data = eval(data);
                        $('#ExpressType').combobox({
                            data: data,
                        });
                        //默认选择第一行
                        $('#ExpressType').combobox('setValue', data[0].Value);
                    })
                },
            });


            $('#decheads').datagrid({
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,

                singleSelect: false,
                rownumbers: true,
                url: "?action=data&InvoiceNoticeIDs=" + InvoiceNoticeIDs,
                onLoadSuccess: function (data) {
                    var InvoiceNoticeIDs = [];
                    for (var i = 0; i < data.rows.length; i++) {
                        var row = data.rows[i];
                        if (InvoiceNoticeIDs.indexOf(row["InvoiceNoticeID"]) == -1) {
                            InvoiceNoticeIDs.push(row["InvoiceNoticeID"])
                        }
                    }
                    //计算每个InvoiceNotice 所对应的xml的开票总额
                    var InvoiceXmlSum = [];
                    var i = 0
                    for (var j = 0; j < InvoiceNoticeIDs.length; j++) {
                        var xmlSum = 0;
                        for (i; i < data.rows.length; i++) {
                            var row = data.rows[i];
                            if (InvoiceNoticeIDs[j] == row["InvoiceNoticeID"]) {
                                xmlSum += row["XmlPrice"];
                            } else {
                                InvoiceXmlSum.push(
                                    {
                                        InvoiceNoticeID: InvoiceNoticeIDs[j],
                                        XmlPrice:  xmlSum.toFixed(2)
                                    });
                                break;
                            }
                        }
                        if (j == InvoiceNoticeIDs.length - 1) {
                            InvoiceXmlSum.push(
                                {
                                    InvoiceNoticeID: InvoiceNoticeIDs[j],
                                    XmlPrice:  xmlSum.toFixed(2)
                                });
                        }
                    }


                    //添加合计行
                    var irow = 0;
                    for (var jrow = 0; jrow < InvoiceXmlSum.length; jrow++) {
                        for (irow; irow < data.rows.length; irow++) {
                            if (InvoiceXmlSum[jrow]["InvoiceNoticeID"] != data.rows[irow]["InvoiceNoticeID"]) {
                                $('#decheads').datagrid('insertRow', {
                                    index: irow,
                                    row: {
                                        InvoiceNoticeID: '合计', //这个 ID 指定为 appendRow, Operation() 方法中用到
                                        InvoiceNoticeAmount: '',
                                        InvoiceNoticeDiff: '',
                                        InvoicePrice: '',
                                        TaxRate: '',
                                        ID: '',
                                        XmlAmount: '',
                                        XmlTax: '',
                                        XmlPrice: InvoiceXmlSum[jrow]["XmlPrice"],
                                        btn: '',
                                    }
                                });
                                irow++;
                                break;
                            }
                        }
                        if (jrow == InvoiceXmlSum.length - 1) {
                            $('#decheads').datagrid('insertRow', {
                                index: irow,
                                row: {
                                    InvoiceNoticeID: '合计', //这个 ID 指定为 appendRow, Operation() 方法中用到
                                    InvoiceNoticeAmount: '',
                                    InvoiceNoticeDiff: '',
                                    InvoicePrice: '',
                                    TaxRate: '',
                                    ID: '',
                                    XmlAmount: '',
                                    XmlTax: '',
                                    XmlPrice: InvoiceXmlSum[jrow]["XmlPrice"],
                                    btn: '',
                                }
                            });
                        }

                    }

                    //合并单元格
                    var mark = 1;
                    for (var im = 0; im < data.rows.length; im++) {
                        //合并 订单编号、费用类型
                        if (im > 0) {
                            if (data.rows[im]['InvoiceNoticeID'] == data.rows[im - 1]['InvoiceNoticeID']) {
                                mark += 1;
                                $("#decheads").datagrid('mergeCells', {
                                    index: im + 1 - mark,
                                    field: 'InvoiceNoticeID',
                                    rowspan: mark
                                });
                                $("#decheads").datagrid('mergeCells', {
                                    index: im + 1 - mark,
                                    field: 'InvoiceNoticeAmount',
                                    rowspan: mark
                                });
                                $("#decheads").datagrid('mergeCells', {
                                    index: im + 1 - mark,
                                    field: 'InvoiceNoticeDiff',
                                    rowspan: mark
                                });
                                $("#decheads").datagrid('mergeCells', {
                                    index: im + 1 - mark,
                                    field: 'InvoicePrice',
                                    rowspan: mark
                                });
                                $("#decheads").datagrid('mergeCells', {
                                    index: im + 1 - mark,
                                    field: 'TaxRate',
                                    rowspan: mark
                                });
                            }
                            else {
                                mark = 1;
                            }
                        }
                    }


                },
                onClickRow: function () {

                },
            });
        });

        //操作
        function Operation(val, row, index) {
            if ('合计' == row.InvoiceNoticeID) {
                return '';
            }
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewItem(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            var ExpressCompany = $("#ExpressName").combobox('getValue');
            var ExpressType = $("#ExpressType").combobox('getValue');

            var data = $('#decheads').datagrid("getRows");

            var strIds = "";
            //拼接字符串
            for (var i = 0; i < data.length; i++) {
                strIds += data[i].InvoiceNoticeID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            MaskUtil.mask();
            $.post('?action=PostInvoice', {
                IDs: JSON.stringify(strIds),
                ExpressCompany: ExpressCompany,
                ExpressType:ExpressType
            }, function (result) {
                var rel = JSON.parse(result);
                MaskUtil.unmask();
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                       
                    }
                });
            });
        }

        function ViewItem(ID) {
            var url = location.pathname.replace(/XmlMachine.aspx/ig, 'XmlItems.aspx') + '?InvoiceXmlID=' + ID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 850,
                height: 620,
                onClose: function () {
                    //Search();
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="edit-swap-amount-content" style="overflow-y: auto; width: 1100px;height:92%;">
        <div>
            <table style="margin: 0 auto; line-height: 30px">                        
                        <tr>
                            <td class="lbl">快递公司：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressName" name="ExpressName" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                            </td>
                            <td class="lbl">快递方式：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressType" name="ExpressType" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                            </td>
                        </tr>                    
             </table>
        </div>
        <div data-options="region:'center',border:false">
            <table id="decheads" data-options="
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                toolbar:'#topBar',
                singleSelect:false,
                rownumbers:true">
                <thead>
                    <tr>
                        <th data-options="field:'InvoiceNoticeID',align:'center'" style="width: 150px;">通知ID</th>
                        <th data-options="field:'InvoiceNoticeAmount',align:'center'" style="width: 100px;">通知金额</th>
                        <th data-options="field:'InvoiceNoticeDiff',align:'center'" style="width: 100px;">通知差额</th>
                        <th data-options="field:'InvoicePrice',align:'center'" style="width: 100px;">通知开票金额</th>
                        <th data-options="field:'TaxRate',align:'center'" style="width: 50px;">税率</th>
                        <th data-options="field:'ID',align:'center'" style="width: 150px;">XmlID</th>
                        <th data-options="field:'XmlAmount',align:'center'" style="width: 100px;">Xml金额</th>
                        <th data-options="field:'XmlTax',align:'center'" style="width: 100px;">Xml税额</th>
                        <th data-options="field:'XmlPrice',align:'center'" style="width: 100px;">Xml开票金额</th>
                        <th data-options="field:'btn',width:150,formatter:Operation,align:'left'">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnNext" class="easyui-linkbutton" data-options="iconCls:'icon-save',width:70," onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel',width:70," onclick="Close()">取消</a>
    </div>
</body>
</html>
