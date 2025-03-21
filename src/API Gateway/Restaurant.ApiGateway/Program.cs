using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Restaurant.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load cấu hình Ocelot từ file JSON
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            // Đăng ký các dịch vụ cần thiết
            builder.Services.AddAuthorization();
            builder.Services.AddOcelot();

            var app = builder.Build();

            // Bật HTTPS nếu cần
            app.UseHttpsRedirection();

            // Bật Authorization (nếu cần)
            app.UseAuthorization();

            // Chạy Ocelot Middleware
            app.UseOcelot().Wait();

            app.Run();
        }
    }
}