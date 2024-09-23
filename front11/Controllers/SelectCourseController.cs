using Microsoft.AspNetCore.Mvc;
using front11.Models;
using front11.Filters;
//using OfficeOpenXml;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DocumentFormat.OpenXml.InkML;


namespace front11.Controllers
{
    [AuthFilter]
    public class SelectCourseController : BaseController
    {
        private readonly _2109060123DbContext _context;

        public SelectCourseController(_2109060123DbContext context)
        {
            _context = context;
        }
        public IActionResult SelectStart()
        {
            return View();
        }





        public IActionResult Select(string searchCourse = "", string searchSemester = "", string searchTeacher = "", int page = 1, int pageSize = 7)
        {
            var studentId = HttpContext.Session.GetString("Sid");
            var selectedCourseIds = _context.SelectedCourses
                .Where(sc => sc.Sid == studentId)
                .Select(sc => sc.Cid)
                .ToList();

            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchCourse))
            {
                query = query.Where(c => c.Cname.Contains(searchCourse));
                ViewBag.SearchCourse = searchCourse;
            }
            if (!string.IsNullOrWhiteSpace(searchSemester))
            {
                query = query.Where(c => c.Csem.Contains(searchSemester));
                ViewBag.SearchSemester = searchSemester;
            }
            if (!string.IsNullOrWhiteSpace(searchTeacher))
            {
                query = query.Where(c => c.Cteacher.Contains(searchTeacher));
                ViewBag.SearchTeacher = searchTeacher;
            }

            var totalCourses = query.Count();
            var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);

            var courses = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new CourseViewModel
            {
                Courses = courses,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedCourseIds = selectedCourseIds
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult SelectCourse(string courseId)
        {
            var studentId = HttpContext.Session.GetString("Sid"); // 假设学生ID存储在会话中

            if (string.IsNullOrEmpty(studentId))
            {
                return Json(new { success = false, message = "学生未登录或会话已过期。" });
            }

            var existingSelection = _context.SelectedCourses.FirstOrDefault(sc => sc.Cid == courseId && sc.Sid == studentId);
            bool isSelected;

            if (existingSelection == null)
            {
                var selectCourse = new SelectedCourse
                {
                    Cid = courseId,
                    Sid = studentId,
                    ScDate = DateTime.Now
                };

                _context.SelectedCourses.Add(selectCourse);
                isSelected = true;
            }
            else
            {
                _context.SelectedCourses.Remove(existingSelection);
                isSelected = false;
            }

            _context.SaveChanges();

            var selectedCourseIds = _context.SelectedCourses
                .Where(sc => sc.Sid == studentId)
                .Select(sc => sc.Cid)
                .ToList();

            var courses = _context.Courses
                .Where(c => selectedCourseIds.Contains(c.Cid))
                .ToList();

            var totalCredits = courses.Sum(c =>
            {
                long score;
                return long.TryParse(c.Cscore, out score) ? score : 0;
            });

            return Json(new
            {
                success = true,
                selected = isSelected,
                message = isSelected ? "选课成功。" : "退课成功。",
                totalCredits
            });
        }

        public IActionResult SelectedCourses(int page = 1, int pageSize = 7)
        {
            var studentId = HttpContext.Session.GetString("Sid");
            var selectedCourseIds = _context.SelectedCourses
                .Where(sc => sc.Sid == studentId)
                .Select(sc => sc.Cid)
                .ToList();

            var query = _context.Courses
                .Where(c => selectedCourseIds.Contains(c.Cid));

            var totalCourses = query.Count();
            var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);

            var courses = query
                .OrderBy(c => c.Cid)  // Or any other ordering if necessary
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCredits = courses.Sum(c =>
            {
                long score;
                return long.TryParse(c.Cscore, out score) ? score : 0;
            });

            var model = new SelectedCourseViewModel
            {
                SelectedCourses = courses,
                TotalCredits = totalCredits,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public IActionResult TimeManage()
        {
            return View();
        }


        // 获取当前的选课时间
        public IActionResult GetSelectionTime()
        {
            var settings = _context.Selectcoursetimes.FirstOrDefault();
            if (settings == null)
            {
                // 如果数据库中没有时间设置，返回失败信息
                return Json(new { success = false, message = "选课时间未设置。" });
            }
            return Json(new { success = true, startTime = settings.SelectionStartTime, endTime = settings.SelectionEndTime });
        }

        // 更新选课时间
        [HttpPost]
        public IActionResult SetSelectionTime([FromBody] Selectcoursetime model)
        {
            if (model.SelectionStartTime == null || model.SelectionEndTime == null)
            {
                return Json(new { success = false, message = "时间设置不能为空。" });
            }

            if (model.SelectionEndTime <= model.SelectionStartTime)
            {
                return Json(new { success = false, message = "结束时间必须晚于开始时间。" });
            }

            var settings = _context.Selectcoursetimes.FirstOrDefault();
            if (settings == null)
            {
                settings = new Selectcoursetime();
                _context.Selectcoursetimes.Add(settings);
            }

            settings.SelectionStartTime = model.SelectionStartTime.Value;
            settings.SelectionEndTime = model.SelectionEndTime.Value;

            _context.SaveChanges();
            return Json(new { success = true, message = "选课时间已更新。" });
        }




    }
}
