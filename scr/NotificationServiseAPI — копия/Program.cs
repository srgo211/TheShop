namespace NotificationServiseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();


            var app = builder.Build();

           

            app.UseAuthorization();

           

            app.Run();
        }
    }
}
