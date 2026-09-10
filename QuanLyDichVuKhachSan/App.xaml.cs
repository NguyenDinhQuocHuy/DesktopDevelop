using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace QuanLyDichVuKhachSan;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Chuẩn hóa định dạng ngày tháng tiếng Việt (dd/MM/yyyy) trên toàn bộ ứng dụng
        var culture = new System.Globalization.CultureInfo("vi-VN");
        culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        culture.DateTimeFormat.DateSeparator = "/";
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

        // Áp dụng định dạng văn hóa cho tất cả các điều khiển WPF (đặc biệt là DatePicker)
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                System.Windows.Markup.XmlLanguage.GetLanguage("vi-VN")));

        // Tự động gán Icon nhận diện khách sạn 5 sao cao cấp cho TẤT CẢ các cửa sổ khi mở lên
        try
        {
            var iconUri = new Uri("pack://application:,,,/Resources/app_icon.ico", UriKind.RelativeOrAbsolute);
            var appIcon = System.Windows.Media.Imaging.BitmapFrame.Create(iconUri);

            EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent, new RoutedEventHandler((sender, args) =>
            {
                if (sender is Window w && w.Icon == null)
                {
                    w.Icon = appIcon;
                }
            }));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App Icon Error] {ex.Message}");
        }

        // Bắt lỗi không xử lý trên luồng UI (Dispatcher)
        DispatcherUnhandledException += App_DispatcherUnhandledException;

        // Bắt lỗi trên các luồng ngầm (AppDomain)
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[Global UI Exception] {e.Exception}");
        MessageBox.Show($"⚠️ Đã xảy ra sự cố trong quá trình xử lý:\n{e.Exception.Message}\n\nỨng dụng vẫn tiếp tục hoạt động an toàn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Global Background Exception] {ex}");
        }
    }
}


