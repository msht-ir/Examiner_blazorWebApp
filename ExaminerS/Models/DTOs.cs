using System.ComponentModel.DataAnnotations;

namespace ExaminerS.Models
    {
    //U
    public class User
        {
        public int UserId { get; set; } = 0;
        public int TeacherId { get; set; } = 0;
        [Required (ErrorMessage = "Username is required")]
        public string UserName { get; set; } = "";
        [Required (ErrorMessage = "Password is required")]
        public string UserPass { get; set; } = "";
        [Required (ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; } = "";
        public string UserNickname { get; set; } = "";
        public int UserTags { get; set; } = 0;  //1:IsActive 2:CanChangePass 4:CanReviewExam 8:CanGetStudentExamTests 16:CanCorrectStudentExamTests 32:CanReviewStudentExamTests
        //wrapper properties for each flag
        public bool IsActive
            {
            get => (UserTags & 1) == 1;
            set
                {
                if (value)
                    UserTags |= 1;
                else
                    UserTags &= ~1;
                }
            }
        public bool CanChangePass
            {
            get => (UserTags & 2) == 2;
            set
                {
                if (value)
                    UserTags |= 2;
                else
                    UserTags &= ~2;
                }
            }
        public bool IsSelected
            {
            get => (UserTags & 64) == 64;
            set
                {
                if (value)
                    UserTags |= 64;
                else
                    UserTags &= ~64;
                }
            }
        public List<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup> ();
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse> ();
        public List<StudentExam> StudentExams { get; set; } = new List<StudentExam> ();
        public List<StudentMessage> StudentMessages { get; set; } = new List<StudentMessage> ();
        public List<Note> StudentNotes { get; set; } = new List<Note> ();
        }
    //G
    public class Group
        {
        public int GroupId { get; set; } = 0;
        public string GroupName { get; set; } = "";
        public int UserId { get; set; } = 0;
        public List<StudentGroup> Students { get; set; } = new List<StudentGroup> ();
        }
     //SG
    public class StudentGroup
        {
        public int StudentGroupId { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        public int GroupId { get; set; } = 0;
        public string DateTimeJoined { get; set; } = "";
        public int StudentGroupTags { get; set; } = 0;
        public string GroupName { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string StudentNickname { get; set; } = "";
        public int StudentTags { get; set; } = 0;
        }  
    //C
    public class Course
        {
        public int CourseId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string CourseName { get; set; } = "";
        public int CourseUnits { get; set; } = 0;
        public bool CourseRtl { get; set; } = false;
        public int CourseIndex { get; set; } = 0;
        public List<CourseTopic> CourseTopics { get; set; } = new List<CourseTopic> ();
        public List<CourseFolder> CourseFolders { get; set; } = new List<CourseFolder> ();
        public List<StudentCourse> Students { get; set; } = new List<StudentCourse> ();
        }   
    //CF
    public class CourseFolder
        {
        public int CourseFolderId { get; set; } = 0;
        public int CourseId { get; set; } = 0;
        public string CourseFolderTitle { get; set; } = "";
        public string CourseFolderUrl { get; set; } = "";
        public bool CourseFolderActive { get; set; } = false;
        }
    //CT
    public class CourseTopic
        {
        public int CourseTopicId { get; set; } = 0;
        public int CourseId { get; set; } = 0;
        public string CourseTopicTitle { get; set; } = "";
        }
    //SC
    public class StudentCourse
        {
        public int StudentCourseId { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        public int CourseId { get; set; } = 0;
        public string CourseName { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string StudentNickname { get; set; } = "";
        public int NumberOfTests { get; set; } = 0;
        public int CorrectAnswers { get; set; } = 0;
        public int StudentCourseTags { get; set;  } = 0;
        public bool IsActive
            {
            get => (StudentCourseTags & 1) == 1;
            set
                {
                if (value)
                    StudentCourseTags |= 1;
                else
                    StudentCourseTags &= ~1;
                }
            }
        public bool Retry
            {
            get => (StudentCourseTags & 2) == 2;
            set
                {
                if (value)
                    StudentCourseTags |= 2;
                else
                    StudentCourseTags &= ~2;
                }
            }
        public bool Review
            {
            get => (StudentCourseTags & 4) == 4;
            set
                {
                if (value)
                    StudentCourseTags |= 4;
                else
                    StudentCourseTags &= ~4;
                }
            }
        }
    //SCT
    public class StudentCourseTest
        {
        public int StudentCourseId { get; set; } = 0;
        public int TestId { get; set; } = 0;
        public int Opt1Id { get; set; } = 0;
        public int Opt2Id { get; set; } = 0;
        public int Opt3Id { get; set; } = 0;
        public int Opt4Id { get; set; } = 0;
        public int Opt5Id { get; set; } = 0;
        public int TestKey { get; set; } = 0;
        public int UserAns { get; set; } = 0;
        public string DateTime { get; set; } = "";
        public int TestIndex { get; set; } = 0; //not used?
        public string TestTitle { get; set; } = "";
        public int TestType { get; set; } = 0; //[1-5]
        public int CourseTopicId { get; set; } = 0;
        public string CourseTopicTitle { get; set; } = "";
        public int TestTags { get; set; } = 0; //1:testRtl 2:optionsRtl
        public int TestLevel { get; set; } = 0; //[1-5]
        public List<TestOption>? TestOptions { get; set; } = new List<TestOption> ();
        }
    //T
    public class Test
        {
        public int TestId { get; set; } = 0;
        public int CourseId { get; set; } = 0;
        public int TopicId { get; set; } = 0;
        public string TestTitle { get; set; } = "";
        public int TestType { get; set; } = 0;
        public int TestLevel { get; set; } = 0;
        public int TestTags { get; set; } = 0;      //1:TestRtl 2:OptionsRtl 4:IsSelected (select some tests in a list, to be added to an exam later)
        public List<TestOption>? TestOptions { get; set; } = new List<TestOption> ();
        }
    //TO
    public class TestOption
        {
        public int TestOptionId { get; set; } = 0;
        public int TestId { get; set; } = 0;
        public string TestOptionTitle { get; set; } = "";
        public int TestOptionTags { get; set; } = 0; //1:ForceLast 2:IsAns
        }
    //E
    public class Exam
        {
        public int ExamId { get; set; } = 0;
        public int CourseId { get; set; } = 0;
        public string ExamTitle { get; set; } = "";
        public string ExamDateTime { get; set; } = "";
        public int ExamDuration { get; set; } = 0;
        public int ExamNTests { get; set; } = 0;
        public int ExamTags { get; set; } = 0; //1:active 2:SampleTestMode 4:TrainingMode 8:RealExamMode
        //wrapper properties for each flag
        public bool IsActive
            {
            get => (ExamTags & 1) == 1;
            set
                {
                if (value)
                    ExamTags |= 1;
                else
                    ExamTags &= ~1;
                }
            }
        public bool IsTrainingMode
            {
            get => (ExamTags & 2) == 2;
            set
                {
                if (value)
                    ExamTags |= 2;
                else
                    ExamTags &= ~2;
                }
            }
        public List<StudentExam> Students { get; set; } = new List<StudentExam> ();
        }
    //EC
    public class ExamComposition
        {
        public int ExamCompositionId { get; set; } = 0;
        public int ExamId { get; set; } = 0;
        public int TopicId { get; set; } = 0;
        public int TopicNTests { get; set; } = 0;
        public int TestsLevel { get; set; } = 0;
        }
    //ET
    public class ExamTest
        {
        public int ExamTestId { get; set; } = 0;
        public int ExamId { get; set; } = 0;
        public int TestId { get; set; } = 0;
        public int PercentCorrect { get; set; } = 0;
        public int PercentIncorrect { get; set; } = 0;
        public int PercentHelped { get; set; } = 0;
        public List<TestOption>? TestOptions { get; set; } = new List<TestOption> ();
        }
    //SE
    public class StudentExam
        {
        public int StudentExamId { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        public string StudentName { get; set; } = "";
        public string StudentNickname { get; set; } = "";
        public int CourseId { get; set; } = 0;
        public string CourseName { get; set; } = "";
        public int ExamId { get; set; } = 0;
        public string ExamTitle { get; set; } = "";
        public string ExamDateTime { get; set; } = "";
        public int ExamDuration { get; set; } = 0;
        public int ExamNTests { get; set; } = 0;
        public int ExamTags { get; set; } = 0;
        public string StartDateTime { get; set; } = "";
        public string FinishDateTime { get; set; } = "";
        public int StudentExamTags { get; set; } = 0;   //1:started 2:finished
        public Double StudentExamPoint { get; set; } = 0;
        public int ExamIndex { get; set; } = 0;
        public List<StudentExamTest> StudentExamTests { get; set; } = new List<StudentExamTest> ();
        }
    //SET
    public class StudentExamTest
        {
        public long StudentExamTestId { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        public int StudentExamId { get; set; } = 0;
        public int TestId { get; set; } = 0;
        public int Opt1Id { get; set; } = 0;
        public int Opt2Id { get; set; } = 0;
        public int Opt3Id { get; set; } = 0;
        public int Opt4Id { get; set; } = 0;
        public int Opt5Id { get; set; } = 0;
        public int StudentExamTestKey { get; set; } = 0;
        public int StudentExamTestAns { get; set; } = 0;
        public int StudentExamTestTags { get; set; } = 0; //1:visited 2:bookmarked 4:answered 8:‍helped 16:revised 32:problemreported
        public int TestIndex { get; set; } = 0;
        public string TestTitle { get; set; } = "";
        public int TestType { get; set; } = 0; //[1-5]
        public int CourseTopicId { get; set; } = 0;
        public string CourseTopicTitle { get; set; } = "";
        public int TestTags { get; set; } = 0;             //1:testRtl 2:optionsRtl
        public int TestLevel { get; set; } = 0;            //[1-5]
        public List<TestOption>? TestOptions { get; set; } = new List<TestOption> ();
        //derived property for easy binding
        public bool IsSelected
            {
            get => (TestTags & 64) == 64;
            set
                {
                if (value)
                    TestTags |= 64;   //set bit 6
                else
                    TestTags &= ~64;  //clear bit 6
                }
            }

        }
    //M
    public class Message
        {
        public int MessageId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string DateTimeCreated { get; set; } = "";
        public string MessageTitle { get; set; } = "";
        public string MessageBody { get; set; } = "";
        public List<StudentMessage> Students { get; set; } = new List<StudentMessage> ();
        }
    //SM
    public class StudentMessage
        {
        public int StudentMessageId { get; set; } = 0;
        public int MessageId { get; set; } = 0;
        public int StudentId { get; set; } = 0;
        public string StudentName { get; set; } = "";
        public string StudentNickname { get; set; } = "";
        public string MessageTitle { get; set; } = "";
        public string MessageBody { get; set; } = "";
        public string DateTimeCreated { get; set; } = "";
        public string DateTimeSent { get; set; } = "";
        public string DateTimeRead { get; set; } = "";
        public int StudentMessageTags { get; set; } = 0;

        }
    //P
    public class Project
        {
        public int ProjectId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string ProjectName { get; set; } = "";
        public int ProjectTags { get; set; }
        public List<Subproject> Subprojects { get; set; } = new List<Subproject> ();
        public List<Note> Notes { get; set; } = new List<Note> ();
        }
    //SP
    public class Subproject
        {
        public int SubprojectId { get; set; } = 0;
        public int ProjectId { get; set; } = 0;
        public string SubprojectName { get; set; } = "";
        public int SubprojectTags { get; set; } = 0;
        public List<Note> Notes { get; set; } = new List<Note> ();
        }
    //N
    public class Note
        {
        public int NoteId { get; set; } = 0;
        public int ParentId { get; set; } = 0;
        public int ParentType { get; set; } = 0; //1:SP 2:St 3:Gr 4:Cr 5:Ex
        public string ParentName { get; set; } = "";
        public string NoteDatum { get; set; } = "";
        public string NoteText { get; set; } = "";
        public int NoteTags { get; set; } = 0; //1:rtl 2:done 3:shared 4:readonly 
        public bool IsRtl
            {
            get => (NoteTags & 1) == 1;
            set
                {
                if (value)
                    NoteTags |= 1;
                else
                    NoteTags &= ~1;
                }
            }

        }
    //NN
    public class NoteNet
        {
        public int NoteNetId { get; set; } = 0;
        public int NoteAId { get; set; } = 0;
        public int NoteBId { get; set; } = 0;
        }
    }
