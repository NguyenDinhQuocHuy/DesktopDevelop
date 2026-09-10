using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuanLyDichVuKhachSan.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private readonly Dictionary<string, (DispatcherTimer Timer, Action Action)> _debouncers = new();

        /// <summary>
        /// Hoãn thực thi một hành động sau khoảng thời gian milliseconds (mặc định 180ms).
        /// Mỗi khi hàm được gọi lại với cùng key trước khi timer hết hạn, timer cũ sẽ được gia hạn và chỉ thực thi hành động mới nhất.
        /// Giúp loại bỏ hoàn toàn hiện tượng khựng/lag khi người dùng gõ phím liên tục vào các ô tìm kiếm real-time.
        /// </summary>
        protected void Debounce(string key, Action action, int milliseconds = 180)
        {
            if (_debouncers.TryGetValue(key, out var entry))
            {
                entry.Timer.Stop();
                _debouncers[key] = (entry.Timer, action);
                entry.Timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
                entry.Timer.Start();
            }
            else
            {
                var timer = new DispatcherTimer(DispatcherPriority.Input)
                {
                    Interval = TimeSpan.FromMilliseconds(milliseconds)
                };
                _debouncers[key] = (timer, action);
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    if (_debouncers.TryGetValue(key, out var current))
                    {
                        current.Action();
                    }
                };
                timer.Start();
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(), canExecute == null ? null : _ => canExecute())
        {
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
