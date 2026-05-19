namespace LSH.Core
{
    public interface IBootable
    {
        void Init();
    }

    public interface IBootableWithContext
    {
        void Init(ICoreBootstrapContext context);
    }
}
