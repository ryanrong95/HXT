<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_demos_chenhan_datagrid.aspx.cs" Inherits="WebApp.examples.controls._demos_chenhan_datagrid" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />

    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />


    <link href="http://fix.szhxd.net/frontframe/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/customs-easyui/Styles/main.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/customs-easyui/Styles/reset.css" rel="stylesheet" />

    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>
    <script src="http://fix.szhxd.net/frontframe/customs-easyui/Scripts/main.js"></script>
    <script src="http://fix.szhxd.net/frontframe/ajaxPrexUrl.js"></script>
    <script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/easyui.jl.js"></script>

    <script>

        var data = [
      { "productid": "FI-SW-01", "productname": "Koi", "unitcost": 10.00, "status": "P", "listprice": 36.50, "attr1": "Large", "itemid": "EST-1" },
      { "productid": "K9-DL-01", "productname": "Dalmation", "unitcost": 12.00, "status": "P", "listprice": 18.50, "attr1": "Spotted Adult Female", "itemid": "EST-10" },
      { "productid": "RP-SN-01", "productname": "Rattlesnake", "unitcost": 12.00, "status": "P", "listprice": 38.50, "attr1": "Venomless", "itemid": "EST-11" },
      { "productid": "RP-SN-01", "productname": "Rattlesnake", "unitcost": 12.00, "status": "P", "listprice": 26.50, "attr1": "Rattleless", "itemid": "EST-12" },
      { "productid": "RP-LI-02", "productname": "Iguana", "unitcost": 12.00, "status": "P", "listprice": 35.50, "attr1": "Green Adult", "itemid": "EST-13" },
      { "productid": "FL-DSH-01", "productname": "Manx", "unitcost": 12.00, "status": "P", "listprice": 158.50, "attr1": "Tailless", "itemid": "EST-14" },
      { "productid": "FL-DSH-01", "productname": "Manx", "unitcost": 12.00, "status": "P", "listprice": 83.50, "attr1": "With tail", "itemid": "EST-15" },
      { "productid": "FL-DLH-02", "productname": "Persian", "unitcost": 12.00, "status": "P", "listprice": 23.50, "attr1": "Adult Female", "itemid": "EST-16" },
      { "productid": "FL-DLH-02", "productname": "Persian", "unitcost": 12.00, "status": "P", "listprice": 89.50, "attr1": "Adult Male", "itemid": "EST-17" },
      { "productid": "AV-CB-01", "productname": "Amazon Parrot", "unitcost": 92.00, "status": "P", "listprice": 63.50, "attr1": "Adult Male", "itemid": "EST-18" }
        ];


        var cardview = $.extend({}, $.fn.datagrid.defaults.view, {
            renderRow: function (target, fields, frozen, rowIndex, rowData) {

                var opts = $.data(target, 'datagrid').options;

                var cc = [];
                if (frozen && opts.rownumbers) {
                    var rownumber = rowIndex + 1;
                    if (opts.pagination) {
                        rownumber += (opts.pageNumber - 1) * opts.pageSize;
                    }
                    cc.push('<td class="datagrid-td-rownumber"><div class="datagrid-cell-rownumber">' + rownumber + '</div></td>');
                }

                for (var i = 0; i < fields.length; i++) {
                    var field = fields[i];
                    var col = $(target).datagrid('getColumnOption', field);
                    if (col) {
                        var value = rowData[field];	// the field value
                        var css = col.styler ? (col.styler(value, rowData, rowIndex) || '') : '';
                        var classValue = '';
                        var styleValue = '';
                        if (typeof css == 'string') {
                            styleValue = css;
                        } else if (cc) {
                            classValue = css['class'] || '';
                            styleValue = css['style'] || '';
                        }
                        var cls = classValue ? 'class="' + classValue + '"' : '';
                        var style = col.hidden ? 'style="display:none;' + styleValue + '"' : (styleValue ? 'style="' + styleValue + '"' : '');

                        cc.push('<td field="' + field + '" ' + cls + ' ' + style + '>');

                        if (col.checkbox) {
                            var style = '';
                        } else {
                            var style = styleValue;
                            if (col.align) { style += ';text-align:' + col.align + ';' }
                            if (!opts.nowrap) {
                                style += ';white-space:normal;height:auto;';
                            } else if (opts.autoRowHeight) {
                                style += ';height:auto;';
                            }
                        }

                        cc.push('<div style="' + style + '" ');
                        cc.push(col.checkbox ? 'class="datagrid-cell-check"' : 'class="datagrid-cell ' + col.cellClass + '"');
                        cc.push('>');

                        if (col.checkbox) {
                            cc.push('<input type="checkbox" name="' + field + '" value="' + (value != undefined ? value : '') + '">');
                        } else if (col.formatter) {
                            cc.push(col.formatter(value, rowData, rowIndex));
                        } else {

                            console.log('test:' + value);

                            cc.push(value);
                        }

                        cc.push('</div>');
                        cc.push('</td>');
                    }
                }
                return cc.join('');
            }
        });

        //标准型号控件
        function standardProductFormatter(value, row, index) {
            var html = '<input value="' + value + '" id="datagrid_' + index + '_partNumber" class="partNumber" name="datagrid_' + index + '_partNumber" style="width: 200px" />'
            return html;
        }



        var columns = [
                    { //产品
                        data: 'Name',
                        type: 'text',
                        options: {
                            required: true,
                            missingMessage: '产品型号必填!',

                        }
                    },
                    { //数量
                        data: 'Quantity',
                        type: 'numeric'
                    },
                     {//数量说明
                         data: 'QuantityRemark',
                         type: 'autocomplete',
                         // editor: 'select',.
                         source: _quantityRemark,
                         strict: true,
                         allowInvalid: true,

                     },
                    {//品牌
                        data: 'Manufacturer',
                        type: 'text'
                    },
                    {//品牌替换
                        data: 'MfReplace',
                        type: 'checkbox'
                    },
                    {//渠道要求
                        data: 'ChannelRequirement',
                        type: 'text'

                    },
                     //{ //品牌要求
                     //    data: 'MfRequirement',
                     //    type: 'autocomplete',
                     //    // editor: 'select',.
                     //    source: _mfRequirement,
                     //    strict: true,                        //值为true，严格匹配
                     //    allowInvalid: true,              //值为true时，允许输入值作为匹配内容，只能在strict为true时使用
                     //},
                    {//封装
                        data: 'Package',
                        type: 'text'
                    },
                      {//批次
                          data: 'Batch',
                          type: 'text'
                      },
                       //{
                       //    data: 'Currency',
                       //    type: 'autocomplete',
                       //    // editor: 'select',.
                       //    source: _currency,
                       //    strict: true,
                       //    allowInvalid: true,


                       //},
                        {//接受价（未税）
                            data: 'UnitPrice',
                            type: 'numeric',
                        },
                        {//接受价（含税）
                            data: 'UnitPrice1',
                            type: 'numeric',
                        },
                        //{//接受价
                        //    data: 'UnitPrice2',
                        //    type: 'numeric'
                        //},
                          //{
                          //    data: 'DeliveryDate',
                          //    type: 'date',
                          //    dateFormat: 'YYYY/M/DD'
                          //},
                         {
                             data: 'TradeType',
                             type: 'autocomplete',
                             // editor: 'select',.
                             source: _tradeType,
                             strict: true,
                             allowInvalid: true,

                         },
                         {
                             data: 'PurchasingCycle',
                             type: 'numeric'
                         },
                          {
                              data: 'Summary',
                              type: 'text'
                          },

        ];

        $(function () {
            $('#tt').datagrid({
                //view: cardview,
                rownumbers: true,
                singleSelect: true,
                collapsible: true,
                //fitColumns: true,
                data: data,
                onLoadSuccess: function () {
                    $('.partNumber').standardPartNumer();
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <table id="tt" style="width: 100%; height: 250px">
                <thead>
                    <tr>
                        <th data-options="field:'itemid',width:80">No.</th>
                        <th data-options="field:'itemid',width:80">Item ID</th>
                        <th data-options="field:'productid',width:240,formatter:standardProductFormatter">Product</th>
                        <th data-options="field:'listprice',width:80,align:'right'">List Price</th>
                        <th data-options="field:'unitcost',width:80,align:'right'">Unit Cost</th>
                        <th data-options="field:'attr1',width:250">Attribute</th>
                        <th data-options="field:'status',width:60,align:'center'">Status</th>
                    </tr>
                </thead>
            </table>
        </div>
    </form>
</body>
</html>
