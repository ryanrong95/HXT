<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecList.aspx.cs" Inherits="WebApp.Declaration.Declare.DecList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script> 	
        jQuery.browser = {};
        (function () {
            jQuery.browser.msie = false;
            jQuery.browser.version = 0;
            if (navigator.userAgent.match(/MSIE ([0-9]+)./)) {
                jQuery.browser.msie = true; jQuery.browser.version = RegExp.$1;
            }
        })();		
    </script>
    <script>
        $(function () {
            var ID = getQueryString("ID");
            $("#DeclarationID").val(ID);
            //订单列表初始化
            $('#orders').myDatagrid({
                nowrap: false,
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
        });

        //查询
        function Search() {
            var GName = $('#GName').textbox('getValue');
            var GNO = $('#GNO').textbox('getValue');
            var OriginPlace = $('#OriginPlcae').textbox('getValue');
            debugger
            var parm = {
                GName: GName,
                GNO: GNO,
                OriginPlace:OriginPlace
            };
            $('#orders').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#GName').textbox('setValue', null);
            $('#GNO').textbox('setValue', null);
            $('#OriginPlace').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            if (window.parent.frames.Source == "Add" || window.parent.frames.Source == "Assign" || window.parent.frames.Source == "Edit") {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">编辑</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            else {
                var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SearchList(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            if (row.GoodsAttr.indexOf('3C目录内') > -1) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="GoodsLimit(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">许可证</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }

        function Edit(ID) {
            var url = location.pathname.replace(/DecList.aspx/ig, 'DecListEdit.aspx?ID=' + ID + '&ListSource=Edit');

            $.myWindow.setMyWindow("DecList2DecListEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑商品信息',
                width: '700px',
                height: '520px'
            });
        }

        function GoodsLimit(ID) {
            var url = location.pathname.replace(/DecList.aspx/ig, 'DecListLimit.aspx?ID=' + ID);

            $.myWindow.setMyWindow("DecList2DecGoodsLimit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑产品许可证/审批/备案信息',
                width: '1000px',
                height: '520px'
            });
        }
        

        function SearchList(ID) {
            var url = location.pathname.replace(/DecList.aspx/ig, 'DecListEdit.aspx?ID=' + ID + '&ListSource=Search');
            $.myWindow.setMyWindow("DecList2DecListEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看商品信息',
                width: '700px',
                height: '620px'
            });
        }

        function Check() {
            MaskUtil.mask();//遮挡层
            var ID = $("#DeclarationID").val();
            $.post("?action=Check", { ID: ID }, function (data) {
                MaskUtil.unmask();
                var result = JSON.parse(data);
                if (result.result) {
                    Download(result.info);
                } else {
                    $.messager.alert('消息', result.info);
                }
            });
        }

        function Download(Url) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = Url;
            a.download = "";
            a.click();
        }

        function CheckHtml() {
            var ID = $("#DeclarationID").val();
            $.post('?action=CheckHtml', { ID: ID }, function (data) {
                var result = JSON.parse(data);                            
                if (result.result) {
                    var test = JSON.parse(result.info);                   
                    //$('body').append(result.info);
                    //$("#logContainer").jqprint({
                    //    importCSS: false
                    //});
                }
            });
        }

        function Print() {
            var ID = $("#DeclarationID").val();            
            var url = location.pathname.replace(/DecList.aspx/ig, 'DecListPrint.aspx?ID=' + ID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印对单数据',
                width: '1000px',
                height: '400px'
            });
        }

        //导出Excel
        function ExportSummary() {
            var ID = $("#DeclarationID").val();
            MaskUtil.mask();
            $.post('?action=Export', {           
                ID: ID,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        if (rel.url.length > 1) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = rel.url;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    }
                });
            })
        }

    </script>


</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">商品名称: </span>
                    <input class="easyui-textbox" id="GName" style="width: 150px" />
                    <span class="lbl">项号: </span>
                    <input class="easyui-textbox" id="GNO" style="width: 150px" />
                     <span class="lbl">产地: </span>
                    <input class="easyui-textbox" id="OriginPlcae" style="width: 150px" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <a id="btnCheck" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Print()">对单</a>
                    <a id="btnExportSummary" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportSummary()">导出汇总</a>
                    <input type="hidden" id="DeclarationID" />
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 5px;">
        <table id="orders" title="商品" data-options="fitColumns:false,border:true,fit:true,toolbar:'#topBar'">
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'GNo',align:'center'" style="width: 80px;">序号</th>
                    <th data-options="field:'CodeTS',align:'center'" style="width: 120px;">商品编码</th>
                    <th data-options="field:'CiqCode',align:'center'" style="width: 100px;">检验检疫编码</th>
                    <th data-options="field:'GName',align:'left'" style="width: 200px;">商品名称</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 180px;">操作</th>
                </tr>
            </thead>
            <thead>
                <tr>
                    <th data-options="field:'GoodsBrand',align:'left'" style="width: 100px;">品牌</th>
                    <th data-options="field:'GModel',align:'left'" style="width: 500px;">规格型号</th>
                    <th data-options="field:'GQty',align:'center'" style="width: 100px;">成交数量</th>
                    <th data-options="field:'GUnit',align:'center'" style="width: 100px;">成交单位</th>
                    <th data-options="field:'FirstQty',align:'center'" style="width: 100px;">法一数量</th>
                    <th data-options="field:'FirstUnit',align:'center'" style="width: 100px;">法一单位</th>
                    <th data-options="field:'SecondQty',align:'center'" style="width: 100px;">法二数量</th>
                    <th data-options="field:'SecondUnit',align:'center'" style="width: 100px;">法二单位</th>
                    <th data-options="field:'DeclPrice',align:'center'" style="width: 120px;">单价</th>
                    <th data-options="field:'DeclTotal',align:'center'" style="width: 120px;">总价</th>
                    <th data-options="field:'TradeCurr',align:'center'" style="width: 100px;">币制</th>
                    <th data-options="field:'OriginCountry',align:'center'" style="width: 100px;">原产国</th>
                    <th data-options="field:'DestinationCountry',align:'center'" style="width: 100px;">最终目的国</th>
                    <th data-options="field:'DistrictCode',align:'center'" style="width: 100px;">境内目的地</th>
                    <th data-options="field:'DestCode',align:'center'" style="width: 100px;">目的地</th>
                    <th data-options="field:'DutyMode',align:'center'" style="width: 100px;">征免方式</th>
                    <th data-options="field:'CaseNo',align:'center'" style="width: 100px;">箱号</th>
                    <th data-options="field:'NetWt',align:'center'" style="width: 100px;">净重</th>
                    <th data-options="field:'GrossWt',align:'center'" style="width: 100px;">毛重</th>
                    <th data-options="field:'GoodsSpec',align:'left'" style="width: 100px;">货物规格</th>
                    <th data-options="field:'GoodsAttr',align:'center'" style="width: 100px;">货物属性</th>
                    <th data-options="field:'Purpose',align:'center'" style="width: 100px;">用途</th>
                    <th data-options="field:'GoodsBatch',align:'center'" style="width: 100px;">批次号</th>
                    <th data-options="field:'CiqName',align:'center'" style="width: 200px;">检验检疫名称</th>
                </tr>
            </thead>

        </table>
    </div>
</body>
</html>
