// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Protocols.TestSuites.FileSharing.FSA.TestSuite {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using Microsoft.SpecExplorer.Runtime.Testing;
    using Microsoft.Protocols.TestTools;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Spec Explorer", "3.5.3146.0")]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class SetFileValidDataLengthInformationTestCase : PtfTestClassBase {
        
        public SetFileValidDataLengthInformationTestCase() {
            this.SetSwitch("ProceedControlTimeout", "100");
            this.SetSwitch("QuiescenceTimeout", "30000");
        }
        
        #region Expect Delegates
        public delegate void GetIfOpenHasManageVolumeAccessDelegate1(bool isHasManageVolumeAccess);
        #endregion
        
        #region Event Metadata
        static System.Reflection.MethodBase GetIfOpenHasManageVolumeAccessInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.IFSAAdapter), "GetIfOpenHasManageVolumeAccess", typeof(bool).MakeByRefType());
        #endregion
        
        #region Adapter Instances
        private Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.IFSAAdapter IFSAAdapterInstance;
        #endregion
        
        #region Class Initialization and Cleanup
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void ClassInitialize(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext context) {
            PtfTestClassBase.Initialize(context);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void ClassCleanup() {
            PtfTestClassBase.Cleanup();
        }
        #endregion
        
        #region Test Initialization and Cleanup
        protected override void TestInitialize() {
            this.InitializeTestManager();
            this.IFSAAdapterInstance = ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.IFSAAdapter)(this.Manager.GetAdapter(typeof(Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.IFSAAdapter))));
        }
        
        protected override void TestCleanup() {
            base.TestCleanup();
            this.CleanupTestManager();
        }
        #endregion
        
        #region Test Starting in S0
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.Model)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.Fsa)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.SetFileInformation)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.NonSmb)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.Positive)]
        public void SetFileValidDataLengthInformationTestCaseS0() {
            this.Manager.BeginTest("SetFileValidDataLengthInformationTestCaseS0");
            this.Manager.Comment("reaching state \'S0\'");
            this.Manager.Comment("executing step \'call FsaInitial()\'");
            this.IFSAAdapterInstance.FsaInitial();
            this.Manager.Comment("reaching state \'S1\'");
            this.Manager.Comment("checking step \'return FsaInitial\'");
            this.Manager.Comment("reaching state \'S4\'");
            bool temp0;
            this.Manager.Comment("executing step \'call GetIfOpenHasManageVolumeAccess(out _)\'");
            this.IFSAAdapterInstance.GetIfOpenHasManageVolumeAccess(out temp0);
            this.Manager.AddReturn(GetIfOpenHasManageVolumeAccessInfo, null, temp0);
            this.Manager.Comment("reaching state \'S6\'");
            int temp5 = this.Manager.ExpectReturn(this.QuiescenceTimeout, true, new ExpectedReturn(SetFileValidDataLengthInformationTestCase.GetIfOpenHasManageVolumeAccessInfo, null, new GetIfOpenHasManageVolumeAccessDelegate1(this.SetFileValidDataLengthInformationTestCaseS0GetIfOpenHasManageVolumeAccessChecker)), new ExpectedReturn(SetFileValidDataLengthInformationTestCase.GetIfOpenHasManageVolumeAccessInfo, null, new GetIfOpenHasManageVolumeAccessDelegate1(this.SetFileValidDataLengthInformationTestCaseS0GetIfOpenHasManageVolumeAccessChecker1)));
            if ((temp5 == 0)) {
                this.Manager.Comment("reaching state \'S8\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp1;
                this.Manager.Comment("executing step \'call CreateFile(NORMAL,NON_DIRECTORY_FILE,NULL,GENERIC_WRITE,FILE" +
                        "_SHARE_WRITE,OPEN_IF,StreamIsFound,IsNotSymbolicLink,DataFile,PathNameValid)\'");
                temp1 = this.IFSAAdapterInstance.CreateFile(Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAttribute.NORMAL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateOptions.NON_DIRECTORY_FILE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamTypeNameToOpen.NULL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAccess.GENERIC_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.ShareAccess.FILE_SHARE_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateDisposition.OPEN_IF, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamFoundType)(0)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.SymbolicLinkType)(1)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileType)(0)), Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileNameStatus.PathNameValid);
                this.Manager.Checkpoint("MS-FSA_R405");
                this.Manager.Checkpoint(@"[In Application Requests an Open of a File , Pseudocode for the operation is as follows:
                        Phase 6 -- Location of file:] Pseudocode for this search:For i = 1 to n-1:
                        If Open.GrantedAccess.FILE_TRAVERSE is not set and AccessCheck( SecurityContext, Link.File.SecurityDescriptor, FILE_TRAVERSE ) 
                        returns FALSE, the operation is not  failed with STATUS_ACCESS_DENIED in Windows.");
                this.Manager.Checkpoint("MS-FSA_R475");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return:CreateAction set to FILE_CREATED.");
                this.Manager.Checkpoint("MS-FSA_R474");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return :Status set to STATUS_SUCCESS.");
                this.Manager.Comment("reaching state \'S12\'");
                this.Manager.Comment("checking step \'return CreateFile/SUCCESS\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus)(0)), temp1, "return of CreateFile, state S12");
                this.Manager.Comment("reaching state \'S16\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp2;
                this.Manager.Comment("executing step \'call SetFileValidDataLengthInfo(False)\'");
                temp2 = this.IFSAAdapterInstance.SetFileValidDataLengthInfo(false);
                this.Manager.Checkpoint("MS-FSA_R3203");
                this.Manager.Checkpoint("[In FileValidDataLengthInformation,Pseudocode for the operation is as follows:]\r\n" +
                        "                    If Open.HasManageVolumeAccess is FALSE, the operation MUST b" +
                        "e failed with STATUS_PRIVILEGE_NOT_HELD.");
                this.Manager.Comment("reaching state \'S20\'");
                this.Manager.Comment("checking step \'return SetFileValidDataLengthInfo/PRIVILEGE_NOT_HELD\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus.PRIVILEGE_NOT_HELD, temp2, "return of SetFileValidDataLengthInfo, state S20");
                this.Manager.Comment("reaching state \'S24\'");
                goto label0;
            }
            if ((temp5 == 1)) {
                this.Manager.Comment("reaching state \'S9\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp3;
                this.Manager.Comment("executing step \'call CreateFile(NORMAL,NON_DIRECTORY_FILE,NULL,GENERIC_WRITE,FILE" +
                        "_SHARE_WRITE,OPEN_IF,StreamIsFound,IsNotSymbolicLink,DataFile,PathNameValid)\'");
                temp3 = this.IFSAAdapterInstance.CreateFile(Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAttribute.NORMAL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateOptions.NON_DIRECTORY_FILE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamTypeNameToOpen.NULL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAccess.GENERIC_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.ShareAccess.FILE_SHARE_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateDisposition.OPEN_IF, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamFoundType)(0)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.SymbolicLinkType)(1)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileType)(0)), Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileNameStatus.PathNameValid);
                this.Manager.Checkpoint("MS-FSA_R405");
                this.Manager.Checkpoint(@"[In Application Requests an Open of a File , Pseudocode for the operation is as follows:
                        Phase 6 -- Location of file:] Pseudocode for this search:For i = 1 to n-1:
                        If Open.GrantedAccess.FILE_TRAVERSE is not set and AccessCheck( SecurityContext, Link.File.SecurityDescriptor, FILE_TRAVERSE ) 
                        returns FALSE, the operation is not  failed with STATUS_ACCESS_DENIED in Windows.");
                this.Manager.Checkpoint("MS-FSA_R475");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return:CreateAction set to FILE_CREATED.");
                this.Manager.Checkpoint("MS-FSA_R474");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return :Status set to STATUS_SUCCESS.");
                this.Manager.Comment("reaching state \'S13\'");
                this.Manager.Comment("checking step \'return CreateFile/SUCCESS\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus)(0)), temp3, "return of CreateFile, state S13");
                this.Manager.Comment("reaching state \'S17\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp4;
                this.Manager.Comment("executing step \'call SetFileValidDataLengthInfo(False)\'");
                temp4 = this.IFSAAdapterInstance.SetFileValidDataLengthInfo(false);
                this.Manager.Checkpoint("MS-FSA_R3210");
                this.Manager.Checkpoint("[In FileValidDataLengthInformation,Pseudocode for the operation is as follows:]Re" +
                        "turn STATUS_SUCCESS.");
                this.Manager.Comment("reaching state \'S21\'");
                this.Manager.Comment("checking step \'return SetFileValidDataLengthInfo/SUCCESS\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus)(0)), temp4, "return of SetFileValidDataLengthInfo, state S21");
                this.Manager.Comment("reaching state \'S25\'");
                goto label0;
            }
            throw new InvalidOperationException("never reached");
        label0:
;
            this.Manager.EndTest();
        }
        
        private void SetFileValidDataLengthInformationTestCaseS0GetIfOpenHasManageVolumeAccessChecker(bool isHasManageVolumeAccess) {
            this.Manager.Comment("checking step \'return GetIfOpenHasManageVolumeAccess/[out False]\'");
            TestManagerHelpers.AssertAreEqual<bool>(this.Manager, false, isHasManageVolumeAccess, "isHasManageVolumeAccess of GetIfOpenHasManageVolumeAccess, state S6");
        }
        
        private void SetFileValidDataLengthInformationTestCaseS0GetIfOpenHasManageVolumeAccessChecker1(bool isHasManageVolumeAccess) {
            this.Manager.Comment("checking step \'return GetIfOpenHasManageVolumeAccess/[out True]\'");
            TestManagerHelpers.AssertAreEqual<bool>(this.Manager, true, isHasManageVolumeAccess, "isHasManageVolumeAccess of GetIfOpenHasManageVolumeAccess, state S6");
        }
        #endregion
        
        #region Test Starting in S2
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.Model)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.Fsa)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.SetFileInformation)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.NonSmb)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategory(Microsoft.Protocols.TestSuites.FileSharing.Common.Adapter.TestCategories.Positive)]
        public void SetFileValidDataLengthInformationTestCaseS2() {
            this.Manager.BeginTest("SetFileValidDataLengthInformationTestCaseS2");
            this.Manager.Comment("reaching state \'S2\'");
            this.Manager.Comment("executing step \'call FsaInitial()\'");
            this.IFSAAdapterInstance.FsaInitial();
            this.Manager.Comment("reaching state \'S3\'");
            this.Manager.Comment("checking step \'return FsaInitial\'");
            this.Manager.Comment("reaching state \'S5\'");
            bool temp6;
            this.Manager.Comment("executing step \'call GetIfOpenHasManageVolumeAccess(out _)\'");
            this.IFSAAdapterInstance.GetIfOpenHasManageVolumeAccess(out temp6);
            this.Manager.AddReturn(GetIfOpenHasManageVolumeAccessInfo, null, temp6);
            this.Manager.Comment("reaching state \'S7\'");
            int temp11 = this.Manager.ExpectReturn(this.QuiescenceTimeout, true, new ExpectedReturn(SetFileValidDataLengthInformationTestCase.GetIfOpenHasManageVolumeAccessInfo, null, new GetIfOpenHasManageVolumeAccessDelegate1(this.SetFileValidDataLengthInformationTestCaseS2GetIfOpenHasManageVolumeAccessChecker)), new ExpectedReturn(SetFileValidDataLengthInformationTestCase.GetIfOpenHasManageVolumeAccessInfo, null, new GetIfOpenHasManageVolumeAccessDelegate1(this.SetFileValidDataLengthInformationTestCaseS2GetIfOpenHasManageVolumeAccessChecker1)));
            if ((temp11 == 0)) {
                this.Manager.Comment("reaching state \'S10\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp7;
                this.Manager.Comment("executing step \'call CreateFile(NORMAL,NON_DIRECTORY_FILE,NULL,GENERIC_WRITE,FILE" +
                        "_SHARE_WRITE,OPEN_IF,StreamIsFound,IsNotSymbolicLink,DataFile,PathNameValid)\'");
                temp7 = this.IFSAAdapterInstance.CreateFile(Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAttribute.NORMAL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateOptions.NON_DIRECTORY_FILE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamTypeNameToOpen.NULL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAccess.GENERIC_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.ShareAccess.FILE_SHARE_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateDisposition.OPEN_IF, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamFoundType)(0)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.SymbolicLinkType)(1)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileType)(0)), Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileNameStatus.PathNameValid);
                this.Manager.Checkpoint("MS-FSA_R405");
                this.Manager.Checkpoint(@"[In Application Requests an Open of a File , Pseudocode for the operation is as follows:
                        Phase 6 -- Location of file:] Pseudocode for this search:For i = 1 to n-1:
                        If Open.GrantedAccess.FILE_TRAVERSE is not set and AccessCheck( SecurityContext, Link.File.SecurityDescriptor, FILE_TRAVERSE ) 
                        returns FALSE, the operation is not  failed with STATUS_ACCESS_DENIED in Windows.");
                this.Manager.Checkpoint("MS-FSA_R475");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return:CreateAction set to FILE_CREATED.");
                this.Manager.Checkpoint("MS-FSA_R474");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return :Status set to STATUS_SUCCESS.");
                this.Manager.Comment("reaching state \'S14\'");
                this.Manager.Comment("checking step \'return CreateFile/SUCCESS\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus)(0)), temp7, "return of CreateFile, state S14");
                this.Manager.Comment("reaching state \'S18\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp8;
                this.Manager.Comment("executing step \'call SetFileValidDataLengthInfo(True)\'");
                temp8 = this.IFSAAdapterInstance.SetFileValidDataLengthInfo(true);
                this.Manager.Checkpoint("MS-FSA_R3203");
                this.Manager.Checkpoint("[In FileValidDataLengthInformation,Pseudocode for the operation is as follows:]\r\n" +
                        "                    If Open.HasManageVolumeAccess is FALSE, the operation MUST b" +
                        "e failed with STATUS_PRIVILEGE_NOT_HELD.");
                this.Manager.Comment("reaching state \'S22\'");
                this.Manager.Comment("checking step \'return SetFileValidDataLengthInfo/PRIVILEGE_NOT_HELD\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus.PRIVILEGE_NOT_HELD, temp8, "return of SetFileValidDataLengthInfo, state S22");
                this.Manager.Comment("reaching state \'S26\'");
                goto label1;
            }
            if ((temp11 == 1)) {
                this.Manager.Comment("reaching state \'S11\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp9;
                this.Manager.Comment("executing step \'call CreateFile(NORMAL,NON_DIRECTORY_FILE,NULL,GENERIC_WRITE,FILE" +
                        "_SHARE_WRITE,OPEN_IF,StreamIsFound,IsNotSymbolicLink,DataFile,PathNameValid)\'");
                temp9 = this.IFSAAdapterInstance.CreateFile(Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAttribute.NORMAL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateOptions.NON_DIRECTORY_FILE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamTypeNameToOpen.NULL, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileAccess.GENERIC_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.ShareAccess.FILE_SHARE_WRITE, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.CreateDisposition.OPEN_IF, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.StreamFoundType)(0)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.SymbolicLinkType)(1)), ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileType)(0)), Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.FileNameStatus.PathNameValid);
                this.Manager.Checkpoint("MS-FSA_R405");
                this.Manager.Checkpoint(@"[In Application Requests an Open of a File , Pseudocode for the operation is as follows:
                        Phase 6 -- Location of file:] Pseudocode for this search:For i = 1 to n-1:
                        If Open.GrantedAccess.FILE_TRAVERSE is not set and AccessCheck( SecurityContext, Link.File.SecurityDescriptor, FILE_TRAVERSE ) 
                        returns FALSE, the operation is not  failed with STATUS_ACCESS_DENIED in Windows.");
                this.Manager.Checkpoint("MS-FSA_R475");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return:CreateAction set to FILE_CREATED.");
                this.Manager.Checkpoint("MS-FSA_R474");
                this.Manager.Checkpoint("[In Creation of a New File,Pseudocode for the operation is as follows:]\r\n        " +
                        "        The object store MUST return :Status set to STATUS_SUCCESS.");
                this.Manager.Comment("reaching state \'S15\'");
                this.Manager.Comment("checking step \'return CreateFile/SUCCESS\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, ((Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus)(0)), temp9, "return of CreateFile, state S15");
                this.Manager.Comment("reaching state \'S19\'");
                Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus temp10;
                this.Manager.Comment("executing step \'call SetFileValidDataLengthInfo(True)\'");
                temp10 = this.IFSAAdapterInstance.SetFileValidDataLengthInfo(true);
                this.Manager.Checkpoint("MS-FSA_R3204");
                this.Manager.Checkpoint(@"[In FileValidDataLengthInformation,Pseudocode for the operation is as follows:]
                    The operation MUST be failed with STATUS_INVALID_PARAMETER under any of the following conditions:
                    If Open.Stream.ValidDataLength is greater than InputBuffer.ValidDataLength.");
                this.Manager.Checkpoint("MS-FSA_R3205");
                this.Manager.Checkpoint("[In FileValidDataLengthInformation,Pseudocode for the operation is as follows:]\r\n" +
                        "                     The operation MUST be failed with STATUS_INVALID_PARAMETER " +
                        "under any of the following conditions:If Open.Stream.IsCompressed is TRUE.");
                this.Manager.Checkpoint("MS-FSA_R3206");
                this.Manager.Checkpoint("[In FileValidDataLengthInformation,Pseudocode for the operation is as follows:]\r\n" +
                        "                     The operation MUST be failed with STATUS_INVALID_PARAMETER " +
                        "under any of the following conditions:If Open.Stream.IsSparse is TRUE.");
                this.Manager.Comment("reaching state \'S23\'");
                this.Manager.Comment("checking step \'return SetFileValidDataLengthInfo/INVALID_PARAMETER\'");
                TestManagerHelpers.AssertAreEqual<Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus>(this.Manager, Microsoft.Protocols.TestSuites.FileSharing.FSA.Adapter.MessageStatus.INVALID_PARAMETER, temp10, "return of SetFileValidDataLengthInfo, state S23");
                this.Manager.Comment("reaching state \'S27\'");
                goto label1;
            }
            throw new InvalidOperationException("never reached");
        label1:
;
            this.Manager.EndTest();
        }
        
        private void SetFileValidDataLengthInformationTestCaseS2GetIfOpenHasManageVolumeAccessChecker(bool isHasManageVolumeAccess) {
            this.Manager.Comment("checking step \'return GetIfOpenHasManageVolumeAccess/[out False]\'");
            TestManagerHelpers.AssertAreEqual<bool>(this.Manager, false, isHasManageVolumeAccess, "isHasManageVolumeAccess of GetIfOpenHasManageVolumeAccess, state S7");
        }
        
        private void SetFileValidDataLengthInformationTestCaseS2GetIfOpenHasManageVolumeAccessChecker1(bool isHasManageVolumeAccess) {
            this.Manager.Comment("checking step \'return GetIfOpenHasManageVolumeAccess/[out True]\'");
            TestManagerHelpers.AssertAreEqual<bool>(this.Manager, true, isHasManageVolumeAccess, "isHasManageVolumeAccess of GetIfOpenHasManageVolumeAccess, state S7");
        }
        #endregion
    }
}
