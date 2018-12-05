function setRF_GIS(vJCTB_GUID) {
    var vXZQDM = getListKeyValue(vJCTB_GUID);
    var isXZQ_SAME_GXQ = isEasyXZQ(vXZQDM);
    var vActiveUrl = $("#rf_gis").attr("src");
    var vUrl = "/framework/ViewGIS.aspx?XZQDM=" + vXZQDM;
    if (isXZQ_SAME_GXQ) {
        if (vUrl != vActiveUrl) {
            $("#rf_gis").attr("src", vActiveUrl);
        }
        else {
            $("#rf_gis").attr("src", vActiveUrl);
        }
    }
}

function setGisXY(vJCTB_GUID) {
    $("#rf_gis").contentWindow.setJCTBView(vJCTB_GUID);
}