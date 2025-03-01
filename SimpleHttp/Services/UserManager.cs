﻿using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Services.Interfaces;


namespace SimpleHttp.Services
{

    public class UserManager : IUserManager
    {

        private readonly AppDbContext db;
        public UserManager(AppDbContext dbContext)
        {
            db = dbContext;
        }
        public async Task<List<User>> GetUsers()
        {
            try
            {

                var users = await db.Users.ToListAsync();
                if (users == null) return new List<User>();
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task UpdateUser(int userId, string? newEmail = null, string? newFirstname = null, string? newLastname = null, string? newphoneNumber = null, string? newPasswordHash = null, string? postalCode = null, string? country = null, string? city = null)
        {
            try
            {

                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return;

                if (!string.IsNullOrEmpty(newFirstname))
                    user.FirstName = newFirstname;

                if (!string.IsNullOrEmpty(newLastname))
                    user.LastName = newLastname;

                if (!string.IsNullOrEmpty(newphoneNumber))
                    user.PhoneNumber = newphoneNumber;
                if (!string.IsNullOrEmpty(newPasswordHash))
                    user.Password = newPasswordHash;

                if (!string.IsNullOrEmpty(newEmail))
                    user.Email = newEmail;

                if (!string.IsNullOrEmpty(country))
                    user.Country = country;

                if (!string.IsNullOrEmpty(city))
                    user.City = city;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<User?> CreateUser(User user)
        {

            try
            {


                var existningUser = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existningUser != null)
                {
                    Console.WriteLine("User already exists");
                    return null;
                }
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
                Console.WriteLine("User successfully created");

                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }
        public async Task AddAdmin(User user)
        {

            user.Role = "Admin";
            await db.AddAsync(user);
            await db.SaveChangesAsync();
        }
        public async Task RemoveAdmin(User user)
        {
            user.Role = "User";
            await db.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            try
            {


                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) throw new Exception("User not found");
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<User?> LogInUser(LogInModel user)
        {

            var userFromDb = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);


            if (userFromDb == null)
            {
                Console.WriteLine("\nUser not found, press Enter to continue ");
                Console.ReadLine();
                Console.Clear();
                return null;
            }
            if (userFromDb.Password != user.Password)
            {
                Console.WriteLine("\nInvalid password, press Enter to continue ");
                Console.ReadLine();
                Console.Clear();

                return null;
            }

            if (userFromDb.Role.ToLower() == "admin")
            {
                Console.WriteLine("Welcome Admin");

                return userFromDb;
            }
            else
            {
                Console.WriteLine("Welcome User");

                return userFromDb;
            }


        }

    }
}
