using Microsoft.AspNetCore.Mvc;
using System.Linq;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celObject = _context.CelestialObjects.FirstOrDefault(e => e.Id == id);
            if (celObject == null) return NotFound();

            celObject.Satellites = _context.CelestialObjects.Where(e => e.Id == id).ToList();
            return Ok(celObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celObject = _context.CelestialObjects.Where(e => e.Name == name);
            if (!celObject.Any()) return NotFound();

            foreach (var celestialObject in celObject)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celObject);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var celObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celObjects);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celObject = _context.CelestialObjects.FirstOrDefault(e => e.Id == id);

            if (celObject == null) return NotFound();

            celObject.Name = celestialObject.Name;
            celObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celObject);
            _context.SaveChanges();

            return NoContent();
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject (int id, string name)
        {
            var celObject = _context.CelestialObjects.FirstOrDefault(e => e.Id == id);

            if (celObject == null) return NotFound();

            celObject.Name = name;
            _context.Update(celObject);
            _context.SaveChanges();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id).ToList();

            if (!celObjects.Any()) return NotFound();

            _context.RemoveRange(celObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
