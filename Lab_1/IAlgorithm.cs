namespace Lab_1.ArrayAlgorithms
{
    public interface IAlgorithm<InputType>
    {
        Task Execute(InputType data);
    }
}