$(document).ready(function () {
    $("#GXQ_CITY_ID").click(function () {
        Event_GXQ_CITYChange();
    });

    $("#XZQ_CITY_ID").click(function () {
        Event_XZQ_CITYChange();
    });
});

function Event_GXQ_CITYChange(vObj) {
    var vUrl = "/api/rest.ashx";
    var vActionType = "GXQInf";
    var vActionMethod = "list";
    vUrl = vUrl + "?action_type=" + vActionType + "&action_method=" + vActionMethod;
    var cValue = "GXQ_CITY_ID=" + $("#GXQ_CITY_ID").val();
    $.ajax({
        type: "POST",
        url: vUrl,
        dataType: "json",
        cache: false,
        data: cValue + "&no-cache=" + Math.round(Math.random() * 10000),
        success: Event_AfterGXQCityChange
    });
}

function Event_AfterGXQCityChange(vObj) {
    $("#GXQ_COUNTY_ID").empty();
    var rs = vObj.rows;
    for (var i = 0; i < rs.length; i++) {
        var vo = rs[i];
        $("#GXQ_COUNTY_ID").append("<option value=" + vo.gxq_id + ">" + vo.gxq_id + "-" + vo.gxq_name + "</option>");
    }
}

function Event_XZQ_CITYChange(vObj) {
    var vUrl = "/api/rest.ashx";
    var vActionType = "XZQInf";
    var vActionMethod = "list";
    vUrl = vUrl + "?action_type=" + vActionType + "&action_method=" + vActionMethod;
    var cValue = "XZQ_CITY_ID=" + $("#XZQ_CITY_ID").val();
    $.ajax({
        type: "POST",
        url: vUrl,
        dataType: "json",
        cache: false,
        data: cValue + "&no-cache=" + Math.round(Math.random() * 10000),
        success: Event_AfterXZQCityChange
    });
}

function Event_AfterXZQCityChange(vObj) {
    $("#XZQ_COUNTY_ID").empty();
    var rs = vObj.rows;
    for (var i = 0; i < rs.length; i++) {
        var vo = rs[i];
        $("#XZQ_COUNTY_ID").append("<option value=" + vo.xzq_id + ">" + vo.xzq_id + "-" + vo.xzq_name + "</option>");
    }
}