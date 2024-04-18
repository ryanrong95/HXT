<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Declaration.Declare.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--    <script>
        gvSettings.fatherMenu = '报关单(XTD)';
        gvSettings.menu = '草稿';
        gvSettings.summary = '报关单草稿';
    </script>--%>
    <script>
        var ThisAdminOriginID = '<%=this.Model.ThisAdminOriginID%>';
        var RealName = '<%=this.Model.RealName%>';
        $(function () {
            var Status = eval('(<%=this.Model.Status%>)');
            //var VoyageType = eval('(<%=this.Model.VoyageType%>)');
            var DecHeadSpecialType = eval('(<%=this.Model.DecHeadSpecialType%>)');
            var CandidateData = eval('(<%=this.Model.CandidateData%>)');

            $('#Status').combobox({
                data: Status
            });

            //var newVoyageType = [];
            //newVoyageType.push({ "TypeValue": "0", "TypeText": "全部" });
            //for (var i = 0; i < VoyageType.length; i++) {
            //    newVoyageType.push({ "TypeValue": VoyageType[i].TypeValue, "TypeText": VoyageType[i].TypeText });
            //}
            //$('#VoyageType').combobox({
            //    data: newVoyageType,
            //});
            //$('#VoyageType').combobox('setValue', "全部");

            $('#MyDecHead').change(function () {
                Search();
            });

            //报关单特殊类型下拉框初始化
            $('#DecHeadSpecialType').combobox({
                data: DecHeadSpecialType,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            //订单列表初始化
            $('#orders').myDatagrid({
                checkOnSelect: false,
                singleSelect: false,
                onCheck: function (index, row) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    //for (var i = 0; i < rows.length; i++) {
                    //    //可选制单员自己的报关单
                    //    if (rows[i].CreateDeclareAdminID == ThisAdminOriginID) {
                    //        $('#orders').myDatagrid('checkRow', i);
                    //    }
                    //    else {
                    //        $('#orders').myDatagrid('uncheckRow', i);
                    //    }
                    //}
                    //$("input[type='checkbox']")[0].checked = true
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                pageSize: 50,

                onLoadSuccess: function (data) {
                    //有核销退回记录
                    $.each(data.rows, function (index, val) {
                        if (val.IsCheckReturned == true) {
                            //红色标记
                            $("tr[datagrid-row-index=" + index + "]").find("td").css('background', 'red');
                            $("tr[datagrid-row-index=" + index + "]").find("td").css('color', 'white');
                        }
                    });
                }
            });

            //发单员选项
            $("#Selectable").combobox({
                data: CandidateData,
                required: true,
                valueField: 'value',
                textField: 'text',
                onChange: function (record) {

                },
            });
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var VoyageID = $('#VoyageID').textbox('getValue');
            //var VoyageType = $('#VoyageType').combobox('getValues')[0];
            var DecHeadSpecialType = $('#DecHeadSpecialType').combobox('getValues');
            var DecHeadSpecialTypeArray = [];
            for (var i = 0; i < DecHeadSpecialType.length; i++) {
                DecHeadSpecialTypeArray.push({ "DecHeadSpecialTypeValue": DecHeadSpecialType[i] });
            }

            var MyDecHead = $('#MyDecHead').prop("checked");

            var parm = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                VoyageID: VoyageID,
                //VoyageType: VoyageType,
                DecHeadSpecialType: JSON.stringify(DecHeadSpecialTypeArray),
                MyDecHead: MyDecHead,
            };
            $('#orders').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#VoyageID').textbox('setValue', null);
            //$('#VoyageType').combobox('setValue', "全部");
            $('#DecHeadSpecialType').combobox('clear');
            $('#MyDecHead').prop('checked', false);
            Search();
        }

        //批量转换舱单
        function Transform() {
            var ids = [];
            var rows = $('#orders').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要转换舱单的报关单！');
                return;
            }
            var hadbills = "";
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].Transformed) {
                    hadbills += rows[i].BillNo + " ";
                }
                else {
                    ids.push(rows[i].ID);
                }
            }
            //
            if (hadbills != "") {
                $.messager.alert('提示', hadbills + ' 提运单已存在！');
                return;
            }
            if (ids.length < 1) {
                $.messager.alert('提示', ' 请勾选需要转换舱单的报关单！');
                return;
            }

            id = ids.join();
            $.messager.confirm('确认', '请您再次确认转换舱单？', function (success) {
                MaskUtil.mask();//遮挡层
                if (success) {
                    $.post('?action=Transform', { ID: id }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
                            Search();
                        });
                    });
                }
            });
        }

        //批量制单
        function Make(obj) {
            var ids = [];
            var message = "";
            var rows = $('#orders').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要制单的报关单！');
                return;
            }
            var hadMark = "";
            var hadDec = "";
            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i].ID);
                //已制单
                if (rows[i].Status == "02") {
                    hadMark += rows[i].ContrNo + " ";
                }
                //已提交申请
                if (rows[i].Status != "01" && rows[i].Status != "02" && rows[i].Status != "E0") {
                    hadDec += rows[i].ContrNo + " ";
                }
            }

            if (hadDec != "") {
                $.messager.alert('error', '存在已申报的单据，无法制单！');
                return;
            }

            if (hadMark != "") {
                message = hadMark + ' 已制单，是否全部重新制单?';
            }
            else {
                if (obj == '1') {
                    message = '确认通过单一窗口接口进行"整合申报"？';
                }
                else {
                    message = '确认通过单一窗口接口进行"两步申报"？';
                }

            }

            id = ids.join();
            Split = obj == '1' ? false : true;
            $.messager.confirm('确认', message, function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Make', { ID: id, Split: Split }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层                      
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
                            Search();
                        });
                    });
                }
            });
        }

        function MakerCheck(id, orderID, VoyageID) {
            //先验证是否有未识别产地
            MaskUtil.mask();//遮挡层
            $.post('?action=CheckOrigin', { ID: id }, function (res) {
                MaskUtil.unmask();//关闭遮挡层                      
                var result = JSON.parse(res);
                if (result.success == false) {
                    $.messager.alert('提示', result.info);
                }
                else {
                    debugger;
                    if (result.info != "") {
                        $.messager.confirm("提示", result.info, function (res) {
                            if (res) {
                                //复核
                                var url = location.pathname.replace(/List.aspx/ig, 'DecCheck.aspx?ID=' + id + '&VoyageID=' + VoyageID + '&OrderID=' + orderID + '&Form=Maker');
                                $.myWindow({
                                    iconCls: "",
                                    url: url,
                                    noheader: false,
                                    title: '复核单据',
                                    width: '1500px',
                                    height: '900px',
                                    onClose: function () {
                                        Search();
                                    }
                                });
                            }
                        });
                    } else {
                        //复核
                        var url = location.pathname.replace(/List.aspx/ig, 'DecCheck.aspx?ID=' + id + '&VoyageID=' + VoyageID + '&OrderID=' + orderID + '&Form=Maker');
                        $.myWindow({
                            iconCls: "",
                            url: url,
                            noheader: false,
                            title: '复核单据',
                            width: '1500px',
                            height: '900px',
                            onClose: function () {
                                Search();
                            }
                        });
                    }
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            if (row.CreateDeclareAdminID == ThisAdminOriginID || RealName == '魏晓毅') {
                var buttons =
                    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditHead(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">编辑</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

                //buttons +=
                //    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="DownLoadExcel(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                //    '<span class="l-btn-text">导出表格</span>' +
                //    '<span class="l-btn-icon icon-yg-excelExport">&nbsp;</span>' +
                //    '</span>' +
                //    '</a>';

                buttons +=
                    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="MakerCheck(\'' + row.ID + '\',\'' + row.OrderID + '\',\'' + row.VoyageID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">复核</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        function InspQuarNameShow(val, row, index) {
            var result = '';
            if (row.IsCharterBus == true) {
                result += '包车/';
            } else {
                //result += '-/';
            }

            if (row.IsHighValue == true) {
                result += '高价值/';
            } else {
                //result += '-/';
            }

            if (row.IsInspection == true) {
                result += '商检/';
            } else {
                //result += '-/';
            }

            if (row.IsQuarantine == true) {
                result += '检疫/';
            } else {
                //result += '-/';
            }

            if (row.IsCCC == true) {
                result += '3C/';
            } else {
                //result += '-/';
            }

            if (row.IsOrigin == true) {
                result += '加征/';
            } else {
                //result += '-';
            }

            if (row.IsSenOrigin == true) {
                result += '敏感产地';
            } else {
                //result += '-';
            }

            return result;
        }

        //提货地址
        function ConsigneeAddress(val, row, index) {
            var warehouse = "";
            var address = row["ConsigneeAddress"];

            if (row["ClientType"] == "自有公司") {
                if (address == null) {
                    //历史记录
                    if (!row["ClientName"].indexOf('比') < 0) {
                        warehouse = "-";
                    }
                    else {
                        warehouse = "志成";
                    }
                }
                //内单
                else if (address.indexOf("中美") > 0) {
                    warehouse = "中美";
                }
                else if (address.indexOf("怡生") > 0) {
                    warehouse = "怡生";
                }
                else if (address.indexOf("志成") > 0) {
                    warehouse = "志成";
                }
                else {
                    warehouse = "-";
                }

            }
            else {
                //外单  香港新库房 ryan 20210816
                warehouse = "日昇";
            }

            return warehouse;
        }

        //计算一些求和, 显示在界面上
        function calcSomeSum(rows) {
            //console.log(rows);
            var SplitInfo = [];
            var packNoSum = 0; //总件数
            var totalDeclarePriceSum = 0; //总金额
            var totalQtySum = 0; //总数量
            var totalModelQtySum = 0; //总型号数量
            var totalGrossWeightSum = 0; //总毛重

            for (var i = 0; i < rows.length; i++) {
                var currentPackNo = Number(rows[i].PackNo);
                var currentTotalDeclarePrice = Number(Number(rows[i].TotalAmount).toFixed(4));
                var currentTotalQty = Number(rows[i].TotalQty);
                var currentTotalModelQty = Number(rows[i].ModelAmount);
                var currentTotalGrossWeight = Number(Number(rows[i].GrossWt).toFixed(2));

                packNoSum += currentPackNo;
                totalDeclarePriceSum += currentTotalDeclarePrice;
                totalQtySum += currentTotalQty;
                totalModelQtySum += currentTotalModelQty;
                totalGrossWeightSum += currentTotalGrossWeight;
                SplitInfo.push({
                    OrderID: rows[i].OrderID,
                    PackBoxes: rows[i].PackBox,
                });
            }

            $("#PackNo-sum").html(packNoSum); //总件数
            $("#TotalDeclarePrice-sum").html(totalDeclarePriceSum.toFixed(4)); //总金额
            $("#TotalQty-sum").html(totalQtySum); //总数量
            $("#TotalModelQty-sum").html(totalModelQtySum); //总型号数量
            $("#TotalGrossWeight-sum").html(totalGrossWeightSum.toFixed(2)); //总毛重

            $.post('?action=ActualBoxNumbers', { Model: JSON.stringify(SplitInfo) }, function (data) {
                var Result = JSON.parse(data);
                $("#ActualPackNo-sum").html(Result.totalPack); //总件数
            });

        }
    </script>

    <script>
        function EditHead(id, OrderID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Declare.aspx?ID=' + id + '&Source=Edit&OrderID=' + OrderID);
            window.location = url;
        }

        function DownLoadExcel(ID, OrderID) {

            $.messager.confirm('确认', '请您再次确认导出Excel!', function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    //var ID = $("#DecHeadID").val();
                    $.post("?action=DownloadExcel", { ID: ID }, function (data) {
                        MaskUtil.unmask();
                        var result = JSON.parse(data);
                        $('#orders').datagrid('reload');
                        if (result.result) {
                            Download(result.url);
                            $.messager.alert('消息', result.info);
                        } else {
                            $.messager.alert('消息', result.info);
                        }
                    });
                }
            });

            /*
            $("#Selectable").combobox("setValue", null);

            $('#downloadExcel-dialog').dialog({
                title: '确认',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        if (!$("#form1").form('validate')) {
                            return;
                        }
                        
                        var CustomSubmiterAdminID = $("#Selectable").combobox("getValue");
                        
                        MaskUtil.mask();

                        $.post("?action=DownloadExcel", { ID: ID, CustomSubmiterAdminID: CustomSubmiterAdminID, }, function (data) {
                            MaskUtil.unmask();
                            var result = JSON.parse(data);
                            if (result.result) {
                                Download(result.url);
                                $('#downloadExcel-dialog').dialog('close');
                                $('#orders').datagrid('reload');
                                $.messager.alert('消息', result.info);
                            } else {
                                $.messager.alert('消息', result.info);
                            }
                        });
                    }
                }, {
                    id: 'btn-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#downloadExcel-dialog').dialog('close');
                    }
                }],
            });

            $('#downloadExcel-dialog').window('center'); //dialog 居中
            */
        }

        function Download(Url) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = Url;
            a.download = "";
            a.click();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <%--<a id="btnCreate" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Make('1')">整合申报</a>
            <a id="btnCreateSplit" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Make('2')">两步申报</a>
            <a id="btnTransform" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Transform()">转换舱单</a>--%>
            <%--<a id="btnSucceed" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Succeed()">报关完成(暂用)</a>--%>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 33px;">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />
                    <%--订单编号id暂时未知--%>
                    <span class="lbl" style="margin-left: 27px;">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <%--                    <span class="lbl">预录入号: </span>
                    <input class="easyui-textbox" id="PreEntryId" />--%>
                    <%-- <span class="lbl">单据状态: </span>
                    <input class="easyui-combobox" id="Status" name="Status" data-options="valueField:'Value',textField:'Text'" />--%>
                    <span class="lbl" style="margin-left: 10px;">报关单特殊类型: </span>
                    <input class="easyui-combobox" id="DecHeadSpecialType" name="DecHeadSpecialType" data-options="multiple:true,panelHeight:'auto'," />
                    <%--<span style="margin-left: 10px;">
                        <input type="checkbox" name="MyDecHead" id="MyDecHead" style="display: none;"/>
                        <label for="MyDecHead">我的报关单</label>
                    </span>--%>
                </li>
            </ul>
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 10px;">运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageID" />
                    <%-- <span class="lbl" style="margin-left: 26px;">运输类型: </span>
                    <input class="easyui-combobox" id="VoyageType" name="VoyageType" data-options="valueField:'TypeValue',textField:'TypeText',editable:false," />--%>
                    <a style="margin-left: 10px;" id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <span id="sum-container" style="margin-left: 55px;">
                        <label>合计</label>
                        <label style="margin-left: 25px;">总件数:</label>
                        <label id="PackNo-sum">0</label>
                        <label style="margin-left: 25px;">实际件数:</label>
                        <label id="ActualPackNo-sum">0</label>
                        <label style="margin-left: 25px;">总金额:</label>
                        <label id="TotalDeclarePrice-sum">0.0000</label>
                        <label style="margin-left: 25px;">总数量:</label>
                        <label id="TotalQty-sum">0</label>
                        <label style="margin-left: 25px;">总型号数量:</label>
                        <label id="TotalModelQty-sum">0</label>
                        <label style="margin-left: 25px;">总毛重:</label>
                        <label id="TotalGrossWeight-sum">0</label>
                    </span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="草稿" data-options="fitColumns:true,fit:true,singleSelect:true,border:false,nowrap:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNO',align:'left'" style="width: 9%">合同号</th>
                    <th data-options="field:'BillNo',align:'left'" style="width: 8%">提(运)单号</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 4%">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 12%">客户名称</th>
                    <th data-options="field:'InspQuarName',align:'center',formatter:InspQuarNameShow" style="width: 8%">报关单特殊类型</th>
                    <th data-options="field:'VoyageID',align:'left'" style="width: 6%">运输批次号</th>
                    <th data-options="field:'VoyageType',align:'left'" style="width: 4%">运输类型</th>
                    <%--                    <th data-options="field:'EntryId',align:'center'" style="width: 100px;">海关编号</th>
                    <th data-options="field:'PreEntryId',align:'center'" style="width: 100px;">预录入号</th>--%>
                    <%--<th data-options="field:'AgentName',align:'left'" style="width: 18%">委托报关企业</th>--%>
                    <%--<th data-options="field:'ConsignorName',align:'left'" style="width: 16%">境外发货人</th>--%>
                    <%--                    <th data-options="field:'ConsigneeName',align:'center'" style="width: 100px;">境内收货人</th>--%>
                    <%--<th data-options="field:'IsInspection',align:'center'" style="width: 6%">商检/检疫</th>--%>
                    <th data-options="field:'ConsigneeAddress',align:'left',formatter:ConsigneeAddress" style="width: 4%">提货库房</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 4%">件数</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%">录入时间</th>
                    <%--<th data-options="field:'InputerID',align:'center'" style="width: 5%">制单员</th>
                       <th data-options="field:'StatusName',align:'center'" style="width: 100px;">单据状态</th>--%>
                    <th data-options="field:'CreateDeclareAdminName',align:'center'" style="width: 5%">制单员</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <%--<div id="downloadExcel-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div style="margin: 15px 15px 15px 15px;">
            <span>请您再次确认导出Excel!</span>
        </div>
        <div style="margin: 15px 15px 15px 15px;">
            <form id="form1" runat="server">
                <span>请选择发单员：</span>
                <input class="easyui-combobox" id="Selectable" name="Bank" panelHeight="120"
                                data-options="required:true,editable:false" style="height: 30px; width: 180px"" />
            </form>
        </div>
    </div>--%>
</body>
</html>
