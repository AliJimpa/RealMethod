namespace RealMethod
{
    public interface ICSVDataTransfer
    {
        public void OnBeginTransfer(string CSVFileName);
        public void OnRowTransfer(string[] Cells, int RowIndex);
        public void OnColumnTransfer(string[] Cells, int ColumnIndex);
        public void OnEndTransfer(string CSVFileName);
    }
}