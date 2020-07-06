using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;

namespace Logic.AppServices
{
    /// <summary>
    /// Author  : Emmanuel Nuyttens
    /// Purpose : Returns a list of students which are registered in the database.
    ///           And allows to filter students for a particular course or a particular number of courses.
    ///           This query is part of the Read-Side of CQRS and does not need the domain model at all !
    ///           Here we do not need the DbContext and EF ORM (equivalent to SessionFactory in NHibernate) but rather direct connection to the database
    ///           using plain ADO.NET code, this to be able to manually create optimized (denormalized) read-queries.
    ///           This direct connection is represented by the QueriesConnectionString class.
    ///           
    /// 
    /// </summary>
    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn;
            NumberOfCourses = numberOfCourses;
        }

        internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
        {
            private readonly QueriesConnectionString _connectionString;

            public GetListQueryHandler(QueriesConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public List<StudentDto> Handle(GetListQuery query)
            {
                
                // enhanced query-method
                string sql = @"
                    SELECT s.StudentID Id, s.Name, s.Email,
	                    s.FirstCourseName Course1, s.FirstCourseCredits Course1Credits, s.FirstCourseGrade Course1Grade,
	                    s.SecondCourseName Course2, s.SecondCourseCredits Course2Credits, s.SecondCourseGrade Course2Grade
                    FROM dbo.Student s
                    WHERE (s.FirstCourseName = @Course
		                    OR s.SecondCourseName = @Course
		                    OR @Course IS NULL)
                        AND (s.NumberOfEnrollments = @Number
                            OR @Number IS NULL)
                    ORDER BY s.StudentID ASC";

                using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    List<StudentDto> students = connection
                        .Query<StudentDto>(sql, new
                        {
                            Course = query.EnrolledIn,
                            Number = query.NumberOfCourses
                        })
                        .ToList();

                    return students;
                }
            }
        }
    }
}
