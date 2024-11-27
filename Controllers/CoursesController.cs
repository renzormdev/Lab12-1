﻿using lab12.Models;
using lab12.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lab12.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly InvoiceContext _context;

        public CoursesController(InvoiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Course> GetAll()
        {
            return _context.Courses.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Course> GetById(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseID == id);
            if (course == null)
            {
                return NotFound("Curso no encontrado.");
            }
            return course;
        }

        [HttpPost]
        public ActionResult<Course> Create(Course course)
        {
            if (course == null)
            {
                return BadRequest("Curso inválido.");
            }

            _context.Courses.Add(course);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = course.CourseID }, course);
        }

        [HttpPost]
        public ActionResult<Course> InsertByNameAndCredit(CourseRequestV1 request)
        {
            var course = new Course
            {
                Name = request.Name,
                Credit = request.Credit
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = course.CourseID }, course);
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, Course course)
        {
            if (id != course.CourseID)
            {
                return BadRequest("El ID del curso no coincide.");
            }

            var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseID == id);
            if (existingCourse == null)
            {
                return NotFound("Curso no encontrado.");
            }

            existingCourse.Name = course.Name;
            existingCourse.Credit = course.Credit;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseID == id);
            if (course == null)
            {
                return NotFound("Curso no encontrado.");
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteCourses([FromBody] CourseRequestV3 request)
        {
            foreach (var course in request.Courses)
            {
                var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseID == course.CourseID);
                if (existingCourse != null)
                {
                    _context.Courses.Remove(existingCourse);
                }
            }

            _context.SaveChanges();
            return NoContent();
        }


        [HttpPost]
        public IActionResult DeleteById(CourseRequestV2 request)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseID == request.CourseID);
            if (course == null)
            {
                return NotFound("Curso no encontrado.");
            }

            _context.Courses.Remove(course);
            _context.SaveChanges();

            return NoContent();
        }



    }
}
