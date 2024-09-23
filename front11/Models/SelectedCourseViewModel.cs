using front11.Models;

public class SelectedCourseViewModel
{
    public List<Course> SelectedCourses { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public long TotalCredits { get; set; }
}
