﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TLKJ.WebSys;

public partial class export_service : PageEx
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckLogin();
    }
}