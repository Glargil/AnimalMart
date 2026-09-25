namespace AnimalMart.Interfaces
{
    public interface IUserRepo
    {
        User CreateUser(User user);
        User GetUser(int userId);
        User UpdateUser(User user);
        void DeleteUser(int userId);
        List<User> GetAllUsers();
    
        /// <summary>
        /// Henter en bruger basseret paa E-mail.
        ///</summary>
        ///<param name="email">Brugerens e-mailadresse. </param>
        ///<returns>Brugerobjekt eller null, hvis brugeren ikke findes.</returns>
        User? GetUserByEmail(string email);

    }
}
