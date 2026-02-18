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
        #region U:usrs
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
        public async Task<bool> Update_User (User user)
            {
            if (user.UserRole.ToLower () == "student")
                {
                var response = await _http.PostAsJsonAsync ("api/Update_Student", user);
                return (response.IsSuccessStatusCode) ? true : false;
                }
            else if (user.UserRole.ToLower () == "teacher")
                {
                var response = await _http.PostAsJsonAsync ("api/Update_Teacher", user);
                return (response.IsSuccessStatusCode) ? true : false;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Delete_Teacher (int userId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Teacher", userId);
            return (response.IsSuccessStatusCode) ? true : false;
            }
        #endregion
        #region S:Students
        public async Task<int> Create_Student (User student)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_Student", student);
            //return response.IsSuccessStatusCode ? 1 : 0;
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
        public async Task<List<User>> Read_StudentsByKeyword (int userId, string keyword, int getGCEM)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentsByKeyword?keyword={keyword}&getGCEM={getGCEM}", userId);
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
        public async Task<List<User>> Read_StudentsByGCEMId (int Id, string mode, int readStudentGCEM)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentsByGCEMSId?mode={mode}&readStudentGCEM={readStudentGCEM}", Id);
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
        public async Task<bool> Update_Student (User student)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Student", student);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_StudentTags (User student)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_StudentTags", student);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Remove_StudentFromList (int studentId, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Students?mode={mode}", studentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Student (int studentId)
            {
            var response = await _http.PostAsJsonAsync ("api/Delete_Student", studentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region G:Groups
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
        public async Task<List<Group>> Read_Groups (User user, bool getGroupStudents)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Groups?getGroupStudents={getGroupStudents}", user);
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
        public async Task<Group> Read_Group (int groupId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Group", groupId);
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
        #region SG:StudentGroups
        public async Task<bool> Create_StudentGroups (int groupId, List<int> lstStudentIds)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentGroups?groupId={groupId}", lstStudentIds);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<StudentGroup>> ReadStudentGroups (int Id, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentGroups?mode={mode}", Id);
            if (response.IsSuccessStatusCode)
                {
                List<StudentGroup> lstStudentGroups = await response.Content.ReadFromJsonAsync<List<StudentGroup>> ();
                return lstStudentGroups ?? new List<StudentGroup> ();
                }
            else
                {
                return new List<StudentGroup> ();
                }
            }
        #endregion
        #region C:Courses
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
        public async Task<Course> Read_Course (int courseId, bool getStudentsList)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Course?getStudentList{getStudentsList}", courseId);
            if (response.IsSuccessStatusCode)
                {
                Course? course = await response.Content.ReadFromJsonAsync<Course> ();
                return course ?? new Course ();
                }
            else
                {
                var errorContent = await response.Content.ReadAsStringAsync ();
                return new Course ();
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
        #region CF:CourseFolders
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
        #region CT:CourseTopics
        public async Task<int> Create_CourseTopic (CourseTopic courseTopic)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_CourseTopic", courseTopic);
            if (response.IsSuccessStatusCode)
                {
                var result = await response.Content.ReadFromJsonAsync<int> ();
                return result;
                }
            return 0;
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
        #region SC:StudentCourses
        public async Task<bool> Create_StudentCourses (int courseId, List<int> lstStudentIds)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentCourses?courseId={courseId}", lstStudentIds);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Create_StudentCourse (int studentId, int courseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentCourse?courseId={courseId}", studentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<StudentCourse>> Read_StudentCourses (int Id, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentCourses?mode={mode}", Id);
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
        //Update
        public async Task<bool> Update_StudentCourse (StudentCourse studentCourse)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentCourse", studentCourse);
            if (response.IsSuccessStatusCode)
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Update_StudentCoursesTags (List<int> lstStudentIds, int CourseId, bool activeStatus)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentCoursesTags?CourseId={CourseId}&activeStatus={activeStatus}", lstStudentIds);
            if (response.IsSuccessStatusCode)
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public async Task<bool> Delete_StudentCourses (int Id, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourses?mode={mode}", Id);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentCourse (int studentCourseId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourse", studentCourseId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> CalculatePoints_StudentCourse (StudentCourse studentCourse)
            {
            var response = await _http.PostAsJsonAsync ($"api/CalculatePoints_StudentCourse", studentCourse);
            return (response.IsSuccessStatusCode) ? true : false;
            }
        #endregion
        #region SCT:StudentCourseTests
        public async Task<bool> Create_StudentCourseTest (StudentCourseTest studentCoursetest)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_StudentCourseTest", studentCoursetest);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<StudentCourseTest>> Read_StudentCourseTests (StudentCourse studentCourse, bool readOptions)
            {
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
        public async Task<bool> Update_StudentCourseTest (StudentCourseTest studentCourseTest, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentCourseTest?mode={mode}", studentCourseTest);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentCourseTests (string mode, StudentCourse studentCourse)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentCourseTests?mode={mode}", studentCourse);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region T:Tests
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
        public async Task<List<Test>> Read_TestsByCourseTopicId (int courseTopicId, int pageNumber, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestsByCourseTopicId?pageNumber={pageNumber}&readOptions={readOptions}", courseTopicId);
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
        public async Task<List<Test>> Read_TestsByStudentCourseId (int studentCourseId, bool readOptions)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_TestsByStudentCourseId?readOptions={readOptions}", studentCourseId);
            if (response.IsSuccessStatusCode)
                {
                var studentCourseTest = await response.Content.ReadFromJsonAsync<List<Test>> ();
                return studentCourseTest ?? new List<Test> ();
                }
            else
                {
                return new List<Test> ();
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
        #region TO:TestOptions
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
        #region E:Exams
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
        public async Task<Exam> Read_Exam (int examId, bool getStudentsList)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Exam?getStudentList={getStudentsList}", examId);
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
        #region EC:ExamCompositions
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
        #region ET:ExamTests
        public async Task<int> Create_ExamTestsByExamComposition (ExamComposition examComposition)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_ExamTestsByExamComposition", examComposition);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<int> Create_ExamTest (ExamTest examTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_ExamTest", examTest);
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
        #region SE:StudentExams
        public async Task<int> Create_StudentExams (int examId, List<int> lstStudentIds)
            {
            StudentExam studentExam = new StudentExam () { StudentId = 0, ExamId = examId, StartDateTime = "-", FinishDateTime = "-", StudentExamTags = 0, StudentExamPoint = 0 };
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentExams?examId={examId}", lstStudentIds);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<StudentExam>> Read_StudentExams (int Id, string mode)
            {
            try
                {
                //modes: BystudentId, ByExamId
                var response = await _http.PostAsJsonAsync ($"api/Read_StudentExams?mode={mode}", Id);
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
        public async Task<bool> Update_StudentsExamTags (string mode, int ExamId)
            {
            //modes: setStudentExamStartedOn|Off , setStudentExamFinishedOn|Off
            var response = await _http.PostAsJsonAsync ($"api/Update_StudentsExamTags?mode={mode}", ExamId);
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
            string currentDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm");
            switch (strNewStudentExamTags)
                {
                case "startedOn":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags | 2);
                        studentExam.StartDateTime = currentDateTime;
                        break;
                        }
                case "startedOff":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags & ~2);
                        studentExam.StartDateTime = "";
                        break;
                        }
                case "finishedOn":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags | 4);
                        studentExam.FinishDateTime = currentDateTime;
                        break;
                        }
                case "finishedOff":
                        {
                        studentExam.StudentExamTags = (studentExam.StudentExamTags & ~4);
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
        #endregion
        #region SET:StudentExamTests
        public async Task<int> Create_StudentExamTest (StudentExamTest studentExamTest)
            {
            var response = await _http.PostAsJsonAsync ("api/Create_StudentExamTest", studentExamTest);
            return response.IsSuccessStatusCode ? 1 : 0;
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
        public async Task<List<StudentExamTest>> Read_StudentsExamTest (StudentExamTest studentExamTest)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentsExamTest", studentExamTest);
            if (response.IsSuccessStatusCode)
                {
                var lstStudentsExamTest = await response.Content.ReadFromJsonAsync<List<StudentExamTest>> ();
                return lstStudentsExamTest ?? new List<StudentExamTest> ();
                }
            else
                {
                return new List<StudentExamTest> ();
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
        public async Task<bool> CalculatePoints_StudentExams (int studentexamid)
            {
            var response = await _http.PostAsJsonAsync ($"api/ClaculatePoints_StudentExams", studentexamid);
            return (response.IsSuccessStatusCode) ? true : false;
            }
        #endregion
        #region M:Messages
        public async Task<int> Create_Message (Message message)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_Message", message);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<Message> Read_Message (int messageId, bool getStudentMessages)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Message?getStudentMessages={getStudentMessages}", messageId);
            if (response.IsSuccessStatusCode)
                {
                Message? message = await response.Content.ReadFromJsonAsync<Message> ();
                return message ?? new Message ();
                }
            else
                {
                return new Message ();
                }
            }
        public async Task<List<Message>> Read_Messages (int userId, bool getStudentMessages)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Messages?getStudentMessages={getStudentMessages}", userId);
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
        public async Task<bool> Delete_Messages (string mode, int recipientId)
            {
            //modes: ByStudentId, ByMessageId, ByStudentMessageId
            var response = await _http.PostAsJsonAsync ($"api/Delete_Messages?mode={mode}", recipientId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region SM:StudentMessages
        public async Task<bool> Create_StudentMessages (int messageId, List<int> lstStudentIds, bool requestFeedback)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_StudentMessages?messageId={messageId}&requestFeedback={requestFeedback}", lstStudentIds);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<List<StudentMessage>> Read_StudentMessages (int Id, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentMessages?mode={mode}", Id);
            if (response.IsSuccessStatusCode)
                {
                List<StudentMessage>? lstMessages = await response.Content.ReadFromJsonAsync<List<StudentMessage>> ();
                return lstMessages ?? new List<StudentMessage> ();
                }
            else
                {
                return new List<StudentMessage> ();
                }
            }
        public async Task<Message> Read_StudentMessage (int studentMessageId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_StudentMessage", studentMessageId);
            if (response.IsSuccessStatusCode)
                {
                Message? message = await response.Content.ReadFromJsonAsync<Message> ();
                return message ?? new Message ();
                }
            else
                {
                return new Message ();
                }
            }
        public async Task<bool> Update_StudentMessageTags (StudentMessage studentMessage)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_StudentMessageTags", studentMessage);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_StudentMessageSetAsRead (StudentMessage studentMessage)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_StudentMessagesetAsRead", studentMessage);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_StudentMessage (int studentMessageId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_StudentMessage", studentMessageId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region CH:Chats
        public async Task<int> Create_Chat (Chat chat)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_Chat", chat);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Chat>> Read_Chats (int studentId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Chats", studentId);
            if (response.IsSuccessStatusCode)
                {
                List<Chat>? lstChats = await response.Content.ReadFromJsonAsync<List<Chat>> ();
                return lstChats ?? new List<Chat> ();
                }
            else
                {
                return new List<Chat> ();
                }
            }
        public async Task<List<Chat>> Read_ChatsWithOneMate (int studentId, int mateId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_ChatsWithOneMate?mateId={mateId}", studentId);
            if (response.IsSuccessStatusCode)
                {
                List<Chat>? lstChats = await response.Content.ReadFromJsonAsync<List<Chat>> ();
                return lstChats ?? new List<Chat> ();
                }
            else
                {
                return new List<Chat> ();
                }
            }
        public async Task<bool> Update_Chat (Chat chat)
            {
            var response = await _http.PostAsJsonAsync ($"api/Update_Chat", chat);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_ChatTags (Chat chat)
            {
            //1:IsRead 2:IsImp 3:IsBookmarked 4:Deleted
            var response = await _http.PostAsJsonAsync ($"api/Update_ChatTags", chat);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Chat (int chatId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Chat", chatId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region P:Projects
        public async Task<int> Create_Project (Project project)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_Project", project);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Project>> Read_Projects (int userId, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Projects?mode={mode}", userId);
            if (response.IsSuccessStatusCode)
                {
                List<Project>? lstProjects = await response.Content.ReadFromJsonAsync<List<Project>> ();
                return lstProjects ?? new List<Project> ();
                }
            else
                {
                return new List<Project> ();
                }
            }
        public async Task<Project> Read_Project (int projectId, bool readNotes)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Project?readNotes={readNotes}", projectId);
            if (response.IsSuccessStatusCode)
                {
                Project? project = await response.Content.ReadFromJsonAsync<Project> ();
                return project ?? new Project ();
                }
            else
                {
                return new Project ();
                }
            }
        public async Task<bool> Update_Project (Project project)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Project", project);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Projects (int userId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Projects", userId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Project (int projectId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Project", projectId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region SP:Subprojects
        public async Task<int> Create_Subproject (Subproject subProject)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_subproject", subProject);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Subproject>> Read_Subprojects (int projectId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_SubpProjects", projectId);
            if (response.IsSuccessStatusCode)
                {
                List<Subproject>? lstSubprojects = await response.Content.ReadFromJsonAsync<List<Subproject>> ();
                return lstSubprojects ?? new List<Subproject> ();
                }
            else
                {
                return new List<Subproject> ();
                }
            }
        public async Task<Subproject> Read_Subproject (int subProjectId, bool readNotes)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Subproject?readNotes={readNotes}", subProjectId);
            if (response.IsSuccessStatusCode)
                {
                Subproject? subProject = await response.Content.ReadFromJsonAsync<Subproject> ();
                return subProject ?? new Subproject ();
                }
            else
                {
                return new Subproject ();
                }
            }
        public async Task<bool> Update_Subproject (Subproject subProject)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Subproject", subProject);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Subprojects (int projectId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Subprojects", projectId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Subproject (int subProjectId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Subproject", subProjectId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion
        #region N:Note
        public async Task<int> Create_Note (Note note)
            {
            var response = await _http.PostAsJsonAsync ($"api/Create_Note", note);
            return response.IsSuccessStatusCode ? 1 : 0;
            }
        public async Task<List<Note>> Read_Notes (int parentId, int parentType)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Notes?parentType={parentType}", parentId);
            if (response.IsSuccessStatusCode)
                {
                List<Note>? lstNotes = await response.Content.ReadFromJsonAsync<List<Note>> ();
                return lstNotes ?? new List<Note> ();
                }
            else
                {
                return new List<Note> ();
                }
            }
        public async Task<List<Note>> Read_NotesBySearchKey (string searchKey, int parentId, string mode)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_NotesBySearchKey?mode={mode}&searchKey={searchKey}", parentId);
            if (response.IsSuccessStatusCode)
                {
                List<Note>? lstNotes = await response.Content.ReadFromJsonAsync<List<Note>> ();
                return lstNotes ?? new List<Note> ();
                }
            else
                {
                return new List<Note> ();
                }
            }
        public async Task<Note> Read_Note (int noteId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Read_Note", noteId);
            if (response.IsSuccessStatusCode)
                {
                Note? note = await response.Content.ReadFromJsonAsync<Note> ();
                return note ?? new Note ();
                }
            else
                {
                return new Note ();
                }
            }
        public async Task<bool> Update_Note (Note note)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_Note", note);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Update_NoteParent (Note note)
            {
            var response = await _http.PostAsJsonAsync ("api/Update_NoteParent", note);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Notes (int parentId, int parentType)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Notes?parentType={parentType}", parentId);
            return response.IsSuccessStatusCode ? true : false;
            }
        public async Task<bool> Delete_Note (int noteId)
            {
            var response = await _http.PostAsJsonAsync ($"api/Delete_Note", noteId);
            return response.IsSuccessStatusCode ? true : false;
            }
        #endregion        
        }
    }
