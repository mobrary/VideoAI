$(document).ready(function () {
    $("#GXQ_COUNTY_ID").change(function () {
        Event_Query($(this));
    });
    $("#XZQ_COUNTY_ID").change(function () {
        Event_Query($(this));
    });

    $("#ORDERBY").change(function () {
        Event_Query($(this));
    });

    $("#QUERY_ITEM").click(function () {
        Event_Query($(this));
    });

    $("[data-val='STATUS']").click(function () {
        var vKeyID = $(this).attr("data-val");
        $("#STATUS").val = vKeyID;
        Event_Query($(this));
    });

    $("#JCBH").keyup(function () {
        Event_Query($(this));
    });

    InitCityCounty();

    var vJCTB = $("#JCBH"); 
    if (vJCTB.length > 0) {
        Event_Query();
    }
});

function InitCityCounty() {
    var vGXQCity = $("#GXQ_CITY_ID");
    var vXZQCity = $("#XZQ_CITY_ID");

    if (typeof (vGXQCity) != "undefined") {
        vGXQCity.change();
    }

    if (typeof (vXZQCity) != "undefined") {
        vXZQCity.change();
    }
}
function Event_Query() {
    Event_CountyChange(null);
}

function Event_CountyChange(vObj) {
    var vTAG = "";
    if ($("#TAG").length == 0) {
        return;
    }
    vTAG = $("#TAG").val();

    var vUrl = "/api/rest.ashx";
    var vGXQ_COUNTY_ID = "";
    if ($("#GXQ_COUNTY_ID").length > 0) {
        vGXQ_COUNTY_ID = $("#GXQ_COUNTY_ID").val();
    }
    var vXZQ_COUNTY_ID = "";
    if ($("#XZQ_COUNTY_ID").length > 0) {
        vXZQ_COUNTY_ID = $("#XZQ_COUNTY_ID").val();
    }
    var vORDERBY = "";
    if ($("#ORDERBY").length > 0) {
        vORDERBY = $("#ORDERBY").val();
    }
    var vJCBH = "";
    if ($("#JCBH").length > 0) {
        vJCBH = $("#JCBH").val();
    }
    var vTAG = "";
    if ($("#TAG").length > 0) {
        vTAG = $("#TAG").val();
    }

    var vSTATUS = "";
    if ($("#STATUS").length > 0) {
        vSTATUS = $("#STATUS").val();
    }

    var vActionType = "JCTB";
    if (vTAG == "1") {
        vActionType = "JZ";
    }
    else if (vTAG == "2") {
        vActionType = "ZG";
    }
    var vActionMethod = "list";
    var vUrl = vUrl + "?action_type=" + vActionType + "&action_method=" + vActionMethod;
    var cValue = "GXQ_COUNTY_ID=" + vGXQ_COUNTY_ID + "&XZQ_COUNTY_ID=" + vXZQ_COUNTY_ID + "&ORDERBY=" + vORDERBY + "&JCBH=" + vJCBH + "&TAG=" + vTAG + "&STATUS=" + vSTATUS;
    $.ajax({
        type: "POST",
        url: vUrl,
        dataType: "json",
        cache: false,
        data: cValue + "&no-cache=" + Math.round(Math.random() * 10000),
        success: Event_AfterCountyChange
    });
}

function getStatusText(vRowItem) {
    var vTEXT = "";
    var vTAG_ID = vRowItem.tag;
    if (vTAG_ID == 1) {
        if (vRowItem.p_sh_status == "7") {
            vTEXT = "已审核";
        }
        else if (vRowItem.p_sh_status == "8") {
            vTEXT = "已退回";
        }
        else if (vRowItem.upload_flag == "1") {
            vTEXT = "已上报";
        }
        else {
            vTEXT = "";
        }
    }
    return vTEXT;
}

function Event_AfterCountyChange(ret) {
    $("#TB_LIST").empty();
    // 保存成功 
    if (ret.result == 1) {
        for (var i = 0; i < ret.rows.length; i++) {
            var obj = ret.rows[i];
            $("#TB_LIST").append("<tr><td  onclick='Event_After_TbClickLink(\"" + obj.jctb_guid + "\");'>" + obj.xzqmc + "-" + obj.jcbh + "</td><td onclick='Event_After_TbClickLink(\"" + obj.jctb_guid + "\");'>" + getStatusText(obj) + "</td></tr>");
        }
        // 保存失败
        if (ret.result == 2) {
            alert("数据获取失败！");
            return;
        }
    }
}

var myArray = new Array();
var arrTemp = new Array();
function Event_After_TbClickLink(vJCTB_GUID) {
    var vTAG = $("#TAG").val();

    if ((vTAG == 1) || (vTAG == 2)) {
        var vActivePage = "JZ.aspx";
        var vActiveText = "";
        if (vTAG == 1) {
            vActivePage = "JZ.aspx";
            vActiveText = "图斑质疑举证";
        }
        else if (vTAG == 2) {
            vActivePage = "ZG.aspx";
            vActiveText = "图斑整改信息";
        }
        var vUrl = "/pages/" + vActivePage + "?JCTB_GUID=" + vJCTB_GUID;
        var vPageText = '<div class="selfui-zg-baseinfo"><div class="selfui-zg-jlStr"></div><hr class="selfui-bg-golden" style="margin-top: 30px; width: 98%;  margin-left: 20px;" /><iframe class="selfui-zg-bansinfo" id="mainframe"  id="mainframe" width="98%" height="800px" scrolling="yes" src="#" frameborder="0"/></iframe>  <hr class="selfui-bg-golden" style="margin-top: 30px; width: 98%;  margin-left: 20px;" /></div>'

        layui.use('element', function () {
            var vLayItem = layui.element;
            vLayItem.tabDelete('demo', "15");
            vLayItem.tabAdd('demo', {
                title: '<i class="layui-icon">&#xe62a;</i>' + vActiveText
                     , content: vPageText
                     , id: "15"
            });
            $("li[lay-id='15']").click();
            $("#mainframe").attr("src", vUrl);
        });
    }
}
