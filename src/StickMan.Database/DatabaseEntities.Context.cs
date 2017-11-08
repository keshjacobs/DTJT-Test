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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EfStickManContext : DbContext
    {
        public EfStickManContext()
            : base("name=EfStickManConnectionString")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<StickMan_Users> StickMan_Users { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<StickMan_FriendRequest> StickMan_FriendRequest { get; set; }
        public virtual DbSet<StickMan_Users_AudioData_UploadInformation> StickMan_Users_AudioData_UploadInformation { get; set; }
        public virtual DbSet<StickMan_Users_Cast_AudioData_UploadInformation> StickMan_Users_Cast_AudioData_UploadInformation { get; set; }
        public virtual DbSet<StickMan_UserSesion> StickMan_UserSesion { get; set; }
        public virtual DbSet<StickMan_UsersFriendList> StickMan_UsersFriendList { get; set; }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual ObjectResult<spAndroidLogin_Result> spAndroidLogin(string login, string password)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spAndroidLogin_Result>("spAndroidLogin", loginParameter, passwordParameter);
        }
    
        public virtual int spAndroidRegister(string login, string name, string password, string email, string dOB, string sex)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));
    
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var dOBParameter = dOB != null ?
                new ObjectParameter("DOB", dOB) :
                new ObjectParameter("DOB", typeof(string));
    
            var sexParameter = sex != null ?
                new ObjectParameter("sex", sex) :
                new ObjectParameter("sex", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spAndroidRegister", loginParameter, nameParameter, passwordParameter, emailParameter, dOBParameter, sexParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_Cast_GetAudioFileMessages_Result> StickMan_usp_Cast_GetAudioFileMessages(string sessionToken, Nullable<int> userID)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_Cast_GetAudioFileMessages_Result>("StickMan_usp_Cast_GetAudioFileMessages", sessionTokenParameter, userIDParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_CastSaveAudioPath_Result> StickMan_usp_CastSaveAudioPath(Nullable<int> userID, Nullable<int> recieverID, string filePath, string filter, string sessionToken)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var recieverIDParameter = recieverID.HasValue ?
                new ObjectParameter("RecieverID", recieverID) :
                new ObjectParameter("RecieverID", typeof(int));
    
            var filePathParameter = filePath != null ?
                new ObjectParameter("FilePath", filePath) :
                new ObjectParameter("FilePath", typeof(string));
    
            var filterParameter = filter != null ?
                new ObjectParameter("Filter", filter) :
                new ObjectParameter("Filter", typeof(string));
    
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_CastSaveAudioPath_Result>("StickMan_usp_CastSaveAudioPath", userIDParameter, recieverIDParameter, filePathParameter, filterParameter, sessionTokenParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_CreateFB_User_Result> StickMan_usp_CreateFB_User(Nullable<int> userID, string userName, string fullName, string password, string mobileNo, string emailID, Nullable<System.DateTime> dOB, string sex, string imagePath, string deviceId)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var fullNameParameter = fullName != null ?
                new ObjectParameter("FullName", fullName) :
                new ObjectParameter("FullName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var mobileNoParameter = mobileNo != null ?
                new ObjectParameter("MobileNo", mobileNo) :
                new ObjectParameter("MobileNo", typeof(string));
    
            var emailIDParameter = emailID != null ?
                new ObjectParameter("EmailID", emailID) :
                new ObjectParameter("EmailID", typeof(string));
    
            var dOBParameter = dOB.HasValue ?
                new ObjectParameter("DOB", dOB) :
                new ObjectParameter("DOB", typeof(System.DateTime));
    
            var sexParameter = sex != null ?
                new ObjectParameter("Sex", sex) :
                new ObjectParameter("Sex", typeof(string));
    
            var imagePathParameter = imagePath != null ?
                new ObjectParameter("ImagePath", imagePath) :
                new ObjectParameter("ImagePath", typeof(string));
    
            var deviceIdParameter = deviceId != null ?
                new ObjectParameter("DeviceId", deviceId) :
                new ObjectParameter("DeviceId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_CreateFB_User_Result>("StickMan_usp_CreateFB_User", userIDParameter, userNameParameter, fullNameParameter, passwordParameter, mobileNoParameter, emailIDParameter, dOBParameter, sexParameter, imagePathParameter, deviceIdParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_CreateUpdate_User_Result> StickMan_usp_CreateUpdate_User(Nullable<int> userID, string userName, string fullName, string password, string mobileNo, string emailID, Nullable<System.DateTime> dOB, string sex, string imagePath, string deviceId)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var fullNameParameter = fullName != null ?
                new ObjectParameter("FullName", fullName) :
                new ObjectParameter("FullName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var mobileNoParameter = mobileNo != null ?
                new ObjectParameter("MobileNo", mobileNo) :
                new ObjectParameter("MobileNo", typeof(string));
    
            var emailIDParameter = emailID != null ?
                new ObjectParameter("EmailID", emailID) :
                new ObjectParameter("EmailID", typeof(string));
    
            var dOBParameter = dOB.HasValue ?
                new ObjectParameter("DOB", dOB) :
                new ObjectParameter("DOB", typeof(System.DateTime));
    
            var sexParameter = sex != null ?
                new ObjectParameter("Sex", sex) :
                new ObjectParameter("Sex", typeof(string));
    
            var imagePathParameter = imagePath != null ?
                new ObjectParameter("ImagePath", imagePath) :
                new ObjectParameter("ImagePath", typeof(string));
    
            var deviceIdParameter = deviceId != null ?
                new ObjectParameter("DeviceId", deviceId) :
                new ObjectParameter("DeviceId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_CreateUpdate_User_Result>("StickMan_usp_CreateUpdate_User", userIDParameter, userNameParameter, fullNameParameter, passwordParameter, mobileNoParameter, emailIDParameter, dOBParameter, sexParameter, imagePathParameter, deviceIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> StickMan_usp_DeleteStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("StickMan_usp_DeleteStatus");
        }
    
        public virtual ObjectResult<StickMan_usp_FbLogin_User_Result> StickMan_usp_FbLogin_User(string userName, string emailID, string deviceId)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var emailIDParameter = emailID != null ?
                new ObjectParameter("EmailID", emailID) :
                new ObjectParameter("EmailID", typeof(string));
    
            var deviceIdParameter = deviceId != null ?
                new ObjectParameter("DeviceId", deviceId) :
                new ObjectParameter("DeviceId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_FbLogin_User_Result>("StickMan_usp_FbLogin_User", userNameParameter, emailIDParameter, deviceIdParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_GetAudioFileMessages_Result> StickMan_usp_GetAudioFileMessages(string sessionToken, Nullable<int> userID)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_GetAudioFileMessages_Result>("StickMan_usp_GetAudioFileMessages", sessionTokenParameter, userIDParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_GetBlockedFriends_Result> StickMan_usp_GetBlockedFriends(string sessionToken, Nullable<int> userId)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_GetBlockedFriends_Result>("StickMan_usp_GetBlockedFriends", sessionTokenParameter, userIdParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_GetFriends_Result> StickMan_usp_GetFriends(string sessionToken, Nullable<int> userId)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_GetFriends_Result>("StickMan_usp_GetFriends", sessionTokenParameter, userIdParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_GetPendingFriendRequests_Result> StickMan_usp_GetPendingFriendRequests(string sessionToken, Nullable<int> userId)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_GetPendingFriendRequests_Result>("StickMan_usp_GetPendingFriendRequests", sessionTokenParameter, userIdParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_GetUsersList_Result> StickMan_usp_GetUsersList(string sessionToken, Nullable<int> userId, string searchKeyword)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var searchKeywordParameter = searchKeyword != null ?
                new ObjectParameter("SearchKeyword", searchKeyword) :
                new ObjectParameter("SearchKeyword", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_GetUsersList_Result>("StickMan_usp_GetUsersList", sessionTokenParameter, userIdParameter, searchKeywordParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_Login_User_Result> StickMan_usp_Login_User(string userName, string password, string deviceId)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var deviceIdParameter = deviceId != null ?
                new ObjectParameter("DeviceId", deviceId) :
                new ObjectParameter("DeviceId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_Login_User_Result>("StickMan_usp_Login_User", userNameParameter, passwordParameter, deviceIdParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_ResponseFriendRequest_Result> StickMan_usp_ResponseFriendRequest(Nullable<int> userID, Nullable<int> respondingToUserID, Nullable<int> friendRequestID, Nullable<int> friendRequestReply, string sessionToken)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var respondingToUserIDParameter = respondingToUserID.HasValue ?
                new ObjectParameter("RespondingToUserID", respondingToUserID) :
                new ObjectParameter("RespondingToUserID", typeof(int));
    
            var friendRequestIDParameter = friendRequestID.HasValue ?
                new ObjectParameter("FriendRequestID", friendRequestID) :
                new ObjectParameter("FriendRequestID", typeof(int));
    
            var friendRequestReplyParameter = friendRequestReply.HasValue ?
                new ObjectParameter("FriendRequestReply", friendRequestReply) :
                new ObjectParameter("FriendRequestReply", typeof(int));
    
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_ResponseFriendRequest_Result>("StickMan_usp_ResponseFriendRequest", userIDParameter, respondingToUserIDParameter, friendRequestIDParameter, friendRequestReplyParameter, sessionTokenParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_SaveAudioPath_Result> StickMan_usp_SaveAudioPath(Nullable<int> userID, Nullable<int> recieverID, string filePath, string filter, string sessionToken)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var recieverIDParameter = recieverID.HasValue ?
                new ObjectParameter("RecieverID", recieverID) :
                new ObjectParameter("RecieverID", typeof(int));
    
            var filePathParameter = filePath != null ?
                new ObjectParameter("FilePath", filePath) :
                new ObjectParameter("FilePath", typeof(string));
    
            var filterParameter = filter != null ?
                new ObjectParameter("Filter", filter) :
                new ObjectParameter("Filter", typeof(string));
    
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_SaveAudioPath_Result>("StickMan_usp_SaveAudioPath", userIDParameter, recieverIDParameter, filePathParameter, filterParameter, sessionTokenParameter);
        }
    
        public virtual ObjectResult<StickMan_usp_SendFriendRequest_Result> StickMan_usp_SendFriendRequest(Nullable<int> senderID, Nullable<int> recieverID, string sessionToken)
        {
            var senderIDParameter = senderID.HasValue ?
                new ObjectParameter("SenderID", senderID) :
                new ObjectParameter("SenderID", typeof(int));
    
            var recieverIDParameter = recieverID.HasValue ?
                new ObjectParameter("RecieverID", recieverID) :
                new ObjectParameter("RecieverID", typeof(int));
    
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<StickMan_usp_SendFriendRequest_Result>("StickMan_usp_SendFriendRequest", senderIDParameter, recieverIDParameter, sessionTokenParameter);
        }
    
        public virtual ObjectResult<usp_CastGetDeleteStatus_Result> usp_CastGetDeleteStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_CastGetDeleteStatus_Result>("usp_CastGetDeleteStatus");
        }
    
        public virtual int usp_DeleteFileFromServer()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_DeleteFileFromServer");
        }
    
        public virtual int usp_GetDeleteStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_GetDeleteStatus");
        }
    }
}
