﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TLKJ.Utils;
using TLKJ.DB;
using System.Data;

namespace TLKJ.DAO
{
    public class XT_TASK_Dao : BaseDao<XT_TASK>
    {
        public void init(HttpRequest res, HttpResponse rep)
        {
            request = res;
            response = rep;
        }
        public int save(XT_TASK vo, String cKeyID)
        {
            ActiveResult vret = new ActiveResult();
            List<String> sqls = new List<string>();
            string sql = null;
            if (cKeyID.Length == 0)
            {
                sql = Insert(vo);
            }
            else
            {
                sql = Update(vo, "REC_ID='" + cKeyID + "'");
            }
            sqls.Add(sql);
            int iCode = DbManager.ExecSQL(sqls);
            return iCode;

        }
        public DBResult Query(String cWhereParm, String cOrderBy, int iPageNo, int iPageSize)
        {
            return DbManager.Query("*", "XT_TASK", cWhereParm, cOrderBy, iPageNo, iPageSize);
        }
        public DBResult Query(String cFieldList, String cTableName, String cWhereParm, String cOrderBy, int iPageNo, int iPageSize)
        {
            return DbManager.Query(cFieldList, cTableName, cWhereParm, cOrderBy, iPageNo, iPageSize);
        }

        public int del_item(String cDBKey)
        {
            ActiveResult vret = new ActiveResult();
            string sql = "";
            sql = "DELETE FROM XT_TASK WHERE REC_ID='" + cDBKey + "'";
            int iCode = DbManager.ExecSQL(sql);
            return iCode;
        }
        public int del_list(String cKeyList)
        {
            String[] KeyList = cKeyList.Split(',');
            ActiveResult vret = new ActiveResult();
            StringBuilder sql = new StringBuilder();
            sql.Append(" DELETE FROM XT_TASK ");
            sql.Append(" WHERE REC_ID IN (");
            for (int i = 0; i < KeyList.Length; i++)
            {
                String cUserID = KeyList[i].Trim();
                if (i == 0)
                {
                    sql.Append("'" + cUserID + "'");
                }
                else
                {
                    sql.Append(",'" + cUserID + "'");
                }
            }
            sql.Append(" )");
            return DbManager.ExecSQL(sql.ToString());
        }

        public XT_TASK FindOne(string cDBKey)
        {
            XT_TASK vPhone = null;
            DataTable dtRows = DbManager.QueryData("SELECT * FROM XT_TASK WHERE REC_ID='" + cDBKey + "'");
            if ((dtRows != null) && (dtRows.Rows.Count > 0))
            {
                vPhone = new XT_TASK();
                ReadDB(vPhone, dtRows);
            }
            return vPhone;
        }
    }
}