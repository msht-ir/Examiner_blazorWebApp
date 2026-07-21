namespace ExaminerS.Models
    {
    public class AppState
        {
        public AppState ()
            {
            Console.WriteLine ("ExaminerS.AppState Class ***** AppState instance created!");
            }
        public event Action? OnChange;

        public int? Offset { get; private set; }
        public void SetOffset (int _Offset) { Offset = _Offset; OnChange?.Invoke (); }
        public string? Message { get; private set; }
        public void SetMessage (string _Message) { Message = _Message; OnChange?.Invoke (); }
        public string? Cmd { get; private set; }
        public void SetCmd (string _Cmd) { Cmd = _Cmd; OnChange?.Invoke (); }
        public string? Clps { get; private set; }
        public void SetClps (string _Clps) { Clps = _Clps; OnChange?.Invoke (); }
        public string? ReturnPage { get; private set; }
        public void SetReturnPage (string _ReturnPage) { ReturnPage = _ReturnPage; OnChange?.Invoke (); }
        public bool? HelpOnHover {get; private set;}
        public void SetHelpOnHover (bool _HelpOnHover) { HelpOnHover = _HelpOnHover; OnChange?.Invoke (); }
        public int? ExamAlarm { get; private set; }
        public void SetExamAlarm (int _ExamAlarm) { ExamAlarm = _ExamAlarm; OnChange?.Invoke (); }

        //user (teacher|student)
        public int? DepartmentId { get; private set; }
        public void SetDepartmentId (int _DepartmentId) { DepartmentId=_DepartmentId; OnChange?.Invoke(); }
        public string? DepartmentName { get; private set; }
        public void SetDepartmentName (string _DepartmentName) { DepartmentName=_DepartmentName; OnChange?.Invoke(); }
        public int? TeacherId { get; private set; }
        public void SetTeacherId (int _TeacherId) { TeacherId = _TeacherId; OnChange?.Invoke (); }
        public int? UserId { get; private set; }
        public void SetUserId (int _UserId) { UserId = _UserId; OnChange?.Invoke (); }
        public string? UserNickname { get; private set; }
        public void SetUserNickname (string _userNickname) { UserNickname = _userNickname; OnChange?.Invoke (); }
        public string? UserName { get; private set; }
        public void SetUserName (string _userName) { UserName = _userName; OnChange?.Invoke (); }
        public string? UserPass { get; private set; }
        public void SetUserPass (string _userPass) { UserPass = _userPass; OnChange?.Invoke (); }
        public string? UserRole { get; private set; } = "Login";
        public void SetUserRole (string _UserRole) { UserRole = _UserRole; OnChange?.Invoke (); }
        public int? UserTags { get; private set; }
        public void SetUserTags (int _UserTags) { UserTags = _UserTags; OnChange?.Invoke (); }
        public int? UserLoginStatus { get; private set; }
        public void SetUserLoginStatus (int _UserLoginStatus) { UserLoginStatus = _UserLoginStatus; OnChange?.Invoke (); }

        //group
        public int? GroupId { get; private set; }
        public void SetGroupId (int _GroupId) { GroupId = _GroupId; OnChange?.Invoke (); }
        public string? GroupName { get; private set; }
        public void SetGroupName (string _GroupName) { GroupName = _GroupName; OnChange?.Invoke (); }

        //student ( as data for user:teacher)
        public int? StudentId { get; private set; }
        public void SetStudentId (int _StudentId) { StudentId = _StudentId; OnChange?.Invoke (); }
        public string? StudentName { get; private set; }
        public void SetStudentName (string _StudentName) { StudentName = _StudentName; OnChange?.Invoke (); }
        public int? StudentTags { get; private set; }
        public void SetStudentTags (int _StudentTags) { StudentTags = _StudentTags; OnChange?.Invoke (); }
        public string? StudentNickname { get; private set; }
        public void SetStudentNickname (string _StudentNickname) { StudentNickname = _StudentNickname; OnChange?.Invoke (); }

        //course
        public int? CourseId { get; private set; }
        public void SetCourseId (int _CourseId) { CourseId = _CourseId; OnChange?.Invoke (); }
        public string? CourseName { get; private set; }
        public void SetCourseName (string _CourseName) { CourseName = _CourseName; OnChange?.Invoke (); }
        public int? CourseTopicId { get; private set; }
        public void SetCourseTopicId (int _CourseTopicId) { CourseTopicId = _CourseTopicId; OnChange?.Invoke (); }
        public string? CourseTopicTitle { get; private set; }
        public void SetCourseTopicTitle (string _CourseTopicTitle) { CourseTopicTitle = _CourseTopicTitle; OnChange?.Invoke (); }

        //test
        public int? TestId { get; private set; }
        public void SetTestId (int _TestId) { TestId = _TestId; OnChange?.Invoke (); }
        public string? TestName { get; private set; }
        public void SetTestName (string _TestName) { TestName = _TestName; OnChange?.Invoke (); }
        public int? TestIndex { get; private set; }
        public void SetTestIndex (int _TestIndex) { TestIndex = _TestIndex; OnChange?.Invoke (); }
        //test-Option
        public int? TestOptionId { get; private set; }
        public void SetTestOptionId (int _TestOptionId) { TestOptionId = _TestOptionId; OnChange?.Invoke (); }
        public string? TestOptionName { get; private set; }
        public void SetTestOptionName (string _TestOptionName) { TestOptionName = _TestOptionName; OnChange?.Invoke (); }

        //exam
        public int? ExamId { get; private set; }
        public void SetExamId (int _ExamId) { ExamId = _ExamId; OnChange?.Invoke (); }
        public string? ExamName { get; private set; }
        public void SetExamName (string _ExamName) { ExamName = _ExamName; OnChange?.Invoke (); }
        public int? ExamTags { get; private set; }
        public void SetExamTags (int _ExamTags) { ExamTags = _ExamTags; OnChange?.Invoke (); }
        public int? ExamCompositionId { get; private set; }
        public void SetExamCompositionId (int _ExamCompositionId) { ExamCompositionId = _ExamCompositionId; OnChange?.Invoke (); }
        public string? ExamCompositionName { get; private set; }
        public void SetExamCompositionName (string _ExamCompositionName) { ExamCompositionName = _ExamCompositionName; OnChange?.Invoke (); }
        public int? ExamTestId { get; private set; }
        public void SetExamTestId (int _ExamTestId) { ExamTestId = _ExamTestId; OnChange?.Invoke (); }
        public string? ExamTestName { get; private set; }
        public void SetExamTestName (string _ExamTestName) { ExamTestName = _ExamTestName; OnChange?.Invoke (); }

        //studentExam
        public int? StudentExamId { get; private set; }
        public void SetStudentExamId (int _StudentExamId) { StudentExamId = _StudentExamId; OnChange?.Invoke (); }
        public string? StudentExamName { get; private set; }
        public void SetStudentExamName (string _StudentExamName) { StudentExamName = _StudentExamName; OnChange?.Invoke (); }

        //StudentExamTest
        public int? StudentExamTestId { get; private set; }
        public void SetStudentExamTestId (int _StudentExamTestId) { StudentExamTestId = _StudentExamTestId; OnChange?.Invoke (); }
        public string? StudentExamTestName { get; private set; }
        public void SetStudentExamTestName (string _StudentExamTestName) { StudentExamTestName = _StudentExamTestName; OnChange?.Invoke (); }
        public int? HelpedTestId { get; private set; }
        public void SetHelpedTestId (int _HelpedTestId) { HelpedTestId = _HelpedTestId; OnChange?.Invoke (); }
        //studentCourse
        public int? StudentCourseId { get; private set; }
        public void SetStudentCourseId (int _StudentCourseId) { StudentCourseId = _StudentCourseId; OnChange?.Invoke (); }
        public string? StudentCourseName { get; private set; }
        public void SetStudentCourseName (string _StudentCourseName) { StudentCourseName = _StudentCourseName; OnChange?.Invoke (); }
        //Message
        public int? StudentMessageId { get; private set; }
        public void SetStudentMessageId (int _StudentMessageId) { StudentMessageId = _StudentMessageId; OnChange?.Invoke (); }
        public int? MessageId { get; private set; }
        public void SetMessageId (int _MessageId) { MessageId = _MessageId; OnChange?.Invoke (); }
        public string? MessageTitle { get; private set; }
        public void SetMessageTitle (string _MessageTitle) { MessageTitle = _MessageTitle; OnChange?.Invoke (); }
        public string? MessageBody { get; private set; }
        public void SetMessageBody (string _MessageBody) { MessageBody = _MessageBody; OnChange?.Invoke (); }
        //Chat
        public int? SelectedChatId { get; private set; }
        public void SetSelectedChatId (int _SelectedChatId) { SelectedChatId = _SelectedChatId; OnChange?.Invoke (); }
        public int? ChatMateId { get; private set; }
        public void SetChatMateId (int _ChatMateId) { ChatMateId = _ChatMateId; OnChange?.Invoke (); }
        //Chatroom
        public int? SelectedChatroomId { get; private set; }
        public void SetSelectedChatroomId (int _SelectedChatroomId) { SelectedChatroomId = _SelectedChatroomId; OnChange?.Invoke (); }
        public string? SelectedChatroomName { get; private set; }
        public void SetSelectedChatroomName (string _SelectedChatroomName) { SelectedChatroomName = _SelectedChatroomName; OnChange?.Invoke (); }
        public int? SelectedChatroomPostId { get; private set; }
        public void SetSelectedChatroomPostId (int _SelectedChatroomPostId) { SelectedChatroomPostId = _SelectedChatroomPostId; OnChange?.Invoke (); }
        //Project
        public int? ProjectId { get; private set; }
        public void SetProjectId (int _ProjectId) { ProjectId = _ProjectId; OnChange?.Invoke (); }
        public string? ProjectName { get; private set; }
        public void SetProjectName (string _ProjectName) { ProjectName = _ProjectName; OnChange?.Invoke (); }
        //Subproject
        public int? SubprojectId { get; private set; }
        public void SetSubprojectId (int _SubprojectId) { SubprojectId = _SubprojectId; OnChange?.Invoke (); }
        public string? SubprojectName { get; private set; }
        public void SetSubprojectName (string _SubprojectName) { SubprojectName = _SubprojectName; OnChange?.Invoke (); }
        //Note
        public int? SelectedNoteId { get; private set; }
        public void SetSelectedNoteId (int _SelectedNoteId) { SelectedNoteId = _SelectedNoteId; OnChange?.Invoke (); }
        public int? NoteId { get; private set; }
        public void SetNoteId (int _NoteId) { NoteId = _NoteId; OnChange?.Invoke (); }
        public int? NoteReferenceId { get; private set; }
        public void SetNoteReferenceId (int _NoteReferenceId) { NoteReferenceId = _NoteReferenceId; OnChange?.Invoke (); }
        public int? NoteReferenceType { get; private set; }
        public void SetNoteReferenceType (int _NoteReferenceType) { NoteReferenceType = _NoteReferenceType; OnChange?.Invoke (); }
        public string? NoteReferenceName { get; private set; }
        public void SetNoteReferenceName (string _NoteReferenceName) { NoteReferenceName = _NoteReferenceName; OnChange?.Invoke (); }
        public string? NoteDateTime { get; private set; }
        public void SetNoteDateTime (string _NoteDateTime) { NoteDateTime = _NoteDateTime; OnChange?.Invoke (); }
        public string? NoteDueDateTime { get; private set; }
        public void SetNoteDueDateTime (string _NoteDueDateTime) { NoteDueDateTime = _NoteDueDateTime; OnChange?.Invoke (); }
        public string? NoteText { get; private set; }
        public void SetNoteText (string _NoteText) { NoteText = _NoteText; OnChange?.Invoke (); }
        public int? NoteCreatorId {get; private set;}
        public void SetNoteCreatorId (int _NoteCreatorId) { NoteCreatorId = _NoteCreatorId; OnChange?.Invoke (); }
        public bool NoteIsRtl { get; private set; }
        public void SetNoteIsRtl (bool _NoteIsRtl) { NoteIsRtl = _NoteIsRtl; OnChange?.Invoke (); }
        public bool NoteIsTodo { get; private set; }
        public void SetNoteIsTodo (bool _NoteIsTodo) { NoteIsTodo = _NoteIsTodo; OnChange?.Invoke (); }
        }
    }
