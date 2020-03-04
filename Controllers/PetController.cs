using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TamagotchiAPI.Models;
using System.Text.Json;
using System;

namespace TamagotchiAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class PetController : ControllerBase
  {
    public DatabaseContext tamagotchiDb { get; set; } = new DatabaseContext();

    public bool IsDead()
    {
      var rnd = new Random();
      var intSelected = rnd.Next(10);

      if (intSelected == 1)
        return true;
      else
        return false;
    }

    public void CheckForDeadPets()
    {
      foreach (var pet in tamagotchiDb.Pets)
      {
        if (!pet.IsDead && (DateTime.Now.Subtract(pet.LastInteractedWithDate).Days > 3))
          pet.IsDead = true;
      }

      tamagotchiDb.SaveChanges();
    }

    //GET /api/pet, this should return all pets in your database.
    [HttpGet]
    public ActionResult GetAllPets()
    {
      CheckForDeadPets();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(tamagotchiDb.Pets),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    //GET /api/pet, this should return all pets in your database.
    [HttpGet("alive")]
    public ActionResult GetAllAlivePets()
    {
      CheckForDeadPets();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(tamagotchiDb.Pets.Where(p => p.IsDead == false)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    //GET /api/pet/{id}, This should return the pet with the corresponding Id.
    [HttpGet("{id}")]
    public ActionResult GetSinglePet(int id)
    {
      CheckForDeadPets();

      tamagotchiDb.Pets.FirstOrDefault(pet => pet.ID == id).LastInteractedWithDate = DateTime.Now;
      tamagotchiDb.SaveChanges();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(tamagotchiDb.Pets.FirstOrDefault(p => p.ID == id)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    //POST /api/pet, This should create a new pet. The body of the request should contain the name of the pet. 
    [HttpPost]
    public ActionResult AddNewPet(Pet petToAdd)
    {
      CheckForDeadPets();

      tamagotchiDb.Pets.Add(petToAdd);
      tamagotchiDb.SaveChanges();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(petToAdd),
        ContentType = "application/json",
        StatusCode = 201
      };
    }

    //PUT /api/pet/{id}/play, This should find the pet by id, and add 5 to its happiness level and add 3 to its hungry level
    [HttpPatch("play/{id}")]
    public ActionResult PlayWithPet(int id)
    {
      CheckForDeadPets();

      var petToPlayWith = tamagotchiDb.Pets.FirstOrDefault(p => p.ID == id);

      petToPlayWith.HappinessLevel += 5;
      petToPlayWith.HungerLevel += 3;
      petToPlayWith.IsDead = IsDead();
      petToPlayWith.LastInteractedWithDate = DateTime.Now;

      if (petToPlayWith.IsDead)
        petToPlayWith.DeathDate = DateTime.Now;

      tamagotchiDb.SaveChanges();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(petToPlayWith),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    //PUT /api/pet/{id}/feed, This should find the pet by id, and remove 5 from its hungry level and add 3 to its happiness level.
    [HttpPatch("feed/{id}")]
    public ActionResult FeedPet(int id)
    {
      CheckForDeadPets();

      var petToFeed = tamagotchiDb.Pets.FirstOrDefault(p => p.ID == id);

      petToFeed.HungerLevel -= 5;
      petToFeed.HappinessLevel += 3;
      petToFeed.IsDead = IsDead();
      petToFeed.LastInteractedWithDate = DateTime.Now;

      if (petToFeed.IsDead)
        petToFeed.DeathDate = DateTime.Now;

      tamagotchiDb.SaveChanges();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(petToFeed),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    //PUT /api/pet/{id}/scold, This should find the pet by id, and remove 5 from its happiness level
    [HttpPatch("scold/{id}")]
    public ActionResult ScoldPet(int id)
    {
      CheckForDeadPets();

      var petToScold = tamagotchiDb.Pets.FirstOrDefault(p => p.ID == id);

      petToScold.HappinessLevel -= 5;
      petToScold.IsDead = IsDead();
      petToScold.LastInteractedWithDate = DateTime.Now;

      if (petToScold.IsDead)
        petToScold.DeathDate = DateTime.Now;

      tamagotchiDb.SaveChanges();

      return new ContentResult()
      {
        Content = JsonSerializer.Serialize(petToScold),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    //DELETE /api/pet/{id}, this should delete a pet from the database by Id
    [HttpDelete("{id}")]
    public ActionResult DeletePet(int id)
    {
      CheckForDeadPets();

      var petToDelete = tamagotchiDb.Pets.FirstOrDefault(p => p.ID == id);

      tamagotchiDb.Pets.Remove(petToDelete);

      tamagotchiDb.SaveChanges();

      return Ok();
    }
  }
}