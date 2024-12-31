using System;

namespace _Di
{
    /// <summary>
    /// Class that represents the registration entity
    /// </summary>
    public class Registration
    {
        public Func<object> Factory { get; set; }
        public object Instance { get; set; }
        public bool IsSingleton { get; set; }
        
        public string Tag { get; set; }
        
    }

    public static class RegistrationExtensions
    {
        public static void WithTag(this Registration registration, string tag)
        {
            registration.Tag = tag;
        }
    }
}