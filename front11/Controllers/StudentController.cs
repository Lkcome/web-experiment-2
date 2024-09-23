using Microsoft.AspNetCore.Mvc;
using front11.Models;
using front11.Filters;
//using OfficeOpenXml;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;


namespace front11.Controllers
{
	[AuthFilter]
	public class StudentController : BaseController
	{
		private readonly _2109060123DbContext _context;

		public StudentController(_2109060123DbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult AddStudent()
		{
			return View();
		}

		public IActionResult ShowAllStudent(int page = 1, int pageSize = 11)
		{
			//将用户名存入ViewData，以便在视图中显示
			ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");
			//查询课程列表
			//using (var db = new _2109060123DbContext())
			//{
			//	List<Student> students = db.Students.ToList();
			//	//返回课程列表视图，以便使用强类型展示
			//	return View(students);
			//}
			var totalCourses = _context.Students.Count();
			var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);
			var courses = _context.Students
			.OrderBy(s => s.Sid)  // 根据需要可以改变排序方式
								  .Skip((page - 1) * pageSize)
								  .Take(pageSize)
								  .ToList();

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;

			return View(courses);

		}
		public IActionResult studentEdit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View();
			}
			using (var db = new _2109060123DbContext())
			{
				Student student = db.Students.Find(id);
				return View(student);
			}
		}
		public IActionResult SaveStudent(Student student)
		{
			using (var db = new _2109060123DbContext())
			{
				if (string.IsNullOrEmpty(student.Sid))
				{
					db.Students.Add(student);
				}
				else
				{
					db.Students.Update(student);
				}
				db.SaveChanges();
			}
			return RedirectToAction("ShowAllStudent");
		}

		[HttpPost]
		public IActionResult DeleteSelected([FromBody] List<string> selectedIds)
		{
			if (selectedIds != null && selectedIds.Count > 0)
			{
				// 获取并删除所有选中学生的选课记录
				var enrollmentsToDelete = _context.SelectedCourses.Where(sc => selectedIds.Contains(sc.Sid)).ToList();
				_context.SelectedCourses.RemoveRange(enrollmentsToDelete);

				// 获取并准备删除的学生列表
				var studentsToDelete = _context.Students.Where(s => selectedIds.Contains(s.Sid)).ToList();

				try
				{
					_context.SaveChanges(); // 先保存删除选课记录的更改
					_context.Students.RemoveRange(studentsToDelete); // 然后删除学生
					_context.SaveChanges();
					return Ok("选中的学生及其选课信息已成功删除");
				}
				catch (DbUpdateException ex)
				{
					var innerException = ex.InnerException?.Message;
					return StatusCode(500, "删除学生时发生错误：" + innerException);
				}
			}
			return BadRequest("没有选中要删除的学生。");
		}


		[HttpGet]
		public IActionResult BatchAdd()
		{
			return View(); // 显示批量添加页面
		}

		[HttpPost]
		public IActionResult BatchAdd([FromBody] List<Student> students)
		{
			if (students == null || students.Count == 0)
			{
				return Json(new { success = false, message = "没有接收到学生数据。" });
			}

			List<string> errors = new List<string>();

			foreach (var student in students)
			{
				// 完整性检查
				if (string.IsNullOrWhiteSpace(student.Sid) ||
					string.IsNullOrWhiteSpace(student.Sname) ||
					string.IsNullOrWhiteSpace(student.Sclass) ||
					string.IsNullOrWhiteSpace(student.Spassword))
				{
					errors.Add($"学生 {student.Sname} 的信息不完整。");
					continue;
				}

				// 唯一性检查
				if (_context.Students.Any(s => s.Sid == student.Sid))
				{
					errors.Add($"学生代码 {student.Sid} 已经存在。");
					continue;
				}

				_context.Students.Add(student);
			}

			if (errors.Any())
			{
				return Json(new { success = false, message = string.Join("\n", errors) });
			}

			_context.SaveChanges();
			return Json(new { success = true, message = "学生成功添加！" });
		}

		[HttpPost]
		public IActionResult ImportExcel(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return Json(new { success = false, message = "文件为空，请上传文件。" });

			var expectedHeaders = new[] { "Sid", "Sname", "Sclass", "Spassword" };
			var students = new List<Student>();
			using (var stream = new MemoryStream())
			{
				file.CopyTo(stream);
				using (var workbook = new XLWorkbook(stream))
				{
					var worksheet = workbook.Worksheet(1);
					var headerRow = worksheet.FirstRowUsed();
					var headers = headerRow.Cells().Select(s => s.GetValue<string>()).ToArray();

					// 确保文件中的标题与预期匹配
					if (!expectedHeaders.All(eh => headers.Contains(eh)))
						return Json(new { success = false, message = "Excel文件的标题不符合要求，请确保包含正确的列标题。" });

					var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // 跳过标题行
					foreach (var row in rows)
					{
						var sid = row.Cell(1).GetValue<string>();
						if (string.IsNullOrWhiteSpace(sid) ||
							string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(3).GetValue<string>()) ||
							string.IsNullOrWhiteSpace(row.Cell(4).GetValue<string>()) )
						{
							return Json(new { success = false, message = "所有字段都需要填写，请确保Excel文件中没有遗漏任何信息。" });
						}

						if (_context.Students.Any(s => s.Sid == sid))
						{
							return Json(new { success = false, message = $"学生ID {sid} 已存在。" });
						}


						students.Add(new Student
						{
							Sid = sid,
							Sname = row.Cell(2).GetValue<string>(),
							Sclass = row.Cell(3).GetValue<string>(),
							Spassword = row.Cell(4).GetValue<string>(),
						});
					}
				}
			}

			_context.Students.AddRange(students);
			_context.SaveChanges();
			return Json(new { success = true, message = "学生成功导入。" });
		}
		public IActionResult BatchEdit(List<string> selectedIds)
		{
			var selectedStudents = _context.Students.Where(s => selectedIds.Contains(s.Sid)).ToList();
			return View(selectedStudents);
		}
		[HttpPost]
		public IActionResult UpdateBatchStudents(List<Student> students)
		{
			if (students == null || !students.Any())
			{
				return View("Error", new ErrorViewModel { RequestId = "No students provided" });
			}

			_context.UpdateRange(students);
			_context.SaveChanges();

			return RedirectToAction("ShowAllStudent");
		}
		[HttpGet]
		[HttpGet]
		public IActionResult SearchStudents(string query)
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

			IQueryable<Student> queryableStudents = _context.Students;

			if (searchDict.Count > 0)
			{
				// 特定列搜索
				foreach (var kvp in searchDict)
				{
					switch (kvp.Key.ToLower())
					{
						case "student id":
							queryableStudents = queryableStudents.Where(s => kvp.Value.Contains(s.Sid));
							break;
						case "student name":
							queryableStudents = queryableStudents.Where(s => kvp.Value.Contains(s.Sname));
							break;
						// 添加更多列根据需要
						case "student score":
							queryableStudents = queryableStudents.Where(s => kvp.Value.Contains(s.Sclass));
							break;
						case "student password":
							queryableStudents = queryableStudents.Where(s => kvp.Value.Contains(s.Spassword));
							break;
						default:
							return Json(new { success = false, message = $"不支持搜索列: {kvp.Key}" });
					}
				}
			}
			else
			{
				// 全局搜索
				queryableStudents = queryableStudents.Where(s =>
					s.Sid.Contains(query) ||
					s.Sname.Contains(query) ||
					s.Sclass.Contains(query) ||
					s.Spassword.Contains(query));
			}

			var students = queryableStudents.ToList(); // 执行查询

			if (students.Any())
			{
				return Json(new { success = true, students });
			}
			else
			{
				return Json(new { success = false, message = "未找到符合条件的学生。" });
			}
		}


		[HttpPost]
		public IActionResult Delete(string id)
		{
			var student = _context.Students.FirstOrDefault(s => s.Sid == id);
			if (student == null)
			{
				return Json(new { success = false, message = "找不到学生。" });
			}

			// 获取所有该学生的选课记录并删除
			var enrollments = _context.SelectedCourses.Where(sc => sc.Sid == id).ToList();
			_context.SelectedCourses.RemoveRange(enrollments);

			try
			{
				_context.SaveChanges(); // 先保存删除选课记录的更改
				_context.Students.Remove(student); // 然后删除学生
				_context.SaveChanges();
				return Json(new { success = true, message = "学生及其选课信息删除成功。" });
			}
			catch (DbUpdateException ex)
			{
				return Json(new { success = false, message = "删除学生时出现错误：" + ex.InnerException?.Message });
			}
		}


	}
}
