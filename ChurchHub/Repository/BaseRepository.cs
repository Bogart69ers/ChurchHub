using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChurchHub.Utils;
using System.Data.Entity;

namespace ChurchHub.Repository
{
    // This class provides basic CRUD operations for entities of type T.
    // It implements the IBaseRepository interface.
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        // The database context instance.
        public DbContext _db;
        // The DbSet for the entity type T.
        public DbSet<T> _table;

        // Constructor initializes the database context and DbSet.
        public BaseRepository()
        {
            _db = new ChurchConnectEntities(); // Creating a new instance of the ChurchHubEntities DbContext.
            _table = _db.Set<T>(); // Setting up DbSet for type T.
        }

        // Method to create a new entity of type T.
        // Returns an ErrorCode indicating success or failure, along with an error message.
        public ErrorCode Create(T t, out string errorMsg)
        {
            try
            {
                _table.Add(t); // Adding the entity to the DbSet.
                _db.SaveChanges(); // Saving changes to the database.
                errorMsg = "Success"; // Setting success message.

                return ErrorCode.Success; // Returning success code.
            }
            catch (Exception ex)
            {
                // Handling exceptions and setting error message accordingly.
                errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ErrorCode.Error; // Returning error code.
            }
        }

        // Method to update an existing entity of type T.
        // Returns an ErrorCode indicating success or failure, along with an error message.
        public ErrorCode Update(object id, T t, out string errorMsg)
        {
            try
            {
                var old_obj = Get(id); // Getting the existing entity by its id.
                _db.Entry(old_obj).CurrentValues.SetValues(t); // Updating its values with the new values.
                _db.SaveChanges(); // Saving changes to the database.
                errorMsg = "Updated"; // Setting success message.

                return ErrorCode.Success; // Returning success code.
            }
            catch (Exception ex)
            {
                // Handling exceptions and setting error message accordingly.
                errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ErrorCode.Error; // Returning error code.
            }
        }

        // Method to delete an entity of type T by its id.
        // Returns an ErrorCode indicating success or failure, along with an error message.
        public ErrorCode Delete(object id, out string errorMsg)
        {
            try
            {
                var obj = Get(id); // Getting the entity by its id.
                _table.Remove(obj); // Removing the entity from the DbSet.
                _db.SaveChanges(); // Saving changes to the database.

                errorMsg = "Deleted"; // Setting success message.

                return ErrorCode.Success; // Returning success code.
            }
            catch (Exception ex)
            {
                // Handling exceptions and setting error message accordingly.
                errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ErrorCode.Error; 
            }
        }

        // Method to retrieve an entity of type T by its id.
        public T Get(object id)
        {
            return _table.Find(id); // Find the entity id
        }

        // Method to retrieve all entities of type T.
        public List<T> GetAll()
        { 
            // Returning all entities as a list.
            return _table.ToList(); 
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
