<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckedList.aspx.cs" Inherits="WebApp.Declaration.Declare.CheckedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '报关单(XTD)';
        gvSettings.menu = '表格申报';
        gvSettings.summary = '报关单已制单';
    </script>--%>
    <style>
        .guaratee {
            border: solid 1px lightgray;
            padding: 0px 10px;
            margin-left: 20px;
        }
        .guarateered {
            border: solid 1px lightgray;
            padding: 0px 10px;
            margin-left: 20px;
            color: red;
        }

    </style>
    <script type="text/javascript">
        var BaseCusReceiptCode = eval('(<%=this.Model.CusReceiptCodeData%>)');
       <%-- var VoyageType = eval('(<%=this.Model.VoyageType%>)');--%>
        var DecHeadSpecialType = eval('(<%=this.Model.DecHeadSpecialType%>)');
        var ThisAdminOriginID = '<%=this.Model.ThisAdminOriginID%>';

        $(function () {
            //下拉框数据初始化
            $('#BaseCusReceiptCode').combobox({
                data: BaseCusReceiptCode
            });
            setCurrentDate();
            var StartDateLoad = $('#StartDate').datebox('getValue');
            var EndDateLoad = $('#EndDate').datebox('getValue');

            //代理订单列表初始化
            $('#datagraid').myDatagrid({
                checkOnSelect: false,
                singleSelect: false,
                pageSize: 50,
                queryParams: { action: 'data', StartDate: StartDateLoad, EndDate: EndDateLoad },
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (data.rows.length > 0) {
                        //循环判断可勾选行
                        for (var i = 0; i < data.rows.length; i++) {
                            //可选发单员自己的报关单
                            if (data.rows[i].CustomSubmiterAdminID == ThisAdminOriginID) {
                                $("input[type='checkbox']")[i + 1].disabled = false;
                            }
                            else {
                                $("input[type='checkbox']")[i + 1].disabled = 'disabled';
                            }
                        }
                    }

                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");

                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");

                        }
                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }
                        $("#viewFileDialog").window('open').window('center');
                    });
                },
                onCheck: function (index, row) {
                    //calcSomeSum($('#datagraid').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    //calcSomeSum($('#datagraid').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    for (var i = 0; i < rows.length; i++) {
                        //可选发单员自己的报关单
                        if (rows[i].CustomSubmiterAdminID == ThisAdminOriginID) {
                            $('#datagraid').myDatagrid('checkRow', i);
                        }
                        else {
                            $('#datagraid').myDatagrid('uncheckRow', i);
                        }
                    }
                    $("input[type='checkbox']")[0].checked = true

                    //calcSomeSum($('#datagraid').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    //calcSomeSum($('#datagraid').datagrid('getChecked'));
                },
            });

            ////运输类型下拉框初始化
            //var newVoyageType = [];
            //newVoyageType.push({ "TypeValue": "0", "TypeText": "全部" });
            //for (var i = 0; i < VoyageType.length; i++) {
            //    newVoyageType.push({ "TypeValue": VoyageType[i].TypeValue, "TypeText": VoyageType[i].TypeText });
            //}
            //$('#VoyageType').combobox({
            //    data: newVoyageType,
            //});
            //$('#VoyageType').combobox('setValue', "全部");

            //报关单特殊类型下拉框初始化
            $('#DecHeadSpecialType').combobox({
                data: DecHeadSpecialType,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            GetGuarantees();

            $('#setGuarantee-dialog').dialog({
                buttons: [{
                    text: '提交',
                    iconCls: 'icon-save',
                    handler: function () {
                        //获取选择的 orderId, 这肯定会有, 打开窗口的动作已经限制, 会有多个
                        var decHeadCheckedItems = $('#datagraid').datagrid('getChecked');
                        var decHeads = [];
                        $.each(decHeadCheckedItems, function (index, item) {
                            decHeads.push({
                                "DecHeadID": item.ID,
                                "OrderID": item.OrderID
                            });
                        });

                        //获取选择的 保函, 这只有一个
                        var guaCheckedItems = $('#setGuarantee-datagrid').datagrid('getChecked');
                        if (guaCheckedItems == null || guaCheckedItems.length <= 0) {
                            $.messager.alert('提示', '请选择保函！', 'info');
                            return;
                        }
                        var guaranteeNo = guaCheckedItems[0].ID;

                        //提交
                        var url = location.pathname + "?action=SubmitGuaranteeNo";
                        var params = {
                            "GuaranteeNo": guaranteeNo,
                            "DecHeads": JSON.stringify(decHeads),
                        };

                        $('#setGuarantee-dialog').dialog('close');
                        MaskUtil.mask();
                        $.post(url, params, function (res) {
                            MaskUtil.unmask();
                            var resData = JSON.parse(res);
                            if (resData.success == "true") {
                                //重新加载 orders 表格的当前页
                                $('#datagraid').datagrid('reload');

                                var noRow = [];
                            } else {
                                $.messager.alert('提示', resData.message);
                            }
                        });


                    }
                },
                {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $('#setGuarantee-dialog').dialog('close');
                        //重新加载 orders 表格的当前页
                        $('#datagraid').datagrid('reload');
                        var noRow = [];
                    }
                }]
            });


        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var SeqNo = $('#SeqNo').textbox('getValue');
            var BaseCusReceiptCode = $('#BaseCusReceiptCode').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var VoyageID = $('#VoyageID').textbox('getValue');
            //var VoyageType = $('#VoyageType').combobox('getValues')[0];

            var DecHeadSpecialType = $('#DecHeadSpecialType').combobox('getValues');
            var DecHeadSpecialTypeArray = [];
            for (var i = 0; i < DecHeadSpecialType.length; i++) {
                DecHeadSpecialTypeArray.push({ "DecHeadSpecialTypeValue": DecHeadSpecialType[i] });
            }

            $('#datagraid').myDatagrid('search', {
                ContrNo: ContrNo, OrderID: OrderID, SeqNo: SeqNo, BaseCusReceiptCode: BaseCusReceiptCode, StartDate: StartDate, EndDate: EndDate, VoyageID: VoyageID,
                DecHeadSpecialType: JSON.stringify(DecHeadSpecialTypeArray),
            });

            GetGuarantees();
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#SeqNo').textbox('setValue', null);
            $('#BaseCusReceiptCode').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#VoyageID').textbox('setValue', null);
            //$('#VoyageType').combobox('setValue', "全部");
            $('#DecHeadSpecialType').combobox('clear');
            setCurrentDate();
            Search();
        }

        //回执订单
        function CheckerCheck(id, ContrNo) {
            var url = location.pathname.replace(/CheckedList.aspx/ig, 'DecCheck.aspx?ID=' + id + '&Form=Checker');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '复核单据',
                width: '1000px',
                height: '800px',
                onClose: function () {
                    Search();
                }
            });
        }
        //查看订单
        function SearchHead(id, OrderID) {
            var url = location.pathname.replace(/CheckedList.aspx/ig, 'Declare.aspx?ID=' + id + '&Source=View&SourcePage=Checked&OrderID=' + OrderID);
            window.location = url;
        }

        //回执订单
        function ReceiptHead(id, ContrNo) {
            var url = location.pathname.replace(/CheckedList.aspx/ig, 'DecTrace.aspx?ID=' + id + '&ContrNo=' + ContrNo);
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
            var buttons = "";

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:1px" onclick="SearchHead(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            //if (row.DoubleCheckerAdminID == ThisAdminOriginID) {
            //    buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:1px" onclick="CheckerCheck(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
            //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //        '<span class="l-btn-text">复核</span>' +
            //        '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
            //        '</span>' +
            //        '</a>';
            //}


            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:1px" onclick="ReceiptHead(\'' + row.ID + '\',\'' + row.ContrNO + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">回执</span>' +
                '<span class="l-btn-icon icon-help">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }


        //获取保函信息
        function GetGuarantees() {
            $.post('?action=GetGuarantees', { ID: "" }, function (res) {
                var result = JSON.parse(res);
                $('#btnhang').find('span[class^=guaratee]').remove();
                $.each(result, function (index, val) {
                    //小于10W红色预警
                    if (val.RemainAmount < 100000) {
                        $('#btnhang').append("<span  class='guarateered'>" + val.BankName + "实时额度：" + val.RemainAmount + "（" + val.ID + "）额度不足！！！</span>");
                    }
                    else {
                        $('#btnhang').append("<span class='guaratee'>" + val.BankName + "实时额度：" + val.RemainAmount + "（" + val.ID + "）</span>");
                    }
                });
            });
        }


        //报关完成 (暂用)
        function Succeed() {
            var ids = [];
            var message = "";
            var rows = $('#datagraid').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选报关完成的报关单！');
                return;
            }

            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i].ID);

            }

            id = ids.join();
            $.messager.confirm('确认', "此步骤暂用，不是实际流程", function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Succeed', { ID: id }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, function (r) {
                            Search();
                        });
                    });
                }
            });
        }

        function DeclareSuccess(ID, ContrNo) {
            var url = location.pathname.replace(/ExcelList.aspx/ig, 'ExcelEdit.aspx?ID=' + ID);
            $.myWindow.setMyWindow("ExcelList2ExcelEdit", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '申报成功',
                width: '500px',
                height: '280px'
            });
        }

        //批量转换舱单
        function Transform() {
            var ids = [];
            var rows = $('#datagraid').datagrid('getChecked');
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
                } else {
                    MaskUtil.unmask();//关闭遮挡层
                }

            });
        }

        //批量制单
        function Make(obj) {
            var ids = [];
            var message = "";
            var rows = $('#datagraid').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('提示', '请勾选需要制单的报关单！');
                return;
            }
            var hadMark = "";
            var hadDec = "";
            var unManifest = "";
            var unGuarantee = "";
            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i].ID);

                //未转换舱单
                if (rows[i].Transformed == "0") {
                    unManifest += rows[i].ContrNo + " ";
                }
                //已制单
                if (rows[i].Status == "02") {
                    hadMark += rows[i].ContrNo + " ";
                }
                //已提交申请
                if (rows[i].Status != "01" && rows[i].Status != "02" && rows[i].Status != "E0" && rows[i].Status != "05" && rows[i].Status != "06") {
                    hadDec += rows[i].ContrNo + " ";
                }
                //未设置保函
                if (rows[i].GuaranteeNo == null || rows[i].GuaranteeNo == "") {
                    unGuarantee += rows[i].ContrNo + " ";
                }
            }

            if (unManifest != "") {
                $.messager.alert('error', '存在未转换舱单的单据，无法制单！');
                return;
            }

            if (unGuarantee != "") {
                $.messager.alert('error', '存在未设置保函的单据，无法制单！');
                return;
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

        function FmtRemainAmount(val, row, index) {
            if (row["RemainAmount"] < 100000) {
                return "<span style='color:red'>" + row["RemainAmount"] + "</span>";
            }
            else {
                return row["RemainAmount"];
            }
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
            }

            $("#PackNo-sum").html(packNoSum); //总件数
            $("#TotalDeclarePrice-sum").html(totalDeclarePriceSum.toFixed(4)); //总金额
            $("#TotalQty-sum").html(totalQtySum); //总数量
            $("#TotalModelQty-sum").html(totalModelQtySum); //总型号数量
            $("#TotalGrossWeight-sum").html(totalGrossWeightSum.toFixed(2)); //总毛重
        }

        function Check(ID) {
            var url = location.pathname.replace(/ExcelList.aspx/ig, 'DecListPrint.aspx?ID=' + ID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印表体对单数据',
                width: '1000px',
                height: '400px'
            });
        }

        function CheckHead(ID) {
            var url = location.pathname.replace(/ExcelList.aspx/ig, 'DecHeadPrint.aspx?ID=' + ID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印表头对单数据',
                width: '1000px',
                height: '470px'
            });
        }

        function Download(ID) {
            $.post('?action=DownloadFiles', { DeclarationID: ID }, function (data) {
                var result = JSON.parse(data);
                if (result.success) {
                    for (i = 0; i < result.data.length; i++) {
                        try {
                            let a = document.createElement('a');
                            a.href = result.data[i];
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                }
            });
        }

        //设置保函
        function ShowSetGuarantee(dangeOrderId, selectType, originVoyageID) {
            //选择运输批次号弹框初始化
            $('#setGuarantee-datagrid').datagrid({
                nowrap: false,
                border: false,
                autoRowHeight: true,
                checkOnSelect: true,
                selectOnCheck: true,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: true,
                url: "?action=GuaranteeData",
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; i++) {
                        if (data.rows[i].VoyageID == originVoyageID) {
                            //选中该行
                            $('#setGuarantee-datagrid').datagrid('checkRow', i);
                            break;
                        }
                    }
                },
            });

            if ('piliang' == selectType) {
                var checkedItems = $('#datagraid').datagrid('getChecked');
                if (checkedItems == null || checkedItems.length <= 0) {
                    $.messager.alert('提示', '请选择报关单！', 'info');
                    return;
                }

                var flag = true;
                $.each(checkedItems, function (index, val) {
                    if (val.GuaranteeNo != null && val.GuaranteeNo != "") {
                        flag = false;
                    }
                });
                if (!flag) {
                    $.messager.alert('提示', '请选择未设置保函的报关单！', 'info');
                    return;
                }
            }

            $('#setGuarantee-datagrid').datagrid('load');
            $("#setGuarantee-dialog .datagrid-header-check").html("");
            $('#setGuarantee-dialog').dialog('open');
        }
    </script>

    <script>
        function setCurrentDate() {
            var CurrentDate = getNowFormatDate();
            $("#StartDate").datebox("setValue", CurrentDate);
            $("#EndDate").datebox("setValue", CurrentDate);
        }

        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnTransform" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Transform()">转换舱单</a>
            <a id="btn-batchSetVoyage" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" 
                onclick="ShowSetGuarantee('', 'piliang', '')">设置保函</a>
            <a id="btnCreate" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Make('1')">整合申报</a>
            <a id="btnCreateSplit" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Make('2')">两步申报</a>
        </div>
        <div id="search">
            <ul>
                <li style="margin-left: 22px;">
                    <span class="lbl">合同号: </span>
                    <input class="easyui-textbox" id="ContrNo" />
                    <%--订单编号id暂时未知--%>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl">统一编号: </span>
                    <input class="easyui-textbox" id="SeqNo" />
                    <span class="lbl" style="margin-left: 10px;">报关单特殊类型: </span>
                    <input class="easyui-combobox" id="DecHeadSpecialType" name="DecHeadSpecialType" data-options="multiple:true,panelHeight:'auto'," />
                </li>
                <li style="margin-left: 10px;">
                    <span class="lbl">单据状态: </span>
                    <input class="easyui-combobox" id="BaseCusReceiptCode" name="BaseCusReceiptCode" data-options="valueField:'Value',textField:'Text'" />
                    <span class="lbl">创建起始日期 </span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至: </span>
                    <input class="easyui-datebox" id="EndDate" />
                    <span class="lbl" style="margin-left: 10px;">运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageID" style="width: 250px;" />
                </li>
                <li id="btnhang">
                    <%--<span class="lbl" style="margin-left: 13px;">运输类型: </span>
                    <input class="easyui-combobox" id="VoyageType" name="VoyageType" data-options="valueField:'TypeValue',textField:'TypeText',editable:false," style="width: 250px;" />--%>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <%--<span id="sum-container" style="margin-left: 55px;">
                        <label>合计</label>
                        <label style="margin-left: 25px;">总件数:</label>
                        <label id="PackNo-sum">0</label>
                        <label style="margin-left: 25px;">总金额:</label>
                        <label id="TotalDeclarePrice-sum">0.0000</label>
                        <label style="margin-left: 25px;">总数量:</label>
                        <label id="TotalQty-sum">0</label>
                        <label style="margin-left: 25px;">总型号数量:</label>
                        <label id="TotalModelQty-sum">0</label>
                        <label style="margin-left: 25px;">总毛重:</label>
                        <label id="TotalGrossWeight-sum">0</label>
                    </span>--%>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagraid" title="复核员复核" data-options="fitColumns:true,border:false,fit:true,singleSelect:true,nowrap:false,toolbar:'#topBar'">
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNO',align:'left'" style="width: 8%">合同号</th>
                    <th data-options="field:'BillNo',align:'left'" style="width: 6%">提(运)单号</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 9%">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 4%">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 12%">客户名称</th>
                    <th data-options="field:'CustomSubmiterAdminName',align:'center'" style="width: 5%">录入及申报员</th>
                    <th data-options="field:'DoubleCheckerAdminName',align:'center'" style="width: 4%">复核员</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 15%">操作</th>
                </tr>
            </thead>
            <thead>
                <tr>
                    <th data-options="field:'InspQuarName',align:'center',formatter:InspQuarNameShow" style="width: 7%">报关单特殊类型</th>
                    <th data-options="field:'VoyageID',align:'left'" style="width: 6%">运输批次号</th>
                    <th data-options="field:'GuaranteeNo',align:'left'" style="width: 9%">保函号</th>
                    <th data-options="field:'TransformedName',align:'center'" style="width: 5%">是否转舱单</th>
                    <th data-options="field:'StatusName',align:'left'" style="width: 6%">单据状态</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 6%">申报日期</th>
                    <th data-options="field:'EntryId',align:'left'" style="width: 9%">海关编号</th>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 9%">统一编号</th>
                    <th data-options="field:'ConsigneeAddress',align:'left',formatter:ConsigneeAddress" style="width: 4%">提货库房</th>
                    <th data-options="field:'PackNo',align:'left'" style="width: 4%">件数</th>
                    <th data-options="field:'GrossWt',align:'left'" style="width: 4%">毛重</th>
                    <th data-options="field:'TotalAmount',align:'left'" style="width: 5%">金额</th>
                    <th data-options="field:'TotalQty',align:'left'" style="width: 5%">数量</th>
                    <th data-options="field:'ModelAmount',align:'left'" style="width: 4%">型号数</th>
                    <th data-options="field:'VoyageType',align:'left'" style="width: 5%">运输类型</th>
                    <%--<th data-options="field:'AgentName',align:'left'" style="width: 10%">委托报关企业</th>--%>
                    <%--<th data-options="field:'IsInspection',align:'center'" style="width: 7%">商检/检疫</th>--%>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 7%">录单时间</th>
                    <th data-options="field:'InputerID',align:'center'" style="width: 5%">制单员</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
    <!------------------------------------------------------------ 选择保函弹框 html Begin ------------------------------------------------------------>
    <div id="setGuarantee-dialog" class="easyui-dialog" title="设置保函" style="width:800px;height:500px;" 
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <table id="setGuarantee-datagrid" data-options="
            nowrap:false,
            border:false,
            autoRowHeight:true,
            checkOnSelect:true,
            selectOnCheck:true,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:true,
            toolbar:'#topBar-setGuarantee-datagrid',
            rownumbers:false">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 5px;"></th>
                    <th data-options="field:'ID',align:'center'" style="width: 20%;">保函号</th>
                    <th data-options="field:'GuaranteeNo',align:'center'" style="width: 15%;">海关保函号</th>
                    <th data-options="field:'PutOnCustoms',align:'center'" style="width: 10%;">备案海关</th>
                    <th data-options="field:'BankName',align:'center'" style="width: 10%;">银行名称</th>
                    <th data-options="field:'GuaranteeAmount',align:'center'" style="width: 10%;">总担保额</th>
                    <th data-options="field:'RemainAmount',align:'center',formatter:FmtRemainAmount" style="width: 10%;">剩余担保额</th>
                    <th data-options="field:'ApproveDate',align:'center'" style="width: 11%;">批准日期</th>
                    <th data-options="field:'ValidDate',align:'center'" style="width: 11%;">有效期至</th>
           
                </tr>
            </thead>
        </table>
    </div>
     <div id="topBar-setGuarantee-datagrid">
     </div>
</html>
