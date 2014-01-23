//---------------------------------------------------------------------------------------------- 
//    Copyright 2012 Microsoft Corporation 
// 
//    Licensed under the Apache License, Version 2.0 (the "License"); 
//    you may not use this file except in compliance with the License. 
//    You may obtain a copy of the License at 
//      http://www.apache.org/licenses/LICENSE-2.0 
// 
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//    See the License for the specific language governing permissions and 
//    limitations under the License. 
//---------------------------------------------------------------------------------------------- 

namespace TokenClient.Common.Tokens
{
    /// <summary> 
    /// Defines the set of constants for the Simple Web Token. 
    /// </summary> 
    public static class SwtSecurityTokenConstants
    {
        public const string Audience = "Audience";
        public const string ExpiresOn = "ExpiresOn";
        public const string Id = "Id";
        public const string Issuer = "Issuer";
        public const string Signature = "HMACSHA256";
        public const string ValidFrom = "ValidFrom";
        public const string ValueTypeUri = "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";
    }
}