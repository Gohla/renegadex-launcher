using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RXL.WPFClient.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if(selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");

            var body = selectorExpression.Body as MemberExpression;
            if(body == null)
                throw new ArgumentException("The body must be a member expression");

            RaisePropertyChanged(body.Member.Name);
        }

        private void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
