:: Copyright (c) Microsoft. All rights reserved.
:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

call CommonRunTestCase.cmd  "TestCategory=RDPEDISP&TestCategory=BVT" 
%RunRDPTestSuite%
pause
