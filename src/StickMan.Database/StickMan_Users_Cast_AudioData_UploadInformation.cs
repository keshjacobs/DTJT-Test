//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StickMan.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class StickMan_Users_Cast_AudioData_UploadInformation
    {
        public Nullable<int> UserID { get; set; }
        public Nullable<int> RecieverID { get; set; }
        public string AudioFilePath { get; set; }
        public Nullable<System.DateTime> UploadTime { get; set; }
        public string Filter { get; set; }
        public Nullable<bool> ReadStatus { get; set; }
        public Nullable<bool> DeleteStatus { get; set; }
        public Nullable<int> ClickCount { get; set; }
        public int Id { get; set; }
    }
}
