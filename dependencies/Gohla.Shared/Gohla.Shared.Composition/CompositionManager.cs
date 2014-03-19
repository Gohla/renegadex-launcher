using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using Autofac;
using Autofac.Core;

namespace Gohla.Shared.Composition
{
    public static class CompositionManager
    {
        private static ComposablePartCatalog _catalog;
        private static bool _isConfigured;

        private static IContainer _container;

        public static void ConfigureDependencies(ContainerBuilder builder, ComposablePartCatalog catalog)
        {
            if(_isConfigured)
                return;

            _catalog = catalog;
            _container = builder.Build();

            _isConfigured = true;
        }

        public static T Get<T>(params Parameter[] parameters)
        {
            if(_container == null)
                return default(T);

            return _container.Resolve<T>(parameters);
        }

        public static IEnumerable<T> GetAll<T>()
        {
            if(_container == null)
                return new List<T>();

            return _container.Resolve<IEnumerable<T>>();
        }
    }
}