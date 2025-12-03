namespace ExaminerS.Models
    {
    public class AppState
        {
        public AppState ()
            {
            Console.WriteLine ("ExaminerS.AppState Class ************** AppState instance created!");
            }
        public event Action? OnChange;

        public int? Offset { get; private set; }
        public void SetOffset (int _Offset) { Offset = _Offset; OnChange?.Invoke (); }
        public string? Message { get; private set; }
        public void SetMessage (string _Message) { Message = _Message; OnChange?.Invoke (); }
        public string? Cmd { get; private set; }
        public void SetCmd (string _Cmd) { Cmd = _Cmd; OnChange?.Invoke (); }
        public string? ReturnPage { get; private set; }
        public void SetReturnPage (string _ReturnPage) { ReturnPage = _ReturnPage; OnChange?.Invoke (); }

        //user (teacher|student)
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
        public string? CourseTopicName { get; private set; }
        public void SetCourseTopicName (string _CourseTopicName) { CourseTopicName = _CourseTopicName; OnChange?.Invoke (); }

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
        public int? MessageId { get; private set; }
        public void SetMessageId (int _MessageId) { MessageId = _MessageId; OnChange?.Invoke (); }
        }
    }
