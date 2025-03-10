﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Protocols.TestTools.StackSdk;
using Microsoft.Protocols.TestTools.StackSdk.Security.Sspi;
using Microsoft.Protocols.TestTools.StackSdk.FileAccessService.Smb2;
using Microsoft.Protocols.TestTools.StackSdk.FileAccessService.Rsvd;

namespace Microsoft.Protocols.TestManager.FileServerPlugin
{
    /// <summary>
    /// Detector to detect RSVD 
    /// </summary>
    public partial class FSDetector
    {
        private string initiatorHostName = "Client01";
        /// <summary>
        /// Check if the server supports RSVD.
        /// </summary>
        public DetectResult CheckRsvdSupport(ref DetectionInfo info)
        {
            DetectResult result = DetectResult.DetectFail;
            logWriter.AddLog(LogLevel.Information, "Share path: " + info.targetShareFullPath);

            #region Copy test VHD file to the target share to begin detecting RSVD
            string vhdOnSharePath = info.targetShareFullPath + @"\" + vhdName;
            CopyTestVHD(vhdOnSharePath);
            #endregion

            #region RSVD version 2

            bool versionTestRes = TestRsvdVersion(vhdName + fileNameSuffix, info.BasicShareName, RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_2);
            if (versionTestRes)
            {
                info.RsvdVersion = RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_2;
                result = DetectResult.Supported;
                logWriter.AddLog(LogLevel.Information, "RSVD version 2 is supported");
                return result;
            }
            else
            {
                logWriter.AddLog(LogLevel.Information, "The server doesn't support RSVD version 2.");
            }
            #endregion

            #region RSVD version 1
            versionTestRes = TestRsvdVersion(vhdName + fileNameSuffix, info.BasicShareName, RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_1);
            if (versionTestRes)
            {
                info.RsvdVersion = RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_1;
                result = DetectResult.Supported;
                logWriter.AddLog(LogLevel.Information, "RSVD version 1 is supported");
                return result;
            }
            else
            {
                result = DetectResult.UnSupported;
                logWriter.AddLog(LogLevel.Information, @"The server doesn't support RSVD.");
            }
            #endregion
            return result;
        }

        /// <summary>
        /// Connect to the share VHD file.
        /// </summary>
        private bool TestRsvdVersion(
            string vhdxName,
            string share,
            RSVD_PROTOCOL_VERSION version)
        {
            using (RsvdClient client = new RsvdClient(new TimeSpan(0, 0, defaultTimeoutInSeconds)))
            {
                CREATE_Response response;
                Smb2CreateContextResponse[] serverContexts;
                client.Connect(SUTName, SUTIpAddress, Credential.DomainName, Credential.AccountName, Credential.Password, SecurityPackageType, true, share);

                Smb2CreateContextRequest[] contexts;
                if (version == RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_2)
                {
                    contexts = new Smb2CreateContextRequest[]
                    {
                        new Smb2CreateSvhdxOpenDeviceContextV2
                        {
                            Version = (uint)RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_2,
                            OriginatorFlags = (uint)OriginatorFlag.SVHDX_ORIGINATOR_PVHDPARSER, 
                            InitiatorHostName = initiatorHostName,
                            InitiatorHostNameLength = (ushort)(initiatorHostName.Length * 2),  // InitiatorHostName is a null-terminated Unicode UTF-16 string 
                            VirtualDiskPropertiesInitialized = 0,
                            ServerServiceVersion = 0,
                            VirtualSectorSize = 0,
                            PhysicalSectorSize = 0,
                            VirtualSize = 0
                        }
                    };
                }
                else
                {
                    contexts = new Smb2CreateContextRequest[]
                    {
                        new Smb2CreateSvhdxOpenDeviceContext 
                        {
                            Version = (uint)RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_1,
                            OriginatorFlags = (uint)OriginatorFlag.SVHDX_ORIGINATOR_PVHDPARSER, 
                            InitiatorHostName = initiatorHostName,
                            InitiatorHostNameLength = (ushort)(initiatorHostName.Length * 2)  // InitiatorHostName is a null-terminated Unicode UTF-16 string 
                        }
                    };
                }

                uint status;
                status = client.OpenSharedVirtualDisk(
                    vhdxName,
                    TestTools.StackSdk.FileAccessService.FsCreateOption.FILE_NO_INTERMEDIATE_BUFFERING,
                    contexts,
                    out serverContexts,
                    out response);

                bool result = false;

                if (status != Smb2Status.STATUS_SUCCESS)
                {
                    result = false;
                    logWriter.AddLog(LogLevel.Information, string.Format("{0} is found not supported.", version));
                    return result;
                }

                result = CheckOpenDeviceContext(serverContexts, version);
                client.CloseSharedVirtualDisk();

                return result;
            }
        }

        private bool CheckOpenDeviceContext(Smb2CreateContextResponse[] servercreatecontexts, RSVD_PROTOCOL_VERSION expectVersion)
        {
            if (servercreatecontexts == null)
            {
                if (expectVersion == RSVD_PROTOCOL_VERSION.RSVD_PROTOCOL_VERSION_1)
                {
                    // <10> Section 3.2.5.1:  Windows Server 2012 R2 without [MSKB-3025091] doesn't return SVHDX_OPEN_DEVICE_CONTEXT_RESPONSE.
                    // So if the open device context returns success, but without SVHDX_OPEN_DEVICE_CONTEXT_RESPONSE, the SUT may still support RSVD version 1.
                    return true;
                }
                else
                {
                    return false;
                }
            }

            foreach (var context in servercreatecontexts)
            {
                Type type = context.GetType();
                if (type.Name == "Smb2CreateSvhdxOpenDeviceContextResponse")
                {
                    Smb2CreateSvhdxOpenDeviceContextResponse openDeviceContext = context as Smb2CreateSvhdxOpenDeviceContextResponse;
                    if ((openDeviceContext != null) && (openDeviceContext.Version == (uint)expectVersion))
                    {
                        return true;
                    }

                }

                if (type.Name == "Smb2CreateSvhdxOpenDeviceContextResponseV2")
                {
                    Smb2CreateSvhdxOpenDeviceContextResponseV2 openDeviceContext = context as Smb2CreateSvhdxOpenDeviceContextResponseV2;
                    if ((openDeviceContext != null) && (openDeviceContext.Version == (uint)expectVersion))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CopyTestVHD(string vhdOnSharePath)
        {
            try
            {
                if (System.IO.File.Exists(vhdOnSharePath))
                {
                    return;
                }
                string vhdxPath = GetVhdSourcePath();
                System.IO.File.Copy(vhdxPath, vhdOnSharePath);
            }
            catch (Exception e)
            {
                throw new Exception("Copy test VHD file failed: " + e.Message);
            }
        }

        private string GetVhdSourcePath()
        {
            string res = string.Empty;
            string assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string vhdxPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assemblyPath), @"data\" + testSuiteName + @"\" + vhdName);
            if (System.IO.File.Exists(vhdxPath))
            {
                res = vhdxPath;
            }

            return res;
        }       
    }
}