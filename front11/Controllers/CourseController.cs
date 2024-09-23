using Microsoft.AspNetCore.Mvc;
using front11.Models;
using front11.Filters;
//using OfficeOpenXml;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing.Printing;


namespace front11.Controllers
{
	[AuthFilter]
	public class CourseController : BaseController
	{
		private readonly _2109060123DbContext _context;

		public CourseController(_2109060123DbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult AddCourse()
		{
			return View();
		}

		public IActionResult ShowAllCourse(int page = 1, int pageSize = 11)
		{
			//将用户名存入ViewData，以便在视图中显示
			ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");
			////查询课程列表
			//using (var db = new _2109060123DbContext())
			//{
			//	List<Course> courses = db.Courses.ToList();
			//	//返回课程列表视图，以便使用强类型展示
			//	return View(courses);
			//}
			var totalCourses = _context.Courses.Count();
			var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);
			var courses = _context.Courses
								  .OrderBy(c => c.Cid)  // 根据需要可以改变排序方式
								  .Skip((page - 1) * pageSize)
								  .Take(pageSize)
								  .ToList();

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;

			return View(courses);


		}
		public IActionResult courseEdit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View();
			}
			using (var db = new _2109060123DbContext())
			{
				Course course = db.Courses.Find(id);
				return View(course);
			}
		}
		public IActionResult SaveCourse(Course course)
		{
			using (var db = new _2109060123DbContext())
			{
				if (string.IsNullOrEmpty(course.Cid))
				{
					db.Courses.Add(course);
				}
				else
				{
					db.Courses.Update(course);
				}
				db.SaveChanges();
			}
			return RedirectToAction("ShowAllCourse");
		}

		[HttpPost]
		public IActionResult DeleteSelected([FromBody] List<string> selectedIds)
		{
			if (selectedIds != null && selectedIds.Count > 0)
			{
				// 检查是否有学生选课
				var coursesWithEnrollments = _context.SelectedCourses
					.Where(sc => selectedIds.Contains(sc.Cid))
					.Select(sc => sc.Cid)
					.Distinct()
					.ToList();

				if (coursesWithEnrollments.Count > 0)
				{
					var courseNames = _context.Courses
						.Where(c => coursesWithEnrollments.Contains(c.Cid))
						.Select(c => c.Cname)
						.ToList();

					return StatusCode(400, $"无法删除课程，因为已经有学生选择了以下课程：{string.Join(", ", courseNames)}");
				}

				var coursesToDelete = _context.Courses.Where(c => selectedIds.Contains(c.Cid)).ToList();

				try
				{
					_context.Courses.RemoveRange(coursesToDelete);
					_context.SaveChanges();
					return Ok("选中的课程已成功删除");
				}
				catch (DbUpdateException ex)
				{
					var innerException = ex.InnerException?.Message;
					return StatusCode(500, "删除课程时发生错误：" + innerException);
				}
			}
			return BadRequest("没有选中要删除的课程。");
		}


		[HttpGet]
		public IActionResult BatchAdd()
		{
			return View(); // 显示批量添加页面
		}

		[HttpPost]
		public IActionResult BatchAdd([FromBody] List<Course> courses)
		{
			if (courses == null || courses.Count == 0)
			{
				return Json(new { success = false, message = "没有接收到课程数据。" });
			}

			List<string> errors = new List<string>();

			foreach (var course in courses)
			{
				// 完整性检查
				if (string.IsNullOrWhiteSpace(course.Cid) ||
					string.IsNullOrWhiteSpace(course.Cname) ||
					string.IsNullOrWhiteSpace(course.Cscore) ||
					string.IsNullOrWhiteSpace(course.Cteacher) ||
					string.IsNullOrWhiteSpace(course.Csem) ||
					string.IsNullOrWhiteSpace(course.Ctime) ||
					string.IsNullOrWhiteSpace(course.Cclassroom))
				{
					errors.Add($"课程 {course.Cname} 的信息不完整。");
					continue;
				}

				// 唯一性检查
				if (_context.Courses.Any(c => c.Cid == course.Cid))
				{
					errors.Add($"课程代码 {course.Cid} 已经存在。");
					continue;
				}

				_context.Courses.Add(course);
			}

			if (errors.Any())
			{
				return Json(new { success = false, message = string.Join("\n", errors) });
			}

			_context.SaveChanges();
			return Json(new { success = true, message = "课程成功添加！" });
		}

		[HttpPost]
		public IActionResult ImportExcel(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return Json(new { success = false, message = "文件为空，请上传文件。" });

			var expectedHeaders = new[] { "Cid", "Cname", "Cscore", "Cteacher", "Csem", "Ctime", "Cclassroom" };
			var courses = new List<Course>();
			using (var stream = new MemoryStream())
			{
				file.CopyTo(stream);
				using (var workbook = new XLWorkbook(stream))
				{
					var worksheet = workbook.Worksheet(1);
					var headerRow = worksheet.FirstRowUsed();
					var headers = headerRow.Cells().Select(c => c.GetValue<string>()).ToArray();

					// 确保文件中的标题与预期匹配
					if (!expectedHeaders.All(eh => headers.Contains(eh)))
						return Json(new { success = false, message = "Excel文件的标题不符合要求，请确保包含正确的列标题。" });

					var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // 跳过标题行
					foreach (var row in rows)
					{
						var cid = row.Cell(1).GetValue<string>();
						if (string.IsNullOrWhiteSpace(cid) ||
							string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(3).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(4).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(5).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(6).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(7).GetValue<string>()))
						{
							return Json(new { success = false, message = "所有字段都需要填写，请确保Excel文件中没有遗漏任何信息。" });
						}

						if (_context.Courses.Any(c => c.Cid == cid))
						{
							return Json(new { success = false, message = $"课程ID {cid} 已存在。" });
						}


						courses.Add(new Course
						{
							Cid = cid,
							Cname = row.Cell(2).GetValue<string>(),
							Cscore = row.Cell(3).GetValue<string>(),
							Cteacher = row.Cell(4).GetValue<string>(),
							Csem = row.Cell(5).GetValue<string>(),
							Ctime = row.Cell(6).GetValue<string>(),
							Cclassroom = row.Cell(7).GetValue<string>(),
						});
					}
				}
			}

			_context.Courses.AddRange(courses);
			_context.SaveChanges();
			return Json(new { success = true, message = "课程成功导入。" });
		}
		public IActionResult BatchEdit(List<string> selectedIds)
		{
			var selectedCourses = _context.Courses.Where(c => selectedIds.Contains(c.Cid)).ToList();
			return View(selectedCourses);
		}
		[HttpPost]
		public IActionResult UpdateBatchCourses(List<Course> courses)
		{
			if (courses == null || !courses.Any())
			{
				return View("Error", new ErrorViewModel { RequestId = "No courses provided" });
			}

			_context.UpdateRange(courses);
			_context.SaveChanges();

			return RedirectToAction("ShowAllCourse");
		}
		[HttpGet]
		[HttpGet]
		public IActionResult SearchCourses(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return Json(new { success = false, message = "搜索条件不能为空。" });
			}

			var searchDict = new Dictionary<string, List<string>>();
			var parts = query.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var part in parts)
			{
				var keyValue = part.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if (keyValue.Length == 2)
				{
					var key = keyValue[0].Trim();
					var values = keyValue[1].Trim().Split(new[] { "**" }, StringSplitOptions.RemoveEmptyEntries);
					if (!searchDict.ContainsKey(key))
					{
						searchDict[key] = new List<string>();
					}
					searchDict[key].AddRange(values);
				}
			}

			IQueryable<Course> queryableCourses = _context.Courses;

			if (searchDict.Count > 0)
			{
				// 特定列搜索
				foreach (var kvp in searchDict)
				{
					switch (kvp.Key.ToLower())
					{
						case "course id":
							queryableCourses = queryableCourses.Where(c => kvp.Value.Contains(c.Cid));
							break;
						case "course name":
							queryableCourses = queryableCourses.Where(c => kvp.Value.Contains(c.Cname));
							break;
						// 添加更多列根据需要
						case "course score":
							queryableCourses = queryableCourses.Where(c => kvp.Value.Contains(c.Cscore));
							break;
						case "course teacher":
							queryableCourses = queryableCourses.Where(c => kvp.Value.Contains(c.Cteacher));
							break;
						case "course semester":
							queryableCourses = queryableCourses.Where(c => kvp.Value.Contains(c.Csem));
							break;
						case "course classroom":
							queryableCourses = queryableCourses.Where(c => kvp.Value.Contains(c.Cclassroom));
							break;
						default:
							return Json(new { success = false, message = $"不支持搜索列: {kvp.Key}" });
					}
				}
			}
			else
			{
				// 全局搜索
				queryableCourses = queryableCourses.Where(c =>
					c.Cid.Contains(query) ||
					c.Cname.Contains(query) ||
					c.Cscore.Contains(query) ||
					c.Cteacher.Contains(query) ||
					c.Csem.Contains(query) ||
					c.Ctime.Contains(query) ||
					c.Cclassroom.Contains(query));
			}

			var courses = queryableCourses.ToList(); // 执行查询

			if (courses.Any())
			{
				return Json(new { success = true, courses });
			}
			else
			{
				return Json(new { success = false, message = "未找到符合条件的课程。" });
			}
		}


		[HttpPost]
		public IActionResult Delete(string id)
		{
			var course = _context.Courses.FirstOrDefault(c => c.Cid == id);
			if (course == null)
			{
				return Json(new { success = false, message = "找不到课程。" });
			}

			try
			{
				_context.Courses.Remove(course);
				_context.SaveChanges();
				return Json(new { success = true, message = "课程删除成功。" });
			}
			catch (DbUpdateException ex)
			{
				return Json(new { success = false, message = "删除课程时出现错误：" + ex.InnerException?.Message });
			}
		}


	}
}
