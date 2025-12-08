using ExaminerS.Models;
using Microsoft.Data.SqlClient;

namespace ExaminerB.Services2Backend
    {
    public interface BeIService
        {
        # region Login
        Task<User?> LoginTeacherAsync (User user);
        Task<User?> LoginStudentAsync (User user);
        Task LogAsync (int userId, SqlConnection cnn);
        #endregion
        #region C01:usrs
        Task<int> Create_TeacherAsync (User user);
        Task<List<User>> Read_TeachersAsync ();
        Task<bool> Update_TeacherPasswordAsync (User user);
        Task<bool> Delete_TeacherAsync (int userId);
        #endregion
        #region C10:Students
        Task<int> Create_StudentAsync (User student);
        Task<List<User>> Read_StudentsAsync (int userId, bool readStudentExams, bool readStudentCourses);
        Task<List<User>> Read_StudentsByGroupIdAsync (int groupId, bool readStudentExams, bool readStudentCourses);
        Task<List<User>> Read_StudentsByExamIdAsync (int examId, bool readStudentExams, bool readStudentCourses);
        Task<List<User>> Read_StudentsByCourseIdAsync (int courseId, bool readStudentExams, bool readStudentCourses);
        Task<User> Read_StudentAsync (int studentId, bool readStudentExams, bool readStudentCourses);
        Task<bool> Update_StudentsTagsAsync (User student);
        Task<bool> Update_StudentAsync (User studen);
        Task<bool> Update_StudentPasswordAsync (User user);
        Task<bool> Delete_StudentsAsync (int groupId);
        Task<bool> Delete_StudentAsync (int studentId);
        #endregion
        #region C02:Courses
        Task<int> Create_CourseAsync (Course course);
        Task<List<Course>> Read_CoursesAsync (int userId);
        Task<bool> Update_CourseAsync (Course course);
        Task<bool> Delete_CourseAsync (int courseId);
        #endregion
        #region C03:CourseTopics
        Task<int> Create_CourseTopicAsync (CourseTopic courseTopic);
        Task<List<CourseTopic>> Read_CourseTopicsAsync (int courseId);
        Task<bool> Update_CourseTopicAsync (CourseTopic courseTopic);
        Task<bool> Delete_CourseTopicAsync (int courseTopicId);
        #endregion
        #region C15:CourseFolders
        Task<int> Create_CourseFolderAsync (CourseFolder courseFolder);
        Task<List<CourseFolder>> Read_CourseFoldersAsync (int courseId);
        Task<bool> Update_CourseFolderAsync (CourseFolder courseFolder);
        Task<bool> Delete_CourseFolderAsync (int courseFolderId);
        #endregion
        #region C04:Tests
        Task<int> Create_TestAsync (Test test);
        Task<Test> Read_TestByTestIdAsync (int testId, bool readOptions);
        Task<Test> Read_TestByStudentExamTestIdAsync (long studentExamTestId, bool readOptions);
        Task<List<Test>> Read_TestsByCourseIdAsync (int courseId, int pageNumber, bool readOptions);
        Task<List<Test>> Read_TestsByCourseTopicIdAsync (int courseTopicId, bool readOptions);
        Task<List<Test>> Read_TestsBySearchAsync (string strSearch, int courseId, bool readOptions);
        Task<List<Test>> Read_TestsByExamIdAsync (int examId, bool readOptions);
        Task<List<Test>> Read_TestsByStudentExamIdAsync (int studentExamId, bool readOptions);
        Task<List<Test>> Read_TestsByStudentCourseIdAsync (int studentCourseId, bool readOptions);
        Task<bool> Update_TestAsync (Test test);
        Task<bool> Delete_TestAsync (int testId);
        Task<bool> ImportExcelTestsAsync (string filePath, int courseId);
        #endregion
        #region C05:TestOptions
        Task<int> Create_TestOptionAsync (TestOption testOption);
        Task<List<TestOption>> Read_TestOptionsAsync (int testId, SqlConnection cnn);
        Task<TestOption> Read_TestOptionAsync (int testOptionId, SqlConnection cnn);
        Task<bool> Update_TestOptionAsync (TestOption testOption);
        Task<bool> Delete_TestOptionsAsync (int testOptionId);
        Task<bool> Delete_TestOptionAsync (int testId);
        #endregion
        #region C06:Exams
        Task<int> Create_ExamAsync (Exam exam);
        Task<List<Exam>> Read_ExamsAsync (int courseId);
        Task<Exam> Read_ExamAsync (int examId);
        Task<bool> Update_ExamAsync (Exam exam);
        Task<bool> Delete_ExamsAsync (int courseId);
        Task<bool> Delete_ExamAsync (int examId);
        #endregion
        #region C07:ExamCompositions
        Task<int> Create_ExamCompositionAsync (ExamComposition examComposition);
        Task<List<ExamComposition>> Read_ExamCompositionsAsync (int examId);
        Task<ExamComposition> Read_ExamCompositionAsync (int examCompositionId);
        Task<bool> Update_ExamCompositionAsync (ExamComposition examComposition);
        Task<bool> Delete_ExamCompositionsAsync (int examId);
        Task<bool> Delete_ExamCompositionAsync (int examCompositionId);
        #endregion
        #region C08:ExamTests
        Task<int> Create_ExamTestsByExamCompositionAsync (ExamComposition examComposition);
        Task<int> Create_ExamTestAsync (ExamTest examTest);
        Task<List<ExamTest>> Read_ExamTestsAsync (int examId);
        Task<ExamTest> Read_ExamTestAsync (int examTestId);
        Task<bool> Update_ExamTestAsync (ExamTest examTest);
        Task<bool> Delete_ExamTestsAsync (int examId);
        Task<bool> Delete_ExamTestAsync (ExamTest examTest);
        #endregion
        #region C09:Groups
        Task<int> Create_GroupAsync (Group group);
        Task<List<Group>> Read_GroupsAsync (User user, bool getStudentExams, bool getStudentCourses);
        Task<Group> Read_GroupAsync (int groupId, bool getStudentExams, bool getStudentCourses);
        Task<bool> Update_GroupAsync (Group group);
        Task<bool> Delete_GroupsAsync (User user);
        Task<int> Delete_GroupAsync (int groupId);
        #endregion
        #region C11:StudentExams
        Task<int> Create_StudentExamsAsync (StudentExam studentExam, int groupId);
        Task<int> Create_StudentExamAsync (StudentExam studentExam);
        Task<List<StudentExam>> Read_StudentExamsAsync (int studentId, bool readInactiveExams);
        Task<StudentExam> Read_StudentExamAsync (int studentExamId, bool readInactiveExams);
        Task<bool> Update_StudentExamAsync (StudentExam studentExam);
        Task<bool> Update_StudentExamTagsAsync (StudentExam tempStudentExam);
        Task<bool> Delete_StudentExamsByStudentIdAsync (int studentId);
        Task<bool> Delete_StudentExamsByExamIdAsync (int examId);
        Task<bool> Delete_StudentExamAsync (int studentExamId);
        Task<bool> CalculatePoints_StudentExamsAsync (int studentexamid);
        #endregion
        #region C12:StudentExamTests
        Task<int> Create_StudentExamTestAsync (StudentExamTest studentExamTest);
        Task<StudentExamTest> Read_StudentExamTestAsync (long studentExamTestId, bool readOptions);
        Task<List<StudentExamTest>> Read_StudentExamTestsAsync (int studentExamId, bool readOptions);
        Task<List<StudentExamTest>> Read_StudentExamTestsAsync (int studentExamId, bool readOptions, SqlConnection cnn);
        Task<List<StudentExamTest>> Read_StudentsExamTestAsync (StudentExamTest studentExamTest);
        Task<bool> Update_StudentExamTestAsync (StudentExamTest studentExamTest);
        Task<bool> Update_StudentExamTestsTagsAsync (StudentExamTest tempStudentExamTest);
        Task<bool> Update_StudentExamTestTagsAsync (StudentExamTest tempStudentExamTest);
        Task<bool> Update_StudentExamTestAnswerAsync (StudentExamTest tempStudentExamTest);
        Task<bool> Delete_StudentExamTestsAsync (int studentExamId);
        Task<bool> Delete_StudentExamTestAsync (long studentExamTestId);
        Task<string> CalculateStats_StudentExamTestsAsync (int examId, int testId);
        #endregion
        #region C13:StudentCourses
        Task<bool> Create_StudentCoursesAsync (int groupId, int courseId);
        Task<bool> Create_StudentCourseAsync (int studentId, int courseId);
        Task<List<StudentCourse>> Read_StudentCoursesAsync (int studentid);
        Task<StudentCourse> Read_StudentCourseAsync (int studentCourseId);
        Task<bool> Delete_StudentCoursesAsync (int courseId, int groupId);
        Task<bool> Delete_StudentCourseAsync (int studentCourseId);
        Task<bool> Delete_StudentCourseByStudentCourseIdAsync (int studentCourseId);
        Task<bool> CalculatePoints_StudentCourseAsync (StudentCourse studentCourse);
        #endregion
        #region C14:StudentCourseTests
        Task<bool> Create_StudentCourseTestAsync (StudentCourseTest studentCourseTest);
        Task<List<StudentCourseTest>> Read_StudentCourseTestsAsync (StudentCourse studentCourse, bool readOptions);
        Task<StudentCourseTest> Read_StudentCourseTestRandomAsync (int studentCourseId, bool readOptions, bool retry);
        Task<bool> Update_StudentCourseTestAsync (StudentCourseTest studentCourseTest, string mode);
        Task<bool> Delete_StudentCourseTestsAsync (string mode, StudentCourse studentCourse);
        #endregion
        #region C16:Messages
        Task<int> Create_MessageAsync (int groupId, Message message);
        Task<List<Message>> Read_MessagesAsync (string mode, int Id);
        Task<List<Message>> Read_MessagesAsync (int userId, string mode, string key);
        Task<List<Message>> Read_MessagesAsync (Message message);
        Task<bool> Update_MessageAsync (Message message);
        Task<bool> Delete_MessagesByIdAsync (string mode, int Id);
        Task<bool> Delete_MessagesByDateTimeAsync (int userId, Message message);
        #endregion
        #region C20:Projects
        Task<int> Create_ProjectAsync (Project project);
        Task<List<Project>> Read_ProjectsAsync (int userId);
        Task<Project> Read_ProjectAsync (int projectId);
        #endregion
        #region C21:Subprojects
        Task<int> Create_SubprojectAsync (Subproject subProject);
        Task<List<Subproject>> Read_SubprojectsAsync (int projectId, bool readNotes);
        Task<Subproject> Read_SubprojectAsync (int subProjectId, bool readNotes);
        Task<bool> Delete_SubprojectAsync (int subProjectId, bool delNotes);
        #endregion
        #region C22:Notes
        Task<int> Create_NoteAsync (Note note);
        Task<List<Note>> Read_NotesAsync (int parentId);
        Task<Note> Read_NoteAsync (int noteId);
        Task<bool> Delete_NotesAsync (int parentId);
        Task<bool> Delete_NoteAsync (int noteId);
        #endregion
        }
    }
