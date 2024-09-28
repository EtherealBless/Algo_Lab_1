namespace Lab_1.ArrayAlgorithms
{
    public interface IAlgorithm<InputType> : IAlgorithm<InputType, Task> { }

    public interface IAlgorithm<InputType, ReturnType>
    {
        ReturnType Execute(InputType data);
    }
}