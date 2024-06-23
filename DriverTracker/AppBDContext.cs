using DriverTracker.Models;
using SQLite;

namespace DriverTracker;

public class AppBDContext
{
    private const String DatabaseFileName = "DriverTrackerDB";
    public SQLiteAsyncConnection Database;

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await Database.Table<User>().ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await Database.Table<User>().Where(p=> p.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<int> AddUserAsync(User newUser)
    {
        if (newUser.UserId != 0)
        {
            return await Database.UpdateAsync(newUser);
        }
        return await Database.InsertAsync(newUser);
    }
}