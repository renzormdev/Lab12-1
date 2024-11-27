using lab12.Models;
using lab12.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab12.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly InvoiceContext _context;

        public StudentsController(InvoiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Student> GetAll()
        {
            return _context.Students.Where(s => s.Active).ToList(); 
        }

        [HttpGet("{id}")]
        public ActionResult<Student> GetById(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        [HttpGet]
        public List<StudentRequestV1> GetStudentsByNameEmail()
        {
            var students = _context.Students
                .Where(s => s.Active)
                .OrderByDescending(s => s.LastName)
                .Select(s => new StudentRequestV1
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email
                })
                .ToList();

            return students;
        }

        [HttpGet]
        public List<StudentRequestV2> GetStudentsByNameAndGrade()
        {
            var students = _context.Students
                .Where(s => s.Active) 
                .OrderByDescending(s => s.FirstName) 
                .Select(s => new StudentRequestV2
                {
                    FirstName = s.FirstName, 
                    GradeId = s.GradeId    
                })
                .ToList();

            return students;
        }





        [HttpPost]
        public ActionResult<Student> Create(Student student)
        {
            if (student == null)
            {
                return BadRequest("Estudiante inválido");
            }

            _context.Students.Add(student);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = student.StudentID }, student);
        }

        [HttpPost]
        public ActionResult<Student> CreateStudentRequestV3(StudentRequestV3 request)
        {
            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                GradeId = request.GradeId,
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = student.StudentID }, student);
        }

        [HttpPost]
        public IActionResult InsertStudentsByGrade(StudentRequestV4 request)
        {
            if (request.Students == null || request.Students.Count == 0)
            {
                return BadRequest("La lista de estudiantes está vacía.");
            }

            foreach (var student in request.Students)
            {
                student.GradeId = request.IdGrade;  // Asocia el IdGrade a cada estudiante
                _context.Students.Add(student);     // Inserta cada estudiante
            }

            _context.SaveChanges();  // Guarda los cambios en la base de datos
            return CreatedAtAction(nameof(GetById), new { id = request.Students.First().StudentID }, request.Students);
        }



        [HttpPut("{id}")]
        public IActionResult Update(int id, Student student)
        {
            if (id != student.StudentID)
            {
                return BadRequest("El ID del estudiante no coincide");
            }

            var existingStudent = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (existingStudent == null)
            {
                return NotFound("Estudiante no encontrado");
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Phone = student.Phone;
            existingStudent.Email = student.Email;
            existingStudent.GradeId = student.GradeId;

            _context.SaveChanges();

            return NoContent(); 
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePhoneEmail(int id, DatosRequestV1 request)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == id);
            if (student != null)
            {
                student.Phone = request.Phone;
                student.Email = request.Email;
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateName(int id, DatosRequestV2 request)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == id);
            if (student != null)
            {
                student.FirstName = request.FirstName;
                student.LastName = request.LastName;
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (student == null)
            {
                return NotFound("Estudiante no encontrado");
            }

            student.Active = false;
            _context.SaveChanges();

            return NoContent(); 
        }
    }
}
