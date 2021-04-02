using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace _04_gui_client_wpf.Core
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        private Dictionary<string, object> store;

        protected NotifyPropertyChanged()
        {
            this.store = new Dictionary<string, object>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected T Get<T>([CallerMemberName] string name = "")
        {
            if (!this.store.TryGetValue(name, out var value))
            {
                value = default(T);
            }

            return (T)value;
        }

        protected void Set<T>(T value, [CallerMemberName] string name = "")
        {
            if (!this.store.ContainsKey(name))
            {
                this.store.Add(name, value);

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                return;
            }

            var oldValue = this.store[name];
            this.store[name] = value;

            if (!object.Equals(value, oldValue))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void EmitPropertiesChanged(params string[] properties)
            => this.EmitPropertiesChanged((IEnumerable<string>)properties);

        protected void EmitPropertiesChanged(IEnumerable<string> properties)
        {
            foreach (var prop in properties)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

    }
}
