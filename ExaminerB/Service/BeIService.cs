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
        #region U:usrs
        Task<int> Create_TeacherAsync (User user);
        Task<List<User>> Read_TeachersAsync ();
        Task<bool> Update_TeacherPasswordAsync (User user);
        Task<bool> Delete_TeacherAsync (int userId);
        #endregion
        #region S:Students
        Task<int> Create_StudentAsync (User student);
        Task<List<User>> Read_StudentsByKeywordAsync (int userId, string keyword, int readStudentGCEM);
        Task<List<User>> Read_StudentsByGCEMSIdAsync (int Id, string mode, int readStudentGCEM);
        Task<bool> Update_StudentAsync (User student);
        Task<bool> Update_StudentTagsAsync (User student);
        Task<bool> Remove_StudentFromListAsync (int studentId, string mode);
        Task<bool> Delete_StudentAsync (int studentId);
        #endregion
        #region G:Groups
        Task<int> Create_GroupAsync (Group group);
        Task<List<Group>> Read_GroupsAsync (User user, bool getGroupStudents);
        Task<Group> Read_GroupAsync (int groupId);
        Task<bool> Update_GroupAsync (Group group);
        Task<int> Delete_GroupAsync (int groupId);
        #endregion
        #region SG:StudentGroups
        Task<bool> Create_StudentGroupsAsync (int groupId, List<int> lstStudentIds);
        Task<List<StudentGroup>> Read_StudentGroupsAsync (int Id, string mode);
        #endregion
        #region C:Courses
        Task<int> Create_CourseAsync (Course course);
        Task<List<Course>> Read_CoursesAsync (int userId);
        Task<Course> Read_CourseAsync (int courseId, bool getStudentsList);
        Task<bool> Update_CourseAsync (Course course);
        Task<bool> Delete_CourseAsync (int courseId);
        #endregion
        #region CF:CourseFolders
        Task<int> Create_CourseFolderAsync (CourseFolder courseFolder);
        Task<List<CourseFolder>> Read_CourseFoldersAsync (int courseId);
        Task<bool> Update_CourseFolderAsync (CourseFolder courseFolder);
        Task<bool> Delete_CourseFolderAsync (int courseFolderId);
        #endregion
        #region CT:CourseTopics
        Task<int> Create_CourseTopicAsync (CourseTopic courseTopic);
        Task<List<CourseTopic>> Read_CourseTopicsAsync (int courseId);
        Task<bool> Update_CourseTopicAsync (CourseTopic courseTopic);
        Task<bool> Delete_CourseTopicAsync (int courseTopicId);
        #endregion
        #region SC:StudentCourses
        Task<bool> Create_StudentCoursesAsync (int courseId, List<int> lstStudentIds);
        Task<bool> Create_StudentCourseAsync (int studentId, int courseId);
        Task<List<StudentCourse>> Read_StudentCoursesAsync (int Id, string mode);
        Task<StudentCourse> Read_StudentCourseAsync (int studentCourseId);
        Task<bool> Update_StudentCourseAsync (StudentCourse studentCourse);
        Task<bool> Update_StudentCoursesTagsAsync (List<int> lstStudentIds, int courseId, bool activeStatus);
        Task<bool> Delete_StudentCoursesAsync (int Id, string mode);
        Task<bool> Delete_StudentCourseAsync (int studentCourseId);
        Task<bool> CalculatePoints_StudentCourseAsync (StudentCourse studentCourse);
        #endregion
        #region SCT:StudentCourseTests
        Task<bool> Create_StudentCourseTestAsync (StudentCourseTest studentCourseTest);
        Task<List<StudentCourseTest>> Read_StudentCourseTestsAsync (StudentCourse studentCourse, bool readOptions);
        Task<StudentCourseTest> Read_StudentCourseTestRandomAsync (int studentCourseId, bool readOptions, bool retry);
        Task<bool> Update_StudentCourseTestAsync (StudentCourseTest studentCourseTest, string mode);
        Task<bool> Delete_StudentCourseTestsAsync (string mode, StudentCourse studentCourse);
        #endregion
        #region T:Tests
        Task<int> Create_TestAsync (Test test);
        Task<Test> Read_TestByTestIdAsync (int testId, bool readOptions);
        Task<Test> Read_TestByStudentExamTestIdAsync (long studentExamTestId, bool readOptions);
        Task<List<Test>> Read_TestsByCourseIdAsync (int courseId, int pageNumber, bool readOptions);
        Task<List<Test>> Read_TestsByCourseTopicIdAsync (int courseTopicId, int pageNumber, bool readOptions);
        Task<List<Test>> Read_TestsBySearchAsync (string strSearch, int courseId, bool readOptions);
        Task<List<Test>> Read_TestsByExamIdAsync (int examId, bool readOptions);
        Task<List<Test>> Read_TestsByStudentExamIdAsync (int studentExamId, bool readOptions);
        Task<List<Test>> Read_TestsByStudentCourseIdAsync (int studentCourseId, bool readOptions);
        Task<bool> Update_TestAsync (Test test);
        Task<bool> Delete_TestAsync (int testId);
        Task<bool> ImportExcelTestsAsync (string filePath, int courseId);
        #endregion
        #region TO:TestOptions
        Task<int> Create_TestOptionAsync (TestOption testOption);
        Task<bool> Update_TestOptionAsync (TestOption testOption);
        Task<bool> Delete_TestOptionsAsync (int testId);
        Task<bool> Delete_TestOptionAsync (int testOptionId);
        #endregion
        #region E:Exams
        Task<int> Create_ExamAsync (Exam exam);
        Task<List<Exam>> Read_ExamsAsync (int courseId);
        Task<Exam> Read_ExamAsync (int examId, bool getStudentsList);
        Task<bool> Update_ExamAsync (Exam exam);
        Task<bool> Delete_ExamsAsync (int courseId);
        Task<bool> Delete_ExamAsync (int examId);
        #endregion
        #region EC:ExamCompositions
        Task<int> Create_ExamCompositionAsync (ExamComposition examComposition);
        Task<List<ExamComposition>> Read_ExamCompositionsAsync (int examId);
        Task<ExamComposition> Read_ExamCompositionAsync (int examCompositionId);
        Task<bool> Update_ExamCompositionAsync (ExamComposition examComposition);
        Task<bool> Delete_ExamCompositionsAsync (int examId);
        Task<bool> Delete_ExamCompositionAsync (int examCompositionId);
        #endregion
        #region ET:ExamTests
        Task<int> Create_ExamTestsByExamCompositionAsync (ExamComposition examComposition);
        Task<int> Create_ExamTestAsync (ExamTest examTest);
        Task<List<ExamTest>> Read_ExamTestsAsync (int examId);
        Task<ExamTest> Read_ExamTestAsync (int examTestId);
        Task<bool> Update_ExamTestAsync (ExamTest examTest);
        Task<bool> Delete_ExamTestsAsync (int examId);
        Task<bool> Delete_ExamTestAsync (ExamTest examTest);
        #endregion
        #region SE:StudentExams
        Task<bool> Create_StudentExamsAsync (int examId, List<int> lstStudentIds);
        Task<List<StudentExam>> Read_StudentExamsAsync (int Id, string mode);
        Task<StudentExam> Read_StudentExamAsync (int studentExamId, bool readInactiveExams);
        Task<bool> Update_StudentExamAsync (StudentExam studentExam);
        Task<bool> Update_StudentsExamTagsAsync (string mode, int examId);
        Task<bool> Update_StudentExamTagsAsync (StudentExam tempStudentExam);
        Task<bool> Delete_StudentExamsByStudentIdAsync (int studentId);
        Task<bool> Delete_StudentExamsByExamIdAsync (int examId);
        Task<bool> Delete_StudentExamAsync (int studentExamId);
        Task<bool> CalculatePoints_StudentExamsAsync (int studentexamid);
        #endregion
        #region SET:StudentExamTests
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
        #region M:Messages
        Task<int> Create_MessageAsync (Message message);
        Task<Message> Read_MessageAsync (int messageId, bool getStudentMessages);
        Task<List<Message>> Read_MessagesAsync (int userId, bool getStudentMessages);
        Task<bool> Update_MessageAsync (Message message);
        Task<bool> Delete_MessagesAsync (string mode, int recipientId);
        #endregion
        #region SM:StudentMessages
        Task<bool> Create_StudentMessagesAsync (int messageId, List<int> lstStudentIds, bool requestFeedback);
        Task<List<StudentMessage>> Read_StudentMessagesAsync (int Id, string mode);
        Task<Message> Read_StudentMessageAsync (int studentMessageId);
        Task<bool> Update_StudentMessageTagsAsync (StudentMessage studentMessage);
        Task<bool> Update_StudentMessageSetAsReadAsync (StudentMessage studentMessage);
        Task<bool> Delete_StudentMessageAsync (int studentMessageId);
        #endregion
        #region CH:Chats
        Task<int> Create_ChatAsync (Chat chat);
        Task<List<Chat>> Read_ChatsAsync (int studentId);
        Task<List<Chat>> Read_ChatsWithOneMateAsync (int studentId, int mateId);
        Task<bool> Update_ChatAsync (Chat chat);
        Task<bool> Update_ChatTagsAsync (Chat chat);
        Task<bool> Delete_ChatAsync (int chatId);
        #endregion
        #region P:Projects
        Task<int> Create_ProjectAsync (Project project);
        Task<List<Project>> Read_ProjectsAsync (int userI, string mode);
        Task<Project> Read_ProjectAsync (int projectId);
        Task<bool> Update_ProjectAsync (Project project);
        #endregion
        #region SP:Subprojects
        Task<int> Create_SubprojectAsync (Subproject subProject);
        Task<List<Subproject>> Read_SubprojectsAsync (int projectId, bool readNotes);
        Task<Subproject> Read_SubprojectAsync (int subProjectId, bool readNotes);
        Task<bool> Update_SubprojectAsync (Subproject subProject);
        Task<bool> Delete_SubprojectAsync (int subProjectId);
        #endregion
        #region N:Notes
        Task<int> Create_NoteAsync (Note note);
        Task<List<Note>> Read_NotesAsync (int parentId, int parentType);
        Task<List<Note>> Read_NotesBySearchKeyAsync (string searchKey, int parentId, string mode);
        Task<Note> Read_NoteAsync (int parentId);
        Task<bool> Update_NoteAsync (Note note);
        Task<bool> Delete_NotesAsync (int parentId, int parentType);
        Task<bool> Delete_NoteAsync (int noteId);
        #endregion
        }
    }
