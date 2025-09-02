using System;

namespace Gamora_Indumentaria
{
    public static class AppEvents
    {
        /// <summary>
        /// Evento disparado cuando se agrega una nueva categoría. Pasa el nuevo CategoryId.
        /// </summary>
        public static event Action<int> CategoryAdded;

        public static void RaiseCategoryAdded(int categoryId)
        {
            try { CategoryAdded?.Invoke(categoryId); } catch { }
        }
    }
}
