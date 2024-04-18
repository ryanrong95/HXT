<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnStockDetail.aspx.cs" Inherits="WebApp.SZWarehouse.Entry.OnStockDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>上架详情-入库通知(SZ)</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '入库通知(SZ)';
        gvSettings.menu = '';
        gvSettings.summary = '';
    </script>--%>
    <style type="text/css">
        .border-table {
            line-height: 15px;
            border-collapse: collapse;
            border: 1px solid lightgray;
            width: 100%;
            text-align: center;
        }

            .border-table tr {
                height: 25px;
            }

                .border-table tr td {
                    font-weight: normal;
                    border: 1px solid lightgray;
                    text-align: left;
                    padding: 5px;
                    font: 14px Arial,Verdana,'微软雅黑','宋体';
                }

                    .border-table tr td:nth-child(1) {
                        width: 15%;
                    }

                    .border-table tr td:nth-child(2) {
                        width: 35%;
                    }

                    .border-table tr td:nth-child(3) {
                        width: 15%;
                    }

                    .border-table tr td:nth-child(4) {
                        width: 35%;
                    }

                    .datagrid-row-selected {
			background: #ffffff;
			color: #000000;
		}

		/*.datagrid-row-over {
			background: #ffffff;
			color: #000000;
		}*/
    </style>
    <script>
        var OnStockListPageNumber = <%=this.Model.OnStockListPageNumber%>;
        var OnStockListPageSize = <%=this.Model.OnStockListPageSize%>;
        var OnStockListScVoyageID = '<%=this.Model.OnStockListScVoyageID%>';
        var OnStockListScCarrierName = '<%=this.Model.OnStockListScCarrierName%>';

        var VoyageInfo = eval('(<%=this.Model.VoyageInfo%>)');
        var InternalClients = eval('(<%=this.Model.InternalClients%>)');
        var TotalPackNo = eval('(<%=this.Model.TotalPackNo%>)');
        var TotalGrossWt = eval('(<%=this.Model.TotalGrossWt%>)');

        $(function () {
            document.getElementById('VoyageNo').innerHTML = VoyageInfo['VoyageNo'];
            document.getElementById('CarrierName').innerHTML = VoyageInfo['Carrier'];
            document.getElementById('HKLicense').innerHTML = VoyageInfo['HKLicense'];
            document.getElementById('DriverName').innerHTML = VoyageInfo['DriverName'];
            document.getElementById('TransportTime').innerHTML = VoyageInfo['TransportTime'];
            document.getElementById('VoyageType').innerHTML = VoyageInfo['VoyageType'];

            document.getElementById('TotalPackNo').innerHTML = TotalPackNo;
            document.getElementById('TotalGrossWt').innerHTML = TotalGrossWt;

            //内单客户下拉框初始化
            $('#InternalClients').combobox({
                disabled:true,
                valueField: 'ID',
                textField: 'Name',
                data: InternalClients,
                onSelect: function () {
                    <%--var type = $('input:radio[name="Order"]:checked').val();
                    if (type == '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                        var params = {
                            Type: type,
                            ClientID: $(this).combobox('getValue'),
                        };
                        $('#voyageDetails').myDatagrid('search', params);

                        params['ID'] = voyage['ID'];
                        $.post('?action=dataVoyageSubtotal', params, function (res) {
                            var result = JSON.parse(res);
                            document.getElementById('TotalPackNo').innerHTML = result.TotalPackNo;
                            document.getElementById('TotalQuantity').innerHTML = result.TotalQuantity;
                            document.getElementById('TotalGrossWt').innerHTML = result.TotalGrossWt;
                            document.getElementById('TotalAmount').innerHTML = result.TotalAmount;
                        });
                    }--%>
                }
            });

            //内外单用户 checkbox 设置为单选（可都不选）
            $("[name='ClientType']:checkbox").click(function () {
                $(this).attr("checked", true);
                $(this).siblings().attr("checked", false);
            });

            $("[name='ClientType']:checkbox").change(function () {
                var clientType = $('input:checkbox[name="ClientType"]:checked').val();
                //选择不是内单用户时，清空客户下拉框, 并且禁用下拉框
                if (clientType != '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                    $('#InternalClients').combobox('clear');
                    $('#InternalClients').combobox('disable');
                } else {
                    $('#InternalClients').combobox('enable');
                }
            });

            $('#boxinfo-datagrid').datagrid({
                url: '?action=BoxInfoList&VoyageID=' + VoyageInfo["VoyageID"],
                onLoadSuccess: function (data) {
                    //是否可以显示 完成按钮
                    var isAllOnStock = 1;
                    if (data.rows.length > 0) {
                        for (var i = 0; i < data.rows.length; i++) {
                            if (data.rows[i].IsOnStockValue == false) {
                                isAllOnStock = 0;
                                break;
                            }
                        }
                    } else {
                        isAllOnStock = 0;
                    }

                    if (isAllOnStock == 1 && data.rows.length == data.total) {
                        $('#btnComplete').linkbutton({ disabled: false, });
                    }

                    //合并单元格
                    //result = result.OrderBy(t => t.ClientCode).ThenBy(t => t.OrderID).ThenBy(t => t.PackingDate).ThenBy(t => t.BoxIndex);
                    //var listResult = result.OrderBy(t => t.PackingDate).ThenBy(t => t.BoxIndex).ThenBy(t => t.ClientCode).ThenBy(t => t.OrderID).ToList();
			        var mark = 1;
			        for (var i = 0; i < data.rows.length; i++) {
				        //合并箱号
				        if (i > 0) {
					        if (data.rows[i]['PackingDate'] == data.rows[i - 1]['PackingDate'] && data.rows[i]['BoxIndex'] == data.rows[i - 1]['BoxIndex']) {
						        mark += 1;
						        $("#boxinfo-datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'BoxIndex',
							        rowspan: mark
						        });
						        $("#boxinfo-datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'PackingDate',
							        rowspan: mark
                                });
                                $("#boxinfo-datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'StockCode',
							        rowspan: mark
                                });
                                $("#boxinfo-datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'IsOnStock',
							        rowspan: mark
                                });
                                $("#boxinfo-datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'CheckBox',
							        rowspan: mark
						        });
					        }
					        else {
						        mark = 1;
					        }
				        }
			        }

			        //mark = 1;
			        //for (var i = 0; i < data.rows.length; i++) {
				       // //合并箱号
				       // if (i > 0) {
					      //  if (data.rows[i]['OrderID'] == data.rows[i - 1]['OrderID']) {
						     //   mark += 1;
						     //   $("#boxinfo-datagrid").datagrid('mergeCells', {
							    //    index: i + 1 - mark,
							    //    field: 'OrderID',
							    //    rowspan: mark
						     //   });
					      //  }
					      //  else {
						     //   mark = 1;
					      //  }
				       // }
			        //}

                },
                onClickRow: function (rowIndex, rowData) {
                    $("#boxinfo-datagrid").datagrid('unselectRow', rowIndex);
                },
            });

            InitOnStockDialog();
        });

        //初始化上架弹框
        function InitOnStockDialog() {
            $('#onstock-dialog').dialog({
                buttons: [{
                    text: '提交',
                    iconCls: 'icon-save',
                    handler: function () {
                        var isValid = $("#formOnStock").form("enableValidation").form("validate");
                        if (!isValid) {
                            return;
                        }

                        $('#onstock-dialog').dialog('close');

                        //批量上架
                        OnStockMany();
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $('#onstock-dialog').dialog('close');
                    }
                }]
            });
        }

        //查询
        function Search() {
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var PackingDate = $('#PackingDate').datebox('getValue');
            var BoxIndex = $('#BoxIndex').textbox('getValue');
            var ClientType = $('input:checkbox[name="ClientType"]:checked').val();
            var ClientID = $("#InternalClients").combobox('getValue');

            //如果不是内单用户，清空 ClientID
            if (ClientType != '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                ClientID = '';
            }
            //如果 ClientType 为 undefined，则给 ClientType 赋值为 ''
            if (ClientType == undefined) {
                ClientType = '';
            }

            var url = '?action=BoxInfoList&VoyageID=' + VoyageInfo["VoyageID"] + '&ClientCode=' + ClientCode + '&ClientName=' + ClientName
                + '&PackingDate=' + PackingDate + '&BoxIndex=' + BoxIndex
                + '&ClientType=' + ClientType + '&ClientID=' + ClientID;
            $('#boxinfo-datagrid').datagrid({
                url: url,
            });
        }

        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#PackingDate').datebox('setValue', null);
            $('#BoxIndex').textbox('setValue', null);
            $("[name='ClientType']:checkbox").siblings().attr("checked", false);
            $('#InternalClients').combobox('clear');
            $('#InternalClients').combobox('disable');
            Search();
        }

        //返回
        function Return() {
            var url = location.pathname.replace(/OnStockDetail.aspx/ig, 'OnStockList.aspx')
                + "?pageNumber=" + OnStockListPageNumber + "&pageSize=" + OnStockListPageSize
                + "&OnStockListScVoyageID=" + encodeURI(OnStockListScVoyageID) + "&OnStockListScCarrierName=" + encodeURI(OnStockListScCarrierName);
            window.location = url;
        }

        //显示上架弹框
        function ShowOnStockDialog() {
            var checkedboxinfos = $('#boxinfo-datagrid').datagrid('getChecked');
            if (checkedboxinfos.length <= 0) {
                $.messager.alert('提示', "请选择要上架的项目");
                return;
            }

            $("#StockCode").textbox('setValue', null);
            $('#onstock-dialog').dialog('open');
        }

        //批量上架
        function OnStockMany() {
            //var allboxnfos = $('#boxinfo-datagrid').datagrid('getData');

            //var checkedboxinfos = $('#boxinfo-datagrid').datagrid('getChecked');

            //var boxInfos = [];

            //$.each(checkedboxinfos, function (index, item) {
            //    //boxInfos.push({
            //    //    "OrderID": item.OrderID,
            //    //    "BoxIndex": item.BoxIndex,
            //    //});

            //    for (var i = 0; i < allboxnfos.rows.length; i++) {
            //        if (allboxnfos.rows[i].PackingDate == item.PackingDate && allboxnfos.rows[i].BoxIndex == item.BoxIndex) {
            //            boxInfos.push({
            //                "OrderID": allboxnfos.rows[i].OrderID,
            //                "BoxIndex": allboxnfos.rows[i].BoxIndex,
            //            });
            //        }
            //    }
            //});

            

            var checkedboxinfos = $('#boxinfo-datagrid').datagrid('getChecked');

            var boxInfos = [];

            $.each(checkedboxinfos, function (index, item) {
                boxInfos.push({
                    "OrderID": item.OrderID,
                    "BoxIndex": item.BoxIndex,
                });
            });

            //提交
            var url = location.pathname.replace(/OnStockDetail.aspx/ig, 'OnStockDetail.aspx?action=OnStock');
            var params = {
                "VoyageID": VoyageInfo['VoyageID'],
                "StockCode": $("#StockCode").textbox('getValue'),
                "BoxInfo": JSON.stringify(boxInfos),
            };

            MaskUtil.mask();
            $.post(url, params, function (res) {
                MaskUtil.unmask();

                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    //刷新列表
                    $('#boxinfo-datagrid').datagrid('reload');
                } else {
                    $.messager.alert('提示', resData.message);
                }
            });
        }

        //该运输批次上架完成
        function DoneOnStock() {
            $.messager.confirm('确认完成', '确定完成该运输批次的上架吗？', function(r){
                if (r) {
		            var url = location.pathname.replace(/OnStockDetail.aspx/ig, 'OnStockDetail.aspx?action=Complete');
                    var params = {
                        "VoyageID": VoyageInfo['VoyageID'],
                    };

                    MaskUtil.mask();
                    $.post(url, params, function (res) {
                        MaskUtil.unmask();

                        var resData = JSON.parse(res);
                        if (resData.success == "true") {
                            $.messager.alert({ 
　　　　                        title:'完成', 
　　　　                        msg:'运输批次号 ' + VoyageInfo['VoyageID'] + ' 上架完成',
　　　　                        icon: 'info',
　　　　                        width: 300,
　　　　                        top:200 , //与上边距的距离
                                fn:function(){
                                    var url = location.pathname.replace(/OnStockDetail.aspx/ig, 'OnStockList.aspx')
                                        + "?pageNumber=" + OnStockListPageNumber + "&pageSize=" + OnStockListPageSize
                                        + "&OnStockListScVoyageID=" + encodeURI(OnStockListScVoyageID) + "&OnStockListScCarrierName=" + encodeURI(OnStockListScCarrierName);
                                    window.location = url;
                                }
                            });
                        } else {
                            $.messager.alert('提示', resData.message);
                        }
                    });
	            }
            });
        }

        //导出入库单
        function ExportEntryBill() {
            var params = {
                VoyageID: VoyageInfo['VoyageID'],
            };

            $.post('?action=ExportEntryBill', params, function (rel) {
                // var result = JSON.parse(res);
                // MaskUtil.unmask();
                var rel = JSON.parse(rel);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
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
                });
            });
        }
    </script>
</head>
<body>
    <div style="padding: 20px;">
        <div style="background-color: white; margin-bottom: 1px">
            <div style="padding: 5px; margin-bottom: 1px">
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportEntryBill()" style="margin-left: 5px;">导出入库单</a>
                <a id="btnComplete" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok',disabled:true," onclick="DoneOnStock()" style="margin-left: 5px;">完成</a>
            </div>
        </div>

        <table class="border-table" style="width: 100%; margin-top: 5px; margin-bottom: 5px">
            <tr>
                <td style="background-color: whitesmoke">运输批次号</td>
                <td id="VoyageNo"></td>
                <td style="background-color: whitesmoke">承运商</td>
                <td id="CarrierName"></td>
            </tr>
            <tr>
                <td style="background-color: whitesmoke">车牌号</td>
                <td id="HKLicense"></td>
                <td style="background-color: whitesmoke">司机姓名</td>
                <td id="DriverName"></td>
            </tr>
            <tr>
                <td style="background-color: whitesmoke">运输时间</td>
                <td id="TransportTime"></td>
                <td style="background-color: whitesmoke">运输类型</td>
                <td id="VoyageType"></td>
            </tr>
            <tr>
                <td style="background-color: whitesmoke">总件数</td>
                <td id="TotalPackNo"></td>
                <td style="background-color: whitesmoke">总毛重(KG)</td>
                <td id="TotalGrossWt"></td>
            </tr>
        </table>

        <div id="topBar-boxinfo-datagrid">
            <div id="search">
                <ul>
                    <li>
                        <input type="checkbox" id="ExternalClient" name="ClientType" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" style="display: none;" />
                        <label for="ExternalClient" style="margin-left: 20px;">B类</label>
                        <input type="checkbox" id="InternalClient" name="ClientType" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" style="display: none;" />
                        <label for="InternalClient" style="margin-left: 20px;">A类</label>
                        <input id="InternalClients" class="easyui-combobox" data-options="editable:false," style="width: 280px" />
                    </li>
                    <li>
                        <span class="lbl" style="margin-left: 20px; margin-right: 5px;">客户编号: </span>
                        <input class="easyui-textbox search" data-options="" id="ClientCode" style="width: 339px" />
                        <span class="lbl" style="margin-left: 20px; margin-right: 5px;">客户名称: </span>
                        <input class="easyui-textbox search" data-options="" id="ClientName" style="width: 339px" />

                    </li>
                    <li>
                        <span class="lbl" style="margin-left: 20px; margin-right: 5px;">装箱日期: </span>
                        <input class="easyui-datebox search" data-options="" id="PackingDate" style="width: 339px" />
                        <span class="lbl" style="margin-left: 43px; margin-right: 5px;">箱号: </span>
                        <input class="easyui-textbox search" data-options="" id="BoxIndex" style="width: 339px" />
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </li>
                    <li>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-filter'" style="margin-left: 18px;" onclick="ShowOnStockDialog()">上架</a>
                    </li>
                </ul>
            </div>
        </div>
        <div id="data" data-options="region:'center',border:false">
            <table id="boxinfo-datagrid" class="easyui-datagrid" title="明细" data-options="
                checkOnSelect:false,
                border:true,
                nowrap:false,
                fitColumns:true,
                fit:false,
                scrollbarSize:0,
                singleSelect:false,
                toolbar:'#topBar-boxinfo-datagrid'">
                <thead>
                    <tr>
                        <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                        <th field="OrderID" data-options="align:'center'" style="width: 60px">订单编号</th>
                        <th field="ClientCode" data-options="align:'center'" style="width: 40px">客户编号</th>
                        <th field="ClientName" data-options="align:'left'" style="width: 100px">客户名称</th>
                        <th field="PackingDate" data-options="align:'center'" style="width: 50px">装箱日期</th>
                        <th field="BoxIndex" data-options="align:'center'" style="width: 50px">箱号</th>
                        <th field="StockCode" data-options="align:'center'" style="width: 50px">库位号</th>
                        <th field="IsOnStock" data-options="align:'center'" style="width: 50px">状态</th>
                    </tr>
                </thead>
            </table>
        </div>

    </div>

    <div id="onstock-dialog" class="easyui-dialog" title="上架" style="width:330px;height:160px;" 
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <div style="margin-top: 28px;   ">
            <form id="formOnStock">
                <span class="lbl" style="margin-left: 20px; margin-right: 5px;">库位号: </span>
                <input class="easyui-textbox" data-options="required:true,tipPosition:'bottom'," id="StockCode" style="width: 200px;" />
            </form
        </div>
    </div>
</body>
</html>
