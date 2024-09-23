namespace front11.Models
{
    public class CourseViewModel
    {
        public List<Course> Courses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<string> SelectedCourseIds { get; set; } // 已选课程ID列表
    }
}