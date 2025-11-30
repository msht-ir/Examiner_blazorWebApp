using ExaminerS.Models;
using Group = ExaminerS.Models.Group;

namespace ExaminerB.Service
    {
    public class FeService
        {
        private readonly HttpClient _http;
        public FeService (HttpClient http)
            {
            _http = http;
            }
        #region LOGIN
        public async Task<User?> Login (User user)
            {
            //api/LoginTEACHER/api / LoginSTUDENT:
            var response = await _http.PostAsJsonAsync ("api/Login" + user.UserRole.ToUpper (), user);
            if (response.IsSuccessStatusCode)
                {
                return await response.Content.ReadFromJsonAsync<User> ();
                }
            else
                {
                Console.WriteLine ($"FeService: Login failed:\n {response.StatusCode}");
                return new User { UserRole = "student" };
                }
            }
        #endregion
        #region C01:usrs
        public async Task<bool> Create_Teacher (User user)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Teacher", user);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<User>> Read_Teachers ()
            {
            var response = await _http.PostAsync ("api/Read_Teachers", null);
            if (response.IsSuccessStatusCode)
                {
                List<User> lstTeachers = await response.Content.ReadFromJsonAsync<List<User>> ();
                return lstTeachers;
                }
            else
                {
                return new List<User> ();
                }
            }
        public async Task<bool> Update_UserPassword (User user)
            {
            if (user.UserRole.ToLower () == "student")
                {
                var response = await _http.PostAsJsonAsync ("api/Update_StudentPassword", user);
                return (response.IsSuccessStatusCode) ? true : false;
                }
            else if (user.UserRole.ToLower () == "teacher")
                {
                var response = await _http.PostAsJsonAsync ("api/Update_TeacherPassword", user);
                return (response.IsSuccessStatusCode) ? true : false;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Delete_Teacher (int userId)
            {
            return false;
            }
        #endregion
        #region C10:Students
        public async Task<bool> Create_Student (User student)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Student", student);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<User>> Read_Students (int userId, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Students?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", userId);
            if (response.IsSuccessStatusCode)
                {
                List<User> lstStudents = await response.Content.ReadFromJsonAsync<List<User>> ();
                return lstStudents;
                }
            else
                {
                return new List<User> ();
                }
            }
        public async Task<List<User>> Read_StudentsByGroupId (int groupId, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentsByGroupId?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", groupId);
            if (response.IsSuccessStatusCode)
                {
                List<User>? lstStudents = await response.Content.ReadFromJsonAsync<List<User>> ();
                return lstStudents;
                }
            else
                {
                return new List<User> ();
                }
            }
        public async Task<List<User>> Read_StudentsByExamId (int examId, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentsByExamId?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", examId);
            if (response.IsSuccessStatusCode)
                {
                List<User> lstStudents = await response.Content.ReadFromJsonAsync<List<User>> ();
                return lstStudents;
                }
            else
                {
                return new List<User> ();
                }
            }
        public async Task<List<User>> Read_StudentsByCourseId (int courseId, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentsByCourseId?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", courseId);
            if (response.IsSuccessStatusCode)
                {
                List<User> lstStudents = await response.Content.ReadFromJsonAsync<List<User>> ();
                return lstStudents;
                }
            else
                {
                return new List<User> ();
                }
            }
        public async Task<User> Read_Student (int studentId, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Student?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", studentId);
            if (response.IsSuccessStatusCode)
                {
                User? student = await response.Content.ReadFromJsonAsync<User> ();
                return student ?? new User ();
                }
            else
                {
                return new User ();
                }
            }
        public async Task<bool> Update_StudentsTags (User student)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentsTags", student);
            return (response.IsSuccessStatusCode) ? true : false;
            }
        public async Task<bool> Update_Student (User student)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Student", student);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_StudentPassword (User user)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_StudentPassword", user);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Students (int groupId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Students", groupId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Student (int studentId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Student", studentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C02:Courses
        public async Task<int> Create_Course (Course course)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Course", course);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Course>> Read_Courses (int userId)
            {
            string url = "api/Read_Courses";
            var response = await _http.PostAsJsonAsync (url, userId);
            if (response.IsSuccessStatusCode)
                {
                List<Course>? lstCourses = await response.Content.ReadFromJsonAsync<List<Course>> ();
                return lstCourses ?? new List<Course> ();
                }
            else
                {
                var errorContent = await response.Content.ReadAsStringAsync ();
                return new List<Course> ();
                }
            }
        public async Task<bool> Update_Course (Course course)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Course", course);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Course (int courseId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Course", courseId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C03:CourseTopics
        public async Task<int> Create_CourseTopic (CourseTopic courseTopic)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_CourseTopic", courseTopic);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<CourseTopic>> Read_CourseTopics (int courseId)
            {
            var response = await _http.PostAsJsonAsync ("api/Read_CourseTopics", courseId);

            if (response.IsSuccessStatusCode)
                {
                List<CourseTopic> lstCourseTopics = await response.Content.ReadFromJsonAsync<List<CourseTopic>> ();
                return lstCourseTopics ?? new List<CourseTopic> ();
                }
            else
                {
                return new List<CourseTopic> ();
                }
            }
        public async Task<bool> Update_CourseTopic (CourseTopic courseTopic)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_CourseTopic", courseTopic);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_CourseTopic (int courseTopicId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_CourseTopic", courseTopicId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C15:CourseFolders
        public async Task<int> Create_CourseFolder (CourseFolder courseFolder)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_CourseFolder", courseFolder);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<CourseFolder>> Read_CourseFolders (int courseId)
            {
            var response = await _http.PostAsJsonAsync ("api/Read_CourseFolders", courseId);

            if (response.IsSuccessStatusCode)
                {
                List<CourseFolder> lstCourseFolders = await response.Content.ReadFromJsonAsync<List<CourseFolder>> ();
                return lstCourseFolders ?? new List<CourseFolder> ();
                }
            else
                {
                return new List<CourseFolder> ();
                }
            }
        public async Task<bool> Update_CourseFolder (CourseFolder courseFolder)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_CourseFolder", courseFolder);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_CourseFolder (int courseFolderId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_CourseFolder", courseFolderId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C04:Tests
        public async Task<int> Create_Test (Test test)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Test", test);
            if (response.IsSuccessStatusCode)
                {
                return await response.Content.ReadFromJsonAsync<int> ();
                }
            else
                {
                return 0;
                }
            }
        public async Task<Test> Read_TestByTestId (int testId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestByTestId?readOptions={readOptions}", testId);
            if (response.IsSuccessStatusCode)
                {
                Test? test = await response.Content.ReadFromJsonAsync<Test> ();
                return test ?? new Test ();
                }
            else
                {
                return new Test ();
                }
            }
        public async Task<StudentExamTest> Read_TestByStudentExamTestId (long studentExamTestId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestByStudentExamTestId?readOptions={readOptions}", studentExamTestId);
            if (response.IsSuccessStatusCode)
                {
                var studentExamTest = await response.Content.ReadFromJsonAsync<StudentExamTest> ();
                return studentExamTest ?? new StudentExamTest ();
                }
            else
                {
                return new StudentExamTest ();
                }
            }
        public async Task<StudentCourseTest> Read_TestByStudentCourseId (string mode, StudentCourse studentCourse)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestByStudentCourseTestId?mode={mode}", studentCourse);
            if (response.IsSuccessStatusCode)
                {
                var studentCourseTest = await response.Content.ReadFromJsonAsync<StudentCourseTest> ();
                return studentCourseTest ?? new StudentCourseTest ();
                }
            else
                {
                return new StudentCourseTest ();
                }
            }
        public async Task<List<Test>> Read_TestsByCourseId (int courseId, int pageNumber, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestsByCourseId?pageNumber={pageNumber}&readOptions={readOptions}", courseId);
            if (response.IsSuccessStatusCode)
                {
                List<Test>? lstTests = await response.Content.ReadFromJsonAsync<List<Test>> ();
                return lstTests ?? new List<Test> ();
                }
            else
                {
                return new List<Test> ();
                }
            }
        public async Task<List<Test>> Read_TestsByCourseTopicId (int courseTopicId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestsByCourseTopicId?readOptions={readOptions}", courseTopicId);
            if (response.IsSuccessStatusCode)
                {
                List<Test>? lstTests = await response.Content.ReadFromJsonAsync<List<Test>> ();
                return lstTests ?? new List<Test> ();
                }
            else
                {
                return new List<Test> ();
                }
            }
        public async Task<List<Test>> Read_TestsBySearch (string strSearch, int courseId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestsBySearch?courseId={courseId}&readOptions={readOptions}", strSearch);
            if (response.IsSuccessStatusCode)
                {
                List<Test>? lstTests = await response.Content.ReadFromJsonAsync<List<Test>> ();
                return lstTests ?? new List<Test> ();
                }
            else
                {
                return new List<Test> ();
                }
            }
        public async Task<List<Test>> Read_TestsByExamId (int examId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestsByExamId?readOptions={readOptions}", examId);
            if (response.IsSuccessStatusCode)
                {
                List<Test>? lstTests = await response.Content.ReadFromJsonAsync<List<Test>> ();
                return lstTests ?? new List<Test> ();
                }
            else
                {
                return new List<Test> ();
                }
            }
        public async Task<List<StudentExamTest>> Read_TestsByStudentExamId (string mode, long studentExamId)
            {
            //modes: {withOptions | withoutOptions}
            //use (Uri.EscapeDataString) if must carry: +/- chars : ($"api/ReadStudentExamTests?mode={Uri.EscapeDataString (mode)}&id={id}", null);
            var response = await _http.PostAsync ($"api/Read_TestsByStudentExamId?mode={mode}&studentexamid={studentExamId}", null);
            if (response.IsSuccessStatusCode)
                {
                var lstStudentExamTests = await response.Content.ReadFromJsonAsync<List<StudentExamTest>> ();
                return lstStudentExamTests ?? new List<StudentExamTest> ();
                }
            else
                {
                return new List<StudentExamTest> ();
                }
            }
        public async Task<bool> Update_Test (Test test)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Test", test);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Test (int testId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Test", testId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> ImportExcelTests (string filePath, int courseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/ImportExcelTests?courseId={courseId}", filePath);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C05:TestOptions
        public async Task<int> Create_TestOption (TestOption testOption)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_TestOption", testOption);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<bool> Update_TestOption (TestOption testOption)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_TestOption", testOption);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_TestOptions (int testId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_TestOptions", testId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_TestOption (int testOptionId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_TestOption", testOptionId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C06:Exams
        public async Task<int> Create_Exam (Exam exam)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Exam", exam);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Exam>> Read_Exams (int courseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Exams", courseId);
            if (response.IsSuccessStatusCode)
                {
                List<Exam>? lstExams = await response.Content.ReadFromJsonAsync<List<Exam>> ();
                return lstExams ?? new List<Exam> ();
                }
            else
                {
                return new List<Exam> ();
                }
            }
        public async Task<Exam> Read_Exam (int examId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Exam", examId);
            if (response.IsSuccessStatusCode)
                {
                Exam? exam = await response.Content.ReadFromJsonAsync<Exam> ();
                return exam ?? new Exam ();
                }
            else
                {
                return new Exam ();
                }
            }
        public async Task<bool> Update_Exam (Exam exam)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Exam", exam);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Exams (int courseId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Exams", courseId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Exam (int examId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Exam", examId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C07:ExamCompositions
        public async Task<int> Create_ExamComposition (ExamComposition examComposition)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_ExamComposition", examComposition);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<ExamComposition>> Read_ExamCompositions (int examId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_ExamCompositions", examId);
            if (response.IsSuccessStatusCode)
                {
                List<ExamComposition>? lstExamCompositions = await response.Content.ReadFromJsonAsync<List<ExamComposition>> ();
                return lstExamCompositions ?? new List<ExamComposition> ();
                }
            else
                {
                return new List<ExamComposition> ();
                }
            }
        public async Task<ExamComposition> Read_ExamComposition (int examCompositionId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_ExamComposition", examCompositionId);
            if (response.IsSuccessStatusCode)
                {
                ExamComposition? examComposition = await response.Content.ReadFromJsonAsync<ExamComposition> ();
                return examComposition ?? new ExamComposition ();
                }
            else
                {
                return new ExamComposition ();
                }
            }
        public async Task<bool> Update_ExamComposition (ExamComposition examComposition)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_ExamComposition", examComposition);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_ExamCompositions (int examId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Exam", examId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_ExamComposition (int examCompositionId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_ExamComposition", examCompositionId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C08:ExamTests
        public async Task<int> Create_ExamTest (ExamTest examTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_ExamTest", examTest);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<int> Create_ExamTestsByExamComposition (ExamComposition examComposition)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_ExamTestsByExamComposition", examComposition);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<ExamTest>> Read_ExamTests (int examId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_ExamTests", examId);
            if (response.IsSuccessStatusCode)
                {
                List<ExamTest>? lstExamTests = await response.Content.ReadFromJsonAsync<List<ExamTest>> ();
                return lstExamTests ?? new List<ExamTest> ();
                }
            else
                {
                return new List<ExamTest> ();
                }
            }
        public async Task<ExamTest> Read_ExamTest (int examTestId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_ExamTest", examTestId);
            if (response.IsSuccessStatusCode)
                {
                ExamTest? examTest = await response.Content.ReadFromJsonAsync<ExamTest> ();
                return examTest ?? new ExamTest ();
                }
            else
                {
                return new ExamTest ();
                }
            }
        public async Task<bool> Update_ExamTest (ExamTest examTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_ExamTest", examTest);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_ExamTests (int examId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_ExamTests", examId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_ExamTest (ExamTest examTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_ExamTest", examTest);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region 09:Groups
        public async Task<bool> Create_Group (Group group)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Group", group);
            if (response.IsSuccessStatusCode)
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public async Task<List<Group>> Read_Groups (User user, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Groups?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", user);
            if (response.IsSuccessStatusCode)
                {
                List<Group>? groups = await response.Content.ReadFromJsonAsync<List<Group>> ();
                return groups ?? new List<Group> ();
                }
            else
                {
                var errorContent = await response.Content.ReadAsStringAsync ();
                return new List<Group> ();
                }
            }
        public async Task<Group> Read_Group (int groupId, bool getStudentExams, bool getStudentCourses)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Group?getStudentExams={getStudentExams}&getStudentCourses={getStudentCourses}", groupId);
            if (response.IsSuccessStatusCode)
                {
                Group? group = await response.Content.ReadFromJsonAsync<Group> ();
                return group ?? new Group ();
                }
            else
                {
                var errorContent = await response.Content.ReadAsStringAsync ();
                return new Group ();
                }
            }
        public async Task<bool> Update_Group (Group group)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Group", group);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Groups (User user)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Groups", user);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<int> Delete_Group (int groupId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Group", groupId);
            if (response.IsSuccessStatusCode)
                {
                int i = await response.Content.ReadFromJsonAsync<int> ();
                return i;
                }
            else
                {
                return 0;
                }
            }
        #endregion
        #region C11:StudentExams
        public async Task<int> Create_StudentExams (int groupId, int examId)
            {
            StudentExam studentExam = new StudentExam () { StudentId = 0, ExamId = examId, StartDateTime = "-", FinishDateTime = "-", StudentExamTags = 0, StudentExamPoint = 0 };
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentExams?groupId={groupId}", studentExam);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<int> Create_StudentExam (int studentId, int examId)
            {
            StudentExam studentExam = new StudentExam () { StudentId = studentId, ExamId = examId, StartDateTime = "-", FinishDateTime = "-", StudentExamTags = 0, StudentExamPoint = 0 };
            var response = await _http.PostAsJsonAsync ("api/Create_StudentExam", studentExam);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<StudentExam>> Read_StudentExams (int studentId, bool readInactiveExams)
            {
            try
                {
                var response = await _http.PostAsJsonAsync ($"api/Read_StudentExams?, readInactiveExams={readInactiveExams}", studentId);
                if (response.IsSuccessStatusCode)
                    {
                    return await response.Content.ReadFromJsonAsync<List<StudentExam>> () ?? new List<StudentExam> ();
                    }
                else
                    {
                    return new List<StudentExam> ();
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("GetStudentExamService error: " + ex.ToString ());
                return new List<StudentExam> ();
                }
            }
        public async Task<StudentExam> Read_StudentExam (int studentExamId, bool readInactiveExams)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentExam?readInactiveExams={readInactiveExams}", studentExamId);
            if (response.IsSuccessStatusCode)
                {
                StudentExam studentExam = await response.Content.ReadFromJsonAsync<StudentExam> ();
                return studentExam;
                }
            return new StudentExam ();
            }
        public async Task<bool> Update_StudentExam (StudentExam studentExam)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentExam", studentExam);
            if (response.IsSuccessStatusCode)
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTags (StudentExam studentExam, string strNewStudentExamTags)
            {
            string currentDateTime = DateTime.Now.ToString ("yyyy-MM-dd . HH:mm");
            switch (strNewStudentExamTags)
                {
                case "startedOn":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags | 1);
                        studentExam.StartDateTime = currentDateTime;
                        break;
                        }
                case "startedOff":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags & ~1);
                        studentExam.StartDateTime = "";
                        break;
                        }
                case "finishedOn":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags | 2);
                        studentExam.FinishDateTime = currentDateTime;
                        break;
                        }
                case "finishedOff":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags & ~2);
                        studentExam.StartDateTime = "";
                        break;
                        }
                }
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentExamTags", studentExam);
            if (response.IsSuccessStatusCode)
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Delete_StudentExamsByStudentId (int studentId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_StudentExamsByStudentId", studentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentExamsByExamId (int examId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Exam", examId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentExam (int studentExamId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_StudentExam", studentExamId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> CalculatePoints_StudentExams (int studentexamid)
            {
            var response = await _http.PostAsJsonAsync ($"api/ClaculatePoints_StudentExams", studentexamid);
            return (response.IsSuccessStatusCode) ? true : false;
            }
        #endregion
        #region C12:StudentExamTests
        public async Task<int> Create_StudentExamTest (StudentExamTest studentExamTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_StudentExamTest", studentExamTest);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<StudentExamTest>> Read_StudentExamTests (int studentExamId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentExamTests?readOptions={readOptions}", studentExamId);
            if (response.IsSuccessStatusCode)
                {
                var lstStudentExamTests = await response.Content.ReadFromJsonAsync<List<StudentExamTest>> ();
                return lstStudentExamTests ?? new List<StudentExamTest> ();
                }
            else
                {
                return new List<StudentExamTest> ();
                }
            }
        public async Task<StudentExamTest> Read_StudentExamTest (long studentExamTestId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentExamTest?readOptions={readOptions}", studentExamTestId);
            if (response.IsSuccessStatusCode)
                {
                var studentExamTest = await response.Content.ReadFromJsonAsync<StudentExamTest> ();
                return studentExamTest ?? new StudentExamTest ();
                }
            else
                {
                return new StudentExamTest ();
                }
            }
        public async Task<bool> Update_StudentExamTest (StudentExamTest studentExamTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_StudentExamTest", studentExamTest);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_StudentExamTestsTags (int studentExamId, int intTags)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentExamTestsTags?intTags={intTags}", studentExamId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_StudentExamTestTags (StudentExamTest tempStudentExamTest)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentExamTestTags", tempStudentExamTest);
            if (response.IsSuccessStatusCode)
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTestAnswer (long studentExamTestId, int answerOptionId)
            {
            StudentExamTest tempTest = new ()
                {
                StudentExamTestId = studentExamTestId,
                StudentExamTestAns = answerOptionId
                };
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentExamTestAnswer", tempTest);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentExamTests (int studentExamId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_StudentExamTests", studentExamId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentExamTest (long studentExamTestId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_StudentExamTest", studentExamTestId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C13:StudentCourses
        public async Task<bool> Create_StudentCourses (int groupId, int courseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentCourses?courseId={courseId}", groupId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Create_StudentCourse (int studentId, int courseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentCourse?courseId={courseId}", studentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<StudentCourse>> Read_StudentCourses (int studentId)
            {
            var response = await _http.PostAsJsonAsync ("api/Read_StudentCourses", studentId);
            if (response.IsSuccessStatusCode)
                {
                var lstStudentCourses = await response.Content.ReadFromJsonAsync<List<StudentCourse>> ();
                return lstStudentCourses ?? new List<StudentCourse> ();
                }
            return new List<StudentCourse> ();
            }
        public async Task<StudentCourse> Read_StudentCourse (int studentCourseId)
            {
            var response = await _http.PostAsJsonAsync ("api/Read_StudentCourse", studentCourseId);
            if (response.IsSuccessStatusCode)
                {
                var studentCourse = await response.Content.ReadFromJsonAsync<StudentCourse> ();
                return studentCourse ?? new StudentCourse ();
                }
            return new StudentCourse ();
            }
        public async Task<bool> Delete_StudentCourses (int courseId, int groupId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourses?courseId={courseId}", groupId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentCourse (int studentCourseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourse", studentCourseId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentCourseByStudentCourseId (int studentCourseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourseByStudentCourseId", studentCourseId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> CalculatePoints_StudentCourse (int studentid, int courseid)
            {
            StudentCourse studentCourse = new StudentCourse ();
            studentCourse.StudentId = studentid;
            studentCourse.CourseId = courseid;
            var response = await _http.PostAsJsonAsync ($"api/CalculatePoints_StudentCourse", studentCourse);
            return (response.IsSuccessStatusCode) ? true : false;
            }
        #endregion
        #region C14:StudentExamTests
        public async Task<bool> Update_StudentCourseTest (StudentCourseTest studentCourseTest, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentCourseTest?mode={mode}", studentCourseTest);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<StudentCourseTest>> Read_StudentCourseTests (int studentCourseId, bool readOptions)
            {
            StudentCourse studentCourse = new StudentCourse () { StudentCourseId = studentCourseId };
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentCourseTests?readOptions={readOptions}", studentCourse);
            if (response.IsSuccessStatusCode)
                {
                var lstStudentCourseTests = await response.Content.ReadFromJsonAsync<List<StudentCourseTest>> ();
                return lstStudentCourseTests ?? new List<StudentCourseTest> ();
                }
            else
                {
                return new List<StudentCourseTest> ();
                }
            }
        public async Task<StudentCourseTest> Read_StudentCourseTestRandom (int studentCourseId, bool readOptions, bool retry)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentCourseTestRandom?readOptions={readOptions}&retry={retry}", studentCourseId);
            if (response.IsSuccessStatusCode)
                {
                var studentCourseTest = await response.Content.ReadFromJsonAsync<StudentCourseTest> ();
                return studentCourseTest ?? new StudentCourseTest ();
                }
            else
                {
                return new StudentCourseTest ();
                }
            }
        public async Task<bool> Delete_StudentCourseTests (string mode, StudentCourse studentCourse)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourseTests?mode={mode}", studentCourse);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region C16:Messages
        public async Task<int> Create_Message (int groupId, Message message)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_Message?groupId={groupId}", message);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Message>> Read_Messages (string mode, int Id)
            {
            //modes: User, Group, Student, Message
            var response = await _http.PostAsJsonAsync ($"api/Read_MessagesById?mode={mode}", Id);
            if (response.IsSuccessStatusCode)
                {
                List<Message>? lstMessages = await response.Content.ReadFromJsonAsync<List<Message>> ();
                return lstMessages ?? new List<Message> ();
                }
            else
                {
                return new List<Message> ();
                }
            }
        public async Task<List<Message>> Read_Messages (int userId, string mode, string key)
            {
            //mode: Search, Date, DateTime
            var response = await _http.PostAsJsonAsync ($"api/Read_MessagesByKey?mode={mode}&key={key}", userId);
            if (response.IsSuccessStatusCode)
                {
                List<Message>? lstMessages = await response.Content.ReadFromJsonAsync<List<Message>> ();
                return lstMessages ?? new List<Message> ();
                }
            else
                {
                return new List<Message> ();
                }
            }
        public async Task<List<Message>> Read_Messages (Message message)
            {
            //mode: Search, Date, DateTime
            var response = await _http.PostAsJsonAsync ($"api/Read_MessagesByMessage", message);
            if (response.IsSuccessStatusCode)
                {
                List<Message>? lstMessages = await response.Content.ReadFromJsonAsync<List<Message>> ();
                return lstMessages ?? new List<Message> ();
                }
            else
                {
                return new List<Message> ();
                }
            }
        public async Task<bool> Update_Message (Message message)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Message", message);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_MessageTags (Message message)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Message", message);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Messages (string mode, int Id)
            {
            //modes: Group, Student, Message
            var response = await _http.PostAsJsonAsync ($"api/Delete_Messages?mode={mode}", Id);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        }
    }
