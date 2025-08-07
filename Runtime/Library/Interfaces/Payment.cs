namespace RealMethod
{
    public interface IPayment
    {
        int GetCapital();
        void Disbursement(int amount);
        void AddFunds(int amount);
    }
    public interface IRefundable
    {
        float Refund(float percent); // return refunded amount
    }
}