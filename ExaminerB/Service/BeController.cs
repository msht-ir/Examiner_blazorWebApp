using ExaminerB.Services2Backend;
using ExaminerS.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Group = ExaminerS.Models.Group;

namespace ExaminerB.Backend
    {
    [ApiController]
    [Route ("api")]
    public class BeController : Controller
        {
        #region Injection and Constructor
        private readonly IConfiguration _config;
        private readonly BeIService _BeService;
        public BeController (IConfiguration config, BeIService BeService)
            {
            _config = config;
            _BeService = BeService;
            }
        #endregion
        #region LOGIN
        [HttpPost ("LoginTeacher")]
        public async Task<ActionResult<User>> LoginTeacher ([FromBody] User user)
            {
            var result = await _BeService.LoginTeacherAsync (user);
            return result != null ? Ok (result) : StatusCode (500, "not found");
            }
        [HttpPost ("LoginStudent")]
        public async Task<ActionResult<User>> LoginStudent ([FromBody] User user)
            {
            var result = await _BeService.LoginStudentAsync (user);
            return result;
            }
        #endregion
        #region U:usrs
        [HttpPost ("Create_Teacher")]
        public async Task<ActionResult<bool>> Create_Teacher ([FromBody] User user)
            {
            var result = await _BeService.Create_TeacherAsync (user);
            return Ok (result);
            }
        [HttpPost ("Read_Teachers")]
        public async Task<ActionResult<List<User>>> Read_Teachers ()
            {
            var result = await _BeService.Read_TeachersAsync ();
            return Ok (result);
            }
        [HttpPost ("Update_TeacherPassword")]
        public async Task<ActionResult<bool>> Update_TeacherPassword ([FromBody] User user)
            {
            var result = await _BeService.Update_TeacherPasswordAsync (user);
            return Ok (result);
            }
        [HttpPost ("Delete_Teacher")]
        public async Task<ActionResult<bool>> Delete_Teacher ([FromBody] int userId)
            {
            var result = await _BeService.Delete_TeacherAsync (userId);
            return Ok (result);
            }
        #endregion
        #region S:Students
        [HttpPost ("Create_Student")]
        public async Task<ActionResult<int>> Create_Student ([FromBody] User student)
            {
            var result = await _BeService.Create_StudentAsync (student);
            return Ok (result);
            }
        [HttpPost ("Read_StudentsByKeyword")]
        public async Task<ActionResult<List<User>>> Read_StudentsByKeyword ([FromBody] int userId, [FromQuery] string keyword, [FromQuery] int getGCEM)
            {
            var result = await _BeService.Read_StudentsByKeywordAsync (userId, keyword, getGCEM);
            return Ok (result);
            }
        [HttpPost ("Read_StudentsByGCEMSId")]
        public async Task<ActionResult<List<User>>> Read_StudentsByGCEMSId ([FromBody] int Id, [FromQuery] string mode, [FromQuery] int readStudentGCEM)
            {
            var result = await _BeService.Read_StudentsByGCEMSIdAsync (Id, mode, readStudentGCEM);
            return Ok (result);
            }
        [HttpPost ("Update_Student")]
        public async Task<ActionResult<bool>> Update_Student ([FromBody] User student)
            {
            var result = await _BeService.Update_StudentAsync (student);
            return Ok (result);
            }
        [HttpPost ("Remove_StudentFromList")]
        public async Task<ActionResult<bool>> Remove_StudentFromList ([FromBody] int studentId, [FromQuery] string mode)
            {
            var result = await _BeService.Remove_StudentFromListAsync (studentId, mode);
            return Ok (result);
            }
        [HttpPost ("Delete_Student")]
        public async Task<ActionResult<bool>> Delete_Student ([FromBody] int studentId)
            {
            var result = await _BeService.Delete_StudentAsync (studentId);
            return Ok (result);
            }
        #endregion
        #region G:Groups
        [HttpPost ("Create_Group")]
        public async Task<ActionResult<int>> Create_Group ([FromBody] Group group)
            {
            var result = await _BeService.Create_GroupAsync (group);
            return Ok (result);
            }
        [HttpPost ("Read_Groups")]
        public async Task<ActionResult<List<Group>>> Read_Groups ([FromBody] User user, [FromQuery] bool getGroupStudents)
            {
            var result = await _BeService.Read_GroupsAsync (user, getGroupStudents);
            return Ok (result);
            }
        [HttpPost ("Read_Group")]
        public async Task<ActionResult<Group>> Read_Group ([FromBody] int groupId)
            {
            var result = await _BeService.Read_GroupAsync (groupId);
            return Ok (result);
            }
        [HttpPost ("Update_Group")]
        public async Task<ActionResult<bool>> Update_Group ([FromBody] Group group)
            {
            var result = await _BeService.Update_GroupAsync (group);
            return Ok (result);
            }
        [HttpPost ("Delete_Group")]
        public async Task<ActionResult<int>> Delete_Group ([FromBody] int groupId)
            {
            var result = await _BeService.Delete_GroupAsync (groupId);
            return Ok (result);
            }
        #endregion
        #region SG:StudentGroups
        #endregion
        #region C:Courses
        [HttpPost ("Create_Course")]
        public async Task<ActionResult<int>> Create_Course ([FromBody] Course course)
            {
            var result = await _BeService.Create_CourseAsync (course);
            return Ok (result);
            }
        [HttpPost ("Read_Courses")]
        public async Task<ActionResult<List<Course>>> Read_Courses ([FromBody] int userId)
            {
            var result = await _BeService.Read_CoursesAsync (userId);
            return Ok (result);
            }
        [HttpPost ("Read_Course")]
        public async Task<ActionResult<Course>> Read_Course ([FromBody] int courseId, [FromQuery] bool getStudentsList)
            {
            var result = await _BeService.Read_CourseAsync (courseId, getStudentsList);
            return Ok (result);
            }
        [HttpPost ("Update_Course")]
        public async Task<ActionResult<bool>> Update_Course ([FromBody] Course course)
            {
            var result = await _BeService.Update_CourseAsync (course);
            return Ok (result);
            }
        [HttpPost ("Delete_Course")]
        public async Task<ActionResult<bool>> Delete_Course ([FromBody] int courseId)
            {
            var result = await _BeService.Delete_CourseAsync (courseId);
            return Ok (result);
            }
        #endregion
        #region CF:CourseFolders
        [HttpPost ("Create_CourseFolder")]
        public async Task<ActionResult<int>> Create_CourseFolder ([FromBody] CourseFolder courseFolder)
            {
            var result = await _BeService.Create_CourseFolderAsync (courseFolder);
            return Ok (result);
            }
        [HttpPost ("Read_CourseFolders")]
        public async Task<ActionResult<List<CourseFolder>>> Read_CourseFolders ([FromBody] int courseId)
            {
            var result = await _BeService.Read_CourseFoldersAsync (courseId);
            return Ok (result);
            }
        [HttpPost ("Update_CourseFolder")]
        public async Task<ActionResult<bool>> Update_CourseFolder ([FromBody] CourseFolder courseFolder)
            {
            var result = await _BeService.Update_CourseFolderAsync (courseFolder);
            return Ok (result);
            }
        [HttpPost ("Delete_CourseFolder")]
        public async Task<ActionResult<bool>> Delete_CourseFolder ([FromBody] int courseFolderId)
            {
            var result = await _BeService.Delete_CourseFolderAsync (courseFolderId);
            return result ? Ok (result) : NotFound (result);
            }
        #endregion
        #region CT:CourseTopics
        [HttpPost ("Create_CourseTopic")]
        public async Task<ActionResult<int>> Create_CourseTopic ([FromBody] CourseTopic courseTopic)
            {
            var result = await _BeService.Create_CourseTopicAsync (courseTopic);
            return Ok (result);
            }
        [HttpPost ("Read_CourseTopics")]
        public async Task<ActionResult<List<CourseTopic>>> Read_CourseTopics ([FromBody] int courseId)
            {
            var result = await _BeService.Read_CourseTopicsAsync (courseId);
            return Ok (result);
            }
        [HttpPost ("Update_CourseTopic")]
        public async Task<ActionResult<bool>> Update_CourseTopic ([FromBody] CourseTopic courseTopic)
            {
            var result = await _BeService.Update_CourseTopicAsync (courseTopic);
            return Ok (result);
            }
        [HttpPost ("Delete_CourseTopic")]
        public async Task<ActionResult<bool>> Delete_CourseTopic ([FromBody] int courseTopicId)
            {
            var result = await _BeService.Delete_CourseTopicAsync (courseTopicId);
            return result ? Ok (result) : NotFound (result);
            }
        #endregion
        #region SC:StudentCourses
        [HttpPost ("Create_StudentCourses")]
        public async Task<ActionResult<bool>> Create_StudentCourses ([FromBody] int groupId, [FromQuery] int courseId)
            {
            var result = await _BeService.Create_StudentCoursesAsync (groupId, courseId);
            return Ok (result);
            }
        [HttpPost ("Create_StudentCourse")]
        public async Task<ActionResult<bool>> Create_StudentCourse ([FromBody] int studentId, [FromQuery] int courseId)
            {
            var result = await _BeService.Create_StudentCourseAsync (studentId, courseId);
            return Ok (result);
            }
        [HttpPost ("Read_StudentCourses")]
        public async Task<ActionResult<List<StudentCourse>>> Read_StudentCourses ([FromBody] int Id, [FromQuery] string mode)
            {
            var result = await _BeService.Read_StudentCoursesAsync (Id, mode);
            return Ok (result);
            }
        [HttpPost ("Read_StudentCourse")]
        public async Task<ActionResult<List<StudentCourse>>> Read_StudentCourse ([FromBody] int studentCourseId)
            {
            var result = await _BeService.Read_StudentCourseAsync (studentCourseId);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentCourses")]
        public async Task<ActionResult<bool>> Delete_StudentCourses ([FromQuery] int Id, [FromBody] string mode)
            {
            var result = await _BeService.Delete_StudentCoursesAsync (Id, mode);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentCourse")]
        public async Task<ActionResult<bool>> Delete_StudentCourse ([FromBody] int studentCourseId)
            {
            var result = await _BeService.Delete_StudentCourseAsync (studentCourseId);
            return Ok (result);
            }
        [HttpPost ("CalculatePoints_StudentCourse")]
        public async Task<ActionResult<bool>> CalculatePoints_StudentCourse ([FromBody] StudentCourse studentCourse)
            {
            var result = await _BeService.CalculatePoints_StudentCourseAsync (studentCourse);
            return Ok (result);
            }
        #endregion
        #region SCT:StudentExamTests
        [HttpPost ("Create_StudentCourseTest")]
        public async Task<ActionResult<bool>> Create_StudentCourseTest ([FromBody] StudentCourseTest studentCourseTest)
            {
            var result = await _BeService.Create_StudentCourseTestAsync (studentCourseTest);
            return Ok (result);
            }
        [HttpPost ("Read_StudentCourseTests")]
        public async Task<ActionResult<List<StudentCourseTest>>> Read_StudentCourseTests ([FromBody] StudentCourse studentCourse, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_StudentCourseTestsAsync (studentCourse, readOptions);

            return Ok (result);
            }
        [HttpPost ("Read_StudentCourseTestRandom")]
        public async Task<ActionResult<StudentCourseTest>> Read_StudentCourseTestRandom ([FromBody] int studentCourseId, [FromQuery] bool readOptions, [FromQuery] bool retry)
            {
            var result = await _BeService.Read_StudentCourseTestRandomAsync (studentCourseId, readOptions, retry);
            return Ok (result);
            }
        [HttpPost ("Update_StudentCourseTest")]
        public async Task<ActionResult<bool>> Update_StudentCourseTest ([FromBody] StudentCourseTest studentCourseTest, [FromQuery] string mode)
            {
            var result = await _BeService.Update_StudentCourseTestAsync (studentCourseTest, mode);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentCourseTests")]
        public async Task<ActionResult<bool>> Delete_StudentCourseTests ([FromQuery] string mode, [FromBody] StudentCourse studentCourse)
            {
            var result = await _BeService.Delete_StudentCourseTestsAsync (mode, studentCourse);
            return Ok (result);
            }
        #endregion
        #region T:Tests
        [HttpPost ("Create_Test")]
        public async Task<ActionResult<int>> Create_Test ([FromBody] Test test)
            {
            var result = await _BeService.Create_TestAsync (test);
            return Ok (result);
            }
        [HttpPost ("Read_TestByTestId")]
        public async Task<ActionResult<Test>> Read_TestByTestId ([FromBody] int testId, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestByTestIdAsync (testId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestByStudentExamTestId")]
        public async Task<ActionResult<Test>> Read_TestByStudentExamTestId ([FromBody] long studentExamTestId, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestByStudentExamTestIdAsync (studentExamTestId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestsByCourseId")]
        public async Task<ActionResult<List<Test>>> Read_TestsByCourseId ([FromBody] int courseId, [FromQuery] int pageNumber, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestsByCourseIdAsync (courseId, pageNumber, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestsByCourseTopicId")]
        public async Task<ActionResult<List<Test>>> Read_TestsByCourseTopicId ([FromBody] int courseTopicId, [FromQuery] int pageNumber, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestsByCourseTopicIdAsync (courseTopicId, pageNumber, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestsBySearch")]
        public async Task<ActionResult<List<Test>>> Read_TestsBySearch ([FromQuery] int courseId, [FromQuery] bool readOptions, [FromBody] string strSearch)
            {
            var result = await _BeService.Read_TestsBySearchAsync (strSearch, courseId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestsByExamId")]
        public async Task<ActionResult<List<Test>>> Read_TestsByExamId ([FromBody] int examId, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestsByExamIdAsync (examId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestsByStudentExamId")]
        public async Task<ActionResult<List<Test>>> Read_TestsByStudentExamId ([FromBody] int studentExamId, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestsByStudentExamIdAsync (studentExamId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_TestsByStudentCourseId")]
        public async Task<ActionResult<List<Test>>> Read_TestsByStudentCourseId ([FromBody] StudentCourse studentCourse, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_TestsByStudentCourseIdAsync (studentCourse.StudentCourseId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Update_Test")]
        public async Task<ActionResult<bool>> Update_Test ([FromBody] Test test)
            {
            var result = await _BeService.Update_TestAsync (test);
            return Ok (true);
            }
        [HttpPost ("Delete_Test")]
        public async Task<ActionResult<bool>> Delete_Test ([FromBody] int testId)
            {
            var result = await _BeService.Delete_TestAsync (testId);
            return Ok (result);
            }
        [HttpPost ("ImportExcelTests")]
        public async Task<ActionResult<bool>> ImportExcelTests ([FromBody] string filePath, [FromQuery] int courseId)
            {
            var result = await _BeService.ImportExcelTestsAsync (filePath, courseId);
            return Ok (result);
            }
        #endregion
        #region TO:TestOptions
        [HttpPost ("Create_TestOption")]
        public async Task<ActionResult<int>> Create_TestOption ([FromBody] TestOption testOption)
            {
            var result = await _BeService.Create_TestOptionAsync (testOption);
            return Ok (result);
            }
        [HttpPost ("Update_TestOption")]
        public async Task<ActionResult<bool>> Update_TestOption ([FromBody] TestOption testOption)
            {
            var result = await _BeService.Update_TestOptionAsync (testOption);
            return Ok (result);
            }
        [HttpPost ("Delete_TestOptions")]
        public async Task<ActionResult<bool>> Delete_TestOptions ([FromBody] int testId)
            {
            var result = await _BeService.Delete_TestOptionsAsync (testId);
            return Ok (result);
            }
        [HttpPost ("Delete_TestOption")]
        public async Task<ActionResult<bool>> Delete_TestOption ([FromBody] int testOptionId)
            {
            var result = await _BeService.Delete_TestOptionAsync (testOptionId);
            return Ok (result);
            }
        #endregion
        #region E:Exams
        [HttpPost ("Create_Exam")]
        public async Task<ActionResult<int>> Create_Exam ([FromBody] Exam exam)
            {
            var result = await _BeService.Create_ExamAsync (exam);
            return Ok (result);
            }
        [HttpPost ("Read_Exams")]
        public async Task<ActionResult<List<Exam>>> Read_Exams ([FromBody] int courseId)
            {
            var result = await _BeService.Read_ExamsAsync (courseId);
            return Ok (result);
            }
        [HttpPost ("Read_Exam")]
        public async Task<ActionResult<Exam>> Read_Exam ([FromBody] int examId, [FromQuery] bool getStudentsList)
            {
            var result = await _BeService.Read_ExamAsync (examId, getStudentsList);
            return Ok (result);
            }
        [HttpPost ("Update_Exam")]
        public async Task<ActionResult<List<Exam>>> Update_Exam ([FromBody] Exam exam)
            {
            var result = await _BeService.Update_ExamAsync (exam);
            return Ok (result);
            }
        [HttpPost ("Delete_Exams")]
        public async Task<ActionResult<bool>> Delete_Exams ([FromBody] int courseId)
            {
            var result = await _BeService.Delete_ExamsAsync (courseId);
            return Ok (result);
            }
        [HttpPost ("Delete_Exam")]
        public async Task<ActionResult<bool>> Delete_Exam ([FromBody] int examId)
            {
            var result = await _BeService.Delete_ExamAsync (examId);
            return Ok (result);
            }
        #endregion
        #region EC:ExamCompositions
        [HttpPost ("Create_ExamComposition")]
        public async Task<ActionResult<int>> Create_ExamComposition ([FromBody] ExamComposition examComposition)
            {
            var result = await _BeService.Create_ExamCompositionAsync (examComposition);
            return Ok (result);
            }
        [HttpPost ("Read_ExamCompositions")]
        public async Task<ActionResult<List<ExamComposition>>> Read_ExamCompositions ([FromBody] int examId)
            {
            var result = await _BeService.Read_ExamCompositionsAsync (examId);
            return Ok (result);
            }
        [HttpPost ("Read_ExamComposition")]
        public async Task<ActionResult<ExamComposition>> Read_ExamComposition ([FromBody] int examCompositionId)
            {
            var result = await _BeService.Read_ExamCompositionAsync (examCompositionId);
            return Ok (result);
            }
        [HttpPost ("Update_ExamComposition")]
        public async Task<ActionResult<bool>> Update_ExamComposition ([FromBody] ExamComposition examComposition)
            {
            var result = await _BeService.Update_ExamCompositionAsync (examComposition);
            return Ok (result);
            }
        [HttpPost ("Delete_ExamCompositions")]
        public async Task<ActionResult<bool>> Delete_ExamCompositions ([FromBody] int examId)
            {
            var result = await _BeService.Delete_ExamCompositionsAsync (examId);
            return Ok (result);
            }
        [HttpPost ("Delete_ExamComposition")]
        public async Task<ActionResult<bool>> Delete_ExamComposition ([FromBody] int examCompositionId)
            {
            var result = await _BeService.Delete_ExamCompositionAsync (examCompositionId);
            return Ok (result);
            }
        #endregion
        #region ET:ExamTests
        [HttpPost ("Create_ExamTestsByExamComposition")]
        public async Task<ActionResult<int>> Create_ExamTestsByExamComposition ([FromBody] ExamComposition examComposition)
            {
            var result = await _BeService.Create_ExamTestsByExamCompositionAsync (examComposition);
            return Ok (result);
            }
        [HttpPost ("Create_ExamTest")]
        public async Task<ActionResult<int>> Create_ExamTest ([FromBody] ExamTest examTest)
            {
            var result = await _BeService.Create_ExamTestAsync (examTest);
            return Ok (result);
            }
        [HttpPost ("Read_ExamTests")]
        public async Task<ActionResult<List<ExamTest>>> Read_ExamTests ([FromBody] int examId)
            {
            var result = await _BeService.Read_ExamTestsAsync (examId);
            return Ok (result);
            }
        [HttpPost ("Read_ExamTest")]
        public async Task<ActionResult<ExamTest>> Read_ExamTest ([FromBody] int examTestId)
            {
            var result = await _BeService.Read_ExamTestAsync (examTestId);
            return Ok (result);
            }
        [HttpPost ("Update_ExamTest")]
        public async Task<ActionResult<bool>> Update_ExamTest ([FromBody] ExamTest examTest)
            {
            var result = await _BeService.Update_ExamTestAsync (examTest);
            return Ok (result);
            }
        [HttpPost ("Delete_ExamTests")]
        public async Task<ActionResult<bool>> Delete_ExamTests ([FromBody] int examId)
            {
            var result = await _BeService.Delete_ExamTestsAsync (examId);
            return Ok (result);
            }
        [HttpPost ("Delete_ExamTest")]
        public async Task<ActionResult<bool>> Delete_ExamTest ([FromBody] ExamTest examTest)
            {
            var result = await _BeService.Delete_ExamTestAsync (examTest);
            return Ok (result);
            }
        #endregion
        #region SE:StudentExams
        [HttpPost ("Create_StudentExams")]
        public async Task<ActionResult<int>> Create_StudentExams ([FromBody] int Id, [FromQuery] string mode, [FromQuery] int examId)
            {
            var result = await _BeService.Create_StudentExamsAsync (Id, mode, examId);
            return Ok (result);
            }
        [HttpPost ("Create_StudentExam")]
        public async Task<ActionResult<int>> Create_StudentExam ([FromBody] StudentExam studentExam)
            {
            var result = await _BeService.Create_StudentExamAsync (studentExam);
            return Ok (result);
            }
        [HttpPost ("Read_StudentExams")]
        public async Task<ActionResult<List<StudentExam>>> Read_StudentExams ([FromBody] int Id, [FromQuery] string mode)
            {
            var result = await _BeService.Read_StudentExamsAsync (Id, mode);
            return Ok (result);
            }   
        [HttpPost ("Read_StudentExam")]
        public async Task<ActionResult<StudentExam>> Read_StudentExam ([FromBody] int studentExamId, [FromQuery] bool readInactiveExams)
            {
            var result = await _BeService.Read_StudentExamAsync (studentExamId, readInactiveExams);
            return Ok (result);
            }
        [HttpPost ("Update_StudentExam")]
        public async Task<ActionResult<bool>> Update_StudentExam ([FromBody] StudentExam studentExam)
            {
            var result = await _BeService.Update_StudentExamAsync (studentExam);
            return Ok (result);
            }
        [HttpPost ("Update_StudentsExamTags")]
        public async Task<ActionResult<bool>> Update_StudentsExamTags ([FromBody] int examId, [FromQuery] string mode)
            {
            var result = await _BeService.Update_StudentsExamTagsAsync (mode, examId);
            return Ok (result);
            }
        [HttpPost ("Update_StudentExamTags")]
        public async Task<ActionResult<bool>> Update_StudentExamTags ([FromBody] StudentExam tempstudentExam)
            {
            var result = await _BeService.Update_StudentExamTagsAsync (tempstudentExam);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentExamsByStudentId")]
        public async Task<ActionResult<bool>> Delete_StudentExamsByStudentId ([FromBody] int studentId)
            {
            var result = await _BeService.Delete_StudentExamsByStudentIdAsync (studentId);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentExamsByExamId")]
        public async Task<ActionResult<bool>> Delete_StudentExamsByExamId ([FromBody] int examId)
            {
            var result = await _BeService.Delete_StudentExamsByExamIdAsync (examId);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentExam")]
        public async Task<ActionResult<bool>> Delete_StudentExam ([FromBody] int studentExamId)
            {
            var result = await _BeService.Delete_StudentExamAsync (studentExamId);
            return Ok (result);
            }
        #endregion
        #region SET:StudentExamTests
        [HttpPost ("Create_StudentExamTest")]
        public async Task<ActionResult<int>> Create_StudentExamTest ([FromBody] StudentExamTest studentExamTest)
            {
            var result = await _BeService.Create_StudentExamTestAsync (studentExamTest);
            return Ok (result);
            }
        [HttpPost ("Read_StudentExamTest")]
        public async Task<ActionResult<StudentExamTest>> Read_StudentExamTest ([FromBody] long studentExamTestId, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_StudentExamTestAsync (studentExamTestId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_StudentExamTests")]
        public async Task<ActionResult<List<StudentExamTest>>> Read_StudentExamTests ([FromBody] int studentExamId, [FromQuery] bool readOptions)
            {
            var result = await _BeService.Read_StudentExamTestsAsync (studentExamId, readOptions);
            return Ok (result);
            }
        [HttpPost ("Read_StudentsExamTest")]
        public async Task<ActionResult<List<StudentExamTest>>> Read_StudentsExamTest ([FromBody] StudentExamTest studentExamTest)
            {
            var result = await _BeService.Read_StudentsExamTestAsync (studentExamTest);
            return Ok (result);
            }
        [HttpPost ("Update_StudentExamTest")]
        public async Task<ActionResult<bool>> Update_StudentExamTest ([FromBody] StudentExamTest studentExamTest)
            {
            var result = await _BeService.Update_StudentExamTestAsync (studentExamTest);
            return Ok (result);
            }
        [HttpPost ("Update_StudentExamTestsTags")]
        public async Task<ActionResult<bool>> Update_StudentExamTestsTags ([FromBody] StudentExamTest tempStudentExamTest)
            {
            var result = await _BeService.Update_StudentExamTestsTagsAsync (tempStudentExamTest);
            return Ok (result);
            }
        [HttpPost ("Update_StudentExamTestTags")]
        public async Task<ActionResult<bool>> Update_StudentExamTestTags ([FromBody] StudentExamTest tempStudentExamTest)
            {
            var result = await _BeService.Update_StudentExamTestTagsAsync (tempStudentExamTest);
            return Ok (result);
            }
        [HttpPost ("Update_StudentExamTestAnswer")]
        public async Task<ActionResult<bool>> Update_StudentExamTestAnswer ([FromBody] StudentExamTest tempStudentExamTest)
            {
            var result = await _BeService.Update_StudentExamTestAnswerAsync (tempStudentExamTest);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentExamTests")]
        public async Task<ActionResult<bool>> Delete_StudentExamTests ([FromBody] int studentExamId)
            {
            var result = await _BeService.Delete_StudentExamTestsAsync (studentExamId);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentExamTest")]
        public async Task<ActionResult<bool>> Delete_StudentExamTest ([FromBody] long studentExamTestId)
            {
            var result = await _BeService.Delete_StudentExamTestAsync (studentExamTestId);
            return Ok (result);
            }
        [HttpPost ("CalculatePoints_StudentExams")]
        public async Task<ActionResult<bool>> CalculatePoints_StudentExams (int studentexamid)
            {
            var result = await _BeService.CalculatePoints_StudentExamsAsync (studentexamid);
            return Ok (result);
            }
        #endregion
        #region M:Messages
        [HttpPost ("Create_Message")]
        public async Task<ActionResult<int>> Create_Message ([FromBody] Message message)
            {
            var result = await _BeService.Create_MessageAsync (message);
            return Ok (result);
            }
        [HttpPost ("Read_Message")]
        public async Task<ActionResult<Message>> Read_Message ([FromBody] int messageId, [FromQuery] bool getStudentMessages)
            {
            var result = await _BeService.Read_MessageAsync (messageId, getStudentMessages);
            return Ok (result);
            }
        [HttpPost ("Read_Messages")]
        public async Task<ActionResult<List<Message>>> Read_Messages ([FromBody] int userId, [FromQuery] bool getStudentMessages)
            {
            var result = await _BeService.Read_MessagesAsync (userId, getStudentMessages);
            return Ok (result);
            }
        [HttpPost ("Update_Message")]
        public async Task<ActionResult<bool>> Update_Message ([FromBody] Message message)
            {
            var result = await _BeService.Update_MessageAsync (message);
            return Ok (result);
            }
        [HttpPost ("Delete_Messages")]
        public async Task<ActionResult<bool>> Delete_MessagesById ([FromQuery] string mode, [FromBody] int recipientId)
            {
            var result = await _BeService.Delete_MessagesAsync (mode, recipientId);
            return result ? Ok (result) : NotFound (result);
            }
        #endregion
        #region SM:StudentMessages
        [HttpPost ("Create_StudentMessage")]
        public async Task<ActionResult<int>> Create_StudentMessage ([FromBody] Message message, [FromQuery] string mode, [FromQuery] int recipientId, [FromQuery] bool typeFeedback)
            {
            var result = await _BeService.Create_StudentMessageAsync (message, mode, recipientId, typeFeedback);
            return Ok (result);
            }
        [HttpPost ("Read_StudentMessages")]
        public async Task<ActionResult<List<StudentMessage>>> Read_StudentMessages ([FromBody] int Id, [FromQuery] string mode)
            {
            var result = await _BeService.Read_StudentMessagesAsync (Id, mode);
            return Ok (result);
            }
        [HttpPost ("Read_StudentMessage")]
        public async Task<ActionResult<Message>> Read_StudentMessage ([FromBody] int studentMessageId)
            {
            var result = await _BeService.Read_StudentMessageAsync (studentMessageId);
            return Ok (result);
            }
        [HttpPost ("Update_StudentMessageTags")]
        public async Task<ActionResult<bool>> Update_StudentMessageTags ([FromBody] StudentMessage studentMessage)
            {
            var result = await _BeService.Update_StudentMessageTagsAsync (studentMessage);
            return Ok (result);
            }
        [HttpPost ("Update_StudentMessageSetAsRead")]
        public async Task<ActionResult<bool>> Update_StudentMessageSetAsRead ([FromBody] StudentMessage studentMessage)
            {
            var result = await _BeService.Update_StudentMessageSetAsReadAsync (studentMessage);
            return Ok (result);
            }
        [HttpPost ("Delete_StudentMessage")]
        public async Task<ActionResult<bool>> Delete_StudentMessage ([FromBody] int studentMessageId)
            {
            var result = await _BeService.Delete_StudentMessageAsync (studentMessageId);
            return Ok (result);
            }
        #endregion
        #region P:Projects
        [HttpPost ("Create_Project")]
        public async Task<ActionResult<int>> Create_Project ([FromBody] Project project)
            {
            var result = await _BeService.Create_ProjectAsync (project);
            return Ok (result);
            }
        [HttpPost ("Read_Projects")]
        public async Task<ActionResult<List<Project>>> Read_Projects ([FromBody] int userId)
            {
            var result = await _BeService.Read_ProjectsAsync (userId);
            return Ok (result);
            }
        [HttpPost ("Read_Project")]
        public async Task<ActionResult<Project>> Read_Project ([FromBody] int projectId)
            {
            var result = await _BeService.Read_ProjectAsync (projectId);
            return Ok (result);
            }
        #endregion
        #region SP:Subprojects
        [HttpPost ("Create_Subproject")]
        public async Task<ActionResult<int>> Create_Subproject ([FromBody] Subproject subProject)
            {
            var result = await _BeService.Create_SubprojectAsync (subProject);
            return Ok (result);
            }
        [HttpPost ("Read_Subprojects")]
        public async Task<ActionResult<List<Subproject>>> Read_Subprojects ([FromBody] int projectId, [FromQuery] bool readNotes)
            {
            var result = await _BeService.Read_SubprojectsAsync (projectId, readNotes);
            return Ok (result);
            }
        [HttpPost ("Read_Subproject")]
        public async Task<ActionResult<Subproject>> Read_Subproject ([FromBody] int subProjectId, [FromQuery] bool readNotes)
            {
            var result = await _BeService.Read_SubprojectsAsync (subProjectId, readNotes);
            return Ok (result);
            }
        [HttpPost ("Delete_Subproject")]
        public async Task<ActionResult<bool>> Delete_Subproject ([FromBody] int subProjectId, [FromQuery] bool delNotes)
            {
            var result = await _BeService.Read_SubprojectsAsync (subProjectId, delNotes);
            return Ok (result);
            }
        #endregion
        #region N:Notes
        [HttpPost ("Create_Note")]
        public async Task<ActionResult<int>> Create_Note ([FromBody] Note note)
            {
            var result = await _BeService.Create_NoteAsync (note);
            return Ok (result);
            }
        [HttpPost ("Read_Notes")]
        public async Task<ActionResult<List<Note>>> Read_Notes ([FromBody] int parentId, int parentType)
            {
            var result = await _BeService.Read_NotesAsync (parentId, parentType);
            return Ok (result);
            }
        [HttpPost ("Read_NotesBySearchKey")]
        public async Task<ActionResult<List<Note>>> Read_NotesBySearchKey ([FromBody] string searchKey)
            {
            var result = await _BeService.Read_NotesBySearchKeyAsync (searchKey);
            return Ok (result);
            }
        [HttpPost ("Read_Note")]
        public async Task<ActionResult<Note>> Read_Note ([FromBody] int noteId)
            {
            var result = await _BeService.Read_NoteAsync (noteId);
            return Ok (result);
            }
        [HttpPost ("Delete_Notes")]
        public async Task<ActionResult<bool>> Delete_Notes ([FromBody] int parentId, [FromQuery] int parentType)
            {
            var result = await _BeService.Delete_NotesAsync (parentId, parentType);
            return Ok (result);
            }
        [HttpPost ("Delete_Note")]
        public async Task<ActionResult<bool>> Delete_Note ([FromBody] int noteId)
            {
            var result = await _BeService.Delete_NoteAsync (noteId);
            return Ok (result);
            }
        #endregion
        }
    }
