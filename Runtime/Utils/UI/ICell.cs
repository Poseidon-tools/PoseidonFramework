namespace Utils.UI
{
    public interface ICell<U> where U : ICellData
    {
        void UpdateCell(U cellData);
    }
}
