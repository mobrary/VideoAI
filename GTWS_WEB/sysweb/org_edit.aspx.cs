﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TLKJ.WebSys;
using TLKJ.Utils;

public partial class sysweb_org_edit : PageEx
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckLogin();
        LoginUserInfo vUserInf = getLoginUserInfo();
        AREA_ID.Value = AppManager.getAreaHeader(vUserInf.ORG_ID);
    }
}