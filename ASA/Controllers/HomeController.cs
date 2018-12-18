using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModel;

namespace Vidly.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;


            return View();

        }

        [HttpPost]
        public ActionResult Login(TempUser st)
        {

            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                if (st.UserType == "Student")
                {
                    var user = _context.StudentModels.Where(u => u.Email == st.Email && u.Password == st.Password).FirstOrDefault();

                    if (user != null)
                    {
                        Session["Id"] = user.Id.ToString();
                        Session["FirstName"] = user.FirstName.ToString();
                        Session["Year"] = user.Year.ToString();
                        Session["Semester"] = user.Semester.ToString();
                        return RedirectToAction("StudentCourse", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email OR Password is not correct!");
                        return RedirectToAction("Login", "Home");
                    }
                }
                else if (st.UserType == "Teacher")
                {

                    
                        var user = _context.TeacherTable.Where(u => u.Email == st.Email && u.Password == st.Password).FirstOrDefault();


                        if (user != null)
                        {
                            Session["Id"] = user.Id.ToString();
                            Session["FirstName"] = user.Name.ToString();
                            return RedirectToAction("TeacherCourses", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email OR Password is not correct!");
                            return RedirectToAction("Login", "Home");
                        }
                    }
                


                    return RedirectToAction("Login", "Home");

                }
            }

            public ActionResult Register()
            {
                return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(StudentModel model)
        {


            _context.StudentModels.Add(model);
            _context.SaveChanges();


            return RedirectToAction("Login", "Home");
        }

        public ActionResult TeacherCourses()
        {
            /*var course = _context.CourseModels.SqlQuery("select * from CourseModels where Id = 2").ToList();
            var teacher = _context.TeacherTable.Include(c => c.Id).ToList();
            var teachercourse = new TeacherCourse
            {
                course = course,
                teacher = teacher

            };*/
            var course = new TeacherCourse();
            course.course = _context.CourseModels.SqlQuery("select * from CourseModels where Id = 2").ToList();
            course.teacher = _context.TeacherTable.FirstOrDefault();
            return View(course);
        }

        public ActionResult Details(int id)
        {
            var resource = new ResourceViewModel();
            TempData["id"] = id;
            
            resource.resource = _context.ResourceTable.SqlQuery("select * from ResourceModels where courseId = "+id).ToList();
            return View(resource);
        }

        [HttpPost]
        public ActionResult upload(HttpPostedFileBase file)
        {
            var m = TempData["id"];
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Content/File/"),
                    Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = file.FileName + " stored in directory " + path;
                    var model = new ResourceModel
                    {
                        FileName = Path.GetFileName(file.FileName),
                        FilePath = path,
                        courseId = (int)m
                    };
                    _context.ResourceTable.Add(model);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }

        public ActionResult StudentCourse()
        {
           
            var course = new StudentCourseViewmodel();
            course.course = _context.CourseModels.SqlQuery("select * from CourseModels where Year =3 AND Semester = 2").ToList();
            course.student = _context.StudentModels.FirstOrDefault();
            return View(course);
        }


        public ActionResult Detailsdownload(int id)
        {
            var resource = new ResourceViewModel();
           // TempData["id"] = id;

            resource.resource = _context.ResourceTable.SqlQuery("select * from ResourceModels where courseId = " + id).ToList();
            return View(resource);
        }
        public ActionResult ByCourseId(int id)
        {
            return Content("/"+id);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult User()
        {
            return View();
        }


        public ActionResult upload()
        {
            return View();
        }




        public ActionResult Book()
        {
            return View();
        }

        public ActionResult qaforum()
        {

            return View();
        }

        [HttpGet]
        public FileResult downloadFile(string path, string name)
        {
            return new FilePathResult(path, "application") { FileDownloadName = name };
            //application/vnd.openxmlformats-officedocument.presentationml.presentation
            //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
        }

        public ActionResult Post()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Post(PostModel model, int id)
        {
            model.studentId = id;
            _context.PostModels.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Post", "Home");
        }

        public ActionResult ShowPost()
        {
            var post = new StudentPostViewModel();
            //post.student = _context.StudentModels.SqlQuery("select * from StudentModels inner join PostModels on PostModels.studentId = StudentModels.Id").ToList();
            post.post = _context.PostModels.ToList();
            return View(post);
        }

    }
}
