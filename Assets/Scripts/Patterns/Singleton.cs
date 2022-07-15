public class Singleton<T>
{
    private static Singleton<T> instance = null;

    public static Singleton<T> GetInstance()
    {
        if (instance == null)
        {
            instance = new Singleton<T>();
        }
        return instance;
    }
}
