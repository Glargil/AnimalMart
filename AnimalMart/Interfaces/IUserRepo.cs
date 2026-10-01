namespace AnimalMart.Interfaces
{
    public interface IUserRepo
    {
        User Create(User user);
        User GetById(int userId);
        User Update(User user);
        void Delete(int userId);
        List<User> GetAll();
    
        /// <summary>
        /// Henter en bruger basseret paa E-mail.
        ///</summary>
        ///<param name="email">Brugerens e-mailadresse. </param>
        ///<returns>Brugerobjekt eller null, hvis brugeren ikke findes.</returns>
        User? GetByEmail(string email);

        //method for updating hashed password
        Task UpdatePasswordHashAsync(int userId, string passwordHash);

    }
}
