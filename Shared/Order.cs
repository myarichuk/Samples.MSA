using System;

namespace Shared
{
    public class Order
    {
        public string CustomerName;
        public BeverageType Type;
        public DateTime WhenReceived;
        public DateTime? WhenCompleted;
        public bool IsComplete => WhenCompleted.HasValue;

        public override string ToString() => 
            $"{nameof(CustomerName)}: {CustomerName}, {nameof(Type)}: {Type}, {nameof(WhenReceived)}: {WhenReceived}";
    }
}
