namespace Develop.Scripts.Services.Abstractions
{
    public interface ILoader<T>
    {
        public T Load(string path);
    }
}
