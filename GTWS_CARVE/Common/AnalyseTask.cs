﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TLKJ.Utils;
using Renci.SshNet;
using TLKJAI;
using System.Threading;

namespace TLKJ_IVS
{
    public class AnalyseTask
    {
        private int iMinVal;
        private int iMaxVal;
        private int iGrayMinVal;
        private int iGrayMaxVal;
        private int iALARM_ALLOW;
        private String cALARM_PATH;

        private int iANALYSE_ALLOW;
        private String cANALYSE_PATH;
        private String cAppDir;

        private static AnalyseTask instance = null;

        private AnalyseTask()
        {
            iMinVal = StringEx.getInt(INIConfig.ReadString("Config", AppConfig.IMAGE_MIN, "0"));
            iMaxVal = StringEx.getInt(INIConfig.ReadString("Config", AppConfig.IMAGE_MAX, "0"));

            iGrayMinVal = StringEx.getInt(INIConfig.ReadString("Config", AppConfig.GRAY_MIN, "0"));
            iGrayMaxVal = StringEx.getInt(INIConfig.ReadString("Config", AppConfig.GRAY_MAX, "0"));

            iALARM_ALLOW = INIConfig.ReadInt("ALARM", "DFS_ALLOW");
            cALARM_PATH = INIConfig.ReadString("ALARM", "DFS_PATH", "");

            iANALYSE_ALLOW = INIConfig.ReadInt("ANALYSE", "DFS_ALLOW");
            cANALYSE_PATH = INIConfig.ReadString("ANALYSE", "DFS_PATH", "");

            cAppDir = Application.StartupPath;
        }

        public static AnalyseTask getInstance()
        {
            if (instance == null)
            {
                instance = new AnalyseTask();
            }
            return instance;
        }
         
        public static Queue<String> AnalyseFileList = null;
        
        public String getAnalyseFile()
        {
            if (cAppDir == null)
            {
                cAppDir = Application.StartupPath;
            }
            if (AnalyseFileList == null)
            {
                AnalyseFileList = new Queue<string>();
            }

            if (AnalyseFileList.Count == 0)
            {
                String cFILE_PATH = INIConfig.ReadString("ANALYSE", "FILE_PATH", "");
                if (!Directory.Exists(cFILE_PATH))
                {
                    return null;
                }
                String[] FileList = Directory.GetFiles(cFILE_PATH);
                for (int i = 0; i < FileList.Length; i++)
                {
                    String cFileName = FileList[i];
                    AnalyseFileList.Enqueue(cFileName);
                    if (i > 10)
                    {
                        break;
                    }
                }
            }
            if (AnalyseFileList.Count > 0)
            {
                String cFileName = AnalyseFileList.Dequeue();
                return cFileName;
            }
            else
            {
                return null;
            }
        }
       

        public bool UpdateAnalyseFile(String cFileName)
        {
            Boolean isUpload = false;
            JActiveTable aMaster = new JActiveTable();
            JActiveTable aSlave = new JActiveTable();
            aSlave.TableName = "XT_IMG_LIST";
            aMaster.TableName = "XT_IMG_REC";

            log4net.WriteLogFile("分析线程正在运行中......");
            if (String.IsNullOrWhiteSpace(cFileName))
            {
                return false;
            }

            String cFileExt = Path.GetExtension(cFileName);
            String cREC_ID = Path.GetFileName(cFileName).Replace(cFileExt, "");
            List<KeyValue> ImageList = IMGAI.getImageList(cFileName, iMinVal, iMaxVal, iGrayMinVal, iGrayMaxVal);
            String cExportFileName = Application.StartupPath + "\\" + cREC_ID + ".zip";
            int iCode = 0;
            Boolean MustRemoveFile = false;
            if ((ImageList != null) && (ImageList.Count > 0))
            {
                List<String> sqls = new List<string>();
                for (int k = 0; (ImageList != null) && (k < ImageList.Count); k++)
                {
                    Application.DoEvents();
                    KeyValue rowKey = ImageList[k];
                    aSlave.ClearField();
                    String cKeyID = StringEx.getString(k + 1000);
                    aSlave.AddField("ID", AutoID.getAutoID() + "_" + cKeyID);
                    aSlave.AddField("ALARM_FLAG", 0);
                    aSlave.AddField("REC_ID", cREC_ID);
                    aSlave.AddField("CREATE_TIME", DateUtils.getDayTimeNum());
                    aSlave.AddField("POINT_LIST", rowKey.Val);
                    sqls.Add(aSlave.getInsertSQL());
                }
                sqls.Insert(0, "DELETE FROM XT_IMG_REC WHERE REC_ID='" + cREC_ID + "'");
                iCode = WebSQL.ExecSQL(sqls);

                if (iCode > 0)
                {
                    if (File.Exists(cExportFileName))
                    {
                        SftpClient ftp = getAnalyseClient();
                        isUpload = CopyUnit.SSH_Upload(ftp, cExportFileName, "ANALYSE");
                    }
                }
                if (isUpload)
                {
                    MustRemoveFile = true;
                    log4net.WriteLogFile("UploadTask.文件上传成功！");

                }
                else
                {
                    log4net.WriteLogFile("UploadTask." + cREC_ID + ":文件上传失败！");
                }
            }
            else
            {
                log4net.WriteLogFile("UploadTask." + cREC_ID + ":图片拆分失败！");
                MustRemoveFile = true;
            }
            if (MustRemoveFile)
            {
                try
                {
                    File.Delete(cFileName);
                }
                catch (Exception ex)
                {
                    log4net.WriteLogFile(ex.Message);
                }
                try
                {
                    File.Delete(cExportFileName);
                }
                catch (Exception ex)
                {
                    log4net.WriteLogFile(ex.Message);
                }
            }

            aMaster.ClearField();
            if (iCode > 0)
            {
                aMaster.AddField("AI_FLAG", 1);
            }
            else
            {
                aMaster.AddField("AI_FLAG", 2);
            }
            iCode = WebSQL.ExecSQL(aMaster.getUpdateSQL(" REC_ID='" + cREC_ID + "' "));
            if (iCode > 0)
            {
                log4net.WriteLogFile("REC_ID为：" + cREC_ID + "的图片抠图成功！");
            }
            return true;
        } 
        private static SftpClient AnalyseClient = null;
        public SftpClient getAnalyseClient()
        {
            if (AnalyseClient == null)
            {
                String cDFSType = "ANALYSE";
                String cDFS_HOST = INIConfig.ReadString(cDFSType, "DFS_HOST", "");
                String cDFS_PORT = INIConfig.ReadString(cDFSType, "DFS_PORT", "22");
                int iDFS_PORT = StringEx.getInt(cDFS_PORT);
                String cDFS_USER = INIConfig.ReadString(cDFSType, "DFS_USER", "root");
                String cDFS_PASS = INIConfig.ReadString(cDFSType, "DFS_PASS", "");
                AnalyseClient = new SftpClient(cDFS_HOST, iDFS_PORT, cDFS_USER, cDFS_PASS);
            }
            if (!AnalyseClient.IsConnected)
            {
                try
                {
                    AnalyseClient.Connect();
                    return AnalyseClient;
                }
                catch (Exception ex)
                {
                    log4net.WriteLogFile(ex.Message);
                    AnalyseClient = null;
                    return null;
                }
            }
            else
            {
                return AnalyseClient;
            }
        }
    }
}
