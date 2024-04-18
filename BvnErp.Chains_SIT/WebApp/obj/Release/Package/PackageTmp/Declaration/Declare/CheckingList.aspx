<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckingList.aspx.cs" Inherits="WebApp.Declaration.Declare.CheckingList" %>

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
                pageSize: 50,
                //onLoadSuccess: function (data) {
                //    if (data.rows.length > 0) {
                //        //循环判断可勾选行
                //        for (var i = 0; i < data.rows.length; i++) {
                //            //可选制单员自己的报关单
                //            if (data.rows[i].CreateDeclareAdminID == ThisAdminOriginID) {
                //                $("input[type='checkbox']")[i + 1].disabled = false;
                //            }
                //            else {
                //                $("input[type='checkbox']")[i + 1].disabled = 'disabled';
                //            }
                //        }
                //    }
                //},
                onCheckAll: function (rows) {
                    for (var i = 0; i < rows.length; i++) {
                        //可选制单员自己的报关单
                        if (rows[i].CreateDeclareAdminID == ThisAdminOriginID) {
                            $('#orders').myDatagrid('checkRow', i);
                        }
                        else {
                            $('#orders').myDatagrid('uncheckRow', i);
                        }
                    }
                    $("input[type='checkbox']")[0].checked = true
                },
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
            var url = location.pathname.replace(/CheckingList.aspx/ig, 'DecCheck.aspx?ID=' + id + '&VoyageID=' + VoyageID + '&OrderID=' + orderID + '&Form=Checker');
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

        //查看订单
        function SearchHead(id, OrderID) {
            var url = location.pathname.replace(/CheckingList.aspx/ig, 'Declare.aspx?ID=' + id + '&Source=View&SourcePage=Checking&OrderID=' + OrderID);
            window.location = url;
        }

        //回执订单
        function ReceiptHead(id, ContrNo) {
            var url = location.pathname.replace(/CheckingList.aspx/ig, 'DecTrace.aspx?ID=' + id + '&ContrNo=' + ContrNo);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '回执信息',
                width: '1000px',
                height: '550px'
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';
            //var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:1px" onclick="SearchHead(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">查看</span>' +
            //    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';

            if (row.DoubleCheckerAdminID == ThisAdminOriginID) {
                //var buttons =
                //    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditHead(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                //    '<span class="l-btn-text">编辑</span>' +
                //    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                //    '</span>' +
                //    '</a>';

                buttons +=
                    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="DownLoadExcel(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">导出表格</span>' +
                    '<span class="l-btn-icon icon-yg-excelExport">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

                buttons +=
                    '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="MakerCheck(\'' + row.ID + '\',\'' + row.OrderID + '\',\'' + row.VoyageID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">复核</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:1px" onclick="ReceiptHead(\'' + row.ID + '\',\'' + row.ContrNO + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">回执</span>' +
                '<span class="l-btn-icon icon-help">&nbsp;</span>' +
                '</span>' +
                '</a>';

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
    </script>

    <script>
        function EditHead(id, OrderID) {
            var url = location.pathname.replace(/CheckingList.aspx/ig, 'Declare.aspx?ID=' + id + '&Source=Edit&OrderID=' + OrderID);
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
        <div id="search">
            <ul>
                <li>
                    <span class="lbl" style="margin-left: 33px;">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />
                    <span class="lbl" style="margin-left: 27px;">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl" style="margin-left: 10px;">报关单特殊类型: </span>
                    <input class="easyui-combobox" id="DecHeadSpecialType" name="DecHeadSpecialType" data-options="multiple:true,panelHeight:'auto'," />

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
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="待复核" data-options="fitColumns:true,fit:true,singleSelect:true,border:false,nowrap:false,toolbar:'#topBar'">
            <thead>
                <tr>
                    <%-- <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>--%>
                    <th data-options="field:'ContrNO',align:'left'" style="width: 9%">合同号</th>
                    <th data-options="field:'BillNo',align:'left'" style="width: 8%">提(运)单号</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 4%">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 12%">客户名称</th>
                    <th data-options="field:'InspQuarName',align:'center',formatter:InspQuarNameShow" style="width: 8%">报关单特殊类型</th>
                    <th data-options="field:'VoyageID',align:'left'" style="width: 6%">运输批次号</th>
                    <th data-options="field:'VoyageType',align:'left'" style="width: 4%">运输类型</th>
                    <th data-options="field:'ConsigneeAddress',align:'left',formatter:ConsigneeAddress" style="width: 4%">提货库房</th>
                    <th data-options="field:'PackNo',align:'center'" style="width: 4%">件数</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%">录入时间</th>
                    <th data-options="field:'InputerID',align:'center'" style="width: 5%">制单员</th>
                    <th data-options="field:'DoubleCheckerAdminName',align:'center'" style="width: 5%">复核员</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
