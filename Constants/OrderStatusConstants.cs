namespace FoodioAPI.Constants
{
    public static class OrderStatusConstants
    {
        public static readonly Guid REQUEST_PAYMENT = Guid.Parse("4342e57e-32e6-4060-9886-02903d45d301");
        public static readonly Guid PENDING = Guid.Parse("1daed2f8-492b-4cf7-937c-b3239f23fcfe");
        public static readonly Guid CONFIRMED = Guid.Parse("0c334fab-07e1-4c5e-bca0-7d2fefe4e0bd");
        public static readonly Guid PAID = Guid.Parse("d68921ff-4aa1-461f-b089-754b39308211");
        public static readonly Guid CANCELLED = Guid.Parse("48c3bd74-4d47-4c33-83ad-62046c9ff4ba");
        public static readonly Guid DELIVERING = Guid.Parse("53f7346a-7d27-4afc-aa12-0f3377be6e83");
        public static readonly Guid DELIVERED = Guid.Parse("96267d4f-78bb-4f78-acb2-2ca07ba9313c");
    }
}

