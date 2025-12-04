using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExaminerS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;
using Group = ExaminerS.Models.Group;

namespace ExaminerB.Services2Backend
    {
    public class BeService : BeIService
        {
        private readonly IConfiguration _config;
        public BeService (IConfiguration config) => _config = config;
        #region Login
        public async Task<User?> LoginTeacherAsync (User user)
            {
            string? connString = _config.GetConnectionString ("cnni");
            string sql = "SELECT Id, UsrName, UsrPass, UsrActive, UsrNickname FROM usrs WHERE UsrName=@usr AND UsrPass=@pwd AND UsrActive=1";
            using SqlConnection cnn = new (connString);
            User userOut = new ();

            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new (sql, cnn);
                cmd.Parameters.AddWithValue ("@usr", user.UserName);
                cmd.Parameters.AddWithValue ("@pwd", user.UserPass);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();

                while (await reader.ReadAsync ())
                    {
                    if ((user.UserName.ToLower () == reader.GetString (1).ToLower ()) && (user.UserPass == reader.GetString (2)))
                        {
                        userOut.UserId = reader.GetInt32 (0);
                        userOut.GroupId = 0;
                        userOut.UserName = reader.GetString (1);
                        userOut.UserPass = reader.GetString (2);
                        userOut.UserRole = "teacher";
                        userOut.UserTags = Convert.ToInt32 (reader.GetBoolean (3));
                        userOut.UserNickname = reader.GetString (4);
                        }
                    }

                await cnn.CloseAsync ();

                if (userOut.UserId > 0)
                    {
                    await LogAsync (userOut.UserId, cnn);
                    return userOut;
                    }

                return null;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("LoginAsTeacher error: " + ex.Message);
                cnn.Close ();
                return null;
                }
            }
        public async Task<User?> LoginStudentAsync (User user)
            {
            string? connString = _config.GetConnectionString ("cnni");
            string sql = "SELECT StudentId, GroupId, StudentName, StudentPass, StudentTags, StudentNickname FROM Students WHERE StudentName=@studentname AND StudentPass=@studentpass AND GroupId=@groupid AND (StudentTags & 1) = 1";
            using SqlConnection cnn = new (connString);
            User userOut = new ();
            try
                {
                await cnn.OpenAsync ();
                using SqlCommand cmd = new (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentname", user.UserName);
                cmd.Parameters.AddWithValue ("@studentpass", user.UserPass);
                cmd.Parameters.AddWithValue ("@groupid", user.GroupId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    if ((user.UserName.ToLower () == reader.GetString (2).ToLower ()) && (user.UserPass == reader.GetString (3)))
                        {
                        userOut.UserId = reader.GetInt32 (0);
                        userOut.GroupId = reader.GetInt32 (1);
                        userOut.UserName = reader.GetString (2);
                        userOut.UserPass = reader.GetString (3);
                        userOut.UserRole = "student";
                        userOut.UserTags = reader.GetInt32 (4);
                        userOut.UserNickname = reader.GetString (5);
                        }
                    }
                if (userOut.UserId > 0)
                    {
                    await cnn.CloseAsync (); //logout because LogAsync tries to open cnn itset 
                    await LogAsync (userOut.UserId, cnn);
                    return userOut;
                    }
                return new User ();
                }
            catch (Exception ex)
                {
                Console.WriteLine ("*BeService: LoginAsStudent error: \n" + ex.Message);
                return new User ();
                }
            }
        public async Task LogAsync (int userId, SqlConnection cnn)
            {
            string sql = "INSERT INTO usrsLogs (UserId, UserLog, DateTime) VALUES (@userid, @userlog, @datetime)";
            await cnn.OpenAsync ();
            SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", userId);
            cmd.Parameters.AddWithValue ("@userlog", 21);
            cmd.Parameters.AddWithValue ("@datetime", DateTime.Now.ToString ("yyyy-MM-dd . HH:mm"));
            await cmd.ExecuteNonQueryAsync ();
            }
        #endregion
        #region C01:usrs
        public async Task<int> Create_TeacherAsync (User user)
            {
            string sql = "INSERT INTO usrs (UsrName, UsrPass, UsrActive) VALUES (@usrname, @usrpass, 1); SELECT CAST (scope_identity() AS int)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@usrname", user.UserName);
            cmd.Parameters.AddWithValue ("@usrpass", user.UserPass);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<User>> Read_TeachersAsync ()
            {
            var users = new List<User> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            using SqlCommand cmd = new ("SELECT ID, UsrName, UsrPass, UsrActive, UsrNickname FROM usrs", cnn);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                users.Add (new User
                    {
                    UserId = reader.GetInt32 (0),
                    UserName = reader.GetString (1),
                    UserPass = reader.GetString (2),
                    UserTags = Convert.ToInt32 (reader.GetBoolean (3)),
                    UserNickname = reader.GetString (4)
                    });
                }
            return users;
            }
        public async Task<bool> Update_TeacherPasswordAsync (User user)
            {
            string sql = "UPDATE usrs SET UsrPass=@usrpass, UsrNickname=@usrnickname WHERE ID=@userid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd3 = new SqlCommand (sql, cnn);
                cmd3.Parameters.AddWithValue ("@usrpass", user.UserPass);
                cmd3.Parameters.AddWithValue ("@usrnickname", user.UserNickname);
                cmd3.Parameters.AddWithValue ("@userid", user.UserId);
                await cmd3.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                return false;
                }
            }
        public async Task<bool> Delete_TeacherAsync (int userId)
            {
            string sql = "DELETE FROM usrs WHERE ID=@id";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@id", userId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region C10:Students
        public async Task<int> Create_StudentAsync (User student)
            {
            string sql = "INSERT INTO Students (GroupId, StudentName, StudentPass, StudentTags, StudentNickname) VALUES (@groupid, @studentname, @studentpass, @studenttags, @studentnickname); SELECT CAST (scope_identity() AS int)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupid", student.GroupId);
                cmd.Parameters.AddWithValue ("@studentname", student.UserName);
                cmd.Parameters.AddWithValue ("@studentpass", student.UserPass);
                cmd.Parameters.AddWithValue ("@studenttags", student.UserTags);
                cmd.Parameters.AddWithValue ("@studentnickname", student.UserNickname);
                int i = (int) await cmd.ExecuteScalarAsync ();
                return i;
                }
            catch
                {
                await cnn.CloseAsync ();
                return 0;
                }
            }
        public async Task<List<User>> Read_StudentsAsync (int userId, bool readStudentExams, bool readStudentCourses)
            {
            var lstStudents = new List<User> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT s.StudentId, s.GroupId, s.StudentName, s.StudentPass, s.StudentTags, s.StudentNickname FROM Students s INNER JOIN Groups g ON s.GroupId=g.GroupId WHERE g.UserId=@userid", cnn);
            cmd.Parameters.AddWithValue ("@userid", userId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstStudents.Add (new User
                    {
                    UserId = reader.GetInt32 (0),
                    GroupId = reader.GetInt32 (1),
                    UserName = reader.GetString (2),
                    UserPass = reader.GetString (3),
                    UserTags = reader.GetInt32 (4),
                    UserNickname = reader.GetString (5),
                    UserRole = "-",
                    StudentExams = new List<StudentExam> (),
                    StudentCourses = new List<StudentCourse> ()

                    });
                }
            if (readStudentExams)
                {
                foreach (User student in lstStudents)
                    {
                    student.StudentExams = await Read_StudentExamsAsync (student.UserId, true);
                    }
                }
            if (readStudentCourses)
                {
                foreach (User student in lstStudents)
                    {
                    student.StudentCourses = await Read_StudentCoursesAsync (student.UserId);
                    }
                }
            return lstStudents;
            }
        public async Task<List<User>> Read_StudentsByGroupIdAsync (int groupId, bool readStudentExams, bool readStudentCourses)
            {
            List<User> lstStudents = new List<User> ();
            string sql = "SELECT s.StudentId, s.GroupId, s.StudentName, s.StudentPass, s.StudentTags, s.StudentNickname FROM Students s WHERE s.GroupId=@groupid ORDER BY s.StudentName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupid", groupId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        var student = new User
                            {
                            UserId = reader.GetInt32 (0),
                            GroupId = reader.GetInt32 (1),
                            UserName = reader.GetString (2),
                            UserPass = reader.GetString (3),
                            UserTags = reader.GetInt32 (4),
                            UserNickname = reader.GetString (5),
                            UserRole = "-",
                            StudentExams = new List<StudentExam> (),
                            StudentCourses = new List<StudentCourse> ()
                            };
                        lstStudents.Add (student);
                        }
                    }
                if (readStudentExams)
                    {
                    foreach (User student in lstStudents)
                        {
                        student.StudentExams = await Read_StudentExamsAsync (student.UserId, true);
                        }
                    }
                if (readStudentCourses)
                    {
                    foreach (User student in lstStudents)
                        {
                        student.StudentCourses = await Read_StudentCoursesAsync (student.UserId);
                        }
                    }
                return lstStudents;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new List<User> ();
                }
            }
        public async Task<List<User>> Read_StudentsByExamIdAsync (int examId, bool readStudentExams, bool readStudentCourses)
            {
            List<User> lstStudents = new List<User> ();
            string sql = "SELECT DISTINCT s.StudentId, s.GroupId, s.StudentName, s.StudentPass, s.StudentTags, s.StudentNickname FROM Students s INNER JOIN StudentExams se ON s.StudentId = se.StudentId WHERE se.ExamId=@examid ORDER BY s.StudentName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@examid", examId);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    var student = new User
                        {
                        UserId = reader.GetInt32 (0),
                        GroupId = reader.GetInt32 (1),
                        UserName = reader.GetString (2),
                        UserPass = reader.GetString (3),
                        UserTags = reader.GetInt32 (4),
                        UserNickname = reader.GetString (5),
                        UserRole = "-",
                        StudentExams = new List<StudentExam> (),
                        StudentCourses = new List<StudentCourse> ()
                        };
                    lstStudents.Add (student);
                    }
                if (readStudentExams)
                    {
                    foreach (User student in lstStudents)
                        {
                        student.StudentExams = await Read_StudentExamsAsync (student.UserId, true);
                        }
                    }
                if (readStudentCourses)
                    {
                    foreach (User student in lstStudents)
                        {
                        student.StudentCourses = await Read_StudentCoursesAsync (student.UserId);
                        }
                    }
                return lstStudents;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new List<User> ();
                }
            }
        public async Task<List<User>> Read_StudentsByCourseIdAsync (int courseId, bool readStudentExams, bool readStudentCourses)
            {
            List<User> lstStudents = new List<User> ();
            string sql = "SELECT s.StudentId, s.GroupId, s.StudentName, s.StudentPass, s.StudentTags, s.StudentNickname FROM Students s INNER JOIN StudentCourses sc ON s.StudentId = sc.StudentId WHERE se.CourseId=@courseid ORDER BY s.StudentName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@courseid", courseId);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    var student = new User
                        {
                        UserId = reader.GetInt32 (0),
                        GroupId = reader.GetInt32 (1),
                        UserName = reader.GetString (2),
                        UserPass = reader.GetString (3),
                        UserTags = reader.GetInt32 (4),
                        UserNickname = reader.GetString (5),
                        UserRole = "-",
                        StudentExams = new List<StudentExam> (),
                        StudentCourses = new List<StudentCourse> ()
                        };
                    lstStudents.Add (student);
                    }
                if (readStudentExams)
                    {
                    foreach (User student in lstStudents)
                        {
                        student.StudentExams = await Read_StudentExamsAsync (student.UserId, true);
                        }
                    }
                if (readStudentCourses)
                    {
                    foreach (User student in lstStudents)
                        {
                        student.StudentCourses = await Read_StudentCoursesAsync (student.UserId);
                        }
                    }
                return lstStudents;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new List<User> ();
                }
            }
        public async Task<User> Read_StudentAsync (int studentId, bool readStudentExams, bool readStudentCourses)
            {
            User student = new User ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT s.StudentId, s.GroupId, s.StudentName, s.StudentPass, s.StudentTags, s.StudentNickname FROM Students s WHERE s.StudentId =@studentid", cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                student.UserId = reader.GetInt32 (0);
                student.GroupId = reader.GetInt32 (1);
                student.UserName = reader.GetString (2);
                student.UserPass = reader.GetString (3);
                student.UserTags = reader.GetInt32 (4);
                student.UserNickname = reader.GetString (5);
                student.UserRole = "-";
                student.StudentExams = new List<StudentExam> ();
                student.StudentCourses = new List<StudentCourse> ();
                }
            if (readStudentExams)
                {
                student.StudentExams = await Read_StudentExamsAsync (student.UserId, true);
                }
            if (readStudentCourses)
                {
                student.StudentCourses = await Read_StudentCoursesAsync (student.UserId);
                }
            return student;
            }
        public async Task<bool> Update_StudentsTagsAsync (User student)
            {
            //student is a temp instant of user that holds: Tags, GroupId
            //tags: 1:Active 2:Pass 4:RevExamTests 8:CourseExams 16:TryCorrect 32:RevCourseExams            
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            sql = "Update Students SET StudentTags=@studenttags WHERE GroupId=@groupid";
            await cnn.OpenAsync ();
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studenttags", student.UserTags); //tags from: received template
            cmd.Parameters.AddWithValue ("@groupid", student.GroupId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Update_StudentAsync (User student)
            {
            //tags: 1:Active 2:ChangePass 4:ReviewExams 8:RunningExams 16:TryCorrect 32:ReviewRunningExams            
            List<User> lstStudents = new List<User> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            sql = "Update Students SET StudentName=@studentname, StudentPass=@studentpass, StudentTags=@studenttags, StudentNickname=@studentnickname WHERE StudentId=@studentid";
            await cnn.OpenAsync ();
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentname", student.UserName);
            cmd.Parameters.AddWithValue ("@studentpass", student.UserPass);
            cmd.Parameters.AddWithValue ("@studenttags", student.UserTags);
            cmd.Parameters.AddWithValue ("@studentnickname", student.UserNickname);
            cmd.Parameters.AddWithValue ("@studentid", student.UserId);
            await cmd.ExecuteNonQueryAsync ();

            return true;
            }
        public async Task<bool> Update_StudentPasswordAsync (User user)
            {
            string? connString = _config.GetConnectionString ("cnni");
            string sql = "UPDATE Students SET StudentPass=@studentpass, StudentNickname=@studentnickname WHERE StudentId=@studentid";
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentpass", user.UserPass);
                cmd.Parameters.AddWithValue ("@studentnickname", user.UserNickname);
                cmd.Parameters.AddWithValue ("@studentid", user.UserId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                return false;
                }
            }
        public async Task<bool> Delete_StudentsAsync (int groupId)
            {
            try
                {
                List<User> lstStudents = new List<User> ();
                string? connString = _config.GetConnectionString ("cnni");
                using SqlConnection cnn = new (connString);
                lstStudents = await Read_StudentsByGroupIdAsync (groupId, false, false);
                await cnn.OpenAsync ();
                foreach (User stdnt in lstStudents)
                    {
                    //14
                    string sql14 = "DELETE FROM StudentExamTests WHERE StudentId=@studentid";
                    var cmd14 = new SqlCommand (sql14, cnn);
                    cmd14.Parameters.AddWithValue ("@studentid", stdnt.UserId); //ids from: lstStudents
                    await cmd14.ExecuteNonQueryAsync ();
                    //13
                    string sql13 = "DELETE FROM StudentCourses WHERE StudentId=@studentid";
                    var cmd13 = new SqlCommand (sql13, cnn);
                    cmd13.Parameters.AddWithValue ("@studentid", stdnt.UserId); //ids from: lstStudents
                    await cmd13.ExecuteNonQueryAsync ();
                    //12
                    string sql12 = "DELETE FROM StudentExamTests WHERE StudentId=@studentid";
                    var cmd12 = new SqlCommand (sql12, cnn);
                    cmd12.Parameters.AddWithValue ("@studentid", stdnt.UserId); //ids from: lstStudents
                    await cmd12.ExecuteNonQueryAsync ();
                    //11
                    string sql11 = "DELETE FROM StudentExams WHERE StudentId=@studentid";
                    var cmd11 = new SqlCommand (sql11, cnn);
                    cmd11.Parameters.AddWithValue ("@studentid", stdnt.UserId); //ids from: lstStudents
                    await cmd11.ExecuteNonQueryAsync ();
                    //10
                    string sql10 = "DELETE FROM StudentExams WHERE StudentId=@studentid";
                    var cmd10 = new SqlCommand (sql10, cnn);
                    cmd10.Parameters.AddWithValue ("@studentid", stdnt.UserId); //ids from: lstStudents
                    await cmd10.ExecuteNonQueryAsync ();
                    }
                string sql09 = "DELETE FROM Groups WHERE GroupId=@groupid";
                var cmd09 = new SqlCommand (sql09, cnn);
                cmd09.Parameters.AddWithValue ("@groupid", groupId);
                await cmd09.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return false;
                }
            }
        public async Task<bool> Delete_StudentAsync (int studentId)
            {
            List<User> lstStudents = new List<User> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                //14
                string sql14 = "DELETE FROM StudentCourseTests WHERE StudentCourseId IN (SELECT StudentCourseId FROM StudentCourses WHERE StudentId=@studentid)";
                var cmd14 = new SqlCommand (sql14, cnn);
                cmd14.Parameters.AddWithValue ("@studentid", studentId);
                await cmd14.ExecuteNonQueryAsync ();
                //13
                string sql13 = "DELETE FROM StudentCourses WHERE StudentId=@studentid";
                var cmd13 = new SqlCommand (sql13, cnn);
                cmd13.Parameters.AddWithValue ("@studentid", studentId);
                await cmd13.ExecuteNonQueryAsync ();
                //12
                string sql12 = "DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE StudentId=@studentid)";
                var cmd12 = new SqlCommand (sql12, cnn);
                cmd12.Parameters.AddWithValue ("@studentid", studentId);
                await cmd12.ExecuteNonQueryAsync ();
                //11
                string sql11 = "DELETE FROM StudentExams WHERE StudentId=@studentid";
                var cmd11 = new SqlCommand (sql11, cnn);
                cmd11.Parameters.AddWithValue ("@studentid", studentId);
                await cmd11.ExecuteNonQueryAsync ();
                //10
                string sql10 = "DELETE FROM Students WHERE StudentId=@studentid";
                var cmd10 = new SqlCommand (sql10, cnn);
                cmd10.Parameters.AddWithValue ("@studentid", studentId);
                await cmd10.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                await cnn.CloseAsync ();
                return false;
                }
            }
        #endregion
        #region C02:Courses
        public async Task<int> Create_CourseAsync (Course course)
            {
            string sql = "INSERT INTO Courses (UserId, CourseName) VALUES (@userid, @coursename); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", course.UserId);
            cmd.Parameters.AddWithValue ("@coursename", course.CourseName);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        public async Task<List<Course>> Read_CoursesAsync (int userId)
            {
            List<Course> lstCourses = new List<Course> ();
            string sql = "SELECT CourseId, UserId, CourseName, CourseUnits, CourseRtl FROM Courses WHERE UserId=@userid ORDER BY CourseName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@userid", userId);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstCourses.Add (new Course
                        {
                        CourseId = reader.GetInt32 (0),
                        UserId = userId,
                        CourseName = reader.GetString (2),
                        CourseUnits = reader.GetInt32 (3),
                        CourseRtl = reader.GetBoolean (4),
                        CourseTopics = new List<CourseTopic> (),
                        CourseFolders = new List<CourseFolder> ()
                        });
                    }
                foreach (Course crs in lstCourses)
                    {
                    var crsTopic = await Read_CourseTopicsAsync (crs.CourseId);
                    if (crsTopic != null)
                        {
                        crs.CourseTopics = crsTopic;
                        }
                    var crsFolder = await Read_CourseFoldersAsync (crs.CourseId);
                    if (crsFolder != null)
                        {
                        crs.CourseFolders = crsFolder;
                        }
                    }
                return lstCourses;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C02 - Error\n" + ex.ToString ());
                return lstCourses;
                }
            }
        public async Task<bool> Update_CourseAsync (Course course)
            {
            string sql = "UPDATE Courses SET CourseName=@coursename WHERE CourseId = @courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursename", course.CourseName);
            cmd.Parameters.AddWithValue ("@courseid", course.CourseId);
            int i = cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_CourseAsync (int courseId)
            {
            string sql = "";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            sql = @"DELETE FROM Courses WHERE CourseId = @courseid;
                    DELETE FROM CourseTopics WHERE CourseId = @courseid;
                    DELETE FROM TestOptions WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM ExamTests WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM StudentExamTests WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM StudentCourseTests WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM Tests WHERE CourseId = @courseid;
                    DELETE FROM ExamCompositions WHERE ExamId IN (SELECT ExamId FROM Exams WHERE CourseId = @courseid);
                    DELETE FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM Exams WHERE CourseId = @courseid);
                    DELETE FROM Exams WHERE CourseId = @courseid;
                    DELETE FROM StudentCourses WHERE CourseId = @courseid;";
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region C03:CourseTopics
        public async Task<int> Create_CourseTopicAsync (CourseTopic courseTopic)
            {
            string sql = "INSERT INTO CourseTopics (CourseId, CourseTopicTitle) VALUES (@courseid, @coursetopictitle); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseTopic.CourseId);
            cmd.Parameters.AddWithValue ("@coursetopictitle", courseTopic.CourseTopicTitle);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        public async Task<List<CourseTopic>> Read_CourseTopicsAsync (int courseId)
            {
            List<CourseTopic> lstCourseTopics = new ();

            string sql = "SELECT CourseTopicId, CourseId, CourseTopicTitle FROM CourseTopics WHERE CourseId=@courseId ORDER BY CourseTopicTitle";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@courseId", courseId);
                SqlDataReader reader = cmd.ExecuteReader ();
                int i = 0;
                lstCourseTopics.Clear ();
                while (await reader.ReadAsync ())
                    {
                    i++;
                    lstCourseTopics.Add (new CourseTopic
                        {
                        CourseTopicId = reader.GetInt32 (0),
                        CourseId = courseId,
                        CourseTopicTitle = reader.GetString (2)
                        });
                    }
                return lstCourseTopics;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                return new List<CourseTopic> ();
                }
            }
        public async Task<bool> Update_CourseTopicAsync (CourseTopic courseTopic)
            {
            string sql = "UPDATE CourseTopics SET CourseTopicTitle = @coursetopictitle WHERE CourseTopicId = @id";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursetopictitle", courseTopic.CourseTopicTitle);
            cmd.Parameters.AddWithValue ("@id", courseTopic.CourseTopicId);
            int i = cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_CourseTopicAsync (int courseTopicId)
            {
            string sql = @"DELETE FROM CourseTopics 
                            WHERE CourseTopicId=@coursetopicid 
                            AND NOT EXISTS (SELECT 1 FROM Tests WHERE TopicId=@coursetopicid) 
                            AND NOT EXISTS (SELECT 1 FROM ExamCompositions WHERE TopicId=@coursetopicid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursetopicid", courseTopicId);
            int i = cmd.ExecuteNonQuery ();
            return (i > 0) ? true : false;
            }
        #endregion
        #region C15:CourseFolders
        public async Task<int> Create_CourseFolderAsync (CourseFolder courseFolder)
            {
            string sql = "INSERT INTO CourseFolders (CourseId, CourseFolderTitle, CourseFolderUrl, CourseFolderActive) VALUES (@courseid, @coursefoldertitle, @coursefolderurl, @coursefolderactive); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseFolder.CourseId);
            cmd.Parameters.AddWithValue ("@coursefoldertitle", courseFolder.CourseFolderTitle);
            cmd.Parameters.AddWithValue ("@coursefolderurl", courseFolder.CourseFolderUrl);
            cmd.Parameters.AddWithValue ("@coursefolderactive", courseFolder.CourseFolderActive);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        public async Task<List<CourseFolder>> Read_CourseFoldersAsync (int courseId)
            {
            List<CourseFolder> lstCourseFolders = new ();

            string sql = "SELECT CourseFolderId, CourseId, CourseFolderTitle, CourseFolderUrl, CourseFolderActive FROM CourseFolders WHERE CourseId=@courseId ORDER BY CourseFolderTitle";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@courseId", courseId);
                SqlDataReader reader = cmd.ExecuteReader ();
                int i = 0;
                lstCourseFolders.Clear ();
                while (await reader.ReadAsync ())
                    {
                    i++;
                    lstCourseFolders.Add (new CourseFolder
                        {
                        CourseFolderId = reader.GetInt32 (0),
                        CourseId = courseId,
                        CourseFolderTitle = reader.GetString (2),
                        CourseFolderUrl = reader.GetString (3),
                        CourseFolderActive = reader.GetBoolean (4)
                        });
                    }
                return lstCourseFolders;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                return new List<CourseFolder> ();
                }
            }
        public async Task<bool> Update_CourseFolderAsync (CourseFolder courseFolder)
            {
            string sql = "UPDATE CourseFolders SET CourseFolderTitle = @coursefoldertitle, CourseFolderUrl = @coursefolderurl, CourseFolderActive = @coursefolderactive WHERE CourseFolderId = @coursefolderid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursefoldertitle", courseFolder.CourseFolderTitle);
            cmd.Parameters.AddWithValue ("@coursefolderurl", courseFolder.CourseFolderUrl);
            cmd.Parameters.AddWithValue ("@coursefolderactive", courseFolder.CourseFolderActive);
            cmd.Parameters.AddWithValue ("@coursefolderid", courseFolder.CourseFolderId);
            int i = cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_CourseFolderAsync (int courseFolderId)
            {
            string sql = @"DELETE FROM CourseFolders WHERE CourseFolderId=@coursefolderid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursefolderid", courseFolderId);
            int i = cmd.ExecuteNonQuery ();
            return (i > 0) ? true : false;
            }
        #endregion
        #region C04:Tests
        public async Task<int> Create_TestAsync (Test test)
            {
            string sql = "INSERT INTO Tests (CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags) VALUES (@courseid, @topicid, @testtitle, @testtype, @testlevel, @testtags); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", test.CourseId);
            cmd.Parameters.AddWithValue ("@topicid", test.TopicId);
            cmd.Parameters.AddWithValue ("@testtitle", test.TestTitle);
            cmd.Parameters.AddWithValue ("@testtype", test.TestType);
            cmd.Parameters.AddWithValue ("@testlevel", test.TestLevel);
            cmd.Parameters.AddWithValue ("@testtags", test.TestTags);
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<Test> Read_TestByTestIdAsync (int testId, bool readOptions)
            {
            Test test = new Test ();
            string sql = "SELECT TestId, CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags FROM Tests WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    test.TestId = reader.GetInt32 (0);
                    test.CourseId = reader.GetInt32 (1);
                    test.TopicId = reader.GetInt32 (2);
                    test.TestTitle = reader.GetString (3);
                    test.TestType = reader.GetInt32 (4);
                    test.TestLevel = reader.GetInt32 (5);
                    test.TestTags = reader.GetInt32 (6);
                    test.TestOptions = new List<TestOption> ();
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                }
            return test;
            }
        public async Task<Test> Read_TestByStudentExamTestIdAsync (long studentExamTestId, bool readOptions)
            {
            Test test = new Test ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN StudentExamTests est ON t.TestId=est.TestId WHERE est.TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", studentExamTestId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    test.TestId = reader.GetInt32 (0);
                    test.CourseId = reader.GetInt32 (1);
                    test.TopicId = reader.GetInt32 (2);
                    test.TestTitle = reader.GetString (3);
                    test.TestType = reader.GetInt32 (4);
                    test.TestLevel = reader.GetInt32 (5);
                    test.TestTags = reader.GetInt32 (6);
                    test.TestOptions = new List<TestOption> ();
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                }
            return test;
            }
        public async Task<List<Test>> Read_TestsByCourseIdAsync (int courseId, int pageNumber, bool readOptions)
            {
            int pageSize = 20;
            int offset = (pageNumber - 1) * pageSize;
            List<Test> lstTests = new List<Test> ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t WHERE t.CourseId=@courseid ORDER BY t.TestId OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            cmd.Parameters.AddWithValue ("@offset", offset);
            cmd.Parameters.AddWithValue ("@pagesize", pageSize);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstTests;
            }
        public async Task<List<Test>> Read_TestsByCourseTopicIdAsync (int courseTopicId, bool readOptions)
            {
            List<Test> lstCourseTopicTests = new List<Test> ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN Courses c ON t.CourseId=c.CourseId WHERE t.TopicId=@topicid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@topicid", courseTopicId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstCourseTopicTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstCourseTopicTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstCourseTopicTests;
            }
        public async Task<List<Test>> Read_TestsBySearchAsync (string strSearch, int courseId, bool readOptions)
            {
            List<Test> lstCourseTopicTests = new List<Test> ();
            strSearch = $"%{strSearch}%";
            string sql = "SELECT TOP 50 t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN Courses c ON t.CourseId=c.CourseId WHERE t.TestTitle LIKE @strSearch AND t.CourseId=@courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.Add ("@strSearch", SqlDbType.NVarChar, 200).Value = strSearch;
            //cmd.Parameters.AddWithValue ("@strsearch", strSearch);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstCourseTopicTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstCourseTopicTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstCourseTopicTests;
            }
        public async Task<List<Test>> Read_TestsByExamIdAsync (int examId, bool readOptions)
            {
            List<Test> lstExamTests = new List<Test> ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN ExamTests et ON t.TestId=et.TestId WHERE et.ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstExamTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstExamTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstExamTests;
            }
        public async Task<List<Test>> Read_TestsByStudentExamIdAsync (int studentExamId, bool readOptions)
            {
            List<Test> lstStudentExamTests = new List<Test> ();
            string sql = "SELECT TestId, CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags FROM Tests WHERE TestId IN (SELECT TestId FROM StudentExamTests WHERE StudentExamId=@studentexamid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstStudentExamTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstStudentExamTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstStudentExamTests;
            }
        public async Task<List<Test>> Read_TestsByStudentCourseIdAsync (int studentCourseId, bool readOptions)
            {
            List<Test> lstStudentCourseTests = new List<Test> ();
            string sql = "SELECT TestId, CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags FROM Tests WHERE TestId IN (SELECT TestId FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstStudentCourseTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstStudentCourseTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstStudentCourseTests;
            }
        public async Task<bool> Update_TestAsync (Test test)
            {
            string sql = "UPDATE Tests SET CourseId=@courseid, TopicId=@topicid, TestTitle=@testtitle, TestType=@testtype, TestLevel=@testlevel, TestTags=@testtags WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", test.CourseId);
            cmd.Parameters.AddWithValue ("@topicid", test.TopicId);
            cmd.Parameters.AddWithValue ("@testtitle", test.TestTitle);
            cmd.Parameters.AddWithValue ("@testtype", test.TestType);
            cmd.Parameters.AddWithValue ("@testlevel", test.TestLevel);
            cmd.Parameters.AddWithValue ("@testtags", test.TestTags);
            cmd.Parameters.AddWithValue ("@testid", test.TestId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_TestAsync (int testId)
            {
            string sql = "DELETE FROM Tests WHERE TestId=@testid; DELETE FROM TestOptions WHERE TestId=@testid;";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> ImportExcelTestsAsync (string filePath, int courseId)
            {
            CourseTopic topic = new CourseTopic ();
            List<TestOption> lstOptions = new List<TestOption> ();
            TestOption option = new TestOption ();
            Test test = new Test ();
            test.CourseId = courseId;
            try
                {
                using (IXLWorkbook WB = new XLWorkbook (filePath))
                    {
                    var WS0 = WB.Worksheets.ElementAtOrDefault (0);
                    int iRow = 0;
                    //Excel columns: {1:Test, 2:testRTL, 3:optRTL, 4:nOpt, 5-9:opt1-5Texts, 10:Ans, 11:forceLast 12:Level 13:Topic} 
                    foreach (IXLRow Rowx in WS0.Rows ())
                        {
                        iRow = iRow + 1;
                        if (iRow > 1)
                            {
                            test.TestTitle = WS0.Cell (iRow, 1).Value.ToString ();
                            test.TestTags = 0;//initialize to 0
                            if (WS0.Cell (iRow, 2).Value.ToString ().ToLower () == "y")
                                {
                                test.TestTags = 1;
                                }
                            if (WS0.Cell (iRow, 3).Value.ToString ().ToLower () == "y")
                                {
                                test.TestTags = test.TestTags + 2;
                                }
                            test.TestType = Convert.ToInt32 (WS0.Cell (iRow, 4).Value.ToString ());   //nOpts
                            test.TestLevel = Convert.ToInt32 (WS0.Cell (iRow, 12).Value.ToString ()); //TestLevel
                            topic.CourseTopicTitle = Strings.Left (WS0.Cell (iRow, 13).Value.ToString (), 30).Trim (); //Topic
                            //test.TopicId = await Read_CourseTopicsAsync (topic);
                            if (test.TopicId == 0)
                                {
                                test.TopicId = await Create_CourseTopicAsync (topic);
                                }
                            //Console.WriteLine ("-----\niRow= " + iRow + "\ntest.CourseId= " + test.CourseId + "\ntopic.CourseTopicTitle= " + topic.CourseTopicTitle + "\ntest.TopicId= " + test.TopicId);
                            //save Test
                            test.TestId = await Create_TestAsync (test);
                            //Options
                            lstOptions.Clear ();
                            for (int col = 5; col <= 9; col++)
                                {
                                lstOptions.Add (new TestOption { TestOptionTitle = WS0.Cell (iRow, col).Value.ToString () });
                                }
                            int answ = Convert.ToInt32 (WS0.Cell (iRow, 10).Value.ToString () ?? "0");
                            lstOptions [answ - 1].TestOptionTags = 2; //IsAns
                            int forceLast = Convert.ToInt32 (WS0.Cell (iRow, 11).Value.ToString () ?? "0");
                            if (forceLast != 0)
                                {
                                lstOptions [forceLast - 1].TestOptionTags += 1; //ForceLast
                                }
                            //Save options
                            for (int i = 0; i < test.TestType; i++)
                                {
                                int addOpt = await Create_TestOptionAsync (lstOptions [i]);
                                if (addOpt != 0)
                                    {
                                    //Console.WriteLine ("testOption " + i + " was not added to db :: " + lstOptions[i].TestOptionTitle);
                                    }
                                }
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in:[ImportExcel] \n" + ex.ToString ());
                }
            return true;
            }
        #endregion
        #region C05:TestOptions
        public async Task<int> Create_TestOptionAsync (TestOption testOption)
            {
            string sql = "INSERT INTO TestOptions (TestId, TestOptionTitle, TestOptionTags) VALUES (@testid, @testoptiontitle, @testoptiontags); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testOption.TestId);
            cmd.Parameters.AddWithValue ("@testoptiontitle", testOption.TestOptionTitle);
            cmd.Parameters.AddWithValue ("@testoptiontags", testOption.TestOptionTags);
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<TestOption>> Read_TestOptionsAsync (int testId, SqlConnection cnn)
            {
            //TestOptions are collected by DTOs: {4:Tests, 8:ExamTests, 12:StudentExamTests, 14:StudentExamTests}
            Random random = new Random ();
            List<TestOption> lstTestOptions = new List<TestOption> ();
            string sql = "SELECT TestOptionId, TestId, TestOptionTitle, TestOptionTags FROM TestOptions WHERE TestId=@testid";
            try
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@testid", testId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        lstTestOptions.Add (new TestOption
                            {
                            TestOptionId = reader.GetInt32 (0),
                            TestId = reader.GetInt32 (1),
                            TestOptionTitle = reader.GetString (2),
                            TestOptionTags = reader.GetInt32 (3)
                            });
                        }
                    }
                //shuffle
                int Nshuffle = lstTestOptions.Count; //shuffle all options
                TestOption tmpx;
                TestOption tmp1 = new TestOption ();
                for (int p = 0; p < lstTestOptions.Count; p++)
                    {
                    if ((lstTestOptions [p].TestOptionTags & 1) == 1)
                        {
                        tmpx = lstTestOptions [p];
                        //send ForceLast to last position
                        tmp1 = lstTestOptions [lstTestOptions.Count - 1];
                        lstTestOptions [lstTestOptions.Count - 1] = tmpx;
                        lstTestOptions [p] = tmp1;
                        Nshuffle = lstTestOptions.Count - 1;
                        break;
                        }
                    }
                //do shuffle
                int rnd = 0;
                for (int i = Nshuffle - 1; i > 0; i--)
                    {
                    int j = random.Next (0, i + 1);
                    var temp = lstTestOptions [i];
                    lstTestOptions [i] = lstTestOptions [j];
                    lstTestOptions [j] = temp;
                    }
                return lstTestOptions;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in BEService C05: Create_TestOptionsAsync: " + ex);
                return new List<TestOption> ();
                }
            }
        public async Task<TestOption> Read_TestOptionAsync (int testOptionId, SqlConnection cnn)
            {
            TestOption testOption = new TestOption ();
            string sql = "SELECT TestOptionId, TestId, TestOptionTitle, TestOptionTags FROM TestOptions WHERE TestOptionId=@testoptionid";
            try
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@testoptionid", testOptionId.ToString ());
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        testOption.TestOptionId = reader.GetInt32 (0);
                        testOption.TestId = reader.GetInt32 (1);
                        testOption.TestOptionTitle = reader.GetString (2);
                        testOption.TestOptionTags = reader.GetInt32 (3);
                        }
                    return testOption;
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in BEService C05: Create_TestOptionsAsync: " + ex);
                return new TestOption ();
                }
            }
        public async Task<bool> Update_TestOptionAsync (TestOption testOption)
            {
            string sql = "UPDATE TestOptions SET TestId=@testid, TestOptionTitle=@testoptiontitle, TestOptionTags=@testoptiontags WHERE TestOptionId=@testoptionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testOption.TestId);
            cmd.Parameters.AddWithValue ("@testoptiontitle", testOption.TestOptionTitle);
            cmd.Parameters.AddWithValue ("@testoptiontags", testOption.TestOptionTags);
            cmd.Parameters.AddWithValue ("@testoptionid", testOption.TestOptionId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_TestOptionsAsync (int testId)
            {
            string sql = "DELETE FROM TestOptions WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testoptionid", testId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_TestOptionAsync (int testOptionId)
            {
            string sql = "DELETE FROM TestOptions WHERE TestOptionId=@testoptionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testoptionid", testOptionId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region C06:Exams
        public async Task<int> Create_ExamAsync (Exam exam)
            {
            string sql = "INSERT INTO Exams (CourseId, ExamTitle, ExamDateTime, ExamDuration, ExamNTests, ExamTags) VALUES (@courseid, @examtitle, @examdatetime, @examduration, @examntests, @examtags); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", exam.CourseId);
            cmd.Parameters.AddWithValue ("@examtitle", exam.ExamTitle);
            cmd.Parameters.AddWithValue ("@examdatetime", exam.ExamDateTime);
            cmd.Parameters.AddWithValue ("@examduration", exam.ExamDuration);
            cmd.Parameters.AddWithValue ("@examntests", exam.ExamNTests);
            cmd.Parameters.AddWithValue ("@examtags", exam.ExamTags);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<Exam>> Read_ExamsAsync (int courseId)
            {
            List<Exam> lstExams = new List<Exam> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamId, CourseId, ExamTitle, ExamDateTime, ExamDuration, ExamNTests, ExamTags FROM Exams WHERE CourseID=@courseid", cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstExams.Add (new Exam
                    {
                    ExamId = reader.GetInt32 (0),
                    CourseId = reader.GetInt32 (1),
                    ExamTitle = reader.GetString (2),
                    ExamDateTime = reader.GetString (3),
                    ExamDuration = reader.GetInt32 (4),
                    ExamNTests = reader.GetInt32 (5),
                    ExamTags = reader.GetInt32 (6)
                    });
                }
            return lstExams;
            }
        public async Task<Exam> Read_ExamAsync (int examId)
            {
            Exam exam = new Exam ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamId, CourseId, ExamTitle, ExamDateTime, ExamDuration, ExamNTests, ExamTags FROM Exams WHERE ExamId=@examid", cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                exam.ExamId = reader.GetInt32 (0);
                exam.CourseId = reader.GetInt32 (1);
                exam.ExamTitle = reader.GetString (2);
                exam.ExamDateTime = reader.GetString (3);
                exam.ExamDuration = reader.GetInt32 (4);
                exam.ExamNTests = reader.GetInt32 (5);
                exam.ExamTags = reader.GetInt32 (6);
                }
            return exam;
            }
        public async Task<bool> Update_ExamAsync (Exam exam)
            {
            string sql = "UPDATE Exams SET CourseId=@courseid, ExamTitle=@examtitle, ExamDateTime=@examdatetime, ExamDuration=@examduration, ExamNTests= @examntests, ExamTags=@examtags WHERE ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", exam.CourseId);
            cmd.Parameters.AddWithValue ("@examtitle", exam.ExamTitle);
            cmd.Parameters.AddWithValue ("@examdatetime", exam.ExamDateTime);
            cmd.Parameters.AddWithValue ("@examduration", exam.ExamDuration);
            cmd.Parameters.AddWithValue ("@examntests", exam.ExamNTests);
            cmd.Parameters.AddWithValue ("@examtags", exam.ExamTags);
            cmd.Parameters.AddWithValue ("@examid", exam.ExamId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamsAsync (int courseId)
            {
            string sql = @"BEGIN TRANSACTION;
WITH ExamIds AS (SELECT ExamId FROM Exams WHERE CourseId=@courseid)
DELETE FROM ExamCompositions WHERE ExamId IN (SELECT ExamId FROM ExamIds); 
DELETE FROM ExamTests WHERE ExamId IN (SELECT ExamId FROM ExamIds);
DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM ExamIds)); 
DELETE FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM ExamIds); 
DELETE FROM Exams WHERE ExamId IN (SELECT ExamId From ExamIds);
COMMIT TRANSACTION;
";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamAsync (int examId)
            {
            string sql = @"BEGIN TRANSACTION;
DELETE FROM Exams WHERE ExamId=@examid;
DELETE FROM ExamCompositions WHERE ExamId=@examid; 
DELETE FROM ExamTests WHERE ExamId=@examid; 
DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE ExamId=@examid); 
DELETE FROM StudentExams WHERE ExamId=@examid; 
COMMIT TRANSACTION;
";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region C07:ExamCompositions
        public async Task<int> Create_ExamCompositionAsync (ExamComposition examComposition)
            {
            string sql = "INSERT INTO ExamCompositions (ExamId, TopicId, TopicNTests, TestsLevel) VALUES (@examid, @topicid, @topicntests, @testslevel); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examComposition.ExamId);
            cmd.Parameters.AddWithValue ("@topicid", examComposition.TopicId);
            cmd.Parameters.AddWithValue ("@topicntests", examComposition.TopicNTests);
            cmd.Parameters.AddWithValue ("@testslevel", examComposition.TestsLevel);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<ExamComposition>> Read_ExamCompositionsAsync (int examId)
            {
            List<ExamComposition> lstExamComposition = new List<ExamComposition> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamCompositionId, ExamId, TopicId, TopicNTests, TestsLevel FROM ExamCompositions WHERE ExamId=@examid", cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstExamComposition.Add (new ExamComposition
                    {
                    ExamCompositionId = reader.GetInt32 (0),
                    ExamId = reader.GetInt32 (1),
                    TopicId = reader.GetInt32 (2),
                    TopicNTests = reader.GetInt32 (3),
                    TestsLevel = reader.GetInt32 (4)
                    });
                }
            return lstExamComposition;
            }
        public async Task<ExamComposition> Read_ExamCompositionAsync (int examCompositionId)
            {
            ExamComposition examComposition = new ExamComposition ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamCompositionId, ExamId, TopicId, TopicNTests, TestsLevel FROM ExamCompositions WHERE ExamCompositionId=@examcompositionid", cnn);
            cmd.Parameters.AddWithValue ("@examcompositionid", examCompositionId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                examComposition.ExamCompositionId = reader.GetInt32 (0);
                examComposition.ExamId = reader.GetInt32 (1);
                examComposition.TopicId = reader.GetInt32 (2);
                examComposition.TopicNTests = reader.GetInt32 (3);
                examComposition.TestsLevel = reader.GetInt32 (4);
                }
            return examComposition;
            }
        public async Task<bool> Update_ExamCompositionAsync (ExamComposition examComposition)
            {
            string sql = "UPDATE ExamCompositions SET ExamId=@examid, TopicId=@topicid, TopicNTests=@topicntests, TestsLevel=@testslevel WHERE ExamCompositionId=@examcompositionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examComposition.ExamId);
            cmd.Parameters.AddWithValue ("@topicid", examComposition.TopicId);
            cmd.Parameters.AddWithValue ("@topicntests", examComposition.TopicNTests);
            cmd.Parameters.AddWithValue ("@testslevel", examComposition.TestsLevel);
            cmd.Parameters.AddWithValue ("@examcompositionid", examComposition.ExamCompositionId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamCompositionsAsync (int examId)
            {
            string sql = "DELETE FROM ExamCompositions WHERE ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamCompositionAsync (int examCompositionId)
            {
            string sql = "DELETE FROM ExamCompositions WHERE ExamCompositionId=@examcompositionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examcompositionid", examCompositionId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        #endregion
        #region C08:ExamTests
        public async Task<int> Create_ExamTestsByExamCompositionAsync (ExamComposition examComposition)
            {
            List<int> lstTestIds = new List<int> ();
            string sql = "SELECT Top (@ntests) TestId From Tests WHERE TopicId=@topicid ORDER BY NEWID()";
            string? connString = _config.GetConnectionString ("cnni");
            using (SqlConnection cnn = new (connString))
                {
                using SqlCommand cmd = new (sql, cnn);
                cmd.Parameters.AddWithValue ("@ntests", examComposition.TopicNTests);
                cmd.Parameters.AddWithValue ("@topicid", examComposition.TopicId);
                await cnn.OpenAsync ();
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstTestIds.Add (reader.GetInt32 (0));
                    }
                }
            ;
            ExamTest examTest = new ExamTest { ExamId = examComposition.ExamId, TestId = 0, PercentCorrect = 0, PercentIncorrect = 0, PercentHelped = 0 };
            foreach (int testId in lstTestIds)
                {
                examTest.TestId = testId;
                await Create_ExamTestAsync (examTest);
                }
            return 1;
            }
        public async Task<int> Create_ExamTestAsync (ExamTest examTest)
            {
            string sql = "INSERT INTO ExamTests (ExamId, TestId, PercentCorrect, PercentIncorrect, PercentHelped) VALUES (@examid, @testid, @percentcorrect, @percentincorrect, @percenthelped); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examTest.ExamId);
            cmd.Parameters.AddWithValue ("@testid", examTest.TestId);
            cmd.Parameters.AddWithValue ("@percentcorrect", examTest.PercentCorrect);
            cmd.Parameters.AddWithValue ("@percentincorrect", examTest.PercentIncorrect);
            cmd.Parameters.AddWithValue ("@percenthelped", examTest.PercentHelped);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<ExamTest>> Read_ExamTestsAsync (int examId)
            {
            List<ExamTest> lstExamTests = new List<ExamTest> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamTestId, ExamId, TestId, PercentCorrect, PercentIncorrect, PercentHelped FROM ExamTests WHERE ExamId=@examid", cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstExamTests.Add (new ExamTest
                    {
                    ExamTestId = reader.GetInt32 (0),
                    ExamId = reader.GetInt32 (1),
                    TestId = reader.GetInt32 (2),
                    PercentCorrect = reader.GetInt32 (3),
                    PercentIncorrect = reader.GetInt32 (4),
                    PercentHelped = reader.GetInt32 (5)
                    });
                }
            return lstExamTests;
            }
        public async Task<ExamTest> Read_ExamTestAsync (int examTestId)
            {
            ExamTest examTest = new ExamTest ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamTestId, ExamId, TestId, PercentCorrect, PercentIncorrect, PercentHelped FROM ExamTests WHERE ExamTestId=@examtestid", cnn);
            cmd.Parameters.AddWithValue ("@examtestid", examTestId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                examTest.ExamTestId = reader.GetInt32 (0);
                examTest.ExamId = reader.GetInt32 (1);
                examTest.TestId = reader.GetInt32 (2);
                examTest.PercentCorrect = reader.GetInt32 (3);
                examTest.PercentIncorrect = reader.GetInt32 (4);
                examTest.PercentHelped = reader.GetInt32 (5);
                }
            return examTest;
            }
        public async Task<bool> Update_ExamTestAsync (ExamTest examTest)
            {
            string sql = "UPDATE ExamTests SET ExamId=@examid, TestId=@testid, PercentCorrect=@percentcorrect, PercentIncorrect=@percentincorrect, PercentHelped=@percenthelped";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examTest.ExamId);
            cmd.Parameters.AddWithValue ("@testid", examTest.TestId);
            cmd.Parameters.AddWithValue ("@percentcorrect", examTest.PercentCorrect);
            cmd.Parameters.AddWithValue ("@percentincorrect", examTest.PercentIncorrect);
            cmd.Parameters.AddWithValue ("@percenthelped", examTest.PercentHelped);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamTestsAsync (int examId)
            {
            string sql = "DELETE FROM ExamTests WHERE ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@exaid", examId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamTestAsync (ExamTest examTest)
            {
            string sql = "DELETE FROM ExamTests WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", examTest.TestId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        #endregion
        #region C09:Groups
        public async Task<int> Create_GroupAsync (Group group)
            {
            string sql = "INSERT INTO Groups (GroupName, UserId) VALUES (@groupname, @userid); SELECT CAST (scope_identity() AS int)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupname", group.GroupName);
                cmd.Parameters.AddWithValue ("@userid", group.UserId);
                int i = (int) await cmd.ExecuteScalarAsync ();
                return i;
                }
            catch
                {
                await cnn.CloseAsync ();
                return 0;
                }
            }
        public async Task<List<Group>> Read_GroupsAsync (User user, bool getStudentExams, bool getStudentCourses)
            {
            List<Group> lstGroups = new List<Group> ();
            string sql = "SELECT g.GroupId, g.GroupName, g.UserId FROM Groups g WHERE g.UserId=@userid ORDER BY g.GroupName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@userid", user.UserId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        var group = new Group
                            {
                            GroupId = reader.GetInt32 (0),
                            GroupName = reader.GetString (1),
                            UserId = reader.GetInt32 (2),
                            Students = new List<User> ()
                            };
                        group.Students = await Read_StudentsByGroupIdAsync (group.GroupId, getStudentExams, getStudentCourses);
                        lstGroups.Add (group);
                        }
                    }
                return lstGroups;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new List<Group> ();
                }
            }
        public async Task<Group> Read_GroupAsync (int groupId, bool getStudentExams, bool getStudentCourses)
            {
            //ModelState.Clear (); // disables auto validation
            Group group = new Group ();
            string sql = "SELECT g.GroupId, g.GroupName, g.UserId FROM Groups g WHERE g.GroupId=@groupid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupid", groupId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        group.GroupId = reader.GetInt32 (0);
                        group.GroupName = reader.GetString (1);
                        group.UserId = reader.GetInt32 (2);
                        group.Students = new List<User> ();
                        }
                    }
                group.Students = await Read_StudentsByGroupIdAsync (groupId, getStudentExams, getStudentCourses);
                return group;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new Group ();
                }
            }
        public async Task<bool> Update_GroupAsync (Group group)
            {
            string sql = "UPDATE Groups SET GroupName=@groupname WHERE GroupId=@groupid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupname", group.GroupName);
                cmd.Parameters.AddWithValue ("@groupid", group.GroupId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch
                {
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<bool> Delete_GroupsAsync (User user)
            {
            string sql = "DELETE FROM Groups WHERE UserId=@userid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", user.UserId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<int> Delete_GroupAsync (int groupId)
            {
            string sql = "DELETE FROM Groups WHERE (GroupId=@groupid AND NOT EXISTS (SELECT 1 FROM Students WHERE GroupId=@groupid)); SELECT COUNT (StudentId) FROM Students WHERE GroupId=@groupid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@groupid", groupId);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        #endregion
        #region C11:StudentExams
        public async Task<int> Create_StudentExamsAsync (StudentExam studentExam, int groupId)
            {
            List<int> lstStudentIds = new List<int> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "SELECT StudentId From Students s WHERE s.GroupId=@groupid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@groupid", groupId);
            var reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstStudentIds.Add (reader.GetInt32 (0));
                }
            foreach (int studentId in lstStudentIds)
                {
                studentExam.StudentId = studentId;
                await Create_StudentExamAsync (studentExam);
                }
            return 1;
            }
        public async Task<int> Create_StudentExamAsync (StudentExam studentExam)
            {
            Random random = new Random ();
            //1 Add record to: StudentExams
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "INSERT INTO StudentExams (StudentId, ExamId, StartDateTime, FinishDateTime, StudentExamTags, StudentExamPoint) VALUES (@studentid, @examid, @startdatetime, @finishdatetime, @studentexamtags, @studentexampoint); SELECT CAST (scope_identity() AS int)";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExam.StudentId);
            cmd.Parameters.AddWithValue ("@examid", studentExam.ExamId);
            cmd.Parameters.AddWithValue ("@startdatetime", "");
            cmd.Parameters.AddWithValue ("@finishdatetime", "");
            cmd.Parameters.AddWithValue ("@studentexamtags", 0);
            cmd.Parameters.AddWithValue ("@studentexampoint", 0);
            int newStudentExamId = (int) await cmd.ExecuteScalarAsync ();
            //2 read exam tests
            List<Test> lstExamTests = new List<Test> ();
            lstExamTests = await Read_TestsByExamIdAsync (studentExam.ExamId, true);
            //3 shuffle tests into: lstExamTests
            int rnd = 0;
            for (int k = 0; k < lstExamTests.Count; k++)
                {
                rnd = random.Next (k, (lstExamTests.Count));
                Test tmpTest = lstExamTests [rnd];
                lstExamTests [rnd] = lstExamTests [k];
                lstExamTests [k] = tmpTest;
                }
            //4 options
            foreach (Test tst in lstExamTests)
                {
                StudentExamTest est = new StudentExamTest ();
                //NOTICE: If Read_TestOptions is called multiple times in rapid succession (as it is inside the loop over lstExamTests), the Random constructor will use the same seed (based on system time), resulting in identical shuffles of lstTestOptions.
                //So even though you're calling Read_TestOptions(testId) for different tests, the shuffled list ends up in the same order, and the key option (tag 2) is always in the same position, likely the first one found.
                //to fix, use a shared Random instance: pass a single Random object to Read_TestOptions
                await Read_TestOptionsAsync (tst.TestId, cnn);
                est.StudentId = studentExam.StudentId;
                est.StudentExamId = newStudentExamId;
                est.TestId = tst.TestId;
                est.Opt1Id = (tst.TestOptions.Count > 0) ? (tst.TestOptions [0].TestOptionId) : 0;
                est.Opt2Id = (tst.TestOptions.Count > 1) ? (tst.TestOptions [1].TestOptionId) : 0;
                est.Opt3Id = (tst.TestOptions.Count > 2) ? (tst.TestOptions [2].TestOptionId) : 0;
                est.Opt4Id = (tst.TestOptions.Count > 3) ? (tst.TestOptions [3].TestOptionId) : 0;
                est.Opt5Id = (tst.TestOptions.Count > 4) ? (tst.TestOptions [4].TestOptionId) : 0;
                foreach (TestOption opt in tst.TestOptions)
                    {
                    if ((opt.TestOptionTags & 2) == 2)
                        {
                        est.StudentExamTestKey = opt.TestOptionId;
                        }
                    }
                //insert into ExamSheets
                string sql2 = "INSERT INTO StudentExamTests (StudentExamId, TestId, Opt1Id, Opt2Id, Opt3Id, Opt4Id, Opt5Id, StudentExamTestKey, StudentExamTestAns, StudentExamTestTags) VALUES (@studentexamid, @testid, @opt1id, @opt2id, @opt3id, @opt4id, @opt5id, @key, 0, 0)";
                var cmd2 = new Microsoft.Data.SqlClient.SqlCommand (sql2, cnn);
                cmd2.CommandType = CommandType.Text;
                cmd2.Parameters.AddWithValue ("@studentexamid", newStudentExamId);
                cmd2.Parameters.AddWithValue ("@testid", est.TestId);
                cmd2.Parameters.AddWithValue ("@opt1id", est.Opt1Id);
                cmd2.Parameters.AddWithValue ("@opt2id", est.Opt2Id);
                cmd2.Parameters.AddWithValue ("@opt3id", est.Opt3Id);
                cmd2.Parameters.AddWithValue ("@opt4id", est.Opt4Id);
                cmd2.Parameters.AddWithValue ("@opt5id", est.Opt5Id);
                cmd2.Parameters.AddWithValue ("@key", est.StudentExamTestKey);
                await cmd2.ExecuteNonQueryAsync ();
                }
            return newStudentExamId;
            }
        public async Task<List<StudentExam>> Read_StudentExamsAsync (int studentId, bool readInactiveExams)
            {
            int i = 0;
            List<StudentExam> lstStudentExams = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT se.StudentExamId, c.CourseId, c.CourseName,
                e.ExamId, e.ExamTitle, e.ExamDateTime, e.ExamDuration, e.ExamNTests, e.ExamTags,
                se.StartDateTime, se.FinishDateTime, se.StudentExamTags, se.StudentExamPoint
                FROM StudentExams se 
                INNER JOIN Exams e ON se.ExamId = e.ExamId
                INNER JOIN Courses c ON e.CourseId = c.CourseId
                WHERE se.StudentId=@studentid ";
            if (!readInactiveExams)
                {
                sql += " AND (e.ExamTags & 1) = 1";
                }
            sql += " ORDER BY e.ExamDateTime";
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", studentId.ToString ());
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    i++;
                    var exam = new StudentExam ();
                    exam.StudentExamId = reader.GetInt32 (0);
                    exam.StudentId = studentId;
                    exam.CourseId = reader.GetInt32 (1);
                    exam.CourseName = reader.GetString (2);
                    exam.ExamId = reader.GetInt32 (3);
                    exam.ExamIndex = i;
                    exam.ExamTitle = reader.GetString (4);
                    exam.ExamDateTime = reader.GetString (5);
                    exam.ExamDuration = reader.GetInt32 (6);
                    exam.ExamNTests = reader.GetInt32 (7);
                    exam.ExamTags = reader.GetInt32 (8);
                    exam.StartDateTime = reader.GetString (9);
                    exam.FinishDateTime = reader.GetString (10);
                    exam.StudentExamTags = reader.GetInt32 (11);
                    exam.StudentExamPoint = reader.GetDouble (12);
                    lstStudentExams.Add (exam);
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("in API StudentExamsController : \n" + ex.ToString ());
                }
            await cnn.CloseAsync ();
            return lstStudentExams;
            }
        public async Task<StudentExam> Read_StudentExamAsync (int studentExamId, bool readInactiveExams)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT TOP 1 se.StudentExamId, se.StudentId, c.CourseId, c.CourseName,
                 e.ExamId, e.ExamTitle, e.ExamDateTime, e.ExamDuration, e.ExamNTests, e.ExamTags,
                 se.StartDateTime, se.FinishDateTime, se.StudentExamTags, se.StudentExamPoint
                 FROM StudentExams se 
                 INNER JOIN Exams e ON se.ExamId = e.ExamId
                 INNER JOIN Courses c ON e.CourseId = c.CourseId
                 WHERE se.StudentExamId=@studentexamid ";
            if (!readInactiveExams)
                {
                sql += " AND (e.ExamTags & 1) = 1";
                }
            sql += " ORDER BY e.ExamDateTime";
            try
                {
                var exam = new StudentExam ();
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        exam.StudentExamId = reader.GetInt32 (0);
                        exam.StudentId = reader.GetInt32 (1);
                        exam.CourseId = reader.GetInt32 (2);
                        exam.CourseName = reader.GetString (3);
                        exam.ExamId = reader.GetInt32 (4);
                        exam.ExamIndex = 0;
                        exam.ExamTitle = reader.GetString (5);
                        exam.ExamDateTime = reader.GetString (6);
                        exam.ExamDuration = reader.GetInt32 (7);
                        exam.ExamNTests = reader.GetInt32 (8);
                        exam.ExamTags = reader.GetInt32 (9);
                        exam.StartDateTime = reader.GetString (10);
                        exam.FinishDateTime = reader.GetString (11);
                        exam.StudentExamTags = reader.GetInt32 (12);
                        exam.StudentExamPoint = reader.GetDouble (13);
                        exam.StudentExamTests = new List<StudentExamTest> ();
                        }
                    }
                exam.StudentExamTests = await Read_StudentExamTestsAsync (exam.StudentExamId, true, cnn);
                return exam;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("BeService: Read_StudentExam ERROR: \n" + ex.ToString ());
                }
            return new StudentExam ();
            }
        public async Task<bool> Update_StudentExamAsync (StudentExam studentExam)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "UPDATE StudentExams SET StudentId=@studentid, ExamId=@examid, StartDateTime=@startdatetime, FinishDateTime=@finishdatetime, StudentExamTags=@studentexamtags, StudentExamPoint=@studentexampoint";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExam.StudentId);
            cmd.Parameters.AddWithValue ("@examid", studentExam.ExamId);
            cmd.Parameters.AddWithValue ("@startdatetime", studentExam.StartDateTime);
            cmd.Parameters.AddWithValue ("@finishdatetime", studentExam.FinishDateTime);
            cmd.Parameters.AddWithValue ("@studentexamtags", studentExam.ExamTags);
            cmd.Parameters.AddWithValue ("@studentexampoint", studentExam.StudentExamPoint);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Update_StudentExamTagsAsync (StudentExam tempStudentExam)
            {
            //Update tags of an exam for a student
            string sql = "UPDATE StudentExams SET StudentExamTags=@studentexamtags, StartDateTime=@startdatetime, FinishDateTime=@finishdatetime WHERE StudentExamId=@studentexamid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentexamtags", tempStudentExam.StudentExamTags);
                cmd.Parameters.AddWithValue ("@startdatetime", tempStudentExam.StartDateTime);
                cmd.Parameters.AddWithValue ("@finishdatetime", tempStudentExam.FinishDateTime);
                cmd.Parameters.AddWithValue ("@studentexamid", tempStudentExam.StudentExamId);
                int c = cmd.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                if (tempStudentExam.FinishDateTime.Length > 0)
                    {
                    bool result = await CalculatePoints_StudentExamsAsync (tempStudentExam.StudentExamId);
                    }
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in: SetTagStarted \n" + ex.ToString ());
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<bool> Delete_StudentExamsByStudentIdAsync (int studentId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExams WHERE StudentId=@studentid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_StudentExamsByExamIdAsync (int examId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExams WHERE Exam=@examid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_StudentExamAsync (int studentExamId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExams WHERE StudentExamId=@studentexamid; DELETE FROM StudentExamTests WHERE StudentExamId=@studentexamid;";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> CalculatePoints_StudentExamsAsync (int studentexamid)
            {
            Console.WriteLine ("Starting CALC...");
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                string sql1 = "SELECT Count (est.StudentExamTestKey) AS t FROM StudentExamTests est INNER JOIN StudentExams se ON est.StudentExamId=se.StudentExamId WHERE est.StudentExamId=@studentexamid";
                await cnn.OpenAsync ();
                var cmdx1 = new SqlCommand (sql1, cnn);
                cmdx1.Parameters.AddWithValue ("@studentexamid", studentexamid);
                int n = (int) cmdx1.ExecuteScalar ();
                string sql2 = "SELECT Count (est.StudentExamTestKey) AS m FROM StudentExamTests est INNER JOIN StudentExams se ON est.StudentExamId=se.StudentExamId WHERE est.StudentExamId=@studentexamid AND StudentExamTestKey=StudentExamTestAns";
                var cmdx2 = new SqlCommand (sql2, cnn);
                cmdx2.Parameters.AddWithValue ("@studentexamid", studentexamid);
                int c = (int) cmdx2.ExecuteScalar ();
                double p = (Math.Abs (2000 * ((1.0 * c) / (1.0 * n))) / 100);
                string sql3 = "UPDATE StudentExams SET StudentExamPoint=@point WHERE StudentExamId=@studentexamid";
                var cmd3 = new SqlCommand (sql3, cnn);
                cmd3.CommandType = CommandType.Text;
                cmd3.Parameters.AddWithValue ("@point", p.ToString ());
                cmd3.Parameters.AddWithValue ("@studentexamid", studentexamid);
                int x = cmd3.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                Console.WriteLine ("n / c / p :" + n + "  -  " + c + "  -  " + p);
                return true;
                }
            catch (Exception ex)
                {
                await cnn.CloseAsync ();
                Console.WriteLine ("CALAC ERROR: \n" + ex.ToString ());
                return false;
                }
            }
        #endregion
        #region C12:StudentExamTests
        public async Task<int> Create_StudentExamTestAsync (StudentExamTest studentExamTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "INSERT INTO StudentExamTests (StudentId, StudentExamId, TestId, Opt1Id, Opt2Id, Opt3Id, Opt4Id, Opt5Id, StudentExamTestKey, StudentExamTestAns, StudentExamTestTags) VALUES (@studentid, @studentexamid, @testid, @opt1id, @opt2id, @opt3id, @opt4id, @opt5id, @StudentExamTestkey, @StudentExamTestans, @StudentExamTesttags); SELECT CAST (scope_identity() AS int)";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExamTest.StudentId);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamTest.StudentExamId);
            cmd.Parameters.AddWithValue ("@testid", studentExamTest.TestId);
            cmd.Parameters.AddWithValue ("@opt1id", studentExamTest.Opt1Id);
            cmd.Parameters.AddWithValue ("@opt2id", studentExamTest.Opt2Id);
            cmd.Parameters.AddWithValue ("@opt3id", studentExamTest.Opt3Id);
            cmd.Parameters.AddWithValue ("@opt4id", studentExamTest.Opt4Id);
            cmd.Parameters.AddWithValue ("@opt5id", studentExamTest.Opt5Id);
            cmd.Parameters.AddWithValue ("@StudentExamTestkey", studentExamTest.StudentExamTestKey);
            cmd.Parameters.AddWithValue ("@StudentExamTestans", studentExamTest.StudentExamTestAns);
            cmd.Parameters.AddWithValue ("@StudentExamTesttags", studentExamTest.StudentExamTestTags);
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<StudentExamTest>> Read_StudentExamTestsAsync (int studentExamId, bool readOptions)
            {
            //this [overload] has its own cnn
            List<StudentExamTest> lstStudentExamTests = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.StudentExamId=@studentexamid";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        lstStudentExamTests.Add (new StudentExamTest
                            {
                            StudentExamTestId = reader.GetInt64 (0),
                            StudentId = reader.GetInt32 (1),
                            StudentExamId = reader.GetInt32 (2),
                            TestId = reader.GetInt32 (3),
                            TestTitle = reader.GetString (4),
                            TestType = reader.GetInt32 (5),
                            CourseTopicId = reader.GetInt32 (6),
                            CourseTopicTitle = reader.GetString (7),
                            TestTags = reader.GetInt32 (8),
                            TestLevel = reader.GetInt32 (9),
                            Opt1Id = reader.GetInt32 (10),
                            Opt2Id = reader.GetInt32 (11),
                            Opt3Id = reader.GetInt32 (12),
                            Opt4Id = reader.GetInt32 (13),
                            Opt5Id = reader.GetInt32 (14),
                            StudentExamTestKey = reader.GetInt32 (15),
                            StudentExamTestAns = reader.GetInt32 (16),
                            StudentExamTestTags = reader.GetInt32 (17),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            });
                        }
                    }
                if (readOptions)
                    {
                    foreach (StudentExamTest t in lstStudentExamTests)
                        {
                        int [] optIds = { t.Opt1Id, t.Opt2Id, t.Opt3Id, t.Opt4Id, t.Opt5Id };
                        for (int j = 0; j < t.TestType && j < 5; j++)
                            {
                            t.TestOptions.Add (await Read_TestOptionAsync (optIds [j], cnn));
                            }
                        }
                    }
                return lstStudentExamTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentExamTest> ();
                }
            }
        public async Task<List<StudentExamTest>> Read_StudentExamTestsAsync (int studentExamId, bool readOptions, SqlConnection cnn)
            {
            //this [overload] is called from another method with an open cnn (see method parameters)
            List<StudentExamTest> lstStudentExamTests = new ();
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.StudentExamId=@studentexamid";
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        var est = new StudentExamTest
                            {
                            StudentExamTestId = reader.GetInt64 (0),
                            StudentId = reader.GetInt32 (1),
                            StudentExamId = reader.GetInt32 (2),
                            TestId = reader.GetInt32 (3),
                            TestTitle = reader.GetString (4),
                            TestType = reader.GetInt32 (5),
                            CourseTopicId = reader.GetInt32 (6),
                            CourseTopicTitle = reader.GetString (7),
                            TestTags = reader.GetInt32 (8),
                            TestLevel = reader.GetInt32 (9),
                            Opt1Id = reader.GetInt32 (10),
                            Opt2Id = reader.GetInt32 (11),
                            Opt3Id = reader.GetInt32 (12),
                            Opt4Id = reader.GetInt32 (13),
                            Opt5Id = reader.GetInt32 (14),
                            StudentExamTestKey = reader.GetInt32 (15),
                            StudentExamTestAns = reader.GetInt32 (16),
                            StudentExamTestTags = reader.GetInt32 (17),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            };
                        lstStudentExamTests.Add (est);
                        }
                    }
                if (readOptions)
                    {
                    foreach (StudentExamTest t in lstStudentExamTests)
                        {
                        int [] optIds = { t.Opt1Id, t.Opt2Id, t.Opt3Id, t.Opt4Id, t.Opt5Id };
                        for (int j = 0; j < t.TestType && j < 5; j++)
                            {
                            t.TestOptions.Add (await Read_TestOptionAsync (optIds [j], cnn));
                            }
                        }
                    }
                return lstStudentExamTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentExamTest> ();
                }
            }
        public async Task<StudentExamTest> Read_StudentExamTestAsync (long studentExamTestId, bool readOptions)
            {
            StudentExamTest studentExamTest = new StudentExamTest ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.StudentExamTestId=@studentexamtestid";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamtestid", studentExamTestId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        studentExamTest.StudentExamTestId = reader.GetInt64 (0);
                        studentExamTest.StudentId = reader.GetInt32 (1);
                        studentExamTest.StudentExamId = reader.GetInt32 (2);
                        studentExamTest.TestId = reader.GetInt32 (3);
                        studentExamTest.TestTitle = reader.GetString (4);
                        studentExamTest.TestType = reader.GetInt32 (5);
                        studentExamTest.CourseTopicId = reader.GetInt32 (6);
                        studentExamTest.CourseTopicTitle = reader.GetString (7);
                        studentExamTest.TestTags = reader.GetInt32 (8);
                        studentExamTest.TestLevel = reader.GetInt32 (9);
                        studentExamTest.Opt1Id = reader.GetInt32 (10);
                        studentExamTest.Opt2Id = reader.GetInt32 (11);
                        studentExamTest.Opt3Id = reader.GetInt32 (12);
                        studentExamTest.Opt4Id = reader.GetInt32 (13);
                        studentExamTest.Opt5Id = reader.GetInt32 (14);
                        studentExamTest.StudentExamTestKey = reader.GetInt32 (15);
                        studentExamTest.StudentExamTestAns = reader.GetInt32 (16);
                        studentExamTest.StudentExamTestTags = reader.GetInt32 (17);
                        studentExamTest.TestIndex = ++index;
                        studentExamTest.TestOptions = new List<TestOption> ();
                        }
                    }
                if (readOptions)
                    {
                    int [] optIds = { studentExamTest.Opt1Id, studentExamTest.Opt2Id, studentExamTest.Opt3Id, studentExamTest.Opt4Id, studentExamTest.Opt5Id };
                    for (int j = 0; j < studentExamTest.TestType && j < 5; j++)
                        {
                        studentExamTest.TestOptions.Add (await Read_TestOptionAsync (optIds [j], cnn));
                        }
                    }
                return studentExamTest;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new StudentExamTest ();
                }
            }
        public async Task<List<StudentExamTest>> Read_StudentsExamTestAsync (StudentExamTest studentExamTest)
            {
            //this [overload] has its own cnn
            List<StudentExamTest> lstStudentExamTests = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.TestId=@testid AND se.ExamId IN (SELECT ExamId FROM StudentExams WHERE StudentExamId=@studentexamid)";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", studentExamTest.TestId);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamTest.StudentExamId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        lstStudentExamTests.Add (new StudentExamTest
                            {
                            StudentExamTestId = reader.GetInt64 (0),
                            StudentId = reader.GetInt32 (1),
                            StudentExamId = reader.GetInt32 (2),
                            TestId = reader.GetInt32 (3),
                            TestTitle = reader.GetString (4),
                            TestType = reader.GetInt32 (5),
                            CourseTopicId = reader.GetInt32 (6),
                            CourseTopicTitle = reader.GetString (7),
                            TestTags = reader.GetInt32 (8),
                            TestLevel = reader.GetInt32 (9),
                            Opt1Id = reader.GetInt32 (10),
                            Opt2Id = reader.GetInt32 (11),
                            Opt3Id = reader.GetInt32 (12),
                            Opt4Id = reader.GetInt32 (13),
                            Opt5Id = reader.GetInt32 (14),
                            StudentExamTestKey = reader.GetInt32 (15),
                            StudentExamTestAns = reader.GetInt32 (16),
                            StudentExamTestTags = reader.GetInt32 (17),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            });
                        }
                    }
                return lstStudentExamTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentExamTest> ();
                }
            }
        public async Task<bool> Update_StudentExamTestAsync (StudentExamTest studentExamTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "UPDATE StudentExamTests SET StudentId=@studentid, StudentExamId= @studentexamid, TestId=@testid, Opt1Id=@opt1id, Opt2Id= @opt2id, Opt3Id=@opt3id, Opt4Id=@opt4id, Opt5Id=@opt5id, StudentExamTestKey=@StudentExamTestkey, StudentExamTestAns=@StudentExamTestans, StudentExamTestTags=@StudentExamTesttags";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExamTest.StudentId);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamTest.StudentExamId);
            cmd.Parameters.AddWithValue ("@testid", studentExamTest.TestId);
            cmd.Parameters.AddWithValue ("@opt1id", studentExamTest.Opt1Id);
            cmd.Parameters.AddWithValue ("@opt2id", studentExamTest.Opt2Id);
            cmd.Parameters.AddWithValue ("@opt3id", studentExamTest.Opt3Id);
            cmd.Parameters.AddWithValue ("@opt4id", studentExamTest.Opt4Id);
            cmd.Parameters.AddWithValue ("@opt5id", studentExamTest.Opt5Id);
            cmd.Parameters.AddWithValue ("@StudentExamTestkey", studentExamTest.StudentExamTestKey);
            cmd.Parameters.AddWithValue ("@StudentExamTestans", studentExamTest.StudentExamTestAns);
            cmd.Parameters.AddWithValue ("@StudentExamTesttags", studentExamTest.StudentExamTestTags);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Update_StudentExamTestsTagsAsync (StudentExamTest tempStudentExamTest)
            {
            //tags: 1:Visited 2:Bookmarked 4:Answered 8:Helped 16:Revised 32:Reported
            string sql = "UPDATE StudentExamTests SET StudentExamTestTags=@studentExamTestTags WHERE StudentExamId=@studentexamid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentExamTestTags", tempStudentExamTest.StudentExamTestTags);
                cmd.Parameters.AddWithValue ("@studentexamid", tempStudentExamTest.StudentExamId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTestTagsAsync (StudentExamTest tempStudentExamTest)
            {
            //tags: 1:Visited 2:Bookmarked 4:Answered 8:Helped 16:Revised 32:Reported
            string sql = "UPDATE StudentExamTests SET StudentExamTestTags=@studentExamTestTags WHERE StudentExamTestId=@StudentExamTestid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentExamTestTags", tempStudentExamTest.StudentExamTestTags);
                cmd.Parameters.AddWithValue ("@StudentExamTestid", tempStudentExamTest.StudentExamTestId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTestAnswerAsync (StudentExamTest tempStudentExamTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            try
                {
                if (tempStudentExamTest.StudentExamTestAns == 0)
                    {
                    sql = "UPDATE StudentExamTests SET StudentExamTestAns=@answ, StudentExamTestTags = (StudentExamTestTags & ~4) WHERE StudentExamTestId=@StudentExamTestid";
                    }
                else
                    {
                    sql = "UPDATE StudentExamTests SET StudentExamTestAns=@answ, StudentExamTestTags = (StudentExamTestTags | 4) WHERE StudentExamTestId=@StudentExamTestid";
                    }
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@answ", tempStudentExamTest.StudentExamTestAns);
                cmd.Parameters.AddWithValue ("@StudentExamTestid", tempStudentExamTest.StudentExamTestId);
                await cmd.ExecuteNonQueryAsync ();
                await cnn.CloseAsync ();
                return true;
                }
            catch (Exception ex)
                {
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<bool> Delete_StudentExamTestsAsync (int studentExamId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExamTests WHERE StudentExamId=@studentexamid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_StudentExamTestAsync (long studentExamTestId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExamTests WHERE StudentExamTestId=@StudentExamTestid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@StudentExamTestid", studentExamTestId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<string> CalculateStats_StudentExamTestsAsync (int examId, int testId)
            {
            string result = "";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "SELECT COUNT (se.StudentExamId) AS cnt FROM StudentExams se INNER JOIN StudentExamTests est ON se.StudentExamId=est.StudentExamId WHERE (se.ExamId=@examid) AND (est.TestId=@testid) AND ((se.StudentExamTags & 1) = 1)";
            string sql1 = sql + " AND (est.StudentExamTestKey = est.StudentExamTestAns)"; //with correct answer
            string sql2 = sql + " AND (est.TestId=@testid) AND (est.StudentExamTestKey != est.StudentExamTestAns) AND (est.StudentExamTestAns != 0)"; //has-answer (not ignored)
            string sql3 = sql + " AND (est.TestId=@testid) AND ((est.StudentExamTestTags & 8) = 8)"; //8:Helped
            await cnn.OpenAsync ();
            try
                {
                //sql-n
                var cmd4 = new SqlCommand (sql, cnn);
                cmd4.Parameters.AddWithValue ("@examid", examId);
                cmd4.Parameters.AddWithValue ("@testid", testId);
                int n = (int) cmd4.ExecuteScalar ();
                if (n == 0)
                    {
                    result = "nStudents = 0";
                    return result;
                    }
                //sql1
                var cmd1 = new SqlCommand (sql1, cnn);
                cmd1.Parameters.AddWithValue ("@examid", examId);
                cmd1.Parameters.AddWithValue ("@testid", testId);
                int c = (int) cmd1.ExecuteScalar ();
                //sql2
                var cmd2 = new SqlCommand (sql2, cnn);
                cmd2.Parameters.AddWithValue ("@examid", examId);
                cmd2.Parameters.AddWithValue ("@testid", testId);
                int w = (int) cmd2.ExecuteScalar ();
                //sql3
                var cmd3 = new SqlCommand (sql3, cnn);
                cmd3.Parameters.AddWithValue ("@examid", examId);
                cmd3.Parameters.AddWithValue ("@testid", testId);
                int h = (int) cmd3.ExecuteScalar ();
                if (n != 0)
                    {
                    result = $"result: {h}! +{c} -{w} ?{(n - c - w)} ={n} -- Percent: {(100 * c / n).ToString ("F2")}%";
                    }
                else
                    {
                    result = "";
                    }
                //double p = (Math.Abs (2000 * ((1.0 * c) / (1.0 * c))) / 100);
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                }
            return result;
            }
        #endregion
        #region C13:StudentCourses
        public async Task<bool> Create_StudentCoursesAsync (int groupId, int courseId)
            {
            string sql = @"INSERT INTO StudentCourses (StudentId, CourseId) SELECT s.StudentId, @courseid FROM Students s
                        WHERE s.GroupId=@groupid
                        AND NOT EXISTS (SELECT 1 FROM StudentCourses sc WHERE sc.StudentId=s.StudentId AND Sc.CourseId=@courseid);";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            cmd.Parameters.AddWithValue ("@groupid", groupId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Create_StudentCourseAsync (int studentId, int courseId)
            {
            string sql = "INSERT INTO StudentCourses (StudentId, CourseId) VALUES (@studentid, @courseid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<List<StudentCourse>> Read_StudentCoursesAsync (int studentid)
            {
            List<StudentCourse> lstStudentCourses = new List<StudentCourse> ();
            string sql = @"SELECT sc.StudentCourseId, sc.StudentId, sc.CourseId, c.CourseName, sc.NumberOfTests, sc.CorrectAnswers 
                        FROM StudentCourses sc INNER JOIN Courses c ON sc.CourseId=c.CourseId 
                        WHERE StudentId=@studentid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", studentid);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstStudentCourses.Add (new StudentCourse
                        {
                        StudentCourseId = reader.GetInt32 (0),
                        StudentId = reader.GetInt32 (1),
                        CourseId = reader.GetInt32 (2),
                        CourseName = reader.GetString (3),
                        NumberOfTests = reader.GetInt32 (4),
                        CorrectAnswers = reader.GetInt32 (5)
                        });
                    }
                await cnn.CloseAsync ();
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in C14 - GetStudentCouses: " + ex);
                await cnn.CloseAsync ();
                }
            return lstStudentCourses;
            }
        public async Task<StudentCourse> Read_StudentCourseAsync (int studentCourseId)
            {
            StudentCourse studentCourse = new StudentCourse ();
            string sql = "SELECT sc.StudentCourseId, sc.StudentId, sc.CourseId, c.CourseName, sc.NumberOfTests, sc.CorrectAnswers FROM StudentCourses sc INNER JOIN Courses c ON sc.CourseId=c.CourseId WHERE StudentCourseId=@studentcourseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        studentCourse.StudentCourseId = reader.GetInt32 (0);
                        studentCourse.StudentId = reader.GetInt32 (1);
                        studentCourse.CourseId = reader.GetInt32 (2);
                        studentCourse.CourseName = reader.GetString (3);
                        studentCourse.NumberOfTests = reader.GetInt32 (4);
                        studentCourse.CorrectAnswers = reader.GetInt32 (5);
                        }
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in C14 - GetStudentCouses: " + ex);
                await cnn.CloseAsync ();
                }
            return studentCourse;
            }
        public async Task<bool> Delete_StudentCoursesAsync (int courseId, int groupId)
            {
            bool result = true;
            List<User> lstStudents = new List<User> ();
            lstStudents = await Read_StudentsByGroupIdAsync (groupId, false, false);
            string sql = "DELETE FROM StudentCourses WHERE StudentId=@studentid AND CourseId=@courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            try
                {
                foreach (User student in lstStudents)
                    {
                    SqlCommand cmd = new SqlCommand (sql, cnn);
                    cmd.Parameters.AddWithValue ("@studentid", student.UserId);
                    cmd.Parameters.AddWithValue ("@courseid", courseId);
                    int i = cmd.ExecuteNonQuery ();
                    if (i < 0)
                        {
                        result = false;
                        }
                    }
                }
            catch (Exception ex)
                {
                result = false;
                }
            await cnn.CloseAsync ();
            return result;
            }
        public async Task<bool> Delete_StudentCourseAsync (int studentCourseId)
            {
            string sql = "DELETE FROM StudentCourses WHERE StudentCourseId=@studentcourseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_StudentCourseByStudentCourseIdAsync (int studentCourseId)
            {
            string sql = "DELETE FROM StudentCourses WHERE StudentCourseId=@studentcourseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> CalculatePoints_StudentCourseAsync (StudentCourse studentCourse)
            {
            try
                {
                string? connString = _config.GetConnectionString ("cnni");
                using SqlConnection cnn = new (connString);
                await cnn.OpenAsync ();
                //count nTests
                string sql1 = "SELECT Count (TestKey) AS t FROM RunningExams WHERE StudentId=@studentid AND CourseId=@courseid";
                SqlCommand cmd1 = new SqlCommand (sql1, cnn);
                cmd1.Parameters.AddWithValue ("@studentid", studentCourse.StudentId);
                cmd1.Parameters.AddWithValue ("@courseid", studentCourse.CourseId);
                int n = (int) cmd1.ExecuteScalar ();
                //count nCorrects
                string sql2 = "SELECT Count (TestKey) AS t FROM RunningExams WHERE StudentId=@studentid AND CourseId=@courseid AND TestKey=UserAns";
                SqlCommand cmd2 = new SqlCommand (sql2, cnn);
                cmd2.Parameters.AddWithValue ("@studentid", studentCourse.StudentId);
                cmd2.Parameters.AddWithValue ("@courseid", studentCourse.CourseId);
                int c = (int) cmd2.ExecuteScalar ();
                //Save n,c
                string sql3 = "UPDATE StudentCourses SET NumberOfTests=@numberoftests, CorrectAnswers-@correctanswers WHERE StudentId=@studentid AND CourseId=@courseid";
                var cmd3 = new SqlCommand (sql3, cnn);
                cmd3.Parameters.AddWithValue ("@numberoftests", n);
                cmd3.Parameters.AddWithValue ("@correctanswers", c);
                cmd3.Parameters.AddWithValue ("@studentid", studentCourse.StudentId);
                cmd3.Parameters.AddWithValue ("@courseid", studentCourse.CourseId);
                await cmd3.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in ReCalcRunningPoint:\n" + ex.ToString ());
                return false;
                }
            }
        #endregion
        #region C14:StudentCourseTests
        public async Task<bool> Create_StudentCourseTestAsync (StudentCourseTest studentCourseTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "INSERT INTO StudentExamTests (UserAns, [DateTime], StudentCourseId, TestId, TestKey) VALUES (@userans, @datetime, @studentcourseid, @testid, @testkey)";
            await cnn.OpenAsync ();
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@userans", studentCourseTest.UserAns.ToString ());
            cmd.Parameters.AddWithValue ("@datetime", studentCourseTest.DateTime);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId.ToString ());
            cmd.Parameters.AddWithValue ("@testid", studentCourseTest.TestId.ToString ());
            cmd.Parameters.AddWithValue ("@testkey", studentCourseTest.TestKey.ToString ());
            await cmd.ExecuteNonQueryAsync ();
            //update counts
            sql = "UPDATE StudentCourses SET ";
            sql += (studentCourseTest.UserAns == studentCourseTest.TestKey) ? "NumberOfTests = NumberOfTests +1 ,CorrectAnswers = CorrectAnswers +1 " : "NumberOfTests = NumberOfTests +1 ";
            sql += "WHERE StudentCourseId=@studentcourseid";
            var cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId.ToString ());
            await cmd2.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<List<StudentCourseTest>> Read_StudentCourseTestsAsync (StudentCourse studentCourse, bool readOptions)
            {
            List<StudentCourseTest> lstStudentCourseTests = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT sct.StudentCourseId, sct.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            sct.TestKey, sct.UserAns, sct.DateTime
            FROM StudentCourseTests sct
            INNER JOIN Tests t ON sct.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE sct.StudentCourseId=@studentcourseid";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourse.StudentCourseId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        var sct = new StudentCourseTest
                            {
                            StudentCourseId = reader.GetInt32 (0),
                            TestId = reader.GetInt32 (1),
                            TestTitle = reader.GetString (2),
                            TestType = reader.GetInt32 (3),
                            CourseTopicId = reader.GetInt32 (4),
                            CourseTopicTitle = reader.GetString (5),
                            TestTags = reader.GetInt32 (6),
                            TestLevel = reader.GetInt32 (7),
                            TestKey = reader.GetInt32 (8),
                            UserAns = reader.GetInt32 (9),
                            DateTime = reader.GetString (10),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            };
                        lstStudentCourseTests.Add (sct);
                        }
                    }
                if (readOptions)
                    {
                    foreach (StudentCourseTest t in lstStudentCourseTests)
                        {
                        t.TestOptions = await Read_TestOptionsAsync (t.TestId, cnn);
                        }
                    }
                return lstStudentCourseTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentCourseTest> ();
                }
            }
        public async Task<StudentCourseTest> Read_StudentCourseTestRandomAsync (int studentCourseId, bool readOptions, bool retry)
            {
            StudentCourseTest studentCourseTest = new StudentCourseTest ();
            string sql = @"SELECT TOP (1) sc.StudentCourseId, t.TestId, t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel
                         FROM Tests t
                         INNER JOIN Courses c ON t.CourseId = c.CourseId 
                         INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId
                         INNER JOIN StudentCourses sc ON sc.CourseId = c.CourseId AND sc.StudentCourseId = @studentcourseid";
            if (retry)
                {
                sql += " WHERE t.TestId IN (SELECT sct.TestId FROM StudentCourseTests sct WHERE sct.StudentCourseId=@studentcourseid AND sct.UserAns <> sct.TestKey) ORDER BY NEWID()";
                }
            else
                {
                sql += " ORDER BY NEWID()";
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        studentCourseTest.StudentCourseId = reader.GetInt32 (0);
                        studentCourseTest.TestId = reader.GetInt32 (1);
                        studentCourseTest.TestTitle = reader.GetString (2);
                        studentCourseTest.TestType = reader.GetInt32 (3);
                        studentCourseTest.CourseTopicId = reader.GetInt32 (4);
                        studentCourseTest.CourseTopicTitle = reader.GetString (5);
                        studentCourseTest.TestTags = reader.GetInt32 (6);
                        studentCourseTest.TestLevel = reader.GetInt32 (7);
                        studentCourseTest.TestIndex = ++index;
                        studentCourseTest.TestOptions = new List<TestOption> ();
                        }
                    }
                if (readOptions)
                    {
                    studentCourseTest.TestOptions = await Read_TestOptionsAsync (studentCourseTest.TestId, cnn);
                    }
                return studentCourseTest;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new StudentCourseTest ();
                }
            }
        public async Task<bool> Update_StudentCourseTestAsync (StudentCourseTest studentCourseTest, string mode)
            {
            Console.WriteLine ($"be ........................ mode= {mode}");
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            if (mode == "new")
                {
                sql = "INSERT INTO StudentCourseTests (StudentCourseId, TestId, TestKey, UserAns, [DateTime]) VALUES (@studentcourseid, @testid, @testkey, @userans, @datetime)";
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                cmd.Parameters.AddWithValue ("@testid", studentCourseTest.TestId);
                cmd.Parameters.AddWithValue ("@testkey", studentCourseTest.TestKey);
                cmd.Parameters.AddWithValue ("@userans", studentCourseTest.UserAns);
                cmd.Parameters.AddWithValue ("@datetime", studentCourseTest.DateTime);
                await cmd.ExecuteNonQueryAsync ();
                sql = (studentCourseTest.UserAns == studentCourseTest.TestKey) ? "UPDATE StudentCourses SET CorrectAnswers=(CorrectAnswers + 1), NumberOfTests=(NumberOfTests + 1)  WHERE StudentCourseId=@studentcourseid" : "UPDATE StudentCourses SET NumberOfTests=(NumberOfTests + 1) WHERE StudentCourseId=@studentcourseid";
                var cmd2 = new SqlCommand (sql, cnn);
                cmd2.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                await cmd2.ExecuteNonQueryAsync ();
                return true;
                }
            else if (mode == "edit")
                {
                sql = "UPDATE StudentCourseTests SET TestKey=@testkey, UserAns=@userans, [DateTime]=@datetime WHERE StudentCourseId=@studentcourseid AND TestId=@testid";
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@testkey", studentCourseTest.TestKey);
                cmd.Parameters.AddWithValue ("@userans", studentCourseTest.UserAns);
                cmd.Parameters.AddWithValue ("@datetime", studentCourseTest.DateTime);
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                cmd.Parameters.AddWithValue ("@testid", studentCourseTest.TestId);
                await cmd.ExecuteNonQueryAsync ();
                sql = (studentCourseTest.UserAns == studentCourseTest.TestKey) ? "UPDATE StudentCourses SET CorrectAnswers=(CorrectAnswers + 1) WHERE StudentCourseId=@studentcourseid" : "";
                Console.WriteLine ($"be..............  \n TestId={studentCourseTest.TestId} \n studentCourseId={studentCourseTest.StudentCourseId} \n Key={studentCourseTest.TestKey} \n Ans={studentCourseTest.UserAns} \n sql={sql}");
                if (sql != "")
                    {
                    var cmd2 = new SqlCommand (sql, cnn);
                    cmd2.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                    await cmd2.ExecuteNonQueryAsync ();
                    }
                return true;
                }
            else
                {
                return false;
                }
            //update counts
            }
        public async Task<bool> Delete_StudentCourseTestsAsync (string mode, StudentCourse studentCourse)
            {
            string sql = "";
            switch (mode)
                {
                case "filter":
                        {
                        sql = @"DELETE FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid AND UserAns=TestKey; 
                                UPDATE StudentCourses 
                                SET NumberOfTests=(SELECT COUNT(TestId) FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid), 
                                CorrectAnswers=(SELECT COUNT(TestId) FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid AND TestKey=UserAns) 
                                WHERE StudentCourseId=@studentcourseid";
                        break;
                        }
                case "wipeout":
                        {
                        sql = @"DELETE FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid; 
                                UPDATE StudentCourses SET NumberOfTests=0, CorrectAnswers=0 WHERE StudentCourseId=@studentcourseid;";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            SqlCommand cmd = new SqlCommand (sql, cnn);
            try
                {
                await cnn.OpenAsync ();
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourse.StudentCourseId);
                cmd.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ($"ERROR in: BeServ-C14_Delete_StudentCourseTestsAsync \n --- mode: {mode}\n courseId:{studentCourse.CourseId}\n student:{studentCourse.StudentId}\n" + ex);
                return false;
                }
            }
        #endregion
        #region C16:Messages
        public async Task<int> Create_MessageAsync (int groupId, Message message)
            {
            List<int> lstStudentIds = new List<int> ();
            //Read StudentIds
            string sql1 = "SELECT StudentId FROM Students WHERE GroupId=@groupid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd1 = new SqlCommand (sql1, cnn);
            cmd1.Parameters.AddWithValue ("@groupid", groupId);
            SqlDataReader reader = await cmd1.ExecuteReaderAsync ();
            lstStudentIds.Clear ();
            while (await reader.ReadAsync ())
                {
                lstStudentIds.Add (reader.GetInt32 (0));
                }
            await cnn.CloseAsync ();
            //Create
            await cnn.OpenAsync ();
            string sql2 = "INSERT INTO Messages (FromId, ToId, DateTimeSent, DateTimeRead, MessageText, MessageTags) VALUES (@fromid, @toid, @datetimesent, @datetimeread, @messagetext, @messagetags)";
            foreach (int studentId in lstStudentIds)
                {
                SqlCommand cmd2 = new SqlCommand (sql2, cnn);
                cmd2.Parameters.AddWithValue ("@fromid", message.FromId);
                cmd2.Parameters.AddWithValue ("@toid", studentId);
                cmd2.Parameters.AddWithValue ("@datetimesent", message.DateTimeSent);
                cmd2.Parameters.AddWithValue ("@datetimeread", message.DateTimeRead);
                cmd2.Parameters.AddWithValue ("@messagetext", message.MessageText);
                cmd2.Parameters.AddWithValue ("@messagetags", message.MessageTags);
                await cmd2.ExecuteNonQueryAsync ();
                }
            await cnn.CloseAsync ();
            return 1;
            }
        public async Task<List<Message>> Read_MessagesAsync (string mode, int Id)
            {
            List<Message> lstMessages = new ();
            string sql = "SELECT MessageId, FromId, ToId, DateTimeSent, DateTimeRead, MessageText, MessageTags FROM Messages";
            switch (mode)
                {
                case "User":
                        {
                        sql += " WHERE FromId=@Id ORDER BY DateTimeSent";
                        break;
                        }
                case "Group":
                        {
                        sql += " WHERE ToId IN (SELECT StudentId From Students WHERE GroupId=@Id) ORDER BY DateTimeSent";
                        break;
                        }
                case "Student":
                        {
                        sql += " WHERE ToId=@Id AND (MessageTags & 4) = 0 ORDER BY DateTimeSent";
                        break;
                        }
                case "StudentWithDels":
                        {
                        sql += " WHERE ToId=@Id ORDER BY DateTimeSent";
                        break;
                        }
                case "Message":
                        {
                        sql += " WHERE MessageId=@Id ORDER BY DateTimeSent";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@Id", Id);
                SqlDataReader reader = cmd.ExecuteReader ();
                lstMessages.Clear ();
                while (await reader.ReadAsync ())
                    {
                    lstMessages.Add (new Message
                        {
                        MessageId = reader.GetInt32 (0),
                        FromId = reader.GetInt32 (1),
                        ToId = reader.GetInt32 (2),
                        DateTimeSent = reader.GetString (3),
                        DateTimeRead = reader.GetString (4),
                        MessageText = reader.GetString (5),
                        MessageTags = reader.GetInt32 (6)
                        });
                    }
                await cnn.CloseAsync ();
                return lstMessages;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new List<Message> ();
                }
            }
        public async Task<List<Message>> Read_MessagesAsync (int userId, string mode, string key)
            {
            //mode: Search, Date, DateTime
            List<Message> lstMessages = new ();
            string strKey = "";
            string sql = "SELECT MessageId, FromId, ToId, DateTimeSent, DateTimeRead, MessageText, MessageTags FROM Messages";
            switch (mode)
                {
                case "Search":
                        {
                        strKey = $"%{key}%";
                        sql += " WHERE MessageText LIKE @strkey AND ToId=@userId ORDER BY DateTimeSent";
                        break;
                        }
                case "Date":
                        {
                        strKey = $"%{key}%";
                        sql += " WHERE DateTimeSent LIKE @strkey AND ToId=@userId ORDER BY DateTimeSent";
                        break;
                        }
                case "DateTime":
                        {
                        strKey = key;
                        sql += " WHERE DateTimeSent=@strkey AND ToId=@userId ORDER BY DateTimeSent";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@strkey", strKey);
                cmd.Parameters.AddWithValue ("@userId", userId);
                SqlDataReader reader = cmd.ExecuteReader ();
                lstMessages.Clear ();
                while (await reader.ReadAsync ())
                    {
                    lstMessages.Add (new Message
                        {
                        MessageId = reader.GetInt32 (0),
                        FromId = reader.GetInt32 (1),
                        ToId = reader.GetInt32 (2),
                        DateTimeSent = reader.GetString (3),
                        DateTimeRead = reader.GetString (4),
                        MessageText = reader.GetString (5),
                        MessageTags = reader.GetInt32 (6)
                        });
                    }
                await cnn.CloseAsync ();
                return lstMessages;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new List<Message> ();
                }
            }
        public async Task<List<Message>> Read_MessagesAsync (Message message)
            {
            //mode: Search, Date, DateTime
            List<Message> lstMessages = new ();
            string strKey = "";
            string sql = "SELECT MessageId, FromId, ToId, DateTimeSent, DateTimeRead, MessageText, MessageTags FROM Messages";
            sql += " WHERE FromId=@fromid AND DateTimeSent=@datetimesent";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@fromid", message.FromId);
                cmd.Parameters.AddWithValue ("@datetimesent", message.DateTimeSent);
                SqlDataReader reader = cmd.ExecuteReader ();
                lstMessages.Clear ();
                while (await reader.ReadAsync ())
                    {
                    lstMessages.Add (new Message
                        {
                        MessageId = reader.GetInt32 (0),
                        FromId = reader.GetInt32 (1),
                        ToId = reader.GetInt32 (2),
                        DateTimeSent = reader.GetString (3),
                        DateTimeRead = reader.GetString (4),
                        MessageText = reader.GetString (5),
                        MessageTags = reader.GetInt32 (6)
                        });
                    }
                await cnn.CloseAsync ();
                return lstMessages;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new List<Message> ();
                }
            }
        public async Task<bool> Update_MessageAsync (Message message)
            {
            string sql = "UPDATE Messages SET FromId=@fromid, ToId=@toid, DateTimeSent=@datetimesent, DateTimeRead=@datetimeread, MessageText=@messagetext, MessageTags=@messagetags WHERE MessageId=@messageid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@fromid", message.FromId);
            cmd.Parameters.AddWithValue ("@toid", message.ToId);
            cmd.Parameters.AddWithValue ("@datetimesent", message.DateTimeSent);
            cmd.Parameters.AddWithValue ("@datetimeread", message.DateTimeRead);
            cmd.Parameters.AddWithValue ("@messagetext", message.MessageText);
            cmd.Parameters.AddWithValue ("@messagetags", message.MessageTags);
            cmd.Parameters.AddWithValue ("@messageid", message.MessageId);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_MessagesByIdAsync (string mode, int Id)
            {
            string sql = "";
            switch (mode)
                {
                case "Group":
                        {
                        sql = @"DELETE FROM Messages WHERE ToId IN (SELECT StudentId FROM Students WHERE GroupId=@id)";
                        break;
                        }
                case "Student":
                        {
                        sql = @"DELETE FROM Messages WHERE ToId=@id";
                        break;
                        }
                case "Message":
                        {
                        sql = @"DELETE FROM Messages WHERE MessageId=@id";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@id", Id);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return (i > 0) ? true : false;
            }
        public async Task<bool> Delete_MessagesByDateTimeAsync (int userId, Message message)
            {
            string sql =  @"DELETE FROM Messages WHERE FromId=@fromid AND DateTimeSent=@datetimesent";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@fromid", message.FromId);
            cmd.Parameters.AddWithValue ("@datetimesent", message.DateTimeSent);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return (i > 0) ? true : false;
            }
        #endregion
        }
    }
