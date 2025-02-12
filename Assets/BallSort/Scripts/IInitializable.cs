public interface IInitializable<T>
{
    bool Initialized { get; set; }
    void Init(T data);
}